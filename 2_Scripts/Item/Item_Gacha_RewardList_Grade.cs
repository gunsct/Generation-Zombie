using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_Gacha_RewardList_Grade : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI m_Name;
		public Item_GradeGroup m_Grade;
		public TextMeshProUGUI m_Prob;
		public Image m_NameBG;
		public GameObject m_High;
		public Color[] m_BGColor;
		public Color[] m_ProbColor;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(int _grade, float _prob, bool _equip = true) {
		m_SUI.m_Name.text = string.Format(_equip ? TDATA.GetString(682) : TDATA.GetString(7107), _grade);
		m_SUI.m_NameBG.color = m_SUI.m_BGColor[_grade - 1];
		m_SUI.m_Grade.SetData(_grade);
		m_SUI.m_Prob.text = string.Format("{0:0.00}<size=80%>%</size>", _prob * 100f);
		m_SUI.m_Prob.color = m_SUI.m_ProbColor[_grade - 1];
		m_SUI.m_High.SetActive(_grade > 5);
	}
}
