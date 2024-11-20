using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolData;
using static UserInfo;

public enum SupplyBoxOpenType
{
	Normal = 0,
	Hard,
	Nightmare
}

public class TSupplyBoxTable : ClassMng
{
	/// <summary> 등급 </summary>
	public int m_Idx;
	/// <summary> 경험치 최소 최대 </summary>
	public int[] Exp = new int[2];
	/// <summary> 달러 최초 최대 </summary>
	public int[] Dollar = new int[2];
	/// <summary> 금니 최초 최대 </summary>
	public int[] Cash = new int[2];
	/// <summary> 소탕권 최초 최대 </summary>
	public int[] PassTicket = new int[2];
	/// <summary> 드랍 그룹, gachagrouptable 인덱스 </summary>
	public Dictionary<int, int> m_Reward = new Dictionary<int, int>();
	/// <summary> 유저 경험치 </summary>
	public int[] UserExp = new int[2];

	public SupplyBoxOpenType OpenType;
	public int OpenValue;

	public TSupplyBoxTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) Exp[i] = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) Dollar[i] = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) Cash[i] = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) PassTicket[i] = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			int gid = pResult.Get_Int32();
			if (gid == 0) pResult.NextReadPos();
			else m_Reward.Add(gid, pResult.Get_Int32());
		}
		for (int i = 0; i < 2; i++) UserExp[i] = pResult.Get_Int32();
		OpenType = pResult.Get_Enum<SupplyBoxOpenType>();
		OpenValue = pResult.Get_Int32();
	}
}

public class TSupplyBoxTableMng : ToolFile
{
	public Dictionary<int, TSupplyBoxTable> DIC_Idx = new Dictionary<int, TSupplyBoxTable>();
	public List<TSupplyBoxTable> Datas = new List<TSupplyBoxTable>();

	public TSupplyBoxTableMng() : base("Datas/SupplyBoxTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSupplyBoxTable data = new TSupplyBoxTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Datas.Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SupplyBoxTable
	TSupplyBoxTableMng m_SupplyBox = new TSupplyBoxTableMng();

	public TSupplyBoxTable GetSupplyBoxTable(int idx)
	{
		if (!m_SupplyBox.DIC_Idx.ContainsKey(idx)) return null;
		return m_SupplyBox.DIC_Idx[idx];
	}

	public int GetSupplyBoxLV(Stage info)
	{
		int lv = 1;
		int maxlv = m_SupplyBox.Datas.Count;
		for (int i = 0, iMax = maxlv; i < iMax; i++)
		{
			var tdata = m_SupplyBox.Datas[i];
			StageIdx idx = info.Idxs.Find(o => o.Pos == (int)tdata.OpenType);
			if (idx.Idx < tdata.OpenValue) break;
			lv = tdata.m_Idx;
		}
		return lv;
	}


	public int GetSupplyBoxMaxLV()
	{
		return m_SupplyBox.Datas[m_SupplyBox.Datas.Count - 1].m_Idx;
	}
}
