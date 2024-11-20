using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
public class PointerEventCheck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	float m_Time;
	Action<GameObject> m_PressCB;
	Action<GameObject> m_RepeatCB;
	Action<GameObject> m_ReleaseCB;
	bool Select = false;

	private void OnEnable()
	{
		StopCoroutine("ReleaseCheck");
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Select = true;
		m_PressCB?.Invoke(gameObject);
		StartCoroutine("ReleaseCheck");
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Select = false;
		m_ReleaseCB?.Invoke(gameObject);
		StopCoroutine("ReleaseCheck");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Select = false;
		m_ReleaseCB?.Invoke(gameObject);
		StopCoroutine("ReleaseCheck");
	}

	IEnumerator ReleaseCheck()
	{
		yield return new WaitForSeconds(m_Time);
		if(Select) m_RepeatCB?.Invoke(gameObject);
	}

	public void SetCB(Action<GameObject> presscb = null, Action<GameObject> repeatcb = null, Action<GameObject> releasecb = null, float time = 1f)
	{
		m_PressCB = presscb;
		m_RepeatCB = repeatcb;
		m_ReleaseCB = releasecb;
		m_Time = time;
	}
}
