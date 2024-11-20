using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class Item_PDA_ZombieFarm_RoomReward_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Name;
		public Transform Bucket;
		public Transform Element;//Item_RewardList_Item
	}
	[SerializeField] SUI m_SUI;
	ZombieRoomInfo m_Info;
	public void SetData(ZombieRoomInfo _info) {
		m_Info = _info;

		//시간당 보상
		List<RES_REWARD_BASE> timerewards = m_Info.GetStackReward();
		UTILE.Load_Prefab_List(timerewards.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < timerewards.Count; i++) {
			Item_RewardList_Item element = m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			element.SetData(timerewards[i], null, false);
		}
	}
}
