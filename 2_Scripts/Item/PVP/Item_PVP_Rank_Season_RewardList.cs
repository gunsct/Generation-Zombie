using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_PVP_Rank_Season_RewardList : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Ranking;
		public Transform Bucket;
		public Transform Element;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(int _idx) {
		TPVPSeasonRewardTable tdata = TDATA.GetPVPSeasonRewardTable(_idx);
		//10042 {0}위 10043 상위 {0}%
		string str = string.Empty;
		if(tdata.m_RankRewardType == PVPRewardRankType.RANK) {
			str = string.Format(TDATA.GetString(10042), tdata.m_MinRanking == tdata.m_MaxRanking ? tdata.m_MinRanking.ToString() : string.Format("{0}~{1}", tdata.m_MinRanking, tdata.m_MaxRanking));
		}
		else {
			str = string.Format(TDATA.GetString(10043), tdata.m_MaxRanking);
		}
		m_SUI.Ranking.text = str;

		List<RES_REWARD_BASE> rewards = tdata.GetRewards();
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i < rewards.Count; i++) {
			Item_RewardList_Item element = m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			element.transform.localScale = Vector3.one * 0.55f;
			element.SetData(rewards[i], null, false);
		}
	}
}
