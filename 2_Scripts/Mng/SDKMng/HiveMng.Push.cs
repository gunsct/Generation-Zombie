using hive;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static hive.AuthV4;
using static ToolData;

public partial class HiveMng : ClassMng
{
	public enum PushMode
	{
		Normal = 0,
		Night,
	}
	RemotePush m_remotePush;
	public void GetPushInfo()
	{
		hive.Push.getRemotePush(onRemotePushCB);
	}

	public void SetShowActivePlayPush(bool useForegroundRemotePush, bool useForegroundLocalPush)
	{
		PushSetting pushSetting = new PushSetting(useForegroundRemotePush, useForegroundLocalPush);
		// 앱 활성화시 알림 수신 여부 설정
		Push.setForegroundPush(pushSetting, onPushSettingCB);

	}
	public void onPushSettingCB(ResultAPI result, PushSetting setting)
	{
		if (result.isSuccess() == true)
		{
			// TODO: pushSetting 내용 확인
		}
	}

	public void SetPush(PushMode Mode, bool IsActive)
	{
		if (m_remotePush == null) return;
		switch(Mode)
		{
		case PushMode.Normal: m_remotePush.isAgreeNotice = IsActive; break;
		case PushMode.Night: m_remotePush.isAgreeNight = IsActive; break;
		}
		
		hive.Push.setRemotePush(m_remotePush, onRemotePushCB);
	}

	// 리모트 푸시 설정 조회 결과 콜백 핸들러
	public void onRemotePushCB(ResultAPI result, RemotePush remotePush)
	{
		//result: ResultAPI { errorCode = SUCCESS, Code = Success, msg = Success. null }
		//remotePush: RemotePush { isAgreeNotice = True, isAgreeNight = True }
		Utile_Class.DebugLog($"getRemotePush result : {result.toString()}\nremotePush : {remotePush.toString()}");
		m_remotePush = remotePush;
	}

	/// <summary> 푸시 수신 상태 </summary>
	/// <param name="Mode">0:유저가 공지 알림 수신을 허용했는지를 나타냄.(마케팅 광고 푸시 수신 여부), 1:유저가 야간 알림 수신을 허용했는지를 나타냄.</param>
	/// <returns></returns>
	public bool IsPush(PushMode Mode)
	{
		if (m_remotePush == null) return true;
		switch(Mode)
		{
		case PushMode.Night: return m_remotePush.isAgreeNight;
		}
		return m_remotePush.isAgreeNotice;
	}


	public enum LocalPushMode
	{
		Making = 1,
		Energy,
		SupplyBox,
		Pass,
		Max
	}

	int GetLocalPushID(LocalPushMode mode)
	{
		return (int)mode;//(int)GetAccType() * 100 + (int)mode;
	}

	public void ClearLocalPush()
	{
		List<int> noticeIds = new List<int>();
		for (LocalPushMode i = 0; i < LocalPushMode.Max; i++) noticeIds.Add(GetLocalPushID(i));
		hive.Push.unregisterLocalPushes(noticeIds);
	}

	public void SetLocalPush(LocalPushMode Mode, string title, string msg, long addsec)
	{
		if (string.IsNullOrWhiteSpace(msg)) return;
		int ID = GetLocalPushID(Mode);
		// 기존 등록 아이디 해제
		hive.Push.unregisterLocalPush(ID);

		// 로컬 푸시 설정
		LocalPush localPush = new LocalPush();
		localPush.noticeId = ID;			// 푸시 알림을 위한 고유 ID. 같은 ID 값을 가진 알림을 받으면 기존에 수신한 푸시 알림은 알림센터에서 사라진다.
		localPush.title = title;			// 푸시 알림 제목
		localPush.msg = msg;				// 푸시 알림 메시지
		localPush.after = (long)addsec;     // 푸시 알림 등록 후 유저에게 노출되는 시점.
		localPush.groupId = "game";         // 푸시 알림 그룹
		localPush.bucketsize = 1;
		
		// 로컬 푸시 등록하기
		hive.Push.registerLocalPush(localPush, (ResultAPI result, LocalPush localPush) => {
			if (result.isSuccess())
			{
				// 로컬 푸시 등록 성공
			}
		});
	}

	public void Check_Loaclpush_Making() {
		// making 상태
		var makings = USERINFO.m_Making.FindAll(o => !o.IS_Complete());
		var making = makings.Count > 0 ? makings.Max(o => o.GetRemainTime()) : 0;
		if (making > 1) SetLocalPush(LocalPushMode.Making, TDATA.GetString(StringTalbe.Push, 1), TDATA.GetString(StringTalbe.Push, 2), (long)making);
	}
	public void Check_Loaclpush_Energy()
	{
		// 총알 충전
		USERINFO.m_Energy.CalcCnt();
		var energy = USERINFO.m_Energy.GetMaxRemainTime();
		if (energy > 1) SetLocalPush(LocalPushMode.Energy, TDATA.GetString(StringTalbe.Push, 3), TDATA.GetString(StringTalbe.Push, 4), (long)energy);
	}

	public void AutoCheck_LocalPush()
	{
		if (!MainMng.IsValid()) return;
		if (USERINFO == null || USERINFO.m_UID == 0) return;
		hive.Push.unregisterAllLocalPushes();
		// making 상태
		Check_Loaclpush_Making();

		// 총알 충전
		Check_Loaclpush_Energy();

		if(USERINFO.m_ShopInfo != null && USERINFO.m_ShopInfo.BUYs.Count > 0)
		{
			// 보급상자
			var buyinfo = USERINFO.m_ShopInfo.BUYs.GroupBy(o => TDATA.GetShopTable(o.Idx).m_Group).ToDictionary(o => o.Key, o => o.ToList());
			if (buyinfo.ContainsKey(ShopGroup.SupplyBox))
			{
				var supply = buyinfo[ShopGroup.SupplyBox].Find(o => TDATA.GetShopTable(o.Idx).m_PriceType == PriceType.Time);
				if (supply != null)
				{
					var tdata = TDATA.GetShopTable(supply.Idx);
					var gap = ((supply.UTime + tdata.GetPrice(1) * 60000L) - UTILE.Get_ServerTime_Milli()) * 0.001d;
					if (gap > 1) SetLocalPush(LocalPushMode.SupplyBox, TDATA.GetString(StringTalbe.Push, 5), TDATA.GetString(StringTalbe.Push, 6), (long)gap);
				}
			}

			// 패스 종료
			if (USERINFO.m_ShopInfo.IsPassBuy())
			{
				var pass = USERINFO.m_ShopInfo.PlayPass();
				var gap = (pass.Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001d;
				if (gap > 1) SetLocalPush(LocalPushMode.Pass, TDATA.GetString(StringTalbe.Push, 13), TDATA.GetString(StringTalbe.Push, 14), (long)gap);
			}
		}

		// 챌린지 시작, 종료 : 신규 일반 주간 푸시 겹침 문제로 인해 제거


	}
}

