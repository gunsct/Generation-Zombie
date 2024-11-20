using hive;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static hive.AuthV4;
using static LS_Web;

public partial class HiveMng : ClassMng
{
	PlayerInfo m_PlayerInfo;
	ProviderType m_PlayerAccType = ProviderType.AUTO;

	public void SetPlayerInfo(PlayerInfo info)
	{
		m_PlayerInfo = info;
		if(info != null)
		{
			var logintype = ProviderType.GUEST;
			if (m_PlayerInfo.providerInfoData.Count() > 0)
			{
				var data = m_PlayerInfo.providerInfoData.ElementAt(0);
				logintype = data.Value.providerType;
			}
			SaveLoginType(logintype);
			GetPushInfo();
		}
		else SaveLoginType(ProviderType.AUTO);
	}

	public PlayerInfo GetPlayerInfo()
	{
		return m_PlayerInfo;
	}

	/// <summary> 유저 ID </summary>
	public long GetPlayerID()
	{
		if (m_PlayerInfo == null) return 0;
		return m_PlayerInfo.playerId;
	}
	/// <summary> 유저 ID </summary>
	public string GetPlayerDID()
	{
		if (m_PlayerInfo == null) return "";
		return m_PlayerInfo.did;
	}
	/// <summary> 유저 ID </summary>
	public string GetToken()
	{
		if (m_PlayerInfo == null) return "";
		return m_PlayerInfo.playerToken;
	}

	public ProviderType GetLoginType(ACC_STATE eAcc)
	{
		switch(eAcc)
		{
		case ACC_STATE.Google: return ProviderType.GOOGLE;
		case ACC_STATE.Apple: return ProviderType.SIGNIN_APPLE;
		case ACC_STATE.FaceBook: return ProviderType.FACEBOOK;
		case ACC_STATE.Guest: return ProviderType.GUEST;
		}
		return ProviderType.AUTO;
	}

	public bool IsSupportLogin(ACC_STATE eAcc)
	{
		switch(eAcc)
		{
		case ACC_STATE.Google:
		case ACC_STATE.Apple:
		case ACC_STATE.FaceBook:
		case ACC_STATE.Guest:
			return LoginType.Contains(GetLoginType(eAcc));
		}
		return false;
	}

	public void SaveLoginType(ProviderType type)
	{
		m_PlayerAccType = type;
		if(type == ProviderType.AUTO) m_PlayerInfo = null;
		PlayerPrefs.SetInt("HIVE_ACC_STATE", (int)type);
		ACC.Save_ACC(ACC_STATE.HIVE);
		// ACC.Save_ACC 안에서 호출하므로 주석
		//PlayerPrefs.SetInt("ACC_STATE", (int)ACC_STATE.HIVE);
		//PlayerPrefs.Save();
	}

	public ACC_STATE GetAccType()
	{
		if(m_PlayerInfo != null)
		{
			if(m_PlayerInfo.providerInfoData.Count() > 0)
			{
				var data = m_PlayerInfo.providerInfoData.ElementAt(0);
				switch (data.Value.providerType)
				{
				case ProviderType.GOOGLE:									return ACC_STATE.Google;
				case ProviderType.APPLE: case ProviderType.SIGNIN_APPLE:	return ACC_STATE.Apple;
				case ProviderType.FACEBOOK:									return ACC_STATE.FaceBook;
				case ProviderType.GUEST:									return ACC_STATE.Guest;
				}
			}
			return ACC_STATE.Guest;
		}

		switch (m_PlayerAccType)
		{
		case ProviderType.GOOGLE:									return ACC_STATE.Google;
		case ProviderType.APPLE: case ProviderType.SIGNIN_APPLE:	return ACC_STATE.Apple;
		case ProviderType.FACEBOOK:									return ACC_STATE.FaceBook;
		case ProviderType.GUEST:									return ACC_STATE.Guest;
		}
		return ACC_STATE.NONE;

	}

