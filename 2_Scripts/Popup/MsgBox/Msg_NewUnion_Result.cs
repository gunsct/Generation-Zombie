using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_NewUnion_Result : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Image Mark;
		public Text Name;
		public TextMeshProUGUI Msg;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649

	public override void SetUI()
	{
		base.SetUI();
		m_sUI.Mark.sprite = USERINFO.m_Guild.GetGuilMark();
		m_sUI.Name.text = USERINFO.m_Guild.Name;
		m_sUI.Msg.text = TDATA.GetString(6032);
	}
}
