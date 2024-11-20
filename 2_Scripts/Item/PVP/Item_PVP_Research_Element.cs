using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
public class Item_PVP_Research_Element : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI[] Lv;
		public GameObject TimeGroup;
		public GameObject[] CompGroup;
		public TextMeshProUGUI Timer;
		public GameObject Alarm;
	}
	[SerializeField] SUI m_SUI;
	ResearchInfo m_Info;
	public TimeContentState m_State { get { return m_Info.m_State; } }
	string m_Anim = "Not";
	Action<ResearchInfo, Action> m_CB;
	TResearchTable m_TData { get { return m_Info.m_TData; } }

	public void Update() {
		if (m_Info == null) return;
		if(m_SUI.Anim.GetCurrentAnimatorStateInfo(0).IsName(m_Anim)) m_SUI.Anim.SetTrigger(m_Anim);
		if (m_Info.m_State == TimeContentState.Play) {
			double remain = m_Info.GetRemainTime();
			m_SUI.Timer.text = UTILE.GetSecToTimeStr(remain);
			if (remain == 0 && !m_Anim.Equals("GetEnable")) m_SUI.Anim.SetTrigger("GetEnable");
		}
	}
	public void SetData(ResearchInfo _info, Action<ResearchInfo, Action> _cb) {
		m_Info = _info;
		m_CB = _cb;

		m_SUI.Icon.sprite = m_TData.GetIcon();
		m_SUI.Name.text = m_TData.GetName();
		m_SUI.Lv[0].text = m_SUI.Lv[1].text = m_Info.m_GetLv == m_Info.m_MaxLV ? "Lv.MAX" : string.Format("Lv.{0}/{1}", m_Info.m_GetLv, m_Info.m_MaxLV);
		m_SUI.TimeGroup.SetActive(m_Info.m_State == TimeContentState.Play && !m_Info.IS_Complete());
		for (int i = 0; i < m_SUI.CompGroup.Length; i++)
			m_SUI.CompGroup[i].SetActive(m_Info.IS_Complete());

		CampBuildInfo storageinfo = USERINFO.m_CampBuild[CampBuildType.Storage];
		CampBuildInfo campinfo = USERINFO.m_CampBuild[CampBuildType.Camp];
		bool canmake = campinfo.LV >= m_Info.m_TData.m_UnLockVal;
		if (canmake) {
			for (int i = 0; i < m_TData.m_Mat.Count; i++) {
				int pos = BaseValue.CAMP_RES_POS(m_TData.m_Mat[i].m_Idx);
				bool can = storageinfo.Values[pos] >= m_TData.m_Mat[i].m_Count;
				if (!can) {
					canmake = can;
					break;
				}
			}
		}
		m_SUI.Alarm.SetActive(canmake);

		switch (m_Info.m_State) {
			case TimeContentState.Idle: m_Anim = m_Info.m_GetLv > 0 ? "Get" : (canmake ? "GetEnable" : "Not"); break;
			case TimeContentState.Play: m_Anim = "GetEnable"; break;
			case TimeContentState.End:
				if (m_Info.m_GetLv == m_Info.m_MaxLV) m_Anim = "Max";
				else m_Anim = "Get"; break;
		}
		m_SUI.Anim.SetTrigger(m_Anim);

	}
	public void Click_Research() {
		if(m_Info.m_State == TimeContentState.Idle) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.PVP_Research_Info, (res, obj) => { }, m_Info, m_CB);
		}
		else {
			m_CB?.Invoke(m_Info, null);
		}
	}
}
