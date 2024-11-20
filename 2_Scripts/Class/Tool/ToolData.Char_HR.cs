using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TChar_HRTable : ClassMng
{
	public int m_Idx;
	public CharHRType m_Category;
	public int m_CategoryName;
	public int m_TagName;
	public TChar_HRTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Category = pResult.Get_Enum<CharHRType>();
		m_CategoryName = pResult.Get_Int32();
		m_TagName = pResult.Get_Int32();
	}

	public string GetCategoryName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_CategoryName);
	}
	public string GetTagName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_TagName);
	}
}
public class TChar_HRTableMng : ToolFile
{
	public Dictionary<int, TChar_HRTable> DIC_Idx = new Dictionary<int, TChar_HRTable>();
	public Dictionary<CharHRType, List<TChar_HRTable>> DIC_Gidx = new Dictionary<CharHRType, List<TChar_HRTable>>();

	public TChar_HRTableMng() : base("Datas/Char_HR")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Gidx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TChar_HRTable data = new TChar_HRTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Gidx.ContainsKey(data.m_Category)) DIC_Gidx.Add(data.m_Category, new List<TChar_HRTable>());
		DIC_Gidx[data.m_Category].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// Char_HRTable
	TChar_HRTableMng m_Char_HR = new TChar_HRTableMng();

	public TChar_HRTable GetChar_HRTable(int _idx) {
		if (!m_Char_HR.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Char_HR.DIC_Idx[_idx];
	}
	public List<TChar_HRTable> GetChar_HRGroupTable(CharHRType _category) {
		if (!m_Char_HR.DIC_Gidx.ContainsKey(_category)) return null;
		return m_Char_HR.DIC_Gidx[_category];
	}
}

