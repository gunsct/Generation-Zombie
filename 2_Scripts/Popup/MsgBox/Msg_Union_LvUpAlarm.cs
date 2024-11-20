using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Msg_Union_LvUpAlarm : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI[] LV;
		public TextMeshProUGUI Msg;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649

	public override void SetUI()
	{
		base.SetUI();
		int LV;
		long Exp;
		USERINFO.m_Guild.Calc_Exp(out LV, out Exp);
		for (int i = 0; i < m_sUI.LV.Length; i++) m_sUI.LV[i].text = LV.ToString();
		m_sUI.Msg.text = string.Format(TDATA.GetString(6178), LV);

		PlayEffSound(SND_IDX.SFX_0171);
	}
}
