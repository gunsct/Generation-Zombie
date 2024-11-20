using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_Store_Select_ListElement : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Item_CharacterCard m_Card;
		public GameObject m_GetMark;
	}
	[SerializeField] SUI m_SUI;
	public TGachaGroupTable m_TData;
	Action<Item_Store_Select_ListElement> m_CB;

	public void SetData(TGachaGroupTable _tdata, Action<Item_Store_Select_ListElement> _cb) {
		m_TData = _tdata;
		m_CB = _cb;

		int idx = m_TData.MRewardKind == RewardKind.Character ? m_TData.m_RewardIdx : TDATA.GetItemTable(m_TData.m_RewardIdx).m_Value;
		m_SUI.m_GetMark.SetActive(USERINFO.m_Chars.Find(o => o.m_Idx == idx) != null);
		CharInfo info = new CharInfo() { m_Idx = idx, m_Grade = TDATA.GetCharacterTable(idx).m_Grade };
		m_SUI.m_Card.SetData(info);
		m_SUI.m_Card.SetLvOnOff(false);
	}
	public void Click_Select() {
		m_CB?.Invoke(this);
	}
}
