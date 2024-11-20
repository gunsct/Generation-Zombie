using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;
using static UserInfo;

[System.Serializable] public class DicIconMenuUI : SerializableDictionary<Main_Play.IconName, Main_Play.SIconMenuUI> { }

public class Main_Play : PopupBase
{
	public enum IconName
	{
		/// <summary> 공지 </summary>
		Notice = 0,
		/// <summary> 인벤 </summary>
		Inven,
		/// <summary> 우편 </summary>
		Post,
		/// <summary> 챌린지 </summary>
		Challenge,
		/// <summary> 친구 </summary>
		Friend,
		/// <summary> 미션 </summary>
		Mission,
		/// <summary> 시즌패스 </summary>
		SeasonPass,
		/// <summary> 추천상품 </summary>
		Products,
		/// <summary> 연합 </summary>
		Union,
		/// <summary> 긴급 임무 </summary>
		Replay,
		/// <summary> 출석 체크 </summary>
		Attendance,
		/// <summary> 월정액 </summary>
		Monthly,
		/// <summary>  </summary>
		End
	}

	public enum AniName
	{
		Start = 0,
		StageIn,
		DiffChange
	}
	[System.Serializable]
	public struct SGuideQuestUI
	{
		public GameObject Active;
		public GameObject Success;
		public Animator Ani;
		public TextMeshProUGUI Label;
		public TextMeshProUGUI[] Title;
		public TextMeshProUGUI Cnt;
		public Item_RewardList_Item Reward;
		[HideInInspector] int State;
		[HideInInspector] public GuidQuestInfo Info;
	}

	[System.Serializable]
	public struct SIconMenuUI
	{
		public GameObject Active;
		public GameObject Alram;
		public STimerUI Time;
		public GameObject Name;
	}
	[System.Serializable]
	public struct STimerUI
	{
		public GameObject Active;
		public Image BG;
		public Color[] BG_Color;
		public Image Icon;
		public Color[] Icon_Color;
		public TextMeshProUGUI Time;
		public Color[] Text_Color;
	}

	[System.Serializable]
	public struct SUI {
		public Animator Anim;
		public GameObject RightMenu;
		public GameObject LeftMenu;
		public Animator NoticeBtnAnim;
		public GameObject[] MenuPanel;
		public Item_MenuGroup MenuBtns;
		public Item_PDA_Menu PDA;
		public Item_DungeonMenu Dungeon;
		public Item_MainMenu_StgMain Stage;
		public Shop Shop;
		public RectTransform StgFrameMask;
		public RectTransform StgFrameMaskChange;
		public Item_CharManagement SrvMng;
		public Animator StageBtnAnim;
		public Transform BGParent;
		public Item_UserActivity_Alarm UserActivityAlarm;
		public Item_RecommendGoodsBanner RecommendGoodsBanner;
		public Transform EventBtnGroup;

		public DicIconMenuUI IconMenu;

		public CanvasGroup[] StartAlpha;
		public GameObject StartBlack;

		public SGuideQuestUI GuideQuest;
		[Header("튜토리얼 전용")]
		public GameObject[] Panels;
	}
	[Header("*메인 켜질때 검정 깜빡임 관련* \n Item_MainMenu_StgMain 하위 Black 은 켜져있어야함 \n Item_MenuGroup, LeftMenuGroup, \n LightMenuGroup 의 CanvasGroup Alpha는 0이어야함. \n StartBlack 은 켜져있어야함")]
	[Space(10)]
	[SerializeField]
	SUI m_SUI;
	public MainMenuType m_State;
	public MainMenuType GetMainUIState { get { return m_State; } }
	Item_MainMenu_Stg_Bg m_BG;

	public bool IsAction { get { return m_Action != null && IsStartEnd; } }
	bool IsStartEnd = false;
	bool IsTutoStartCheck = false;

	IEnumerator m_Action;
	Coroutine m_ChallengeChecker;
	//스테이지 별도 보상용
	List<RES_REWARD_BASE> m_GetRewards = new List<RES_REWARD_BASE>();

	float[] m_IdleTimer = new float[2];
	List<SND_IDX> m_LoopSNDM = new List<SND_IDX>();
	List<SND_IDX> m_LoopSNDF = new List<SND_IDX>();

	public Item_MenuGroup GetMenuGroup {
		get { return m_SUI.MenuBtns; }
	}
	public Item_CharManagement GetSrvMng { get { return m_SUI.SrvMng; } }
	public Item_MainMenu_StgMain GetStgMenu { get { return m_SUI.Stage; } }

	private void Awake() {
		m_SUI.StartBlack.SetActive(true);
		for (int i = 0; i < m_SUI.StartAlpha.Length; i++) m_SUI.StartAlpha[i].alpha = 0f;

		if (MainMng.IsValid()) {
			DLGTINFO.f_RFInvenAlarm += SetInvenNewAlram;
			DLGTINFO.f_RFMissionAlarm += SetMissionAlram;
			DLGTINFO.f_RFGuidQuestUI += SetGuideQuestUI;
		}

		m_SUI.UserActivityAlarm.gameObject.SetActive(false);

		for (SND_IDX i = SND_IDX.VOC_3001; i <= SND_IDX.VOC_3040; i++) m_LoopSNDM.Add(i);
		for (SND_IDX i = SND_IDX.VOC_3101; i <= SND_IDX.VOC_3140; i++) m_LoopSNDF.Add(i);

	}

	public void SetAnim(string _trig) {
		m_SUI.Anim.SetTrigger(_trig);
	}

	IEnumerator Start() {
		IsTutoStartCheck = false;
		m_IdleTimer[0] = 0f;
		m_IdleTimer[1] = UTILE.Get_Random(15f, 25f);
		SetGuideQuest(false);
		SetInvenNewAlram(false);
		SetPostAlram();
		SetMissionAlram();
		SetSeasonPassAlram();
		SetFriendAlram();
		SetUnionAlram();
		SetAttendanceAlram();
		SetMonthlyAlram();

		MainMenuType premenu = STAGEINFO.GetPreMenu();
		MenuChange((int)premenu);
		if (STAGEINFO.m_StageContentType == StageContentType.PvP && MAIN.GetBackState() == MainState.PVP) {
			m_SUI.Dungeon.ClickGoDungeon((int)StageContentType.PvP, true);
		}
		else if (premenu == MainMenuType.Dungeon) {
			if(STAGEINFO.m_StageContentType != StageContentType.Tower && STAGEINFO.m_StageContentType != StageContentType.Subway) 
				STAGEINFO.m_LastPlayLv = STAGEINFO.m_LV;
			m_SUI.Dungeon.ClickGoDungeon((int)STAGEINFO.m_StageContentType, true);
			STAGEINFO.m_LastPlayLv = -1;
			STAGEINFO.m_LastLv = -1;
		}
		else if (STAGEINFO.m_PlayType == StagePlayType.Event && STAGEINFO.EUID != 0) {
			MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => o.UID == STAGEINFO.EUID);
			if (evt != null)
			{
				Click_Event(evt);
				PopupBase evtpopup = POPUP.GetPopup();
				switch (evtpopup.m_Popup)
				{
				case PopupName.Event_10:
					evtpopup.GetComponent<Event_10>().Click_GoStage();
					break;
				case PopupName.Event_11:
					evtpopup.GetComponent<Event_11>().Click_GoStage();
					break;
				}
			}
		}
		else if (STAGEINFO.m_StageContentType == StageContentType.Replay || STAGEINFO.m_StageContentType == StageContentType.ReplayHard || STAGEINFO.m_StageContentType == StageContentType.ReplayNight) {
			m_SUI.Stage.Click_Replay();
		}

