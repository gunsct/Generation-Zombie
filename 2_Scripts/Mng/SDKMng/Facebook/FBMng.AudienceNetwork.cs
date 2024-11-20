//using AudienceNetwork;
//using hive;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using static hive.AuthV4;

public partial class FBMng : ClassMng
{
	void Init_AudienceNetwork()
	{
//#if !UNITY_EDITOR
//		AudienceNetworkAds.Initialize();
//		SetAdvertiserTracking(true);
//#endif
	}

	void SetAdvertiserTracking(bool Active)
	{
//#if !UNITY_EDITOR && UNITY_IOS
//		AdSettings.SetAdvertiserTrackingEnabled(Active);
//#endif
	}
}

