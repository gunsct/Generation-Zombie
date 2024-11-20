using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Item_Store_Tab_Auction : Item_Store_Tab_Base
{
#pragma warning disable 0649
	[Serializable]
	public struct SUI
	{
		public float Gap;
		public GameObject MovePanel;
		public Animator Ani;

		public TextMeshProUGUI MinPrice;

		public GameObject[] Btns;
	}

	[SerializeField] SUI m_sUI;
#pragma warning restore 0649
	AuctionInfo m_Now { get { return USERINFO.m_Auction; } }
	AuctionInfo m_Befor = new AuctionInfo();

	bool m_IsMove;
	int m_Page;
	public override void SetData(Action CB)
	{
		m_Page = 0;
		ReLoad(() => {
			SetUI();
			CB?.Invoke();
			m_IsMove = true;
			iTween.ValueTo(gameObject, iTween.Hash("from", -1183f, "to", -182f, "time", 0.26f, "easetype", "easeOutQuad", "onupdate", "MovePanel"));
			iTween.ValueTo(gameObject, iTween.Hash("from", -182f, "to", 0, "time", 0.51f, "delay", 0.26f, "easetype", "easeOutQuad", "onupdate", "MovePanel", "oncomplete", "StartMoveEnd"));
			USERINFO.SetCheckNewAuctionGoods(m_Now);
		});
	}

	void ReLoad(Action CB)
	{
		m_Befor = USERINFO.m_Auction;
		USERINFO.m_Auction = new AuctionInfo();
		m_Now.Load(CB, true);
	}

	public override void SetUI()
	{
		m_IsMove = false;
		RectTransform rtf = (RectTransform)m_sUI.MovePanel.transform;
		rtf.anchoredPosition = Vector2.zero;
		int cnt = m_Now.Items.Count;
		if (cnt < 2)
		{
			m_sUI.Btns[0].SetActive(false);
			m_sUI.Btns[1].SetActive(false);
		}
		if (cnt < 1)
		{
			// 아직 리스트가 없을때
			float x = -m_sUI.Gap;
			for (int i = 0; i < m_sUI.MovePanel.transform.childCount; i++)
			{
				rtf = (RectTransform)m_sUI.MovePanel.transform.GetChild(i);
				rtf.GetComponent<Item_Store_LoopPanel>().SetData(null, null, null);
				rtf.anchoredPosition = new Vector2(x, 0);
				x += m_sUI.Gap;
			}
			return;
		}
		m_sUI.Btns[0].SetActive(true);
		m_sUI.Btns[1].SetActive(true);
		int Pos = m_Page;
		// 현재
		AuctionItem item = m_Now.Items[Pos];
		Item_Store_LoopPanel obj = m_sUI.MovePanel.transform.GetChild(1).GetComponent<Item_Store_LoopPanel>();
		((RectTransform)obj.transform).anchoredPosition = Vector2.zero;
		obj.SetData(item, m_Befor.Items.Find(o => o.m_Uid == item.m_Uid), OnReset);

		// 이전
		Pos = (m_Now.Items.Count + (m_Page - 1)) % m_Now.Items.Count;
		item = m_Now.Items[Pos];
		obj = m_sUI.MovePanel.transform.GetChild(0).GetComponent<Item_Store_LoopPanel>();
		((RectTransform)obj.transform).anchoredPosition = new Vector2(-m_sUI.Gap, 0);
		obj.SetData(item, m_Befor.Items.Find(o => o.m_Uid == item.m_Uid), OnReset);

		// 다음
		Pos = (m_Page + 1) % m_Now.Items.Count;
		item = m_Now.Items[Pos];
		obj = m_sUI.MovePanel.transform.GetChild(2).GetComponent<Item_Store_LoopPanel>();
		((RectTransform)obj.transform).anchoredPosition = new Vector2(m_sUI.Gap, 0);
		obj.SetData(item, m_Befor.Items.Find(o => o.m_Uid == item.m_Uid), OnReset);

		SetPriceUI();
	}

	void SetPriceUI()
	{
		m_sUI.MinPrice.text = Utile_Class.CommaValue(m_Now.Items[m_Page].GetMinBuyPrice());
	}

	public void StartMoveEnd()
	{
		m_IsMove = false;
		m_sUI.MovePanel.transform.GetChild(0).GetComponent<Item_Store_LoopPanel>().StartChangeAction();
	}

	int GetLeftPos()
	{
		return (m_Now.Items.Count + (m_Page - 1)) % m_Now.Items.Count;
	}

	public void OnLeft()
	{
		if (m_Now.Items.Count < 2) return;
		if (m_IsMove) return;
		m_IsMove = true;
		m_Page = GetLeftPos();

		// 맨 뒤에거 앞으로 넣어주기
		RectTransform rtf = (RectTransform)m_sUI.MovePanel.transform;
		RectTransform change = (RectTransform)rtf.GetChild(2);
		change.anchoredPosition = new Vector2(((RectTransform)rtf.GetChild(0)).anchoredPosition.x - m_sUI.Gap, 0);
		change.SetSiblingIndex(0);

		// 데이터 셋팅
		int Pos = GetLeftPos();
		AuctionItem item = m_Now.Items[Pos];
		change.GetComponent<Item_Store_LoopPanel>().SetData(item, m_Befor.Items.Find(o => o.m_Uid == item.m_Uid), OnReset);

		SetPriceUI();
		// 이동
		PLAY.PlayEffSound(SND_IDX.SFX_1901);
		m_sUI.Ani.SetTrigger("Change");
		iTween.ValueTo(gameObject, iTween.Hash("from", rtf.anchoredPosition.x, "to", rtf.anchoredPosition.x + m_sUI.Gap, "time", 0.5f, "easetype", "easeOutQuad", "onupdate", "MovePanel", "oncomplete", "MoveEnd"));
	}

	int GetRightPos()
	{
		return (m_Page + 1) % m_Now.Items.Count;
	}
	public void OnRight()
	{
		if (m_Now.Items.Count < 2) return;
		if (m_IsMove) return;
		m_IsMove = true;
		m_Page = GetRightPos();
		// 가장 앞에거 뒤로 넣기
		RectTransform rtf = (RectTransform)m_sUI.MovePanel.transform;
		RectTransform change = (RectTransform)rtf.GetChild(0);
		change.anchoredPosition = new Vector2(((RectTransform)rtf.GetChild(2)).anchoredPosition.x + m_sUI.Gap, 0);
		change.SetSiblingIndex(2);

		// 데이터 셋팅
		int Pos = GetRightPos();
		AuctionItem item = m_Now.Items[Pos];
		change.GetComponent<Item_Store_LoopPanel>().SetData(item, m_Befor.Items.Find(o => o.m_Uid == item.m_Uid), OnReset);

		SetPriceUI();
		// 이동
		PLAY.PlayEffSound(SND_IDX.SFX_1901);
		m_sUI.Ani.SetTrigger("Change");
		iTween.ValueTo(gameObject, iTween.Hash("from", rtf.anchoredPosition.x, "to", rtf.anchoredPosition.x - m_sUI.Gap, "time", 0.5f, "easetype", "easeOutQuad", "onupdate", "MovePanel", "oncomplete", "MoveEnd"));
	}

	void MovePanel(float value)
	{
		RectTransform rtf = (RectTransform)m_sUI.MovePanel.transform;
		rtf.anchoredPosition = new Vector2(value, 0);
	}

	void MoveEnd()
	{
		m_IsMove = false;
		m_sUI.MovePanel.transform.GetChild(1).GetComponent<Item_Store_LoopPanel>().StartChangeAction();
	}

	public void OnBuy(int Mode)
	{
		if (m_IsMove) return;
		if (m_Now.Items.Count < 1) return;

		var Item = m_Now.Items[m_Page];
		PopupName popup = Mode == 1 ? PopupName.Auction_Bid : PopupName.Auction_BidNow;
		Auction_Bid ui = POPUP.Set_Popup(PopupPos.POPUPUI, popup, (result, obj) => {
			if (result == 1)
			{
				SetUI();
				MoveEnd();
			}
		}, Item).GetComponent<Auction_Bid>();
		ui.SetBuyCallBack((cb) => {
			long price = ui.GetPrice();
			if (USERINFO.m_Cash < price)
			{
				POPUP.StartLackPop(BaseValue.CASH_IDX);
				return;
			}
			if(Item.Times[1] < UTILE.Get_ServerTime_Milli())
			{
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(5016));
				ui.Close(0);
				return;
			}

			if (Item.UserNo == USERINFO.m_UID)
			{
				POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(5007), string.Format(TDATA.GetString(5008), Utile_Class.CommaValue(price)), (btn, obj) => {
					if((EMsgBtn)btn == EMsgBtn.BTN_YES) Buy(ui, price, cb);
				});
				return;
			}
			Buy(ui, price, cb);
		});
	}

	void Buy(Auction_Bid ui, long price, Action cb)
	{
		var Item = m_Now.Items[m_Page];
		WEB.SEND_REQ_AUCTION_BUY((res) => {
			if (!res.IsSuccess())
			{
				switch (res.result_code)
				{
				case EResultCode.ERROR_AUCTION_PRICE:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(5015));
					break;
				case EResultCode.ERROR_AUCTION_TIME:
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(5016));
					break;
				case EResultCode.ERROR_CASH:
					POPUP.StartLackPop(BaseValue.CASH_IDX);
					break;
				default:
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					break;
				}
				ReLoad(() => { USERINFO.SetCheckNewAuctionGoods(m_Now); 
					ui.Close(0); });
				return;
			}

			PLAY.PlayEffSound(SND_IDX.SFX_1801);

			// 구매자 정보 변경
			m_Now.Items[m_Page].SetDATA(res.Info);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(5042));
			cb?.Invoke();
		}, m_Now.Items[m_Page].m_Uid, price);
	}

	public void OnReset()
	{
		ReLoad(() =>
		{
			PlayEffSound(SND_IDX.SFX_0012);
			SetUI();
			MoveEnd();
			var item = m_Now.Items[m_Page];
			var befor = m_Befor.Items.Find(o => o.m_Uid == item.m_Uid);
			if (befor == null) return;
			if (item.GetPrice() == befor.GetPrice() && item.UserNo == befor.UserNo) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(715)); 
			USERINFO.SetCheckNewAuctionGoods(m_Now);
		});
	}

	public void OnBuyGuide()
	{
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Msg_Auction_Guide);
	}
}
