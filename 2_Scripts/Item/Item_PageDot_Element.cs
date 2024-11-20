using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_PageDot_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public GameObject[] OnOff;
		public Image Gauge;
	}
	[SerializeField] SUI m_SUI;
	Action m_CB;
	public void SetData(bool _on) {
		m_SUI.OnOff[0].SetActive(!_on);
		m_SUI.OnOff[1].SetActive(_on);
	}
	public void SetData(float _time, Action _cb, bool _init = false) {
		if (m_SUI.Gauge == null) return;
		m_SUI.OnOff[0].SetActive(true);
		m_SUI.OnOff[1].SetActive(true);
		m_CB = _cb;
		m_SUI.Gauge.fillAmount = 0f;
		if(!_init) iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "onupdate", "TW_Gauge", "time", _time, "oncomplete", "TW_GaugeEnd", "name", "Gauge"));
	}
	void TW_Gauge(float _amount) {
		m_SUI.Gauge.fillAmount = _amount;
	}
	void TW_GaugeEnd() {
		m_SUI.Gauge.fillAmount = 0f;
		m_CB?.Invoke();
	}
	public void GaugeStop() {
		iTween.StopByName(gameObject, "Gauge");
		m_SUI.Gauge.fillAmount = 0f;
	}
	public void GaugePause(bool _pause) {
		if (_pause) iTween.Pause(gameObject);
		else iTween.Resume(gameObject);
	}
}
