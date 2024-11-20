using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;
using static Item_Union_Member_Element;

public partial class Union_Main : PopupBase
{
	public enum MainUITab
	{
		NONE = -1,
		/// <summary> 활동 </summary>
		INFO,
		/// <summary> 맴버 </summary>
		MEMBER
	}
	public enum MemberListMode
	{
		/// <summary> 일반 </summary>
		Normal = 0,
		/// <summary> 제거 </summary>
		Mng,
	}

	[Serializable]
	public struct SMainUI
	{
		public GameObject Active;

		[ReName("길드 관리", "마스터 위임 받기", "공지 수정")]
		public GameObject[] Btns;

		[ReName("활동", "맴버")]
		public Item_Tab[] Tabs;
		public SInfoUI Info;
		public SMemberUI Member;
	}

	[Serializable]
	public struct SInfoUI
	{
		public GameObject Active;

		public Image Mark;
		public Image Nation;
		public Text Name;
		public Text Master;

		public TextMeshProUGUI LV;
		public TextMeshProUGUI Exp;
		public Image ExpGauge;
		public TextMeshProUGUI Cnt;

		public Text Notice;
		public GameObject NoticeEmpty;

		public SRaidUI Raid;
		public SResUI Res;

	}

	[Serializable]
	public struct SRaidUI
	{
		public GameObject Active;
		public Image BG;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Time;
	}

	[Serializable]
	public struct SResUI
	{
		public GameObject Active;
		public Image BG;
		public TextMeshProUGUI Cnt;
	}

	[Serializable]
	public struct SMemberUI
	{
		public GameObject Active;

		public GameObject MngBtn;
		public Item_SortingGroup Sort;

		public ScrollRect Scroll;
		public RectTransform Prefab;

		[HideInInspector] public MemberListMode MngMode;
	}

	MainUITab m_MainUITabPos = MainUITab.NONE;

	Action<MemberListMode> m_MemberModeChange;

	void Reset_MainUI()
	{
		Set_Main_UI();
		Set_Info_UI();
		if (m_MainUITabPos == MainUITab.MEMBER) Set_Member_UI();
	}

	void Set_Main_UI()
	{
		m_SUI.Main.Active.SetActive(true);
		m_SUI.Shop.Active.SetActive(false);

		m_SUI.Main.Btns[0].SetActive(USERINFO.m_Guild.MyGrade() == GuildGrade.Master);
		m_SUI.Main.Btns[1].SetActive(USERINFO.m_Guild.MyGrade() == GuildGrade.Spare_Master);
		m_SUI.Main.Btns[2].SetActive(USERINFO.m_Guild.MyGrade() == GuildGrade.Master);
	}

#region Info

	void Set_Info_UI()
	{
		m_SUI.Main.Active.SetActive(true);
		m_SUI.Shop.Active.SetActive(false);
		m_SUI.Main.Info.Active.SetActive(true);
		m_SUI.Main.Member.Active.SetActive(false);

		var info = USERINFO.m_Guild;

		m_SUI.Main.Info.Mark.sprite = USERINFO.m_Guild.GetGuilMark();
		m_SUI.Main.Info.Name.text = info.Name;
		m_SUI.Main.Info.Master.text = info.Master.Name;
		m_SUI.Main.Info.Nation.sprite = BaseValue.GetNationIcon(info.Nation);
		int LV = 1;
		long Exp = 0;
		info.Calc_Exp(out LV, out Exp);
		m_SUI.Main.Info.LV.text = string.Format("Lv.{0}", LV.ToString());
		var texp = TDATA.GetGuild_ExpTable(LV);
		m_SUI.Main.Info.Exp.text = string.Format("{0}/{1}", Utile_Class.CommaValue(Exp), Utile_Class.CommaValue(texp.m_EXP));
		m_SUI.Main.Info.ExpGauge.fillAmount = (float)Exp / (float)texp.m_EXP;
		m_SUI.Main.Info.Cnt.text = string.Format("{0}/{1}", info.MemberCnt, info.MaxUserCnt);

		Set_Option_NoticeUI();

		Set_RaidUI();
		Set_ResUI();
	}


	void Set_Option_NoticeUI()
	{
		if (USERINFO.m_Guild.Notice.Length > 0)
		{
			m_SUI.Main.Info.NoticeEmpty.SetActive(false);
			m_SUI.Main.Info.Notice.gameObject.SetActive(true);
		}
		else
		{
			m_SUI.Main.Info.NoticeEmpty.SetActive(true);
			m_SUI.Main.Info.Notice.gameObject.SetActive(false);
		}
		m_SUI.Main.Info.Notice.text = USERINFO.m_Guild.Notice;
	}

