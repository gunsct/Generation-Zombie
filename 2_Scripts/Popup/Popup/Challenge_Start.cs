using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Challenge_Start : PopupBase
{
	[Serializable]
	public struct STimerUI
	{
		public GameObject Active;
		public Image Guage;
		public TextMeshProUGUI Label;
	}
	[Serializable]
	public struct STitleUI
	{
		public GameObject Active;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Info;
		public STimerUI Timer;
	}

	[Serializable]
	public struct SInfoUI
	{
		public GameObject Active;

		public Item_Challenge_Ranking[] Ranking;
		public Item_Challenge_Ranking My;
		public TextMeshProUGUI ETime;
	}

	[Serializable]
	public struct SUI
	{
		public Image Blur;
		public Image[] Icon;
		public STitleUI Title;
		public SInfoUI Info;

		public Animator Ani;
	}

	[SerializeField] SUI m_SUI;
	ChallengeInfo m_Info;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (ChallengeInfo)aobjValue[0];
		m_SUI.Blur.sprite = (Sprite)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);
		m_NextTimer = NextTimer();
		StartCoroutine(m_NextTimer);
		PlayEffSound(SND_IDX.SFX_0101);
	}

	public override void SetUI()
	{
		base.SetUI();
		SetTitleUI();
		SetChallengeUI();
	}


	void SetTitleUI()
	{
		Sprite icon = m_Info.GetImg();
		for (int i = m_SUI.Icon.Length - 1; i > -1; i--) m_SUI.Icon[i].sprite = icon;
		m_SUI.Title.Title.text = m_Info.GetName();
		m_SUI.Title.Info.text = m_Info.GetInfo();
	}

	void SetChallengeUI()
	{
		for (int i = 0; i < 3; i++)
		{
			int rank = i + 1;
			RES_RANKING_INFO info = m_Info.RankUsers.Find(o => o.Rank == rank);
			//m_SUI.Info.Ranking[i].SetData(info, m_Info.GerRankReward(rank), rank);
			m_SUI.Info.Ranking[i].SetData(info, null, rank);
		}

		m_SUI.Info.My.SetData(m_Info.MyInfo, m_Info.GerRankReward(m_Info.MyInfo == null ? -1 : m_Info.MyInfo.Rank));
	}

	void SetTimerUI(float time, float MaxTime)
	{
		int end = (int)(time + 0.9f);
		m_SUI.Title.Timer.Label.text = end.ToString();
		m_SUI.Title.Timer.Guage.fillAmount = time / MaxTime;
	}

	void SetRemindTimeUI()
	{
		m_SUI.Info.ETime.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_Info.GetRemainTime());
	}

	IEnumerator m_NextTimer;

	public void OnNext()
	{
		if(m_NextTimer != null)
		{
			StopCoroutine(m_NextTimer);
			m_NextTimer = null;
			SetRemindTimeUI();
			m_SUI.Info.Active.SetActive(true);
			m_SUI.Title.Active.SetActive(false);
			m_SUI.Title.Timer.Active.SetActive(false);
			return;
		}
		Close();
	}

	IEnumerator NextTimer()
	{
		m_SUI.Info.Active.SetActive(false);
		m_SUI.Title.Active.SetActive(true);
		m_SUI.Title.Timer.Active.SetActive(true);

		float MaxTime = 5f;
		float time = 5f;
		SetTimerUI(time, MaxTime);
		// 다음 시간 표기
		while (time > 0)
		{
			yield return new WaitForEndOfFrame();
			time -= Time.deltaTime;
			SetTimerUI(time, MaxTime);
		}
		yield return new WaitForEndOfFrame();
		SetRemindTimeUI();
		m_SUI.Info.Active.SetActive(true);
		m_SUI.Title.Active.SetActive(false);
		m_SUI.Title.Timer.Active.SetActive(false);
		m_NextTimer = null;
	}

	public void Update()
	{
		if (!m_SUI.Info.Active.activeSelf) return;

		SetRemindTimeUI();
	}


	public override void Close(int Result = 0)
	{
		StartCoroutine(IE_Close(Result));
	}

	public IEnumerator IE_Close(int Result)
	{
		m_SUI.Ani.SetTrigger("End");
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		base.Close(Result);
	}
}
