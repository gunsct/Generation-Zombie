using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;
using System.Linq;

public class PVP_Main : PopupBase
{
	[Serializable]
	public class PVPUserInfo
	{
		public Image Portrait;
		public Image Nation;
		public Text Name;
		public Image TierIcon;
		public TextMeshProUGUI TierName;
		public TextMeshProUGUI LeaguePoint;
		public Transform CPBucket;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Animator StartBtnAnim;
		public PVPUserInfo[] UserInfos;
		public GameObject[] Panels;
		public Item_Tab[] Tabs;
		public Transform CPNumFont;         //Item_PVP_Main_CP_NumFont
		public GameObject RankingAlarm;
		public TextMeshProUGUI DayPlayCnt;
		public GameObject[] GoBtn;			//0:가능, 1:불가능

		public GameObject[] TutoObj;//0:대전 상대 정보, 1:팀 셋팅버튼, 2;교체버튼, 3:상점버튼 4:나가기
	}
	[Serializable]
	public struct SRUI
	{
		public Animator RecieveBtn;
		public TextMeshProUGUI[] RemainTime;	//리그보상까지 남은 시간
		public TextMeshProUGUI[] LeftCnts;
		public Slider Gauge;
		public Image GaugeGlow;
		public GameObject[] Panels;				//0:leftcnt,1:recieve,2:deactive
	}
	[Serializable]
	public struct STUI
	{
		public TextMeshProUGUI[] SurvStat;
		public Transform CharBuckets;
		public Transform CharPrefabs;       //Item_PVP_Main_EnemyChar
		public GameObject[] StealMatGroup;
		public TextMeshProUGUI[] StealMats;
	}
	[Serializable]
	public struct SSUI
	{
		public TextMeshProUGUI Timer;
		public TextMeshProUGUI RefreshCnt;
		public Item_PVP_Store_Element[] Goods;
	}
	[Serializable]
	public struct SBCUI {
		public Animator Anim;
		public Text UserName;
		public TextMeshProUGUI[] BuildLv;
		public GameObject[] BuildAlarm;
		public GameObject ResearchLock;
		public TextMeshProUGUI ResearchTitle;
		public TextMeshProUGUI LockDesc;
		public GameObject FXGroup;
	}

	[SerializeField] SUI m_SUI;
	[SerializeField] SRUI m_SRUI;
	[SerializeField] STUI m_STUI;
	[SerializeField] SSUI m_SSUI;
	[SerializeField] SBCUI m_SBCUI;

	RES_PVP_GROUP m_PVPGroupInfo;
	List<REQ_CAMP_PLUNDER_LOG_DATA> m_PVPDefLog;
	RES_PVP_USER_BASE[] m_PVPUsers = new RES_PVP_USER_BASE[2];//0:본인,1:상대방
	RES_PVP_USER_DETAIL m_Target;
	int m_TabPos = -1;
	double m_Timer;
	IEnumerator m_Action;