	void Set_RaidUI()
	{
		m_SUI.Main.Info.Raid.Active.SetActive(false);
	}

	void Set_ResUI()
	{
		if (USERINFO.m_Guild.ResIdx == 0)
		{
			m_SUI.Main.Info.Res.Active.SetActive(false);
		}
		else
		{
			m_SUI.Main.Info.Res.Active.SetActive(true);
			var tdata = TDATA.GetGuildRes(USERINFO.m_Guild.ResIdx);
			m_SUI.Main.Info.Res.Cnt.text = string.Format("{0}/{1}", USERINFO.m_Guild.ResExp, tdata.m_Mat.m_Count);
		}

	} 
#endregion

#region Member
	void Set_Member_UI()
	{
		m_SUI.Main.Active.SetActive(true);
		m_SUI.Shop.Active.SetActive(false);

		m_SUI.Main.Info.Active.SetActive(false);
		m_SUI.Main.Member.Active.SetActive(true);

		m_SUI.Main.Member.MngBtn.SetActive(USERINFO.m_Guild.MyGrade() == GuildGrade.Master);

		m_SUI.Main.Member.Sort.SetData(SetSort, Item_SortingGroup.Mode.UnionMember);


		SetSort();
	}

	void LoadMemberList()
	{
		int Max = USERINFO.m_Guild.Users.Count;

		UTILE.Load_Prefab_List(Max, m_SUI.Main.Member.Scroll.content, m_SUI.Main.Member.Prefab);

		m_MemberModeChange = null;
		for (int i = 0; i < Max; i++)
		{
			Item_Union_Member_Element element = m_SUI.Main.Member.Scroll.content.GetChild(i).GetComponent<Item_Union_Member_Element>();
			element.SetData(USERINFO.m_Guild.Users[i], Click_MemberBtn);
			m_MemberModeChange += element.ChangeMode;
		}

		m_MemberModeChange?.Invoke(m_SUI.Main.Member.MngMode);
	}

	void SetSort()
	{
		var Sorting = m_SUI.Main.Member.Sort;
		switch (Sorting.m_Condition)
		{
		case SortingType.Name:
			USERINFO.m_Guild.Users.Sort((befor, after) =>
			{
				if (!befor.Name.Equals(after.Name)) return after.Name.CompareTo(befor.Name);
				return befor.UserNo.CompareTo(after.UserNo);
			});
			break;
		case SortingType.CombatPower:
			USERINFO.m_Guild.Users.Sort((befor, after) =>
			{
				long aftercp = after.Power;
				long beforcp = befor.Power;
				if (beforcp != aftercp) return aftercp.CompareTo(beforcp);
				return befor.UserNo.CompareTo(after.UserNo);
			});
			break;
		case SortingType.Point:
			USERINFO.m_Guild.Users.Sort((befor, after) =>
			{
				if (after.Point != befor.Point) return after.Point.CompareTo(befor.Point);
				return befor.UserNo.CompareTo(after.UserNo);
			});
			break;
		case SortingType.Time:
			USERINFO.m_Guild.Users.Sort((befor, after) =>
			{
				// 시간은 최근 접속시간으로 표시가되므로 반대로 생각해야됨
				if (after.UTime != befor.UTime) return befor.UTime.CompareTo(after.UTime);
				return befor.UserNo.CompareTo(after.UserNo);
			});
			break;
		}

		if (Sorting.m_Ascending) USERINFO.m_Guild.Users.Reverse();


		LoadMemberList();
	} 
#endregion

#region Btn
	#region Info
	/// <summary> 탭 버튼 </summary>
	bool ClickTab(Item_Tab _tab)
	{
		Tab pos = (Tab)_tab.m_Pos;
		if (pos == m_TabPos) return false;
		for (int i = 0; i < m_SUI.Tabs.Length; i++)
		{
			bool Active = i == _tab.m_Pos;
			m_SUI.Tabs[i].SetActive(Active);
			m_SUI.Tabs[i].SetAlram(IsAlram(Active, (Tab)i) && (i == 0 ? true : USERINFO.m_Guild.UID != 0));
		}
		//기여도 구매한 경우 고려
		if(m_TabPos == Tab.SHOP && pos == Tab.MAIN) {
			Set_Info_UI();
			if (m_MainUITabPos == MainUITab.MEMBER) Set_Member_UI();
		}
		m_TabPos = pos;
		switch (m_TabPos)
		{
		// 프로토콜 보내서 신청 리스트 다시 받는다.
		case Tab.MAIN:
			Set_Main_UI();
			if (m_MainUITabPos == MainUITab.NONE) m_SUI.Main.Tabs[0].OnClick();
			break;
		case Tab.SHOP: Set_Shop_UI(); break;
		}
		return true;
	}


