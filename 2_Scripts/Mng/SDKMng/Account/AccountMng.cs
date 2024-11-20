using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum ACC_STATE
{
	/// <summary> 연동 없음 </summary>
	NONE = -1,
	/// <summary> 게스트 </summary>
	Guest = 0,
	/// <summary> 페이스북 </summary>
	FaceBook,
	/// <summary> Google (Android) </summary>
	Google,
	/// <summary> Apple(iOS 게임센터는 사라짐) </summary>
	Apple,
	/// <summary> Google Play Game Service </summary>
	GPGS,
	/// <summary> 하이브에 연결(하이브 SDK에서 idp확인) </summary>
	HIVE,
	END
}

public abstract class IFAccount
{
	public const int IDLE							= 0x0000;	// 대기중
	public const int SUCCESS						= 0x0001;	// 성공
	public const int ERROR_LOGIN					= 0xFFFF;	// 로그인 실패
	public const int ERROR_INIT						= 0xFFFE;	// 초기화 실패
	public const int ERROR_NOT_LOAD_USER_INFO		= 0xFFFD;	// 유저 정보 로드 실패
	public const int ERROR_OS_VERSION				= 0xFFFC;	// OS 업데이트 필요
	public const int ERROR_CANCEL					= 0xFFFB;	// 취소함
	public const int ERROR_NOT_SUPPORT				= 0xFFFA;   // 지원하지 않음

	/// <summary> 로그인 </summary>
	public abstract void Login(Action<int, string> cb = null);
	/// <summary> 로그아웃 </summary>
	public abstract void Logout(Action<int> cb = null);

	/// <summary> 유저 ID </summary>
	public abstract string GetID();
	/// <summary> 유저 Email </summary>
	public abstract string GetEmail();
	/// <summary> 유저 이름 </summary>
	public abstract string GetName();
	/// <summary> 유저 프로필 이미지 </summary>
	public abstract void GetImage(Action<Texture2D> CB);
	/// <summary> 유저가 미성년자인가? </summary>
	public abstract bool IS_Underage();
}

public class AccountMng : ClassMng
{
	ACC_STATE _LoginType;
	/// <summary> 접속 상태 </summary>
	public ACC_STATE LoginType { get {
			switch(_LoginType)
			{
			case ACC_STATE.HIVE:	return HIVE.GetAccType();
			}
			return _LoginType;
	} }

	IFAccount[] m_pAccs = new IFAccount[(int)ACC_STATE.END];
	IFAccount m_pLogin { get { return m_pAccs[(int)LoginType]; } }

	public AccountMng()
	{
		_LoginType = (ACC_STATE)PlayerPrefs.GetInt("ACC_STATE", (int)ACC_STATE.NONE);
	}

	/// <summary> 로그인 </summary>
	public void Login(ACC_STATE eState, Action<int, string> cb)
	{
		if (!IsSupport(eState))
		{
			cb?.Invoke(IFAccount.ERROR_NOT_SUPPORT, MainMng.Instance.m_ToolData.GetString(460));
			return;
		}
		if (m_pAccs[(int)eState] == null)
		{
			//switch(eState)
			//{
			//case ACC_STATE.Guest:		m_pAccs[(int)eState] = new GuestLogin();			break;
			//case ACC_STATE.FaceBook:	m_pAccs[(int)eState] = new FacebookLogin();			break;
			//case ACC_STATE.Google:		m_pAccs[(int)eState] = new GoogleLogin();			break;
			//case ACC_STATE.Apple:		m_pAccs[(int)eState] = new AppleLogin();			break;
			//default: cb?.Invoke(IFAccount.ERROR_LOGIN, MainMng.Instance.m_ToolData.GetString(460)); return;
			//}

		}
		m_pAccs[(int)eState].Login(cb);
	}

	public void Save_ACC(ACC_STATE eState)
	{
		PlayerPrefs.SetInt("ACC_STATE", (int)eState);
		PlayerPrefs.Save();
		_LoginType = eState;
	}

