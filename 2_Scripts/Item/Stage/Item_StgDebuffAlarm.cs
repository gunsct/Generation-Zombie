using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class Item_StgDebuffAlarm : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public GameObject[] IconGroup;
		public Animator[] Anims;
		public Image[] Icons;
		public GameObject ToolTip;
	}
	[SerializeField]
	SUI m_SUI;
	public TStatusDebuffTable m_TData;
	int m_PreStep = 0;
	int m_Step = 0;
	private void Awake() {
		for (int i = 0; i < 2; i++) m_SUI.IconGroup[i].gameObject.SetActive(false);
	}
	public void SetData(TStatusDebuffTable _table) {
		m_TData = _table;
		m_Step = TDATA.GetStatusDebuffPos(m_TData);
		m_PreStep = 0;
		m_SUI.Icons[0].sprite = m_SUI.Icons[1].sprite = UTILE.LoadImg(m_TData.m_Icon, "png");
		var ac = gameObject.activeSelf;
		Refresh(_table);
	}
	/// <summary> 디버프 단계 갱신 </summary>
	public void Refresh(TStatusDebuffTable _table, Action<Item_StgDebuffAlarm> _cb = null, Action<int> _cb2 = null) {
		if(_table == null) {
			StopAllCoroutines();
			StartCoroutine(IE_Delete(_cb)); 
			_cb2?.Invoke(0);
		}
		else {
			m_TData = _table;
			m_Step = TDATA.GetStatusDebuffPos(m_TData);
			if(m_PreStep != m_Step) StopAllCoroutines();
			m_SUI.ToolTip.SetActive(m_Step > 0);
			switch (m_PreStep) {
				case 0:
					switch (m_Step) {
						case 0:break;
						case 1:
							StartCoroutine(IE_StepStart(m_SUI.Anims[0]));
							break;
						case 2:
							StartCoroutine(IE_StepStack(m_SUI.Anims[0], true));
							StartCoroutine(IE_StepStart(m_SUI.Anims[1], 0.2f));
							break;
					}
					break;
				case 1:
					switch (m_Step) {
						case 0:
							StartCoroutine(IE_StepEnd(m_SUI.Anims[0]));
							break;
						case 1:
							break;
						case 2:
							StartCoroutine(IE_StepStack(m_SUI.Anims[0], true));
							StartCoroutine(IE_StepStart(m_SUI.Anims[1]));
							break;
					}
					break;
				case 2:
					switch (m_Step) {
						case 0:
							StartCoroutine(IE_StepEnd(m_SUI.Anims[1]));
							StartCoroutine(IE_StepStack(m_SUI.Anims[0], false));
							StartCoroutine(IE_StepEnd(m_SUI.Anims[0], 0.5f));
							break;
						case 1:
							StartCoroutine(IE_StepStack(m_SUI.Anims[0], false));
							StartCoroutine(IE_StepEnd(m_SUI.Anims[1]));
							break;
						case 2:
							break;
					}
					break;
			}
		}
		if (m_PreStep > m_Step) _cb2?.Invoke(m_Step);
		m_PreStep = m_Step;
	}
	IEnumerator IE_Delete(Action<Item_StgDebuffAlarm> _cb) {
		//1개일때 2개일때 체크해서 애니 돌려주고 끄기
		if (m_Step == 2) {
			StartCoroutine(IE_StepEnd(m_SUI.Anims[1]));
			StartCoroutine(IE_StepStack(m_SUI.Anims[0], false));
			Coroutine lastcor = StartCoroutine(IE_StepEnd(m_SUI.Anims[0], 0.5f));
			yield return lastcor;
		}
		else if (m_Step == 1) {
			Coroutine lastcor = StartCoroutine(IE_StepEnd(m_SUI.Anims[0]));
			yield return lastcor;
		}
		_cb.Invoke(this);
	}
	IEnumerator IE_StepEnd(Animator _anim, float _time = 0f) {
		yield return new WaitForSeconds(_time);
		_anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(_anim));
		_anim.gameObject.SetActive(false);
	}
	IEnumerator IE_StepStart(Animator _anim, float _time = 0f) {
		yield return new WaitForSeconds(_time);
		_anim.gameObject.SetActive(true);
		_anim.SetTrigger("Start");
	}
	IEnumerator IE_StepStack(Animator _anim, bool _up, float _time = 0f) {
		yield return new WaitForSeconds(_time);
		_anim.gameObject.SetActive(true);
		_anim.SetTrigger(_up ? "Stack" : "Normal");
	}
	public void ClickShowInfo() {
		Main_Stage main = POPUP.GetMainUI().GetComponent<Main_Stage>();
		main.SetAlarmToolTip(m_TData.GetName(), m_TData.GetDesc(), transform.position);
	}
	public void PointerDown() {
		Main_Stage main = POPUP.GetMainUI().GetComponent<Main_Stage>();
		main.SetAlarmToolTip(m_TData.GetName(), m_TData.GetDesc(), transform.position);
	}
	public void PointerUp() {
		//POPUP.GetMainUI().GetComponent<Main_Stage>().OffAlarmToolTip();
	}
}
