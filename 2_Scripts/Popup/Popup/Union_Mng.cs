using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public class Union_Mng : PopupBase
{
	public enum Tab
	{
		NONE = -1,
		/// <summary> 가입 요청 목록 </summary>
		REQ_MNG,
		/// <summary> 길그 설정 </summary>
		OPTION
	}
	public enum ListMode
	{
		/// <summary> 일반 </summary>
		Normal = 0,
		/// <summary> 제거 </summary>
		Remove,
	}

	[Serializable]
	public struct SReqListUI
	{
		public GameObject Active;

		public GameObject NoneActive;
		public GameObject ListActive;

		[ReName("ModeChange", "EndRemoveMode", "AllRemove")]
		public GameObject[] ActiveBtn;

		public TextMeshProUGUI MemberCnt;
		public TextMeshProUGUI Cnt;
		public ScrollRect Scroll;
		public RectTransform Prefab;

		[HideInInspector] public ListMode SelectMode;
	}

	[Serializable]
	public struct SOptionInputUI
	{
		public Text Info;
		public GameObject Placeholder;
	}
	[Serializable]
	public struct SOptionUI
	{
		public GameObject Active;

		public Image Mark;
		public SOptionInputUI Name;
		public SOptionInputUI Intro;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI JoinType;
		public ScrollRect Scroll;
	}

	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_Tab[] Tabs;

		public SReqListUI ReqMng;
		public SOptionUI Option;

	}

	[SerializeField] SUI m_SUI;

	Tab m_TabPos = Tab.NONE;
	bool IsStart;
	Action<ListMode> ModeChange;

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsStart = false;
		m_SUI.Tabs[0].SetData(0, TDATA.GetString(6057), ClickTab);
		m_SUI.Tabs[1].SetData(1, TDATA.GetString(6058), ClickTab);

		Set_ReqList_UI(true);
		base.SetData(pos, popup, cb, aobjValue);

		m_SUI.Tabs[0].OnClick();
	}

	public override void SetUI() {
		base.SetUI();
	}
	void Load_ReqList()
	{
		WEB.SEND_REQ_GUILD_REQUSER_LIST((res) => {
			Set_ReqList_UI();
			USERINFO.m_Guild.Set_Alram_ReqUser_Off();
		}, USERINFO.m_Guild.UID);
	}

	void Set_ReqList_UI(bool First = false)
	{
		m_SUI.ReqMng.Active.SetActive(true);
		m_SUI.Option.Active.SetActive(false);

		int Cnt = USERINFO.m_Guild.ReqUsers.Count;
		m_SUI.ReqMng.Cnt.text = string.Format(TDATA.GetString(6008), Cnt, BaseValue.GUILD_MAX_REQ_CNT);
		m_SUI.ReqMng.MemberCnt.text = string.Format("{0} / {1}", USERINFO.m_Guild.MemberCnt, USERINFO.m_Guild.MaxUserCnt);
		
		if (Cnt < 0 || First)
		{
			m_SUI.ReqMng.NoneActive.SetActive(true);
			m_SUI.ReqMng.ListActive.SetActive(false);
		}
		else
		{
			m_SUI.ReqMng.NoneActive.SetActive(false);
			m_SUI.ReqMng.ListActive.SetActive(true);

			UTILE.Load_Prefab_List(Cnt, m_SUI.ReqMng.Scroll.content, m_SUI.ReqMng.Prefab);
			ModeChange = null;
			for (int i = 0; i < Cnt; i++)
			{
				Item_Union_JoinMail element = m_SUI.ReqMng.Scroll.content.GetChild(i).GetComponent<Item_Union_JoinMail>();
				element.SetData(USERINFO.m_Guild.ReqUsers[i], Click_UserBtn);
				ModeChange += element.ChangeMode;
			}
		}
		Set_ReqList_Mode(First ? ListMode.Normal : m_SUI.ReqMng.SelectMode);


		USERINFO.m_Guild.Set_Alram_ReqUser_Off();
	}

	void Set_Option_UI()
	{
		m_SUI.ReqMng.Active.SetActive(false);
		m_SUI.Option.Active.SetActive(true);
		Set_Option_MarkUI();
		Set_Option_NameUI();
		Set_Option_IntroUI();
		Set_Option_LVUI();
		Set_Option_JoinTypeUI();
	}


	void Set_Option_MarkUI()
	{
		m_SUI.Option.Mark.sprite = USERINFO.m_Guild.GetGuilMark();
	}

	void Set_Option_NameUI()
	{
		if(string.IsNullOrWhiteSpace(USERINFO.m_Guild.Name))
		{
			m_SUI.Option.Name.Placeholder.SetActive(true);
			m_SUI.Option.Name.Info.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Option.Name.Placeholder.SetActive(false);
			m_SUI.Option.Name.Info.gameObject.SetActive(true);
			m_SUI.Option.Name.Info.text = USERINFO.m_Guild.Name;
		}
	}

	void Set_Option_IntroUI()
	{
		if (string.IsNullOrWhiteSpace(USERINFO.m_Guild.Intro))
		{
			m_SUI.Option.Intro.Placeholder.SetActive(true);
			m_SUI.Option.Intro.Info.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Option.Intro.Placeholder.SetActive(false);
			m_SUI.Option.Intro.Info.gameObject.SetActive(true);
			m_SUI.Option.Intro.Info.text = USERINFO.m_Guild.Intro;
		}
	}

	void Set_Option_LVUI()
	{
		m_SUI.Option.LV.text = USERINFO.m_Guild.JoinLV.ToString();
	}

	void Set_Option_JoinTypeUI()
	{
		m_SUI.Option.JoinType.text = BaseValue.GetGuildJoinType(USERINFO.m_Guild.JoinType);
	}

	void Set_ReqList_Mode(ListMode mode)
	{
		m_SUI.ReqMng.ActiveBtn[0].SetActive(mode == ListMode.Normal);
		m_SUI.ReqMng.ActiveBtn[1].SetActive(mode != ListMode.Normal);
		m_SUI.ReqMng.ActiveBtn[2].SetActive(mode != ListMode.Normal);

		m_SUI.ReqMng.SelectMode = mode;
		ModeChange?.Invoke(mode);
	}

	bool IsAlram(bool Active, Tab tab)
	{
		if (Active) return false;
		switch(tab)
		{
		case Tab.REQ_MNG: return USERINFO.m_Guild.ReqUsers.Count > 0;
		}
		return false;
	}

