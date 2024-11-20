using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_YN : MsgBoxBase
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

	public override void SetMsg(string Title, string Msg)
	{
		base.SetMsg(Title, Msg);

		if (m_sUI.Title) m_sUI.Title.text = Title;
		if (m_sUI.Msg) m_sUI.Msg.text = Msg;
	}
}
