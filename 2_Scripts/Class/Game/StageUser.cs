using System.Collections.Generic;
using UnityEngine;
public enum EStageBuffKind
{
	Stage = 0,
	Synergy,
	END
}

// UI 버프 표시용 정보
public class StageBuff : ClassMng
{
	public EStageBuffKind m_Kind;
	public int m_Idx;
}
public class StageDebuff: ClassMng
{
	public DebuffType m_Type;
	public int m_Idx;
}
public class StageUser : ClassMng
{
	public float[,] m_Stat = new float[(int)StatType.Max, 2];
	public List<StageDebuff> m_Debuffs = new List<StageDebuff>();
	public List<StageBuff> m_Buffs = new List<StageBuff>();
	public Dictionary<DebuffType, float> m_DebuffValues = new Dictionary<DebuffType, float>();
	public Dictionary<StageCardType, float> m_BuffValues = new Dictionary<StageCardType, float>();
	public Dictionary<JobType, int> m_Synergys = new Dictionary<JobType, int>();
	/// <summary> 스테이지 카드로 등장할 수 있는 시너지 </summary>
	List<JobType> m_CheckSynergys = new List<JobType>();
	/// <summary> 시너지 발동 횟수 카운팅</summary>
	public Dictionary<JobType, int> m_SynergyUseCnt = new Dictionary<JobType, int>();
	public int[] m_AP = new int[2];
	//public int m_Guard = 0;

	// 공식용 케릭터 레벨
	public int m_CharLV = 0;

	// 실패조건의 남은 턴
	public int m_Turn = 0;
	public int m_Time = 0;
	// 진행한 전체 턴수
	public int m_TurnCnt = 0;
	public float m_AddLV = 0;
	public float m_AddZombieLV = 0;

	public long m_TeachChar = 0;
	
	public int m_specialReRollCnt = 1;
	
