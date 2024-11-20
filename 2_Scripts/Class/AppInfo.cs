using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Globalization;
#endif
using UnityEngine;
using UnityEngine.Networking;

// 로그인타입 모델에따라 서버에서 걸러야할것들이 필요함
// ex) PC의경우 과금은 무조건 성공으로 하고 진행해야됨
public enum EPlatform
{
	PC = 0      //PC
	, ANDROID   //안드로이드
	, IPHONE    //아이폰
	, END
}

//마켓 속성
public enum EMarketType
{
	ALL = 0				// 전체 또는 에뮬
	, GOOGLE			//구글 마켓
	, APPLE				//애플 마켓
	, ONE_STORE			//3사 통합 마켓
	, AMAZON			//아마존
	, GALAXYAPPS		//갤럭시 앱스
	, HIVE				// 하이브 인앱 검증용
	, CODA				// 코다샵
	, END				// 마켓이아니고 토탈개수
}
public class AppInfo
{
	public string UUID;
	public string PushToken {
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				PlayerPrefs.SetString("PushToken", value);
				PlayerPrefs.Save();
			}
		}
		get {
			return PlayerPrefs.GetString("PushToken", "");
		}
	}

#if UNITY_EDITOR
	public EPlatform Platform = EPlatform.PC;
#elif UNITY_ANDROID
	public EPlatform Platform = EPlatform.ANDROID;
#elif UNITY_IOS || UNITY_IPHONE
	public EPlatform Platform = EPlatform.IPHONE;
#endif


#if UNITY_IOS || UNITY_IPHONE
	public EMarketType Market = EMarketType.APPLE;
#else
#	if ONE_STORE
		public EMarketType Market = EMarketType.ONE_STORE;
#	else
		public EMarketType Market = EMarketType.GOOGLE;
#	endif
#endif

	LanguageCode _Language;
	public LanguageCode m_Language
	{
		get { return _Language; }
		set
		{
			_Language = value;
			
			PlayerPrefs.SetString("LanguageCode", Utile_Class.Get_LanguageCode(_Language));
			PlayerPrefs.Save();
		}
	}

	public string m_LanguageCode
	{
		get { return Utile_Class.Get_LanguageCode(m_Language); }
	}
	public string m_LanguageName
	{
		get { return Utile_Class.Get_LanguageName(m_Language); }
	}

	public string m_strVersion
	{
		get
		{
			return string.Format(version.VerFormat, Application.version, CLIENT_VERSION_CODE.ToString("0#"));
		}
	}
#if USER_TEST && UNITY_EDITOR
	public long m_TestUserNo { get { return version.TestUserNo; } }
#endif
	string _Nation;
	public string CountryCode {
		set { _Nation = value; }
		get {
			string nation = _Nation;
			if (MainMng.Instance.HIVE != null)
			{
				nation = MainMng.Instance.HIVE.GetNation();
				if (string.IsNullOrEmpty(nation) || nation.Equals("UNKNOWN")) return _Nation;
			}
			return nation;
		}
	}

	version_data version;

	[Serializable]
	public class version_data
	{
		public int Android_CLIENT_VERSION = 1;
		public int iOS_CLIENT_VERSION = 1;
#if USER_TEST && UNITY_EDITOR
		public long TestUserNo;
#endif
		public string VerFormat;
	}

	public int CLIENT_VERSION_CODE
	{
		get
		{
			if (version == null) LoadVersion();
#if UNITY_IOS || UNITY_IPHONE
			return version.iOS_CLIENT_VERSION;
#else
			return version.Android_CLIENT_VERSION;
#endif
		}
	}

#if UNITY_EDITOR
	CultureInfo pCulture;

	[System.Runtime.InteropServices.DllImport("KERNEL32.DLL")]
	private static extern int GetSystemDefaultLCID();
#endif
	public bool m_Agree = false;
	public AppInfo()
	{
#if UNITY_EDITOR
#if UNITY_EDITOR_OSX
		CountryCode = Utile_Class.Get_CountryCode();
#else
		pCulture = new CultureInfo(GetSystemDefaultLCID());
		string[] temp = pCulture.ToString().Split('-');
		CountryCode = temp.Length < 2 ? temp[0] : temp[1];
#endif
#elif NOT_USE_NET
		CountryCode = Utile_Class.Get_CountryCode();
#else
		MainMng.Instance.StartCoroutine(GetCountryInfo());
#endif
		LoadVersion();


		m_Language = Utile_Class.Get_LanguageCode(PlayerPrefs.GetString("LanguageCode", Utile_Class.Get_LanguageCode(Application.systemLanguage)));

		UUID = GPresto.Protector.Engine.GPrestoEngine.GetUUID();//SystemInfo.deviceUniqueIdentifier;
	}
	public bool IsEmulator()
	{
#if UNITY_EDITOR
		return true;
#elif UNITY_ANDROID
		//AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		//AndroidJavaObject context = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity").Call<AndroidJavaObject>("getApplicationContext");
		//AndroidJavaClass cls = new AndroidJavaClass("com.nekolaboratory.EmulatorDetector");
		//return cls.CallStatic<bool>("isEmulator", context);
		AndroidJavaClass cls = new AndroidJavaClass("com.bishopsoft.Presto.SDK.Presto");
		bool IsEmul = cls.CallStatic<int>("getEmul") > 0;
		cls.Dispose();
		return IsEmul;
#else
		return false;
#endif
	}


	public void LoadVersion()
	{
		TextAsset text = Resources.Load<TextAsset>("version");
		if (text != null && text.text.Length > 2)
		{
			version = JsonConvert.DeserializeObject<version_data>(text.text);
		}
	}

	public class GPrestorCrossCheckData
	{
		public string sData = GPresto.Protector.Engine.GPrestoEngine.GetsData();
#if UNITY_EDITOR
		public int engine_state = 1000000;
#else
		public int engine_state = GPresto.Protector.Engine.GPrestoEngine.GetStatus() ? 1 : 0;
#endif
	}
	public GPrestorCrossCheckData GetGPrestorCrossCheckData()
	{
		return new GPrestorCrossCheckData();
	}


	[Serializable]
	public class Res_Ipapi
	{
		public string city;
		public string country;
		public string countryCode;
		public string isp;
		/// <summary> 위도 </summary>
		public float lat;
		/// <summary> 경도 </summary>
		public float lon;
		public string org;
		public string query;
		public string region;
		public string regionName;
		public string status;
		public string timezone;
		public string zip;
	}

	IEnumerator GetCountryInfo()
	{
		// IP로 국가 알아내기
		UnityWebRequest www = new UnityWebRequest("http://ip-api.com/json");
		www.downloadHandler = new DownloadHandlerBuffer();
		yield return www.SendWebRequest();
		if (www.result != UnityWebRequest.Result.ConnectionError && www.result != UnityWebRequest.Result.ProtocolError)
		{
			Res_Ipapi pResData = JsonConvert.DeserializeObject<Res_Ipapi>(www.downloadHandler.text);
			if ("success".Equals(pResData.status)) CountryCode = pResData.countryCode;
		}

		// 받기 실패했다면 디바이스 언어로 셋팅
		if (string.IsNullOrEmpty(CountryCode) || CountryCode.Length < 1) CountryCode = Utile_Class.Get_CountryCode();
	}
}
