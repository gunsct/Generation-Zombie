using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Eva_Tag : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	int m_Idx;
	Action<int> m_CB;

	TChar_HRTable m_TData { get { return TDATA.GetChar_HRTable(m_Idx); } }

	public void SetData(int _idx, Action<int> _cb) {
		m_Idx = _idx;
		m_CB = _cb;
		m_SUI.Name.text = m_TData.GetTagName();
	}

	public void ClickTag() {
		m_CB?.Invoke(m_Idx);
	}
}