		Stage stg = USERINFO.m_Stage[StageContentType.Stage];
		switch (STAGEINFO.m_PlayType)
		{
		case StagePlayType.Stage:
			StageIdx stgidx = stg.Idxs[USERINFO.GetDifficulty()];
			if (STAGEINFO.m_Idx != stgidx.Idx)
			{
				if (STAGEINFO.m_Idx / 100 == stgidx.Idx / 100)
				{//챕터 안바뀐 경우
					if (USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].ChapterReward == 0) {
						m_SUI.Stage.SetStageChage(stgidx.Idx, false);
					}
				}
				else
				{
					//챕터 바뀜
					m_SUI.Stage.SetChapterChange(stgidx.Idx);
				}
			}
			else
			{
				m_SUI.Stage.InMenu(stg.IS_LastStage());
			}
			m_SUI.StageBtnAnim.SetTrigger("Selected");
			break;
		default:
			m_SUI.Stage.InMenu(stg.IS_LastStage());
			break;
		}

		//버튼 잠금 표기
		m_SUI.MenuBtns.SetData();

		m_SUI.Stage.SetStartBtnFx(false);
		m_SUI.Anim.speed = 1;
		Utile_Class.AniResetAllTriggers(m_SUI.Anim);
		m_SUI.Anim.SetTrigger(AniName.Start.ToString());
		// 이전에는 한프레임 쉬어주면 트리거 전환이 됐었는데 체크가 안되서 해당 애니 이름인지 확인해서 넘겨줌
		yield return new WaitWhile(() => { return !m_SUI.Anim.GetCurrentAnimatorStateInfo(0).IsName("MainStart"); });

		if (STAGEINFO.m_Result == StageResult.Fail && PlayerPrefs.GetInt($"GrowthWay_{USERINFO.m_UID}") == 1) {
			PlayerPrefs.SetInt($"GrowthWay_{USERINFO.m_UID}", 0);
			m_SUI.Stage.Idle(stg.IS_LastStage());
			Utile_Class.AniSkip(m_SUI.Anim);
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.GrowthWay);
			IsTutoStartCheck = true;
			IsStartEnd = true;
			m_SUI.Stage.SetStartBtnFx(true);
			m_Action = null;
			yield break;
		}

		bool allactionend = true;
		int chapreward = stg.Idxs[USERINFO.GetDifficulty()].ChapterReward;
		if (chapreward != 0) {
			allactionend = false;
			StageBonusReward(() => {
				POPUP.SetLoadingFade(UIMng.LoadingType.Black);
				allactionend = true;
			});
		}
		yield return new WaitWhile(() => !allactionend);
		//보급상자 레벨업 체크
		POPUP.SetLoadingFade(UIMng.LoadingType.FadeOut);
		int presupplylv = USERINFO.m_SupplyBoxLV;
		int nowsupplylv = TDATA.GetSupplyBoxLV(USERINFO.m_Stage[StageContentType.Stage]);
		if (presupplylv != nowsupplylv) {
			allactionend = false;
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.SupplyBox_LvUp, (result, obj) => {
				USERINFO.m_SupplyBoxLV = nowsupplylv;
				POPUP.SetLoadingFade(UIMng.LoadingType.Black);
				allactionend = true;
			}, presupplylv, nowsupplylv);
		}
		yield return new WaitWhile(() => !allactionend);
		//프로필 생성 체크
		POPUP.SetLoadingFade(UIMng.LoadingType.FadeOut);
		if (stg.Idxs[USERINFO.GetDifficulty()].Idx >= 201 && (USERINFO.IsFirstNameSet || USERINFO.m_Profile == 0)) {
			allactionend = false;
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.UserProfile, (result, obj) => {
				POPUP.SetLoadingFade(UIMng.LoadingType.Black);
				allactionend = true;
				DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
			}, true);
		}
		yield return new WaitWhile(() => !allactionend);

		POPUP.SetLoadingFade(UIMng.LoadingType.FadeOut);

		AnimatorStateInfo info = m_SUI.Anim.GetCurrentAnimatorStateInfo(0);
		// 시작 연출 타임
		float actiontime = 2f / info.length;
		yield return new WaitWhile(() => {
			info = m_SUI.Anim.GetCurrentAnimatorStateInfo(0);
			return info.normalizedTime < actiontime;
		});

		// 일부 멈춤
		m_SUI.Anim.speed = 0;

		yield return new WaitWhile(() => !allactionend);
		yield return new WaitWhile(() => POPUP.IS_Connecting());//추가이벤트 습격 때문, 전투씬에서 전환했을때
		//추가 이벤트 안한 경우
		if (USERINFO.m_AddEvent != 0) {
			m_SUI.Anim.speed = 1;
			allactionend = false;
			PopupBase block = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TouchBlock);
			m_SUI.Stage.SetAddEventAnim(true, () => {
				block.Close();
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.AddEvent, (result, obj) => {
					block = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TouchBlock);
					if (m_SUI.Stage != null) {
						m_SUI.Stage.SetAddEventAnim(false, () => {
							block.Close();
							allactionend = true;
						});
					}
					else {
						block.Close();
						allactionend = true;
					}

					USERINFO.m_AddEvent = 0;
#if NOT_USE_NET
					MAIN.Save_UserInfo();
#endif
				}, USERINFO.m_AddEvent);
			});
		}

		yield return new WaitWhile(() => !allactionend);
		m_SUI.Anim.speed = 0;

		yield return new WaitForFixedUpdate();
		yield return new WaitWhile(() => POPUP.IS_PopupUI());

		TUTO.Start(TutoStartPos.PlayStart);
		IsTutoStartCheck = true;
		if (!TUTO.IsTutoPlay() && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= 201) yield return CheckPopupAction();

		//이벤트 버튼 세팅
		SetEventBtn();
		//긴급임무 버튼 세팅

		//신규 패키지 배너 띄우기
		if (!TUTO.IsTutoPlay()) m_SUI.RecommendGoodsBanner.CheckNewGoods();

		StartCoroutine(UserActivityLoop());

		m_SUI.Anim.speed = 1;
		IsStartEnd = true;
		m_SUI.Stage.SetStartBtnFx(true);
		m_Action = null;
	}
	public bool OnBack()
	{
		if (m_Action != null) return false;
		switch(m_State)
		{
		case MainMenuType.PDA:
			return m_SUI.PDA.OnBack();
		}
		return false;
	}

	private void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFInvenAlarm -= SetInvenNewAlram; 
			DLGTINFO.f_RFMissionAlarm -= SetMissionAlram;
			DLGTINFO.f_RFGuidQuestUI -= SetGuideQuestUI;
		}
		if(m_ChallengeChecker != null) StopCoroutine(m_ChallengeChecker);
	}
	private void Update() {
		SetSeasonPassTime();
		SetMonthlyTime();
		LoopSND();
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
#if NOT_USE_NET
		SetIconMenu(IconName.Challenge, false);
		SetIconMenu(IconName.Post, false);
#else
		SetIconMenu(IconName.Challenge, true);
		SetIconMenu(IconName.Post, true);
#endif
#if NOT_USE_GUILD
		SetIconMenu(IconName.Union, false);
#else
		SetIconMenu(IconName.Union, TDATA.GetConfig_Int32(ConfigType.GuildOpen) <= USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx);
#endif
		int dailylock = TDATA.GetConfig_Int32(ConfigType.DailyQuestOpen);
		SetIconMenu(IconName.Mission, dailylock <= USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx);

		SetIconMenu(IconName.Attendance, !USERINFO.IsFirstNameSet && USERINFO.m_Event.GetViewAttEvent().Count > 0);

		RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		bool issell = USERINFO.m_ShopInfo.Infos.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE) == null;
		SetIconMenu(IconName.Monthly, issell && rginfo != null && packinfo != null && packinfo.GetLastTime() * 0.001d <= 86400 * 3);
	}

	public override void SetUI()
	{
		base.SetUI();
		UTILE.GetServerDayofWeek();
		SetBG();
	}
	public void SetIconMenu(IconName icon, bool active)
	{
		if(m_SUI.IconMenu[icon].Active.activeSelf != active) m_SUI.IconMenu[icon].Active.SetActive(active);
	}

	public void SetAlram(IconName icon, bool active)
	{
		m_SUI.IconMenu[icon].Alram.SetActive(active);
	}

	IEnumerator CheckPopupAction()
	{
		MyChallenge challenge = USERINFO.m_MyChallenge;


		Sprite blurimg = null;

		// 챌린지 정보 알림
		// Befor == null 이면 정보없음
		if (challenge.Befor != null)
		{
			if (challenge.Befor != null
				&& !PlayerPrefs.GetInt($"Challenge_Reward_{USERINFO.m_UID}", 0).Equals(challenge.Befor.No))
			{
				PlayerPrefs.SetInt($"Challenge_Reward_{USERINFO.m_UID}", challenge.Befor.No);
				PlayerPrefs.Save();
				if (blurimg == null)
				{
					//yield return UTILE.GetCaptureBlurSprite((img) => { blurimg = img; }, 15);
					yield return UTILE.GetCaptureResizeSprite((img) => { blurimg = img; }, 0.025f);
				}
				// 결과 보상 알림 1회 알림
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_EndReward, null, challenge.Befor, blurimg);
				yield return new WaitWhile(() => POPUP.IS_PopupUI());
			}
		}

		if (challenge.WeekEnd.Count > 0 && PlayerPrefs.GetInt($"Challenge_Reward_Week_{USERINFO.m_UID}", 0) != challenge.WeekEnd[0].No)
		{
			PlayerPrefs.SetInt($"Challenge_Reward_Week_{USERINFO.m_UID}", challenge.WeekEnd[0].No);
			PlayerPrefs.Save();
			var cnt = challenge.WeekEnd.Count(o => o.MyInfo == null || o.MyInfo.Point == 0);
			if(cnt < challenge.WeekEnd.Count)
			{
				if (blurimg == null)
				{
					//yield return UTILE.GetCaptureBlurSprite((img) => { blurimg = img; }, 15);
					yield return UTILE.GetCaptureResizeSprite((img) => { blurimg = img; }, 0.025f);
				}
				// 결과 보상 알림 1회 알림
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_EndReward_Week, (res, obj)=> {
					//*팝업 너무 많이 뜬다는 GVI요청
					//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Start_Week, null, challenge.Week[0], blurimg);
				}, challenge.WeekEnd, blurimg);
				yield return new WaitWhile(() => POPUP.IS_PopupUI());
			}
		}

		//*팝업 너무 많이 뜬다는 GVI요청
		//ChallengeType next = challenge.Next;
		//// 챌린지 변경이 아닌한 한번만 보여주기 위해
		//bool isViewPopup = challenge.Now != null
		//	&& !PlayerPrefs.GetInt($"Challenge_Now_{USERINFO.m_UID}", 0).Equals(challenge.Now.No)
		//	&& challenge.Now.Times[1] > UTILE.Get_ServerTime_Milli();

		//if (isViewPopup && challenge.Now.No > 0)
		//{
		//	PlayerPrefs.SetInt($"Challenge_Now_{USERINFO.m_UID}", challenge.Now.No);
		//	PlayerPrefs.Save();
		//	if(blurimg == null)
		//	{
		//		//yield return UTILE.GetCaptureBlurSprite((img) => { blurimg = img; }, 15);
		//		yield return UTILE.GetCaptureResizeSprite((img) => { blurimg = img; }, 0.025f);
		//	}

		//	// 챌린지 변경 알림
		//	POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Start, null, challenge.Now, blurimg);
		//	yield return new WaitWhile(() => POPUP.IS_PopupUI());
		//}

		//// 다음 챌린지 정보(현재 챌린지를 보고난 다음이거나 다음 챌린지 정보가 달라졌을때)
		//if (challenge.Next != ChallengeType.END && (isViewPopup || !PlayerPrefs.GetString($"Challenge_Next_{USERINFO.m_UID}", "").Equals(challenge.Next.ToString())))
		//{
		//	POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Next, null, challenge.Next, challenge.NextSTime);
		//	yield return new WaitWhile(() => POPUP.IS_PopupUI());
		//}

		//// 다음에 노출되지 않도록 저장
		//PlayerPrefs.SetString($"Challenge_Next_{USERINFO.m_UID}", challenge.Next.ToString());
		//PlayerPrefs.Save();

