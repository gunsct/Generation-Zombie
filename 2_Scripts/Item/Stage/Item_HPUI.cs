using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_HPUI : ObjMng
{

	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI HP;
		public Image[] Gauge;
	}

	[SerializeField]
	SUI m_SUI;
	Action m_CB;

	void Awake() {
		m_SUI.HP.text = "0";
		if (MainMng.IsValid()) {
			DLGTINFO.f_RfHPUI += SetData;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RfHPUI -= SetData;
		}
	}
	private void OnEnable() {
		m_SUI.Anim.SetTrigger("Normal");
	}
	//Left, Right, Center
	public void SetData(float _crntval, float _preval, float _maxval, Action _cb = null) {
		m_CB = _cb;
		if (_crntval < _preval) {
			string[] trig = new string[3] { "Left", "Center", "Right" };
			m_SUI.Anim.SetTrigger(trig[UTILE.Get_Random(0, trig.Length)]);
		}
		else if(_crntval == _preval) {
			m_SUI.Anim.SetTrigger("Normal");
		}

		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash("from", float.Parse(m_SUI.HP.text), "to", _crntval, "time", 1f, "easetype", "easeinOutQuad", "onupdate", "TW_Counting", "name", "Count"));
		//iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Gauge[0].material.GetFloat("Fill_Amount"), "to", _crntval / _maxval, "time", 1f, "easetype", "easeinOutQuad", "onupdate", "TW_GaugeHead", "name", "Gauge1"));
		//iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Gauge[1].material.GetFloat("Fill_Amount"), "to", _crntval / _maxval, "time", 2f, "easetype", "easeinOutQuad", "onupdate", "TW_GagueTail", "oncomplete", "TW_CB", "name", "Gauge2"));  
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Gauge[0].fillAmount, "to", _crntval / _maxval, "time", 1f, "easetype", "easeinOutQuad", "onupdate", "TW_GaugeHead", "name", "Gauge1"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Gauge[1].fillAmount, "to", _crntval / _maxval, "time", 2f, "easetype", "easeinOutQuad", "onupdate", "TW_GagueTail", "oncomplete", "TW_CB", "name", "Gauge2"));
	}
	void TW_CB() {
		m_CB?.Invoke();
	}
	void TW_GaugeHead(float _amount) {
		//m_SUI.Gauge[0].material.SetFloat("Fill_Amount", _amount);
		m_SUI.Gauge[0].fillAmount = _amount;
	}
	void TW_GagueTail(float _amount) {
		//m_SUI.Gauge[1].material.SetFloat("Fill_Amount", _amount);
		m_SUI.Gauge[1].fillAmount = _amount;
	}
	void TW_Counting(float _amount) {
		m_SUI.HP.text = Mathf.RoundToInt(_amount).ToString();
	}
}
