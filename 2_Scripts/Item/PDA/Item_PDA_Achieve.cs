using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_PDA_Achieve : Item_PDA_Base
{
	public enum State
	{
		Menu = 0,
		/// <summary> 업적 </summary>
		// 업적
		Achieve_Main,
		/// <summary> 컬렉션 </summary>
		Collection_Main,
		/// <summary> 컬렉션 버프 정보 보기 </summary>
		Collection_BuffList,
		End
	}

	State m_State;
	Item_PDA_Base[] Items = new Item_PDA_Base[(int)State.End];
	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);
		StateChange(State.Menu, null);
	}

	public override bool OnBack()
	{
		return Items[(int)m_State].OnBack();
	}

	public void StateChange(object state, object[] args)
	{
		m_State = (State)state;
		if (m_State == State.End) {
			OnClose();
			return;
		}
		if(Items[(int)m_State] == null) {
			switch (m_State)
			{
			case State.Menu:
				Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Achivement_Menu", true, transform).GetComponent<Item_PDA_Achivement_Menu>();
				break;
			case State.Achieve_Main:
				Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Achieve_Main", true, transform).GetComponent<Item_PDA_Achieve_Main>();
				break;
			case State.Collection_Main:
				Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Collection_Main", true, transform).GetComponent<Item_PDA_Collection_Main>();
				break;
			case State.Collection_BuffList:
				Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Collection_BuffList", true, transform).GetComponent<Item_PDA_Collection_BuffList>();
				break;
			//case State.Detail:
			//	Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Research_Detail", true, transform).GetComponent<Item_PDA_Research_Detail>();
			//	break;
			//case State.Result:
			//	Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_Research_Result", true, transform).GetComponent<Item_PDA_Research_Result>();
			//	break;
			default:
				return;
			}
		}

		for(int i = (int)State.End - 1; i > -1; i--)
		{
			if (Items[i] == null) continue;
			bool Active = i == (int)m_State;
			if (Items[i].gameObject.activeSelf != Active) Items[i].gameObject.SetActive(Active);
		}

		Items[(int)m_State].SetData(StateChange, args);
		PlayEffSound(SND_IDX.SFX_0121);
	}

	public GameObject GetItem(State state)
	{
		return Items[(int)state].gameObject;
	}
}