#if !NOT_USE_NET
		// 하루 지났을때만 노출
		if (USERINFO.m_Event.IsDataUp() && !USERINFO.IsFirstNameSet)
		{
			// 이벤트 관련 프로세스 체크
			// SEND_REQ_MY_FAEVENT_INFO 호출시 새로운 이벤트 체크및 생성되서 데이터 받음
			// SEND_REQ_CHECK_MY_FAEVENT_INFO를 호출해서 출첵을 함(보상받기)
			// TODO 아래거는 여기서 빼고 툴데이터 로드 한 다음에 한번 해주는거로 옮기기
			bool check = false;
			USERINFO.m_Event.Load((res) => { check = true; });
			yield return new WaitUntil(() => check);

			var checklist = USERINFO.m_Event.GetViewAttEvent();
			SetIconMenu(IconName.Attendance, checklist.Count > 0);
			// 체크 대상이 있을때
			check = checklist.Any(o => o.IsViewCheck());
			if (check)
			{
				// 이벤트 팝업 보여주기
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.FAEvent, (result, obj) => SetAttendanceAlram(), true);
			}
			yield return new WaitWhile(() => POPUP.IS_PopupUI());

		}
		// 실시간 체크에서 이동
		else
		{
			USERINFO.m_Event.AutoCheck();
#if EVENT_TIME_TEST
			USERINFO.m_Event.Load((res) => { }, false);
#endif
		}
#endif
		SetMonthPackage();

		var daytime = (int)(UTILE.Get_ServerTime() / 86400);
		// 대배너 띄우기 로그인후 진입이거나 일일 한번만
		if (MAIN.IS_BackState(MainState.TITLE) || PlayerPrefs.GetInt($"PromotionTime", daytime) != daytime)
		{
			HIVE.ShowPromotion(hive.PromotionType.BANNER, false, (result, type) =>{});
			PlayerPrefs.SetInt($"PromotionTime", daytime);
			PlayerPrefs.Save();
		}
	}
	/// <summary> 유저 활동 </summary>
	void CheckUserActivity() {
		if (TUTO.IsTutoPlay()) return;

		m_SUI.UserActivityAlarm.gameObject.SetActive(true);
		m_SUI.NoticeBtnAnim.SetTrigger("UserAlarm");
		m_SUI.UserActivityAlarm.SetData(()=> { m_SUI.NoticeBtnAnim.SetTrigger("Normal"); });
	}
	IEnumerator UserActivityLoop() {
		yield return new WaitWhile(() => m_SUI.UserActivityAlarm.gameObject.activeSelf);
		yield return new WaitWhile(() => m_State != MainMenuType.Stage);
		yield return new WaitWhile(() => POPUP.IS_PopupUI());
		// 스테이지 진입 짧은 시간에 들어오는 경우가 있어 패스
		//yield return new WaitWhile(() => !MAIN.IS_State(MainState.PLAY));
		CheckUserActivity();
		yield return new WaitForSeconds(UTILE.Get_Random(40, 50));
		StartCoroutine(UserActivityLoop());
	}
	void StageBonusReward(Action _cb) {
#if NOT_USE_NET
		StageIdx stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		TChapterTable chaptable = TDATA.GetChapterTable(USERINFO.GetDifficulty(), stgidx.ChapterReward);
		if (chaptable == null) return;
		stgidx.ChapterReward = 0;
		//프리랑 상의해서 챕터보상으로 아이템 외의 것도 줄거면 테이블에 타입 하나 받기
		RewardKind kind = chaptable.m_RewardType;
		//chaptable
		switch (kind) {
			case RewardKind.None:
				break;
			case RewardKind.Character:
				CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == chaptable.m_Reward);
				if (charinfo != null) {
					//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(186));
					ItemInfo pieceitem = USERINFO.InsertItem(charinfo.m_TData.m_PieceIdx, BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade));
					m_GetRewards.Add(new RES_REWARD_ITEM() {
						Type = Res_RewardType.Item,
						UID = pieceitem.m_Uid,
						Idx = pieceitem.m_Idx,
						Cnt = pieceitem.m_TData.GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
						result_code = EResultCode.SUCCESS_REWARD_PIECE
					});
				}
				else {
					CharInfo charInfo = USERINFO.InsertChar(chaptable.m_Reward);
					RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
					rchar.SetData(charInfo);
					m_GetRewards.Add(rchar);
				}
				break;
			case RewardKind.Item:
				if (TDATA.GetItemTable(chaptable.m_Reward).m_Type == ItemType.RandomBox) {//박스는 바로 까서 주기
					List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
					TItemTable itemTable = TDATA.GetItemTable(chaptable.m_Reward);
					for (int j = chaptable.m_RewardCount - 1; j > -1; j--) rewards.AddRange(TDATA.GetGachaItem(itemTable));
					for (int j = 0; j < rewards.Count; j++) {
						// 캐릭터 보상은 없음
						if (rewards[j].Type == Res_RewardType.Char) continue;
						RES_REWARD_ITEM item = (RES_REWARD_ITEM)rewards[j];
						m_GetRewards.Add(rewards[j]);
					}
				}
				else {
					ItemInfo iteminfo = USERINFO.InsertItem(chaptable.m_Reward, chaptable.m_RewardCount);
					TItemTable tdata = TDATA.GetItemTable(chaptable.m_Reward);
					RES_REWARD_MONEY rmoney;
					RES_REWARD_ITEM ritem;
					switch (tdata.m_Type) {
						case ItemType.Dollar:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Money;
							rmoney.Befor = USERINFO.m_Money - chaptable.m_RewardCount;
							rmoney.Now = USERINFO.m_Money;
							rmoney.Add = chaptable.m_RewardCount;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Cash:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Cash;
							rmoney.Befor = USERINFO.m_Cash - chaptable.m_RewardCount;
							rmoney.Now = USERINFO.m_Cash;
							rmoney.Add = chaptable.m_RewardCount;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.Energy:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Energy;
							rmoney.Befor = USERINFO.m_Energy.Cnt - chaptable.m_RewardCount;
							rmoney.Now = USERINFO.m_Energy.Cnt;
							rmoney.Add = chaptable.m_RewardCount;
							rmoney.STime = (long)USERINFO.m_Energy.STime;
							m_GetRewards.Add(rmoney);
							break;
						case ItemType.InvenPlus:
							rmoney = new RES_REWARD_MONEY();
							rmoney.Type = Res_RewardType.Inven;
							rmoney.Befor = USERINFO.m_InvenSize - chaptable.m_RewardCount;
							rmoney.Now = USERINFO.m_InvenSize;
							rmoney.Add = chaptable.m_RewardCount;
							m_GetRewards.Add(rmoney);
							break;
						default:
							ritem = new RES_REWARD_ITEM();
							ritem.Type = Res_RewardType.Item;
							ritem.UID = iteminfo.m_Uid;
							ritem.Idx = chaptable.m_Reward;
							ritem.Cnt = chaptable.m_RewardCount;
							m_GetRewards.Add(ritem);
							break;
					}
					break;
				}
				break;
			case RewardKind.Zombie:
				ZombieInfo zombieInfo = USERINFO.InsertZombie(chaptable.m_Reward);
				RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
				zombie.UID = zombieInfo.m_UID;
				zombie.Idx = zombieInfo.m_Idx;
				zombie.Grade = zombieInfo.m_Grade;
				m_GetRewards.Add(zombie);
				break;
			case RewardKind.DNA:
				TDnaTable dnaTable = TDATA.GetDnaTable(chaptable.m_Reward);
				DNAInfo dnaInfo = new DNAInfo(dnaTable.m_Idx);
				USERINFO.m_DNAs.Add(dnaInfo);
				RES_REWARD_DNA dna = new RES_REWARD_DNA();
				dna.UID = dnaInfo.m_UID;
				dna.Idx = dnaInfo.m_Idx;
				dna.Grade = dnaInfo.m_Grade;
				m_GetRewards.Add(dna);
				break;
		}

		MAIN.Save_UserInfo();

		ViewChapterReward(_cb);
