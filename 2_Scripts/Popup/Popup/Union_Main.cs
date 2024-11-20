using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public partial class Union_Main : PopupBase
{
	public enum Tab
	{
		NONE = -1,
		/// <summary> 활동 </summary>
		MAIN,
		/// <summary> 상점 </summary>
		SHOP
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		[ReName("메인", "연합상점")]
		public Item_Tab[] Tabs;

		public SMainUI Main;
		public SShopUI Shop;

	}

	[SerializeField] SUI m_SUI;

	Tab m_TabPos = Tab.NONE;
	bool IsStart;
	SND_IDX m_PreBGSND;

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);

		// 생성 완료 팝업이 떠있는동안 대기
		yield return new WaitWhile(() => {
			var msg = POPUP.GetMsgBox();
			return msg != null && msg.m_Popup == PopupName.Msg_NewUnion_Result;
		});

		// 출석 체크
		yield return Check_Attendance();
		yield return Check_LVChange();
		yield return Check_EndRes();

		if (USERINFO.m_Guild.IsAlram(GuildInfo_Base.AlramMode.Mastar))
		{
			POPUP.Set_MsgBox(PopupName.Msg_OK, string.Empty, TDATA.GetString(6074), (result, obj) => { });
			USERINFO.m_Guild.SaveMyGrade();
		}

		IsStart = true;

		if (USERINFO.m_Guild.IsAlram(GuildInfo_Base.AlramMode.Spare_Master))
		{
			Click_ApplyMaster();
		}
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PreBGSND = SND.GetNowBG;
		PlayBGSound(SND_IDX.BGM_0020);

		IsStart = false;
		m_SUI.Tabs[0].SetData(0, TDATA.GetString(920), ClickTab);
		m_SUI.Tabs[1].SetData(1, TDATA.GetString(6061), ClickTab);

		m_SUI.Main.Tabs[0].SetData(0, TDATA.GetString(6181), ClickMainTab);
		m_SUI.Main.Tabs[1].SetData(1, TDATA.GetString(6182), ClickMainTab);

		USERINFO.m_Guild.Set_Alram_Res_Off();
		USERINFO.m_GuildKickCheck.Save();
		base.SetData(pos, popup, cb, aobjValue);
		
	}

	public override void SetUI() {
		base.SetUI();
		if(m_TabPos == Tab.NONE) m_SUI.Tabs[0].OnClick();
		else
		{
			switch (m_TabPos)
			{
			// 프로토콜 보내서 신청 리스트 다시 받는다.
			case Tab.MAIN:
				Set_Main_UI();
				if (m_MainUITabPos == MainUITab.NONE) m_SUI.Main.Tabs[0].OnClick();
				else
				{
					switch (m_MainUITabPos)
					{
					// 프로토콜 보내서 신청 리스트 다시 받는다.
					case MainUITab.INFO: Set_Info_UI(); break;
					case MainUITab.MEMBER:
						m_SUI.Main.Member.MngMode = MemberListMode.Normal;
						Set_Member_UI();
						break;
					}
				}
				break;
			//case Tab.SHOP: Set_Shop_UI(); break;
			}
		}

	}

	IEnumerator Check_Attendance()
	{
		if (USERINFO.m_Guild.GetAlramMode() == GuildInfo_Base.AlramMode.Attendance)
		{
			bool IsEnd = false;
			WEB.SEND_REQ_GUILD_ATTENDANCE((res) =>
			{
				var rews = res.GetRewards();
				if (rews.Count < 1) return;
				POPUP.Set_MsgBox(PopupName.Msg_Union_Attendance_Result, string.Empty, string.Empty, (btn, obj) => {
					IsEnd = true;
					Set_Info_UI();
				}, rews);
			});
			yield return new WaitWhile(() => !IsEnd);
		}
	}

	IEnumerator Check_LVChange()
	{
		var lv = PlayerPrefs.GetInt($"MYGUILD_LV_{USERINFO.m_UID}", 1);
		USERINFO.m_Guild.SaveLV();
		int nowlv = PlayerPrefs.GetInt($"MYGUILD_LV_{USERINFO.m_UID}", 1);

		if (nowlv > 1 && lv < nowlv)
		{
			bool IsEnd = false;
			// 레벨업 알림
			POPUP.Set_MsgBox(PopupName.Msg_Union_LvUpAlarm, string.Empty, string.Empty, (btn, obj) => {
				IsEnd = true;
			});
			yield return new WaitWhile(() => !IsEnd);
		}
	}
	IEnumerator Check_EndRes()
	{
		if (USERINFO.m_Guild.IsAlram(GuildInfo_Base.AlramMode.ResEnd))
		{
			bool IsEnd = false;
			POPUP.Set_MsgBox(PopupName.Msg_Union_Research_Finish, string.Empty, string.Empty, (result, obj) => {
				IsEnd = true;
			});
			yield return new WaitWhile(() => !IsEnd);
		}
	}

	bool IsAlram(bool Active, Tab tab)
	{
		if (Active) return false;
		//switch(tab)
		//{
		//case Tab.MAIN: return USERINFO.m_Guild.GetAlramMode() != GuildInfo_Base.AlramMode.None;
		//}
		return false;
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
			else if(m_TabPos == Tab.MAIN) Reset_MainUI();
		}, 0, true, true, true);
	}

	void SetErrorMsg(ushort result_code)
	{
		string GName = USERINFO.m_Guild.Name;
		switch (result_code)
		{
		case EResultCode.ERROR_GUILD_JOIN:
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6065));
			break;
		case EResultCode.ERROR_GUILD_GRADE:         // 마스터 아님
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6173));
			ReLoadGuild();
			break;
		case EResultCode.ERROR_GUILD_NOT_MEMBER:    // 강퇴당함(마스터 이전후 강퇴됨)
			ReLoadGuild();
			break;
		case EResultCode.ERROR_GUILD_MAX_MEMBER:    // 최대치 오버
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

	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		// 연축 제거
		PlayBGSound(m_PreBGSND);
		base.Close(Result);
		//StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		PlayBGSound(m_PreBGSND);
		base.Close(_result);
	} 
}
