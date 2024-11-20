using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_PDA_Option_AccountData : Item_PDA_Base
{
	public enum SelectPos
	{
		Befor = 0,
		Now
	}
	[Serializable]
	public struct SInfoUI
	{
		public Image Profile;
		public Text Name;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Cash;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public SInfoUI[] Infos;
	}
	[SerializeField]
	SUI m_SUI;
	RES_ACC_INFO[] Infos;
	ACC_STATE LoginType;
	bool IsAction = false;

	private void OnEnable() {
		StartCoroutine(AnimEnd());
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		Infos = (RES_ACC_INFO[])args[0];
		LoginType = (ACC_STATE)args[1];

		//Profile.sprite = TDATA.GetUserProfileImage(user.Profile);
		base.SetData(CloaseCB, args);
		for(int i = 0; i < 2; i++)
		{
			RES_ACC_INFO acc = Infos[i];
			m_SUI.Infos[i].Profile.sprite = TDATA.GetUserProfileImage(acc.Profile);
			m_SUI.Infos[i].Name.text = acc.m_Name;
			m_SUI.Infos[i].LV.text = acc.LV.ToString();
			m_SUI.Infos[i].Cash.text = Utile_Class.CommaValue(acc.m_Cash);
		}
	}

	IEnumerator AnimEnd()
	{
		IsAction = true;
		m_SUI.Anim.SetTrigger("Start");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		IsAction = false;
	}

	public void OnSelect(int pos)
	{
		if (IsAction) return;
		PlayEffSound(SND_IDX.SFX_0121);
		// 상세보기창으로 연결
		SelectPos state = (SelectPos)pos;
		m_CloaseCB?.Invoke(Item_PDA_Option.State.Change_Acc, new object[] { state, Infos, LoginType });
	}
}
