using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPVPDefTable : ClassMng
{
	/// <summary> 고유 랭크인덱스 </summary>
	public int m_Idx;
	public PVPEquipAtkType m_AtkType;
	public int[] m_Ratios = new int[3];

	public TPVPDefTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_AtkType = pResult.Get_Enum<PVPEquipAtkType>();
		for (PVPArmorType i = PVPArmorType.LightArmor; i < PVPArmorType.Max; i++) m_Ratios[(int)i - 1] = pResult.Get_Int32();
	}
	public float GetRatio(PVPArmorType _armor) {
		return (float)m_Ratios[(int)_armor - 1] / 100f;
	}
}

public class TPVPDefTableMng : ToolFile
{
	public List<TPVPDefTable> DIC_Type = new List<TPVPDefTable>();

	public TPVPDefTableMng() : base("Datas/PvPDefTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVPDefTable data = new TPVPDefTable(pResult);
		DIC_Type.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVPDefTableMng m_PVPDef = new TPVPDefTableMng();

	public TPVPDefTable GeTPVPDefTable(PVPEquipAtkType _atk) {
		return m_PVPDef.DIC_Type.Find(o => o.m_AtkType == _atk);
	}
}

