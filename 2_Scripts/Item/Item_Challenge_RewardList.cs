using System;
using TMPro;
using UnityEngine;

public class Item_Challenge_RewardList : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Rank;
		public TextMeshProUGUI Cnt;
	}

	[SerializeField] SUI m_SUI;
	public void SetData(ChallengeReward Reward) {

		if (Reward.rank[0] == Reward.rank[1]) m_SUI.Rank.text = string.Format(TDATA.GetString(603), Utile_Class.GetNationRankNum(APPINFO.m_Language, Reward.rank[1]));
		else if (Reward.rank[1] < 1) m_SUI.Rank.text = string.Format(TDATA.GetString(604), Utile_Class.GetNationRankNum(APPINFO.m_Language, Reward.rank[0]));
		else m_SUI.Rank.text = string.Format(TDATA.GetString(603), $"{Utile_Class.GetNationRankNum(APPINFO.m_Language, Reward.rank[0])}~{Utile_Class.GetNationRankNum(APPINFO.m_Language, Reward.rank[1])}");
		m_SUI.Cnt.text = $"x{Utile_Class.CommaValue(Reward.item.Cnt)}";
	}
}
