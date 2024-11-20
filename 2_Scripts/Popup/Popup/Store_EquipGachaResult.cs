using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using System.Linq;
using TMPro;
using UnityEngine.UI;

public class Store_EquipGachaResult : PopupBase
{
	public enum State
	{
		Ready,
		Reward,
		Result,
		End
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject RewardPrefab;
		public Transform Bucket;
		public TextMeshProUGUI BuyBtnTxt;
		public Item_Store_Buy_Button BuyBtn;
		public ScrollRect Scroll;
		public GameObject SkipBtn;
	}
	[SerializeField] SUI m_SUI;
	State m_State = State.Ready;
	int m_MaxGrade = 1;
	Action<int, TShopTable> m_RCB;
	IEnumerator m_RewardAction;
	IEnumerator m_Action;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	TShopTable m_TDataShop;
	public TShopTable GetTable { get { return m_TDataShop; } }

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_Rewards = (List<RES_REWARD_BASE>)aobjValue[0];
		m_TDataShop = (TShopTable)aobjValue[1];
		m_RCB = (Action<int, TShopTable>)aobjValue[2];
		PlayEffSound(SND_IDX.SFX_1011);

		var lvdata = USERINFO.GetEquipGachaLv();
		int lv = lvdata.GetLv + (lvdata.Exp - m_TDataShop.m_Rewards[0].m_ItemCnt < 0 ? -1 : 0);
		lv = Mathf.Max(1, lv);
		var group = TDATA.GetGachaGroup(TDATA.GetEquipGachaTable(lv).m_Gid).m_List;
		for(int i = 0; i < group.Count; i++) {
			TItemTable tdata = TDATA.GetItemTable(group[i].m_RewardIdx);
			if (m_MaxGrade < tdata.m_Grade) m_MaxGrade = tdata.m_Grade;
		}
		//var top = m_Rewards.OrderBy(o => o.GetGrade()).Last();//m_Rewards.OrderBy(o => TDATA.GetItemTable(o.GetIdx()).m_Grade).Last();
		//m_MaxGrade = top.GetGrade();//TDATA.GetItemTable(top.GetIdx()).m_Grade;

		UTILE.Load_Prefab_List(m_Rewards.Count, m_SUI.Bucket, m_SUI.RewardPrefab.transform);
		for(int i = 0; i < m_SUI.Bucket.childCount; i++) {
			Item_EquipGachaList_Item element = m_SUI.Bucket.GetChild(i).GetComponent<Item_EquipGachaList_Item>();
			element.AlphaOnOff(false);
			element.SetData(m_MaxGrade, m_Rewards[i]);//Item_EquipGachaList_Item 로 스크립트 하나 더
		}
		switch (m_TDataShop.m_Idx) {
			case 301:
			case 304:
			case 307:
				m_SUI.BuyBtnTxt.text = TDATA.GetString(681);//680, 678
				break;
			case 302:
			case 308:
				m_SUI.BuyBtnTxt.text = TDATA.GetString(680);//680, 678
				break;
			case 303:
			case 309:
				m_SUI.BuyBtnTxt.text = TDATA.GetString(678);//680, 678
				break;
		}

		//m_SUI.BuyBtn.gameObject.SetActive(m_TDataShop.GetPrice() > 0);
		if (m_TDataShop.m_PriceType == PriceType.AD_InitTime && !USERINFO.IS_CanBuy(m_TDataShop)) {
			m_TDataShop = TDATA.GetGroupShopTable(ShopGroup.ItemGacha).Find(o => o.m_PriceType == PriceType.Item && o.m_Rewards[0].m_ItemIdx == m_TDataShop.m_Rewards[0].m_ItemIdx && o.m_Rewards[0].m_ItemCnt == m_TDataShop.m_Rewards[0].m_ItemCnt);
		}
		else if(m_TDataShop.m_PriceType == PriceType.AD_AddTime) {
			if(m_TDataShop.GetPrice() > 0)
				m_TDataShop = TDATA.GetGroupShopTable(ShopGroup.ItemGacha).Find(o => o.m_PriceType == PriceType.Item && o.m_Rewards[0].m_ItemIdx == m_TDataShop.m_Rewards[0].m_ItemIdx && o.m_Rewards[0].m_ItemCnt == m_TDataShop.m_Rewards[0].m_ItemCnt);
			else m_SUI.BuyBtn.gameObject.SetActive(false);
		}
		//if (m_TDataShop.m_PriceType == PriceType.Item && !USERINFO.IS_CanBuy(m_TDataShop)) {
		//	m_TDataShop = TDATA.GetGroupShopTable(ShopGroup.ItemGacha).Find(o => o.m_PriceType == PriceType.Cash && o.m_Rewards[0].m_ItemIdx == m_TDataShop.m_Rewards[0].m_ItemIdx && o.m_Rewards[0].m_ItemCnt == m_TDataShop.m_Rewards[0].m_ItemCnt);
		//}
		//m_SUI.BuyBtn.SetData(m_TDataShop.m_Idx);

