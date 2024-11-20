using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_Mk_CharEquip : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Img;
		public Image GradeBG;
		public Image Frame;
	}
	[SerializeField] SUI m_SUI;
	int m_Idx;
	TItemTable m_TData { get { return TDATA.GetItemTable(m_Idx); } }

	public void SetData(int _idx) {
		m_Idx = _idx;
		m_SUI.Img.sprite = m_TData.GetItemImg();
		m_SUI.GradeBG.sprite = BaseValue.CharBG(m_TData.m_Grade);
		m_SUI.Frame.sprite = BaseValue.GradeFrame(m_TData.m_Type, m_TData.m_Grade);
	}
}
