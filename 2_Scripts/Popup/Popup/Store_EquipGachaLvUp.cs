using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Store_EquipGachaLvUp : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public GameObject[] LvGroup;
		public Image[] Lv;
		public Sprite[] Nums;
		public TextMeshProUGUI BonusDesc;
	}
	[SerializeField] SUI m_SUI;

	IEnumerator m_Action; //end ani check
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		int lv = (int)aobjValue[0];
		if (lv < 100) {
			m_SUI.LvGroup[0].SetActive(true);
			m_SUI.LvGroup[1].SetActive(false);
			m_SUI.LvGroup[2].SetActive(true);
			m_SUI.Lv[0].sprite = m_SUI.Nums[lv / 10 >= 1 ? lv / 10 : 10];
			m_SUI.Lv[2].sprite = m_SUI.Nums[lv % 10];
		}
		else {
			m_SUI.LvGroup[0].SetActive(true);
			m_SUI.LvGroup[1].SetActive(true);
			m_SUI.LvGroup[2].SetActive(true);
			m_SUI.Lv[0].sprite = m_SUI.Nums[lv / 100];
			m_SUI.Lv[1].sprite = m_SUI.Nums[lv % 100 / 10];
			m_SUI.Lv[2].sprite = m_SUI.Nums[lv % 10];
		}
		var tdata = TDATA.GetEquipGachaTable(USERINFO.GetEquipGachaLv().GetLv);
		if (tdata.m_EffectType == EquipGachaEffectType.None) m_SUI.BonusDesc.text = TDATA.GetString(10890);
		else m_SUI.BonusDesc.text = string.Format("{0} : {1}", TDATA.GetString(10828), tdata.GetEffectDesc());
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
