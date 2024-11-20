using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Union_Research_EffectList_Element : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Value;
	}

	[SerializeField] SUI m_sUI;
	TResearchTable.Effect m_Eff;
#pragma warning restore 0649
	public void SetData(TResearchTable.Effect eff)
	{
		m_Eff = eff;
		SetUI();
	}

	void SetUI()
	{
		m_sUI.Name.text = m_Eff.GetName();
		// 수치 값 셋팅
		var value = m_Eff.m_Value;
		switch (m_Eff.m_Eff)
		{
		case ResearchEff.MakingOpen:
		case ResearchEff.BulletMaxUp:
		case ResearchEff.AdventureOpen:
		case ResearchEff.TrainingOpen:
		case ResearchEff.AdventureCountUp:
		case ResearchEff.AdventureLevelUp:
		case ResearchEff.RemodelingOpen:
		case ResearchEff.GuardMaxUp:
		case ResearchEff.MakingLevelUp:
		case ResearchEff.SupplyBoxGradeUp:
		case ResearchEff.MemberMaxUp:
			m_sUI.Value.text = value.ToString();
			break;
		default:
			m_sUI.Value.text = string.Format("{0}%", Mathf.RoundToInt(value * 100));
			break;
		}
	}
}
