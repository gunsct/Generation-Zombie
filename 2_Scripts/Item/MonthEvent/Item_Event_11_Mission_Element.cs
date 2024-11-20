using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class Item_Event_11_Mission_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{

		public Animator Anim;
		public Item_RewardList_Item Reward;
	}
	[SerializeField] SUI m_SUI;
	MissionData m_Info;
	Action<MissionData> m_CB;

	public void SetData(MissionData _info, Action<MissionData> _cb) {
		m_Info = _info;
		m_CB = _cb;

		PostReward cleardata = m_Info.m_TData.m_Rewards[0];
		List<RES_REWARD_BASE> reward = MAIN.GetRewardData(cleardata.Kind, cleardata.Idx, cleardata.Cnt);
		m_SUI.Reward.SetData(reward[0], null, false);

		switch (m_Info.State[0]) {
			case RewardState.Idle:
				m_SUI.Anim.SetTrigger(m_Info.IS_Complete() ? "Active" : "Deactive");
				break;
			case RewardState.Get:
				m_SUI.Anim.SetTrigger("Complete");
				break;
			case RewardState.None:
				m_SUI.Anim.SetTrigger("Deactive");
				break;
		}
	}
	public void Click_GetReward() {
		m_CB?.Invoke(m_Info);
	}
}
