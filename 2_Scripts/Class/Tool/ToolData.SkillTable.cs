using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TSkillTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 스킬이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Info;
	/// <summary> 스킬 종류 </summary>
	public SkillKind m_Kind;
	/// <summary> 기본 값 </summary>
	public float m_Base;
	/// <summary> 레벨당 증가량 </summary>
	public float m_Up;
	/// <summary> 최대 레벨 </summary>
	public int m_MaxLV;
	/// <summary> 스킬 설정 값 </summary>
	public float[] m_Value = new float[3];
	/// <summary> 스킬 성공 확률 </summary>
	public float m_SuccProb;
	/// <summary> 모방 스킬에서 나올 확률 </summary>
	public int m_LearningProb;
	/// <summary> 영역 타입 </summary>
	public SkillAreaType m_AreaType;
	/// <summary> 스킬 타입 </summary>
	public SkillType m_Type;
	/// <summary> 스킬 등급 </summary>
	public Grade m_Grade;
	/// <summary> 아이콘 </summary>
	public string m_Icon;
	/// <summary> 스킬 사용시 감소 행동력 </summary>
	public int m_BaseAP;
	/// <summary> 액티브 스킬 쿨타임 </summary>
	public int m_Cool;

	public TSkillTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Info = pResult.Get_Int32();
		m_Kind = pResult.Get_Enum<SkillKind>();
		m_Base = pResult.Get_Float();
		m_Up = pResult.Get_Float();
		m_MaxLV = pResult.Get_Int32();
		m_Value[0] = pResult.Get_Float();
		m_Value[1] = pResult.Get_Float();
		m_Value[2] = pResult.Get_Float();
		m_SuccProb = pResult.Get_Float();
		m_LearningProb = pResult.Get_Int32();
		m_AreaType = pResult.Get_Enum<SkillAreaType>();
		m_Type = pResult.Get_Enum<SkillType>();
		m_Grade = pResult.Get_Enum<Grade>();
		m_Icon = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Icon) && !m_Icon.Contains("/"))
			Debug.LogError($"[ SkillTable ({m_Idx}) ] m_Icon 패스 체크할것");
#endif
		m_BaseAP = pResult.Get_Int32();
		m_Cool = pResult.Get_Int32();
	}

	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetTypeName() {
		return TDATA.GetString(ToolData.StringTalbe.UI, BaseValue.SKILL_TYPE_IDX + (int)m_Type);
	}

	public string GetInfoColorCode(int ColorMode)
	{
		switch(ColorMode)
		{
		case 1: return "#AE5C00";
		}
		return "#dbbb4f";
	}

	public string GetInfo(int lv = 1, int ColorMode = 0) {
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Info), string.Format("{0:0.##}", GetValue(lv)), GetInfoColorCode(ColorMode), string.Format("{0:0.##}", GetValue(lv) * 100f));
	}

	public UnityEngine.Sprite GetImg()
	{
		return UTILE.LoadImg(m_Icon, "png");
	}

	public float GetValue(int lv = 1)
	{
		if (lv > m_MaxLV) lv = m_MaxLV;
		return (m_Base + (lv - 1f) * m_Up);
	}
	public StatType GetStatType() {
		switch (m_Kind) {
			case SkillKind.AtkUp:return StatType.Atk;
			case SkillKind.DefUp:return StatType.Def;
			case SkillKind.TotalHpUp:return StatType.HP;
			case SkillKind.HealUp:return StatType.Heal;
			default: return StatType.None;
		}
	}
}

public class TSkillTableMng : ToolFile
{
	public Dictionary<int, TSkillTable> DIC_Idx = new Dictionary<int, TSkillTable>();
	public Dictionary<Grade, List<SkillKind>> DIC_Grade = new Dictionary<Grade, List<SkillKind>>();

	public TSkillTableMng() : base("Datas/SkillTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Grade.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSkillTable data = new TSkillTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Grade.ContainsKey(data.m_Grade))
			DIC_Grade.Add(data.m_Grade, new List<SkillKind>());
		DIC_Grade[data.m_Grade].Add((SkillKind)data.m_Idx);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SkillTable
	TSkillTableMng m_Skill = new TSkillTableMng();

	public TSkillTable GetSkill(int idx)
	{
		if (!m_Skill.DIC_Idx.ContainsKey(idx)) return null;
		return m_Skill.DIC_Idx[idx];
	}
	public TSkillTable GetSkillTable(SkillKind _kind) {
		return m_Skill.DIC_Idx.Values.ToList().Find(o => o.m_Kind == _kind);
	}

	public TSkillTable GetRandomSkill(bool _setskill)
	{
		List<TSkillTable> list = new List<TSkillTable>(m_Skill.DIC_Idx.Values).FindAll(t => {
			if (t.m_LearningProb < 1) return false;
			switch (t.m_Kind)
			{
			case SkillKind.LearningAbility:
			case SkillKind.CoolReset:
				return false;
			}
			//테이블은 세트 스킬인데 발동 모발이 세트스킬이 아닌 경우
			if (t.m_Type == SkillType.SetActive && !_setskill) return false;
			return true;
		});

		int total = 0;
		for(int i = 0; i < list.Count; i++) total += list[i].m_LearningProb;

		int rand = UTILE.Get_Random(0, total);
		for (int i = 0; i < list.Count; i++)
		{
			if (rand < list[i].m_LearningProb) return list[i];
			rand -= list[i].m_LearningProb;
		}
		return list[0];
	}

	public List<SkillKind> GetGradeSkill(Grade _type) {
		if (!m_Skill.DIC_Grade.ContainsKey(_type)) return null;
		return m_Skill.DIC_Grade[_type];
	}
}
