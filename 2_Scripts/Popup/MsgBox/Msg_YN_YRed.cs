using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_YN_YRed : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Msg;

		public TextMeshProUGUI[] BtnLabel;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		for (int i = 0; i < m_sUI.BtnLabel.Length; i++)
		{
			if (aobjValue.Length > i) m_sUI.BtnLabel[i].text = (string)aobjValue[i];
		}
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetMsg(string Title, string Msg)
	{
		base.SetMsg(Title, Msg);

		if (m_sUI.Title) m_sUI.Title.text = Title;
		if (m_sUI.Msg) m_sUI.Msg.text = Msg;
	}
}
