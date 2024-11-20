using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_DG_Tower_Stg : ObjMng
{
	public enum State
	{
		Now,
		Cleared,
		Change,
		Not
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image[] LvImg;
		public Sprite[] LvSprites;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	public int m_Lv;

	public void SetData(int _lv) {
		m_Lv = _lv;
		int viewlv = m_Lv + 1;
		int[] num = new int[3];
		num[0] = viewlv / 100;
		num[1] = viewlv % 100 / 10;
		num[2] = viewlv % 10;

		m_SUI.LvImg[0].gameObject.SetActive(viewlv > 99);
		m_SUI.LvImg[1].gameObject.SetActive(viewlv > 9);

		for (int i = 0; i < 3; i++) {
			m_SUI.LvImg[i].sprite = m_SUI.LvSprites[num[i]];
		}
	}
	public void SetAnim(State _state) {
		m_State = _state;
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}
}
