using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Talk_Select : ObjMng
{
	public enum SelectType
	{
		Normal,
		Special,
		End
	}
	[System.Serializable]
	public struct SelectDesc
	{
		public TextMeshProUGUI[] Txt;
	}
	[System.Serializable]
	struct SUI
	{
		public Animator Anim;
		public SelectDesc[] Desc;
		public Button Button;
	}
	[SerializeField]
	SUI m_SUI;
	Action<TCaseSelectTable, bool> m_EndCB;
	Action<Item_Talk_Select> m_BlockCB;
	public TCaseSelectTable m_TData;
	SelectType m_Type;
	int m_ChangeCnt;
	bool Is_Select;
	public Animator Anim { get { return m_SUI.Anim; } }
	/// <summary> 선택지에 따른 기능, 문구 전달</summary>
	public void SetData(Action<TCaseSelectTable, bool> _cb, Action<Item_Talk_Select> _cb2, TCaseSelectTable _table, SelectType _type, int _changecnt = 0) {
		m_Type = _type;

		m_EndCB = _cb;
		m_BlockCB = _cb2;
		m_TData = _table;
		int pos = _changecnt % 2;
		for (int i = 0; i < m_SUI.Desc[pos].Txt.Length; i++) m_SUI.Desc[pos].Txt[i].text = _table.GetString();//선택지
		Is_Select = false;
	}

	public void SetPlay() {
		SetType(m_TData.m_StrType);
	}

	void SetType(SelectStringType _strtype) {
		switch (_strtype) {
			case SelectStringType.None:
				break;
			/// <summary> 일정 시간 뒤 내용이 변하는 선택지 </summary>  
			case SelectStringType.TimeChange:
				//if (m_Type == SelectType.Special) PlayEffSound(SND_IDX.SFX_9403);
				Invoke("TW_TimeChange", m_TData.m_SelectVals[0]);
				break;
			/// <summary> 선택할 수 없는 선택지 </summary>  
			case SelectStringType.TimeBlock:
				SetAnim("TimeEnd");
				if (m_Type == SelectType.Normal) SetAnim("Color_Time");
				//if (m_Type == SelectType.Special) PlayEffSound(SND_IDX.SFX_9403);
				Invoke("TW_TimeBlock", m_TData.m_SelectVals[0]);
				break;
		}
	}
	void TW_TimeChange() {
		m_ChangeCnt++;
		//뒷면 앞면 번갈아 바뀌게..
		SetAnim(m_Type == SelectType.Normal ? m_ChangeCnt % 2 == 1 ? "Color_Refresh" : "Color_NotSelect" : "Refresh");

		SetData(m_EndCB, m_BlockCB, TDATA.GetCaseSelectTable(m_TData.m_SelectVals[1]), m_Type, m_ChangeCnt);
		SetPlay();
	}
	void TW_TimeBlock() {
		m_BlockCB?.Invoke(this);
		ButtonInteractOff(true);
		Is_Select = true;
	}
	/// <summary> 0,1번 선택지에 따른 세팅때 콜백 함수로 전달</summary>
	public void ClickSelect() {
		if (Is_Select) return;
		Is_Select = true;
		if (m_Type == SelectType.Special) PlayEffSound(SND_IDX.SFX_9404);
		else SetAnim("Color_Select");
		SetAnim("Select");
		CancelInvoke();
		m_SUI.Button.interactable = false;
		m_EndCB?.Invoke(m_TData, false);
	}
	/// <summary> 선택지 선택 후 다음으로 개행할때 버튼 비활성화 호출</summary>
	public void ButtonInteractOff(bool _time) {
		if (m_Type == SelectType.Special) PlayEffSound(SND_IDX.SFX_9404);
		if ((m_Type == SelectType.Normal || m_Type == SelectType.Special) && _time) { }
		else {
			SetAnim(m_Type == SelectType.Normal ? "NotSelect" : _time ? "Break" : "End");
		}
		if (m_Type == SelectType.Normal) {
			SetAnim(m_ChangeCnt % 2 == 1 ? "Color_Refresh" : "Color_NotSelect");
			if (!_time) SetAnim("TimeEndStop");
		}

		CancelInvoke();
		m_SUI.Button.interactable = false;
		Is_Select = true;
	}
	public void SetAnim(string _trig) {
		if (_trig.Equals("TimeEnd") || _trig.Equals("Color_Time")) m_SUI.Anim.speed = 10f / (float)m_TData.m_SelectVals[0];
		else m_SUI.Anim.speed = 1f;

		m_SUI.Anim.SetTrigger(_trig);
	}
	public Vector3 GetTxtWPos() {
		return m_SUI.Desc[0].Txt[0].transform.position;
	}
}
