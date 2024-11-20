using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public enum MissionState
{
	/// <summary> 대기중 </summary>
	Idle,
	/// <summary> 보상 받기 </summary>
	Reward,
	/// <summary> 종료 </summary>
	End
}

public class MissionData : ClassMng
{
	public long UID = 0;
	/// <summary> 패스용 상점 아이템 인덱스 </summary>
	public long SIdx = 0;
	/// <summary> 인덱스 </summary>
	public int Idx = 0;
	/// <summary> 진행 상태 </summary>
	public int[] Cnt = new int[4];
	/// <summary> 보상 지급 상태 </summary>
	public RewardState[] State = new RewardState[2] { RewardState.Idle, RewardState.Idle };
	/// <summary> 시작시간 </summary>
	public long STime;
	/// <summary> 종료시간 </summary>
	public long ETime;
	/// <summary> 마지막 업데이트 시간 </summary>
	public long UTime;

	[JsonIgnore] public TMissionTable m_TData { get { return TDATA.GetMissionTable(Idx); } }

	public MissionData() { }
	public MissionData(int _idx) {
		UID = Utile_Class.GetUniqeID();
		SIdx = 0;
		Idx = _idx;
		Cnt = new int[4];
		UTime = (long)UTILE.Get_ServerTime_Milli();
		STime = UTime + 86400000;
		ETime = UTime + 86400000;
	}

	public int GetCnt(int pos)
	{
		if (m_TData.m_Check.Count <= pos) return 0;
		if (Cnt.Length <= pos) return 0;
		var check = m_TData.m_Check[pos];
		var re = Cnt[pos];
		switch (check.m_Type)
		{
		case MissionType.StageIdx: return check.m_Val[1] <= USERINFO.m_Stage[StageContentType.Stage].Idxs[check.m_Val[0]].Idx ? 1 : 0;
		case MissionType.CharLevel:
			if (check.m_Val[1] != 0) return USERINFO.m_Chars.Find(o => o.m_Idx == check.m_Val[1] && o.m_LV >= check.m_Val[0]) != null ? 1 : 0;
			re = USERINFO.m_Chars.Count(o => o.m_LV >= check.m_Val[0]);
			break;
		case MissionType.CharGrade:
			if (check.m_Val[1] != 0) return USERINFO.m_Chars.Find(o => o.m_Idx == check.m_Val[1] && o.m_Grade >= check.m_Val[0]) != null ? 1 : 0;
			re = USERINFO.m_Chars.Count(o => o.m_Grade >= check.m_Val[0]);
			break;
		case MissionType.SkillUpgrade:
			re = USERINFO.m_Chars.Count(o => o.IS_SetEquip());
			break;
		case MissionType.GiveItem:
			re = USERINFO.m_Items.FindAll(o => o.m_Idx == check.m_Val[0]).Sum(o => o.m_Stack);
			break;
		}

		return Mathf.Min(re, check.m_Cnt);
	}

	public bool IS_End()
	{
		for (int i = 0, iMax = m_TData.m_Rewards.Length; i < iMax; i++)
		{
			if (m_TData.m_Rewards[i].Kind == RewardKind.None) continue;
			if (State[i] == RewardState.Idle) return false;
		}

		return true;
	}

	public bool IS_Complete()
	{
		for (int i = 0, iMax = Math.Min(Cnt.Length, m_TData.m_Check.Count); i < iMax; i++)
		{
			if (GetCnt(i) < m_TData.m_Check[i].m_Cnt) return false;
		}

		return true;
	}

	public float GetPer() {
		var total = 0f;
		var now = 0f;
		for (int i = 0, iMax = Math.Min(Cnt.Length, m_TData.m_Check.Count); i < iMax; i++)
		{
			total += m_TData.m_Check[i].m_Cnt;
			now += GetCnt(i);
		}

		return now / Mathf.Max(1f, total);
	}

	public double IS_RemainTime() {
		double timezone = UTILE.Get_ServerTime_Milli() - UTILE.Get_ServerTime_Milli() % 86400000;
		double remain = (timezone + 86400000 - UTILE.Get_ServerTime_Milli()) * 0.001d;
		return remain;
	}