	void SelectResolve(Action<ResultAPI> CB)
	{
		AuthV4.Helper.resolveConflict((result, playerInfo) =>
		{
			MAIN.StartCoroutine(CallRenderThread(() =>
			{
				if (result.isSuccess())
				{
					if (playerInfo != null)
					{
						// 로그인 성공
						BlockCheck(() =>
						{
							SetPlayerInfo(playerInfo);
							CB?.Invoke(result);
						});
					}
					else
					{
						SetPlayerInfo(playerInfo);
						CB?.Invoke(result);
					}
				}
				else if (result.needExit())
				{
					// TODO: 앱 종료 기능을 구현하세요
					// 예) Application.Quit();
					Debug.Log("ResultAPI.needExit(): " + result.needExit() + "\n");
					MAIN.Exit();
					return;
				}
				else
				{
					playerInfo = null;
					CB?.Invoke(result);
				}
			}));
		});
	}

	public void AutoLogin(Action CB)
	{
		m_PlayerAccType = (ProviderType)PlayerPrefs.GetInt("HIVE_ACC_STATE", (int)ProviderType.AUTO);
		if (m_PlayerAccType == ProviderType.AUTO)
		{
			m_PlayerInfo = null;
			CB?.Invoke();
			return;
		}

		// Hive SDK 로그인(signIn) 시도
		AuthV4.Helper.signIn((result, playerInfo) => {

			//Helper.signIn!! result: ResultAPI { errorCode = SUCCESS, Code = Success, msg = Success.SUCCESS }
			//playerInfo: ProfileInfo {
			//	playerId = 30000109416
			//	, playerName = Epeolini4117
			//	, playerImageUrl = https://sandbox-hive-fn.qpyou.cn/hubweb/avatar_img/public/noimage.png
			//	, playerToken = f5ae832bcf18013dd2fbbb7bda9982
			//	, did = 5000117946
			//	, providerInfoData = {  }
			//	, customProviderInfoData = {  }
			//}
			Utile_Class.DebugLog($"Helper.signIn !! result : {result.toString()}\nplayerInfo : {playerInfo?.toString()}");

			MAIN.StartCoroutine(CallRenderThread(() => {
				if (result.isSuccess())
				{
					if (playerInfo != null)
					{
						// 로그인 성공
						BlockCheck(() => {
							SetPlayerInfo(playerInfo);
							CB?.Invoke();
						});
					}
					else
					{
						SetPlayerInfo(playerInfo);
						CB?.Invoke();
					}
				}
				else if (result.needExit())
				{
					// TODO: 앱 종료 기능을 구현하세요
					// 예) Application.Quit();
					Debug.Log("ResultAPI.needExit(): " + result.needExit() + "\n");
					MAIN.Exit();
					return;
				}
				else
				{
					switch (result.code)
					{
					case ResultAPI.Code.AuthV4ConflictPlayer:
						// 계정 충돌
						SelectResolve((result) =>
						{
							CB?.Invoke();
						});
						return;
					//case ResultAPI.Code.AuthV4HelperImplifiedLoginFail:
					//	// 묵시적 로그인에 실패
					//	break;
					//default:
					//	// 기타 예외 상황
					//	break;
					}
					playerInfo = null;
					CB?.Invoke();
				}
			}));
		});
	}

	/// <summary> 블럭 계정 확인 </summary>
	/// <param name="CB"></param>
	public void BlockCheck(Action CB)
	{
		// Hive SDK AuthV4 제재 유저 확인 요청
		AuthV4.checkBlacklist(true, (ResultAPI result, List<AuthV4.AuthV4MaintenanceInfo> maintenanceInfo) => {
			Utile_Class.DebugLog($"AuthV4.checkBlacklist !! result : {result.toString()}\nplayerInfo : {maintenanceInfo}");

			MAIN.StartCoroutine(CallRenderThread(() =>
			{
				if (result.isSuccess())
				{
					// 일반 유저인경우
					CB?.Invoke();
				}
				else if (result.needExit())
				{
					// TODO: 앱 종료 기능을 구현하세요.
					// 예) Application.Quit();
					Debug.Log("ResultAPI.needExit(): " + result.needExit() + "\n");
					MAIN.Exit();
					return;
				}
			}));
		});
	}

