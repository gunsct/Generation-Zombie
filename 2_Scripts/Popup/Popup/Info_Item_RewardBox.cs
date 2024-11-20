using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public class Info_Item_RewardBox : Info_Item
{
    [Serializable]
    public struct BUI
	{
		public Image[] GradeBGs;
		public TextMeshProUGUI Desc;
		public GameObject RewardCard; //Item_University_PieceList_Element
		public Transform Bucket;
	}
	[SerializeField] BUI m_BUI;
	ItemInfo m_Info;
	List<RES_REWARD_BASE> m_Rewards;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (ItemInfo)aobjValue[0];
		m_Rewards = (List<RES_REWARD_BASE>)aobjValue[3];
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		m_BUI.Desc.text = m_Info.m_TData.GetInfo();
		SetRewardList();
		m_SUI.Item.SetCntActive(false);
	}

	void SetRewardList() {
		GachaGroup gachagroup = null;
		int probsum = 0;
		if (m_Info.m_TData.m_Type == ItemType.RandomBox || m_Info.m_TData.m_Type == ItemType.PickupRandomBox) {
			gachagroup = TDATA.GetGachaGroup(m_Info.m_TData.m_Value);
			probsum = gachagroup.m_TotalProb;
		}

		List<int> grades = new List<int>();
		//장비류 묶음
		Dictionary<ItemType, int[]> equips = new Dictionary<ItemType, int[]>();
		Dictionary<ItemType, Dictionary<int, int>> probs = new Dictionary<ItemType, Dictionary<int, int>>();
		
		for (int i = m_Rewards.Count - 1; i > -1; i--) {
			RES_REWARD_BASE reward = m_Rewards[i];
			if (reward.Type != Res_RewardType.Item) continue;
			TItemTable tdata = TDATA.GetItemTable(reward.GetIdx());
			EquipType eqtype = tdata.GetEquipType();
			if (eqtype == EquipType.End) continue;
			ItemType itemtype = tdata.GetGroupType();// eqtype == EquipType.Weapon ? ItemType.Weapon : tdata.m_Type;

			if (!equips.ContainsKey(itemtype)) equips.Add(itemtype, new int[2] { BaseValue.MAX_ITEM_GRADE, 0 });
			if (equips[itemtype][0] > tdata.m_Grade) equips[itemtype][0] = tdata.m_Grade;
			if (equips[itemtype][1] < tdata.m_Grade) equips[itemtype][1] = tdata.m_Grade;

			if (probsum > 0) {
				if (!probs.ContainsKey(itemtype)) probs.Add(itemtype, new Dictionary<int, int>());
				if (!probs[itemtype].ContainsKey(tdata.m_Grade)) probs[itemtype].Add(tdata.m_Grade, 0);
				if (probs[itemtype][tdata.m_Grade] == 0) { 
					var list = gachagroup.m_List.FindAll(o => TDATA.GetItemTable(o.m_RewardIdx).GetGroupType() == itemtype && o.m_RewardGrade == tdata.m_Grade);
					probs[itemtype][tdata.m_Grade] = list.Sum(o => o.m_Prob);
				}
			}

			m_Rewards.Remove(reward);
		}

		//장비류와 그외 아이콘 출력
		for (int i = 0; i < equips.Count; i++) {
			int[] grademinmax = equips.ElementAt(i).Value;
			for (int j = grademinmax[1] - grademinmax[0]; j > -1 ; j--) {
				Item_Info_Item_RewardBox_Element itemgroup = Utile_Class.Instantiate(m_BUI.RewardCard, m_BUI.Bucket).GetComponent<Item_Info_Item_RewardBox_Element>();
				string prob = probsum == 0 ? null : string.Format("{0:0.###}%", (float)probs[equips.ElementAt(i).Key][grademinmax[0] + j] / (float)probsum * 100f);
				itemgroup.SetData(equips.ElementAt(i).Key, new int[] { grademinmax[0] + j, grademinmax[0] + j }, prob);
				if (!grades.Contains(grademinmax[0] + j)) grades.Add(grademinmax[0] + j);
			}
		}
		for (int i = 0;i< m_Rewards.Count; i++) {
			Item_Info_Item_RewardBox_Element reward = Utile_Class.Instantiate(m_BUI.RewardCard, m_BUI.Bucket).GetComponent<Item_Info_Item_RewardBox_Element>();
			string prob = probsum == 0 ? null : string.Format("{0:0.###}%", (float)(gachagroup.m_List.FindAll(o => o.m_RewardIdx == m_Rewards[i].GetIdx()).Sum(o=>o.m_Prob) / probsum) * 100f);
			reward.SetData(m_Rewards[i], null, false, prob);
			if (!grades.Contains(reward.GetGrade)) grades.Add(reward.GetGrade);
		}
		//UTILE.Load_Prefab_List(m_Rewards.Count, m_BUI.Bucket, m_BUI.RewardCard.transform);
		//for (int i = 0;i< m_BUI.Bucket.childCount; i++) {
		//	Item_Info_Item_RewardBox_Element reward = m_BUI.Bucket.GetChild(i).GetComponent<Item_Info_Item_RewardBox_Element>();
		//	reward.SetData(m_Rewards[i], null, false);
		//	if (!grades.Contains(reward.GetGrade)) grades.Add(reward.GetGrade);
		//}
		grades.Sort();
		m_BUI.GradeBGs[0].sprite = BaseValue.GetInfoGradeBG(grades[0]);
		m_BUI.GradeBGs[1].sprite = BaseValue.GetInfoGradeBG(grades[grades.Count - 1]);
	}
}
