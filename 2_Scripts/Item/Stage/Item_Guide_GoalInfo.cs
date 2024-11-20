using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Guide_GoalInfo : ObjMng
{
	public enum State
	{
		Start,
		Normal,
		Change,
		Complete
	}

	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI[] Count;
	}
	[SerializeField] SUI m_SUI;
	public int m_Pos;
	public StageClearType m_Type;
	int m_MaxCnt;
	public TGuideTable m_TData { get { return TDATA.GetGuideTable(m_Type); } }

	List<string> m_Anim = new List<string>();
	private void OnEnable() {
		if (m_Anim.Count > 0) {
			for (int i = m_Anim.Count - 1; i > -1; i--) {
				m_SUI.Anim.SetTrigger(m_Anim[i]);
				m_Anim.RemoveAt(i);
			}
		}
	}
	public void SetData(StageClearType _type, int _pos, bool _view = true, int _maxcnt = 0, bool _startanim = true) {
		m_Type = _type;
		m_Pos = _pos;
		m_MaxCnt = _maxcnt;
		SetUI(_view);
		if(_startanim) SetAnim(State.Start, null);
	}
	public void Refresh() {
		SetUI();
		SetAnim(State.Change, null);
	}
	void SetUI(bool _view = true) {
		if (_view) {
			switch (m_Type) {
				case StageClearType.KillEnemy_Turn:
					m_SUI.Desc.text = string.Format(STAGEINFO.m_TStage.GetClearDesc(m_Pos), STAGEINFO.m_Check.GetClearValue(m_Pos));
					break;
				default:
					m_SUI.Desc.text = STAGEINFO.m_TStage.GetClearDesc(m_Pos);
					break;
			}
		}
		else {
			m_SUI.Desc.text = m_MaxCnt > 2 ? string.Format("{0} {1}", TDATA.GetString(1061), m_Pos) : TDATA.GetString(1061);
		}
		m_SUI.Desc.color = m_TData.GetTextDarkColor();
		if (_view) {
			if (m_TData.m_IsRateValue) {
				m_SUI.Count[0].text = m_SUI.Count[1].text = string.Format("{0} <size=30>% / {1} <size=27>%", STAGEINFO.m_Check.GetClearCnt(m_Pos), STAGEINFO.m_Check.GetClearMaxCnt(m_Pos));
			}
			else {
				m_SUI.Count[0].text = m_SUI.Count[1].text = string.Format("{0} / {1}", STAGEINFO.m_Check.GetClearCnt(m_Pos), STAGEINFO.m_Check.GetClearMaxCnt(m_Pos));
			}
		}
		else {
			m_SUI.Count[0].text = m_SUI.Count[1].text = "? / ?";
		}
	}
	public void SetAnim(State _state, Action<GameObject> _cb) {
		if(gameObject.activeInHierarchy) StartCoroutine(Anim(_state, _cb));
	}

	IEnumerator Anim(State _state, Action<GameObject> _cb) {
		m_Anim.Add(_state.ToString());
		m_SUI.Anim.SetTrigger(_state.ToString());
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		_cb?.Invoke(gameObject);
	}
	public void SetAnim(string _trig) {
		m_Anim.Add(_trig);
		m_SUI.Anim.SetTrigger(_trig);
	}
}
