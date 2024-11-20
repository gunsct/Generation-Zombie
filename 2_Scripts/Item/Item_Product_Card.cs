using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Product_Card : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public GameObject[] IconPanel;
		public Image NonFrameIcon;          //박스 같은 아이콘만 띄울 경우
		public Item_RewardItem_Card FrameCard;  //프레임 있는 아이템일 경우
		public Image MoneyIcon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Price;
		public GameObject Btn;
		public TextMeshProUGUI BtnTxt;//지금은 안쓰고 혹시나 시간제로 팔것들용
	}
	[SerializeField]
	SUI m_SUI;
	Action<TShopTable> m_CB;
	public TShopTable m_ShopInfo;
	public int m_Price;
	public PriceType m_Type;
	public void SetData(TShopTable _shopinfo, TItemTable _iteminfo, Action<TShopTable> _cb) {
		m_ShopInfo = _shopinfo;
		m_CB = _cb;

		TItemTable table = _iteminfo;
		if(table == null) _iteminfo = TDATA.GetItemTable(m_ShopInfo.m_Rewards[0].m_ItemIdx);
		m_SUI.IconPanel[0].SetActive(table.m_Type != ItemType.RandomBox ? false : true);
		if (m_SUI.NonFrameIcon != null)
			m_SUI.NonFrameIcon.sprite = table.GetItemImg();
		m_SUI.IconPanel[1].SetActive(table.m_Type != ItemType.RandomBox ? true : false);
		if(m_ShopInfo != null)
			m_SUI.FrameCard.SetData(table.m_Idx, m_ShopInfo.m_Rewards[0].m_ItemCnt);
		else
			m_SUI.FrameCard.SetData(table.m_Idx, 1);

		if (m_SUI.Name != null)
			m_SUI.Name.text = table.GetName();

		if (m_ShopInfo != null && m_ShopInfo.m_PriceType == PriceType.Cash) {
			m_Type = PriceType.Cash;
			m_Price = m_ShopInfo.GetPrice();
			m_SUI.MoneyIcon.sprite = BaseValue.GetItemIcon(ItemType.Cash);
		}
		else {//달러
			m_Type = PriceType.Money;
			m_Price = table.GetPrice();
			m_SUI.MoneyIcon.sprite = BaseValue.GetItemIcon(ItemType.Dollar);
		}
		if (m_SUI.Price != null)
			m_SUI.Price.text = m_Price.ToString();
	}
	public void ClickBuy() {
		string msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(m_Type));
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
			if (result == 1) {
				m_CB?.Invoke(m_ShopInfo);
			}
		}, m_Type, 0, m_Price);
	}
}