#else
		SendStageChapterReward((res) => {
			USERINFO.SetDATA(res.Stage);
			m_GetRewards.Clear();
			m_GetRewards.AddRange(res.GetRewards());
			if (m_GetRewards.Count < 1) return;
			ViewChapterReward(_cb);
		});
#endif
	}

	void ViewChapterReward(Action _cb) {
		//클리어하고 현재 스테이지가 챕터 마지막이면 챕터보상 팝업 아니면 일반 보상획득 팝업
		var stg = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		if (stg.Idx % 100 == 1) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.ChapterClearReward, (result, obj) => {
				m_SUI.Stage.SetStageChage(stg.Idx, false);
				_cb?.Invoke(); 
			}, m_GetRewards[0]);
		}
		else {
			MAIN.SetRewardList(new object[] { m_GetRewards }, () => {
				m_GetRewards.Clear();
				m_SUI.Stage.SetStageChage(stg.Idx, true);
				_cb?.Invoke();
			});
		}
	}
	void SendStageChapterReward(Action<LS_Web.RES_STAGE_CHAPTERREWARD> cb) {
		UserInfo.StageIdx stageidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		UserInfo.Stage stage = USERINFO.m_Stage[StageContentType.Stage];
		WEB.SEND_REQ_STAGE_CHAPTERREWARD((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 클리어 다시 시도
					// 타이틀부터 다시 시작함 (플레이상태에서 시드 선택정보가 있으면 다시 시도함)
					MAIN.ReStart();
				});
				return;
			}
			cb?.Invoke(res);
		}, stage.UID, stageidx.Week, stageidx.Pos);
	}

	public void SetBG() {
		if (m_BG != null)
			Destroy(m_BG.gameObject);
		m_BG = UTILE.LoadPrefab(TDATA.GetStageTable(
			USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, 
			USERINFO.GetDifficulty()).GetBGName(), true, m_SUI.BGParent)
			.GetComponent<Item_MainMenu_Stg_Bg>();
		m_BG.transform.SetAsFirstSibling();
		m_BG.MaskOff();
	}
	/// <summary> 현재 덱이 빈칸이 있는지 체크 </summary>
	bool CheckDeckNotEmpty() {
		for (int i = 0; i < USERINFO.m_PlayDeck.m_Char.Length; i++) {
			if (i > 2 && USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx / 100 <= i) break;
			if (USERINFO.m_PlayDeck.m_Char[i] == 0) {
				POPUP.Set_Popup(PopupPos.MSGBOX, PopupName.Msg_CenterAlarm).GetComponent<MsgBoxBase>().SetMsg(string.Empty, TDATA.GetString(44));
				return false;
			}
		}
		return true;
	}
	public GameObject GetMenuBtn(MainMenuType menu)
	{
		return m_SUI.MenuBtns.GetMenuBtn(menu);
		//return m_SUI.MenuButton[(int)menu].gameObject;
	}

	public GameObject GetStageDiffBtn(StageDifficultyType type)
	{
		return m_SUI.Stage.GetStageDiffBtn(type);
	}

	public GameObject GetMenuUI(MainMenuType menu)
	{
		switch (menu)
		{
		case MainMenuType.Shop:
			return m_SUI.Shop.gameObject;
		case MainMenuType.Dungeon:
			return m_SUI.Dungeon.gameObject;
		case MainMenuType.Character:
			return m_SUI.SrvMng.gameObject;
		case MainMenuType.PDA:
			return m_SUI.PDA.gameObject;
		}
		return m_SUI.Stage.gameObject;
	}

	public GameObject GetStageStartBtn()
	{
		return m_SUI.Panels[0];
	}

	public GameObject GetInvenBtn()
	{
		return m_SUI.IconMenu[IconName.Inven].Active;
	}

	public GameObject GetSideIcon(Main_Play.IconName name)
	{
		return m_SUI.IconMenu[name].Active;
	}
	public GameObject GetReplayBtn() {
		return m_SUI.Stage.GetReplayBtn();
	}
	public void GoStage()
	{
		if (IsAction) return;
		if (POPUP.IS_PopupUI()) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 1)) return;
		if (USERINFO.m_Stage[StageContentType.Stage].IS_LastStage())
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(ToolData.StringTalbe.UI, 185));
			return;
		}

		UserInfo.StageIdx sidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		TStageTable table = TDATA.GetStageTable(sidx.Idx, USERINFO.GetDifficulty());
		switch (table.m_Mode)
		{
		case StageModeType.Training:
			var tdata = TDATA.GetStageTable(sidx.Idx, USERINFO.GetDifficulty());
			if (tdata.m_Energy > 0 && USERINFO.m_Energy.Cnt < tdata.m_Energy)
			{
				POPUP.StartLackPop(BaseValue.ENERGY_IDX);
				return;
			}
			if (table.m_Mode == StageModeType.Training && PlayerPrefs.GetInt($"TrainingGuide_{USERINFO.m_UID}", 0) < 1) {
				PlayerPrefs.SetInt($"TrainingGuide_{USERINFO.m_UID}", 1);
				PlayerPrefs.Save();
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tuto_Video, (result, obj) => { StartStage(); }, TutoVideoType.Training);
			}
			else
				StartStage();
			PlayEffSound(SND_IDX.SFX_0300);
			break;
		default:
			DLGTINFO?.f_OBJSndOff?.Invoke();
			SND.StopEff();
			PlayEffSound(SND_IDX.SFX_0300);
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DeckSetting, (result, obj) =>
			{
				if (result == 1) {
					List<int> notetutostgidx = new List<int>() { 1002001, 1002003, 1002006, 1002008, 1002011 };
					if (notetutostgidx.Contains(table.m_Idx) && table.m_Mode == StageModeType.NoteBattle && PlayerPrefs.GetInt($"NoteBattleGuide_{USERINFO.m_UID}_{table.m_Idx}", 0) < 1) {
						PlayerPrefs.SetInt($"NoteBattleGuide_{USERINFO.m_UID}_{table.m_Idx}", 1);
						PlayerPrefs.Save();
						POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tuto_Video, (result, obj) => { StartStage(); }, TutoVideoType.NoteBattle, table.m_Idx);
					}
					else if(table.m_Mode == StageModeType.Tower && PlayerPrefs.GetInt($"TowerGuide_{USERINFO.m_UID}", 0) < 1) {
						PlayerPrefs.SetInt($"TowerGuide_{USERINFO.m_UID}", 1);
						PlayerPrefs.Save();
						POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tuto_Video, (result, obj) => { StartStage(); }, TutoVideoType.Tower);
					}
					else
						StartStage();
				}
				//추천 상품 체크 및 세팅
				SetRecommendGoods(false);
			}, table, StageContentType.Stage, DayOfWeek.Sunday, USERINFO.GetDifficulty());
			break;
		}
	}

	void StartStage()
	{
		if (IsAction) return;
		UserInfo.StageIdx sidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
#if NOT_USE_NET
		m_Action = IE_GoStage();
		StartCoroutine(m_Action);
		USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, TDATA.GetStageTable(sidx.Idx).m_Energy);
#else
		PLAY.GetStagePlayCode((result) => {
			if(result != EResultCode.SUCCESS)
			{
				SetUI();
				return;
			}

			m_Action = IE_GoStage();
			StartCoroutine(m_Action);
		}, StageContentType.Stage, sidx.Idx, sidx.Week, sidx.Pos);
