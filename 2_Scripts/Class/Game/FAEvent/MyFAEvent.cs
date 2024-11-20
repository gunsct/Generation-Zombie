
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class MyFAEvent : ClassMng
{
	/// <summary> 유저 이벤트 고유번호(서버로는 유저 이벤트 고유 번호로 전송) </summary>
	public long UID;
	/// <summary> 이벤트 고유번호(미션 체크에서 필요해서 넣어줌) </summary>
	public long EventUID;
	/// <summary> 이벤트 이름 </summary>
	public int Title;
	/// <summary> 정보 </summary>
	public int Msg;
	/// <summary> 적용 프리팹이름 </summary>
	public string Prefab;

	/// <summary> 이벤트 타입 </summary>
	public LS_Web.FAEventType Type;
	/// <summary> 이벤트 정보 JSONObject </summary>
	public string Info;
	/// <summary> 이벤트 타입의 검색용 값들 </summary>
	public List<long> TypeValues;

	/// <summary> 각 이벤트에 대한 값들
	/// <para> EventType </para>
	/// <para> New_Attendance => 0:진행한 회차 </para>
	/// <para> Rot_Attendance => 0:진행한 회차 </para>
	/// <para> FirstPurchase => 구매한 아이템들 </para>
	/// <para> Stage_Minigame => 0 : 진행 스테이지 레벨, 1 : 리롤 카운트 </para>
	/// <para> GrowUP => 0 : 진행 스테이지 레벨, 1 : 리롤 카운트, 2 : 칠면조 레벨, 3 : 칠면조 Exp </para>
	/// <para> OpenEvnet => 사용 안함 </para>
	/// </summary>
	public List<long> Values;

	/// <summary> 0:시작시간, 1:종료시간, 2:업데이트 시간 </summary>
	public long[] Times;

	public object RealData;

	public double GetRemainEndTime() {
		return Times[1] - UTILE.Get_ServerTime_Milli();
	}
	public void SetDATA(RES_MY_FAEVENT_INFO data)
	{
		UID = data.UID;
		EventUID = data.EventUID;
		Title = data.Title;
		Msg = data.Msg;
		Prefab = data.Prefab;
		Type = data.Type;
		Info = data.Info;
		Values = data.Values;
		Times = data.Times;
		RealData = null;
		// 다시 체크해야되는 시간인지 확인
		switch (Type)
		{
		case FAEventType.New_Attendance:
			RealData = GetRealInfo<List<FAEventData_Attendance>>();
			break;
		case FAEventType.Rot_Attendance:
			RealData = GetRealInfo<FAEventData_Rot_Attendance>();
			break;
		case FAEventType.FirstPurchase:
			RealData = GetRealInfo<FAEventData_FirstPurchase>();
			break;
		case FAEventType.ReturnUser_Attendance:
			RealData = GetRealInfo<FAEventData_ReturnUser_Attendance>();
			break;
		case FAEventType.Stage_Minigame:
			RealData = GetRealInfo<FAEventData_Stage_Minigame>();
			break;
		case FAEventType.GrowUP:
			RealData = GetRealInfo<FAEventData_GrowUP>();
			break;
		case FAEventType.OpenEvent:
			RealData = GetRealInfo<FAEventData_OpenEvent>();
			break;
		}
	}

	public T GetRealInfo<T>() where T : class
	{
		if (RealData != null) return (T)RealData;
		return JsonConvert.DeserializeObject<T>(Info);
	}

	public string GetTitle()
	{
		return TDATA.GetString(ToolData.StringTalbe.Post, Title);
	}
	public string GetMsg()
	{
		return TDATA.GetString(ToolData.StringTalbe.Post, Msg);
	}

	public bool IsPlayEvent()
	{
		long now = (long)UTILE.Get_ServerTime_Milli();
		if (Times[0] != 0 && Times[0] > now) return false;
		if (Times[1] != 0 && Times[1] < now) return false;
		switch (Type)
		{
		case FAEventType.OpenEvent:
			if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < 201) return false;
			break;
		default:
			if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < 401) return false;
			break;
		}
		return true;
	}


	public bool IsRemoveEvent()
	{
		bool re = false;
		switch (Type)
		{
		case LS_Web.FAEventType.New_Attendance:
			if(Times[1] == 0)
			{
				var real = (List<FAEventData_Attendance>)RealData;
				re = !IsReward() && real.Max(o => o.No) <= Values[0];
			}
			break;
		//case LS_Web.FAEventType.Rot_Attendance:
		//	if(Times[1] == 0)
		//	{
		//		var real = (FAEventData_Rot_Attendance)RealData;
		//		re = !IsReward() && real.Lists.Max(o => o.No) <= Values[0];
		//	}
		//	break;
		case FAEventType.ReturnUser_Attendance:
			return GetRemainEndTime() < 0;
		}
		return re;
	}

	public bool IsAutoCheck()
	{
		return string.IsNullOrEmpty(Prefab) && IsReward();
	}

	public bool IsViewCheck()
	{
		return !string.IsNullOrEmpty(Prefab) && IsReward();
	}

	/// <summary> 보상 받을 가능성이 있응지 여부 </summary>
	public bool IsReward()
	{
		long checktime = 0;
		bool check = true;
		// 다시 체크해야되는 시간인지 확인
		switch(Type)
		{
		case FAEventType.New_Attendance:
			// 하루단위 체크
			checktime = 86400000L;// 24 * 60 * 60 * 1000L;
			// 받을 보상이 있을때
			check = ((List<FAEventData_Attendance>)RealData).Max(o => o.No) > Values[0];
			break;
		case FAEventType.Rot_Attendance:
			// 하루단위 체크
			checktime = 86400000L;// 24 * 60 * 60 * 1000L;
			// 받을 보상이 있을때
			check = ((FAEventData_Rot_Attendance)RealData).Lists.Max(o => o.No) > Values[0];
			break;
		case FAEventType.ReturnUser_Attendance:
			// 하루단위 체크
			checktime = 86400000L;// 24 * 60 * 60 * 1000L;
								  // 받을 보상이 있을때
			check = ((FAEventData_ReturnUser_Attendance)RealData).Attendance.Max(o => o.No) > Values[0];
			break;
		case FAEventType.FirstPurchase: return false;
		case FAEventType.ConnectTime:
			return Values[0] < 1 && Times[0] <= UTILE.Get_ServerTime_Milli() && Times[1] > UTILE.Get_ServerTime_Milli();
		default: return false;
		}

		// 비교해야될 간격이 다를경우
		return Times[2] / checktime != (long)UTILE.Get_ServerTime_Milli() / checktime && check;
	}
}


