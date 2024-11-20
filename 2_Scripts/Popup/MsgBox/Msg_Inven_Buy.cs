using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Msg_Inven_Buy : MsgBoxBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Now;
		public TextMeshProUGUI Next;
		public TextMeshProUGUI Price;
		public Slider Cnt;
		public Color[] PriceColor;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		m_sUI.Cnt.minValue = 1;
		m_sUI.Cnt.maxValue = GetMaxCnt();
		m_sUI.Cnt.value = 1;
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI()
	{
		m_sUI.Title.text = TDATA.GetString(238);
		m_sUI.Now.text = string.Format("<size=50>{0}</size>\n{1}", TDATA.GetString(239), USERINFO.m_InvenSize);
		SetValue();
	}

	int GetMaxCnt()
	{
		return (BaseValue.INVEN_SLOT_MAX - USERINFO.m_InvenSize) / BaseValue.INVEN_BUY_CNT;
	}

	public void SetValue()
	{
		int Cnt = GetBuyCnt();
		int Size = USERINFO.m_InvenSize + (Cnt * BaseValue.INVEN_BUY_CNT);
		int Price = USERINFO.GetInvenPrice(Cnt);
		m_sUI.Next.text = string.Format("<size=50>{0}</size>\n{1}", TDATA.GetString(240), Size);
		m_sUI.Price.text = Utile_Class.CommaValue(Price);
		m_sUI.Price.color = m_sUI.PriceColor[Price > USERINFO.m_Cash ? 1 : 0];
	}

	public void OnPlus()
	{
		++m_sUI.Cnt.value;
	}

	public void OnMinus()
	{

		--m_sUI.Cnt.value;
	}

	public int GetBuyCnt()
	{
		return Mathf.RoundToInt(m_sUI.Cnt.value);
	}
}
