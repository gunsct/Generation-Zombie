using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Store_PurchaseConfirm : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_RewardList_Item Reward;
		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI Desc;
		public Item_Store_Buy_Button Btn;
		public GameObject Notice;
		public GameObject PassItem;
		public GameObject FX;
		public Button BuyBtn;
		public GameObject[] PayObjs;	//0:달러1:금니2:pvp코인3:길드코인
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	TShopTable m_TData;
	bool Is_CheckCnt;
	public bool IS_CanBuy;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_SUI.Reward.SetData((RES_REWARD_BASE)aobjValue[0], null, false);
		m_TData = (TShopTable)aobjValue[1];
		Is_CheckCnt = aobjValue.Length > 2 ? (bool)aobjValue[2] : true;
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();

		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		DLGTINFO?.f_RFCoinUI?.Invoke(USERINFO.m_PVPCoin, USERINFO.m_PVPCoin);
		DLGTINFO?.f_RFGCoinUI?.Invoke(USERINFO.m_GCoin, USERINFO.m_GCoin);

		m_SUI.Btn.SetData(m_TData.m_Idx);
		bool ispay = m_SUI.Btn.Is_Pay;
		PriceType pricetype = ispay ? PriceType.Pay : m_TData.m_PriceType;
		switch (m_TData.m_Group) {
			case ShopGroup.Pass:
				m_SUI.Notice.SetActive(true);
				m_SUI.FX.SetActive(false);
				m_SUI.PassItem.SetActive(true);
				m_SUI.Reward.gameObject.SetActive(false);
				break;
			default:
				m_SUI.Notice.SetActive(false);
				m_SUI.FX.SetActive(true);
				m_SUI.PassItem.SetActive(false);
				m_SUI.Reward.gameObject.SetActive(true);
				break;
		}
		for (int i = 0; i < m_SUI.PayObjs.Length; i++) m_SUI.PayObjs[i].SetActive(false);
		switch (pricetype) {
			case PriceType.Money:
				IS_CanBuy = USERINFO.m_Money >= m_TData.GetPrice();
				m_SUI.BuyBtn.interactable = Is_CheckCnt ? IS_CanBuy : true; 
				m_SUI.PayObjs[0].SetActive(true);
				break;
			case PriceType.Cash:
				IS_CanBuy = USERINFO.m_Cash >= m_TData.GetPrice();
				m_SUI.BuyBtn.interactable = Is_CheckCnt ? IS_CanBuy : true;
				m_SUI.PayObjs[1].SetActive(true);
				break;
			case PriceType.Energy:
				IS_CanBuy = USERINFO.m_Energy.Cnt >= m_TData.GetPrice();
				break;
			case PriceType.PVPCoin:
				IS_CanBuy = USERINFO.m_PVPCoin >= m_TData.GetPrice();
				m_SUI.PayObjs[2].SetActive(true);
				break;
			case PriceType.GuildCoin:
				IS_CanBuy = USERINFO.m_GCoin >= m_TData.GetPrice();
				m_SUI.PayObjs[3].SetActive(true);
				break;
			case PriceType.AD:
			case PriceType.Pay:
				IS_CanBuy = true;
				break;
		}

		string name = m_TData.GetName();
		string desc = m_TData.GetInfo();
		if (m_TData.m_Rewards.Count > 0 && m_TData.m_Rewards[0].m_ItemType == RewardKind.DNA)
		{
			var tdna = TDATA.GetDnaTable(m_TData.m_Rewards[0].m_ItemIdx);
			var val = tdna.m_OptionVal * 100f;
			desc = string.Format(desc, val, val);
		}
		//string name = string.Empty;
		//string desc = string.Empty;
		//switch (m_TData.m_Group) {
		//	case ShopGroup.Pass:
		//		break;
		//	default:
		//		TItemTable table = TDATA.GetItemTable(m_TData.m_Rewards[0].m_ItemIdx);
		//		name = m_TData.GetName();
		//		desc = m_TData.GetInfo();
		//		break;
		//}
		m_SUI.Name[0].text = m_SUI.Name[1].text = name;
		m_SUI.Desc.text = desc;
	}
	public void ClickBuy() {
		if (m_Action != null) return;
		Close(1);
	}
	public void ClickTerms()
	{
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
