using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_Union_Research_Finish : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Msg;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PlayEffSound(SND_IDX.SFX_1522);
		var tdata = TDATA.GetGuildRes(USERINFO.m_Guild.EndRes[USERINFO.m_Guild.EndRes.Count - 1]);
		m_sUI.Msg.text = tdata.GetDesc();
		USERINFO.m_Guild.Set_Alram_Res_End_Off();
		base.SetData(pos, popup, cb, aobjValue);
		PlayEffSound(SND_IDX.SFX_0171);
	}
}
