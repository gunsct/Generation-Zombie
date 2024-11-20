using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Event_10_Gift_Purchase : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_RewardList_Item[] Rewards;
		public Item_Store_Buy_Button Btn;
	}
    [SerializeField] SUI m_SUI;
	MissionData m_Info;
	IEnumerator m_Action; //end ani check
	Dictionary<int, int> m_NeedCnt = new Dictionary<int, int>();
	int m_Price;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (MissionData)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();

		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		m_Price = 0;
		for(int i = 0;i< m_Info.m_TData.m_Check.Count; i++) {
			TMissionTable.TMissionCheck check = m_Info.m_TData.m_Check[i];
			int need = check.m_Cnt - m_Info.GetCnt(i);
			int sidx = TDATA.GetItemTable(check.m_Val[0]).m_ShopIdx;
			TShopTable tdata = TDATA.GetShopTable(sidx);
			m_NeedCnt.Add(check.m_Val[0], need);
			m_Price += tdata.GetPrice(need);
		}
		for(int i = 0; i < m_SUI.Rewards.Length; i++) {
			if (i < m_NeedCnt.Count && m_NeedCnt.ElementAt(i).Value > 0) {
				m_SUI.Rewards[i].SetData(RewardKind.Item, m_NeedCnt.ElementAt(i).Key, m_NeedCnt.ElementAt(i).Value, 1, 0, null, false);
				m_SUI.Rewards[i].SetCntActive(true);
				m_SUI.Rewards[i].gameObject.SetActive(true);
			}
			else m_SUI.Rewards[i].gameObject.SetActive(false);
		}
		m_SUI.Btn.SetData(PriceType.Cash, m_Price);
	}
	public void Click_Buy() {
		if (USERINFO.m_Cash < m_Price) {
			POPUP.StartLackPop(BaseValue.CASH_IDX);
			return;
		}
		List<REQ_SHOP_BUY.Buy_Item> items = new List<REQ_SHOP_BUY.Buy_Item>();
		for (int i = 0; i < m_NeedCnt.Count; i++) {
			KeyValuePair<int, int> item = m_NeedCnt.ElementAt(i);
			if (item.Value > 0) {
				items.Add(new REQ_SHOP_BUY.Buy_Item() { Idx = TDATA.GetItemTable(item.Key).m_ShopIdx, Cnt = item.Value });
			}
		}
		WEB.SEND_REQ_SHOP_BUY((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				SetUI();
				return;
			}
			Close(1);
		}, items);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		base.Close(_result);
	}
}
