using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;

public class Item_Union_MarkElement : ObjMng
{
    [Serializable] 
    public struct SUI
	{
		public Animator Anim;
		public Image Mark;
	}
	[SerializeField] SUI m_SUI;
	public int m_Idx;
	bool Is_Lock;
	Action<int> m_CB;
	
	public void SetData(int _idx, Action<int> _cb) {
		m_Idx = _idx;
		m_CB = _cb;
		var tdata = TDATA.GetItemTable(_idx);
		Is_Lock = tdata.m_Value != 0 && !USERINFO.m_Guild.Items.Exists(o => o.Idx == _idx);
		m_SUI.Mark.sprite = TDATA.GetGuideMark(_idx);
	}
	public void Click() {
		if (Is_Lock)
		{
			POPUP.ViewItemToolTip(new RES_REWARD_ITEM() { Idx = m_Idx }, (RectTransform)transform);
			return;
		}

		m_CB?.Invoke(m_Idx);
	}
	public void SetAnim(bool _on) {
		if (Is_Lock) m_SUI.Anim.SetTrigger("Lock");
		else m_SUI.Anim.SetTrigger(_on ? "On" : "Off");
	}
}
