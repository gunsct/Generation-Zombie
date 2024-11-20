using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Item_EqChange : ObjMng
{

	[System.Serializable]
	public struct SCharUI
	{
		public GameObject Active;
		public Item_CharSmall_Card Char;
	}

	[System.Serializable]
	public struct SUI
	{
		public Item_Item_Card Item;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI CP;
		public SCharUI Char;
		public Item_Button Equip;
		public TextMeshProUGUI EquipLabel;
	}

	[SerializeField] SUI m_SUI;

	public ItemInfo m_Info;
	public CharInfo m_Char;
	Action<Item_EqChange, int> m_ChangeCB;

	public void SetData(ItemInfo data, Action<Item_EqChange, int> changecb)
	{
		m_Info = data;
		m_ChangeCB = changecb;

		m_SUI.Item.SetData(data);
		m_SUI.Name.text = data.m_TData.GetName();
		m_SUI.CP.text = Utile_Class.CommaValue(data.m_CP);

		m_Char = USERINFO.GetEquipChar(data.m_Uid);
		if (m_Char == null)
		{
			m_SUI.Char.Active.SetActive(false);
			m_SUI.Equip.SetActive(true, false, UIMng.BtnBG.Green);
			m_SUI.EquipLabel.text = TDATA.GetString(204);
		}
		else
		{
			m_SUI.Char.Active.SetActive(true);
			m_SUI.Char.Char.SetData(m_Char);
			m_SUI.Equip.SetActive(true, false, UIMng.BtnBG.Normal);
			m_SUI.EquipLabel.text = TDATA.GetString(251);
		}
	}


	public void OnChange()
	{
		m_ChangeCB?.Invoke(this, 0);
	}

	public void OnCheck()
	{
		m_ChangeCB?.Invoke(this, 1);
	}

	public GameObject GetBtn()
	{
		return m_SUI.Equip.gameObject;
	}

}
