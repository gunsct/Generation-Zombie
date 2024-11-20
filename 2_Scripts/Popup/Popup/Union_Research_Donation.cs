using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Union_Research_Donation : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI	MyItemCnt;
		public TextMeshProUGUI Exp;

		public Slider Slider;
	}
	[SerializeField] SUI m_SUI;
	bool IsStart;
	TGuild_ResearchTable m_TData { get { return TDATA.GetGuildRes(USERINFO.m_Guild.ResIdx); } }
	int m_ItemCnt;
	int m_ResIdx;

	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
		yield return null;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		var tdata = m_TData;
		m_ResIdx = USERINFO.m_Guild.ResIdx;
		m_ItemCnt = USERINFO.GetItemCount(tdata.m_Mat.m_Idx);
		int Min = 0, Max = 0;
		Max = Math.Max(0, Math.Min(m_ItemCnt, USERINFO.m_Guild.MyInfo.GetMaxResCnt()));
		Min = Math.Min(0, Max);

		m_SUI.Slider.minValue = Min;
		m_SUI.Slider.maxValue = Max;
		m_SUI.Slider.value = 0;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		base.SetUI();

		var tdata = m_TData;
		m_SUI.Item.SetData(RewardKind.Item, tdata.m_Mat.m_Idx, 1, 1, 0, null, false);
		m_SUI.MyItemCnt.text = string.Format("({0}:{1})", TDATA.GetString(243), m_ItemCnt);

		OnValueChange();
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
			else if (USERINFO.m_Guild.ResIdx != m_ResIdx)
			{
				USERINFO.m_Guild.Set_Alram_Res_End_Off();
				Close(0);
			}
			else SetUI();
		}, 0, true, true, true);
	}
	#region Btn
	public void OnValueChange()
	{
		m_SUI.Exp.text = $"{m_SUI.Slider.value} / {m_SUI.Slider.maxValue}";
	}
	public void Click_Add(int add)
	{
		if (!IsStart) return;
		m_SUI.Slider.value += add;
	}

	public void Click_Max()
	{
		if (!IsStart) return;
		m_SUI.Slider.value = m_SUI.Slider.maxValue;
	}

	public void Click_Give()
	{
		var tdata = m_TData;
		if (!IsStart) return;
		if (m_SUI.Slider.value < 1) return;
		POPUP.Set_MsgBox(PopupName.Msg_Union_Research_Donation_Confirm, string.Empty, string.Format(TDATA.GetString(6051), m_SUI.Slider.value), (result, obj) => {
			if (result == 1) Send_ResGive();
		}, tdata.m_Mat.m_Idx, m_ItemCnt, (int)m_SUI.Slider.value);
	}

	void Send_ResGive()
	{
		WEB.SEND_REQ_GUILD_RES_GIVE((res) =>
		{
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_NOT_FOUND_USER:
				case EResultCode.ERROR_NOT_FOUND_GUILD:
				case EResultCode.ERROR_GUILD_NOT_MEMBER:    // 강퇴당함(마스터 이전후 강퇴됨)
					Close((int)Union_JoinList.CloseResult.LoadGuild);
					break;
				case EResultCode.ERROR_GUILD_DIF_RESEARCH:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6198));
					ReLoadGuild();
					break;
				case EResultCode.ERROR_GUILD_END_RESEARCH:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6097));
					ReLoadGuild();
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					ReLoadGuild();
					break;
				}
				return;
			}
			PlayEffSound(SND_IDX.SFX_1523);
			Close(0);
			if (USERINFO.m_Guild.IsAlram(GuildInfo_Base.AlramMode.ResEnd)) POPUP.Set_MsgBox(PopupName.Msg_Union_Research_Finish, string.Empty, string.Empty, (result, obj) => { });
			else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6052));
		}, USERINFO.m_Guild.ResIdx, (int)m_SUI.Slider.value);
	}


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
