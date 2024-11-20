using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Item_Store_Buy_Button : ObjMng
{
	[Serializable]
	public struct SUI //패스 상품
	{
		public TextMeshProUGUI Price;
		public Image Icon;
		public TextMeshProUGUI DiffPrice;
		public Image DiffIcon;
		public GameObject[] DiffObj;	//+, price, icon
		public Item_Store_DoubleMark DoubleMark;
		public Image BG;
		public Sprite[] BGImg;
	}
	[SerializeField] SUI m_sUI;
	LS_Web.RES_SHOP_PID_INFO m_Pinfo;
	public bool Is_Pay { get { return m_Pinfo != null; } }
	public bool Is_CanBuy;
	TShopTable m_TData { get { return TDATA.GetShopTable(m_Idx); } }
	int m_Idx, m_Cnt;
	public void SetData(int Idx, int Cnt = 1) {
		Is_CanBuy = false;
		m_Pinfo = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == Idx);
		m_Idx = Idx;
		m_Cnt = Cnt;
		if (m_Pinfo == null)
		{
			var tdata = TDATA.GetShopTable(Idx);
			SetData(tdata.m_PriceType, tdata.GetPrice(Cnt), tdata.m_PriceIdx);
		}
		else
		{
			m_sUI.Icon.gameObject.SetActive(false);
			var price = IAP.GetPrice(m_Pinfo.PID);
			m_sUI.Price.text = string.IsNullOrEmpty(price) ? m_Pinfo.PriceText : price;
			Is_CanBuy = true;
		}
		if (m_sUI.BG != null && m_sUI.BGImg.Length > 1) m_sUI.BG.sprite = m_sUI.BGImg[Is_CanBuy ? 0 : 1];
		if (m_sUI.DoubleMark != null) m_sUI.DoubleMark.SetData(Idx);
	}

	public void SetData(PriceType type, int price, int _itemidx = 0)
	{
		for (int i = 0; i < m_sUI.DiffObj.Length; i++)
			m_sUI.DiffObj[i].SetActive(false);
		// 일반 구매 상품
		m_sUI.Icon.gameObject.SetActive(true);
		m_sUI.Price.text = Utile_Class.CommaValue(price);
		switch (type)
		{
			case PriceType.Money:
				m_sUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Dollar);
				m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, price, "#D2533C", "#FFFFFF");
				Is_CanBuy = USERINFO.m_Money >= price;
				break;
			case PriceType.Cash:
				m_sUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Cash);
				m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Cash, price, "#D2533C", "#FFFFFF");
				Is_CanBuy = USERINFO.m_Cash >= price;
				break;
			case PriceType.Energy:
				m_sUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Energy);
				m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Energy.Cnt, price, "#D2533C", "#FFFFFF");
				Is_CanBuy = USERINFO.m_Energy.Cnt >= price;
				break;
			case PriceType.PVPCoin:
				m_sUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.PVPCoin);
				m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_PVPCoin, price, "#D2533C", "#FFFFFF");
				Is_CanBuy = USERINFO.m_PVPCoin >= price;
				break;
			case PriceType.GuildCoin:
				m_sUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Guild_Coin);
				m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_GCoin, price, "#D2533C", "#FFFFFF");
				Is_CanBuy = USERINFO.m_GCoin >= price;
				break;
			case PriceType.Mileage:
				m_sUI.Icon.sprite = BaseValue.GetItemIcon(ItemType.Mileage);
				m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Mileage, price, "#D2533C", "#FFFFFF");
				Is_CanBuy = USERINFO.m_Mileage >= price;
				break;
			case PriceType.Item:
				if (_itemidx > 0) {
					m_sUI.Icon.sprite = BaseValue.GetItemIcon(_itemidx);
					m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.GetItemCount(_itemidx), price, "#D2533C", "#FFFFFF");
					Is_CanBuy = USERINFO.GetItemCount(_itemidx) >= price;
				}
				break;
		}
	}
	public void SetData(PriceType[] _difftype, int[] _diffprice, int[] _diffitemidx) {
		if(_diffprice[0] > 0 && _diffprice[1] < 1) {
			SetData(_difftype[0], _diffprice[0], _diffitemidx[0]);
		}
		else if(_diffprice[0] > 0 && _diffprice[1] > 0) {
			Is_CanBuy = true;

			for (int i = 0; i < m_sUI.DiffObj.Length; i++)
				m_sUI.DiffObj[i].SetActive(true);

			Image[] Icons = new Image[2] { m_sUI.Icon, m_sUI.DiffIcon };
			TextMeshProUGUI[] Prices = new TextMeshProUGUI[2] { m_sUI.Price, m_sUI.DiffPrice };
			// 일반 구매 상품
			for (int i = 0; i < 2; i++) {
				Icons[i].gameObject.SetActive(true);
				Prices[i].text = Utile_Class.CommaValue(_diffprice[i]);
				switch (_difftype[i]) {
					case PriceType.Money:
						Icons[i].sprite = BaseValue.GetItemIcon(ItemType.Dollar);
						m_sUI.Price.color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, _diffprice[i], "#D2533C", "#FFFFFF");
						if(Is_CanBuy) Is_CanBuy = USERINFO.m_Money >= _diffprice[i];
						break;
					case PriceType.Cash:
						Icons[i].sprite = BaseValue.GetItemIcon(ItemType.Cash);
						Prices[i].color = BaseValue.GetUpDownStrColor(USERINFO.m_Cash, _diffprice[i], "#D2533C", "#FFFFFF");
						if (Is_CanBuy) Is_CanBuy = USERINFO.m_Cash >= _diffprice[i];
						break;
					case PriceType.Energy:
						Icons[i].sprite = BaseValue.GetItemIcon(ItemType.Energy);
						Prices[i].color = BaseValue.GetUpDownStrColor(USERINFO.m_Energy.Cnt, _diffprice[i], "#D2533C", "#FFFFFF");
						if (Is_CanBuy) Is_CanBuy = USERINFO.m_Energy.Cnt >= _diffprice[i];
						break;
					case PriceType.PVPCoin:
						Icons[i].sprite = BaseValue.GetItemIcon(ItemType.PVPCoin);
						Prices[i].color = BaseValue.GetUpDownStrColor(USERINFO.m_PVPCoin, _diffprice[i], "#D2533C", "#FFFFFF");
						if (Is_CanBuy) Is_CanBuy = USERINFO.m_PVPCoin >= _diffprice[i];
						break;
					case PriceType.GuildCoin:
						Icons[i].sprite = BaseValue.GetItemIcon(ItemType.Guild_Coin);
						Prices[i].color = BaseValue.GetUpDownStrColor(USERINFO.m_GCoin, _diffprice[i], "#D2533C", "#FFFFFF");
						if (Is_CanBuy) Is_CanBuy = USERINFO.m_GCoin >= _diffprice[i];
						break;
					case PriceType.Mileage:
						Icons[i].sprite = BaseValue.GetItemIcon(ItemType.Mileage);
						Prices[i].color = BaseValue.GetUpDownStrColor(USERINFO.m_Mileage, _diffprice[i], "#D2533C", "#FFFFFF");
						if (Is_CanBuy) Is_CanBuy = USERINFO.m_Mileage >= _diffprice[i];
						break;
					case PriceType.Item:
						if (_diffitemidx[i] > 0) {
							Icons[i].sprite = BaseValue.GetItemIcon(_diffitemidx[i]);
							Prices[i].color = BaseValue.GetUpDownStrColor(USERINFO.GetItemCount(_diffitemidx[i]), _diffprice[i], "#D2533C", "#FFFFFF");
							if (Is_CanBuy) Is_CanBuy = USERINFO.GetItemCount(_diffitemidx[i]) >= _diffprice[i];
						}
						break;
				}
			}
		}
		else if(_diffprice[0] < 1) {

			SetData(_difftype[1], _diffprice[1], _diffitemidx[1]);
		}
	}

	public bool CheckLack()
	{
		if(!Is_CanBuy)	POPUP.StartLackPop(m_TData.GetPriceIdx());
		return Is_CanBuy;
	}
}
