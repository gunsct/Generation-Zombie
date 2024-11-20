using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Challenge_Main : PopupBase
{

	[Serializable]
	public struct SNextUI
	{
		public GameObject Active;

		public Image[] Icon;

		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI Info;
	}

	[Serializable]
	public struct SPlayUI
	{
		public GameObject Active;

		public Image[] Icon;

		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI Info;
		public TextMeshProUGUI Rank;
	}

	[Serializable]
	public struct SDayUI
	{
		public GameObject Active;
		public TextMeshProUGUI Time;
		public Image TimeIcon;
		public SPlayUI Now;
		public SNextUI Next;
	}

	[Serializable]
	public struct SWeekUI
	{
		public GameObject Active;
		public GameObject DataActive;
		public TextMeshProUGUI Time;
		public Image TimeIcon;
		public SPlayUI[] Datas;
		public SNextUI Next;
	}

	[Serializable]
	public struct SUI
	{
		public Image Blur;
		public Color[] TimeColor;

		public SDayUI Day;

		public SWeekUI Week;
	}

	[SerializeField] SUI m_SUI;
	MyChallenge m_Info { get { return USERINFO.m_MyChallenge; } }
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_SUI.Blur.sprite = (Sprite)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
		PlayEffSound(SND_IDX.SFX_0101);
	}

	public void Update()
	{
		SetTimeUI();
	}

	public override void SetUI()
	{
		base.SetUI();
		var servertime = UTILE.Get_ServerTime_Milli();
		// 일반 챌린지
		ChallengeInfo info = m_Info.Now;
		if (info == null || info.Times[1] <= servertime) // 챌린지가 종료됨
		{
			if(m_Info.NextSTime <= servertime)
			{
				// 챌린지가 준비 안됨
				m_SUI.Day.Active.SetActive(false);
				m_SUI.Day.Now.Active.SetActive(false);
				m_SUI.Day.Next.Active.SetActive(false);
			}
			else
			{
				m_SUI.Day.Active.SetActive(true);
				m_SUI.Day.Now.Active.SetActive(false);
				m_SUI.Day.Next.Active.SetActive(true);


				var icon = UTILE.LoadImg(string.Format("BG/Challenge/Challenge_{0}", m_Info.Next.ToString()), "png");
				for (int i = m_SUI.Day.Next.Icon.Length - 1; i > -1; i--) m_SUI.Day.Next.Icon[i].sprite = icon;
				for (int i = m_SUI.Day.Next.Name.Length - 1; i > -1; i--) m_SUI.Day.Next.Name[i].text = TDATA.GetChallengeName(m_Info.Next);
				m_SUI.Day.Next.Info.text = TDATA.GetChallengeInfo(m_Info.Next);
			}
		}
		else
		{
			m_SUI.Day.Active.SetActive(true);
			m_SUI.Day.Now.Active.SetActive(true);
			m_SUI.Day.Next.Active.SetActive(false);

			var icon = info.GetImg();
			for (int i = m_SUI.Day.Now.Icon.Length - 1; i > -1; i--) m_SUI.Day.Now.Icon[i].sprite = icon;
			for (int i = m_SUI.Day.Now.Name.Length - 1; i > -1; i--) m_SUI.Day.Now.Name[i].text = info.GetName();
			m_SUI.Day.Now.Info.text = info.GetInfo();
			m_SUI.Day.Now.Rank.text = BaseValue.GetRank(info.MyInfo == null || info.MyInfo.Rank == 0 || info.MyInfo.Point < 1 ? 0 : info.MyInfo.Rank);
		}

		// 주간 챌린지
		if (m_Info.Week == null || m_Info.Week.Count < 1)
		{
			m_SUI.Week.Active.SetActive(false);
			m_SUI.Week.DataActive.SetActive(false);
			m_SUI.Week.Next.Active.SetActive(false);
		}
		else if(m_Info.Week[0].Times[0] > servertime)
		{
			m_SUI.Week.Active.SetActive(true);
			m_SUI.Week.DataActive.SetActive(false);
			m_SUI.Week.Next.Active.SetActive(true);
		}
		else
		{
			m_SUI.Week.Active.SetActive(true);
			m_SUI.Week.DataActive.SetActive(true);
			m_SUI.Week.Next.Active.SetActive(false);
			for (int i = 0; i < m_SUI.Week.Datas.Length; i++)
			{
				if (m_Info.Week.Count <= i)
				{
					m_SUI.Week.Datas[i].Active.SetActive(false);
					continue;
				}
				m_SUI.Week.Datas[i].Active.SetActive(true);
				info = m_Info.Week[i];
				var icon = info.GetImg();
				for (int j = m_SUI.Week.Datas[i].Icon.Length - 1; j > -1; j--) m_SUI.Week.Datas[i].Icon[j].sprite = icon;
				for (int j = m_SUI.Week.Datas[i].Name.Length - 1; j > -1; j--) m_SUI.Week.Datas[i].Name[j].text = info.GetName();
				m_SUI.Week.Datas[i].Info.text = info.GetInfo();
				m_SUI.Week.Datas[i].Rank.text = BaseValue.GetRank(info.MyInfo == null || info.MyInfo.Rank == 0 || info.MyInfo.Point < 1 ? 0 : info.MyInfo.Rank);
			}
		}

		SetTimeUI();
	}

	void SetTimeUI()
	{
		var servertime = UTILE.Get_ServerTime_Milli();
		// 시간 정보 셋팅
		if (m_SUI.Day.Active.activeSelf)
		{
			if (m_SUI.Day.Now.Active.activeSelf)
			{
				m_SUI.Day.TimeIcon.color = m_SUI.TimeColor[0];
				m_SUI.Day.Time.color = m_SUI.TimeColor[0];
				m_SUI.Day.Time.text = string.Format(TDATA.GetString(597), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_Info.Now.GetRemainTime()));
			}
			else if (m_SUI.Day.Next.Active.activeSelf)
			{
				m_SUI.Day.TimeIcon.color = m_SUI.TimeColor[1];
				m_SUI.Day.Time.color = m_SUI.TimeColor[1];
				m_SUI.Day.Time.text = string.Format(TDATA.GetString(600), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, Math.Max(0, m_Info.NextSTime - servertime) * 0.001d));
			}
		}
		if (m_SUI.Week.Active.activeSelf)
		{
			if (m_Info.Week[0].Times[0] > servertime)
			{
				m_SUI.Week.TimeIcon.color = m_SUI.TimeColor[1];
				m_SUI.Week.Time.color = m_SUI.TimeColor[1];
				m_SUI.Week.Time.text = string.Format(TDATA.GetString(600), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, Math.Max(0, m_Info.Week[0].Times[0] - servertime) * 0.001d));
			}
			else
			{
				m_SUI.Week.TimeIcon.color = m_SUI.TimeColor[0];
				m_SUI.Week.Time.color = m_SUI.TimeColor[0];
				m_SUI.Week.Time.text = string.Format(TDATA.GetString(597), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_Info.Week[0].GetRemainTime()));
			}
		}
	}

	public void OnDetailInfo(int Pos)
	{
		ChallengeMode mode = ChallengeMode.Week;
		// 상세보기
		if (Pos < 0) mode = ChallengeMode.Normal;

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Detail, null, m_SUI.Blur.sprite, mode, Pos);
	}
}
