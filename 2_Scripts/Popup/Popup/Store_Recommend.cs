using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Store_Recommend : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject TimerGroup;
		public TextMeshProUGUI Timer;
		public Transform ItemBucket;
		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI Desc;
		public Color[] BuyCntBGColor;
		public Color[] BuyCntLabelColor;
		public Color[] BuyCntValColor;
		public Image BuyCntBG;
		public TextMeshProUGUI BuyCntLabel;
		public TextMeshProUGUI BuyCnt;
		public GameObject BuyCntGroup;
		public GameObject GoodsPrefab;//item_Pass_GetListElement
		public Transform Bucket;
		public Item_Store_Buy_Button BuyBtn;
		public Transform PageDot;//Item_PageDot_Element
		public Transform PageGroup;
		public Transform RLGroup;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	List<RecommendGoodsInfo> m_TDatas;
	int m_NowPage = 0;
	TShopTable m_NowTData;
	RES_SHOP_USER_BUY_INFO m_BuyInfo;
	GameObject m_ItemObj;
	private void Update() {
		if(m_NowTData != null)
			m_SUI.Timer.text = string.Format(TDATA.GetString(802), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_TDatas[m_NowPage].GetRemainTime));
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_TDatas = (List<RecommendGoodsInfo>)aobjValue[0];
		m_NowPage = (int)aobjValue[1];
		m_NowTData = m_TDatas[m_NowPage].m_STData;
		m_BuyInfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_NowTData.m_Idx);

		PlayerPrefs.SetString(string.Format("NEW_RECOMMEND_GOODS_{0}", USERINFO.m_UID), "-1");
		PlayerPrefs.SetInt(string.Format("NEW_STORE_GOODS_{0}_{1}", USERINFO.m_UID, m_NowTData.m_Idx), 0);
		PlayerPrefs.Save();
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		PlayEffSound(SND_IDX.SFX_1019);

		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		m_SUI.BuyBtn.SetData(m_NowTData.m_Idx);

		//RES_REWARD_ITEM res = new RES_REWARD_ITEM() { Idx = itemdata.m_Idx, Type = Res_RewardType.Item, Cnt = 1, Grade = itemdata.m_Grade };

		if(m_ItemObj != null) Destroy(m_ItemObj);
		m_ItemObj = UTILE.LoadPrefab(string.Format("Item/Store/GoodsBanner/{0}", m_TDatas[m_NowPage].m_SSACTData.m_BannerPrefab), true, m_SUI.ItemBucket);
		m_ItemObj.GetComponent<Item_GoodsBanner_Element>().SetData(m_TDatas[m_NowPage], null);

		if(m_NowTData.m_Group == ShopGroup.DailyPack) {
			TPackageRewardTable tdata = TDATA.GeTPackageRewardGroupTable(m_NowTData.m_Idx)[0];
			for (int i = 0; i < m_SUI.Name.Length; i++) m_SUI.Name[i].text = Utile_Class.Remove_RichTextTag(tdata.GetName()).Replace("\n", " ").Replace("\r", " ");
			m_SUI.Desc.text = tdata.GetDesc();
		}
		else {
			TItemTable itemdata = TDATA.GetItemTable(m_NowTData.m_Rewards[0].m_ItemIdx);
			for (int i = 0; i < m_SUI.Name.Length; i++) m_SUI.Name[i].text = Utile_Class.Remove_RichTextTag(itemdata == null ? m_NowTData.GetName() : itemdata.GetName()).Replace("\n", " ").Replace("\r", " ");
			m_SUI.Desc.text = itemdata == null ? m_NowTData.GetInfo() : itemdata.GetInfo();
		}

		m_SUI.BuyCntGroup.SetActive(m_NowTData.m_LimitCnt > 0);
		int buycnt = m_BuyInfo == null ? 0 : m_BuyInfo.Cnt;
		m_SUI.BuyCnt.text = string.Format("{0}/{1}", buycnt, m_NowTData.m_LimitCnt);
		m_SUI.BuyCntBG.color = m_SUI.BuyCntBGColor[buycnt < m_NowTData.m_LimitCnt ? 0 : 1];
		m_SUI.BuyCntLabel.color = m_SUI.BuyCntLabelColor[buycnt < m_NowTData.m_LimitCnt ? 0 : 1];
		m_SUI.BuyCnt.color = m_SUI.BuyCntValColor[buycnt < m_NowTData.m_LimitCnt ? 0 : 1];
		m_SUI.TimerGroup.SetActive(m_TDatas[m_NowPage].m_SSACTData.m_CloseType == ShopAdviceCloseType.Time);
		m_SUI.Timer.text = string.Format(TDATA.GetString(802), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_TDatas[m_NowPage].GetRemainTime));

		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		for (int i = 0; i < m_NowTData.m_Rewards.Count; i++)
			if (m_NowTData.m_Rewards[i].m_ItemIdx != 0) rewards.AddRange(MAIN.GetRewardData(m_NowTData.m_Rewards[i].m_ItemType, m_NowTData.m_Rewards[i].m_ItemIdx, m_NowTData.m_Rewards[i].m_ItemCnt, true, false, false));

		UTILE.Load_Prefab_List(rewards.Count, m_SUI.Bucket, m_SUI.GoodsPrefab.transform);
		for(int i = 0; i < rewards.Count; i++) {
			m_SUI.Bucket.GetChild(i).GetComponent<Item_Pass_GetListElement>().SetData(rewards[i]);
		}

		m_SUI.PageGroup.gameObject.SetActive(m_TDatas.Count > 1);
		m_SUI.RLGroup.gameObject.SetActive(m_TDatas.Count > 1);
		if (m_TDatas.Count > 1) {
			UTILE.Load_Prefab_List(m_TDatas.Count, m_SUI.PageGroup, m_SUI.PageDot);
			for (int i = 0; i < m_TDatas.Count; i++) {//리스트 안에 Black 오브제가 하나 있음
				Item_PageDot_Element dot = m_SUI.PageGroup.GetChild(i).GetComponent<Item_PageDot_Element>();
				dot.SetData(i == m_NowPage);
			}
		}
		base.SetUI();
	}
	public void ClickRL(bool _right) {
		if (_right) {
			m_NowPage++;
			if (m_NowPage >= m_TDatas.Count) m_NowPage = 0;
			m_SUI.Anim.SetTrigger("RightBtn");
		}
		else {
			m_NowPage--;
			if (m_NowPage < 0) m_NowPage = m_TDatas.Count - 1;
			m_SUI.Anim.SetTrigger("LeftBtn");
		}
		m_NowTData = m_TDatas[m_NowPage].m_STData;
		m_BuyInfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_NowTData.m_Idx);
		SetUI();
	}
	public void ClickBuy() {
		if (!USERINFO.IS_CanBuy(m_NowTData)) {
			POPUP.StartLackPop(m_NowTData.GetPriceIdx(), false, (result)=> {
				if (result == 1) SetUI();
			});
			return;
		}
		USERINFO.ITEM_BUY(m_NowTData.m_Idx, 1, (res) => {
			if (res == null) return;
			if (!res.IsSuccess())
			{
				if (res.result_code != EResultCode.ERROR_SHOP_BUY_MARKET_ERROR) WEB.StartErrorMsg(res.result_code, (btn, obj) => {});
				return;
			}
			if (res.Rewards != null) {
				List<RES_REWARD_BASE> Rewards = res.GetRewards();
				if (Rewards.Count > 0) {
					MAIN.SetRewardList(new object[] { Rewards }, () => {
						Close(1);
					});
				}
			}
		}, false);
	}
	public void ClickTerms() {
		if (m_Action != null) return;
		string url = "";
#if NOT_USE_NET
		url = "http://59.13.192.250:11000/Files/PFA/Offer_{0}.txt";
#else
		url = WEB.GetConfig(EServerConfig.Offer_url);
#endif

		url = string.Format(url, APPINFO.m_LanguageCode);

		POPUP.Set_WebView(TDATA.GetString(513), url);
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
