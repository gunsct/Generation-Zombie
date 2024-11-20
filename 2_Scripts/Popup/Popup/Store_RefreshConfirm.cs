using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Store_RefreshConfirm : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI AdsCnt;
		public Item_AD_Btn AdsBtn;
		public TextMeshProUGUI CashPrice;
		public Image PriceIcon;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	TShopTable m_CashTable;
	TShopTable m_AdsTable;
	int m_AdsTimer;
	RES_SHOP_USER_BUY_INFO m_AdsBuyInfo;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_CashTable = (TShopTable)aobjValue[0];
		m_AdsTable = (TShopTable)aobjValue[1];
		m_AdsBuyInfo = (RES_SHOP_USER_BUY_INFO)aobjValue[2];

		m_AdsTimer = m_AdsTable.GetPrice() * 60;

		m_SUI.CashPrice.text = m_CashTable.GetPrice().ToString();
		m_SUI.CashPrice.color = BaseValue.GetUpDownStrColor(USERINFO.m_Cash, m_CashTable.GetPrice(), "#D2533C", "#FFFFFF");
		switch (m_CashTable.m_PriceType) {
			case PriceType.Money:
				m_SUI.PriceIcon.sprite = BaseValue.GetItemIcon(ItemType.Dollar);
				break;
			case PriceType.Cash:
				m_SUI.PriceIcon.sprite = BaseValue.GetItemIcon(ItemType.Cash);
				break;
			case PriceType.Energy:
				m_SUI.PriceIcon.sprite = BaseValue.GetItemIcon(ItemType.Energy);
				break;
		}
	}

	private void Update() {
		double blackmarkettime = m_AdsBuyInfo == null ? 0 : m_AdsBuyInfo.UTime;
		double difftime = m_AdsTimer - (UTILE.Get_ServerTime_Milli() - blackmarkettime) * 0.001;
		string time = string.Format(" ({0})", UTILE.GetSecToTimeStr(difftime));
		string cnt = string.Format(" ({0}/{1})", m_AdsBuyInfo == null ? 0 : m_AdsBuyInfo.Cnt, m_AdsTable.m_LimitCnt);
		if (difftime > 0 && m_AdsBuyInfo != null)
		{
			m_SUI.AdsBtn.SetLabel(time, false, time);
			m_SUI.AdsBtn.Interactable(false);
		}
		else
		{
			m_SUI.AdsBtn.Interactable(m_AdsTable.m_LimitCnt - (m_AdsBuyInfo == null ? 0 : m_AdsBuyInfo.Cnt) > 0);
			m_SUI.AdsBtn.SetLabel(string.Format("{0} ({1}/{2})", TDATA.GetString(5107), m_AdsBuyInfo == null ? 0 : m_AdsBuyInfo.Cnt, m_AdsTable.m_LimitCnt), false, cnt);
		}
		m_SUI.AdsCnt.text = string.Format("{0}{1}", m_AdsTable.m_LimitCnt - (m_AdsBuyInfo == null ? 0 : m_AdsBuyInfo.Cnt), TDATA.GetString(5103));
	}

	public void Click_Ads() {
		if (m_Action != null) return;
		if (m_AdsTable.m_LimitCnt - (m_AdsBuyInfo == null ? 0 : m_AdsBuyInfo.Cnt) < 1) return;

		ADS.ShowAds((result) => {
			if (result == IAds.ResultCode.Succecss)
				Close(2);
		});
	}
	public void Click_Cash() {
		if (m_Action != null) return;
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(5020), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					Close(1);
				}
				else {
					POPUP.StartLackPop(m_CashTable.GetPriceIdx(), false, (result) => {
						Close(3);
					});
				}
			}
		}, m_CashTable.m_PriceType, m_CashTable.m_PriceIdx, m_CashTable.GetPrice(), false);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}

	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
