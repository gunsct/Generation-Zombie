using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class ZombieDecomposition : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Transform Bucket;
		public Transform Element;
	}
	[SerializeField] SUI m_SUI;
	List<ZombieInfo> m_Infos = new List<ZombieInfo>();
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	public List<RES_REWARD_BASE> GetReward() { return m_Rewards; }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Infos = (List<ZombieInfo>)aobjValue[0];
		
		for(int i = 0; i < m_Infos.Count; i++) {
			var rewards = m_Infos[i].m_TData.m_RemoveRewards;
			for (int j = 0;j < rewards.Count; j++) {
				RES_REWARD_ITEM reward = (RES_REWARD_ITEM)m_Rewards.Find(o => o.GetIdx() == rewards[j].Idx);
				if (reward == null)
					m_Rewards.Add(new RES_REWARD_ITEM() { Idx = rewards[j].Idx, Cnt = rewards[j].Cnt });
				else reward.Cnt += rewards[j].Cnt;
			}
		}
		UTILE.Load_Prefab_List(m_Rewards.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < m_Rewards.Count; i++) {
			m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>().SetData(m_Rewards[i], null, false);
		}

		base.SetData(pos, popup, cb, aobjValue);
	}

	public void Click_Decomposition() {
		POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(980), TDATA.GetString(981), (result, obj) => {
			if (result == (int)EMsgBtn.BTN_YES) {
				WEB.SEND_REQ_ZOMBIE_DESTROY((res) => {
					if (!res.IsSuccess()) {
						switch (res.result_code) {
							default:
								WEB.StartErrorMsg(res.result_code);
								break;
						}
						return;
					}
					Close(1);
				}, m_Infos);
			}
		});
	}
}
