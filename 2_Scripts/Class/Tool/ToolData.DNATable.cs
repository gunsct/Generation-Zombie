using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TDnaTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 내용 </summary>
	public int m_Desc;
	/// <summary> 등급 </summary>
	public int m_Grade;
	/// <summary> 옵션타입 </summary>
	public OptionType m_OptionType;
	/// <summary> 이미지 </summary>
	public string m_ImgName;
	/// <summary> 배경 타입 </summary>
	public DNABGType m_BGType;
	/// <summary> 옵션값 </summary>
	public float m_OptionVal;
	/// <summary> 랜덤 스텟 테이블 참조용 그룹 </summary>
	public int m_RandStatGroup;
	
	public TDnaTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_Grade = pResult.Get_Int32();
		m_OptionType = pResult.Get_Enum<OptionType>();
		m_ImgName = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_ImgName) && !m_ImgName.Contains("/"))
			Debug.LogError($"[ DnaTable ({m_Idx}) ] m_ImgName 패스 체크할것");
#endif
		m_BGType = pResult.Get_Enum<DNABGType>();
		m_OptionVal = pResult.Get_Float();
		m_RandStatGroup = pResult.Get_Int32();
	}
	
	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		float val = m_OptionVal * 100f;
		string desc = TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
		return string.Format(desc, val, val);
	}
	public string GetGradeGroupName(Grade grade)
	{
		return $"{BaseValue.GradeName((int) grade)}";
	}

	public Sprite GetIcon()
	{
		return UTILE.LoadImg(m_ImgName, "png");
	}
}
public class TDNATableMng : ToolFile
{
	public Dictionary<int, TDnaTable> DIC_Idx = new Dictionary<int, TDnaTable>();
	public TDNATableMng() : base("Datas/DNATable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TDnaTable data = new TDnaTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// DNATable
	TDNATableMng m_DNA = new TDNATableMng();

	public TDnaTable GetDnaTable(int _idx) {
		if (!m_DNA.DIC_Idx.ContainsKey(_idx)) return null;
		return m_DNA.DIC_Idx[_idx];
	}

	public TDnaTable GetDnaTable(OptionType _type) {
		List<TDnaTable> tables = new List<TDnaTable>(m_DNA.DIC_Idx.Values);
		return tables.Find(o => o.m_OptionType == _type);
	}

	public List<TDnaTable> GetAllDnaTable() {
		return new List<TDnaTable>(m_DNA.DIC_Idx.Values);
	}
}

