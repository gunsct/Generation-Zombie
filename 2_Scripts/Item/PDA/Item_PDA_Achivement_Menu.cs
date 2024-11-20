using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;
using System.Linq;

public class Item_PDA_Achivement_Menu : Item_PDA_Base
{
	public enum Page
	{
		Archieve_Main,
		Collection_Main,
	}

	[Serializable]
	public struct SUI
	{
		[ReName("Archieve", "Collection")]
		public GameObject[] Alrams;
	}
	[SerializeField] SUI m_SUI;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		SetUI();
	}

	public void SetUI()
	{
		// 알람 셋팅
		m_SUI.Alrams[0].SetActive(USERINFO.m_Achieve.IsAlram());
		m_SUI.Alrams[1].SetActive(USERINFO.m_Collection.IsAlram());
	}


	public void BtnOnClick(int Pos)
	{
		Page type = (Page)Pos;
		object[] param = null;
		Item_PDA_Achieve.State NextPage;
		switch (type)
		{
		case Page.Archieve_Main:
			NextPage = Item_PDA_Achieve.State.Achieve_Main;
			break;
		case Page.Collection_Main:
			NextPage = Item_PDA_Achieve.State.Collection_Main;
			param = new object[] { CollectionType.Zombie };
			break;
		default: return;
		}
		PlayEffSound(SND_IDX.SFX_0121);
		m_CloaseCB?.Invoke(NextPage, param);
	}

	public void ClickExit()
	{
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();
	}
	public override void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Cloase, 1)) return;
		m_CloaseCB?.Invoke(Item_PDA_Achieve.State.End, null);
	}
}
