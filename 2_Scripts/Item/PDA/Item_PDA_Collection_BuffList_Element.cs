using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Collection_BuffList_Element : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Buff;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649
	public void SetData(TCollectionTable info)
	{
		m_SUI.LV.text = string.Format("Lv. {0}", info.m_LV);
		m_SUI.Name.text = info.GetName();
		m_SUI.Buff.text = info.GetDes();
	}
}
