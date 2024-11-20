using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using Coffee.UIEffects;

public class Challenge_EndReward : PopupBase
{
	[Serializable]
	public struct STopRankUI
	{
		[ReName("Icon_X", "FaceGroup", "Deco")]
		public GameObject[] Active;
		public UIEffect BGEff;
		public Image BG;

		public Image Profile;
		public Text Name;

	}

	[Serializable]
	public struct SUI
	{
		public Image Blur;
		public Image Icon;
		public TextMeshProUGUI Info;

		public STopRankUI[] TopRank;
		public Item_Challenge_Ranking MyRank;

		public Animator Ani;
	}

	[SerializeField] SUI m_SUI;
	[SerializeField, ReName("None", "1등", "2등~")]
	Color[] RankBGColors;

	ChallengeInfo m_Info;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_Info = (ChallengeInfo)aobjValue[0];
		m_SUI.Blur.sprite = (Sprite)aobjValue[1];
		m_Info.RankUsers.Sort((befor, after) =>
		{
			int brank = befor.Rank != 0 ? befor.Rank : 100000;
			int arank = after.Rank != 0 ? after.Rank : 100000;
			if (brank != arank) return brank.CompareTo(arank);
			return befor.UserNo.CompareTo(after.UserNo);
		});
		PlayEffSound(SND_IDX.SFX_0191);
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		base.SetUI();

		m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("BG/Challenge/Challenge_{0}", m_Info.Type.ToString()), "png");
		m_SUI.Info.text = m_Info.GetInfo();

		// top ranking
		for (int i = 0; i < 3; i++)
		{
			RES_RANKING_INFO rank = m_Info.RankUsers[i];
			if (rank.Point < 1)
			{
				m_SUI.TopRank[i].Active[0].SetActive(true);
				m_SUI.TopRank[i].Active[1].SetActive(false);
				m_SUI.TopRank[i].Active[1].SetActive(false);
				m_SUI.TopRank[i].BGEff.enabled = true;
				m_SUI.TopRank[i].BG.color = RankBGColors[0];
			}
			else
			{
				m_SUI.TopRank[i].Active[0].SetActive(false);
				m_SUI.TopRank[i].Active[1].SetActive(true);
				m_SUI.TopRank[i].Active[1].SetActive(true);
				m_SUI.TopRank[i].BGEff.enabled = false;
				m_SUI.TopRank[i].BG.color = i == 0 ? RankBGColors[1] : RankBGColors[2];

				m_SUI.TopRank[i].Profile.sprite = TDATA.GetUserProfileImage(rank.Profile);
				m_SUI.TopRank[i].Name.text = rank.m_Name;
			}
		}

		// 내 랭킹
		m_SUI.MyRank.SetData(m_Info.MyInfo, m_Info.GerRankReward(m_Info.MyInfo == null ? -1 : m_Info.MyInfo.Rank));
	}
	public void SetAniEventSND(int _sidx) {
		PlayFXSnd(_sidx);
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
