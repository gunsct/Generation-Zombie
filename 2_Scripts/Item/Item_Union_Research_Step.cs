using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Text;

public class Item_Union_Research_Step : ObjMng
{
	[Serializable] 
    public struct SUI
	{
		public Animator Ani;
		public ImgNumber Num;
	}

	[SerializeField] SUI m_SUI;
	public int m_Num;
	bool IsStart;
	public void Start()
	{
		IsStart = true;
	}
	public void SetData(int Number) {
		m_Num = Number;
		m_SUI.Num.SetValue(Number);
	}
	public void StartAni(int selectno)
	{
		if (!IsStart) return;
		Utile_Class.AniResetAllTriggers(m_SUI.Ani);
		m_SUI.Ani.SetTrigger(selectno == m_Num ? "On" : "Off");
	}
}
