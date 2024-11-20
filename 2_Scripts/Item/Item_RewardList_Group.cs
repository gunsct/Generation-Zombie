using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class Item_RewardList_Group : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public GameObject[] Panels;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(Res_RewardType _type, object[] _vals) {
		for (int i = 0; i < m_SUI.Panels.Length; i++) m_SUI.Panels[i].SetActive(false);
		switch (_type) {
			case Res_RewardType.Item:// _vals 0:ItemType, 1: int[] grademinmax
				m_SUI.Panels[0].SetActive(true);
				SetItem((ItemType)_vals[0], _vals);
				break;
			case Res_RewardType.Zombie:
				m_SUI.Panels[1].SetActive(true);
				break;
		}
	}

	void SetItem(ItemType _type, object[] _vals) {
		int[] grades = (int[])_vals[1];
		int idx = _vals.Length > 2 ? (int)_vals[2] : 0;
		Item_ItemSquare_RewardGroup item = m_SUI.Panels[0].GetComponent<Item_ItemSquare_RewardGroup>();
		item.SetData(_type, grades, idx);
	}
}
