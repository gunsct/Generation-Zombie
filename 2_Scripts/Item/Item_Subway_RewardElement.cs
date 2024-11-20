using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class Item_Subway_RewardElement : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_RewardList_Item Reward;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(RES_REWARD_BASE data, Action<GameObject> selectcb = null, bool IsStartEff = true) {
		m_SUI.Reward.SetData(data, selectcb, IsStartEff);
	}
}
