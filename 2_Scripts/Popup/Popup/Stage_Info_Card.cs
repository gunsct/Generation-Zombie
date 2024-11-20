using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Stage_Info_Card : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI[] Descs;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		Item_Stage card = (Item_Stage)aobjValue[0];

		TStageCardTable cdata = card.m_Info.m_NowTData;
		TEnemyTable edata = card.m_Info.m_TEnemyData;
		m_SUI.Icon.sprite = card.m_Info.GetImg();
		m_SUI.Name.text = card.m_Info.GetName();
		List<string> descs = new List<string>();
		if (card.m_Info.IS_EnemyCard) {
			descs = card.m_Info.GetEnemyDesc();
		}
		else {
			descs.Add(card.m_Info.GetDesc());
		}
		for (int i = 0; i < m_SUI.Descs.Length; i++) {
			if (i < descs.Count) {
				m_SUI.Descs[i].text = descs[i];
			}
			else m_SUI.Descs[i].gameObject.SetActive(false);
		}
		
		base.SetData(pos, popup, cb, aobjValue);
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
