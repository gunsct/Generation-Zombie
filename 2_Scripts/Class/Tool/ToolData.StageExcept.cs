using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TStageExceptTable : ClassMng
{
	public int m_Sidx;
	public StageDifficultyType m_Type;
	public List<int> m_Chars = new List<int>();
	public List<StageCardType> m_Cards = new List<StageCardType>();

	public TStageExceptTable(CSV_Result pResult) {
		// 인덱스 제거
		m_Sidx = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<StageDifficultyType>();
		for(int i = 0; i < 5; i++) {
			int cidx = pResult.Get_Int32();
			if (cidx != 0) m_Chars.Add(cidx);
		}
		for (int i = 0; i < 5; i++) {
			StageCardType type = pResult.Get_Enum<StageCardType>();
			if (type != StageCardType.None) m_Cards.Add(type);
		}
	}
	public bool IS_Contain(int _idx) {
		return m_Chars.Contains(_idx);
	}
}

public class TStageExceptTableMng : ToolFile
{
	public Dictionary<int, List<TStageExceptTable>> DIC_Type = new Dictionary<int, List<TStageExceptTable>>();

	public TStageExceptTableMng() : base("Datas/Stage_Except")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStageExceptTable data = new TStageExceptTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_Sidx)) DIC_Type.Add(data.m_Sidx, new List<TStageExceptTable>());
		DIC_Type[data.m_Sidx].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StageExcept
	TStageExceptTableMng m_StageExcept = new TStageExceptTableMng();

	public TStageExceptTable GeTStageExceptTable(int _idx, StageDifficultyType _diff) {
		if (!m_StageExcept.DIC_Type.ContainsKey(_idx)) return null;
		return m_StageExcept.DIC_Type[_idx].Find(o=>o.m_Type == _diff);
	}
}

