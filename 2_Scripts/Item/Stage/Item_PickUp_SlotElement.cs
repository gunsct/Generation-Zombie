using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_PickUp_SlotElement : ObjMng
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
		public GameObject Empty;
	}
	[SerializeField] SUI m_SUI;

	int m_Pos;
	public int m_Idx;
	Action<Item_PickUp_SlotElement> m_CB;

	State m_State = State.None;
	public bool IS_Lock;

	public TCharacterTable m_TData { get { return TDATA.GetCharacterTable(m_Idx); } }
	public void SetData(int _idx) {
		SetData(m_Pos, _idx, m_CB);
	}
	public void SetData(int _pos, int _idx, Action<Item_PickUp_SlotElement> _cb) {
		if(_pos >= 0) m_Pos = _pos;
		m_Idx = _idx;
		m_CB = _cb;

		int openstgIdx = BaseValue.GetSelectivePickupOpenStage(m_Pos);
		int nowstgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;
		IS_Lock = openstgIdx > nowstgidx;
		m_SUI.LockTxt.text = string.Format(TDATA.GetString(7102), openstgIdx / 100, openstgIdx % 100);
		m_SUI.LockObj.SetActive(IS_Lock);
		m_SUI.Empty.SetActive(!IS_Lock);

		m_SUI.Card.gameObject.SetActive(m_Idx != 0);
		if (m_Idx != 0) {
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
		}
	}

	public void Click_Card() {
		if (m_State != State.None) return;
		if (m_Idx == 0) return;
		m_CB?.Invoke(this);
		m_Idx = 0; 
		m_SUI.Card.gameObject.SetActive(false);
	}
	public void OpenDetail() {
		if (m_State != State.None) return;
		if (m_Idx == 0) return;
		m_State = State.Hold;
		USERINFO.ShowCharInfo(m_Idx, USERINFO.m_Chars, (result, obj) => {
			m_State = State.None;
			DLGTINFO.f_RFCharInfoCard?.Invoke();
		});
	}
}
