using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TIngameRewardTable : ClassMng
{
	public enum Condition
	{
		/// <summary> 걸러지지 않음 </summary>
		None,
		/// <summary> StageTable의 LimitTurn이 0인 스테이지에서 걸러짐 </summary>
		Turn,
		/// <summary> StageTable의 FailCondition이 Time이 아닌 스테이지에서 걸러짐 </summary>
		Time,
		/// <summary> StageTable의 StartMen이 -1인 스테이지에서 걸러짐 </summary>
		Men,
		/// <summary> StageTable의 StartHyg이 -1인 스테이지에서 걸러짐 </summary>
		Hyg,
		/// <summary> StageTable의 StartSat이 -1인 스테이지에서 걸러짐 </summary>
		Sat,
		/// <summary> StageTable의 DarkLevel이 0인 스테이지에서 걸러짐 </summary>
		Dark,
		/// <summary> StageCardTable의 CardType이 Fire인 카드가 없는 스테이지의 경우 걸러짐 </summary>
		Fire,
		/// <summary> StageTable의 Play타입에 FieldAirstrike이 없는 스테이지의 경우 걸러짐 </summary>
		AirStrike,
		/// <summary> StageMakingTable의 MaterialType이 ShockBomb이 오픈된 상태가 아니라면 걸러짐 </summary>
		Boom,
		/// <summary> StageMakingTable의 MaterialType이 MedBottle가 오픈된 상태가 아니라면 걸러짐 </summary>
		Heal,
		/// <summary> StageMakingTable의 MaterialType이 FireBomb가 오픈된 상태가 아니라면 걸러짐 </summary>
		Arson
		/// <summary>  </summary>
	}
	/// <summary> 그룹 인덱스 </summary>
	public int m_GroupId;
	/// <summary> 보상 레벨 </summary>
	public int m_Lv;
	/// <summary> 보상 값, 카드면 카드 인덱스, 버프는 스킬 인덱스 </summary>
	public int m_Val;
	/// <summary> 확률 </summary>
	public int m_Prob;
	/// <summary> 등장 시작 스테이지 </summary>
	public int m_StgIdx;
	/// <summary> 각 속성별로 리스트에서 제외되는 규칙 </summary>
	public Condition m_Condition;

	public TIngameRewardTable(CSV_Result pResult) {
		m_GroupId = pResult.Get_Int32();
		m_Lv = pResult.Get_Int32();
		m_Val = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
		m_StgIdx = pResult.Get_Int32();
		m_Condition = pResult.Get_Enum<Condition>();
	}

	/// <summary> 모드별 나올 수 있는 카드타입 체크 </summary>
	public bool CheckMode(StageModeType _mode) {
		bool canpick = false;
		switch (_mode) {
			case StageModeType.Stage:
				switch (TDATA.GetStageCardTable(m_Val).m_Type) {
					case StageCardType.EnergyUp:
					case StageCardType.AddGuard:
						canpick = false;
						break;
					default:
						canpick = true;
						break;
				}
				break;
			case StageModeType.Tower:
			case StageModeType.NoteBattle:
				switch (TDATA.GetStageCardTable(m_Val).m_Type) {
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
					case StageCardType.CriticalDmgUp:
					case StageCardType.Gamble:
						canpick = true;
						break;
				}
				break;
			case StageModeType.Training:
				canpick = false;
				break;
			default:
				canpick = true;
				break;
		}
		return canpick;
	}
	public bool CheckCondition() {
		bool canpick = false;
		TStageTable sdata = STAGEINFO.m_TStage;
		List<TStageCardTable> cards = TDATA.GetStageCardGroup(sdata.m_Idx % 100);

		switch (m_Condition) {
			case Condition.None: canpick = true; break;
			case Condition.Turn: canpick = sdata.m_LimitTurn != 0; break;
			case Condition.Time: canpick = sdata.m_Fail.m_Type == StageFailType.Time; break;
			case Condition.Men: canpick = sdata.m_Stat[(int)StatType.Men] > -1; break;
			case Condition.Hyg: canpick = sdata.m_Stat[(int)StatType.Hyg] > -1; break;
			case Condition.Sat: canpick = sdata.m_Stat[(int)StatType.Sat] > -1; break;
			case Condition.Dark: canpick = sdata.m_DarkLv > 0; break;
			case Condition.AirStrike: canpick = sdata.m_PlayType.Find(o => o.m_Type == PlayType.FieldAirstrike) != null; break;
			case Condition.Fire:
			case Condition.Boom:
			case Condition.Heal:
			case Condition.Arson:
				int stgidx = 0;
				StageMaterialType type = StageMaterialType.None;
				if (m_Condition == Condition.Boom) type = StageMaterialType.ShockBomb;
				else if (m_Condition == Condition.Heal) type = StageMaterialType.MedBottle;
				else if (m_Condition == Condition.Arson) type = StageMaterialType.FireBomb;
				else if (m_Condition == Condition.Fire) type = StageMaterialType.FireSpray;

#if STAGE_TEST
				if (STAGEINFO.m_StageContentType != StageContentType.Stage) {
					TModeTable mtdata = TDATA.GetModeTable(STAGEINFO.m_Idx);
					stgidx = mtdata == null ? 0 : mtdata.m_StageLimit;
					if (stgidx == 0) stgidx = PlayerPrefs.GetInt($"TestStageClearIdx_{USERINFO.m_UID}");
				}
				else {
					stgidx = STAGEINFO.m_Idx;
				}
#else
				if (STAGEINFO.m_PlayType == StagePlayType.Stage)
					stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx;
				else
					stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;
#endif
				canpick = stgidx >= TDATA.GetStageMakingData(type).m_Condition.m_Value && (m_Condition == Condition.Fire ? cards.Find(o=>o.m_Type == StageCardType.Fire && (o.m_Prob > 0 || o.m_DarkProb > 0)) != null : true);
				break;
		}

		return canpick;
	}
	public bool CheckStage(int _sidx, StageDifficultyType _type) {
		TStageExceptTable data = TDATA.GeTStageExceptTable(_sidx, _type);
		if (data == null) return true;
		else return !data.m_Cards.Contains(TDATA.GetStageCardTable(m_Val).m_Type);
	}
}
public class TIngameRewardGroup : ClassMng
{
	public Dictionary<int, int> m_LvTotalProp = new Dictionary<int, int>();
	public List<TIngameRewardTable> m_List = new List<TIngameRewardTable>();

