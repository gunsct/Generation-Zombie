using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_Store_Ticket_Buy : Msg_YN
{
	[System.Serializable]
	struct GSUI
	{
		public Image BuyItemIcon;
		public TextMeshProUGUI BuyItemCnt;
		public Image Icon;
		public TextMeshProUGUI[] Cost;
		public Button YesBtn;
	}
	[SerializeField]
	GSUI m_GSUI;
	int[] m_Val = new int[2];
	public bool IS_CanBuy { get { return m_Val[0] >= m_Val[1]; } }
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		switch ((PriceType)aobjValue[0]) {
			case PriceType.Money:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Dollar);
				m_Val[0] = (int)USERINFO.m_Money;
				break;
			case PriceType.Cash:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Cash);
				m_Val[0] = (int)USERINFO.m_Cash;
				break;
			case PriceType.Energy:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Energy);
				m_Val[0] = (int)USERINFO.m_Energy.Cnt;
				break;
			case PriceType.PVPCoin:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.PVPCoin);
				m_Val[0] = (int)USERINFO.m_PVPCoin;
				break;
			case PriceType.GuildCoin:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Guild_Coin);
				m_Val[0] = (int)USERINFO.m_GCoin;
				break;
			case PriceType.Mileage:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Mileage);
				m_Val[0] = (int)USERINFO.m_Mileage;
				break;
			case PriceType.Item:
				m_GSUI.Icon.sprite = BaseValue.GetItemIcon((int)aobjValue[1]);
				m_Val[0] = (int)USERINFO.GetItemCount((int)aobjValue[1]);
				break;
		}
		//val[0] = (int)aobjValue[1];
		m_Val[1] = (int)aobjValue[2];
		m_GSUI.Cost[0].text = m_Val[0].ToString();
		m_GSUI.Cost[0].color = BaseValue.GetUpDownStrColor((long)m_Val[0], (long)m_Val[1]);
		m_GSUI.Cost[1].text = m_Val[1].ToString();

		int buyitemidx = (int)aobjValue[3];
		int need = (int)aobjValue[4];
		m_GSUI.BuyItemIcon.sprite = TDATA.GetItemTable(buyitemidx).GetItemImg();
		m_GSUI.BuyItemCnt.text = string.Format("{0} / {1}", USERINFO.GetItemCount(buyitemidx), need);

		bool cntckcnt = aobjValue.Length > 5 ? (bool)aobjValue[5] : true;
		m_GSUI.YesBtn.interactable = cntckcnt ? m_Val[0] >= m_Val[1] : true;
	}
}
