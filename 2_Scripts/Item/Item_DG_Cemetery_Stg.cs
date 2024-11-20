using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_DG_Cemetery_Stg : ObjMng
{
	public enum State
	{
		Select,
		NotSelect,
		NotOpen,
		Lock
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] Num;
		public TextMeshProUGUI LockTxt;
		public GameObject LockObj;
		public Image Icon;
		public Sprite[] SelectLockIcons;
		public Sprite[] NotOpenIcons;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	int m_Lv;
	Action<int> m_CB;
	private void OnEnable() {
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}
	public void SetData(int _lv, int _limitstg, StageDifficultyType _diff, Action<int> _cb) {
		m_Lv = _lv;
		_lv++;
		m_CB = _cb;
		for(int i = 0;i< m_SUI.Num.Length;i++)
			m_SUI.Num[i].text = (m_Lv + 1).ToString();

		m_SUI.LockTxt.text = string.Format("{0}-{1}", _limitstg / 100, _limitstg % 100);
	}

	public void SetAnim(State _state) {
		m_State = _state;
		switch (_state) {
			case State.NotSelect:
			case State.Select:
				m_SUI.Icon.sprite = m_SUI.SelectLockIcons[m_Lv % 3];
				m_SUI.Icon.color = Utile_Class.GetCodeColor("#FFFFFF");
				string[] colors = new string[3] { "#D9D4C5", "#CACFB1", "#CFC3B1" };
				m_SUI.Num[1].color = Utile_Class.GetCodeColor(colors[m_Lv % 3]);
				m_SUI.LockObj.SetActive(false);
				break;
			case State.NotOpen:
				m_SUI.Icon.sprite = m_SUI.NotOpenIcons[m_Lv % 3];
				m_SUI.Icon.color = Utile_Class.GetCodeColor("#FFFFFF");
				m_SUI.Num[1].color = Utile_Class.GetCodeColor("#6A6A6A");
				m_SUI.LockObj.SetActive(false);
				break;
			case State.Lock:
				m_SUI.Icon.sprite = m_SUI.SelectLockIcons[m_Lv % 3];
				m_SUI.Icon.color = Utile_Class.GetCodeColor("#D91818");
				m_SUI.Num[1].color = Utile_Class.GetCodeColor("#741B1B");
				m_SUI.LockObj.SetActive(true);
				break;
		}
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}

	public void Click() {
		m_CB?.Invoke(m_Lv);
	}
}
