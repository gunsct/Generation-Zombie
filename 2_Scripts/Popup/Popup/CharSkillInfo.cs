using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class CharSkillInfo : PopupBase
{
	[Serializable]
	public struct SUI
	{
		//터치불가 스킬 아이콘 2개
		public Item_Skill_Card[] Skill;//아이콘에 버튼 해제
		public TextMeshProUGUI[] SkillDesc;
	}
	[SerializeField]
	SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		CharInfo info = (CharInfo)aobjValue[0];
		bool isSetEquip = info.IS_SetEquip();
		for(int i = 0; i < 2; i++)
		{
			SkillType type = (SkillType)i;
			if (i == 0 && isSetEquip) type = SkillType.SetActive;

			m_SUI.Skill[i].SetData(info.m_Skill[i].m_Idx, info.m_Skill[i].m_LV, false, type == SkillType.SetActive, info);
			m_SUI.SkillDesc[i].text = info.m_Skill[i].m_TData.GetInfo(info.m_Skill[i].m_LV);
		}
	}
}
