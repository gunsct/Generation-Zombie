using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TReassemblyTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 재료 타입 </summary>
	public ItemType m_MatType;
	/// <summary> 나올 장비의 확률들 </summary>
	public int[] m_EquipProbs = new int[(int)EquipType.Max];

	public TReassemblyTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_MatType = pResult.Get_Enum<ItemType>();
		for(int i = 0;i< m_EquipProbs.Length; i++) {
			m_EquipProbs[i] = pResult.Get_Int32();
		}
	}

	public EquipType GetRandEquipType() {//2 1 3 4 2 = 12 0~11
		int allprop = m_EquipProbs.Sum();
		int rand = UTILE.Get_Random(0, allprop);
		int prop = 0;
		for(int i = 0;i< m_EquipProbs.Length; i++)
		{
			if (m_EquipProbs[i] == 0) continue;
			if (rand < prop) return (EquipType)i;
			rand -= m_EquipProbs[i];
		}
		return EquipType.Weapon;
	}
}

public class TReassemblyTableMng : ToolFile
{
	public Dictionary<ItemType, TReassemblyTable> DIC_Type = new Dictionary<ItemType, TReassemblyTable>();

	public TReassemblyTableMng() : base("Datas/ReassemblyTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TReassemblyTable data = new TReassemblyTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_MatType)) DIC_Type.Add(data.m_MatType, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ReassemblyTable
	TReassemblyTableMng m_Reassembly = new TReassemblyTableMng();

	public TReassemblyTable GetReassemblyTable(ItemType _type) {
		if (!m_Reassembly.DIC_Type.ContainsKey(_type)) return null;
		return m_Reassembly.DIC_Type[_type];
	}
}
