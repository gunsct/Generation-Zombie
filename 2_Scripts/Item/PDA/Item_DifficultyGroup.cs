using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_DifficultyGroup : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image[] Icons;
		public Sprite[] Sprites;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(int _diff) {
		for (int i = 0; i < m_SUI.Icons.Length; i++) {
			m_SUI.Icons[i].sprite = i < _diff ? m_SUI.Sprites[0] : m_SUI.Sprites[1];
		}
	}
}
