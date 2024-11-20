using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TGambleCardTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 성공 확률 </summary>
	public float m_SuccProp;
	/// <summary> 성공 실패 정보 </summary>
	public int[] m_ResultIdx = new int[2];
	/// <summary> 뽑힐 확률 </summary>
	public int m_Prop;
	/// <summary> 겜블 그룹 </summary>
	public int m_Gid;

	public TGambleCardTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_SuccProp = pResult.Get_Float();
		for (int i = 0; i < 2; i++) m_ResultIdx[i] = pResult.Get_Int32();
		m_Prop = pResult.Get_Int32();
		m_Gid = pResult.Get_Int32();
	}

	public string GetAllDesc() {
		return string.Format("{0} \n\n{1}", GetSuccFailDesc(true), GetSuccFailDesc(false));
	}
	public int GetSuccFailNumber(bool _succ) {
		if (_succ) return Mathf.RoundToInt(12 + (1 - m_SuccProp * 11));
		else return Mathf.RoundToInt(12 - m_SuccProp * 11);
	}
	public string GetSuccFailDesc(bool _succ) {
		if (_succ) {
			TStageCardTable succ = TDATA.GetStageCardTable(m_ResultIdx[0]);
			return string.Format(succ.GetOnlyInfo(), Mathf.Min(99, Mathf.FloorToInt(m_SuccProp * 100)), Mathf.RoundToInt(succ.m_Value1 * 100), Mathf.RoundToInt(succ.m_Type == StageCardType.Material ? succ.m_Value2 : succ.m_Value1));
		}
		else {
			TStageCardTable fail = TDATA.GetStageCardTable(m_ResultIdx[1]);
			return string.Format(fail.GetOnlyInfo(), Mathf.Max(0, Mathf.FloorToInt((m_SuccProp) * 100) - 1), Mathf.RoundToInt(fail.m_Value1 * 100), Mathf.RoundToInt(fail.m_Type == StageCardType.Material ? fail.m_Value2 : fail.m_Value1));
		}
	}
	/// <summary> 모드별 나올 수 있는 카드타입 체크 </summary>
	public bool CheckMode(StageModeType _mode) {
		bool canpick = false;
		for (int i = 0; i < 2; i++) {
			switch (_mode) {
				case StageModeType.Stage:
					switch (TDATA.GetStageCardTable(m_ResultIdx[i]).m_Type) {
						case StageCardType.EnergyUp:
						case StageCardType.AddGuard:
							return canpick = false;
						default:
							canpick = true;
							break;
					}
					break;
				case StageModeType.Tower:
				case StageModeType.NoteBattle:
					switch (TDATA.GetStageCardTable(m_ResultIdx[i]).m_Type) {
						case StageCardType.HpUp:
						case StageCardType.AtkUp:
						case StageCardType.DefUp:
						case StageCardType.EnergyUp:
						case StageCardType.AddGuard:
						case StageCardType.RecoveryHp:
						case StageCardType.RecoveryHpPer:
						case StageCardType.RecoveryHyg:
						case StageCardType.RecoveryMen:
						case StageCardType.RecoverySat:
						case StageCardType.PerRecoveryHyg:
						case StageCardType.PerRecoveryMen:
						case StageCardType.PerRecoverySat:
						case StageCardType.MenUp:
						case StageCardType.HygUp:
						case StageCardType.SatUp:
						case StageCardType.HealUp:
							canpick = true;
							break;
						default:
							return canpick = false;
					}
					break;
				case StageModeType.Training:
					return canpick = false;
				default:
					canpick = true;
					break;
			}
		}
		return canpick;
	}
}

public class TGambleCardTableMng : ToolFile
{
	public Dictionary<int, TGambleCardTable> DIC_Idx = new Dictionary<int, TGambleCardTable>();

	public TGambleCardTableMng() : base("Datas/GambleCardTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TGambleCardTable data = new TGambleCardTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// GambleCardTable
	TGambleCardTableMng m_GambleCard = new TGambleCardTableMng();

	public TGambleCardTable GetGambleCardTable(int _idx) {
		if (!m_GambleCard.DIC_Idx.ContainsKey(_idx)) return null;
		return m_GambleCard.DIC_Idx[_idx];
	}
	public TGambleCardTable GetRandGambleCardTable(StageModeType _modetype, int _gid = 0) {
		List<TGambleCardTable> canpicktables = new List<TGambleCardTable>(m_GambleCard.DIC_Idx.Values);
		canpicktables = canpicktables.FindAll(t => t.CheckMode(_modetype) && t.m_Gid == _gid);
		int allprop = canpicktables.Sum(t => t.m_Prop);
		int randprop = UTILE.Get_Random(0, allprop);
		int nowprop = 0;
		for(int i = 0;i< canpicktables.Count; i++) {
			int preprop = nowprop;
			nowprop += canpicktables[i].m_Prop;
			if (preprop <= randprop && randprop < nowprop) return canpicktables[i];
		}
		return null;
	}
}

