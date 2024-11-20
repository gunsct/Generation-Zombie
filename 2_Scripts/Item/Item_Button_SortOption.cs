using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Button_SortOption : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;

	public void SetName(string name) {
		var uistring = m_SUI.Name.GetComponent<UIString>();
		if (uistring != null) uistring.enabled = false;
		m_SUI.Name.text = name;
	}
}
