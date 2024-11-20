using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Store_GatchaMileage : ObjMng
{
	public enum State
	{
		Normal,
		Limit,
		SoldOut
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI StockMsg;
		public TextMeshProUGUI StockCnt;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Price;
		public GameObject SeasonMark;
	}
	[SerializeField] SUI m_SUI;
	TShopTable m_TData;
	LS_Web.RES_SHOP_USER_BUY_INFO m_BuyInfo;
	Action<int, List<int>> m_CB;
	Action m_CB2;
	public void SetData(TShopTable _tdata, Action<int, List<int>> _cb, Action _cb2) {
		m_TData = _tdata;
		m_CB = _cb;
		m_CB2 = _cb2;
		m_BuyInfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_TData.m_Idx);

		if(m_TData.m_LimitCnt == 0) {
			m_SUI.Anim.SetTrigger("Product_Normal");
			m_SUI.StockMsg.text = TDATA.GetString(1049);
		}
		else if(m_BuyInfo != null && m_BuyInfo?.Cnt >= m_TData.m_LimitCnt) {
			m_SUI.Anim.SetTrigger("SoldOut");
			m_SUI.StockMsg.text = TDATA.GetString(1051);
		}
		else {
			m_SUI.Anim.SetTrigger("Product_Limit");
			m_SUI.StockMsg.text = TDATA.GetString(1049);
		}

		LS_Web.RES_REWARD_BASE item = MAIN.GetRewardBase(m_TData, m_TData.m_Rewards[0].m_ItemType)[0];
		m_SUI.Item.SetData(item, null, false);

		m_SUI.StockCnt.text = (m_TData.m_LimitCnt - (m_BuyInfo == null ? 0 : m_BuyInfo.Cnt)).ToString();

		int price = m_TData.GetPrice();
		m_SUI.Price.text = Utile_Class.CommaValue(price);
		m_SUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Mileage, price);
		m_SUI.SeasonMark.SetActive(m_TData.m_TagType == TagType.SEASON);
	}
	public void Click_Buy() {
		if (m_TData.m_Rewards[0].m_ItemType == RewardKind.Item && TDATA.GetItemTable(m_TData.m_Rewards[0].m_ItemIdx).m_Type == ItemType.Select) {
			Action <int,List<int>> cb = (res, list) => {
				if (m_BuyInfo != null && m_BuyInfo?.Cnt >= m_TData.m_LimitCnt && m_TData.m_LimitCnt > 0) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7007));
					return;
				}
				else if (m_TData.GetPrice() > USERINFO.m_Mileage) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(1050));
					return;
				}
				m_CB?.Invoke(res, list);
			};
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_Select, (res, obj) => {
				if (res == 1) {
					m_CB2?.Invoke();
				}
			}, m_TData.m_Idx, Store_Select.State.Shop, cb);
		}
		else {
			if (m_BuyInfo != null && m_BuyInfo?.Cnt >= m_TData.m_LimitCnt && m_TData.m_LimitCnt > 0) {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(7007));
				return;
			}
			else if (m_TData.GetPrice() > USERINFO.m_Mileage) {
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(1050));
				return;
			}
			m_CB?.Invoke(m_TData.m_Idx, null);
		}
	}
}
