using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;

public class PVP_Camp : PopupBase
{
	[Serializable]
	public struct SUI {
		public TextMeshProUGUI Lv;
		public Image[] MatIcons;
		public TextMeshProUGUI[] MatNames;
		public TextMeshProUGUI[] Ratio;
		public TextMeshProUGUI[] Max;
		public GameObject[] UnLockGroup;
		public GameObject[] LockGroup;
		public TextMeshProUGUI[] LockDesc;	//6215
	}
	[SerializeField] SUI m_SUI;
	RES_PVP_USER_BASE m_UserInfo;
	bool Is_Action;
	PVPUserCampInfo m_CampInfo { get { return m_UserInfo.CampInfo; } }
	CampBuildInfo m_CampBuildInfo { get { return USERINFO.m_CampBuild[CampBuildType.Camp]; } }
	TPVP_CampTable m_TData { get { return TDATA.GetTPVP_CampTable(m_CampBuildInfo.LV);} }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		m_SUI.Lv.text = string.Format("{0} <size=80%>Lv.{1}</size>", TDATA.GetString(6205), m_CampBuildInfo.LV);
		List<TPVP_CampTable> tdatas = TDATA.GetAllPVP_CampTable();
		for (int i = 0; i < 3; i++) {
			TItemTable itdata = TDATA.GetItemTable(BaseValue.CAMP_RES_IDX(i));
			m_SUI.MatIcons[i].sprite = itdata.GetItemImg();
			m_SUI.MatNames[i].text = itdata.GetName();
			m_SUI.Ratio[i].text = string.Format("{0:0.##}%", m_TData.m_RatioCnt[i].Ratio * 100);
			m_SUI.Max[i].text = Utile_Class.CommaValue(m_TData.m_RatioCnt[i].Cnt);
			if(i > 0) {
				for (int j = m_CampBuildInfo.LV; j <= tdatas.Count; j++) {
					TPVP_CampTable tdata = TDATA.GetTPVP_CampTable(j);
					if (tdata.m_RatioCnt[i].Cnt == 0) {
						m_SUI.LockGroup[i].SetActive(true);
						m_SUI.UnLockGroup[i].SetActive(false);
						TPVP_CampTable ntdata = TDATA.GetTPVP_CampTable(j + 1);
						if (ntdata != null && ntdata.m_RatioCnt[i].Cnt == 0) continue;
						else {
							m_SUI.LockDesc[i].text = string.Format(TDATA.GetString(6215), TDATA.GetString(6205), j + 1);
							break;
						}
					}
					else {
						m_SUI.LockGroup[i].SetActive(false);
						m_SUI.UnLockGroup[i].SetActive(true);
						break;
					}
				}
			}
		}
		
		base.SetUI();
	}
	/// <summary> 시설 강화 </summary>
	public void Click_Upgrade() {
		if (Is_Action) return;
		//최대 레벨 예외처리
		if (m_CampBuildInfo.LV >= TDATA.GetTPVP_Camp_NodeLevelGroup(CampBuildType.Camp).Count) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(963));
			return;
		}
		Is_Action = true;
		WEB.SEND_REQ_CAMP_BUILD((res) => {
			Is_Action = false;
			if (res.IsSuccess()) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Base_Upgrade, (res, obj) => {
					if (res == 2) Close(2);
					else SetUI();
				}, CampBuildType.Camp);
			}
		});
	}
}
