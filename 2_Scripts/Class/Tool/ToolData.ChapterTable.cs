using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TChapterTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 난이도 </summary>
	public StageDifficultyType m_Diff;
	/// <summary> 챕터 넘버 </summary>
	public int m_Chapter;
	/// <summary> 스테이지 인덱스 </summary>
	public int m_Stage;
	/// <summary> 보상 타입 </summary>
	public RewardKind m_RewardType;
	/// <summary> 챕터 완료 시 지급될 보상. ItemTable Index 참조 </summary>
	public int m_Reward;
	/// <summary> 보상 아이템 수량 </summary>
	public int m_RewardCount;
	/// <summary> 캐릭터, 장비 보상 레벨로 쓸듯</summary>
	public int m_Val;

	public TChapterTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Diff = pResult.Get_Enum<StageDifficultyType>();
		m_Chapter = pResult.Get_Int32();
		m_Stage = pResult.Get_Int32();
		m_RewardType = pResult.Get_Enum<RewardKind>();
		m_Reward = pResult.Get_Int32();
		m_RewardCount = pResult.Get_Int32();
		m_Val = pResult.Get_Int32();
	}
}

public class TChapterTableMng : ToolFile
{
	public Dictionary<StageDifficultyType, Dictionary<int, TChapterTable>> DIC_Type = new Dictionary<StageDifficultyType, Dictionary<int, TChapterTable>>();

	public TChapterTableMng() : base("Datas/ChapterTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TChapterTable data = new TChapterTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_Diff)) DIC_Type.Add(data.m_Diff, new Dictionary<int, TChapterTable>());
		if (!DIC_Type[data.m_Diff].ContainsKey(data.m_Stage)) DIC_Type[data.m_Diff].Add(data.m_Stage, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ChapterTable
	TChapterTableMng m_Chapter = new TChapterTableMng();

	public TChapterTable GetChapterTable(int _diff, int _stgidx) {
		TChapterTable table = null;
		if(m_Chapter.DIC_Type.ContainsKey((StageDifficultyType)_diff) && m_Chapter.DIC_Type[(StageDifficultyType)_diff].ContainsKey(_stgidx))
			table = m_Chapter.DIC_Type[(StageDifficultyType)_diff][_stgidx];
		return table;
	}
}
