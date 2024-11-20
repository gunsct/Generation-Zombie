using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_Store_2XActive : MsgBoxBase
{
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Time;
	}
	[SerializeField] SUI m_sUI;
	long Etime;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		Etime = (long)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		base.SetUI();
		TimeUI();
	}

	private void Update()
	{
		TimeUI();
	}

	void TimeUI()
	{
		long time = (long)((Etime - UTILE.Get_ServerTime_Milli()) * 0.001f);
		m_sUI.Time.text = TimeSpan.FromSeconds(time).ToString(TDATA.GetString(5028));
	}
}
