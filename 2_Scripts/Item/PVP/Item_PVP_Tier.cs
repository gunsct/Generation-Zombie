using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_PVP_Tier : ObjMng
{
	public enum Type
	{
		Rank,
		Tier
	}
   [Serializable]
   public struct SUI
	{
		public Animator Anim;
		public Image[] Icon;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	public Type m_Type;
	int m_Idx;
	public void SetData(int _idx, Type _type) {
		m_Idx = _idx;
		m_Type = _type;
		TPvPRankTable tdata = TDATA.GeTPvPRankTable(m_Idx);
		m_SUI.Icon[0].sprite = m_SUI.Icon[1].sprite = m_Type == Type.Rank ? tdata.GetRankIcon() : tdata.GetTierIcon();
		if(m_Type == Type.Rank)
			m_SUI.Name.text = tdata.GetRankName();
		else
			m_SUI.Name.text = string.Format("{0} {1}", tdata.GetRankName(), tdata.GetTierName());
	}
	public void ActiveAnim(bool _on) {
		m_SUI.Anim.SetTrigger(_on ? "On" : "Off");
	}
}
