using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_PickUp_ListElement : ObjMng
{
	public enum State
	{
		None,
		Hold
	}
    [Serializable]
    public struct SUI
	{
		public Item_CharacterCard Card;
		public GameObject LockObj;
		public TextMeshProUGUI LockTxt;
		public GameObject SelectObj;
	}
	[SerializeField] SUI m_SUI;

	public int m_Idx;
	bool IS_Select;
	Action<Item_PickUp_ListElement> m_CB;
	State m_State = State.None;
	public bool IS_Lock;
	public TCharacterTable m_TData { get { return TDATA.GetCharacterTable(m_Idx); } }

	public void SetData(int _idx, bool _select, Action<Item_PickUp_ListElement> _cb) {
		m_Idx = _idx;
		IS_Select = _select;
		m_CB = _cb;
		CharInfo info = USERINFO.GetChar(m_Idx);
		if (info == null) {
			m_SUI.Card.SetData(_idx);
			m_SUI.Card.SetGrade(TDATA.GetCharacterTable(m_Idx).m_Grade);
			m_SUI.Card.SetCharState(true);
			m_SUI.Card.SetLvOnOff(false);
		}
		else {
			m_SUI.Card.SetData(info); 
			m_SUI.Card.SetLvOnOff(true);
		}

		int pickstgidx = m_TData.m_SelectivePickupStage;
		int nowstgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;
		IS_Lock = pickstgidx > nowstgidx;
		m_SUI.LockTxt.text = string.Format(TDATA.GetString(7102), pickstgidx / 100, pickstgidx % 100);
		m_SUI.LockObj.SetActive(IS_Lock);
		m_SUI.SelectObj.SetActive(IS_Select);
	}

	public void Click_Card() {
		if (m_State != State.None) return;
		m_CB?.Invoke(this);
	}
	public void SwapSelect() {
		IS_Select = !IS_Select;
		m_SUI.SelectObj.SetActive(IS_Select);
	}
	public void OpenDetail() {
		if (m_State != State.None) return;
		m_State = State.Hold;
		USERINFO.ShowCharInfo(m_Idx, USERINFO.m_Chars, (result, obj) => {
			m_State = State.None;
			DLGTINFO.f_RFCharInfoCard?.Invoke();
		});
	}
}
