using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_MatItem_Card : Item_Item_Card
{
	[System.Serializable]
	public struct MUI
	{
		public GameObject FrameLack;
		public TextMeshProUGUI[] MatCount;
	}
	[SerializeField]
	MUI m_MUI;

	public override void SetData(int _idx, int _needcnt, int _grade = 1) {
		m_Idx = _idx;
		m_SUI.Icon.sprite = m_TItem.GetItemImg();
		m_SUI.Frame.sprite = BaseValue.GradeFrame(m_TItem.m_Type, _grade, _idx);
		if (m_TItem.m_Type == ItemType.DNAMaterial) {
			m_SUI.Frame.color = BaseValue.RNAFrameColor(m_Idx)[0];
		}

		int had = USERINFO.GetItemCount(m_Idx);
		m_MUI.MatCount[0].text = Utile_Class.CommaValue(had);
		m_MUI.MatCount[1].text = Utile_Class.CommaValue(_needcnt);

		m_MUI.FrameLack.SetActive(had < _needcnt);
		m_MUI.MatCount[0].color = BaseValue.GetUpDownStrColor(had, _needcnt);
	}
}
