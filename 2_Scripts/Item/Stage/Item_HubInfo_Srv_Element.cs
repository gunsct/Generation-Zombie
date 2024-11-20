using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Item_HubInfo_Srv_Element : ObjMng
{
	public enum State
	{
		NotGet,
		Get,
		Complete
	}
   [Serializable]
   public struct SUI
	{
		public Animator Anim;
		public Item_CharacterCard Card;
		public TextMeshProUGUI Title;
		public Image StatIcon;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	Action m_CB;
	public bool Is_Get;
	TCharacterTable m_TData;
	public bool SetData(TCharacterTable _tdata, StatType _type, Action _cb) {
		m_TData = _tdata;
		m_CB = _cb;
		CharInfo info = USERINFO.m_Chars.Find(o => o.m_Idx == m_TData.m_Idx);
		if (info == null) m_SUI.Anim.SetTrigger("NotGet");
		else if (_type == StatType.None) m_SUI.Anim.SetTrigger("Complete");
		else m_SUI.Anim.SetTrigger(info.m_LV == TDATA.GetExpTableCnt() ? "Complete" : "Get");

		if (info == null) m_SUI.Card.SetData(m_TData.m_Idx);
		else m_SUI.Card.SetData(info);

		for (int i = 1; i < 3; i++) {
			TSkillTable tdata = TDATA.GetSkill(m_TData.m_SkillIdx[i]);
			if (tdata == null) continue;
			if (tdata.GetStatType() == StatType.None && _type == StatType.None)
				m_SUI.Title.text = tdata.GetInfo();
			else if(tdata.GetStatType() != StatType.None && _type != StatType.None) {
				m_SUI.Title.text = string.Format("+{0}", info == null ? 0 : m_TData.GetPassiveStatValue(_type, info.m_LV));
				if(m_SUI.StatIcon != null)m_SUI.StatIcon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)tdata.GetStatType()), "png");
			}
		}
		return Is_Get = info != null;
	}

	public void Click_ViewInfo() {
		USERINFO.ShowCharInfo(m_TData.m_Idx, new List<CharInfo>(), (res, obj) => { m_CB?.Invoke(); }, false);
	}
}
