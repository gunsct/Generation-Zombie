using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Stage_Skill_Judith : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Card;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		TStageCardTable data = (TStageCardTable)aobjValue[0];
		m_SUI.Card.sprite = data.GetImg();
		m_SUI.Name.text = data.GetName();
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
