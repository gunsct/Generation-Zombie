using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Item_PVP_Rank_League_RewardList_Rank : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Ranking;
		public Transform Bucket;
		public Transform Element;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(TPVPLeagueRewardTable _tdata) {

		m_SUI.Ranking.text = string.Format(TDATA.GetString(10042), _tdata.m_MinRanking == _tdata.m_MaxRanking ? _tdata.m_MinRanking.ToString() : string.Format("{0}~{1}", _tdata.m_MinRanking, _tdata.m_MaxRanking));

		List<RES_REWARD_BASE> rewards = _tdata.GetRewards();
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i< rewards.Count; i++) {
			Item_RewardList_Item element = m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			element.transform.localScale = Vector3.one * 0.55f;
			element.SetData(rewards[i], null, false);
		}
	}
}
