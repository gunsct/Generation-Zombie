using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Adv_CharCountGroup : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public Sprite[] Sprites;
		public TextMeshProUGUI Count;
		public Color[] TxtColor;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(int _count, int _max) {
		m_SUI.Icon.sprite = _count < _max ? m_SUI.Sprites[0] : m_SUI.Sprites[1];
		m_SUI.Count.color = m_SUI.TxtColor[_count < _max ? 0 : 1];
		m_SUI.Count.text = string.Format("{0}/{1}", _count, _max);
	}
}
