using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PVP_Research_List : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_PVP_Research_Element[] Elements;
	}
	[SerializeField] SUI m_SUI;
	ResearchType m_Type;
	int m_Line;
	int m_OpenCampLv;
	public void SetData(ResearchType _type, int _line, Action<ResearchInfo, Action> _cb) {
		m_Type = _type;
		m_Line = _line;
		m_OpenCampLv = TDATA.GetPVP_CampLvFromTire(0);
		for (int i = 0; i < m_SUI.Elements.Length; i++) {
			TResearchTable tdata = TDATA.GetResearchTablePos(m_Type, m_Line, i);
			if (tdata == null) m_SUI.Elements[i].gameObject.SetActive(false);
			else {
				m_SUI.Elements[i].gameObject.SetActive(true);
				ResearchInfo info = USERINFO.GetResearchInfo(tdata.m_Type, tdata.m_Idx);
				m_SUI.Elements[i].SetData(info, _cb);
			}
		}
	}
}
