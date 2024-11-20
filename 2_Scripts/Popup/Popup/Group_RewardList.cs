using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using System.Linq;

public class Group_RewardList : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Transform Prefab;
		public Transform GroupPrefab;
		public Transform Bucket;
		public TextMeshProUGUI Title;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		ItemType type = (ItemType)aobjValue[0];
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>((List<RES_REWARD_BASE>)aobjValue[1]);
		//타입별 타이틀
		m_SUI.Title.text = string.Format(TDATA.GetString(815), BaseValue.GetGroupItemName(type));
		//중복 제거
		rewards = USERINFO.GetExceptOverlap(rewards);
		//장비류 등급 분포 추리기
		Dictionary<ItemType, int[]> equips = new Dictionary<ItemType, int[]>();
		for (int i = rewards.Count - 1; i > -1; i--) {
			RES_REWARD_BASE reward = rewards[i];
			if (reward.Type != Res_RewardType.Item) continue;
			TItemTable tdata = TDATA.GetItemTable(reward.GetIdx());
			EquipType eqtype = tdata.GetEquipType();
			if (eqtype == EquipType.End) continue;
			ItemType itemtype = eqtype == EquipType.Weapon ? ItemType.Weapon : tdata.m_Type;

			if (!equips.ContainsKey(itemtype)) equips.Add(itemtype, new int[2] { BaseValue.MAX_ITEM_GRADE, 0 });
			if (equips[itemtype][0] > tdata.m_Grade) equips[itemtype][0] = tdata.m_Grade;
			if (equips[itemtype][1] < tdata.m_Grade) equips[itemtype][1] = tdata.m_Grade;
			rewards.Remove(reward);
		}
		//장비류와 그외 아이콘 출력
		for (int i = 0; i < equips.Count; i++) {
			Item_RewardList_Group itemgroup =  Utile_Class.Instantiate(m_SUI.GroupPrefab.gameObject, m_SUI.Bucket).GetComponent<Item_RewardList_Group>();
			itemgroup.SetData(Res_RewardType.Item, new object[] { equips.ElementAt(i).Key, equips.ElementAt(i).Value });
		}
		for (int i = 0; i < rewards.Count; i++) {
			Item_RewardList_Item reward =  Utile_Class.Instantiate(m_SUI.Prefab.gameObject, m_SUI.Bucket).GetComponent<Item_RewardList_Item>();
			reward.transform.localScale = Vector3.one * 0.8f;
			reward.SetData(rewards[i], null, false);
		}
	}

	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
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
