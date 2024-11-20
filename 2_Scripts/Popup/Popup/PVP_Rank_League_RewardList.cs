using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.UI;
using static LS_Web;

public class PVP_Rank_League_RewardList : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject TierTabs;
		public SimpleScrollSnap TabScroll;
		public Transform RankBucket;
		public Transform RankElement;   //Item_PVP_Tier
		public Item_Tab[] Tabs;         //보상 탭
		public ScrollRect TierScroll;
		public Transform TierBucket;
		public Transform[] TierElement;	//item_PVP_Rank_League_RewardList
	}
	[SerializeField] SUI m_SUI;
	RES_PVP_GROUP m_Info;
	int m_RankIdx;
	int m_RewardTabPos;					//0:tier,1:first
	IEnumerator m_Action;

	private void Update() {
		for (int i = 0; i < m_SUI.TabScroll.Content.childCount; i++) {
			Transform trans = m_SUI.TabScroll.Content.GetChild(i);
			trans.localScale = Vector3.one * ((Mathf.Max(0f, 350f - Mathf.Abs(trans.position.x - Canvas_Controller.BASE_SCREEN_WIDTH * 0.5f)) / 350f) * 0.2f + 0.8f);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (RES_PVP_GROUP)aobjValue[0];

		m_RankIdx = m_Info.Rankidx == 103 ? 102 : m_Info.Rankidx;
		m_RewardTabPos = -1;

		m_SUI.Tabs[0].SetData(0, TDATA.GetString(10044), ClickRewardTab);
		m_SUI.Tabs[0].SetAlram(false);
		m_SUI.Tabs[1].SetData(1, TDATA.GetString(10045), ClickRewardTab);
		m_SUI.Tabs[1].SetAlram(false);

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		SetRankTab();
		m_SUI.Tabs[m_RewardTabPos == -1 ? 0 : m_RewardTabPos].OnClick();
		SetFirstAlarm();
		base.SetUI();
	}
	void SetRankTab() {
		List<TPvPRankTable> tdatas = new List<TPvPRankTable>();
		tdatas.AddRange(TDATA.GetAllPVPRankTable());
		tdatas.Remove(tdatas.Find(o => o.m_SortingOrder == 1));
		for(int i = 0;i< tdatas.Count; i++) {
			m_SUI.TabScroll.Add(m_SUI.RankElement.gameObject, i);
		}
		for (int i = 0; i < tdatas.Count; i++) {
			Item_PVP_Tier tab = m_SUI.TabScroll.Content.GetChild(i).GetComponent<Item_PVP_Tier>();
			tab.SetData(tdatas[i].m_Idx, Item_PVP_Tier.Type.Tier);
		}
		m_SUI.TabScroll.GoToPanel(tdatas.Find(o => o.m_Idx == m_RankIdx).m_SortingOrder - 2);

		for (int i = 0; i < m_SUI.TabScroll.Content.childCount; i++) {
			Item_PVP_Tier tier = m_SUI.TabScroll.Content.GetChild(i).GetComponent<Item_PVP_Tier>();
			tier.ActiveAnim(i == m_SUI.TabScroll.CurrentPanel);
		}
	}
	public void ScrollRankTab() {
		PlayEffSound(SND_IDX.SFX_1412);
		m_RankIdx = TDATA.GetAllPVPRankTable().Find(o => o.m_SortingOrder == (m_SUI.TabScroll.CurrentPanel + 2)).m_Idx;
		SetTierReward();
		for(int i = 0; i < m_SUI.TabScroll.Content.childCount; i++) {
			Item_PVP_Tier tier = m_SUI.TabScroll.Content.GetChild(i).GetComponent<Item_PVP_Tier>();
			tier.ActiveAnim(i == m_SUI.TabScroll.CurrentPanel);
		}
	}
	void SetTierReward() {
		m_SUI.TierTabs.SetActive(true);
		m_SUI.TierScroll.verticalNormalizedPosition = 1f;

		List<TPVPLeagueRewardTable> tdatas = TDATA.GetAllTPVPLeagueRewardTable().FindAll(o => o.m_Idx == m_RankIdx);
		tdatas.Sort((TPVPLeagueRewardTable _before, TPVPLeagueRewardTable _after) => {
			if (_before.m_Idx == _after.m_Idx) return _before.m_MinRanking.CompareTo(_after.m_MinRanking);
			return _after.m_Idx.CompareTo(_before.m_Idx);
		});

		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.TierBucket, m_SUI.TierElement[0]);
		for (int i = 0; i < tdatas.Count; i++) {
			Item_PVP_Rank_League_RewardList_Rank element = m_SUI.TierBucket.GetChild(i).GetComponent<Item_PVP_Rank_League_RewardList_Rank>();
			element.SetData(tdatas[i]);
		}
	}
	void SetFirstAlarm() {
		bool canget = false;
		List<int> getrewards = m_Info.RankReward;
		List<TPvPRankTable> alltdata = TDATA.GetAllPVPRankTable();
		int maxorder = TDATA.GeTPvPRankTable(m_Info.MyMaxRankIdx) == null ? 0 : TDATA.GeTPvPRankTable(m_Info.MyMaxRankIdx).m_SortingOrder;

		for(int i = 2; i < maxorder; i++) {
			if(!getrewards.Contains(alltdata.Find(o=>o.m_SortingOrder == i).m_Idx)){
				canget = true;
				break;
			}
		}
		m_SUI.Tabs[1].SetAlram(canget);
	}
	void SetFirstReward() {
		m_SUI.TierTabs.SetActive(false);

		int mymaxrank = m_Info.MyMaxRankIdx;
		int maxorder = TDATA.GeTPvPRankTable(mymaxrank) == null ? 0 : TDATA.GeTPvPRankTable(mymaxrank).m_SortingOrder;
		int upidlepos = 0;
		int maxrankpos = 0;
		Item_PVP_Rank_League_RewardList_First.State gettype = Item_PVP_Rank_League_RewardList_First.State.Lock;
		List<int> getrewards = m_Info.RankReward;
		List<TPVPRankRewardTable> tdatas = new List<TPVPRankRewardTable>();
		tdatas.AddRange(TDATA.GetAllPVPRankRewardTable());
		tdatas.RemoveAt(0);

		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.TierBucket, m_SUI.TierElement[1]);
		for (int i = 0; i < tdatas.Count; i++) {
			Item_PVP_Rank_League_RewardList_First element = m_SUI.TierBucket.GetChild(i).GetComponent<Item_PVP_Rank_League_RewardList_First>();
			if (maxorder < TDATA.GeTPvPRankTable(tdatas[i].m_Idx).m_SortingOrder || maxorder == 0) gettype = Item_PVP_Rank_League_RewardList_First.State.Lock;
			else gettype = getrewards.Contains(tdatas[i].m_Idx) ? Item_PVP_Rank_League_RewardList_First.State.Get : Item_PVP_Rank_League_RewardList_First.State.Idle;

			if (upidlepos == 0 && gettype == Item_PVP_Rank_League_RewardList_First.State.Idle) upidlepos = i;
			if (tdatas[i].m_Idx == mymaxrank) maxrankpos = i;
			element.SetData(tdatas[i], gettype, ClickGetReward);
		}
		if (upidlepos == 0) upidlepos = maxorder;

		m_SUI.TierScroll.verticalNormalizedPosition = 1f - (float)upidlepos / (float)tdatas.Count;
	}
	bool ClickRewardTab(Item_Tab _tab) {
		if (m_RewardTabPos == _tab.m_Pos) return false;
		m_RewardTabPos = _tab.m_Pos;
		m_SUI.Tabs[0].SetActive(m_RewardTabPos == 0);
		m_SUI.Tabs[1].SetActive(m_RewardTabPos == 1); 
		for (int i = m_SUI.TierBucket.childCount - 1; i > -1; i--) DestroyImmediate(m_SUI.TierBucket.GetChild(i).gameObject);
		if (m_RewardTabPos == 0) SetTierReward();
		else SetFirstReward();

		return true;
	}
	void ClickGetReward(int _rankidx) {
		WEB.SEND_REQ_PVP_REWARD((res) => {
			if (res.IsSuccess()) {
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					m_Info.RankReward.Add(_rankidx);
					SetFirstReward();
					SetFirstAlarm();
				});
			}
		}, PVP_RewardKind.FirstRank, _rankidx);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
