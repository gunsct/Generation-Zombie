using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Collection_BuffList_Element_Title : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Name;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649
	public void SetData(CollectionType _type)
	{
		int idx = 0;
		switch (_type) {
			case CollectionType.Character: idx = 82; break;
			case CollectionType.DNA: idx = 340; break;
			case CollectionType.Equip: idx = 112; break;
			case CollectionType.Zombie: idx = 425; break;
		}
		m_SUI.Name.text = string.Format("{0} {1}",TDATA.GetString(idx), TDATA.GetString(418));
	}
}
