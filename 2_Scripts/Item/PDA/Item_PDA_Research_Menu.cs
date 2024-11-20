using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PDA_Research_Menu : Item_PDA_Base
{
#pragma warning disable 0649
	[System.Serializable]
	struct SBtnUI
	{
		public GameObject Active;
		public Animator Anim;
		public GameObject Alram;
		public GameObject Lock;
		public TextMeshProUGUI Cnt;
		public GameObject Progress;
	}
	//하단 진행중 연구
	[Serializable]
	struct SBPUI
	{
		public Animator Anim;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Time;
		public Image Gauge;
	}
	[System.Serializable]
	struct SUI
	{
		public SBtnUI[] Btns;
		public SBPUI Progress;
		public Animator Ani;
	}
#pragma warning restore 0649

	[SerializeField] SUI m_SUI;

	private void OnEnable() {
		StartCoroutine(StartAction());
	}
	private void Update() {
		SetProgressResearch();
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);
		if (TUTO.IsTuto(TutoKind.Research, (int)TutoType_Research.Select_PDA_ResearchBtn)) TUTO.Next();
		SetUI();
	}
	IEnumerator StartAction()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		if (TUTO.IsTuto(TutoKind.Research, (int)TutoType_Research.ViewResearch)) TUTO.Next(this);
	}

	public void SetUI()
	{
		// 연구 버튼
		List<ResearchInfo> infos = USERINFO.ResearchInfos(ResearchType.Research);
		bool IsAlram = USERINFO.IsCompResearching(ResearchType.Research);
		m_SUI.Btns[0].Anim.SetTrigger("Normal");
		m_SUI.Btns[0].Cnt.text = string.Format("{0}/{1}", infos.Count, TDATA.GetResearchTable_GroupIdxs(ResearchType.Research).Count);
		m_SUI.Btns[0].Alram.SetActive(IsAlram);

		infos = USERINFO.ResearchInfos(ResearchType.Training);
		IsAlram = USERINFO.IsCompResearching(ResearchType.Training);
		bool IsOpen = USERINFO.ResearchValue(ResearchEff.TrainingOpen) > 0;
		m_SUI.Btns[1].Anim.SetTrigger(IsOpen ? "Normal" : "Lock");
		m_SUI.Btns[1].Cnt.text = string.Format("{0}/{1}", infos.Count, TDATA.GetResearchTable_GroupIdxs(ResearchType.Training).Count);
		m_SUI.Btns[1].Alram.SetActive(IsAlram);


		infos = USERINFO.ResearchInfos(ResearchType.Remodeling);
		IsAlram = USERINFO.IsCompResearching(ResearchType.Remodeling);
		IsOpen = USERINFO.ResearchValue(ResearchEff.RemodelingOpen) > 0;
		m_SUI.Btns[2].Anim.SetTrigger(IsOpen ? "Normal" : "Lock");
		m_SUI.Btns[2].Cnt.text = string.Format("{0}/{1}", infos.Count, TDATA.GetResearchTable_GroupIdxs(ResearchType.Remodeling).Count);
		m_SUI.Btns[2].Alram.SetActive(IsAlram);
	}
	/// <summary> 현재 진행중인 연구 표기 </summary>
	void SetProgressResearch() {
		ResearchInfo info = null;
		for (ResearchType i = ResearchType.Research; i<= ResearchType.Remodeling; i++) {
			if(info == null)
				info = USERINFO.IsResearching(i);
			if (info != null) break;
		}
		for (int i = 0; i < m_SUI.Btns.Length; i++) {
			m_SUI.Btns[i].Progress.SetActive(info == null ? false : i == (int)info.m_Type && info.m_State == TimeContentState.Play);
		}
		m_SUI.Progress.Anim.SetTrigger(info == null ? "Off" : info.m_State == TimeContentState.Play ? "Progress" : "Complete");
		if (info != null) {
			double remain = info.GetRemainTime();
			m_SUI.Progress.Name.text = string.Format("{0} Lv.{1}", info.m_TData.GetName(), info.m_GetLv + 1);
			m_SUI.Progress.Time.text = remain > 0 ? UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.dd_hh_mm_ss, remain) : TDATA.GetString(210);
			m_SUI.Progress.Gauge.fillAmount = 1f - remain == 0 ? 0f : (float)(remain / (info.GetMaxTime() * 0.001d));
		}
	}
	public void ClickProgress() {
		ResearchInfo info = USERINFO.IsResearching();
		if (info == null) return;
		ViewTree((int)info.m_Type);
	}
	public void ClickExit() {
		PlayEffSound(SND_IDX.SFX_0121);
		OnClose();
	}
	public override void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Cloase, 1)) return;
		m_CloaseCB?.Invoke(Item_PDA_Research.State.End, null);
	}

	public void ViewTree(int Pos)
	{
		ResearchType type = (ResearchType)Pos;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Research_Menu, type)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		if (m_SUI.Btns[Pos].Lock.activeSelf) return;
		m_CloaseCB?.Invoke(Item_PDA_Research.State.Tree, new object[] { type });
	}

	public GameObject GetBtn(ResearchType type)
	{
		return m_SUI.Btns[(int)type].Active;
	}
}