	public int m_leftReRollCnt = 3;
	// 남은 액티브 스킬 카운트
	public int m_RemainActiveSkillCnt = -1;
	public void Init() {
		m_Buffs.Clear();
		m_SynergyUseCnt.Clear();
		m_Synergys.Clear();
		m_BuffValues.Clear();
		m_DebuffValues.Clear();
		for (int i = m_Debuffs.Count - 1; i > -1; i--) {
			m_Debuffs.Remove(m_Debuffs[i]);
		}

		m_TurnCnt = 0;
		m_AP[1] = BaseValue.BASIC_AP;
		m_AP[0] = Mathf.Clamp(STAGEINFO.m_TStage.m_AP, 0, m_AP[1]);
		//ap량은 스킬이랑 시너지로 증가 가능
		//m_AP += Mathf.RoundToInt(m_AP * USERINFO.GetSkillValue(SkillKind.PlusDefUp));//ap관련 스킬 있을 경우
		m_AddLV = 0;
		m_AddZombieLV = 0;

		m_specialReRollCnt = 0;

		TStageTable tdata = STAGEINFO.m_TStage;
		m_leftReRollCnt = tdata.m_ReRollCnt; // 스테이지 테이블에 리롤카운트만큼

		m_RemainActiveSkillCnt = -1;

		m_CheckSynergys.Clear();
		USERINFO.CheckSynergy();
		for (int i = USERINFO.m_SynergyAll.Count - 1; i > -1; i--) m_CheckSynergys.Add(USERINFO.m_SynergyAll[i]);
		// 사업가와 학생은 스테이지 카드로 등록 불가
		m_CheckSynergys.Remove(JobType.Businessman);
		m_CheckSynergys.Remove(JobType.Student);

		DeckInfo PlayDeck = USERINFO.m_PlayDeck;
		// 시작 시너지 등록
		for (int i = PlayDeck.m_Synergy.Count - 1; i > -1; i--) SetBuff(EStageBuffKind.Synergy, (int)PlayDeck.m_Synergy[i]);

		for (int i = 0; i < (int)StatType.Max; i++) m_Stat[i, 1] = m_Stat[i, 0] = 0;


		m_CharLV = 0;
		int charcnt = 0;
		for (int i = 0; i < 5; i++) {
			CharInfo info = USERINFO.GetChar(PlayDeck.m_Char[i]);

			if (info == null) continue;
			//info.GetCombatPower();//내부에서 매번 cp 계산 안하려고 미리 갱신

			charcnt++;
			m_CharLV += info.m_LV;
			for (StatType stat = StatType.Men; stat < StatType.Max; stat++) 
				m_Stat[(int)stat, 1] += GetCharStat(info, stat, false);
		}

		// 패시브 스킬 최종적으로 계산
		for (StatType stat = StatType.Men; stat < StatType.Max; stat++)
		{
			//m_Stat[(int)stat, 1] = BaseValue.CalcStatValue(stat, m_Stat[(int)stat, 1], USERINFO.GetSkillStatValue(stat));
			m_Stat[(int)stat, 0] = m_Stat[(int)stat, 1];
		}

		if (charcnt > 0) m_CharLV /= charcnt;
		m_Stat[(int)StatType.Guard, 1] = m_Stat[(int)StatType.Guard, 0];
		m_Buffs.Clear();
		SetStartServiverStatValue(StatType.Men, tdata.m_Stat[(int)StatType.Men]);
		SetStartServiverStatValue(StatType.Hyg, tdata.m_Stat[(int)StatType.Hyg]);
		SetStartServiverStatValue(StatType.Sat, tdata.m_Stat[(int)StatType.Sat]);
		SetStartServiverStatValue(StatType.HP, tdata.m_Stat[(int)StatType.HP]);

		//생존스텟에 따른 디버프 체크
		SetDebuff();
	}
	/// <summary> 레벨업 버프 획득시 리셋 및 차액 스텟 회복 </summary>
	public void StatReset() {
		DeckInfo PlayDeck = USERINFO.m_PlayDeck;
		m_CharLV = 0;
		int charcnt = 0;
		float[,] prestats = new float[(int)StatType.Max,2];
		for(int i = 0; i < (int)StatType.Max; i++) {
			prestats[i,0] = GetStat((StatType)i);
			prestats[i,1] = GetMaxStat((StatType)i);
		}

		for (int i = 0; i < (int)StatType.Max; i++) m_Stat[i, 1] = m_Stat[i, 0] = 0;
		for (int i = 0; i < 5; i++) {
			CharInfo info = USERINFO.GetChar(PlayDeck.m_Char[i]);
			if (info == null) continue;
			charcnt++;
			m_CharLV += Mathf.Clamp(info.m_LV + Mathf.RoundToInt(GetBuffValue(StageCardType.LevelUp)), 1, (int)info.m_StgLvLimit);

			for (StatType stat = StatType.Men; stat < StatType.Max; stat++)
				m_Stat[(int)stat, 1] += GetCharStat(info, stat, false);
		}

		// 패시브 스킬 최종적으로 계산
		for (StatType stat = StatType.Men; stat < StatType.Max; stat++)
		{
			//m_Stat[(int)stat, 1] = BaseValue.CalcStatValue(stat, m_Stat[(int)stat, 1], USERINFO.GetSkillStatValue(stat));
			m_Stat[(int)stat, 0] = m_Stat[(int)stat, 1];
		}

		if (charcnt > 0) m_CharLV /= charcnt;
		//생존스텟 최대치 증가만큼 회복
		for (int i = 0; i <= (int)StatType.SurvEnd; i++) {
			m_Stat[i, 0] = prestats[i,0];
			AddStat((StatType)i, (int)(GetMaxStat((StatType)i) - prestats[i,1]), false);
		}

		//가드수도 최대치 증가만큼 증가
		m_Stat[(int)StatType.Guard, 0] += GetMaxStat(StatType.Guard) - prestats[(int)StatType.Guard, 1];
	}
	public float GetCharStat(CharInfo info, StatType stat, bool calcskill = true)
	{
		float times = 1f;

		switch(stat)
		{
		case StatType.HP:
		case StatType.Atk:
		case StatType.Def:
		case StatType.Sta:
		case StatType.RecSta:
		case StatType.Guard:
			break;
		case StatType.Heal:
			// 스탯 6배 증가
			// 안쪽 계산에서 rank + times
			// rank는 기본이 1이므로 5만 더함
			times += 6;
			break;
		}

		float value = info.GetStat(stat, 0, 0, times, Mathf.RoundToInt(GetBuffValue(StageCardType.LevelUp)));

		//if(calcskill)
		//{
		//	return BaseValue.CalcStatValue(stat, value, USERINFO.GetSkillStatValue(stat));
		//}
		return value;
	}

