using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Union_Option_UnionDescEdit : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public InputField Input;
		public TextMeshProUGUI Info;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Cnt;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	public int GetMaxLength()
	{
		return BaseValue.GUILD_INTRO_LENGTH;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {

		m_SUI.Input.lineType = InputField.LineType.SingleLine;
		m_SUI.Input.characterLimit = GetMaxLength();
		((Text)m_SUI.Input.placeholder).text = TDATA.GetString(6027);
		m_SUI.Input.text = USERINFO.m_Guild.Intro;
		m_SUI.Title.text = TDATA.GetString(6086);
		m_SUI.Info.text = TDATA.GetString(6027);

		OnValueChange();
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
	}

	bool CheckString() {
		bool can = true;
		if (m_SUI.Input.text.Length > GetMaxLength()) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6029));
		}
		return can;
	}

	public void OnValueChange()
	{
		m_SUI.Cnt.text = string.Format("({0}/{1})", m_SUI.Input.text.Length, m_SUI.Input.characterLimit);
	}

	public void Click_Apply() {
		if (m_Action != null) return;
		if (!CheckString()) return;

		WEB.SEND_REQ_GUILD_INFO_CHANGE((res) =>
		{
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_LENGTH:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6029));
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					break;
				}
				if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) Close((int)Union_JoinList.CloseResult.Grade);
				return;
			}
			Close(0);
		}, GUILD_INFO_CHANGE_MODE.Intro, m_SUI.Input.text);
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
