using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;
public enum CollectionType
{
	/// <summary> 캐릭터 수집 </summary>
	Character,
	/// <summary> 좀비 수집 </summary>
	Zombie,
	/// <summary> DNA 수집 </summary>
	DNA,
	/// <summary> 장비 수집(사용안함) </summary>
	Equip,
	/// <summary>  </summary>
	END = Equip
}

public class TCollectionTable : ClassMng
{
	public class Stat
	{
		public StatType m_Type;
		public float m_Value;
	}

	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Info;
	/// <summary> 레벨 </summary>
	public int m_LV;
	/// <summary> 타입 </summary>
	public CollectionType m_Type;
	/// <summary> 등급 </summary>
	public int m_Grade;
	/// <summary> 수집 목록 </summary>
	public List<int> m_Colloets = new List<int>();
	/// <summary> 증가 스텟 </summary>
	public Stat m_Stat;

	public TCollectionTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Info = pResult.Get_Int32();
		m_LV = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<CollectionType>();
		m_Grade = pResult.Get_Int32();
		for(int i = 0; i < 5; i++)
		{
			int temp = pResult.Get_Int32();
			if (temp < 1) continue;
			m_Colloets.Add(temp);
		}

		StatType stat = pResult.Get_Enum<StatType>();
		float value = pResult.Get_Float();
		if (stat == StatType.None) return;
		m_Stat = new Stat() { m_Type = stat, m_Value = value };
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}

	public string GetDes()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Info);
	}

	public bool IS_Check(int _idx, int _grade) {
		return m_Grade == _grade && m_Colloets.Contains(_idx);
	}
}

public class TCollectionGroup : ClassMng
{
	public int m_Idx;
	public int m_MaxLV = 0;
	public CollectionType m_Type;
	public Dictionary<int, TCollectionTable> m_List = new Dictionary<int, TCollectionTable>();

	public void Add(TCollectionTable item)
	{
		if (m_List.ContainsKey(item.m_LV)) return;
		m_List.Add(item.m_LV, item);
		if (m_MaxLV < item.m_LV) m_MaxLV = item.m_LV;
	}

	public List<TCollectionTable> GetCheckList(int _idx, int _grade) {
		List<TCollectionTable> table = new List<TCollectionTable>(m_List.Values);
		return table.FindAll(o => o.IS_Check(_idx, _grade));
	}
}
public class TCollectionTableMng : ToolFile
{
	public List<TCollectionGroup> Groups = new List<TCollectionGroup>();
	public Dictionary<int, TCollectionGroup> DIC_Idx = new Dictionary<int, TCollectionGroup>();
	public TCollectionTableMng() : base("Datas/CollectionTable")
	{
	}


	public override void DataInit()
	{
		Groups.Clear();
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TCollectionTable data = new TCollectionTable(pResult);
		if (!DIC_Idx.ContainsKey(data.m_Idx))
		{
			var group = new TCollectionGroup() { m_Idx = data.m_Idx, m_Type = data.m_Type };
			DIC_Idx.Add(data.m_Idx, group);
			Groups.Add(group);
		}
		DIC_Idx[data.m_Idx].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// CollectionTable
	TCollectionTableMng m_Collection = new TCollectionTableMng();

	public List<TCollectionGroup> GetCollectionGroups()
	{
		return m_Collection.Groups;
	}

	public List<TCollectionGroup> GetCollectionTypeGroups(CollectionType Type)
	{
		return m_Collection.Groups.FindAll(o => o.m_Type == Type);
	}
	public TCollectionGroup GetCollectionGroup(int idx)
	{
		if (!m_Collection.DIC_Idx.ContainsKey(idx)) return null;
		return m_Collection.DIC_Idx[idx];
	}

	public TCollectionTable GetCollectionTable(int idx, int lv) {
		if (!m_Collection.DIC_Idx.ContainsKey(idx)) return null;
		if (!m_Collection.DIC_Idx[idx].m_List.ContainsKey(lv)) return null;
		return m_Collection.DIC_Idx[idx].m_List[lv];
	}
}

