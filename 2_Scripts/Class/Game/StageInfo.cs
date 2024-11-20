using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StagePlayType
{
	None = 0,
	Stage,
	OutContent,
	Event,

}

public class StageInfo : ClassMng
{
	public StagePlayType m_PlayType = StagePlayType.None;
	public StageModeType m_StageModeType = StageModeType.None;
	public StageContentType m_StageContentType = StageContentType.None;
	public DayOfWeek m_Week = DayOfWeek.Sunday;
	public int m_LV = 1;
	public int m_Pos = 0;
	public int m_Idx = 101;
	public StageResult m_Result = StageResult.None;
	public float m_RunTime;//진행된 시간
	public Sprite m_BG;
	public TStageTable m_TStage { get { return TDATA.GetStageTable(m_Idx, m_PlayType == StagePlayType.Stage ? USERINFO.GetDifficulty() : 0); } }

	/// <summary> 플레이 유저 </summary>
	public StageUser m_User = new StageUser();

	/// <summary> 진행체크 </summary>
	public StageCheck m_Check = new StageCheck();

	/// <summary> 조합 재료 </summary>
	public int[] m_Materials = new int[(int)StageMaterialType.None];

	/// <summary> 시작 스테이지의 플레이 코드 </summary>
	public string PlayCode;
	/// <summary> 이벤트 아이디 </summary>
	public long EUID;

	/// <summary> 다운타운 마지막 플레이 레벨 </summary>
	[JsonIgnore] public int m_LastPlayLv = -1;
	/// <summary> 다운타운 마지막 클리어 레벨 </summary>
	[JsonIgnore] public int m_LastLv = -1;

	public bool IS_ContentStg { get { return m_StageContentType == StageContentType.Stage || IS_ReplayStg; } }
	public bool IS_ReplayStg { get { return m_StageContentType == StageContentType.Replay || m_StageContentType == StageContentType.ReplayHard || m_StageContentType == StageContentType.ReplayNight; } }


	public void Init()
	{
		m_PlayType = StagePlayType.None;
		m_StageContentType = StageContentType.None; 
		m_StageModeType = StageModeType.None;
		m_Week = DayOfWeek.Sunday;
		m_Result = StageResult.None;
		m_BG = null;
		m_RunTime = 0;
	}

	public void PlayInit() {
		m_Result = StageResult.None;
		m_Check.Init();
		m_User.Init();
		Array.Clear(m_Materials, 0, m_Materials.Length);
		m_RunTime = 0;
	}
	public UserPickCharInfo GetClearUserPickInfo()
	{
		if (m_StageModeType == StageModeType.Training) return null;
		return USERINFO.GetClearUserPickInfo(m_StageContentType, m_Week, m_Pos, m_PlayType == StagePlayType.Stage || m_PlayType == StagePlayType.Event ? m_Idx : m_LV);
	}

	public void SetRunTime(float addtime)
	{
		m_RunTime += addtime;
	}

	public MainMenuType GetPreMenu()
	{
		MainMenuType premenu = MainMenuType.Stage;
		switch (STAGEINFO.m_PlayType)
		{
		case StagePlayType.OutContent: premenu = MainMenuType.Dungeon; break;
		}
		return premenu;
	}

	/// <summary> 2배속 사용 스테이지 모드인지 체크 </summary>
	public bool IsPlaySpeedUpMode()
	{
		switch (m_StageModeType)
		{
		case StageModeType.Training:
		case StageModeType.NoteBattle:
			return false;
		}
		return true;
	}

	public bool IsViewGuideQuest()
	{
		return (USERINFO.GetDifficulty() == 0 && m_Idx > 101) || USERINFO.GetDifficulty() > 0 || TUTO.IsEndTuto(TutoKind.Stage_101);
		//return (USERINFO.GetDifficulty() == 0 && m_Idx > 103) || USERINFO.GetDifficulty() > 0 || TUTO.IsEndTuto(TutoKind.Stage_103);
	}

