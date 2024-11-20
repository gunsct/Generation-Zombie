using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_HubInfoUI : ObjMng
{
	[Serializable] 
	public struct SUI
	{
		public Text Name;
		public TextMeshProUGUI Lv;
		public Slider ExpGauge;
		public TextMeshProUGUI CharCnt;
		public TextMeshProUGUI CP;
		public Transform BtnArrow;
	}
	[SerializeField] SUI m_SUI;
	bool Is_Open;

	void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFHubInfoUI += SetData;
		}
	}
	public void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFHubInfoUI -= SetData;
		}
	}
	/// <summary>
	/// 0:
	/// </summary>
	/// <param name="_state"></param>
	public void SetData(int _state = -1) {
		switch (_state) {
			case 0://scroll up
				Is_Open = false;
				m_SUI.BtnArrow.localEulerAngles = new Vector3(0f, 0f, 270f);
				break;
			case 1://scroll down
				Is_Open = true;
				m_SUI.BtnArrow.localEulerAngles = new Vector3(0f, 0f, 90f);
				break;
		}
		m_SUI.Lv.text = USERINFO.m_LV.ToString();
		char[] name = USERINFO.m_Name.ToCharArray();
		if(name.Length > 10) {
			char[] shotname = new char[13];
			for(int i = 0; i < 10; i++) {
				shotname[i] = name[i];
			}
			for(int i = 10; i < 13; i++) {
				shotname[i] = '.';
			}
			m_SUI.Name.text = shotname.ArrayToString();
		}
		else
			m_SUI.Name.text = USERINFO.m_Name;
		TExpTable edata = TDATA.GetExpTable(USERINFO.m_LV);
		if (edata.m_UserExp <= USERINFO.m_Exp[0]) m_SUI.ExpGauge.value = 1f;
		else m_SUI.ExpGauge.value = USERINFO.m_Exp[1] == 0 ? 0f : (float)(Math.Max(0,edata.m_UserExp - USERINFO.m_Exp[0]) / edata.m_UserExp);

		m_SUI.CharCnt.text = string.Format("{0} <color=#6F6D67>/ {1}</color>", USERINFO.m_Chars.Count, TDATA.GetAllCharacterInfos().Count);

		m_SUI.CP.text = Utile_Class.CommaValue(USERINFO.GetUserCombatPower(true));
	}
	public void Click_Btn() {
		if (TUTO.IsTutoPlay()) return;
		PopupBase popup = POPUP.GetPopup();
		PopupBase main = POPUP.GetMainUI();
		if (popup == null || popup?.m_Popup != PopupName.HubInfo) {
			if (main.m_Popup == PopupName.Play && main.GetComponent<Main_Play>().IsAction) return;
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.HubInfo, (res, obj) => {
				DLGTINFO?.f_RFHubInfoUI?.Invoke(1);
			});
			DLGTINFO?.f_RFHubInfoUI?.Invoke(0);
		}
		else if(popup != null && popup.m_Popup == PopupName.HubInfo) {
			DLGTINFO?.f_RFHubInfoUI?.Invoke(1);
			popup.Close();
		}
	}
}
