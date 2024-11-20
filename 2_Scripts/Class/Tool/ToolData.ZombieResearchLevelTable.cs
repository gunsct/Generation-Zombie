using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TZombieResearchLevelTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_LV;
	/// <summary> 필요 경험치 </summary>
	public long m_Exp;
	/// <summary> 데미지 비율, 소수점으로 입력 </summary>
	public float m_Value;

	public TZombieResearchLevelTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_LV = pResult.Get_Int32();
		m_Exp = pResult.Get_Int64();
		m_Value = pResult.Get_Float();
	}
}
public class TZombieResearchLevelTableMng : ToolFile
{
	public Dictionary<int, List<TZombieResearchLevelTable>> DIC_Idx = new Dictionary<int, List<TZombieResearchLevelTable>>();

	public TZombieResearchLevelTableMng() : base("Datas/ZombieResearchLevelTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TZombieResearchLevelTable data = new TZombieResearchLevelTable(pResult);
		if (!DIC_Idx.ContainsKey(data.m_Idx)) DIC_Idx.Add(data.m_Idx, new List<TZombieResearchLevelTable>());
		DIC_Idx[data.m_Idx].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ZombieResearchLevelTable
	TZombieResearchLevelTableMng m_ZombieResearchLV = new TZombieResearchLevelTableMng();

	public TZombieResearchLevelTable GetZombieResearchLevelTable(int _Group, int LV) {
		if (!m_ZombieResearchLV.DIC_Idx.ContainsKey(_Group)) return null;
		return m_ZombieResearchLV.DIC_Idx[_Group].Find(o => o.m_LV == LV);
	}
	public List<TZombieResearchLevelTable> GetZombieResearchLevelTables(int _Group)
	{
		if (!m_ZombieResearchLV.DIC_Idx.ContainsKey(_Group)) return null;
		return m_ZombieResearchLV.DIC_Idx[_Group];
	}
}

