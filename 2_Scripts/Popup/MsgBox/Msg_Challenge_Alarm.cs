using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Msg_Challenge_Alarm : MsgBoxBase
{
	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Up;
		public TextMeshProUGUI Rank;
		public Animator RankUpAni;

		public Image[] Icon;

		public Text Name;
		public Image Profile;
	}
	[SerializeField] SUI m_SUI;
	RES_CHALLENGE_MYRANKING Rankinfo;
	IEnumerator m_RankAni;


	bool IsEnd = false;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		Rankinfo = (RES_CHALLENGE_MYRANKING)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		base.SetUI();
		int rank = Rankinfo.MyInfo.Rank + Rankinfo.RankGap;
		m_SUI.Title.text = Rankinfo.GetName();
		m_SUI.Up.text = Rankinfo.RankGap.ToString();
		m_SUI.Rank.text = Rankinfo.BeforRank == 0 ? TDATA.GetString(400) : BaseValue.GetRank(rank);

		Sprite icon = Rankinfo.GetImg();
		for (int i = m_SUI.Icon.Length - 1; i > -1; i--) m_SUI.Icon[i].sprite = icon;

		m_SUI.Name.text = USERINFO.m_Name;
		m_SUI.Profile.sprite = TDATA.GetUserProfileImage(USERINFO.m_Profile);
	}


	public override void Close(int Result = 0)
	{
		if (m_RankAni != null) return;
		base.Close(Result);
	}

	public void OnUpStart()
	{
		if (m_RankAni != null || IsEnd) return;
		m_RankAni = UpAni();
		StartCoroutine(m_RankAni);
	}
	
	IEnumerator UpAni()
	{
		// 랭킹 상승
		int rank = Rankinfo.MyInfo.Rank + Rankinfo.RankGap;
		// 최대 2초동안 얼마나 많은 스피드로 움직일지 정함
		m_SUI.RankUpAni.speed = 1f / 0.2f;
		int Gap = Mathf.Max(1, Rankinfo.RankGap / 10);
		while (rank != Rankinfo.MyInfo.Rank)
		{
			m_SUI.RankUpAni.SetTrigger("Change_1");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.RankUpAni));
			rank = Mathf.Max(rank - Gap, Rankinfo.MyInfo.Rank);
			m_SUI.Rank.text = BaseValue.GetRank(rank);
			m_SUI.RankUpAni.SetTrigger("Change_2");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.RankUpAni));
		}
		m_SUI.RankUpAni.SetTrigger("Stop");
		m_SUI.RankUpAni.speed = 1;
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.RankUpAni));
		yield return new WaitForSeconds(1f);
		m_RankAni = null;
		// 자동 닫힘
		OnNO();
	}

	public void OnClick()
	{
		if (IsEnd) return;
		IsEnd = true;
		if (m_RankAni != null)
		{
			StopCoroutine(m_RankAni);
			m_RankAni = null;
		}
		m_SUI.Rank.text = BaseValue.GetRank(Rankinfo.MyInfo.Rank);
		m_SUI.RankUpAni.SetTrigger("Stop");
		m_SUI.RankUpAni.speed = 1;
		IsSkip = true;
		// 자동 닫힘
		OnNO();
	}
}
