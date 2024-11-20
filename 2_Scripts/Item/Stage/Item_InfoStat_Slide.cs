using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_InfoStat_Slide : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI StatVal;
		public TextMeshProUGUI Name;
		public Image Icon;
		public Slider Slide;
		public Image[] SlideBar;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(StatType _type, int _val, float _slideval = 0f) {
		if (m_SUI.Name != null) {
			m_SUI.Name.text = TDATA.GetStatString(_type);
			m_SUI.Name.color = POPUP.StatColor(_type);
		}
		if (m_SUI.Icon != null) {
			m_SUI.Icon.sprite = POPUP.StatIcon(_type);
			m_SUI.Icon.color = POPUP.StatColor(_type);
		}
		m_SUI.StatVal.text = string.Format("{0:#,###}", _val);
		m_SUI.StatVal.color = POPUP.StatColor(_type);
		if (m_SUI.Slide != null) {
			m_SUI.Slide.value = _slideval;
			m_SUI.SlideBar[0].color = m_SUI.SlideBar[1].color = POPUP.StatColor(_type);
		}
	}
}
