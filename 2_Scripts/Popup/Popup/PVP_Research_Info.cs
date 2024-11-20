using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class PVP_Research_Info : PopupBase
{
	[Serializable]
	public class Condition
	{
		public Image Icon;
		public TextMeshProUGUI Lv;
	}
	[Serializable]
	public class Mat
	{
		public GameObject Obj;
		public Image Icon;
		public Image Bg;
		public TextMeshProUGUI Cnt;
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public Animator IconAnim;
		public TextMeshProUGUI[] NLv;
		public TextMeshProUGUI[] Lv;
		public TextMeshProUGUI[] Desc;
		public GameObject[] Panels;		//0:info,1:max
		public TextMeshProUGUI MaxLv;
		public TextMeshProUGUI MaxDesc;
		public Condition Conditions;
		public Mat[] Mats;
		public Vector3[] MatPos;
		public Color[] MatBgColor;
		public TextMeshProUGUI Time;
		public Image BtnBg;
		public Sprite[] BtnBgImg;
		public GameObject Btn;
	}
	[SerializeField] SUI m_SUI;
	ResearchInfo m_Info;
	TResearchTable m_TData;
	TResearchTable m_NTData;
	Action<ResearchInfo, Action> m_CB;
	bool Is_Mat = true;
	bool Is_Condition = true;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (ResearchInfo)aobjValue[0];
		m_CB = (Action<ResearchInfo, Action>)aobjValue[1];
		m_TData = m_Info.m_TData;
		m_NTData = TDATA.GetResearchTable(m_Info.m_Type, m_Info.m_Idx, m_Info.m_GetLv + 1);

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		CampBuildInfo campinfo = USERINFO.m_CampBuild[CampBuildType.Camp];
		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		DLGTINFO?.f_RFPVPJunkUI?.Invoke(storageinfo.Values[0], storageinfo.Values[0]);
		DLGTINFO?.f_RFPVPCultivateUI?.Invoke(storageinfo.Values[1], storageinfo.Values[1]);
		DLGTINFO?.f_RFPVPChemicalUI?.Invoke(storageinfo.Values[2], storageinfo.Values[2]);
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);

		m_SUI.Icon.sprite = m_Info.m_TData.GetIcon();
		m_SUI.NLv[0].text = m_SUI.NLv[1].text = string.Format("Lv.{0}/{1}", m_Info.m_GetLv, m_Info.m_MaxLV);
		switch (m_Info.m_State) {
			case TimeContentState.Idle: m_SUI.IconAnim.SetTrigger("Not"); break;
			case TimeContentState.Play: m_SUI.IconAnim.SetTrigger(m_Info.IS_Complete() ? "GetEnable" : "Not"); break;
			case TimeContentState.End: m_SUI.IconAnim.SetTrigger(m_Info.m_GetLv == m_Info.m_MaxLV ? "Max" : "Get"); break;
		}
		for (int i = 0; i < m_SUI.Lv.Length; i++) {
			m_SUI.Lv[i].text = string.Format("Lv.{0}", m_Info.m_GetLv + i);
		}
		if (m_Info.m_GetLv == m_Info.m_MaxLV) {
			m_SUI.Panels[0].SetActive(false);
			m_SUI.Panels[1].SetActive(true);
			m_SUI.MaxLv.text = string.Format("Lv.{0} (MAX)", m_Info.m_MaxLV);
			m_SUI.MaxDesc.text = m_TData.GetInfo(1);
		}
		else {
			m_SUI.Panels[0].SetActive(true);
			m_SUI.Panels[1].SetActive(false);
			m_SUI.Desc[0].text = m_TData.GetInfo(1);
			switch (m_NTData.m_Eff.m_Eff) {
				case ResearchEff.PVPJunkCountDown:
				case ResearchEff.PVPCultivateCountDown:
				case ResearchEff.PVPChemicalCountDown:
					m_SUI.Desc[1].text = string.Format("{0}\n<color=#53BC42>(+{1})</color>", m_NTData.GetInfo(1), m_NTData.m_Eff.m_Value - m_TData.m_Eff.m_Value);
					break;
				default:
					m_SUI.Desc[1].text = string.Format("{0}\n<color=#53BC42>(+{1}%)</color>", m_NTData.GetInfo(1), (m_NTData.m_Eff.m_Value - m_TData.m_Eff.m_Value) * 0.01f);
					break;
			}
		}

		//현재는 캠프 레벨로만 쓰고있어서 단순화함
		Is_Condition = campinfo.LV >= m_TData.m_UnLockVal;
		m_SUI.Conditions.Lv.text = string.Format("{0} Lv.{1}", TDATA.GetString(6205), m_TData.m_UnLockVal);
		m_SUI.Conditions.Lv.color = Utile_Class.GetCodeColor(Is_Condition ? "#53BC42" : "#FF4739");
		m_SUI.Conditions.Icon.color = Utile_Class.GetCodeColor(Is_Condition ? "#FFFFFF" : "#787878");

		for (int i = 0, m = 0; i < 3; i++) {
			if (m_TData.m_Mat.Count - 1 < i || m_TData.m_Mat[i].m_Idx == 0) m_SUI.Mats[i].Obj.SetActive(false);
			else {
				int pos = BaseValue.CAMP_RES_POS(m_TData.m_Mat[i].m_Idx);
				bool can = storageinfo.Values[pos] >= m_TData.m_Mat[i].m_Count;
				m_SUI.Mats[i].Icon.sprite = TDATA.GetItemTable(m_TData.m_Mat[i].m_Idx).GetItemImg();
				m_SUI.Mats[i].Bg.color = m_SUI.MatBgColor[pos];
				m_SUI.Mats[i].Cnt.text = string.Format("{0}/{1}", storageinfo.Values[pos], m_TData.m_Mat[i].m_Count);
				m_SUI.Mats[i].Cnt.color = BaseValue.GetUpDownStrColor(storageinfo.Values[pos], m_TData.m_Mat[i].m_Count, "#FF4739", "#53BC42");
				m_SUI.Mats[i].Obj.transform.localPosition = m_SUI.MatPos[m];
				if (!can) Is_Mat = can;
				m++;
			}
		}

		m_SUI.Time.text = UTILE.GetSecToTimeStr(m_TData.GetTime() * 0.001d);
		m_SUI.BtnBg.sprite = m_SUI.BtnBgImg[Is_Condition && Is_Mat ? 0 : 1];
		m_SUI.Btn.SetActive(m_Info.m_GetLv != m_Info.m_MaxLV);

		base.SetUI();
	}
	public void Click_Research() {
		if (!Is_Condition) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6220));
			return;
		}
		else if(!Is_Mat) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6221));
			return;
		}
		m_CB?.Invoke(m_Info, ()=> { Close(); });
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
