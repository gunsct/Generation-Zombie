using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Union_Option_JoinLevelEdit : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Slider slider;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Info;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		PlayEffSound(SND_IDX.SFX_0141);

		int MaxLV = BaseValue.CHAR_MAX_LV;
		m_SUI.Info.text = string.Format(TDATA.GetString(6175), MaxLV);
		m_SUI.slider.value = USERINFO.m_Guild.JoinLV;
		m_SUI.slider.minValue = 1;
		m_SUI.slider.maxValue = MaxLV;
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		OnValueChange();
	}

	public void OnValueChange()
	{
		m_SUI.LV.text = m_SUI.slider.value.ToString();
	}

	public void Click_Add(int add)
	{
		m_SUI.slider.value += add;
	}

	public void Click_Apply() {
		if (m_Action != null) return;

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
		}, GUILD_INFO_CHANGE_MODE.JoinLV, m_SUI.slider.value.ToString());
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
