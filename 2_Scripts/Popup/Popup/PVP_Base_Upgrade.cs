using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PVP_Base_Upgrade : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI[] Lvs;
		public Item_PVP_Base_Upgrade_Info[] Infos;
		public Item_PVP_Base_Upgrade_Condition[] Conditions;
		public Item_PVP_Base_Upgrade_Mat[] Mats;
		public Item_Store_Buy_Button Btn;
		public Image BtnBg;
		public Sprite[] BtnBgImg;
	}
	[SerializeField] SUI m_SUI;
	CampBuildType m_Type;
	bool Is_CanUpgrade;
	bool Is_Action;
	CampBuildInfo m_CampBuildInfo { get { return USERINFO.m_CampBuild[m_Type]; } }
	TPVP_Camp_NodeLevel m_TData { get { return TDATA.GetTPVP_Camp_NodeLevel(m_Type, m_CampBuildInfo.LV); } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Type = (CampBuildType)aobjValue[0];

		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];

		string name = string.Empty;
		switch (m_Type) {
			case CampBuildType.Camp: name = TDATA.GetString(6205); break;
			case CampBuildType.Storage: name = TDATA.GetString(6207); break;
			case CampBuildType.Resource: name = TDATA.GetString(6206); break;
		}
		m_SUI.Name.text = name;
		m_SUI.Lvs[0].text = string.Format("Lv.{0}", m_CampBuildInfo.LV);
		m_SUI.Lvs[1].text = string.Format("Lv.{0}", m_CampBuildInfo.LV + 1);

		for (int i = 0, pos = 0, offpos = 2; i < 3;i++) {
			if (m_TData.m_Condition[i].m_Type == CampBuildType.None) {
				m_SUI.Conditions[offpos].gameObject.SetActive(false);
				offpos--;
				continue;
			}
			m_SUI.Conditions[pos].SetData(m_TData, i);
			pos++;
		}
		for (int i = 0; i < 3; i++) {
			m_SUI.Infos[i].SetData(m_Type, m_CampBuildInfo.LV, i);
			m_SUI.Mats[i].SetData(m_TData, i, (int)storageinfo.Values[i]);
		}
		//순서상 위에 세팅하면서 체크하게 해둠
		Check_Upgrade();
		m_SUI.Btn.SetData(PriceType.Money, m_TData.m_Cost[3]);
		m_SUI.BtnBg.sprite = m_SUI.BtnBgImg[Is_CanUpgrade ? 0 : 1];

		base.SetUI();
	}
	/// <summary> 업그레이드 가능한지 체크 </summary>
	void Check_Upgrade() {
		Is_CanUpgrade = true;
		for (int i = 0; i < 3; i++) {
			if(m_SUI.Conditions[i].gameObject.activeSelf && !m_SUI.Conditions[i].Is_Can) {
				Is_CanUpgrade = false;
				break;
			}
			if(m_SUI.Mats[i].gameObject.activeSelf && !m_SUI.Mats[i].Is_Can) {
				Is_CanUpgrade = false;
				break;
			}
		}
		if (m_TData.m_Cost[3] > USERINFO.m_Money) Is_CanUpgrade = false;
	}
	/// <summary> 시설 강화 </summary>
	public void Click_Upgrade() {
		if (Is_Action) return;
		if (!Is_CanUpgrade) {
			if (m_TData.m_Cost[3] > USERINFO.m_Money) POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10836));
			return;
		}
		Is_Action = true;
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		long[] preval = new long[4] { storageinfo.Values[0], storageinfo.Values[1], storageinfo.Values[2], USERINFO.m_Money };
		WEB.SEND_REQ_CAMP_BUILD_LVUP((res) => {
			if (res.IsSuccess()) {
				PlayEffSound(SND_IDX.SFX_0186);
				if (m_CampBuildInfo.LV >= TDATA.GetTPVP_Camp_NodeLevelGroup(m_Type).Count) {
					Is_Action = false;
					Close();
				}
				else {
					WEB.SEND_REQ_CAMP_BUILD((res) => {
						Is_Action = false;
						if (res.IsSuccess()) {
							DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], preval[0]);
							DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], preval[1]);
							DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], preval[2]);
							DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, preval[3]);

							SetUI();
						}
					});
				}
			}
			else {
				Is_Action = false;
				if (res.result_code == EResultCode.ERROR_CAMP_RES) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6248));
					Close(2);
				}
			}
		}, m_Type);
	}
}
