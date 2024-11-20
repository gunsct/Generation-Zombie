using Newtonsoft.Json;
using System;
using System.Net; //IPAddress, Dns
using System.Net.Sockets; //AddressFamily
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static LS_Web;

public partial class LS_Web : ClassMng
{
	public enum ServerMode
	{
		LIVE_SERVER = 0,
		DEV_SERVER,
		TEST_SERVER,
		OUT_SERVER,
		LOCAL_SERVER,
	}
	Dictionary<ServerMode, string> m_ServerURLS = new Dictionary<ServerMode, string>()
	{
#if DEV_SERVER || OUT_SERVER || LOCAL_SERVER
		{ ServerMode.LIVE_SERVER, "https://play.playgenerationzombie.com" },	// 라이브 서버
		{ ServerMode.DEV_SERVER, "http://121.165.94.138:11000/PZW" },			// 개발 서버
		{ ServerMode.TEST_SERVER, "http://3.37.213.195:13001" },				// 테스트 서버 (QA)
		{ ServerMode.OUT_SERVER, "http://121.165.94.138:12000/PZW" },			// 외부 유출용 서버(퍼블리셔 전달)
		{ ServerMode.LOCAL_SERVER, "http://localhost:13000" },					// 로컬 서버(서버 개발자 컴퓨터로 접속)
#elif GRABITYUS
		{ ServerMode.LIVE_SERVER, "https://play.playgenerationzombie.com" },	// 라이브 서버
		{ ServerMode.DEV_SERVER, "http://52.23.161.172" },						// 개발 서버
		{ ServerMode.TEST_SERVER, "http://52.23.161.172" },						// 테스트 서버 (QA)
		{ ServerMode.OUT_SERVER, "http://52.23.161.172" },						// 외부 유출용 서버(퍼블리셔 전달)
		{ ServerMode.LOCAL_SERVER, "http://localhost:13000" },					// 로컬 서버(서버 개발자 컴퓨터로 접속)
#else
		{ ServerMode.LIVE_SERVER, "https://play.playgenerationzombie.com" },	// 라이브 서버
		{ ServerMode.DEV_SERVER, "http://121.165.94.138:11000/PZW" },			// 개발 서버
		{ ServerMode.TEST_SERVER, "http://3.37.213.195:13001" },				// 테스트 서버 (QA)
		{ ServerMode.OUT_SERVER, "http://121.165.94.138:12000/PZW" },			// 외부 유출용 서버(퍼블리셔 전달)
		{ ServerMode.LOCAL_SERVER, "http://localhost:13000" },					// 로컬 서버(서버 개발자 컴퓨터로 접속)
#endif
	};
	Dictionary<EServerConfig, string> m_ServerConfig = new Dictionary<EServerConfig, string>();
	const float WAIT_TIME = 15f;

	string SESSION = "";

	bool isNet = false;
	bool isNetErrorPopup = false;
	Queue<IEnumerator> Posts = new Queue<IEnumerator>();

	ServerMode m_SerVerMode;
	UnityWebRequest WWW_REQ = null;
	public void Set_DisConnect()
	{
		Posts.Clear();
		NetEnd();
		POPUP.SetConnecting(false);
	}

	public string GetBaseURL()
	{
		return m_ServerURLS[m_SerVerMode];
	}

	public bool CheckServer()
	{
		var befor = m_SerVerMode;
#if LOCAL_SERVER
		m_SerVerMode = ServerMode.LOCAL_SERVER;
#elif TEST_SERVER
		m_SerVerMode = ServerMode.TEST_SERVER;
#elif DEV_SERVER
		m_SerVerMode = ServerMode.DEV_SERVER;
#elif OUT_SERVER
		m_SerVerMode = ServerMode.OUT_SERVER;
#else
		m_SerVerMode = ServerMode.LIVE_SERVER;
//#	if UNITY_IOS || UNITY_IPHONE
		var data = WEB.GetConfig(EServerConfig.client_ver);
		int code = 0;
		if (int.TryParse(data, out code))
		{
			if (APPINFO.CLIENT_VERSION_CODE > code) m_SerVerMode = ServerMode.TEST_SERVER;
			else m_SerVerMode = befor;
		}
//#	endif
#endif
		return m_SerVerMode != befor;
	}

	public ServerMode Get_Server()
	{
		return m_SerVerMode;
	}

	public bool IS_SendNet()
	{
		return !isNet && !isNetErrorPopup;
	}

	public void Init()
	{
		LoadLocalData();
		MAIN.StartCoroutine(DataSend());
	}

	UnityWebRequest POST(string url, string strData)
	{
		byte[] abyData = Encoding.UTF8.GetBytes(strData);
		int nLen = abyData.Length;
		bool bCompress = false;
		if (nLen > 100)
		{
			// 압축
			byte[] abyTemp = Utile_Class.Compress(abyData, 0, nLen);
			if (abyTemp.Length < nLen)
			{
				abyData = abyTemp;
				nLen = abyData.Length;
				bCompress = true;
			}
		}

		Utile_Class.Encode(abyData, nLen, 0);
		string strBase64 = Convert.ToBase64String(abyData);
		// Base64
		abyData = Encoding.UTF8.GetBytes(strBase64);


		Web_Log("REQ //////////////////////////////////////////////////////////////////////////////////"
			+ "\n// URL : " + url
			+ "\n// HEADERS ==========================="
			+ "\n// Compress : " + bCompress.ToString().ToLower()
			+ "\n// ==================================="
			+ "\n// PostData(default) : " + strData
			+ "\n// PostData(EBase64) : " + strBase64);
		UnityWebRequest www = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
		www.uploadHandler = new UploadHandlerRaw(abyData);
		www.downloadHandler = new DownloadHandlerBuffer();
		www.SetRequestHeader("Content-Type", "application/json; charset=UTF-8");
		www.SetRequestHeader("Compress", bCompress.ToString().ToLower());
		www.SetRequestHeader("Session", SESSION);
		return www;
	}

	public void SetSession(string session)
	{
#if USER_TEST
		SESSION = "Emul";
#else
		SESSION = session;
#endif
	}

