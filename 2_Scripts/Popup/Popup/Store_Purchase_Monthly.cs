using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Store_Purchase_Monthly : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public GameObject[] RemainGroup;	//0:남은이용기간,1:연장기간
		public TextMeshProUGUI RemainTxt;   //0:남은이용기간,1:연장기간
		public TextMeshProUGUI[] Reward;
		public Item_Store_Buy_Button BuyBtn;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	RecommendGoodsInfo m_Info;
	RES_SHOP_USER_BUY_INFO m_BuyInfo;
	RES_SHOP_DAILYPACK_INFO m_PackInfo;
	List<TPackageRewardTable> m_PTDatas;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Info = (RecommendGoodsInfo)aobjValue[0];
		m_BuyInfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_Info.m_SIdx);
		m_PackInfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == m_Info.m_SIdx);
		m_PTDatas = TDATA.GeTPackageRewardGroupTable(m_Info.m_SIdx);

		PlayerPrefs.SetInt(string.Format("NEW_STORE_GOODS_{0}_{1}", USERINFO.m_UID, m_Info.m_STData.m_Idx), 0);
		PlayerPrefs.Save();

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();

		m_SUI.Reward[0].text = string.Format(TDATA.GetString(10880), m_Info.m_STData.m_Rewards[0].m_ItemCnt);
		m_SUI.Reward[1].text = string.Format(TDATA.GetString(10881), m_PTDatas[0].m_RewardCmt);

		m_SUI.BuyBtn.SetData(m_Info.m_SIdx);
	}

	void TimeUI()
	{
		var isPlay = m_PackInfo != null && m_PackInfo.IsPlayPack();
		if (!isPlay) {
			for (int i = 0; i < m_SUI.RemainGroup.Length; i++)
				m_SUI.RemainGroup[i].SetActive(false);
			return;
		}

		for (int i = 0; i < m_SUI.RemainGroup.Length; i++)
			m_SUI.RemainGroup[i].SetActive(true);

		double remain = m_PackInfo.GetLastTime() * 0.001d;
		int add = m_PTDatas.Count - (int)Math.Floor(remain / 86400);
		m_SUI.RemainTxt.text = string.Format(TDATA.GetString(10876), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, remain));
	}

	private void Update()
	{
		TimeUI();
	}

	public void ClickBuy() {
		if (m_SUI.BuyBtn.CheckLack()) {
			USERINFO.ITEM_BUY(m_Info.m_SIdx, 1, (res) => {
				if (res == null) return;
				if (!res.IsSuccess())
				{
					if (res.result_code != EResultCode.ERROR_SHOP_BUY_MARKET_ERROR) WEB.StartErrorMsg(res.result_code, (btn, obj) => {});
					return;
				}
				if (res.Rewards != null) {
					List<RES_REWARD_BASE> Rewards = res.GetRewards();
					if (Rewards.Count > 0) {
						List<RES_REWARD_BASE> post = Rewards.FindAll(o => o.result_code == EResultCode.SUCCESS_POST);
						if (post.Count > 0) {
							Rewards.RemoveAll(o=> post.Contains(o));
							POPUP.Set_MsgBox(PopupName.Msg_RewardGet_Monthly, (res, obj) => {
								if (Rewards.Count > 0) {
									MAIN.SetRewardList(new object[] { Rewards }, () => {

									});
								}
							}, post);
						}
						else
						MAIN.SetRewardList(new object[] { Rewards }, () => {

						});
					}
				}

				Close(1);
			}, false);
		}
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
