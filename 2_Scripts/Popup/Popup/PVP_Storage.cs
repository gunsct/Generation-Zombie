using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;

public class PVP_Storage : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Lv;
		public Item_PVP_Storage_Element[] Mats;
	}
	[SerializeField] SUI m_SUI;
	bool Is_Action;
	CampBuildInfo m_CampBuildInfo { get { return USERINFO.m_CampBuild[CampBuildType.Storage]; } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		m_SUI.Lv.text = string.Format("{0} <size=80%>Lv.{1}</size>", TDATA.GetString(6207), m_CampBuildInfo.LV);

		for(int i = 0; i < m_SUI.Mats.Length; i++) {
			m_SUI.Mats[i].SetData(m_CampBuildInfo.LV, i, (int)m_CampBuildInfo.Values[i]);
		}

		base.SetUI();
	}
	/// <summary> 시설 강화 </summary>
	public void Click_Upgrade() {
		if (Is_Action) return;
		//최대 레벨 예외처리
		if (m_CampBuildInfo.LV >= TDATA.GetTPVP_Camp_NodeLevelGroup(CampBuildType.Storage).Count) {
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
				}, CampBuildType.Storage);
			}
		});
	}
}
