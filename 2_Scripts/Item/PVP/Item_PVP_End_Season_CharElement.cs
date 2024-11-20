using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_PVP_End_Season_CharElement : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] Ranking;
		public Image Portrait;
		public TextMeshProUGUI Lv;
		public Image Nation;
		public Text Name;
	}
	[SerializeField] SUI m_SUI;
	public void SetData(RES_RANKING_INFO _info, int _pos = 3) {
		string ranking = string.Empty;
		int rankpos = 0;

		if (_info == null) {
			m_SUI.Anim.SetTrigger("Empty");
			rankpos = _pos;
			m_SUI.Ranking[0].text = m_SUI.Ranking[1].text = ranking;
			m_SUI.Portrait.sprite = TDATA.GetUserProfileImage(0);
		}
		else {
			m_SUI.Anim.SetTrigger("Normal");
			rankpos = _info.Rank;
			m_SUI.Portrait.sprite = TDATA.GetUserProfileImage(_info.Profile);
			m_SUI.Lv.text = _info.LV.ToString();
			m_SUI.Nation.sprite = BaseValue.GetNationIcon(_info.Nation);
			m_SUI.Name.text = _info.Name;
		}
		switch (rankpos) {
			case 1: ranking = "1st"; break;
			case 2: ranking = "2nd"; break;
			case 3: ranking = "3rd"; break;
		}
		m_SUI.Ranking[0].text = m_SUI.Ranking[1].text = ranking;
	}
}
