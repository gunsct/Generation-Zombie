using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Collection_BuffList_Element_Stat : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Desc;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649
	public void SetData(StatType _type, float _val)
	{
		m_SUI.Desc.text = string.Format("{0} +{1}", TDATA.GetStatString(_type), _type == StatType.Critical ? string.Format("{0:0.0}%", _val * 100f) : Mathf.RoundToInt(_val).ToString());
	}
}