	List<TShopTable> m_PVPTDatas = new List<TShopTable>();
	int m_DailyGoodsResetTime;
	SND_IDX m_NowBG;
	int m_CounterIdx = 0;
	bool Is_GoBattle;
	TPvPRankTable m_PRTData { get { return TDATA.GeTPvPRankTable(m_PVPGroupInfo.Rankidx); } }
	PVPUserCampInfo m_CampInfo { get { return m_PVPUsers[0].CampInfo; } }
	CampBuildInfo m_CampBuildInfo { get { return USERINFO.m_CampBuild[CampBuildType.Camp]; } }
	private void Start()
	{
		if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.PVP_Action)) TUTO.Next();
	}
	private void Update() {
		if (!WEB.IS_SendNet()) return;
		if (m_PVPGroupInfo == null) return;

		if(m_SRUI.Panels[2].activeSelf)
		{
			m_PVPGroupInfo.MyKillInfo.InitInfo();
			double time = (UTILE.Get_ZeroTime(m_PVPGroupInfo.MyKillInfo.UTime) + 86400000L - UTILE.Get_ServerTime_Milli()) * 0.001d;
			if (time <= 0) time = 0;
			m_SRUI.RemainTime[0].text = m_SRUI.RemainTime[1].text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, time));
			if (m_Timer < time) {
				WEB.SEND_REQ_PVP_GROUP_INFO((res) => {
					if (res.IsSuccess())
						SetReward();
				});
			}
			m_Timer = time;
		}
		double storeremaintime = UTILE.Get_RemainDayTime(DayOfWeek.Monday);
		m_SSUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, storeremaintime));

		double seasoninittime = double.Parse(PlayerPrefs.GetString($"SHOP_PVPSTORE_TIME_{ShopResetType.Season}_{USERINFO.m_UID}", "0"));
		double weekinittime = double.Parse(PlayerPrefs.GetString($"SHOP_PVPSTORE_TIME_{ShopResetType.DayOfWeek}_{USERINFO.m_UID}", "0"));
		double dailyinittime = double.Parse(PlayerPrefs.GetString($"SHOP_PVPSTORE_TIME_{ShopResetType.ZeroTime}_{USERINFO.m_UID}", "0"));
		double initremaintime = UTILE.Get_RemainDayTime(DayOfWeek.Monday, weekinittime);
		double Initdailyresettime = dailyinittime + m_DailyGoodsResetTime - UTILE.Get_ServerTime_Milli();
		if(USERINFO.m_ShopInfo.NowSeason.STime > seasoninittime) {
			USERINFO.InitPVPStore(ShopResetType.Season, true);
			SetStore();
		}
		if (initremaintime <= 0) {
			USERINFO.InitPVPStore(ShopResetType.ZeroTime, true);
			USERINFO.InitPVPStore(ShopResetType.DayOfWeek, true);
			SetStore();
		}
		if (Initdailyresettime <= 0) {
			USERINFO.InitPVPStore(ShopResetType.ZeroTime, true);
			SetStore();
		}
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_SUI.Panels[1].SetActive(false);
		m_SUI.Panels[2].SetActive(false);
		m_NowBG = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_0660);

		m_PVPTDatas = TDATA.GetGroupShopTable(ShopGroup.PVPShop); 
		m_DailyGoodsResetTime = m_PVPTDatas.Find(o => o.m_ResetType == ShopResetType.ZeroTime).m_ResetTime * 60000;

		m_PVPGroupInfo = (RES_PVP_GROUP)aobjValue[0];
		m_PVPDefLog = (List<REQ_CAMP_PLUNDER_LOG_DATA>)aobjValue[1];
		m_Timer = (m_PVPGroupInfo.stime - UTILE.Get_ServerTime_Milli()) * 0.001d;
		m_PVPUsers[0] = m_PVPGroupInfo.Users.Find(o => o.UserNo == USERINFO.m_UID);
		m_PVPUsers[0].Power = USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].GetCombatPower(true);
		m_PVPUsers[1]  = m_Target = m_PVPGroupInfo.Target;
		m_SUI.Tabs[0].SetData(0, TDATA.GetString(999), ClickTab);
		m_SUI.Tabs[0].SetAlram(false);
		m_SUI.Tabs[0].OnClick();
		m_SUI.Tabs[1].SetData(1, TDATA.GetString(1000), ClickTab);
		m_SUI.Tabs[1].SetAlram(false);
		m_SUI.Tabs[2].SetData(2, TDATA.GetString(6205), ClickTab);
		m_SUI.Tabs[2].SetAlram(false);

		if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.Select_PVP)) TUTO.Next();
		else SetEndLeagueNSeason();

		SetStore();

		base.SetData(pos, popup, cb, aobjValue);
	}
	void CheckDefLog() {
		if (m_PVPDefLog.Count > 0) {
			List<int> logidx = m_PVPDefLog.Select(o => o.Idx).ToList();
			List<int> idxs = USERINFO.GetPVPDefLog();
			for (int i = logidx.Count - 1; i > -1; i--) {
				if (!idxs.Contains(logidx[i])) {
					idxs.Add(logidx[i]);
				}
				else logidx.Remove(logidx[i]);
			}
			int overcnt = Math.Max(0, idxs.Count - 5);
			if (overcnt > 0) {
				for (int i = 0; i < overcnt; i++) {
					idxs.RemoveAt(0);
				}
			}
			USERINFO.SetPVPDefLog(idxs);

			List<REQ_CAMP_PLUNDER_LOG_DATA> logs = m_PVPDefLog.FindAll(o => logidx.Contains(o.Idx));
			if (logs != null && logs.Count > 0) {
				m_SBCUI.FXGroup.SetActive(true);
				m_SBCUI.Anim.SetTrigger("Start_Raid");
				PlayEffSound(SND_IDX.SFX_9020);
				StartCoroutine(Utile_Class.CheckAni(m_SUI.Anim, 73f / 224f, () => {
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_DefenseList, (res, obj) => {
						m_SBCUI.Anim.SetTrigger("RaidClose");
					}, m_PVPDefLog, (Action<long, int>)SetCounter);
				}));
			}
			else if(!m_SBCUI.Anim.GetCurrentAnimatorStateInfo(0).IsName("Start")) m_SBCUI.Anim.SetTrigger("Start");
		}
		else {
			m_SBCUI.FXGroup.SetActive(false);
			if (!m_SBCUI.Anim.GetCurrentAnimatorStateInfo(0).IsName("Start")) m_SBCUI.Anim.SetTrigger("Start");
		}
	}
	public void Click_PVPDefLog() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_DefenseList, (res, obj) => { }, m_PVPDefLog, (Action<long, int>)SetCounter);
	}

	public override void SetUI() {
		base.SetUI();

		//보상 세팅
		SetReward();

		//상대방 덱 세팅
		SetTargetInfo();

		for (int i = 0; i < 2; i++) {//아마 USERINFO에 PvPInfo가 들어갈듯
			m_SUI.UserInfos[i].Portrait.sprite = TDATA.GetUserProfileImage(m_PVPUsers[i].Profile);
			m_SUI.UserInfos[i].Nation.sprite = BaseValue.GetNationIcon(m_PVPUsers[i].Nation);
			m_SUI.UserInfos[i].Name.text = m_PVPUsers[i].Name;
			if(i == 0) m_PVPUsers[0].Power = USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].GetCombatPower(true);
			SetCP(m_SUI.UserInfos[i], m_PVPUsers[i].Power, i == 0 ? Utile_Class.GetCodeColor("#B3ECFF") : Utile_Class.GetCodeColor("#FFD2D1"));
			m_SUI.UserInfos[i].TierIcon.sprite = m_PRTData.GetTierIcon();
			m_SUI.UserInfos[i].TierName.text = string.Format("{0} {1}", m_PRTData.GetRankName(), m_PRTData.GetTierName());
			m_SUI.UserInfos[i].LeaguePoint.text = string.Format("{0} LP", Utile_Class.CommaValue(m_PVPUsers[i].Point[1]));
		}
		//랭킹 버튼 알람
		SetRankAlarm();
		m_SUI.DayPlayCnt.text = string.Format("{0}/{1}", Math.Max(0,BaseValue.PVP_DAY_PLAY_COUNT - m_PVPUsers[0].GetDayPlayCnt()), BaseValue.PVP_DAY_PLAY_COUNT);
		SetGoBtn();
	}
	/// <summary> 리그, 시즌 종료 </summary>
	void SetEndLeagueNSeason()
	{
		if (m_PVPGroupInfo.RState[1] == RewardState.Idle)
		{//리그보상
			WEB.SEND_REQ_PVP_REWARD((res) =>
			{
				if (res.IsSuccess())
				{
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_End_League, (result, obj) =>
					{
						Check_SeasenReward();
					}, res);
				}
			}, PVP_RewardKind.League, m_PVPGroupInfo.Rankidx);
		}
		else if (m_PVPGroupInfo.RState[0] == RewardState.Idle)
		{
			//시즌보상

			Check_SeasenReward();
		}
	}

	void Check_SeasenReward()
	{
		if (m_PVPGroupInfo.RState[0] == RewardState.Idle)
		{//시즌보상
			WEB.SEND_REQ_PVP_REWARD((res) =>
			{
				if (res.IsSuccess())
				{
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_End_Season, (result, obj) =>
					{

					}, res);
				}
			}, PVP_RewardKind.Season, m_PVPGroupInfo.Rankidx);
		}
	}
	/// <summary> 보상 세팅 </summary>
	void SetReward()
	{
		m_PVPGroupInfo.MyKillInfo.InitInfo();
		TPVPRankRewardTable tdata = TDATA.GeTPVPRankRewardTable(m_PVPGroupInfo.Rankidx);
		int maxrewardcnt = tdata.m_KillPoint.Length;
		int getrewardcnt = m_PVPGroupInfo.MyKillInfo.RewardCnt;
		int preneedcnt = getrewardcnt > 0 ? tdata.m_KillPoint[getrewardcnt - 1].Need : 0;
		int killcnt = m_PVPGroupInfo.MyKillInfo.Kill - preneedcnt;
		int needcnt = getrewardcnt >= maxrewardcnt ? 0 :tdata.m_KillPoint[getrewardcnt].Need - preneedcnt;

		m_SRUI.Gauge.value = (float)killcnt / (float)needcnt; 
		m_SRUI.GaugeGlow.gameObject.SetActive(killcnt > 0);
		m_SRUI.GaugeGlow.color = Utile_Class.GetCodeColor(killcnt == needcnt ? "#0B8900" : "#F58000");

		m_SRUI.LeftCnts[0].text = m_SRUI.LeftCnts[1].text = string.Format(TDATA.GetString(10003), Mathf.Max(0,needcnt - killcnt));
		m_SRUI.Panels[0].SetActive(killcnt < needcnt && getrewardcnt < maxrewardcnt);
		m_SRUI.Panels[1].SetActive(killcnt >= needcnt && getrewardcnt < maxrewardcnt);
		m_SRUI.Panels[2].SetActive(getrewardcnt >= maxrewardcnt);
		m_SRUI.RecieveBtn.SetTrigger(killcnt >= needcnt && getrewardcnt < maxrewardcnt ? "On" : "Off");
	}
	/// <summary> 전투력 표기 </summary>
	void SetCP(PVPUserInfo _info, long _cp, Color _color) {
		// [20230328] ODH 전투력 0일때 숫자 안나옴 셋팅 방식 변경
		string cp = _cp.ToString();
		int length = cp.Length;
		UTILE.Load_Prefab_List(length, _info.CPBucket, m_SUI.CPNumFont);
		for (int i = 0; i < length; i++)
		{
			Item_PVP_Main_CP_NumFont num = _info.CPBucket.GetChild(i).GetComponent<Item_PVP_Main_CP_NumFont>();
			num.SetData(UTILE.LoadImg(string.Format("UI/UI_PVP/PVP_NumberFont_{0}", cp.Substring(i, 1)), "png"), _color);
		}
	}
	/// <summary> 상대방 pvp정보에 정보 세팅</summary>
	void SetTargetInfo() {
		for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
			float val = 0f;
			for (int j = 0; j < m_Target.Chars.Count; j++) {
				val += m_Target.Chars[j].Stat.ContainsKey(i) ? m_Target.Chars[j].Stat[i] : 0;
			}
			m_STUI.SurvStat[(int)i].text = Mathf.RoundToInt(val).ToString();
		}
		//캐릭터 정보
		UTILE.Load_Prefab_List(5, m_STUI.CharBuckets, m_STUI.CharPrefabs);
		RES_PVP_CHAR[] chars = new RES_PVP_CHAR[2];
		for(int i = 0; i < 5; i++) {
			chars[0] = m_Target.Chars[i];
			chars[1] = m_Target.Chars[i + 5];
			m_STUI.CharBuckets.GetChild(i).GetComponent<Item_PVP_Main_EnemyChar>().SetData(chars, i < 5 - m_PRTData.m_HideMemberCnt);
		}
		var tci = m_PVPUsers[1].CampInfo;
		for (int i = 0;i< 3; i++) {
			TPVP_CampTable ctdata = TDATA.GetTPVP_CampTable(USERINFO.m_CampBuild[CampBuildType.Camp].LV);
			TPVP_CampTable tctdata = TDATA.GetTPVP_CampTable(tci == null ? 1 : tci.BuildLV[0]);
			TPVP_Camp_Storage stdata = TDATA.GetPVP_Camp_Storage(USERINFO.m_CampBuild[CampBuildType.Storage].LV);
			TPVP_Camp_Storage tstdata = TDATA.GetPVP_Camp_Storage(tci == null ? 1 : tci.BuildLV[1]);
			bool matoff = tstdata.m_StealMat[i] == 0 || stdata.m_SaveMat[i] == 0;
			m_STUI.StealMatGroup[i].SetActive(!matoff);
			m_STUI.StealMats[i * 2].text = m_STUI.StealMats[i * 2 + 1].text = string.Format("{0}~{1}", tstdata.m_StealMat[i], tctdata.m_RatioCnt[i].Cnt);
		}
	}
	
	/// <summary> 랭킹 버튼 알람 </summary>
	void SetRankAlarm() {
		bool canget = false;
		List<int> getrewards = m_PVPGroupInfo.RankReward;
		List<TPvPRankTable> alltdata = TDATA.GetAllPVPRankTable();
		int maxorder = TDATA.GeTPvPRankTable(m_PVPGroupInfo.MyMaxRankIdx) == null ? 0 : TDATA.GeTPvPRankTable(m_PVPGroupInfo.MyMaxRankIdx).m_SortingOrder;

		for (int i = 2; i < maxorder; i++) {
			if (!getrewards.Contains(alltdata.Find(o => o.m_SortingOrder == i).m_Idx)) {
				canget = true;
				break;
			}
		}
		m_SUI.RankingAlarm.SetActive(canget);
	}
	/// <summary> 탭 버튼 </summary>
	bool ClickTab(Item_Tab _tab)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 0, _tab.m_Pos)) return false;
		if (m_Action != null) return false;
		if (_tab.m_Pos == 2) {
			if (POPUP.IS_Connecting()) return false;
			WEB.SEND_REQ_CAMP_BUILD((res) => {
				if (res.IsSuccess()) SetTab(_tab);
			});
		}
		else SetTab(_tab);
		return true;
	}
	void SetTab(Item_Tab _tab) {
		int pretab = m_TabPos;
		for (int i = 0; i < m_SUI.Tabs.Length; i++) {
			if (i != _tab.m_Pos) m_SUI.Tabs[i].SetActive(false);
		}
		if (m_TabPos == -1) {
			for (int i = 0; i < m_SUI.Panels.Length; i++)
				m_SUI.Panels[i].SetActive(_tab.m_Pos == i);
			if (_tab.m_Pos == 0) m_SUI.Panels[0].GetComponent<Animator>().SetTrigger("Normal");
			else if (_tab.m_Pos == 1) m_SUI.Panels[1].GetComponent<Animator>().SetTrigger("Normal");
		}
		else {
			switch (_tab.m_Pos) {
				case 0:
					m_SUI.Panels[0].SetActive(true);
					for (int i = 0; i < m_SUI.Panels.Length; i++)
						if (m_SUI.Panels[i].GetComponent<Animator>() != null) m_SUI.Panels[i].GetComponent<Animator>()?.SetTrigger("ToBattle");
					break;
				case 1:
					m_SUI.Panels[1].SetActive(true);
					for (int i = 0; i < m_SUI.Panels.Length; i++)
						if (m_SUI.Panels[i].GetComponent<Animator>() != null) m_SUI.Panels[i].GetComponent<Animator>()?.SetTrigger("ToStore");
					DLGTINFO?.f_RFCoinUI?.Invoke(USERINFO.m_PVPCoin, USERINFO.m_PVPCoin);
					break;
				case 2:
					m_SUI.Panels[2].SetActive(true);
					CheckDefLog();
					//for (int i = 0; i < m_SUI.Panels.Length; i++)
					//	m_SUI.Panels[i].GetComponent<Animator>()?.SetTrigger("ToBaseCamp");
					SetBaseCamp();
					break;
			}
			if (m_SUI.Panels[pretab].GetComponent<Animator>() != null) StartCoroutine(m_Action = IE_PanelChangeAction(m_SUI.Panels[pretab].GetComponent<Animator>(), () => { m_SUI.Panels[pretab].SetActive(false); }));
			else m_SUI.Panels[pretab].SetActive(false);
		}
		m_TabPos = _tab.m_Pos;
	}
	IEnumerator IE_PanelChangeAction(Animator _anim, Action _cb) {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(_anim));

		_cb?.Invoke();
		m_Action = null;
	}
	public void ClickRanking()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 1)) return;
		if (m_Action != null) return;
		WEB.SEND_REQ_RANKING((res) => {
			if (res.IsSuccess()) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Rank, (result, obj) => { SetUI(); }, m_PVPGroupInfo, res);
			}
		}, RankType.PVP);
	}
	/// <summary> 타겟 새로 찾기 </summary>
	public void ClickResearch()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 2)) return;
		if (m_Action != null) return;

		TShopTable tdata = TDATA.GetShopTable(m_PRTData.m_SearchSIdx);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(10007), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					WEB.SEND_REQ_PVP_SEARCH_USER((res) => {
						if (res.IsSuccess()) {
							m_PVPUsers[1] = m_Target = res.User;
							PVPINFO.SetUser(UserPos.Target, m_Target);
							SetUI();
						}
					}, true);
				}
				else {
					POPUP.StartLackPop(tdata.GetPriceIdx());
				}
			}
		}, tdata.m_PriceType, tdata.GetPriceIdx(), tdata.GetPrice(), false);

		//USERINFO.ITEM_BUY(m_PRTData.m_SearchSIdx, 1, (res) => {
		//	PlayEffSound(SND_IDX.SFX_1414);
		//	if (res.IsSuccess()) {
		//		WEB.SEND_REQ_PVP_SEARCH_USER((res) => {
		//			if (res.IsSuccess()) {
		//				m_PVPUsers[1] = m_Target = res.User;
		//				PVPINFO.SetUser(UserPos.Target, m_Target);
		//				SetUI();
		//			}
		//		}, true);
		//	}
		//}, true, string.Empty, TDATA.GetString(10007), false);
	}
	/// <summary> 덱세팅 진입 </summary>
	public void ClickGoPvPDeck()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 3)) return;
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_DeckSetting, (result, obj)=> {
			SetUI();

		});
		//덱세팅창으로 전환
		//내꺼만 바꿔주면 됌
		//m_SUI.UserInfos[0].SetData();
	}
	/// <summary> pvp 진입, 덱이 세팅되어 있어야함 </summary>
	public void ClickGoBattle() {
		if (Is_GoBattle) return;
		if (POPUP.IS_Connecting()) return;
		int cidx = m_CounterIdx;
		m_CounterIdx = 0;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 4)) return;
		if (m_Action != null) return;
		if (!USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_ATK].IS_FullDeck(true)) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10076));
			return;
		}
		else if (!USERINFO.m_Deck[BaseValue.MAX_DECK_POS_PVP_DEF].IS_FullDeck(true)) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10077));
			return;
		}
		Is_GoBattle = true;
		bool is_possible = true;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			if (res.IsSuccess()) {
				List<int> matpos = new List<int>() { 0, 1, 2 };
				CampBuildInfo storage = USERINFO.m_CampBuild[CampBuildType.Storage];
				TPVP_Camp_Storage tdata = TDATA.GetPVP_Camp_Storage(storage.LV);
				for (int i = 0; i < 3; i++) {
					if (tdata.m_SaveMat[i] > 0 && storage.Values[i] >= tdata.m_SaveMat[i]) {
						is_possible = false;
					}
					if (tdata.m_SaveMat[i] == 0)
						matpos.Remove(i);
				}
				if (!is_possible) {
					POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Storage_Lack, (res, obj) => {
						if (res == 1) LackCheck(cidx);
						else Is_GoBattle = false;
					}, matpos, 0);
				}
				else LackCheck(cidx);
			}
			else Is_GoBattle = false;
		}, CampBuildType.Storage);

		
	}
	void SetGoBtn() {
		bool active;
		if (m_PVPUsers[0].GetDayPlayCnt() >= BaseValue.PVP_DAY_PLAY_COUNT) {
			var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.PVP_TICKET_SHOP_IDX);
			int cnt = 0;
			if (buyinfo == null) cnt = TDATA.GetShopTable(BaseValue.PVP_TICKET_SHOP_IDX).m_LimitCnt;
			else cnt = TDATA.GetShopTable(BaseValue.PVP_TICKET_SHOP_IDX).m_LimitCnt - buyinfo.Cnt;
			active = cnt > 0;
		}
		else
			active = true;

		m_SUI.GoBtn[0].SetActive(active);
		m_SUI.GoBtn[1].SetActive(!active);
	}
	void LackCheck(int _cidx) {
		if (m_PVPUsers[0].GetDayPlayCnt() >= BaseValue.PVP_DAY_PLAY_COUNT) {
			var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.PVP_TICKET_SHOP_IDX);
			int cnt = 0;
			if (buyinfo == null) cnt = TDATA.GetShopTable(BaseValue.PVP_TICKET_SHOP_IDX).m_LimitCnt;
			else cnt = TDATA.GetShopTable(BaseValue.PVP_TICKET_SHOP_IDX).m_LimitCnt - buyinfo.Cnt;
			if (cnt > 0) {
				USERINFO.ITEM_BUY(BaseValue.PVP_TICKET_SHOP_IDX, 1, (res) => {
					if (res != null && res.IsSuccess()) PVPStart(true, _cidx);
					else Is_GoBattle = false;
				}, true, null, string.Format(TDATA.GetString(6251), cnt));
				Is_GoBattle = false;
			}
			else {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(563));
				Is_GoBattle = false;
			}
		}
		else PVPStart(false, _cidx);
	}
	void PVPStart(bool _buy, int _cidx) {
		PlayEffSound(SND_IDX.SFX_1301);
		WEB.SEND_REQ_PVP_START((res) => {
			if (res.IsSuccess()) {
				for (int i = 0; i < 2; i++) {
					RES_PVP_USER_DETAIL user = res.Users[i];
					PVPINFO.SetUser((UserPos)i, user);
					PVPINFO.m_CounterIdx = _cidx;
				}
				STAGEINFO.m_StageContentType = StageContentType.PvP;
				StartCoroutine(m_Action = IE_BattleStartAction());
			}
			else if (res.result_code == EResultCode.ERROR_PVP_STATE) {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10008));
				Is_GoBattle = false;
				return;
			}
		}, _buy, _cidx);
	}
	void SetCounter(long _uid, int _idx) {
		m_CounterIdx = _idx;
		//ClickGoBattle();
		WEB.SEND_REQ_PVP_USER_DETAIL_INFO((res) => {
			if (res.IsSuccess()) {
				Action cb = ClickGoBattle;
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Revenge, (res, obj) => {
					m_CounterIdx = 0;
				}, new RES_PVP_USER_BASE[] { m_PVPUsers[0], res.User }, res.User, m_PRTData, cb);
			}
		}, m_PVPGroupInfo.LeagueNo, _uid);
	}
	IEnumerator IE_BattleStartAction() {
		m_SUI.Anim.SetTrigger("BattleStart");
		m_SUI.StartBtnAnim.SetTrigger("BattleStart");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		Is_GoBattle = false;
		MAIN.StateChange(MainState.PVP);
	}
	/// <summary> 보상 받기 </summary>
	public void ClickGetReward()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PVP_Main, 5)) return;
		if (m_Action != null) return;
		m_PVPGroupInfo.MyKillInfo.InitInfo();
		TPVPRankRewardTable tdata = TDATA.GeTPVPRankRewardTable(m_PVPGroupInfo.Rankidx);
		int maxrewardcnt = tdata.m_KillPoint.Length;
		int getrewardcnt = m_PVPGroupInfo.MyKillInfo.RewardCnt;
		if (getrewardcnt >= maxrewardcnt) return;
		int killcnt = m_PVPGroupInfo.MyKillInfo.Kill;
		int needcnt = tdata.m_KillPoint[getrewardcnt].Need;

		if(m_PVPGroupInfo.state == 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10008));
			return;
		}
		if(maxrewardcnt <= getrewardcnt) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10009));
			return;
		}
		if(needcnt > killcnt) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10010));
			return;
		}

		WEB.SEND_REQ_PVP_REWARD((res) => {
			if (res.IsSuccess()) {
				m_PVPGroupInfo.MyKillInfo = res.MyKillInfo;
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					//m_PVPGroupInfo.MyKillInfo.RewardCnt++;
					SetReward();
					SetStore();
				});
			}
		}, PVP_RewardKind.DayKill, m_PVPGroupInfo.Rankidx);
		//받을수 있을때 없을때 따로 센터알림도 띄워주고
	}
	#region Store
	/// <summary> 상점 세팅 </summary>
	void SetStore() {
		//매일 체크해서 일일, 주간 상품 갱신,주는 서버타임 월요일 00시 일일은 매일 00시
		//재화, 상품, 시간
		m_SSUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, UTILE.Get_RemainDayTime(DayOfWeek.Monday)));

		var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.PVPSTORE_REFRESH_SHOP_IDX);
		var tdata = TDATA.GetShopTable(BaseValue.PVPSTORE_REFRESH_SHOP_IDX);
		m_SSUI.RefreshCnt.text = string.Format("{0}({1}/{2})", TDATA.GetString(750), tdata.m_LimitCnt - (buyinfo != null ? buyinfo.Cnt : 0), tdata.m_LimitCnt);

		//상품은 shoptable에 lv별로 총 9개 랜덤으로 뽑고 시간 체크는 resettype으로 확인
		Dictionary<int, bool> season = USERINFO.m_PVPStoreBuy[ShopResetType.Season];
		Dictionary<int, bool> daily = USERINFO.m_PVPStoreBuy[ShopResetType.ZeroTime];
		Dictionary<int, bool> week = USERINFO.m_PVPStoreBuy[ShopResetType.DayOfWeek];
		double dailyinittime = double.Parse(PlayerPrefs.GetString($"SHOP_PVPSTORE_TIME_{ShopResetType.ZeroTime}_{USERINFO.m_UID}", "0"));
		double weekinittime = double.Parse(PlayerPrefs.GetString($"SHOP_PVPSTORE_TIME_{ShopResetType.DayOfWeek}_{USERINFO.m_UID}", "0"));
		double initremaintime = UTILE.Get_RemainDayTime(DayOfWeek.Monday, weekinittime);

		var pvpshopinfos = USERINFO.m_ShopInfo.GetInfos(ShopGroup.PVPShop);
		if (season.Count < 1 || pvpshopinfos.Find(o => o.Idx == season.ElementAt(0).Key) == null) {
			if(season.Count > 0) USERINFO.InitPVPStore(ShopResetType.Season, true);
			List<TShopTable> tdatas = m_PVPTDatas.FindAll(o => o.m_Level == 1 && pvpshopinfos.Find(r => r.Idx == o.m_Idx) != null && o.m_NoOrProb > 0);
			int probsum = tdatas.Sum(o => o.m_NoOrProb);
			int prob = UTILE.Get_Random(0, probsum);
			int preprob = 0;
			for (int i = 0; i < tdatas.Count; i++) {
				preprob += tdatas[i].m_NoOrProb;
				if (preprob >= prob) {
					USERINFO.InsertPVPStore(ShopResetType.Season, tdatas[i].m_Idx, false);
					break;
				}
			}
			season = USERINFO.m_PVPStoreBuy[ShopResetType.Season];
		}
		
		if (daily.Count < 1 || dailyinittime + m_DailyGoodsResetTime - UTILE.Get_ServerTime_Milli() <= 0) {
			if (daily.Count > 0) USERINFO.InitPVPStore(ShopResetType.ZeroTime, true);
			for (int i = 2; i <= 4; i++) {
				List<TShopTable> tdatas = m_PVPTDatas.FindAll(o => o.m_Level == i && pvpshopinfos.Find(r => r.Idx == o.m_Idx) != null && o.m_NoOrProb > 0);
				int probsum = tdatas.Sum(o => o.m_NoOrProb);
				int prob = UTILE.Get_Random(0, probsum);
				int preprob = 0;
				for (int j = 0; j < tdatas.Count; j++) {
					preprob += tdatas[j].m_NoOrProb;
					if (preprob >= prob) {
						USERINFO.InsertPVPStore(ShopResetType.ZeroTime, tdatas[j].m_Idx, false);
						break;
					}
				}
			}
			daily = USERINFO.m_PVPStoreBuy[ShopResetType.ZeroTime];
		}
		if (week.Count < 1 || initremaintime <= 0) {
			if (week.Count > 0) USERINFO.InitPVPStore(ShopResetType.DayOfWeek, true);
			for (int i = 5; i <= 9; i++) {
				List<TShopTable> tdatas = m_PVPTDatas.FindAll(o => o.m_Level == i && pvpshopinfos.Find(r => r.Idx == o.m_Idx) != null && o.m_NoOrProb > 0);
				int probsum = tdatas.Sum(o => o.m_NoOrProb);
				int prob = UTILE.Get_Random(0, probsum);
				int preprob = 0;
				for (int j = 0; j < tdatas.Count; j++) {
					preprob += tdatas[j].m_NoOrProb;
					if (preprob >= prob) {
						USERINFO.InsertPVPStore(ShopResetType.DayOfWeek, tdatas[j].m_Idx, false);
						break;
					}
				}
			}
			week = USERINFO.m_PVPStoreBuy[ShopResetType.DayOfWeek];
		}
		m_SSUI.Goods[0].gameObject.SetActive(season.Count > 0);
		if (season.Count > 0)
			m_SSUI.Goods[0].SetData(TDATA.GetShopTable(season.ElementAt(0).Key), CB_BuyItem);
		for (int i = 0; i < 3; i++) {
			m_SSUI.Goods[1 + i].gameObject.SetActive(i < daily.Count);
			if(i < daily.Count)
				m_SSUI.Goods[1 + i].SetData(TDATA.GetShopTable(daily.ElementAt(i).Key), CB_BuyItem);
		}
		for (int i = 0; i < 5; i++) {
			m_SSUI.Goods[4 + i].gameObject.SetActive(i < week.Count);
			if (i < week.Count)
				m_SSUI.Goods[4 + i].SetData(TDATA.GetShopTable(week.ElementAt(i).Key), CB_BuyItem);
		}
	}
	/// <summary> 상품 구매 </summary>
	void CB_BuyItem(Item_PVP_Store_Element _item) {
		if (m_Action != null) return;
		long precoin = USERINFO.m_PVPCoin;

		PlayEffSound(SND_IDX.SFX_1014);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PurchaseConfirm, (res, obj) => {
			if (res == 1) {
				if (!_item.IS_CanBuy()) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7007));
					return;
				}
				else {
					USERINFO.ITEM_BUY(_item.m_TData.m_Idx, 1, (res) => {
						if (res.IsSuccess()) {
							MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
								SetStore();
								DLGTINFO?.f_RFCoinUI?.Invoke(USERINFO.m_PVPCoin, precoin);
							});
						}
					});
				}
			}
		}, MAIN.GetRewardBase(_item.m_TData, RewardKind.Item)[0], _item.m_TData, false);
	}
	/// <summary> 상품 새로고침 </summary>
	public void ClickStoreRefresh() {
		if (m_Action != null) return;
		var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.PVPSTORE_REFRESH_SHOP_IDX);
		if(buyinfo != null && buyinfo.Cnt >= TDATA.GetShopTable(BaseValue.PVPSTORE_REFRESH_SHOP_IDX).m_LimitCnt) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(9035));
			return;
		}

		long precoin = USERINFO.m_PVPCoin;
		PlayEffSound(SND_IDX.SFX_1014);

		TShopTable tdata = TDATA.GetShopTable(BaseValue.PVPSTORE_REFRESH_SHOP_IDX);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(7013), (result, obj) =>
		{
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					USERINFO.ITEM_BUY(BaseValue.PVPSTORE_REFRESH_SHOP_IDX, 1, (res) => {
						if (res == null) return;
						if (res.IsSuccess()) {
							var daily = USERINFO.m_PVPStoreBuy[ShopResetType.ZeroTime];
							for (int i = 0; i < daily.Count; i++) {
								var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == daily.ElementAt(i).Key);
								if (buyinfo != null) buyinfo.Cnt = 0;
							}
							var dayweek = USERINFO.m_PVPStoreBuy[ShopResetType.DayOfWeek];
							for (int i = 0; i < dayweek.Count; i++) {
								var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == dayweek.ElementAt(i).Key);
								if (buyinfo != null) buyinfo.Cnt = 0;
							}
							USERINFO.InitPVPStore(ShopResetType.DayOfWeek, true);
							USERINFO.InitPVPStore(ShopResetType.ZeroTime, true);
							SetStore();
							DLGTINFO?.f_RFCoinUI?.Invoke(USERINFO.m_PVPCoin, precoin);
						}
					});
				}
				else {
					POPUP.StartLackPop(tdata.GetPriceIdx());
				}
			}
		}, tdata.m_PriceType, tdata.GetPriceIdx(), tdata.GetPrice(), false);
	}
	public void Click_Store() {
		ClickTab(m_SUI.Tabs[1]);
	}
