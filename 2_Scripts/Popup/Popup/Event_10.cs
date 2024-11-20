using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static Utile_Class;

public class Event_10 : PopupBase
{
	public enum State
	{
		Main,
		Reward,
		Mission,
		None
	}
	[Serializable]
	public class MenuBtn
	{
		public Animator Anim;
		public GameObject Alarm;
	}
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Timer;
		public TextMeshProUGUI[] ItemCnts;
		public GameObject[] Panels;     //stage,gift,mission
		public MenuBtn[] MenuBtns;

		public Animator ThunderAnim;
		public Animator PumkinAnim;
	}
	[SerializeField] SUI m_SUI;
	State m_State = State.None;
	MyFAEvent m_Event;
	IEnumerator m_TimerCor;
	SND_IDX m_NowBG;

	private void Update() {
		if (m_Event == null) return;
		double time = m_Event.GetRemainEndTime();
		if (time < 0) time = 0;
		m_SUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(TimeStyle.single, time * 0.001d));
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_NowBG = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_1000);
		m_Event = (MyFAEvent)aobjValue[0];// USERINFO.m_Event.Datas.Find(o => o.Type == LS_Web.FAEventType.Stage_Minigame);

		base.SetData(pos, popup, cb, aobjValue);
		SetMenu(0);
		if (PlayerPrefs.GetInt(string.Format("VIEW_EVENT_{0}_{1}", m_Event.UID, USERINFO.m_UID), 0) == 0) Click_Help();
	}
	public override void SetUI() {
		DLGTINFO?.f_RFShellUI.Invoke(USERINFO.m_Energy.Cnt);
		DLGTINFO?.f_RFCashUI.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		for (int i = 0; i < m_SUI.ItemCnts.Length; i++) {
			m_SUI.ItemCnts[i].text = USERINFO.GetItemCount(BaseValue.EVENT_10_ITEMIDX[i]).ToString();
		}
		SetAlarm();
		base.SetUI();
	}
	/// <summary>
	/// 0:메인,1:보상,2:미션
	/// </summary>
	/// <param name="_pos"></param>
	public void SetMenu(int _pos) {
		if (IS_EvtEnd()) return;
		if (m_State == (State)_pos) return;
		if(m_State != State.None) {
			PlayEffSound(SND_IDX.SFX_3050);
			SetDecoSND(_pos);
		}

		for (int i = 0; i < m_SUI.Panels.Length; i++) {
			m_SUI.Panels[i].SetActive(false);
		}
		for(int i = 0;i< m_SUI.MenuBtns.Length; i++) {
			m_SUI.MenuBtns[i].Anim.SetTrigger("Normal");
		}
		m_State = (State)_pos;

		switch (m_State) {
			case State.Main:
				m_SUI.MenuBtns[0].Anim.SetTrigger("Select");
				break;
			case State.Reward:
				WEB.SEND_REQ_MISSIONINFO((res) => {
					if (res.IsSuccess()) {
						m_SUI.MenuBtns[1].Anim.SetTrigger("Select");
						m_SUI.Panels[1].SetActive(true);
						m_SUI.Panels[1].GetComponent<Item_Event_10_Gift>().SetData(m_Event);
					}
					else SetMenu(0);
				});
				break;
			case State.Mission:
				m_SUI.MenuBtns[2].Anim.SetTrigger("Select");
				m_SUI.Panels[2].SetActive(true);
				m_SUI.Panels[2].GetComponent<Item_Event_10_Mission>().SetData(m_Event);
				break;
		}
	}
	void SetAlarm() {
		bool mission = USERINFO.m_Mission.IsSuccess(MissionMode.Event_CharMission);
		bool gift = USERINFO.m_Mission.IsSuccess(MissionMode.Event_miniGame) || USERINFO.m_Mission.IsSuccess(MissionMode.Event_miniGame_Clear);
		m_SUI.MenuBtns[0].Alarm.SetActive(false);
		m_SUI.MenuBtns[1].Alarm.SetActive(gift);
		m_SUI.MenuBtns[2].Alarm.SetActive(mission);
	}
	void SetDecoSND(int _pos) {
		if ((State)_pos == State.Main) {
			m_SUI.ThunderAnim.SetTrigger("Thunder");
			m_SUI.PumkinAnim.SetTrigger("Pumkin");
		}
		else {
			m_SUI.ThunderAnim.SetTrigger("ThunderStop");
			m_SUI.PumkinAnim.SetTrigger("PumkinStop");
		}
	}
	public void Click_Help() {
		if (IS_EvtEnd()) return;
		if (m_State != State.Main) return;
		PlayEffSound(SND_IDX.SFX_3050);
		PlayerPrefs.SetInt(string.Format("VIEW_EVENT_{0}_{1}", m_Event.UID, USERINFO.m_UID), 1);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Help_Info_Event_10);
	}
	public void Click_GoStage() {
		if (IS_EvtEnd()) return;
		if (m_State != State.Main) return; 
		SetDecoSND(1);
		PlayEffSound(SND_IDX.SFX_3002);
		m_SUI.Panels[0].SetActive(true);
		Item_Event_10_Stage stage = m_SUI.Panels[0].GetComponent<Item_Event_10_Stage>();
		stage.SetData(m_Event);
	}
	public override void Close(int Result = 0) {
		switch (m_State) {
			case State.Main:
				if (m_SUI.Panels[0].activeSelf) {
					SetDecoSND(0);
					m_SUI.Panels[0].SetActive(false);
					return;
				}
				break;
			case State.Reward:
			case State.Mission:
				SetMenu(0);
				return;
		}
		PlayBGSound(m_NowBG);
		base.Close(Result);
	}
	bool IS_EvtEnd() {
		if (m_Event.GetRemainEndTime() <= 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2036));
			return true;
		}
		else return false;
	}
}
