using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Item_PDA_ZombieFarm_AllGetConfirm : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public Transform Bucket;
		public Transform Element;
	}
	[SerializeField] SUI m_SUI;

	List<RES_REWARD_BASE> m_Reward = new List<RES_REWARD_BASE>();
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		m_Reward = (List<RES_REWARD_BASE>)args[0];
		SetUI();
	}
	void SetUI() {
		UTILE.Load_Prefab_List(m_Reward.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < m_Reward.Count; i++) {
			Item_RewardList_Item element = m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			element.SetData(m_Reward[i], null, false);
			element.transform.localScale = Vector3.one * 0.55f;
		}
	}
	public void ClickGet() {
#if NOT_USE_NET
# else
		WEB.SEND_REQ_ZOMBIE_PRODUCE((res) => {
			if (res.IsSuccess()) {
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					OnClose();
				});
			}
		}, USERINFO.m_ZombieRoom.Select(o => o.Pos).ToList());
#endif
	}
}
