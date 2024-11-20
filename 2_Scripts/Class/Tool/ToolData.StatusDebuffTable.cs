using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TStatusDebuffTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> </summary>
	public StatType m_StatType;
	/// <summary> </summary>
	public float m_StatVal;
	/// <summary> </summary>
	public DebuffType m_DebuffType;
	/// <summary> </summary>
	public float m_DebuffVal;
	/// <summary> 스테이지 난이도 </summary>
	public int m_StgIdx;
	/// <summary> </summary>
	public string m_Icon;
	/// <summary> 디버프 제목 </summary>
	public int m_Name;
	/// <summary> 디버프 설명 </summary>
	public int m_Desc;

	public TStatusDebuffTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_StatType = pResult.Get_Enum<StatType>();
		m_StatVal = pResult.Get_Float();
		m_DebuffType = pResult.Get_Enum<DebuffType>();
		m_DebuffVal = pResult.Get_Float();
		m_StgIdx = pResult.Get_Int32();
		m_Icon = pResult.Get_String();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
	}

	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
	}
}
public class TStatusDebuffTableMng : ToolFile
{
	public Dictionary<int, TStatusDebuffTable> DIC_Idx = new Dictionary<int, TStatusDebuffTable>();
	public Dictionary<int, Dictionary<StatType, List<TStatusDebuffTable>>> DIC_Type = new Dictionary<int, Dictionary<StatType, List<TStatusDebuffTable>>>();

	public TStatusDebuffTableMng() : base("Datas/StatusDebuffTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStatusDebuffTable data = new TStatusDebuffTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_StgIdx)) DIC_Type.Add(data.m_StgIdx, new Dictionary<StatType, List<TStatusDebuffTable>>());
		if (!DIC_Type[data.m_StgIdx].ContainsKey(data.m_StatType)) DIC_Type[data.m_StgIdx].Add(data.m_StatType, new List<TStatusDebuffTable>());
		DIC_Type[data.m_StgIdx][data.m_StatType].Add(data);

		if (!DIC_Idx.ContainsKey(data.m_Idx)) DIC_Idx.Add(data.m_Idx, data);
	}

	public override void CheckData()
	{
		//오름차순 정렬
		for (int i = 0; i < DIC_Type.Count; i++)
		{
			for (int j = 0; j < DIC_Type.ElementAt(i).Value.Count; j++)
				DIC_Type.ElementAt(i).Value.ElementAt(j).Value.Sort((TStatusDebuffTable _before, TStatusDebuffTable _after) => {
					return _before.m_StatVal.CompareTo(_after.m_StatVal);
				});
		}
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StatusDebuffTable
	TStatusDebuffTableMng m_StatusDebuff = new TStatusDebuffTableMng();
	public TStatusDebuffTable GetStatusDebuffTable(int _idx) {
		if (!m_StatusDebuff.DIC_Idx.ContainsKey(_idx)) return null;
		return m_StatusDebuff.DIC_Idx[_idx];
	}
	public TStatusDebuffTable GetStatusDebuffTable(int _stgidx, StatType _stat, float _now, float _max)
	{
		TStatusDebuffTable table = null;
		int stgidx = GetDiffStgIdx(_stgidx);
		if (!m_StatusDebuff.DIC_Type.ContainsKey(stgidx)) return null;
		for(int i = 0; i < m_StatusDebuff.DIC_Type[stgidx][_stat].Count; i++) {
			if (Mathf.RoundToInt(m_StatusDebuff.DIC_Type[stgidx][_stat][i].m_StatVal * _max) >= Mathf.RoundToInt(_now)) {
				table = m_StatusDebuff.DIC_Type[stgidx][_stat][i];
				break;
			}
		}
		return table;
	}
	public int GetStatusDebuffPos(TStatusDebuffTable _table) {//수치 낮은 순으로
		return m_StatusDebuff.DIC_Type[_table.m_StgIdx][_table.m_StatType].Count - m_StatusDebuff.DIC_Type[_table.m_StgIdx][_table.m_StatType].IndexOf(_table);
	}
	public TStatusDebuffTable GetStatusDebuffFirst(int _stgidx, StatType _stat) {
		return m_StatusDebuff.DIC_Type[GetDiffStgIdx(_stgidx)][_stat][1];
	}
	int GetDiffStgIdx(int _idx) {
		if (_idx < BaseValue.HARDSTGIDX) return BaseValue.NORMALSTGIDX;
		else if (_idx < BaseValue.NIGHTMARESTGIDX) return BaseValue.HARDSTGIDX;
		else return BaseValue.NIGHTMARESTGIDX;
	}
}
