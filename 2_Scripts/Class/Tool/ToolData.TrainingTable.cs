using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolData;

public class TTrainingTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 표시 레벨  </summary>
	public int m_LV;
	/// <summary> 트레이닝 타입 </summary>
	public TrainingType m_Type;
	/// <summary> 트레이닝 이름 </summary>
	public int m_Name;
	/// <summary> 바 세팅 </summary>
	public string[] m_Areas = new string[3];
	/// <summary> 스피드 </summary>
	public float m_Speed;
	/// <summary> 시간 제한 </summary>
	public float m_Time;
	/// <summary> 회차별 딜레이 </summary>
	public float m_PointDelay;
	/// <summary> 타이밍 바 행동 타입 </summary>
	public PointType m_PointType;
	/// <summary> 실패시 타일 리셋하는가 </summary>
	public bool m_FailTileReset;
	/// <summary> 제한시간이 턴당이 아니라 전체로 사용하는가 </summary>
	public bool m_AllTimeLimit;
	/// <summary> 바가 매턴 리셋되는지 </summary>
	public bool m_PointReset;

	public TTrainingTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_LV = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<TrainingType>();
		m_Name = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) m_Areas[i] = pResult.Get_String();
		m_Speed = pResult.Get_Float();
		m_Time = pResult.Get_Float();
		m_PointDelay = pResult.Get_Float();
		m_PointType = pResult.Get_Enum<PointType>();
		m_FailTileReset = pResult.Get_Boolean();
		m_AllTimeLimit = pResult.Get_Boolean();
		m_PointReset = pResult.Get_Boolean();
	}

	public string GetName() {
		return TDATA.GetString(StringTalbe.Etc, m_Name);
	}
}

public class TTrainingTableMng : ToolFile
{
	public Dictionary<int, TTrainingTable> DIC_Idx = new Dictionary<int, TTrainingTable>();

	public TTrainingTableMng() : base("Datas/TrainingTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TTrainingTable data = new TTrainingTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// TrainingTable
	TTrainingTableMng m_Training = new TTrainingTableMng();

	public TTrainingTable GetTrainingTable(int idx)
	{
		if (!m_Training.DIC_Idx.ContainsKey(idx)) return null;
		return m_Training.DIC_Idx[idx];
	}
}