	bool ClickMainTab(Item_Tab _tab)
	{
		MainUITab pos = (MainUITab)_tab.m_Pos;
		if (pos == m_MainUITabPos) return false;
		for (int i = 0; i < m_SUI.Main.Tabs.Length; i++)
		{
			bool Active = i == _tab.m_Pos;
			m_SUI.Main.Tabs[i].SetActive(Active);
			m_SUI.Main.Tabs[i].SetAlram(IsMainUIAlram(Active, (MainUITab)i) && (i == 0 ? true : USERINFO.m_Guild.UID != 0));
		}

		m_MainUITabPos = pos;
		switch (m_MainUITabPos)
		{
		// 프로토콜 보내서 신청 리스트 다시 받는다.
		case MainUITab.INFO: Set_Info_UI(); break;
		case MainUITab.MEMBER:
			m_SUI.Main.Member.MngMode = MemberListMode.Normal;
			Set_Member_UI(); 
			break;
		}
		return true;
	}

	bool IsMainUIAlram(bool Active, MainUITab tab)
	{
		if (Active) return false;
		//switch (tab)
		//{
		//case MainUITab.INFO: return USERINFO.m_Guild.IsAlram() != GuildInfo_Base.AlramMode.None;
		//}
		return false;
	}
	public void Click_GuildInfo()
	{
		POPUP.Set_MsgBox(PopupName.Msg_Guide, string.Empty, TDATA.GetString(6034));
	}

