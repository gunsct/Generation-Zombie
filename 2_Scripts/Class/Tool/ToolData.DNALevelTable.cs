using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;

public class TDNALevelTable : ClassMng
{
	public class IdxCnt
	{
		public int Idx;
		public int Cnt;
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 등급 </summary>
	public int m_Grade;
	/// <summary> 타입 </summary>
	public DNABGType m_Type;
	/// <summary> DNA 레벨 </summary>
	public int m_Lv;
	/// <summary> 0이 아닐 경우 생성 시 RandomStatTable의 Index를 참조하여 부여 </summary>
	public int m_EssentialStatGrant;
	/// <summary> 다음 단계를 달성하기 위해 재료 Index, cnt </summary>
	public List<IdxCnt> m_LvUpMats = new List<IdxCnt>();
	/// <summary> 다음 단계를 달성하기 위한 요구 달러 </summary>
	public int m_LvUpDollar;
	/// <summary> 분해 시 보상으로 지급되는 ItemIndex, cnt </summary>
	public List<IdxCnt> m_Rewards = new List<IdxCnt>();
	/// <summary> 변형 시 요구되는 재료 ItemIndex, cnt </summary>
	public List<IdxCnt> m_TransMats = new List<IdxCnt>();
	/// <summary> 옵션 변경을 위한 요구 달러 </summary>
	public int m_TransDollar;
	/// <summary>  </summary>
	//지울것들

	public TDNALevelTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Grade = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<DNABGType>();
		m_Lv = pResult.Get_Int32();
		m_EssentialStatGrant = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx == 0) {
				pResult.NextReadPos();
			}
			else m_LvUpMats.Add(new IdxCnt() { Idx = idx, Cnt = pResult.Get_Int32() });
		}
		m_LvUpDollar = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx == 0) {
				pResult.NextReadPos();
			}
			else m_Rewards.Add(new IdxCnt() { Idx = idx, Cnt = pResult.Get_Int32() });
		}
		for (int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx == 0) {
				pResult.NextReadPos();
			}
			else m_TransMats.Add(new IdxCnt() { Idx = idx, Cnt = pResult.Get_Int32() });
		}
		m_TransDollar = pResult.Get_Int32();
	}
	
	public string GetGradeGroupName(Grade grade)
	{
		return $"{BaseValue.GradeName((int) grade)}";
	}
}
public class TDNALevelTableMng : ToolFile
{
	public Dictionary<DNABGType, Dictionary<int, Dictionary<int, TDNALevelTable>>> DIC_Idx = new Dictionary<DNABGType, Dictionary<int, Dictionary<int, TDNALevelTable>>>();
	public Dictionary<DNABGType, Dictionary<int, List<TDNALevelTable>>> DIC_BG_GRADE = new Dictionary<DNABGType, Dictionary<int, List<TDNALevelTable>>>();
	public TDNALevelTableMng() : base("Datas/DNALevelTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_BG_GRADE.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TDNALevelTable data = new TDNALevelTable(pResult);
		if(!DIC_Idx.ContainsKey(data.m_Type)) DIC_Idx.Add(data.m_Type, new Dictionary<int, Dictionary<int, TDNALevelTable>>());
		if (!DIC_Idx[data.m_Type].ContainsKey(data.m_Grade)) DIC_Idx[data.m_Type].Add(data.m_Grade, new Dictionary<int, TDNALevelTable>());
		if (!DIC_Idx[data.m_Type][data.m_Grade].ContainsKey(data.m_Lv)) DIC_Idx[data.m_Type][data.m_Grade].Add(data.m_Lv, data);

		if(!DIC_BG_GRADE.ContainsKey(data.m_Type)) DIC_BG_GRADE.Add(data.m_Type, new Dictionary<int, List<TDNALevelTable>>());
		if (!DIC_BG_GRADE[data.m_Type].ContainsKey(data.m_Grade)) DIC_BG_GRADE[data.m_Type].Add(data.m_Grade, new List<TDNALevelTable>());
		DIC_BG_GRADE[data.m_Type][data.m_Grade].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// DNALevelTable
	TDNALevelTableMng m_DNALevel = new TDNALevelTableMng();

	public TDNALevelTable GetDNALevelTable(DNABGType _type, int _grade, int _lv) {
		if (!m_DNALevel.DIC_Idx.ContainsKey(_type)) return null;
		if (!m_DNALevel.DIC_Idx[_type].ContainsKey(_grade)) return null;
		if (!m_DNALevel.DIC_Idx[_type][_grade].ContainsKey(_lv)) return null;
		return m_DNALevel.DIC_Idx[_type][_grade][_lv];
	}
	public int GetDNALVOpCnt(DNABGType _type, int _grade, int _lv)
	{
		if (!m_DNALevel.DIC_BG_GRADE.ContainsKey(_type)) return 0;
		if (!m_DNALevel.DIC_BG_GRADE[_type].ContainsKey(_grade)) return 0;
		return m_DNALevel.DIC_BG_GRADE[_type][_grade].Count(o => o.m_Lv <= _lv);
	}
	public List<TDNALevelTable> GetDNALVOps(DNABGType _type, int _grade)
	{
		if (!m_DNALevel.DIC_BG_GRADE.ContainsKey(_type)) return null;
		if (!m_DNALevel.DIC_BG_GRADE[_type].ContainsKey(_grade)) return null;
		return m_DNALevel.DIC_BG_GRADE[_type][_grade];
	}
}

