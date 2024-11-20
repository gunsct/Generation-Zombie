using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
	[SerializeField] private float m_DestroyTime;
	Transform m_TrackTarget;
	private void Update() {
		if (m_TrackTarget != null)
			transform.position = m_TrackTarget.position;
	}

	private void Start()
	{
		if(m_DestroyTime > 0.1f) GameObject.Destroy(gameObject, m_DestroyTime);
	}

	public float GetTime()
	{
		return m_DestroyTime;
	}

	public void SetTrackTarget(Transform _trans) {
		m_TrackTarget = _trans;
	}
}
