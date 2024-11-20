using hive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static hive.AuthV4;

public partial class HiveMng : ClassMng
{
	// Hive 인증키
	String hiveCertKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJIaXZlIiwiaWF0IjoxNjkwODY3NjMxLCJqdGkiOiIxNjgyMzQyOTMzIn0.KDuUJOGJ0lsHVOnnJ9XFiYO2rKOyoaTjvp-O7qsG45M";
#pragma warning disable 0414
#if UNITY_ANDROID
	string Hive_Appid = "com.GVI.GenZombie.android.google.global.normal";
#else
	string Hive_Appid = "com.GVI.GenZombie.iOS.google.global.normal";
#endif
#pragma warning restore 0414
	public bool isAutoSignIn = false;
	public List<ProviderType> LoginType;
	public string DID;

	Action<string> _TokenCB;
	Action<string> _MsgCB;
	List<int> _NightTime = new List<int>();
	bool Fix;

	public HiveMng()
	{
		Utile_Class.DebugLog($"HiveMng Start !!!");
		//하이브 플러그인을 위한 게임오브젝트를 생성한다.
		HIVEUnityPlugin.InitPlugin();
#if USE_LOG_MANAGER
		Configuration.setUseLog(true);
#else
		Configuration.setUseLog(false);
#endif
		Configuration.setHiveCertificationKey(hiveCertKey);
		// 하이브 테마
		Configuration.setHiveTheme(HiveThemeType.hiveLight);
	}

	public bool IsSandBox()
	{
//#if UNITY_IOS || UNITY_IPHONE
		var data = WEB.GetConfig(EServerConfig.client_ver);
		int code = 0;
		if (!int.TryParse(data, out code)) return false;
		if (APPINFO.CLIENT_VERSION_CODE > code) return true;
//#endif
		return false;
	}

	public void SetServer()
	{
		ZoneType _Zone = ZoneType.REAL;
		string SERVERID = "LIVE";
		var server = WEB.Get_Server();
		// 설정 xml 파일에 zone 필드가 sandbox 로 설정되어 있어도 real 로 설정된 것과 동일한 동작을 수행한다.
		if (server != LS_Web.ServerMode.LIVE_SERVER) _Zone = ZoneType.SANDBOX;
		switch(server)
		{
		case LS_Web.ServerMode.DEV_SERVER: SERVERID = "DEV"; break;
		case LS_Web.ServerMode.TEST_SERVER: SERVERID = "TEST"; break;
		}

		Configuration.setZone(_Zone);
		Configuration.setServerId(SERVERID);
		//Configuration.updateServerId(SERVERID);
		Utile_Class.DebugLog($"SetServer zone : {_Zone}\nsetServerId : {SERVERID}");
		SetLanguage(APPINFO.m_Language, true);
	}

