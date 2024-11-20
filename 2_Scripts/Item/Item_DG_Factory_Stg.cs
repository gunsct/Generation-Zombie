using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_DG_Factory_Stg : ObjMng
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
		m_SUI.Anim.SetTrigger(m_State.ToString());
	}

	public void Click() {
		m_CB?.Invoke(m_Lv);
	}
}
