using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{

	/// <summary> 돌발이벤트 종료(NPC 토크 다이얼로그 종료시 호출) </summary>
	/// <param name="idx">구매할 아이템 인덱스</param>
	public void SEND_REQ_ADDEVENT_END(Action<RES_BASE> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_ADDEVENT_END, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.m_AddEvent = 0;
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 돌발 이벤트 블랙 마켓 아이템 구매
	public class REQ_ADD_EVENT_BLACKMARKET : REQ_BASE
	{
		/// <summary> 구매 아이템 인덱스 </summary>
		public int Idx;
	}

	/// <summary> 돌발이벤트 블랙마켓 아이템 구매 </summary>
	/// <param name="idx">구매할 아이템 인덱스</param>
	public void SEND_REQ_ADD_EVENT_BLACKMARKET(Action<RES_BASE> action, int idx)
	{
		REQ_ADD_EVENT_BLACKMARKET _data = new REQ_ADD_EVENT_BLACKMARKET();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = idx;

		SendPost(Protocol.REQ_ADD_EVENT_BLACKMARKET, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 돌발 이벤트 습격(노트 전투)
	public class REQ_ADD_EVENT_SUDDENENEMY : REQ_BASE
	{
		/// <summary> 결과 </summary>
		public bool IsWin;
	}
	public void SEND_REQ_ADD_EVENT_SUDDENENEMY(Action<RES_BASE> action, bool IsWin)
	{
		REQ_ADD_EVENT_SUDDENENEMY _data = new REQ_ADD_EVENT_SUDDENENEMY();
		_data.UserNo = USERINFO.m_UID;
		_data.IsWin = IsWin;

		SendPost(Protocol.REQ_ADD_EVENT_SUDDENENEMY, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.m_AddEvent = 0;
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 돌발 이벤트 NPC 토크 선택지 선택하면 호출
	public class REQ_ADD_EVENT_NPC : REQ_BASE
	{
		/// <summary> 선택한 인덱스 </summary>
		public int Idx;
	}

	/// <summary> 돌발 이벤트 NPC 토크 선택지 선택 </summary>
	/// <param name="Idx">선택한 선택지 인덱스</param>
	public void SEND_REQ_ADD_EVENT_NPC(Action<RES_BASE> action, int Idx)
	{
		REQ_ADD_EVENT_NPC _data = new REQ_ADD_EVENT_NPC();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = Idx;

		SendPost(Protocol.REQ_ADD_EVENT_NPC, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.m_AddEvent = 0;
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
}
