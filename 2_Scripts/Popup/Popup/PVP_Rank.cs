using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;

public class PVP_Rank : PopupBase
{
	enum State
	{
		League,
		Season,
		Max
	}
    [Serializable]
    public struct SUI
	{
		public GameObject[] Panels;         //0:league,1:season
		public Item_Tab[] Tabs;
	}
	[Serializable]
	public struct SSUI
	{
		public TextMeshProUGUI Timer;
		public ScrollReck_ViewItemController ScrollController;
		public RectTransform Prefab;
		public Item_PVP_Ranking_List_Element[] Users;
		public Item_PVP_Ranking_List_Element My;
	}
	[Serializable]
	public struct SLUI
	{
		public Item_PVP_Tier TierGroup;
		public TextMeshProUGUI Timer;
		public GameObject PointGroup;
		public GameObject NoRanking;
		public Image PointTierIcon;
		public TextMeshProUGUI PointRankTierName;
		public TextMeshProUGUI PointLP;
		public TextMeshProUGUI UpDownDesc;
		public Item_PVP_Ranking_List_Element My;
		public ScrollRect Scroll;
		public Transform Bucket;
		public GameObject UserElement;      //Item_PVP_Ranking_List_Element
		public GameObject[] RankUpDown;     //0:up,1:down
		public GameObject RewardDot;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SSUI m_SSUI;
	[SerializeField] SLUI m_SLUI;

	State m_State = State.Max;
	RES_PVP_GROUP m_LInfo;
	RES_RANKING m_SInfo;
	List<RES_RANKING_INFO> m_RankingInfos = new List<RES_RANKING_INFO>();
	List<Item_PVP_Ranking_List_Element> m_LeagueElements = new List<Item_PVP_Ranking_List_Element>();

	double[] m_Time = new double[2];                        //0:league,1:season
	float m_CanvasH;