	public bool IS_TurnUse()
	{
		TStageTable tdata = STAGEINFO.m_TStage;
		return tdata.m_LimitTurn > 0;
	}

	public bool Is_UseStat(StatType stat)
	{
		TStageTable tdata = STAGEINFO.m_TStage;
		if (tdata == null) return false;
		switch (stat)
		{
		case StatType.Men:	return tdata.m_Stat[(int)StatType.Men] > -1;
		case StatType.Hyg:	return tdata.m_Stat[(int)StatType.Hyg] > -1;
		case StatType.Sat:	return tdata.m_Stat[(int)StatType.Sat] > -1;
		case StatType.HP:	return tdata.m_Stat[(int)StatType.HP] > -1;
		}
		return true;
	}

	void SetStartServiverStatValue(StatType stat, float rate)
	{
		if (rate < 0) rate = 0;
		m_Stat[(int)stat, 0] = Mathf.RoundToInt(GetMaxStat(stat) * rate);

	}

	public int AddStat(StatType type, int value, bool _usesynergy = true)
	{
		float preval = GetStat(type);
		int val = value;
		//디버프
		if (type == StatType.HP && val > 0) val -= Mathf.RoundToInt(val * STAGE_USERINFO.GetBuffValue(StageCardType.Wound));

		m_Stat[(int)type, 0] = Mathf.Clamp(preval + val, 0, GetMaxStat(type));

		if (_usesynergy) {
			//시너지
			if (type == StatType.Men || type == StatType.Hyg || type == StatType.Sat) {
				if (m_SynergyUseCnt.ContainsKey(JobType.Explorer) && m_SynergyUseCnt[JobType.Explorer] < 1) {
					float? synergy2 = GetSynergeValue(JobType.Explorer, 1);
					if (synergy2 != null && m_Stat[(int)type, 0] < 1) {
						m_Stat[(int)type, 0] = Mathf.Clamp(GetMaxStat(type) * (float)synergy2, 0, GetMaxStat(type));
						STAGE_USERINFO.ActivateSynergy(JobType.Explorer);
						Utile_Class.DebugLog_Value("Explorer 시너지 발동 " + type.ToString() + " 스탯 0될거 30%");
					}
				}
			}
		}

		switch (type)
		{
		case StatType.Men:
		case StatType.Hyg:
		case StatType.Sat:
			DLGTINFO?.f_RfStatUI?.Invoke(type, m_Stat[(int)type, 0], preval, GetMaxStat(type));
			break;
		case StatType.HP:
			DLGTINFO?.f_RfHPUI?.Invoke(m_Stat[(int)type, 0], preval, GetMaxStat(type));
			DLGTINFO?.f_RfHPLowUI?.Invoke(m_Stat[(int)type, 0] < GetMaxStat(type) * 0.3f);
			break;
		}
		//대사 스테이지에서만 나옴
		if(MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			TConditionDialogueGroupTable dltable = null;
			Item_Stage_Char info = STAGE.m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
			switch (type) {
				case StatType.Men:
					dltable = info?.m_Info?.m_TData.GetSpeechTable(val > 0 ? DialogueConditionType.MenUp : DialogueConditionType.MenDown);
					break;
				case StatType.Hyg:
					dltable = info?.m_Info?.m_TData.GetSpeechTable(val > 0 ? DialogueConditionType.HygUp : DialogueConditionType.HypDown);
					break;
				case StatType.Sat:
					dltable = info?.m_Info?.m_TData.GetSpeechTable(val > 0 ? DialogueConditionType.SatUp : DialogueConditionType.SatDown);
					break;
				case StatType.HP:
					if(m_Stat[(int)type, 0] / GetMaxStat(type) < 0.3f)
						dltable = info?.m_Info?.m_TData.GetSpeechTable(DialogueConditionType.UnderHp);
					break;
			}
			if (dltable != null && dltable.IS_CanSpeech()) SetSpeech(dltable, info.transform, 1.5f);
		}

		//생존스텟에 따른 디버프 체크
		SetDebuff();

		return val;
	}

