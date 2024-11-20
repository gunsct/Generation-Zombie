using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using System.Linq;

public class ZombieFarmAllGet : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI RemainTime;
		public Transform Bucket;
		public Transform Element;//Item_PDA_ZombieFarm_RoomReward_Element
	}
	[SerializeField] SUI m_SUI;

	DNAInfo m_Info;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();

		UTILE.Load_Prefab_List(USERINFO.m_ZombieRoom.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < USERINFO.m_ZombieRoom.Count; i++) {
			Item_PDA_ZombieFarm_RoomReward_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PDA_ZombieFarm_RoomReward_Element>();
			element.SetData(USERINFO.m_ZombieRoom[i]);
		}
	}
	public void ClickAllGet() {
		if (m_Action != null) return;
		List<int> roomposs = USERINFO.m_ZombieRoom.Select(o => o.Pos).ToList();
		WEB.SEND_REQ_ZOMBIE_PRODUCE((res) => {
			if (res.IsSuccess()) {
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					SetUI();
				});
			}
		}, roomposs);
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
