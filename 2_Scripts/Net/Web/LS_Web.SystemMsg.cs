using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	public enum SystemMsgType
	{
		/// <summary> 일반 알림 </summary>
		Notice = 0,
		Char,
		MakingEQ,
		Zombie
	}

	public class SystemMsg
	{
		public long UID;
		/// <summary> 메세지 타입 </summary>
		public SystemMsgType type;
		/// <summary> 데이터 </summary>
		public string Data;
		/// <summary> 전송 시간</summary>
		public DateTime STime;
	}

	public class SystemMsg_Data_Item
	{
		public string Name;
		[JsonIgnore] public string m_Name { get { return BaseValue.GetUserName(Name); } }
		public int Grade;
		public int Idx;
	}

	public class SystemMsg_Data_Notice
	{
		public string Lang;
		public string Msg;
	}

	public class REQ_SYSTEM_MSG : REQ_BASE
	{
		/// <summary> 마지막 정보의 UID </summary>
		public long UID;
	}

	public class RES_SYSTEM_MSG
	{
		public long UID;
		public SystemMsgType Type;
		/// <summary> Type으로 구분
		/// Notice : SystemMsg_Data_Notice
		/// 기타 : SystemMsg_Data_Item
		/// </summary>
		public string Data;
		public long STime;
	}
	public class RES_SYSTEM_MSG_ALL : RES_BASE
	{
		public List<RES_SYSTEM_MSG> Msgs = new List<RES_SYSTEM_MSG>();
	}

	public void SEND_REQ_SYSTEM_MSG(Action<RES_SYSTEM_MSG_ALL> action, long UID)
	{
		REQ_SYSTEM_MSG _data = new REQ_SYSTEM_MSG();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;

		SendPost(Protocol.REQ_SYSTEM_MSG, JsonConvert.SerializeObject(_data), false, (result, data) => {
			RES_SYSTEM_MSG_ALL res = ParsResData<RES_SYSTEM_MSG_ALL>(data);
			action?.Invoke(res);
		});
	}
}