	public void Login(ACC_STATE type, Action<ResultAPI> CB)
	{
		var logintype = GetLoginType(type);

		// 선택된 ProviderType이 Google 인 경우
		AuthV4.signIn(logintype, (ResultAPI result, AuthV4.PlayerInfo playerInfo) =>
		{
			//ResultAPI { errorCode = SUCCESS, Code = Success, msg = Success. null }
			//ProfileInfo {
			//	playerId = 10099473804
			//	, playerName = Brachynomadini8872
			//	, playerImageUrl = https://hive-fn.qpyou.cn/hubweb/avatar_img/public/noimage.png
			//	, playerToken = 2bdfcad88d9dfa212f8e725bbf401f
			//	, did = 5175419503
			//	, providerInfoData = {  }
			//	, customProviderInfoData = {  } }
			Utile_Class.DebugLog($"AuthV4.signIn !! result : {result.toString()}\nplayerInfo : {playerInfo?.toString()}");
			MAIN.StartCoroutine(CallRenderThread(() =>
			{
				if (result.isSuccess() == true)
				{
					BlockCheck(() =>
					{
						SetPlayerInfo(playerInfo);
						CB?.Invoke(result);
					});
					// 인증 성공
					// playerInfo : 인증된 사용자 정보.
					//// ProviderType.GOOGLE 의 이메일 정보 조회 예시
					//Dictionary<AuthV4.ProviderType, AuthV4.ProviderInfo> providerInfoData = playerInfo.providerInfoData;
					//AuthV4.ProviderInfo providerInfo = providerInfoData[AuthV4.ProviderType.GOOGLE];
					//string email = providerInfo.providerEmail;
				}
				else
				{
					switch (result.code)
					{
					case ResultAPI.Code.AuthV4ConflictPlayer:
						// 계정 충돌
						SelectResolve((resolve_result) =>
						{
							CB?.Invoke(resolve_result);
						});
						return;
						//case ResultAPI.Code.AuthV4HelperImplifiedLoginFail:
						//	// 묵시적 로그인에 실패
						//	break;
						//default:
						//	// 기타 예외 상황
						//	break;
					}
					CB?.Invoke(result);
				}
			}));
		});
	}

	public void Logout(Action<ResultAPI> CB)
	{
		AuthV4.Helper.signOut(delegate (ResultAPI result, AuthV4.PlayerInfo playerInfo) {
			//ResultAPI { errorCode = SUCCESS, Code = Success, msg = Success.SUCCESS }
			//ProfileInfo {
			//	playerId = 10099473804
			//	, playerName = Brachynomadini8872
			//	, playerImageUrl = https://hive-fn.qpyou.cn/hubweb/avatar_img/public/noimage.png
			//	, playerToken = 2bdfcad88d9dfa212f8e725bbf401f
			//	, did = 5175419503
			//	, providerInfoData = {  }
			//	, customProviderInfoData = {  }
			//	}
			Utile_Class.DebugLog($"Helper.signOut !! result : {result.toString()}\nplayerInfo : {playerInfo?.toString()}");
			MAIN.StartCoroutine(CallRenderThread(() =>
			{
				SetPlayerInfo(null);
				//switch (result.code)
				//{
				//case ResultAPI.Code.Success:
				//	// 로그인 성공
				//	break;
				//default:
				//	// 기타 예외 상황
				//	break;
				//}
				CB?.Invoke(result);
			}));
		});
	}

