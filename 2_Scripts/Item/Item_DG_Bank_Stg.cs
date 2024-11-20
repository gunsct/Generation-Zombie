using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Text;

public class Item_DG_Bank_Stg : ObjMng
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
		public TextMeshProUGUI Num;
		public TextMeshProUGUI LockTxt;
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
		int nonzero = 2;
		StringBuilder zerotxt = new StringBuilder();
		while (_lv / 10 > 0) {
			_lv /= 10;
			nonzero--;
		}
		for (int i = 0; i < nonzero; i++) zerotxt.Append("0");
		m_SUI.Num.text = string.Format("<color=#4a4a4a>{0}</color>{1}", zerotxt.ToString(), m_Lv + 1);
		m_SUI.LockTxt.text = string.Format("{0}-{1}", _limitstg / 100, _limitstg % 100);
	}

	public void SetAnim(State _state) {
		m_State = _state;
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}

	public void Click() {
		m_CB?.Invoke(m_Lv);
	}
}