#endif
	}

	IEnumerator IE_GoStage() {
		TStageTable table = TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, USERINFO.GetDifficulty());
		if (table.m_Mode == StageModeType.Training) {
			m_SUI.Stage.InStage();
			m_SUI.Anim.SetTrigger(AniName.StageIn.ToString());

			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => m_SUI.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
		}

		if (TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, USERINFO.GetDifficulty()).m_TalkDlg[0] == 0) {

			PLAY.GoStage(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx);
		}
		else if (USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount < 2)
		{
			AllStopSound();
			var TStage = TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx, USERINFO.GetDifficulty());
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DL_Talk, (result, obj) => {
				PLAY.GoStage(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx);
			}, TStage.m_TalkDlg[0], TStage.GetName(), TStage.GetImg(true), TStage.GetImg(false), true, 1);
		}
		else PLAY.GoStage(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx);
	}

	public void ClickMenuButton(int _idx)
	{
		//강해지는법에서 이 버튼을 호출함
		if (POPUP.IS_PopupUI()) {
			PopupName popuup = POPUP.GetPopup().m_Popup;
			switch(popuup) {
				case PopupName.RewardList:
				case PopupName.RewardList_Equip:
				case PopupName.CharDraw:
				case PopupName.AddEvent:
				case PopupName.GrowthWay:
					break;
				default: return;
			}
		}
		if (Utile_Class.IsAniPlay(m_SUI.Anim)) return;
		if (!IsStartEnd) return;
		MenuChange(_idx);
	}

	IEnumerator m_MenuCloaseAni;
	IEnumerator CheckMenuCloase(int idx, object[] args)
	{
		yield return m_MenuCloaseAni;
		m_MenuCloaseAni = null;
		MenuChange(idx, false, args);
	}

	public void MenuAlarmRefresh() {
		m_SUI.MenuBtns.SetData();
	}
	public void MenuChange(int _idx, bool EndCheck = true, params object[] args)
	{
		TouchFX.Mode = 1;
		if (IsAction) return;

		if (m_MenuCloaseAni != null) return;
		MainMenuType beforstate = m_State;
		MainMenuType state = (MainMenuType)_idx;

		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Menu, state)) return;

		DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);

		// 메뉴들 켜지거나 꺼질때 버튼 잠금 및 알람 갱신

		//인벤토리 알람 갱신
		SetInvenNewAlram(PlayerPrefs.GetInt($"InvenNewAlarm_{USERINFO.m_UID}") == 1);
		PlayerPrefs.SetInt(string.Format("MainMenuBtnAlarm_{0}_{1}", state.ToString(), USERINFO.m_UID), 0);
		PlayerPrefs.Save();
		m_SUI.MenuBtns.SetData();
		//메뉴 바뀔때 효과음 끄기
		SND.StopEff(true);

		if (EndCheck)
		{
			switch (m_State)
			{
			case MainMenuType.PDA:
				m_MenuCloaseAni = m_SUI.PDA.EndCheck();
				StartCoroutine(CheckMenuCloase(_idx, args));
				return;
			}
		}

		switch (state)
		{
			case MainMenuType.Shop: if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) {
					PlayCommVoiceSnd(VoiceType.Fail); 
					return;
				}
				break;
			case MainMenuType.PDA: if (!USERINFO.CheckContentUnLock(ContentType.PDA, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					return;
				}
				break;
			//case MainMenuType.Dungeon: if (!USERINFO.CheckContentUnLock(ContentType.Factory, true)) {
			//		PlayCommVoiceSnd(VoiceType.Fail);
			//		return;
			//	}
			//	break;
			case MainMenuType.Character: if (!USERINFO.CheckContentUnLock(ContentType.Character, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					return;
				}
				break;
		}
		//사운드
		switch (state) {
			case MainMenuType.PDA:
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3251,
							SND_IDX.VOC_3252
						});
						break;
					default:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3241,
							SND_IDX.VOC_3242
						});
						break;
				}
				break;
			case MainMenuType.Shop:
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3271,
							SND_IDX.VOC_3272
						});
						break;
					default:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3261,
							SND_IDX.VOC_3262
						});
						break;
				}
				break;
			case MainMenuType.Dungeon:
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3231,
							SND_IDX.VOC_3232
						});
						break;
					default:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3221,
							SND_IDX.VOC_3222
						});
						break;
				}
				break;
			case MainMenuType.Character:
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3211,
							SND_IDX.VOC_3212
						});
						break;
					default:
						PlayVoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3201,
							SND_IDX.VOC_3202
						});
						break;
				}
				break;
		}
		SetPostAlram();
		SetMissionAlram();
		SetSeasonPassAlram();
		SetFriendAlram();
		SetMonthlyAlram();
		m_SUI.MenuBtns.AllBtnChange(state);
		SetPanel(state);
		bool IsActive = state == MainMenuType.Stage;
		SetGuideQuest(IsActive);
		m_SUI.RightMenu.SetActive(IsActive);
		m_SUI.LeftMenu.SetActive(IsActive); 
		if (IsActive && m_SUI.UserActivityAlarm.gameObject.activeSelf) m_SUI.NoticeBtnAnim.SetTrigger("UserAlarm");

		//상점에서 다른 메뉴로 갈때 상점 탭 초기화
		if (beforstate == MainMenuType.Shop) m_SUI.Shop.SetInitTab();
		switch (state)
		{
		case MainMenuType.Stage:
			//추천 상품 체크 및 세팅
			SetRecommendGoods(true);
			if (beforstate != state) m_SUI.RecommendGoodsBanner.CheckNewGoods();

			if (beforstate != MainMenuType.Stage && beforstate != MainMenuType.End) m_SUI.Stage.TabMenu();
			m_SUI.Stage.SetData(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx);
			m_SUI.Stage.Idle(USERINFO.m_Stage[StageContentType.Stage].IS_LastStage());
				//DLGTINFO.f_RFGuidQuestUI?.Invoke(GuidQuestInfo.InfoType.End);
				break;
		//case MainMenuType.Making:
		//    m_SUI.Making.SetData();
		//    break;
		case MainMenuType.Shop:
			m_SUI.Shop.InitTab();
			if (TUTO.IsTuto(TutoKind.ShopSupplyBox, (int)TutoType_ShopSupplyBox.Focus_ShopMenu)) {
				m_SUI.Shop.StateChange((int)Shop.Tab.Shop);
				TUTO.Next();
			}
			else if (TUTO.IsTuto(TutoKind.ShopEquipGacha, (int)TutoType_ShopEquipGacha.Focus_ShopMenu)) {
				m_SUI.Shop.StateChange((int)Shop.Tab.Shop);
				TUTO.Next();
			}
			else if (TUTO.IsTuto(TutoKind.PickupGacha, (int)TutoType_Pickup.Focus_ShopMenu)) {
				m_SUI.Shop.StateChange((int)Shop.Tab.Shop);
				TUTO.Next();
			}
			else if (args != null && args.Length > 0)
			{
				Shop.Tab tab = (Shop.Tab)args[0];
				switch (tab)
				{
				case Shop.Tab.Pass:
				case Shop.Tab.Auction:
					m_SUI.Shop.StateChange((int)tab);
					break;
				default:
					m_SUI.Shop.StateChange((int)Shop.Tab.Shop);
					if (args.Length > 1) m_SUI.Shop.StartPos(true, (ShopGroup)args[1]);
					break;
				}
			}
			else
				m_SUI.Shop.StateChange((int)Shop.Tab.Shop);
				break;
		case MainMenuType.Dungeon:
			m_SUI.Dungeon.SetData();
			//if (TUTO.IsTuto(TutoKind.Factory, (int)TutoType_Factory.Select_Dungeon_Menu)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Academy, (int)TutoType_Academy.Select_Dungeon_Menu)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Bank, (int)TutoType_Bank.Select_Dungeon_Menu)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Tower, (int)TutoType_Tower.Select_Dungeon_Menu)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Cemetery, (int)TutoType_Cemetery.Select_Dungeon_Menu)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.University, (int)TutoType_University.Select_Dungeon_Menu)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Subway, (int)TutoType_Subway.Select_Dungeon_Menu)) TUTO.Next();
			if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.Select_Dungeon_Menu)) TUTO.Next();
			break;
		case MainMenuType.Character:
			m_SUI.SrvMng.SetData();
				//if (TUTO.IsTuto(TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.Select_CharInfo_Menu)) TUTO.Next(m_SUI.SrvMng);
				//else if (TUTO.IsTuto(TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.Select_CharInfo_Menu)) TUTO.Next(m_SUI.SrvMng);
				////else if (TUTO.IsTuto(TutoKind.Serum, (int)TutoType_Serum.Select_CharInfo_Menu)) TUTO.Next(m_SUI.SrvMng);
				//else if (TUTO.IsTuto(TutoKind.DNA, (int)TutoType_DNA.Select_CharInfo_Menu)) TUTO.Next(m_SUI.SrvMng);
				break;
		case MainMenuType.PDA:
			m_SUI.PDA.SetData();
			if (TUTO.IsTuto(TutoKind.Making, (int)TutoType_Making.Select_PDA)) TUTO.Next(m_SUI.PDA);
			else if (TUTO.IsTuto(TutoKind.Research, (int)TutoType_Research.Select_PDA)) TUTO.Next(m_SUI.PDA);
			else if (TUTO.IsTuto(TutoKind.Zombie, (int)TutoType_Zombie.Select_PDA)) TUTO.Next(m_SUI.PDA);
			else if (TUTO.IsTuto(TutoKind.DNA_Make, (int)TutoType_DNA_Make.Select_PDA)) TUTO.Next(m_SUI.PDA);
			else if (TUTO.IsTuto(TutoKind.Adventure, (int)TutoType_Adventure.Select_PDA)) TUTO.Next(m_SUI.PDA);
			break;
		}
	}
	void SetPanel(MainMenuType _state) {
		m_BG.SketchFXOnOff(_state != MainMenuType.Stage);
		m_State = _state;
		for (int i = 0; i < m_SUI.MenuPanel.Length; i++)
		{
			bool Active = i == (int)m_State;
			if(m_SUI.MenuPanel[i].activeSelf != Active) m_SUI.MenuPanel[i].SetActive(Active);
		}
		// 유저 평가 튜토리얼로 셋팅 조건 : 가차튜토리얼 후 스테이지 화면으로 넘어왔을때
		//if(TUTO.IsEndTuto(TutoKind.ShopSupplyBox) && !TUTO.IsEndTuto(TutoKind.UserReview)) TUTO.Start(TutoStartPos.NONE);
	}

	public void CB_Dungeon_Refresh() {
		m_SUI.Dungeon.SetData();
	}
	public void ViewInven()
	{
		if (IsAction) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 0)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Inventory, (result, obj)=> { SetInvenNewAlram(false); }, Inventory.EMenu.ETC);
	}

	public void SetInvenNewAlram(bool _new) {
		SetAlram(IconName.Inven, _new);
	}

	public void SetPostAlram()
	{
		SetAlram(IconName.Post, USERINFO.m_Posts.Count > 0);
	}

	public void SetMissionAlram()
	{
		bool day = USERINFO.m_Mission.IsSuccess(MissionMode.Day);
		bool daily = USERINFO.m_Mission.IsSuccess(MissionMode.DailyQuest);
		var beqlist = USERINFO.m_Mission.Get_Missions(MissionMode.BeginnerQuest);
		var bcq = beqlist.FindAll(o => o.m_TData.m_Check[0].m_Type == MissionType.BeginnerQuestClear);
		int bcqcnum = 0;
		for(int i = 0; i < bcq.Count; i++) {
			if (bcq[i].IS_End() && bcqcnum < bcq[i].m_TData.m_ModeGid + 1) bcqcnum = bcq[i].m_TData.m_ModeGid + 1;
		}
		bool begin = beqlist.Find(o => o.IsPlayMission() && o.State[0] == RewardState.Idle && o.IS_Complete() && o.m_TData.m_ModeGid < bcqcnum) != null;
		SetAlram(IconName.Mission,  day || daily || begin);
	}

	public void SetUnionAlram()
	{
		SetAlram(IconName.Union, USERINFO.m_Guild.GetAlramMode() != GuildInfo_Base.AlramMode.None);
	}

	public void SetFriendAlram()
	{
		bool Active = false;
		var friends = USERINFO.m_Friend.Friends.FindAll(o => o.State == Friend_State.Friend || o.State == Friend_State.Deleted); 
		int remaincnt = TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount) - friends.Count;
		int idlecnt = USERINFO.m_Friend.Friends.Count(o => o.State == Friend_State.Idle);
		var cnt = friends.Count(o => o.GetGiftState() == Friend_Gift_State.Get);
		// 남은 획득 가능한 개수
		var max = Math.Min(friends.Count,TDATA.GetConfig_Int32(ConfigType.MaxFriendReceiveCount)) - cnt;
		var rev = friends.Count(o => o.GetGiftState() == Friend_Gift_State.Gift);
		Active = max > 0 && rev > 0 || (remaincnt > 0 && idlecnt > 0);
		if (!Active) Active = idlecnt > 0;
		SetAlram(IconName.Friend, Active);
	}

	public void SetSeasonPassAlram()
	{
		var pass = USERINFO.m_ShopInfo.PassInfo;
		if(pass.Count < 1)
		{
			SetIconMenu(IconName.SeasonPass, false);
			return;
		}
		int seasonlock = TDATA.GetConfig_Int32(ConfigType.StoreOpen);
		SetIconMenu(IconName.SeasonPass, seasonlock <= USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx);
		SetSeasonPassTime();
		SetAlram(IconName.SeasonPass, USERINFO.m_Mission.IsSuccess(MissionMode.Pass, pass[0].Idx));
	}

	MsgBoxBase ChallengeAlram;

	void FixedUpdate()
	{
#if !NOT_USE_NET
		if (!IsStartEnd) return;

		// 챌린지 알람이 있는지 체크
		if (!TUTO.IsTutoPlay() && ChallengeAlram == null && MAIN.m_ChallengeAlram.Count > 0)
		{
			// 현재 챌린지 정보가 없을 수 있으므로 무시
			// 방법 1. 랭킹 관련 행동을 했을때 각 프로토콜에서 해당 정보를 내려줌
			// 방법 2. 10분마다 랭킹 호출해서 받아옴(사용안함)
			RES_CHALLENGE_MYRANKING alram = MAIN.m_ChallengeAlram[0];
			MAIN.m_ChallengeAlram.RemoveAt(0);
			ChallengeAlram = POPUP.Set_MsgBox(PopupName.Msg_Challenge_Alarm, (btn, obj) => { }, alram);
		}
#endif
	}

