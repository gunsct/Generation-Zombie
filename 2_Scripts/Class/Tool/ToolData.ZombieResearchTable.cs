using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TZombieResearchTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 이미지 </summary>
	public string m_Icon;

	public TZombieResearchTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Icon = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Icon) && !m_Icon.Contains("/"))
			Debug.LogError($"[ ZombieResearchTable ({m_Idx}) ] m_Icon 패스 체크할것");
#endif
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public Sprite GetIcon()
	{
		return UTILE.LoadImg(m_Icon, "png");
	}
}
public class TZombieResearchTableMng : ToolFile
{
	public Dictionary<int, TZombieResearchTable> DIC_Idx = new Dictionary<int, TZombieResearchTable>();
	public List<TZombieResearchTable> Datas = new List<TZombieResearchTable>();

	public TZombieResearchTableMng() : base("Datas/ZombieResearchTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TZombieResearchTable data = new TZombieResearchTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ZombieResearchTable
	TZombieResearchTableMng m_ZombieResearch = new TZombieResearchTableMng();
	public List<TZombieResearchTable> GetZombieResearchTables()
	{
		return m_ZombieResearch.Datas;
	}

	public TZombieResearchTable GetZombieResearchTable(int _Idx) {
		if (!m_ZombieResearch.DIC_Idx.ContainsKey(_Idx)) return null;
		return m_ZombieResearch.DIC_Idx[_Idx];
	}
}

