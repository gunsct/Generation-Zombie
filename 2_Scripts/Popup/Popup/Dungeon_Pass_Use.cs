using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Dungeon_Pass_Use : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI GetCnt;
		public TextMeshProUGUI UseCnt;
		public TextMeshProUGUI BtnTxt;
		public GameObject[] EnergyGroups;
		public TextMeshProUGUI[] EnergyTxts;
		public Button[] Btns;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	int[] m_InTicketCnt = new int[2];
	int m_PassTicketCnt = 0;
	int m_CanUseCnt;
	int m_CanTicket;
	int m_UseCnt = 1;
	int m_Energy;
	bool IS_Replay;
	bool m_IsRefresh;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_InTicketCnt[0] = (int)aobjValue[0];
		m_InTicketCnt[1] = (int)aobjValue[1];
		m_Energy = (int)aobjValue[2];
		IS_Replay = (bool)aobjValue[3];
		RefreshUI();
	}
	void RefreshUI() {
		m_PassTicketCnt = USERINFO.GetItemCount(BaseValue.CLEARTICKET_IDX);
		m_SUI.GetCnt.text = string.Format("x{0}",m_PassTicketCnt);
		m_CanTicket = IS_Replay ? m_PassTicketCnt : Mathf.Min(m_InTicketCnt[0], m_PassTicketCnt);
		m_CanUseCnt = m_Energy == 0 ? m_CanTicket : Mathf.Min(m_CanTicket, (int)(USERINFO.m_Energy.Cnt / m_Energy));//입장권, 에너지, 티켓
		if(m_InTicketCnt[0] > 0)
			m_SUI.UseCnt.text = string.Format("<color=#D4CD99>{0}</color> <size=80%>/ {1}</size>", m_CanUseCnt, m_InTicketCnt[0]);
		else
			m_SUI.UseCnt.text = string.Format("<color=#D4CD99>{0}</color>", m_CanUseCnt);
		//m_SUI.Btns[0].interactable = m_SUI.Btns[1].interactable = m_CanTicket > 0;
		m_SUI.BtnTxt.text = string.Format(TDATA.GetString(415), m_UseCnt);
		int maxcnt = m_CanUseCnt < 1 ? m_InTicketCnt[0] * m_Energy : m_CanUseCnt * m_Energy;
		m_SUI.EnergyGroups[0].SetActive(maxcnt > 0);
		m_SUI.EnergyTxts[0].text = string.Format("<size=140%>-</size> {0}", maxcnt);
		m_SUI.EnergyTxts[0].color = BaseValue.GetUpDownStrColor(Mathf.Min(m_PassTicketCnt, (int)USERINFO.m_Energy.Cnt), maxcnt, "#D2533C", "#FFFFFF");
		m_SUI.EnergyGroups[1].SetActive(m_Energy > 0);
		m_SUI.EnergyTxts[1].text = string.Format("<size=140%>-</size> {0}", m_Energy);
		m_SUI.EnergyTxts[1].color = BaseValue.GetUpDownStrColor(Mathf.Min(m_PassTicketCnt, (int)USERINFO.m_Energy.Cnt), m_Energy, "#D2533C", "#FFFFFF");
	}
	public void UseBtn(bool _all) {
		if (m_Action != null) return;
		int cnt = _all ? m_CanUseCnt : Math.Min(m_CanUseCnt, 1);

		//에너지 부족시
		if (m_Energy > 0 && USERINFO.m_Energy.Cnt < Mathf.Max(m_Energy * cnt, 1)) {
			POPUP.StartLackPop(BaseValue.ENERGY_IDX, false, (res) => {
				RefreshUI();
			});
			return;
		}
		if(m_PassTicketCnt < cnt || cnt < 1) {//소탕권이 부족합니다
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(540));
			return;
		}
		StartCoroutine(IE_CloseAction(cnt));
	}
	public void ClickClose() {
		if (m_Action != null) return;
		StartCoroutine(m_Action = IE_CloseAction(0));
	}
	IEnumerator IE_CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Close(_result);
	}
}
