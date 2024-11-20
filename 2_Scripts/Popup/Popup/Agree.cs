using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Agree : PopupBase
{
	[Serializable]
	public enum EAgreeToggle
	{
		Terms_of_Service = 0,
		Privacy_Policy,
		Refundable_In_App,
		Alarm_Night_Recieve
	}

#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Toggle[] Toggles;//0:TOS 1:PP 2: RIA 3: ANR
		public TextMeshProUGUI[] ToggleLabels;
	}

	[SerializeField] SUI m_sUI;
	bool m_IsAllNeed;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void OnToggleValueChange(int nPos)
	{
		m_IsAllNeed = true;
		for (int i = m_sUI.Toggles.Length - 1; i > -1; i--)
		{
			if (!m_sUI.Toggles[i].isOn && (EAgreeToggle)i < EAgreeToggle.Refundable_In_App)
			{
				m_IsAllNeed = false;
				break;
			}
		}
	}

	public void OnDetail(int nPos)
	{
		EAgreeToggle agree = (EAgreeToggle)nPos;
		string url = "";
#if NOT_USE_NET
		switch (agree)
		{
		case EAgreeToggle.Privacy_Policy:
			url = "http://59.13.192.250:11000/Files/PFA/Privacy_Policy_{0}.txt";
			break;
		case EAgreeToggle.Refundable_In_App:
			url = "http://59.13.192.250:11000/Files/PFA/Offer_{0}.txt";
			break;
		default:
			url = "http://59.13.192.250:11000/Files/PFA/Offer_{0}.txt";
			break;
		}
#else
		switch (agree)
		{
		case EAgreeToggle.Privacy_Policy:
			url = WEB.GetConfig(EServerConfig.Privacy_Policy_url);
			break;
		case EAgreeToggle.Refundable_In_App:
			url = WEB.GetConfig(EServerConfig.Offer_url);
			break;
		case EAgreeToggle.Terms_of_Service:
			url = WEB.GetConfig(EServerConfig.Terms_of_Service_url);
			break;
		}
#endif

		url = string.Format(url, APPINFO.m_LanguageCode);

		POPUP.Set_WebView(m_sUI.ToggleLabels[nPos].text, url);
	}

	public void OnAgree(bool _all)
	{
		if (!_all && !m_IsAllNeed) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(481));
			return;
		}

		//if (m_sUI.Toggles[(int)EAgreeToggle.Alarm_Night_Recieve].isOn || _all) {
		//	//MAIN.FIREBASE.SetNightPush(true);
		//}
		////MAIN.FIREBASE.SetNormalPush(true);
		HIVE.SetPush(HiveMng.PushMode.Night, m_sUI.Toggles[(int)EAgreeToggle.Alarm_Night_Recieve].isOn || _all);
		HIVE.SetPush(HiveMng.PushMode.Normal, true);
		APPINFO.m_Agree = true;
		Close(1);
	}
}
