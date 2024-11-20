using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LS_Web
{
	public class REQ_MAKINGINFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}


	public class RES_ALL_MAKINGINFO : RES_BASE
	{
		public List<RES_MAKINGINFO> Makings = new List<RES_MAKINGINFO>();
	}

	public class RES_MAKINGINFO : RES_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 연구 인덱스 </summary>
		public int Idx;
		/// <summary> 연구 레벨 </summary>
		public int Cnt;
		/// <summary> 0:시작시간, 1:종료시간 </summary>
		public long[] Times = new long[2];
		/// <summary> 상태 </summary>
		public TimeContentState State;
	}

	public void SEND_REQ_MAKINGINFO(Action<RES_ALL_MAKINGINFO> action, params long[] UIDs)
	{
		REQ_MAKINGINFO _data = new REQ_MAKINGINFO();
		_data.UserNo = USERINFO.m_UID;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);

		SendPost(Protocol.REQ_MAKINGINFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ALL_MAKINGINFO res = WEB.ParsResData<RES_ALL_MAKINGINFO>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				if (UIDs == null) USERINFO.m_Making.Clear();
				USERINFO.SetDATA(res.Makings);
			}

			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Makin Start
	public class REQ_MAKING_START : REQ_BASE
	{
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 생산 개수 </summary>
		public int Cnt;
	}
	public class RES_MAKING_START : RES_BASE
	{
		/// <summary> 생산 정보 </summary>
		public RES_MAKINGINFO Making;
	}

	public void SEND_REQ_MAKING_START(Action<RES_MAKING_START> action, int Idx, int Cnt)
	{
		REQ_MAKING_START _data = new REQ_MAKING_START();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = Idx;
		_data.Cnt = Cnt;

		SendPost(Protocol.REQ_MAKING_START, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_MAKING_START res = WEB.ParsResData<RES_MAKING_START>(data);
			if (res.IsSuccess())
			{
				USERINFO.SetDATA(res.Making);
				
				var tdata = TDATA.GetMakingTable(Idx);
				USERINFO.Check_Mission(MissionType.Making, 0, 0, 1);
				USERINFO.Check_Mission(MissionType.Making, (int)tdata.m_Group, 0, 1);
				HIVE.Check_Loaclpush_Making();
			}
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Makin End
	public class REQ_MAKING_END : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
		/// <summary> 캐시 사용 유무 </summary>
		public bool CashUse;
	}

	public class RES_MAKING_END : RES_BASE
	{
		/// <summary> 완료된 생산 정보 </summary>
		public List<RES_MAKINGINFO> Makings = new List<RES_MAKINGINFO>();
	}

	public void SEND_REQ_MAKING_END(Action<RES_MAKING_END> action, List<long> UIDs, bool IsCash = false)
	{
		REQ_MAKING_END _data = new REQ_MAKING_END();
		_data.UserNo = USERINFO.m_UID;
		_data.UIDs.AddRange(UIDs);
		_data.CashUse = IsCash;

		SendPost(Protocol.REQ_MAKING_END, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_MAKING_END res = WEB.ParsResData<RES_MAKING_END>(data);
			if (res.IsSuccess())
			{

				Dictionary<AchieveType, int> cntcheck = new Dictionary<AchieveType, int>();
				if(res.Rewards != null)
				{
					var items = res.GetRewards(Res_RewardType.Item).Select(o => (RES_REWARD_ITEM)o).ToList();
					cntcheck.Add(AchieveType.Weapon_Making_Count, 0);
					cntcheck.Add(AchieveType.Armor_Making_Count, 0);
					cntcheck.Add(AchieveType.Etc_Making_Count, 0);
					for (int i = items.Count - 1; i > -1; i--)
					{
						var tdata = TDATA.GetItemTable(items[i].Idx);
						var eqtype = tdata.GetEquipType();
						switch(eqtype)
						{
						case EquipType.Weapon: cntcheck[AchieveType.Weapon_Making_Count]++; break;
						case EquipType.End: cntcheck[AchieveType.Etc_Making_Count]++; break;
						default: cntcheck[AchieveType.Armor_Making_Count]++; break;
						}
					}
				}

				USERINFO.m_Achieve.Check_Achieve(AchieveType.Making_Count, 0, res.Makings.Count(o => o.State == TimeContentState.End));
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Weapon_Making_Count, 0, cntcheck[AchieveType.Weapon_Making_Count]);
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Armor_Making_Count, 0, cntcheck[AchieveType.Armor_Making_Count]);
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Etc_Making_Count, 0, cntcheck[AchieveType.Etc_Making_Count]);
				// 전체 받기
				USERINFO.SetDATA(res.Makings);

				HIVE.Check_Loaclpush_Making();
			}

			action?.Invoke(res);
		});
	}
}
