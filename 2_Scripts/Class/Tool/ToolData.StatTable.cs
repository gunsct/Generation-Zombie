using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TStatTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 스탯, HP, Atk, Def, Sta, RecSta </summary>
	public Dictionary<StatType, float> m_Stat = new Dictionary<StatType, float>();

	public TStatTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();

		m_Stat.Add(StatType.Men, pResult.Get_Float());
		m_Stat.Add(StatType.Hyg, pResult.Get_Float());
		m_Stat.Add(StatType.Sat, pResult.Get_Float());

		m_Stat.Add(StatType.HP, pResult.Get_Float());
		m_Stat.Add(StatType.Atk, pResult.Get_Float());
		m_Stat.Add(StatType.Def, pResult.Get_Float());
		m_Stat.Add(StatType.Sta, pResult.Get_Float());
		m_Stat.Add(StatType.RecSta, pResult.Get_Float());
		m_Stat.Add(StatType.Guard, pResult.Get_Float());
		m_Stat.Add(StatType.Heal, pResult.Get_Float());

		m_Stat.Add(StatType.NormalNote, pResult.Get_Float());
		m_Stat.Add(StatType.SlashNote, pResult.Get_Float());
		m_Stat.Add(StatType.ComboNote, pResult.Get_Float());
		m_Stat.Add(StatType.ChargeNote, pResult.Get_Float());
		m_Stat.Add(StatType.ChainNote, pResult.Get_Float());

		m_Stat.Add(StatType.Speed, pResult.Get_Float());
		m_Stat.Add(StatType.Critical, pResult.Get_Float());
		m_Stat.Add(StatType.CriticalDmg, pResult.Get_Float());
		m_Stat.Add(StatType.HeadShot, pResult.Get_Float());
	}

	public float GetStat(StatType _type, int _lv) {
		if (!m_Stat.ContainsKey(_type)) return 0;
		switch (_type) {
			case StatType.Atk:
			case StatType.Def:
			case StatType.HP:
			case StatType.Heal:
			case StatType.Speed:
				return BaseValue.GetStat(_lv, m_Stat[_type]);
			default:
				return m_Stat[_type];
		}
	}
}
public class TStatTableMng : ToolFile
{
	public Dictionary<int, TStatTable> DIC_Idx = new Dictionary<int, TStatTable>();

	public TStatTableMng() : base("Datas/StatTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStatTable data = new TStatTable(pResult);
		if (!DIC_Idx.ContainsKey(data.m_Idx))
			DIC_Idx.Add(data.m_Idx, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StatTable
	TStatTableMng m_Stat = new TStatTableMng();

	public TStatTable GetStatTable(int _idx) {
		if (!m_Stat.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Stat.DIC_Idx[_idx];
	}
}