	public int CalcDropStatValue(StatType type, float basevlaue)
	{
		if (basevlaue > 0) return Mathf.RoundToInt(basevlaue);
		float Re = basevlaue;
		float times = 1f;
		switch (type)
		{
		case StatType.Men:
			times = Mathf.Clamp(1f - GetStat(StatType.MenDecreaseDef), 0f, 1f);
			break;
		case StatType.Hyg:
			times = Mathf.Clamp(1f - GetStat(StatType.HygDecreaseDef), 0f, 1f);
			break;
		case StatType.Sat:
			times = Mathf.Clamp(1f - GetStat(StatType.SatDecreaseDef), 0f, 1f);
			break;
		}

		return Mathf.RoundToInt(Re * times);
	}

	public float GetStat(StatType type)
	{
		float add = 0;
		float per = 1f;
		switch (type)
		{
			case StatType.Atk:
				per += GetBuffValue(StageCardType.AtkUp);
				break;
			case StatType.Def:
				per += GetBuffValue(StageCardType.DefUp);
				break;
			case StatType.RecSta:
				per += GetBuffValue(StageCardType.EnergyUp);
				break;
			case StatType.Speed:
				per += GetBuffValue(StageCardType.SpeedUp);
				break;
			case StatType.Critical:
				per += GetBuffValue(StageCardType.CriticalUp);
				break;
			case StatType.CriticalDmg:
				per += GetBuffValue(StageCardType.CriticalDmgUp);
				break;
			case StatType.Heal:
				per += GetBuffValue(StageCardType.HealUp);
				break;
			case StatType.Guard:
				add += GetBuffValue(StageCardType.AddGuard);
				break;
			case StatType.HeadShot:
				per += GetBuffValue(StageCardType.HeadShotUp);
				break;
		}
		return m_Stat[(int)type, 0] * per + add;
	}

	public float GetMaxStat(StatType type)
	{
		int add = 0;
		float per = 1f;
		switch(type)
		{
		case StatType.Men:
			add += Mathf.RoundToInt(GetBuffValue(StageCardType.MenUp));
			break;
		case StatType.Hyg:
			add += Mathf.RoundToInt(GetBuffValue(StageCardType.HygUp));
			break;
		case StatType.Sat:
			add += Mathf.RoundToInt(GetBuffValue(StageCardType.SatUp));
			break;
		case StatType.HP:
			per += GetBuffValue(StageCardType.HpUp);
			break;
		}
		return m_Stat[(int)type, 1] * per + add;
	}

