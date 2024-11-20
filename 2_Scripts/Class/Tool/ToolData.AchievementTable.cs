using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Serialization;

public class TAchievementTable : ClassMng
{
	public enum Tab
	{
		Stage = 0,
		Growth,
		Collection,
		Zombie,
		End
	}
	/// <summary> 그룹인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Des;
	/// <summary> 분류 탭 </summary>
	public Tab m_Tab;
	/// <summary> 인덱스 </summary>
	public AchieveType m_Type;
	/// <summary> 레벨 </summary>
	public int m_LV;
	/// <summary> 값 </summary>
	public long[] m_Values = new long[2];
	/// <summary> 보상 </summary>
	public RewardInfo m_Reward;
	public bool m_Hide;

	public TAchievementTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Des = pResult.Get_Int32();
		m_Tab = pResult.Get_Enum<Tab>();
		m_Type = pResult.Get_Enum<AchieveType>();
		m_LV = pResult.Get_Int32();
		m_Values[0] = pResult.Get_Int64();
		m_Values[1] = pResult.Get_Int64();
		RewardKind type = pResult.Get_Enum<RewardKind>();
		int Idx = pResult.Get_Int32();
		if (type == RewardKind.None || Idx < 1) return;
		m_Reward = new RewardInfo();
		m_Reward.Kind = type;
		m_Reward.Idx = Idx;
		m_Reward.Cnt = pResult.Get_Int32();
		m_Reward.Grade = pResult.Get_Int32();
		m_Reward.LV = pResult.Get_Int32();
		m_Hide = pResult.Get_Boolean();
	}

	public string GetName()
	{
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Name), m_Values[0], m_Values[1]);
	}

	public string GetDes()
	{
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Des), m_Values[0], m_Values[1]);
	}
}

public class TAchievementGroup : ClassMng
{
	public int m_Idx;
	public AchieveType m_Type;
	public List<TAchievementTable> m_List = new List<TAchievementTable>();

	public void Add(TAchievementTable item)
	{
		m_List.Add(item);
	}
}

public class TAchievementTableMng : ToolFile
{
	public List<TAchievementGroup> Groups = new List<TAchievementGroup>();
	public Dictionary<TAchievementTable.Tab, List<TAchievementGroup>> DIC_Tab = new Dictionary<TAchievementTable.Tab, List<TAchievementGroup>>();
	// 검색용 인덱스, 레벨, 타입
	public Dictionary<int, Dictionary<int, Dictionary<AchieveType, TAchievementTable>>> DIC_Idx = new Dictionary<int, Dictionary<int, Dictionary<AchieveType, TAchievementTable>>>();
	public Dictionary<AchieveType, List<long>> DIC_Value = new Dictionary<AchieveType, List<long>>();

	public TAchievementTableMng() : base("Datas/AchievementTable")
	{
	}

	public override void DataInit()
	{
		Groups.Clear();
		DIC_Tab.Clear();
		DIC_Idx.Clear();
		DIC_Value.Clear();

		DIC_Value.Add(AchieveType.Character_Grade_Count, new List<long>());
		DIC_Value.Add(AchieveType.Equip_Grade_Count, new List<long>());
		DIC_Value.Add(AchieveType.DNA_LevelUp_Count, new List<long>());

		// 등급은 등급별수집을위해 미리 만들어줌
		for (int i = 0; i < 11; i++)
		{
			DIC_Value[AchieveType.Character_Grade_Count].Add(i);
			DIC_Value[AchieveType.Equip_Grade_Count].Add(i);
			DIC_Value[AchieveType.DNA_LevelUp_Count].Add(i);
		}
	}

	public override void CheckData()
	{
		// 레벨별 소팅
		for (int i = Groups.Count - 1; i > -1; i--)
		{
			Groups[i].m_List.Sort((befor, after) => befor.m_LV.CompareTo(after.m_LV));
		}
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TAchievementTable data = new TAchievementTable(pResult);
		if (data.m_Hide) return;
		if (!DIC_Idx.ContainsKey(data.m_Idx))
		{
			DIC_Idx.Add(data.m_Idx, new Dictionary<int, Dictionary<AchieveType, TAchievementTable>>());


			var group = new TAchievementGroup() { m_Idx = data.m_Idx, m_Type = data.m_Type };
			Groups.Add(group);

			if (!DIC_Tab.ContainsKey(data.m_Tab)) DIC_Tab.Add(data.m_Tab, new List<TAchievementGroup>());
			DIC_Tab[data.m_Tab].Add(group);
		}

		Groups.Find(t => t.m_Idx == data.m_Idx)?.Add(data);

		if (!DIC_Value.ContainsKey(data.m_Type)) DIC_Value.Add(data.m_Type, new List<long>());
		if (!DIC_Value[data.m_Type].Any(o => o == data.m_Values[0])) DIC_Value[data.m_Type].Add(data.m_Values[0]);

		if (!DIC_Idx.ContainsKey(data.m_Idx)) DIC_Idx.Add(data.m_Idx, new Dictionary<int, Dictionary<AchieveType, TAchievementTable>>());
		if (!DIC_Idx[data.m_Idx].ContainsKey(data.m_LV)) DIC_Idx[data.m_Idx].Add(data.m_LV, new Dictionary<AchieveType, TAchievementTable>());
		if (!DIC_Idx[data.m_Idx][data.m_LV].ContainsKey(data.m_Type)) DIC_Idx[data.m_Idx][data.m_LV].Add(data.m_Type, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// AchievementTable
	TAchievementTableMng m_Achievement = new TAchievementTableMng();

	public List<TAchievementGroup> GetAchievementGroup()
	{
		return m_Achievement.Groups;
	}

	public List<TAchievementGroup> GetAchievementTabGroup(TAchievementTable.Tab Tab)
	{
		if (!m_Achievement.DIC_Tab.ContainsKey(Tab)) return null;
		return m_Achievement.DIC_Tab[Tab];
	}

	public TAchievementTable GetAchievementTable(int Idx, int lv, AchieveType type) {
		if (!m_Achievement.DIC_Idx.ContainsKey(Idx)) return null;
		if (!m_Achievement.DIC_Idx[Idx].ContainsKey(lv)) return null;
		if (!m_Achievement.DIC_Idx[Idx][lv].ContainsKey(type)) return null;
		return m_Achievement.DIC_Idx[Idx][lv][type];
	}

	public List<long> GetAchieveValueList(AchieveType type)
	{
		if (!m_Achievement.DIC_Value.ContainsKey(type)) return new List<long>();
		return m_Achievement.DIC_Value[type];
	}
}

