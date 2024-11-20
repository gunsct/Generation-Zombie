using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_Guide_GoalCard : ObjMng
{
	public enum State
	{
		Start,
		Normal,
		Change,
		Complete,
		Blind,
		BlindOff
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image[] Icon;
	}
	[SerializeField] SUI m_SUI;
	public int m_Pos;
	public StageClearType m_Type;
	public State m_State;
	List<string> m_Anim = new List<string>();
	private void OnEnable() {
		if (m_Anim.Count > 0) {
			for (int i = m_Anim.Count - 1; i > -1; i--) {
				m_SUI.Anim.SetTrigger(m_Anim[i]);
				m_Anim.RemoveAt(i);
			}
		}
	}
	public void SetData(TStageCondition<StageClearType> _condition, int _pos, bool _startanim = true) {
		m_Type = _condition.m_Type;
		m_Pos = _pos;
		m_SUI.Icon[0].sprite = m_SUI.Icon[1].sprite = _condition.GetIcon_Card();
		if (_startanim) SetAnim(State.Start, null);
	}

	public void SetAnim(State _state, Action<GameObject> _cb) {
		StartCoroutine(Anim(_state, _cb));
	}
	public void SetAnim(string _anim) {
		m_Anim.Add(_anim);
		m_SUI.Anim.SetTrigger(_anim);
	}
	IEnumerator Anim(State _state, Action<GameObject> _cb) {
		m_State = _state;
		m_Anim.Add(_state.ToString());
		m_SUI.Anim.SetTrigger(_state.ToString());
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		_cb?.Invoke(gameObject);
	}
}
