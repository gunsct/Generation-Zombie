using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_EtcGetGuide : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI MainName;
		public GameObject[] Btns;
	}
	[SerializeField] SUI m_SUI;
	int m_Idx;
	Action<ContentType> m_CB;
	TGetGuideTable m_TData { get { return TDATA.GetGetGuideTable(m_Idx); } }

	public void SetData(int _idx, Action<ContentType> _cb) {
		m_Idx = _idx;
		m_CB = _cb;

		m_SUI.MainName.text = m_TData.GetMainName();
	}

	public void ClickGoContent() {
		m_CB?.Invoke(m_TData.m_GoContent);
	}
}