	IEnumerator PassVIPOpenAction(Action BuyCB = null)
	{
		yield return USERINFO.PassVIPOpenAction();
		BuyCB?.Invoke();
	}

	public void StageReset(Action<int> result)
	{
		if(STAGEINFO.m_TStage.m_Energy > 0 && USERINFO.m_Energy.Cnt < STAGEINFO.m_TStage.m_Energy)
		{
			POPUP.StartLackPop(BaseValue.ENERGY_IDX);
			return;
		}
		//FireBase-Analytics
		if (m_PlayType != StagePlayType.Event) {
			STAGEINFO.StageStatisticsLog(StageFailKind.None, 7);
			StageFailAnalytics(StageFailKind.None, 7);
		}

		UserInfo.Stage stage = USERINFO.m_Stage[m_StageContentType];
#if NOT_USE_NET
		//에너지 소모
		USERINFO.GetShell(-STAGEINFO.m_TStage.m_Energy);
		USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, STAGEINFO.m_TStage.m_Energy);
		//스테이지는 시도 회수 증가
		switch (m_StageContentType)
		{
		case StageContentType.University:
		case StageContentType.Subway:
			stage.Idxs.Find(t => t.Week == STAGEINFO.m_Week && t.Pos == STAGEINFO.m_Pos).PlayCount++;
			break;
		default:
			stage.Idxs[0].PlayCount++;
			break;
		}
		Reset();
		result?.Invoke(EResultCode.SUCCESS);
#else
		UserInfo.StageIdx stageidx = stage.Idxs.Find(o => o.Week == m_Week && o.Pos == m_Pos);
		WEB.SEND_REQ_STAGE_START((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_STAGE((res2) => { result?.Invoke(res.result_code); });
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, m_TStage.m_Energy);
			Reset();
			result?.Invoke(res.result_code);
		}, stage.UID, m_Week, m_Pos, m_PlayType == StagePlayType.Stage || m_PlayType == StagePlayType.Event ? m_Idx : m_LV, stageidx.DeckPos, true, EUID);