	string GetServerPath(string protocol)
	{
		int urlpath = 1;
		if (protocol == Protocol.REQ_APPCHECK
			|| protocol == Protocol.REQ_CONFIG
			|| protocol == Protocol.REQ_SERVERTIME
			|| protocol == Protocol.REQ_AUTH
			|| protocol == Protocol.REQ_ALL_INFO
			|| protocol == Protocol.REQ_ACC_INFO
			|| protocol == Protocol.REQ_ACC_CHANGE
			|| protocol == Protocol.REQ_ACC_DELETE
			)
			urlpath = 0;

#if DEV_SERVER || OUT_SERVER || LOCAL_SERVER
		return urlpath == 0 ? "/Login" : "/Game";
#elif GRABITYUS
		//switch (m_SerVerMode)
		//{
		//case ServerMode.LIVE_SERVER: return urlpath == 0 ? "/Live/Login" : "/Live/Game";
		//case ServerMode.DEV_SERVER:
		//case ServerMode.TEST_SERVER:
		//case ServerMode.OUT_SERVER: return urlpath == 0 ? "/QA/Login" : "/QA/Game";
		//default: return urlpath == 0 ? ":10000" : ":10001";
		//}

		switch (m_SerVerMode)
		{
		case ServerMode.LIVE_SERVER: return urlpath == 0 ? "/Login" : "/Game";
		default: return urlpath == 0 ? ":10000" : ":10001";
		}
#else
		return urlpath == 0 ? "/Login" : "/Game";
#endif
	}

	public void SendPost(string protocol, string sendData, bool ViewConFrame, Action<ushort, string> CB = null)
	{
		Posts.Enqueue(NetPost(protocol, sendData, ViewConFrame, CB));
		//MAIN.StartCoroutine(NetPost(protocol, sendData, ViewConFrame, CB));
	}
	public void SendPost(string protocol, string sendData, Action<ushort, string> CB = null)
	{
		SendPost(protocol, sendData, true, CB);
		//MAIN.StartCoroutine(NetPost(protocol, sendData, true, CB));
	}

	public IEnumerator RunThrowingIterator(IEnumerator enumerator, Action<Exception> done)
	{
		while (true)
		{
			object current;
			try
			{
				if (enumerator.MoveNext() == false)
				{
					break;
				}
				current = enumerator.Current;
			}
			catch (Exception ex)
			{
				done(ex);
				yield break;
			}
			yield return current;
		}
	}

	IEnumerator DataSend()
	{
		while (true)
		{
			yield return new WaitWhile(() => Posts.Count < 1);
			yield return Posts.Dequeue();
		}
	}

	public bool CheckNetState(bool ErrorPop)
	{
		if(Application.internetReachability == NetworkReachability.NotReachable)
		{
			if(ErrorPop) StartStateMsg(EStateError.ERROR_NETERROR);
			return false;
		}
		return true;
	}
	private static void PrintMyAddress()
	{
		IPAddress[] host = Dns.GetHostAddresses(Dns.GetHostName());

		for (int i = 0; i < host.Length; ++i)
		{
			if (host[i].AddressFamily == AddressFamily.InterNetworkV6) //IPv6
			{
				Web_Log($"IPv6 주소 [{host[i]}]");
			}

			if (host[i].AddressFamily == AddressFamily.InterNetwork) //IPv4
			{
				Web_Log($"IPv4 주소 [{host[i]}]");
			}
		}
	}