#endregion
#region BaseCamp
	void SetBaseCamp() {
		//재화 갱신
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		m_SBCUI.UserName.text = string.Format(TDATA.GetString(10831), USERINFO.m_Name);
		for (int i = 0; i < 4; i++) {
			if(i < 3) m_SBCUI.BuildLv[i].text = string.Format("Lv.{0}", USERINFO.m_CampBuild[(CampBuildType)i].LV);
			bool isalarm = true;
			switch ((CampBuildType)i) {
				case CampBuildType.Camp:
					isalarm = CheckLvUp(CampBuildType.Camp);
					break;
				case CampBuildType.Storage:
					isalarm = CheckLvUp(CampBuildType.Storage);
					break;
				case CampBuildType.Resource:
					CampBuildInfo resourceinfo = USERINFO.m_CampBuild[CampBuildType.Resource];
					bool isget = false;
					for (int j = 0; j < 3; j++) {
						if (!resourceinfo.IS_CanMakeTime(j) && resourceinfo.IS_CanGetTime(j)) {
							isget = true;
							break;
						}
					}
					isalarm = isget || CheckLvUp(CampBuildType.Resource);
					break;
				case CampBuildType.Research:
					isalarm = false;
					break;
			}
			m_SBCUI.BuildAlarm[i].SetActive(isalarm);
		}

		m_SBCUI.ResearchLock.SetActive(false);
	}
	bool CheckLvUp(CampBuildType _type) {
		TPVP_Camp_NodeLevel tdata = TDATA.GetTPVP_Camp_NodeLevel(_type, USERINFO.m_CampBuild[_type].LV);
		for(int i = 0;i< tdata.m_Condition.Length; i++) {
			var condition = tdata.m_Condition[i];
			if (condition.m_Type == CampBuildType.None) continue;
			if(USERINFO.m_CampBuild[condition.m_Type].LV < condition.m_Lv) {
				return false;
			}
		}
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		for (int i = 0; i < tdata.m_Cost.Length; i++) {
			if (i == 3) {
				if (tdata.m_Cost[i] > USERINFO.m_Money) return false;
			}
			else {
				if (tdata.m_Cost[i] > storageinfo.Values[i]) return false;
			}
		}
		return true;
	}
	public void Click_Camp() {
		if (POPUP.IS_Connecting()) return;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			if (res.IsSuccess()) {
				m_PVPDefLog = res.Logs;
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Camp, (res, obj) => {
					if (res == 2) {
						WEB.SEND_REQ_CAMP_BUILD((res) => {
							if (res.IsSuccess()) {
								m_PVPDefLog = res.Logs;
								CheckDefLog();
								SetUI();
							}
						});
					}
					else {
						SetBaseCamp();
						CheckDefLog();
					}
				});
			}
		});
	}
	/// <summary> 창고 팝업 </summary>
	public void Click_Storage() {
		if (POPUP.IS_Connecting()) return;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			if (res.IsSuccess()) {
				m_PVPDefLog = res.Logs;
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Storage, (res, obj) => {
					if (res == 2) {
						WEB.SEND_REQ_CAMP_BUILD((res) => {
							if (res.IsSuccess()) {
								m_PVPDefLog = res.Logs;
								CheckDefLog();
								SetUI();
							}
						});
					}
					else {
						SetBaseCamp();
						CheckDefLog();
					}
				});
			}
		});
	}
	/// <summary> 자원 관리실 팝업 </summary>
	public void Click_Resource() {
		if (POPUP.IS_Connecting()) return;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			if (res.IsSuccess()) {
				m_PVPDefLog = res.Logs;
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Resource, (res, obj) => {
					if (res == 2) {
						WEB.SEND_REQ_CAMP_BUILD((res) => {
							if (res.IsSuccess()) {
								m_PVPDefLog = res.Logs;
								CheckDefLog();
								SetUI();
							}
						});
					}
					else {
						SetBaseCamp();
						CheckDefLog();
					}
				});
			}
		});
	}
	/// <summary> 연구 팝업 </summary>
	public void Click_Research() {
		if (POPUP.IS_Connecting()) return;
		TPVP_CampTable tdata = TDATA.GetTPVP_CampTable(m_CampBuildInfo.LV);
		if (tdata.Tire < 0) return;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			if (res.IsSuccess()) {
				m_PVPDefLog = res.Logs;
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Research, (res, obj) => {
					if (res == 2) {
						WEB.SEND_REQ_CAMP_BUILD((res) => {
							if (res.IsSuccess()) {
								m_PVPDefLog = res.Logs;
								CheckDefLog();
								SetUI();
							}
						});
					}
					else {
						SetBaseCamp();
						CheckDefLog();
					}
				});
			}
		});
	}
	public override void Close(int Result = 0) {
		PlayBGSound(m_NowBG);
		base.Close(Result);
	}
#endregion
	///////튜토용
	/// <summary>0:대전 상대 정보, 1:팀 셋팅버튼, 2;교체버튼, 3:상점버튼 4:나가기</summary>
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}
}
