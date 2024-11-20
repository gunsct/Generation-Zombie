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
	#region REQ_MISSIONINFO
	public class REQ_MISSIONINFO : REQ_BASE
	{
		public MissionMode Mode;
	}
	public class RES_ALL_MISSIONINFO : RES_BASE
	{
		public List<RES_MISSIONINFO> Missions = new List<RES_MISSIONINFO>();
	}

	public class RES_MISSIONINFO : RES_BASE
	{
		public long UID = 0;
		/// <summary> 패스용 상점 아이템 인덱스 </summary>
		public long SIdx = 0;
		/// <summary> 인덱스 </summary>
		public int Idx = 0;
		/// <summary> 진행 상태 </summary>
		public int[] Cnt = new int[4];
		/// <summary> 보상 지급 상태 </summary>
		public RewardState[] State;
		/// <summary> 시작시간 </summary>
		public long STime;
		/// <summary> 종료시간 </summary>
		public long ETime;
		/// <summary> 마지막 업데이트 시간 </summary>
		public long UTime;
	}

	public void SEND_REQ_MISSIONINFO(Action<RES_ALL_MISSIONINFO> action, MissionMode mode = MissionMode.None)
	{
		REQ_MISSIONINFO _data = new REQ_MISSIONINFO();
		_data.UserNo = USERINFO.m_UID;
		_data.Mode = mode;

		SendPost(Protocol.REQ_MISSIONINFO, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_ALL_MISSIONINFO>(data);
			if (res.IsSuccess()) USERINFO.m_Mission.SetData(res.Missions, mode == MissionMode.None);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_MISSION_REWARD
	public class REQ_MISSION_REWARD : REQ_BASE
	{
		public List<long> UIDs;
		/// <summary> 보상 위치(-1 전체 받기) </summary>
		public int Pos;
	}

	public class RES_MISSION_REWARD_DATA
	{
		public RES_MISSIONINFO Now;
		public RES_MISSIONINFO Next;
	}

	public class RES_MISSION_REWARD : RES_BASE
	{
		public List<RES_MISSION_REWARD_DATA> Datas;
	}

	public void SEND_REQ_MISSION_REWARD(Action<RES_MISSION_REWARD> action, List<MissionData> infos, int Pos = 0)
	{
		REQ_MISSION_REWARD _data = new REQ_MISSION_REWARD();
		_data.UserNo = USERINFO.m_UID;
		_data.UIDs = infos.Select(o => o.UID).ToList();
		_data.Pos = Pos;

		SendPost(Protocol.REQ_MISSION_REWARD, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_MISSION_REWARD>(data);
			if (res.IsSuccess())
			{
				for(int i = 0; i < res.Datas.Count; i++)
				{
					RES_MISSION_REWARD_DATA temp = res.Datas[i];
					USERINFO.m_Mission.SetData(temp.Now);
					USERINFO.m_Mission.SetData(temp.Next);

					TMissionTable tdata = TDATA.GetMissionTable(temp.Now.Idx);
					switch(tdata.m_Mode)
					{
					case MissionMode.Day: USERINFO.Check_Mission(MissionType.DailyQuestClear, 0, 0, 1); break;
					case MissionMode.BeginnerQuest: USERINFO.Check_Mission(MissionType.BeginnerQuestClear, tdata.m_ModeGid, 0, 1); break;
					case MissionMode.Event_miniGame: USERINFO.Check_Mission(MissionType.Event_miniGame_Clear, tdata.m_ModeGid, 0, 1); break;
					case MissionMode.ReturnUserQuest: USERINFO.Check_Mission(MissionType.ReturnUserQuestClaer, tdata.m_ModeGid, 0, 1); break;
					}
					USERINFO.Check_Mission(MissionType.ModeClear, (int)tdata.m_Mode, 0, 1);
				}

			}
			action?.Invoke(res);
		});
	}
	#endregion


	#region REQ_MISSION_REWARD
	public class REQ_MISSION_RESET : REQ_BASE
	{
		public int SIdx;
		public long UID;
	}

	public class RES_MISSION_RESET : RES_BASE
	{
		public RES_MISSIONINFO Change;
	}

	public void SEND_REQ_MISSION_RESET(Action<RES_MISSION_RESET> action, int ShopItemIdx, MissionData info)
	{
		REQ_MISSION_RESET _data = new REQ_MISSION_RESET();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.UID;
		_data.SIdx = ShopItemIdx;

		SendPost(Protocol.REQ_MISSION_RESET, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_MISSION_RESET>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_ShopInfo.SetBuyInfo(ShopItemIdx, 1);
				USERINFO.m_Mission.SetData(res.Change);
				TMissionTable tdata = TDATA.GetMissionTable(info.Idx);
			}
			action?.Invoke(res);
		});
	}
	#endregion
}
