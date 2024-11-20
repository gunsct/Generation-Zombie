using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;

public class Item_PDA_Ranking_Main : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public Animator Ani;

		public TextMeshProUGUI Title;
		public TextMeshProUGUI Info;
		public TextMeshProUGUI ValueTitle;

		// 유저 리스트
		public RectTransform List;
		public RectTransform Prefab;

		public Item_PDA_Ranking_Element MyInfo;

		public TextMeshProUGUI[] PageLabels;
		public Animator[] PageAnis;
	}

	[SerializeField] SUI m_SUI;
	RankType m_Page = RankType.END;
	// 탭내이동은 한번씩만 하도록 변경
	Dictionary<RankType, RES_RANKING> m_Data = new Dictionary<RankType, RES_RANKING>();

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		if (TUTO.IsTuto(TutoKind.Making, (int)TutoType_Making.Select_Making)) TUTO.Next();
		if (args == null) return;
		RankType type = (RankType)args[0];
		m_Data.Add(type, (RES_RANKING)args[1]);

		for(int i = 0; i < m_SUI.PageLabels.Length; i++) m_SUI.PageLabels[1].text = GetTitle((RankType)i);

		m_Page = type;
		SetUI(type);
		SetPage(type);
	}

	string GetTitle(RankType type)
	{
		switch (type)
		{
		case RankType.Power: return string.Format(TDATA.GetString(591), TDATA.GetString(592));
		case RankType.StageCnt: return string.Format(TDATA.GetString(591), TDATA.GetString(594));
		}
		return "";
	}
	string GetInfo(RankType type)
	{
		switch (type)
		{
		case RankType.Power: return TDATA.GetString(593);
		case RankType.StageCnt: return TDATA.GetString(595);
		}
		return "";
	}

	string GetValueTitle(RankType type)
	{
		switch (type)
		{
		case RankType.Power: return TDATA.GetString(592);
		case RankType.StageCnt: return TDATA.GetString(596);
		}
		return "";
	}

	void SetUI(RankType page)
	{
		bool IsSamePage = m_Page == page;
		m_Page = page;
		RES_RANKING data = m_Data[page];

		int UserCnt = data.RankUsers.Count;
		// 유저 리스트
		UTILE.Load_Prefab_List(UserCnt, m_SUI.List, m_SUI.Prefab);
		for(int i = 0, rank = 0, cnt = 0; i < UserCnt; i++)
		{
			var item = m_SUI.List.GetChild(i).GetComponent<Item_PDA_Ranking_Element>();
			var info = data.RankUsers[i];
			bool firstdata = false;
			if(rank != info.Rank)
			{
				rank = info.Rank;
				cnt = data.RankUsers.FindAll(o => o.Rank == rank).Count;
			}
			item.SetData(info, cnt, firstdata);
		}
		// 내 랭킹
		m_SUI.MyInfo.SetData(data.MyInfo);

		m_SUI.Title.text = GetTitle(page);
		m_SUI.Info.text = GetInfo(page);
		m_SUI.ValueTitle.text = GetValueTitle(page);

		// 탭 선택
		if (!IsSamePage)
		{
			m_SUI.Ani.SetTrigger("SideMove");
			SetPage(page);
		}
	}

	void SetPage(RankType type)
	{
		for (int i = 0; i < m_SUI.PageAnis.Length; i++)
		{
			m_SUI.PageAnis[i].SetTrigger(i == (int)type ? "On" : "Off");
		}
	}

	bool IsConnect = false;
	public void ChangePage(int page)
	{
		RankType type = (RankType)page;
		if (IsConnect) return;
		if (m_Page == type) return;
		if(m_Data.ContainsKey(type))
		{
			SetUI(type);
			return;
		}
		IsConnect = true;
		WEB.SEND_REQ_RANKING((res) =>
		{
			if (res.IsSuccess())
			{
				m_Data.Add(type, res);
				SetUI(type);
			}
			IsConnect = false;
		}, type);
	}
}
