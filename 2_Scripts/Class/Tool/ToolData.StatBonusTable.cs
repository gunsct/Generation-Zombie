using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TStatBonusTable : ClassMng
{
	/// <summary> 고유 랭크인덱스 </summary>
	public int m_Idx;
	public StatType m_Stat;
	public int m_StartLv;
	public int m_Gap;
	public float m_Val;

	public TStatBonusTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Stat = pResult.Get_Enum<StatType>();
		m_StartLv = pResult.Get_Int32();
		m_Gap = pResult.Get_Int32();
		m_Val = pResult.Get_Float();
	}
	public float GetVal(int _lv)
	{
		return Mathf.Max(0f, (((_lv + m_Gap) - m_StartLv) / m_Gap) * m_Val);
	}
}

public class TStatBonusTableMng : ToolFile
{
	public List<TStatBonusTable> DIC_Type = new List<TStatBonusTable>();

	public TStatBonusTableMng() : base("Datas/StatBonusTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStatBonusTable data = new TStatBonusTable(pResult);
		DIC_Type.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TStatBonusTableMng m_StatBonus = new TStatBonusTableMng();

	public TStatBonusTable GetStatBonusTable(StatType _stat) {
		return m_StatBonus.DIC_Type.Find(o => o.m_Stat == _stat);
	}
	public List<TStatBonusTable> GetAllStatBonusTable() {
		return m_StatBonus.DIC_Type;
	}
}

