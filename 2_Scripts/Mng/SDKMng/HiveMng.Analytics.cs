using hive;
using System;
using System.Collections.Generic;
using UnityEngine;
using static hive.AuthV4;

public partial class HiveMng : ClassMng
{
	public void Analytics_StartCDN()
	{
		Analytics.sendUserEntryFunnelsLogs("700", null);
	}
	public void Analytics_EndCDN()
	{
		Analytics.sendUserEntryFunnelsLogs("800", null);
	}

	public void Analytics_TutorialComplete()
	{
		// 3-1 종료
		Analytics.sendEvent("TutorialComplete");
	}

	public void Analytics_Stage401()
	{
		// 3-1 종료
		Analytics.sendEvent("Stage4-1");
	}

	public void Analytics_AdView()
	{
		// 3-1 종료
		Analytics.sendEvent("AdView");
	}

	public void Analytics_ad_reward()
	{
		// 광고 보상 수령 시점에 체크
		Analytics.sendEvent("ad_reward");
	}

	public void Analytics_add_to_cart()
	{
		// 캐쉬 상품 구매 시도 후 결제 완료까지 도달하지 못했을 시 체크
		Analytics.sendEvent("add_to_cart");
	}

	public void Analytics_FirstOpen()
	{
		if (PlayerPrefs.GetInt("FirstOpen", 0) == 1) return;
		Analytics.sendEvent("first_open");
		PlayerPrefs.SetInt("FirstOpen", 1);
		PlayerPrefs.Save();
	}

	void Send_Analytics(JSONObject data)
	{
		Analytics.sendAnalyticsLog(data);
	}
}

