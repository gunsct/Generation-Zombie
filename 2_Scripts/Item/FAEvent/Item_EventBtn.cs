using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_EventBtn : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Timer;
		public GameObject Alarm;
	}
	[SerializeField] SUI m_SUI;
	MyFAEvent m_Info;
	Action<MyFAEvent> m_CB;

	private void Update() {
		if (m_Info == null) return;
		double time = m_Info.GetRemainEndTime() * 0.001d;
		if (time <= 0) time = 0;
		m_SUI.Timer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, time);
	}
	public void SetData(MyFAEvent _info, Action<MyFAEvent> _cb) {
		m_Info = _info;
		m_CB = _cb;
		SetAlarm();
	}
	/// <summary>  </summary>
	void SetAlarm() {
		switch (m_Info.Type) {
			case LS_Web.FAEventType.Stage_Minigame:
				bool mission = USERINFO.m_Mission.IsSuccess(MissionMode.Event_CharMission);
				bool gift = USERINFO.m_Mission.IsSuccess(MissionMode.Event_miniGame) || USERINFO.m_Mission.IsSuccess(MissionMode.Event_miniGame_Clear);
				m_SUI.Alarm.SetActive(mission || gift);
				break;
			case LS_Web.FAEventType.OpenEvent:
				m_SUI.Alarm.SetActive(USERINFO.m_Mission.IsSuccess(MissionMode.OpenEvent, m_Info.EventUID));
				break;
			default:
				m_SUI.Alarm.SetActive(false);
				break;
		}
	}
	public void Click() {
		m_CB?.Invoke(m_Info);
	}
}
