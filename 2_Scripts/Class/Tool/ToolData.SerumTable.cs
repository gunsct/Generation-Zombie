using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TSerumTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 캐릭터 인덱스 </summary>
	public int m_GId;
	/// <summary> 블럭 위치 </summary>
	public int m_BlockPos;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Desc;
	/// <summary> 선행 혈청 </summary>
	public List<int> m_PrecedIdx = new List<int>();
	/// <summary> 재료 </summary>
	public int m_Material;
	/// <summary> 재료 수 </summary>
	public int m_MatCnt;
	/// <summary> 필요 재화 수 </summary>
	public int m_DollarCnt;
	/// <summary> 색상 </summary>
	public SerumColorType m_Color;
	/// <summary> 대상 타입 </summary>
	public SerumTargetType m_TargetType;
	/// <summary> 능력 타입 </summary>
	public StatType m_Type;
	/// <summary> 능력 값 </summary>
	public float m_Val;
	/// <summary> 값의 타입, 퍼센트 or 절대값 </summary>
	public StatValType m_ValType;

	public TSerumTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_GId = pResult.Get_Int32();
		m_BlockPos = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) {
			int pre = pResult.Get_Int32();
			if (pre != 0) m_PrecedIdx.Add(pre);
		}
		m_Material = pResult.Get_Int32();
		m_MatCnt = pResult.Get_Int32();
		m_DollarCnt = pResult.Get_Int32(); 
		m_Color = pResult.Get_Enum<SerumColorType>();
		m_TargetType = pResult.Get_Enum<SerumTargetType>();
		m_Type = pResult.Get_Enum<StatType>();
		m_Val = pResult.Get_Float();
		m_ValType = pResult.Get_Enum<StatValType>();
	}
}

public class TSerumTableMng : ToolFile
{
	public Dictionary<int, TSerumTable> DIC_Idx = new Dictionary<int, TSerumTable>();
	public Dictionary<int, Dictionary<int, List<TSerumTable>>> DIC_Group = new Dictionary<int, Dictionary<int, List<TSerumTable>>>();//캐릭터-블럭-리스트

	public TSerumTableMng() : base("Datas/SerumTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Group.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSerumTable data = new TSerumTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Group.ContainsKey(data.m_GId)) DIC_Group.Add(data.m_GId, new Dictionary<int, List<TSerumTable>>());
		if (!DIC_Group[data.m_GId].ContainsKey(data.m_BlockPos)) DIC_Group[data.m_GId].Add(data.m_BlockPos, new List<TSerumTable>());
		DIC_Group[data.m_GId][data.m_BlockPos].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SerumTable
	TSerumTableMng m_Serum = new TSerumTableMng();

	public TSerumTable GetSerumTable(int _idx) {
		if (!m_Serum.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Serum.DIC_Idx[_idx];
	}
	public List<TSerumTable> GetSerumTableGroup(int _gidx, int _pos) {
		if (!m_Serum.DIC_Group.ContainsKey(_gidx)) return null;
		if (!m_Serum.DIC_Group[_gidx].ContainsKey(_pos)) return null;
		return m_Serum.DIC_Group[_gidx][_pos];
	}
}
