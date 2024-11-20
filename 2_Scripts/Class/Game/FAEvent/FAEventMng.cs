
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class FAEventMng : ClassMng
{
	// 데이터 하루단위 체크
	public DateTime UTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	// 자동 갱신 시간 체크
	public double ATime = 0;

	// 이벤트 데이터
	public List<MyFAEvent> Datas = new List<MyFAEvent>();

	// 이벤트 데이터 로드중 체크
	public bool IsLoading = false;


	public void SetDATA(List<RES_MY_FAEVENT_INFO> data)
	{
		for (int i = data.Count - 1; i > -1; i--)
		{
			SetDATA(data[i]);
		}
	}
	public void SetDATA(RES_MY_FAEVENT_INFO data)
	{
		if (data == null) return;
		MyFAEvent info = Datas.Find(o => o.UID == data.UID);
		if (info == null)
		{
			info = new MyFAEvent();
			Datas.Add(info);
		}
		info.SetDATA(data);

		if (info.IsRemoveEvent()) Datas.Remove(info);
	}

	public bool IsDataUp()
	{
		return !Utile_Class.IsSameDay(UTime);
	}

	public List<MyFAEvent> GetViewAttEvent()
	{
		return Datas.FindAll(o => !string.IsNullOrEmpty(o.Prefab) && (o.Type == FAEventType.New_Attendance || o.Type == FAEventType.Rot_Attendance || o.Type == FAEventType.ReturnUser_Attendance));
	}

	public bool IsCheck()
	{
		if (Datas.Count < 1) return false;
		return Datas.Any(o => o.IsViewCheck());//보여줘야 하는 애들 && 보상 있는 
	}

	public bool IsCheckAutoTime()
	{
#if NOT_USE_NET
		return false;
#else
		// 하루단위 체크는 플레이 화면 들어가면서하므로 패스
		if (IsDataUp()) return false;
		// 10분간격 또는 받을 보상이 있을때
		return UTILE.Get_ServerTime() - ATime > 600 || Datas.Any(o => o.IsAutoCheck());
#endif
	}

	public void AutoCheck()
	{
		if (!IsCheckAutoTime()) return;
		// 통신이 가능한 상태가 아니면 패스
		if (!WEB.IS_SendNet()) return;
		// 호출 시점들로인해 2중호출되어 보상이 2번 받아지는 현상이 있음
		Load((res) => {
			// 받은 내용중 알림이 있으면 알려준다.
			if (res.InitPost?.Count > 0)
			{
				// 추후 시스템 알림을 알려주는게 있으면 넣어주기
				// ex) 이벤트 타입이 FAEventType.ConnectTime 이놈일때
				// info 정보에 Push메세지 있음 언어별로 사용 단 value[0] > 0일때 넣어줌
				// 서버에서는 다음 호출때는 해당 정보가 삭제됨
			}
		});
	}

	public void Load(Action<RES_ALL_MY_FAEVENT_INFO> EndCB, bool _refresh = true)
	{
		if (IsLoading) return;
		IsLoading = true;
		WEB.SEND_REQ_MY_FAEVENT_INFO((res) => {
			IsLoading = false;
			EndCB?.Invoke(res);
		}, _refresh);
	}
}


