using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Info_Char_DNAStat_Main : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Grade;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(int _idx) {
		TDnaTable tdata = TDATA.GetDnaTable(_idx);
		m_SUI.Icon.sprite = tdata.GetIcon();
		m_SUI.Grade.text = UTILE.Get_RomaNum(tdata.m_Grade);
		m_SUI.Name.text = tdata.GetName();
		m_SUI.Desc.text = tdata.GetDesc();
	}
}
