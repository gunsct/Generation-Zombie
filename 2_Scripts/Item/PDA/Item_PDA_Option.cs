using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_Option : Item_PDA_Base
{
	public enum State
	{
		/// <summary> 메인 화면 </summary>
		Main = 0,
		/// <summary> 계정 선택 </summary>
		Select_Acc,
		/// <summary> 계정 변경 </summary>
		Change_Acc,
		/// <summary>  </summary>
		End
	}
	State m_State;
	Item_PDA_Base[] Items = new Item_PDA_Base[(int)State.End];
	[SerializeField] GameObject[] MenuPrefabs;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		StateChange(State.Main, args);
	}
	public override bool OnBack()
	{
		return Items[(int)m_State].OnBack();
	}
	public void StateChange(object state, object[] args)
	{
		m_State = (State)state;
		if (m_State == State.End)
		{
			OnClose();
			return;
		}
		if (Items[(int)m_State] == null)
		{
			switch (m_State)
			{
			case State.Main:
				Items[(int)m_State] = Utile_Class.Instantiate(MenuPrefabs[0], transform).GetComponent<Item_PDA_Option_Main>();
				break;
			case State.Select_Acc:
				Items[(int)m_State] = Utile_Class.Instantiate(MenuPrefabs[1], transform).GetComponent<Item_PDA_Option_AccountData>();
				break;
			case State.Change_Acc:
				Items[(int)m_State] = Utile_Class.Instantiate(MenuPrefabs[2], transform).GetComponent<Item_PDA_Option_AccountDataConfirm>();
				break;
			default:
				return;
			}
		}

		for (int i = (int)State.End - 1; i > -1; i--)
		{
			if (Items[i] == null) continue;
			bool Active = i == (int)m_State;
			if (Items[i].gameObject.activeSelf != Active) Items[i].gameObject.SetActive(Active);
		}

		Items[(int)m_State].SetData(StateChange, args);
	}

	public Item_PDA_Base GetItem(State state)
	{
		return Items[(int)state];
	}
}