	/// <summary> 계정 연동 유도 </summary>
	public void AccConnect(ACC_STATE type, Action<ResultAPI, PlayerInfo> CB)
	{
		POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		var logintype = GetLoginType(type);
		// Hive SDK AuthV4 커넥트(연동) 요청
		AuthV4.Helper.connect(logintype, delegate (ResultAPI result, AuthV4.PlayerInfo playerInfo) {
			//result: ResultAPI { errorCode = CONFLICT_PLAYER, Code = AuthV4ConflictPlayer, msg = [AuthV4 - Common] Conflict player Already connected other player }
			//playerInfo: ProfileInfo {
			//	playerId = 10099579399, playerName = , playerImageUrl = , playerToken = , did = , providerInfoData = {
			//		{ providerType = GOOGLE, providerName = GOOGLE, providerUserId = 108613178951500259226, providerEmail =  }
			//		 ,  }, customProviderInfoData = { }
			//}
			Utile_Class.DebugLog($"Helper.connect !! result : {result.toString()}\nplayerInfo : {playerInfo?.toString()}");
			MAIN.StartCoroutine(CallRenderThread(() =>
			{
				POPUP.SetConnecting(false);
				switch (result.code)
				{
				case ResultAPI.Code.Success:
					SetPlayerInfo(playerInfo);
					// 연동 성공
					break;
				}
				//WEB.SEND_REQ_ACC_INFO((res) =>
				//{
				//	if (!res.IsSuccess())
				//	{
				//		ifacc.Logout();
				//		WEB.StartErrorMsg(res.result_code);
				//		return;
				//	}

				//	if (res.UserNo > 0) m_CloaseCB?.Invoke(Item_PDA_Option.State.Select_Acc, new object[] { new RES_ACC_INFO[] { res, USERINFO.GetACCINFO() }, logintype });
				//	else USERINFO.ACC_CHANGE(type, playerInfo.playerId, () => { RefreshAuthLink(); });
				//}, ACC_STATE.HIVE, playerInfo.playerId);
				CB?.Invoke(result, playerInfo);
			}));
		});
	}

	/// <summary> 계정 연동 유도 </summary>
	public void SelectAccConnect(long selectedPlayerId, Action<ResultAPI, bool> CB)
	{
		// Hive SDK AuthV4 계정 충돌 선택 요청
		AuthV4.selectConflict(selectedPlayerId, (ResultAPI result, AuthV4.PlayerInfo playerInfo) => {

			//ResultAPI { errorCode = SUCCESS, Code = Success, msg = Success.SUCCESS }
			//playerInfo: ProfileInfo {
			//	playerId = 10099579984
			//	, playerName = 오대호
			//	, playerImageUrl = https://hive-fn.qpyou.cn/hubweb/hive_img/profile/U/2023/08/12/22/10099579984_1f19309882bb0fe2b9e62694b716cb2c.jpg
			//	, playerToken = c2872062ca059428565a8fedb141c4, did = 5175563876
			//	, providerInfoData = {  { providerType = GOOGLE, providerName = GOOGLE, providerUserId = 108613178951500259226, providerEmail =  }
			//	,  }, customProviderInfoData = { }
			//}
			Utile_Class.DebugLog($"AuthV4.selectConflict !! result : {result.toString()}\nplayerInfo : {playerInfo?.toString()}");
			MAIN.StartCoroutine(CallRenderThread(() =>
			{
				if (result.isSuccess())
				{
					// 계정 선택 성공
					var beforPlayerID = m_PlayerInfo.playerId;
					SetPlayerInfo(playerInfo);
					MAIN.ReStart();
					CB?.Invoke(result, beforPlayerID == m_PlayerInfo.playerId);
				}
				else if (result.needExit())
				{
					// TODO: 앱 종료 기능을 구현하세요
					// 예) Application.Quit();
					Debug.Log("ResultAPI.needExit(): " + result.needExit() + "\n");
					MAIN.Exit();
					return;
				}
				CB?.Invoke(result, false);
			}));
		});

	}
}