#region Challenge
	public void OnChallenge() {
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 2)) return;
		if (m_Action != null) return;
		StartCoroutine(m_Action = StartChallengePopup());
	}

	IEnumerator StartChallengePopup()
	{
		Sprite blurimg = null;
		yield return UTILE.GetCaptureBlurSprite((img) => { blurimg = img; }, 15);
		yield return UTILE.GetCaptureResizeSprite((img) => { blurimg = img; }, 0.025f);

		WEB.SEND_REQ_CHALLENGEINFO_ALL((res) =>
		{
			// 챌린지 변경 알림
			MyChallenge info = USERINFO.m_MyChallenge;
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Main, null, blurimg);
			m_Action = null;
			//if (info.Now.UID > 0) POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Main, null, blurimg);
			//else if (info.Next != ChallengeType.END) POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Next, null, info.Next, info.NextSTime);
		});
	} 
#endregion

	public void OnPost() {
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 3)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PostList, (result, obj) => {
			SetPostAlram();
		});
	}

	void LoopSND() {
		if (m_State == MainMenuType.Stage) {
			if (!Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0)) m_IdleTimer[0] += Time.deltaTime;
			else m_IdleTimer[0] = 0;

			if (m_IdleTimer[0] >= m_IdleTimer[1]) {
				m_IdleTimer[0] = 0;
				m_IdleTimer[1] = UTILE.Get_Random(15f, 25);
				//메인-스테이지, 팝업 없을때
				if (m_State != MainMenuType.Stage || POPUP.IS_PopupUI()) return;
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						PlayEffSound(m_LoopSNDF[UTILE.Get_Random(0, m_LoopSNDF.Count)]);
						break;
					default:
						PlayEffSound(m_LoopSNDM[UTILE.Get_Random(0, m_LoopSNDM.Count)]);
						break;
				}
			}
		}
		else m_IdleTimer[0] = 0;
	}

#region Friend
	public void OnFriend() {
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 5)) return;
		USERINFO.m_Friend.LoadFriendInfo(() =>
		{
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Friend, (result, obj) => { SetFriendAlram(); });
		});
	}
	#endregion

#region Announcement
	public void ClickAnnouncement() {
		if (IsAction) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 4)) return;
		HIVE.ShowPromotion(hive.PromotionType.NOTICE);
		//UTILE.OpenURL(WEB.GetConfig(EServerConfig.Notice_URL));
		//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Announcement, null, WEB.GetConfig(EServerConfig.Notice_URL));
	}
	#endregion

#region DailuQuest
	public void ClickDailyQuest() {
		if (IsAction) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 6)) return;
		int dailylock = TDATA.GetConfig_Int32(ConfigType.DailyQuestOpen);
		if (dailylock > USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx) {
			TStageTable data = TDATA.GetStageTable(dailylock);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), dailylock / 100, dailylock % 100, data.GetName()));
			return;
		}
		// 유저정보 최신화
#if NOT_USE_NET
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.NewNDailyMission, (result, obj)=> { SetMissionAlram(); });
		if (TUTO.IsTuto(TutoKind.NewMission, (int)TutoType_NewMission.Focus_MissionBtn)) TUTO.Next();
#else
		WEB.SEND_REQ_MISSIONINFO((res) => { 
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.NewNDailyMission, (result, obj) => {
				SetMissionAlram();
				SetGuideQuest(m_State == MainMenuType.Stage);
			}); 
			if (TUTO.IsTuto(TutoKind.NewMission, (int)TutoType_NewMission.Focus_MissionBtn)) TUTO.Next();
		});
#endif
	}
	#endregion

#region RecommendGoods
	/// <summary>
	/// 스테이지 돌아올때, 덱세팅창 껏을때, 
	/// </summary>
	public void SetRecommendGoods(bool _gen) {
		if (!USERINFO.CheckContentUnLock(ContentType.Store)) {
			m_SUI.RecommendGoodsBanner.gameObject.SetActive(false);
		}
		else {
			m_SUI.RecommendGoodsBanner.gameObject.SetActive(true);
			m_SUI.RecommendGoodsBanner.SetData(_gen);
		}
	}
	#endregion

