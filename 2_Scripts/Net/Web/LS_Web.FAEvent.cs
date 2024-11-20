using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	public enum FAEventType
	{
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 타임 이벤트 TTime 사용
		/// <summary> 출석체크(신규유저) </summary>
		New_Attendance = 0,
		/// <summary> 출석체크(로테이션) </summary>
		Rot_Attendance,
		/// <summary> 최초구매 </summary>
		FirstPurchase,
		/// <summary> 복귀 유저 </summary>
		ReturnUser_Attendance,
		/// <summary> 타임이벤트 종류 끝 </summary>
		End_TimeEvent = 1000,
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 기간제 이벤트 STime, ETime 사용
		/// <summary> 시간내 접속 이벤트 </summary>
		ConnectTime,
		/// <summary> 스테이지, 미니게임(아이템 수집), 미션</summary>
		Stage_Minigame,
		/// <summary> 스테이지, 미니게임(칠면조 키우기), 미션</summary>
		GrowUP,
		/// <summary> 오픈 이벤트 (특정 기간 동안만 발동)</summary>
		OpenEvent,
		End
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 자신이 진행중인 이벤트 정보
	public class RES_ALL_MY_FAEVENT_INFO : RES_BASE
	{
		public List<RES_MY_FAEVENT_INFO> Events = new List<RES_MY_FAEVENT_INFO>();
		public List<RES_MISSIONINFO> Missions = new List<RES_MISSIONINFO>();
	}

	public class RES_MY_FAEVENT_INFO : RES_BASE
	{
		/// <summary> 유저 이벤트 고유번호 </summary>
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
		public FAEventType Type;
		/// <summary> 이벤트 정보 JSONObject </summary>
		public string Info;

		/// <summary> 각 이벤트에 대한 값들
		/// <para> EventType </para>
		/// <para> New_Attendance => 0:진행한 회차 </para>
		/// <para> Rot_Attendance => 0:진행한 회차 </para>
		/// <para> FirstPurchase => 구매한 아이템들 </para>
		/// <para> Stage_Minigame => 0 : 진행 스테이지 레벨, 1 : 리롤 카운트 </para>
		/// <para> GrowUP => 0 : 진행 스테이지 레벨, 1 : 리롤 카운트, 2 : 칠면조 레벨, 3 : 칠면조 Exp </para>
		/// </summary>
		public List<long> Values;

		/// <summary> 0:시작시간, 1:종료시간, 2:업데이트 시간 </summary>
		public long[] Times = new long[3];
	}

	/// <summary> 유저의 이벤트 데이터 받기 </summary>
	public void SEND_REQ_MY_FAEVENT_INFO(Action<RES_ALL_MY_FAEVENT_INFO> action, bool _refresh = true)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		if(_refresh) USERINFO.m_Event.UTime = DateTime.UtcNow;
		// 다음 갱신 시간을위해 시간 저장
		USERINFO.m_Event.ATime = UTILE.Get_ServerTime();
		SendPost(Protocol.REQ_MY_FAEVENT_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ALL_MY_FAEVENT_INFO res = ParsResData<RES_ALL_MY_FAEVENT_INFO>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_Event.Datas.Clear();
				USERINFO.m_Event.SetDATA(res.Events);
				if (_refresh) USERINFO.m_Event.UTime = DateTime.UtcNow;
				// 다음 갱신 시간을위해 시간 저장
				USERINFO.m_Event.ATime = UTILE.Get_ServerTime();
				USERINFO.m_Mission.SetData(res.Missions, false);
			}
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 이벤트 정보 갱신
	public class REQ_CHECK_MY_FAEVENT_INFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 이벤트 타입의 검색용 값들 </summary>
		public List<long> Values;
	}

	/// <summary> 이벤트 체크(보상 받기) </summary>
	public void SEND_REQ_CHECK_MY_FAEVENT_INFO(Action<RES_MY_FAEVENT_INFO> action, MyFAEvent data)
	{
		REQ_CHECK_MY_FAEVENT_INFO _data = new REQ_CHECK_MY_FAEVENT_INFO();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = data.UID;
		_data.Values = data.TypeValues;

		SendPost(Protocol.REQ_CHECK_MY_FAEVENT_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_MY_FAEVENT_INFO res = ParsResData<RES_MY_FAEVENT_INFO>(data);
			if (res.IsSuccess()) USERINFO.m_Event.SetDATA(res);
			action?.Invoke(res);
		});
	}


	#region REQ_EVENT_GROWUP
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Growup Evnet
	public class REQ_EVENT_GROWUP : REQ_BASE
	{
		/// <summary> 이벤트 고유 번호 </summary>
		public long EUID;
		/// <summary> 아이템 사용 개수 </summary>
		public int Cnt;
	}
	public class RES_EVENT_GROWUP : RES_BASE
	{
		/// <summary> 이벤트 정보 </summary>
		public RES_MY_FAEVENT_INFO Event;
	}

	/// <summary> 이벤트 체크(보상 받기) </summary>
	public void SEND_REQ_EVENT_GROWUP(Action<RES_EVENT_GROWUP> action, MyFAEvent eventinfo, int Cnt)
	{
		REQ_EVENT_GROWUP _data = new REQ_EVENT_GROWUP();
		_data.UserNo = USERINFO.m_UID;
		_data.EUID = eventinfo.UID;
		_data.Cnt = Cnt;

		SendPost(Protocol.REQ_EVENT_GROWUP, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_EVENT_GROWUP res = ParsResData<RES_EVENT_GROWUP>(data);
			if (res.IsSuccess())
			{
				var beforLV = eventinfo.Values[2];
				USERINFO.Check_MissionUpDown(MissionType.EventGrowupLevel, (int)beforLV, (int)res.Event.Values[2], 1, eventinfo.EventUID);
			}
			action?.Invoke(res);
		});
	}
	#endregion
}
