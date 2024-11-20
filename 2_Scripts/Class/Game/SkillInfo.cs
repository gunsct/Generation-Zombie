using UnityEngine;
using Newtonsoft.Json;

public class SkillInfo : ClassMng
{
	public int m_Idx;
	public int m_LV;

	[JsonIgnore] public TSkillTable m_TData { get { return TDATA.GetSkill(m_Idx); } }

	public bool CheckLvUp() {
		TSkillGrowthTable table = TDATA.GetSkillGrowthTable(m_TData.m_Grade, m_LV);
		if (table == null) return false;
		if (m_TData.m_MaxLV <= m_LV) return false;
		for (int i = 0; i < table.m_NeedMaterial.Count; i++) {
			if (USERINFO.GetItemCount(table.m_NeedMaterial[i].m_Idx) < table.m_NeedMaterial[i].m_Cnt) return false;
		}
		if (USERINFO.m_Money < table.m_NeedMoney) return false;
		return true;
	}

	public float GetSkillValue()
	{
		return m_TData.GetValue(m_LV);
	}
	/// <summary> 스킬 사용시 감소 행동력</summary>
	public int GetSkillAP() {
		return m_TData.m_BaseAP;
	}
}

