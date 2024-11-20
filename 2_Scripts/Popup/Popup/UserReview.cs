using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class UserReview : PopupBase
{
	public enum State
	{
		Start,
		No,
		Yes,
		End
	}
    [Serializable]
    public struct SUI
	{
		public Animator Ani;
		public TextMeshProUGUI Msg;
		public TextMeshProUGUI[] BtnLabel;
	}
	[SerializeField] SUI m_SUI;

	State m_Page;
	bool m_BtnLock;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		StateChange(State.Start);
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void StateChange(State state)
	{
		m_BtnLock = true;
		m_Page = state;
		switch (m_Page)
		{
		case State.Start:
			m_SUI.Msg.text = TDATA.GetString(1801);
			m_SUI.BtnLabel[0].text = TDATA.GetString(1807);
			m_SUI.BtnLabel[1].text = TDATA.GetString(1806);
			return;
		case State.Yes:
			m_SUI.Msg.text = TDATA.GetString(1802);
			m_SUI.BtnLabel[0].text = TDATA.GetString(288);
			m_SUI.BtnLabel[1].text = TDATA.GetString(1804);
			break;
		case State.No:
			m_SUI.Msg.text = TDATA.GetString(1803);
			m_SUI.BtnLabel[0].text = TDATA.GetString(288);
			m_SUI.BtnLabel[1].transform.parent.gameObject.SetActive(false);
			break;
		}
		m_SUI.Ani.SetTrigger(m_Page.ToString());
	}

	public void AniEnd()
	{
		m_BtnLock = false;
	}

	public void OnYes()
	{
		if (m_BtnLock) return;
		switch(m_Page)
		{
		case State.Start:
			StateChange(State.Yes);
			break;
		case State.Yes:
			HIVE.Review();
			Close(1);
			//MAIN.m_pReview.SetReview((result) => {
			//	Close(1);
			//});
			break;
		case State.No:
#if NOT_USE_NET
			UTILE.OpenURL("https://forms.gle/829YJDmVY6D3KLLN9");
#else
			UTILE.OpenURL(WEB.GetConfig(EServerConfig.PeedBack_url));
#endif
			break;
		}
	}
	public void OnNo()
	{
		if (m_BtnLock) return;
		switch (m_Page)
		{
		case State.Start:
			StateChange(State.No);
			break;
		case State.Yes:
		case State.No:
			Close();
			break;
		}
	}
}
