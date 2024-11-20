using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_StgDebuffCardAlarmElement : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public TextMeshProUGUI[] Count;
		public GameObject CountGroup;
		public Item_AlarmToolTip ToolTip;
	}
	[SerializeField] SUI m_SUI;
	StageCardType m_Type;
	int[] m_Cnt = new int[2];

	private void Awake() {
		m_SUI.ToolTip.gameObject.SetActive(false);
	}
	public void SetData(StageCardType _type, float _count) {
		m_Type = _type;
		var info = BaseValue.GET_DEBUFFCARDINFO(m_Type);
		m_SUI.Icon.sprite = info.Icon;
		m_SUI.ToolTip.SetData(info.Name, info.Desc);
		m_Cnt[0] = m_Cnt[1] = Mathf.RoundToInt(_count);
		bool viewcount = false;
		switch (m_Type) {
			case StageCardType.ConActiveSkillLimit:
			case StageCardType.ConRandomChoice:
			case StageCardType.MergeDelete:
			case StageCardType.ConSkipTurn:
			case StageCardType.ConMergeSlotDown:
				viewcount = true;
				break;
		}
		m_SUI.CountGroup.gameObject.SetActive(viewcount);
		for(int i = 0; i < m_SUI.Count.Length; i++) {
			m_SUI.Count[i].text = m_Cnt[0].ToString();
		}
	}
	public void ViewInfo(bool _on) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageDebuffToolTip, (int)m_Type)) return;
		if (_on) {
			StartCoroutine(ViewInfoOnDelay());
		}
		else {
			m_SUI.ToolTip.gameObject.SetActive(false);
		}
	}
	IEnumerator ViewInfoOnDelay() {
		yield return new WaitForEndOfFrame();
		m_SUI.ToolTip.gameObject.SetActive(true);
	}
	public bool IS_OnAalarm() {
		return m_SUI.ToolTip.gameObject.activeSelf;
	}
	public void RefreshCount(int _cnt) {
		m_Cnt[0] -= _cnt;
		if (m_Cnt[0] < 1) {
			switch (m_Type) {
				case StageCardType.ConActiveSkillLimit:
				case StageCardType.ConMergeSlotDown:
					m_Cnt[0] = 0;
					break;
				default:
					m_Cnt[0] = m_Cnt[1];
					break;
			}
		}
		for (int i = 0; i < m_SUI.Count.Length; i++) {
			m_SUI.Count[i].text = m_Cnt[0].ToString();
		}
		if (m_Cnt[0] == 1) {
			switch (m_Type) {
				case StageCardType.ConActiveSkillLimit:
				case StageCardType.ConMergeSlotDown:
					break;
				default:
					m_SUI.Anim.SetTrigger("Point");
					break;
			}
		}
		else if (m_Cnt[0] > 1) {
			m_SUI.Anim.SetTrigger("CountChange");
		}
	}
}
