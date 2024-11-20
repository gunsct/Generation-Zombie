using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

public class Union_NewUnion : PopupBase
{
	[Serializable]
	public struct SInputUI
	{
		public InputField Input;
		public TextMeshProUGUI Cnt;
	}

	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Mark;
		public SInputUI Name;
		public SInputUI Intro;
		public Image Nation;
		public Item_Store_Buy_Button StoreBtn;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	public RES_GUILDINFO_SIMPLE MyJoin;
	int m_MarkIdx = BaseValue.ITEM_IDX_UNION_BASE_MARK;
	TShopTable m_TSData { get { return TDATA.GetShopTable(BaseValue.SHOP_IDX_UNION_MAKE); } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		MyJoin = aobjValue[0] != null ? (RES_GUILDINFO_SIMPLE)aobjValue[0] : null;
		m_SUI.Name.Input.characterLimit = BaseValue.NICKNAME_LENGTH;
		m_SUI.Intro.Input.characterLimit = BaseValue.GUILD_INTRO_LENGTH;
		m_SUI.StoreBtn.SetData(m_TSData.m_Idx);
		SetMark(BaseValue.ITEM_IDX_UNION_BASE_MARK);
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();
		m_SUI.Nation.sprite = BaseValue.GetNationIcon(USERINFO.m_Nation);
	}
	public void ClickChangeMark() {
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_MarkEdit, (result, obj) => { if(result != 0) SetMark(result); }, m_MarkIdx, Union_Option_MarkEdit.Mode.Create);
	}
	void SetMark(int _idx) {
		if (_idx < 1) return;
		m_MarkIdx = _idx;
		m_SUI.Mark.sprite = TDATA.GetGuideMark(_idx);
	}

	public void OnNameValueChange()
	{
		m_SUI.Name.Cnt.text = string.Format("({0}/{1})", m_SUI.Name.Input.text.Length, m_SUI.Name.Input.characterLimit);
	}

	public void OnIntroValueChange()
	{
		m_SUI.Intro.Cnt.text = string.Format("({0}/{1})", m_SUI.Intro.Input.text.Length, m_SUI.Intro.Input.characterLimit);
	}
	public void OnNameEnd()
	{
		// 공백 제거
		if (m_SUI.Name.Input.text.IndexOf(" ") > -1) m_SUI.Name.Input.text = m_SUI.Name.Input.text.Replace(" ", "");
	}

	bool CheckName() {
		bool can = true;
		if (m_SUI.Name.Input.text.Length < 1) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(925));
		}
		else if (m_SUI.Name.Input.text.Length > BaseValue.NICKNAME_LENGTH) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(516));
		}
		else if (Utile_Class.IS_SpecialChar(m_SUI.Name.Input.text)) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
		}
		//이름 이미 있는지 체크

		return can;
	}

	bool CheckCreate()
	{
		var grtime = USERINFO.m_GRTime + 24 * 60 * 60 * 1000L;
		if (grtime > UTILE.Get_ServerTime_Milli())
		{
			POPUP.Set_MsgBox(PopupName.Msg_Timer, string.Empty, string.Empty, (result, obj) => {}, 6118, grtime, Utile_Class.TimeStyle.hh_mm);
			return false;
		}
		// 가입 신청 중인 연합 확인
		if (MyJoin != null && MyJoin.UID != 0)
		{
			POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, string.Empty, string.Format(TDATA.GetString(6044), MyJoin.Name), (result, obj) => {
				if (result == 1)
				{
					WEB.SEND_REQ_CANCEL_GUILD_JOIN((res) => {
						if (!res.IsSuccess())
						{
							switch (res.result_code)
							{
							case EResultCode.ERROR_GUILD_JOIN:
								// 이미 가입된 길드가 있음
								Close((int)Union_JoinList.CloseResult.LoadGuild);
								POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6154));
								break;
							default:
								WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
								break;
							}
							return;
						}
						else MyJoin = new RES_GUILDINFO_SIMPLE();
					});
				}
			}, TDATA.GetString(288), TDATA.GetString(6064));
			return false;
		}
		if (!CheckName()) return false;
		return true;
	}

	public void ClickMake() {
		if (m_Action != null) return;
		if (!CheckCreate()) return;
		//창설 비용 체크 먼저
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(6013), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					PlayEffSound(SND_IDX.SFX_1502);
					WEB.SEND_REQ_GUILD_CREATE((res) => {
						if (!res.IsSuccess())
						{
							switch (res.result_code)
							{
							case EResultCode.ERROR_NICKNAME:
								POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6029));
								break;
							case EResultCode.ERROR_USED_NAME:
								POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6011));
								break;
							case EResultCode.ERROR_GUILD_INTRO_LENGTH:
								POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6029));
								break;
							case EResultCode.ERROR_GUILD_JOIN_TIME:
								WEB.SEND_REQ_USERINFO((ures) => {
									USERINFO.SetDATA(ures);
									var grtime = USERINFO.m_GRTime + 24 * 60 * 60 * 1000L;
									POPUP.Set_MsgBox(PopupName.Msg_Timer, string.Empty, string.Empty, (result, obj) => { }, 6118, grtime, Utile_Class.TimeStyle.hh_mm);
								}, USERINFO.m_UID);
								break;
							case EResultCode.ERROR_GUILD_JOIN:
								// 이미 가입된 길드가 있음
								Close((int)Union_JoinList.CloseResult.LoadGuild);
								POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6154));
								break;
							default:
								WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
								break;
							}
							return;
						}
						Close((int)Union_JoinList.CloseResult.Success);
						POPUP.Set_MsgBox(PopupName.Msg_NewUnion_Result, string.Empty, string.Empty, (result, obj) => { });
							
					}, m_MarkIdx, m_SUI.Name.Input.text, m_SUI.Intro.Input.text);
				}
				else {
					POPUP.StartLackPop(m_TSData.GetPriceIdx(), true);
				}
			}
		}, m_TSData.m_PriceType, m_TSData.m_PriceIdx, m_TSData.GetPrice(), false);
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