#region Btn

#region Request Join User Manager
	/// <summary> 탭 버튼 </summary>
	bool ClickTab(Item_Tab _tab)
	{
		Tab pos = (Tab)_tab.m_Pos;
		if (pos == m_TabPos) return false;
		for (int i = 0; i < m_SUI.Tabs.Length; i++)
		{
			bool Active = i == _tab.m_Pos;
			m_SUI.Tabs[i].SetActive(Active);
			m_SUI.Tabs[i].SetAlram(IsAlram(Active, (Tab)i));
		}

		m_TabPos = pos;
		switch (m_TabPos)
		{
		// 프로토콜 보내서 신청 리스트 다시 받는다.
		case Tab.REQ_MNG:
			m_SUI.ReqMng.Scroll.verticalNormalizedPosition = 1f;
			Load_ReqList();
			break;
		case Tab.OPTION:
			m_SUI.Option.Scroll.verticalNormalizedPosition = 1f;
			Set_Option_UI(); 
			break;
		}
		return true;
	}

	void ReLoadGuild()
	{
		USERINFO.m_Guild.LoadGuild(() =>
		{
			if (USERINFO.m_Guild.UID == 0)
			{
				USERINFO.m_Guild.Set_AlramOff();
				Close((int)Union_JoinList.CloseResult.LoadGuild);
			}
			else if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) Close((int)Union_JoinList.CloseResult.UIReset);
			// 프로토콜 보내서 신청 리스트 다시 받는다.
			else Load_ReqList();
		}, 0, true, true, true);
	}

	void SetErrorMsg(ushort result_code)
	{
		string GName = USERINFO.m_Guild.Name;
		switch (result_code)
		{
		case EResultCode.ERROR_GUILD_JOIN:
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6065));
			Load_ReqList();
			break;
		case EResultCode.ERROR_GUILD_GRADE:			// 마스터 아님
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
			ReLoadGuild();
			break;
		case EResultCode.ERROR_GUILD_NOT_MEMBER:	// 강퇴당함(마스터 이전후 강퇴됨)
			ReLoadGuild();
			break;
		case EResultCode.ERROR_GUILD_MAX_MEMBER:	// 최대치 오버
			ReLoadGuild();
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6121));
			break;
		default:
			WEB.StartErrorMsg(result_code, (btn, obj) => {
				ReLoadGuild();
			});
			break;
		}
	}

	public void Click_UserBtn(RES_GUILD_REQUSER User)
	{
		if (!IsStart) return;
		if (USERINFO.m_Guild.ReqUsers.Count < 1) return;
		string GName = USERINFO.m_Guild.Name;
		switch (m_SUI.ReqMng.SelectMode)
		{
		case ListMode.Normal:
			if (USERINFO.m_Guild.MemberCnt >= USERINFO.m_Guild.MaxUserCnt)
			{
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6121));
				return;
			}
			// 승인
			WEB.SEND_REQ_GUILD_REQUSER_APPLY((res) =>
			{
				if (!res.IsSuccess())
				{
					SetErrorMsg(res.result_code);
					return;
				}
				// 유저 제거
				USERINFO.m_Guild.ReqUsers.Remove(User);
				// 리스트 갱신
				Set_ReqList_UI();
			}, User.UserNo);
			break;
		case ListMode.Remove:
			// 거절
			WEB.SEND_REQ_GUILD_REQUSER_REJECT((res) =>
			{
				if (!res.IsSuccess())
				{
					SetErrorMsg(res.result_code);
					return;
				}
				// 유저 제거
				USERINFO.m_Guild.ReqUsers.Remove(User);
				// 리스트 갱신
				Set_ReqList_UI();
			}, new List<long>() { User.UserNo });
			break;
		}
	}

	public void Click_RemoveAllUser()
	{
		if (!IsStart) return;
		if (USERINFO.m_Guild.ReqUsers.Count < 1) return;
		WEB.SEND_REQ_GUILD_REQUSER_REJECT((res) =>
		{
			if (!res.IsSuccess())
			{
				SetErrorMsg(res.result_code);
				return;
			}
			// 프로토콜 보내서 신청 리스트 다시 받는다.
			Load_ReqList();
		}, USERINFO.m_Guild.ReqUsers.Select(o => o.UserNo).ToList());
	}
	public void Click_SetRemoveMode()
	{
		if (!IsStart) return;
		Set_ReqList_Mode(ListMode.Remove);
	}

	public void Click_CloseRemoveMode()
	{
		if (!IsStart) return;
		Set_ReqList_Mode(ListMode.Normal);
	}
