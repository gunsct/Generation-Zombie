using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TConditionDialogueGroupTable : ClassMng
{
	public class Personality
	{
		public PersonalityType Type;
		public float Prop;
	}
	/// <summary> 그룹 인덱스 </summary>
	public int m_Gid;
	/// <summary> 조건 </summary>
	public DialogueConditionType m_Condition;
	/// <summary> 조건 값 </summary>
	public int m_ConditionVal;
	/// <summary> uistring etc 인덱스 </summary>
	public int m_StringIdx;
	/// <summary> 말풍선 타입 </summary>
	public CharDialogueType m_SpeechType;
	/// <summary> 확률 </summary>
	public float m_Prob;
	/// <summary> 성격 대사 </summary>
	public List<Personality> m_Personalitys = new List<Personality>();

	public TConditionDialogueGroupTable(CSV_Result pResult) {
		m_Gid = pResult.Get_Int32();
		m_Condition = pResult.Get_Enum<DialogueConditionType>();
		m_ConditionVal = pResult.Get_Int32();
		m_StringIdx = pResult.Get_Int32();
		m_SpeechType = pResult.Get_Enum<CharDialogueType>();
		m_Prob = pResult.Get_Float();
		for(int i = 0; i < 2; i++) {
			PersonalityType type = pResult.Get_Enum<PersonalityType>();
			if (type != PersonalityType.None) {
				m_Personalitys.Add(new Personality() {
					Type = type,
					Prop = pResult.Get_Float()
				});
			}
			else pResult.NextReadPos();
		}
	}
	public bool IS_CanSpeech() {
		return UTILE.Get_Random(0f, 1f) < m_Prob;
	}
	public string GetStr() {
		return TDATA.GetString(ToolData.StringTalbe.Dialog, m_StringIdx);
	}
}

public class TConditionDialogueGroupTableMng : ToolFile
{
	public Dictionary<int, Dictionary<DialogueConditionType, TConditionDialogueGroupTable>> DIC_Group = new Dictionary<int, Dictionary<DialogueConditionType, TConditionDialogueGroupTable>>();
	public TConditionDialogueGroupTableMng() : base("Datas/ConditionDialogueGroupTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_Group.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TConditionDialogueGroupTable data = new TConditionDialogueGroupTable(pResult);

		if (!DIC_Group.ContainsKey(data.m_Gid)) DIC_Group.Add(data.m_Gid, new Dictionary<DialogueConditionType, TConditionDialogueGroupTable>());
		if (!DIC_Group[data.m_Gid].ContainsKey(data.m_Condition)) DIC_Group[data.m_Gid].Add(data.m_Condition, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ConditionDialogueGroupTable
	TConditionDialogueGroupTableMng m_ConditionGroup = new TConditionDialogueGroupTableMng();

	public TConditionDialogueGroupTable GetTConditionDialogueGroupTable(int _gid, DialogueConditionType _condition) {
		if (!m_ConditionGroup.DIC_Group.ContainsKey(_gid)) return null;
		if (!m_ConditionGroup.DIC_Group[_gid].ContainsKey(_condition)) return null;
		return m_ConditionGroup.DIC_Group[_gid][_condition];
	}
}
