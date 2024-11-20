using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolData;

public class TEquipGachaTable : ClassMng
{
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary> 다음 레벨을 달성하기 위해 필요한 가차 횟수 </summary>
	public int m_Exp;
	/// <summary> 해당 레벨에 가차를 돌렸을 경우 굴려주는 가차 그룹 목록 </summary>
	public int m_Gid;
	/// <summary> 피스메이커 레벨에 따른 효과 </summary>
	public EquipGachaEffectType m_EffectType;
	public float m_EffectVal;

	public TEquipGachaTable(CSV_Result pResult)
	{
		m_Lv = pResult.Get_Int32();
		m_Exp = pResult.Get_Int32();
		m_Gid = pResult.Get_Int32();
		m_EffectType = pResult.Get_Enum<EquipGachaEffectType>();
		m_EffectVal = pResult.Get_Float();
	}
	public string GetEffectDesc() {
		int sidx = 0;
		switch (m_EffectType) {
			case EquipGachaEffectType.WeaponStatUp: sidx = 25; break; 
			case EquipGachaEffectType.HelmetStatUp: sidx = 26; break;
			case EquipGachaEffectType.CostumeStatUp: sidx = 27; break;
			case EquipGachaEffectType.ShoesStatUp: sidx = 28; break;
			case EquipGachaEffectType.AccStatUp: sidx = 29; break;
		}
		return string.Format("{0} {1:0.##}", TDATA.GetString(sidx), string.Format(TDATA.GetString(10827), m_EffectVal * 100));
	}
}

public class TEquipGachaTableMng : ToolFile
{
	public Dictionary<int, TEquipGachaTable> DIC_Idx = new Dictionary<int, TEquipGachaTable>();

	public TEquipGachaTableMng() : base("Datas/EquipGachaTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEquipGachaTable data = new TEquipGachaTable(pResult);
		DIC_Idx.Add(data.m_Lv, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EquipGachaTable
	TEquipGachaTableMng m_EquipGacha = new TEquipGachaTableMng();

	public TEquipGachaTable GetEquipGachaTable(int idx)
	{
		if (!m_EquipGacha.DIC_Idx.ContainsKey(idx)) return null;
		return m_EquipGacha.DIC_Idx[idx];
	}
	public List<TEquipGachaTable> GetEquipGachaTableList() {
		return new List<TEquipGachaTable>(m_EquipGacha.DIC_Idx.Values);
	}
}
