using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TRandomStatTable : ClassMng
{
	/// <summary>  인덱스 </summary>
	public int m_Idx;
	/// <summary> 랜덤 스텟 그룹 </summary>
	public int m_StatGroup;
	/// <summary> 부여 스텟 </summary>
	public StatType m_Stat;
	/// <summary> 최소 최대값 </summary>
	public int[] m_Val = new int[2];
	/// <summary> 확률(상대 확률) </summary>
	public int m_Prob;

	public TRandomStatTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_StatGroup = pResult.Get_Int32();
		m_Stat = pResult.Get_Enum<StatType>();
		m_Val[0] = pResult.Get_Int32();
		m_Val[1] = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
	}
	public int GetVal() {
		return UTILE.Get_Random(m_Val[0], m_Val[1] + 1);
	}
}

public class TRandomStatGroup
{
	public int m_TotalProb = 0;
	public List<TRandomStatTable> m_List = new List<TRandomStatTable>();

	public void Add(TRandomStatTable item) {
		m_TotalProb += item.m_Prob;
		m_List.Add(item);
	}
}

public class TRandomStatTableMng : ToolFile
{
	public Dictionary<int, TRandomStatGroup> DIC_Part = new Dictionary<int, TRandomStatGroup>();
	public Dictionary<int, TRandomStatTable> DIC_All = new Dictionary<int, TRandomStatTable>();
	public TRandomStatTableMng() : base("Datas/RandomStatTable")
	{
	}

	public override void DataInit()
	{
		DIC_Part.Clear();
		DIC_All.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TRandomStatTable data = new TRandomStatTable(pResult);
		if (!DIC_All.ContainsKey(data.m_Idx)) DIC_All.Add(data.m_Idx, data);
		if (!DIC_Part.ContainsKey(data.m_StatGroup)) DIC_Part.Add(data.m_StatGroup, new TRandomStatGroup());
		DIC_Part[data.m_StatGroup].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// RandomStatTable
	TRandomStatTableMng m_RandomStat = new TRandomStatTableMng();

	/// <summary> 그룹아이디로 확률체크해서 뽑기 </summary>
	public TRandomStatTable GetPickRandomStat(int _group) {
		if (!m_RandomStat.DIC_Part.ContainsKey(_group)) return null; 
		TRandomStatGroup group = m_RandomStat.DIC_Part[_group];
		int dropprop = UTILE.Get_Random(0, group.m_TotalProb);
		for (int i = 0; i < group.m_List.Count; i++) {
			TRandomStatTable item = group.m_List[i];
			if (dropprop < item.m_Prob) return item;
			dropprop -= item.m_Prob;
		}
		return null;
	}
	public TRandomStatTable GetRandomStatTable(int _group, StatType _stat) {
		if (!m_RandomStat.DIC_Part.ContainsKey(_group)) return null;
		return m_RandomStat.DIC_Part[_group].m_List.Find(o => o.m_Stat == _stat);
	}
	public TRandomStatTable GetRandomStatTable(int _idx) {
		if (!m_RandomStat.DIC_All.ContainsKey(_idx)) return null;
		return m_RandomStat.DIC_All[_idx];
	}
	public TRandomStatGroup GetRandomStatGroup(int _group) {
		if (!m_RandomStat.DIC_Part.ContainsKey(_group)) return null;
		return m_RandomStat.DIC_Part[_group];
	}
}

