using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
#if UNITY_ANDROID
	using UnityEngine.Android;
#endif
using UnityEngine.UI;
using static LS_Web;

public class Main_Title : PopupBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Anim;
		public GameObject Touch;
		public TextMeshProUGUI Ver;
		public TextMeshProUGUI CopyLight;
	}

	[SerializeField] SUI m_sUI;

	EIntroState m_State;
	bool IsAni = false;
#pragma warning restore 0649
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		Init();
	}

	public void Init()
	{
		//if (MAIN.IS_BackState(MainState.START))
		//{
		//	POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.COMPnPDSignature, (result, obj) =>
		//	{
		//		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.EarPhone, (result, obj) =>
		//		{
		//			StateChange(EIntroState.None);
		//			StartCoroutine(AniPlay("Start"));
		//			//퍼미션체크
		//			//#if UNITY_ANDROID
		//			//					StartCoroutine(PermissionCheckCoroutine());
		//			//#else
		//			//					StateChange(EIntroState.None);
		//			//					StartCoroutine(AniPlay("Start"));
		//			//#endif
		//		});
		//	});
		//}
		//else
		//{
			StateChange(EIntroState.None);
			StartCoroutine(AniPlay("Start"));
		//}
		m_sUI.Ver.text = APPINFO.m_strVersion;
		m_sUI.CopyLight.text = TDATA.GetString(852);
	}

	void StartAni()
	{

	}

	IEnumerator AniPlay(string aniname)
	{
		IsAni = true;
		m_sUI.Anim.SetTrigger(aniname);
		yield return new WaitForEndOfFrame();
		// 스킵 체크
		while (Utile_Class.IsAniPlay(m_sUI.Anim, 350f / 599f))
		{
#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IPHONE)
			if(Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Ended)
#else
			if (Input.GetMouseButtonUp(0))
#endif
			{
				Utile_Class.AniSkip(m_sUI.Anim, 350f);
				break;
				//// 다음 터치 임시 막기(연속으로 넘어가는거 막기위해서)
				//yield return new WaitForSeconds(0.3f);
			}
			yield return null;
		}

		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_sUI.Anim, 400f/599f));

		yield return new WaitWhile(() => HIVE == null);
		bool IsEnd = false;


		//if (MAIN.IS_BackState(MainState.START))

		MAIN.Is_LoadServerConfig = false;
		MAIN.Load_ServerConfig((result) =>
		{
			if (WEB.CheckServer()) MAIN.Load_ServerConfig((result) => {
				HIVE?.SetServer();
				MAIN.Is_LoadServerConfig = true;
			});
			else
			{
				HIVE?.SetServer();
				MAIN.Is_LoadServerConfig = true;
			}
		});

		yield return new WaitUntil(() => MAIN.Is_LoadServerConfig);

		IsEnd = false;
		HIVE.Init(() => {
			IsEnd = true;
		});

		yield return new WaitUntil(() => IsEnd);


		IsEnd = false;
		HIVE.ServerCheck(() =>
		{
			IsEnd = true;
		});
		yield return new WaitUntil(() => IsEnd);

		StateChange(EIntroState.TouchToStart);

		MAIN.StartSystemMsg();
		IsAni = false;
	}

	enum EIntroState
	{
		None = -1,
		/// <summary> 추가다운 체크 </summary>
		CheckCDN = 0,
		/// <summary> 터치 시작 화면 </summary>
		TouchToStart,
		/// <summary> 이용약관 </summary>
		Agree,
		/// <summary> 로그인 </summary>
		Login,
		/// <summary> 플레이 </summary>
		GoPlay,
	}

	void StateChange(EIntroState state, Action EndCB = null) {
		OnTouchActive(state == EIntroState.TouchToStart);
		POPUP.Init_PopupUI();
		switch (state)
		{
		case EIntroState.CheckCDN:
			CDNCheck();
			break;
		case EIntroState.Agree:
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Agree, (result, obj) =>
			{
				// 로그인 화면으로 넘기기
				if (EndCB != null) EndCB?.Invoke();
				else StateChange(EIntroState.Login);
			});
			break;
		case EIntroState.Login:
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Login, (result, obj) =>
			{
				if (EndCB != null) EndCB?.Invoke();
				else if (result == 1) StateChange(EIntroState.GoPlay);
				else StateChange(EIntroState.TouchToStart);
			});
			break;
		case EIntroState.GoPlay:
			// 파일 로딩후 플레이로 넘김
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DataLoad, (result, obj) => {
				if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.DelayStartEnd))
					TUTO.Next();
				else
					MAIN.StateChange(MainState.PLAY);
			});
			break;
		}
		m_State = state;
	}

	void CheckAutoLogin()
	{
		HIVE.AutoLogin(() =>
		{
			if (HIVE.GetPlayerID() == 0)
			{
				//StateChange(EIntroState.Agree);
				StateChange(EIntroState.Login);
			}
			else EndLogin();
		});
		//#if NOT_USE_NET && SELECT_LANGUAGE
		//		StateChange(EIntroState.Login);
		//#else
		//		if (MAIN.ACC.LoginType == ACC_STATE.NONE) {
		//			// 이용약관 페이지
		//			StateChange(EIntroState.Agree);
		//		}
		//		else {
		//			Login(MAIN.ACC.LoginType);
		//		}
		//#endif
	}

	public void Login(ACC_STATE acc)
	{
		// 연속 호출 막기
		MAIN.Login(acc, (result) => {
			if(result.isSuccess())
			{
				EndLogin();
			}
			else
			{
				StateChange(EIntroState.Login);
			}
		});
	}

	public void EndLogin(bool IsCancelDelete = false)
	{
		MAIN.Auth((result) =>
		{
			switch (result)
			{
			case EResultCode.SUCCESS:
			case EResultCode.SUCCESS_NEW_AUTH:
				StateChange(EIntroState.GoPlay);
				return;
			case EResultCode.ERROR_AGREE:
				StateChange(EIntroState.Agree, () =>
				{
					Login(MAIN.ACC.LoginType);
				});
				return;
			case EResultCode.ERROR_DELETE_USER:
				POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, string.Empty, TDATA.GetString(1066), (result, obj) => {
					if (result == 1)
					{
						// 탈퇴 취소 프로토콜 연결
						EndLogin(true);
					}
					else
					{
						// 이미 로그인을 진행후 들어온것이기때문에 다시 로그인 버튼이 동작 하도록 로그아웃 해준다.
						// 게스트는 여기로 들어올 수 없음
						HIVE.Logout((result) => { StateChange(EIntroState.Login); });
					}
				}, TDATA.GetString(11), TDATA.GetString(10));
				return;
			default:
				// 자동로그인 취소
				//MAIN.ACC.LoginType = ACC_STATE.NONE;
				break;
			}
			if (m_State >= EIntroState.Login) StateChange(EIntroState.Login);
		}, IsCancelDelete);
	}

	public void OnClick() {
		if (IsAni) return;
		StateChange(EIntroState.CheckCDN);
	}

	void OnTouchActive(bool Active) {
		m_sUI.Touch.SetActive(Active);
	}

	void CDNCheck()
	{
#if NOT_USE_NET || (UNITY_EDITOR && !DATA_PATH_CDN)
		CheckAutoLogin();
#else
		string url = WEB.GetConfig(EServerConfig.CDN_url);
		if (string.IsNullOrEmpty(url))
		{
			CheckAutoLogin();
			return;
		}

		WEB.CDN_Check((result, datas) =>
		{
			if(result == CDN_RESULT.COMPLATE && datas.Count > 0)
			{
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Down_CDN, (result, obj) => {
					// result > 0 일부 받지못한 파일이 있음
					// result < 0 취소함
					if (result != 0) StateChange(EIntroState.TouchToStart);
					else CheckAutoLogin();
				}, datas);
				return;
			}
			CheckAutoLogin();
			return;
		});