	public bool IsPlayMission()
	{
		var now = UTILE.Get_ServerTime_Milli();
		if (STime > 0 && STime > now) return false;   // 시작하지 않은 미션
		if (ETime > 0 && ETime < now) return false;   // 종료된 미션
		return true;
	}
}

public class MissionInfo : ClassMng
{
	List<MissionData> Missions = new List<MissionData>();
	Dictionary<int, List<MissionData>> BeginnerQuest = null;

	public void SetData(List<RES_MISSIONINFO> list, bool clear = true)
	{
		if (clear)
		{
			Missions.Clear();
			BeginnerQuest = null;
		}
		for (int i = list.Count - 1; i > -1; i--) SetData(list[i]);
	}

	public void SetData(RES_MISSIONINFO data)
	{
		if (data == null) return;
		var mission = Missions.Find(o => o.UID == data.UID);
		if (mission == null)
		{
			mission = new MissionData();
			Missions.Add(mission);
		}


		mission.UID = data.UID;
		mission.SIdx = data.SIdx;
		mission.Idx = data.Idx;
		mission.Cnt = data.Cnt;
		mission.State = data.State;
		mission.STime = data.STime;
		mission.ETime = data.ETime;
		mission.UTime = data.UTime;
	}
	/// <summary> 클라용 미션 추가 </summary>
	public void SetData(MissionData _data) {
		Missions.Add(_data);
		MAIN.Save_UserInfo();
	}
	/// <summary> 클라용 미션 제거 </summary>
	public void DelData(MissionData _data) {
		Missions.Remove(_data);
		MAIN.Save_UserInfo();
	}

	public bool IsSuccess(MissionMode mode, long ShopIdx = 0)
	{
		if (mode == MissionMode.Pass) return Get_Missions(mode, ShopIdx).Find(o => !o.IS_End() && o.IS_Complete()) != null;
		//else if (mode == MissionMode.BeginnerQuest) return USERINFO.CheckSuccNowStepNewMission();
		return Get_Missions(mode, ShopIdx).Find(o => o.IsPlayMission() && o.State[0] == RewardState.Idle && o.IS_Complete()) != null;
	}

	public MissionData SuccessMission(MissionMode mode, int ShopIdx = 0)
	{
		if (mode == MissionMode.Pass) return Get_Missions(mode, ShopIdx).Find(o => !o.IS_End() && o.IS_Complete());
		else if (mode == MissionMode.BeginnerQuest)
		{
			var list = USERINFO.GetNowStepNewMission();
			return list != null ? list.Find(o => !o.IS_End() && o.IS_Complete()) : null;
		}
		return Get_Missions(mode, ShopIdx).Find(o => !o.IS_End() && o.IS_Complete());
	}

	public List<MissionData> Get_Missions()
	{
		return Missions;
	}

	public List<MissionData> Get_Missions(MissionMode mode, long ShopIdx = 0)
	{
		switch(mode)
		{
		case MissionMode.Pass: return Missions.FindAll(o => o.m_TData != null && o.m_TData.m_Mode == mode && o.SIdx == ShopIdx && o.m_TData.m_Prob > 0);
		}
		return Missions.FindAll(o => o.m_TData != null && o.m_TData.m_Mode == mode && o.m_TData.m_Prob > 0);
	}

	public Dictionary<int, List<MissionData>> GetBeginnerQuest()
	{
		if (BeginnerQuest == null || BeginnerQuest.Count < 1) BeginnerQuest = Get_Missions(MissionMode.BeginnerQuest).GroupBy(o => o.m_TData.m_ModeGid).OrderBy(o => o.Key).ToDictionary(o => o.Key, o => o.ToList());
		return BeginnerQuest;
	}
	public MissionData GetGuideMission()
	{
		return Get_Missions(MissionMode.Guide).Find(o => !o.IS_End());
	}

	public MissionData Get_Mission(int _idx) {
		return Missions.Find(o => o.Idx == _idx);
	}
	public MissionData Get_Missions(long UID)
	{
		return Missions.Find(o => o.UID == UID);
	}

