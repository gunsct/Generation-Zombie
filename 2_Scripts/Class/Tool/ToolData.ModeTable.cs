using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TModeTable : ClassMng
{
	/// <summary> 모드 타입 </summary>
	public StageContentType m_Content;
	/// <summary> 오픈 요일 (일, 월, 화, 수, 목, 금, 토) </summary>
	public int m_OpenDay;
	/// <summary> 위치 </summary>
	public int m_Pos;
	/// <summary> 난이도, (순서), 앞선 난이도를 클리어 해야 오픈 </summary>
	public int m_Difficulty;
	/// <summary> 해당 모드에서 사용할 스테이지 Index </summary>
	public int m_StageIdx;
	/// <summary> 스테이지 난이도 제한 </summary>
	public StageDifficultyType m_DiffType;
	/// <summary> 스테이지 제한 </summary>
	public int m_StageLimit;
	public TModeTable(CSV_Result pResult) {
		m_Content = pResult.Get_Enum<StageContentType>();
		m_OpenDay = pResult.Get_Int32();
		m_Pos = pResult.Get_Int32();
		m_Difficulty = pResult.Get_Int32();
		m_StageIdx = pResult.Get_Int32();
		m_DiffType = StageDifficultyType.Normal;
		m_StageLimit = pResult.Get_Int32();
	}
}
public class TModeTableMng : ToolFile
{
	public Dictionary<StageContentType, Dictionary<DayOfWeek, List<TModeTable>[]>> DIC_Type = new Dictionary<StageContentType, Dictionary<DayOfWeek, List<TModeTable>[]>>();
	public Dictionary<int, TModeTable> DIC_Stage = new Dictionary<int, TModeTable>();

	public TModeTableMng() : base("Datas/ModeTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
		DIC_Stage.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TModeTable data = new TModeTable(pResult);

		if (!DIC_Type.ContainsKey(data.m_Content)) DIC_Type.Add(data.m_Content, new Dictionary<DayOfWeek, List<TModeTable>[]>());

		DayOfWeek week = (DayOfWeek)data.m_OpenDay;
		if (!DIC_Type[data.m_Content].ContainsKey(week)) DIC_Type[data.m_Content].Add(week, new List<TModeTable>[4] { new List<TModeTable>(), new List<TModeTable>(), new List<TModeTable>(), new List<TModeTable>() });
		DIC_Type[data.m_Content][week][data.m_Pos].Add(data);

		if (!DIC_Stage.ContainsKey(data.m_StageIdx)) DIC_Stage.Add(data.m_StageIdx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ModeTable
	TModeTableMng m_Mode = new TModeTableMng();

	public List<TModeTable> GetModeTable(StageContentType content, DayOfWeek week, int pos)
	{
		if (!m_Mode.DIC_Type.ContainsKey(content)) return new List<TModeTable>();
		if (!m_Mode.DIC_Type[content].ContainsKey(week)) return new List<TModeTable>();
		return m_Mode.DIC_Type[content][week][pos];
	}

	public TModeTable GetModeTable(int stageidx)
	{
		if (!m_Mode.DIC_Stage.ContainsKey(stageidx)) return null;
		return m_Mode.DIC_Stage[stageidx];
	}

	public TModeTable GetModeTable(StageContentType content, int lv, DayOfWeek week = DayOfWeek.Sunday, int pos = 0)
	{
		return GetModeTable(content, week, pos).Find(obj => obj.m_Difficulty == lv);
	}
}
