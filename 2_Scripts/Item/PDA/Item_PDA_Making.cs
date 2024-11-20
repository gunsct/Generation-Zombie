using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_PDA_Making : Item_PDA_Base
{
	public enum State
	{
		Ready,
		Main,
		Detail,
		GradeInfo,
		CharEqInfo,
		End
	}
	[Serializable]
	public struct SUI
	{
		public GameObject[] MenuPrefabs;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	public State GetState { get { return m_State; } }
	Item_PDA_Base[] Items = new Item_PDA_Base[(int)State.End];
	public Item_PDA_Base GetMenu { get { return Items[(int)State.Main]; } }
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		if (TUTO.IsTuto(TutoKind.Making, (int)TutoType_Making.Select_Making)) TUTO.Next();
		StateChange(State.Ready, args);
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
			switch (m_State)
			{
			case State.Ready:
				Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[0], transform).GetComponent<Item_PDA_Making_Ready>();
				break;
			case State.Main:
				PlayEffSound(SND_IDX.SFX_0121);
				Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[1], transform).GetComponent<Item_PDA_Making_Main>();
				break;
			case State.Detail:
				PlayEffSound(SND_IDX.SFX_0121);
				Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[2], transform).GetComponent<Item_PDA_Making_Detail>();
				break;
			case State.GradeInfo:
				PlayEffSound(SND_IDX.SFX_0121);
				Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[3], transform).GetComponent<Item_PDA_Making_GradeInfo>();
				break;
			case State.CharEqInfo:
				PlayEffSound(SND_IDX.SFX_0121);
				Items[(int)m_State] = Utile_Class.Instantiate(m_SUI.MenuPrefabs[4], transform).GetComponent<Item_PDA_Making_CharEquipHelp>();
				break;
			}
		}

		for (int i = (int)State.End - 1; i > -1; i--) {
			if (Items[i] == null) continue;
			bool Active = i == (int)m_State;
			if (Items[i].gameObject.activeSelf != Active) Items[i].gameObject.SetActive(Active);
		}

		Items[(int)m_State].SetData(StateChange, args);
		PlayEffSound(SND_IDX.SFX_0121);
	}
}
