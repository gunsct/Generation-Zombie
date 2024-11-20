using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TEnemyLevelTable : ClassMng
{
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary> 스텟 </summary>
	public float[] m_Stat = new float[(int)EEnemyStat.End];
	public float m_DebuffCardStatRatio;
	public TEnemyLevelTable(CSV_Result pResult) {
		m_Lv = pResult.Get_Int32();

		for (EEnemyStat i = EEnemyStat.HP; i < EEnemyStat.End; i++) {
			if (i == EEnemyStat.HIDING) continue;
			m_Stat[(int)i] = pResult.Get_Float();
		}
		m_DebuffCardStatRatio = pResult.Get_Float();
	}

	public float GetStat(EEnemyStat stat) {
		return m_Stat[(int)stat];
	}
}

public class TEnemyLevelTableMng : ToolFile
{
	public Dictionary<int, TEnemyLevelTable> DIC_LV = new Dictionary<int, TEnemyLevelTable>();
	public TEnemyLevelTableMng() : base("Datas/EnemyLevelTable")
	{
	}

	public override void DataInit()
	{
		DIC_LV.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEnemyLevelTable data = new TEnemyLevelTable(pResult);
		if (!DIC_LV.ContainsKey(data.m_Lv))
			DIC_LV.Add(data.m_Lv, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemyLevelTable
	TEnemyLevelTableMng m_EnemyLV = new TEnemyLevelTableMng();

	public TEnemyLevelTable GetEnemyLevelTable(int _lv) {
		if (!m_EnemyLV.DIC_LV.ContainsKey(_lv)) return null;
		return m_EnemyLV.DIC_LV[_lv];
	}
}

