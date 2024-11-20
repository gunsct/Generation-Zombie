using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Text;
using Newtonsoft.Json;
using static LS_Web;
using System.Linq;
using hive;
using GPresto.Protector;
#if !UNITY_EDITOR && UNITY_ANDROID
using UnityEngine.Android;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum MainState
{
	START = -1      // 최초 로드상태
	, TITLE = 0     // 타이틀
	, PLAY          // 플레이
	, STAGE         // 스테이지
	, BATTLE        // 대전
	, TOWER         // 소탕, 타워등 맵으로 진행하는 모드
	, PVP			// PVP
#if STAGE_TEST
	, STAGE_TEST         // 소탕, 타워등 맵으로 진행하는 모드
#endif
}
public enum StageResult
{
	None = -1		// 결과없음
	, Clear = 0     // 성공
	, Fail          // 실패
}

public enum SceneLoadMode
{
	NORMAL = 1      // 일반 로드
	, LOADANI       // 로딩 에니 넣어서 사용
	, BACKGROUND    // 백그라운드 로드
	, ADDITIVESTART // 멀티 로드 시작
	, ADDITIVEEND   // 멀티 로드 복귀
}

public class MainMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Instance
	private static MainMng m_Instance = null;
	public static MainMng Instance
	{
		get
		{
			if (!m_Instance)
			{
				m_Instance = Utile_Class.SetInstance(typeof(MainMng)) as MainMng;
				DontDestroyOnLoad(m_Instance.gameObject);   //씬 전환시 안날라가게...
															// 생성되었으니 다른 로그인 씬으로 넘겨준다.

				GPresto.Protector.Engine.GPrestoEngine.Start();

				m_Instance.gameObject.AddComponent(typeof(SpeedHackScanner));

			}

			return m_Instance;
		}
	}

	public static bool IsValid()
	{
		return m_Instance != null;
	}

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void RuntimeInit()
	{
		if (IsValid()) return;
		Instance.Init();//.Init();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Value
	MainState m_BState = MainState.START;
	public MainState m_State = MainState.START;
	public Utile_Class m_Utile = null;
	public UIMng m_UIMng;
	public AppInfo m_AppInfo;
	public AccountMng m_AccountMng;
	public LS_Web m_Web;
	public ToolData m_ToolData; 
	public UserInfo m_UserInfo;
	public BattleInfo m_BattleInfo;
	public DelegateInfo m_DelegateInfo;
	public AdsMng m_ADS;
	public ReviewMng m_pReview;
	public IAPMng m_IAP;

	public HiveMng m_Hive;
	public FBMng m_FB;

	public List<RES_CHALLENGE_MYRANKING> m_ChallengeAlram = new List<RES_CHALLENGE_MYRANKING>();

	// 스테이지 진입에 필요한 셋팅값
	public StageInfo m_StageInfo = new StageInfo();

	// PVP에 필요한 셋팅값
	public PVPInfo m_PVPInfo = new PVPInfo();
#if STAGE_TEST
	public bool m_StartStageTestMng = false;
#endif
	IEnumerator m_SysMsgCor = null;
	List<RES_SYSTEM_MSG> m_SysMsgs = new List<RES_SYSTEM_MSG>();
	float m_EventRefreshTimer;
	const float EVENT_REFRESH_TIME = 600f;
	public bool Is_LoadServerConfig = false;
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Process
	// Start is called before the first frame update
	void Awake()
	{
		if (IsValid() && m_Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		// 로그 보기 셋팅
#if USE_LOG_MANAGER
		Debug.unityLogger.logEnabled = true;
#else
		Debug.unityLogger.logEnabled = false;
#endif
#if (UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID)
#else
		// PC용 사이즈 조절
		Screen.SetResolution(720, 1280, false);
#endif

#if UNITY_EDITOR
		// 30프레임 조절하기
		Utile_Class.SetMaxFrame(0, 0);
#else
		// 60프레임 조절하기
		Utile_Class.SetMaxFrame(0, 60);
#endif
		// 화면 자동 잠김 무시
		Utile_Class.Set_AutoScreenSleep(false);
		//Utile_Class.SetMaxFrame(60);
		// 퀄리티 조절
		//Utile_Class.SetQualityLevel(Utile_Class.QualityName.Low);

		//
		//QualitySettings.antiAliasing = PlayerPrefs.GetInt("AntiAliasing", 1);

		//QualitySettings.masterTextureLimit = PlayerPrefs.GetInt("HalfTexture", 0);

		//QualitySettings.vSyncCount = PlayerPrefs.GetInt("Vsync", 1);

		//Application.targetFrameRate = PlayerPrefs.GetInt("FPS", 60);

		//QualitySettings.SetQualityLevel(PlayerPrefs.GetInt("Quality", 0), true);
	}

	private void Start()
	{
		SDK_Init();
		CheckPermission();
	}

	public void CheckPermission()
	{
//#if !UNITY_EDITOR && UNITY_ANDROID
//		// 푸시 알림 권한(하이브 Push.requestPushPermission(); 호출로 변경)
//		string[] permissions = new string[] {
//			"android.permission.POST_NOTIFICATIONS"
//			};
//		if(permissions.Length > 0)
//		{
//			List<string> reqpermissions = new List<string>();
//			for(int i = 0; i < permissions.Length; i++)
//			{
//				if (!Permission.HasUserAuthorizedPermission(permissions[i])) reqpermissions.Add(permissions[i]);
//			}
//			Permission.RequestUserPermissions(reqpermissions.ToArray());
//		}
//#endif
		//#if !UNITY_EDITOR
		//#if UNITY_2020_2_OR_NEWER
		//#if UNITY_ANDROID
		//		if (
		//		!Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_SCAN")
		//		  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_ADVERTISE")
		//		  || !Permission.HasUserAuthorizedPermission("android.permission.BLUETOOTH_CONNECT"))
		//			Permission.RequestUserPermissions(new string[] {
		//	"android.permission.BLUETOOTH_SCAN",
		//	"android.permission.BLUETOOTH_ADVERTISE",
		//	"android.permission.BLUETOOTH_CONNECT"
		//  });
		//#endif
		//#endif
		//#endif
	}

	void Init()
	{
		if (m_Utile == null) m_Utile = new Utile_Class();
		if (m_Web == null)
		{
			m_Web = new LS_Web();
			m_Web.CheckServer();
		}
		m_Web.Init();
		if (m_UIMng == null) m_UIMng = UTILE.LoadPrefab("UIMng", true).GetComponent<UIMng>();

		if (m_AppInfo == null) m_AppInfo = new AppInfo();
		if (m_AccountMng == null) m_AccountMng = new AccountMng();

		if (m_ToolData == null)
		{
			m_ToolData = new ToolData();
			m_ToolData.Init();
		}

		if (m_UserInfo == null) m_UserInfo = new UserInfo();
		if (m_BattleInfo == null) m_BattleInfo = new BattleInfo();
		if (m_DelegateInfo == null) m_DelegateInfo = new DelegateInfo();

		SND.AllStop();
		WEB.SetSession("");
		m_State = (MainState)SceneManager.GetActiveScene().buildIndex;

		StartCoroutine(OneDayCheck());

		//Is_LoadServerConfig = false;
		//MAIN.Load_ServerConfig((result) =>
		//{
		//	if (m_Web.CheckServer()) MAIN.Load_ServerConfig((result) => {
		//		HIVE?.SetServer();
		//		Is_LoadServerConfig = true;
		//	});
		//	else {
		//		HIVE?.SetServer();
		//		Is_LoadServerConfig = true;
		//	}
		//});
#if OUT_SERVER
		WEB.SEND_REQ_APPCHECK((res) =>
		{
			if (res.result_code != EResultCode.SUCCESS) Exit();
		});
#endif
		m_ChallengeAlram.Clear();
	}

	void SDK_Init()
	{
		if (m_Hive == null) m_Hive = new HiveMng();
		if (m_ADS == null) m_ADS = new AdsMng();
		if (m_pReview == null) m_pReview = new ReviewMng();
		if (m_IAP == null) m_IAP = new IAPMng();
		if (m_FB == null)
		{
			m_FB = new FBMng();
			m_FB.Init();
		}
	}

#if !STAGE_TEST
	double m_CheckPauseTime = 0;
#endif
	bool IsReStartCheck;
	void OnApplicationPause(bool paused)
	{
#if !STAGE_TEST
		if (!paused)
		{
			if (SpeedHack.IsDetect())
			{
				Debug.LogError("SpeedHack Detected !!!!!!!!!!!!!!!");
				return;
			}
			// 10분정도 접속을 안했다면 타이틀로 보내기
			// UTILE.Get_Time() < m_CheckPauseTime : 시간정보 변경해서 내림
			// UTILE.Get_Time() - m_CheckPauseTime > 600 : 시간정보가 변경되었든 아님 시간이 흐른뒤 다시 킨상태
			// POPUP.IS_Connecting() 이 켜져있고 Pause가 발생했다면 SDK에의해 발생한것이므로 다시 시작 막음
			if (m_CheckPauseTime > 1 && m_State > MainState.TITLE && !IsReStartCheck && (UTILE.Get_Time() < m_CheckPauseTime || UTILE.Get_Time() - m_CheckPauseTime > 600))
			{
				ReStart();
			}
#if !NOT_USE_NET
			// 시간정보 동기화
			else
			{
				HIVE?.ReLoadPromotion();
				if(!IsReStartCheck) WEB.SEND_REQ_SERVERTIME();
			}
#endif
			IsReStartCheck = false;
			m_CheckPauseTime = 0;
		}
		else
		{
			m_CheckPauseTime = UTILE.Get_Time();
			IsReStartCheck = POPUP.IS_Connecting();
			if (!TUTO.IsTutoPlay()) SetPause();
		}
#endif
		}
		void SetPause(bool _outcheck = false) {
		PopupBase main = POPUP.GetMainUI();
		if (main == null) return;
		if (POPUP.GetPopup() != null) return;
		switch (main.m_Popup) {
			case PopupName.Stage:
				main.GetComponent<Main_Stage>().ClickPause();
				break;
			case PopupName.BattleUI:
				main.GetComponent<BattleUI>().ClickPause();
				break;
			case PopupName.Training:
				main.GetComponent<Training>().Click_Pause();
				break;
			case PopupName.Play:
				if (_outcheck) POPUP.SetExitPopup();
				break;
		}
	}

	private void Update()
	{
#if UNITY_ANDROID || UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Escape)) OnBack();
#endif
//#if NOT_USE_NET
//		if ((int)m_State > (int)MainState.TITLE) {
//			EventCheck();
//		}
//#endif
	}

	void OnBack() {
		// 광고중에는 뒤로가기 불가
		if (ADS.Is_Show == true) return;
		// 터치 불가일때 차단
		if (!POPUP.IsUseEventSytem) return;
		// 튜토리얼중에는 뒤로가기 불가
		if (TUTO.IsTutoPlay()) {
			//SetPause(true);
			//POPUP.SetExitPopup();
			return;
		}
		if (POPUP.OnBack()) return;
		if (POPUP.IS_PopupUI() && POPUP.GetPopup().m_Popup == PopupName.Stage_Result) return;

		switch (m_State)
		{
			case MainState.PLAY:
				if (PLAY.OnBack()) return;
				break;
			case MainState.STAGE://스테이지에서 어떤 팝업도 없을때는 일시정지창이 뜸
			case MainState.TOWER:
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Pause);
				return;
			case MainState.BATTLE:
				if(STAGEINFO.m_StageModeType != StageModeType.None) {
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Pause);
					return;
				}
				break;
			case MainState.PVP:
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Pause);
				return;
		}

		POPUP.SetExitPopup();
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Destroy
	void End()
	{
		//NetEnd();
		//m_pAccInfo.Logout();
	}

	public void Exit()
	{
		End();
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
//#elif UNITY_ANDROID
//		// IL2CPP 종료 버그로인해 플러그인 가져옴
//		AndroidJavaClass ajc = new AndroidJavaClass("com.lancekun.quit_helper.AN_QuitHelper");
//		AndroidJavaObject UnityInstance = ajc.CallStatic<AndroidJavaObject>("Instance");
//		UnityInstance.Call("AN_Exit");
#else
		Application.Quit();
#endif
	}
	private void OnDestroy()
	{
		End();
	}

	void OnApplicationQuit()
	{
		End();
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 매인 상태
	System.Action m_LoadEndCB;
	AsyncOperation m_LoadAsync = null;
	bool m_Loading;
	public bool ISLoading { get { return m_Loading; } }
	public bool IS_BackState(MainState state)
	{
		return m_BState == state;
	}

	public bool IS_State(MainState state)
	{
		if (ISLoading) return false;
		return m_State == state;
	}

	public MainState GetBackState()
	{
		return m_BState;
	}
	public bool IS_BNStateBattle() {
		return m_BState == MainState.BATTLE || m_State == MainState.BATTLE;
	}
	public AsyncOperation StateChange(MainState state, SceneLoadMode loadMode = SceneLoadMode.NORMAL, System.Action pEndCB = null)
	{
//#if STAGE_TEST
//		if (m_StartStageTestMng)
//		{
//			MAIN.Exit();
//			return null;
//		}
//#endif
		POPUP.SetLoadingFade(UIMng.LoadingType.Black);

		if (state != MainState.STAGE) Time.timeScale = 1f;
		//switch (state) {
		//	case MainState.STAGE:
		//		bool acc = false;
		//		if (MAIN.IS_State(MainState.BATTLE)) {
		//			if (MAIN.IS_BackState(MainState.STAGE)) acc = STAGE.m_IS_GameAccel;
		//			else if (MAIN.IS_BackState(MainState.TOWER)) acc = TOWER.m_IS_GameAccel;
		//			Time.timeScale = acc ? 2f : 1f;
		//		}
		//		else Time.timeScale = 1f;
		//		break;
		//	default: Time.timeScale = 1f; break;
		//}

		m_LoadAsync = null;
		if (m_State == state) return m_LoadAsync;
		// 사운드 멈추기
		LoadingStopSound();

		// 현재 로드되는 씬상태로 체인지
		m_BState = m_State;
		m_State = state;

		switch (state)
		{
		case MainState.BATTLE:
			POPUP.MainChange(PopupPos.BATTLE);
			break;
		default:
			POPUP.MainChange(PopupPos.MAINUI);
			break;
		}

		// 로드 종료 콜백 연결
		m_LoadEndCB = pEndCB;

		// 팝업 초기화 대전에 들어가거나 나올때는 초기화해주면 안됨
		if (IS_BNStateBattle()) { }
		//if ((m_BState == MainState.STAGE && m_State == MainState.BATTLE)            // 스테이지에서 대전으로 넘어갈때
		//	|| (m_BState == MainState.BATTLE && m_State == MainState.STAGE)         // 대전에서 스테이지로 넘어갈때
		//	|| (m_BState == MainState.TOWER && m_State == MainState.BATTLE)         // 대전에서 스테이지로 넘어갈때
		//	|| (m_BState == MainState.BATTLE && m_State == MainState.TOWER)
		//	|| (m_BState == MainState.BATTLE && m_State == MainState.PLAY)
		//	|| (m_BState == MainState.PLAY && m_State == MainState.BATTLE)) { }    // 대전에서 스테이지로 넘어갈때
		else m_UIMng.Init();

		switch (loadMode)
		{
		// 일반 씬로드
		case SceneLoadMode.NORMAL:
			//if(!ScenesCheck(eMainState)) 
			SceneManager.LoadScene((int)state);
			StartCoroutine(Scene_Loading());
			break;
		case SceneLoadMode.ADDITIVESTART:
			m_LoadAsync = SceneManager.LoadSceneAsync((int)state, LoadSceneMode.Additive);
			StartCoroutine(Scene_Loading());
			break;
		case SceneLoadMode.ADDITIVEEND:
			m_LoadAsync = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
			StartCoroutine(Scene_Loading());
			break;

		// 페이드인 아웃 추가(연출 없이 로드되는 부분이 부자연스러울때 사용)
		//case SceneLoadMode.LOADANI:
		//	if (!ScenesCheck(eMainState))
		//	{
		//		m_pPopup.Start_Loading((eState) =>
		//		{
		//			switch (eState)
		//			{
		//			case LoadingState.IDLE:
		//				// 플레이 시작
		//				SceneManager.LoadScene((int)eMainState);
		//				CallEndLoad();
		//				break;
		//			}
		//		});
		//	}
		//	else CallEndLoad();
		//	break;
		// 연출이 있는 상태에서 백그라운드 로딩하여 자연스럽게 넘어가게 하기위해 사용
		case SceneLoadMode.BACKGROUND:
			m_LoadAsync = SceneManager.LoadSceneAsync((int)state);
			StartCoroutine(Scene_Loading());

			// 종료후 pAsync.allowSceneActivation = true; 해주어야 전환됨
			break;
		}
		return m_LoadAsync;
	}
	void CallEndLoad() {
		// 씬 전환하면서 iTWeen의 target이 null 오브젝트들 지워주기위해 셋팅
		iTween.NullObjectClear();
		UTILE.Init_Assets();
		POPUP.SetLoadingFade(UIMng.LoadingType.FadeOut);
		m_LoadEndCB?.Invoke();
		m_LoadEndCB = null;
		//m_UIMng.SetCanvasControll();
	}

	public void ActiveScene(System.Action cb = null)
	{
		if (m_LoadAsync == null) {
			return;
		}
		m_LoadAsync.allowSceneActivation = true;
		if (SceneManager.sceneCount > 1)
		{
			StartCoroutine(SceneCheck(SceneManager.GetSceneAt(SceneManager.sceneCount - 1), cb));// SkipOneFrame(() => { SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1)); }));
		}
		else
		{
			m_Loading = false;
			POPUP.SetLoadingFade(UIMng.LoadingType.FadeOut);
			cb?.Invoke();
		}
	}

	IEnumerator SceneCheck(Scene sn, System.Action cb)
	{
		yield return new WaitWhile(() => !sn.isLoaded);
		m_Loading = false;
		SceneManager.SetActiveScene(sn);
		cb?.Invoke();
	}

	// 로딩 종료체크
	IEnumerator Scene_Loading()
	{
		if (m_LoadAsync != null)
		{
			m_Loading = true;
			m_LoadAsync.allowSceneActivation = false;
			while (!m_LoadAsync.isDone)
			{
				if (m_LoadAsync.progress >= 0.9f) {
					break;
				}
				yield return null;
			}
		}
		CallEndLoad();
		yield return null;
	}

	public void ReStart()
	{
		if (TUTO.IsTutoPlay()) return;
		StopAllCoroutines();
		WEB.SetSession("");
		WEB.Set_DisConnect();
		POPUP.Init();
		MAIN.Init();
		if (m_State != MainState.TITLE)
		{
			STAGEINFO.Init();
			StateChange(MainState.TITLE);
			m_BState = MainState.START;
		}
		else
		{
			m_BState = MainState.START;
			Load_ServerConfig((result) => {
				TITLE.Init();
			});
		}
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Net
	public void Load_ServerConfig(Action<int> result, params EServerConfig[] args)
	{
		Utile_Class.DebugLog($"Load_ServerConfig servermode => {WEB.Get_Server()}");
#if NOT_USE_NET
		result(EResultCode.SUCCESS);
#else
		WEB.SEND_REQ_CONFIG((res) =>
		{
			for (int i = res.Configs.Count - 1; i > -1; i--) WEB.SetConfig(res.Configs[i]);
			result(res.result_code);
		}, args);
#endif
	}

	// 연속 호출 막기
	bool IsLoginProc = false;
	public void Login(ACC_STATE acc, Action<hive.ResultAPI> cb) {
		if (IsLoginProc) return;
		if (!WEB.CheckNetState(true)) return;
		IsLoginProc = true;
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		HIVE.Login(acc, (result) => {
			StartCoroutine(SetLoginResult(result, cb));
		});

		//ACC.Login(acc, (result, msg) => {
		//	if (result != IFAccount.SUCCESS)
		//	{
		//		if (!string.IsNullOrEmpty(msg)) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, msg);
		//	}
		//	// 연속 호출 막기
		//	IsLoginProc = false;
		//	POPUP.SetConnecting(false);
		//	cb?.Invoke(result);
		//});
	}

	IEnumerator SetLoginResult(hive.ResultAPI result, Action<hive.ResultAPI> cb)
	{
		yield return new WaitForEndOfFrame();
		if (!result.isSuccess()) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, result.message);
		IsLoginProc = false;
		POPUP.SetConnecting(false);
		cb?.Invoke(result);
	}

	public void Auth(Action<int> cb, bool IsCancelDelete = false)
	{
#if NOT_USE_NET
		cb?.Invoke(EResultCode.SUCCESS);
#elif USER_TEST && UNITY_EDITOR
		USERINFO.m_UID = APPINFO.m_TestUserNo;
		cb?.Invoke(EResultCode.SUCCESS);
#else
		string ID = ACC.GetID();
		if(string.IsNullOrEmpty(ID) || ID.Equals("0"))
		{
			POPUP.Set_MsgBox(PopupName.Msg_OK, TDATA.GetString(2), TDATA.GetString(457), (result, obj) =>{});
			return;
		}
		WEB.SEND_REQ_AUTH((res) =>
		{
			cb?.Invoke(res.result_code);
		}, ACC_STATE.HIVE, ACC.GetID(), IsCancelDelete);
#endif
	}

	/// <summary> 서버 아닌 임시 테스트용 저장 스테이지 끝날때만 저장 </summary>
	[System.Diagnostics.Conditional("NOT_USE_NET")]
	public void Save_UserInfo()
	{
#if !STAGE_TEST
		string data = JsonConvert.SerializeObject(m_UserInfo);

		byte[] abyData = Encoding.UTF8.GetBytes(data);
		int nLen = abyData.Length;
		Utile_Class.Encode(abyData, nLen, 0);

		m_Utile.SaveData("UserData", ".sv", abyData);
#endif
	}
	/// <summary> 서버 아닌 임시 테스트용 시작할때 로드 </summary>
	[System.Diagnostics.Conditional("NOT_USE_NET")]
	public void Load_UserInfo() {
		byte[] abyTemp = UTILE.GetData("UserData", ".sv", true, true);
		if (abyTemp != null) {
			// 복호화
			Utile_Class.Decode(abyTemp, abyTemp.Length, 0);
			string json = Encoding.UTF8.GetString(abyTemp);

			m_UserInfo = JsonConvert.DeserializeObject<UserInfo>(json);
			m_UserInfo.Check_AllEquipItem();
			m_UserInfo.CheckSynergy();
			m_UserInfo.SetDataBlackMarket();
			m_UserInfo.SetDataPVPStore(ShopResetType.Season);
			m_UserInfo.SetDataPVPStore(ShopResetType.DayOfWeek);
			m_UserInfo.SetDataPVPStore(ShopResetType.ZeroTime);
			m_UserInfo.SetShopData();
			m_UserInfo.SetSysMsgData();
		}
		else {
			m_UserInfo = new UserInfo();

			//최초 3개 캐릭 넣어주기
			List<int> m_GetCharIdx = new List<int>();
			for (int i = 0; i < 3; i++)
			{
				var idx = BaseValue.START_CHARACTER(i);
				if (idx < 1) continue;
				if (!m_GetCharIdx.Contains(idx))
				{
					m_GetCharIdx.Add(idx);
				}
			}
			for (int i = 0; i < m_GetCharIdx.Count; i++)
			{
				var deck = m_UserInfo.m_Deck[0];
				if (deck.m_Char[i] == 0)
				{
					deck.SetChar(i, USERINFO.InsertChar(m_GetCharIdx[i]).m_UID);
				}
			}
			m_UserInfo.CheckSynergy();
			m_UserInfo.InitBlackMarket();
			m_UserInfo.InitSysMsg();
			Save_UserInfo();
		}
		m_UserInfo.m_SupplyBoxLV = TDATA.GetSupplyBoxLV(m_UserInfo.m_Stage[StageContentType.Stage]);
	}

	IEnumerator OneDayCheck()
	{
		yield return new WaitWhile(() => m_BState < MainState.TITLE);

		// 서버시간 기중 하루를 체크해주어야함
		double day = 60 * 60 * 24;
		while (true)
		{
			yield return new WaitWhile(() => m_State < MainState.TITLE);
			double time = UTILE.Get_ServerTime();
			double temp = day - (time % day);
			yield return new WaitForSeconds((float)temp);
			m_UserInfo.OutStageCntInit();
			//m_UserInfo.Check_AdvList();
			// 하루 지남 각 상태에 알림 해줌
			m_DelegateInfo.f_DayChange?.Invoke();
		}
	}

	public void TimeSlowNFast(float _interval, float _startscale, float _endscale) {
		Time.timeScale = _startscale;
		StartCoroutine(IE_Slow(_interval, _endscale));
	}

	IEnumerator IE_Slow(float _amount, float _end) {
		yield return new WaitForSecondsRealtime(_amount);
		if (Time.timeScale < _end) {
			Time.timeScale = Math.Min(_end, Time.timeScale + _amount * 0.4f);
			StartCoroutine(IE_Slow(_amount, _end));
		}
	}
	/// <summary> 보상 팝업 호출 공용</summary>
	/// 장비-일반-캐릭터 순으로 보여줌
	public void SetRewardList(object[] _params, Action _cb, int _maxgrade = 0) {
		StartCoroutine(IE_RewardList(_params, _cb, _maxgrade));
	}
	public IEnumerator IE_RewardList(object[] _params, Action _cb, int _maxgrade = 0) {
		PopupBase popup = null;
		List<RES_REWARD_BASE> rewards = (List<RES_REWARD_BASE>)_params[0];
		List<RES_REWARD_BASE> equips = new List<RES_REWARD_BASE>();
		List<RES_REWARD_BASE> nonequips = new List<RES_REWARD_BASE>();
		List<RES_REWARD_BASE> characters = new List<RES_REWARD_BASE>();
		object[] param = _params;
		for (int i = 0; i < rewards.Count; i++) {
			switch (rewards[i].Type) {
				case Res_RewardType.Item:
					RES_REWARD_ITEM item = (RES_REWARD_ITEM)rewards[i];
					if (TDATA.GetItemTable(item.Idx).GetEquipType() != EquipType.End)
						equips.Add(rewards[i]);
					else if (rewards[i].result_code == EResultCode.SUCCESS_REWARD_PIECE) {
						characters.Add(rewards[i]);
						nonequips.Add(rewards[i]);
					}
					else nonequips.Add(rewards[i]);
					break;
				case Res_RewardType.Char:
					characters.Add(rewards[i]);
					break;
				default: nonequips.Add(rewards[i]); break;
			}
		}


		if (equips.Count > 0) {
			param = new object[2] { equips, _maxgrade };
			popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList_Equip, (result, obj) => {
				popup = null;
			}, param);

			yield return new WaitWhile(() => popup != null);
		}

		if (characters.Count > 0) {
			bool post = false;
			var items = characters.Select(o => {
				if (o.result_code == EResultCode.SUCCESS_POST) post = true;
				if (o.Type == Res_RewardType.Char) {
					RES_REWARD_CHAR info = (RES_REWARD_CHAR)o;
					return new OpenItem() { m_Type = OpenItemType.Character, m_Idx = info.Idx, m_Grade = new int[2] { info.Grade, info.Grade } };
				}
				else {
					RES_REWARD_ITEM info = (RES_REWARD_ITEM)o;
					return new OpenItem() { m_Type = OpenItemType.Item, m_Idx = info.Idx, m_Cnt = info.Cnt, m_Grade = new int[2] { info.Grade, info.Grade } };
				}
			}).ToList();
			//items = items.FindAll(o => USERINFO.GetChar(o.m_Idx) != null);
			//캐릭터는 갓챠 팝업으로
			if (items.Count > 0 && !post) {
				popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.CharDraw, (result, obj) => {

					popup = null;
				}, items);
			}
			if (post) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(535));
			yield return new WaitWhile(() => popup != null);
		}

		if (nonequips.Count > 0) {
			Dictionary<int, RES_REWARD_BASE> overlapreward = new Dictionary<int, RES_REWARD_BASE>();
			for (int i = nonequips.Count - 1; i >= 0; i--) {
				switch (nonequips[i].Type) {
					case Res_RewardType.Money:
					case Res_RewardType.Cash:
					case Res_RewardType.Exp:
					case Res_RewardType.Energy:
					case Res_RewardType.PVPCoin:
					case Res_RewardType.GCoin:
					case Res_RewardType.GPoint:
					case Res_RewardType.CampRes_Junk:
					case Res_RewardType.CampRes_Cultivate:
					case Res_RewardType.CampRes_Chemical:
					case Res_RewardType.Mileage:
					case Res_RewardType.Inven:
						RES_REWARD_MONEY money = (RES_REWARD_MONEY)nonequips[i];
						if (!overlapreward.ContainsKey(money.GetIdx())) overlapreward.Add(money.GetIdx(), money);
						else {
							((RES_REWARD_MONEY)overlapreward[money.GetIdx()]).Befor -= money.Add;
							((RES_REWARD_MONEY)overlapreward[money.GetIdx()]).Add += money.Add;
						}
						nonequips.RemoveAt(i);
						break;
					case Res_RewardType.Item:
						RES_REWARD_ITEM Item = (RES_REWARD_ITEM)nonequips[i];
						if (!overlapreward.ContainsKey(Item.GetIdx())) overlapreward.Add(Item.GetIdx(), Item);
						else {
							((RES_REWARD_ITEM)overlapreward[Item.GetIdx()]).Cnt += Item.Cnt;
						}
						nonequips.RemoveAt(i);
						break;
				}
			}
			nonequips.AddRange(overlapreward.Values);
			param[0] = nonequips;
			popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList, (result, obj) => {
				popup = null;
			}, param);

			yield return new WaitWhile(() => popup != null);
		}
		_cb?.Invoke();
	}
	public List<RES_REWARD_BASE> SetReward(RewardKind _kind, int _idx, int _cnt, bool _insert = false) {
		List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
		switch (_kind) {
			case RewardKind.Item:
				if (TDATA.GetItemTable(_idx).m_Type == ItemType.RandomBox) {//박스는 바로 까서 주기
					List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
					TItemTable itemTable = TDATA.GetItemTable(_idx);
					rewards.AddRange(TDATA.GetGachaItem(itemTable, _insert));
					for (int j = 0; j < rewards.Count; j++) {
						// 캐릭터 보상은 없음
						if (rewards[j].Type == Res_RewardType.Char) continue;
						RES_REWARD_ITEM item = (RES_REWARD_ITEM)rewards[j];
						m_Rewards.Add(rewards[j]);
					}
				}
				else {
					TItemTable itemtable = TDATA.GetItemTable(_idx);
					ItemInfo item = _insert ? USERINFO.InsertItem(_idx, _cnt) : new ItemInfo(_idx);
					switch (itemtable.m_Type) {
						case ItemType.Dollar:
							m_Rewards.Add(new RES_REWARD_MONEY() {
								Type = Res_RewardType.Money,
								Befor = USERINFO.m_Money - _cnt,
								Now = USERINFO.m_Money,
								Add = _cnt
							});
							break;
						case ItemType.Cash:
							m_Rewards.Add(new RES_REWARD_MONEY() {
								Type = Res_RewardType.Cash,
								Befor = USERINFO.m_Cash - _cnt,
								Now = USERINFO.m_Cash,
								Add = _cnt
							});
							break;
						case ItemType.Exp:
							m_Rewards.Add(new RES_REWARD_MONEY() {
								Type = Res_RewardType.Exp,
								Befor = USERINFO.m_Exp[1] - _cnt,
								Now = USERINFO.m_Exp[1],
								Add = _cnt
							});
							break;
						case ItemType.Energy:
							m_Rewards.Add(new RES_REWARD_MONEY() {
								Type = Res_RewardType.Energy,
								Befor = USERINFO.m_Energy.Cnt - _cnt,
								Now = USERINFO.m_Energy.Cnt,
								Add = _cnt
							});
							break;
						case ItemType.InvenPlus:
							m_Rewards.Add(new RES_REWARD_MONEY() {
								Type = Res_RewardType.Inven,
								Befor = USERINFO.m_InvenSize - _cnt,
								Now = USERINFO.m_InvenSize,
								Add = _cnt
							});
							break;
						default:
							m_Rewards.Add(new RES_REWARD_ITEM() {
								Type = Res_RewardType.Item,
								UID = item.m_Uid,
								Idx = item.m_Idx,
								Cnt = itemtable.GetEquipType() == EquipType.End ? _cnt : 1
							});
							break;
					}
				}
				break;
			case RewardKind.Character:
				CharInfo info = USERINFO.m_Chars.Find(t => t.m_Idx == _idx);
				if (info != null) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(186));
					ItemInfo pieceitem = _insert ? USERINFO.InsertItem(info.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(info.m_TData.m_Grade)) : new ItemInfo(info.m_TData.m_PieceIdx);
					m_Rewards.Add(new RES_REWARD_ITEM() {
						Type = Res_RewardType.Item,
						UID = pieceitem.m_Uid,
						Idx = pieceitem.m_Idx,
						Cnt = pieceitem.m_TData.GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(info.m_TData.m_Grade) : 1,
						result_code = EResultCode.SUCCESS_REWARD_PIECE
					});
				}
				else {
					CharInfo charInfo = _insert ? USERINFO.InsertChar(_idx) : new CharInfo(_idx);
					RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
					rchar.SetData(charInfo);
					m_Rewards.Add(rchar);
				}
				break;
			case RewardKind.Zombie:
				ZombieInfo zombieInfo = _insert ? USERINFO.InsertZombie(_idx) : new ZombieInfo(_idx);
				m_Rewards.Add(new RES_REWARD_ZOMBIE {
					Grade = zombieInfo.m_Grade,
					Idx = zombieInfo.m_Idx,
					UID = zombieInfo.m_UID
				});
				break;
			case RewardKind.DNA:
				DNAInfo dnaInfo = _insert ? USERINFO.InsertDNA(_idx, 1) : new DNAInfo(_idx, 1);
				m_Rewards.Add(new RES_REWARD_DNA {
					Grade = dnaInfo.m_Grade,
					Idx = dnaInfo.m_Idx,
					UID = dnaInfo.m_UID
				});
				break;
		}
		return m_Rewards;
	}
	public List<RES_REWARD_BASE> GetRewardList(List<RES_REWARD_BASE> _inlist) {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();

		for(int i = 0; i < _inlist.Count; i++) {
			RES_REWARD_BASE reward = rewards.Find(o => o.GetIdx() == _inlist[i].GetIdx());
			switch (_inlist[i].Type) {
				case Res_RewardType.Money:
				case Res_RewardType.Cash:
				case Res_RewardType.Exp:
				case Res_RewardType.Energy:
				case Res_RewardType.Inven:
					if (reward == null) rewards.Add(_inlist[i]);
					else ((RES_REWARD_MONEY)reward).Add += ((RES_REWARD_MONEY)_inlist[i]).Add;
					break;
				case Res_RewardType.UserExp:
					if (reward == null) rewards.Add(_inlist[i]);
					else ((RES_REWARD_USEREXP)reward).AExp += ((RES_REWARD_USEREXP)_inlist[i]).AExp;
					break;
				case Res_RewardType.Item:
				case Res_RewardType.Box:
					if (reward == null) rewards.Add(_inlist[i]);
					else ((RES_REWARD_ITEM)reward).Cnt += ((RES_REWARD_ITEM)_inlist[i]).Cnt;
					break;
				default:
					rewards.Add(_inlist[i]);
					break;
			}
		}
		rewards.Sort((RES_REWARD_BASE _before, RES_REWARD_BASE _after) => {
			if(_before.Type != _after.Type) return _after.Type.CompareTo(_before.Type);
			if(_before.GetGrade() != _after.GetGrade()) return _after.GetGrade().CompareTo(_before.GetGrade());
			return _after.GetIdx().CompareTo(_before.GetIdx());
		});

		return rewards;
	}
	public List<RES_REWARD_BASE> GetRewardBase(TShopTable _table, RewardKind _type) {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		for (int i = 0; i < _table.m_Rewards.Count; i++) {
			var reward = _table.m_Rewards[i];
			if (reward.m_ItemIdx == 0) continue;
			RES_REWARD_BASE res = new RES_REWARD_BASE();
			RES_REWARD_CHAR rchar;
			RES_REWARD_MONEY rmoney;
			RES_REWARD_ITEM ritem;
			RES_REWARD_DNA rDNA;
			RES_REWARD_ZOMBIE rZombie;
			RewardKind kind = reward.m_ItemType;
			switch (kind) {
				case RewardKind.None:
				case RewardKind.Event:
					return null;
				case RewardKind.Character:
					rchar = new RES_REWARD_CHAR();
					CharInfo charInfo = new CharInfo(reward.m_ItemIdx);
					rchar.SetData(charInfo);
					res = rchar;
					break;
				case RewardKind.Item:
					TItemTable tdata = TDATA.GetItemTable(reward.m_ItemIdx);
					switch (tdata.m_Type) {
						case ItemType.Dollar:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Money;
							rmoney.Befor = USERINFO.m_Money - reward.m_ItemCnt;
							rmoney.Now = USERINFO.m_Money;
							rmoney.Add = reward.m_ItemCnt;
							res = rmoney;
							break;
						case ItemType.Cash:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Cash;
							rmoney.Befor = USERINFO.m_Cash - reward.m_ItemCnt;
							rmoney.Now = USERINFO.m_Cash;
							rmoney.Add = reward.m_ItemCnt;
							res = rmoney;
							break;
						case ItemType.Energy:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Energy;
							rmoney.Befor = USERINFO.m_Energy.Cnt - reward.m_ItemCnt;
							rmoney.Now = USERINFO.m_Energy.Cnt;
							rmoney.Add = reward.m_ItemCnt;
							rmoney.STime = (long)USERINFO.m_Energy.STime;
							res = rmoney;
							break;
						case ItemType.InvenPlus:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Inven;
							rmoney.Befor = USERINFO.m_InvenSize - reward.m_ItemCnt;
							rmoney.Now = USERINFO.m_InvenSize;
							rmoney.Add = reward.m_ItemCnt;
							res = rmoney;
							break;
						case ItemType.Zombie:
							rZombie = new RES_REWARD_ZOMBIE();
							rZombie.UID = 0;
							rZombie.Idx = tdata.m_Value;
							rZombie.Grade = TDATA.GetDnaTable(rZombie.Idx).m_Grade;
							res = rZombie;
							break;
						case ItemType.DNA:
							rDNA = new RES_REWARD_DNA();
							rDNA.UID = 0;
							rDNA.Idx = tdata.m_Value;
							rDNA.Grade = TDATA.GetDnaTable(rDNA.Idx).m_Grade;
							res = rDNA;
							break;
						default:
							ritem = new RES_REWARD_ITEM();
							ritem.Type = Res_RewardType.Item;
							ritem.UID = 0;
							ritem.Idx = reward.m_ItemIdx;
							ritem.Cnt = reward.m_ItemCnt;
							res = ritem;
							break;
					}
					break;
				case RewardKind.Zombie:
					rZombie = new RES_REWARD_ZOMBIE();
					rZombie.UID = 0;
					rZombie.Idx = reward.m_ItemIdx;
					rZombie.Grade = TDATA.GetDnaTable(rZombie.Idx).m_Grade;
					res = rZombie;
					break;
				case RewardKind.DNA:
					rDNA = new RES_REWARD_DNA();
					rDNA.UID = 0;
					rDNA.Idx = reward.m_ItemIdx;
					rDNA.Grade = TDATA.GetDnaTable(rDNA.Idx).m_Grade;
					res = rDNA;
					break;
			}
			rewards.Add(res);
		}
		return rewards;
	}
	/// <summary> 스테이지 강제로 클리어 </summary>
	public void ClearCurrentStage() {
		if (STAGEINFO.m_Result == StageResult.None) {
			switch (STAGEINFO.m_StageModeType) {
				case StageModeType.Stage:
					STAGE?.StageClear();
					break;
				case StageModeType.Tower:
					TOWER?.StageClear();
					break;
				case StageModeType.Training:
					POPUP.GetMainUI().GetComponent<Training>()?.StageClear();
					break;
				case StageModeType.NoteBattle:
					BATTLE?.CheatClear();
					BATTLEINFO?.Clear();
					break;
			}
		}
	}

	/// <summary> 몇분 간격으로 시스템 메시지를 받아둔다 기본적으로 메시지는 최대 유저알림 10개 +@ Notice 타입 </summary>
	public IEnumerator GetSystemMsg()
	{
		// 타이틀 이후로 넘어갈때까지는 대기
		yield return new WaitWhile(() => m_State > MainState.TITLE);
		bool isend = false;
		WEB.SEND_REQ_SYSTEM_MSG((res) => {
			isend = true;
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			if (res.Msgs.Count > 0) {
				var check = m_SysMsgs.Count > 0 ? m_SysMsgs[0] : USERINFO.m_SysMsgInfo;
				// 중복 데이터및 이전 이전 시간의 데이터는 제거
				res.Msgs.RemoveAll(o => (o.STime < check.STime && o.UID < check.UID) || m_SysMsgs.Any(s => s.UID == o.UID));
				if (res.Msgs.Count < 1) return;
				int etccnt = m_SysMsgs.FindAll(o => o.Type != SystemMsgType.Notice).Count;
				for (int i = 0; i < res.Msgs.Count; i++) {
					// 공지사항이나 일반 알림이 10개 이하로 있으면 등록
					if(res.Msgs[i].Type == SystemMsgType.Notice || m_SysMsgs.FindAll(o => o.Type != SystemMsgType.Notice).Count < 10) m_SysMsgs.Add(res.Msgs[i]);
					// 유저알림등 일반 알림은 10개까지만 개수 체크 이전거는 지우고 최신거로 대처
					else
					{
						// 오래된 메세지는 지우기
						var msg = m_SysMsgs.Find(o => o.Type != SystemMsgType.Notice);
						m_SysMsgs.Remove(msg);
						m_SysMsgs.Add(res.Msgs[i]);
					}
				}
				// 보이는 시간 순서대로 소팅해두기
				res.Msgs.Sort((RES_SYSTEM_MSG _before, RES_SYSTEM_MSG _after) => { return _before.STime.CompareTo(_after.STime); });
			}
			// 받은 데이터의 마지막 정보
		}, m_SysMsgs.Count > 0 ? m_SysMsgs[m_SysMsgs.Count - 1].UID : USERINFO.m_SysMsgInfo.UID);

		yield return new WaitWhile(() => !isend);
	}

	public void StartSystemMsg()
	{
		//주기적으로 시스템 메시지 받아 두기
		if (m_SysMsgCor != null)
		{
			StopCoroutine(m_SysMsgCor);
		}
#if !NOT_USE_NET
		else
		{
			m_SysMsgCor = IE_GetSystemMsg();
			StartCoroutine(m_SysMsgCor);
		}
#endif
	}

	IEnumerator IE_GetSystemMsg() {
		while(true)
		{
			// 통신완료까지는 대기
			yield return GetSystemMsg();
			// 10분동안 대기(1~5분으로 바꿔도 상관없을듯)
			yield return new WaitForSecondsRealtime(600);
		}

		//if(m_SysMsgCor != null) {
		//	StopCoroutine(m_SysMsgCor);
		//}
		//m_SysMsgCor = IE_GetSystemMsg();
		//StartCoroutine(m_SysMsgCor);
	}
	public List<RES_SYSTEM_MSG> GetUserActivityMsg() {
		return m_SysMsgs.FindAll(o => o.Type != SystemMsgType.Notice);
	}
	public List<RES_SYSTEM_MSG> GetNoticeMsg() {
		return m_SysMsgs.FindAll(o => o.Type == SystemMsgType.Notice);
	}
	public void UseSysMsg(RES_SYSTEM_MSG _msg) {
		if (m_SysMsgs.Contains(_msg)) m_SysMsgs.Remove(_msg);
	}

	public RES_SYSTEM_MSG GetSysMsg()
	{
		var _msg = m_SysMsgs.Find(o => o.STime <= UTILE.Get_ServerTime_Milli());
		if (_msg == null) return null;
		m_SysMsgs.Remove(_msg);
		return _msg;
	}

	public void GoldToothStatistics(GoldToothContentsType _type, int _val = 0) {
#if NOT_USE_NET || UNITY_EDITOR
	return;
#else
		//FIREBASE.LogEvent("GoldToothStatistics", _type.ToString(), _val);
#endif
	}

	public void StartReview(Action<int> EndCB)
	{
		if (m_pReview == null)
		{
			EndCB?.Invoke(IFReview.ERROR_NOTFOUND);
			return;
		}

		m_pReview.SetReview(EndCB);
	}

	public List<RES_REWARD_BASE> GetRewardData(PostReward _reward)
	{
		return GetRewardData(_reward.Kind, _reward.Idx, _reward.Cnt);
	}
	public List<RES_REWARD_BASE> GetRewardData(RewardKind _rewardkind, int _rewardidx, int _cnt, bool _unboxing = true, bool _char4piece = true, bool _unboxingrandom = true) {
		List<RES_REWARD_BASE> m_GetRewards = new List<RES_REWARD_BASE>();
		switch (_rewardkind) {
			case RewardKind.None:
				break;
			case RewardKind.Character:
				CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == _rewardidx);
				if (charinfo != null && _char4piece) {
					RES_REWARD_BASE reward = m_GetRewards.Find(o => o.Type == Res_RewardType.Item && o.GetIdx() == charinfo.m_TData.m_PieceIdx);
					if (reward == null) {
						m_GetRewards.Add(new RES_REWARD_ITEM() {
							Type = Res_RewardType.Item,
							Idx = charinfo.m_TData.m_PieceIdx,
							Cnt = TDATA.GetItemTable(charinfo.m_TData.m_PieceIdx).GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
							result_code = EResultCode.SUCCESS_REWARD_PIECE
						});
					}
					else
						((RES_REWARD_ITEM)reward).Cnt += TDATA.GetItemTable(charinfo.m_TData.m_PieceIdx).GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1;
				}
				else {
					CharInfo charInfo = new CharInfo(_rewardidx);
					RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
					rchar.SetData(charInfo);
					m_GetRewards.Add(rchar);
				}
				break;
			case RewardKind.Item:
				var itemTable = TDATA.GetItemTable(_rewardidx);
				if (((itemTable.m_Type == ItemType.RandomBox && _unboxingrandom) || itemTable.m_Type == ItemType.AllBox) && _unboxing) {//박스는 바로 까서 주기
					List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
					GachaGroup gachagroup = TDATA.GetGachaGroup(itemTable.m_Value);
					List<RES_REWARD_BASE> items = TDATA.GetGachaItemList(itemTable, _char4piece);

					for (int j = 0; j < items.Count; j++) {
						RES_REWARD_BASE reward = rewards.Find((t) => t.GetIdx() == items[j].GetIdx());
						RES_REWARD_BASE rewardprefab = m_GetRewards.Find((t) => t.GetIdx() == items[j].GetIdx());

						if (reward == null || rewardprefab == null)
							rewards.Add(items[j]);
						else {
							((RES_REWARD_ITEM)reward).Cnt += ((RES_REWARD_ITEM)items[j]).Cnt;
						}
					}

					for (int j = 0; j < rewards.Count; j++) {
						// 캐릭터 보상은 없음
						if (rewards[j].Type == Res_RewardType.Char) {
							m_GetRewards.AddRange(GetRewardData(RewardKind.Character, rewards[j].GetIdx(), 1, _unboxing, _char4piece, _unboxingrandom));
						}
						else if (rewards[j].Type != Res_RewardType.Item)
							m_GetRewards.Add(rewards[j]);
						else {
							RES_REWARD_BASE reward = m_GetRewards.Find(o => o.Type == rewards[j].Type && o.GetIdx() == rewards[j].GetIdx());
							if (reward == null) {
								m_GetRewards.Add(rewards[j]);
							}
							else {
								((RES_REWARD_ITEM)reward).Cnt += ((RES_REWARD_ITEM)rewards[j]).Cnt;
							}
						}
					}
				}
				else {
					TItemTable tdata = TDATA.GetItemTable(_rewardidx);
					RES_REWARD_MONEY rmoney;
					RES_REWARD_ITEM ritem;
					switch (tdata.m_Type) {
						case ItemType.Exp:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Exp;
							rmoney.Befor = USERINFO.m_Exp[0] - _cnt;
							rmoney.Now = USERINFO.m_Exp[0];
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Dollar:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Money;
							rmoney.Befor = USERINFO.m_Money - _cnt;
							rmoney.Now = USERINFO.m_Money;
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Cash:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Cash;
							rmoney.Befor = USERINFO.m_Cash - _cnt;
							rmoney.Now = USERINFO.m_Cash;
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Energy:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Energy;
							rmoney.Befor = USERINFO.m_Energy.Cnt - _cnt;
							rmoney.Now = USERINFO.m_Energy.Cnt;
							rmoney.Add = _cnt;
							rmoney.STime = (long)USERINFO.m_Energy.STime;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.InvenPlus:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Inven;
							rmoney.Befor = USERINFO.m_InvenSize - _cnt;
							rmoney.Now = USERINFO.m_InvenSize;
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.PVPCoin:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.PVPCoin;
							rmoney.Befor = USERINFO.m_PVPCoin - _cnt;
							rmoney.Now = USERINFO.m_PVPCoin;
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Guild_Coin:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.GCoin;
							rmoney.Befor = USERINFO.m_GCoin - _cnt;
							rmoney.Now = USERINFO.m_GCoin;
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Mileage:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Mileage;
							rmoney.Befor = USERINFO.m_Mileage - _cnt;
							rmoney.Now = USERINFO.m_Mileage;
							rmoney.Add = _cnt;
							m_GetRewards.Add(rmoney);
							break;
						default:
							RES_REWARD_BASE reward = m_GetRewards.Find(o => o.Type == Res_RewardType.Item && o.GetIdx() == _rewardidx);
							if (reward == null) {
								ritem = new RES_REWARD_ITEM();
								ritem.Type = Res_RewardType.Item;
								ritem.UID = 0;
								ritem.Idx = _rewardidx;
								ritem.Cnt = _cnt;
								m_GetRewards.Add(ritem);
							}
							else {
								((RES_REWARD_ITEM)reward).Cnt += _cnt;
							}
							break;
					}
					break;
				}
				break;
			case RewardKind.Zombie:
				ZombieInfo zombieInfo = new ZombieInfo(_rewardidx);
				RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
				zombie.UID = zombieInfo.m_UID;
				zombie.Idx = zombieInfo.m_Idx;
				zombie.Grade = zombieInfo.m_Grade;
				m_GetRewards.Add(zombie);
				break;
			case RewardKind.DNA:
				DNAInfo dnaInfo = new DNAInfo(_rewardidx);
				RES_REWARD_DNA dna = new RES_REWARD_DNA();
				dna.UID = dnaInfo.m_UID;
				dna.Idx = dnaInfo.m_Idx;
				dna.Grade = dnaInfo.m_Grade;
				m_GetRewards.Add(dna);
				break;
		}

		return m_GetRewards;
	}
	public void GoPDAMaking(Item_PDA_Menu _pda, int _val) {
		StartCoroutine(GoPDAMakingAction(_pda, _val));
	}
	IEnumerator GoPDAMakingAction(Item_PDA_Menu _pda, int _val) {
		yield return new WaitWhile(() => _pda.GetMenuObj == null);
		yield return new WaitWhile(() => _pda.GetMenuObj.GetComponent<Item_PDA_Making>().GetState != Item_PDA_Making.State.Main);
		yield return new WaitForEndOfFrame();
		switch (_val) {
			case 0://아무장비
			case 1://장비생산
				_pda.GetMenuObj.GetComponent<Item_PDA_Making>().GetMenu.GetComponent<Item_PDA_Making_Main>().ChangePage(0, true);
				break;
			case 2://전용 장비
				_pda.GetMenuObj.GetComponent<Item_PDA_Making>().GetMenu.GetComponent<Item_PDA_Making_Main>().ChangePage(1, true);
				break;
			case 3://연구 재료
				_pda.GetMenuObj.GetComponent<Item_PDA_Making>().GetMenu.GetComponent<Item_PDA_Making_Main>().ChangePage(2, true);
				break;
			case 4://생산 재료
				_pda.GetMenuObj.GetComponent<Item_PDA_Making>().GetMenu.GetComponent<Item_PDA_Making_Main>().ChangePage(3, true);
				break;
		}
	}

