using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class AchieveData : ClassMng
{
	public AchieveType Type;
	// 혹시 몰라 배열로 만듬
	public long[] Value = new long[1];
	public long Cnt;
}

public class AchieveInfo : ClassMng
{
	/// <summary> 업적 누적 체크용 데이터 </summary>
	public List<AchieveData> Datas = new List<AchieveData>();
	/// <summary> 보상받은 데이터 (인덱스, 레벨) </summary>
	public Dictionary<int, List<int>> RewardLVs = new Dictionary<int, List<int>>();

	public List<TAchievementTable> NowCheckTable = new List<TAchievementTable>();

	public bool IsAlram() => NowCheckTable.Count(o => IsSucAchieve(o)) > 0;

	public void Init(RES_ACHIEVE_INFO data = null)
	{
		Datas.Clear();
		RewardLVs.Clear();
		if (data == null)
		{
			ResetCheckAchieve();
			return;
		}
		Datas = data.Datas;
		for(int i = 0; i < data.EAchieve.Count; i++)
		{
			AchieveEnd item = data.EAchieve[i];
			RewardLVs.Add(item.Idx, item.LVs);
		}
		ResetCheckAchieve();
	}

	public AchieveData GetAchieveData(TAchievementTable tdata)
	{
		return Datas.Find(o => o.Type == tdata.m_Type && o.Value[0] == tdata.m_Values[0]);
	}

	public void ResetCheckAchieve()
	{
		// 해당 그룹데이터
		List<TAchievementGroup> Group = TDATA.GetAchievementGroup();
		NowCheckTable.Clear();
		for (int i = 0; i < Group.Count; i++)
		{
			var Data = Group[i];
			// 해당 그룹의 보상받지 않은 데이터중 레벨이 가장 낮은 데이터
			List<TAchievementTable> list;
			if (!RewardLVs.ContainsKey(Data.m_Idx)) list = Data.m_List;
			else list = Data.m_List.FindAll(l => !RewardLVs[l.m_Idx].Any(r => r == l.m_LV));
			if (list == null || list.Count < 1) continue;
			// 레벨이 가장 낮은 데이터
			NowCheckTable.Add(list.OrderBy(o => o.m_LV).FirstOrDefault());
		}
	}

	public List<TAchievementTable> GetAchieveList(TAchievementTable.Tab tab = TAchievementTable.Tab.End)
	{
		if (tab == TAchievementTable.Tab.End) return NowCheckTable;
		return NowCheckTable.FindAll(o => o.m_Tab == tab);
	}

	public List<TAchievementTable> GetSucAchieveList(TAchievementTable.Tab tab = TAchievementTable.Tab.End)
	{
		if (tab == TAchievementTable.Tab.End) return NowCheckTable.FindAll(o => IsSucAchieve(o));
		return NowCheckTable.FindAll(o => o.m_Tab == tab && IsSucAchieve(o));
	}

	public bool IsSucAchieve(TAchievementTable tdata)
	{
		AchieveData data = GetAchieveData(tdata);
		long Cnt = 0;
		if (data != null) Cnt = data.Cnt;
		return Cnt >= tdata.m_Values[1];
	}

	public int Sort(TAchievementTable befor, TAchievementTable after)
	{
		AchieveData bdata = GetAchieveData(befor);
		AchieveData adata = GetAchieveData(after);
		long bCnt = bdata != null ? bdata.Cnt : 0;
		long aCnt = adata != null ? adata.Cnt : 0;

		bool bisSuc = bCnt >= befor.m_Values[1];
		bool aisSuc = aCnt >= after.m_Values[1];
		// 성공 순서
		if (bisSuc != aisSuc) return aisSuc.CompareTo(bisSuc);
		else if(bisSuc == aisSuc)
		{
			// 달성도 순
			float bPer = (float)((double)bCnt / (double)befor.m_Values[1]);
			float aPer = (float)((double)aCnt / (double)after.m_Values[1]);
			if (bPer != aPer) return aPer.CompareTo(bPer);
		}
		return befor.m_Idx.CompareTo(after.m_Idx);
	}

