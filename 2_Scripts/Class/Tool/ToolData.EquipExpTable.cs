using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TEquipExpTable : ClassMng
{
	/// <summary> 장착부위 </summary>
	public EquipType m_EqType;
	/// <summary> 등급 </summary>
	public int m_Grade;
	/// <summary> 레벨 </summary>
	public int m_LV;
	/// <summary> 레벨(0:필요경험치, 1:해당 레벨의 아이템 재료 경험치) </summary>
	public int[] m_Exp = new int[2];

	public TEquipExpTable(CSV_Result pResult)
	{
		m_EqType = pResult.Get_Enum<EquipType>();
		m_Grade = pResult.Get_Int32();
		m_LV = pResult.Get_Int32();
		m_Exp[0] = pResult.Get_Int32();
		m_Exp[1] = pResult.Get_Int32();
	}
}
public class TEquipExpTableMng : ToolFile
{
	public Dictionary<EquipType, Dictionary<int, Dictionary<int, TEquipExpTable>>> DIC_Type = new Dictionary<EquipType, Dictionary<int, Dictionary<int, TEquipExpTable>>>();

	public TEquipExpTableMng() : base("Datas/EquipExpTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEquipExpTable data = new TEquipExpTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_EqType)) DIC_Type.Add(data.m_EqType, new Dictionary<int, Dictionary<int, TEquipExpTable>>());
		if (!DIC_Type[data.m_EqType].ContainsKey(data.m_Grade)) DIC_Type[data.m_EqType].Add(data.m_Grade, new Dictionary<int, TEquipExpTable>());
		DIC_Type[data.m_EqType][data.m_Grade].Add(data.m_LV, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EquipExpTable
	TEquipExpTableMng m_EQExp = new TEquipExpTableMng();

	public TEquipExpTable GetEquipExpTable(EquipType eqpos, int grade, int lv)
	{
		if (!m_EQExp.DIC_Type.ContainsKey(eqpos)) return null;
		if (!m_EQExp.DIC_Type[eqpos].ContainsKey(grade)) return null;
		if (!m_EQExp.DIC_Type[eqpos][grade].ContainsKey(lv)) return null;
		return m_EQExp.DIC_Type[eqpos][grade][lv];
	}
}
