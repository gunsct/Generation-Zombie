using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Auction_Bid : PopupBase
{
#pragma warning disable 0649
	[Serializable]
	public struct SWarningUI
	{
		public GameObject Active;
		public TextMeshProUGUI Notice;
	}
	[Serializable]
	public struct SPriceUI
	{
		public Image BG;
		public Image Line;
		public TextMeshProUGUI Price;
		public TMP_InputField Input;
		public Color[] colBG;
		public Color[] colText;
		public Color[] colImg;
	}
	[Serializable]
	public struct SUI
	{
		public Animator Ani;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Price;
		public SPriceUI MyPrice;
		public SWarningUI Warning;
	}
	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	AuctionItem Item;
	long MinPrice;
	Action<Action> BuyCB;
	/// <summary> 연출에 의한 클릭 막기 </summary>
	protected bool m_ISAniPlay { get { return m_IsAniTransition != null || Utile_Class.IsAniPlay(m_sUI.Ani); } }
	IEnumerator m_IsAniTransition = null;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		Item = (AuctionItem)aobjValue[0];
		MinPrice = Item.GetMinBuyPrice();
		base.SetData(pos, popup, cb, aobjValue);
		if (m_Popup == PopupName.Auction_BidNow) m_sUI.Title.text = TDATA.GetString(706);
		else m_sUI.Title.text = TDATA.GetString(707);
	}

	public void SetBuyCallBack(Action<Action> CB)
	{
		BuyCB = CB;
	}

	public override void SetUI()
	{
		base.SetUI();
		SetPrice(MinPrice);

		m_sUI.Price.text = Utile_Class.CommaValue(Item.GetPrice());
		m_sUI.Item.SetData(Item.GetReward(), (obj) => { }, false);
		CheckWarning();
	}

	public void OnValueChange()
	{
		CheckWarning();
	}

	void CheckWarning()
	{
		long price = GetPrice();
		SetPrice(price);

		if (m_sUI.Warning.Active == null) return;
		string notice = string.Format(TDATA.GetString(712), Utile_Class.CommaValue(MinPrice));
		m_sUI.Warning.Notice.text = notice;
	}

	void SetPrice(long Price)
	{
		if (m_Popup == PopupName.Auction_BidNow) m_sUI.MyPrice.Price.text = Utile_Class.CommaValue(Price);
		else m_sUI.MyPrice.Input.text = Utile_Class.CommaValue(Price);

		int colpos = 0;
		if (USERINFO.m_Cash < Price) colpos = 1;
		if(m_sUI.MyPrice.BG != null) m_sUI.MyPrice.BG.color = m_sUI.MyPrice.colBG[colpos];
		m_sUI.MyPrice.Price.color = m_sUI.MyPrice.colText[colpos];
		if (m_sUI.MyPrice.Line != null) m_sUI.MyPrice.Line.color = m_sUI.MyPrice.colImg[colpos];
	}

	public long GetPrice()
	{
		string temp = m_sUI.MyPrice.Price.text;
		// 텍스트의 맨뒤에 쓰레기가 들어있음
		string replace = temp.Substring(0, temp.Length - 1).Replace(",", "");

		long price = 0;
		long.TryParse(replace, out price);
		return Math.Max(MinPrice, price);
	}

	public void OnYes()
	{
		BuyCB?.Invoke(() => { Close(1); });
	}

	public override void Close(int Result = 0)
	{
		if (m_IsAniTransition != null) return;
		StartCoroutine(Close((EMsgBtn)Result));
	}

	public IEnumerator Close(EMsgBtn btn)
	{
		m_IsAniTransition = AniInTransitionCheck();
		StartCoroutine(m_IsAniTransition);
		m_sUI.Ani.SetTrigger("Close");
		yield return new WaitWhile(() => m_ISAniPlay);
		base.Close((int)btn);
	}
	IEnumerator AniInTransitionCheck()
	{
		// 애니가 시작된후에는 한프레임 쉬어주어야함
		yield return new WaitForFixedUpdate();
		m_IsAniTransition = null;
	}
}
