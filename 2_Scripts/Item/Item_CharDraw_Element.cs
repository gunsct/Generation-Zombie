using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_CharDraw_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_GachaReward Char;
		public GameObject[] Star5Actives;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(CharInfo _char, int _grade = 0) {
		m_SUI.Char.SetData(_char, _grade);
		for(int i = 0; i < m_SUI.Star5Actives.Length; i++) {
			m_SUI.Star5Actives[i].SetActive(_grade >= 5);
		}
	}
	public void SetData(int _piece, int _cnt, int _chargrade = 1) {
		m_SUI.Char.SetData(_piece, _cnt, _chargrade);
		for (int i = 0; i < m_SUI.Star5Actives.Length; i++) {
			m_SUI.Star5Actives[i].SetActive(_chargrade >= 5);
		}
	}
	public void SetAnim() {
		m_SUI.Anim.SetTrigger("Start");
	}
}
