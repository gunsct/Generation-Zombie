using System;
using System.Collections;
using UnityEngine;

public abstract class IAds {
	public enum ResultCode
	{
		None,
		Succecss,
		LoadFail,
		ShowFail
	}
	public Action<ResultCode> m_CB;
	public Action<bool> m_EndCheck;
	public abstract void ShowAds(Action<ResultCode> _cb, Action<bool> _endcheck);

	public abstract void RewardEnd();
}
public class AdsMng : ClassMng
{
	public IAds m_Ads; 
	public bool Is_Show = false;
	bool m_IsEndCall;
	public AdsMng() {
		m_Ads = new GoogleAds();
	}

	public void ShowAds(Action<IAds.ResultCode> _cb) {
		if(USERINFO.m_ShopInfo.IsPassBuy())
		{
			_cb?.Invoke(IAds.ResultCode.Succecss);
			return;
		}
#if NOTUSE_GOOGLE_ADS
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, "Coming Soon");
		return;
#else
		if (Is_Show) return;
		Is_Show = true;
		IAds.ResultCode Result = IAds.ResultCode.None;
#if UNITY_EDITOR
		// 에디터상에서 폰과 달리 결과가 나중에 호출됨 종료가 먼저 되었느냐에따라 보상호출 위치 셋팅
		// 보상을 안주는 경우도 있으므로
		m_IsEndCall = false;
#endif
		POPUP.LockConnecting(true);
		//POPUP.SetConnecting(true, UIMng.ConnectingTrigger.Now);
		m_Ads.ShowAds((res) =>
		{
			Result = res;
#if UNITY_EDITOR
			if(m_IsEndCall) MainMng.Instance.StartCoroutine(EndCallCB(Result, _cb));
#endif
		}, (end) =>
		{
			HIVE.Analytics_AdView();
			Is_Show = false;
#if UNITY_EDITOR
			m_IsEndCall = true;
#endif
			MainMng.Instance.StartCoroutine(EndCallCB(Result, _cb));
		});
#endif
	}

	IEnumerator EndCallCB(IAds.ResultCode result, Action<IAds.ResultCode> _cb)
	{
		// 랜더링 프로세스 맞추기 위해 한프레임 쉬어줌
		yield return new WaitForEndOfFrame();
		POPUP.LockConnecting(false);
		//POPUP.SetConnecting(false);
		Utile_Class.DebugLog($"AdReward callback result : {result}");
		switch (result) {
			case IAds.ResultCode.None:
				MainMng.Instance.POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, MainMng.Instance.TDATA.GetString(834));
				break;
			case IAds.ResultCode.Succecss:
			case IAds.ResultCode.LoadFail:
			case IAds.ResultCode.ShowFail:
				_cb?.Invoke(result);
				break;
				//case IAds.ResultCode.LoadFail:
				//case IAds.ResultCode.ShowFail:
				//	MainMng.Instance.POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, MainMng.Instance.TDATA.GetString(834));
				//	break;
				//default:
				//	_cb?.Invoke(result);
				//	break;
		}
	}
}
