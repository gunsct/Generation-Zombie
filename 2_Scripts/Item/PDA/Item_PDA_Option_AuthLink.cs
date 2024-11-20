using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PDA_Option_AuthLink : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
		public Sprite[] Sprites;
		public TextMeshProUGUI Title;
		public GameObject[] Checks;//0:연동완료, 1:연동하기
	}
	[SerializeField] SUI m_SUI;
	public ACC_STATE m_State;

	void Start(){
		m_SUI.Icon.sprite = m_SUI.Sprites[(int)m_State - 1];
		m_SUI.Title.text = ACC.AccName(m_State);
		m_SUI.Checks[0].SetActive(m_State == ACC.LoginType);
		m_SUI.Checks[1].SetActive(m_State != ACC.LoginType);
	}
}
