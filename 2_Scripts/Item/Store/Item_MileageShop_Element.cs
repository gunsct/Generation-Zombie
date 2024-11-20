using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_MileageShop_Element : ObjMng
{
	public enum State
	{
		Normal,
		Lock
	}
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI BuyCnt;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Price;
	}
	[SerializeField] SUI m_SUI;
	TShopTable m_TData;
	LS_Web.RES_SHOP_USER_BUY_INFO m_BuyInfo;
	Action<int> m_CB;
	public void SetData(TShopTable _tdata, Action<int> _cb) {
		m_TData = _tdata;
		m_BuyInfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TData.m_Idx);

		int price = m_TData.GetPrice();
		m_SUI.Price.text = Utile_Class.CommaValue(price);
		m_SUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Mileage, price);
	}
	public void Click_Buy() {
		if (m_BuyInfo.Cnt >= m_TData.m_LimitCnt) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7007));
			return;
		}
		else if(m_TData.GetPrice() > USERINFO.m_Mileage) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, "마일리지가 부족합니다");
			return;
		}
		m_CB?.Invoke(m_TData.m_Idx);
	}
}
