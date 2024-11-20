using System.Collections.Generic;
using UnityEngine;

public enum StageCheckType
{
	/// <summary> 목표 카드를 선택 했을 경우 (유틸리티나 스킬에 의한 선택은 제외)
	/// <para> Value : 목표 Card ID - 해당 스테이지의 StageCardTable의 Index 참조 </para>
	/// <para> cnt : 선택 요구 개수 </para>
	/// </summary>
	CardUse = 0,
	/// <summary> 특정 몬스터 죽이기 (0 아무몬스터)
	/// <para> Value : 몬스터 인덱스 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy,
	/// <summary> 특정 타입 몬스터 죽이기(0 아무몬스터)
	/// <para> Value : 몬스터 타입번호 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy_Type,
	/// <summary> 특전 종족의 몬스터 죽이기 (0 마무 몬스터)
	/// <para> Value : 몬스터 종족 번호 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy_Tribe,
	/// <summary> 특정 등급 내 몬스터 죽이기 (0 아무 그룹 몬스터)
	/// <para> Value : 몬스터 등급 번호 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy_Grade,
	/// <summary> 생존 스테이터스를 일정 수치 이상 회복
	/// <para> Value : 스텟 번호 </para>
	/// <para> cnt : 증가량 </para>
	/// </summary>
	Rec_Stat,
	/// <summary> 목표 시간만큼 생존
	/// <para> cnt : 목표 시간(단위 : 시) </para>
	/// </summary>
	Survival,
	/// <summary> 피난민 목표 횟수만큼 구출, 피난민 전부, 비감염 피난민, 감염 피난민
	/// <para> Value01 : 목표 수치 </para>
	/// </summary>
	Rescue,
	Rescue_Refugee,
	Rescue_Infectee,
	/// <summary> 배틀 횟수가 목표 횟수에 도달했을 경우 패배 </summary>
	TurmoilCount,

	/// <summary> 퀘스트 조건 완료 시 </summary>
	ClearCondition,
	/// <summary> 카드 라인 생성
	/// <para> cnt : 생성 개수 </para>
	/// </summary>
	CreateCardLine,
	/// <summary> 아무 스킬 사용 시 클리어
	/// <para> cnt : 생성 개수 </para>
	/// </summary>
	UseSkill,
	/// <summary> 필드의 ‘화염’ 카드를 N회 이상 소화시킬 경우 클리어.</summary>
	SuppressionF,
	/// <summary> 필드의 ‘보급 상자’를 N회 이상 습득하였을 경우 클리어.</summary>
	GetBox,
	/// <summary> 제작을 N회 이상 하였을 경우 클리어. </summary>
	AnyMaking,
	/// <summary> Value01 : 디폴트 카드 테이블의 타입 번호 0은 전부, Value02 개수 </summary>
	Fire_Card,
	/// <summary> Value01 : 에너미의 타입 번호 0은 전부, Value02 개수 </summary>
	Fire_Enemy,
	/// <summary> Value 01 : 특정 감염된 피난민 인덱스 (0일 경우 모든 감염된 피난민), count01 : 목표 횟수 </summary>
	Kill_infectee,
	/// <summary> 한 턴 내에 n마리 이상의 적을 처치해야지만 성공 카운트가 1 올라가는 목표, count01 : 목표 횟수 </summary>
	KillEnemy_Turn,
	/// <summary> </summary>
	End
}

public enum StageCheckState
{
	/// <summary> 플레이 </summary>
	Play = 0,
	/// <summary> 달성 </summary>
	Success
}

public class StageCheckInfo : ClassMng
{
	public int m_Idx;
	public StageCheckState m_State = StageCheckState.Play;
	public int m_Cnt = 0;

	public StageCheckInfo(int idx)
	{
		m_Idx = idx;
	}
}

public class StageCheck : ClassMng
{
	/// <summary> 스테이지에서 몬스터를 죽인 카운트 </summary>
	public int m_KillEnemyCnt = 0;
	/// <summary> 소탕용 CountBox획득 개수 </summary>
	public int m_CountBoxCnt = 0;
	/// <summary> 클리어 조건이 턴종료일때 실패체크에서 제거 </summary>
	public bool m_IsTurnEndClear = false;
	public void Init()
	{
		m_IsTurnEndClear = false;
		m_CountBoxCnt = m_KillEnemyCnt = 0;
		ClearCheckInit();
		FailCheckInit();
		CardAppearInit();
	}

