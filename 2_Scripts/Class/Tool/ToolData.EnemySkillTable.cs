using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum EAtkType
{
	[Tooltip("없음")]
	None = 0,
	[Tooltip("타격")]
	Hit,
	[Tooltip("잡기")]
	Catch,
	[Tooltip("연사(ex. 라이플)")]
	Continuous,
	[Tooltip("이동(ex. 스나이퍼)")]
	Move,
	End
}

public class EnemySkillTableGroup : ClassMng
{
	public List<EnemySkillTable> List = new List<EnemySkillTable>();
	/// <summary> 그룹아이디 </summary>
	public int m_GroupID;
	/// <summary> 확률 최대값 </summary>
	public int m_MaxProb = 0;
	public void Add(EnemySkillTable note)
	{
		m_MaxProb += note.m_Prob;
		List.Add(note);
	}

	public int GetRandSkill()
	{
		int Rand = UTILE.Get_Random(0, m_MaxProb);
		for (int i = 0; i < List.Count; i++)
		{
			EnemySkillTable data = List[i];
			if (Rand < data.m_Prob) return i;
			Rand -= data.m_Prob;
		}
		return 0;
	}

	public EnemySkillTable GetStageAISkill()
	{
		List<EnemySkillTable> skills = List.FindAll(skill => skill.IS_StageAISKILL());
		int maxprob = 0;
		for(int i = skills.Count - 1; i > -1; i--)
		{
			maxprob += skills[i].m_Prob;
		}
		int Rand = UTILE.Get_Random(0, maxprob);
		for (int i = 0; i < List.Count; i++)
		{
			EnemySkillTable data = skills[i];
			if (Rand < data.m_Prob) return data;
			Rand -= data.m_Prob;
		}
		return null;
	}
}

public class EnemySkillTable : ClassMng
{
	/// <summary> 그룹아이디 </summary>
	public int m_GroupID;
	/// <summary> 확률 </summary>
	public int m_Prob;
	/// <summary> type </summary>
	public EAtkType m_Type;
	/// <summary> type </summary>
	public ENoteSize m_Size;
	/// <summary> 개수 </summary>
	public int[] m_Cnt = new int[2];
	/// <summary> 전조 시간 </summary>
	public float[] m_SignTime = new float[2];
	/// <summary>
	/// 스킬 값
	/// <para> Catch : 0 -> 시간, 1 -> 체인 카운트 min, 체인 카운트 max </para>
	/// </summary>
	public float[] m_SkillValues = new float[3];

	/// <summary>
	/// 스킬 값
	/// <para> Catch : 0 -> 시간, 1 -> 체인 카운트 min, 체인 카운트 max </para>
	/// </summary>
	public Dictionary<StatType, int> m_SrvDmg = new Dictionary<StatType, int>();
	public float m_DefTime;


	public EnemySkillTable(CSV_Result pResult)
	{
		pResult.NextReadPos();  // 인덱스
		m_GroupID = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<EAtkType>();
		m_Size = pResult.Get_Enum<ENoteSize>();
		m_Cnt[0] = pResult.Get_Int32();
		m_Cnt[1] = pResult.Get_Int32();
		m_SignTime[0] = pResult.Get_Float();
		m_SignTime[1] = pResult.Get_Float();
		for(int i = 0; i < 3; i++) m_SkillValues[i] = pResult.Get_Float();
		m_SrvDmg.Add(StatType.Men, pResult.Get_Int32());
		m_SrvDmg.Add(StatType.Hyg, pResult.Get_Int32());
		m_SrvDmg.Add(StatType.Sat, pResult.Get_Int32());
		m_DefTime = pResult.Get_Float();
	}

	public int GetCnt()
	{
		return UTILE.Get_Random(m_Cnt[0], m_Cnt[1] + 1);
	}
	public bool IS_StageAISKILL()
	{
		switch (m_Type)
		{
		case EAtkType.Catch:
			return false;
		}
		return true;
	}

	public int Get_SrvDmg(StatType type)
	{
		if (!m_SrvDmg.ContainsKey(type)) return 0;
		return m_SrvDmg[type];

	}
}
public class TEnemySkillTableMng : ToolFile
{
	public Dictionary<int, EnemySkillTableGroup> DIC_GID = new Dictionary<int, EnemySkillTableGroup>();

	public TEnemySkillTableMng() : base("Datas/EnemySkillTable")
	{
	}

	public override void CheckData() { }

	public override void DataInit()
	{
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		EnemySkillTable data = new EnemySkillTable(pResult);
		if (!DIC_GID.ContainsKey(data.m_GroupID)) DIC_GID.Add(data.m_GroupID, new EnemySkillTableGroup() { m_GroupID = data.m_GroupID });
		DIC_GID[data.m_GroupID].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemySkillTable
	TEnemySkillTableMng m_EnemySkill = new TEnemySkillTableMng();

	public EnemySkillTableGroup GetEnemySkillTableGroup(int groupid) {
		if (!m_EnemySkill.DIC_GID.ContainsKey(groupid)) return new EnemySkillTableGroup() { m_GroupID = 0};
		return m_EnemySkill.DIC_GID[groupid];
	}
}

