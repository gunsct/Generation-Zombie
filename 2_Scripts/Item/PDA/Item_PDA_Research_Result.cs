using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PDA_Research_Result : Item_PDA_Base
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI LV;
		public TextMeshProUGUI Msg;
		public TextMeshProUGUI Info;
	}
#pragma warning restore 0649

	[SerializeField] SUI m_SUI;
	ResearchType m_Tree;
	ResearchInfo m_Info;
	bool m_IsFirstOpen, m_IsLock, m_IsUpgrade;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);
		m_Tree = (ResearchType)args[0];
		m_Info = (ResearchInfo)args[1];
		SetUI();
	}

	public void SetUI() {
		PlayEffSound(SND_IDX.SFX_0123);
		PlayCommVoiceSnd(VoiceType.Success);
		TResearchTable tdata = m_Info.m_TData;
		int MaxLV = m_Info.m_MaxLV;
		bool IsMaxLV = m_Info.m_GetLv == MaxLV;
		m_IsFirstOpen = m_Info.Is_Open();
		// 연구 정보
		m_SUI.Icon.sprite = tdata.GetIcon();
		m_SUI.LV.text = IsMaxLV ? "MAX" : string.Format("Lv {0} / {1}", m_Info.m_GetLv, MaxLV);
		m_SUI.Msg.text = string.Format(TDATA.GetString(256), tdata.GetName(), m_Info.m_GetLv);
		m_SUI.Info.text = tdata.GetInfo(2);
	}

	public override void OnClose() {
		m_CloaseCB?.Invoke(Item_PDA_Research.State.Tree, new object[] { m_Tree });
	}
}