	public void Add(TIngameRewardTable item) {
		if (!m_LvTotalProp.ContainsKey(item.m_Lv)) m_LvTotalProp.Add(item.m_Lv, 0);
		m_LvTotalProp[item.m_Lv] += item.m_Prob;
		m_List.Add(item);
	}
	/// <summary> 특정 레벨 이하 리스트 </summary>
	public List<TIngameRewardTable> GetLvList(int _lv, Predicate<TIngameRewardTable> check = null) {
		List<TIngameRewardTable> list = new List<TIngameRewardTable>();
		for (int i = 0; i < m_List.Count; i++) {
			if (m_List[i].m_Lv <= _lv) list.Add(m_List[i]);
		}
		if (check != null) list = list.FindAll(check);
		return list;
	}
	/// <summary> 특정 레벨 이하 확률 총합</summary>
	public int GetLvTotalProp(int _lv) {
		int total = 0;
		for(int i = _lv; i > -1; i--) {
			if (!m_LvTotalProp.ContainsKey(i)) continue;
			total += m_LvTotalProp[i];
		}
		return total;
	}
}

public class TIngameRewardTableMng : ToolFile
{
	public Dictionary<int, TIngameRewardGroup> DIC_GID = new Dictionary<int, TIngameRewardGroup>();

	public TIngameRewardTableMng() : base("Datas/IngameRewardTable")
	{
	}

	public override void DataInit()
	{
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TIngameRewardTable data = new TIngameRewardTable(pResult);
		// 확률이 없늠놈은 담지 않는다. 사용안하겠다는 의미
		if (data.m_Prob <= 0) return;
		int gid = data.m_GroupId;
		if (!DIC_GID.ContainsKey(gid)) DIC_GID.Add(gid, new TIngameRewardGroup());
		DIC_GID[gid].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// IngameRewardTable
	TIngameRewardTableMng m_IngameReward = new TIngameRewardTableMng();

	/// <summary> 그룹아이디로 확률체크해서 뽑기 </summary>
	public TIngameRewardTable GetPickIngameReward(int _gid, int _lv = 0, Predicate<TIngameRewardTable> check = null, bool _battlereward = false) {
		TIngameRewardTable item = null;
		int lv = Mathf.Min(_lv, BaseValue.INGAME_REWARD_MAXLV);
		if (!m_IngameReward.DIC_GID.ContainsKey(_gid)) return null;
		TIngameRewardGroup group = m_IngameReward.DIC_GID[_gid];
		List<TIngameRewardTable> lvlist = group.GetLvList(lv, check);
		//스테이지 모드타입에 따른 1차 체크
		lvlist = lvlist.FindAll(o => o.CheckMode(STAGEINFO.m_StageModeType) && o.CheckCondition());

		if (_battlereward) {//전투 보상일때는 스테이지 테이블에서 예외 목록 2차 체크
			lvlist = lvlist.FindAll(o => o.m_StgIdx <= STAGEINFO.m_Idx && o.CheckStage(STAGEINFO.m_Idx, (StageDifficultyType)(STAGEINFO.IS_ContentStg ? USERINFO.GetDifficulty() : 0)));
		}
		//뽑을게 없으면 공용에서 뽑기 
		if (lvlist.Count == 0) lvlist.AddRange(m_IngameReward.DIC_GID[301].GetLvList(lv, check));
		int totalprob = 0;
		int sumprob = 0;
		for (int i = lvlist.Count - 1; i > -1; i--) totalprob += lvlist[i].m_Prob;
		int dropprop = UTILE.Get_Random(0, totalprob);
		for (int i = 0; i < lvlist.Count; i++) {
			item = lvlist[i];
			sumprob += item.m_Prob;
			if (sumprob > dropprop) return item;
		}
		return null;
	}
}
