using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class DicItem_BeginnerQuest_Tab_Alram : SerializableDictionary<Item_BeginnerQuest_Tab.AlramMode, GameObject> { }
public class Item_BeginnerQuest_Tab : ObjMng
{
	public enum AlramMode
	{
		None = 0,
		Notice,
		Complete,
		Lock
	}

#pragma warning disable 0649

	[System.Serializable]
	struct SState
	{
		public GameObject Active;
		public TextMeshProUGUI Name;
	}

	[System.Serializable]
	struct SUI
	{
		public DicItem_BeginnerQuest_Tab_Alram Alram;
		public SState OFF;
		public SState ON;
	}

	[SerializeField] SUI m_sUI;
	public int m_Pos;
	Func<Item_BeginnerQuest_Tab, bool> m_SelectFN;
	AlramMode m_Mode;
#pragma warning restore 0649

	public void SetData(int pos, string tabName, Func<Item_BeginnerQuest_Tab, bool> SelectFN)
	{
		m_Pos = pos;
		m_SelectFN = SelectFN;
		m_sUI.OFF.Name.text = tabName;
		m_sUI.ON.Name.text = tabName;
		SetActive(false);
	}

	public void SetActive(bool Active)
	{
		m_sUI.OFF.Active.SetActive(!Active);
		m_sUI.ON.Active.SetActive(Active);
	}

	public void SetAlram(AlramMode alram)
	{
		m_Mode = alram;
		foreach (var obj in m_sUI.Alram)
		{
			if (obj.Value != null) obj.Value.SetActive(obj.Key == alram);
		}
	}

	public void OnClick()
	{
		if(m_SelectFN(this))
		{
			if(m_Mode != AlramMode.Lock)	SetAlram(AlramMode.None);
			SetActive(true);
		}
	}
}
