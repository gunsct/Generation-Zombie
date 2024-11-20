using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class Event_10_Gift_RewardList : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_RewardList_Item MainReward;
		public Transform Element;
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => o.Type == FAEventType.Stage_Minigame);
		FAEventData_Stage_Minigame minigame = (FAEventData_Stage_Minigame)evt.RealData;
		List<int> idxs = minigame.Minigames;

		List<TMissionTable> tdatas = new List<TMissionTable>();
		TMissionTable cleardata = null;
		for (int i = 0; i < idxs.Count; i++) {
			TMissionTable data = TDATA.GetMissionTable(idxs[i]);
			if (data.m_Mode == MissionMode.Event_miniGame_Clear) cleardata = data;
			else tdatas.Add(data);
		}

		//주요 보상
		PostReward clearreward = cleardata.m_Rewards[0];
		List<RES_REWARD_BASE> clearrewards = MAIN.GetRewardData(clearreward.Kind, clearreward.Idx, clearreward.Cnt);
		m_SUI.MainReward.SetData(clearrewards[0], null, false);
		//이외 보상
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		for(int i = 0;i< tdatas.Count; i++) {
			PostReward data = tdatas[i].m_Rewards[0];
			List<RES_REWARD_BASE> reward = MAIN.GetRewardData(data.Kind, data.Idx, data.Cnt).FindAll(o=> rewards.Find(r=>r.GetIdx() == o.GetIdx()) == null);
			rewards.AddRange(reward);
		}
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < rewards.Count; i++) {
			m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>().SetData(rewards[i], null, false);
		}
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		base.Close(_result);
	}
}