	IEnumerator CheckNetState(Action<int> CB, bool ViewConFrame)
	{
		// 통신 가능 상태 체크
		float time = Time.unscaledTime + WAIT_TIME;
		int checkcnt = 0;
		int result = 0;
		while (!CheckNetState(false))
		{
			yield return new WaitForEndOfFrame();
			//time -= Time.unscaledTime;
			//Debug.Log(time + " : " + Time.unscaledTime);
			if (time <= Time.unscaledTime)
			{
				// 재요청 팝업
				checkcnt++;
				if (checkcnt > 2)
				{
					result = 1;
					break;
				}
				else
				{
					time = -1;
					Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
						+ "\n// Net State Check Error");
					POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(2), TDATA.GetString(1119), (result, obj) =>
					{
						EMsgBtn btn = (EMsgBtn)result;
						if (btn == EMsgBtn.BTN_YES)
						{
							time = Time.unscaledTime + WAIT_TIME;
							// 컨텍팅 다시 켜주기
							if (ViewConFrame) POPUP.SetConnecting(true);
						}
						else
						{
							NetEnd();
							MAIN.Exit();
						}
					});
					// 팝업이 눌리도록 컨넥팅 꺼주기
					POPUP.SetConnecting(false);
					yield return new WaitWhile(() => time <= 0);
				}
			}
		}
		CB?.Invoke(result);
	}

	void NetEnd(bool netstate = false)
	{
		if (WWW_REQ != null) WWW_REQ.Dispose();
		WWW_REQ = null;
		isNet = netstate;
	}

	IEnumerator NetPost(string protocol, string strData, bool ViewConFrame, Action<ushort, string> CB)
	{
		//PrintMyAddress();
		// 네트워크 기본 에러팝업들은 대부분 앱종료나 다시 시작하는 경우들이므로 에러가뜨고 넘어오는것들은 무시함
		if (isNetErrorPopup) yield break;
		yield return new WaitWhile(() => isNet);

#if WEB_ERROR_TEST && !NOT_USE_NET
		ushort ushConState = EStateError.SUCCESS;
		if (PlayerPrefs.GetString("WEB_ERROR_TEST_PROTOCOL", "").Equals(protocol))
		{
			Web_Log("REQ ERROR TEST //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + string.Format("{0}{1}{2}", GetBaseURL(protocol), GetServer(protocol), protocol)
				+ "\n// PostData(default) : " + strData);

			var msg = JsonConvert.SerializeObject(new RES_BASE() { result_code = (ushort)PlayerPrefs.GetInt("WEB_ERROR_TEST_ERRORCODE", 0) });

			Web_Log("RES ERROR TEST //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + string.Format("{0}{1}{2}", GetBaseURL(protocol), GetServer(protocol), protocol)
				+ "\n// RcvData(default) : " + msg);
			if (BaseCheck(protocol, out ushConState, msg )) CB?.Invoke(ushConState, msg);
			yield break;
		}
#endif

		isNet = true;

		if (ViewConFrame) POPUP.SetConnecting(true);
		//if (MAIN.IS_State(MainState.TITLE))
		//{
		//	if (CheckNetState(true))
		//	{
		//		if (ViewConFrame) POPUP.SetConnecting(false);
		//		NetEnd();
		//		yield break;
		//	}
		//}
		//else
		//{
			int checkstate = -1;
			yield return CheckNetState((result) => { checkstate = result; }, ViewConFrame);
			yield return new WaitWhile(() => checkstate < 0);
			if (checkstate == 1)
			{
				StateError(protocol, EStateError.ERROR_NETERROR, "", CB);
				POPUP.SetConnecting(false);

				NetEnd();
				yield break;
			}
		//}

#if LOCAL_SERVER
		string strURL = string.Format("{0}{2}", GetBaseURL(), GetServerPath(protocol), protocol);
#else
		string strURL = string.Format("{0}{1}{2}", GetBaseURL(), GetServerPath(protocol), protocol);
#endif

		NetEnd(isNet);
		// 재확인을 위한 프로토콜 번호
		long ProtocolNo = (long)MainMng.Instance.UTILE.Get_ServerTime_Milli();
		WWW_REQ = POST(strURL, strData);
		WWW_REQ.SetRequestHeader("reqcode", ProtocolNo.ToString());

#if AUTO_INIT
		if(protocol.Equals(Protocol.REQ_AUTH))
		{
			WWW_REQ.SetRequestHeader("AutoInit", (PlayerPrefs.GetInt("AUTO_INIT", 0) == 1).ToString().ToLower());
			PlayerPrefs.SetInt("AUTO_INIT", 0);
			PlayerPrefs.Save();
		}
#endif

		// 전송시작
		//yield return WWW_REQ.SendWebRequest();
		WWW_REQ.SendWebRequest();
		float time = Time.unscaledTime + WAIT_TIME;
		while (!WWW_REQ.isDone)
		{
			if (time <= Time.unscaledTime)
			{
				//StateError(protocol, EStateError.ERROR_TIMEOUT, "", CB);

				Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
					+ "\n// URL : " + strURL
					+ "\n// ERROR_TIMEOUT");

				yield return RE_Respons_LastData(ProtocolNo, protocol, strData, ViewConFrame, CB);
				yield break;
			}
			yield return new WaitForEndOfFrame();
		}

		ushort ushConState = EStateError.SUCCESS;
		string json = string.Empty;
		if (WWW_REQ.result == UnityWebRequest.Result.ConnectionError)
		{
			// 네트워크 에러
			ushConState = EStateError.ERROR_NETERROR;
			json = WWW_REQ.error;

			Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + strURL
				+ "\n// ERROR_NETWORK_EXCEPTION"
				+ "\n// ERROR : " + WWW_REQ.error);

			yield return RE_Respons_LastData(ProtocolNo, protocol, strData, ViewConFrame, CB);
			yield break;
		}
		else if (WWW_REQ.result == UnityWebRequest.Result.ProtocolError)
		{
			// HTTP 에러
			ushConState = EStateError.ERROR_SERVER_EXCEPTION;
			json = WWW_REQ.error;

			Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + strURL
				+ "\n// ERROR_SERVER_EXCEPTION"
				+ "\n// ERROR : " + WWW_REQ.error);
		}
		else
		{
			Dictionary<string, string> responseHeaders = WWW_REQ.GetResponseHeaders();
			json = WWW_REQ.downloadHandler.text;
			// base64해제
			byte[] abyData = Convert.FromBase64String(json);

			// 복호화
			Utile_Class.Decode(abyData, abyData.Length, 0);
			// 압축 해제
			bool bConpress = responseHeaders.ContainsKey("Compress") && bool.Parse(responseHeaders["Compress"]);
			if (bConpress) abyData = Utile_Class.Decompress(abyData, 0, abyData.Length);//, 0, Utile_Class.CompressType.RFC1951);
			json = Encoding.UTF8.GetString(abyData);

			Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + strURL
				+ "\n// HEADERS ==========================="
				+ "\n// Compress : " + bConpress
				+ "\n// ==================================="
				+ "\n// RcvData(default) : " + WWW_REQ.downloadHandler.text
				+ "\n// RcvData(DBase64) : " + json);
		}

		NetEnd(isNet);
		POPUP.SetConnecting(false);
		StateError(protocol, ushConState, json, CB);
		yield return new WaitWhile(() => isNetErrorPopup);
		isNet = false;

	}
	IEnumerator RE_Respons_LastData(long CheckCode, string protocol, string strData, bool ViewConFrame, Action<ushort, string> CB)
	{
		if (isNetErrorPopup) yield break;
		//yield return new WaitWhile(() => isNet); // 결과만 재요청의경우 이미 기존 연결이 유지되고있을수 있으므로 체크 안함

		int checkstate = -1;
		yield return CheckNetState((result) => { checkstate = result; }, ViewConFrame);
		yield return new WaitWhile(() => checkstate < 0);
		if (checkstate == 1)
		{
			StateError(protocol, EStateError.ERROR_NETERROR, "", CB);
			POPUP.SetConnecting(false);

			NetEnd();
			yield break;
		}
		REQ_RE_RES_LSAT_DATA req = new REQ_RE_RES_LSAT_DATA();
		req.ProtocolCode = CheckCode;
#if LOCAL_SERVER
		string strURL = string.Format("{0}{2}", GetBaseURL(), GetServerPath(Protocol.REQ_RE_RES_LSAT_DATA), protocol);
#else
		string strURL = string.Format("{0}{1}{2}", GetBaseURL(), GetServerPath(Protocol.REQ_RE_RES_LSAT_DATA), protocol);
#endif

		NetEnd(isNet);
		// 재확인을 위한 프로토콜 번호
		long ProtocolNo = (long)MainMng.Instance.UTILE.Get_ServerTime_Milli();
		WWW_REQ = POST(strURL, JsonConvert.SerializeObject(req));
		WWW_REQ.SetRequestHeader("reqcode", ProtocolNo.ToString());

		// 전송시작
		//yield return WWW_REQ.SendWebRequest();
		WWW_REQ.SendWebRequest();

		float time = Time.unscaledTime + WAIT_TIME;
		while (!WWW_REQ.isDone)
		{
			if (time <= Time.unscaledTime)
			{
				//StateError(protocol, EStateError.ERROR_TIMEOUT, "", CB);

				Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
					+ "\n// URL : " + strURL
					+ "\n// ERROR_TIMEOUT");

				StateError(protocol, EStateError.ERROR_TIMEOUT, "", CB);
				POPUP.SetConnecting(false);
				NetEnd();
				yield break;
			}
			yield return new WaitForEndOfFrame();
		}
		while (!WWW_REQ.isDone) yield return new WaitForEndOfFrame();

		ushort ushConState = EStateError.SUCCESS;
		string json = string.Empty;
		if (WWW_REQ.result == UnityWebRequest.Result.ConnectionError)
		{
			// 네트워크 에러
			ushConState = EStateError.ERROR_NETERROR;
			json = WWW_REQ.error;

			Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + strURL
				+ "\n// ERROR_NETWORK_EXCEPTION"
				+ "\n// ERROR : " + WWW_REQ.error);
		}
		else if (WWW_REQ.result == UnityWebRequest.Result.ProtocolError)
		{
			// HTTP 에러
			ushConState = EStateError.ERROR_SERVER_EXCEPTION;
			json = WWW_REQ.error;

			Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + strURL
				+ "\n// ERROR_SERVER_EXCEPTION"
				+ "\n// ERROR : " + WWW_REQ.error);
		}
		else
		{
			Dictionary<string, string> responseHeaders = WWW_REQ.GetResponseHeaders();
			json = WWW_REQ.downloadHandler.text;
			// base64해제
			byte[] abyData = Convert.FromBase64String(json);

			// 복호화
			Utile_Class.Decode(abyData, abyData.Length, 0);
			// 압축 해제
			bool bConpress = responseHeaders.ContainsKey("Compress") && bool.Parse(responseHeaders["Compress"]);
			if (bConpress) abyData = Utile_Class.Decompress(abyData, 0, abyData.Length);//, 0, Utile_Class.CompressType.RFC1951);
			json = Encoding.UTF8.GetString(abyData);

			Web_Log("RES //////////////////////////////////////////////////////////////////////////////////"
				+ "\n// URL : " + strURL
				+ "\n// HEADERS ==========================="
				+ "\n// Compress : " + bConpress
				+ "\n// ==================================="
				+ "\n// RcvData(default) : " + WWW_REQ.downloadHandler.text
				+ "\n// RcvData(DBase64) : " + json);


			RES_BASE pData = ParsResData<RES_BASE>(json);
			if(pData.result_code == EResultCode.ERROR_PROTOCOL_CODE)
			{
				// 마지막 전송 코드가 아닐때 다시 요청
				NetEnd(isNet);
				SendPost(protocol, strData, ViewConFrame, CB);
				yield break;
			}
		}

		NetEnd(isNet);
		POPUP.SetConnecting(false);
		StateError(protocol, ushConState, json, CB);
		yield return new WaitWhile(() => isNetErrorPopup);
		isNet = false;
	}

	void StateError(string protocol, ushort ushStateCode, string strMSG, Action<ushort, string> CB)
	{
		switch (ushStateCode)
		{
		case EStateError.SUCCESS:
			if (BaseCheck(protocol, out ushStateCode, strMSG)) CB(ushStateCode, strMSG);
			break;
		case EStateError.ERROR_NETERROR:          // 네트워크 에러
		case EStateError.ERROR_SERVER_EXCEPTION:  // 서버 에러
		case EStateError.ERROR_TIMEOUT:           // 타임아웃
			StartStateMsg(ushStateCode);
			break;
		}
	}

	bool BaseCheck(string protocol, out ushort ushCode, string strMsg)
	{
		RES_BASE pData = ParsResData<RES_BASE>(strMsg);

		// 기본 에러코드에 맞는 에러팝업
		switch (pData.result_code)
		{
		case EResultCode.ERROR_SERVER_EXCEPTION:
		case EResultCode.ERROR_DB_EXCEPTION:
		case EResultCode.ERROR_PARAMETER:
		case EResultCode.ERROR_SERVER_CHECK:
		case EResultCode.ERROR_NEW_VER:
		case EResultCode.ERROR_TOOLDATA:
		case EResultCode.ERROR_USED_UUID:
		case EResultCode.ERROR_USER_BLOCK:
		case EResultCode.ERROR_NOT_FOUND_USER:
		case EResultCode.ERROR_LOGIN:
		case EResultCode.ERROR_LOGIN_ID:
		case EResultCode.ERROR_DIF_DIVICE:
		case EResultCode.ERROR_CROSS_CHECK_DATA_NULL:
		case EResultCode.ERROR_CROSS_CHECK:
			ushCode = pData.result_code;
			StartMsg(pData.result_code);
			return false;
		}

		if (pData.UseMoneys != null && pData.UseMoneys.Count > 0)
		{
			for (int i = pData.UseMoneys.Count - 1; i > -1; i--)
			{
				RES_USE_MONEY use = pData.UseMoneys[i];
				switch (use.Type)
				{
				case ItemType.Dollar:
					USERINFO.m_Achieve.Check_Achieve(AchieveType.Dollar_Use, 0, use.Use);
					break;
				case ItemType.Cash:
					USERINFO.m_Achieve.Check_Achieve(AchieveType.GoldTeeth_Use, 0, use.Use);
					break;
				case ItemType.Energy:
					USERINFO.m_Achieve.Check_Achieve(AchieveType.Energy_Use, 0, use.Use);
					break;
				}
			}
		}

		if (pData.Rewards != null)
		{
			Dictionary<Res_RewardType, List<RES_REWARD_BASE>>  dic = pData.GetRewards_Dic();
			// 초기화데이터는 차감도 포함되므로 보상 데이터에서 검색 
			// 금니 획득
			var checkAdds = dic.ContainsKey(Res_RewardType.Cash) ? dic[Res_RewardType.Cash].FindAll(o => ((RES_REWARD_MONEY)o).Add > 0).Sum(o => ((RES_REWARD_MONEY)o).Add) : 0;
			//var checkAdds = pData.GetRewards(Res_RewardType.Cash).FindAll(.Rewards.SelectMany(r => r.Rewards.SelectMany(x => x.Infos.FindAll(o => o.Type == Res_RewardType.Cash && ((RES_REWARD_MONEY)o).Add > 0))).Sum(o => ((RES_REWARD_MONEY)o).Add);
			if (checkAdds > 0) USERINFO.m_Achieve.Check_Achieve(AchieveType.GoldTeeth_Count, 0, checkAdds);
			// 달러 획득
			checkAdds = dic.ContainsKey(Res_RewardType.Money) ? dic[Res_RewardType.Money].FindAll(o => ((RES_REWARD_MONEY)o).Add > 0).Sum(o => ((RES_REWARD_MONEY)o).Add) : 0;
			//checkAdds = pData.Rewards.SelectMany(r => r.Rewards.SelectMany(x => x.Infos.FindAll(o => o.Type == Res_RewardType.Money && ((RES_REWARD_MONEY)o).Add > 0))).Sum(o => ((RES_REWARD_MONEY)o).Add);
			if (checkAdds > 0) USERINFO.m_Achieve.Check_Achieve(AchieveType.Dollar_Count, 0, checkAdds);
			List<RES_REWARD_BASE> list;
			if (dic.ContainsKey(Res_RewardType.DNA))
			{
				list = dic[Res_RewardType.DNA];
				if(!protocol.Equals(Protocol.REQ_DNA_UPGRADE))
				{
					for (int i = 0; i < list.Count; i++)
					{
						var info = (RES_REWARD_DNA)list[i];
						var tdata = TDATA.GetDnaTable(info.Idx);
						USERINFO.m_Achieve.Check_Achieve(AchieveType.DNA_Grade_Count, tdata.m_Grade);
						USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.DNA_LevelUp_Count, 0, info.Lv);
						USERINFO.m_Collection.Check(CollectionType.DNA, info.Idx, tdata.m_Grade);
					}
				}
			}

			if (dic.ContainsKey(Res_RewardType.Zombie))
			{
				list = dic[Res_RewardType.Zombie];
				for (int i = 0; i < list.Count; i++)
				{
					var info = (RES_REWARD_ZOMBIE)list[i];
					var tdata = TDATA.GetZombieTable(info.Idx);
					USERINFO.m_Achieve.Check_Achieve(AchieveType.Zombie_Grade_Count, tdata.m_Grade);
					USERINFO.m_Collection.Check(CollectionType.Zombie, info.Idx, tdata.m_Grade);
				}
			}

			// 보상 미션 체크
			list = pData.GetRewards();
			for(int i = list.Count - 1; i> -1; i--)
			{
				RES_REWARD_BASE rew = list[i];
				if (rew.result_code == EResultCode.SUCCESS_POST) continue;
				switch(rew.Type)
				{
				case Res_RewardType.Item:
					USERINFO.Check_Mission(MissionType.GetItem, rew.GetIdx(), 0, ((RES_REWARD_ITEM)rew).Cnt);
					break;
				}
			}
		}

		if (pData.InitCash != null)
		{
			var befor = USERINFO.m_Cash;
			var after = pData.InitCash[0] + pData.InitCash[1];
			DLGTINFO?.f_RFCashUI?.Invoke(after, befor);
			USERINFO.m_BCash = befor;
			USERINFO._Cash[0] = pData.InitCash[0];
			USERINFO._Cash[1] = pData.InitCash[1];
		}

		if (pData.InitMoney != null)
		{
			USERINFO.m_BMoney = USERINFO.m_Money;
			DLGTINFO?.f_RFMoneyUI?.Invoke(pData.InitMoney.Value, USERINFO.m_Money);
			USERINFO.m_Money = pData.InitMoney.Value;
		}

		if (pData.InitExp != null)
		{
			USERINFO.m_BExp = USERINFO.m_Exp[(int)EXPType.Ingame];
			USERINFO.m_Exp[(int)EXPType.Ingame] = pData.InitExp.Value;
		}

		if (pData.InitInven != null)
		{
			USERINFO.m_InvenSize = pData.InitInven.Value;
		}

		if (pData.InitEQGachaCnt != null)
		{
			USERINFO.m_ShopEquipGachaExp = pData.InitEQGachaCnt.Value;
		}

		if (pData.InitPVPCoin != null)
		{
			USERINFO.m_PVPCoin = pData.InitPVPCoin.Value;
		}

		if (pData.InitGCoin != null)
		{
			DLGTINFO?.f_RFGCoinUI?.Invoke(pData.InitGCoin.Value, USERINFO.m_GCoin);
			USERINFO.m_GCoin = pData.InitGCoin.Value;
		}

		if (pData.InitGPoint != null && USERINFO.m_Guild != null && USERINFO.m_Guild.MyInfo != null)
		{
			USERINFO.m_Guild.MyInfo.Point = pData.InitGPoint.Value;
		}

		if (pData.InitGExp != null && USERINFO.m_Guild != null)
		{
			USERINFO.m_Guild.TotalExp = pData.InitGExp.Value;
		}

		if (pData.InitMileage != null) {
			DLGTINFO?.f_RFMileageUI?.Invoke(pData.InitMileage.Value, USERINFO.m_Mileage);
			USERINFO.m_Mileage = pData.InitMileage.Value;
		}

		if (pData.InitLV != null)
		{
			// 레벨및 경험치 변경됨
			USERINFO.m_Exp[(int)EXPType.User] = pData.InitLV.Exp;
			USERINFO.m_LV = pData.InitLV.LV;
		}

		if (pData.InitEnergy != null)
		{
			USERINFO.m_Energy.Cnt = pData.InitEnergy.Cnt;
			USERINFO.m_Energy.STime = pData.InitEnergy.STime;
			DLGTINFO?.f_RFShellUI?.Invoke(USERINFO.m_Energy.Cnt);
			HIVE.Check_Loaclpush_Energy();
		}

		// 갱신 정보 셋팅해주기
		if (pData.InitItems != null)
		{
			USERINFO.SetDATA(pData.InitItems);
			if(pData.InitItems.Find(o=>USERINFO.IsUseEquipItem( o.UID)) != null)
				USERINFO.GetUserCombatPower();
		}

		if (pData.InitZombies != null)
		{
			USERINFO.SetDATA(pData.InitZombies);
		}

		if (pData.InitDNAs != null)
		{
			USERINFO.SetDATA(pData.InitDNAs);
		}

		if (pData.Decks != null) {
			USERINFO.SetDATA(pData.Decks);
		}

		if (pData.InitChars != null) {
			USERINFO.SetDATA(pData.InitChars);
		}

		if (pData.InitPost != null)
		{
			USERINFO.SetDATA(pData.InitPost);
		}

		if (pData.InitEvents != null)
		{
			USERINFO.m_Event.SetDATA(pData.InitEvents);
		}

		if (pData.MyChallenge != null && pData.MyChallenge.Count > 0)
		{
			// 챌린지 순위 변동 발생
			var list = pData.MyChallenge.FindAll(o => o.Mode != ChallengeMode.Week);
			if (list.Count > 0)
			{
				// 기존 같은 정보가 있다면 제거후
				MAIN.m_ChallengeAlram.RemoveAll(o => list.Find(l => l.Mode == o.Mode && l.Type == o.Type) != null);
				// 새로운 데이터로 넣어주기
				MAIN.m_ChallengeAlram.AddRange(list);
			}
		}


		if (pData.InitCampBuilds != null && pData.InitCampBuilds.Count > 0)
		{
			// 챌린지 순위 변동 발생
			USERINFO.SetDATA(pData.InitCampBuilds);
		}

		if (pData.InitPackInfo != null && pData.InitPackInfo.Count > 0)
		{
			// 챌린지 순위 변동 발생
			USERINFO.m_ShopInfo.SetPackageInfo(pData.InitPackInfo);
		}
		ushCode = pData.result_code;

		return true;
	}

	public void StartStateMsg(ushort ushStateCode)
	{
		string title = TDATA.GetString(2);
		string msg = "";
		switch (ushStateCode)
		{
		case EStateError.ERROR_TIMEOUT:
		case EStateError.ERROR_NETERROR:
			// 네트워크 연결이 원활하지 않습니다.\n네트워크 상태를 확인해주세요.
			msg = TDATA.GetString(4);
			break;
		default:
			// 네트워크 통신에 문제가 발생 했습니다.\n(CODE: {0}) ushStateCode
			msg = string.Format(TDATA.GetString(3), ushStateCode);
			break;

		}
		POPUP.LockConnecting(false);
		isNetErrorPopup = true;
		MAIN.StopAllCoroutines();
		POPUP.Set_MsgBox(PopupName.Msg_OK, title, msg, (result, obj) =>
		{
			switch (ushStateCode)
			{
			case EStateError.ERROR_TIMEOUT:
			case EStateError.ERROR_NETERROR:
				// 네트워크 연결이 원활하지 않습니다.\n네트워크 상태를 확인해주세요.
				MAIN.Exit();
				break;
			default:
				// 네트워크 통신에 문제가 발생 했습니다.\n(CODE: {0}) ushStateCode
				MAIN.ReStart();
				break;

			}
			isNetErrorPopup = false;
		});
	}

	void StartMsg(ushort ushOutCode)
	{
		POPUP.LockConnecting(false);
		string msg = "";
		switch (ushOutCode)
		{
		case EResultCode.ERROR_SERVER_CHECK:
			WEB.SEND_REQ_CONFIG((res) => {
				// 서버 점검중 입니다.\n예상 점검 종료 시간 : {1}(UTC {0})
				StartErrorPopup(ushOutCode, string.Format(TDATA.GetString(5), GetConfig(EServerConfig.server_time_zone), GetConfig(EServerConfig.server_check_etime)));
			}, new EServerConfig[] { EServerConfig.server_check_stime, EServerConfig.server_check_etime });
			return;
		case EResultCode.ERROR_NEW_VER:
			// 새 버전이 출시되었습니다.\n게임 진행을 위해서는 새 버전을 설치해 주세요.\n확인 버튼을 누르시면 설치 화면으로 이동합니다.
			msg = TDATA.GetString(7);
			break;
		case EResultCode.ERROR_USER_BLOCK:
			// 접속이 제한된 계정입니다.\n해당 계정으로 접속을 원하실 경우\n<color=Red>[ {0} ]</color>로 문의 부탁드립니다.\n문의처 : {1}
			msg = string.Format(TDATA.GetString(8), GetConfig(EServerConfig.inquiry_company_name), GetConfig(EServerConfig.inquiry_email));
			break;
		case EResultCode.ERROR_LOGIN:
			msg = TDATA.GetString(583);
			break;
		case EResultCode.ERROR_LOGIN_ID:
			msg = TDATA.GetString(890);
			break;
		case EResultCode.ERROR_DIF_DIVICE:
			msg = TDATA.GetString(582);
			break;
		default:
			// 네트워크 통신에 문제가 발생 했습니다.\n(CODE: {0:X})
			msg = string.Format(TDATA.GetString(3), string.Format("{0:X}", ushOutCode));
			break;

		}

		StartErrorPopup(ushOutCode, msg);
	}

	void StartErrorPopup(ushort ushOutCode, string Msg)
	{
		string title = TDATA.GetString(2);
		isNetErrorPopup = true;
		MAIN.StopAllCoroutines();
		var popup = POPUP.Set_MsgBox(PopupName.Msg_OK, title, Msg, (result, obj) =>
		{
			isNetErrorPopup = false;
			switch (ushOutCode)
			{
			case EResultCode.ERROR_SERVER_CHECK:
			case EResultCode.ERROR_CROSS_CHECK_DATA_NULL:
			case EResultCode.ERROR_CROSS_CHECK:
				MAIN.Exit();
				return;
			case EResultCode.ERROR_NEW_VER:
				UTILE.OpenURL(GetConfig(EServerConfig.market_url));
				MAIN.Exit();
				return;
			// 이메일 연결
			case EResultCode.ERROR_USER_BLOCK:
				// UTILE.Send_Email(GetConfig(EServerConfig.inquiry_email), "", "");
				HIVE.ShowInquiry();
				//MAIN.Exit();
				return;
			case EResultCode.ERROR_LOGIN_ID:
			case EResultCode.ERROR_DIF_DIVICE:
				ACC.Logout((result) => {
					//ACC.LoginType = ACC_STATE.NONE;
					PlayerPrefs.SetInt("ACC_STATE", (int)ACC_STATE.NONE);
					PlayerPrefs.Save();
					MAIN.ReStart();
				}, ACC_STATE.NONE);
				return;
			}
			MAIN.ReStart();
		});
		popup.isCloaseLock = ushOutCode == EResultCode.ERROR_USER_BLOCK;
	}

	public void StartErrorMsg(ushort ushOutCode, System.Action<int, GameObject> cb = null)
	{
		switch (ushOutCode)
		{
			case EResultCode.ERROR_ENERGY:
				POPUP.StartLackPop(BaseValue.ENERGY_IDX);
				break;
			case EResultCode.ERROR_CASH:
				POPUP.StartLackPop(BaseValue.CASH_IDX);
				break;
			case EResultCode.ERROR_MONEY:
				POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
				break;
			case EResultCode.ERROR_CHAR_EXP:
				POPUP.StartLackPop(BaseValue.EXP_IDX, true);
				break;
			case EResultCode.ERROR_PVPCOIN:
				POPUP.StartLackPop(BaseValue.PVPCOIN_IDX);
				break;
			case EResultCode.ERROR_GCOIN:
				POPUP.StartLackPop(BaseValue.GUILDCOIN_IDX);
				break;
			case EResultCode.ERROR_MILEAGE:
				POPUP.StartLackPop(BaseValue.MILEAGE_IDX);
				break;
			case EResultCode.ERROR_INVEN_SIZE:
				// 가방 구매가 가능할경우 체크
				if(USERINFO.IsBuyBagSize())
				{
					POPUP.Set_MsgBox(PopupName.Msg_YN, "", TDATA.GetString(4004), (btn, obj) =>
					{
						if ((EMsgBtn)btn == EMsgBtn.BTN_YES)
						{
							POPUP.StartInvenBuyPopup(cb);
						}
					});
				}
				else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(272));
				break;
			case EResultCode.ERROR_FILTER:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(537));
				break;
			case EResultCode.ERROR_ETC_ITEM:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(374));
				break;
			case EResultCode.ERROR_NOT_OPEN:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(562));
				break;
			case EResultCode.ERROR_BUY_CNT:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(563));
				break;
			case EResultCode.ERROR_PLAY_CNT:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(564));
				break;
			case EResultCode.ERROR_MAX_GRADE:
			case EResultCode.ERROR_ITEM_MAX_GRADE:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(565));
				break;
			case EResultCode.ERROR_CHAR_PRE_SERUM:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(446));
				break;
			case EResultCode.ERROR_RES_PLAY:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(255));
				break;
			case EResultCode.ERROR_RES_PRECED:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(566));
				break;
			case EResultCode.ERROR_RES_MAX_LV:
			case EResultCode.ERROR_MAX_LV:
			case EResultCode.ERROR_DNA_MAX_LV:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(568));
				break;
			case EResultCode.ERROR_RES_TIME:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(567));
				break;
			case EResultCode.ERROR_MAKING_IDX:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(569));
				break;
			case EResultCode.ERROR_MAKING_LV:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(570));
				break;
			case EResultCode.ERROR_MAX_CAGE:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(561));
				break;
			case EResultCode.ERROR_MAX_ROOM_SIZE:
			case EResultCode.ERROR_CAGE_SIZE:
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(571));
				break;
			case EResultCode.ERROR_GUILD_GRADE:
				if(USERINFO.m_Guild.MyInfo != null) USERINFO.m_Guild.MyInfo.Grade = GuildGrade.Normal;
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
				break;
			default:
				string title = TDATA.GetString(2);
				string msg = string.Format(TDATA.GetString(3), string.Format("{0:X}", ushOutCode));
				POPUP.Set_MsgBox(PopupName.Msg_OK, title, msg, cb);
				break;
		}
	}


	// config
	EServerConfig GetConfigIdx(string name)
	{
		switch (name)
		{
		case "google_client_ver": case "appstore_client_ver": case "onestore_client_ver": return EServerConfig.client_ver;
		case "google_url_market": case "appstore_url_market": case "onestore_url_market": return EServerConfig.market_url;
		case "google_url_CDN": case "appstore_url_CDN": case "onestore_url_CDN": return EServerConfig.CDN_url;
		}
		if (!Enum.IsDefined(typeof(EServerConfig), name)) return EServerConfig.End;
		return (EServerConfig)Enum.Parse(typeof(EServerConfig), name, true);
	}

	public string GetConfigKeyString(EServerConfig key)
	{
		switch (key)
		{
		case EServerConfig.market_url:
			switch (APPINFO.Market)
			{
			case EMarketType.APPLE: return "appstore_url_market";
			case EMarketType.ONE_STORE: return "onestore_url_market";
			}
			return "google_url_market";
		case EServerConfig.client_ver:
			switch (APPINFO.Market)
			{
			case EMarketType.APPLE: return "appstore_client_ver";
			case EMarketType.ONE_STORE: return "onestore_client_ver";
			}
			return "google_client_ver";
		case EServerConfig.CDN_url:
			switch (APPINFO.Market)
			{
			case EMarketType.APPLE: return "appstore_url_CDN";
			case EMarketType.ONE_STORE: return "onestore_url_CDN";
			}
			return "google_url_CDN";
		}
		return key.ToString();
	}

	public void SetConfig(ConfigData data)
	{
		if (!IS_USE_DATA(data.Name)) return;
		EServerConfig eKey = GetConfigIdx(data.Name);

		switch (eKey)
		{
		case EServerConfig.server_time_zone: UTILE.Set_ServerTimeZone(data.Value); break;

		}
		if (m_ServerConfig.ContainsKey(eKey)) m_ServerConfig[eKey] = data.Value;
		else m_ServerConfig.Add(eKey, data.Value);
	}

	public string GetConfig(EServerConfig key)
	{
		if (!m_ServerConfig.ContainsKey(key)) return "";
		return m_ServerConfig[key];
	}

	public bool IS_USE_DATA(string name)
	{
		EMarketType matket = EMarketType.END;
		switch (name)
		{
		case "google_client_ver": case "google_url_market": case "google_url_CDN": matket = EMarketType.GOOGLE; break;
		case "appstore_client_ver": case "appstore_url_market": case "appstore_url_CDN": matket = EMarketType.APPLE; break;
		case "onestore_client_ver": case "onestore_url_market": case "onestore_url_CDN": matket = EMarketType.ONE_STORE; break;
		}
		return matket == APPINFO.Market || matket == EMarketType.END;
	}

	public T ParsResData<T>(string json) where T : RES_BASE
	{
		T obj = JsonConvert.DeserializeObject<T>(json, new JsonConverter[] { new RES_REWARD_BASE_Conv() });
		return obj;
	}
}