#endregion
#region Option
	public void Click_ChangeMark()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_MarkEdit, (result, obj) => {
			if (result == (int)Union_JoinList.CloseResult.Grade)
			{
				Close(result);
				return;
			}
			Set_Option_MarkUI();
		}, USERINFO.m_Guild.Icon, Union_Option_MarkEdit.Mode.Option);
	}
	public void Click_ChangeName()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_NameChange, (result, obj) => {

			if (result == (int)Union_JoinList.CloseResult.Grade)
			{
				Close(result);
				return;
			}
			Set_Option_NameUI();
		});
	}
	public void Click_ChangeIntro()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_UnionDescEdit, (result, obj) => {
			if (result == (int)Union_JoinList.CloseResult.Grade)
			{
				Close(result);
				return;
			}
			Set_Option_IntroUI();
		});
	}
	public void Click_ChangeLV()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_JoinLevelEdit, (result, obj) => {
			if(result == (int)Union_JoinList.CloseResult.Grade)
			{
				Close(result);
				return;
			}
			Set_Option_LVUI();
		});
	}
	public void Click_ChangeJoinType()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_JoinTypeEdit, (result, obj) => {
			if(result != (int)USERINFO.m_Guild.JoinType)
			{
				WEB.SEND_REQ_GUILD_INFO_CHANGE((res) =>
				{
					if (!res.IsSuccess())
					{
						SetErrorMsg(res.result_code);
						if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) Close((int)Union_JoinList.CloseResult.Grade);
						return;
					}
					Set_Option_JoinTypeUI();
				}, GUILD_INFO_CHANGE_MODE.JoinType, result.ToString());
			}

		});
	}
	public void Click_Destroy()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Option_Disband, (result, obj) => {
			if (result == 1) SEND_Destroy();
		});
	}

	void SEND_Destroy()
	{
		WEB.SEND_REQ_GUILD_DESTROY((res) => {
			if (!res.IsSuccess())
			{
				SetErrorMsg(res.result_code);
				if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) Close((int)Union_JoinList.CloseResult.Grade);
				return;
			}
			PlayEffSound(SND_IDX.SFX_1503);
			Close((int)Union_JoinList.CloseResult.Destroy);
		});
	}

	#endregion

	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	} 
#endregion
}
