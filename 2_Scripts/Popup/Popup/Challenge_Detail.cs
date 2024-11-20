using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Challenge_Detail : PopupBase
{
	[Serializable]
	public struct SRankingUI
	{
		public ScrollRect Scroll;
		public RectTransform Item;
	}

	[Serializable]
	public struct SUI
	{
		public Image Blur;
		public Image[] Icon;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Info;
		public TextMeshProUGUI ETime;

		public Item_Challenge_Ranking My;
		public SRankingUI Ranking;

		// 다음 챌린지 버튼
		public GameObject NextBtn;
		public TextMeshProUGUI NextName;
		public Image NextIcon;


		public Animator Ani;

	}

	[SerializeField] SUI m_SUI;
	MyChallenge m_Info { get { return USERINFO.m_MyChallenge; } }
	ChallengeMode m_Mode;
	int m_Pos;
	IEnumerator m_Action;
	ChallengeInfo m_Now { get { return m_Mode == ChallengeMode.Week ? m_Info.Week[m_Pos] : m_Info.Now; } }
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		PlayEffSound(SND_IDX.SFX_1413);
		m_SUI.Blur.sprite = (Sprite)aobjValue[0];
		m_Mode = (ChallengeMode)aobjValue[1];
		m_Pos = (int)aobjValue[2];
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void Update()
	{
		m_SUI.ETime.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, m_Now.GetRemainTime());
	}

	public override void SetUI()
	{
		base.SetUI();
		SetList();
		RES_RANKING_INFO user = m_Now.MyInfo;
		m_SUI.My.SetData(user, null);

		Sprite icon = m_Now.GetImg();
		for (int i = m_SUI.Icon.Length - 1; i > -1; i--) m_SUI.Icon[i].sprite = icon;
		m_SUI.Title.text = m_Now.GetName();
		m_SUI.Info.text = m_Now.GetInfo();
		m_SUI.ETime.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.day_hr_min_sec, m_Now.GetRemainTime());

		if (m_Mode == ChallengeMode.Week || (m_Mode != ChallengeMode.Week && m_Info.Next == ChallengeType.END )) m_SUI.NextBtn.SetActive(false);
		else
		{
			m_SUI.NextBtn.SetActive(true);
			m_SUI.NextName.text = TDATA.GetChallengeName(m_Info.Next);
			m_SUI.NextIcon.sprite = UTILE.LoadImg(string.Format("BG/Challenge/Challenge_{0}", m_Info.Next.ToString()), "png");
		}
	}

	void SetList() {
		List<RES_RANKING_INFO> rankuser = m_Now.RankUsers;
		int Max = 0;
		if (rankuser == null) Max = 0;
		else {
			var list = m_Now.RankUsers.FindAll(o => o.Rank > 0);
			Max = Math.Min(100, list.Count);
		}

		RectTransform content = m_SUI.Ranking.Scroll.content;

		UTILE.Load_Prefab_List(Max, content, m_SUI.Ranking.Item);

		for (int i = 0; i < Max; i++)
		{
			Item_Challenge_Ranking item = content.GetChild(i).GetComponent<Item_Challenge_Ranking>();
			RES_RANKING_INFO user = m_Now.RankUsers[i];
			item.SetData(user, null);
		}
	}

	public void OnNextChallengeInfo() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_Next, null, m_Info.Next, m_Info.NextSTime);
	}

	public void OnViewRewards() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Challenge_RewardList, null, m_Mode, m_Now.CRewards);
	}
	public override void Close(int Result = 0)
	{
		if (m_Action != null) return;
		StartCoroutine(m_Action = IE_Close(Result));
	}

	public IEnumerator IE_Close(int Result)
	{
		m_SUI.Ani.SetTrigger("End");
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		base.Close(Result);
	}
}
