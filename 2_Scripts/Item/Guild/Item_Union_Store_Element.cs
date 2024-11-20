using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Union_Store_Element : ObjMng
{
	public enum State
	{
		Normal = 0,
		SoldOut,
		Lock
	}
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public Item_RewardList_Item Item;
		public Item_Store_Buy_Button Price;

		public TextMeshProUGUI Cnt;
		public TextMeshProUGUI Lock;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	bool IsStart;
	State m_State;
	public TShopTable m_Tdata;
	Action<Item_Union_Store_Element> m_ClickCB;
	void OnEnable()
	{
		StartCoroutine(StartCheck());
	}

	public void SetData(TShopTable tdata, Action<Item_Union_Store_Element> CB)
	{
		m_Tdata = tdata;
		m_ClickCB = CB;
		SetUI();
	}

	IEnumerator StartCheck()
	{
		IsStart = false;
		m_sUI.Ani.SetTrigger(m_State.ToString());
		yield return Utile_Class.CheckAniPlay(m_sUI.Ani);
		IsStart = true;
	}

	public void SetUI()
	{
		m_sUI.Item.SetData(m_Tdata.m_Rewards[0].m_ItemType, m_Tdata.m_Rewards[0].m_ItemIdx, m_Tdata.m_Rewards[0].m_ItemCnt, 1, 0, null, false);

		var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_Tdata.m_Idx);
		if (buyinfo == null)
		{
			buyinfo = new RES_SHOP_USER_BUY_INFO() { Idx = m_Tdata.m_Idx, Cnt = 0 };
			USERINFO.m_ShopInfo.BUYs.Add(buyinfo);
		}

		m_sUI.Cnt.text = $"{buyinfo.Cnt} / {m_Tdata.m_LimitCnt}";
		m_sUI.Lock.text = string.Format(TDATA.GetString(6202), m_Tdata.m_Level);

		m_State = State.Normal;
		int LV;
		long Exp;
		USERINFO.m_Guild.Calc_Exp(out LV, out Exp);
		if (LV < m_Tdata.m_Level) m_State = State.Lock;
		else if(buyinfo.Cnt >= m_Tdata.m_LimitCnt) m_State = State.SoldOut;

		m_sUI.Price.SetData(m_Tdata.m_Idx, 1);
		m_sUI.Ani.SetTrigger(m_State.ToString());
	}

	public void OnClick()
	{
		if (!IsStart) return;
		if (m_State != State.Normal) return;
		m_ClickCB?.Invoke(this);
	}

}
