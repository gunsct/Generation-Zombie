using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PowerMark : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public Sprite[] Sprites;
		public TextMeshProUGUI Val;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(SortingType _type, CharInfo _info, bool _pvp = false) {
		switch (_type) {
			case SortingType.CombatPower:
			case SortingType.Grade:
			case SortingType.Level:
				m_SUI.Icon.sprite = m_SUI.Sprites[(int)SortingType.CombatPower];
				m_SUI.Val.text = Utile_Class.CommaValue(_info == null ? 0 : (_pvp ? _info.m_PVPCP : _info.m_CP));
				break;
			case SortingType.Men:
				m_SUI.Icon.sprite = m_SUI.Sprites[(int)_type];
				m_SUI.Val.text = Utile_Class.CommaValue(_info == null ? 0 : Mathf.RoundToInt(_info.GetStat(StatType.Men)));
				break;
			case SortingType.Hyg:
				m_SUI.Icon.sprite = m_SUI.Sprites[(int)_type];
				m_SUI.Val.text = Utile_Class.CommaValue(_info == null ? 0 : Mathf.RoundToInt(_info.GetStat(StatType.Hyg)));
				break;
			case SortingType.Sat:
				m_SUI.Icon.sprite = m_SUI.Sprites[(int)_type];
				m_SUI.Val.text = Utile_Class.CommaValue(_info == null ? 0 : Mathf.RoundToInt(_info.GetStat(StatType.Sat)));
				break;
		}
	}
}
