using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_PDA_RNA_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Image Icon;
		public Image Frame;
		public Image CntBG;
		public TextMeshProUGUI CntTxt;
	}
	[SerializeField] SUI m_SUI;
	TItemTable m_TData { get { return TDATA.GetItemTable(m_Idx); } }
	int m_Idx;

	public void SetData(int _idx, float _cnt) {
		m_Idx = _idx;
		m_SUI.Icon.sprite = TDATA.GetItemTable(_idx).GetItemImg();
		m_SUI.CntTxt.text = _cnt.ToString("N1");

		Color[] color = BaseValue.RNAFrameColor(_idx);
		m_SUI.Frame.color = color[1];
		m_SUI.CntBG.color = color[2];
	}
	public void ClickView() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_Item_Card, m_Idx)) return;
		if (TDATA.GetItemTable(m_Idx) != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}
	public RES_REWARD_BASE GetRewardInfo() {
		var res = new RES_REWARD_ITEM();
		res.Idx = m_TData.m_Idx;
		res.Grade = m_TData.m_Grade;
		return res;
	}
}
