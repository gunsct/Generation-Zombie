using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TAdventureTable : ClassMng
{
	public class ADReward
	{
		public int m_Idx;//지급되는 보상 Index(ItemTable 참조)
		public int m_Cnt;//보상 지급량
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 탐험 미션명, String_Etc 참조 </summary>
	public int m_Name;
	/// <summary> 탐험 설명, String_Etc 참조 </summary>
	public int m_Desc;
	/// <summary> 모험 등급 </summary>
	public int m_AdventureGrade;
	/// <summary> 확률에 집계될 조건 레벨 </summary>
	public int m_OpenLevel;
	/// <summary> 파티에 참여해야 하는 캐릭터 수 </summary>
	public int m_PartyCount;
	/// <summary> 파티에 필수 포함되어야 하는 등급 </summary>
	public int m_PartyGrade;
	/// <summary> 파티 필수 포함 등급을 만족해야 하는 캐릭터 수 </summary>
	public int m_PartyGradeCount;
	/// <summary> 해당 미션을 완료하기 위해 필요한 시간 (단위 : 분) </summary>
	public int m_Time;
	/// <summary> 지급되는 보상 Index(ItemTable 참조) </summary>
	public List<ADReward> m_Reward = new List<ADReward>();
	/// <summary> 등장 확률 (Prob/조건이 맞는 탐험의 Prob의 합) </summary>
	public int m_Prob;


	public TAdventureTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_AdventureGrade = pResult.Get_Int32();
		m_OpenLevel = pResult.Get_Int32();
		m_PartyCount = pResult.Get_Int32();
		m_PartyGrade = pResult.Get_Int32();
		m_PartyGradeCount = pResult.Get_Int32();
		m_Time = pResult.Get_Int32();
		for(int i = 0; i < 2; i++) {
			int idx = pResult.Get_Int32();
			if (idx == 0) pResult.NextReadPos();
			else m_Reward.Add(new ADReward() { m_Idx = idx, m_Cnt = pResult.Get_Int32() });
		}
		m_Prob = pResult.Get_Int32();
	}
	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
	}

	public long GetTime() {
		long Re = m_Time * 1000L;
		float per = USERINFO.GetSkillValue(SkillKind.ExploreTimeUp) + USERINFO.ResearchValue(ResearchEff.ExploreTimeUp);
		Re -= Mathf.RoundToInt(Re * per);
		Re = Math.Max(Re, 0);
		return Re;
	}
}

public class TAdventureTableMng : ToolFile
{
	public Dictionary<int, TAdventureTable> DIC_Idx = new Dictionary<int, TAdventureTable>();
	public List<TAdventureTable> Tables = new List<TAdventureTable>();

	public TAdventureTableMng() : base("Datas/AdventureTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Tables.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TAdventureTable data = new TAdventureTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Tables.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// AdventureTable
	TAdventureTableMng m_Adventure = new TAdventureTableMng();

	public TAdventureTable GetAdventureTable(int _idx) {
		if (!m_Adventure.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Adventure.DIC_Idx[_idx];
	}
	public List<TAdventureTable> GetAdventureTables() {
		return new List<TAdventureTable>(m_Adventure.DIC_Idx.Values);

	}

	public List<TAdventureTable> GetAdventureTables(int LV)
	{
		return m_Adventure.Tables.FindAll(o => o.m_OpenLevel <= LV + Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.AdventureLevelUp)));
	}
	public TAdventureTable GetAdventureTables(List<TAdventureTable> list, int value)
	{
		for (int i = 0; i < list.Count; i++)
		{
			TAdventureTable tdata = list[i];
			if (value < tdata.m_Prob) return tdata;
			value -= tdata.m_Prob;
		}
		return null;
	}

}

