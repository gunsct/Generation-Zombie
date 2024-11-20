using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_CharManageCard_EventReward : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Cnt;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(int _idx, int _cnt) {
		m_SUI.Icon.sprite = TDATA.GetItemTable(_idx).GetItemImg();
		m_SUI.Cnt.text = _cnt.ToString();
	}
}
