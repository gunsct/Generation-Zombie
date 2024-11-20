using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_OptionSlot : ObjMng
{
	public enum State
	{
		Have,
		NextUnlock,
		Lock
	}
	[System.Serializable]
	public struct SUI
	{
		public GameObject SlotObj;
		public GameObject ResetBtn;
		public TextMeshProUGUI StatDesc;
	}
	[SerializeField]
	SUI m_SUI;
	Action<int> m_CB;
	int m_Pos;
	int m_Grade;
	public State m_State;
	public void SetData(int _pos, int _grade, State _state, string _statdesc = "", Action<int> _cb = null) {
		m_CB = _cb;
		m_Pos = _pos;
		m_Grade = _grade;
		m_State = _state;
		m_SUI.SlotObj.SetActive(m_State == State.Have);

		if (m_State != State.Have) {
			m_SUI.StatDesc.text = string.Format("{0} {1}", TDATA.GetString(99 + m_Pos + 1), TDATA.GetString(104));
			m_SUI.ResetBtn.SetActive(false);
		}
		else {
			m_SUI.StatDesc.text = _statdesc;
			m_SUI.ResetBtn.SetActive(true);
		}
	}

	/// <summary> </summary>
	public void ClickResetStat() {
		int price = TDATA.GetShopTable(BaseValue.Get_Grade_Shop_Idx_EqOp(m_Grade, false)).GetPrice();
		if (USERINFO.m_Cash < price) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(75));
		}
		else {
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, TDATA.GetString(76), TDATA.GetString(77), (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						m_CB?.Invoke(m_Pos);
					}
					else {
						POPUP.StartLackPop(BaseValue.CASH_IDX);
					}
				}
			}, PriceType.Cash, BaseValue.CASH_IDX, price, false);
		}
	}
}
