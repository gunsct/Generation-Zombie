using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_CharSmall_Card : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Image Icon;
		public Image GradeBG;
		public Image Frame;
		public TextMeshProUGUI Lv;
		public GameObject LvActive;
		public Item_GradeGroup GradeGroup;
		public UIEffect UIFX;
	}
	[SerializeField]
	SUI m_SUI;
	int m_Idx;
	public int m_Grade = 1;
	TCharacterTable m_TData { get { return TDATA.GetCharacterTable(m_Idx); } }
	public void SetData(int _idx) {
		m_Idx = _idx;
		m_SUI.Icon.sprite = m_TData.GetPortrait();
		SetGrade(m_TData.m_Grade);
		if (m_SUI.UIFX != null) m_SUI.UIFX.effectFactor = USERINFO.m_PlayDeck.IS_InDeck(m_Idx) ? 0f : 1f;
	}

	public void SetData(CharInfo info)
	{
		m_Idx = info.m_Idx;
		m_SUI.Icon.sprite = m_TData.GetPortrait();
		if(m_SUI.Lv != null) m_SUI.Lv.text = info.m_LV.ToString();
		SetGrade(info.m_Grade);
	}

	public void SetGrade(int _grade = 1)
	{
		if(m_SUI.GradeGroup != null) m_SUI.GradeGroup.SetData(Mathf.Max(_grade, 1));
		if(m_SUI.GradeBG != null) m_SUI.GradeBG.sprite = BaseValue.CharBG(_grade);
		if (m_SUI.Frame != null) m_SUI.Frame.sprite = BaseValue.CharFrame(_grade);
		m_Grade = _grade;
	}
	public void SetLv(int _lv = 1) {
		if (m_SUI.LvActive != null) m_SUI.LvActive.SetActive(_lv > 1);
		if (m_SUI.Lv != null) m_SUI.Lv.text = _lv.ToString();
	}
	public virtual void ClickCard() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_ToolTip, m_Idx)) return;
		if (m_TData != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}


	public RES_REWARD_BASE GetRewardInfo()
	{
		var res = new RES_REWARD_CHAR();
		res.Idx = m_Idx;
		res.Grade = 0;
		return res;
	}
}
