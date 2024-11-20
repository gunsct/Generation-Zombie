using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TCharacterGradeTable : ClassMng
{
	/// <summary> 캐릭터 인덱스 </summary>
	public int m_CharIdx;
	/// <summary> 현 랭크 </summary>
	public int m_Rank;
	/// <summary> 해당 랭크에서 캐릭터가 달성 할 수 있는 최대 레벨  </summary>
	public int m_MaxLv;
	/// <summary> 재료 인덱스 </summary>
	public int m_MatIdx;
	/// <summary> 재료 수량 </summary>
	public int m_MatCount;
	/// <summary> 재화 수량 </summary>
	public int m_Money;

	public TCharacterGradeTable(CSV_Result pResult) {
		m_CharIdx = pResult.Get_Int32();
		m_Rank = pResult.Get_Int32();
		m_MaxLv = pResult.Get_Int32();
		m_MatIdx = pResult.Get_Int32();
		m_MatCount = pResult.Get_Int32();
		m_Money = pResult.Get_Int32();
	}
}
public class TCharacterGradeTableMng : ToolFile
{
	public Dictionary<int, Dictionary<int, TCharacterGradeTable>> DIC_Idx = new Dictionary<int, Dictionary<int, TCharacterGradeTable>>();
	public TCharacterGradeTableMng() : base("Datas/CharacterGradeTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TCharacterGradeTable data = new TCharacterGradeTable(pResult);
		if (!DIC_Idx.ContainsKey(data.m_CharIdx))
			DIC_Idx.Add(data.m_CharIdx, new Dictionary<int, TCharacterGradeTable>());
		if (!DIC_Idx[data.m_CharIdx].ContainsKey(data.m_Rank))
			DIC_Idx[data.m_CharIdx].Add(data.m_Rank, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// CharacterGradeTable
	TCharacterGradeTableMng m_CharGrade = new TCharacterGradeTableMng();

	public TCharacterGradeTable GetCharGradeTable(int _idx, int _rank) {
		if (!m_CharGrade.DIC_Idx.ContainsKey(_idx)) return null;
		if (!m_CharGrade.DIC_Idx[_idx].ContainsKey(_rank)) return null;
		return m_CharGrade.DIC_Idx[_idx][_rank];
	}
}

