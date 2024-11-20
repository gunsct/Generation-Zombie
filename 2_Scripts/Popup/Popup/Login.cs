using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static hive.AuthV4;

public class Login : PopupBase
{
	[Serializable]
	public enum EBtns
	{
		Facebook = 0,
		Google,
		Apple,
		Guest
	}

#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public GameObject Guest;
		public GameObject Google;
		public GameObject Apple;
		public GameObject Facebook;
	}

	[SerializeField] SUI m_sUI;
	[SerializeField] GameObject m_LoginBtnsPanel;
	[SerializeField] GameObject m_LanguageBtnsPanel;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
#if NOT_USE_NET && SELECT_LANGUAGE
		m_LoginBtnsPanel.SetActive(false);
		m_LanguageBtnsPanel.SetActive(true);
#else
		m_LoginBtnsPanel.SetActive(true);
		m_LanguageBtnsPanel.SetActive(false);

		m_sUI.Google.SetActive(ACC.IsSupport(ACC_STATE.Google));
		m_sUI.Apple.SetActive(ACC.IsSupport(ACC_STATE.Apple));
		m_sUI.Facebook.SetActive(ACC.IsSupport(ACC_STATE.FaceBook));
		m_sUI.Guest.SetActive(ACC.IsSupport(ACC_STATE.Guest));
#endif
	}

	public void OnClick(int pos)
	{
		((Main_Title)POPUP.GetMainUI()).Login((ACC_STATE)pos);
	}

	public void LanguageChange(int code)
	{
		APPINFO.m_Language = (LanguageCode)code;
		TDATA.LoadString();
		Close(1);
	}
}