	public void Check(StageCheckType type, int value, int cnt = 1, bool isZeroCheck = true)
	{
		//연속 미션일 경우 이전 미션 안되면 뒤에건 체크 안함
		if (STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.Continuity && m_ClearCheck.Length > 1) 
			if (!PreClearCheck(type, value)) return;

		CheckClear(type, value, cnt);
		CheckFail(type, value, cnt);
		CheckCardAppear(type, value, cnt);
		CheckCardDisappear(type, value, cnt);

		// 해당 타입의 아무거나 체크
		bool ignoretype = true;
		switch (type) {
			case StageCheckType.Rec_Stat:
			case StageCheckType.KillEnemy_Turn:
				ignoretype = false;
				break;
			default:
				ignoretype = true;
				break;
		}
		if (isZeroCheck && value != 0 && ignoretype)
		{
			Check(type, 0, cnt);
			// 스테이지 클리어 조건으로 들어갈수있으므로 추후작업에따라 제거필요
			switch(type)
			{
			case StageCheckType.KillEnemy:
				m_KillEnemyCnt+=cnt;
				break;
			case StageCheckType.CardUse:
				if((StageCardType)value == StageCardType.CountBox) m_CountBoxCnt++;
				break;
			}
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Clear 조건 체크
	public StageCheckInfo[] m_ClearCheck;
	void ClearCheckInit()
	{
		TStageTable stage = STAGEINFO.m_TStage;
		m_ClearCheck = new StageCheckInfo[stage.m_Clear.Count];
		for (int i = m_ClearCheck.Length - 1; i > -1; i--)
		{
			m_ClearCheck[i] = new StageCheckInfo(i);
			if (stage.m_Clear[i].m_Type == StageClearType.Survival) m_IsTurnEndClear = true;
		}
	}

	/// <summary> 연속 미션에서 이전 미션 완료됬는지 체크 </summary>
	public bool PreClearCheck(StageCheckType _type, int _val) {
		if (m_ClearCheck.Length <= 1) return true;

		StageClearType cleartype = StageClearType.None;
		switch (_type) {
			case StageCheckType.CardUse: cleartype = StageClearType.CardUse; break;
			case StageCheckType.KillEnemy: cleartype = StageClearType.KillEnemy; break;
			case StageCheckType.KillEnemy_Type: cleartype = StageClearType.KillEnemy_Type; break;
			case StageCheckType.KillEnemy_Tribe: cleartype = StageClearType.KillEnemy_Tribe; break;
			case StageCheckType.KillEnemy_Grade: cleartype = StageClearType.KillEnemy_Grade; break;
			case StageCheckType.Rec_Stat: cleartype = StageClearType.Rec_Stat; break;
			case StageCheckType.Survival: cleartype = StageClearType.Survival; break;
			case StageCheckType.Rescue: cleartype = StageClearType.Rescue; break;
			case StageCheckType.Rescue_Refugee: cleartype = StageClearType.Rescue_Refugee; break;
			case StageCheckType.Rescue_Infectee: cleartype = StageClearType.Rescue_Infectee; break;
			case StageCheckType.UseSkill: cleartype = StageClearType.UseSkill; break;
			case StageCheckType.SuppressionF: cleartype = StageClearType.SuppressionF; break;
			case StageCheckType.GetBox: cleartype = StageClearType.GetBox; break;
			case StageCheckType.AnyMaking: cleartype = StageClearType.AnyMaking; break;
			case StageCheckType.Fire_Card: cleartype = StageClearType.Fire_Card; break;
			case StageCheckType.Fire_Enemy: cleartype = StageClearType.Fire_Enemy; break;
			case StageCheckType.Kill_infectee: cleartype = StageClearType.Kill_infectee; break;
			case StageCheckType.KillEnemy_Turn: cleartype = StageClearType.KillEnemy_Turn; break;
		}

		//이전 미션 진행중이면 체크안함
		for (int i = 1; i < m_ClearCheck.Length; i++) {
			if (STAGEINFO.m_TStage.m_Clear[i].m_Type != cleartype) continue;
			if (STAGEINFO.m_TStage.m_Clear[i].m_Value != _val) continue;
			if(m_ClearCheck[i - 1].m_State == StageCheckState.Play) return false;
		}
		return true;
	}
	public void CheckClear(StageCheckType checkType, int Value, int Cnt)
	{
		StageClearType type = StageClearType.None;
		switch (checkType)
		{
			case StageCheckType.CardUse: type = StageClearType.CardUse; break;
			case StageCheckType.KillEnemy: type = StageClearType.KillEnemy; break;
			case StageCheckType.KillEnemy_Type: type = StageClearType.KillEnemy_Type; break;
			case StageCheckType.KillEnemy_Tribe: type = StageClearType.KillEnemy_Tribe; break;
			case StageCheckType.KillEnemy_Grade: type = StageClearType.KillEnemy_Grade; break;
			case StageCheckType.Rec_Stat: type = StageClearType.Rec_Stat; break;
			case StageCheckType.Survival: type = StageClearType.Survival; break;
			case StageCheckType.Rescue: type = StageClearType.Rescue; break;
			case StageCheckType.Rescue_Refugee: type = StageClearType.Rescue_Refugee; break;
			case StageCheckType.Rescue_Infectee: type = StageClearType.Rescue_Infectee; break;
			case StageCheckType.UseSkill: type = StageClearType.UseSkill; break;
			case StageCheckType.SuppressionF: type = StageClearType.SuppressionF; break;
			case StageCheckType.GetBox: type = StageClearType.GetBox; break;
			case StageCheckType.AnyMaking: type = StageClearType.AnyMaking; break;
			case StageCheckType.Fire_Card: type = StageClearType.Fire_Card; break;
			case StageCheckType.Fire_Enemy: type = StageClearType.Fire_Enemy; break;
			case StageCheckType.Kill_infectee: type = StageClearType.Kill_infectee; break;
			case StageCheckType.KillEnemy_Turn: type = StageClearType.KillEnemy_Turn; break;
		}

		if (type == StageClearType.None) return;

		TStageTable stage = STAGEINFO.m_TStage;
		for (int i = stage.m_Clear.Count - 1; i > -1; i--)
		{
			TStageCondition<StageClearType> clear = stage.m_Clear[i];
			StageCheckInfo check = m_ClearCheck[i];
			if (check.m_State == StageCheckState.Success) continue;
			if (clear.m_Type != type) continue;
			switch (clear.m_Type) {
				case StageClearType.KillEnemy_Turn:
					if (clear.m_Value < Value) continue;
					break;
				default:
					if (clear.m_Value != Value) continue;
					break;
			}
			TGuideTable guide = TDATA.GetGuideTable(clear.m_Type);
			float CheckCnt = clear.m_Cnt;

			check.m_Cnt += Cnt;
			//switch(clear.m_Type)
			//{
			//case StageClearType.Rec_Stat:
			//    if (guide.m_IsRateValue)
			//    {
			//        CheckCnt = STAGE_USERINFO.GetMaxStat((StatType)clear.m_Value) * CheckCnt;
			//    }
			//    break;
			//}
			int Max = Mathf.RoundToInt(CheckCnt);
			if (check.m_Cnt >= Max)
			{
				check.m_Cnt = Mathf.RoundToInt(CheckCnt);
				check.m_State = StageCheckState.Success;
				STAGE?.m_MainUI?.ClearGuide(type, i);
				TOWER?.m_MainUI?.ClearGuide(type, i);

				if (i == 0) Check(StageCheckType.ClearCondition, 0);
			}
			else
			{
				if (STAGEINFO.m_Result == StageResult.Fail) continue;
				STAGE?.m_MainUI?.RefreshGuide(type, i);
				TOWER?.m_MainUI?.RefreshGuide(type, i);
			}
		}
	}
	public int GetClearValue(int Pos) {
		TStageTable stage = STAGEINFO.m_TStage;
		TStageCondition<StageClearType> clear = stage.m_Clear[Pos];
		return clear.m_Value;
	}

	public int GetClearCnt(int Pos)
	{
		int Re = m_ClearCheck[Pos].m_Cnt;
		TStageTable stage = STAGEINFO.m_TStage;
		TStageCondition<StageClearType> clear = stage.m_Clear[Pos];
		TGuideTable guide = TDATA.GetGuideTable(clear.m_Type);
		//switch (clear.m_Type)
		//{
		//case StageClearType.Rec_Stat:
		//    if (guide.m_IsRateValue)
		//    {
		//        Re = Mathf.RoundToInt((float)Re / (float)STAGE_USERINFO.GetMaxStat((StatType)clear.m_Value) * 100f);
		//    }
		//    break;
		//}
		return Re;
	}

	public int GetClearMaxCnt(int Pos)
	{
		TStageTable stage = STAGEINFO.m_TStage;
		TStageCondition<StageClearType> clear = stage.m_Clear[Pos];
		TGuideTable guide = TDATA.GetGuideTable(clear.m_Type);
		float Re = clear.m_Cnt;
		if(guide.m_IsRateValue) Re = Re * 100f;
		return Mathf.RoundToInt(Re);
	}

	public bool IsClear()
	{
		for (int i = m_ClearCheck.Length - 1; i > -1; i--)
		{
			if (m_ClearCheck[i].m_State != StageCheckState.Success) return false;

		}
		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_201, (int)TutoType_Stage_201.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_205, (int)TutoType_Stage_205.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_301, (int)TutoType_Stage_301.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_501, (int)TutoType_Stage_501.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_601, (int)TutoType_Stage_601.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Delay_Clear)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_801, (int)TutoType_Stage_801.Delay_Clear)) TUTO.Next();
		return true;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Fail 조건 체크
	public StageCheckInfo m_FailCheck;
	void FailCheckInit()
	{
		m_FailCheck = new StageCheckInfo(0);
	}

