using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TExpTable : ClassMng
{
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary> 해당 레벨을 달성하기 위한 요구 경험치 </summary>
	public int m_Exp;
	/// <summary> 해당 레벨을 달성하기 위한 요구 달러 </summary>
	public int m_Money;
	/// <summary> 계정 레벨 요구 경험치 </summary>
	public int m_UserExp;
	public TExpTable(CSV_Result pResult) {
		m_Lv = pResult.Get_Int32();
		m_Exp = pResult.Get_Int32();
		m_Money = pResult.Get_Int32();
		m_UserExp = pResult.Get_Int32();
	}
}
public class TExpTableMng : ToolFile
{
	public Dictionary<int, TExpTable> DIC_Lv = new Dictionary<int, TExpTable>();

	public TExpTableMng() : base("Datas/ExpTable")
	{
	}

	public override void DataInit()
	{
		DIC_Lv.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TExpTable data = new TExpTable(pResult);
		DIC_Lv.Add(data.m_Lv, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ExpTable
	TExpTableMng m_Exp = new TExpTableMng();

	public TExpTable GetExpTable(int _lv) {
		if (!m_Exp.DIC_Lv.ContainsKey(_lv)) return null;
		return m_Exp.DIC_Lv[_lv];
	}
	public int GetExpTableCnt() { return m_Exp.DIC_Lv.Count; }
}

