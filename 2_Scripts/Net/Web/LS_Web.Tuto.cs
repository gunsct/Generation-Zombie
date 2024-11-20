using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_TUTOEND : REQ_TUTO
	{
		/// <summary> 튜토리얼 진행 번호 </summary>
		public int No;
	}


	public class RES_TUTOINFO : RES_BASE
	{
		/// <summary> 튜토리얼 타입 </summary>
		public TutoKind Type;
		/// <summary> 튜토리얼 진행 번호 </summary>
		public int No;
	}

	public class REQ_TUTO : REQ_BASE
	{
		/// <summary> 튜토리얼 타입 </summary>
		public TutoKind Type;
	}

	public class RES_ALL_TUTOINFO : RES_BASE
	{
		/// <summary> 튜토리얼 진행 정보 </summary>
		public List<RES_TUTOINFO> Tutos;
	}

	public void SEND_REQ_TUTO(Action<RES_ALL_TUTOINFO> action, long UserNo, TutoKind Kind = TutoKind.None)
	{
		REQ_TUTO _data = new REQ_TUTO();
		_data.UserNo = UserNo;
		_data.Type = Kind;
		SendPost(Protocol.REQ_TUTO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ALL_TUTOINFO>(data));
		});
	}

	public void SEND_REQ_TUTOEND(Action<RES_TUTOINFO> action, TutoKind kind, int no)
	{
		REQ_TUTOEND _data = new REQ_TUTOEND();
		_data.UserNo = USERINFO.m_UID;
		_data.Type = kind;
		_data.No = no;

		SendPost(Protocol.REQ_TUTOEND, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(JsonConvert.DeserializeObject<RES_TUTOINFO>(data));
		});
	}
}