	public void CheckFail(StageCheckType checkType, int Value, int Cnt)
	{
		StageFailType type = StageFailType.None;
		switch (checkType)
		{
		case StageCheckType.TurmoilCount: type = StageFailType.TurmoilCount; break;
		case StageCheckType.Survival: type = StageFailType.None; break;
		}

		if (type == StageFailType.None) return;

		TStageTable stage = STAGEINFO.m_TStage;
		TStageCondition<StageFailType> fail = stage.m_Fail;
		StageCheckInfo check = m_FailCheck;
		if (check.m_State == StageCheckState.Success) return;
		if (fail.m_Type != type) return;
		if (fail.m_Value != Value) return;
		int Max = Mathf.RoundToInt(fail.m_Cnt);

		check.m_Cnt += Cnt;

		if (check.m_Cnt >= Max)
		{
			check.m_Cnt = Max;
			check.m_State = StageCheckState.Success;
		}
	}
	public StageFailKind IsFail() {
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.TurmoilCount && m_FailCheck.m_Cnt >= STAGEINFO.m_TStage.m_Fail.m_Cnt) return StageFailKind.TurmoilCount;
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time) {
			float time = STAGEINFO.m_TStage.m_Fail.m_Value;
			//시너지
			StageUser user = null;
			if (MAIN.IS_State(MainState.STAGE)) user = STAGE.m_User;
			else if (MAIN.IS_State(MainState.BATTLE)) user = BATTLEINFO.m_User;
			float? synergy2 = user.GetSynergeValue(JobType.Scientist, 1);
			if (synergy2 != null) {
				time = time + (float)synergy2;
				Utile_Class.DebugLog_Value("Scientist 시너지 발동 " + "변화 전 -> 후 : 전 :" + STAGEINFO.m_TStage.m_Fail.m_Value.ToString() + " 후 : " + time.ToString());
				//STAGE.m_User.m_SynergyUseCnt[JobType.Scientist]++;
			}
			//TimePlus 버프카드 적용
			time = time * (1f + STAGE_USERINFO.GetBuffValue(StageCardType.TimePlus));
			float timer = 0;
			if (MAIN.IS_State(MainState.STAGE)) timer = STAGE.m_Timer;
			else if (MAIN.IS_State(MainState.BATTLE)) timer = BATTLE.m_Timer;
			if (timer >= time) return StageFailKind.Time;
		}
		if (STAGE_USERINFO.IS_TurnUse() && STAGE_USERINFO.m_Turn < 1) return StageFailKind.Turn;//!m_IsTurnEndClear && 
		if (STAGE_USERINFO.Is_UseStat(StatType.Men) && STAGE_USERINFO.GetStat(StatType.Men) < 1) {
			if (MAIN.IS_State(MainState.STAGE) && STAGE.m_RebornSrv[(int)StatType.Men] < 1) {
				float dnaremen = 0f;
				for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
					float dna = STAGE.m_Chars[i].m_Info.GetDNABuff(OptionType.MenResurrection);
					if (dna > 0) STAGE.DNAAlarm(STAGE.m_Chars[i].m_Info, OptionType.MenResurrection);
						dnaremen += dna;
				}
				if(dnaremen > 0f) {
					STAGE.Call_AddStat_Action(STAGE.m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * dnaremen));
					STAGE.m_RebornSrv[(int)StatType.Men]++;
					Utile_Class.DebugLog_Value("멘탈 0 사망 후 DNA MenResurrection로 부활");
					return StageFailKind.None;
				}
			}
			return StageFailKind.Men;
		}
		if (STAGE_USERINFO.Is_UseStat(StatType.Hyg) && STAGE_USERINFO.GetStat(StatType.Hyg) < 1) {
			if (MAIN.IS_State(MainState.STAGE) && STAGE.m_RebornSrv[(int)StatType.Hyg] < 1) {
				float dnaremen = 0f;
				for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
					float dna = STAGE.m_Chars[i].m_Info.GetDNABuff(OptionType.HygResurrection);
					if (dna > 0) STAGE.DNAAlarm(STAGE.m_Chars[i].m_Info, OptionType.HygResurrection);
					dnaremen += dna;
				}
				if (dnaremen > 0f) {
					STAGE.Call_AddStat_Action(STAGE.m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * dnaremen));
					STAGE.m_RebornSrv[(int)StatType.Hyg]++;
					Utile_Class.DebugLog_Value("위생 0 사망 후 DNA HygResurrection로 부활");
					return StageFailKind.None;
				}
			}
			return StageFailKind.Hyg;
		}
		if (STAGE_USERINFO.Is_UseStat(StatType.Sat) && STAGE_USERINFO.GetStat(StatType.Sat) < 1) {
			if (MAIN.IS_State(MainState.STAGE) && STAGE.m_RebornSrv[(int)StatType.Sat] < 1) {
				float dnaremen = 0f;
				for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
					float dna = STAGE.m_Chars[i].m_Info.GetDNABuff(OptionType.SatResurrection);
					if (dna > 0) STAGE.DNAAlarm(STAGE.m_Chars[i].m_Info, OptionType.SatResurrection);
					dnaremen += dna;
				}
				if (dnaremen > 0f) {
					STAGE.Call_AddStat_Action(STAGE.m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * dnaremen));
					STAGE.m_RebornSrv[(int)StatType.Sat]++;
					Utile_Class.DebugLog_Value("허기 0 사망 후 DNA SatResurrection 부활");
					return StageFailKind.None;
				}
			}
			return StageFailKind.Sat;
		}
		if (STAGE_USERINFO.Is_UseStat(StatType.HP) && STAGE_USERINFO.GetStat(StatType.HP) < 1) {
			if (MAIN.IS_State(MainState.STAGE) && STAGE.m_RebornSrv[(int)StatType.HP] < 1) {
				float dnaremen = 0f;
				for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
					float dna = STAGE.m_Chars[i].m_Info.GetDNABuff(OptionType.HpResurrection);
					if (dna > 0) STAGE.DNAAlarm(STAGE.m_Chars[i].m_Info, OptionType.HpResurrection);
					dnaremen += dna;
				}
				if (dnaremen > 0f) {
					STAGE.Call_AddStat_Action(STAGE.m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP)));
					STAGE.m_RebornSrv[(int)StatType.HP]++;
					Utile_Class.DebugLog_Value("체력 0 사망 후 DNA HpResurrection 부활");
					return StageFailKind.None;
				}
			}
			return StageFailKind.HP;
		}
		return m_FailCheck.m_State == StageCheckState.Success ? StageFailKind.FailType : StageFailKind.None;
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 스테이지 카드 등장 체크
	public Dictionary<int, StageCheckInfo> m_CardAppearCheck = new Dictionary<int, StageCheckInfo>();
	public Dictionary<int, StageCheckInfo> m_CardDisappearCheck = new Dictionary<int, StageCheckInfo>();
	public List<StageCheckInfo> m_CardAppearCheck_List = new List<StageCheckInfo>();
	public List<StageCheckInfo> m_CardDisappearCheck_List = new List<StageCheckInfo>();
	void CardAppearInit()
	{
		m_CardAppearCheck.Clear();
		m_CardDisappearCheck.Clear();
		m_CardAppearCheck_List.Clear();
		m_CardDisappearCheck_List.Clear();

		if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training) return;
		List<TStageCardTable> list = STAGEINFO.GetStageCardGroup();
		for (int i = 0; i < list.Count; i++)
		{
			TStageCardTable card = list[i];
			StageCheckInfo dissappearinfo = new StageCheckInfo(list[i].m_Idx);
			if (card.m_Disappear.m_Cnt > 0) {
				m_CardDisappearCheck_List.Add(dissappearinfo);
				m_CardDisappearCheck.Add(dissappearinfo.m_Idx, dissappearinfo);
			}
			StageCheckInfo appearinfo = new StageCheckInfo(list[i].m_Idx);
			if (card.m_Appear.m_Type != StageCardAppearType.None) {
				m_CardAppearCheck_List.Add(appearinfo);
				m_CardAppearCheck.Add(appearinfo.m_Idx, appearinfo);
			}
		}
	}
	/// <summary> 스테이지에 카드 등장 시점 체크 </summary>
	public void CheckCardAppear(StageCheckType checkType, int Value, int Cnt)
	{
		StageCardAppearType type = StageCardAppearType.None;
		switch (checkType)
		{
		case StageCheckType.CardUse: type = StageCardAppearType.CardUse; break;
		case StageCheckType.KillEnemy: type = StageCardAppearType.KillEnemy; break;
		case StageCheckType.ClearCondition: type = StageCardAppearType.ClearCondition; break;
		case StageCheckType.CreateCardLine: type = StageCardAppearType.Time; break;
		}

		if (type == StageCardAppearType.None) return;

		for(int i = m_CardAppearCheck_List.Count - 1; i > -1; i--)
		{
			StageCheckInfo check = m_CardAppearCheck_List[i];
			TStageCardTable card = TDATA.GetStageCardTable(check.m_Idx);
			TStageCardAppearInfo appear = card.m_Appear;
			if (check.m_State == StageCheckState.Success) continue;
			if (appear.m_Type != type) continue;
			if (appear.m_Value != Value) continue;

			int Max = appear.m_Cnt;

			check.m_Cnt += Cnt;

			if (check.m_Cnt >= Max)
			{
				check.m_Cnt = Max;
				check.m_State = StageCheckState.Success;
			}
		}
	}
	/// <summary> 스테이지에 카드 미등장 시점 체크 </summary>
	public void CheckCardDisappear(StageCheckType checkType, int Value, int Cnt) {
		StageCardAppearType type = StageCardAppearType.None;
		switch (checkType) {
			case StageCheckType.CardUse: type = StageCardAppearType.CardUse; break;
			case StageCheckType.KillEnemy: type = StageCardAppearType.KillEnemy; break;
			case StageCheckType.ClearCondition: type = StageCardAppearType.ClearCondition; break;
			case StageCheckType.CreateCardLine: type = StageCardAppearType.Time; break;
		}

		if (type == StageCardAppearType.None) return;

		for (int i = m_CardDisappearCheck_List.Count - 1; i > -1; i--) {
			StageCheckInfo check = m_CardDisappearCheck_List[i];
			TStageCardTable card = TDATA.GetStageCardTable(check.m_Idx);
			TStageCardAppearInfo disappear = card.m_Disappear;
			if (check.m_State == StageCheckState.Success) continue;
			if (disappear.m_Type != type) continue;
			if (disappear.m_Value != Value) continue;

			if (check.m_State == StageCheckState.Success) continue;

			int Max = disappear.m_Cnt;

			check.m_Cnt += Cnt;

			if (check.m_Cnt >= Max) {
				check.m_Cnt = Max;
				check.m_State = StageCheckState.Success;
			}
		}
	}
	public bool IS_AppearCard(int idx)
	{
		if (!m_CardAppearCheck.ContainsKey(idx) && !m_CardDisappearCheck.ContainsKey(idx)) return true;
		if (m_CardAppearCheck.ContainsKey(idx) && !m_CardDisappearCheck.ContainsKey(idx)) return m_CardAppearCheck[idx].m_State == StageCheckState.Success;
		if (!m_CardAppearCheck.ContainsKey(idx) && m_CardDisappearCheck.ContainsKey(idx)) return m_CardDisappearCheck[idx].m_State == StageCheckState.Play;
		return m_CardAppearCheck[idx].m_State == StageCheckState.Success && m_CardDisappearCheck[idx].m_State == StageCheckState.Play;
	}
	public bool IS_CreateCard(int idx) {
		if (!m_CardAppearCheck.ContainsKey(idx) && !m_CardDisappearCheck.ContainsKey(idx)) return true;
		if (m_CardAppearCheck.ContainsKey(idx) && !m_CardDisappearCheck.ContainsKey(idx)) return m_CardAppearCheck[idx].m_Cnt <= STAGE_USERINFO.m_TurnCnt;
		if (!m_CardAppearCheck.ContainsKey(idx) && m_CardDisappearCheck.ContainsKey(idx)) return m_CardDisappearCheck[idx].m_Cnt > STAGE_USERINFO.m_TurnCnt;
		return m_CardAppearCheck[idx].m_Cnt <= STAGE_USERINFO.m_TurnCnt && m_CardDisappearCheck[idx].m_Cnt > STAGE_USERINFO.m_TurnCnt;
	}
}
