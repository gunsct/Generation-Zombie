using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TStageMakingTable : ClassMng
{
	public class MakeMaterial
	{
		/// <summary> 재료 </summary>
		public StageMaterialType m_Type;
		/// <summary> 개수 </summary>
		public int m_Cnt;
	}
	public class Condition
	{
		/// <summary> 재료 </summary>
		public StageMakingConditionType m_Type;
		/// <summary> 개수 </summary>
		public int m_Value;
	}
	/// <summary> 제작 카드 인덱스 </summary>
	public int m_CardIdx;
	/// <summary> 재료 타입,유틸이지만 다른 유틸의 재료일 경우 </summary>
	public StageMaterialType m_MatType;
	/// <summary> 필요 재료 정보 </summary>
	public List<MakeMaterial> m_Materal = new List<MakeMaterial>();
	public Condition m_Condition = new Condition();

	public TStageMakingTable(CSV_Result pResult)
	{
		m_CardIdx = pResult.Get_Int32();
		m_MatType = pResult.Get_Enum<StageMaterialType>();

		for (int i = 0; i < 2; i++)
		{
			StageMaterialType type = pResult.Get_Enum<StageMaterialType>();
			if (type == StageMaterialType.None)
			{
				pResult.NextReadPos();
				continue;
			}

			m_Materal.Add(new MakeMaterial()
			{
				m_Type = type,
				m_Cnt = pResult.Get_Int32()
			});
		}

		m_Condition.m_Type = pResult.Get_Enum<StageMakingConditionType>();
		m_Condition.m_Value = pResult.Get_Int32();
	}

	public bool IsUseMaterial(StageMaterialType type)
	{
		for (int i = m_Materal.Count - 1; i > -1; i--)
		{
			MakeMaterial mat = m_Materal[i];
			if (mat.m_Type == type) return true;
		}
		return false;
	}
}

public class TStageMakingTableMng : ToolFile
{
	public List<TStageMakingTable> Datas = new List<TStageMakingTable>();

	public TStageMakingTableMng() : base("Datas/StageMakingTable")
	{
	}

	public override void DataInit()
	{
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStageMakingTable data = new TStageMakingTable(pResult);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StageMakingTable
	TStageMakingTableMng m_StageMaking = new TStageMakingTableMng();

	public List<TStageMakingTable> GetStageMakingList(StageMaterialType type = StageMaterialType.None)
	{
		if(type != StageMaterialType.None) return m_StageMaking.Datas.FindAll(t => t.IsUseMaterial(type));
		return m_StageMaking.Datas;
	}

	public TStageMakingTable GetStageMakingData(int idx)
	{
		return m_StageMaking.Datas.Find(t => t.m_CardIdx == idx);
	}
	public TStageMakingTable GetStageMakingData(StageMaterialType _type) {
		return m_StageMaking.Datas.Find(t => t.m_MatType == _type);
	}

	public int GetMakingGrade(StageMaterialType _mattype) {
		switch (_mattype) {
			case StageMaterialType.Sniping:
			case StageMaterialType.ShockBomb:
			case StageMaterialType.MedBottle:
			case StageMaterialType.Bread:
			case StageMaterialType.Disinfectant:
			case StageMaterialType.Candle:
			case StageMaterialType.LightStick:
			case StageMaterialType.FireSpray:
			case StageMaterialType.FireBomb:
				return 1;
			case StageMaterialType.ShotGun:
			case StageMaterialType.Dynamite:
			case StageMaterialType.FirstAidKit:
			case StageMaterialType.Hamburger:
			case StageMaterialType.Soap:
			case StageMaterialType.Perfume:
			case StageMaterialType.FlashLight:
			case StageMaterialType.FireExtinguisher:
			case StageMaterialType.FireGun:
				return 2;
			case StageMaterialType.GatlingGun:
			case StageMaterialType.Grenade:
			case StageMaterialType.None:
				return 3;
			default: return 3;
		}
	}
}

