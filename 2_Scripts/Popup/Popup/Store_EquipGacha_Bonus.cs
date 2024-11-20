using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Store_EquipGacha_Bonus : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject[] LvGroup;
		public Image[] Lv;
		public Sprite[] Nums;
		public TextMeshProUGUI[] BonusDesc;
		public Color[] DescColor;
		public GameObject NextGroup;
		public TextMeshProUGUI NextLv;
		public TextMeshProUGUI NextEff;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		int lv = USERINFO.GetEquipGachaLv().GetLv;
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
		for (EquipType i = EquipType.Weapon; i < EquipType.Max; i++) {
			float val = USERINFO.GetEquipGachaLvBonus(i); int sidx = 0;
			switch (i) {
				case EquipType.Weapon: sidx = 25; break;
				case EquipType.Helmet: sidx = 26; break;
				case EquipType.Costume: sidx = 27; break;
				case EquipType.Shoes: sidx = 28; break;
				case EquipType.Accessory: sidx = 29; break;
			}
			m_SUI.BonusDesc[(int)i].text = string.Format("{0} {1}", TDATA.GetString(sidx), string.Format(TDATA.GetString(10827), string.Format("{0:0.##}", val * 100)));
			m_SUI.BonusDesc[(int)i].color = m_SUI.DescColor[val > 0 ? 0 : 1];
		}
		if(TDATA.GetEquipGachaTableList().Count == lv) {
			m_SUI.NextGroup.SetActive(false);
		}
		else {
			m_SUI.NextGroup.SetActive(true);
			m_SUI.NextLv.text = string.Format(TDATA.GetString(10866), lv + 1);
			TEquipGachaTable tdata = TDATA.GetEquipGachaTable(lv + 1);
			if (tdata.m_EffectType == EquipGachaEffectType.None) m_SUI.NextEff.text = TDATA.GetString(10890);
			else m_SUI.NextEff.text = string.Format("{0} : {1}", TDATA.GetString(10828), tdata.GetEffectDesc());
		}
		base.SetUI();
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
