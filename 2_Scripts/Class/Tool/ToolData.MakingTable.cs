using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum MakingGroup
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 장비 </summary>
	Equip,
	/// <summary> 인사기록 </summary>
	PrivateEquip,
	/// <summary> 연구재료 </summary>
	ResearchMaterial,
	/// <summary> 생산 재료 </summary>
	MakingMaterial,
	/// <summary> DNA </summary>
	DNA,
	/// <summary>  </summary>
	End

}

public class TMakingTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_ItemIdx;
	/// <summary> 그룹 </summary>
	public MakingGroup m_Group;
	/// <summary> MakingGroup.Equip : 장착부위 </summary>
	public int m_Category;
	/// <summary> 제작 레벨  </summary>
	public int m_LV;
	/// <summary> 해당 제작의 우선 순위  </summary>
	public int m_Point;
	/// <summary> 개당 제작 시간 </summary>
	public int m_Time;
	/// <summary> 개당 제작 비용 </summary>
	public int m_Dollar;
	public class MakeMat
	{
		public int m_Idx;
		public int m_Count;
	}
	/// <summary> 개당 재료 </summary>
	public List<MakeMat> m_Mats = new List<MakeMat>();

	public TMakingTable(CSV_Result pResult) {
		m_ItemIdx = pResult.Get_Int32();
		m_Group = pResult.Get_Enum<MakingGroup>();
		m_Category = pResult.Get_Int32();
		m_LV = pResult.Get_Int32();
		m_Point = pResult.Get_Int32();
		m_Time = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx == 0) {
				pResult.NextReadPos();
			}
			else {
				MakeMat mat = new MakeMat() {
					m_Idx = idx,
					m_Count = pResult.Get_Int32()
				};
				m_Mats.Add(mat);
			}
		}
		m_Dollar = pResult.Get_Int32();
	}

	public bool GetCanMake(int _cnt = 1) {
		if (!IS_EnoughMat(_cnt)) return false;
		if (!IS_EnoughDollar(_cnt)) return false;
		return true;
	}
	public bool IS_EnoughMat(int _cnt = 1) {
		for (int i = 0; i < m_Mats.Count; i++) {
			int cost = m_Mats[i].m_Count;
			if (m_Group == MakingGroup.DNA) cost = Mathf.RoundToInt(m_Mats[i].m_Count * (1f - USERINFO.GetSkillValue(SkillKind.DNAProduceDown)));
			if (cost * _cnt > USERINFO.GetItemCount(m_Mats[i].m_Idx)) return false;
		}
		return true;
	}
	public bool IS_EnoughDollar(int _cnt = 1) {
		int cost = m_Dollar;
		if (cost * _cnt > USERINFO.m_Money) return false;
		return true;
	}
	public long GetTime()
	{
		long Re = m_Time * 1000L;
		float per = USERINFO.GetSkillValue(SkillKind.MakingSpeedUp);
		per += USERINFO.ResearchValue(ResearchEff.MakingSpeedUp);
		switch (m_Group)
		{
		case MakingGroup.Equip:
			switch (m_Category)
			{
			case 1: per += USERINFO.ResearchValue(ResearchEff.WeaponTimeUp); break;
			case 2: per += USERINFO.ResearchValue(ResearchEff.HelmetTimeUp); break;
			case 3: per += USERINFO.ResearchValue(ResearchEff.CostumeTimeUp); break;
			case 4: per += USERINFO.ResearchValue(ResearchEff.ShoesTimeUp); break;
			case 5: per += USERINFO.ResearchValue(ResearchEff.AccessoryTimeUp); break;
			}
			break;
		case MakingGroup.PrivateEquip:
			per += USERINFO.ResearchValue(ResearchEff.SpecialEquipTimeUp);
			break;
		case MakingGroup.ResearchMaterial:
			per += USERINFO.ResearchValue(ResearchEff.ResearchMaterialTimeUp);
			break;
		case MakingGroup.MakingMaterial:
			per += USERINFO.ResearchValue(ResearchEff.CraftMaterialTimeUp);
			break;
		}

		Re -= Mathf.RoundToInt(Re * per);
		Re = Math.Max(Re, 0);
		return Re;
	}

	public Sprite GetGradeIcon() {
		return UTILE.LoadImg(string.Format("UI/UI_Exploration/Mk_Grade{0}", m_LV), "png");
	}
	public string GetGradeName() {
		switch (m_LV) {
			case 1: return TDATA.GetString(280);
			case 2: return TDATA.GetString(281);
			case 3: return TDATA.GetString(282);
			case 4: return TDATA.GetString(283);
			case 5: return TDATA.GetString(284);
			default:return TDATA.GetString(280);
		}
	}
}

public class TMakingTableMng : ToolFile
{
	public Dictionary<int, TMakingTable> DIC_Idx = new Dictionary<int, TMakingTable>();

	public TMakingTableMng() : base("Datas/MakingTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TMakingTable data = new TMakingTable(pResult);
		DIC_Idx.Add(data.m_ItemIdx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// MakingTable
	TMakingTableMng m_Making = new TMakingTableMng();
	public List<TMakingTable> GetAllMakingTable() {
		return new List<TMakingTable>(m_Making.DIC_Idx.Values);
	}
	public List<TMakingTable> GetGroupMakingTable(MakingGroup _group) {
		return new List<TMakingTable>(m_Making.DIC_Idx.Values).FindAll(o => o.m_Group == _group);
	}
	public TMakingTable GetMakingTable(int _idx) {
		if (!m_Making.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Making.DIC_Idx[_idx];
	}
	public TMakingTable GetMakingTableOrder(int _cnt) {
		if (m_Making.DIC_Idx.Count - 1 < _cnt) return null;
		return m_Making.DIC_Idx.ElementAt(_cnt).Value;
	}
	public int GetMakingTableCount() {
		return m_Making.DIC_Idx.Count;
	}
}

