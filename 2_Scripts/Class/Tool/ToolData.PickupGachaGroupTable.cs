using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TPickupGachaGroupTable : ClassMng
{
	/// <summary> 픽업가챠 그룹ID </summary>
	public int m_Gidx;
	/// <summary> 픽업가챠 등급 </summary>
	public int m_RewardGrade;
	/// <summary> 픽업가챠 등급 총 확률(만분율: 100% = 10,000) </summary>
	public int m_TotalProb;
	/// <summary> 픽업가챠 등급 선택 생존자 확률(만분율: 100% = 10,000) </summary>
	public int m_SelectedProb;
	/// <summary> 픽업가챠 등급 생존자 그룹ID(GachaGroupTable → GroupIndex 참고) </summary>
	public int m_Value;

	public TPickupGachaGroupTable(CSV_Result pResult)
	{
		m_Gidx = pResult.Get_Int32();
		m_RewardGrade = pResult.Get_Int32();
		m_TotalProb = pResult.Get_Int32();
		m_SelectedProb = pResult.Get_Int32();
		m_Value = pResult.Get_Int32();
	}
}
public class TPickupGachaGroupTableMng : ToolFile
{
	public List<TPickupGachaGroupTable> Datas = new List<TPickupGachaGroupTable>();

	public TPickupGachaGroupTableMng() : base("Datas/PickupGachaGroupTable")
	{
	}

	public override void DataInit()
	{
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPickupGachaGroupTable data = new TPickupGachaGroupTable(pResult);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PickupGachaGroupTable
	TPickupGachaGroupTableMng m_PickupGachaGroup = new TPickupGachaGroupTableMng();

	public List<TPickupGachaGroupTable> GetPickupGachaGroupTable(int _gid)
	{
		return m_PickupGachaGroup.Datas.FindAll(o=>o.m_Gidx == _gid);
	}
}