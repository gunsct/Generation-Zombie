using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TStageGuideTable : ClassMng
{
	public int m_CardIdx;
	public int m_StgIdx;

	public TStageGuideTable(CSV_Result pResult) {
		m_CardIdx = pResult.Get_Int32();
		m_StgIdx = pResult.Get_Int32();
	}
}
public class TStageGuideTableMng : ToolFile
{
	public Dictionary<int, List<TStageGuideTable>> DIC_Type = new Dictionary<int, List<TStageGuideTable>>();

	public TStageGuideTableMng() : base("Datas/StageGuideTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStageGuideTable data = new TStageGuideTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_StgIdx)) DIC_Type.Add(data.m_StgIdx, new List<TStageGuideTable>());
		DIC_Type[data.m_StgIdx].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StageGuideTable
	TStageGuideTableMng m_StageGuide = new TStageGuideTableMng();

	public List<TStageGuideTable> GetStageGuideTable(int _stgidx) {
		if (m_StageGuide.DIC_Type.ContainsKey(_stgidx)) return m_StageGuide.DIC_Type[_stgidx];
		return null;
	}
}