	public float GetAtkSkillVal(TEnemyTable _enemy, bool _note = false) {
		float skillmul = 0;
		switch (_enemy.m_Type) {
			case EEnemyType.Zombie: if(_note) skillmul += USERINFO.GetSkillValue(SkillKind.ZomAtkUp); break;
			case EEnemyType.Animal: if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.AnimalAtkUp); break;
			case EEnemyType.Mutant: if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.MutantAtkUp); break;
			case EEnemyType.Mafia:
			case EEnemyType.Scavenger:
			case EEnemyType.Zealot:
			case EEnemyType.Gangster:
			case EEnemyType.Wolfs:
			case EEnemyType.SatRefugee:
			case EEnemyType.MenRefugee:
			case EEnemyType.HygRefugee:
			case EEnemyType.HpRefugee:
			case EEnemyType.AllRefugee:
			case EEnemyType.RandomRefugee:
			case EEnemyType.SatInfectee:
			case EEnemyType.MenInfectee:
			case EEnemyType.HygInfectee:
			case EEnemyType.HpInfectee:
			case EEnemyType.RandomInfectee:
			case EEnemyType.Allinfectee:
			case EEnemyType.Npc:
			case EEnemyType.MaterialRefugee:
				if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.HumanAtkUp); break;
		}
		switch (_enemy.m_Grade) {
			case EEnemyGrade.Normal: skillmul += USERINFO.GetSkillValue(SkillKind.NormalAtkUp); break;
			case EEnemyGrade.Elite: skillmul += USERINFO.GetSkillValue(SkillKind.EliteAtkUp); break;
			case EEnemyGrade.Boss: skillmul += USERINFO.GetSkillValue(SkillKind.BossAtkUp); break;
		}
		return skillmul;
	}
	public float GetDefSkillVal(TEnemyTable _enemy, bool _note = false) {
		float skillmul = 0;
		switch (_enemy.m_Type) {
			case EEnemyType.Zombie: if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.ZomDefUp); break;
			case EEnemyType.Animal: if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.AnimalDefUp); break;
			case EEnemyType.Mutant: if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.MutantDefUp); break;
			case EEnemyType.Mafia:
			case EEnemyType.Scavenger:
			case EEnemyType.Zealot:
			case EEnemyType.Gangster:
			case EEnemyType.Wolfs:
			case EEnemyType.SatRefugee:
			case EEnemyType.MenRefugee:
			case EEnemyType.HygRefugee:
			case EEnemyType.HpRefugee:
			case EEnemyType.RandomRefugee:
			case EEnemyType.AllRefugee:
			case EEnemyType.SatInfectee:
			case EEnemyType.MenInfectee:
			case EEnemyType.HygInfectee:
			case EEnemyType.HpInfectee:
			case EEnemyType.RandomInfectee:
			case EEnemyType.Allinfectee:
			case EEnemyType.Npc:
			case EEnemyType.MaterialRefugee:
				if (_note) skillmul += USERINFO.GetSkillValue(SkillKind.HumanDefUp); break;
		}
		switch (_enemy.m_Grade) {
			case EEnemyGrade.Normal: skillmul += USERINFO.GetSkillValue(SkillKind.NormalDefUp); break;
			case EEnemyGrade.Elite: skillmul += USERINFO.GetSkillValue(SkillKind.EliteDefUp); break;
			case EEnemyGrade.Boss: skillmul += USERINFO.GetSkillValue(SkillKind.BossDefUp); break;
		}
		return skillmul;
	}
	public bool IS_CreateSynergy()
	{
		return m_CheckSynergys.Count > 0;
	}
	public JobType CreateSynergy()
	{
		int cnt = m_CheckSynergys.Count;
		if (cnt < 1) return JobType.None;
		return m_CheckSynergys[UTILE.Get_Random(0, cnt)];
	}
	public void ActivateSynergy(JobType _type) {
		m_SynergyUseCnt[_type]++;
		if (TDATA.GetSynergyTable(_type).m_UseAlarm) {
			if (STAGE != null && STAGE.m_MainUI != null) STAGE.m_MainUI.ActiveSynergy(_type);
		}
	}
	public void SetBuff(EStageBuffKind kind, int idx)
	{
		if (ISBuff(StageCardType.HateBuff)) return;

		m_Buffs.Add(new StageBuff() { m_Kind = kind, m_Idx = idx });
		StageCardType type;
		switch (kind)
		{
		case EStageBuffKind.Stage:
			TStageCardTable data = TDATA.GetStageCardTable(idx);
			float val = data.m_Value1;

			type = data.m_Type;
			if (!m_BuffValues.ContainsKey(type)) {
					if(type == StageCardType.MergeSlotCount) {
						int max = BaseValue.STAGE_MAKE_GETMAX - Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConMergeSlotDown));
						int lockcnt = Mathf.Clamp(max - (STAGEINFO.m_TStage.m_MakingCnt + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MergeSlotCount))), 0, max);
						if (val > 0) {
							m_BuffValues.Add(type, Mathf.Clamp(val, 0, lockcnt));
						}
						else {
							m_BuffValues.Add(type, Mathf.Clamp(val, lockcnt - max, 0));
						}
					}
					else
						m_BuffValues.Add(type, val);
			}
			else
			{
				//중복 안되는것들 예외?
				switch (type)
				{
					case StageCardType.Wound:
					case StageCardType.ConActiveAP:
					case StageCardType.DarkTurn:
					case StageCardType.ConActiveSkillLimit:
					case StageCardType.ApPlus:
					case StageCardType.RandomMaterial:
					case StageCardType.HateBuff:
					case StageCardType.ConRandomChoice:
					case StageCardType.MergeDelete:
					case StageCardType.ConEnemyTeamwork:
					case StageCardType.ConSkipTurn:
					case StageCardType.ConStageCardLock:
					case StageCardType.ConRewardCardLock:
					case StageCardType.ConBlindRewardInfo:
					case StageCardType.ConKnockDownChar:
					case StageCardType.MergeFailChance:
					case StageCardType.SkillHp:
					case StageCardType.SkillStatus:
					case StageCardType.MoreMaterial:
					case StageCardType.NoHpBar:
					case StageCardType.EnemyProbUp:
					case StageCardType.NoAutoHeal:
					case StageCardType.TurnAllStatus:
					case StageCardType.TurnStatusMen:
					case StageCardType.TurnStatusHyg:
					case StageCardType.TurnStatusSat:
					case StageCardType.TurnStatusHp:
					case StageCardType.GoEnemy:
					case StageCardType.ConDeadly:
						break;
					case StageCardType.HpUp:
						m_BuffValues[type] += (1f + m_BuffValues[type]) * val;
						break;
					case StageCardType.MergeSlotCount:
							int max = BaseValue.STAGE_MAKE_GETMAX - Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConMergeSlotDown));
							int lockcnt = Mathf.Clamp(max - (STAGEINFO.m_TStage.m_MakingCnt + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MergeSlotCount))), 0, max);
						if(val > 0) {
							m_BuffValues[type] += Mathf.Clamp(val, 0, lockcnt);
						}
						else {
							m_BuffValues[type] += Mathf.Clamp(val, lockcnt - max, 0);
						}
						break;
					default: m_BuffValues[type] += val; break;
				}
			}
			if (POPUP.GetBattleUI() == null && POPUP.GetMainUI().m_Popup == PopupName.Stage)
			{//캐릭터 카드가 있는 스테이지 유아이에서만
				switch (type)
				{
				case StageCardType.LevelUp:
					for (int i = 0; i < STAGE.m_Chars.Length; i++) STAGE.m_Chars[i].RefreshLvText(); break;
				case StageCardType.APConsumDown:
				case StageCardType.ApPlus:
					for (int i = 0; i < STAGE.m_Chars.Length; i++) {
						STAGE.m_Chars[i].RefreshAPText();
						STAGE.m_Chars[i].SetAPUI(m_AP[0]);
					}
					break;
				case StageCardType.ConActiveAP:
					int pre = m_AP[0];
					m_AP[0] = Mathf.Clamp(Mathf.RoundToInt(m_BuffValues[type]), 0, m_AP[1]);
					DLGTINFO?.f_RfAPUI?.Invoke(m_AP[0], pre, m_AP[1]);
					break;
				case StageCardType.DarkTurn:
					DLGTINFO?.f_HDClockUI?.Invoke(true);
					break;
				}
			}
			if (STAGEINFO.m_StageModeType == StageModeType.Stage)
			{
				switch (type)
				{
				case StageCardType.ConMergeSlotDown:
				case StageCardType.MergeSlotCount:
					STAGE.m_MainUI.GetMaking.LockInit();
					break;
				case StageCardType.ConActiveSkillLimit:
					m_RemainActiveSkillCnt = Mathf.RoundToInt(m_BuffValues[type]);
					break;
				case StageCardType.MaterialCountDown:
				case StageCardType.RandomMaterial:
					STAGE.AllRefreshCardImgName();

					break;
				}
			}
			Utile_Class.DebugLog_Value(type.ToString() + " 버프 값 : " + m_BuffValues[type].ToString());
			break;
		case EStageBuffKind.Synergy:
			JobType job = (JobType)idx;
			if (!m_Synergys.ContainsKey(job))
			{
				m_Synergys.Add(job, 1);
				m_SynergyUseCnt.Add(job, 0);
			}
			m_CheckSynergys.Remove(job);
			break;
		}
	}

	public bool ISBuff(StageCardType buff)
	{
		return m_BuffValues.ContainsKey(buff);
	}

	public float GetBuffValue(StageCardType buff)
	{
		if (!ISBuff(buff)) return 0f;
		return m_BuffValues[buff];
	}

	public bool ISSynergy(JobType synergy)
	{
		return m_Synergys.ContainsKey(synergy);
	}

	public float? GetSynergeValue(JobType synergy, int _pos)
	{
		if (!ISSynergy(synergy)) return null;
		TSynergyTable TSynergy = TDATA.GetSynergyTable(synergy);
		return TSynergy.m_Value[_pos];
	}

	public bool IS_Die()
	{
		return GetStat(StatType.HP) < 1;
	}
	/// <summary> 캐릭터 대사</summary>
	public void SetSpeech(TConditionDialogueGroupTable _table, Transform _target, float _time) {
		if (TUTO.IsTutoPlay()) return;
		if (STAGE != null) STAGE?.m_MainUI.GetSpeechBubble().SetData(_target, _table, _time);
		if (BATTLE != null) BATTLE?.m_MainUI.GetSpeechBubble().SetData(_target, _table, _time);
	}
	public void SetSpeech(string _txt, Transform _target, float _time) {
		if (TUTO.IsTutoPlay()) return;
		if (STAGE != null) STAGE?.m_MainUI.GetSpeechBubble().SetData(_target, _txt, _time);
		if (BATTLE != null) BATTLE?.m_MainUI.GetSpeechBubble().SetData(_target, _txt, _time);
	}
	/// <summary> 매턴 끝나고 마지막에 생존스텟으로 디버프 체크 </summary>
	public void SetDebuff() {
		for (int i = 0; i < (int)StatType.SurvEnd; i++) {
			if (!Is_UseStat((StatType)i)) continue;
			List<StageDebuff> deldebuff = m_Debuffs.FindAll(t => TDATA.GetStatusDebuffTable(t.m_Idx).m_StatType == (StatType)i && Mathf.RoundToInt(TDATA.GetStatusDebuffTable(t.m_Idx).m_StatVal * m_Stat[i, 1]) < Mathf.RoundToInt(m_Stat[i, 0]));
			if(deldebuff.Count > 0) {
				for (int j = deldebuff.Count - 1; j > -1; j--) {
					m_Debuffs.Remove(deldebuff[j]);
					if(m_DebuffValues.ContainsKey(deldebuff[j].m_Type)) m_DebuffValues.Remove(deldebuff[j].m_Type);
				}
			}

			TStatusDebuffTable table = TDATA.GetStatusDebuffTable(STAGEINFO.m_PlayType == StagePlayType.Stage ? STAGEINFO.m_Idx : 0, (StatType)i, m_Stat[i, 0], m_Stat[i, 1]);
			if (table == null) {
				if (STAGE != null) STAGE?.m_MainUI.SetDebuffAlarm((StatType)i, null);
				continue;
			}

			if (m_DebuffValues.ContainsKey(table.m_DebuffType)) {
				m_DebuffValues[table.m_DebuffType] = table.m_DebuffVal;
			}
			else m_DebuffValues.Add(table.m_DebuffType, table.m_DebuffVal);

			StageDebuff debuff = m_Debuffs.Find(t => t.m_Idx == table.m_Idx);
			if (debuff == null) m_Debuffs.Add(debuff = new StageDebuff() { m_Type = table.m_DebuffType, m_Idx = table.m_Idx });
			else debuff.m_Idx = table.m_Idx;
			if (STAGE != null) STAGE?.m_MainUI.SetDebuffAlarm((StatType)i, debuff);
		}
	}
	public void CharSpeech(DialogueConditionType _type, Item_Stage_Char _user = null) {
		Item_Stage_Char user = _user != null ? _user : STAGE.m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
		TConditionDialogueGroupTable table = user.m_Info.m_TData.GetSpeechTable(_type);
		if (table != null && table.IS_CanSpeech()) SetSpeech(table, user.transform, 1.5f);
	}

	public bool IS_BadCardBuff(StageCardType _type) {
		switch (_type) {
			case StageCardType.ConMergeSlotDown:
			case StageCardType.MaterialCountDown:
			case StageCardType.Wound:
			case StageCardType.ConActiveAP:
			case StageCardType.DarkTurn:
			case StageCardType.ConActiveSkillLimit:
			case StageCardType.ApPlus:
			case StageCardType.RandomMaterial:
			case StageCardType.HateBuff:
			case StageCardType.ConRandomChoice:
			case StageCardType.MergeDelete:
			case StageCardType.ConEnemyTeamwork:
			case StageCardType.ConSkipTurn:
			case StageCardType.ConStageCardLock:
			case StageCardType.ConRewardCardLock:
			case StageCardType.ConBlindRewardInfo:
			case StageCardType.ConKnockDownChar:
			case StageCardType.MergeFailChance:
			case StageCardType.SkillHp:
			case StageCardType.SkillStatus:
			case StageCardType.MoreMaterial:
			case StageCardType.NoHpBar:
			case StageCardType.EnemyProbUp:
			case StageCardType.NoAutoHeal:
			case StageCardType.TurnAllStatus:
			case StageCardType.TurnStatusMen:
			case StageCardType.TurnStatusHyg:
			case StageCardType.TurnStatusSat:
			case StageCardType.TurnStatusHp:
			case StageCardType.GoEnemy:
			case StageCardType.ConDeadly:
				return true;
			default: return false;
		}
	}
	public int GetRewardRerollPrice() {
		TShopTable tdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_REROLLING);
		return tdata.m_UpPrice * m_specialReRollCnt + tdata.m_Price;
	}
}
