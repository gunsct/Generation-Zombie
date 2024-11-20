using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_Char_Adventure_Simple : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Portrait;
		public Image GradeBG;
		public Image GrageFrame;
		public Item_GradeGroup GradeGroup;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(CharInfo _info) {
		m_SUI.Portrait.sprite = _info.m_TData.GetPortrait();
		SetGrade(_info.m_Grade);
	}
	public void SetGrade(int _grade = 0) {
		int grade = _grade;
		m_SUI.GradeBG.sprite = BaseValue.CharBG(grade);
		if (m_SUI.GrageFrame != null) m_SUI.GrageFrame.sprite = BaseValue.CharFrame(grade);

		m_SUI.GradeGroup.SetData(Mathf.Max(_grade, 1));
	}
}
