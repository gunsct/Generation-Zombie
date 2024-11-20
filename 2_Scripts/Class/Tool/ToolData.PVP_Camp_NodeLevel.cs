using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class TPVP_Camp_NodeLevel : ClassMng
{
	public class Condition
	{
		/// <summary> Node를 Level로 업그레이드하기 위한 조건 </summary>
		public CampBuildType m_Type;
		/// <summary> 조건 레벨 </summary>
		public int m_Lv;
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 대상 시설 지정 </summary>
	public CampBuildType m_Node;
	/// <summary> 시설 레벨</summary>
	public int m_Lv;
	/// <summary> 업그레이드 비용 0:Junk, 1:Cultivate, 2:Chemical, 3:Dollar</summary>
	public int[] m_Cost = new int[4];
	/// <summary> 업그레이드 조건 </summary>
	public Condition[] m_Condition = new Condition[3];

	public TPVP_Camp_NodeLevel(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Node = pResult.Get_Enum<CampBuildType>();
		m_Lv = pResult.Get_Int32();
		for (int i = 0; i < 4; i++) {
			m_Cost[i] = pResult.Get_Int32();
		}
		for (int i = 0; i < 3; i++) {
			m_Condition[i] = new Condition() { 
				m_Type = pResult.Get_Enum<CampBuildType>(), 
				m_Lv = pResult.Get_Int32() 
			};
		}
	}
}

public class TPVP_Camp_NodeLevelMng : ToolFile
{
	public Dictionary<CampBuildType, Dictionary<int, TPVP_Camp_NodeLevel>> DIC_Type = new Dictionary<CampBuildType, Dictionary<int, TPVP_Camp_NodeLevel>>();

	public TPVP_Camp_NodeLevelMng() : base("Datas/PVP_Camp_NodeLevel")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVP_Camp_NodeLevel data = new TPVP_Camp_NodeLevel(pResult);
		if (!DIC_Type.ContainsKey(data.m_Node)) DIC_Type.Add(data.m_Node, new Dictionary<int, TPVP_Camp_NodeLevel>());
		if (!DIC_Type[data.m_Node].ContainsKey(data.m_Lv)) DIC_Type[data.m_Node].Add(data.m_Lv, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVP_Camp_NodeLevelMng m_PVP_Camp_NodeLevel = new TPVP_Camp_NodeLevelMng();

	public TPVP_Camp_NodeLevel GetTPVP_Camp_NodeLevel(CampBuildType _type, int _lv) {
		if (!m_PVP_Camp_NodeLevel.DIC_Type.ContainsKey(_type)) return null;
		if (!m_PVP_Camp_NodeLevel.DIC_Type[_type].ContainsKey(_lv)) return null;
		return m_PVP_Camp_NodeLevel.DIC_Type[_type][_lv];
	}
	public List<TPVP_Camp_NodeLevel> GetTPVP_Camp_NodeLevelGroup(CampBuildType _type) {
		if (!m_PVP_Camp_NodeLevel.DIC_Type.ContainsKey(_type)) return null;
		return m_PVP_Camp_NodeLevel.DIC_Type[_type].Values.ToList();
	}
}