	public void Check_Achieve(AchieveType type, int value = 0, long cnt = 1)
	{
		if (cnt == 0) return;
		var data = Datas.Find(o => o.Type == type && o.Value[0] == value);
		if(data == null)
		{
			data = new AchieveData();
			data.Type = type;
			data.Value[0] = value;
			data.Cnt = 0;
			Datas.Add(data);
		}
		data.Cnt += cnt;
		DLGTINFO.f_RFGuidQuestUI?.Invoke(GuidQuestInfo.InfoType.Achieve);
	}

	/// <summary> 레벨달성등 해당 크거나 작을때 체크 </summary>
	/// <param name="type">체크 타입</param>
	/// <param name="Befor">이전값</param>
	/// <param name="After">현재값</param>
	public void Check_AchieveUpDown(AchieveType type, int Befor = 0, int After = 0)
	{
		switch(type)
		{
		// 레벨 달성
		case AchieveType.Character_Level_Count:
		case AchieveType.Character_Grade_Count:
		case AchieveType.Equip_Level_Count:
		case AchieveType.Equip_Grade_Count:
		case AchieveType.DNA_LevelUp_Count:
			break;
		default: return;

		}

		List<long> values = TDATA.GetAchieveValueList(type);
		if (values == null || values.Count < 1) return;
		List<long> checkvalue = values.FindAll(o => Befor < o && After >= o);
		for(int i = checkvalue.Count - 1; i > -1; i--)
		{
			var value = checkvalue[i];
			var data = Datas.Find(o => o.Type == type && o.Value[0] == value);
			if (data == null)
			{
				data = new AchieveData();
				data.Type = type;
				data.Value[0] = value;
				data.Cnt = 0;
				Datas.Add(data);
			}
			data.Cnt++;
		}
		DLGTINFO.f_RFGuidQuestUI?.Invoke(GuidQuestInfo.InfoType.Achieve);
	}
	public void Check_StageClear(StageContentType Mode, int pos, int Cnt = 1, int Idx = 0, long EUID = 0)
	{
		if (EUID != 0)
		{
			USERINFO.Check_Mission(MissionType.EventStageClear, 0, 0, Cnt, EUID);
			USERINFO.Check_Mission(MissionType.EventStageIdx, pos, Idx, Cnt, EUID);
		}
		USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.All, 0, Cnt);
		switch (Mode)
		{
		case StageContentType.Stage:
			switch (pos)
			{
			case 0: USERINFO.m_Achieve.Check_Achieve(AchieveType.Normal_Stage_Clear, 0, Cnt); break;
			case 1: USERINFO.m_Achieve.Check_Achieve(AchieveType.Hard_Stage_Clear, 0, Cnt); break;
			case 2: USERINFO.m_Achieve.Check_Achieve(AchieveType.Nightmare_Stage_Clear, 0, Cnt); break;
			}
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, pos, Cnt);
			break;
		case StageContentType.Bank:
			USERINFO.m_Achieve.Check_Achieve(AchieveType.Bank_Clear, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
			break;
		case StageContentType.Academy:
			USERINFO.m_Achieve.Check_Achieve(AchieveType.Academy_Clear, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
			break;
		case StageContentType.University:
			USERINFO.m_Achieve.Check_Achieve(AchieveType.University_Clear, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
			break;
		case StageContentType.Tower:
			USERINFO.m_Achieve.Check_Achieve(AchieveType.Tower_Clear, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
			break;
		case StageContentType.Cemetery:
			USERINFO.m_Achieve.Check_Achieve(AchieveType.Cemetery_Clear, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
			break;
		case StageContentType.Factory:
			USERINFO.m_Achieve.Check_Achieve(AchieveType.Factory_Clear, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
			USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
			break;
		}
	}
}

