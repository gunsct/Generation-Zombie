
using System;
using System.Collections.Generic;
using UnityEngine;
public enum EBattleResult
{
	NONE = 0,
	WIN,
	LOSE,
	RUN
}

public enum EBattleMode
{
	/// <summary> 일반 전투 </summary>
	Normal = 0,
	/// <summary> 몬스터 습격 </summary>
	EnemyAtk,
	End
}

public class BattleInfo : ClassMng
{
	public EBattleResult m_Result = EBattleResult.NONE;
	//public ScenarioEventInfo m_Scenario;
	public System.Action m_BattleEndCB;

	public EBattleMode m_BattleMode = EBattleMode.Normal;

	public EUserBattleState GetFirstAtkType()
	{
		switch(m_BattleMode)
		{
		case EBattleMode.EnemyAtk: return EUserBattleState.Def;
		}
		return EUserBattleState.Atk;
	}
	/// <summary> 스테이지 결과창 보는 여부</summary>
	public bool m_ViewResult;

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 종료
	public void Result()
	{
#if !STAGE_TEST && BATTLE_TEST
		List<int> list = TDATA.GetEnemyIdxs();
		BATTLE.m_TestMonIdx = list[UTILE.Get_Random(0, list.Count)];
		BATTLE.Init(m_BattleMode, BATTLE.m_TestMonIdx, BATTLE.m_TestEnemyLV);
#else
		TEnemyTable enemy = m_EnemyTData;
		switch (STAGEINFO.m_StageModeType)
		{
		case StageModeType.NoteBattle:
			if(STAGEINFO.m_PlayType == StagePlayType.Stage)
			{
				if(STAGEINFO.m_Check.IsClear())
				{
					// 스테이지 종료
					Clear();
				}
				else if (m_User.GetStat(StatType.HP) < 1 || STAGEINFO.m_Check.IsFail() == StageFailKind.Time) {
					StageFail();
				}
				else
				{
					BATTLE.m_MainUI.SetUserHP();
					STAGE_USERINFO.m_AddLV += STAGEINFO.m_TStage.m_AddEnemyLV;
					BATTLE.Init(m_BattleMode, STAGEINFO.GetCreateEnemyIdx(), STAGEINFO.GetCreateEnemyLV(Mathf.RoundToInt(STAGE_USERINFO.m_AddLV), false));
				}
			}
			else
			{
				if (STAGEINFO.m_Check.IsClear()) Clear();
				else {
					if (m_User.GetStat(StatType.HP) < 1 || STAGEINFO.m_Check.IsFail() == StageFailKind.Time) {
						StageFail();
						//if (STAGEINFO.m_Check.GetClearCnt(0) == 0) StageFail();
						//else Clear();
					}
					else {
						BATTLE.m_MainUI.SetUserHP();
						STAGE_USERINFO.m_AddLV += STAGEINFO.m_TStage.m_AddEnemyLV;
						BATTLE.Init(m_BattleMode, STAGEINFO.GetCreateEnemyIdx(STAGEINFO.m_StageContentType == StageContentType.Bank, ++m_ContinuityCnt), STAGEINFO.GetCreateEnemyLV(Mathf.RoundToInt(STAGE_USERINFO.m_AddLV), false));
					}
				}
			}
			break;
		default:
			GoBackMainState();
			break;
		}
#endif
	}
	public void Clear() {
		MAIN.TimeSlowNFast(0.02f, 0.3f, 1f);
		STAGEINFO.m_Result = StageResult.Clear;
#if NOT_USE_NET
		OnClear();
		USERINFO.m_Achieve.Check_StageClear(USERINFO.m_Stage[STAGEINFO.GetContentType()].Mode, STAGEINFO.m_Pos, STAGEINFO.m_Idx);
#else
		STAGEINFO.StageClear((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 클리어 다시 시도
					Clear();
				});
				return;
			}
			OnClear(res);
		});
#endif

	}
#if NOT_USE_NET
	void OnClear()
#else
	void OnClear(LS_Web.RES_STAGE_CLEAR res)
