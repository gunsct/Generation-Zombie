using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TSerumBlockTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 블록을 열기 위해 요구되는 캐릭터 등급 </summary>
	public int m_NeedCharLv;
	/// <summary> 오픈에 요구되는 아이템 Count </summary>
	public int m_NeedItemCnt;

	public TSerumBlockTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_NeedCharLv = pResult.Get_Int32();
		m_NeedItemCnt = pResult.Get_Int32();
	}
}

public class TSerumBlockTableMng : ToolFile
{
	public Dictionary<int, TSerumBlockTable> DIC_Idx = new Dictionary<int, TSerumBlockTable>();
	public List<TSerumBlockTable> Tables = new List<TSerumBlockTable>();
	public TSerumBlockTableMng() : base("Datas/SerumBlockTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Tables.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSerumBlockTable data = new TSerumBlockTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Tables.Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SerumBlockTable
	TSerumBlockTableMng m_SerumBlock = new TSerumBlockTableMng();

	public TSerumBlockTable GetSerumBlockTable(int _idx) {
		if (!m_SerumBlock.DIC_Idx.ContainsKey(_idx)) return null;
		return m_SerumBlock.DIC_Idx[_idx];
	}
	public TSerumBlockTable GetSerumBlockTableGrade(int _lv) {
		return m_SerumBlock.Tables.Find(o => o.m_NeedCharLv == _lv);
	}
}
