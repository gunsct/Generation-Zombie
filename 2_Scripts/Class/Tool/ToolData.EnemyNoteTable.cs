using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public enum ENoteType
{
	[InspectorName("일반")]
	Normal = 0,
	[InspectorName("콤보")]
	Combo,
	[InspectorName("슬래쉬")]
	Slash,
	[InspectorName("챠지")]
	Charge,
	[InspectorName("체인")]
	Chain, 
	[InspectorName("고정")]
	Fixing, 
	[InspectorName("랜덤")]
	Random,
	// 공격 노트의경우 Random아래에는 설정하면 안됨
	// 여기부터는 다른 이슈로 사용되는 노트들만 적용해야됨
	/// <summary> 도망용 노트 </summary>
	[HideInInspector]
	Run
}
public enum ENoteSize
{
	Small = 0,
	Medium,
	Large,
	None,
}

public class EnemyNoteTableGroup : ClassMng
{
	public List<EnemyNoteTable> List = new List<EnemyNoteTable>();
	public EnemyNoteTable RunNote;
	/// <summary> 그룹아이디 </summary>
	public int m_GroupID;
	/// <summary> 확률 최대값 </summary>
	public int m_MaxProb = 0;
	public void Add(EnemyNoteTable note)
	{
		if(note.m_Type == ENoteType.Run)
		{
			RunNote = note;
			return;
		}
		m_MaxProb += note.m_Prob;
		List.Add(note);
	}

	public EnemyNoteTable GetRandNoteTable()
	{
		int Rand = UTILE.Get_Random(0, m_MaxProb);
		for(int i = 0; i < List.Count; i++)
		{
			EnemyNoteTable data = List[i];
			if (Rand < data.m_Prob) return data;
			Rand -= data.m_Prob;
		}
		return null;
	}
	public List<EnemyNoteTable> GetNotOverrideNotTables(List<EnemyNoteTable> _prepick) {
		return List.FindAll(o => !_prepick.Contains(o));
	}
	public EnemyNoteTable GetNotOverrideNotTable(List<EnemyNoteTable> _prepick) {
		List<EnemyNoteTable> cannote = List.FindAll(o => !_prepick.Contains(o));
		if(cannote.Count == 0) {
			_prepick.Clear();
			cannote.AddRange(List);
		}
		int maxprop = cannote.Sum(o => o.m_Prob);
		int Rand = UTILE.Get_Random(0, maxprop);
		for (int i = 0; i < cannote.Count; i++) {
			EnemyNoteTable data = cannote[i];
			if (Rand < data.m_Prob) return data;
			Rand -= data.m_Prob;
		}
		return null;
	}
}

public class EnemyNoteTable : ClassMng
{
	/// <summary> 그룹아이디 </summary>
	public int m_GroupID;
	/// <summary> 확률 </summary>
	public int m_Prob;
	/// <summary> type </summary>
	public ENoteType m_Type;
	/// <summary> type </summary>
	public ENoteSize m_Size;
	/// <summary> 종료시간 </summary>
	public float m_EndTime;
	/// <summary> 개수 </summary>
	public int[] m_Cnt = new int[2];
	public EnemyNoteTable(CSV_Result pResult)
	{
		pResult.NextReadPos();  // 인덱스
		m_GroupID = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<ENoteType>();
		m_Size = pResult.Get_Enum<ENoteSize>();
		m_EndTime = pResult.Get_Float();
		m_Cnt[0] = pResult.Get_Int32();
		m_Cnt[1] = pResult.Get_Int32();
	}

	public int GetCreateCnt()
	{
		if (m_Cnt[1] < 1) return 1;
		return UTILE.Get_Random(m_Cnt[0], m_Cnt[1] + 1);
	}
}
public class TEnemyNoteTableMng : ToolFile
{
	public Dictionary<int, EnemyNoteTableGroup> DIC_GID = new Dictionary<int, EnemyNoteTableGroup>();

	public TEnemyNoteTableMng() : base("Datas/EnemyNoteTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		EnemyNoteTable data = new EnemyNoteTable(pResult);
		if (!DIC_GID.ContainsKey(data.m_GroupID)) DIC_GID.Add(data.m_GroupID, new EnemyNoteTableGroup());
		DIC_GID[data.m_GroupID].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemyNoteTable
	TEnemyNoteTableMng m_EnemyNote = new TEnemyNoteTableMng();

	public EnemyNoteTableGroup GetEnemyNoteTableGroup(int groupid) {
		if (!m_EnemyNote.DIC_GID.ContainsKey(groupid)) return null;
		return m_EnemyNote.DIC_GID[groupid];
	}
}

