using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_PDA_Adventure : Item_PDA_Base
{
	public enum State
	{
		Main,
		Detail,
		AutoDispatch,
		End
	}
	[Serializable]
	public struct SUI
	{
		public GameObject[] MenuPrefabs;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	Item_PDA_Base[] Items = new Item_PDA_Base[(int)State.End];

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		StateChange(State.Main, args);
	}
	public override bool OnBack()
	{
		return Items[(int)m_State].OnBack();
	}


	public void StateChange(object state, object[] args) {
		m_State = (State)state;
		if (m_State == State.End) {
			OnClose();
			return;
		}
		if (Items[(int)m_State] == null) {
			switch (m_State) {
				case State.Main:
					Items[(int)m_State] =  Utile_Class.Instantiate(m_SUI.MenuPrefabs[0], transform).GetComponent<Item_Adventure_Main>();
					break;
				case State.Detail:
					PlayEffSound(SND_IDX.SFX_0121);
					Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[1], transform).GetComponent<Item_Adventure_Detail>();
					break;
				case State.AutoDispatch:
					PlayEffSound(SND_IDX.SFX_0121);
					Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[2], transform).GetComponent<Item_Adventure_AutoDispatch>();
					break;
			}
		}

		for (int i = (int)State.End - 1; i > -1; i--) {
			if (Items[i] == null) continue;
			bool Active = i == (int)m_State;
			if (Items[i].gameObject.activeSelf != Active) Items[i].gameObject.SetActive(Active);
		}

		Items[(int)m_State].SetData(StateChange, args);
	}
}
