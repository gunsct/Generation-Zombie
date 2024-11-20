using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectEventTrigger : EventTrigger
{
	ScrollRect _Scroll;
	public ScrollRect m_Scroll {
		get{
			if(_Scroll == null)
			{
				Transform parent = transform.parent;
				while (!_Scroll)
				{
					_Scroll = parent.GetComponent<ScrollRect>();
					parent = parent.parent;
					if (parent == null) break;
				}
			}
			return _Scroll;
		}
	}
	PointerEventData m_PressData;
	PointerEventData m_BeginDrag;
	bool m_IsEventTrigger;
	bool m_IsScrollTrigger;
	double m_TimeCheck;
	private void Awake()
	{
		m_PressData = null;
		m_IsEventTrigger = false;
	}

	private void Update()
	{
		if(m_PressData != null)
		{
			if(m_IsScrollTrigger)
			{
				InitPress();
				return;
			}
			m_TimeCheck += Time.deltaTime;
			if(m_TimeCheck >= 0.3f)
			{
				base.OnPointerDown(m_PressData);
				m_PressData = null;
				m_IsEventTrigger = true;
			}
		}
	}

	void InitPress()
	{
		m_PressData = null;
		m_TimeCheck = 0;
		m_IsEventTrigger = false;
	}

	public override void OnPointerDown(PointerEventData eventData)
	{
		m_IsScrollTrigger = false;
		m_PressData = eventData;
		m_TimeCheck = 0;
		m_IsEventTrigger = false;
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		InitPress();
		if (m_IsScrollTrigger) return;
		base.OnPointerExit(eventData);
	}

	public override void OnPointerUp(PointerEventData eventData)
	{
		InitPress();
		if (m_IsScrollTrigger) return;
		base.OnPointerUp(eventData);
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (m_IsScrollTrigger) return;
		InitPress();
		base.OnPointerClick(eventData);
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		if (m_IsEventTrigger) return;
		m_BeginDrag = eventData;
	}

	public override void OnDrag(PointerEventData eventData)
	{
		if (m_IsEventTrigger) return;
		if (m_Scroll == null) return;
		if (m_BeginDrag != null)
		{
			if (Mathf.Abs(eventData.delta.x) < 2 && Mathf.Abs(eventData.delta.y) < 2) return;
			InitPress();
			m_Scroll.OnBeginDrag(m_BeginDrag);
			m_BeginDrag = null;
			m_IsScrollTrigger = true;
		}
		m_Scroll.OnDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		if (m_IsEventTrigger) return;
		if (m_Scroll == null) return;
		InitPress();
		m_Scroll.OnEndDrag(eventData);
		m_IsScrollTrigger = false;
	}
}
