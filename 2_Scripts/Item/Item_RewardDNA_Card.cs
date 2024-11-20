using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static LS_Web;
using static UnityEngine.ParticleSystem;

public class Item_RewardDNA_Card : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public Image Frame;
		public Image GradeBG;
		public Image Deco;
		public ParticleSystem Particle;
		public Shadow Shadow;
		public GameObject FrameGlow;
		public Image FrameGlowImg;
		public TextMeshProUGUI Grade;
		public TextMeshProUGUI[] Lv;
		public GameObject Check;
		public GameObject Warning;
		public Color[] FrameColors;
		public Color[] DecoColors;
		public Sprite[] BGGrades;
		public Color[] ParticleMinColors;
		public Color[] ParticleMaxColors;
		public GameObject NotEquip;
	}
	[SerializeField]
	protected SUI m_SUI;
	public int m_Idx;
	public int m_Grade;
	public int m_Lv;
	public int m_Pos;
	public long m_UID;
	private Action<object, int> m_OnClick;
	
	protected TDnaTable m_TData { get { return TDATA.GetDnaTable(m_Idx); } }
	public DNAInfo m_Info { get { return USERINFO.GetDNA(m_UID); } }
	public void SetData(int idx, int pos, int lv, long uid = -1, Action<object, int> onClick = null, bool _noeq = false) {
		if (idx != -1)
		{
			m_Idx = idx;
			m_Pos = pos;
			m_UID = uid;
			m_SUI.Icon.sprite = m_TData.GetIcon();
			m_Grade = m_TData.m_Grade;
			if (lv < 1) lv = 1;
			m_Lv = lv;
			m_SUI.Grade.text = UTILE.Get_RomaNum(m_Grade);
			m_SUI.Lv[0].text = string.Format("<size=80%>Lv. </size>{0}", lv);
			m_SUI.Lv[1].text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", lv);
			m_SUI.Frame.sprite = BaseValue.GradeFrame(ItemType.DNA, m_Grade);
			if (m_SUI.Shadow != null) m_SUI.Shadow.enabled = false;
			if (m_SUI.FrameGlow != null) m_SUI.FrameGlow.SetActive(false);
			SetWarningActive(false);
			SetCheckActive(false);
			var tdata = TDATA.GetDnaTable(m_Idx);
			SetColor(tdata.m_BGType);
		}
		m_OnClick = onClick;
		m_SUI.NotEquip.SetActive(_noeq);
	}
	public void SetColor(DNABGType _type) {
		m_SUI.Frame.color = m_SUI.FrameColors[(int)_type];
		m_SUI.GradeBG.sprite = m_SUI.BGGrades[(int)_type];
		m_SUI.Deco.color = m_SUI.DecoColors[(int)_type];
		MainModule particle = m_SUI.Particle.main;
		particle.startColor = new MinMaxGradient(m_SUI.ParticleMinColors[(int)_type], m_SUI.ParticleMaxColors[(int)_type]);
	}
	public void PlayStartAnim()
	{
		m_SUI.Anim.SetTrigger("Start");
	}

	public void SetWarningActive(bool isActive)
	{
		m_SUI.Warning.SetActive(isActive);
	}

	public void SetCheckActive(bool isActive)
	{
		m_SUI.Check.SetActive(isActive);
	}

	public virtual void ClickCard() {
		if (m_SUI.NotEquip.activeSelf) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(1027));
			return;
		}
		if (m_OnClick != null) m_OnClick?.Invoke(this, m_Pos);
		else { 
			if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_ToolTip, m_Idx)) return;
			if (m_TData != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
		}
	}

	public RES_REWARD_BASE GetRewardInfo()
	{
		var res = new RES_REWARD_DNA();
		res.Idx = m_Idx;
		res.Grade = m_Grade;
		res.Lv = m_Lv;
		return res;
	}
}
