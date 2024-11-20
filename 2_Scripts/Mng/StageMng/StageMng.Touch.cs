using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// TouchCheck
#pragma warning disable 0414
	bool IsRelease;
#pragma warning restore 0414
	float m_HoldTimer = 0f;
	public bool TouchCheck()
	{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		int touchCount = Input.touchCount;
		if(touchCount == 1)// 반응이 있을때
		{
			switch(Input.GetTouch(0).phase)
			{
			case TouchPhase.Began:			// 터치 시작
				TouchPress();
				break;
			case TouchPhase.Moved:			// 드래그 여부
			case TouchPhase.Stationary:		// 이동중 멈춤
				TouchMove();
				break;
			case TouchPhase.Ended:			// 이동중 멈춤
				return TouchReleased();
			}
		}
		else
		{
			// 투터치일경우 인식 막기
			switch(m_TouchState)
			{
			case ETouchState.PRESS:
			case ETouchState.MOVE:
				return TouchReleased();
			}
			
		}
#else
		if (m_TouchState != ETouchState.NONE && (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)) return TouchReleased();
		else if (Input.GetMouseButtonDown(0)) TouchPress();
		else if (Input.GetMouseButton(0)) TouchMove();
		else if (Input.GetMouseButtonUp(0)) return TouchReleased();
#endif
		return false;
	}

	IEnumerator m_TouchTimer;
	IEnumerator TouchCheckTimer()
	{
		IsRelease = false;
		yield return new WaitForSeconds(0.5f);
		IsRelease = true;

		m_TouchTimer = null;
	}

	void TouchPress()
	{
		m_TouchState = ETouchState.PRESS;
		m_TouchPoint = Input.mousePosition;
		InitCamMove();

		if (m_TouchTimer != null)
		{
			StopCoroutine(m_TouchTimer);
			m_TouchTimer = null;
		}
		m_TouchTimer = TouchCheckTimer();
		StartCoroutine(m_TouchTimer);
	}

	void TouchMove()
	{
		if (m_TouchState == ETouchState.NONE) return;

		switch (m_TouchState)
		{
		case ETouchState.PRESS:
			if (Mathf.Abs(m_TouchPoint.x - Input.mousePosition.x) >= 10)
			{
				m_TouchState = ETouchState.MOVE;
			}
			break;
		case ETouchState.MOVE:
			Vector3 v3Gap = m_TouchPoint - Input.mousePosition;
			float fGapX = v3Gap.x;
			SetCamMove(fGapX * 0.005f);
			m_TouchPoint = Input.mousePosition;
			break;
		}
	}
	bool TouchReleased()
	{
		bool click = false;
		if (m_TouchTimer != null)
		{
			StopCoroutine(m_TouchTimer);
			m_TouchTimer = null;
		}
		IsRelease = false;
		if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) {
			m_TouchState = ETouchState.NONE;
			return false;
		}
		if (m_TouchState != ETouchState.MOVE) click = true;
		else
		{
			Vector3 v3Gap = m_TouchPoint - Input.mousePosition;
			float fGapX = v3Gap.x;
			SetCamMove(fGapX * 0.005f);
			m_TouchPoint = Input.mousePosition;
		}
		m_TouchState = ETouchState.NONE;

		return click;
	}
}
