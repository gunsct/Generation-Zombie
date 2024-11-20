using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_ItemSquare_SingleCheck : Item_RewardItem_Card
{
	public GameObject m_Check;
	public Action<Item_ItemSquare_SingleCheck> m_CB;
	public ItemInfo m_Info;

	private void Awake() {
		SetCheck(false);
	}

	public void SetData(ItemInfo _info, Action<Item_ItemSquare_SingleCheck> _cb = null) {
		base.SetData(_info.m_Idx, _info.m_Stack, _info.m_Lv, _info.m_Grade, LockMode: _info.m_Lock ? LockActiveMode.Normal : LockActiveMode.None);
		m_Info = _info;
		m_CB = _cb;
	}

	public void SetCheck(bool _on) {
		m_Check.SetActive(_on);
	}

	public override void ClickCard() {
		if (m_CB != null) m_CB.Invoke(this);
		else if (TDATA.GetItemTable(m_Idx) != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}
}
