using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class PVP_End_Season : PopupBase
{
	enum State
	{
		Reward,
		New
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_PVP_End_Season_CharElement[] Rankers;
		public TextMeshProUGUI ResDesc;
		public Transform RewardBucket;
		public Transform RewardElement;     //item_rewardlist_item
		public Image TierIcon;
		public TextMeshProUGUI TierName;
	}
	[SerializeField] SUI m_SUI;
	RES_PVP_REWARD m_RInfo;
	State m_State;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PlayEffSound(SND_IDX.SFX_1411);

		m_RInfo = (RES_PVP_REWARD)aobjValue[0];
		m_RInfo.RankUsers.Sort((RES_RANKING_INFO _before, RES_RANKING_INFO _after) => {
			return _before.Rank.CompareTo(_after.Rank);
		});

		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		TPvPRankTable tdata = TDATA.GeTPvPRankTable(m_RInfo.RankIdx);
		m_SUI.TierIcon.sprite = tdata.GetTierIcon();
		m_SUI.TierName.text = string.Format("{0} {1}", tdata.GetRankName(), tdata.GetTierName());
		//랭커
		for (int i = 0; i < m_SUI.Rankers.Length; i++) {
			m_SUI.Rankers[i].SetData(i < m_RInfo.RankUsers.Count ? m_RInfo.RankUsers[i] : null, i + 1);
		}
		//보상
		m_SUI.ResDesc.text = m_RInfo.MyInfo.Rank > 0 ? string.Format(TDATA.GetString(10026), m_RInfo.MyInfo.Rank) : TDATA.GetString(1749);

		if (m_RInfo.MyInfo.Rank > 0) {
			List<PostInfo> pinfos = USERINFO.m_Posts.FindAll(o => m_RInfo.InitPost.Find(u => u.UID == o.UID) != null);
			List<RES_REWARD_BASE> rewards = pinfos.SelectMany(r => r.GetRewards()).ToList();
			UTILE.Load_Prefab_List(rewards.Count, m_SUI.RewardBucket, m_SUI.RewardElement);
			for (int i = 0; i < rewards.Count; i++) {
				Item_RewardList_Item reward = m_SUI.RewardBucket.GetChild(i).GetComponent<Item_RewardList_Item>();
				reward.transform.localScale = Vector3.one * 0.7f;
				reward.SetData(rewards[i], null, false);
			}
		}
		base.SetUI();
	}
	public void ClickNewSeason() {
		if (m_State == State.Reward) {
			m_State = State.New;
			m_SUI.Anim.SetTrigger("NewSeason");
		}
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		if(m_State == State.Reward) {
			ClickNewSeason();
			return;
		}
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
