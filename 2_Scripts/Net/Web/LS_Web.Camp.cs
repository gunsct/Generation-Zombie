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
	// RES_CAMP_BUILD_INFO

	public class RES_CAMP_BUILD_INFO
	{
		/// <summary> 유저 번호 </summary>
		public long UserNo;

		/// <summary> 건물 타입 </summary>
		public CampBuildType Build;

		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 각 건물별 값들
		/// <para>창고 : 자원 Junk, Cultivate, Chemical </para>
		/// <para>생산 : 자원 생산 종료 시간 Junk, Cultivate, Chemical </para>
		/// </summary>
		public long[] Values;

		/// <summary> 0 : 레벨업 시작 시간, 1 : 레벨업 종료 시간, 2 : 마지막 업데이트 시간 </summary>
		public long[] Time;
	}

	#region REQ_CAMP_BUILD
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_CAMP_BUILD
	public class REQ_CAMP_BUILD : REQ_BASE
	{
		public CampBuildType Type = CampBuildType.None;
	}

	public class RES_CAMP_BUILD : RES_BASE
	{
		public List<RES_CAMP_BUILD_INFO> CampBuilds = new List<RES_CAMP_BUILD_INFO>();
		/// <summary> 방어 약탈 정보 </summary>
		public List<REQ_CAMP_PLUNDER_LOG_DATA> Logs = new List<REQ_CAMP_PLUNDER_LOG_DATA>();
	}
	public void SEND_REQ_CAMP_BUILD(Action<RES_CAMP_BUILD> action = null, CampBuildType type = CampBuildType.None)
	{
		REQ_CAMP_BUILD _data = new REQ_CAMP_BUILD();
		_data.UserNo = USERINFO.m_UID;
		_data.Type = type;

		SendPost(Protocol.REQ_CAMP_BUILD, JsonConvert.SerializeObject(_data), (result, data) => {
			// ERROR_NOT_FOUND_CAMP_BUILD
			var res = ParsResData<RES_CAMP_BUILD>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.CampBuilds);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_CAMP_BUILD_LVUP

	public class REQ_CAMP_BUILD_LVUP : REQ_BASE
	{
		public CampBuildType Type;
	}
	public void SEND_REQ_CAMP_BUILD_LVUP(Action<RES_BASE> action, CampBuildType type)
	{
		REQ_CAMP_BUILD_LVUP _data = new REQ_CAMP_BUILD_LVUP();
		_data.UserNo = USERINFO.m_UID;
		_data.Type = type;

		SendPost(Protocol.REQ_CAMP_BUILD_LVUP, JsonConvert.SerializeObject(_data), (result, data) => {
			// ERROR_NOT_FOUND_CAMP_BUILD
			// ERROR_BUILD_NOT_IDLE
			// ERROR_BUILD_MAX_LV
			// ERROR_CAMP_RES
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	#endregion

	#region REQ_CAMP_RES_START

	public class REQ_CAMP_RES_START : REQ_BASE
	{
		/// <summary>
		/// 0 : junk
		/// <para> 1 : Cultivate </para>
		/// <para> 2 : Chemical </para>
		/// </summary>
		public int Pos;
	}
	public void SEND_REQ_CAMP_RES_START(Action<RES_BASE> action, int pos)
	{
		REQ_CAMP_RES_START _data = new REQ_CAMP_RES_START();
		_data.UserNo = USERINFO.m_UID;
		_data.Pos = pos;

		SendPost(Protocol.REQ_CAMP_RES_START, JsonConvert.SerializeObject(_data), (result, data) => {
			// ERROR_NOT_FOUND_CAMP_BUILD
			// ERROR_CAMP_RES_PRODUCTION
			// ERROR_TOOLDATA
			// ERROR_BUILD_LV
			// ERROR_MONEY
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	#endregion

	#region REQ_CAMP_RES_END

	public class REQ_CAMP_RES_END : REQ_BASE
	{
		/// <summary>
		/// 0 : junk
		/// <para> 1 : Cultivate </para>
		/// <para> 2 : Chemical </para>
		/// </summary>
		public int Pos;
	}
	public void SEND_REQ_CAMP_RES_END(Action<RES_BASE> action, int pos)
	{
		REQ_CAMP_RES_START _data = new REQ_CAMP_RES_START();
		_data.UserNo = USERINFO.m_UID;
		_data.Pos = pos;

		SendPost(Protocol.REQ_CAMP_RES_END, JsonConvert.SerializeObject(_data), (result, data) => {
			// ERROR_NOT_FOUND_CAMP_BUILD
			// ERROR_TOOLDATA
			// ERROR_BUILD_LV
			// ERROR_BUILD_RES_MAX 자원 보관 최대치 
			// ERROR_CASH
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	#endregion

	#region REQ_CAMP_PLUNDER_LOG

	public enum CounterState
	{
		// <summary> 반격 대기중 </summary>
		Idle = 0,
		/// <summary> 반격 결과 대기중 </summary>
		Counter_IDLE,
		/// <summary> 반격(성공) </summary>
		Counter_WIN,
		/// <summary> 반격(실패) </summary>
		Counter_FAIL,
		/// <summary> 반격 당함 </summary>
		Countered

	}
	public class REQ_CAMP_PLUNDER_LOG_DATA
	{
		/// <summary> 전투 고유 인덱스 </summary>
		public int Idx;
		/// <summary> 대상 (공격자) </summary>
		public long Target;
		/// <summary> 반격 유무 </summary>
		public CounterState State;
		/// <summary> 약탈 시간 </summary>
		public long BTime;
		/// <summary> 약탈 내용 List<RewardInfo> </summary>
		public string Rewards;

		// 대상 현 정보
		/// <summary> 방어덱 전투력 </summary>
		public long Power = 0;
		/// <summary> 프로필이미지 </summary>
		public int Profile;
		/// <summary> 닉네임 </summary>
		public string Name;
		/// <summary> 국가코드 </summary>
		public string Nation;
		/// <summary> 레벨 </summary>
		public int LV;
		
		public List<RewardInfo> GetReward() {
			return JsonConvert.DeserializeObject<List<RewardInfo>>(Rewards);
		}
	}

	public class REQ_CAMP_PLUNDER_LOG : REQ_BASE
	{
		/// <summary> 0 : 5개 리스트, 1~:단일 로그만 가져올경우 </summary>
		public int Idx;
	}
	public class RES_CAMP_PLUNDER_LOG : RES_BASE
	{
		public List<REQ_CAMP_PLUNDER_LOG_DATA> Logs = new List<REQ_CAMP_PLUNDER_LOG_DATA>();
	}

	public void SEND_REQ_CAMP_PLUNDER_LOG(Action<RES_CAMP_PLUNDER_LOG> action, int idx = 0)
	{
		REQ_CAMP_PLUNDER_LOG _data = new REQ_CAMP_PLUNDER_LOG();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = idx;

		SendPost(Protocol.REQ_CAMP_PLUNDER_LOG, JsonConvert.SerializeObject(_data), (result, data) => {
			// ERROR_CAMP_PLUNDER_LOG
			action?.Invoke(ParsResData<RES_CAMP_PLUNDER_LOG>(data));
		});
	}
	#endregion
}
