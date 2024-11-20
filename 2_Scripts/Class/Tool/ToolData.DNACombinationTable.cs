using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TDNACombinationTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 등급 </summary>
	public int m_Grade;
	/// <summary> 달러 </summary>
	public int m_Dollar;
	/// <summary> 확율 10만분율 </summary>
	public int m_Prob;


	public TDNACombinationTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Grade = pResult.Get_Int32();
		m_Dollar = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
	}
}
public class TDNACombinationTableMng : ToolFile
{
	public Dictionary<int, TDNACombinationTable> DIC_Grade = new Dictionary<int, TDNACombinationTable>();
	public TDNACombinationTableMng() : base("Datas/DNACombinationTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_Grade.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TDNACombinationTable data = new TDNACombinationTable(pResult);
		DIC_Grade.Add(data.m_Grade, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// DnaCombinationTable
	TDNACombinationTableMng m_DNACombination = new TDNACombinationTableMng();

	public TDNACombinationTable GetDnaCombinationTable(int grade) {
		if (!m_DNACombination.DIC_Grade.ContainsKey(grade)) return null;
		return m_DNACombination.DIC_Grade[grade];
	}
}

