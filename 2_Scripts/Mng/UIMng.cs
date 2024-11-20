using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static LS_Web;

public enum PopupName
{
	NONE = -1,					// 없는 상태
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Title
	Title,						// Title Main UI
	EarPhone,					// 타이틀에 이어폰 화면
	Intro,						// 인트로 동영상
	Agree,						// 제작필요
	Login,						// 제작필요
	TitleMenu,					// 제작필요
	Down_CDN,					// CDN 다운로드
	DataLoad,					// 툴데이터 로딩
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Play
	Play,						// Play Main UI
	Alternative,				// 보상 양자택일
	CharDraw,					// 캐릭터 뽑기
	OpenItems,					// 상자 아이템 열기
	Inventory,					// 인벤토리
	Info_Char,					// 캐릭터 정보창
	Info_Char_NotGet,			// 캐릭터 정보창(미획득)
	Info_Char_Detail,           // 캐릭터 상세 정보창
	LvBonusList,				// 캐릭터 레벨합 보너스 스탯 정보창
	Info_Skill,					// 스킬 정보창
	Info_Item_Equip,			// 장비 아이템 정보
	Info_Item_ETC,				// 기타 아이템 정보
	EquipLevelUp,				// 장비 레벨업
	EquipChange,				// 장비 변경
	EquipGradeUp,				// 장비 등급
	EquipReassembly,            // 장비 재조립
	EquipReassembly_List,       // 장비 재조립 확률표
	EquipSelect,				// 장비 선택(승급, 재조립)
	OptionChange,				// 장비 옵션 변경
	OptionChange_List,			// 장비 옵션 해제 확률
	Filter_EquipLevelUp,		// 장비 레벨업 필터
	DL_Talk,					// 다이얼로그 - 토크
	Raid,						// 습격 팝업
	RewardAssetAni,				// 보상 공통(금니, 달러, 캐릭터 경험치 연출)
	RewardList,					// 보상 리스트
	RewardList_Equip,			// 제작에서 장비보상일때
	ChapterClearReward,			// 챕터 보상
	CharUpgrade,				// 캐릭터 등급업
	CharUpgrade_Result,			// 캐릭터 등급업 결과
	SerumGuide,					// 캐릭터 혈청 가이드
	Character_Evaluation,		// 캐릭터 평가
	PersonnelFileList,			// 변환할 인사파일 목록
	PersonnelFileChange,		// 인사파일 변환
	DrawShop,					// 뽑기 상점
	Synergy_Applied,			// 덱의 시너지
	Synergy_All,				// 시너지 도감
	Info_Dungeon,				// 던전 정보창
	DeckSetting,				// 덱 세팅
	SortingOption,				// 덱세팅 정렬 옵션
	Dungeon_Dollar,				// 달러 던전
	Dungeon_Exp,				// 경험치 던전
	Dungeon_University,         // 일일 던전
	Dungeon_University_Detail,  // 일일 던전 상세창
	Dungeon_University_CharpieceList,//일일 던전 인사파일 목록
	//Dungeon_Detail,				// 일일 던전 상세창
	Dungeon_Tower,				// 타워 던전
	Dungeon_Training,			// 아카데미 던전(트레이닝)
	Dungeon_Training_Detail,	// 아카데미 던전 상세창
	Dungeon_Bank,				// 던전-은행
	Dungeon_Subway,				// 던전-지하철
	Dungeon_Factory,			// 던전-공장
	Dungeon_Cemetery,           // 던전-묘지
	DeckSetting_Adventure,      // 탐험 덱세팅
	Item_ToolTip,               // 아이템 툴팁
	BuyGold,					// 골드 무,유료 구분 및 구매 팝업
	Info_Item_RewardBox,		// 박스형 아이템 툴팁
	PostList,					// 아이템 툴팁
	Tower_RefugeeReward,		// 타워 피난민 선택창
	TowerRandEvent,				// 타워 랜덤 이벤트
	Tower_Single,               // 타워 생존스탯 카드
	DNAMaking,					// DNA 생성
	DNAMerge_List,				// DNA 조합 확률
	DNASelect,                  // DNA 교체(선택)창
	DNADecomposition,			// DNA 분해
	DNALvUp,                    // DNA 레벨업
	DNAOptionChange,            // DNA 옵션 변경
	DNAOptionChange_List,		// DNA 옵션 확률표
	DNAList,					// DNA 모든 리스트 정보
	Help_Info_Char_DNASet,		// DNA 세트효과 정보
	Info_DNA,					// DNA 상세 정보
	ZombieDecomposition,		// 좀비 분해
	AddEvent,					// 추가 이벤트
	Challenge_EndReward,		// 챌린지 결과 보상
	Challenge_EndReward_Week,	// 챌린지 결과 보상
	Challenge_Start,            // 챌린지 정보
	Challenge_Start_Week,		// 주간 챌린지 시작
	Challenge_Main,				// 진행중 챌린지
	Challenge_Next,				// 다음 챌린지 정보
	Challenge_Detail,			// 챌린지 상세 정보
	Challenge_RewardList,		// 챌린지 보상목록
	Dungeon_Pass_Use,			// 소탕권 사용
	Dungeon_Pass_Result,		// 소탕권 결과
	Serum,						// 혈청
	Serum_Info,					// 혈청 정보
	Serum_Statistic,			// 혈청 통계
	TouchBlock,					// 화면 가림용
	Option,						// 옵션창
	GrowthWay,					// 강해지는법 유도
	GrowthWay_Warning,          // 강해지는법 유도(스테이지 진입)
	FAEvent,					// 게임 이벤트 UI(신규출첵, 일반 출첵)
	Announcement,				// 공지 사항
	Store_SupplyBoxOpen,		// 상점 보급 상자 팝업
	Store_Box_Preview,			// 상점 보급 상자 목록
	Store_PurchaseConfirm,      // 상점 구입 팝업
	Store_GatchaMileage,        // 마일리지 상점
	Store_Select,	// 마일리지 캐릭터 선택
	Store_EquipGacha_Bonus,		// 피스메이커 혜택
	Gacha_Pickup,				// 가챠 캐릭터 픽업
	Gacha_RewardList,           // 가차 확률표
	Group_RewardList,			// 아이콘 그룹으로 된거 보상 목록
	UserProfile,				// 유저 프로필 팝업
	Guide_BattleNote,           // 노트 전투 가이드 팝업
	Guide_Tower,				// 타워 가이드 팝업
	Guide_Training,				// 트레이닝 가이드 팝업
	Tuto_Video,					// 튜토리얼 비디오
	UserReview,					// 유저 평가
	Friend,						// 친구
	Info_User,					// 유저 정보(간략한 정보)
	Store_RefreshConfirm,       // 상점(블랙마캣) 갱신 팝업
	NewNDailyMission,			// 일퀘 및 초보자 미션
	Auction_Bid,				// 입찰
	Auction_BidNow,				// 즉시 입찰
	Store_PassBuy,				// 패스 구매
	Store_PassLvUp,				// 패스 레벨업 구매
	Store_PassLvUp_Single,		// 패스 레벨업 단일 구매
	Store_EquipGachaLvUp,       // 상점 장비 레벨업 
	Store_EquipGachaResult,		// 상점 장비 뽑기 결과창
	Store_Recommend,			// 추천 상품
	Store_Purchase_Monthly,		// 월정액 상품
	SupplyBox_LvUp,				// 보급상자 레벨업
	UserExpGet,					// 유저 경험치 획득
	Credit,						// 크레딧
	COMPnPDSignature,			// 회사 로고 및 피디 시그니쳐
	StgReplay,					// 리플레이
	HubInfo,					// 허브 인포
	Event_Welcome,				// 오픈이벤트
	Event_1,					// 월 이벤트
	Event_2,					// 월 이벤트
	Event_3,					// 월 이벤트
	Event_4,					// 월 이벤트
	Event_5,					// 월 이벤트
	Event_6,					// 월 이벤트
	Event_7,					// 월 이벤트
	Event_8,					// 월 이벤트
	Event_9,					// 월 이벤트
	Event_10,					// 월 이벤트
	Event_10_Gift_Purchase,		// 10월 이벤트 사탕 구매
	Event_10_Gift_RewardList,	// 10월 이벤트 보상 목록
	Help_Info_Event_10,			// 10월 이벤트 정보
	Event_10_RecomSrv,			// 10월 이벤트 추천 캐릭터
	Event_11,					// 월 이벤트
	Event_12,					// 월 이벤트
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// PVP
	Union_JoinList,				// 연합 리스트
	Union_JoinInfo,				// 연합 정보
	Union_NewUnion,				// 연합 생성
	Union_Main,					// 연합 메인
	Union_Research,				// 연합 연구
	Union_Research_List,		// 연합 연구 리스트
	Union_Research_EffectList,	// 연합 연구 효과
	Union_Research_Donation,	// 연합 연구 진행
	Union_Mng,					// 연합 설정
	Union_NoticeEdit,			// 연합 공지 변경
	Union_Option_MarkEdit,		// 연합 마크 설정
	Union_Option_NameChange,	// 연합 이름 설정
	Union_Option_UnionDescEdit,	// 연합 인삿말 설정
	Union_Option_JoinLevelEdit,	// 연합 가입 레벨 설정
	Union_Option_JoinTypeEdit,	// 연합 가입 타입 설정
	Union_Option_Disband,		// 연합 해산
	SortingOption_Union_Member,	// 연합 맴버 정렬 옵션
	Union_Store,				// 연합 상점
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// PVP
	PVP,						// pvp Main UI
	PVP_Pause,					// pvp 일시정지
	PVP_Main,					// pvp 매칭 화면
	PVP_DeckSetting,			// pvp 덱세팅
	PVP_Result_Point,			// pvp 결과-점수
	PVP_Result_League,			// pvp 결과-랭크
	PVP_End_League,				// pvp 리그 종료
	PVP_End_Season,				// pvp 시즌 종료
	PVP_Info_OtherUser,			// pvp 다른 유저 정보
	PVP_Rank,					// pvp 랭킹
	PVP_Rank_League_RewardList, // pvp 리그 보상정보
	PVP_Rank_Season_RewardList, // pvp 시즌 보상정보
	PVP_StartReward,			// pvp 시작 보상
	PVP_FailCause_SR,			// pvp 실패 사유 스프라이트 랜더
	PVP_FailCause_CV,			// pvp 실패 사유 스프라이트 캔버스
	PVP_Camp,					// pvp 기지
	PVP_Storage,                // pvp 창고
	PVP_Storage_Lack,			// pvp 자원 부족 팝업
	PVP_Resource,				// pvp 자원 생산
	PVP_Research,               // pvp 연구
	PVP_Research_Info,          // pvp 연구 정보
	PVP_Research_EffectList,	// pvp 연구 적용 효과 목록
	PVP_Base_Upgrade,           // pvp 기지 업그레이드
	PVP_Result_Raid,            // pvp 승리시 약탈 정보
	PVP_DefenseList,			// pvp 방어 리스트
	PVP_Revenge,                // pvp 복수
	Msg_PVP_Research_LvUp,		// pvp 연구 레벨업
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage
	Stage,						// Stage Main UI
	Stage_Pause,				// 스테이지 일시정지
	Stage_Mission,				// 스테이지 시작 팝업
	Stage_Result,				// 스테이지 성공 및 보상
	StageFailed,				// 스테이지 실패
	Stage_FailCause,			// 스테이지 패배 원인
	Stage_Info,                 // 덱세팅에서 스테이지 정보
	Stage_Info_Card,            // 스테이지 특정 카드 정보
	Stage_CardList,				// 스테이지 기믹 가이드
	BuffList,					// 스테이지 누적 버프창
	Stage_Making,				// 스테이지 제작창
	Stage_Making_New,			// 스테이지 제작 가능 알림
	Stage_MakingInfo,			// 스테이지 제작 도안
	Stage_StartReward,			// 스테이지 사전선택 보상
	Stage_Reward,				// 스테이지 보상(전투)
	Stage_Gamble,               // 스테이지 보상의 도박 카드 연출
	Stage_Skill_Judith,         // 스테이지 유틸 아이템 받는 연출
	Stage_CardUse,              // 캐릭터 스킬 사용 설명업
	Stage_Continue,				// 스테이지 이어하기
	Stage_Continue_Use,			// 스테이지 이어하기 연출
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Training
	Training,					// 훈련 스테이지
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Battle
	BattleUI,					// 대전 메인 UI
	PowerBattle,				// 힘겨루기
	BattleReward,				// 대전 보상
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MsgBox
	Msg_None,					// 버튼 없는 박스( 해당 박스가 나오면 닫힐때까진 아무것도 하지못함. 백버튼 반응 : 종료 팝업)
	Msg_OK,						// OK 박스
	Msg_YN,						// Yes, No 박스
	Msg_WebView,				// 웹뷰 박스
	Msg_CenterAlarm,            // 센터 알람
	Msg_Store_Ticket_Buy,		// 뽑기 전용 
	Msg_YN_Cost,				// 골드 사용 YN 박스
	Msg_Inven_Buy,				// 가방 확장
	Msg_CommingSoon,			// 미구현
	Msg_CDN,					// CDN 확인
	Msg_Challenge_Alarm,        // 챌린지 랭킹 변동 알림
	Msg_Store_GatchaMileage_Alarm, //뽑기 마일리지 알람
	Msg_RewardGet_Center,       // 이벤트 보상 연출
	Msg_RewardGet_Monthly,      // 월정액 상품 획득
	Msg_Auction_Guide,			// 경매장 가이드
	Msg_Store_VIPOpen,			// 시즌패스 오픈 연출
	Msg_Store_2XActive,			// 2배속 활성 연출
	Msg_StageWarning,			// 전투력 격차 경고
	Msg_Guide,					// 가이드 설명
	Msg_Timer,					// 시간
	Msg_NewUnion_Result,		// 길드 생성 성공
	Msg_Union_Attendance_Result,	// 길드 출석 보상
	Msg_Union_LvUpAlarm,		// 길드 레벨 달성
	Msg_YN_YRed,				// 확인 버튼 빨간색으로 (parms에 버튼 문구 셋팅 0 : 취소, 1: 확인);
	Msg_YN_BtnControl,			// 버튼 색상 변경 가능한 버전 (parms에 Msg_YN_BtnControl.BtnInfo 셋팅 0 : 취소, 1: 확인);
	Msg_Union_Research_Donation_Confirm,			// 연구 참여 확인 팝업
	Msg_Union_Research_Finish,  // 연구 완료
	Msg_YN_Cost_BtnControl,		// 골드 사용 YN 박스 버튼 색상 변경 가능한 버전 (parms에 Msg_YN_BtnControl.BtnInfo 셋팅 0 : 취소, 1: 확인);
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Tuto
	TutoUI,						// 튜토리얼
	Tuto_Start,					// 튜토리얼 시작 연출(아웃 컨텐츠용)

#if UNITY_EDITOR && NOT_USE_NET && USE_CHEAT
	CheatPage,					// 치트 페이지
#endif
#if VIEW_STAGE_STAT
	ViewStageUserStat,
#endif
}

