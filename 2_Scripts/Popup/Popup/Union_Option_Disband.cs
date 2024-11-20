using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Union_Option_Disband : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public InputField Input;
		public TextMeshProUGUI Msg;
		public Item_Button Btn;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {

		int MaxLV = BaseValue.CHAR_MAX_LV;
		m_SUI.Msg.text = string.Format(TDATA.GetString(6092), USERINFO.m_Guild.Name);
		m_SUI.Input.text = "";
		BtnCheck();
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
	}

	void BtnCheck()
	{
		m_SUI.Btn.SetBG(!USERINFO.m_Guild.Name.Equals(m_SUI.Input.text) ? UIMng.BtnBG.Not : UIMng.BtnBG.Red);
	}

	public void OnEditEnd()
	{
		BtnCheck();
	}

	public void Click_Apply() {
		if (m_Action != null) return;
		if(!USERINFO.m_Guild.Name.Equals(m_SUI.Input.text)) return;
		View_Destroy_Notice_POPUP();
	}

	private void View_Destroy_Notice_POPUP()
	{
		POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, TDATA.GetString(6091), TDATA.GetString(6093), (result, obj) => {
			Close(result);
		}, TDATA.GetString(11), TDATA.GetString(6091));
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
