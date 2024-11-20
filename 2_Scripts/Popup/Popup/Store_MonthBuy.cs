using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Store_MonthBuy : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public RectTransform Prefab;

		public Item_Store_Buy_Button BuyBtn;

		public TextMeshProUGUI Time;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	RES_SHOP_ITEM_INFO m_Info;
	bool m_IsBuy;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		//m_Info = USERINFO.m_ShopInfo.GetInfos(ShopGroup.DailyPack)

		//TPackageRewardTable tdata = TDATA.GeTPackageRewardGroupTable(m_NowTData.m_Idx)[0];
		//for (int i = 0; i < m_SUI.Name.Length; i++) m_SUI.Name[i].text = tdata.GetName();
		//m_SUI.Desc.text = tdata.GetDesc();

		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		m_IsBuy = (bool)aobjValue[0];

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();

		if (m_IsBuy) {
			m_SUI.BuyBtn.gameObject.SetActive(true);
			var tdata = TDATA.GetShopTable(m_Info.Idx);
			switch (tdata.m_PriceType) {
				case PriceType.Money:
					m_SUI.BuyBtn.GetComponent<Button>().interactable = USERINFO.m_Money >= tdata.GetPrice();
					break;
				case PriceType.Cash:
					m_SUI.BuyBtn.GetComponent<Button>().interactable = USERINFO.m_Cash >= tdata.GetPrice();
					break;
			}
			m_SUI.BuyBtn.SetData(tdata.m_Idx);
		}
		else m_SUI.BuyBtn.gameObject.SetActive(false);
	}

	void TimeUI()
	{
		if (m_Info == null) return;
		long time = (long)((m_Info.Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001f);
		m_SUI.Time.text = TimeSpan.FromSeconds(time).ToString(TDATA.GetString(5028));
	}

	private void Update()
	{
		TimeUI();
	}

	public void ClickBuy() {
		if (m_Action != null) return;
		Close(1);
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