public class REQ_BASE
{
	/// <summary> 유저 번호 </summary>
	public long UserNo;
	/// <summary> 유저 총 전투력 </summary>
	public long UserPower;
	/// <summary> 앱 마켓 모드 </summary>
	public EMarketType AppMode;
	/// <summary> 앱 버전 코드</summary>
	public int AppVerCode;

	public REQ_BASE()
	{
		if(MainMng.Instance.USERINFO != null && MainMng.Instance.USERINFO.m_UID != 0)
		{
			UserNo = MainMng.Instance.USERINFO.m_UID;
		}

		AppInfo info = MainMng.Instance.APPINFO;
		AppMode = info.Market;
		AppVerCode = info.CLIENT_VERSION_CODE;
	}
}

public class RES_BASE
{
	public ushort result_code;

	public string servertime;

	//public long time;
	/// <summary> 공통 null 이면 데이터를 보내지 않음 </summary>
	public List<RES_REWARDS> Rewards;
	/// <summary> 재화 사용 정보 (통합으로 사용하기위해 셋팅)</summary>
	public List<RES_USE_MONEY> UseMoneys;
	/// <summary> 사용된 아이템 (통합으로 사용하기위해 셋팅) </summary>
	public List<RES_REWARD_ITEM> UseItem;


	/// <summary> 변경된 덱 정보 </summary>
	public List<RES_DECKINFO> Decks;
	/// <summary> 동기화용 캐시 </summary>
	public long[] InitCash;
	/// <summary> 동기화용 달러 </summary>
	public long? InitMoney;
	/// <summary> 동기화용 경험치 </summary>
	public long? InitExp;
	/// <summary> 가방 사이즈 </summary>
	public int? InitInven;
	/// <summary> 장비 가차 횟수 </summary>
	public long? InitEQGachaCnt;
	/// <summary> PVP 재화 </summary>
	public long? InitPVPCoin;
	/// <summary> 길드 코인 </summary>
	public long? InitGCoin;
	/// <summary> 길드 기여도 </summary>
	public long? InitGPoint;
	/// <summary> 길드 누적 경험치 </summary>
	public long? InitGExp;
	/// <summary> 마일리지 </summary>
	public long? InitMileage;
	/// <summary> 동기화용 유저 레벨 </summary>
	public RES_USERLV InitLV;
	/// <summary> 동기화용 에너지 </summary>
	public RES_TIMEITEM InitEnergy;
	/// <summary> 동기화용 지급된 캐직터의 마지막 결과 </summary>
	public List<RES_CHARINFO> InitChars;
	/// <summary> 동기화용 지급된 아이템의 마지막 결과 </summary>
	public List<RES_ITEMINFO> InitItems;
	/// <summary> 동기화용 지급된 DNA의 마지막 결과 </summary>
	public List<RES_DNAINFO> InitDNAs;
	/// <summary> 동기화용 지급된 좀비의 마지막 결과 </summary>
	public List<RES_ZOMBIEINFO> InitZombies;
	/// <summary> 동기화용 우편함 결과 </summary>
	public List<RES_POSTINFO> InitPost;
	/// <summary> 동기화용 이벤트 </summary>
	public List<RES_MY_FAEVENT_INFO> InitEvents;

