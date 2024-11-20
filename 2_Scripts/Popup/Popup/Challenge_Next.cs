using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Challenge_Next : PopupBase
{
	[Serializable]
	public struct SRankingUI
	{
		public ScrollRect Scroll;
		public GameObject Item;
	}

	[Serializable]
	public struct SUI
	{
		public Image[] Icon;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Info;
		public TextMeshProUGUI STime;
		public Animator Ani;
	}

	[SerializeField] SUI m_SUI;
	ChallengeType m_Next;
	long m_STime;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		PlayEffSound(SND_IDX.SFX_0192);
		m_Next = (ChallengeType)aobjValue[0];
		m_STime = (long)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);

	}
	public override void SetUI()
	{
		base.SetUI();
		Sprite icon = UTILE.LoadImg(string.Format("BG/Challenge/Challenge_{0}", m_Next.ToString()), "png");
		for (int i = m_SUI.Icon.Length - 1; i > -1; i--) m_SUI.Icon[i].sprite = icon;
		m_SUI.Title.text = TDATA.GetChallengeName(m_Next);
		m_SUI.Info.text = TDATA.GetChallengeInfo(m_Next);

		SetTimeUI();
	}

	void SetTimeUI()
	{
		m_SUI.STime.text = string.Format(TDATA.GetString(600),UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, Math.Max(0, m_STime - UTILE.Get_ServerTime_Milli()) * 0.001d));
	}

	private void Update()
	{
		SetTimeUI();
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
