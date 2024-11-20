using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

public class Store_PassLvUp_Single : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] LvTxt;
		public Item_Store_Buy_Button BuyBtn;
	}
	[SerializeField] SUI m_SUI;
	TShopTable m_TData;
	List<MissionData> m_Missions;
	int m_NowLV;
	int m_Price;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_Missions = (List<MissionData>)aobjValue[0];
		m_NowLV = (int)aobjValue[1];

		DLGTINFO.f_RFMoneyUI(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO.f_RFCashUI(USERINFO.m_Cash, USERINFO.m_Cash);

		m_SUI.LvTxt[0].text = (m_NowLV - 1).ToString();
		m_SUI.LvTxt[1].text = m_NowLV.ToString();

		m_TData = TDATA.GetShopTable(BaseValue.PASS_LV_SHOP_IDX);
		m_SUI.BuyBtn.SetData(m_TData.m_Idx);
		m_Price = m_TData.GetPrice();
	}

	public void ClickBuy() {
		if (m_Action != null) return;
		if (USERINFO.m_Cash >= m_Price) {
			WEB.SEND_REQ_SHOP_BUY_PASS_LV((res) => {
				if (!res.IsSuccess()) {
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					return;
				}
				Close(1);
			}, m_Missions.Select(o => o.UID).ToList());
		}
		else {
			POPUP.StartLackPop(m_TData.GetPriceIdx());
		}
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
