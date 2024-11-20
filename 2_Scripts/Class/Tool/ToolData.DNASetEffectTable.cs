using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TDNASetEffectTable : ClassMng
{
	public int m_Idx;
	/// <summary> 색상 타입 </summary>
	public DNABGType m_Type;
	/// <summary> 장착 수 </summary>
	public int m_Cnt;
	/// <summary> 세트 효과 타입 </summary>
	public StatType m_SetFXType;
	/// <summary> 세트 효과 값 </summary>
	public float m_SetFXVal;

	public TDNASetEffectTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<DNABGType>();
		m_Cnt = pResult.Get_Int32();
		m_SetFXType = pResult.Get_Enum<StatType>();
		m_SetFXVal = pResult.Get_Float();
	}
	
	public string GetGradeGroupName(Grade grade)
	{
		return $"{BaseValue.GradeName((int) grade)}";
	}
}
public class TDNASetEffectTableMng : ToolFile
{
	public Dictionary<DNABGType, Dictionary<int, TDNASetEffectTable>> DIC_Idx = new Dictionary<DNABGType, Dictionary<int, TDNASetEffectTable>>();
	public TDNASetEffectTableMng() : base("Datas/DNASetEffectTable")
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
		TDNASetEffectTable data = new TDNASetEffectTable(pResult);
		if(!DIC_Idx.ContainsKey(data.m_Type))
			DIC_Idx.Add(data.m_Type, new Dictionary<int, TDNASetEffectTable>());
		DIC_Idx[data.m_Type].Add(data.m_Cnt, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// DNASetEffectTable
	TDNASetEffectTableMng m_DNASetEffect = new TDNASetEffectTableMng();

	public List<TDNASetEffectTable> GetDNASetFXTables(DNABGType _colortype) {
		if (!m_DNASetEffect.DIC_Idx.ContainsKey(_colortype)) return null;
		return m_DNASetEffect.DIC_Idx[_colortype].Values.ToList();
	}
	public TDNASetEffectTable GetDNASetFXTable(DNABGType _colortype, int _cnt) {
		if (!m_DNASetEffect.DIC_Idx.ContainsKey(_colortype)) return null;
		while(_cnt > 0) {//혹시 1개도 세트로 하거나 5개 미만을 최대로 잡을 경우..
			if (m_DNASetEffect.DIC_Idx[_colortype].ContainsKey(_cnt)) {
				return m_DNASetEffect.DIC_Idx[_colortype][_cnt];
			}
			_cnt--;
		}
		return null;
	}
	public List<TDNASetEffectTable> GetDNASetFXTables(DNABGType _colortype, int _cnt) {
		if (!m_DNASetEffect.DIC_Idx.ContainsKey(_colortype)) return null;
		List<TDNASetEffectTable> sets = new List<TDNASetEffectTable>();
		while (_cnt > 0) {//혹시 1개도 세트로 하거나 5개 미만을 최대로 잡을 경우..
			if (m_DNASetEffect.DIC_Idx[_colortype].ContainsKey(_cnt)) {
				sets.Add(m_DNASetEffect.DIC_Idx[_colortype][_cnt]);
			}
			_cnt--;
		}
		return sets;
	}
}

