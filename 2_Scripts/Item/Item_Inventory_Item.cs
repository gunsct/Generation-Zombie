using Newtonsoft.Json;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static Item_RewardItem_Card;
using static LS_Web;

public class Item_Inventory_Item : Item_RewardList_Item
{
	[System.Serializable]
	public struct SPriceUI
	{
		public GameObject Active;
		public TextMeshProUGUI Price;
	}
	[System.Serializable]
	public struct SSellUI
	{
		public GameObject Active;
		public SPriceUI Price;
		public GameObject Minus;
	}
	[System.Serializable]
	public struct SNotSellUI
	{
		public GameObject Active;
	}

	[SerializeField] SSellUI m_Sell;
	[SerializeField] SNotSellUI m_NotSell;
	[SerializeField] GameObject m_NewAlarm;

	public ItemInfo m_ItemInfo;
	public ItemInfo m_UpgradeItemInfo;
	public DNAInfo m_DNAInfo;
	Inventory.EState m_State;
	Func<ItemInfo, int, int> m_ChangeCB;

	public bool GetNotSelect { get { return m_NotSell.Active.activeSelf; } }
	private void Awake() {
		SetNewAlarm(false);
	}

	public Action<Inventory.EState> SetData(ItemInfo data, Func<ItemInfo, int, int> changecb, Action<GameObject> selectcb, LockActiveMode LockMarkMode = LockActiveMode.Normal, ItemInfo _upgradeitem = null)
	{
		m_ItemInfo = data;
		m_UpgradeItemInfo = _upgradeitem;
		m_ChangeCB = changecb;
		m_SelectCB = selectcb;
		TItemTable tdata = TDATA.GetItemTable(data.m_Idx);
		m_SUI.DNA.gameObject.SetActive(false);
		if (LockMarkMode != LockActiveMode.None && !data.m_Lock) LockMarkMode = LockActiveMode.None;

		switch (tdata.m_Type)
		{
		case ItemType.CharaterPiece:
			m_SUI.Item.gameObject.SetActive(false);
			m_SUI.Piece.gameObject.SetActive(true);
			m_SUI.Piece.SetData(data.m_Idx, data.m_Stack, LockMode: LockMarkMode);
			break;
		default:
			m_SUI.Item.gameObject.SetActive(true);
			m_SUI.Piece.gameObject.SetActive(false);
			m_SUI.Item.SetData(data.m_Idx, data.m_Stack, data.m_Lv, tdata.GetEquipType() == EquipType.End ? tdata.m_Grade : data.m_Grade, LockMode: LockMarkMode);
			break;
		}
		return StateChange;
	}
	public Action<Inventory.EState> SetData(DNAInfo data, Action<GameObject> selectcb) {
		m_DNAInfo = data;
		m_ChangeCB = null;
		m_SelectCB = selectcb;

		m_SUI.Item.gameObject.SetActive(false);
		m_SUI.Piece.gameObject.SetActive(false);
		m_SUI.DNA.gameObject.SetActive(true);
		m_SUI.DNA.SetData(m_DNAInfo.m_Idx, -1, m_DNAInfo.m_Lv, m_DNAInfo.m_UID);
		return StateChange;
	}
	public void StateChange(Inventory.EState state)
	{
		m_State = state;
		if(m_ItemInfo != null) m_ItemInfo.m_TempCnt = 0;
		switch(m_State)
		{
		case Inventory.EState.Sell:
			m_SUI.Event.enabled = false;
			if (m_ItemInfo.m_TData.GetPrice() < 1 || m_ItemInfo.m_Lock)
			{
				m_Sell.Active.SetActive(false);
				m_NotSell.Active.SetActive(true);
			}
			else
			{
				m_Sell.Active.SetActive(true);
				m_NotSell.Active.SetActive(false);
			}
			break;
		case Inventory.EState.EqLVUP:
			m_SUI.Event.enabled = false;
			m_Sell.Active.SetActive(true);
			m_NotSell.Active.SetActive(m_ItemInfo.m_NotSelect);
			break;
		default:
			m_SUI.Event.enabled = false;
			//m_SUI.Event.enabled = true;
			m_Sell.Active.SetActive(false);
			m_NotSell.Active.SetActive(false);
			break;
		}
		if (m_ItemInfo != null) {
			SetCntUI();
		}
	}

