using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Mk_Element_Char : Item_Mk_Element_Parent
{
   [Serializable]
   public struct SUI
	{
		public Image Portrait;
		public Image GradeBG;
		public TextMeshProUGUI Name;
		public GameObject Dot;
	}

	[SerializeField] SUI m_SUI;
	/// <summary> 태생 등급 </summary>
	public int GetGrade() {
		return TDATA.GetCharacterTable(TDATA.GetItemTable(TDATA.GetGachaItemList(TDATA.GetItemTable(m_Mk_TData.m_ItemIdx))[0].GetIdx()).m_Value).m_Grade;
	}

	public void SetData(TMakingTable _tdata, Action<Item_Mk_Element_Parent> _cb) {
		m_Mk_TData = _tdata;
		m_CB = _cb;

		m_SUI.Portrait.sprite = m_Item_TData.GetItemImg();
		m_SUI.GradeBG.sprite = BaseValue.CharBG(GetGrade());
		m_SUI.Name.text = m_Item_TData.GetName();
		m_SUI.Dot.SetActive(m_Mk_TData.GetCanMake());
	}

	public override void SetState(TimeContentState _advstate) {
		if (m_Info != null) {
			m_State = _advstate;
		}
	}

	/// <summary> 카드 선택시 제작 위해 인덱스 반환 </summary>
	public void ClickBtn() {
		m_CB?.Invoke(this);
	}
}
