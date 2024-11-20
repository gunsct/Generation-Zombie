using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;

public class Item_Pass_GetListElement : ObjMng
{
	[Serializable]
    public struct SUI
	{
		public Item_RewardList_Item Reward;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
	}
	[SerializeField] SUI m_SUI;
	public void SetData(PostReward data)
	{
		var res = data.Get_RES_REWARD_BASE();
		m_SUI.Reward.SetData(res);
		m_SUI.Name.text = res.GetName();
		m_SUI.Cnt.text = string.Format("x{0}", data.Cnt);
	}
	public void SetData(RES_REWARD_BASE data) {
		int cnt = 1;
		switch(data.Type) {
			case Res_RewardType.Cash:
			case Res_RewardType.Energy:
			case Res_RewardType.Exp:
			case Res_RewardType.Inven:
			case Res_RewardType.Money:
				cnt = ((RES_REWARD_MONEY)data).Add;
				break;
			case Res_RewardType.Item:
				cnt = ((RES_REWARD_ITEM)data).Cnt;
				break;
			default: 
				cnt = 1;
				break;
		}
		m_SUI.Reward.SetData(data);
		m_SUI.Name.text = data.GetName();
		m_SUI.Cnt.text = string.Format("x{0}", cnt);
	}
}
