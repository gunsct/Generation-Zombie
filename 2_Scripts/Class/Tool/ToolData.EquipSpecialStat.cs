using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TEquipSpecialStat : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 전용 캐릭터 </summary>
	public int m_Char;
	/// <summary> 아이템 스텟 </summary>
	public ItemStat m_Stat;

	public TEquipSpecialStat(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Char = pResult.Get_Int32();
		m_Stat = new ItemStat() {
			m_Stat = pResult.Get_Enum<StatType>(),
			m_Val = pResult.Get_Int32()
		};
	}
}

public class TEquipSpecialStatMng : ToolFile
{
	public Dictionary<int, TEquipSpecialStat> DIC_Idx = new Dictionary<int, TEquipSpecialStat>();

	public TEquipSpecialStatMng() : base("Datas/EquipSpecialStat")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEquipSpecialStat data = new TEquipSpecialStat(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EquipSpecialStat
	TEquipSpecialStatMng m_EQSpecialStat = new TEquipSpecialStatMng();

	public TEquipSpecialStat GetEquipSpecialStat(int idx)
	{
		if (!m_EQSpecialStat.DIC_Idx.ContainsKey(idx)) return null;
		return m_EQSpecialStat.DIC_Idx[idx];
	}
}
