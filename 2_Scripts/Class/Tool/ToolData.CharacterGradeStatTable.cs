using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TCharacterGradeStatTable : ClassMng
{
	/// <summary> 레벨 </summary>
	public int m_Grade;
	/// <summary> 스텟별 가중치 </summary>
	public Dictionary<StatType, float> m_StatRatio = new Dictionary<StatType, float>();

	public TCharacterGradeStatTable(CSV_Result pResult) {
		m_Grade = pResult.Get_Int32();
		for (int i = (int)StatType.Men; i < (int)StatType.Max; i++)
			m_StatRatio.Add((StatType)i, pResult.Get_Float());
	}

	public float GetStatRatio(StatType stat)
	{
		if (!m_StatRatio.ContainsKey(stat)) return 1f;
		return m_StatRatio[stat];

	}
}

public class TCharacterGradeStatTableMng : ToolFile
{
	public Dictionary<int, TCharacterGradeStatTable> DIC_Grade = new Dictionary<int, TCharacterGradeStatTable>();
	public TCharacterGradeStatTableMng() : base("Datas/CharacterGradeStatTable")
	{
	}

	public override void DataInit()
	{
		DIC_Grade.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TCharacterGradeStatTable data = new TCharacterGradeStatTable(pResult);
		DIC_Grade.Add(data.m_Grade, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// CharacterGradeStatTable
	TCharacterGradeStatTableMng m_CharGradeStat = new TCharacterGradeStatTableMng();

	public TCharacterGradeStatTable GetCharGradeStatTable(int _grade) {
		if (!m_CharGradeStat.DIC_Grade.ContainsKey(_grade)) return null;
		return m_CharGradeStat.DIC_Grade[_grade];
	}
}

