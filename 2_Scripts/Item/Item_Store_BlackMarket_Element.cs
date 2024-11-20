using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;

public class Item_Store_BlackMarket_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_RewardList_Item Reward;
		public Item_Store_Buy_Button Btn;
		public GameObject DoubleMark;
	}
	[SerializeField] SUI m_SUI;
	string m_AnimStr = string.Empty;
	public TShopTable m_TData;
	Action<TShopTable> m_CB;
	bool IsStart = false;

	private void OnEnable() {
		IsStart = true;
		if (!string.IsNullOrEmpty(m_AnimStr)) m_SUI.Anim.SetTrigger(m_AnimStr); ;
	}
	public void SetData(TShopTable _tdata, RES_REWARD_BASE _res, bool _bought, Action<TShopTable> _cb) {
		m_TData = _tdata;
		m_CB = _cb;
		m_AnimStr = _bought ? "SoldOut" : "Act";
		if(IsStart) m_SUI.Anim.SetTrigger(m_AnimStr);

		m_SUI.Reward.SetData(_res, null, false);
		m_SUI.Btn.SetData(_tdata.m_Idx);
		m_SUI.DoubleMark.SetActive(_tdata.m_TagType == TagType.DOUBLE);
	}

	public void ClickBuy() {
		m_CB?.Invoke(m_TData);
	}
}