	/// <summary> 챌린지 순위변동 동기화 </summary>
	public List<RES_CHALLENGE_MYRANKING> MyChallenge;

	/// <summary> 캠프 빌드 동기화 </summary>
	public List<RES_CAMP_BUILD_INFO> InitCampBuilds;

	/// <summary> 상점 팩 정보 </summary>
	public List<RES_SHOP_DAILYPACK_INFO> InitPackInfo;


	public bool IsSuccess()
	{
		switch (result_code)
		{
		case EResultCode.SUCCESS:
		case EResultCode.SUCCESS_NEW_AUTH:
		case EResultCode.SUCCESS_REWARD_PIECE:
		case EResultCode.SUCCESS_POST:
		case EResultCode.SUCCESS_INVEN:
			return true;
		}
		return false;
	}

	public List<RES_REWARD_BASE> GetRewards()
	{
		if (Rewards == null) return new List<RES_REWARD_BASE>();
		return Rewards.SelectMany(r => r.Rewards.SelectMany(x => x.Infos)).ToList();
	}

	public List<RES_REWARD_BASE> GetRewards(Res_RewardType type)
	{
		if (Rewards == null) return new List<RES_REWARD_BASE>();
		return Rewards.SelectMany(r => r.Rewards.SelectMany(x => x.Infos.FindAll(o => o.Type == type))).ToList();
	}

	public Dictionary<Res_RewardType, List<RES_REWARD_BASE>> GetRewards_Dic()
	{
		if (Rewards == null) return new Dictionary<Res_RewardType, List<RES_REWARD_BASE>>();
		var list = GetRewards();
		var group = list.GroupBy(o => o.Type);
		return group.ToDictionary(p => p.Key, p=> p.ToList());
	}
}

public class RES_TIMEITEM
{
	/// <summary> 정보 이름 </summary>
	public long Cnt;
	/// <summary> 갱신 시간(카운트가 진행된 시간) </summary>
	public long STime;
}
public class RES_USE_MONEY
{
	/// <summary> 사용한 아이템 타입 </summary>
	public ItemType Type;
	/// <summary> 인덱스(캠프 자원때문에 추가됨) </summary>
	public int Idx;
	/// <summary> 소모량 </summary>
	public long Use;
	/// <summary> 이전값 </summary>
	public long Befor;
	/// <summary> 현재값 </summary>
	public long Now;
	/// <summary> 갱신 시간(카운트가 진행된 시간) </summary>
	public long STime;
}
