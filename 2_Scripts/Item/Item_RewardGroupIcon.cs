using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;

public class Item_RewardGroupIcon : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
	}
	[SerializeField] SUI m_SUI;
	ItemType m_Type;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();

	public void SetData(ItemType _type, List<RES_REWARD_BASE> _rewards) {
		m_Type = _type;
		m_SUI.Icon.sprite = BaseValue.GetGroupItemIcon(m_Type);
		m_Rewards = _rewards;
	}
	public void ClickView() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_ToolTip, 0)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Group_RewardList, null, m_Type, m_Rewards);
	}
}
