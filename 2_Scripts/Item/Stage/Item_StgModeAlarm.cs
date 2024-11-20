using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Item_StgModeAlarm : ObjMng
{
	enum State
	{
		Normal,
		Point,
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public GameObject CountGroup;
		public TextMeshProUGUI[] Val;
	}
	[SerializeField]
	SUI m_SUI;
	public PlayType m_Mode;
	public int[] m_Val = new int[2];
	State m_State = State.Normal;

	private void Awake() {
		m_SUI.CountGroup.SetActive(false);
	}
	private void OnEnable() {
		if(m_State == State.Point) m_SUI.Anim.SetTrigger("Loop");
	}
	/// <summary> 플레이 모드로 세팅 </summary>
	public void SetData(TStageTable.StagePlayType _mode) { 
		m_Mode =_mode.m_Type;
		BaseValue.StagePlayTypeInfo info = BaseValue.GetStagePlayTypeInfo(m_Mode);
		switch (m_Mode) {
			case PlayType.FieldAirstrike:
			case PlayType.FireSpread:
			case PlayType.CardShuffle:
				SetData(m_Mode, info.m_Icon, _mode.m_Val[0]);
				break;
			case PlayType.StreetLight:
				SetData(m_Mode, info.m_Icon, _mode.m_Val[1]);
				break;
			case PlayType.NoCool:
			case PlayType.CardLock:
			case PlayType.EasyCardLock:
			case PlayType.CardRandomLock:
			case PlayType.Blind:
			case PlayType.RandomCharOut:
			case PlayType.HighCharOut:
			case PlayType.LowCharOut:
			case PlayType.BanActive:
				SetData(m_Mode, info.m_Icon, 0);
				break;
		}
	}
	/// <summary> 모드는 아니지만 유아이 쓰고싶을 때 </summary>
	public void SetData(PlayType _mode, Sprite _icon, int _maxval) {
		m_Mode = _mode;
		m_SUI.Icon.sprite = _icon;
		if (_maxval > 0) {
			m_Val[0] = m_Val[1] = _maxval;
			m_SUI.CountGroup.SetActive(true);

			RefreshAlarm(0);
		}
	}
	public void RefreshAlarm(int _add) {
		m_State = State.Normal;
		m_Val[0] = Mathf.Max(0, m_Val[0] + _add);
		for (int i = 0;i<m_SUI.Val.Length;i++)  m_SUI.Val[i].text = m_Val[0].ToString();
		if(_add != 0) {
			if (m_Val[0] == 1) {
				m_State = State.Point;
				m_SUI.Anim.SetTrigger("Point");
			}
			else m_SUI.Anim.SetTrigger("CountChange");
		}
	}
	public int GetRemain() {
		return m_Val[1] - m_Val[0];
	}
	public void ClickShowInfo() {
		Main_Stage main = POPUP.GetMainUI().GetComponent<Main_Stage>();
		BaseValue.StagePlayTypeInfo info = BaseValue.GetStagePlayTypeInfo(m_Mode);
		main.SetAlarmToolTip(info.m_Name, info.m_Desc, transform.position);
	}
	public void PointerDown() {
		Main_Stage main = POPUP.GetMainUI().GetComponent<Main_Stage>();
		switch (m_Mode) {
			case PlayType.FieldAirstrike:
				main.SetAlarmToolTip(TDATA.GetString(ToolData.StringTalbe.Etc, 70001), TDATA.GetString(ToolData.StringTalbe.Etc, 71001), transform.position);
				break;
			case PlayType.NoCool:
				main.SetAlarmToolTip(TDATA.GetString(ToolData.StringTalbe.Etc, 70002), TDATA.GetString(ToolData.StringTalbe.Etc, 71002), transform.position);
				break;
			case PlayType.StreetLight:
				main.SetAlarmToolTip(TDATA.GetString(ToolData.StringTalbe.Etc, 71004), TDATA.GetString(ToolData.StringTalbe.Etc, 71005), transform.position);
				break;
			case PlayType.FireSpread:
				main.SetAlarmToolTip(TDATA.GetString(ToolData.StringTalbe.Etc, 70004), TDATA.GetString(ToolData.StringTalbe.Etc, 71006), transform.position);
				break;
			case PlayType.TurmoilCount:
				main.SetAlarmToolTip(TDATA.GetString(ToolData.StringTalbe.Etc, 70003), TDATA.GetString(ToolData.StringTalbe.Etc, 71003), transform.position);
				break;
			case PlayType.CardShuffle:
				main.SetAlarmToolTip(TDATA.GetString(ToolData.StringTalbe.Etc, 70006), TDATA.GetString(ToolData.StringTalbe.Etc, 71028), transform.position);
				break;
		}
	}
	public void PointerUp() {
		//POPUP.GetMainUI().GetComponent<Main_Stage>().OffAlarmToolTip();
	}
}   
