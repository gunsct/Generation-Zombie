using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_InfoStat : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI StatVal;
		public TextMeshProUGUI Name;
		public Image Icon;
		public Animator Anim;
	}
	[SerializeField]
	protected SUI m_SUI;
	int m_Prestat = 0;
	public void SetData(StatType _type, int _val, bool comma = false) {
		if (m_SUI.Name != null)
			m_SUI.Name.text = TDATA.GetStatString(_type);
		if(m_SUI.Icon != null)
			m_SUI.Icon.sprite = POPUP.StatIcon(_type);
		m_SUI.StatVal.text = comma ? Utile_Class.CommaValue(_val) : _val.ToString();
		if (m_SUI.Anim != null && m_Prestat != 0 && m_Prestat < _val) {
			m_SUI.Anim.SetTrigger("Up");
		}
		m_Prestat = _val;
	}
}
