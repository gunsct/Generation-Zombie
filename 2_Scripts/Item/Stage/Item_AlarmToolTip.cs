using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_AlarmToolTip : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public RectTransform Rect;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(string _name, string _desc, Vector3 _pos) {
		m_SUI.Name.text = _name;
		m_SUI.Desc.text = _desc;
		transform.position = new Vector3(transform.position.x, _pos.y - 350f, 0f);
	}
	public void SetData(string _name, string _desc) {
		m_SUI.Name.text = _name;
		m_SUI.Desc.text = _desc;
	}
}
