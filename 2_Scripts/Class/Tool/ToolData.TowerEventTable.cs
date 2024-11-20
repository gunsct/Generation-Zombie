using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TTowerEventTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 그룹 인덱스 </summary>
	public int m_Gidx;
	/// <summary> 이벤트 타입 </summary>
	public TowerSOEventType m_Type;
	/// <summary> 이벤트 밸류 </summary>
	public int m_Val;
	/// <summary> 확률 </summary>
	public int m_Prob;

	public TTowerEventTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Gidx = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<TowerSOEventType>();
		m_Val = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
	}
	/// <summary> Secret or Open 체크</summary>
	public TowerSOType GetSOType() {
		switch (m_Type) {
			case TowerSOEventType.NormalSupplyBox:
			case TowerSOEventType.EpicSupplyBox:
			case TowerSOEventType.Refugee:
			case TowerSOEventType.Rest:
				return TowerSOType.All;
			case TowerSOEventType.BadSupplyBox:
			case TowerSOEventType.StatusBuffEvent:
			case TowerSOEventType.SuddenAttack:
				return TowerSOType.Secret;
			//return TowerSOType.Open;
		}
		return TowerSOType.All;
	}
}
public class TTowerEventTableMng : ToolFile
{
	public Dictionary<int, TTowerEventTable> DIC_Idx = new Dictionary<int, TTowerEventTable>();
	public Dictionary<int, List<TTowerEventTable>> DIC_GID = new Dictionary<int, List<TTowerEventTable>>();

	public TTowerEventTableMng() : base("Datas/TowerEventTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TTowerEventTable data = new TTowerEventTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_GID.ContainsKey(data.m_Gidx)) DIC_GID.Add(data.m_Gidx, new List<TTowerEventTable>());
		DIC_GID[data.m_Gidx].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// TowerEventTable
	TTowerEventTableMng m_TowerEvent = new TTowerEventTableMng();

	public TTowerEventTable GetTowerEventTable(int _idx) {
		if (!m_TowerEvent.DIC_Idx.ContainsKey(_idx)) return null;
		return m_TowerEvent.DIC_Idx[_idx];
	}
	public List<TTowerEventTable> GetTowerEventGroupTable(int _gid, TowerSOType _sotype) {
		if (!m_TowerEvent.DIC_GID.ContainsKey(_gid)) return null;
		List<TTowerEventTable> tables = m_TowerEvent.DIC_GID[_gid].FindAll(t => t.GetSOType() == _sotype || t.GetSOType() == TowerSOType.All);
		return tables;
	}
	public TTowerEventTable GetTowerEventRandTable(int _gid, TowerSOType _sotype) {
		if (!m_TowerEvent.DIC_GID.ContainsKey(_gid)) return null;
		List<TTowerEventTable> tables = m_TowerEvent.DIC_GID[_gid].FindAll(t => t.GetSOType() == _sotype || t.GetSOType() == TowerSOType.All);
		int allprop = tables.Sum(t => t.m_Prob);
		int randprop = UTILE.Get_Random(0, allprop);
		int nowprop = 0;
		for(int i = 0;i< tables.Count; i++) {
			int preprop = nowprop;
			nowprop += tables[i].m_Prob;
			if (preprop <= randprop && randprop < nowprop) return tables[i];
		}
		return null;
	}
}

