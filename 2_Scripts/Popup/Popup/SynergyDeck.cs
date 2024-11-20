using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SynergyDeck : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public GameObject InfoObj;
		public Transform[] ApplyParent;

		public ScrollRect Scroll;

		public GameObject AllSynergyBtn;
		public GameObject CloseBtn;
	}
	[SerializeField]
	SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		//현재 덱정보로 캐릭터 3개의 시너지 조건 체크해서 되는건 위로 안되는건 아래로

		List<JobType> cansynergy = USERINFO.m_PlayDeck.m_Synergy;
		for(int i = 0; i < cansynergy.Count; i++) {
			Utile_Class.Instantiate(m_SUI.InfoObj, m_SUI.ApplyParent[0]).GetComponent<Item_Synergy_Info>().SetData(cansynergy[i]);
		}
		List<JobType> cantsynergy = USERINFO.m_PlayDeck.m_NotSynergy;
		for (int i = 0; i < cantsynergy.Count; i++) {
			Utile_Class.Instantiate(m_SUI.InfoObj, m_SUI.ApplyParent[1]).GetComponent<Item_Synergy_Info>().SetData(cantsynergy[i]);
		}
	}

	public GameObject GetScroll()
	{
		return m_SUI.Scroll.gameObject;
	}
	public GameObject GetAllSynergy() {
		return m_SUI.AllSynergyBtn;
	}
	public GameObject GetCloseBtn()
	{
		return m_SUI.CloseBtn;
	}

	/// <summary> 시너지 도감 버튼 </summary>
	public void ClickSynergyAll() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.SynergyDeck, 0)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Synergy_All, null, (JobType)1);
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		base.Close(Result);
	}
}
