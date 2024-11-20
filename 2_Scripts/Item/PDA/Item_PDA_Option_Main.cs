using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using hive;

[System.Serializable] public class DicLoginBtnData : SerializableDictionary<ACC_STATE, LoginBtnData> { }
[System.Serializable] public class DicLangeuageBtn : SerializableDictionary<LanguageCode, Image> { }
[System.Serializable]
public class LoginBtnData
{
	// 1f / tiling value
	public GameObject Active;
	public TextMeshProUGUI Label;
	public string LoginText;
}

public class Item_PDA_Option_Main : Item_PDA_Base
{
	public enum Menu
	{
		Default = 0,
		Auth,
		ETC,
		End
	}
	public enum EAgreeToggle
	{
		Terms_of_Service = 0,
		Privacy_Policy,
		Refundable_In_App,
		Alarm_Night_Recieve
	}
	[Serializable]
	public struct SUI {
		public GameObject[] MenuBtns;
		public GameObject[] MenuGroup;
		public GameObject AuthAlarm;
		public Animator[] Sound;
		public Animator PushAlarm;
		public Animator NightPushAlarm;
		public GameObject NightPush;
		public GameObject AuthGuest;
		public GameObject LogoutBtn;
		public GameObject AuthPanel;
		public DicLoginBtnData AuthBtns;
		public TextMeshProUGUI[] TermLabels;
		public DicLangeuageBtn Lang;
		public Text CrntLang;
		public Material[] LangBtnMats;
		public TextMeshProUGUI[] CashTxts;
		public Animator FingerPrint;
		public GameObject[] SNSBtns;

		public TextMeshProUGUI CSCodeID;


		public GameObject[] MenuPrefabs;
	}
	[SerializeField] SUI m_SUI;
	Menu m_NowMenu = Menu.End;
	LanguageCode m_PreLang;

	private void Awake() {
		m_SUI.MenuGroup[(int)Menu.Default].SetActive(true);
		m_SUI.MenuGroup[(int)Menu.Auth].SetActive(false);
		m_SUI.MenuGroup[(int)Menu.ETC].SetActive(false);
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		Menu menu = m_NowMenu;
		if (args != null && args.Length > 0) menu = (Menu)args[0];

		RefreshAuthLink();

		m_PreLang = APPINFO.m_Language;
		for(LanguageCode i = LanguageCode.KO; i < LanguageCode.MAX; i++)
		{
			if (!m_SUI.Lang.ContainsKey(i)) continue;

			if (!Utile_Class.IsUseLanguage(i))
			{
				m_SUI.Lang[i].gameObject.SetActive(false);
				continue;
			}
			else m_SUI.Lang[i].gameObject.SetActive(true);
			if (i == m_PreLang)
			{
				m_SUI.Lang[i].material = m_SUI.LangBtnMats[1];
				m_SUI.CrntLang.text = m_SUI.Lang[i].transform.GetChild(0).GetComponent<Text>().text;
			}
			else
			{
				m_SUI.Lang[i].material = m_SUI.LangBtnMats[0];
			}
		}

		m_SUI.CSCodeID.text = $"ID : {HIVE.GetPlayerID()}";

		m_SUI.CashTxts[0].text = USERINFO.m_Cash.ToString();//TODO:유료 금니 받아오기
		m_SUI.CashTxts[1].text = USERINFO.m_Cash.ToString();//TODO:무료 금니 받아오기

		m_SUI.AuthAlarm.SetActive(PlayerPrefs.GetInt($"OptionAuthAlarm_{USERINFO.m_UID}", 0) == 0);

		SetMenu((int)menu);
	}

