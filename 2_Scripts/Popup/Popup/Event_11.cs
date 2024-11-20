using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static Utile_Class;
using UnityEngine.UI;
using System.Linq;

public class Event_11 : PopupBase
{
	public enum State
	{
		Main,
		Minigame,
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
		public AnimEventCallBack CharAnimCB;
		public Image[] CharIcon;		//0:back, 1:front
		public Image[] CharJob;
		public TextMeshProUGUI ItemCnts;
		public GameObject[] Panels;     //stage, mission, minigame
		public MenuBtn[] MenuBtns;
	}
	[SerializeField] SUI m_SUI;
	State m_State = State.None;
	MyFAEvent m_Event;
	FAEventData_GrowUP m_GrowUp;
	IEnumerator m_TimerCor;
	SND_IDX m_NowBG;
	List<TCharacterTable> m_NeedChars = new List<TCharacterTable>();
	int m_CharPos = 0;

	private void Update() {
		if (m_Event == null) return;
		double time = m_Event.GetRemainEndTime();
		if (time < 0) time = 0;
		m_SUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(TimeStyle.single, time * 0.001d));
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_NowBG = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_1000);
		m_Event = (MyFAEvent)aobjValue[0];//USERINFO.m_Event.Datas.Find(o => o.Type == LS_Web.FAEventType.GrowUP);
		m_GrowUp = (FAEventData_GrowUP)m_Event.RealData;
		m_NeedChars = m_GrowUp.StageCharReward.Select(o => TDATA.GetCharacterTable(o.Idx)).ToList();

		for (int i = 0; i < 2; i++) {
			TCharacterTable needchar = m_NeedChars[i];
			m_SUI.CharIcon[i].sprite = needchar.GetPortrait();
			m_SUI.CharJob[i].sprite = needchar.GetJobIcon()[0];
		}
		m_SUI.CharAnimCB.m_CB.Add(SetCharChange);

		base.SetData(pos, popup, cb, aobjValue);

		SetMenu(0);
		if (PlayerPrefs.GetInt(string.Format("VIEW_EVENT_{0}_{1}", m_Event.UID, USERINFO.m_UID), 0) == 0) Click_Help();
	}
	public override void SetUI() {
		DLGTINFO?.f_RFShellUI.Invoke(USERINFO.m_Energy.Cnt);
		DLGTINFO?.f_RFCashUI.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);

		m_SUI.ItemCnts.text = USERINFO.GetItemCount(BaseValue.EVENT_11_ITEMIDX).ToString();

		SetAlarm();
		base.SetUI();
	}
	void SetCharChange() {
		m_CharPos++;
		if (m_CharPos == m_NeedChars.Count) m_CharPos = 0;
		TCharacterTable needchar = m_NeedChars[m_CharPos];
		m_SUI.CharIcon[1].sprite = m_SUI.CharIcon[0].sprite;
		m_SUI.CharJob[1].sprite = m_SUI.CharJob[0].sprite;
		m_SUI.CharIcon[0].sprite = needchar.GetPortrait();
		m_SUI.CharJob[0].sprite = needchar.GetJobIcon()[0];

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
			case State.Mission:
				WEB.SEND_REQ_MISSIONINFO((res) => {
					if (res.IsSuccess()) {
						m_SUI.MenuBtns[1].Anim.SetTrigger("Select");
						m_SUI.Panels[1].SetActive(true);
						m_SUI.Panels[1].GetComponent<Item_Event_11_Minigame>().SetData(m_Event);
					}
					else SetMenu(0);
				});
				break;
			case State.Minigame:
				m_SUI.MenuBtns[2].Anim.SetTrigger("Select");
				m_SUI.Panels[2].SetActive(true);
				m_SUI.Panels[2].GetComponent<Item_Event_11_Mission>().SetData(m_Event);
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
		}
		else {
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
		Item_Event_11_Stage stage = m_SUI.Panels[0].GetComponent<Item_Event_11_Stage>();
		stage.SetData(m_Event);
	}
	public void Click_RecommendCharacter() {
		if (IS_EvtEnd()) return;
		PlayEffSound(SND_IDX.SFX_3050);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Event_10_RecomSrv, null, m_GrowUp.StageCharReward);
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
			case State.Minigame:
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
