using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_PickUpCharCard : ObjMng
{
	[Serializable]
	public class SUI
	{
		public SpriteRenderer Portrait;
		public SpriteRenderer GradeBG;
		public SpriteRenderer GradeFrame;
		public TextMeshPro Name;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(CharInfo Info) {
		m_SUI.Portrait.sprite = Info.m_TData.GetPortrait();
		m_SUI.GradeBG.sprite = BaseValue.CharBG(Info.m_Grade);
		m_SUI.GradeFrame.sprite = BaseValue.CharFrame(Info.m_Grade);
		m_SUI.Name.text = Info.m_TData.GetCharName();
	}
}