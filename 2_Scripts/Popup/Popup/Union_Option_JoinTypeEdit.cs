using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Union_Option_JoinTypeEdit : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject[] SelectActive;

	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	GuildJoinType m_SelectType;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		Click_Mode((int)USERINFO.m_Guild.JoinType);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
	}

	public void Click_Mode(int pos)
	{
		m_SelectType = (GuildJoinType)pos;
		for (int i = 0; i < m_SUI.SelectActive.Length; i++) m_SUI.SelectActive[i].SetActive(i == pos);
	}

	public void Click_Apply() {
		if (m_Action != null) return;
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close((int)m_SelectType);
	}
}