	public void SetMenu(int _menu)
	{
		if (_menu == (int)Menu.End) _menu = (int)Menu.Default;
		if (m_NowMenu == (Menu)_menu) return;

		for(int i = 0; i < (int)Menu.End; i++)
		{
			if (i != _menu)
			{
				m_SUI.MenuBtns[i].GetComponent<Animator>().SetTrigger("Off");
				m_SUI.MenuGroup[i].SetActive(false);
			}
			else
			{
				m_SUI.MenuBtns[i].GetComponent<Animator>().SetTrigger("On");
				m_SUI.MenuGroup[i].SetActive(true);
			}
		}

		m_NowMenu = (Menu)_menu;

		switch ((Menu)_menu) {
			case Menu.Default:
				m_SUI.Sound[0].SetTrigger(Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0)) ? "Off" : "On");
				m_SUI.Sound[1].SetTrigger(Convert.ToBoolean(PlayerPrefs.GetInt("FXSND_MUTE", 0)) ? "Off" : "On");

				m_SUI.NightPush.SetActive(IS_UseNightPushCheck());
				m_SUI.PushAlarm.SetTrigger(HIVE.IsPush(HiveMng.PushMode.Normal) ? "On" : "Off");
				m_SUI.NightPushAlarm.SetTrigger(HIVE.IsPush(HiveMng.PushMode.Night) ? "On" : "Off");

				m_SUI.FingerPrint.SetTrigger(Convert.ToBoolean(PlayerPrefs.GetInt("FINGER_PRINT_ONOFF", 1)) ? "On" : "Off");

				//m_SUI.PushAlarm.SetTrigger(Convert.ToBoolean(PlayerPrefs.GetInt("PUSH_ONOFF", 0)) ? "On" : "Off");
				//m_SUI.NightPushAlarm.SetTrigger(Convert.ToBoolean(PlayerPrefs.GetInt("NIGHTPUSH_ONOFF", 0)) ? "On" : "Off");
				break;
			case Menu.Auth:
				PlayerPrefs.SetInt($"OptionAuthAlarm_{USERINFO.m_UID}", 1);
				PlayerPrefs.Save();
				m_SUI.AuthAlarm.SetActive(false);
				break;
			case Menu.ETC:
				m_SUI.SNSBtns[0].SetActive(!string.IsNullOrEmpty(WEB.GetConfig(EServerConfig.Facebook_url)));
				m_SUI.SNSBtns[1].SetActive(!string.IsNullOrEmpty(WEB.GetConfig(EServerConfig.Discode_url)));
				m_SUI.SNSBtns[2].SetActive(!string.IsNullOrEmpty(WEB.GetConfig(EServerConfig.Twitter_url)));
				break;
		}
		PlayEffSound(SND_IDX.SFX_0121);
	}
	/// <summary> 배경음 온오프 </summary>
	public void SetBGSound() {
		bool mute = !Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0));
		m_SUI.Sound[0].SetTrigger(mute ? "Off" : "On");
		PlayerPrefs.SetInt("BGSND_MUTE", Convert.ToInt32(mute));
		PlayerPrefs.Save();
		SND.SetBGMute(mute);
		PlayEffSound(SND_IDX.SFX_0121);
	}
	/// <summary> 효과음 온오프 </summary>
	public void SetFXSound() {
		bool mute = !Convert.ToBoolean(PlayerPrefs.GetInt("FXSND_MUTE", 0));
		m_SUI.Sound[1].SetTrigger(mute ? "Off" : "On");
		PlayerPrefs.SetInt("FXSND_MUTE", Convert.ToInt32(mute));
		PlayerPrefs.Save();
		PlayEffSound(SND_IDX.SFX_0121);
	}
	/// <summary> 푸시 알림 동의 </summary>
	public void SetPush() {
		bool on = !HIVE.IsPush(HiveMng.PushMode.Normal);// !Convert.ToBoolean(PlayerPrefs.GetInt("PUSH_ONOFF", 0));
		m_SUI.PushAlarm.SetTrigger(on ? "On" : "Off");
		HIVE.SetPush(HiveMng.PushMode.Normal, on);
		//MAIN.FIREBASE.SetNormalPush(on);
		PlayEffSound(SND_IDX.SFX_0121);
	}
	/// <summary> 야간 푸시 알림 동의 </summary>
	public void SetNightPush() {
		bool on = !HIVE.IsPush(HiveMng.PushMode.Night); //!Convert.ToBoolean(PlayerPrefs.GetInt("NIGHTPUSH_ONOFF", 0));
		m_SUI.NightPushAlarm.SetTrigger(on ? "On" : "Off");
		HIVE.SetPush(HiveMng.PushMode.Night, on);
		//MAIN.FIREBASE.SetNightPush(on);
		PlayEffSound(SND_IDX.SFX_0121);
	}
	bool IS_UseNightPushCheck() {
		switch (USERINFO.m_Nation) {
			case "KR": return true;
			default:return false;
		}
	}
	/// <summary> 계정 연동 로그아웃하면 타이틀로가서 다시 연동해야하기 떄문에 필요 없어 </summary>
	public void SetAuthLink(int _type)
	{
		ACC_STATE logintype = (ACC_STATE)_type;
		if (logintype == ACC.LoginType) return;
#if NOT_USE_NET
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, "Coming Soon");
#else
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);

		HIVE.AccConnect(logintype, (result, info) =>
		{
			switch (result.code)
			{
			case ResultAPI.Code.Success:
				// 연동 성공
				RefreshAuthLink();
				break;
			case ResultAPI.Code.AuthV4ConflictPlayer:
				// 탈퇴 취소 프로토콜 연결
				LoadACCInfo(logintype, info, false);
				break;
			default:
				// 기타 예외 상황
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, result.message);
				break;
			}
		});

		//MAIN.Login(logintype, (result) =>
		//{
		//	if(result.isSuccess())
		//	{
		//		IFAccount ifacc = ACC.GetIFAcc(logintype);
		//		string ID = ifacc.GetID();
		//		WEB.SEND_REQ_ACC_INFO((res) =>
		//		{
		//			if (!res.IsSuccess())
		//			{
		//				ifacc.Logout();
		//				WEB.StartErrorMsg(res.result_code);
		//				return;
		//			}

		//			if (res.UserNo > 0) m_CloaseCB?.Invoke(Item_PDA_Option.State.Select_Acc, new object[] { new RES_ACC_INFO[] { res, USERINFO.GetACCINFO() }, logintype });
		//			else USERINFO.ACC_CHANGE(logintype, ID, () => { RefreshAuthLink(); });
		//		}, logintype, ID);
		//	}
		//});
