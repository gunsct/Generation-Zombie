using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using Coffee.UIEffects;

public class Item_Challenge_EndReward_Week : ObjMng
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

		public Image Rank;
	}

	[Serializable]
	public struct SMyRankUI
	{
		public TextMeshProUGUI Rank;
		public GameObject RewardPanel;
		public Item_RewardItem_Card Reward;
	}

	[Serializable]
	public struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI[] Name;

		public STopRankUI[] TopRank;
		public SMyRankUI MyRank;
	}

	[SerializeField] SUI m_SUI;
	[SerializeField] Sprite[] m_RankImg;
	[SerializeField, ReName("None", "1등", "2등~")]
	Color[] RankBGColors;

	ChallengeInfo m_Info;

	public void SetData(ChallengeInfo info)
	{
		m_Info = info;

		SetUI();
	}

	public void SetUI()
	{
		m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("BG/Challenge/Challenge_{0}", m_Info.Type.ToString()), "png");
		for(int i = m_SUI.Name.Length - 1; i > -1; i--) m_SUI.Name[i].text = m_Info.GetName();

		// 1~3위까지만 뽑기
		//List<RES_RANKING_INFO> list = new List<RES_RANKING_INFO>();
		var list = m_Info.RankUsers.FindAll(o => o.Rank > 0 && o.Rank < 4);
		// 내가 포함되어있을수있을수 있음
		// 내정보가 해당 순위의 가장 처음으로 오도록 소팅
		if(list.Count > 0)
		{
			//list.AddRange(check.FindAll(o => o.Rank < m_Info.MyInfo.Rank);

			list.Sort((befor, after) => {
				if(befor.Rank == after.Rank)
				{
					if (befor.UserNo == USERINFO.m_UID) return -1;
					if (after.UserNo == USERINFO.m_UID) return 1;
					return 0;
				}
				return befor.Rank.CompareTo(after.Rank);
			});
			// 등록순 3명만 가져가기
			list = list.GetRange(0, Math.Min(3, m_Info.RankUsers.Count));
		}

		// top ranking
		RES_RANKING_INFO user;
		for (int i = 0, lastrank = 0; i < 3; i++)
		{
			if(i < list.Count)
			{
				user = list[i];
				lastrank = user.Rank;

				m_SUI.TopRank[i].Active[0].SetActive(false);
				m_SUI.TopRank[i].Active[1].SetActive(true);
				m_SUI.TopRank[i].Active[1].SetActive(true);
				m_SUI.TopRank[i].BGEff.enabled = false;
				m_SUI.TopRank[i].BG.color = user.Rank == 1 ? RankBGColors[1] : RankBGColors[2];
				m_SUI.TopRank[i].Rank.sprite = m_RankImg[user.Rank - 1];

				m_SUI.TopRank[i].Profile.sprite = TDATA.GetUserProfileImage(user.Profile);
				m_SUI.TopRank[i].Name.text = user.m_Name;
			}
			else
			{
				m_SUI.TopRank[i].Active[0].SetActive(true);
				m_SUI.TopRank[i].Active[1].SetActive(false);
				m_SUI.TopRank[i].Active[1].SetActive(false);
				m_SUI.TopRank[i].BGEff.enabled = true;
				m_SUI.TopRank[i].BG.color = RankBGColors[0];
				lastrank++;
				m_SUI.TopRank[i].Rank.sprite = m_RankImg[lastrank - 1];
			}
		}

		// 내 랭킹

		user = m_Info.MyInfo;
		ChallengeReward reward = m_Info.GerRankReward(m_Info.MyInfo == null ? 0 : m_Info.MyInfo.Rank);
		m_SUI.MyRank.Rank.text = BaseValue.GetRank(user.Rank);
		if (user.Rank == 0 || reward == null)
		{
			m_SUI.MyRank.RewardPanel.SetActive(false);
		}
		else
		{
			m_SUI.MyRank.RewardPanel.SetActive(true);

			m_SUI.MyRank.Reward.SetData(reward.item.Idx, reward.item.Cnt, reward.item.LV, reward.item.Grade);
		}
	}

	public void SetAniEventSND(int _sidx) {
		PlayFXSnd(_sidx);
	}
}
