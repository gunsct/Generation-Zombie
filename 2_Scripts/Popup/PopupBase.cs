
using UnityEngine;

public class PopupBase : ObjMng
{
	[HideInInspector] public PopupPos m_PopupPos;
	[HideInInspector] public PopupName m_Popup;
	protected System.Action<int, GameObject> m_EndCB;
	float m_TimeScale = 1f;
	protected bool IS_TouchLock;
	public virtual void ResetUI() { }

	public virtual void SetData(PopupPos pos, PopupName popup, System.Action<int, GameObject> cb, object[] aobjValue)
	{
		if(MAIN.m_State > MainState.TITLE) TUTO.Start(TutoStartPos.POPUP_START, popup, aobjValue);
#if USE_LOG_MANAGER
		string str = $"SetData {name} / {pos} / {popup} / {cb} / {aobjValue}";
		if (aobjValue != null)
		{
			str += " - ";
			foreach (var o in aobjValue)
			{
				if(o == null) str += $" {"null"} ";
				else str += $" {o.ToString()} ";
			}
		}

		Utile_Class.DebugLog(str);
#endif
		
		m_TimeScale = Time.timeScale;
		if (!IsCheckTimeScale()) Time.timeScale = 1;
		m_PopupPos = pos;
		m_Popup = popup;
		m_EndCB = cb;
		SetUI();
		Utile_Class.DebugLog(name + " / " + pos + " / " + popup + " / " + cb + " / " + aobjValue);
	}

	bool IsCheckTimeScale() {
		switch (m_Popup) {
			case PopupName.Stage_Pause:
			case PopupName.Stage_FailCause:
			case PopupName.Stage_Continue:
			case PopupName.PVP_Pause:
			case PopupName.Msg_CenterAlarm:
				return true;
			default:return TUTO.IsTutoPlay();
		}
		//return false;
	}

	public virtual void SetUI() { }

	public virtual void Close(int Result = 0)
	{
		switch(m_Popup)
		{
		case PopupName.Tuto_Start: break;
		default:
			if (m_Popup != PopupName.Msg_CenterAlarm && TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
			break;
		}
		Utile_Class.DebugLog($"Close {name} / {Result}");
		Time.timeScale = m_TimeScale;
		m_EndCB?.Invoke(Result, gameObject);
		POPUP.RemoveUI(m_PopupPos, this);
	}

	public virtual void OnlyInfo() { }

	public virtual void EvtTrig_TouchLock() {
		IS_TouchLock = true;
	}
	public virtual void EvtTrig_TouchUnLock() {
		IS_TouchLock = false;
	}
}
