using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

[System.Serializable] public class DicSortingSprite : SerializableDictionary<SortingType, Sprite> { }
[System.Serializable] public class DicSortingUseType : SerializableDictionary<SortingType, bool> { }
public class Item_SortingGroup : ObjMng
{
	public enum Mode
	{
		Normal = 0,
		UnionMember
	}

	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Grade;
		public Image Icon;
		public DicSortingSprite Imgs;
		public DicSortingUseType UseType;
		public GameObject[] OrderBtn;
	}
	[SerializeField]
	SUI m_SUI;

	public SortingType m_Condition;
	/// <summary>
	/// 오름차순(▲) : 1,2,3,4,5,6,7
	/// <para>내림차순(▼) : 7,6,5,4,3,2,1</para>
	/// </summary>
	public bool m_Ascending;
	Action m_CB;
	Mode m_SortingMode;

	private void Awake() {
		SetIconName();
		m_Ascending = false;
		m_SUI.OrderBtn[0].SetActive(m_Ascending);
		m_SUI.OrderBtn[1].SetActive(!m_Ascending);
	}
	public void SetData(Action _cb, Mode mode = Mode.Normal) {
		m_CB = _cb;
		m_SortingMode = mode;
	}
	public void ClickSortingCondition()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Sorting, 0)) return;
		var popup = m_SortingMode == Mode.UnionMember ? PopupName.SortingOption_Union_Member : PopupName.SortingOption;
		POPUP.Set_Popup(PopupPos.POPUPUI, popup, (result, obj) => {
			m_Condition = (SortingType)result;
			SetIconName();
			m_CB?.Invoke();
		}, m_Condition, m_SUI.UseType);
	}

	void SetIconName() {
		switch (m_Condition) {
			case SortingType.Grade: m_SUI.Grade.text = TDATA.GetString(274); break;
			case SortingType.CombatPower: m_SUI.Grade.text = TDATA.GetString(275); break;
			case SortingType.Level: m_SUI.Grade.text = TDATA.GetString(36); break;
			case SortingType.Men: m_SUI.Grade.text = TDATA.GetString(1601); break;
			case SortingType.Hyg: m_SUI.Grade.text = TDATA.GetString(1621); break;
			case SortingType.Sat: m_SUI.Grade.text = TDATA.GetString(1611); break;
			case SortingType.Name: m_SUI.Grade.text = TDATA.GetString(6046); break;
			case SortingType.Point: m_SUI.Grade.text = TDATA.GetString(6047); break;
			case SortingType.Time: m_SUI.Grade.text = TDATA.GetString(6048); break;
			case SortingType.Job: m_SUI.Grade.text = TDATA.GetString(88); break;
		}
		if(m_SUI.Icon != null)m_SUI.Icon.sprite = m_SUI.Imgs[m_Condition];
	}
	public void ClickSortOrder()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Info_Sorting, 1)) return;
		m_Ascending = !m_Ascending;
		m_SUI.OrderBtn[0].SetActive(m_Ascending);
		m_SUI.OrderBtn[1].SetActive(!m_Ascending);
		m_CB?.Invoke();
	}
}
