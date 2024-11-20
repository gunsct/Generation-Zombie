using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Text;


[System.Serializable] public class DicItem_Union_Research_ElementBtn : SerializableDictionary<Item_Union_Research_Element.AniName, GameObject> { }
public class Item_Union_Research_Element : ObjMng
{
	public enum Mode
	{
		Start = 0,
		Stop,
		End
	}
	public enum AniName
	{
		Normal = 0,
		NotYet,
		Now,
		Complete,
		OtherStep,
		End
	}
	[Serializable]
	public struct SLockUI
	{
		public TextMeshProUGUI Label;
	}
	[Serializable]
	public struct SMatUI
	{
		public Image Icon;
		public TextMeshProUGUI Cnt;

	}

	[Serializable]
	public struct SInfoUI
	{
		public TextMeshProUGUI Name;

		public SMatUI Mat;

	}

	[Serializable] 
    public struct SUI
	{
		public Animator Ani;
		public SInfoUI Info;
		public SLockUI Lock;

		public DicItem_Union_Research_ElementBtn Btns;
	}

	[SerializeField] SUI m_SUI;
	int m_Idx;
	Action<Mode, int> m_CB;
	AniName m_PlayAni;
	bool IsStart;
	public void Start()
	{
		IsStart = true;
		StartAni();
	}
	public void SetData(int Idx, int LV, int step, Action<Mode, int> cb) {
		m_Idx = Idx;
		m_CB = cb;
		var tdata = TDATA.GetGuildRes(Idx);

		m_SUI.Info.Name.text = tdata.GetDesc();
		var titem = TDATA.GetItemTable(tdata.m_Mat.m_Idx);
		m_SUI.Info.Mat.Icon.sprite = UTILE.LoadImg("UI/Icon/Icon_Union_Research", "png");
		m_SUI.Info.Mat.Cnt.text = Utile_Class.CommaValue(tdata.m_Mat.m_Count);

		m_PlayAni = AniName.Normal;

		var label = new StringBuilder(1024);
		if(USERINFO.m_Guild.ResIdx == Idx) m_PlayAni = AniName.Now;
		else if (USERINFO.m_Guild.EndRes.Contains(Idx)) m_PlayAni = AniName.Complete;
		// 락상태 확인
		else
		{
			if (step < tdata.m_Group) m_PlayAni = AniName.NotYet;

			if (LV < tdata.m_Unlock.m_LV)
			{
				label.Append(string.Format(TDATA.GetString(6111), tdata.m_Unlock.m_LV));
				m_PlayAni = AniName.NotYet;
			}

			if (tdata.m_Unlock.m_ResIdx > 0 && !USERINFO.m_Guild.EndRes.Contains(tdata.m_Unlock.m_ResIdx))
			{
				if (m_PlayAni == AniName.NotYet) label.Append("\n");
				var tldata = TDATA.GetGuildRes(tdata.m_Unlock.m_ResIdx);
				label.Append(string.Format(TDATA.GetString(6177), tldata.m_Group, tldata.GetName()));
				m_PlayAni = AniName.NotYet;
			}
		}

		m_SUI.Lock.Label.text = label.ToString();
		StartAni();
		for (AniName i = AniName.Normal; i < AniName.End; i++)
		{
			if (!m_SUI.Btns.ContainsKey(i)) continue;
			m_SUI.Btns[i].SetActive(false);
		}
		if (USERINFO.m_Guild.MyGrade() == GuildGrade.Master && m_SUI.Btns.ContainsKey(m_PlayAni)) m_SUI.Btns[m_PlayAni].SetActive(true);
	}
	void StartAni()
	{
		if (!IsStart) return;
		Utile_Class.AniResetAllTriggers(m_SUI.Ani);
		m_SUI.Ani.SetTrigger(m_PlayAni.ToString());
	}
	public void Click_Start()
	{
		m_CB?.Invoke(Mode.Start, m_Idx);
	}
	public void Click_Stop()
	{
		m_CB?.Invoke(Mode.Stop, m_Idx);
	}
}
