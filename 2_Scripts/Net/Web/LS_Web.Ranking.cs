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
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_RANKING
	public class REQ_RANKING : REQ_BASE
	{
		/// <summary> 랭킹 </summary>
		public RankType Type;
	}

	public class RES_RANKING : RES_BASE
	{
		/// <summary> 100위까지의 유저 정보 </summary>
		public List<RES_RANKING_INFO> RankUsers;
		/// <summary> 자신의 정보 </summary>
		public RES_RANKING_INFO MyInfo;
	}

	public void SEND_REQ_RANKING(Action<RES_RANKING> action, RankType type)
	{
		REQ_RANKING _data = new REQ_RANKING();
		_data.UserNo = USERINFO.m_UID;
		_data.Type = type;
		SendPost(Protocol.REQ_RANKING, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(WEB.ParsResData<RES_RANKING>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_RANKING
	public class REQ_MY_RANKING : REQ_BASE
	{
		/// <summary> 랭킹 </summary>
		public RankType Type;
	}

	public class RES_MY_RANKING : RES_BASE
	{
		/// <summary> 자신의 정보 </summary>
		public RES_RANKING_INFO MyInfo;
	}

	public void SEND_REQ_MY_RANKING(Action<RES_MY_RANKING> action, RankType type)
	{
		REQ_MY_RANKING _data = new REQ_MY_RANKING();
		_data.UserNo = USERINFO.m_UID;
		_data.Type = type;
		SendPost(Protocol.REQ_MY_RANKING, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(WEB.ParsResData<RES_MY_RANKING>(data));
		});
	}
}