	public void Click_Raid()
	{
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6031));
	}

	public void Click_Shelter()
	{
		POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6036));
	}
	public void Click_Research()
	{
		if (!IsStart) return;
		GoResearch();
	}
	public void GoResearch() {
		USERINFO.m_Guild.LoadGuild(() => {
			if (USERINFO.m_Guild.UID == 0) {
				USERINFO.m_Guild.Set_AlramOff();
				Close((int)Union_JoinList.CloseResult.LoadGuild);
			}
			else POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Research, (result, obj) => { Set_Info_UI(); });

		}, 0, false, true, false);
	}
	public void Click_ApplyMaster()
	{
		if (!IsStart) return;
		// 길드 위임 받기
		if (USERINFO.m_Guild.MyGrade() != GuildGrade.Spare_Master) return;
		USERINFO.m_Guild.SaveSpareCheck();
		POPUP.Set_MsgBox(PopupName.Msg_YN_BtnControl, TDATA.GetString(6115), TDATA.GetString(6116), (result, obj) => {
			if (result == 1)
			{
				WEB.SEND_REQ_GUILD_APPLY_MASTAR((res) => {
					if (!res.IsSuccess())
					{
						switch(res.result_code)
						{
						case EResultCode.ERROR_GUILD_GRADE:
							USERINFO.m_Guild.MyInfo.Grade = GuildGrade.Normal;
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6159));
							Reset_MainUI();
							break;
						default:
							WEB.StartErrorMsg(res.result_code, (btn, obj) => {
								ReLoadGuild();
							});
							break;
						}
						return;
					}
					POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(6074), (result, obj) => { });
					Reset_MainUI();
				});
			}
		}
		, new Msg_YN_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_NO, Label = TDATA.GetString(11), BG = UIMng.BtnBG.Brown }
		, new Msg_YN_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_YES, Label = TDATA.GetString(5026), BG = UIMng.BtnBG.Green });
	}

	public void Click_Notice()
	{
		if (!IsStart) return;
		if (USERINFO.m_Guild.MyGrade() != GuildGrade.Master) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_NoticeEdit, (result, obj) => { Set_Option_NoticeUI(); });
	}

	public void Click_Mng()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Mng, (result, obj) => {
			switch ((Union_JoinList.CloseResult)result)
			{
			case Union_JoinList.CloseResult.Grade:
				// 등급 변경에러
				Set_Info_UI();
				break;
			case Union_JoinList.CloseResult.Destroy:
				// 길드 해산
				POPUP.Init_PopupUI();
				POPUP.Set_MsgBox(PopupName.Msg_OK, TDATA.GetString(6091), string.Format(TDATA.GetString(6094), USERINFO.m_Guild.Name), (result, obj) => { });
				Close(result);
				break;
			case Union_JoinList.CloseResult.LoadGuild:
				// 길드 퇴출때만 사용
				if (USERINFO.m_Guild.UID == 0) Close(result);
				else Set_Info_UI();
				break;
			}
		});
	}
	#endregion

	#region Member
	public void Click_MemberMng()
	{
		m_SUI.Main.Member.MngMode = (MemberListMode)(1 - (int)m_SUI.Main.Member.MngMode);
		m_MemberModeChange?.Invoke(m_SUI.Main.Member.MngMode);
	}
	void Click_MemberBtn(CallMode mode, RES_GUILD_USER user)
	{
		switch(mode)
		{
		case CallMode.Remove:	MemberMng_Remove(user);	break;
		case CallMode.Master:	MemberMng_Master(user); break;
		}
	}

	void MemberMng_Remove(RES_GUILD_USER user)
	{
		if (user.Grade == GuildGrade.Master)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6184));
			return;
		}
		if(!USERINFO.m_Guild.IsUserKickState())
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6117));
			return;
		}

		int Title = 6106;
		int Msg = 6107;
		int YBtn = 6042;
		bool IsMyInfo = true;
		if (user.UserNo != USERINFO.m_UID)
		{
			Title = 6066;
			Msg = 6101;
			YBtn = 6069;
			IsMyInfo = false;
		}

		POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, TDATA.GetString(Title), TDATA.GetString(Msg), (result, obj) => {
			if (result == 1)
			{
				WEB.SEND_REQ_GUILD_REMOVE_USER((res) => {

					if (!res.IsSuccess())
					{
						switch (res.result_code)
						{
						case EResultCode.ERROR_NOT_FOUND_GUILD:
						case EResultCode.ERROR_GUILD_NOT_MEMBER:    // 강퇴당함(마스터 이전후 강퇴됨)
							ReLoadGuild();
							if(!IsMyInfo) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6160));
							break;
						case EResultCode.ERROR_GUILD_DESTROY:
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6184));
							break;
						case EResultCode.ERROR_GUILD_GRADE:
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
							ReLoadGuild();
							break;
						case EResultCode.ERROR_NOT_FOUND_USER:
							ReLoadGuild();
							break;
						default:
							WEB.StartErrorMsg(res.result_code, (btn, obj) => {
								ReLoadGuild();
							});
							break;
						}
						return;
					}
					PlayEffSound(SND_IDX.SFX_1503);
					if (IsMyInfo) {
						Close((int)Union_JoinList.CloseResult.None);
						POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(6108), (result, obj) => { });
						return;
					}

					POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(6102), (result, obj) => { });
					Reset_MainUI();
				}, user.UserNo);
			}
		}, TDATA.GetString(11), TDATA.GetString(YBtn));
	}

	void MemberMng_Master(RES_GUILD_USER user)
	{
		POPUP.Set_MsgBox(PopupName.Msg_YN_YRed, string.Empty, TDATA.GetString(6104), (result, obj) => {
			if (result == 1)
			{
				WEB.SEND_REQ_GUILD_MEMBER_GRADE_CHANGE((res) => {

					if (!res.IsSuccess())
					{
						switch (res.result_code)
						{
						case EResultCode.ERROR_GUILD_GRADE:
							USERINFO.m_Guild.MyInfo.Grade = GuildGrade.Normal;
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
							ReLoadGuild();
							break;
						case EResultCode.ERROR_GUILD_NOT_MEMBER:
							POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6160));
							USERINFO.m_Guild.Users.RemoveAll(o => o.UserNo == user.UserNo);
							USERINFO.m_Guild._UserCnt = USERINFO.m_Guild.Users.Count;
							Reset_MainUI();
							break;
						default:
							WEB.StartErrorMsg(res.result_code, (btn, obj) => {
								ReLoadGuild();
							});
							break;
						}
						return;
					}
					POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(6105), (result, obj) => { });
					USERINFO.m_Guild.SaveMyGrade();
					m_SUI.Main.Member.MngMode = MemberListMode.Normal;
					Reset_MainUI();
				}, user.UserNo, GuildGrade.Master);
			}
		}, TDATA.GetString(11), TDATA.GetString(6070));
	}
	#endregion
	#endregion
	public void Click_Store() {
		ClickTab(m_SUI.Tabs[1]);
	}
}
