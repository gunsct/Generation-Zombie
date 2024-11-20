using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_CDN : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Msg;
	}

	[SerializeField] SUI m_sUI;
	long m_Datasize;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		m_Datasize = (long)aobjValue[0];
	}


	public override void SetMsg(string Title, string Msg)
	{
		base.SetMsg(Title, Msg);

		m_sUI.Msg.text = string.Format(TDATA.GetString(308), UTILE.Get_FileSize(m_Datasize));
	}
}
