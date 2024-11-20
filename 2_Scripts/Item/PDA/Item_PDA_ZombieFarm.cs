using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Item_PDA_ZombieFarm : Item_PDA_Base
{
	public enum State
	{
		Main,
		CatchedList,
		ZombieInfo,
		RoomInfo,
		SetRoom,
		AllGetConfirm,
		End
	}
	
	[Serializable]
	public struct SUI
	{
		public GameObject[] MenuPrefabs;
	}

	[SerializeField] private SUI m_SUI;
	private State m_State;
	private Item_PDA_Base[] Items = new Item_PDA_Base[(int) State.End];
	
	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);

		StateChange(State.Main, args);
	}
	public override bool OnBack()
	{
		return Items[(int)m_State].OnBack();
	}

	private void StateChange(object state, object[] args)
	{
		m_State = (State) state;
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
					Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm_Main", true, transform).GetComponent<Item_PDA_ZombieFarm_Main>();
					break;
				case State.CatchedList:
					Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm_CatchedList", true, transform).GetComponent<Item_PDA_ZombieFarm_CatchedList>();
					break;
				case State.ZombieInfo:
					Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm_ZombieInfo", true, transform).GetComponent<Item_PDA_ZombieFarm_ZombieInfo>();
					break;
				case State.RoomInfo:
					Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm_RoomInfo", true, transform).GetComponent<Item_PDA_ZombieFarm_RoomInfo>();
					break;
				case State.SetRoom:
					Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm_SetRoom", true, transform).GetComponent<Item_PDA_ZombieFarm_SetRoom>();
					break;
				case State.AllGetConfirm:
					Items[(int)m_State] = UTILE.LoadPrefab("Item/MainMenu/PDA/Item_PDA_ZombieFarm_AllGetConfirm", true, transform).GetComponent<Item_PDA_ZombieFarm_AllGetConfirm>();
					break;
			}
		}


		for (int i = (int)State.End - 1; i > -1; i--)
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