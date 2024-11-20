using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_UserProfile_Portrait : ObjMng
{
	public enum State
	{
		Not,
		Select,
		In,
		Out,
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Portrait;
	}
	[SerializeField] SUI m_SUI;
	public TUserProfileImageTable m_TData;
	Action<Item_UserProfile_Portrait> m_CB;
	State m_State;

	private void OnEnable() {
		SetAnim(m_State);
	}
	public void SetData(TUserProfileImageTable _tdata, Action<Item_UserProfile_Portrait> _cb) {
		m_TData = _tdata;
		m_CB = _cb;

		m_SUI.Portrait.sprite = m_TData.GetImage();
	}
	public void Select() {
		SetAnim(State.Select);
		m_CB?.Invoke(this);
	}
	public void SetAnim(State _state) {
		m_State = _state;
		switch (_state) {
			case State.Not:m_SUI.Anim.SetTrigger("Not");break;
			case State.Select: m_SUI.Anim.SetTrigger("Select"); break;
			case State.In: m_SUI.Anim.SetTrigger("In"); break;
			case State.Out: m_SUI.Anim.SetTrigger("Out"); break;
		}
	}
}
