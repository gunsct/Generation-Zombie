using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Talk_Char : ObjMng
{
   [System.Serializable]
   struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Name;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(TTalkerTable _talker) {
		m_SUI.Icon.sprite = _talker.GetSprPortrait();
		m_SUI.Name.text = _talker.GetName();
	}
}
