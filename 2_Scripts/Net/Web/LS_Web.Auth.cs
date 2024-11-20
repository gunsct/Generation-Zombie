using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static AppInfo;
using static hive.AuthV4;

public partial class LS_Web
{
	public class REQ_AUTH : REQ_BASE
	{
		/// <summary> 앱 확인 코드 </summary>
		public int VerCode;
		/// <summary> 앱 마켓 </summary>
		public int Market;
		/// <summary> 로그인 타입 </summary>
		public int LoginType;
		/// <summary> 로그인 아이디 </summary>
		public string LoginID;
		/// <summary> UUID </summary>
		public string UUID;
		/// <summary> 보안프로그램 UUID </summary>
		public string SUUID;
		/// <summary> 푸시토큰 </summary>
		public string PushToken;
		/// <summary> 언어코드 </summary>
		public string Lang;
		/// <summary> 국가코드 </summary>
		public string Country;
		/// <summary> 유저타임존 </summary>
		public string TimeZone;
		/// <summary> 디바이스 정보 </summary>
		public string Device;
		/// <summary> 클라에서 이용약관 동의를 체크했는지 여부(자동로그인때는 상관없지만 어플을 재설치후에는 동의창이 나오기때문에 갱신해주어야된다) </summary>
		public bool Agree;
		/// <summary> 삭제 취소 </summary>
		public bool CancelDelete;

		public GPrestorCrossCheckData GPrestorData;
	}

	public class RES_AUTH : RES_BASE
	{
		public long UserNo;
		public string Session;
	}

