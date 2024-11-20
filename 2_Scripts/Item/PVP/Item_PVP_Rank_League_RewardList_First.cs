using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Item_PVP_Rank_League_RewardList_First : ObjMng
{
	public enum State
	{
		Idle,
		Get,
		Lock
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_PVP_Tier TierGroup;
		public Item_RewardList_Item[] Rewards;
	}
	[SerializeField] SUI m_SUI;
	int m_Idx;
	State m_GetType;
	Action<int> m_CB;

	public void SetData(TPVPRankRewardTable _tdata, State _get = State.Lock, Action<int> _cb = null) {
		m_GetType = _get;
		m_CB = _cb;
		m_Idx = _tdata.m_Idx;

		switch (m_GetType) {
			case State.Idle: m_SUI.Anim.SetTrigger("Highlight");break;
			case State.Get: m_SUI.Anim.SetTrigger("Complete");break;
			case State.Lock: m_SUI.Anim.SetTrigger("NotGet"); break;
		}

		TPvPRankTable rtdata = TDATA.GeTPvPRankTable(m_Idx);
		m_SUI.TierGroup.SetData(m_Idx, Item_PVP_Tier.Type.Tier);

		List<RES_REWARD_BASE> rewards = _tdata.GetReward();
		for(int i = 0;i< m_SUI.Rewards.Length; i++) {
			Item_RewardList_Item element = m_SUI.Rewards[i];
			if (i < rewards.Count) {
				element.gameObject.SetActive(true);
				element.transform.localScale = Vector3.one * 0.55f;
				element.SetData(rewards[i], null, false);
			}
			else element.gameObject.SetActive(false);
		}
	}
	public void ClickGet() {
		if (m_GetType != State.Idle) return;
		m_CB?.Invoke(m_Idx);
	}
}
