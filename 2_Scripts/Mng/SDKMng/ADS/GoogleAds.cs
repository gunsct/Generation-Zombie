using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static hive.HIVEAdKit;

public class GoogleAds : IAds
{
	RewardedAd m_RewardedAd;
	AdRequest m_Request;
	ResultCode m_Result = 0;	//1:성공,2:유저가 광고 종료,3:광고 로드 실패,4:광고 보여주기 실패

	private string adSessionKey;
	protected string adPlacementInfo;
	// HIVEAnalyticsLogManager manager = new HIVEAnalyticsLogManager();

#pragma warning disable 0414
	private static string adNetwork = "admob";
#pragma warning restore 0414

	//#if UNITY_EDITOR || NOT_USE_NET || TEST_SERVER || DEV_SERVER || OUT_SERVER || LOCAL_SERVER || TEST_AD
	//#   if UNITY_IOS || UNITY_IPHONE
	//		string adid = "ca-app-pub-3940256099942544/1712485313";
	//#   else
	//		string adid = "ca-app-pub-3940256099942544/5224354917";
	//#   endif
	//#else
	//#	if GRABITYUS
	//#		if UNITY_IOS || UNITY_IPHONE
	//			string adid = "ca-app-pub-5524627796340129~7802130090";
	//#		else
	//			string adid = "ca-app-pub-5524627796340129~3853202918";
	//#		endif
	//#	else
	//#		if UNITY_IOS || UNITY_IPHONE
	//			//string adid = "ca-app-pub-9708838453844124~6715387245";
	//			string adid = "ca-app-pub-9708838453844124/2584570547";
	//#		else
	//			//string adid = "ca-app-pub-9708838453844124~5451510687";
	//			string adid = "ca-app-pub-9708838453844124/7108223199";
	//#		endif
	//#	endif
	//#endif

	// Reward ad ID 넣을것 AppID는 유니티 설정에서만 넣을것
#if GRABITYUS
#	if UNITY_IOS || UNITY_IPHONE
	readonly string[] reward_ad_id = new string[] { "ca-app-pub-3940256099942544/1712485313", "ca-app-pub-5524627796340129/1427956065" };
#else
	readonly string[] reward_ad_id = new string[] { "ca-app-pub-3940256099942544/5224354917", "ca-app-pub-5524627796340129/2005483362" };
#endif
#else
#if UNITY_IOS || UNITY_IPHONE
	readonly string[] reward_ad_id = new string[] { "ca-app-pub-3940256099942544/1712485313", "ca-app-pub-5524627796340129~7802130090" };
#else
	readonly string[] reward_ad_id = new string[] { "ca-app-pub-3940256099942544/5224354917", "ca-app-pub-5524627796340129~3853202918" };
#endif
//#	if UNITY_IOS || UNITY_IPHONE
//	readonly string[] reward_ad_id = new string[] { "ca-app-pub-3940256099942544/1712485313", "ca-app-pub-9708838453844124~6715387245" };
//#	else
//	readonly string[] reward_ad_id = new string[] { "ca-app-pub-3940256099942544/5224354917", "ca-app-pub-9708838453844124~5451510687" };
//#	endif
#endif

	public GoogleAds()
	{
		MobileAds.Initialize(initStatus => { });
		//adSessionKey = manager.CreateUniqueSessionKey();

		//m_RewardedAd = new RewardedAd(adid);
		//// Called when an ad request has successfully loaded.
		//m_RewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
		//// Called when an ad request failed to load.
		//m_RewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
		//// Called when an ad is shown.
		//m_RewardedAd.OnAdOpening += HandleRewardedAdOpening;
		//// Called when an ad request failed to show.
		//m_RewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
		//// Called when the user should be rewarded for interacting with the ad.
		//m_RewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
		//// Called when the ad is closed.
		//m_RewardedAd.OnAdClosed += HandleRewardedAdClosed;

		//m_RewardedAd.OnPaidEvent += HandlePaidEvent;
		//m_RewardedAd.OnAdClicked += HandleAdClick;

		// Create an empty ad request.
		//[Obsolete("Use AdRequest directly instead.")]
		//m_Request = new AdRequest.Builder().Build();
	}

	/// <summary>
	/// Destroys the ad.
	/// </summary>
	public void DestroyAd()
	{
		if (m_RewardedAd != null)
		{
			Debug.Log("Destroying rewarded ad.");
			m_RewardedAd.Destroy();
			m_RewardedAd = null;
		}
	}

	public override void ShowAds(Action<ResultCode> _cb, Action<bool> _endcheck) {
		m_CB = _cb;
		m_EndCheck = _endcheck;
		// Load the rewarded ad with the request.
		//m_RewardedAd.LoadAd(m_Request);

		if (m_RewardedAd != null) DestroyAd();

		//var adRequest = new AdRequest.Builder().Build();
		var adRequest = new AdRequest();
		var mode = int.Parse(MainMng.Instance.WEB.GetConfig(EServerConfig.ADsMode));
		var reward_ad_id = this.reward_ad_id[mode];
		//var data = ;
		// send the request to load the ad.
		RewardedAd.Load(reward_ad_id, adRequest,
			(RewardedAd ad, LoadAdError error) =>
			{
				// if error is not null, the load request failed.
				if (error != null || ad == null)
				{
					Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
					HandleRewardedAdFailedToLoad(error);
					return;
				}

				Debug.Log("Rewarded ad loaded with response : "
						  + ad.GetResponseInfo());

				m_RewardedAd = ad;
				m_RewardedAd.OnAdFullScreenContentOpened += HandleRewardedAdOpening;
				m_RewardedAd.OnAdFullScreenContentFailed += HandleRewardedAdFailedToShow;
				m_RewardedAd.OnAdPaid += HandlePaidEvent;
				m_RewardedAd.OnAdFullScreenContentClosed += HandleRewardedAdClosed;
				m_RewardedAd.OnAdClicked += HandleAdClick;

				//// Raised when an impression is recorded for an ad.
				//m_RewardedAd.OnAdImpressionRecorded += () =>
				//{
				//    Debug.Log("Rewarded ad recorded an impression.");
				//};
				HandleRewardedAdLoaded();
			});
	}

