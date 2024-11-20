using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;

public class Item_PVP_Ranking_List_Element : Item_PVP_Result_List_Element
{
	[Serializable]
	public struct SCUI
	{
		public GameObject InfoBtn;
		public GameObject CPGroup;
		public TextMeshProUGUI CP;
	}
	[SerializeField] SCUI m_SCUI;
	int m_RankIdx;
	Action<RES_PVP_USER_BASE, int> m_CB;
	public void SetData(int _rankidx, RES_PVP_USER_BASE _info, Action<RES_PVP_USER_BASE, int> _cb) {
		m_RankIdx = _rankidx;
		m_Info = new RES_PVP_USER_BASE[2] { _info, _info };
		m_Pos = m_Info[0].UserNo == USERINFO.m_UID ? 0 : 1;
		m_UpDown = 0;
		m_CB = _cb;

		m_Ranking = new int[2];
		m_Ranking[0] = m_Info[0].Rank;
		m_Ranking[1] = m_Info[1].Rank;

		SetUI(m_Info[0]);
	}
	public void SetData(RES_RANKING_INFO _info, Action<RES_PVP_USER_BASE, int> _cb) {

		RES_PVP_USER_BASE info = new RES_PVP_USER_BASE() {
			UserNo = _info.UserNo,
			Rank = _info.Rank,
			Power = 0,
			Point = new long[2] { _info.Point, _info.Point },
			LV = _info.LV,
			Nation = _info.Nation,
			Profile = _info.Profile,
			Name = _info.Name
		};
		m_RankIdx = _info.PVPRank;
		m_Info = new RES_PVP_USER_BASE[2] { info, info };
		m_Pos = m_Info[0].UserNo == USERINFO.m_UID ? 0 : 1;
		m_UpDown = 0;
		m_CB = _cb;

		m_Ranking = new int[2];
		m_Ranking[0] = m_Info[0].Rank;
		m_Ranking[1] = m_Info[1].Rank;

		SetUI(m_Info[0]);
	}
	public override void SetUI(RES_PVP_USER_BASE _info) {
		if (m_SCUI.CP != null) m_SCUI.CP.text = Utile_Class.CommaValue(_info.Power);

		base.SetUI(_info);
	}
	public void ClickViewInfo() {
		m_CB?.Invoke(m_Info[0], m_RankIdx);
	}
}
