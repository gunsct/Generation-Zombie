using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Msg_PVP_Research_LvUp : MsgBoxBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public TextMeshProUGUI[] Lv;
		public TextMeshProUGUI Desc;
	}
	[SerializeField] SUI m_SUI;
	ResearchInfo m_Info;
	//IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (ResearchInfo)aobjValue[0];
		StartCoroutine(IE_AutoClose());
		PlayEffSound(SND_IDX.SFX_0105);

		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		TResearchTable tdata = m_Info.m_TData;
		m_SUI.Icon.sprite = tdata.GetIcon();
		m_SUI.Lv[0].text = m_SUI.Lv[1].text = string.Format("Lv.{0}", m_Info.m_GetLv);
		m_SUI.Desc.text = tdata.GetInfo();
		base.SetUI();
	}
	IEnumerator IE_AutoClose() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Close();
	}
	//public override void Close(int Result = 0) {
	//	if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
	//	if (m_Action != null) return;
	//	m_Action = CloseAction(Result);
	//	StartCoroutine(m_Action);
	//}
	//IEnumerator CloseAction(int _result) {
	//	m_SUI.Anim.SetTrigger("Close");
	//	yield return new WaitForEndOfFrame();
	//	yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
	//	base.Close(_result);
	//}
}