#region GrowthWay
	public void GoGrowthWay(GrowthWayType type, int charidx)
	{
		if(!PlayMng.IsValid())
		{
			return;
		}

		StartCoroutine(MoveGrowthWay(type, charidx));
	}
	IEnumerator MoveGrowthWay(GrowthWayType type, int charidx)
	{
		switch (type)
		{
		/// <summary> 캐릭터 레벨 업 가능 시, 캐릭터 장비 레벨 업 가능 시, 캐릭터 승급 가능 시 </summary>
		case GrowthWayType.CharacterUp: yield return GoCharInfo(charidx); break;
		/// <summary> 현재 플레이 할 수 있는 다운타운 스테이지가 있을 경우 </summary>
		case GrowthWayType.GetMaterial: yield return GoDownTown(); break;
		/// <summary> 상점은 알람에서 제외 </summary>
		case GrowthWayType.Shop: yield return GoShop(); break;
		case GrowthWayType.Replay: yield return GoRePlay(); break;
		default: POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(185)); break;
		}
	}

	IEnumerator GoCharInfo(int charidx)
	{
		if (!USERINFO.CheckContentUnLock(ContentType.Character, true)) yield break;
		POPUP.Init_PopupUI();
		Main_Play mainui = (Main_Play)POPUP.GetMainUI();
		mainui.ClickMenuButton((int)MainMenuType.Character);
		if (charidx > 0)
		{
			Item_CharManageCard card = ((Main_Play)POPUP.GetMainUI()).GetSrvMng.GetCharCard(charidx);
			card.OpenDetailStrSol();
		}
		yield break;
	}
	IEnumerator GoDownTown()
	{
		if (!USERINFO.CheckContentUnLock(ContentType.Factory, true)) yield break;
		POPUP.Init_PopupUI();
		Main_Play mainui = (Main_Play)POPUP.GetMainUI();
		mainui.ClickMenuButton((int)MainMenuType.Dungeon);
		yield break;
	}
	IEnumerator GoShop()
	{
		if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) yield break;
		POPUP.Init_PopupUI();
		Main_Play mainui = (Main_Play)POPUP.GetMainUI();
		mainui.ClickMenuButton((int)MainMenuType.Shop);
		yield break;
	}
	IEnumerator GoRePlay()
	{
		if (!USERINFO.CheckContentUnLock(ContentType.Replay, true)) yield break;
		POPUP.Init_PopupUI();
		Main_Play mainui = (Main_Play)POPUP.GetMainUI();
		mainui.GetStgMenu.Click_Replay();
		yield break;
	}
#endregion
}