using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPVP_CampTable : ClassMng
{
	public class RatioCnt
	{
		public float Ratio;
		public int Cnt;
	}
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary> 자원 최대 비율, 수량 </summary>
	public RatioCnt[] m_RatioCnt = new RatioCnt[3];
	/// <summary> 티어 </summary>
	public int Tire;

	public TPVP_CampTable(CSV_Result pResult) {
		m_Lv = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			m_RatioCnt[i] = new RatioCnt() { 
				Ratio = pResult.Get_Float(), 
				Cnt = pResult.Get_Int32()
			};
		}
		Tire = pResult.Get_Int32();
	}
}

public class TPVP_CampTableMng : ToolFile
{
	public List<TPVP_CampTable> DIC_Type = new List<TPVP_CampTable>();

	public TPVP_CampTableMng() : base("Datas/PVP_Camp")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVP_CampTable data = new TPVP_CampTable(pResult);
		DIC_Type.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVP_CampTableMng m_PVP_Camp = new TPVP_CampTableMng();

	public TPVP_CampTable GetTPVP_CampTable(int _lv) {
		return m_PVP_Camp.DIC_Type.Find(o => o.m_Lv == _lv);
	}
	public int GetPVP_CampLvFromTire(int _tier) {
		TPVP_CampTable tdata = m_PVP_Camp.DIC_Type.Find(o => o.Tire == _tier);
		if (tdata != null) return tdata.m_Lv;
		return 0;
	}
	public List<TPVP_CampTable> GetAllPVP_CampTable() {
		return m_PVP_Camp.DIC_Type;
	}
}

