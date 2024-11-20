using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_Timer : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Msg;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	int m_MsgIdx;
	long m_ETime;
	Utile_Class.TimeStyle m_TimeStyle;

	private void Start()
	{
		StartCoroutine(Timecheck());
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		m_MsgIdx = (int)aobjValue[0];
		m_ETime = (long)aobjValue[1];
		m_TimeStyle = (Utile_Class.TimeStyle)aobjValue[2];
		SetMsg(GetTime());
	}

	IEnumerator Timecheck()
	{
		while (true)
		{
			double gaptime = GetTime();
			SetMsg(gaptime);
			if (gaptime < 0.1f) break;
			yield return new WaitForSeconds(1f - (float)(UTILE.Get_ServerTime() % 1D));
		}
	}

	void SetMsg(double time)
	{
		m_sUI.Msg.text = string.Format(TDATA.GetString(m_MsgIdx), UTILE.GetSecToTimeStr(m_TimeStyle, time));
	}

	double GetTime()
	{
		return Math.Max(0f, (m_ETime - UTILE.Get_ServerTime_Milli()) * 0.001d);
	}
}
