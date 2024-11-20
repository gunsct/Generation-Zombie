using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PVP_Research_EffectList_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Val;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(Sprite _sprite, string _name, float _val) {
		m_SUI.Icon.sprite = _sprite;
		m_SUI.Name.text = _name;
		m_SUI.Val.text = string.Format("{0:0.##}%", _val * 100f);
	}
}
