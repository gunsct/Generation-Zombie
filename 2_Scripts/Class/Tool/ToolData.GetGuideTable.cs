using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TGetGuideTable : ClassMng
{
	public int m_Idx;
	public int m_Gid;
	public int m_MainName;
	public ContentType m_GoContent;
	public TGetGuideTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Gid = pResult.Get_Int32();
		m_MainName = pResult.Get_Int32();
		m_GoContent = pResult.Get_Enum<ContentType>();
	}

	public string GetMainName() {
		return TDATA.GetString(ToolData.StringTalbe.UI, m_MainName);
	}
}
public class TGetGuideTableMng : ToolFile
{
	public Dictionary<int, TGetGuideTable> DIC_Idx = new Dictionary<int, TGetGuideTable>();
	public Dictionary<int, List<TGetGuideTable>> DIC_Gidx = new Dictionary<int, List<TGetGuideTable>>();

	public TGetGuideTableMng() : base("Datas/GetGuideTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Gidx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TGetGuideTable data = new TGetGuideTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Gidx.ContainsKey(data.m_Gid)) DIC_Gidx.Add(data.m_Gid, new List<TGetGuideTable>());
		DIC_Gidx[data.m_Gid].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// GetGuideTable
	TGetGuideTableMng m_GetGuide = new TGetGuideTableMng();

	public TGetGuideTable GetGetGuideTable(int _idx) {
		if (!m_GetGuide.DIC_Idx.ContainsKey(_idx)) return null;
		return m_GetGuide.DIC_Idx[_idx];
	}
	public List<TGetGuideTable> GetGetGuideGroupTable(int _gid) {
		if (!m_GetGuide.DIC_Gidx.ContainsKey(_gid)) return null;
		return m_GetGuide.DIC_Gidx[_gid];
	}
}