	public override void RewardEnd()
	{
		//manager.AdReward(adid, adSessionKey, adNetwork, adPlacementInfo, "", 0L);
	}

	void ShowAD()
	{
		const string rewardMsg =
			"Rewarded ad rewarded the user. Type: {0}, amount: {1}.";
		if (m_RewardedAd != null && m_RewardedAd.CanShowAd())
		{
			m_RewardedAd.Show((Reward reward) =>
			{
				HandleUserEarnedReward(reward);
				//보상 획득하기
				Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
			});
		}
		else
		{
			ShowAds(m_CB, m_EndCheck);
		}
	}

	/// <summary> 광고 로드가 완료될 때 실행됩니다. </summary>
	public void HandleRewardedAdLoaded()
	{
		Utile_Class.DebugLog("AdLoaded event received");
		//manager.AdLoaded(adid, adSessionKey, adNetwork, adPlacementInfo);
		ShowAD();
		//m_RewardedAd.Show();
	}

	/// <summary> 광고가 표시될 때 실행되며 기기 화면을 덮습니다. 이때 필요한 경우 앱의 오디오 출력 또는 게임 루프를 일시중지하는 것이 좋습니다. </summary>
	public void HandleRewardedAdOpening()
	{
		Utile_Class.DebugLog("AdOpening event received");
		//manager.AdOpening(adid, adSessionKey, adNetwork, adPlacementInfo);
		MainMng.Instance.SND.AllMute(true);
	}
	/// <summary> 사용자가 닫기 아이콘을 탭하거나 뒤로 버튼을 사용하여 보상형 동영상 광고를 닫을 때 실행됩니다. 앱에서 오디오 출력 또는 게임 루프를 일시중지했을 때 이 메소드로 재개하면 편리합니다. </summary>
	public void HandleRewardedAdClosed() {
		MainMng.Instance.SND.AllMute(false);
		Utile_Class.DebugLog("AdClosed event received");
		//manager.AdClosed(adid, adSessionKey, adNetwork, adPlacementInfo);
		m_EndCheck?.Invoke(m_Result == ResultCode.Succecss);
	}

	/// <summary> 사용자가 동영상 시청에 대한 보상을 받아야 할 때 실행됩니다. Reward 매개변수는 사용자에게 제공되는 보상을 설명합니다. </summary>
	//public void HandleUserEarnedReward(AdValue args)
	public void HandleUserEarnedReward(Reward reward)
	{
		Utile_Class.DebugLog("AdReward event received : " + reward.ToString());
		//manager.PaidEvent(adid, adSessionKey, adNetwork, reward.Amount);
		//manager.AdReward(adid, adSessionKey, adNetwork, adPlacementInfo, "", 0L);
		//string type = args.Type;
		//double amount = args.Amount;

		m_Result = ResultCode.Succecss;
		m_CB?.Invoke(m_Result);
	}


	/// <summary> 광고 로드에 실패할 때 실행됩니다. 제공된 AdErrorEventArgs의 Message 속성은 발생한 실패의 유형을 설명합니다. </summary>
	public void HandleRewardedAdFailedToLoad(LoadAdError error) {
		Utile_Class.DebugLog("AdFailedToLoad event received with errorCode : " + error.ToString());
		//manager.AdFailedToLoad(adid, adSessionKey, adNetwork);
		//MainMng.Instance.POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, MainMng.Instance.TDATA.GetString(834));
		m_Result = ResultCode.LoadFail;
		m_CB?.Invoke(m_Result);
		m_EndCheck?.Invoke(false);
	}
	/// <summary> 광고 표시에 실패할 때 실행됩니다. 제공된 AdErrorEventArgs의 Message 속성은 발생한 실패의 유형을 설명합니다. </summary>
	public void HandleRewardedAdFailedToShow(AdError error)
	{
		//MainMng.Instance.POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, MainMng.Instance.TDATA.GetString(834));
		Utile_Class.DebugLog("AdFailedToShow event received : " + error.ToString());
		//manager.AdFailedToShow(adid, adSessionKey, adNetwork);
		m_Result = ResultCode.ShowFail;
		m_CB?.Invoke(m_Result);
		m_EndCheck?.Invoke(false);
	}

	public void HandleAdClick()
	{
		Utile_Class.DebugLog("AdClick event received");
		//manager.AdClick(adid, adSessionKey, adNetwork, adPlacementInfo);
	}

	/// <summary>  </summary>
	public void HandlePaidEvent(AdValue args)
	{
		Utile_Class.DebugLog("PaidEvent event received ecpm : " + args.ToString());
		//manager.PaidEvent(adid, adSessionKey, adNetwork, (double)args.Value);
		MainMng.Instance.SND.AllMute(false);
		m_EndCheck?.Invoke(false);
	}
}