#endif
		//stage_pause close가 먼저 진행된다음에 타임스케일 조절해야함
		Time.timeScale = 1f;
	}

	void Reset()
	{
		m_RunTime = 0;
		//리셋
		switch (m_StageModeType)
		{
			case StageModeType.NoteBattle:
				STAGEINFO.PlayInit();
				if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time) DLGTINFO?.f_RFModeTimer?.Invoke(STAGEINFO.m_TStage.m_Fail.m_Value);
				BATTLE.Init(EBattleMode.Normal, GetCreateEnemyIdx(m_StageContentType == StageContentType.Bank), GetCreateEnemyLV(0, false), 0, null, true);
				break;
			case StageModeType.Training:
				TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
				POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) => {
					// 메인 UI 변경
					POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
					PLAY.SetBGSND();
				}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
				break;
			default:
				if (MAIN.IS_State(MainState.BATTLE)) {
					//스테이지나 타워에서 전투중일 경우 배틀씬 꺼버리고 이전 씬 켜줘야할듯
					if (MAIN.IS_BackState(MainState.STAGE)) BATTLEINFO.m_BattleEndCB = STAGE.Restart;
					else if (MAIN.IS_BackState(MainState.TOWER)) BATTLEINFO.m_BattleEndCB = TOWER.Restart;
					if (MAIN.IS_State(MainState.STAGE)) STAGE?.m_MyCam.gameObject.SetActive(true);
					else if (MAIN.IS_BackState(MainState.TOWER)) TOWER?.m_MyCam.gameObject.SetActive(true);
						BATTLEINFO.GoBackMainState();
				}
				else {
					TOWER?.Restart();
					STAGE?.Restart();
				}
				break;
		}
	}

	public void SetStage(StagePlayType type, StageModeType modetype, int stageidx, int lv = 1, DayOfWeek week = DayOfWeek.Sunday, int pos = 0)
	{
		m_PlayType = type;
		m_StageModeType = modetype;
		TModeTable modetable = TDATA.GetModeTable(stageidx);
		m_StageContentType = modetable != null ? modetable.m_Content : StageContentType.Stage;
		m_Week = week;
		m_Idx = stageidx;
		m_Pos = pos;
		m_Result = StageResult.None;
		m_LastPlayLv = -1;
		if(type == StagePlayType.OutContent) {
			switch (m_StageContentType) {
				case StageContentType.Academy:
				case StageContentType.University:
					m_LastLv = USERINFO.m_Stage[m_StageContentType].Idxs.Find(t => t.Week == week && t.Pos == m_Pos).Idx;
					break;
				case StageContentType.Tower:
				case StageContentType.Cemetery:
				case StageContentType.Bank:
				case StageContentType.Factory:
					m_LastLv = USERINFO.m_Stage[m_StageContentType].Idxs[0].Idx;
					break;
				default:
					m_LastLv = -1;
					break;
			}
		}
		switch (type)
		{
		case StagePlayType.Stage: m_LV = stageidx % 100; break;
		default: m_LV = lv; break;
		}
		switch(modetype)
		{
		case StageModeType.Training: break;
		default: TDATA.LoadStageData(m_TStage.StageCardTableName); break;

		}
		if(modetype != StageModeType.Training) PlayInit();

		switch (m_StageModeType)
		{
		case StageModeType.Tower:
			m_BG = UTILE.LoadImg("BG/MapBG/Tower_BaseBG", "png");
			break;
		default:
			m_BG = m_TStage.GetStageBG();
			break;
		}

		USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, m_TStage.m_Energy);
		//FireBase-Analytics
		StartStageIdx();
	}
	public void SetStage_Replay(UserInfo.Stage _stg, UserInfo.StageIdx _stgidx) {
		m_PlayType = StagePlayType.Stage;
		m_StageContentType = _stg.Mode;
		m_Week = _stgidx.Week;
		m_Idx = _stgidx.Idx;
		m_Pos = _stgidx.Pos;
		m_Result = StageResult.None; 
		m_LV = m_Idx % 100;
		TStageTable tdata = TDATA.GetStageTable(m_Idx, USERINFO.GetDifficulty());
		m_StageModeType = tdata.m_Mode;
		switch (tdata.m_Mode) {
			case StageModeType.Training: break;
			default: TDATA.LoadStageData(m_TStage.StageCardTableName); break;

		}
		if (tdata.m_Mode != StageModeType.Training) PlayInit();

		switch (m_StageModeType) {
			case StageModeType.Tower:
				m_BG = UTILE.LoadImg("BG/MapBG/Tower_BaseBG", "png");
				break;
			default:
				m_BG = m_TStage.GetStageBG();
				break;
		}

		USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, m_TStage.m_Energy);
		//FireBase-Analytics
		StartStageIdx();
	}
	public StageContentType GetContentType()
	{
		return m_StageContentType;
		//return m_PlayType == StagePlayType.Stage ? StageContentType.Stage : m_StageContentType;
	}
	public UserInfo.StageIdx GetStageIdxInfo()
	{
		return USERINFO.m_Stage[m_StageContentType].Idxs.Find(o => o.Week == m_Week && o.Pos == m_Pos);
	}
	public UserInfo.Stage GetUserStageInfo()
	{
		return USERINFO.m_Stage[m_StageContentType];
	}

	public void StageClear(Action<LS_Web.RES_STAGE_CLEAR> cb)
	{
		int idx = m_PlayType == StagePlayType.Stage || m_PlayType == StagePlayType.Event ? m_Idx : m_LV;
		UserInfo.StageIdx stage = USERINFO.m_Stage[m_StageContentType].Idxs.Find(o => o.Week == m_Week && o.Pos == m_Pos);

		if (m_PlayType != StagePlayType.Event) {
			//FireBase-Analytics
			STAGEINFO.StageStatisticsLog(StageFailKind.None, 0);
		}
		WEB.SEND_REQ_STAGE_CLEAR((res) =>
		{
			if (res.IsSuccess())
			{
				PlayCode = "";
			}
			cb?.Invoke(res);
		}, USERINFO.m_Stage[m_StageContentType], m_Week, m_Pos, idx, PlayCode, STAGE_USERINFO.m_TurnCnt, Mathf.FloorToInt(m_RunTime), EUID, stage.DeckPos);
	}


	public MainState GetModeTypeMainState()
	{
		switch (m_StageModeType)
		{
		case StageModeType.NoteBattle:
			return MainState.BATTLE;
		case StageModeType.Tower:
			return MainState.TOWER;
		}
		return MainState.STAGE;
	}

	public List<TStageCardTable> GetMaterialCardIdxs(StageMaterialType type = StageMaterialType.None)
	{
		if (type == StageMaterialType.None) return TDATA.GetStageCardGroup(m_StageContentType == StageContentType.Subway ? 1 : m_LV).FindAll(t => (t.GetProb() > 0 || t.m_DarkProb > 0) && t.m_Type == StageCardType.Material);
		else return TDATA.GetStageCardGroup(m_StageContentType == StageContentType.Subway ? 1 : m_LV).FindAll(t => (t.GetProb() > 0 || t.m_DarkProb > 0) && t.m_Type == StageCardType.Material && t.m_Value1 == (int)type);
	}

	public List<TStageCardTable> GetStageCardGroup(bool isProbCheck = false)
	{
		if (isProbCheck) return TDATA.GetStageCardGroup(m_StageContentType == StageContentType.Subway ? 1 : m_LV).FindAll(t => t.GetProb() > 0 || t.m_DarkProb > 0);
		else return TDATA.GetStageCardGroup(m_StageContentType == StageContentType.Subway ? 1 : m_LV);
	}

	public int GetDefaultCardIdx(StageCardType _type) {
		return TDATA.GetStageCardGroup(0).Find((t) => t.m_Type == _type).m_Idx;
	}
	TStageCardTable GetPickCard(Predicate<TStageCardTable> check)
	{
		List<TStageCardTable> list = GetStageCardGroup().FindAll(check);

		if (list.Count < 0) return null;

		// 등장조건에 
		int TotProb = 0;
		for (int i = list.Count - 1; i > -1; i--) TotProb += list[i].GetProb();
		int Rand = UTILE.Get_Random(0, TotProb);

		for (int i = list.Count - 1; i > -1; i--)
		{
			if (Rand < list[i].GetProb()) return list[i];
			Rand -= list[i].GetProb();

		}
		return null;
	}
	//순차로 뽑을 경우
	TStageCardTable GetPickCardOrder(Predicate<TStageCardTable> _check, int _cnt) {
		List<TStageCardTable> list = GetStageCardGroup().FindAll(_check);
		if (list.Count < 0) return null;

		for(int i = 0; i < _cnt; i++) {
			list.RemoveAt(0);
		}
		if (list.Count < 0) return null;

		return list[0];
	}

	bool CheckUseMeleeStageCard(TStageCardTable card)
	{
		if (card.GetProb() < 1) return false;
		if (!m_Check.IS_AppearCard(card.m_Idx)) return false;
		return true;
	}

	public int GetCreateEnemyIdx(bool _order = false, int _cnt = 0)
	{
		TStageCardTable cardtable = null;
		if(_order) cardtable = GetPickCardOrder(CheckUseMeleeStageCard, _cnt);
		else cardtable = GetPickCard(CheckUseMeleeStageCard);

		if (cardtable == null) return 0;
		return Mathf.RoundToInt(cardtable.m_Value1);
	}
	/// <summary> 스테이지에 카드가 보스 1개뿐이어야함</summary>
	public TEnemyTable IS_BossStage()
	{
		List<TStageCardTable> list = GetStageCardGroup().FindAll(CheckUseMeleeStageCard);
		if (list == null) return null;
		TEnemyTable table = TDATA.GetEnemyTable(Mathf.RoundToInt(list[0].m_Value1));
		if (table.m_Grade == EEnemyGrade.Boss) return table;
		else return null;
	}
	public int GetCreateEnemyLV(int AddLV = 0, bool USERange = true)
	{
		int lv = STAGEINFO.m_StageContentType == StageContentType.Subway ? USERINFO.m_LV : STAGEINFO.m_TStage.m_LV;
		int gap = USERange ? STAGEINFO.m_TStage.m_EnemyLevelRange : 0;
		int mlv = (int)TDATA.GetConfig_Long(ConfigType.MaxLevel);

		return Mathf.Clamp(lv + AddLV + UTILE.Get_Random(-gap, gap), 1, mlv);
	}

	public void InsertMaterial(StageMaterialType type, int cnt)
	{
		if (type == StageMaterialType.None) return;
		m_Materials[(int)type] += cnt;
	}
	/// <summary> 총 보유중인 재료 개수 </summary>
	public int GetHadMatCnt()
	{
		int cnt = 0;
		for (int i = 0; i < m_Materials.Length; i++)
		{
			cnt += m_Materials[i];
		}
		return cnt;
	}
	public bool IsStageCardMakingCondition(StageMakingConditionType type, int value)
	{
		if (m_StageContentType == StageContentType.University) return true;
		else {
			switch (type) {
				case StageMakingConditionType.Character:
					DeckInfo deck = USERINFO.m_PlayDeck;
					for (int i = 0; i < 5; i++) {
						CharInfo info = USERINFO.GetChar(deck.m_Char[i]);
						if (info == null) continue;
						if (info.m_Idx == value) return true;
					}
					return false;
				case StageMakingConditionType.Research:
					// TODO : 연구가 셋팅되면 추가할 것
					break;
				case StageMakingConditionType.Stage:
					return USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx >= value;
			}
		}
		return true;
	}

	// 제작 가능한 종류 개수
	public List<TStageMakingTable> MakeList(StageMaterialType type = StageMaterialType.None)
	{
		List<TStageMakingTable> re = new List<TStageMakingTable>();
		List<TStageMakingTable> list = TDATA.GetStageMakingList(type);

		for (int i = 0; i < list.Count; i++)
		{
			TStageMakingTable makinginfo = list[i];
			if (!IsStageCardMakingCondition(makinginfo.m_Condition.m_Type, makinginfo.m_Condition.m_Value - 1)) continue;
			bool isMake = true;
			for (int j = makinginfo.m_Materal.Count - 1; j > -1; j--)
			{
				TStageMakingTable.MakeMaterial mat = makinginfo.m_Materal[j];
				if (m_Materials[(int)mat.m_Type] < mat.m_Cnt)
				{
					isMake = false;
					break;
				}
			}

			if (isMake) re.Add(makinginfo);
		}
		return re;
	}
	/// <summary> 스테이지 결과 로그 </summary>
	/// <param name="_stgidx"> 플레이한 스테이지 인덱스 </param>
	/// <param name="_result"> 0:승리, 1:실패 </param>
	/// <param name="_diffculty">0:노멀, 1:나이트메어, 2:아포칼립스 </param>
	/// <param name="_mode">0:스테이지, 1:노트전투, 2:트레이닝, 3:타워 </param>
	/// <param name="_combatpower"> 플레이덱 전투력 </param>
	/// <param name="_turn"> 진행 턴 수 </param>
	/// <param name="_playtime"> 타임어택 진행 시간 </param>
	/// <param name="_failreason"> 실패 원인 0:성공, 1:턴0, 2:시간0, 3:HP0, 4:포만감0, 5:정신0, 6:위생0, 7:메뉴에서 스테이지 재시작, 8:메뉴에서 메인으로 복귀, 9:전투제한, 10:다른 임무 남았을때 </param>
	/// <param name="_playcnt"> 도전 횟수 </param>
	public void StageStatisticsLog(StageFailKind _failkind, int _failreason = 0) {
#if NOT_USE_NET || UNITY_EDITOR
		return;
#else
		int CombatPower = 0;
		for (int i = 0; i < USERINFO.m_PlayDeck.m_Char.Length; i++) {
			if (USERINFO.m_PlayDeck.m_Char[i] != 0) {
				CombatPower += USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]).GetCombatPower();
			}
		}
		int Diffculty = 0;
		if(m_StageContentType == StageContentType.Stage) Diffculty = USERINFO.GetDifficulty();

		int PlayCount = 0;
		UserInfo.Stage stage = USERINFO.m_Stage[m_StageContentType];
		switch (m_StageContentType) {
			case StageContentType.University:
				PlayCount = stage.Idxs.Find(t => t.Week == STAGEINFO.m_Week && t.Pos == STAGEINFO.m_Pos).PlayCount;
				break;
			default:
				PlayCount = stage.Idxs[0].PlayCount;
				break;
		}
		int Time = 0;
		switch (m_StageModeType) {
			case StageModeType.Stage: Time = Mathf.FloorToInt(STAGE.m_Timer); break;
			case StageModeType.NoteBattle: Time = Mathf.FloorToInt(BATTLE.m_Timer); break;
		}

		int FailReason = 0;
		if (_failkind == StageFailKind.None) FailReason = _failreason;
		else {
			switch (_failkind) {
				case StageFailKind.Turn: FailReason = 1; break;
				case StageFailKind.Time: FailReason = 2; break;
				case StageFailKind.HP: FailReason = 3; break;
				case StageFailKind.Sat: FailReason = 4; break;
				case StageFailKind.Men: FailReason = 5; break;
				case StageFailKind.Hyg: FailReason = 6; break;
				case StageFailKind.TurmoilCount: FailReason = 9; break;
				case StageFailKind.OtherMission: FailReason = 10; break;
			}
		}

		//Parameter[] pramas = {
		//	new Parameter("StageIdx", m_Idx),
		//	new Parameter("StageResult", (int)m_Result),
		//	new Parameter("Difficulty", Diffculty),
		//	new Parameter("Mode", (int)m_StageModeType),
		//	new Parameter("CombatPower", CombatPower),
		//	new Parameter("Turn", STAGE_USERINFO.m_TurnCnt),
		//	new Parameter("Time", Time),
		//	new Parameter("FailReason", FailReason),
		//	new Parameter("CumulativeFailCount", PlayCount)
		//};
		//FIREBASE.LogEvent("StageStatistics", pramas);
