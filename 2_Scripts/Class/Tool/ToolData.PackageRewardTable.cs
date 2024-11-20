using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TPackageRewardTable : ClassMng
{
	public int m_Idx;
	public int m_Step;
	public int m_Name;
	public int m_Desc;
	public int m_SIdx;
	public RewardKind m_RewardType;
	public int m_RewardIdx;
	public int m_RewardCmt;
	public int m_RewardGrade;
	public int m_RewardLv;

	public TPackageRewardTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Step = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_SIdx = pResult.Get_Int32();
		m_RewardType = pResult.Get_Enum<RewardKind>();
		m_RewardIdx = pResult.Get_Int32();
		m_RewardCmt = pResult.Get_Int32();
		m_RewardGrade = pResult.Get_Int32();
		m_RewardLv = pResult.Get_Int32();
	}
	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
	}
}
public class TPackageRewardTableMng : ToolFile
{
	public Dictionary<int, TPackageRewardTable> DIC = new Dictionary<int, TPackageRewardTable>();
	public Dictionary<int, List<TPackageRewardTable>> DIC_Group = new Dictionary<int, List<TPackageRewardTable>>();

	public TPackageRewardTableMng() : base("Datas/PackageRewardTable")
	{
	}

	public override void DataInit()
	{
		DIC.Clear();
		DIC_Group.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPackageRewardTable data = new TPackageRewardTable(pResult);
		DIC.Add(data.m_Idx, data);
		if (!DIC_Group.ContainsKey(data.m_SIdx)) DIC_Group.Add(data.m_SIdx, new List<TPackageRewardTable>());
		DIC_Group[data.m_SIdx].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ExpTable
	TPackageRewardTableMng m_Package = new TPackageRewardTableMng();

	public List<TPackageRewardTable> GeTPackageRewardGroupTable(int _sidx) {
		if (!m_Package.DIC_Group.ContainsKey(_sidx)) return null;
		return m_Package.DIC_Group[_sidx];
	}
}

