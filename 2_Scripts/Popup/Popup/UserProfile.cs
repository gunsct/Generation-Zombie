using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class UserProfile : PopupBase
{
	public enum State
	{
		Main,
		Portrait,
		Name,
		End
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject[] Btns;//다음-뒤로-완료
		public Item_UserProfile_Portrait Portrait;
		public GameObject PortraitPrefab;
		public Transform Bucket;
		public InputField Name;
		public GameObject PortraitBtn;
		public GameObject NameBtn;
		public Animator[] GenderCkeckAnim;//남-여
		public TextMeshProUGUI[] ConfirmBtnTxt;
		public GameObject ConfirmPriceGroup;
		public TextMeshProUGUI ConfirmPriceTxt;
		public Button ConfirmBtn;
		public GameObject[] ConfirmBtnIcon;
		public Image NameBG;
		public Color[] NameBGColor;
	}
	[SerializeField] SUI m_SUI;
	State m_State;
	bool m_First = false;
	bool[] m_IsConfirm = new bool[2];//0:초상화,1:이름
	TUserProfileImageTable m_TData { get { return TDATA.GetUserProfileImageTable(USERINFO.m_Profile); } }
	Item_UserProfile_Portrait m_PreSelect;

	SND_IDX m_PreBG;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_PreBG = SND.GetNowBG;
		SND.PlayBgSound(SND_IDX.BGM_0053);

		m_SUI.Name.characterLimit = BaseValue.NICKNAME_LENGTH;

		m_First = (bool)aobjValue[0];

		PlayEffSound(SND_IDX.SFX_9600);

		m_SUI.ConfirmBtnTxt[0].text = m_SUI.ConfirmBtnTxt[1].text = m_First ? TDATA.GetString(199) : TDATA.GetString(517);
		m_SUI.ConfirmPriceGroup.SetActive(!m_First);
		SetConfirmBtn();

		for (int i = 0; i < TDATA.GetUserProfileImageList().Count; i++) {
			if (TDATA.GetUserProfileImageList()[i].m_Idx == 0) continue;
			Item_UserProfile_Portrait portrait = Utile_Class.Instantiate(m_SUI.PortraitPrefab, m_SUI.Bucket).GetComponent<Item_UserProfile_Portrait>();
			portrait.SetData(TDATA.GetUserProfileImageList()[i], (res) => {
				if (m_PreSelect != null && m_PreSelect != res) m_PreSelect.SetAnim(Item_UserProfile_Portrait.State.Not);
				m_PreSelect = res;
				SetPortait(m_PreSelect.m_TData);
			});
			if (portrait.m_TData == m_TData) {
				m_PreSelect = portrait;
				m_PreSelect.SetAnim(Item_UserProfile_Portrait.State.Select);
			}
			//if (!m_First && m_TData.m_Idx == TDATA.GetUserProfileImageList()[i].m_Idx) portrait.SetAnim(Item_UserProfile_Portrait.State.Select);
		}
		if (m_PreSelect != null) SetPortait(m_PreSelect.m_TData);
		else SetPortait(m_TData);
		StartCoroutine(StartAnim());
	}
	void SetPortait(TUserProfileImageTable _tdata) {
		m_SUI.Portrait.SetData(_tdata, null);
		m_SUI.Portrait.SetAnim(Item_UserProfile_Portrait.State.In);
		GenderType gender = _tdata.m_Gender;
		m_SUI.GenderCkeckAnim[0].SetTrigger(gender == GenderType.Male && m_PreSelect != null ? "Select" : "Not");
		m_SUI.GenderCkeckAnim[1].SetTrigger(gender == GenderType.Female && m_PreSelect != null ? "Select" : "Not");
	}
	public void SetNameBGColor() {
		if (m_SUI.Name.text.Equals(string.Empty))
			m_SUI.NameBG.color = m_SUI.NameBGColor[0];
		else
			m_SUI.NameBG.color = m_SUI.NameBGColor[1];
	}
	void SetConfirmBtn() {
		int changeticketcnt = USERINFO.GetItemCount(BaseValue.NAMECHANGETICKET_IDX);
		if (changeticketcnt > 0) {
			m_SUI.ConfirmPriceTxt.text = "1";
			//m_SUI.ConfirmBtn.interactable = m_First || changeticketcnt > 0;
			m_SUI.ConfirmBtnIcon[0].SetActive(true);
			m_SUI.ConfirmBtnIcon[1].SetActive(false);
		}
		else {
			m_SUI.ConfirmPriceTxt.text = TDATA.GetShopTable(BaseValue.NAMECHANGE_SHOP_IDX).GetPrice().ToString();
			m_SUI.ConfirmPriceTxt.color = BaseValue.GetUpDownStrColor(USERINFO.m_Cash, TDATA.GetShopTable(120).GetPrice(), "#D2533C", "#FFFFFF");
			m_SUI.ConfirmBtnIcon[0].SetActive(false);
			m_SUI.ConfirmBtnIcon[1].SetActive(true);
		}
	}
	IEnumerator StartAnim() {
		m_SUI.Anim.SetTrigger(m_First ? "Start_First" : "LaterStart");

		if (m_First) {
			yield return new WaitForEndOfFrame();
			if (m_PreSelect == null) m_SUI.Portrait.SetAnim(Item_UserProfile_Portrait.State.Out);
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

			SetAnim(State.Portrait);

			yield return new WaitWhile(() => !m_IsConfirm[0]);

			SetAnim(State.Name);

			yield return new WaitWhile(() => !m_IsConfirm[1]);

			m_SUI.Anim.SetTrigger("End");

			yield return new WaitForEndOfFrame();

			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 10f / 289f));
			PlayEffSound(SND_IDX.SFX_9602);
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 122f / 289f));
			PlayEffSound(SND_IDX.SFX_9603);
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
			yield return new WaitWhile(() => POPUP.IS_Connecting());

			Close();
		}
		else {
			m_SUI.Name.text = USERINFO.m_Name;
		}
	}
	public void ClickConfirm(int _pos) {
		if (_pos == 1 && !CheckName()) return;
		if (_pos == 0 && m_PreSelect == null) return;
		PlayEffSound(SND_IDX.SFX_9601);
		if (!m_First) {
			if (_pos == 0) {
#if NOT_USE_NET
				USERINFO.m_Profile = m_PreSelect.m_TData.m_Idx;
				MAIN.Save_UserInfo();
				SetAnim(State.Main); 
				m_IsConfirm[_pos] = true;
#else
				WEB.SEND_REQ_USER_PROFILE_SET((res) => { 
					SetAnim(State.Main); 
					m_IsConfirm[_pos] = true;
				}, m_PreSelect.m_TData.m_Idx);
#endif
			}
			else if (_pos == 1) {
				if (USERINFO.GetItemCount(BaseValue.NAMECHANGETICKET_IDX) < 1) {
					if (USERINFO.m_Cash < TDATA.GetShopTable(BaseValue.NAMECHANGE_SHOP_IDX).GetPrice()) {
						POPUP.StartLackPop(BaseValue.CASH_IDX, false, (res) => { SetConfirmBtn(); });
					}
					else {
						POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, TDATA.GetString(531), TDATA.GetString(821), (res, obj) => {
							if (res == 1) {
								PlayEffSound(SND_IDX.SFX_9603);
#if NOT_USE_NET
								SetConfirmBtn();
								USERINFO.GetCash(-TDATA.GetShopTable(BaseValue.NAMECHANGE_SHOP_IDX).GetPrice());
								USERINFO.m_Name = m_SUI.Name.text;
								MAIN.Save_UserInfo();
								SetAnim(State.Main);
								m_IsConfirm[_pos] = true;
#else
								WEB.SEND_REQ_USER_NICKNAME_SET((res) => {
									if (!res.IsSuccess())
									{
										switch (res.result_code)
										{
										case EResultCode.ERROR_NICKNAME:
											if (m_SUI.Name.text.Length < 1) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(524));
											else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
											break;
										default:
											WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
											break;
										}
										return;
									}
									SetAnim(State.Main);
									SetConfirmBtn();
									m_IsConfirm[_pos] = true;
								}, m_SUI.Name.text, 0);
#endif
							}
						}, PriceType.Cash, BaseValue.CASH_IDX, TDATA.GetShopTable(BaseValue.NAMECHANGE_SHOP_IDX).GetPrice());
					}
					return;
				}
				POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(531), TDATA.GetString(533), (res, obj)=> {
					if (res == 1) {
					PlayEffSound(SND_IDX.SFX_9603);
#if NOT_USE_NET
					SetConfirmBtn();
					USERINFO.DeleteItem(BaseValue.NAMECHANGETICKET_IDX, 1);
					USERINFO.m_Name = m_SUI.Name.text;
					MAIN.Save_UserInfo();
					SetAnim(State.Main);
						m_IsConfirm[_pos] = true;
#else
						WEB.SEND_REQ_USER_NICKNAME_SET((res) => {
							if (!res.IsSuccess())
							{
								switch (res.result_code)
								{
								case EResultCode.ERROR_NICKNAME:
									if (m_SUI.Name.text.Length < 1) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(524));
									else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
									break;
								default:
									WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
									break;
								}
								return;
							}
							SetAnim(State.Main);
							SetConfirmBtn();
							m_IsConfirm[_pos] = true;
						}, m_SUI.Name.text, 1);
#endif
					}
				});
			}
		}
		else {
			if(_pos == 0) {
				//TODO:무료 초상화 변경 프로토콜
#if NOT_USE_NET
				USERINFO.m_Profile = m_PreSelect.m_TData.m_Idx;
				MAIN.Save_UserInfo();
				SetAnim(State.Name);
				m_IsConfirm[_pos] = true;
#else
				WEB.SEND_REQ_USER_PROFILE_SET((res) => { 
					SetAnim(State.Name);
					m_IsConfirm[_pos] = true;
				}, m_PreSelect.m_TData.m_Idx);
#endif
			}
			else if(_pos == 1) {
				//TODO:무료 닉네임 변경 프로토콜
#if NOT_USE_NET
				USERINFO.m_Name = m_SUI.Name.text;
				MAIN.Save_UserInfo(); 
				m_IsConfirm[_pos] = true;
#else
				WEB.SEND_REQ_USER_NICKNAME_SET((res) => {
					if(!res.IsSuccess())
					{
						switch(res.result_code)
						{
						case EResultCode.ERROR_NICKNAME:
							if (m_SUI.Name.text.Length < 1) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(524));
							else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
							break;
						default:
							WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
							break;
						}
						return;
					}
					m_IsConfirm[_pos] = true;
				}, m_SUI.Name.text, 100);
#endif
			}
		}
	}
	/// <summary> 중복가능, 특문, 공백 불가, 20자 이하</summary>
	bool CheckName() {
		bool can = true;
		if(m_SUI.Name.text.Length < 1)
		{
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(524));
		}
		else if (m_SUI.Name.text.Length > BaseValue.NICKNAME_LENGTH)
		{
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(516));
		}
		else if (Utile_Class.IS_SpecialChar(m_SUI.Name.text)) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(515));
		}
		else if (m_SUI.Name.text.Equals(USERINFO.m_Name)) {
			can = false;
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(534));
		}

		return can;
	}
	public void ClickBack() {//프로필은 기본으로 이름도 원래거로
		switch (m_State) {
			case State.Portrait:
				m_IsConfirm[0] = false;
				SetPortait(m_TData);
				SetAnim(State.Main);
				break;
			case State.Name:
				m_IsConfirm[1] = false;
				m_SUI.Name.text = USERINFO.m_Name;
				SetAnim(m_First ? State.Portrait : State.Main);
				break;
			default:return;
		}
		SetAnim(m_State);
	}
	public void ClickPortrait() {
		SetAnim(State.Portrait);
	}
	public void ClickName() {
		SetAnim(State.Name);
	}
	void SetAnim(State _state) {
		if (m_State == _state) return;
		switch (_state) {
			case State.Main:
				m_SUI.Anim.SetTrigger("LaterMain");
				m_State = State.Main;
				break;
			case State.Portrait: 
				m_SUI.Anim.SetTrigger("1_Portrait");
				m_State = State.Portrait;
				break;
			case State.Name: 
				m_SUI.Anim.SetTrigger("2_Username");
				m_State = State.Name; 
				break;
			case State.End: 
				m_SUI.Anim.SetTrigger("End");
				m_State = State.End; 
				break;
		}
		if (m_First) {
			m_SUI.Btns[0].SetActive(_state == State.Portrait);
			m_SUI.Btns[1].SetActive(_state != State.Portrait);
			m_SUI.Btns[2].SetActive(_state != State.Portrait);
		}
		else {
			m_SUI.Btns[0].SetActive(false);
			m_SUI.Btns[1].SetActive(true);
			m_SUI.Btns[2].SetActive(true);
		}
	}
	public override void Close(int Result = 0) {
		SND.PlayBgSound(m_PreBG);
		base.Close(Result);
	}
}
