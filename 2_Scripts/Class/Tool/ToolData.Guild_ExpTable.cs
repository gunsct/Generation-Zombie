using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TGuild_ExpTable : ClassMng
{
	/// <summary> 레벨 </summary>
	public int m_LV;
	/// <summary> EXP </summary>
	public int m_EXP;
	/// <summary> 사용 유무 먁스 레벨 계산용 </summary>
	public bool m_USE;

	public TGuild_ExpTable(CSV_Result pResult)
	{
		m_LV = pResult.Get_Int32();
		m_EXP = pResult.Get_Int32();
		m_USE = pResult.Get_Boolean();
	}
}
public class TGuild_ExpTableMng : ToolFile
{
	public Dictionary<int, TGuild_ExpTable> DIC_LV = new Dictionary<int, TGuild_ExpTable>();
	public int MaxLV = 0;

	public TGuild_ExpTableMng() : base("Datas/Guild_ExpTable")
	{
	}

	public override void DataInit()
	{
		DIC_LV.Clear();
		MaxLV = 0;
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TGuild_ExpTable table = new TGuild_ExpTable(pResult);
		if (DIC_LV.ContainsKey(table.m_LV)) return;
		DIC_LV.Add(table.m_LV, table);
		if (table.m_USE && MaxLV < table.m_LV) MaxLV = table.m_LV;
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// Guild_ExpTable
	TGuild_ExpTableMng m_Guild_Exp = new TGuild_ExpTableMng();

	public void GetGuild_LV(long TotalExp, out int LV, out long Exp)
	{
		LV = 1;
		Exp = TotalExp;
		for (int i = 1; i < m_Guild_Exp.MaxLV; i++)
		{
			if (!m_Guild_Exp.DIC_LV.ContainsKey(i)) continue;
			var info = m_Guild_Exp.DIC_LV[i];
			LV = info.m_LV;
			if (TotalExp < info.m_EXP) return;
			TotalExp -= info.m_EXP;
			Exp = TotalExp;
		}
		LV = m_Guild_Exp.MaxLV;
		Exp = TotalExp;
		return;
	}

	public TGuild_ExpTable GetGuild_ExpTable(int LV)
	{
		if (!m_Guild_Exp.DIC_LV.ContainsKey(LV)) return null;
		return m_Guild_Exp.DIC_LV[LV];
	}

	public int GetGuild_MaxLV()
	{
		return m_Guild_Exp.MaxLV;
	}

}

