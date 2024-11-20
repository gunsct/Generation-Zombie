using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Tab : ObjMng
{
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
		public GameObject Alram;
		public SState OFF;
		public SState ON;
	}

	[SerializeField] SUI m_sUI;
	public int m_Pos;
	Func<Item_Tab, bool> m_SelectFN;
#pragma warning restore 0649

	public void SetData(int pos, string tabName, Func<Item_Tab, bool> SelectFN)
	{
		m_Pos = pos;
		m_SelectFN = SelectFN;
		m_sUI.OFF.Name.text = tabName;
		m_sUI.ON.Name.text = tabName;
	}

	public void SetActive(bool Active)
	{
		m_sUI.OFF.Active.SetActive(!Active);
		m_sUI.ON.Active.SetActive(Active);
	}

	public void SetAlram(bool Active)
	{
		if(m_sUI.Alram != null) m_sUI.Alram.SetActive(Active);
	}

	public void OnClick()
	{
		SetAlram(false);
		if (m_SelectFN != null) SetActive(m_SelectFN(this));
	}
}
