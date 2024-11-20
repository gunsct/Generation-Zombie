using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

[System.Serializable] public class Dic_Item_DungeonMenu_Content : SerializableDictionary<StageContentType, Item_DungeonMenu.SContetUI> { }

public class Item_DungeonMenu : ObjMng
{
	[System.Serializable]
	public struct SContetUI
	{
		public Button m_Btn;
		public GameObject LockObj;
		public Animator LockAnim;
		public TextMeshProUGUI LockTxt;
		public GameObject Alarm;
	}	

	[System.Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Timer;
		public GameObject PVPTimerGroup;
		public TextMeshProUGUI PVPTimer;
		public Dic_Item_DungeonMenu_Content ContentUI;
	}
	[SerializeField]
	SUI m_SUI;
	bool Is_Predungeon;//던전 클리어하고 다시 켜진건지
	double m_PVPStime;
	bool Is_GoPVP;
	IEnumerator m_PVPTimeCor;

	private void Update()
	{
		m_SUI.Timer.text = UTILE.GetSecToTimeStr(0, 86400 - UTILE.Get_ServerTime() % 86400);//UTILE.Get_ServerTime() - ((UTILE.Get_ServerTime() / 86400) + 1) * 86400
		if (m_SUI.PVPTimerGroup.activeSelf && PVPINFO.m_Group != null) {
			m_PVPStime = Math.Max(0, (PVPINFO.m_Group.stime - UTILE.Get_ServerTime_Milli()) * 0.001d);
			m_SUI.PVPTimer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.hh_mm_ss, m_PVPStime);
			if (m_PVPStime <= 0) m_SUI.PVPTimerGroup.SetActive(false);
		}
	}

	ContentType GetContentType(StageContentType type)
	{
		switch (type)
		{
		case StageContentType.Academy: return ContentType.Academy;
		case StageContentType.University: return ContentType.University;
		case StageContentType.Tower: return ContentType.Tower;
		case StageContentType.Cemetery: return ContentType.Cemetery;
		case StageContentType.Factory: return ContentType.Factory;
		case StageContentType.Subway: return ContentType.Subway;
		case StageContentType.PvP: return ContentType.PvP;
		}
		return ContentType.Bank;
	}

	public void SetData() {
		SetUI();

		WEB.SEND_REQ_PVP_GROUP_INFO((res) => {
			if (res.IsSuccess()) {
				PVPINFO.m_Group = res;
				m_SUI.PVPTimerGroup.SetActive(res.state == 0);
			}
		});
	}
	void SetUI() {
		foreach (var info in m_SUI.ContentUI) {
			var type = GetContentType(info.Key);
			bool unlockbtn = USERINFO.CheckContentUnLock(type);
			int openidx = BaseValue.CONTENT_OPEN_IDX(type);
			m_SUI.ContentUI[info.Key].LockAnim.SetTrigger(unlockbtn ? "Loop" : "Lock");
			m_SUI.ContentUI[info.Key].LockObj.SetActive(!unlockbtn);
			m_SUI.ContentUI[info.Key].LockTxt.text = string.Format(TDATA.GetString(163), openidx / 100, openidx % 100);
			if (type == ContentType.Tower || type == ContentType.PvP)
				m_SUI.ContentUI[info.Key].Alarm.SetActive(false);
			else
				m_SUI.ContentUI[info.Key].Alarm.SetActive(unlockbtn && USERINFO.m_Stage[info.Key].GetItemCnt(false) > 0);
		}

		DLGTINFO?.f_RFShellUI?.Invoke(USERINFO.m_Energy.Cnt);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
	}
	public void ClickGoDungeon(int _pos, bool _predungeon) {
		Is_Predungeon = _predungeon;
		ClickGoDungeon(_pos);
	}
	[EnumAction(typeof(StageContentType))]
	public void ClickGoDungeon(int _pos)
	{
		StageContentType type = (StageContentType)_pos;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Dungeon_Menu, type)) return;
		if(type != StageContentType.PvP)
			USERINFO.m_Stage[type].CalcLimit();
		switch (type)
		{
			case StageContentType.Bank:
				if (!USERINFO.CheckContentUnLock(ContentType.Bank, true)) {
					PlayCommVoiceSnd(VoiceType.Fail); 
					break;
				}
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Bank, (result, obj) => { SetUI(); }, StageContentType.Bank);
				//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Detail, (result, obj) => { }, StageContentType.Bank);
				//if (TUTO.IsTuto(TutoKind.Bank, (int)TutoType_Bank.Select_Bank)) TUTO.Next();
				break;
			case StageContentType.Academy:
				if (!USERINFO.CheckContentUnLock(ContentType.Academy, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					break;
				}
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Training, (result, obj) => { SetUI(); }, StageContentType.Academy, Is_Predungeon);
				//if (TUTO.IsTuto(TutoKind.Academy, (int)TutoType_Academy.Select_Academy)) TUTO.Next();
				break;
			case StageContentType.University:
				if (!USERINFO.CheckContentUnLock(ContentType.University, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					break;
				}
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_University, (result, obj) => { SetUI(); });
				//if (TUTO.IsTuto(TutoKind.University, (int)TutoType_University.Select_University)) TUTO.Next();
				break;
			case StageContentType.Tower:
				if (!USERINFO.CheckContentUnLock(ContentType.Tower, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					break;
				}
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Tower, (result, obj) => { SetUI(); }, StageContentType.Tower, Is_Predungeon);
				//if (TUTO.IsTuto(TutoKind.Tower, (int)TutoType_Tower.Select_Tower)) TUTO.Next();
				break;
			case StageContentType.Cemetery:
				if (!USERINFO.CheckContentUnLock(ContentType.Cemetery, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					break;
				}
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Cemetery, (result, obj) => { SetUI(); }, StageContentType.Cemetery);
				//if (TUTO.IsTuto(TutoKind.Cemetery, (int)TutoType_Cemetery.Select_Cemetery)) TUTO.Next();
				break;
			case StageContentType.Factory:
				if (!USERINFO.CheckContentUnLock(ContentType.Factory, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					break;
				}
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Factory, (result, obj) => { SetUI(); }, StageContentType.Factory);
				//if (TUTO.IsTuto(TutoKind.Factory, (int)TutoType_Factory.Select_Factory)) TUTO.Next();
				break;
			case StageContentType.Subway:
				if (!USERINFO.CheckContentUnLock(ContentType.Subway, true)) {
					PlayCommVoiceSnd(VoiceType.Fail);
					break;
				}
				//if (TUTO.IsTuto(TutoKind.Subway, (int)TutoType_Subway.Select_Subway)) TUTO.Next();
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Dungeon_Subway, (result, obj) => { SetUI(); }, StageContentType.Subway, UTILE.GetServerDayofWeek());
				break;
			case StageContentType.PvP:
				OpenPVP();
				break;
		}
		Is_Predungeon = false;
	}
	public void OpenPVP(Action _cb = null) {
		if (Is_GoPVP) return;
		if (!USERINFO.CheckContentUnLock(ContentType.PvP, true)) {
			PlayCommVoiceSnd(VoiceType.Fail);
			return;
		}
		Is_GoPVP = true;
		WEB.SEND_REQ_PVP_GROUP_INFO((res) => {
			if (res.IsSuccess()) {
				PVPINFO.m_Group = res;
				if (res.state == 1 || TUTO.IsTuto(TutoKind.PVP_Main)) {
					WEB.SEND_REQ_CAMP_BUILD((res) => {
						if (res.IsSuccess()) {
							PlayEffSound(SND_IDX.SFX_1300);
							POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Main, (res, obj)=> { Is_GoPVP = false; }, PVPINFO.m_Group, res.Logs);
							_cb?.Invoke();
						}
						else Is_GoPVP = false;
					});
				}
				else {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10008));
					Is_GoPVP = false;
					return;
				}
			}
			else Is_GoPVP = false;
		});
	}
	public GameObject GetBtn(StageContentType type)
	{
		
		if (!m_SUI.ContentUI.ContainsKey(type)) return null;
		return m_SUI.ContentUI[type].m_Btn.gameObject;
	}
}