public enum PopupPos
{
	EXIT = -1
	, MAINUI = 0			// 메인 UI
	, BATTLE				// 베틀 메인 UI
	, POPUPUI				// 팝업 UI
	, MSGBOX				// 메세지박스 UI
	, TOOLTIP				// 툴팁 UI
	, TUTO					// 튜토리얼 UI
	, NONE					// 일반적으로 그냥 프리팹 연결로 사용한것 상위가 있어 Close호출되어도 제거가 되지 않도록하기위해 사용 ex> 대전 PowerBattle 같은경우 BattleUI안에서 컨트롤함
}

public class UIMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Canvas_Controller
	Canvas_Controller CC;
	public Canvas_Controller GetCC { get { return CC; } }
	public void SetCanvasControll() {
		CC.Set();
	}

	public void SetMainCameraRect() {
		CC.SetMainCameraRect();
	}
	public Rect GetCameraRect() {
		return CC.GetCameraRect();
	}
	public float GetRatioH() {
		return (float)Screen.height / GetComponent<RectTransform>().rect.height;
	}
	public float GetCanvasH() {
		return GetComponent<RectTransform>().rect.height;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Values
#pragma warning disable 0649
	[SerializeField] TextMeshProUGUI m_ScreenLog;
	[SerializeField] EventSystem m_EventSystem;
	public bool IsUISelect { get { return EventSystem.current.currentSelectedGameObject != null; } }
	public bool IsUseEventSytem { get { return m_EventSystem.enabled; } }
#pragma warning restore 0649

	/// <summary> 메시지박스 풀용 데이터 </summary>
	class MsgBoxData{
		public PopupName m_Name;
		public Action<int, GameObject> m_CB = null;
		public object[] m_Args;
	}
	List<MsgBoxData> m_MsgBoxPool = new List<MsgBoxData>();
	MsgBoxBase m_NowMsgBoxPool;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Start
	private void Awake() {
		TW_LoadingFade(0f);
#if VIEW_FPS || VIEW_VER || VIEW_OTHER
		m_ScreenLog.gameObject.SetActive(true);
#else
		m_ScreenLog.gameObject.SetActive(false);
#endif
		CC = GetComponent<Canvas_Controller>();
		DontDestroyOnLoad(gameObject);  //씬 전환시 안날라가게...

		SetConnecting(false);
#if VIEW_FPS
		StartCoroutine("worstReset");
#endif
#if !UNITY_EDITOR
		for (int i = 0; i < CheatBtns.Length; i++) CheatBtns[i].SetActive(false);
#else
		for (int i = 0; i < CheatBtns.Length; i++) CheatBtns[i].SetActive(true);
#endif
		//ChangeZtestMode(UnityEngine.Rendering.CompareFunction.Always);
	}
#if VIEW_FPS || VIEW_VER
	float deltaTime = 0f;
	float fps, msec;
	float worstFps = 100f;

	void CalcFPS() {
		msec = deltaTime * 1000.0f;
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
		fps = 1.0f / deltaTime;  //초당 프레임 - 1초에
								 //새로운 최저 fps가 나왔다면 worstFps 바꿔줌.
		if (fps < worstFps) worstFps = fps;
	}
	IEnumerator worstReset() //코루틴으로 15초 간격으로 최저 프레임 리셋해줌.
	{
		while (true) {
			yield return new WaitForSeconds(15f);
			worstFps = 100f;
		}
	}
#endif
	private void Update() {
#if VIEW_FPS || VIEW_VER || VIEW_OTHER
		StringBuilder formater = new StringBuilder(1024);
#if QA_BUILD
		formater.Append("[QA] ");
#endif
#if VIEW_VER
		formater.Append($"{APPINFO.m_strVersion}\n");
#endif
#if VIEW_FPS
		formater.Append($"Fps : {msec.ToString("F1")}ms ({fps.ToString("F1")} fps) \nWorst : {worstFps.ToString("F1")} fps\nvSync : {QualitySettings.vSyncCount}\nFrameRate : {Application.targetFrameRate}\n");
		CalcFPS();
#endif
#if VIEW_OTHER
		formater.Append(Utile_Class.GetTimeLog());
#endif
		m_ScreenLog.text = formater.ToString();
#endif

		//팝업풀
		if(m_MsgBoxPool.Count > 0 && m_NowMsgBoxPool == null) {
			MsgBoxData data = new MsgBoxData() {
				m_Name = m_MsgBoxPool[0].m_Name,
				m_CB = m_MsgBoxPool[0].m_CB,
				m_Args = m_MsgBoxPool[0].m_Args
			};
			m_MsgBoxPool.RemoveAt(0);
			m_NowMsgBoxPool = (MsgBoxBase)Set_Popup(PopupPos.MSGBOX, data.m_Name, data.m_CB, data.m_Args);
			m_NowMsgBoxPool.SetMsg("", "");
		}
	}

	public bool OnBack() {

		if (m_ExitPopup) {
			m_ExitPopup.OnNO();
			return true;
		}

		if(TUTO.IsTutoPlay()) return false;

		if (m_MsgList.Count > 0) {
			for(int i = m_MsgList.Count - 1; i > -1; i--)
			{
				MsgBoxBase msg = m_MsgList[i].GetComponent<MsgBoxBase>();
				switch (msg.m_Popup)
				{
				case PopupName.Msg_None: return false;
					// 알림 메세지는 자동 닫힘이므로 무시
				case PopupName.Msg_CenterAlarm:
				case PopupName.Msg_Challenge_Alarm:
				case PopupName.Msg_Store_GatchaMileage_Alarm:
					continue;
				}
				msg.OnNO();
				return true;
			}
		}

		if (m_PopupList.Count > 0) {
			PopupBase popup = m_PopupList[m_PopupList.Count - 1];
			switch (popup.m_Popup) {
				case PopupName.Agree:
				case PopupName.Login:
				case PopupName.EarPhone:
				case PopupName.TitleMenu:
				case PopupName.RewardAssetAni:
				case PopupName.Stage_Mission:
				case PopupName.Stage_Result:
				case PopupName.Stage_Reward:
				case PopupName.Stage_StartReward:
				case PopupName.Stage_Gamble:
				case PopupName.Stage_Skill_Judith:
				case PopupName.Stage_Continue:
				case PopupName.Stage_Continue_Use:
				case PopupName.AddEvent:
				case PopupName.DL_Talk:
				case PopupName.Stage_CardUse:
				case PopupName.Raid:
				case PopupName.RewardList:
				case PopupName.CharUpgrade:
				case PopupName.CharUpgrade_Result:
				case PopupName.DrawShop:
				case PopupName.OpenItems:
				case PopupName.TowerRandEvent:
				case PopupName.Tower_RefugeeReward:
				case PopupName.Tower_Single:
				case PopupName.Store_SupplyBoxOpen:
				case PopupName.UserProfile:
				case PopupName.Down_CDN:
				case PopupName.DataLoad:
				case PopupName.Tuto_Video:
					return false;
			}
			popup.Close();
			return true;
		}
		if (m_ToolTipList.Count > 0) {
			PopupBase popup = m_ToolTipList[m_ToolTipList.Count - 1];
			popup.Close();
			return true;
		}
		return false;
	}

	public void Init()
	{
		Init_MainUI();
		Init_BattleMainUI();
		Init_PopupUI();
		Init_MsgBoxUI();
		Init_ToolTipUI();

		Init_WorldTalkUI();
	}

	public void ChangeZtestMode(UnityEngine.Rendering.CompareFunction mode)
	{
		Canvas.GetDefaultCanvasMaterial().SetInt("unity_GUIZTestMode", (int)mode);
	}


	public bool IS_LockTouch() {
		return !m_EventSystem.enabled;
	}
	/// <summary> UI 터치 막기 </summary>
	public void TouchLock(bool Lock) {
		if (m_EventSystem.enabled == Lock) m_EventSystem.enabled = !Lock;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Popup

	string GetPath(PopupName popup)
	{
		switch (popup)
		{
		case PopupName.Title:
		case PopupName.Play:
		case PopupName.Stage:
		case PopupName.BattleUI:
		case PopupName.PVP:
			return "Main";
		case PopupName.Msg_None:
		case PopupName.Msg_OK:
		case PopupName.Msg_YN:
		case PopupName.Msg_WebView:
		case PopupName.Msg_CenterAlarm:
		case PopupName.Msg_Store_Ticket_Buy:
		case PopupName.Msg_YN_Cost:
		case PopupName.Msg_Inven_Buy:
		case PopupName.Msg_CommingSoon:
		case PopupName.Msg_CDN:
		case PopupName.Msg_Challenge_Alarm:
		case PopupName.Msg_Store_GatchaMileage_Alarm:
		case PopupName.Msg_RewardGet_Center:
		case PopupName.Msg_RewardGet_Monthly:
		case PopupName.Msg_Auction_Guide:
		case PopupName.Msg_Store_VIPOpen:
		case PopupName.Msg_StageWarning:
		case PopupName.Msg_Guide:
		case PopupName.Msg_Timer:
		case PopupName.Msg_NewUnion_Result:
		case PopupName.Msg_Union_Attendance_Result:
		case PopupName.Msg_Union_LvUpAlarm:
		case PopupName.Msg_YN_YRed:
		case PopupName.Msg_YN_BtnControl:
		case PopupName.Msg_Union_Research_Donation_Confirm:
		case PopupName.Msg_Union_Research_Finish:
		case PopupName.Msg_YN_Cost_BtnControl:
		case PopupName.Msg_PVP_Research_LvUp:
			return "MsgBox";
		}
		return "Popup";
	}
	public void MainChange(PopupPos popupPos)
	{
		switch (popupPos)
		{
		case PopupPos.BATTLE:
			m_TfMainUI.gameObject.SetActive(false);
			m_TfPopupUI.gameObject.SetActive(false);
			return;
		}
		Init_BattleMainUI();
		m_TfMainUI.gameObject.SetActive(true);
		m_TfPopupUI.gameObject.SetActive(true);
	}

	Transform GetPanel(PopupPos pos) {
		Transform panel = m_TfMainUI;
		switch (pos) {
			case PopupPos.BATTLE: return m_TfBattleUI;
			case PopupPos.POPUPUI: return MAIN.IS_State(MainState.BATTLE) ? m_TfBattleUI : m_TfPopupUI;
			case PopupPos.MSGBOX: return m_TfMsgBoxUI;
			case PopupPos.TOOLTIP:return m_TfToolTipUI;
		}
		return m_TfMainUI;
	}

	public bool IS_ViewPopup(PopupName popup) {
		return m_PopupList.Find((pop) => pop.m_Popup == popup) != null;
	}

	public PopupBase Set_Popup(PopupPos pos, PopupName popup, System.Action<int, GameObject> cb = null, params object[] args) {
		Transform panel = GetPanel(pos);
		switch (pos) {
			case PopupPos.MAINUI:
 				Init_MainUI();
				Init_PopupUI(popup != PopupName.Play);
				break;
			case PopupPos.BATTLE:
				Init_BattleMainUI();
				Init_PopupUI(popup != PopupName.BattleUI);
				break;
			case PopupPos.POPUPUI:
				break;
			case PopupPos.MSGBOX:
				break;
			case PopupPos.TOOLTIP:
				break;
			case PopupPos.TUTO: return null;
		}
		string prefabName = string.Format("{0}/{1}", GetPath(popup), popup.ToString());
#if UNITY_EDITOR && NOT_USE_NET && USE_CHEAT
		if (popup == PopupName.CheatPage) prefabName = "7_Sample/CheatPage/CheatPage";
#endif
		PopupBase obj = UTILE.LoadPrefab(prefabName, true, panel).GetComponent<PopupBase>();
		switch (pos) {
			case PopupPos.MAINUI:
				m_MainUI = obj;
				break;
			case PopupPos.BATTLE:
				m_BattleUI = obj;
				break;
			case PopupPos.POPUPUI:
				m_PopupList.Add(obj);
				break;
			case PopupPos.MSGBOX:
				m_MsgList.Add((MsgBoxBase)obj);
				break;
			case PopupPos.TOOLTIP:
				m_ToolTipList.Add(obj);
				break;
		}
		obj.SetData(pos, popup, cb, args);
		return obj;
	}

	public MsgBoxBase Set_MsgBox(PopupName popup, System.Action<int, GameObject> cb = null, params object[] args)
	{
		if((popup == PopupName.Msg_Challenge_Alarm || popup == PopupName.Msg_Store_GatchaMileage_Alarm) && m_MsgBoxPool.Count > 0) {
			m_MsgBoxPool.Add(new MsgBoxData() { m_Name = popup, m_CB = cb, m_Args = args });
			return null;
		}
		MsgBoxBase box = (MsgBoxBase)Set_Popup(PopupPos.MSGBOX, popup, cb, args);
		box.SetMsg("", "");
		return box;
	}

	public MsgBoxBase Set_MsgBox(PopupName popup, string Title, string Msg, System.Action<int, GameObject> cb = null, params object[] args) {
		MsgBoxBase box = (MsgBoxBase)Set_Popup(PopupPos.MSGBOX, popup, cb, args);
		box.SetMsg(Title, Msg);
		return box;
	}

	public MsgBoxBase StartLackPop(int idx, bool isAlarmMsg = false, Action<int> cb = null, string _msg = null, int _cnt = 1)
	{
		TItemTable item = TDATA.GetItemTable(idx);
		if(isAlarmMsg)
		{
			return Set_MsgBox(PopupName.Msg_CenterAlarm, "", Utile_Class.StringFormat(TDATA.GetString(202), item.GetName()));
		}
		else if(idx == BaseValue.EXP_IDX) {
			return Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(367));
		}
		//else if(idx == BaseValue.DOLLAR_IDX) {
		//	return Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(366));
		//}
		else if (idx == BaseValue.ENERGY_IDX)
		{
			//string msg = string.Format("{0}\n<color=#ff0000>{1}</color>", Utile_Class.StringFormat(TDATA.GetString(201), item.GetName()), TDATA.GetString(536));
			TShopTable tshop = TDATA.GetShopTable(BaseValue.ENERGY_SHOP_IDX);
			string msg = _msg == null ? Utile_Class.StringFormat(TDATA.GetString(833), item.GetName(), tshop.m_Rewards[0].m_ItemCnt) : _msg;
			int price = USERINFO.m_ShopInfo.GetEnergyPrice();
			var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.ENERGY_SHOP_IDX);
			var cnt = buyinfo == null ? tshop.m_LimitCnt : tshop.m_LimitCnt - buyinfo.Cnt;
			if (cnt < 1) return Set_MsgBox(PopupName.Msg_OK, TDATA.GetString(200), Utile_Class.StringFormat(TDATA.GetString(202), item.GetName()));

			return Set_MsgBox(PopupName.Msg_YN_Cost_BtnControl, TDATA.GetString(200), msg, (result, obj) => {
				EMsgBtn btn = (EMsgBtn)result;
				if (btn == EMsgBtn.BTN_YES)
				{
					if (obj.GetComponent<Msg_YN_Cost_BtnControl>().IS_CanBuy)
					{
						///아이템 획득 연출
						USERINFO.ITEM_BUY(BaseValue.ENERGY_SHOP_IDX, 1, (res) => { cb?.Invoke(res.result_code); });
					}
					else
					{
						StartLackPop(tshop.GetPriceIdx(), !MAIN.IS_State(MainState.PLAY));
					}
				}
			}, tshop.m_PriceType, tshop.m_PriceIdx, price
			, new Msg_YN_Cost_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_NO, Label = TDATA.GetString(11), BG = UIMng.BtnBG.Brown }
			, new Msg_YN_Cost_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_YES, Label = string.Format(TDATA.GetString(1076), cnt, tshop.m_LimitCnt), BG = UIMng.BtnBG.Green });
		}
		else if (idx == BaseValue.CONTINUETICKET_IDX)
		{
			TShopTable tshop = TDATA.GetShopTable(BaseValue.CONTINUETICKET_GOLD_SHOP_IDX);
			return Set_MsgBox(PopupName.Msg_YN_Cost_BtnControl, TDATA.GetString(200), TDATA.GetString(1075)
				, (result, obj) => {
					EMsgBtn btn = (EMsgBtn)result;
					if (btn == EMsgBtn.BTN_YES)
					{
						if (obj.GetComponent<Msg_YN_Cost_BtnControl>().IS_CanBuy) USERINFO.ITEM_BUY(BaseValue.CONTINUETICKET_GOLD_SHOP_IDX, _cnt, (res) => { cb?.Invoke(res.result_code); });
						else
						{
							StartLackPop(tshop.GetPriceIdx(), !MAIN.IS_State(MainState.PLAY));
							cb?.Invoke(-1);
						}
					}
					else cb?.Invoke(-1);
				}, tshop.m_PriceType, tshop.m_PriceIdx, tshop.GetPrice(_cnt) 
				, new Msg_YN_Cost_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_NO, Label = TDATA.GetString(11), BG = UIMng.BtnBG.Brown }
				, new Msg_YN_Cost_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_YES, Label = TDATA.GetString(10), BG = UIMng.BtnBG.Green });
		} 
		else if(idx == BaseValue.NAMECHANGETICKET_IDX) {
			//string msg = string.Format("{0}\n<color=#ff0000>{1}</color>", Utile_Class.StringFormat(TDATA.GetString(201), item.GetName()), TDATA.GetString(536));
			string msg = Utile_Class.StringFormat(TDATA.GetString(201), item.GetName());
			TShopTable tshop = TDATA.GetShopTable(BaseValue.NAMECHANGE_SHOP_IDX);
			return Set_MsgBox(PopupName.Msg_YN_Cost, TDATA.GetString(200), msg
				, (result, obj) => {
					EMsgBtn btn = (EMsgBtn)result;
					if (btn == EMsgBtn.BTN_YES)
					{
						if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
							USERINFO.ITEM_BUY(BaseValue.NAMECHANGE_SHOP_IDX, 1, (res) => { cb?.Invoke(res.result_code); });
						}
						else {
							POPUP.StartLackPop(tshop.m_PriceIdx);
						}
					}
				}, tshop.m_PriceType, tshop.m_PriceIdx, tshop.GetPrice(), false);
		}
		else if(idx == BaseValue.PVPCOIN_IDX) {
			return Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7010));
		}
		else if(idx == BaseValue.GUILDCOIN_IDX) {
			return Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6075));
		}
		else if(MAIN.IS_State(MainState.PLAY))
		{
			return Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(200), Utile_Class.StringFormat(TDATA.GetString(201), item.GetName())
				, (result, obj) =>
				{
					EMsgBtn btn = (EMsgBtn)result;
					if (btn == EMsgBtn.BTN_YES)
					{
						// 해당 상점으로 이동
						ShopGroup? shop = null;
						if (idx == BaseValue.CASH_IDX) shop = ShopGroup.Cash;
						else if (idx == BaseValue.DOLLAR_IDX) shop = ShopGroup.Money;
						//else if (idx == BaseValue.ENERGY_IDX) shop = ShopGroup.Energy;
						else if (idx == BaseValue.INVEN_IDX)
						{
							StartInvenBuyPopup(null);
							return;
						}
						Init_PopupUI();

						//현재 상태가 샵일때는 탭 이동만(메뉴 체인지하면 현재 탭이 기본 샵이라 문제생김
						Main_Play mainplay = m_MainUI.GetComponent<Main_Play>();
						if (mainplay.m_State == MainMenuType.Shop && shop != null) {
							Shop shopcomp = mainplay.GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
							if (shopcomp.GetTab != Shop.Tab.Shop) shopcomp.StateChange(0);
							shopcomp.StartPos(mainplay.m_State != MainMenuType.Shop, (ShopGroup)shop);
							//initpopupui 에서 그냥 remove 해버려서 강제로해줘야함
							shopcomp.GetPanel(Shop.Tab.Shop).GetComponent<Item_Store_Tab_Shop>().SetItemGachaGauge();
						}
						else
							mainplay.MenuChange((int)MainMenuType.Shop, false, Shop.Tab.Shop, shop);
					}
				});
		}
		else
		{
			return Set_MsgBox(PopupName.Msg_OK, TDATA.GetString(200), Utile_Class.StringFormat(TDATA.GetString(202), item.GetName()));
		}
	}

	public void StartInvenBuyPopup(System.Action<int, GameObject> cb = null)
	{
		Set_MsgBox(PopupName.Msg_Inven_Buy, "", "", (result, obj) =>
		{
			EMsgBtn btn = (EMsgBtn)result;
			if (btn == EMsgBtn.BTN_YES)
			{
				Msg_Inven_Buy msg = obj.GetComponent<Msg_Inven_Buy>();
				int Cnt = msg.GetBuyCnt();
				int Price = USERINFO.GetInvenPrice(Cnt);
				if (USERINFO.m_Cash < Price)
				{
					StartLackPop(BaseValue.CASH_IDX);
					return;
				}
#if NOT_USE_NET
				PlayEffSound(SND_IDX.SFX_0004);
				//FireBase-Analytics
				MAIN.GoldToothStatistics(GoldToothContentsType.Inventory);

				USERINFO.GetCash(-Price);
				USERINFO.AddInven(Cnt);
				Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(803));
				cb?.Invoke(result, obj);
#else
				WEB.SEND_REQ_SHOP_BUY_INVEN((res) => {
					if (!res.IsSuccess())
					{
						WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
						return;
					}
					PlayEffSound(SND_IDX.SFX_0004);
					PlayEffSound(SND_IDX.SFX_1010);
					//FireBase-Analytics
					MAIN.GoldToothStatistics(GoldToothContentsType.Inventory);

					Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(803));

					cb?.Invoke(result, obj);
				}, Cnt);
#endif
			}
			else cb?.Invoke(result, obj);
		});
	}

	public MsgBoxBase Set_WebView(string Title, string Url, System.Action<int, GameObject> cb = null) {
		MsgBoxBase box = (MsgBoxBase)Set_Popup(PopupPos.MSGBOX, PopupName.Msg_WebView, cb, null);
		box.SetMsg(Title, Url);
		return box;
	}

	public void RemoveUI(PopupPos pos, PopupBase popup)
	{
		PopupName name = popup.m_Popup;
		switch (pos)
		{
		case PopupPos.MAINUI:
			if (m_MainUI == popup) Init_MainUI();
			break;
		case PopupPos.BATTLE:
			if (m_BattleUI == popup) Init_BattleMainUI();
			break;
		case PopupPos.POPUPUI:
			for (int i = m_PopupList.Count - 1; i > -1; i--)
			{
				if (m_PopupList[i] == popup)
				{
					if (i > 0)
					{
						m_PopupList[i - 1].gameObject.SetActive(true);
						m_PopupList[i - 1].SetUI();
					}
					Destroy(popup.gameObject);
					m_PopupList.RemoveAt(i);
					break;
				}
			}
			switch (name)
			{
			case PopupName.Synergy_Applied:
				break;
			case PopupName.Info_Char:
			case PopupName.Inventory:
			case PopupName.StgReplay:
				TUTO.Start(TutoStartPos.PlayStart);
				//TUTO.Start(TutoStartPos.POPUP_REMOVE, name);
				break;
			case PopupName.PVP_DeckSetting:
				//if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.DeckSet_Close)) TUTO.Next();
				break;
			case PopupName.PVP_Main:
				//if (TUTO.IsTuto(TutoKind.PVP, (int)TutoType_PVP.PVPMain_Close)) TUTO.Next();
				break;
			}
			break;
		case PopupPos.MSGBOX:
			if (m_MsgList.Count > 0)
			{
				for (int i = m_MsgList.Count - 1; i > -1; i--)
				{
					if (m_MsgList[i] == popup)
					{
						Destroy(popup.gameObject);
						m_MsgList.RemoveAt(i);
						break;
					}
				}
			}
			m_NowMsgBoxPool = null;
			break;
		case PopupPos.TOOLTIP:
			if (m_ToolTipList.Count > 0) {
				for (int i = m_ToolTipList.Count - 1; i > -1; i--) {
					if (m_ToolTipList[i] == popup) {
						Destroy(popup.gameObject);
						m_ToolTipList.RemoveAt(i);
						break;
					}
				}
			}
			break;
		case PopupPos.TUTO:
			switch(popup.m_Popup)
			{
			case PopupName.Tuto_Start:
				if (m_Tuto_Start != null)
				{
					Destroy(m_Tuto_Start.gameObject);
					m_Tuto_Start = null;
				}
				break;
			default:
				if (m_Tuto != null)
				{
					m_Tuto.RemoveClone();
					Destroy(m_Tuto.gameObject);
					m_Tuto = null;
				}
				break;
			}
			break;
		case PopupPos.EXIT:
			Destroy(popup.gameObject);
			m_ExitPopup = null;
			break;
		}

	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// MainUI
#pragma warning disable 0649
	[SerializeField] Transform m_TfMainUI;
	PopupBase m_MainUI;
#pragma warning restore 0649
	public void Init_MainUI() {
		if (m_MainUI) {
			Destroy(m_MainUI.gameObject);
			m_MainUI = null;
		}
	}

	public PopupBase GetMainUI() {
		return m_MainUI;
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////
	/// BattleMainUI
#pragma warning disable 0649
	[SerializeField] Transform m_TfBattleUI;
	PopupBase m_BattleUI;
#pragma warning restore 0649
	public void Init_BattleMainUI() {

		for (int i = m_TfBattleUI.childCount - 1; i > -1; i--) Destroy(m_TfBattleUI.GetChild(i).gameObject);
		m_BattleUI = null;
	}
	public PopupBase GetBattleUI() {
		return m_BattleUI;
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////
	// PopupUI
#pragma warning disable 0649
	[SerializeField] Transform m_TfPopupUI;
	List<PopupBase> m_PopupList = new List<PopupBase>();
#pragma warning restore 0649
	public void Init_PopupUI(bool all = true) {
		if (!all) {
			for (int i = m_PopupList.Count - 1; i > -1; i--) {
				PopupBase popup = m_PopupList[i];
				switch (popup.m_Popup) {
					case PopupName.RewardList:
					case PopupName.RewardList_Equip:
					case PopupName.CharDraw:
					case PopupName.AddEvent:
						break;
					default:
						m_PopupList.Remove(popup);
						if(popup != null) Destroy(popup.gameObject);
						break;
				}
			}
		}
		else {
			m_PopupList.Clear();
			for (int i = m_TfPopupUI.childCount - 1; i > -1; i--) Destroy(m_TfPopupUI.GetChild(i).gameObject);
		}
	}

	public bool IS_PopupUI() {
		return m_PopupList.FindAll(t => t.gameObject.activeInHierarchy == true).Count > 0;
		//return m_PopupList.Count > 0;
	}
	public PopupBase GetPopup() {
		if (!IS_PopupUI()) return null;
		return m_PopupList[m_PopupList.Count - 1];
	}
#pragma warning disable 0649
	public Transform m_TfToolTipUI;
	List<PopupBase> m_ToolTipList = new List<PopupBase>();
#pragma warning restore 0649
	public void Init_ToolTipUI() {
		m_ToolTipList.Clear();
		for (int i = m_TfToolTipUI.childCount - 1; i > -1; i--) Destroy(m_TfToolTipUI.GetChild(i).gameObject);
	}
	public bool IS_ToolTipUI() {
		return m_ToolTipList.Count > 0;
	}
	public PopupBase GetToolTip() {
		return m_ToolTipList[m_ToolTipList.Count - 1];
	}
	//PopupName parent, ItemInfo item, Action<Info_Item.InfoChange, object[]> infochangeCB, Action<int, GameObject> CB = null
	public PopupBase ViewItemInfo(Action<int, GameObject> CB, object[] param)
	{
		PopupName popup = PopupName.Info_Item_Equip;
		if (((ItemInfo)param[0]).m_TData.GetInvenGroupType() != ItemInvenGroupType.Equipment) popup = PopupName.Info_Item_ETC;
		return Set_Popup(PopupPos.POPUPUI, popup, CB, param);//item, parent, infochangeCB
	}

	public PopupBase ViewItemToolTip(RES_REWARD_BASE _reward,  RectTransform _trans) {
		if (_reward.Type == Res_RewardType.Item && (TDATA.GetItemTable(_reward.GetIdx()).m_Type == ItemType.RandomBox || TDATA.GetItemTable(_reward.GetIdx()).m_Type == ItemType.AllBox)) {
			return Set_Popup(PopupPos.POPUPUI, PopupName.Info_Item_RewardBox, null, new object[] { new ItemInfo(_reward.GetIdx()), PopupName.NONE, null, MAIN.GetRewardData(RewardKind.Item, _reward.GetIdx(), 1) });
		}
		else if(_reward.Type == Res_RewardType.Item && TDATA.GetItemTable(_reward.GetIdx()).GetInvenGroupType() == ItemInvenGroupType.Equipment) {
			ItemInfo info = null;
			long uid = ((RES_REWARD_ITEM)_reward).UID;
			if (uid > 0) info = USERINFO.GetItem(uid);
			else info = new ItemInfo(_reward.GetIdx(), uid);
			PopupBase popup = Set_Popup(PopupPos.POPUPUI, PopupName.Info_Item_Equip, null, new object[] { info, PopupName.NONE, null }); 
			popup.GetComponent<Info_Item_Equip>().OnlyInfo();
			return popup;
		}
		else {
			return Set_Popup(PopupPos.TOOLTIP, PopupName.Item_ToolTip, null, _reward, _trans);
		}
	}
	public PopupBase ViewEqChange(PopupName parent, ItemInfo item, Action<int, GameObject> CB = null)
	{
		return Set_Popup(PopupPos.POPUPUI, PopupName.EquipChange, CB, item);
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////
	// MsgBosUI
#pragma warning disable 0649
	public Transform m_TfMsgBoxUI;
	List<MsgBoxBase> m_MsgList = new List<MsgBoxBase>();
#pragma warning restore 0649
	public void Init_MsgBoxUI() {
		m_MsgList.Clear();
		for (int i = m_TfMsgBoxUI.childCount - 1; i > -1; i--) Destroy(m_TfMsgBoxUI.GetChild(i).gameObject);
	}
	public bool IS_MsgUI() {
		return m_MsgList.Count > 0;
	}
	public PopupBase GetMsgBox() {
		if(m_MsgList.Count > 0) return m_MsgList[m_MsgList.Count - 1];
		return null;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////
	// Exit
#pragma warning disable 0649
	[SerializeField] Transform m_TfExitUI;
	[HideInInspector] public MsgBoxBase m_ExitPopup;
#pragma warning restore 0649

	public void SetExitPopup() {
		string prifabName = string.Format("{0}/{1}", GetPath(PopupName.Msg_YN), PopupName.Msg_YN.ToString());
		m_ExitPopup = (MsgBoxBase)(UTILE.LoadPrefab(prifabName, true, m_TfExitUI).GetComponent<PopupBase>());//.GetComponent<RectTransform>();
		m_ExitPopup.SetData(PopupPos.EXIT, PopupName.Msg_YN, (result, obj) => {
			EMsgBtn btn = (EMsgBtn)result;
			if ((EMsgBtn)result == EMsgBtn.BTN_YES) MAIN.Exit();
			m_ExitPopup = null;
		}, null);
		m_ExitPopup.SetMsg("", string.Format(TDATA.GetString(18), TDATA.GetString(1)));
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////
	// Loading
#pragma warning disable 0649
	[SerializeField] Transform m_Loading;
	[SerializeField] Transform m_LoadingFade;
#pragma warning restore 0649
	public enum LoadingType { Black, FadeIn, FadeOut }
	public void SetLoadingFade(LoadingType _type) {
		iTween.StopByName(gameObject, "LoadingFade");
		if (_type == LoadingType.Black) TW_LoadingFade(1f);
		else {
			if (_type == LoadingType.FadeIn && m_LoadingFade.GetComponent<CanvasGroup>().alpha == 1f) return;
			else if (_type == LoadingType.FadeOut && m_LoadingFade.GetComponent<CanvasGroup>().alpha == 0f) return;
			iTween.ValueTo(gameObject, iTween.Hash("from", m_LoadingFade.GetComponent<CanvasGroup>().alpha, "to", _type == LoadingType.FadeIn ? 1f : 0f, "onupdate", "TW_LoadingFade", "time", 0.5f, "name", "LoadingFade"));
		}
	}
	void TW_LoadingFade(float _amount) {
		m_LoadingFade.GetComponent<CanvasGroup>().alpha = _amount;
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////
	// Connecting
#pragma warning disable 0649
	[SerializeField] Transform m_Connecting;
	bool m_ConnectingLock;
#pragma warning restore 0649
	public enum ConnectingTrigger
	{
		Delay = 0,
		Now,
		End
	}

	public void SetConnecting(bool Active, ConnectingTrigger _trig = ConnectingTrigger.Delay) {
		if (m_ConnectingLock) return;
		Utile_Class.DebugLog("!!! SetConnecting Set : " + Active);
		m_Connecting.gameObject.SetActive(Active);
		if (Active) m_Connecting.GetComponent<Animator>().SetTrigger(_trig.ToString());
	}
	public bool IS_Connecting() {
		return m_Connecting.gameObject.activeSelf;
	}

	/// <summary> 결제등 외부 라이브러리에의해 강제로 컨넥팅 보여주어야하는 상황들을 위해 만듬
	/// <para>연결 조합으로인해 표시가 안나올때도 있어</para>
	/// <para>연결중 사라지지 않게 하기위해 끝날때까지</para>
	/// <para>★ 사용 </para>
	/// <para>1. 과금 결제 시도 (PUID -> 과금 API -> 완료 -> 아이템 지급 사이에 딜레이가 심함)</para>
	/// </summary>
	public void LockConnecting(bool Active)
	{
		Utile_Class.DebugLog("!!! LockConnecting Set : " + Active);
		m_ConnectingLock = Active;
		m_Connecting.gameObject.SetActive(Active);
		if (Active) m_Connecting.GetComponent<Animator>().SetTrigger(ConnectingTrigger.Now.ToString());
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// WorldTalk
#pragma warning disable 0649
	[SerializeField] Transform m_WorldTalkUI;
#pragma warning restore 0649

	public Transform GetWorldUIPanel() {
		return m_WorldTalkUI;
	}

	void Init_WorldTalkUI() {
		for (int i = m_WorldTalkUI.childCount - 1; i > -1; i--) Destroy(m_WorldTalkUI.GetChild(i).gameObject);
	}

	public void TalkStart(string talk, Vector3 woldPos, float time = 1f) {
		Vector3 position = Utile_Class.GetCanvasPosition(woldPos);
		Item_Talk_Cry item = UTILE.LoadPrefab("Item/Stage/Item_Talk_Cry", true, m_WorldTalkUI).GetComponent<Item_Talk_Cry>();
		item.transform.position = position;
		item.SetData(talk, time);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Tuto UI
	[SerializeField] Transform m_TutoPanel;
	TutoUI m_Tuto;
	TutoStart m_Tuto_Start;
	IEnumerator m_TutoTimer;
	public TutoUI ShowTutoUI(Action<int, GameObject> cb = null)
	{
		if (m_Tuto == null)
		{
			string prifabName = string.Format("{0}/{1}", GetPath(PopupName.TutoUI), PopupName.TutoUI.ToString());
			m_Tuto = UTILE.LoadPrefab(prifabName, true, m_TutoPanel).GetComponent<TutoUI>();
			m_Tuto.SetData(PopupPos.TUTO, PopupName.TutoUI, cb, null);
		}
		else if (!m_Tuto.gameObject.activeSelf) m_Tuto.gameObject.SetActive(true);
		return m_Tuto;
	}

	/// <summary> 시작 연출 </summary>
	/// <param name="dlgIdx">다른 대화나 캐릭터를 변경하고 싶을때 사용 (0:사용안함, 1~다이얼로그 인덱스)</param>
	/// <param name="cb">종료 콜백</param>
	public void ShowTutoStartAction(int dlgIdx = 0, Action cb = null)
	{
		string prifabName = string.Format("{0}/{1}", GetPath(PopupName.Tuto_Start), PopupName.Tuto_Start.ToString());
		m_Tuto_Start = UTILE.LoadPrefab(prifabName, true, m_TutoPanel).GetComponent<TutoStart>();
		m_Tuto_Start.SetData(PopupPos.TUTO, PopupName.Tuto_Start, (result, obj)=> { cb?.Invoke(); }, new object[] { dlgIdx });
	}

	public void StartTutoTimer(Action EndCB, float time = 4f)
	{
		StopTutoTimer();
		if(time > 1f)
			time *= 0.5f;
		m_TutoTimer = TutoTimer(EndCB, time);
		StartCoroutine(TutoTimer(EndCB, time));
	}

	public void StopTutoTimer()
	{
		if (m_TutoTimer == null) return;
		StopCoroutine(m_TutoTimer);
		m_TutoTimer = null;
	}

	IEnumerator TutoTimer(Action EndCB, float time = 4f)
	{
		yield return new WaitForSeconds(time);
		EndCB?.Invoke();
		m_TutoTimer = null;
	}

	public void RemoveTutoUI()
	{
		if (m_Tuto != null) RemoveUI(PopupPos.TUTO, m_Tuto);
	}
	public bool IS_TutoUI() {
		return m_Tuto != null;
	}
	public TutoUI GetTutoUI() {
		return m_Tuto;
	}
	//////////////////////
	/// StatName
	public Sprite StatIcon(StatType _type) {
		return UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)_type), "png");
	}
	public Color StatColor(StatType _type) {
		switch (_type) {
			case StatType.Men:return Utile_Class.GetCodeColor("#B3AC3C");
			case StatType.Hyg: return Utile_Class.GetCodeColor("#3CAEB3");
			case StatType.Sat: return Utile_Class.GetCodeColor("#B25813");
			case StatType.Atk: return Utile_Class.GetCodeColor("#A43434");
			case StatType.Def: return Utile_Class.GetCodeColor("#3C52B3");
			case StatType.Heal: return Utile_Class.GetCodeColor("#47A862");
			default: return Color.white;
		}
	}
	public Color DayColor(DayOfWeek _day) {
		switch (_day) {
			case DayOfWeek.Monday: return Utile_Class.GetCodeColor("#DB7F7F");
			case DayOfWeek.Tuesday: return Utile_Class.GetCodeColor("#FBAC59");
			case DayOfWeek.Wednesday: return Utile_Class.GetCodeColor("#79C680");
			case DayOfWeek.Thursday: return Utile_Class.GetCodeColor("#65BDEA");
			case DayOfWeek.Friday: return Utile_Class.GetCodeColor("#A182C3");
			default:return Color.white;
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 버튼 배경 이미지
	public enum BtnBG
	{
		Normal = 0,
		Yellow,
		Green,
		Red,
		Brown,
		Not
	}
	public Sprite GetBtnBG(BtnBG btn)
	{
		string Name = "UI/Button/Button_General_03";
		switch(btn)
		{
		case BtnBG.Brown: Name = "UI/FirstLoadImg/Button_General"; break;
		case BtnBG.Yellow: Name = "UI/FirstLoadImg/Button_General_02"; break;
		case BtnBG.Green: Name = "UI/Button/Button_General_03"; break;
		case BtnBG.Red: Name = "UI/Button/Button_General_04"; break;
		case BtnBG.Not: Name = "UI/Button/Button_General_Deact"; break;
		}
		return UTILE.LoadImg(Name, "png");
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//테스트용 치트
	public GameObject[] CheatBtns;
	public void ClickOpenCheatPage() {
#if UNITY_EDITOR && NOT_USE_NET && USE_CHEAT
		if (IS_PopupUI() && m_PopupList.Find(t=>t.name == "CheatPage") != null) return;
		Set_Popup(PopupPos.POPUPUI, PopupName.CheatPage, (result, obj) => { MAIN.Save_UserInfo(); });
#endif
	}
	public void ClickStageClear() {
#if USE_STAGE_CLEAR_CHEAT
		if (MainMng.IsValid()) {
			if (MAIN.IS_State(MainState.TITLE)) return;
			if (MAIN.IS_State(MainState.START)) return;
			if (MAIN.IS_State(MainState.PLAY) && GetMainUI().m_Popup == PopupName.Play) return;
			MAIN.ClearCurrentStage();
		}
#endif
	}
	public void ClickStatView() {
#if VIEW_STAGE_STAT
		if (MainMng.IsValid()) {
			if (MainMng.Instance.IS_State(MainState.TITLE)) return;
			if (MainMng.Instance.IS_State(MainState.START)) return;
			if (MainMng.Instance.IS_State(MainState.PLAY)) return;
			if (IS_PopupUI() && m_PopupList.Find(t => t.name == "ViewStageUserStat") != null) return;
			Set_Popup(PopupPos.POPUPUI, PopupName.ViewStageUserStat);
		}
#endif
	}
}
