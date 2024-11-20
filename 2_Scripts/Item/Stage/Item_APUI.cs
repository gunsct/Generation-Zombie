using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_APUI : ObjMng
{

	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI AP;
		public Image Gauge;
		public Image GaugeFX;
		public Image Deco;
		public Animator Anim;
	}

	[SerializeField]
	SUI m_SUI;

	void Awake() {
		TW_Gauge(0f);
		if (MainMng.IsValid()) {
			DLGTINFO.f_RfAPUI += SetData;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RfAPUI -= SetData;
		}
	}
	//Left, Right, Center
	public void SetData(float _crntval,float _preval, float _maxval) {
		iTween.StopByName(gameObject, "Count");
		iTween.StopByName(gameObject, "Gauge");
		iTween.ValueTo(gameObject, iTween.Hash("from", _preval, "to", _crntval, "time", 1f, "easetype", "easeOutQuint", "onupdate", "TW_Counting", "name", "Count"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.Gauge.fillAmount, "to", _crntval / _maxval, "time", 1f, "onupdate", "TW_Gauge", "easetype", "easeOutQuint", "name", "Gauge"));
		if (_preval < _crntval) PlayEffSound(SND_IDX.SFX_0454);
		if (_crntval != _preval) m_SUI.Anim.SetTrigger("Change");
	}

	IEnumerator ActionEndCheck()
	{
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
	}
	public void TutoAPGaugeAction(float _delay, Action _cb) {
		gameObject.SetActive(true);//50/234
		StartCoroutine(IE_TutoAPGaugeStartAction(_delay, _cb));
	}
	IEnumerator IE_TutoAPGaugeStartAction(float _delay, Action _cb) {
		m_SUI.Anim.SetTrigger("FirstIn");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, _delay));

		m_SUI.Anim.speed = 0f;

		yield return new WaitForSeconds(3f);

		_cb?.Invoke();
	}
	public void TuToApGaugeFullAction(float _mul) {
		m_SUI.Anim.speed = 1f;
		STAGE_USERINFO.m_AP[0] = Mathf.RoundToInt(STAGE_USERINFO.m_AP[1] * _mul);
		DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], 0, STAGE_USERINFO.m_AP[1]);
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			STAGE.m_Chars[i].SetAPUI(STAGE_USERINFO.m_AP[0]);
		}
	}
	void TW_Gauge(float _amount) {
		m_SUI.Gauge.fillAmount = _amount;
		m_SUI.GaugeFX.fillAmount = _amount;
		m_SUI.Deco.fillAmount = _amount;
	}
	void TW_Counting(float _amount) {
		m_SUI.AP.text = Mathf.RoundToInt(_amount).ToString();
	}
}
