using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Guide_Common : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public GameObject[] PreBtns;
		public GameObject[] NextBtns;
		public GameObject CloseBtn;
		public GameObject[] Pages;
		public GameObject[] OnDots;
		public GameObject[] OffDots;
	}
	[SerializeField] SUI m_SUI;
	int[] m_Pagecnt = new int[2];//0:현재, 1:최대
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_Pagecnt[0] = 0;
		m_Pagecnt[1] = m_SUI.Pages.Length;
		Click_Pre();
	}

	public void Click_Next() {
		if (m_Action != null) return;
		m_Pagecnt[0]++;
		if (m_Pagecnt[0] >= m_Pagecnt[1] - 1) {
			m_Pagecnt[0] = m_Pagecnt[1] - 1;
		}
		else
			m_SUI.Anim.SetTrigger("Next");

		for (int i = 0; i < m_SUI.PreBtns.Length; i++) m_SUI.PreBtns[i].SetActive(m_Pagecnt[0] > 0);
		for (int i = 0; i < m_SUI.NextBtns.Length; i++) m_SUI.NextBtns[i].SetActive(m_Pagecnt[0] < m_Pagecnt[1] - 1);
		if (m_SUI.CloseBtn != null) m_SUI.CloseBtn.SetActive(m_Pagecnt[0] == m_Pagecnt[1] - 1);
		for (int i = 0; i < m_Pagecnt[1]; i++) {
			m_SUI.Pages[i].SetActive(i == m_Pagecnt[0]);
			if(m_SUI.OnDots.Length == m_Pagecnt[1] && m_SUI.OnDots[i] != null) m_SUI.OnDots[i].SetActive(i == m_Pagecnt[0]);
			if (m_SUI.OffDots.Length == m_Pagecnt[1] && m_SUI.OffDots[i] != null) m_SUI.OffDots[i].SetActive(i != m_Pagecnt[0]);
		}
	}

	public void Click_Pre() {
		if (m_Action != null) return;
		m_Pagecnt[0]--;
		if (m_Pagecnt[0] <= 0) {
			m_Pagecnt[0] = 0;
		}
		else
			m_SUI.Anim.SetTrigger("Prev");

		for (int i = 0; i < m_SUI.PreBtns.Length; i++) m_SUI.PreBtns[i].SetActive(m_Pagecnt[0] > 0);
		for (int i = 0; i < m_SUI.NextBtns.Length; i++) m_SUI.NextBtns[i].SetActive(m_Pagecnt[0] < m_Pagecnt[1] - 1);
		if(m_SUI.CloseBtn != null) m_SUI.CloseBtn.SetActive(m_Pagecnt[0] == m_Pagecnt[1] - 1);
		for (int i = 0; i < m_Pagecnt[1]; i++) {
			m_SUI.Pages[i].SetActive(i == m_Pagecnt[0]);
			if (m_SUI.OnDots.Length == m_Pagecnt[1] && m_SUI.OnDots[i] != null) m_SUI.OnDots[i].SetActive(i == m_Pagecnt[0]);
			if (m_SUI.OffDots.Length == m_Pagecnt[1] && m_SUI.OffDots[i] != null) m_SUI.OffDots[i].SetActive(i != m_Pagecnt[0]);
		}
	}

	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
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
