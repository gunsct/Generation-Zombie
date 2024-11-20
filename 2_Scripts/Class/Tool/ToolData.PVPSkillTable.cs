using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPVPSkillTable : ClassMng
{
	/// <summary> 고유 랭크인덱스 </summary>
	public int m_Idx;
	/// <summary> 스킬 타입 (어태커 / 서포터 스킬은 각각 개별 분류) </summary>
	public PVPSkillType m_Type;
	/// <summary> 스킬 이름 인덱스 (String_Etc의 Index 참조) </summary>
	public int m_Name;
	/// <summary> 스킬 설명 인덱스 (String_Etc의 Index 참조) </summary>
	public int m_Desc;
	/// <summary> 스킬 관련 값 0스탯,1범위,2값 </summary>
	public float[] m_Vals = new float[3];
	/// <summary> 스킬에 종속된 무기 타입 (무기 타입 별 공격 범위가 달라짐) </summary>
	public ItemType m_Weapon;
	/// <summary> 공격 타입 </summary>
	public PVPEquipAtkType m_AtkType;
	/// <summary> 공격 우선 순위 타겟 타입
	///서포터의 경우 해당 컬럼에 조건이 적용 되어 있을 시, 그 기준은 서포터
	///귀속된 어태커를 기준으로 위치 계산</summary>
	public PVPAtkTargetType m_AtkTartgetType;
	/// <summary> 해당 스킬 사용 시 소모되는 스테이터스 종류 01 </summary>
	public StatType m_UseStatType;
	/// <summary> 해당 스킬 사용 시 소모되는 스테이터스 수치 값 01 </summary>
	public int m_UseStatVal;
	/// <summary> 해당 스킬 사용에 피격 당할 경우 감소될 위생 수치 </summary>
	public int m_AtkHygDmg;
	/// <summary> 공격 명중 보정 값 </summary>
	public float m_AtkCorrect;
	/// <summary> 서포터 스킬의 타격 / 피격 / 유틸리티 타입  </summary>
	public PVPTurnType m_TurnType;
	/// <summary> TurnType에 따른 스킬 적용 턴 수 </summary>
	public int m_TurnCnt;
	/// <summary> 0일 경우 도망치지 않음, 백분율 </summary>
	public float m_RunPer;
	/// <summary> ap 회복량 보정 기존 회복량 * (1-보정값) </summary>
	public float m_SpeedCorrection;

	public TPVPSkillTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<PVPSkillType>();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) m_Vals[i] = pResult.Get_Float();
		m_Weapon = pResult.Get_Enum<ItemType>();
		m_AtkType = pResult.Get_Enum<PVPEquipAtkType>();
		m_AtkTartgetType = pResult.Get_Enum<PVPAtkTargetType>();
		m_UseStatType = pResult.Get_Enum<StatType>();
		m_UseStatVal = pResult.Get_Int32();
		m_AtkHygDmg = pResult.Get_Int32();
		m_AtkCorrect = pResult.Get_Float();
		m_TurnType = pResult.Get_Enum<PVPTurnType>();
		m_TurnCnt = pResult.Get_Int32();
		m_RunPer = pResult.Get_Float();
		m_SpeedCorrection = pResult.Get_Float();
	}

	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		//팀은 35201, 적군 35202
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), TDATA.GetString(ToolData.StringTalbe.Etc, m_TurnType == PVPTurnType.Team ? 35201 : 35202), m_Vals[1], m_Vals[2] * 100);
	}
}

public class TPVPSkillTableMng : ToolFile
{
	public Dictionary<int, TPVPSkillTable> DIC_Type = new Dictionary<int, TPVPSkillTable>();

	public TPVPSkillTableMng() : base("Datas/PVPSkillTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVPSkillTable data = new TPVPSkillTable(pResult);
		DIC_Type.Add(data.m_Idx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVPSkillTableMng m_PVPSkill = new TPVPSkillTableMng();

	public TPVPSkillTable GeTPVPSkillTable(int _idx) {
		if (!m_PVPSkill.DIC_Type.ContainsKey(_idx)) return null;
		return m_PVPSkill.DIC_Type[_idx];
	}
}

