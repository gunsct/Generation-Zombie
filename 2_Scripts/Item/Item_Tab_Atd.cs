using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Tab_Atd : ObjMng
{
#pragma warning disable 0649

	[System.Serializable]
	struct SUI
	{
		public GameObject Alram;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Shadow;
		public Animator Ani;
		public Button Btn;
	}

	[SerializeField] SUI m_sUI;
	Action<int> m_SelectCB;
	int m_Pos;
#pragma warning restore 0649

	public void SetData(string tabName, int pos, Action<int> SelectCB)
	{
		m_sUI.Title.text = m_sUI.Shadow.text = tabName;
		m_SelectCB = SelectCB;
		m_Pos = pos;
	}

	public void SetActive(bool Active, bool Alram)
	{
		Utile_Class.AniResetAllTriggers(m_sUI.Ani);
		if (Active)
		{
			m_sUI.Btn.interactable = false;
			m_sUI.Ani.SetTrigger("Act");
			m_sUI.Alram.SetActive(false);
		}
		else
		{
			m_sUI.Btn.interactable = true;
			m_sUI.Ani.SetTrigger("Deact");
			m_sUI.Alram.SetActive(Alram);
		}
	}

	public void OnClick()
	{
		m_SelectCB?.Invoke(m_Pos);
	}
}
