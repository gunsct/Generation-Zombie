using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_ToolTipStat : ObjMng
{
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Amount;
		public RawImage BG;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(string _name, float _val) {
		m_SUI.Name.text = _name;
		m_SUI.Amount.text = _val.ToString("+#.##;-#.##;0");
	}
	public void SetColor(Color _bg, Color _val) {
		m_SUI.BG.color = _bg;
		m_SUI.Name.color = _val;
		m_SUI.Amount.color = _val;
	}
}