#endif
	}

	void LoadACCInfo(ACC_STATE logintype, AuthV4.PlayerInfo info, bool IsCancelDelete = false)
	{
		POPUP.SetConnecting(false);
		WEB.SEND_REQ_ACC_INFO((res) =>
		{
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_DELETE_USER:
					// 계정 충돌
					POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, string.Empty, TDATA.GetString(1066), (result, obj) => {
						if (result == 1)
						{
							LoadACCInfo(logintype, info, true);
						}
					}, TDATA.GetString(11), TDATA.GetString(10));
					break;
				default:
					WEB.StartErrorMsg(res.result_code);
					break;
				}
				return;
			}

			if (res.UserNo > 0)
			{
				res.HivePlayerInfo = info;
				var myAccInfo = USERINFO.GetACCINFO();
				myAccInfo.HivePlayerInfo = HIVE.GetPlayerInfo();
				m_CloaseCB?.Invoke(Item_PDA_Option.State.Select_Acc, new object[] { new RES_ACC_INFO[] { res, myAccInfo }, logintype });
			}
			// 서버쪽에 생성이 안되어 있는 상태(자동으로 게스트 계정 선택)
			else
			{
				POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
				HIVE.SelectAccConnect(HIVE.GetPlayerID(), (result2, isReset) =>
				{
					POPUP.SetConnecting(false);
					if (result2.isSuccess()) RefreshAuthLink();
					else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, result2.message);
				});
			}
		}, ACC_STATE.HIVE, info.playerId.ToString(), IsCancelDelete);
	}

	void RefreshAuthLink() {
		var logintype = ACC.LoginType;
		m_SUI.AuthGuest.SetActive(logintype == ACC_STATE.Guest);
		// Hive : 게스트 로그인 상태에서 로그아웃 시 동일한 PlayerID를 더 이상 찾을 수 없으므로 게스트 상태에서는 로그아웃을 제공하지 않도록 구현해주세요.
		m_SUI.LogoutBtn.SetActive(logintype != ACC_STATE.Guest);
		int cnt = 0;
		if(logintype == ACC_STATE.Guest) 
		{
			for (ACC_STATE i = ACC_STATE.Guest; i < ACC_STATE.END; i++)
			{
				if (!m_SUI.AuthBtns.ContainsKey(i)) continue;
				bool active = ACC.IsSupport(i);
				m_SUI.AuthBtns[i].Active.SetActive(active);
				m_SUI.AuthBtns[i].Label.text = i == logintype ? TDATA.GetString(498) : m_SUI.AuthBtns[i].LoginText;
				if (active) cnt++;

			}
		}
		else
		{
			for (ACC_STATE i = ACC_STATE.Guest; i < ACC_STATE.END; i++)
			{
				if (!m_SUI.AuthBtns.ContainsKey(i)) continue;
				bool active = ACC.IsSupport(i) && i == logintype;
				m_SUI.AuthBtns[i].Active.SetActive(active);
				m_SUI.AuthBtns[i].Label.text = active ? TDATA.GetString(498) : m_SUI.AuthBtns[i].LoginText;
				if (active) cnt++;
			}
		}

		m_SUI.AuthPanel.SetActive(cnt > 0);

		//if(logintype != ACC_STATE.Guest && m_SUI.AuthBtns.ContainsKey(logintype))
		//{
		//	m_SUI.AuthBtns[logintype].Active.transform.SetSiblingIndex(1);
		//}
	}
	/// <summary> 약관 보여주기 </summary>
	public void ViewTerms(int nPos) {
		//		EAgreeToggle agree = (EAgreeToggle)nPos;
		//		string url = "";
		//#if NOT_USE_NET
		//		switch (agree)
		//		{
		//		case EAgreeToggle.Privacy_Policy:
		//			url = "http://59.13.192.250:11000/Files/PFA/Privacy_Policy_{0}.txt";
		//			break;
		//		case EAgreeToggle.Refundable_In_App:
		//			url = "http://59.13.192.250:11000/Files/PFA/Offer_{0}.txt";
		//			break;
		//		default:
		//			url = "http://59.13.192.250:11000/Files/PFA/Offer_{0}.txt";
		//			break;
		//		}
		//#else
		//		switch (agree)
		//		{
		//		case EAgreeToggle.Privacy_Policy:
		//			url = WEB.GetConfig(EServerConfig.Privacy_Policy_url);
		//			break;
		//		case EAgreeToggle.Refundable_In_App:
		//			url = WEB.GetConfig(EServerConfig.Offer_url);
		//			break;
		//		case EAgreeToggle.Terms_of_Service:
		//			url = WEB.GetConfig(EServerConfig.Terms_of_Service_url);
		//			break;
		//		}
		//#endif

		//		url = string.Format(url, APPINFO.m_LanguageCode);

		//		POPUP.Set_WebView(m_SUI.TermLabels[nPos].text, url);
		HIVE.ShowTerms(() => { } );
		PlayEffSound(SND_IDX.SFX_0121);
	}
	/// <summary> 계정 탈퇴</summary>
	public void DeleteAccount()
	{
#if UNITY_IOS || UNITY_IPHONE
		int Msg = 1062;
#else
		int Msg = 1063;
#endif
		POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, string.Empty, TDATA.GetString(Msg), (result, obj) => {
			if (result == 1)
			{
				// 탈퇴 프로토콜 연결
				WEB.SEND_REQ_ACC_DELETE((res) =>
				{
					// 성공
					POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(1064), (result, obj) => {
						SignOut(false);
					});
				});
			}
			else
			{
				POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(1065), (result, obj) => {});
			}
		}, TDATA.GetString(11), TDATA.GetString(1068));
		PlayEffSound(SND_IDX.SFX_0121);
	}

	/// <summary> 로그 아웃</summary>
	public void LogOut()
	{
		POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, TDATA.GetString(512), (result, obj) => {
			if (result == 1)
			{
				SignOut(true);
			}
		});
		PlayEffSound(SND_IDX.SFX_0121);
	}

	void SignOut(bool GuestCheck)
	{
		ACC.Logout((result) => {
			//ACC.LoginType = ACC_STATE.NONE;
			PlayerPrefs.SetInt("ACC_STATE", (int)ACC_STATE.NONE);
#if AUTO_INIT
					PlayerPrefs.SetInt("AUTO_INIT", 1);
#endif
			PlayerPrefs.Save();
			MAIN.ReStart();
		}, ACC_STATE.NONE, GuestCheck);
	}
	/// <summary> 탈퇴 </summary>
	public void Secession() {

	}

	/// <summary> 언어 선택 </summary>
	public void SetLanguage(string code)
	{
		LanguageCode lang = Utile_Class.Get_LanguageCode(code);
		if (m_PreLang != lang)
		{
			POPUP.Set_MsgBox(PopupName.Msg_YN, string.Empty, TDATA.GetString(511), (result, obj) => {
				if (result == 1)
				{
					m_SUI.Lang[m_PreLang].material = m_SUI.LangBtnMats[0];
					m_SUI.Lang[lang].material = m_SUI.LangBtnMats[1];
					m_SUI.CrntLang.text = m_SUI.Lang[lang].transform.GetChild(0).GetComponent<Text>().text;
					APPINFO.m_Language = lang;
					HIVE.SetLanguage(APPINFO.m_Language);
					TDATA.LoadString();
					HIVE.AutoCheck_LocalPush();
					MAIN.StateChange(MainState.TITLE);
				}
			});
		}
		PlayEffSound(SND_IDX.SFX_0121);
	}

	/// <summary> 제작진 보여주기 </summary>
	public void ViewCredit() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Credit, null);
		PlayEffSound(SND_IDX.SFX_0121);
	}
	/// <summary> 유료 재화 정보 </summary>
	public void ViewCashInfo() {

	}
	/// <summary> 
	/// 회사 SNS 접속
	/// 0:문의하기, 1:facebook, 2:discord, 3:x(twitter)
	/// </summary>
	public void ViewSNS(int _pos) {
		string url = string.Empty;
		//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Announcement, null, WEB.GetConfig(EServerConfig.Notice_URL));
		switch (_pos) {
			case 0: HIVE.ShowInquiry(); break;
			case 1: 
				url = string.Format(WEB.GetConfig(EServerConfig.Facebook_url), APPINFO.m_LanguageCode);
				//POPUP.Set_WebView(string.Empty, url);
				break;
			case 2:
				url = string.Format(WEB.GetConfig(EServerConfig.Discode_url), APPINFO.m_LanguageCode);
				//POPUP.Set_WebView(string.Empty, url);
				break;
			case 3:
				url = string.Format(WEB.GetConfig(EServerConfig.Twitter_url), APPINFO.m_LanguageCode);
				//POPUP.Set_WebView(string.Empty, url);
				break;

		}
		if(!string.IsNullOrEmpty(url))
		{
			UTILE.OpenURL(url);
			PlayEffSound(SND_IDX.SFX_0121);
		}
	}
	/// <summary> 지문 사용 여부 </summary>
	public void SetFingerPrint() {
		bool on = !Convert.ToBoolean(PlayerPrefs.GetInt("FINGER_PRINT_ONOFF", 1));
		m_SUI.FingerPrint.SetTrigger(on ? "On" : "Off");
		PlayerPrefs.SetInt("FINGER_PRINT_ONOFF", Convert.ToInt32(on));
		PlayerPrefs.Save();
		PlayEffSound(SND_IDX.SFX_0121);
	}

	public void OnCopyCSCodeID()
	{
		Utile_Class.Copy_Clipboard(HIVE.GetPlayerID().ToString());
	}

	public override void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Cloase, 1)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		m_CloaseCB?.Invoke(Item_PDA_Option.State.End, null);
	}
}