		int getcnt = USERINFO.GetItemCount(m_TDataShop.m_PriceIdx);
		int needcnt = m_TDataShop.GetPrice();
		TShopTable ticketdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_ITEMGACHA_TICKET);
		m_SUI.BuyBtn.SetData(new PriceType[2] { m_TDataShop.m_PriceType, ticketdata.m_PriceType }, new int[2] { getcnt > 0 ? Math.Min(needcnt, getcnt) : 0, ticketdata.GetPrice(Math.Max(0, needcnt - getcnt)) }, new int[2] { m_TDataShop.m_PriceIdx, ticketdata.m_PriceIdx });


		StartCoroutine(m_RewardAction = RewardAction());
	}
	IEnumerator RewardAction() {
		if (m_State != State.Ready) yield break;
		m_State = State.Reward;
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 85f / 116f));

		for (int i = 0; i < m_SUI.Bucket.childCount; i++) {
			Item_EquipGachaList_Item item = m_SUI.Bucket.GetChild(i).GetComponent<Item_EquipGachaList_Item>();
			item.AlphaOnOff(true);
			PlayEffSound(item.GetGrade == m_MaxGrade ? SND_IDX.SFX_1012 : SND_IDX.SFX_1013);
			m_SUI.Scroll.verticalNormalizedPosition = 1f - Mathf.Abs((m_SUI.Bucket.GetChild(i).localPosition.y + ((RectTransform)m_SUI.Bucket.GetChild(i)).rect.yMax ) / m_SUI.Scroll.content.rect.height);
			yield return new WaitForSeconds(0.1f);
		}
		m_SUI.SkipBtn.SetActive(false);
		m_SUI.Anim.SetTrigger("Next");
		m_State = State.Result;
	}
	public void ClickStop() {
		if (m_State != State.Reward) return;
		m_SUI.SkipBtn.SetActive(false);
		StopCoroutine(m_RewardAction);
		for (int i = 0; i < m_SUI.Bucket.childCount; i++) {
			m_SUI.Bucket.GetChild(i).GetComponent<Item_EquipGachaList_Item>().AlphaOnOff(true);
		}
		m_SUI.Scroll.verticalNormalizedPosition = 0f;
		m_SUI.Anim.SetTrigger("Next");
		m_State = State.Result;
	}
	public void ClickReDraw() {
		if (m_Action != null) return;
		if (m_State != State.Result) return;
		//string msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx));
		//POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
		//	if (result == 1) {
		//		if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
		//			///아이템 획득 연출
		//			Close(1);
		//		}
		//		else {
		//			POPUP.StartLackPop(m_TDataShop.GetPriceIdx());
		//		}
		//	}
		//}, m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx, m_TDataShop.GetPrice(), false);
		int getcnt = USERINFO.GetItemCount(m_TDataShop.m_PriceIdx);
		int needcnt = m_TDataShop.GetPrice();
		bool canbuy = getcnt >= needcnt;
		string msg = string.Empty;
		if (canbuy) {
			msg = Utile_Class.StringFormat(TDATA.GetString(790), BaseValue.GetPriceTypeName(m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx));//_tdata.m_PriceType == PriceType.Item ? TDATA.GetItemTable(_tdata.m_PriceIdx).GetName() : TDATA.GetString(122);
			POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, msg, (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						///아이템 획득 연출
						m_RCB?.Invoke(1, m_TDataShop);
						Close(1);
					}
					else {
						POPUP.StartLackPop(m_TDataShop.GetPriceIdx());
					}
				}
			}, m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx, m_TDataShop.GetPrice(), false);
		}
		else {
			TShopTable ticketdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_ITEMGACHA_TICKET);
			msg = Utile_Class.StringFormat(TDATA.GetString(1045), BaseValue.GetPriceTypeName(m_TDataShop.m_PriceType, m_TDataShop.m_PriceIdx));
			POPUP.Set_MsgBox(PopupName.Msg_Store_Ticket_Buy, string.Empty, msg, (result, obj) => {
				if (result == 1) {
					if (obj.GetComponent<Msg_Store_Ticket_Buy>().IS_CanBuy) {
						///아이템 획득 연출
						USERINFO.ITEM_BUY(ticketdata.m_Idx, needcnt - getcnt, (res) => {
							if (res != null && res.IsSuccess()) {
								m_RCB?.Invoke(1, m_TDataShop);
								Close(1);
							}
						});
					}
					else {
						POPUP.StartLackPop(ticketdata.GetPriceIdx());
					}
				}
			}, ticketdata.m_PriceType, ticketdata.m_PriceIdx, ticketdata.GetPrice(needcnt - getcnt), m_TDataShop.m_PriceIdx, needcnt, false);
		}
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (m_State != State.Result) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		StartCoroutine(m_Action = CloseAction(Result));
		m_State = State.End;
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}

}
