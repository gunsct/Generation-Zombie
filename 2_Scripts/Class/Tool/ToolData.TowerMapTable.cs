using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TTowerMapTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 스테이지 </summary>
	public int m_Stg;
	/// <summary> 스테이지 난이도 유형 </summary>
	public StageDifficultyType m_DifficultyType;
	/// <summary> 배경 프리셋 </summary>
	public string m_BGPreset;
	/// <summary> 행 </summary>
	public int m_Row;
	/// <summary> 열 </summary>
	public int m_Column;
	/// <summary> 좌-중-우 연결되는 인덱스 </summary>
	public int[] m_Ways = new int[3];
	/// <summary> 이벤트 타입 </summary>
	public TowerEventType m_EventType;
	/// <summary> 이벤트 밸류</summary>
	public int m_EventVal;

	public TTowerMapTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Stg = pResult.Get_Int32();
		m_DifficultyType = pResult.Get_Enum<StageDifficultyType>();
		m_BGPreset = pResult.Get_String();
		m_Row = pResult.Get_Int32();
		m_Column = pResult.Get_Int32();
		for(int i = 0;i<3;i++) m_Ways[i] = pResult.Get_Int32();
		m_EventType = pResult.Get_Enum<TowerEventType>();
		m_EventVal = pResult.Get_Int32();
	}
}
public class TTowerMapTableMng : ToolFile
{
	public Dictionary<int, TTowerMapTable> DIC_Idx = new Dictionary<int, TTowerMapTable>();
	public Dictionary<StageDifficultyType, Dictionary<int, List<TTowerMapTable>>> DIC_Type = new Dictionary<StageDifficultyType, Dictionary<int, List<TTowerMapTable>>>();

	public TTowerMapTableMng() : base("Datas/TowerMapTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TTowerMapTable data = new TTowerMapTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Type.ContainsKey(data.m_DifficultyType)) DIC_Type.Add(data.m_DifficultyType, new Dictionary<int, List<TTowerMapTable>>());
		if (!DIC_Type[data.m_DifficultyType].ContainsKey(data.m_Stg)) DIC_Type[data.m_DifficultyType].Add(data.m_Stg, new List<TTowerMapTable>());
		DIC_Type[data.m_DifficultyType][data.m_Stg].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// TowerMapTable
	TTowerMapTableMng m_TowerMap = new TTowerMapTableMng();

	public TTowerMapTable GetTowerMapTable(int _idx) {
		if (!m_TowerMap.DIC_Idx.ContainsKey(_idx)) return null;
		return m_TowerMap.DIC_Idx[_idx];
	}
	public List<TTowerMapTable> GetTowerMapGroupTable(StageDifficultyType _diff, int _lv) {
		if (!m_TowerMap.DIC_Type.ContainsKey(_diff)) return null;
		if (!m_TowerMap.DIC_Type[_diff].ContainsKey(_lv)) return null;
		return m_TowerMap.DIC_Type[_diff][_lv];
	}
}

