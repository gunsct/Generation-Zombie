using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public class PVP_End_League : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] Title;
		public Image TierIcon;
		public TextMeshProUGUI TierName;
		public TextMeshProUGUI ResDesc;
		public Transform RewardBucket;
		public Transform RewardElement;		//item_rewardlist_item
	}

	[SerializeField] SUI m_SUI;
	RES_PVP_REWARD m_RInfo;
	int m_RankIdx;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PlayEffSound(SND_IDX.SFX_1411);

		m_RInfo = (RES_PVP_REWARD)aobjValue[0];
		m_RankIdx = m_RInfo.MyInfo.Next_PVPRank;

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		TPvPRankTable ntdata = TDATA.GeTPvPRankTable(m_RankIdx);
		TPvPRankTable ptdata = TDATA.GeTPvPRankTable(m_RInfo.MyInfo.PVPRank);
		int stridx;//승16,강17,유18

		m_SUI.Title[0].text = m_SUI.Title[1].text = string.Format("{0} {1} {2}", ptdata.GetRankName(), ptdata.GetTierName(), TDATA.GetString(10022));
		m_SUI.TierIcon.sprite = ntdata.GetTierIcon();
		m_SUI.TierName.text = string.Format("{0} {1}", ntdata.GetRankName(), ntdata.GetTierName());
		//강등
		if (ptdata.m_SortingOrder > ntdata.m_SortingOrder) stridx = 10017;
		else if (ptdata.m_SortingOrder < ntdata.m_SortingOrder) {//승급
			if (ptdata.m_UpTierType == UpTierType.POINT) stridx = 10047;
			else stridx = 10016;
		}
		else {//유지
			if (ptdata.m_UpTierType == UpTierType.POINT) stridx = 10048;
			else stridx = 10018;
		}
		m_SUI.ResDesc.text = string.Format(TDATA.GetString(stridx), ptdata.m_UpTierType == UpTierType.POINT ? m_RInfo.MyInfo.Point : m_RInfo.MyInfo.Rank);

		List<PostInfo> pinfos = USERINFO.m_Posts.FindAll(o=> m_RInfo.InitPost.Find(u=>u.UID == o.UID) != null);
		List<RES_REWARD_BASE> rewards = pinfos.SelectMany(r=>r.GetRewards()).ToList();
		UTILE.Load_Prefab_List(rewards.Count, m_SUI.RewardBucket, m_SUI.RewardElement);
		for(int i = 0; i < rewards.Count; i++) {
			Item_RewardList_Item reward = m_SUI.RewardBucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			reward.transform.localScale = Vector3.one * 0.7f;
			reward.SetData(rewards[i], null, false);
		}

		base.SetUI();
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
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
