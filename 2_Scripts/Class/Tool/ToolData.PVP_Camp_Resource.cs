using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPVP_Camp_Resource : ClassMng
{
	public class Mat
	{
		/// <summary> 생산 수량 </summary>
		public int MakeCnt;
		/// <summary> 필요 달러 </summary>
		public int NeedMoney;
		/// <summary> 소요시간(분) </summary>
		public int NeedTime;

		public double GetNeedTime { get { return NeedTime * 60 * 1000; } }
	}
	/// <summary> 시설 레벨 </summary>
	public int m_Lv;
	/// <summary> 생산 재료 0:junk,1:cultivate,2:chemical </summary>
	public Mat[] m_Mat = new Mat[3];

	public TPVP_Camp_Resource(CSV_Result pResult) {
		m_Lv = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			m_Mat[i] = new Mat() {
				MakeCnt = pResult.Get_Int32(),
				NeedMoney = pResult.Get_Int32(),
				NeedTime = pResult.Get_Int32()
			};
		}
	}
}

public class TPVP_Camp_ResourceMng : ToolFile
{
	public Dictionary<int, TPVP_Camp_Resource> DIC_Type = new Dictionary<int, TPVP_Camp_Resource>();

	public TPVP_Camp_ResourceMng() : base("Datas/PVP_Camp_Resource")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVP_Camp_Resource data = new TPVP_Camp_Resource(pResult);
		if (!DIC_Type.ContainsKey(data.m_Lv)) DIC_Type.Add(data.m_Lv, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVP_Camp_ResourceMng m_PVP_Camp_Resource = new TPVP_Camp_ResourceMng();

	public TPVP_Camp_Resource GetPVP_Camp_Resource(int _lv) {
		if (!m_PVP_Camp_Resource.DIC_Type.ContainsKey(_lv)) return null;
		return m_PVP_Camp_Resource.DIC_Type[_lv];
	}
}

