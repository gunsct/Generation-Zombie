using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_Challenge_Ranking : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image Profile;
		public Image Nation;
		public TextMeshProUGUI Rank;
		public Text Name;
		public TextMeshProUGUI Point;
		public TextMeshProUGUI LV;


		public GameObject RewardActive;
		public Item_RewardItem_Card Reward;
	}


	[SerializeField] SUI m_SUI;
	string[] silhouettename = { "Card/Char/9996_Char", "Card/Char/9997_Char", "Card/Char/9998_Char", "Card/Char/9993_Char", "Card/Char/9994_Char", "Card/Char/9995_Char" };
	public void SetData(RES_RANKING_INFO user, ChallengeReward Reward = null, int rank = 1) {
		if(user == null)
		{
			// 랭킹 데이터 없을경우
			m_SUI.Profile.sprite = UTILE.LoadImg(silhouettename[(rank - 1) % 3], "png");
			m_SUI.Rank.text = BaseValue.GetRank(0);
			m_SUI.Name.text = TDATA.GetString(400);
			m_SUI.Nation.gameObject.SetActive(false);
			m_SUI.Point.text = "-";
			m_SUI.LV.transform.parent.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Profile.sprite = TDATA.GetUserProfileImage(user.Profile);
			m_SUI.Rank.text = BaseValue.GetRank(user.Rank);
			m_SUI.Name.text = user.m_Name;
			m_SUI.Nation.gameObject.SetActive(true);
			m_SUI.Nation.sprite = BaseValue.GetNationIcon(user.Nation);
			m_SUI.Point.text = user.Point.ToString();
			m_SUI.LV.transform.parent.gameObject.SetActive(true);
			m_SUI.LV.text = user.LV.ToString();
		}

		if (user?.Rank == 0 || Reward == null)
		{
			m_SUI.RewardActive?.SetActive(false);
		}
		else
		{
			m_SUI.RewardActive?.SetActive(true);
			m_SUI.Reward.SetData(Reward.item.Idx, Reward.item.Cnt, Reward.item.LV, Reward.item.Grade);
		}
	}
}
