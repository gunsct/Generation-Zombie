using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Ranking_Element : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Rank;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Point;
		public Text Name;
		public Image Profile;
		public Image Nation;
	}

	[SerializeField] SUI m_SUI;
#pragma warning restore 0649
	public RES_RANKING_INFO m_Info;
	public void SetData(RES_RANKING_INFO info, int SameRankCnt = 1, bool FirstData = true)
	{
		m_Info = info;
		m_SUI.Rank.text = info.Rank < 1 ? "-" : info.Rank.ToString();
		m_SUI.LV.text = info.LV.ToString();
		m_SUI.Name.text = info.m_Name;
		m_SUI.Point.text = Utile_Class.CommaValue(info.Point);
		m_SUI.Profile.sprite = TDATA.GetUserProfileImage(info.Profile);
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(info.Nation);
	}
}
