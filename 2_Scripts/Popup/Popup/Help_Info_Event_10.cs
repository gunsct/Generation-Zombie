using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static Utile_Class;
public class Help_Info_Event_10 : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
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

		base.Close(_result);
	}
}
