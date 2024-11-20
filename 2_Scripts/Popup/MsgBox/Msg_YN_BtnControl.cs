using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Item_PDA_Collection_Main;

[System.Serializable] public class DicMsg_YN_BtnControlBtn : SerializableDictionary<EMsgBtn, Msg_YN_BtnControl.SBtnUI> { }

public class Msg_YN_BtnControl : MsgBoxBase
{
	public class BtnInfo
	{
		public EMsgBtn Btn;
		public UIMng.BtnBG BG;
		public string Label;
	}

#pragma warning disable 0649
	[System.Serializable]
	public struct SBtnUI
	{
		public Item_Button Btn;
		public TextMeshProUGUI Label;
	}

	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Msg;

		public DicMsg_YN_BtnControlBtn Btn;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		for(int i = 0; i < 2; i++)
		{
			var info = (BtnInfo)aobjValue[i];
			m_sUI.Btn[info.Btn].Btn.SetBG(info.BG);
			m_sUI.Btn[info.Btn].Label.text = info.Label;
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
