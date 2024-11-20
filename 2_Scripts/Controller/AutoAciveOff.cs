using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoAciveOff : MonoBehaviour
{
	[SerializeField] private float m_OffTime;
	Transform m_TrackTarget;
	float m_Timer;
	Action<GameObject> m_EndCB;
	private void OnEnable() {
		m_Timer = m_OffTime;
	}
	private void Update() {
		if (m_TrackTarget != null)
			transform.position = m_TrackTarget.position;
		if (m_Timer > 0f)
		{
			m_Timer -= Time.deltaTime;
		}
		else
		{
			m_EndCB?.Invoke(gameObject);
			gameObject.SetActive(false);
		}
	}
	public void SetTime(float _time) {
		m_Timer = _time;
	}
	public float GetTime() {
		return m_OffTime;
	}

	public void SetEndCB(Action<GameObject> CB)
	{
		m_EndCB = CB;
	}

	public void SetTrackTarget(Transform _trans) {
		m_TrackTarget = _trans;
	}
}
