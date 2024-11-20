using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_Store_Cash_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI[] Name;
		public Item_Store_Buy_Button Btn;
		public Image GoodIcon;
	}
	[SerializeField] SUI m_SUI;
	TShopTable m_TData;
	Action<TShopTable> m_CB;
	public void SetData(TShopTable _tdata, Action<TShopTable> _cb) {
		m_TData = _tdata;
		m_CB = _cb;

		m_SUI.Name[0].text = m_SUI.Name[1].text = string.Format("x{0}", _tdata.m_Rewards[0].m_ItemCnt);
		m_SUI.Btn.SetData(_tdata.m_Idx);
		m_SUI.GoodIcon.sprite = m_TData.GetImg();
		//float prey = m_SUI.GoodIcon.GetComponent<RectTransform>().rect.height;
		//m_SUI.GoodIcon.SetNativeSize();
		//float crnty = m_SUI.GoodIcon.GetComponent<RectTransform>().rect.height;
		//m_SUI.GoodIcon.transform.localScale = Vector3.one * prey / crnty;
	}

	public void ClickBuy() {
		m_CB.Invoke(m_TData);
	}
}
