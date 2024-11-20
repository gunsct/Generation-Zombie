using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TSkillGrowthTable : ClassMng
{
	public class NeedMaterial
	{
		public int m_Idx;
		public int m_Cnt;
	}
	/// <summary> 등급 </summary>
	public Grade m_Grade;
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary> 재료 </summary>
	public List<NeedMaterial> m_NeedMaterial = new List<NeedMaterial>();
	/// <summary> 돈 </summary>
	public int m_NeedMoney;

	public TSkillGrowthTable(CSV_Result pResult) {
		m_Grade = pResult.Get_Enum<Grade>();
		m_Lv = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx == 0) {
				pResult.NextReadPos();
				continue;
			}
			m_NeedMaterial.Add(new NeedMaterial() { m_Idx = idx, m_Cnt = pResult.Get_Int32()});
		}
		m_NeedMoney = pResult.Get_Int32();
	}
}
public class TSkillGrowthTableMng : ToolFile
{
	public Dictionary<Grade, Dictionary<int, TSkillGrowthTable>> DIC_Grade = new Dictionary<Grade, Dictionary<int, TSkillGrowthTable>>();

	public TSkillGrowthTableMng() : base("Datas/SkillGrowthTable")
	{
	}

	public override void DataInit()
	{
		DIC_Grade.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSkillGrowthTable data = new TSkillGrowthTable(pResult);
		if (!DIC_Grade.ContainsKey(data.m_Grade))
			DIC_Grade.Add(data.m_Grade, new Dictionary<int, TSkillGrowthTable>());
		if (!DIC_Grade[data.m_Grade].ContainsKey(data.m_Lv))
			DIC_Grade[data.m_Grade].Add(data.m_Lv, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SkillGrowthTable
	TSkillGrowthTableMng m_SkillGrowth = new TSkillGrowthTableMng();

	public TSkillGrowthTable GetSkillGrowthTable(Grade _grade, int _lv) {
		if (!m_SkillGrowth.DIC_Grade.ContainsKey(_grade)) return null;
		if (!m_SkillGrowth.DIC_Grade[_grade].ContainsKey(_lv)) return null;
		if (_lv < 1) return null;
		return m_SkillGrowth.DIC_Grade[_grade][_lv];
	}
}

