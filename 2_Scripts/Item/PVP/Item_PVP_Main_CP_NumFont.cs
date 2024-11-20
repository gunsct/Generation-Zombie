using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_PVP_Main_CP_NumFont : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(Sprite _img, Color _color) {
		m_SUI.Icon.sprite = _img;
		m_SUI.Icon.color = _color;
	}
}
