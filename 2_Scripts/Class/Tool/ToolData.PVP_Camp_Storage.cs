using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPVP_Camp_Storage : ClassMng
{
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary> 해당 레벨에서 보관 가능한 재료 갯수, 0:junk,1:cultivate,2:chemical </summary>
	public int[] m_SaveMat = new int[3];
	/// <summary> 해당 레벨에서 약탈 가능한 재료 갯수, 0:junk,1:cultivate,2:chemical </summary>
	public int[] m_StealMat = new int[3];

	public TPVP_Camp_Storage(CSV_Result pResult) {
		m_Lv = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) m_SaveMat[i] = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) m_StealMat[i] = pResult.Get_Int32();
	}
}

public class TPVP_Camp_StorageMng : ToolFile
{
	public Dictionary<int, TPVP_Camp_Storage> DIC_Type = new Dictionary<int, TPVP_Camp_Storage>();

	public TPVP_Camp_StorageMng() : base("Datas/PVP_Camp_Storage")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVP_Camp_Storage data = new TPVP_Camp_Storage(pResult);
		if (!DIC_Type.ContainsKey(data.m_Lv)) DIC_Type.Add(data.m_Lv, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVP_Camp_StorageMng m_PVP_Camp_Storage = new TPVP_Camp_StorageMng();
	public TPVP_Camp_Storage GetPVP_Camp_Storage(int _lv) {
		if (!m_PVP_Camp_Storage.DIC_Type.ContainsKey(_lv)) return null;
		return m_PVP_Camp_Storage.DIC_Type[_lv];
	}
}

