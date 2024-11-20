using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class PVPMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// TouchCheck
	ETouchState m_TouchState;
	Vector3 m_TouchPoint;
	public bool TouchCheck() {
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
		int touchCount = Input.touchCount;
		if(touchCount > 0)// 반응이 있을때
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
#else
		if (m_TouchState != ETouchState.NONE && (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height)) return TouchReleased();
		else if (Input.GetMouseButtonDown(0)) TouchPress();
		else if (Input.GetMouseButton(0)) TouchMove();
		else if (Input.GetMouseButtonUp(0)) return TouchReleased();
#endif
		return false;
	}

	void TouchPress() {
		m_TouchState = ETouchState.PRESS;
		m_TouchPoint = Input.mousePosition;
	}

	void TouchMove() {
		if (m_TouchState == ETouchState.NONE) return;

		switch (m_TouchState) {
			case ETouchState.PRESS:
				if (Mathf.Abs(m_TouchPoint.x - Input.mousePosition.x) >= 10) {
					m_TouchState = ETouchState.MOVE;
				}
				break;
			case ETouchState.MOVE:
				Vector3 v3Gap = m_TouchPoint - Input.mousePosition;
				float fGapX = v3Gap.x;
				m_TouchPoint = Input.mousePosition;
				break;
		}
	}
	bool TouchReleased() {
		bool click = false;
		if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width || Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) {
			m_TouchState = ETouchState.NONE;
			return false;
		}
		if (m_TouchState != ETouchState.MOVE) click = true;
		else {
			Vector3 v3Gap = m_TouchPoint - Input.mousePosition;
			float fGapX = v3Gap.x;
			m_TouchPoint = Input.mousePosition;
		}
		m_TouchState = ETouchState.NONE;

		return click;
	}
}
