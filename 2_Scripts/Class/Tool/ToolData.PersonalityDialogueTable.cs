using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ToolData;

public class TPersonalityDialogueTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 상황 설명 이미지 </summary>
	public PersonalityType m_Type;
	/// <summary> 조건 </summary>
	public DialogueConditionType m_Condition;
	/// <summary> 내용 </summary>
	public List<int> m_Desc = new List<int>();

	public TPersonalityDialogueTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Type = pResult.Get_Enum< PersonalityType>();
		m_Condition = pResult.Get_Enum<DialogueConditionType>();
		for(int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx != 0) m_Desc.Add(idx);
		}
	}

	public string GetDesc() {
		return TDATA.GetString(StringTalbe.Dialog, m_Desc[UTILE.Get_Random(0, m_Desc.Count)]);
	}
}

public class TPersonalityDialogueTableMng : ToolFile
{
	public Dictionary<int, TPersonalityDialogueTable> DIC_Idx = new Dictionary<int, TPersonalityDialogueTable>();
	public Dictionary<DialogueConditionType, Dictionary<PersonalityType, TPersonalityDialogueTable>> DIC_Type = new Dictionary<DialogueConditionType, Dictionary<PersonalityType, TPersonalityDialogueTable>>();

	public TPersonalityDialogueTableMng() : base("Datas/PersonalityDialogueTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPersonalityDialogueTable data = new TPersonalityDialogueTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Type.ContainsKey(data.m_Condition)) DIC_Type.Add(data.m_Condition, new Dictionary<PersonalityType, TPersonalityDialogueTable>());
		if (!DIC_Type[data.m_Condition].ContainsKey(data.m_Type)) DIC_Type[data.m_Condition].Add(data.m_Type, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PersonalityDialogueTable
	TPersonalityDialogueTableMng m_PerconalityDialogue = new TPersonalityDialogueTableMng();

	public TPersonalityDialogueTable GetPersonalityDialogueTable(int idx)
	{
		if (!m_PerconalityDialogue.DIC_Idx.ContainsKey(idx)) return null;
		return m_PerconalityDialogue.DIC_Idx[idx];
	}

	public TPersonalityDialogueTable GetPersonalityDialogueGroupTable(DialogueConditionType _condition, PersonalityType _personality) {
		if (!m_PerconalityDialogue.DIC_Type.ContainsKey(_condition)) return null;
		if (!m_PerconalityDialogue.DIC_Type[_condition].ContainsKey(_personality)) return null;
		return m_PerconalityDialogue.DIC_Type[_condition][_personality];
	}
}