	public bool OnPlus(int Add) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.EquipLevelUp, 0)) return false;
		if (m_ChangeCB == null) return false;
		int temp = Mathf.Max(0, m_ItemInfo.m_Stack - m_ItemInfo.m_TempCnt);
		if (temp < 1) return false;

		Add = Mathf.Min(Add, temp);
		if (Add < 1) return false;
		Add = m_ChangeCB.Invoke(m_ItemInfo, Add);
		if (Add <= 0) return false;
		m_ItemInfo.m_TempCnt += Add;
		if (m_ItemInfo.m_TempCnt >= m_ItemInfo.m_Stack) m_ItemInfo.m_TempCnt = m_ItemInfo.m_Stack;
		SetCntUI();
		return true;
	}

	public bool OnMinus(int Add) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.EquipLevelUp, 0)) return false;
		if (m_ChangeCB == null) return false;
		if (m_ItemInfo.m_TempCnt < 1) return false;
		Add = Mathf.Min(Add, m_ItemInfo.m_TempCnt);
		if (Add < 1) return false;
		Add = m_ChangeCB.Invoke(m_ItemInfo, -Add);
		if (Add >= 0) return false;
		m_ItemInfo.m_TempCnt += Add;
		if (m_ItemInfo.m_TempCnt < 1) m_ItemInfo.m_TempCnt = 0;
		SetCntUI();
		return true;
	}

	public void SetCntUI()
	{
		switch(m_State)
		{
		case Inventory.EState.Sell:
			if (!m_Sell.Active.activeSelf) break;
			if(m_ItemInfo.m_TempCnt < 1)
			{
				if (m_Sell.Price.Active.activeSelf) m_Sell.Price.Active.SetActive(false);
				if (m_Sell.Minus.activeSelf) m_Sell.Minus.SetActive(false);
				switch (m_ItemInfo.m_TData.m_Type)
				{
				case ItemType.CharaterPiece:
					m_SUI.Piece.SetCnt(m_ItemInfo.m_Lv, m_ItemInfo.m_Stack);
					break;
				default:
					m_SUI.Item.SetCnt(m_ItemInfo.m_Lv, m_ItemInfo.m_Stack);
					break;
				}
			}
			else
			{
				if (!m_Sell.Price.Active.activeSelf) m_Sell.Price.Active.SetActive(true);
				if (!m_Sell.Minus.activeSelf) m_Sell.Minus.SetActive(true);
				switch (m_ItemInfo.m_TData.m_Type)
				{
				case ItemType.CharaterPiece:
					m_SUI.Piece.SetSell_Cnt(m_ItemInfo.m_TempCnt, m_ItemInfo.m_Stack);
					break;
				default:
					m_SUI.Item.SetSell_Cnt(m_ItemInfo.m_TempCnt, m_ItemInfo.m_Stack);
					break;
				}
				m_Sell.Price.Price.text = Utile_Class.CommaValue(m_ItemInfo.GetCellPrice());
			}
			break;
		case Inventory.EState.EqLVUP:
			if (!m_Sell.Active.activeSelf) break;
			if (m_Sell.Price.Active.activeSelf) m_Sell.Price.Active.SetActive(false);
			if (m_Sell.Minus.activeSelf != m_ItemInfo.m_TempCnt > 0) m_Sell.Minus.SetActive(m_ItemInfo.m_TempCnt > 0);
			switch (m_ItemInfo.m_TData.m_Type)
			{
			case ItemType.CharaterPiece:
				m_SUI.Piece.SetSell_Cnt(m_ItemInfo.m_TempCnt, m_ItemInfo.m_Stack);
				break;
			default:
				m_SUI.Item.SetSell_Cnt(m_ItemInfo.m_TempCnt, m_ItemInfo.m_Stack);
				break;
			}
			break;
		default:
			switch (m_ItemInfo.m_TData.m_Type)
			{
			case ItemType.CharaterPiece:
				m_SUI.Piece.SetCnt(m_ItemInfo.m_Lv, m_ItemInfo.m_Stack);
				break;
			default:
				m_SUI.Item.SetCnt(m_ItemInfo.m_Lv, m_ItemInfo.m_Stack);
				break;
			}
			break;
		}
	}

	public int GetCellPrice(int? Cnt = null)
	{
		return m_ItemInfo.m_TData.GetPrice() * (Cnt == null ? m_ItemInfo.m_TempCnt : Cnt.Value);
	}

	public int GetExpPrice(int? Cnt = null)
	{
		int Price = 0;
		if (m_ItemInfo.m_TData.GetInvenGroupType() == ItemInvenGroupType.Equipment) Price = BaseValue.ITEM_EXP_MONEY(m_ItemInfo.m_TExpData.m_Exp[1]) * (Cnt == null ? 1 : (Cnt > 0 ? 1 : -1));
		else Price = BaseValue.ITEM_EXP_MONEY(m_ItemInfo.m_TData.m_Value * (Cnt == null ? m_ItemInfo.m_TempCnt : Cnt.Value));


		float per = 1f;
		switch (m_ItemInfo.m_TData.GetEquipType())
		{
		case EquipType.Weapon: per -= USERINFO.ResearchValue(ResearchEff.WeaponSale); break;
		case EquipType.Helmet: per -= USERINFO.ResearchValue(ResearchEff.HelmetSale); break;
		case EquipType.Costume: per -= USERINFO.ResearchValue(ResearchEff.CostumeSale); break;
		case EquipType.Shoes: per -= USERINFO.ResearchValue(ResearchEff.ShoesSale); break;
		case EquipType.Accessory: per -= USERINFO.ResearchValue(ResearchEff.AccSale); break;
		}

		return (int)(Price * per);
	}

	public void OnMinus_Press()
	{
		switch (m_State)
		{
		case Inventory.EState.Normal:
			return;
		}
		StartCoroutine("Action_Minus");
	}

	public void OnMinus_Release()
	{
		switch (m_State)
		{
		case Inventory.EState.Normal:
			return;
		}
		StopCoroutine("Action_Minus");
	}

	public void OnMinus_Click()
	{
		OnMinus(1);
	}


	IEnumerator Action_Minus()
	{
		float nexttime = 0.5f;
		int Add = 1;
		yield return new WaitForSeconds(nexttime);
		while (OnMinus(Add))
		{
			nexttime = Mathf.Clamp(nexttime - 0.1f, 0.1f, 0.5f);
			Add <<= 1;
			yield return new WaitForSeconds(nexttime);
		}
	}

	public void OnPlus_Press()
	{
		switch(m_State)
		{
		case Inventory.EState.Normal:
			return;
		}
		StartCoroutine("Action_Plus");
	}

	public void OnPlus_Release()
	{
		switch (m_State)
		{
		case Inventory.EState.Normal:
			return;
		}
		StopCoroutine("Action_Plus");
	}
	public void OnPlus_Click()
	{
		OnPlus(1);
	}

	IEnumerator Action_Plus()
	{
		float nexttime = 0.5f;
		int Add = 1;
		yield return new WaitForSeconds(nexttime);

		while (OnPlus(CheckAddCnt(Add)))
		{
			nexttime = Mathf.Clamp(nexttime - 0.1f, 0.1f, 0.5f);
			Add <<= 1;
			yield return new WaitForSeconds(nexttime);
		}
	}
	int CheckAddCnt(int _add) {
		int add = _add;
		int pricecnt = 0;
		if (m_State == Inventory.EState.EqLVUP) {
			float per = 1f;
			per -= USERINFO.GetSkillValue(SkillKind.EquipLevelUpSale);
			if (m_UpgradeItemInfo != null) {
				switch (m_UpgradeItemInfo.m_TData.GetEquipType()) {
					case EquipType.Weapon: per -= USERINFO.ResearchValue(ResearchEff.WeaponSale); break;
					case EquipType.Helmet: per -= USERINFO.ResearchValue(ResearchEff.HelmetSale); break;
					case EquipType.Costume: per -= USERINFO.ResearchValue(ResearchEff.CostumeSale); break;
					case EquipType.Shoes: per -= USERINFO.ResearchValue(ResearchEff.ShoesSale); break;
					case EquipType.Accessory: per -= USERINFO.ResearchValue(ResearchEff.AccSale); break;
				}
			}
			pricecnt = Mathf.RoundToInt((USERINFO.m_Money - m_ItemInfo.m_TData.GetPrice()) / (m_ItemInfo.GetExpPrice(1) * per * Mathf.Max(1, m_ItemInfo.m_TempCnt)));
			if (pricecnt < add) {
				add = pricecnt;
			}
		}
		return add;
	}
	public void SetNewAlarm(bool _new) {
		m_NewAlarm.SetActive(_new);
	}
}
