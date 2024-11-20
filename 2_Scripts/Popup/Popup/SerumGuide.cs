using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SerumGuide : PopupBase
{
	//[Serializable]
	//public struct SUI {
	//	public GameObject[] Panels;
	//	public GameObject[] Btns;       //0:이전,1:현재
	//}
	//[SerializeField] SUI m_SUI;
	//int[] m_Page = new int[2] { 0, 0 };	//0:이전,1:현재

	//private void Awake() {
	//	for(int i = 0; i < m_SUI.Panels.Length; i++) {
	//		m_SUI.Panels[i].SetActive(false);
	//	}
	//}
	//public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
	//	base.SetData(pos, popup, cb, aobjValue);
	//	m_SUI.Panels[m_Page[1]].SetActive(true);
	//	SetBtn();
	//}
	//void SetBtn() {
	//	m_SUI.Btns[0].SetActive(m_Page[1] != 0);
	//	m_SUI.Btns[1].SetActive(m_Page[1] != m_SUI.Panels.Length - 1);
	//}
	//public void Click_Page(int _next) {
	//	m_SUI.Panels[m_Page[1]].SetActive(false);
	//	m_Page[0] = m_Page[1];
	//	m_Page[1] += _next;
	//	if (m_Page[1] == m_SUI.Panels.Length) m_Page[1] = 0;
	//	else if (m_Page[1] < 0) m_Page[1] = m_SUI.Panels.Length - 1;
	//	m_SUI.Panels[m_Page[1]].SetActive(true);
	//	SetBtn();
	//}
}