	private void Awake() {
		m_CanvasH = POPUP.GetCanvasH();
	}
	private void Update() {
		if(m_LInfo != null) {
			m_Time[0] = (m_LInfo.etime - UTILE.Get_ServerTime_Milli()) * 0.001d;
			if (m_Time[0] <= 0) m_Time[0] = 0;
			m_Time[1] = (m_LInfo.setime - UTILE.Get_ServerTime_Milli()) * 0.001d;
			if (m_Time[1] <= 0) m_Time[1] = 0;

			m_SLUI.Timer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_Time[0]);
			m_SSUI.Timer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_Time[1]);
		}
	}
	private void LateUpdate() {
		if(m_CanvasH != POPUP.GetCanvasH()) {
			if (m_State != State.Season) return;
			m_CanvasH = POPUP.GetCanvasH();
			SetSeasonScroll();
			SetSeason(ScrollReck_ViewItemController.RefreshMode.Normal);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_SSUI.ScrollController.SetData(0, m_SSUI.Prefab, SetSeason);

		m_LInfo = (RES_PVP_GROUP)aobjValue[0];
		m_SInfo = (RES_RANKING)aobjValue[1];
		m_RankingInfos.AddRange(m_SInfo.RankUsers);

		m_Time[0] = (m_LInfo.etime - UTILE.Get_ServerTime_Milli()) * 0.001d;
		if (m_Time[0] <= 0) m_Time[0] = 0;
		m_Time[1] = (m_LInfo.setime - UTILE.Get_ServerTime_Milli()) * 0.001d;
		if (m_Time[1] <= 0) m_Time[1] = 0;

		//리그 랭킹 오브제 세팅
		for (int i = 0; i < m_LInfo.Users.Count; i++) {
			Item_PVP_Ranking_List_Element element = Utile_Class.Instantiate(m_SLUI.UserElement, m_SLUI.Bucket).GetComponent<Item_PVP_Ranking_List_Element>();
			m_LeagueElements.Add(element);
		}
		//시즌 랭킹 오브제 세팅
		SetSeasonScroll();

		base.SetData(pos, popup, cb, aobjValue);

		m_SUI.Tabs[0].OnClick();
	}
	void SetSeasonScroll() {
		m_SSUI.ScrollController.InitPosition();
		int cnt = m_SSUI.ScrollController.GetViewCnt();
		m_SSUI.Users = new Item_PVP_Ranking_List_Element[cnt];
		for (int i = 0; i < cnt; i++) {
			m_SSUI.Users[i] = m_SSUI.ScrollController.GetItem<Item_PVP_Ranking_List_Element>(i);
		}
	}
	public override void SetUI() {
		m_SUI.Tabs[0].SetData(0, TDATA.GetString(10040), ClickTab);
		m_SUI.Tabs[0].SetAlram(false);
		m_SUI.Tabs[1].SetData(1, TDATA.GetString(10041), ClickTab);
		m_SUI.Tabs[1].SetAlram(false); 

		SetFirstAlarm();

		base.SetUI();
	}
	/// <summary> 탭 전환 </summary>
	bool ClickTab(Item_Tab _tab) {
		if (m_State == (State)_tab.m_Pos) return false;
		m_State = (State)_tab.m_Pos;
		for(State i = 0;i< State.Max; i++) m_SUI.Tabs[(int)i].SetActive(m_State == i);

		if (m_State == State.League) {
			SetLeague();
		}
		else {
			m_SSUI.ScrollController.InitPosition();
			m_SSUI.ScrollController.SetItemCount(m_RankingInfos.Count);
		}
		return true;
	}

	void SetFirstAlarm() {
		bool canget = false;
		List<int> getrewards = m_LInfo.RankReward;
		List<TPvPRankTable> alltdata = TDATA.GetAllPVPRankTable();
		int maxorder = TDATA.GeTPvPRankTable(m_LInfo.MyMaxRankIdx) == null ? 0 : TDATA.GeTPvPRankTable(m_LInfo.MyMaxRankIdx).m_SortingOrder;

		for (int i = 2; i < maxorder; i++) {
			TPvPRankTable tdata = alltdata.Find(o => o.m_SortingOrder == i);
			if (!getrewards.Contains(tdata.m_Idx)) {
				canget = true;
				break;
			}
		}
		m_SUI.Tabs[1].SetAlram(canget);
		m_SLUI.RewardDot.SetActive(canget);
	}
	/// <summary> 리그 랭킹 세팅 </summary>
	void SetLeague() {
		if (m_State != State.League) return;
		m_SUI.Panels[0].SetActive(true);
		m_SUI.Panels[1].SetActive(false);

		m_SLUI.TierGroup.SetData(m_LInfo.Rankidx, Item_PVP_Tier.Type.Tier);
		m_SLUI.Timer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_Time[0]);

		TPvPRankTable ranktdata = TDATA.GeTPvPRankTable(m_LInfo.Rankidx);
		List<RES_PVP_USER_BASE> users = m_LInfo.Users;
		RES_PVP_USER_BASE me = users.Find(o => o.UserNo == USERINFO.m_UID);

		m_SLUI.PointGroup.SetActive(ranktdata.m_UpTierType == UpTierType.POINT);
		m_SLUI.NoRanking.SetActive(ranktdata.m_Rank == 1 && ranktdata.m_Tier == 3);
		if(ranktdata.m_UpTierType == UpTierType.POINT) {
			TPvPRankTable uptdata = TDATA.GeTPvPRankTable(ranktdata.m_UpTierIdx);
			m_SLUI.PointTierIcon.sprite = uptdata.GetTierIcon();
			m_SLUI.PointRankTierName.text = string.Format(TDATA.GetString(10049), uptdata.GetRankName(), uptdata.GetTierName());
			m_SLUI.PointLP.text = string.Format("{0} / {1}", me.Point[1], ranktdata.m_UpTierVal);
		}
		m_SLUI.UpDownDesc.text = string.Format(ranktdata.GetUpDownDesc(), ranktdata.m_UpTierVal, ranktdata.m_DownTierVal);
		m_SLUI.RankUpDown[0].SetActive(ranktdata.m_UpTierType == UpTierType.LEAGUERANK);
		m_SLUI.RankUpDown[1].SetActive(ranktdata.m_DownTierType == DownTierType.LEAGUERANK);

		for (int i = 0; i < users.Count; i++) {
			Item_PVP_Ranking_List_Element element = m_LeagueElements[i];
			element.SetData(m_LInfo.Rankidx, users[i], ClickViewInfo);
			if (users[i].UserNo == USERINFO.m_UID) {
				m_SLUI.My.SetData(new RES_PVP_USER_BASE[2] { users[i], users[i] });
			}
			if (ranktdata.m_UpTierType == UpTierType.LEAGUERANK && i == ranktdata.m_UpTierVal - 1) {
				m_SLUI.RankUpDown[0].transform.SetSiblingIndex(element.transform.GetSiblingIndex());
			}
			else if (ranktdata.m_DownTierType == DownTierType.LEAGUERANK && i == ranktdata.m_DownTierVal - 1) {
				m_SLUI.RankUpDown[1].transform.SetSiblingIndex(element.transform.GetSiblingIndex() - 1);
			}
		}
		int mypos = m_LeagueElements.FindIndex(o => o.m_Info[0].UserNo == m_SLUI.My.m_Info[0].UserNo);
		m_SLUI.Scroll.verticalNormalizedPosition = 1f - (float)mypos / (float)m_SLUI.Bucket.childCount;
	}
	/// <summary> 시즌 랭킹 세팅 </summary>
	void SetSeason(ScrollReck_ViewItemController.RefreshMode mode) {
		if (m_State != State.Season) return;
		m_SUI.Panels[0].SetActive(false);
		m_SUI.Panels[1].SetActive(true);

		m_SSUI.Timer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single ,m_Time[1]);
		m_SSUI.My.SetData(m_SInfo.MyInfo, ClickViewInfo);
		var listcnt = m_SSUI.ScrollController.GetViewCnt();
		int offset = m_SSUI.ScrollController.GetViewLine() * m_SSUI.ScrollController.GetOneLineItemCnt();
		for (int i = 0; i < listcnt; i++, offset++) {
			Item_PVP_Ranking_List_Element item = m_SSUI.ScrollController.GetItem<Item_PVP_Ranking_List_Element>(i);
			if (item == null) continue;
			if (offset > -1 && offset < m_RankingInfos.Count) {
				item.SetData(m_RankingInfos[offset], ClickViewInfo);
				item.gameObject.SetActive(true);
			}
			else item.gameObject.SetActive(false);
		}
	}
	/// <summary> 유저 정보창 열기 </summary>
	void ClickViewInfo(RES_PVP_USER_BASE _info, int _rankidx) {
		WEB.SEND_REQ_PVP_USER_DETAIL_INFO((res) => {
			if(res.IsSuccess()) POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Info_OtherUser, null, res.User, _rankidx);
		}, 0, _info.UserNo);
	}
	/// <summary> 리그 티어 정보 </summary>
	public void ClickViewLeagueRewardInfo() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Rank_League_RewardList, (result, obj)=> { SetFirstAlarm(); }, m_LInfo);
	}
	/// <summary> 시즌 보상 정보 </summary>
	public void ClickViewSeasonRewardInfo() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Rank_Season_RewardList);
	}
}
