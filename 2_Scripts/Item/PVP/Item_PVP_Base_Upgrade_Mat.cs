using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class Item_PVP_Base_Upgrade_Mat : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Cnt;
	}
	[SerializeField] SUI m_SUI;
	public bool Is_Can;
	public void SetData(TPVP_Camp_NodeLevel _tdata, int _pos, int _getcnt) {
		int needcnt = _tdata.m_Cost[_pos];
		if (needcnt == 0) gameObject.SetActive(false);
		else {
			if (needcnt == 0) gameObject.SetActive(true);
			m_SUI.Cnt.text = string.Format("{0}/{1}", _getcnt, needcnt);
			m_SUI.Cnt.color = BaseValue.GetUpDownStrColor(_getcnt, needcnt);
			Is_Can = _getcnt >= needcnt;
		}
	}
}
