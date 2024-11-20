using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PVP_Rank_Season_RewardList : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Transform Bucket;
		public Transform Element;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PlayEffSound(SND_IDX.SFX_1413);

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		List<TPVPSeasonRewardTable> tdatas = TDATA.GetAllPVPSeasonRewardTable();
		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < tdatas.Count; i++) {
			Item_PVP_Rank_Season_RewardList element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PVP_Rank_Season_RewardList>();
			element.SetData(tdatas[i].m_Idx);
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
