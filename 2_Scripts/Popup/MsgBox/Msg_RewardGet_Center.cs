using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Msg_RewardGet_Center : MsgBoxBase
{
	public enum Action
	{
		Get = 0,
		Start_Once
	}
	[System.Serializable]
	public struct SUI
	{
		public Item_RewardList_Item Item;
		public Animator Ani;
	}
	[SerializeField] SUI m_SUI;
	RES_CHALLENGE_MYRANKING Rankinfo;
	IEnumerator m_RankAni;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		List<RES_REWARD_BASE> rewards = (List<RES_REWARD_BASE>)aobjValue[0];
		m_SUI.Item.SetData(rewards[0]);
		if(aobjValue.Length > 1) m_SUI.Ani.SetTrigger(((Action)aobjValue[1]).ToString());
		base.SetData(pos, popup, cb, aobjValue);
	}
}
