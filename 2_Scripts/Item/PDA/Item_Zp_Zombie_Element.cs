using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

public class Item_Zp_Zombie_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Portrait;
		public TextMeshProUGUI Grade;
		public GameObject GradeGroup;
	}
	[SerializeField] SUI m_SUI;
	ZombieInfo m_Info;
	int m_CagePos;
	Action<object, object[]> m_StateCB;
	public Image GetIcon { get { return m_SUI.Portrait; } }

	public void SetData(ZombieInfo _info, int _cagepos, Action<object, object[]> _statecb) {
		m_Info = _info;
		m_CagePos = _cagepos;
		m_StateCB = _statecb;

		m_SUI.GradeGroup.SetActive(m_Info != null);
		m_SUI.Portrait.gameObject.SetActive(m_Info != null);
		if (m_Info != null) {
			m_SUI.Portrait.sprite = m_Info.m_TData.GetItemBigImg();
			m_SUI.Portrait.transform.localScale = Vector3.one * (m_SUI.Portrait.sprite.name.Contains("84_Enemy") ? 0.63f : 1f);
			m_SUI.Grade.text = string.Format("<color=#9CCD9C>Z</color>{0}", UTILE.Get_RomaNum(m_Info.m_Grade));
		}
	}
	public void ClickViewInfo() {
		m_StateCB.Invoke(Item_PDA_ZombieFarm.State.ZombieInfo, new object[] { m_Info, m_CagePos });
	}
}
