using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolData;

public class TCaseSelectTable : ClassMng
{
	public class SelectReward
	{
		public RewardKind m_RewardType;
		public int m_Value;
		public int m_Count;
	}
	/// <summary> </summary>
	public int m_Idx;
	/// <summary> </summary>
	public int m_GID;
	/// <summary> </summary>
	public SelectStringType m_StrType;
	/// <summary> </summary>
	public int m_SelectStr;
	/// <summary> </summary>
	public int[] m_SelectVals = new int[2];
	/// <summary> </summary>
	public List<SelectReward> m_Rewards = new List<SelectReward>();
	/// <summary> </summary>
	public bool m_Hide;
	/// <summary> 다음 다이얼로그 </summary>
	public int m_NextDLIdx;
	/// <summary> 보상 받기 전 다이얼로그 </summary>
	public int m_MiddleDLIdx;

	public TCaseSelectTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_GID = pResult.Get_Int32();
		m_StrType = pResult.Get_Enum<SelectStringType>();
		m_SelectStr = pResult.Get_Int32();
		m_SelectVals[0] = pResult.Get_Int32();
		m_SelectVals[1] = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) {
			RewardKind rewardtype = pResult.Get_Enum<RewardKind>();
			if(rewardtype == RewardKind.None) {
				pResult.NextReadPos();
				pResult.NextReadPos();
			}
			else {
				m_Rewards.Add(new SelectReward() { 
					m_RewardType = rewardtype,
					m_Value = pResult.Get_Int32(),
					m_Count = pResult.Get_Int32()
				});
			}
		}
		m_Hide = pResult.Get_Boolean();
		m_NextDLIdx = pResult.Get_Int32();
		m_MiddleDLIdx = pResult.Get_Int32();
	}

	public string GetString() {
		return TDATA.GetString(StringTalbe.Dialog, m_SelectStr);
	}
}

public class TCaseSelectTableMng : ToolFile
{
	public Dictionary<int, TCaseSelectTable> DIC_Idx = new Dictionary<int, TCaseSelectTable>();
	public Dictionary<int, List<TCaseSelectTable>> DIC_Group = new Dictionary<int, List<TCaseSelectTable>>();

	public TCaseSelectTableMng() : base("Datas/CaseSelectTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Group.Clear();
	}


	public override void ParsLine(CSV_Result pResult)
	{
		TCaseSelectTable data = new TCaseSelectTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Group.ContainsKey(data.m_GID)) DIC_Group.Add(data.m_GID, new List<TCaseSelectTable>());
		DIC_Group[data.m_GID].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// CaseSelectTable
	TCaseSelectTableMng m_CaseSelect = new TCaseSelectTableMng();

	public TCaseSelectTable GetCaseSelectTable(int _idx) {
		if (!m_CaseSelect.DIC_Idx.ContainsKey(_idx)) return null;
		return m_CaseSelect.DIC_Idx[_idx];
	}
	public List<TCaseSelectTable> GetCaseSelectGroupTable(int _gid) {
		if (!m_CaseSelect.DIC_Group.ContainsKey(_gid)) return null;
		return m_CaseSelect.DIC_Group[_gid];
	}
}