#endif
	}

	public void StartStageIdx() {
#if NOT_USE_NET || UNITY_EDITOR
		return;
#else
		int Diffculty = 0;
		if (m_StageContentType == StageContentType.Stage) Diffculty = USERINFO.GetDifficulty();

		//Parameter[] pramas = {
		//	new Parameter("StageIdx", m_Idx),
		//	new Parameter("Difficulty", Diffculty)
		//};
		//FIREBASE.LogEvent("StartStageIndex", pramas);
#endif
	}

	public void StageFailAnalytics(StageFailKind _failkind, int _failreason = 0) {
#if !NOT_USE_NET
		int FailReason = 0;
		if (_failkind == StageFailKind.None) FailReason = _failreason;
		else {
			switch (_failkind) {
				case StageFailKind.Turn: FailReason = 1; break;
				case StageFailKind.Time: FailReason = 2; break;
				case StageFailKind.HP: FailReason = 3; break;
				case StageFailKind.Sat: FailReason = 4; break;
				case StageFailKind.Men: FailReason = 5; break;
				case StageFailKind.Hyg: FailReason = 6; break;
				case StageFailKind.TurmoilCount: FailReason = 9; break;
				case StageFailKind.OtherMission: FailReason = 10; break;
			}
		}

		WEB.SEND_REQ_STAGE_FAIL(USERINFO.m_Stage[m_StageContentType], m_Week, m_Pos, m_PlayType == StagePlayType.Stage ? m_Idx : m_LV, PlayCode, FailReason, STAGE_USERINFO.m_TurnCnt, Mathf.FloorToInt(STAGEINFO.m_RunTime));
#endif
	}
}
