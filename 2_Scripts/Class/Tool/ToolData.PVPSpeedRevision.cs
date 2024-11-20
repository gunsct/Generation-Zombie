using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPVPSpeedRevision : ClassMng
{
	/// <summary> speed 순위 </summary>
	public int m_Rank;
	/// <summary> 속도 보정값 </summary>
	public int m_RevisedSpeed;

	public TPVPSpeedRevision(CSV_Result pResult) {
		m_Rank = pResult.Get_Int32();
		m_RevisedSpeed = pResult.Get_Int32();
	}
}

public class TPVPSpeedRevisionMng : ToolFile
{
	public List<TPVPSpeedRevision> DIC_List = new List<TPVPSpeedRevision>();

	public TPVPSpeedRevisionMng() : base("Datas/PVPSpeedRevision")
	{
	}

	public override void DataInit()
	{
		DIC_List.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVPSpeedRevision data = new TPVPSpeedRevision(pResult);
		DIC_List.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVPSpeedRevisionMng m_PVPSpeedRevision = new TPVPSpeedRevisionMng();

	public TPVPSpeedRevision GeTPVPSpeedRevision(int _rank) {
		return m_PVPSpeedRevision.DIC_List[_rank];
	}
}

