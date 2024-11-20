using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable] public class DicItem_TopAsset : SerializableDictionary<Item_TopAsset.TopAsset, Item_MoneyGold> { }
public class Item_TopAsset : ObjMng
{
	public enum TopAsset
	{
		Money = 0,
		GoldTeeth,
		CharExp,
		PCoin,
		GCoin,
		Ticket,
		End
	}
	[SerializeField] DicItem_TopAsset m_Assets;

	public void ActiveAsset(TopAsset asset, bool Active)
	{
		if (m_Assets.ContainsKey(asset)) m_Assets[asset].gameObject.SetActive(Active);
	}

	public void Active_AutoCheck(bool Active)
	{
		for (TopAsset i = TopAsset.Money; i < TopAsset.End; i++) if (m_Assets.ContainsKey(i)) m_Assets[i].Active_AutoCheck(Active);
	}

	public void SetData(TopAsset asset, long _crntval, long _preval)
	{
		if (m_Assets.ContainsKey(asset)) m_Assets[asset].SetData(_crntval, _preval);
	}
}
