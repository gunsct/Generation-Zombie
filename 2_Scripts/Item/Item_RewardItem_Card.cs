using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_RewardItem_Card : ObjMng
{
	public enum LockActiveMode
	{
		None = 0,
		Normal,
		Block
	}

	[System.Serializable]
	public struct SEQLV
	{
		public GameObject Active;
		public TextMeshProUGUI[] LV;
	}
	[System.Serializable]
	public struct SUI
	{
		public Image Icon;
		public Image Frame;
		public Image GradeBG;
		public Shadow Shadow;
		public GameObject FrameGlow;
		public Image FrameGlowImg;
		public TextMeshProUGUI Count;
		public SEQLV LV;
		public GameObject SetEquipIcon;
		public GameObject CharPieceDefault;
		public GameObject LockBG;
		public GameObject Lock;
	}
	[SerializeField]
	protected SUI m_SUI;
	public int m_Idx;
	public int m_LV;
	public int m_Grade;
	protected TItemTable m_TData { get { return TDATA.GetItemTable(m_Idx); } }
	public GameObject GetLvGroup { get { return m_SUI.LV.Active; } }
	public GameObject GetCntGroup { get { return m_SUI.Count.gameObject; } }
	public ItemInfo m_Item;
	Action<ItemInfo> m_CB;
	public void SetData(ItemInfo item, Action<ItemInfo> _CB, LockActiveMode LockMode = LockActiveMode.None)
	{
		m_Item = item;
		if (LockMode == LockActiveMode.None && item.m_Lock) LockMode = LockActiveMode.Normal;
		SetData(item.m_Idx, item.m_Stack, item.m_Lv, LockMode: LockMode);
		m_CB = _CB;
	}

	public void SetData(int _idx, int? _cnt = null, int _lv = 1, int _grade = 0, LockActiveMode LockMode = LockActiveMode.None) {
		m_CB = null;
		m_Idx = _idx;
		m_LV = _lv > 0 ? _lv : 1;
		m_SUI.Icon.sprite = m_TData.GetItemImg();
		m_Grade = _grade == 0 ? m_TData.m_Grade : _grade;
		if (m_SUI.Frame != null) {
			m_SUI.Frame.gameObject.SetActive(m_Idx != BaseValue.COMMON_PERSONNELFILE_IDX);
			m_SUI.Frame.sprite = BaseValue.GradeFrame(m_TData.m_Type, m_Grade, m_Idx);
			if(m_TData.m_Type == ItemType.DNAMaterial) m_SUI.Frame.color = BaseValue.RNAFrameColor(m_Idx)[0];
			else m_SUI.Frame.color = Color.white;
		}
		if (m_SUI.GradeBG != null) {
			m_SUI.GradeBG.gameObject.SetActive(m_Idx != BaseValue.COMMON_PERSONNELFILE_IDX);
			m_SUI.GradeBG.sprite = m_TData.m_Type == ItemType.CharaterPiece ? BaseValue.CharBG(m_Grade) : BaseValue.ItemGradeBG(m_TData.m_Type, m_Grade);
		}
		if (m_SUI.Shadow != null) m_SUI.Shadow.enabled = m_TData.m_Type != ItemType.Zombie && m_TData.m_Type != ItemType.DNA;
		if (m_SUI.FrameGlow != null) {
			m_SUI.FrameGlow.SetActive(m_Grade >= 6);
			m_SUI.FrameGlowImg.color = BaseValue.GradeFrameGlowColor(m_Grade);
		}
		SetCnt(m_LV, _cnt);
		if(m_SUI.SetEquipIcon != null) m_SUI.SetEquipIcon.SetActive(m_TData.GetInvenGroupType() == ItemInvenGroupType.Equipment && m_TData.m_Value > 0);
		if (m_SUI.CharPieceDefault != null) m_SUI.CharPieceDefault.SetActive(m_Idx == BaseValue.COMMON_PERSONNELFILE_IDX);
		if (m_SUI.Lock != null)
		{
			m_SUI.LockBG.SetActive(LockMode == LockActiveMode.Block);
			m_SUI.Lock.SetActive(LockMode != LockActiveMode.None);
		}
	}

	public void SetCnt(int lv, int? Cnt)
	{
		if (m_TData.GetEquipType() == EquipType.End)
		{
			if (m_SUI.Count != null)
			{
				if(Cnt != null && Cnt.Value > 0) {
					m_SUI.Count.gameObject.SetActive(true);
					m_SUI.Count.text = Cnt.ToString();
				}
				else
					m_SUI.Count.gameObject.SetActive(false);
			}
			if (m_SUI.LV.Active != null) m_SUI.LV.Active.gameObject.SetActive(false);
		}
		else
		{

			if (m_SUI.Count != null) m_SUI.Count.gameObject.SetActive(false);
			if (m_SUI.LV.Active != null)
			{
				m_SUI.LV.Active.gameObject.SetActive(true);
				m_SUI.LV.LV[0].text = string.Format("<size=80%>Lv. </size>{0}", lv);
				m_SUI.LV.LV[1].text = string.Format("<size=80%><color=#aaaaaa>Lv. </color></size>{0}", lv);
			}
		}
	}

	public void SetSell_Cnt(int cnt, int max)
	{
		if (m_TData.GetInvenGroupType() != ItemInvenGroupType.Equipment)
		{
			if (m_SUI.Count != null)
			{
				m_SUI.Count.gameObject.SetActive(true);
				m_SUI.Count.text = string.Format("{0} / {1}", cnt, max);
			}
		}
	}
	public void SetMaterial(Material _mat) {
		m_SUI.Icon.material = m_SUI.Frame.material = _mat;
	}
	public virtual void ClickCard()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Item_ToolTip, m_Idx)) return;
		if (m_CB != null) m_CB.Invoke(m_Item);
		else if (m_TData != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}

	public RES_REWARD_BASE GetRewardInfo()
	{
		var res = new RES_REWARD_ITEM();
		res.Idx = m_Idx;
		res.Grade = m_Grade;
		res.UID = m_Item != null ? m_Item.m_Uid : 0;
		return res;
	}
}
