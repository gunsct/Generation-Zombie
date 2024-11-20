using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PVP_Result_List_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image[] RankBGnMark;
		public Color[] RankBGColor;
		public Color[] RankMarkColor;
		public Color[] FrameColor;
		public Color[] BGColor;
		public Color[] RankingColor;
		public GameObject MyRankBG;
		public Image Nation;
		public Image Portrait;
		public Image Frame;
		public Image BG;
		public Text Name;
		public TextMeshProUGUI Lv;
		public TextMeshProUGUI Rank;
		public TextMeshProUGUI LP;
		public TextMeshProUGUI Order;
		public Image OrderArrow;
		public GameObject ChangeGroup;
	}
	[SerializeField] SUI m_SUI;
	public RES_PVP_USER_BASE[] m_Info = new RES_PVP_USER_BASE[2];
	public int m_Pos;                  //0:본인,1:이외
	public int m_UpDown;                //1:업,0:그대로,-1:다운
	public int[] m_Ranking = new int[2];

	public void SetData(RES_PVP_USER_BASE[] _info) {
		m_Info = _info;
		m_Pos = m_Info[0].UserNo == USERINFO.m_UID ? 0 : 1;
		if (_info[0].Rank > _info[1].Rank) m_UpDown = 1;
		else if (_info[0].Rank < _info[1].Rank) m_UpDown = -1;
		else m_UpDown = 0;

		m_Ranking = new int[2];
		m_Ranking[0] = _info[0].Rank;
		m_Ranking[1] = _info[1].Rank;

		SetUI(m_Info[0]);
	}
	public virtual void SetUI(RES_PVP_USER_BASE _info) {
		m_SUI.MyRankBG.SetActive(m_Pos == 0);
		m_SUI.Rank.text = _info.Rank.ToString();
		m_SUI.Rank.color = m_SUI.RankingColor[m_Pos];
		m_SUI.RankBGnMark[0].color = m_SUI.RankBGColor[m_Pos];
		m_SUI.RankBGnMark[1].color = m_SUI.RankMarkColor[m_Pos];
		m_SUI.Portrait.sprite = TDATA.GetUserProfileImage(_info.Profile);
		m_SUI.Frame.color = m_SUI.FrameColor[m_Pos];
		m_SUI.BG.color = m_SUI.BGColor[m_Pos];
		m_SUI.Lv.text = _info.LV.ToString();
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(_info.Nation);
		m_SUI.Name.text = _info.Name;
		m_SUI.LP.text = Utile_Class.CommaValue(_info.Point[1]);
		m_SUI.ChangeGroup.gameObject.SetActive(m_UpDown != 0);
		if (m_UpDown != 0) {
			m_SUI.Order.text = Mathf.Abs(m_Info[0].Rank - m_Info[1].Rank).ToString();
			m_SUI.Order.color = Utile_Class.GetCodeColor(m_UpDown == 1 ? "#FF693D" : "#65D447");
			m_SUI.OrderArrow.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_SV_{0}", m_UpDown == 1 ? "Up" : "Down"), "png");
		}
	}
	public void OrderChange() {
		SetUI(m_Info[1]);
	}
}
