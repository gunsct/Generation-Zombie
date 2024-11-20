using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;

public class Item_Store_Box_Preview_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_RewardList_Item Item;
		public Item_RewardGroupIcon ItemGroup;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
	}
	[SerializeField] SUI m_SUI;
	
	public void SetData(RES_REWARD_BASE _reward, int[] _cnts) {
		m_SUI.ItemGroup.gameObject.SetActive(false);
		m_SUI.Item.SetData(_reward, null, false);
		m_SUI.Name.text = _reward.GetName();
		m_SUI.Cnt.text = string.Format("x{0}~{1}", _cnts[0], _cnts[1]);
	}
	public void SetData(ItemType _type, List<RES_REWARD_BASE> _rewards, int[] _cnts) {
		m_SUI.Item.gameObject.SetActive(false);
		m_SUI.ItemGroup.SetData(_type, _rewards);
		m_SUI.Name.text = BaseValue.GetGroupItemName(_type);
		m_SUI.Cnt.text = string.Format("x{0}~{1}", _cnts[0], _cnts[1]);
	}
}