#endif
	}

//	IEnumerator PermissionCheckCoroutine() {
//		bool IsPermission = true;

//		yield return new WaitForEndOfFrame();
//		if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false) {
//			Permission.RequestUserPermission(Permission.ExternalStorageWrite);

//			yield return new WaitForSeconds(0.2f); // 0.2초의 딜레이 후 focus를 체크하자.
//			yield return new WaitUntil(() => Application.isFocused == true);

//			if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite) == false) {
//				IsPermission = false;

//				POPUP.Set_MsgBox(PopupName.Msg_OK, "권한 필요", "외부저장소", (result, obj) => {
//					if (result == 0) {
//						OpenAppSetting(); // 원래는 다이얼로그 선택에서 Yes를 누르면 호출됨.
//						MAIN.Exit();
//					}
//				});
//			}
//		}

//		if(IsPermission) {
//			StateChange(EIntroState.None);
//			StartCoroutine(AniPlay("Start"));
//		}
//	}

//	private void OpenAppSetting() {
//		try {
//#if UNITY_ANDROID
//			using (var unityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
//			using (AndroidJavaObject currentActivityObject = unityClass.GetStatic<AndroidJavaObject>("currentActivity")) {
//				string packageName = currentActivityObject.Call<string>("getPackageName");

//				using (var uriClass = new AndroidJavaClass("android.net.Uri"))
//				using (AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null))
//				using (var intentObject = new AndroidJavaObject("android.content.Intent", "android.settings.APPLICATION_DETAILS_SETTINGS", uriObject)) {
//					intentObject.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
//					intentObject.Call<AndroidJavaObject>("setFlags", 0x10000000);
//					currentActivityObject.Call("startActivity", intentObject);
//				}
//			}
//#endif
//		}
//		catch (Exception ex) {
//			Debug.LogException(ex);
//		}
//	}
}
