using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Skill_Card : ObjMng
{
	[System.Serializable]
	public struct SSetUI
	{
		public GameObject Frame;
		public Color[] IconCol;
	}

	[System.Serializable]
	public struct SUI
	{
		public Image Icon;
		public Image Frame;
		public Image GradeBG;
		public GameObject Lock;
		public GameObject LvGroup;
		public GameObject LvGroupGlow;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI LvVal;
		public SSetUI SetUI;
	}
	[SerializeField]
	SUI m_SUI;

	public int m_Idx;
	int m_Lv = 1;
	//bool m_Lock = true;
	bool m_IsSet = false;
	CharInfo m_Char;
	Action m_EndCB;
	TSkillTable m_TData { get { return TDATA.GetSkill(m_Idx); } }
	public void SetData(int _idx, int _lv, bool _lock = false, bool _IsEqSet = false, CharInfo _char = null, Action _cb = null) {
		m_Idx = _idx;
		m_Lv = _lv;
		m_Char = _char;
		m_EndCB = _cb;
		m_IsSet = _IsEqSet;

		m_SUI.Icon.sprite = m_TData.GetImg();
		if(m_SUI.Name != null) m_SUI.Name.text = m_TData.GetName();
		m_SUI.LvGroup.SetActive(m_TData.m_Type == SkillType.Passive1);
		m_SUI.LvGroupGlow.SetActive(m_TData.m_Type == SkillType.Passive1);
		m_SUI.LvVal.text = string.Format("{0} {1:#,###}", "Lv", m_Lv);

		//m_SUI.Frame.sprite = BaseValue.GradeFrame((int) m_TData.m_Grade);
		//m_SUI.GradeBG.sprite = BaseValue.ItemGradeBG((int) m_TData.m_Grade);

		int pos = _IsEqSet ? 1 : 0;
		string[] GradeBG = { "Card/Frame/BG_Skill", "Card/Frame/BG_Skill_Enhanced" };
		//string[] Frame = { "Card/Frame/CardFrame2_0", "Card/Frame/Frame_Skill_Enhanced" };

		m_SUI.GradeBG.sprite = UTILE.LoadImg(GradeBG[pos], "png");
		//m_SUI.Frame.sprite = UTILE.LoadImg(Frame[pos], "png");
		m_SUI.SetUI.Frame.SetActive(_IsEqSet);
		m_SUI.Icon.color = m_SUI.SetUI.IconCol[pos];

		m_SUI.Lock.SetActive(false);
	}

	/// <summary> 카드 클릭시 상세정보창, 콜백있으면 연결 </summary>
	public void ClickCard() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Char_Btn, 11)) return;
		if (m_Char == null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Skill, (result,obj) => {
			m_EndCB?.Invoke();
		}, m_Idx, m_Lv, m_Char.GetSkillType(m_Idx) != SkillType.Passive1, m_Char);
	}
}
