using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_SerumStat : ObjMng
{
	[Serializable]
    public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Val;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(StatType _type, float _absval, float _perval) {
		Sprite icon = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)_type), "png");
		m_SUI.Icon.sprite = icon;
		if(_absval > 0 && _perval > 0)
			m_SUI.Val.text = string.Format("{0} +{1}, +{2:0.##}%", TDATA.GetStatString(_type), _absval, _perval * 100f);
		else if(_absval > 0 && _perval == 0)
			m_SUI.Val.text = string.Format("{0} +{1}", TDATA.GetStatString(_type), _absval);
		else if(_absval == 0 && _perval > 0)
			m_SUI.Val.text = string.Format("{0} +{1:0.##}%", TDATA.GetStatString(_type), _perval * 100f);
	}
}