	public IFAccount GetIFAcc(ACC_STATE eState)
	{
		return m_pAccs[(int)eState];
	}

	public bool IsSupport(ACC_STATE eAcc)
	{
		return MainMng.Instance.HIVE.IsSupportLogin(eAcc);
		//switch (eAcc)
		//{
		//case ACC_STATE.Guest: return GuestLogin.IsSupport;
		//case ACC_STATE.FaceBook: return FacebookLogin.IsSupport;
		//case ACC_STATE.Google: return GoogleLogin.IsSupport;
		//case ACC_STATE.Apple: return AppleLogin.IsSupport;
		//}
		//return false;
	}

	/// <summary> 연동 이름 </summary>
	public string AccName(ACC_STATE eAcc = ACC_STATE.NONE)
	{
		switch (eAcc)
		{
		case ACC_STATE.FaceBook: return "Facebook";
		case ACC_STATE.Google: return "Google";
		case ACC_STATE.Apple: return "Apple";
		}
		return "Guest";
	}

	/// <summary> 로그아웃 </summary>
	public void Logout(Action<int> cb = null, ACC_STATE eAcc = ACC_STATE.NONE, bool IsGuestCheck = false)
	{
		if(IsGuestCheck && LoginType == ACC_STATE.Guest)
		{
			HIVE.SetPlayerInfo(null);
			cb?.Invoke(IFAccount.SUCCESS);
			return;
		}

		HIVE.Logout((result) => { cb?.Invoke(IFAccount.SUCCESS); });
		//switch (eAcc)
		//{
		//case ACC_STATE.NONE:
		//	if(LoginType != ACC_STATE.NONE) m_pLogin?.Logout(cb);
		//	break;
		//default:
		//	m_pAccs[(int)eAcc]?.Logout(cb);
		//	break;
		//}
		//cb?.Invoke(IFAccount.SUCCESS);
	}

	/// <summary> 유저 ID </summary>
	public string GetID(ACC_STATE eAcc = ACC_STATE.NONE)
	{
		return HIVE.GetPlayerID().ToString();
	}
	/// <summary> 유저 Email </summary>
	public string GetEmail(ACC_STATE eAcc = ACC_STATE.NONE)
	{
		// 사용 안함
		return "";
		//if (eAcc == ACC_STATE.NONE && m_pLogin != null) return m_pLogin.GetEmail();
		//else if(m_pAccs[(int)eAcc] != null) return m_pAccs[(int)eAcc].GetEmail();
		//return "";
	}
	/// <summary> 유저 이름 </summary>
	public string GetName(ACC_STATE eAcc = ACC_STATE.NONE)
	{
		// 사용 안함
		return "";
		//if(eAcc == ACC_STATE.NONE && m_pLogin != null) return m_pLogin.GetName();
		//else if(m_pAccs[(int)eAcc] != null) return m_pAccs[(int)eAcc].GetName();
		//return "";
	}
	/// <summary> 유저 프로필 이미지 </summary>
	public void GetImage(Action<Texture2D> cb = null, ACC_STATE eAcc = ACC_STATE.NONE)
	{
		if (eAcc == ACC_STATE.NONE && m_pLogin != null) m_pLogin.GetImage(cb);
		else if (m_pAccs[(int)eAcc] != null) m_pAccs[(int)eAcc].GetImage(cb);
		else cb?.Invoke(new Texture2D(1, 1));
	}
	/// <summary> 유저가 미성년자인가? </summary>
	public bool IS_Underage(ACC_STATE eAcc = ACC_STATE.NONE)
	{
		// 사용 안함
		return false;
		//if (eAcc == ACC_STATE.NONE && m_pLogin != null) return m_pLogin.IS_Underage();
		//else if(m_pAccs[(int)eAcc] != null) return m_pAccs[(int)eAcc].IS_Underage();
		//return false;
	}
}
