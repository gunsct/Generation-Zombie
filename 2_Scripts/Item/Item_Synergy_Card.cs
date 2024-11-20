using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Synergy_Card : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Img;
		public TextMeshProUGUI Name;
	}
	[SerializeField]
	SUI m_SUI;
	Action<JobType> m_CB;
	JobType m_Type;
	public void SetData(JobType _type, Action<JobType> _cb = null) {
		m_Type = _type;
		TSynergyTable table = TDATA.GetSynergyTable(m_Type);
		if(table == null) gameObject.SetActive(false);
		else
		{
			if (m_SUI.Img != null) m_SUI.Img.sprite = table.GetIcon();
			if (m_SUI.Name != null) m_SUI.Name.text = table.GetName();
		}
		m_CB = _cb;
	}

	public void SetAnim(string _trig) {
		if(m_SUI.Anim != null) m_SUI.Anim.SetTrigger(_trig);
	}
	public void ClickCard() {
		m_CB?.Invoke(m_Type);
	}
}
