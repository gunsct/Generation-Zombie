using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_SeviverDmg : ObjMng
{
#pragma warning disable 0649
	[SerializeField] SpriteRenderer m_Icon;
	[SerializeField] TextMesh m_Dmg;
#pragma warning restore 0649
	public void SetData(StatType type, int Dmg)
	{
		switch(type)
		{
		case StatType.Men:
		case StatType.Hyg:
		case StatType.Sat:
			break;
		default: return;
		}
		m_Icon.sprite = POPUP.StatIcon(type);
		m_Icon.color = POPUP.StatColor(type);
		m_Dmg.text = Dmg.ToString();
		gameObject.SetActive(true);
	}
}
