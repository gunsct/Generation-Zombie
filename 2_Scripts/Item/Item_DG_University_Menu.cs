using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_DG_University_Menu : ObjMng
{
	public enum State
	{
		First,
		Act_Start,
		Deact_Start
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Title;
		public Image Icon;
		public Sprite[] Icons;
	}
	[SerializeField]
	SUI m_SUI;
	DayOfWeek m_Day;
	int m_Pos;
	int m_StgIdx;
	Action[] m_CB; //0:선택시 상세창 뜰때, 1: 상세창 꺼질때
	State m_State;

	private void OnEnable() {
		SetAnim(m_State);
	}
	public void SetData(DayOfWeek _day, int _pos, Action[] _cb) {
		m_Day = _day;
		m_Pos = _pos;
		m_CB = _cb;
		SetUI();
	}
	public void SetUI() {
		UserInfo.Stage stage = USERINFO.m_Stage[StageContentType.University];
		UserInfo.StageIdx stlv = stage.Idxs.Find(t => t.Week == m_Day && t.Pos == m_Pos);
		m_StgIdx = TDATA.GetModeTable(StageContentType.University, stlv.Idx, m_Day, m_Pos).m_StageIdx;
		TStageTable tdata = TDATA.GetStageTable(m_StgIdx);
		m_SUI.Title.text = tdata.GetName();//string.Format("{0} {1} {2}", TDATA.GetString(141 + (int)m_Day), TDATA.GetString(138), m_Pos < 1 ? "A" : "B");
		m_SUI.Icon.sprite = m_SUI.Icons[((int)m_Day - 1) * 2 + m_Pos];
	}
	/// <summary> 상태별 애니 </summary>
	public void SetAnim(State _ani) {
		m_State = _ani;
		m_SUI.Anim.SetTrigger(_ani.ToString());
	}
	/// <summary> 상세 정보창 띄움 </summary>
	public void ClickDungeonDetail()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Daily, m_Day, m_Pos)) return;
		if (m_State != State.Act_Start) return;
		if (POPUP.GetPopup().m_Popup == PopupName.Dungeon_University && !POPUP.GetPopup().GetComponent<Dungeon_University>().IS_End) {
			m_CB[0]?.Invoke();
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_University_Detail, (result, obj) => {
			}, StageContentType.University, m_Day, m_Pos, m_CB[1]);
		}
	}
}