#endif
	{
		if (!m_ViewResult) {
			GoBackMainState();
			return;
		}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Result, (result, obj) => {
#if NOT_USE_NET
			switch (STAGEINFO.m_PlayType)
			{
			case StagePlayType.Stage:
				TStageTable tdata = STAGEINFO.m_TStage;
				if (tdata != null)
				{
					var stage = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
					stage.Clear = tdata.m_Idx;
					if (tdata.m_NextIdx != 0) stage.Idx = tdata.m_NextIdx;
					stage.PlayCount = 0;
					stage.ChapterReward = TDATA.GetChapterTable(USERINFO.GetDifficulty(), stage.Clear) == null ? 0 : stage.Clear; 
					TStageTable stagetable = TDATA.GetStageTable(stage.Clear, USERINFO.GetDifficulty());
					if (stagetable.m_ClearEvent) USERINFO.m_AddEvent = TDATA.GetRandEventTable().m_Idx;
					stage.Selects.Clear();
					USERINFO.SetMainMenuAlarmVal(MainMenuType.Stage);
				}
				break;
			default:
				USERINFO.OutGameClear();
				break;
			}
			MAIN.Save_UserInfo();
#endif
			GoBackMainState();
		}
#if NOT_USE_NET
		);
#else
		, res);
#endif
	}
	public void StageFail() {
		if (STAGEINFO.m_Result == StageResult.Fail) return;
		MAIN.TimeSlowNFast(0.02f, 0.3f, 1f);
		STAGEINFO.m_Result = StageResult.Fail;
		StageFailKind failkind = STAGEINFO.m_Check.IsFail();

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_FailCause, (result, obj) => {
			if (m_ViewResult) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.StageFailed, (result, obj) => {
					if (result == 0)
						GoBackMainState();
				}, failkind);
			}
		}, failkind); 
	}

	public void GoBackMainState()
	{
		AsyncOperation Async = null;
		switch (STAGEINFO.m_StageModeType)
		{
		case StageModeType.NoteBattle:
		case StageModeType.None:
				Async = MAIN.StateChange(MAIN.GetBackState(), SceneLoadMode.BACKGROUND, () => {
				MAIN.ActiveScene(() =>
				{
					m_BattleEndCB?.Invoke();
				});
			});
			break;
		default:
			Async = MAIN.StateChange(MAIN.GetBackState(), SceneLoadMode.ADDITIVEEND, () => {
				MAIN.ActiveScene(() =>
				{
					if (POPUP.GetMainUI().m_Popup == PopupName.Stage) POPUP.GetMainUI().GetComponent<Main_Stage>().AccToggleCheck();
					m_BattleEndCB?.Invoke();
				});
			});
			break;
		}
	}

	bool CheckBattleReward(TIngameRewardTable reward)
	{
		TStageCardTable tcard = TDATA.GetStageCardTable(reward.m_Val);
		if (tcard == null) return false;
		if (reward.m_Prob < 1) return false;
		switch (tcard.m_Type)
		{
		case StageCardType.Synergy:
			if (!m_User.IS_CreateSynergy()) return false;
			break;
		}
		return !tcard.m_IsEndType;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// User
	public StageUser m_User { get { return STAGEINFO.m_User; } }
	public EUserBattleState m_State;
	public int m_ContinuityCnt = 0;
	public void SetData()
	{
		//m_Scenario = null;
		m_Result = EBattleResult.NONE;

#if !STAGE_TEST && BATTLE_TEST
		m_EnemyLV = BATTLE.m_TestEnemyLV;
		m_User.m_Stat[(int)StatType.HP, 0] = m_User.m_Stat[(int)StatType.HP, 1] = BATTLE.m_TestUserMaxHP;
		m_User.m_Stat[(int)StatType.Sta, 0] = m_User.m_Stat[(int)StatType.Sta, 1] = 100;
		m_User.m_Stat[(int)StatType.Atk, 0] = m_User.m_Stat[(int)StatType.Atk, 1] = BATTLE.m_TestUserAtk;
		m_User.m_Stat[(int)StatType.Def, 0] = m_User.m_Stat[(int)StatType.Def, 1] = BATTLE.m_TestUserDef;
		m_User.m_Stat[(int)StatType.RecSta, 0] = BATTLE.m_TestRecoveryStamina;
		m_User.m_Stat[(int)StatType.Guard, 0] = m_User.m_Stat[(int)StatType.Guard, 1] = BATTLE.m_TestUserShield;
#endif
		m_User.m_Stat[(int)StatType.Sta, 0] = m_User.GetMaxStat(StatType.Sta);
		m_EnemyHP = m_EnemyHPMax = m_EnemyTData.GetStat(EEnemyStat.HP, m_EnemyLV);
	}

	public int GetAtk(ENoteType note)
	{
		int Re = 0;
#if !STAGE_TEST && BATTLE_TEST
		Re = BATTLE.m_TestUserAtk;
#else
		int ItemAtk = 0;
		float ItemAtkPer = 0;
		int SkillAtk = 0;
		float SkillAtkPer = 0;

		switch (note) {
			case ENoteType.Normal: SkillAtkPer += USERINFO.GetSkillValue(SkillKind.NormalNoteAtkUp); break;
			case ENoteType.Combo: SkillAtkPer += USERINFO.GetSkillValue(SkillKind.ComboNoteAtkUp); break;
			case ENoteType.Slash: SkillAtkPer += USERINFO.GetSkillValue(SkillKind.SliceNoteAtkUp); break;
			case ENoteType.Charge: SkillAtkPer += USERINFO.GetSkillValue(SkillKind.ChargeNoteAtkUp); break;
			case ENoteType.Chain: SkillAtkPer += USERINFO.GetSkillValue(SkillKind.ChainNoteAtkUp); break;
		}
		SkillAtkPer += STAGE_USERINFO.GetAtkSkillVal(BATTLEINFO.m_EnemyTData, true);
		// 최종공격력 = ((근력 * 10) +장착아이템공격력(+) + 공격력관련스킬(+)) * (1 + 장착아이템공격력(%) + 공격력관련스킬(%)) 
		Re = (int)((m_User.GetStat(StatType.Atk) + ItemAtk + SkillAtk) * (1 + ItemAtkPer + SkillAtkPer));
#endif
		return Re;
	}

	public int GetDef()
	{
		int Re = 0;
#if !STAGE_TEST && BATTLE_TEST
		Re = BATTLE.m_TestUserDef;
#else
		int ItemDef = 0;
		float ItemDefPer = 0;
		int SkillDef = 0;
		float SkillDefPer = 0;

		SkillDefPer += STAGE_USERINFO.GetDefSkillVal(BATTLEINFO.m_EnemyTData, true);
		// 최종방어력 = ((민첩 * 7) + 장착아이템방어력(+) + 방어력관련스킬(+)) * (1 + 장착아이템방어력(%) + 방어력관련스킬(%)) 
		Re = (int)((m_User.GetStat(StatType.Def) + ItemDef + SkillDef) * (1 + ItemDefPer + SkillDefPer));
#endif
		return Re;
	}

	public int GetDamage(int Atk, int AtkLV, int Def)
	{
		int dmg = BaseValue.GetDamage(Atk, AtkLV, Def, 1f, ENoteHitState.End);
		//권장전투력 데미지 배율
		dmg = Mathf.RoundToInt(dmg * BaseValue.GetCPDmgRatio(false, false, true));
		return dmg;
	}

	public int GetNoteDamage(int Atk, int AtkLV, int Def, float Times, ENoteHitState noteHitState)
	{
		int dmg = BaseValue.GetDamage(Atk, AtkLV, Def, Times, noteHitState);
		//권장전투력 데미지 배율
		dmg = Mathf.RoundToInt(dmg * BaseValue.GetCPDmgRatio(true, false, true));
		return dmg;
	}

	public int GetShildCnt()
	{
		return Mathf.RoundToInt(m_User.GetStat(StatType.Guard));
	}

	public void UseShild() {
		m_User.m_Stat[(int)StatType.Guard, 0] -= 1;
	}

	public int USE_EVA_STAMINA_VALUE()
	{
		float Re = BaseValue.EVA_STAMINA_VALUE;
		float rate = 1f;

		return Mathf.RoundToInt(Re * rate);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Enemy

	/// <summary> 몬스터 인덱스 </summary>
	public int m_EnemyIdx;
	public int m_EnemyLV;
	public int m_EnemyAddLV;
	/// <summary> 몬스터 툴데이터 </summary>
	public TEnemyTable m_EnemyTData { get { return TDATA.GetEnemyTable(m_EnemyIdx); } }
	public TEnemyLevelTable m_EnemyLvTData { get { return TDATA.GetEnemyLevelTable(m_EnemyLV); } }

	/// <summary> 몬스터 HP </summary>
	public int m_EnemyHP, m_EnemyHPMax;


	public void SetEnemy(int Idx, int LV = 1)
	{
		m_EnemyIdx = Idx;
		m_EnemyLV = LV;
	}

	public EnemySkillTable GetSkill(int nPos)
	{
		return m_EnemyTData.GetSkill(nPos);
	}

	public int GetEnemyAtk()
	{
		return m_EnemyTData.GetStat(EEnemyStat.ATK, m_EnemyLV);
	}

	public int GetEnemyDef()
	{
		return m_EnemyTData.GetStat(EEnemyStat.DEF, m_EnemyLV);
	}
}