	public void Init(Action CB)
	{
		Utile_Class.DebugLog($"HiveMng Init !!!");

		m_PlayerAccType = (ProviderType)PlayerPrefs.GetInt("HIVE_ACC_STATE", (int)ProviderType.AUTO);

		SetServer();
		SetShowActivePlayPush(false, false);

		////유저 확인 퍼미션 없음
		//PermissionViewData data = Configuration.getPermissionViewData(HIVELanguage.HIVELanguageKO);

		//for (int i = 0; i < data.permissions.Count; i++)
		//{
		//	/* TODO :
		//	   권한명 : data.permissions[i].nativePermissionName
		//	   타이틀 : data.permissions[i].title
		//	   내용 : data.permissions[i].contents
		//	   카테고리 : data.permissions[i].permissionCategory
		//	 */
		//	Utile_Class.DebugLog($"Permission Name : {data.permissions[i].nativePermissionName}, title : {data.permissions[i].title}, contents : {data.permissions[i].contents}, Category : {data.permissions[i].permissionCategory}");
		//}
		//for (int i = 0; i < data.commons.Count; i++)
		//{
		//	/* TODO : data.permissions(권한 고지 내용)가 한 개 이상일 때만 존재 합니다.
		//		권한명 : data.commons[i].nativePermissionName
		//		타이틀 : data.commons[i].title
		//		내용 : data.commons[i].contents
		//		카테고리 : data.commons[i].permissionCategory
		//	 */
		//	Utile_Class.DebugLog($"commons Name : {data.permissions[i].nativePermissionName}, title : {data.permissions[i].title}, contents : {data.permissions[i].contents}, Category : {data.permissions[i].permissionCategory}");
		//}

		// isAutoSignIn : 자동 로그인 가능 여부 (ex. true)
		// did : 앱 설치 시 생성되는 앱 식별자로 동일한 종류의 앱을 식별할 때 사용합니다.  (ex. 123456789)
		// providerTypeList : 현재 앱에서 제공 가능한 IdP 인증 목록 명시적 로그인을 커스터마이징하거나 IdP 연동 상태 정보를 구성할 때 필수적으로 사용해야 하는 필드입니다. (ex. ProviderType.FACEBOOK, ProviderType.HIVE)
		// Hive SDK 초기화 요청
		AuthV4.setup((ResultAPI result, Boolean isAutoSignIn, String did, List<ProviderType> providerTypeList) => {
			// ResultAPI { errorCode = SUCCESS, Code = Success, msg = Success. null }
			//isAutoSignIn: false
			//did: 5000117611
			//providerList:[GUEST, GOOGLE, FACEBOOK, SIGNIN_APPLE]
			Utile_Class.DebugLog($"setup result : {result.toString()}\nisAutoSignIn : {isAutoSignIn}\nLoginType : {LoginType}\nDID : {DID}");
			if (result.isSuccess())
			{
				// 초기화 성공. 자동 로그인 가능 여부에 따라 로그인을 처리하세요.
				this.isAutoSignIn = isAutoSignIn;
				LoginType = providerTypeList;
				DID = did;
			}
			else if (result.needExit())
			{
				// TODO: 앱 종료 기능을 구현하세요
				// 예) Application.Quit();
				MAIN.Exit();
				return;
			}
			else
			{
				// 초기화 실패
				Debug.LogError("setup result.isSuccess() false !!!!!!!!!");
			}


			SetServer();

			Push.requestPushPermission();
			//Ad_Init();
			Analytics_FirstOpen();
			CB?.Invoke();
			//GetNation();
		});
	}

	public void ServerCheck(Action CB)
	{
		// Hive SDK 서버 점검 팝업 띄우기
		AuthV4.checkMaintenance(true, (ResultAPI result, List<AuthV4MaintenanceInfo> maintenanceInfoList) => {
			Utile_Class.DebugLog($"ServerCheck result : {result.toString()}\nmaintenanceInfoList : {maintenanceInfoList}");
			if (result.isSuccess())
			{
				// 점검 데이터가 없는 경우
				// isShow가 false인 경우
			}
			else if (result.needExit())
			{
				// TODO: 앱 종료 기능을 구현하세요
				// 예) Application.Quit();   
				Debug.Log("ResultAPI.needExit(): " + result.needExit() + "\n");
				MAIN.Exit();
				return;
			}
			CB?.Invoke();
		});
	}

	public string GetHiveLanguageCode(string code)
	{
		switch (code)
		{
		case "zh-CN": return "zh-hans";
		case "zh-TW": return "zh-hant";
		}
		return code;
	}

	public void SetLanguage(LanguageCode code, bool IsInit = false)
	{
		if(IsInit) Configuration.setGameLanguage(GetHiveLanguageCode(Utile_Class.Get_LanguageCode(code)));
		else Configuration.updateGameLanguage(GetHiveLanguageCode(Utile_Class.Get_LanguageCode(code)));
	}

	IEnumerator CallRenderThread(Action CB = null)
	{
		yield return new WaitForEndOfFrame();
		CB?.Invoke();
	}

	/// <summary> Hive 서버에서 판단한 국가 코드 </summary>
	public string GetNation()
	{
		//Nation: KR
		//TimeZone: { "ip":"118.235.6.152","zone_id":0,"country_code":"KR","zone_name":"Asia\\/Seoul","country_name":"South Korea","abbreviation":"","gmt_offset":0,"dst":0}
		//ServerID: DEV
		Utile_Class.DebugLog($"Nation : {Configuration.getHiveCountry()}\nTimeZone : {Configuration.getHiveTimeZone()}\nServerID : {Configuration.getServerId()}");
		return Configuration.getHiveCountry();
	}

	/// <summary> 문의 하기 </summary>
	/// <param name="CB"></param>
	public void ShowInquiry(Action CB = null)
	{
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		AuthV4.showInquiry((result) =>
		{
			POPUP.SetConnecting(false);
			CB?.Invoke();
		});
	}
	/// <summary> 약관 다시보기 </summary>
	/// <param name="CB"></param>
	public void ShowTerms(Action CB = null)
	{
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		AuthV4.showTerms((result) =>
		{
			POPUP.SetConnecting(false);
			CB?.Invoke();
		});
	}
}

