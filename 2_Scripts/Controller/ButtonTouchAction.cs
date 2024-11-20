using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonTouchAction : ObjMng, IPointerDownHandler, IPointerUpHandler
{
	Button m_Button;
	Vector3 m_Scale;

	private void Awake() {
		m_Button = GetComponent<Button>();
		m_Scale = transform.localScale;
	}
	public void OnPointerDown(PointerEventData eventData) {
		iTween.StopByName(gameObject, "Up");
		iTween.StopByName(gameObject, "Down");
		iTween.ValueTo(gameObject, iTween.Hash("from", transform.localScale.x, "to", 0.95f, "time", 0.08f, "onupdate", "TW_Scale","name", "Down"));
	}

	public void OnPointerUp(PointerEventData eventData) {
		iTween.StopByName(gameObject, "Up");
		iTween.StopByName(gameObject, "Down");
		iTween.ValueTo(gameObject, iTween.Hash("from", transform.localScale.x, "to", m_Scale.x, "time", 0.08f, "onupdate", "TW_Scale", "name", "Up"));
	}

	void TW_Scale(float _amount) {
		transform.localScale = Vector3.one * _amount;
	}
}
