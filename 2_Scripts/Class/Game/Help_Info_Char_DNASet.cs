using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Help_Info_Char_DNASet : PopupBase
{
    [Serializable]
    public struct SUI
    {
		public Animator Anim;
		public Item_Help_Info_Char_DNASet_Element[] SetStats;
	}
    [SerializeField] SUI m_SUI;
    IEnumerator m_Action;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		for (DNABGType i = DNABGType.Red; i < DNABGType.End; i++) {
			List<TDNASetEffectTable> tdatas = TDATA.GetDNASetFXTables(i);
			m_SUI.SetStats[(int)i - 1].SetData(i, tdatas);
		}
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
