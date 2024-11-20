using Coffee.UIEffects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Info_Character_NotGet : PopupBase
{
	public enum State
	{
		Normal,
		Select
	}
	
	[Serializable]
	public struct SJobGroup
	{
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public TextMeshProUGUI Need;
	}

	[Serializable]
	public struct  SSkillGroup
	{
		public Item_Skill_Card Skill;
		public TextMeshProUGUI Names;
		public TextMeshProUGUI Descs;
		public GameObject Stars;
		public GameObject SetGrow;
	}

	[System.Serializable]
	public struct SUI
	{
		public ScrollRect ScrollRect;

		public TextMeshProUGUI CharName;
		public Image CharImg;

		public SJobGroup JobGroup;

		public Item_GradeGroup GradeGroup;

		public SSkillGroup[] SkillGroup;

		public TextMeshProUGUI[] StatValues;

		public TextMeshProUGUI GetBonus;

		public Animator Ani;

		public Transform ExitBtn;
		public Vector3[] ExitBtnPos;
		public Item_Store_Buy_Button SelectBuyBtn;
		public GameObject SelectPostBtn;
	}
	[SerializeField] SUI m_SUI;
	State m_State = State.Normal;
	IEnumerator m_Action;
	IEnumerator m_MoveTopAction;

	int m_Idx;
	int m_StateVal;
	TCharacterTable TChar { get { return TDATA.GetCharacterTable(m_Idx); } }

	private StatType[] m_statTypes = new StatType[] { StatType.Men, StatType.Hyg, StatType.Sat };

	/// <summary> aobjValue 0 : 캐릭터 인덱스 </summary>
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		//m_EndCB = cb;

		m_Idx = (int)aobjValue[0];
		m_State = (State)aobjValue[1];
		if (aobjValue.Length > 2) m_StateVal = (int)aobjValue[2];

		var charTable = TChar;

		// 변경될 일이 없는 UI

		var sprPortrait = charTable.GetPortrait();
		m_SUI.CharImg.sprite = sprPortrait;
		SetGrade(charTable.m_Grade);

		SND.StopAllVoice();
		SND_IDX vocidx = TChar.GetVoice(TCharacterTable.VoiceType.CharInfo);
		PlayVoiceSnd(new List<SND_IDX>() { vocidx });

		base.SetData(pos, popup, cb, aobjValue);

		StartCoroutine(CheckStartAction());
	}

	IEnumerator CheckStartAction()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		m_Action = null;
	}

	public override void SetUI()
	{
		base.SetUI();
		var charTable = TChar;
		m_SUI.CharName.text = charTable.GetCharName();

		// 시너지
		var synergyTable = TDATA.GetSynergyTable(charTable.m_Job[0]);
		m_SUI.JobGroup.Icon.sprite = synergyTable.GetIcon();
		m_SUI.JobGroup.Name.text = synergyTable.GetName();
		m_SUI.JobGroup.Desc.text = synergyTable.GetDesc();
		m_SUI.JobGroup.Need.text = synergyTable.m_NeedCount.ToString();

		SetSkillUI();

		SetStatUI();

		SetGetBonus();

		if (m_State == State.Select && m_StateVal > 0) {
			m_SUI.SelectBuyBtn.SetData(m_StateVal);
		}
		m_SUI.SelectBuyBtn.gameObject.SetActive(m_State == State.Select && m_StateVal > 0);
		m_SUI.SelectPostBtn.SetActive(m_State == State.Select && m_StateVal == 0);
		m_SUI.ExitBtn.localPosition = m_SUI.ExitBtnPos[(int)m_State];
	}
	
	void SetGrade(int _limit) {
		m_SUI.GradeGroup.SetData(_limit);
	}

	void SetStatUI() {
		TCharacterGradeStatTable tstat = TDATA.GetCharGradeStatTable(TChar.m_Grade);
		int MaxLV = TDATA.GetCharGradeTable(m_Idx, TChar.m_Grade).m_MaxLv;
		for (int i = 0; i < m_statTypes.Length; i++)
		{
			StatType stat = m_statTypes[i];
			float rank = 1f + tstat.GetStatRatio(stat);
			float basestat = TDATA.GetStatTable(m_Idx).GetStat(stat, MaxLV);
			m_SUI.StatValues[i].text = ((int)BaseValue.CalcStatValue(stat, basestat * rank, 0)).ToString();
		}
	}

	void SetSkillUI()
	{
		for (int i = 0; i < m_SUI.SkillGroup.Length; i++) {
			SkillType type = SkillType.Passive1;
			if (i == 0) type = SkillType.Active;
			int idx = TDATA.GetCharacterTable(m_Idx).m_SkillIdx[(int)type];
			TSkillTable skillTable = TDATA.GetSkill(idx);

			m_SUI.SkillGroup[i].Skill.SetData(idx, skillTable.m_MaxLV);
			m_SUI.SkillGroup[i].Names.text = skillTable.GetName();
			m_SUI.SkillGroup[i].Descs.text = skillTable.GetInfo(skillTable.m_MaxLV, 1);
			m_SUI.SkillGroup[i].Stars.SetActive(false);
			m_SUI.SkillGroup[i].SetGrow.SetActive(false);
		}
	}
	void SetGetBonus() {
		StringBuilder msg = new StringBuilder();
		TCharacterTable cdata = TDATA.GetCharacterTable(m_Idx);
		for (int i = 1; i < 3; i++) {
			TSkillTable stdata = TDATA.GetSkill(cdata.m_SkillIdx[i]);
			if (stdata == null) continue;
			if (stdata.GetStatType() == StatType.None)
				msg.Append(string.Format("[{0}]", stdata.GetInfo()));
			else if (stdata.GetStatType() != StatType.None)
				msg.Append(string.Format("[{0} +{1}]", TDATA.GetStatString(stdata.GetStatType()), cdata.GetPassiveStatValue(stdata.GetStatType(), 1)));
			msg.Append("\n");
		}
		m_SUI.GetBonus.text = msg.ToString();
	}
	public override void Close(int Result = 0)
	{
		if (m_Action != null) return;
		base.Close(Result);
	}

	public void OnBtnTopClicked()
	{
		if (m_MoveTopAction != null)
		{
			return;
		}
		m_MoveTopAction = CoMoveTop();
		StartCoroutine(m_MoveTopAction);
	}

	private IEnumerator CoMoveTop(Action callback = null)
	{
		var content = m_SUI.ScrollRect.content;
		m_SUI.ScrollRect.StopMovement();
		m_SUI.ScrollRect.velocity = Vector2.zero;

		iTween.ValueTo(gameObject, iTween.Hash(
			"from", m_SUI.ScrollRect.verticalNormalizedPosition,
			"to", 1f,
			"time", 0.5f,
			"easetype", "easeInOutBack",
			"onupdate", "MoveTop"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));
		callback?.Invoke();
		
		m_MoveTopAction = null;
	}

	private void MoveTop(float yPos)
	{
		m_SUI.ScrollRect.verticalNormalizedPosition = yPos;
	}

	public void OnBtnClickSynergyAll()
	{
		ClickSynergyInfo(TChar.m_Job[0]);
	}

	public void ClickSynergyInfo(JobType _type)
	{
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Synergy_All, null, _type);
	}
	public void Click_Select() {
		Close(m_Idx);
	}
}
