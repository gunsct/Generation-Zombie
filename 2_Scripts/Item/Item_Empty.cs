using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Empty : ObjMng
{
	public enum State
	{
		CharSet,
		Inventory,
		Upgrade,
		DNA
	}
	public State m_State;
	public TextMeshProUGUI m_Txt;

	private void Awake() {
		switch (m_State) {
			case State.CharSet: m_Txt.text = TDATA.GetString(276);break;
			case State.Inventory: m_Txt.text = TDATA.GetString(277); break;
			case State.Upgrade: m_Txt.text = TDATA.GetString(312); break;
			case State.DNA: m_Txt.text = TDATA.GetString(1087); break;
		}
	}
}
