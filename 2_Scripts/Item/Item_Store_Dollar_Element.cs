using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_Store_Dollar_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Name;
		public Item_Store_Buy_Button Btn;
		public Image GoodIcon;
	}
	[SerializeField] SUI m_SUI;
	TShopTable m_TData;
	Action<TShopTable> m_CB;
	public void SetData(TShopTable _tdata, Action<TShopTable> _cb) {
		m_TData = _tdata;
		m_CB = _cb;

		m_SUI.Name.text = string.Format("{0} {1}", _tdata.m_Rewards[0].m_ItemCnt, TDATA.GetItemTable(_tdata.m_Rewards[0].m_ItemIdx).GetName());
		m_SUI.Btn.SetData(_tdata.m_Idx);
		m_SUI.GoodIcon.sprite = _tdata.GetImg();
	}

	public void ClickBuy() {
		m_CB.Invoke(m_TData);
	}
}
