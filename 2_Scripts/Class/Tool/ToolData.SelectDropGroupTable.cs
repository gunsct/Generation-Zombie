using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TSelectDropGroupTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary>  </summary>
	public int m_Gid;
	/// <summary> 보상 카드 인덱스 </summary>
	public int m_Val;
	/// <summary> 뽑힐 확률 </summary>
	public int m_Prop;

	public TSelectDropGroupTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Gid = pResult.Get_Int32();
		m_Val = pResult.Get_Int32();
		m_Prop = pResult.Get_Int32();
	}
}
public class TSelectDropGroupTableMng : ToolFile
{
	public Dictionary<int, TSelectDropGroupTable> DIC_Idx = new Dictionary<int, TSelectDropGroupTable>();
	public Dictionary<int, List<TSelectDropGroupTable>> DIC_GID = new Dictionary<int, List<TSelectDropGroupTable>>();

	public TSelectDropGroupTableMng() : base("Datas/SelectDropGroupTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSelectDropGroupTable data = new TSelectDropGroupTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_GID.ContainsKey(data.m_Gid)) DIC_GID.Add(data.m_Gid, new List<TSelectDropGroupTable>());
		DIC_GID[data.m_Gid].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SelectDropGroupTable
	TSelectDropGroupTableMng m_SelectDropGroup = new TSelectDropGroupTableMng();

	public TSelectDropGroupTable GetSelectDropGroupTable(int _idx) {
		if (!m_SelectDropGroup.DIC_Idx.ContainsKey(_idx)) return null;
		return m_SelectDropGroup.DIC_Idx[_idx];
	}
	public TSelectDropGroupTable GetRandSelectDropGroupTable(int _gid, List<TSelectDropGroupTable> _ignore = null) {
		if (!m_SelectDropGroup.DIC_GID.ContainsKey(_gid)) return null;
		List<TSelectDropGroupTable> list = m_SelectDropGroup.DIC_GID[_gid].Where(o => !_ignore.Contains(o)).ToList();

		int allprop = list.Sum(t => t.m_Prop);
		int randprop = UTILE.Get_Random(0, allprop);
		int nowprop = 0;
		for(int i = 0;i< list.Count; i++) {
			int preprop = nowprop;
			nowprop += list[i].m_Prop;
			if (preprop <= randprop && randprop < nowprop) return list[i];
		}
		return null;
	}
}

