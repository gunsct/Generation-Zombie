using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_Mk_Element : Item_Mk_Element_Parent
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image Icon;
		public Image GradeBG;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Price;
		public Slider Slider;
		public TextMeshProUGUI NeedGrade;
		public Image GradeIcon;
		public TextMeshProUGUI MakeTimer;
		public GameObject Btn;
		public TextMeshProUGUI BtnTxt;
		public Material[] BtnMats;
		public GameObject PastBtn;
	}
	[SerializeField] SUI m_SUI;

	private void Awake() {
		m_SUI.Btn.gameObject.SetActive(false);
	}
	private void OnEnable() {
		SetAnimState(m_AnimState);
	}
	private void Update() {
		if (m_State == TimeContentState.Play) {
			if (m_Info != null) m_SUI.Slider.value = 1f - (float)m_Info.GetRemainTime() / (float)(m_Info.GetMaxTime() * 0.001d);
		}
	}
	public void SetData(TMakingTable _tdata, Action<Item_Mk_Element_Parent> _cb) {
		m_Mk_TData = _tdata;
		m_CB = _cb;

		m_SUI.Icon.sprite = m_Item_TData.GetItemImg();
		m_SUI.GradeBG.sprite = BaseValue.ItemGradeBG(ItemType.None, _tdata.m_LV);
		m_SUI.Name.text = m_Item_TData.GetName();
		m_SUI.Price.text = string.Format("$ {0}",m_Mk_TData.m_Dollar);
		m_SUI.NeedGrade.text = string.Format("{0} {1}", m_Mk_TData.GetGradeName(), TDATA.GetString(279));
		m_SUI.GradeIcon.sprite = m_Mk_TData.GetGradeIcon();

		SetState(m_Info != null ? m_Info.m_State : TimeContentState.Idle);
	}
	public override void SetState(TimeContentState _advstate) {
		if (m_Info != null) {
			m_State = _advstate;
			SetAnimState(m_Info.IS_Complete() ? AnimState.Complete : AnimState.Process);
		}
		else {
			int grade = USERINFO.m_MakeLV;//연구 등급 USERINFO에서 가져와야함
			if (grade < m_Mk_TData.m_LV) SetAnimState(AnimState.Lock);
			else SetAnimState(m_Mk_TData.GetCanMake() ? AnimState.Active : AnimState.MateLack);
		}
	}
	public void SetAnimState(AnimState _state) {
		m_AnimState = _state;
		m_SUI.Anim.SetTrigger(m_AnimState.ToString());
		switch (m_AnimState) {
			case AnimState.Active: 
				m_SUI.BtnTxt.text = TDATA.GetString(278);
				m_SUI.Btn.GetComponent<Image>().material = m_SUI.BtnMats[0];
				m_SUI.Btn.gameObject.SetActive(true);
				m_SUI.PastBtn.SetActive(false);
				m_SUI.Price.fontStyle = FontStyles.Normal;
				break;
			case AnimState.Lock: 
				m_SUI.Btn.gameObject.SetActive(false);
				m_SUI.PastBtn.SetActive(false);
				m_SUI.Price.fontStyle = FontStyles.Underline; 
				break;
			case AnimState.MateLack: m_SUI.BtnTxt.text = TDATA.GetString(285);
				m_SUI.Btn.GetComponent<Image>().material = m_SUI.BtnMats[1];
				m_SUI.Btn.gameObject.SetActive(true);
				m_SUI.PastBtn.SetActive(false);
				m_SUI.Price.fontStyle = FontStyles.Underline;
				break;
			case AnimState.Process:
				m_SUI.PastBtn.SetActive(true);
				m_SUI.Btn.gameObject.SetActive(false); 
				break;
			case AnimState.Complete: m_SUI.BtnTxt.text = TDATA.GetString(286);
				m_SUI.Btn.GetComponent<Image>().material = m_SUI.BtnMats[2];
				m_SUI.Btn.gameObject.SetActive(true);
				m_SUI.PastBtn.SetActive(false);
				break;
		}
	}

	/// <summary> 카드 선택시 제작 위해 인덱스 반환 </summary>
	public void ClickBtn() {
		m_CB?.Invoke(this);
	}
	public void RefreshTimer() {
		if (m_Info == null) return;
		if (m_Info.m_State != TimeContentState.Play) return;
		else if (m_AnimState == AnimState.Complete) return;

		m_SUI.MakeTimer.text = UTILE.GetSecToTimeStr(m_Info.GetRemainTime());
		float per = 1f - (float)m_Info.GetRemainTime() / (float)(m_Info.GetMaxTime() * 0.001d);
		if (m_Info.IS_Complete()) SetState(TimeContentState.Play);
	}
	public void ClickToolTip() {
		POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}

	public RES_REWARD_BASE GetRewardInfo()
	{
		var res = new RES_REWARD_ITEM();
		res.Idx = m_Item_TData.m_Idx;
		res.Grade = m_Item_TData.m_Grade;
		return res;
	}
}
