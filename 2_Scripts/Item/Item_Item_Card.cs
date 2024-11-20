using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Item_Card : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Image Icon;
		public Image Frame;
		public Image GradeBG;
		public Shadow Shadow;
		public GameObject FrameGlow;
		public Image FrameGlowImg;
		public TextMeshProUGUI Name;
		public GameObject[] GradeStar;
		public GameObject LvGroup;
		public TextMeshProUGUI Lv;
		public GameObject SetEquipIcon;
	}
	[SerializeField]
	protected SUI m_SUI;
	protected int m_Idx;
	protected int m_Grade;
	protected int m_LV;
	protected TItemTable m_TItem { get { return TDATA.GetItemTable(m_Idx); } }
	public virtual void SetData(int _idx, int _LV = 0, int _grade = 0) {
		m_Idx = _idx;
		m_LV = _LV;
		m_Grade = _grade;
		if (m_TItem != null) m_SUI.Icon.sprite = m_TItem.GetItemImg();
		if (m_SUI.Name != null && m_TItem != null) m_SUI.Name.text = m_TItem.GetName();

		if (m_SUI.Frame != null) {
			m_SUI.Frame.sprite = BaseValue.GradeFrame(m_TItem != null ? m_TItem.m_Type : ItemType.None, _grade, m_Idx);
			if (m_TItem != null && m_TItem.m_Type == ItemType.DNAMaterial) {
				m_SUI.Frame.color = BaseValue.RNAFrameColor(m_Idx)[0];
			}
		}
		if (m_SUI.GradeBG != null) m_SUI.GradeBG.sprite = BaseValue.ItemGradeBG(m_TItem != null ? m_TItem.m_Type : ItemType.None, _grade);
		if (m_SUI.Shadow != null) m_SUI.Shadow.enabled = m_TItem == null ? true :m_TItem.m_Type != ItemType.Zombie && m_TItem.m_Type != ItemType.DNA;
		if (m_SUI.FrameGlow != null) {
			m_SUI.FrameGlow.SetActive(_grade >= 6);
			m_SUI.FrameGlowImg.color = BaseValue.GradeFrameGlowColor(_grade);
		}

		if (m_SUI.LvGroup != null) m_SUI.LvGroup.SetActive(_LV > 0);
		if (m_SUI.Lv != null) m_SUI.Lv.text = string.Format("Lv. {0}", _LV.ToString());
		if(m_SUI.SetEquipIcon != null) m_SUI.SetEquipIcon.SetActive(m_TItem != null ? m_TItem.GetInvenGroupType() == ItemInvenGroupType.Equipment && m_TItem.m_Value > 0 : false);
	}

	public virtual void SetData(ItemInfo info)
	{
		m_Idx = info.m_Idx;
		m_LV = info.m_Lv;
		m_Grade = info.m_Grade;
		m_SUI.Icon.sprite = m_TItem.GetItemImg();
		if (m_SUI.Name != null) m_SUI.Name.text = m_TItem.GetName();

		if (m_SUI.Frame != null) {
			m_SUI.Frame.sprite = BaseValue.GradeFrame(m_TItem.m_Type, info.m_Grade, m_Idx);
			if (m_TItem.m_Type == ItemType.DNAMaterial) {
				m_SUI.Frame.color = BaseValue.RNAFrameColor(m_Idx)[0];
			}
		}
		if (m_SUI.GradeBG != null) m_SUI.GradeBG.sprite = BaseValue.ItemGradeBG(m_TItem.m_Type, info.m_Grade);
		if (m_SUI.Shadow != null) m_SUI.Shadow.enabled = m_TItem.m_Type != ItemType.Zombie && m_TItem.m_Type != ItemType.DNA;
		if (m_SUI.FrameGlow != null) {
			m_SUI.FrameGlow.SetActive(info.m_Grade >= 6);
			m_SUI.FrameGlowImg.color = BaseValue.GradeFrameGlowColor(info.m_Grade);
		}

		if (m_SUI.LvGroup != null) m_SUI.LvGroup.SetActive(info.m_Lv > 0);
		if (m_SUI.Lv != null) m_SUI.Lv.text = string.Format("Lv. {0}", info.m_Lv.ToString());
		if (m_SUI.SetEquipIcon != null) m_SUI.SetEquipIcon.SetActive(m_TItem.GetInvenGroupType() == ItemInvenGroupType.Equipment && m_TItem.m_Value > 0);
	}

	public void ClickCard() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_Item_Card, m_Idx)) return;
		if (TDATA.GetItemTable(m_Idx) != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}
	public RES_REWARD_BASE GetRewardInfo()
	{
		var res = new RES_REWARD_ITEM();
		res.Idx = m_Idx;
		res.Grade = m_Grade;
		res.LV = m_LV;
		return res;
	}
}