#region MonthPackage
	void SetMonthPackage() {
		RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		if(packinfo != null && packinfo.GetTime() <= 0 && packinfo.GetLastTime() > 0) {
			WEB.SEND_REQ_SHOP_GET_DAILYPACKITEM((res) => {
				if (!res.IsSuccess()) {
					return;
				}
				POPUP.Set_MsgBox(PopupName.Msg_RewardGet_Monthly, (res, obj)=> { SetMonthlyAlram(); }, res.GetRewards());
			}, packinfo.Idx);
		}
	}


	void SetMonthlyAlram()
	{

		SetMonthlyTime();
		//RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		//RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		//RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		//bool active = rginfo != null && (buyinfo == null || (packinfo != null && packinfo.GetLastTime() * 0.001d <= 86400 * 3));

		//SetIconMenu(IconName.Monthly, active);
		//if (active) {
		//	SetMonthlyTime();
		//}
	}
	void SetMonthlyTime()
	{
		RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		var ListTime = -1d;
		var buygap = 86400 * 3;
		bool issell = USERINFO.m_ShopInfo.Infos.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE) == null;
		if (packinfo != null) ListTime = packinfo.GetLastTime() * 0.001d;
		bool active = issell && rginfo != null && (packinfo == null || ListTime < buygap);
		SetIconMenu(IconName.Monthly, active);

		bool IsTimer = ListTime > 0 && ListTime < buygap;
		m_SUI.IconMenu[IconName.Monthly].Time.Active.SetActive(IsTimer);
		m_SUI.IconMenu[IconName.Monthly].Name.SetActive(!IsTimer);
		if (IsTimer)
		{
			int day = (int)(ListTime / 86400L);
			int pos = day < 1 ? 1 : 0;
			m_SUI.IconMenu[IconName.Monthly].Time.BG.color = m_SUI.IconMenu[IconName.Monthly].Time.BG_Color[pos];
			m_SUI.IconMenu[IconName.Monthly].Time.Icon.color = m_SUI.IconMenu[IconName.Monthly].Time.Icon_Color[pos];
			m_SUI.IconMenu[IconName.Monthly].Time.Time.color = m_SUI.IconMenu[IconName.Monthly].Time.Text_Color[pos];
			m_SUI.IconMenu[IconName.Monthly].Time.Time.text = string.Format(TDATA.GetString(724), day);
		}
	}

	//void SetMonthlyAlram() {
	//	RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
	//	RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
	//	RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
	//	bool active = rginfo != null && packinfo != null && packinfo.GetLastTime() * 0.001d <= 86400 * 3;

	//	SetIconMenu(IconName.Monthly, active);
	//	if (active) {
	//		SetMonthlyTime();
	//	}
	//}
	//void SetMonthlyTime() {
	//	RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
	//	RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
	//	RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
	//	bool active = rginfo != null && packinfo != null && packinfo.GetLastTime() * 0.001d <= 86400 * 3;

	//	SetIconMenu(IconName.Monthly, active);
	//	m_SUI.IconMenu[IconName.Monthly].Time.Active.SetActive(packinfo != null);
	//	m_SUI.IconMenu[IconName.Monthly].Name.SetActive(packinfo == null);
	//	if (packinfo != null) {
	//		int day = (int)(packinfo.GetLastTime() / 86400000L);
	//		int pos = day < 1 ? 1 : 0;
	//		m_SUI.IconMenu[IconName.Monthly].Time.BG.color = m_SUI.IconMenu[IconName.Monthly].Time.BG_Color[pos];
	//		m_SUI.IconMenu[IconName.Monthly].Time.Icon.color = m_SUI.IconMenu[IconName.Monthly].Time.Icon_Color[pos];
	//		m_SUI.IconMenu[IconName.Monthly].Time.Time.color = m_SUI.IconMenu[IconName.Monthly].Time.Text_Color[pos];
	//		m_SUI.IconMenu[IconName.Monthly].Time.Time.text = string.Format(TDATA.GetString(724), day);
	//	}
	//}
	public void Click_MonthPackage() {
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 12)) return;
		RecommendGoodsInfo rginfo = USERINFO.GetRecommendGoods(ShopAdviceCondition.Shop).Find(o => o.m_SIdx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		//RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
		//if (buyinfo != null) {
		//	rginfo = new RecommendGoodsInfo() {
		//		m_SIdx = buyinfo.Idx,
		//		m_UTime = buyinfo.UTime
		//	};
		//}
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Purchase_Monthly, (res, obj) => {
			SetMonthlyAlram();
		}, rginfo);
	}
#endregion

#region SeasonPass
	public void SetSeasonPassTime()
	{
		var pass = USERINFO.m_ShopInfo.PassInfo;
		if(pass.Count < 1)
		{
			SetIconMenu(IconName.SeasonPass, false);
			return;
		}
		int seasonlock = TDATA.GetConfig_Int32(ConfigType.StoreOpen);
		SetIconMenu(IconName.SeasonPass, seasonlock <= USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx);
		int day = (int)((pass[0].Times[1] - UTILE.Get_ServerTime_Milli() + 86399999L) / 86400000L);
		int pos = day < 2 ? 1 : 0;
		m_SUI.IconMenu[IconName.SeasonPass].Time.BG.color = m_SUI.IconMenu[IconName.SeasonPass].Time.BG_Color[pos];
		m_SUI.IconMenu[IconName.SeasonPass].Time.Icon.color = m_SUI.IconMenu[IconName.SeasonPass].Time.Icon_Color[pos];
		m_SUI.IconMenu[IconName.SeasonPass].Time.Time.color = m_SUI.IconMenu[IconName.SeasonPass].Time.Text_Color[pos];
		m_SUI.IconMenu[IconName.SeasonPass].Time.Time.text = string.Format(TDATA.GetString(724), day);
	}
	public void GoSeasonPass()
	{
		if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
		POPUP.Init_PopupUI();
		POPUP.Init_MsgBoxUI();
		if (m_State != MainMenuType.Shop) MenuChange((int)MainMenuType.Shop, false, Shop.Tab.Pass);
		else m_SUI.Shop.StateChange((int)Shop.Tab.Pass);
	}

	public void GoAccChange()
	{
		POPUP.Init_PopupUI();
		POPUP.Init_MsgBoxUI();
		if (m_State != MainMenuType.PDA) MenuChange((int)MainMenuType.PDA, false, Shop.Tab.Pass);

		m_SUI.PDA.StateChange((int)Item_PDA_Menu.State.Option, new object[] { Item_PDA_Option_Main.Menu.Auth });
	}

	public void ClickSeasonPass()
	{
		if (IsAction) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 7)) return;
		GoSeasonPass();
	}
	#endregion

#region Guild
	public void ClickUnion()
	{
		GoUnion();
	}
	public void GoUnion(Action _rccb = null, Action _mcb = null) {
		if (IsAction) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 9)) return;
		USERINFO.m_Guild.LoadGuild(() => {
			// 진입전 상품 갱신부터 해둠
			USERINFO.m_Guild.GetMyShopList();

			if (USERINFO.m_Guild.UID == 0) {
				WEB.SEND_REQ_GUILD_RECOMMEND((res) => {
					if (res.IsSuccess()) {
						// 자신의 길드정보 갱신해주기
						POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_JoinList, (result, obj) => {
							switch ((Union_JoinList.CloseResult)result) {
								case Union_JoinList.CloseResult.LoadGuild:
									ClickUnion();
									break;
								case Union_JoinList.CloseResult.Success:
									ClickUnion();
									break;
							}
						}, res);
						_rccb?.Invoke();
					}
				}, new List<long>());

				// 추방 되었을때 알림
				USERINFO.m_GuildKickCheck.CheckKick();
			}
			else {
				// 길드 정보창 완료후 셋팅
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Main, (result, obj) => {
					switch ((Union_JoinList.CloseResult)result) {
						case Union_JoinList.CloseResult.LoadGuild:  // 길드 퇴출되거나
							SetUnionAlram();
							// UI 리셋해주기
							// 자신의 길드 UID 가 0이면 길드창 닫아주기
							ClickUnion();
							//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6064));
							break;
						case Union_JoinList.CloseResult.Success:
							ClickUnion();
							break;
					}
					SetUnionAlram();
				});
				_mcb?.Invoke();
			}
		}, 0, true, true, true);
	}
	#endregion

#region Event
	public void SetEventBtn() {
		List<FAEventType> evttype = BaseValue.EVENT_LIST;
		for (int i = m_SUI.EventBtnGroup.childCount - 1; i > -1; i--)
			Destroy(m_SUI.EventBtnGroup.GetChild(i).gameObject);

		for (int i = 0; i < evttype.Count; i++) {
			MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => o.Type == evttype[i]);
			if (evt == null) continue;
			if (!evt.IsPlayEvent()) continue;
			GameObject btn = UTILE.LoadPrefab(string.Format("Item/Item_MenuBtn_{0}", evt.Prefab), true, m_SUI.EventBtnGroup);
			if (btn != null) {
				btn.GetComponent<Item_EventBtn>().SetData(evt, Click_Event);
			}
		}
	}
	public void Click_Event(MyFAEvent evt)
	{
		if (evt == null) return;
		if (!IsTutoStartCheck) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_Btn, 11)) return;

		PopupName popup = (PopupName)Enum.Parse(typeof(PopupName), evt.Prefab);
		POPUP.Set_Popup(PopupPos.POPUPUI, popup, (res, obj) => {
			SetEventBtn();
		}, evt);
	}
	#endregion


