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
	public enum AnalyticsType
	{
		/// <summary> 스테이지 시작 </summary>
		STAGE_START = 1,
		/// <summary> 스테이지 결과 </summary>
		STAGE_RESULT,
		/// <summary> 재화 사용 </summary>
		USE_ITEM,
		/// <summary> 재화 획득 </summary>
		GET_ITEM,
		/// <summary> 구매 </summary>
		BUY,
		/// <summary> 이어하기 버튼 눌렀을때 </summary>
		continue_rescureflare_button,
		/// <summary> 이어하기(광고) 버튼 눌렀을때 </summary>
		continue_ad_button,
		/// <summary> 유저가 스테이지 패배 시, [포기하기] 버튼을 누를 때 </summary>
		continue_giveup_button,
		/// <summary> 유저가 스토리 [스킵] 버튼을 누를 때 </summary>
		story_skip_button,
		/// <summary>  </summary>
		END
	}

	public class REQ_ANALYTICS : REQ_BASE
	{
		public AnalyticsType Type;
		public long PlayerID;
		public long[] Values = new long[20];
	}

	public void SEND_REQ_ANALYTICS(AnalyticsType Type, params long[] values)
	{
		REQ_ANALYTICS _data = new REQ_ANALYTICS();
		_data.UserNo = USERINFO.m_UID;
		_data.PlayerID = HIVE.GetPlayerID();
		_data.Type = Type;
		_data.Values = values;
		SendPost(Protocol.REQ_ANALYTICS, JsonConvert.SerializeObject(_data), false, (result, data) => {});
	}
}
