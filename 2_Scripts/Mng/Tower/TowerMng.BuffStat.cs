using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TowerMng : ObjMng
{
	EF_BuffCenterAlarm m_AddStatAlarm;
	public void AddStat(StatType type, int value, bool UseCheck = true, StageCardInfo _info = null, string _name = null)
	{
		if (value == 0) return;

		m_User.AddStat(type, value);

		if (STAGE_USERINFO.Is_UseStat(type)) {
			if (m_AddStatAlarm == null) {
				EF_BuffCenterAlarm alarm = UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>();
				m_AddStatAlarm = alarm;
			}
			m_AddStatAlarm.SetData(null, type, value, _info, _name);
		}

		if (UseCheck)
		{
			if (value > 0) PlayAddStatSnd(type);
			m_Check.Check(StageCheckType.Rec_Stat, (int)type, value);
		}
	}

	public void SetBuff(EStageBuffKind kind, int idx)
	{
		TStageCardTable data = TDATA.GetStageCardTable(idx);
		StageCardType type = data.m_Type;
		float befor = 0;
		// 스텟 MAX 증가는 현재 수치도 같이 증가 시켜준다.
		// 클리어 조건에 문제가 되지 않도록 Stage에서 올려준다.
		switch (type)
		{
		/// <summary> HP 회복 </summary>
		case StageCardType.HpUp:
			befor = m_User.GetMaxStat(StatType.HP);
			break;
		/// <summary> 포만도 증가 </summary>
		case StageCardType.SatUp:
			befor = m_User.GetMaxStat(StatType.Sat);
			break;
		/// <summary> 청결도 증가 </summary>
		case StageCardType.HygUp:
			befor = m_User.GetMaxStat(StatType.Hyg);
			break;
		/// <summary> 정신력 증가 </summary>
		case StageCardType.MenUp:
			befor = m_User.GetMaxStat(StatType.Men);
			break;
		}

		float prebuffval = m_User.GetBuffValue(type);
		m_User.SetBuff(kind, idx);

		if (m_AddStatAlarm == null) {
			EF_BuffCenterAlarm alarm = UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>();
			m_AddStatAlarm = alarm;
		}
		m_AddStatAlarm.SetData(null, type, m_User.GetBuffValue(type) - prebuffval);

		switch (type)
		{
		/// <summary> HP 회복 </summary>
		case StageCardType.HpUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) - befor), false);
			break;
		/// <summary> 포만도 증가 </summary>
		case StageCardType.SatUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) - befor), false);
			break;
		/// <summary> 청결도 증가 </summary>
		case StageCardType.HygUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) - befor), false);
			break;
		/// <summary> 정신력 증가 </summary>
		case StageCardType.MenUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) - befor), false);
			break;
		}
		
		//하드/나이트메어 시작 디버프 및 조합칸 버프디버프 
		switch (type) {
			case StageCardType.MergeDelete:
				PlayEffSound(SND_IDX.SFX_0215);
				break;
			case StageCardType.MergeFailChance:
				PlayEffSound(SND_IDX.SFX_0216);
				break;
			case StageCardType.HateBuff:
				PlayEffSound(SND_IDX.SFX_0472);
				break;
			case StageCardType.SkillStatus:
			case StageCardType.TurnAllStatus:
			case StageCardType.TurnStatusHp:
				PlayEffSound(SND_IDX.SFX_0471);
				break;
			case StageCardType.TurnStatusHyg:
			case StageCardType.TurnStatusMen:
			case StageCardType.TurnStatusSat:
				PlayEffSound(SND_IDX.SFX_0470);
				break;
			case StageCardType.MergeSlotCount:
				PlayEffSound(SND_IDX.SFX_0196);
				break;
			default:
				if (prebuffval < m_User.GetBuffValue(type)) {
					switch (type) {
						case StageCardType.TimePlus:
						case StageCardType.LimitTurnUp:
						case StageCardType.AddRerollCount:
							PlayEffSound(SND_IDX.SFX_0462);
							break;
						default:
							PlayEffSound(SND_IDX.SFX_0461);
							break;
					}
				}
				else if (prebuffval > m_User.GetBuffValue(type)) {
					switch (type) {
						case StageCardType.TimePlus:
						case StageCardType.LimitTurnUp:
						case StageCardType.AddRerollCount:
							PlayEffSound(SND_IDX.SFX_0472);
							break;
					}
				}
				break;
		}
		//--m_MainUI?.GetBuffFX();
	}
}