	/// <param name="args">[0] : ACC_STATE loginType, [1] : string ID, [2] : Delete Cancel</param>
	public void SEND_REQ_AUTH(Action<RES_AUTH> action, params object[] args)
	{
		REQ_AUTH _data = new REQ_AUTH();
		AppInfo info = APPINFO;
		_data.VerCode = info.CLIENT_VERSION_CODE;
		_data.Market = (int)info.Market;
		_data.LoginType = (int)((ACC_STATE)args[0]);
		_data.LoginID = (string)(args[1]);
		_data.UUID = MainMng.Instance.HIVE.GetPlayerDID();// info.UUID;
		_data.PushToken = MainMng.Instance.HIVE.GetToken();// info.logintoken;
		_data.Lang = APPINFO.m_LanguageCode;
		_data.Country = info.CountryCode;
		_data.TimeZone = Utile_Class.Get_TimeZone_String();
		_data.Device = SystemInfo.deviceModel + "|" + SystemInfo.operatingSystem;
		_data.Agree = false;// APPINFO.m_Agree;// PlayerPrefs.GetInt("AGREE_CHECK", 0) == 1;
		_data.CancelDelete = (bool)(args[2]);
		_data.SUUID = info.UUID;
		_data.GPrestorData = APPINFO.GetGPrestorCrossCheckData();
		SendPost(Protocol.REQ_AUTH, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_AUTH res = WEB.ParsResData<RES_AUTH>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_UID = res.UserNo;//.SetDATA(res);
				WEB.SetSession(res.Session);
			}
			MainMng.Instance.m_Utile.SetServerTime(res.servertime);
			action?.Invoke(res);
		});

		if (_data.Agree)
		{
			PlayerPrefs.SetInt("AGREE_CHECK", 1);
			PlayerPrefs.Save();
			APPINFO.m_Agree = false;
		}
	}

	public class REQ_ALL_INFO : REQ_BASE
	{
		public bool Login;
	}

	public class RES_ALL_INFO : RES_BASE
	{
		/// <summary> 유저 정보 </summary>
		public RES_USERINFO User;
		/// <summary> 스테이지 정보 </summary>
		public List<RES_STAGE> Stages;
		/// <summary> 튜토리얼 진행 정보 </summary>
		public List<RES_TUTOINFO> Tutos;
		/// <summary> 캐릭터 정보 </summary>
		public List<RES_CHARINFO> Chars;
		/// <summary> 보유 아이템 정보 </summary>
		public List<RES_ITEMINFO> Items;
		/// <summary> 탐사 정보 </summary>
		public List<RES_ADVINFO> Advs;
		/// <summary> 연구 정보 </summary>
		public List<RES_RESEARCHINFO> Researchs;
		/// <summary> 생산 정보 </summary>
		public List<RES_MAKINGINFO> Makings;
		/// <summary> DNA 정보 </summary>
		public List<RES_DNAINFO> DNAs;
		/// <summary> 좀비 정보 </summary>
		public List<RES_ZOMBIEINFO> Zombies;
		/// <summary> 좀비 방 정보 </summary>
		public List<RES_ZOMBIE_ROOM_INFO> ZombieRooms;
		/// <summary> 우편함 정보 </summary>
		public List<RES_POSTINFO> Posts;
		/// <summary> 업적 정보 </summary>
		public RES_ACHIEVE_INFO Achieve;
		/// <summary> 컬렉션 정보 </summary>
		public RES_COLLECTION_INFO Collection;
		/// <summary> 캠프 정보 </summary>
		public List<RES_CAMP_BUILD_INFO> CampBuilds;
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ALL_INFO
	public void SEND_REQ_ALL_INFO(Action<RES_ALL_INFO> action, bool Init = false)
	{
		REQ_ALL_INFO _data = new REQ_ALL_INFO();
		_data.UserNo = USERINFO.m_UID;
		_data.Login = Init;

		SendPost(Protocol.REQ_ALL_INFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ALL_INFO res = WEB.ParsResData<RES_ALL_INFO>(data);
			if (res.IsSuccess()) {
				USERINFO.SetDATA(res, Init);
				SEND_REQ_SHOP_INFO((res2) => {
					USERINFO.SetDATA(res2);
					IAP.Init();
					action?.Invoke(res);
				});
			}
		});
	}



	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ALL_INFO
	public class REQ_ACC_INFO : REQ_BASE
	{
		/// <summary> 로그인 타입 </summary>
		public int LoginType;
		/// <summary> 로그인 아이디 </summary>
		public string LoginID;
		/// <summary> 삭제 취소 </summary>
		public bool CancelDelete;
	}

	public class RES_ACC_INFO : RES_BASE
	{
		public long UserNo;
		public long[] Cash = new long[2];
		public int LV;
		public int Profile;
		public string Name;
		public long DeleteTime;

		// hive 전용 임의로 셋팅해야됨
		[JsonIgnore] public long m_Cash { get { return Cash[0] + Cash[1]; } }
		[JsonIgnore] public string m_Name { get { return BaseValue.GetUserName(Name);} }

		[JsonIgnore] public PlayerInfo HivePlayerInfo;
	}
	public void SEND_REQ_ACC_INFO(Action<RES_ACC_INFO> action, ACC_STATE type, string id, bool IsCancelDelete = false)
	{
		REQ_ACC_INFO _data = new REQ_ACC_INFO();
		_data.LoginType = (int)type;
		_data.LoginID = id;
		_data.CancelDelete = IsCancelDelete;

		SendPost(Protocol.REQ_ACC_INFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(WEB.ParsResData<RES_ACC_INFO>(data));
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ACC_CHANGE

	public class REQ_ACC_CHANGE : REQ_BASE
	{
		/// <summary> 로그인 타입 </summary>
		public int LoginType;
		/// <summary> 로그인 아이디 </summary>
		public string LoginID;
	}
	public void SEND_REQ_ACC_CHANGE(Action<RES_BASE> action, ACC_STATE type, string id)
	{
		REQ_ACC_CHANGE _data = new REQ_ACC_CHANGE();
		_data.UserNo = USERINFO.m_UID;
		_data.LoginType = (int)type;
		_data.LoginID = id;

		SendPost(Protocol.REQ_ACC_CHANGE, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(WEB.ParsResData<RES_BASE>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ACC_DELETE
	public void SEND_REQ_ACC_DELETE(Action<RES_BASE> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		SendPost(Protocol.REQ_ACC_DELETE, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(WEB.ParsResData<RES_BASE>(data));
		});
	}
}
