using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_EquipOption : ObjMng
{
  [Serializable]
  public struct SUI
	{
		public TextMeshProUGUI Optiontxt;
		public GameObject Btn;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(string _txt) {
		m_SUI.Optiontxt.text = _txt;
	}
	public void SetBtn(bool _enable) {
		if(m_SUI.Btn != null) m_SUI.Btn.SetActive(_enable);
	}
}
