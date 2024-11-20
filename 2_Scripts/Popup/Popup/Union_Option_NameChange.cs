using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Union_Option_NameChange : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public InputField Name;
		public TextMeshProUGUI Cnt;
		public Item_Store_Buy_Button Btn;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {

		m_SUI.Name.characterLimit = BaseValue.NICKNAME_LENGTH;
		m_SUI.Name.text = USERINFO.m_Guild.Name;
		m_SUI.Btn.SetData(BaseValue.SHOP_IDX_UNION_NAME_CHANGE);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
	}
	bool CheckName() {
		bool can = true;
		if (m_SUI.Name.text.Length < 1) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(925));
		}
		else if (m_SUI.Name.text.Length > BaseValue.NICKNAME_LENGTH) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(516));
		}
		else if (Utile_Class.IS_SpecialChar(m_SUI.Name.text)) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
		}
		else if (m_SUI.Name.text.Equals(USERINFO.m_Guild.Name))
		{
			can = false;
			//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
		}
		//이름 이미 있는지 체크

		return can;
	}

	public void OnValueChange()
	{
		m_SUI.Cnt.text = string.Format("({0}/{1})", m_SUI.Name.text.Length, m_SUI.Name.characterLimit);
	}

	public void OnEditEnd()
	{
		// 공백 제거
		if(m_SUI.Name.text.IndexOf(" ") > -1) m_SUI.Name.text = m_SUI.Name.text.Replace(" ", "");
	}

	public void ClickBuy() {
		if (m_Action != null) return;
		if (!CheckName()) return;
		if (!m_SUI.Btn.CheckLack()) return;

		WEB.SEND_REQ_GUILD_INFO_CHANGE((res) =>
		{
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_LENGTH:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(516));
					break;
				case EResultCode.ERROR_NICKNAME:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6029));
					break;
				case EResultCode.ERROR_USED_NAME:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6011));
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					break;
				}
				if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) Close((int)Union_JoinList.CloseResult.Grade);
				return;
			}
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6085));
			Close(0);
		}, LS_Web.GUILD_INFO_CHANGE_MODE.Name, m_SUI.Name.text);
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
