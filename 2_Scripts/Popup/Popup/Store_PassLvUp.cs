using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Store_PassLvUp : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public RectTransform Prefab;

		public TextMeshProUGUI BeforLV;
		public TextMeshProUGUI AfterLV;

		public Item_Store_Buy_Button BuyBtn;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	List<PostReward> Items;
	List<MissionData> Missions;
	int NowLV;
	TShopTable m_TShopData;
	int m_Price;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		Missions = (List<MissionData>)aobjValue[0];
		NowLV = (int)aobjValue[1];
		Items = Missions.SelectMany(o => TDATA.GetMissionTable(o.Idx).m_Rewards)
			.GroupBy(o => o.Kind)
			.ToDictionary(o => o.Key
						, o => o.GroupBy(p => p.Idx)
								.Select(p => new PostReward(){ Kind = o.Key, Idx = p.Key, Cnt = p.Sum(t => t.Cnt), Grade = p.First().Grade } )
						)
			.SelectMany(o => o.Value.ToList())
			.ToList();

		m_TShopData = TDATA.GetShopTable(BaseValue.PASS_LV_SHOP_IDX);
		m_Price = m_TShopData.GetPrice(Missions.Count);

		//.GroupBy(o => o.Idx)
		//.Select(o => new PostReward() { Kind = RewardKind.Item, Idx = o.Key, Cnt = o.Sum(p => p.Cnt) })
		//.ToList();
		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		UTILE.Load_Prefab_List(Items.Count, m_SUI.Scroll.content, m_SUI.Prefab);

		for (int i = Items.Count - 1; i > -1; i--)
		{
			m_SUI.Scroll.content.GetChild(i).GetComponent<Item_Pass_GetListElement>().SetData(Items[i]);
		}

		m_SUI.BeforLV.text = (NowLV - 1).ToString();
		m_SUI.AfterLV.text = TDATA.GetMissionTable(Missions[Missions.Count - 1].Idx).m_LinkIdx.ToString();

		m_SUI.BuyBtn.SetData(m_TShopData.m_PriceType, m_Price, m_TShopData.m_PriceIdx);
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
			}, Missions.Select(o => o.UID).ToList());
		}
		else {
			POPUP.StartLackPop(m_TShopData.GetPriceIdx());
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
