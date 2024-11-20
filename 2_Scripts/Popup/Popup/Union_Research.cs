using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Union_Research : PopupBase
{
	[Serializable]
	public struct SNoneUI
	{
		public TextMeshProUGUI Msg;
	}
	[Serializable]
	public struct SResUI
	{
		public TextMeshProUGUI[] Name;
		public TextMeshProUGUI[] Cnt;
		public TextMeshProUGUI[] MatName;
		public TextMeshProUGUI Per;
		public Image Photo;
		public Image Gauge;
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;

		public SNoneUI None;
		public SResUI Res;
	}

	[SerializeField] SUI m_SUI;

	bool IsStart;
	TGuild_ResearchTable m_TData { get { return TDATA.GetGuildRes(USERINFO.m_Guild.ResIdx); } }
	string m_Trigger;
	private IEnumerator Start()
	{
		m_SUI.Anim.SetTrigger(m_Trigger);
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
		IsStart = true;
		yield return null;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		IsStart = false;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();

		var tdata = m_TData;
		m_Trigger = "Nothing";
		if (tdata == null)
		{
			// 연구 없음
			NoneUI();
		}
		else
		{
			ResUI();
			m_Trigger = $"Type_{tdata.m_TypeNo}";
		}

		m_SUI.Anim.SetTrigger(m_Trigger);
	}

	void NoneUI()
	{
		// 진행 가능한 연구가 있는지 확인
		int msg = TDATA.IsCanGuildRes() ? 6187 : 6089;
		m_SUI.None.Msg.text = TDATA.GetString(msg);
	}

	void ResUI()
	{
		var tdata = m_TData;
		string msg = string.Format("<size=70%>// {0}{1}</size>\n{2}", tdata.m_Group, TDATA.GetString(324), tdata.GetDesc());
		for (int i = 0; i < m_SUI.Res.Name.Length; i++) m_SUI.Res.Name[i].text = msg;

		msg = string.Format("{0}\n<size=70%>/ {1}</size>", USERINFO.m_Guild.ResExp, tdata.m_Mat.m_Count);
		for (int i = 0; i < m_SUI.Res.Cnt.Length; i++) m_SUI.Res.Cnt[i].text = msg;

		var titem = TDATA.GetItemTable(tdata.m_Mat.m_Idx);
		msg = $": {titem.GetName()}";
		for (int i = 0; i < m_SUI.Res.MatName.Length; i++) m_SUI.Res.MatName[i].text = msg;
		ResGaugeUI();

		m_SUI.Res.Photo.sprite = tdata.GetPoto();

	}

	void ResGaugeUI()
	{
		var tdata = m_TData;
		float per = (float)((double)USERINFO.m_Guild.ResExp / (double)tdata.m_Mat.m_Count);
		m_SUI.Res.Per.text = $"{Mathf.RoundToInt(per * 100)}<size=50%>%</size>";
		m_SUI.Res.Gauge.fillAmount = per;
	}
#region Btn
	public void Click_Res()
	{
		if (!IsStart) return;
		if(USERINFO.m_Guild.MyInfo.GetMaxResCnt() < 1)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6095));
			return;
		}
		var tdata = m_TData;
		if (USERINFO.GetItemCount(tdata.m_Mat.m_Idx) < 1)
		{
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(6096));
			return;
		}

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Research_Donation, (result, obj) => {
			if (result != 0) Close(result);
			else SetUI();
		});
	}

	public void Click_ResList()
	{
		if (!IsStart) return;
		var ResIdx = USERINFO.m_Guild.ResIdx;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Research_List, (result, obj) => {
			if (result != 0) Close(result);
			else if (ResIdx != USERINFO.m_Guild.ResIdx) SetUI();
		});
	}

	public void Click_BuffList()
	{
		if (!IsStart) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Union_Research_EffectList, (result, obj) => {});
	}

	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		StartCoroutine(CloseAction(Result));
	}
	IEnumerator CloseAction(int _result)
	{
		IsStart = false;
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	} 
#endregion
}
