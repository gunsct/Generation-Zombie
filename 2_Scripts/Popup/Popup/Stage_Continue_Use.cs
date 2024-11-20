using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stage_Continue_Use : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
	}
	[SerializeField] SUI m_SUI;

	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		Close();
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Start");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