#region Attendance
	public void ClickAttendance()
	{
		if (TUTO.IsTutoPlay()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.FAEvent, (result, obj) => SetAttendanceAlram(), USERINFO.m_Event.IsCheck());
		//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.FAEvent, null, false);
	}
	public void SetAttendanceAlram()
	{
		var checklist = USERINFO.m_Event.GetViewAttEvent();
		SetIconMenu(IconName.Attendance, !USERINFO.IsFirstNameSet && checklist.Count > 0);
		// 체크 대상이 있을때
		SetAlram(IconName.Attendance, !USERINFO.IsFirstNameSet && checklist.Any(o => o.IsViewCheck()));
	}
	#endregion

#region Guide Quest

	IEnumerator m_GuideQuestAction = null;

	void ChangeGuideQuest()
	{
		if (m_GuideQuestAction != null)
		{
			StopCoroutine(m_GuideQuestAction);
			m_GuideQuestAction = null;
		}
		m_GuideQuestAction = IE_GuideQuestStartAction();
		StartCoroutine(m_GuideQuestAction);
	}

	public void SetGuideQuest(bool IsActive)
	{
		if (USERINFO.IsFirstNameSet) IsActive = false;
		if (IsActive)
		{
			if (IsGuideQuestChange()) ChangeGuideQuest();
			else
			{
				if(m_SUI.GuideQuest.Info != null) m_SUI.GuideQuest.Active.SetActive(true);
				SetGuideQuestUI(GuidQuestInfo.InfoType.End);
			}
		}
		else
		{
			if (m_GuideQuestAction != null)
			{
				StopCoroutine(m_GuideQuestAction);
				m_GuideQuestAction = null;
			}
			m_SUI.GuideQuest.Active.SetActive(false);
		}
	}

	public void SetGuideQuestUI(GuidQuestInfo.InfoType mode)
	{
		if (!m_SUI.GuideQuest.Active.activeSelf) return;
		if (m_SUI.GuideQuest.Info == null)
		{
			m_SUI.GuideQuest.Active.SetActive(false);
			return;
		}
		if (mode != GuidQuestInfo.InfoType.End && m_SUI.GuideQuest.Info.type != mode) return;

		switch (m_SUI.GuideQuest.Info.type)
		{
		case GuidQuestInfo.InfoType.Mission:
			// 보상을 받아야되거나 완료된 미션만 표시함
			var mission = (MissionData)m_SUI.GuideQuest.Info.Data;
			var tmis = mission.m_TData;
			string label = "";
			switch(tmis.m_Mode)
			{
				case MissionMode.Day:			label = string.Format("[{0}]", TDATA.GetString(722)); break;
				case MissionMode.BeginnerQuest: label = string.Format("[{0}]", TDATA.GetString(887)); break;
				case MissionMode.Guide:			label = string.Format("[{0}]", TDATA.GetString(1115)); break;
				default:label = string.Format("[{0}]", TDATA.GetString(2106)); break;
				}
			m_SUI.GuideQuest.Label.text = label;
			m_SUI.GuideQuest.Title[1].text = m_SUI.GuideQuest.Title[0].text = tmis.GetName();
			m_SUI.GuideQuest.Cnt.text = $"({mission.GetCnt(0)}/{tmis.m_Check[0].m_Cnt})";
			m_SUI.GuideQuest.Success.SetActive(mission.IS_Complete());
			m_SUI.GuideQuest.Reward.SetData(tmis.m_Rewards[0].Get_RES_REWARD_BASE(), null, IsStartEff: false);
			break;
		case GuidQuestInfo.InfoType.Achieve:
			// 완료된 업적만 표시함
			var tachi = (TAchievementTable)m_SUI.GuideQuest.Info.Data;
			AchieveData achieve = USERINFO.m_Achieve.GetAchieveData(tachi);
			long Cnt = 0;
			if (achieve != null) Cnt = achieve.Cnt;
			m_SUI.GuideQuest.Label.text = string.Format("[{0}]", TDATA.GetString(417));
			m_SUI.GuideQuest.Title[1].text = m_SUI.GuideQuest.Title[0].text = tachi.GetName();
			m_SUI.GuideQuest.Cnt.text = $"({Cnt}/{tachi.m_Values[1]})";
			m_SUI.GuideQuest.Success.SetActive(true);
			m_SUI.GuideQuest.Reward.SetData(tachi.m_Reward.Kind, tachi.m_Reward.Idx, tachi.m_Reward.Cnt, tachi.m_Reward.LV, tachi.m_Reward.Grade, IsStartEff:false);
			break;
		default: m_SUI.GuideQuest.Active.SetActive(false); break;
		}
	}
	IEnumerator IE_GuideQuestStartAction()
	{
		if (m_SUI.GuideQuest.Active.activeSelf)
		{
			m_SUI.GuideQuest.Ani.SetTrigger("Off");
			yield return Utile_Class.CheckAniPlay(m_SUI.GuideQuest.Ani);
		}

		yield return new WaitWhile(() => !IsStartEnd);
		m_SUI.GuideQuest.Active.SetActive(true);
		m_SUI.GuideQuest.Info = USERINFO.GetGuideQuest();
		DLGTINFO.f_RFGuidQuestUI?.Invoke(GuidQuestInfo.InfoType.End);
		m_SUI.GuideQuest.Ani.SetTrigger("On");
		m_GuideQuestAction = null;
	}
	public bool IsGuideQuestChange()
	{
		var info = USERINFO.GetGuideQuest();
		bool Change = false;
		if (m_SUI.GuideQuest.Info == null && info != null) Change = true;
		else if (m_SUI.GuideQuest.Info != null && info == null) Change = true;
		else if (m_SUI.GuideQuest.Info != null && info != null)
		{
			// 변경인지 확인
			if (m_SUI.GuideQuest.Info.type != info.type) Change = true;
			else
			{
				switch (m_SUI.GuideQuest.Info.type)
				{
				case GuidQuestInfo.InfoType.Mission:
					// 보상을 받아야되거나 완료된 미션만 표시함
					if (((MissionData)m_SUI.GuideQuest.Info.Data).Idx != ((MissionData)info.Data).Idx) Change = true;
					break;
				case GuidQuestInfo.InfoType.Achieve:
						if (((TAchievementTable)m_SUI.GuideQuest.Info.Data).m_Idx != ((TAchievementTable)info.Data).m_Idx) Change = true;
						else if (((TAchievementTable)m_SUI.GuideQuest.Info.Data).m_LV != ((TAchievementTable)info.Data).m_LV) Change = true;
					break;
				}
			}
		}
		return Change;
	}

	public void OnClick_GuideQuest()
	{
		if (!IsStartEnd) return;
		if (m_GuideQuestAction != null) return;
		if (TUTO.IsTutoPlay()) return;
		switch (m_SUI.GuideQuest.Info.type)
		{
		case GuidQuestInfo.InfoType.Mission:
			// 보상을 받아야되거나 완료된 미션만 표시함
			var mission = (MissionData)m_SUI.GuideQuest.Info.Data;
			if (!mission.IS_Complete()) return;
			PlayEffSound(mission.m_TData.m_Mode == MissionMode.Event_miniGame_Clear ? SND_IDX.SFX_3031 : SND_IDX.SFX_3030);
			USERINFO.m_Mission.GetReward(mission, 0, (res) => {
				if (res.IsSuccess())
				{
#if !NOT_USE_NET
					MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
						ChangeGuideQuest();
					});
#endif
				}
			});
			break;
		case GuidQuestInfo.InfoType.Achieve:
			// 완료된 업적만 표시함
			var tachi = (TAchievementTable)m_SUI.GuideQuest.Info.Data;
#if !NOT_USE_NET
			WEB.SEND_REQ_ACHIEVE_REWARD((res) => {
				if (res.IsSuccess())
				{
					MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
						ChangeGuideQuest();
					});
				}
			}, new List<REQ_ACHIEVE_REWARD>(){ new REQ_ACHIEVE_REWARD() { Idx = tachi.m_Idx, LV = tachi.m_LV } });
#endif
			break;
		}
	}
#endregion

	//////////////////////////////////////TEST/////////////////////////////////////////////////////////
	[SerializeField] int ADDEVENTIDX = 1;
	[ContextMenu("TEST")]
	void ADDEVENTTEST() {
		USERINFO.m_AddEvent = ADDEVENTIDX;
		PopupBase block = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TouchBlock);
		m_SUI.Stage.SetAddEventAnim(true, () => {
			block.Close();
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.AddEvent, (result, obj) => {
				block = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TouchBlock);
				if (m_SUI.Stage != null) {
					m_SUI.Stage.SetAddEventAnim(false, () => {
						block.Close();
					});
				}
				else {
					block.Close();
				}
			}, USERINFO.m_AddEvent);
		});
	}

	[SerializeField] int DLIdx;
	[ContextMenu("DialogueText")]
	void DLTEST() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DL_Talk, (result, obj) => {
		}, DLIdx, null, null, null, true, 0);
	}
	[ContextMenu("STRSOLTEST")]
	void STRSOLTEST() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.GrowthWay);
	}
}