	public void Check_Mission(MissionType type, int value1, int value2, int cnt = 1, long EUID = 0)
	{
		if (cnt == 0) return;
		var now = UTILE.Get_ServerTime_Milli();
		foreach (var info in Missions)
		{
			if (info.IS_Complete()) continue;
			if (!info.IsPlayMission()) continue;
			if (EUID != 0 && info.SIdx != EUID) continue;		// 이벤트 전용 체크
			var tdata = TDATA.GetMissionTable(info.Idx);
			if (tdata == null) continue;
			if (tdata.m_Check.Count < 1) continue;

			for (int i = 0, iMax = Math.Min(info.Cnt.Length, tdata.m_Check.Count); i < iMax; i++)
			{
				if (tdata.m_Check[i].m_Cnt <= info.Cnt[i]) continue;
				if (tdata.m_Check[i].m_Type != type) continue;
				if (type != MissionType.Event_miniGame_Clear)
				{
					if (tdata.m_Check[i].m_Val[0] != value1) continue;
					if (tdata.m_Check[i].m_Val[1] != value2) continue;
				}
				info.Cnt[i] = Math.Min(tdata.m_Check[i].m_Cnt, info.Cnt[i] + cnt);
			}
		}
	}
	public void Check_MissionUpDown(MissionType type, int min, int max, int cnt = 1, long EUID = 0)
	{
		if (cnt == 0) return;
		var now = UTILE.Get_ServerTime_Milli();

		foreach (var info in Missions)
		{
			if (info.IS_Complete()) continue;
			if (!info.IsPlayMission()) continue;
			if (EUID != 0 && info.SIdx != EUID) continue;		// 이벤트 전용 체크

			var tdata = TDATA.GetMissionTable(info.Idx);
			if (tdata == null) continue;
			if (tdata.m_Check.Count < 1) continue;

			for (int i = 0, iMax = Math.Min(info.Cnt.Length, tdata.m_Check.Count); i < iMax; i++)
			{
				if (tdata.m_Check[i].m_Cnt <= info.Cnt[i]) continue;
				if (tdata.m_Check[i].m_Type != type) continue;
				if (type == MissionType.Event_miniGame_Clear) continue;

				bool isArea = tdata.m_Check[i].m_Val[0] > min && tdata.m_Check[i].m_Val[0] <= max;
				//switch (type)
				//{
				//case MissionType.EventGrowupLevel:
				//	if (tdata.m_Check[i].m_Val[0] <= min) continue;
				//	if (tdata.m_Check[i].m_Val[0] > max) continue;
				//	break;
				//default:
				//	if (tdata.m_Check[i].m_Val[0] <= min) continue;
				//	if (tdata.m_Check[i].m_Val[1] > max) continue;
				//	break;
				//}
				if (!isArea) continue;
				info.Cnt[i] = Math.Min(tdata.m_Check[i].m_Cnt, info.Cnt[i] + cnt);
			}
		}
	}

	public void GetReward(MissionData data, int pos, Action<RES_MISSION_REWARD> CB)
	{
#if !NOT_USE_NET
		WEB.SEND_REQ_MISSION_REWARD((res) => {
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_MISSIONINFO((res2) => { CB?.Invoke(null); });
				return;
			}
			CB?.Invoke(res);
		}, new List<MissionData>() { data }, pos);
#endif
	}

	public static void Sort(List<MissionData> ms)
	{
		ms.Sort((MissionData _before, MissionData _after) => {
			int bval = 1, aval = 1;

			if (_after.IS_End()) aval = 0;
			else if (_after.IS_Complete()) aval = 2;

			if (_before.IS_End()) bval = 0;
			else if (_before.IS_Complete()) bval = 2;

			if (bval != aval) return aval.CompareTo(bval);
			float bper = _before.GetPer();
			float aper = _after.GetPer();
			if(bper != aper) return aper.CompareTo(bper);
			return _before.Idx.CompareTo(_after.Idx);
		});
	}
}


