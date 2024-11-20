using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Store_PassBuy : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public RectTransform Prefab;

		public Item_Store_Buy_Button BuyBtn;

		public TextMeshProUGUI Time;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	List<PostReward> Items;
	RES_SHOP_ITEM_INFO Pass;
	bool m_IsBuy;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		Pass = USERINFO.m_ShopInfo.PassInfo[0];
		var Missions = USERINFO.m_Mission.Get_Missions(MissionMode.Pass, Pass.Idx);
		//Items = Missions.GroupBy(o => TDATA.GetMissionTable(o.Idx).m_Rewards[1].Idx)
		//	.Select(o => new PostReward() { Kind = RewardKind.Item, Idx = o.Key, Cnt = o.Sum(p => TDATA.GetMissionTable(p.Idx).m_Rewards[1].Cnt) })
		//	.ToList();

		Items = Missions.Select(o => TDATA.GetMissionTable(o.Idx).m_Rewards[1])
			.GroupBy(o => o.Kind)
			.ToDictionary(o => o.Key
						, o => o.GroupBy(p => p.Idx)
								.Select(p => new PostReward() { Kind = o.Key, Idx = p.Key, Cnt = p.Sum(t => t.Cnt), Grade = p.First().Grade })
						)
			.SelectMany(o => o.Value.ToList())
			.ToList();

		DLGTINFO?.f_RFMoneyUI?.Invoke(USERINFO.m_Money, USERINFO.m_Money);
		DLGTINFO?.f_RFCashUI?.Invoke(USERINFO.m_Cash, USERINFO.m_Cash);
		m_IsBuy = (bool)aobjValue[0];

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		UTILE.Load_Prefab_List(Items.Count, m_SUI.Scroll.content, m_SUI.Prefab);

		for(int i = Items.Count - 1; i > -1; i--)
		{
			m_SUI.Scroll.content.GetChild(i).GetComponent<Item_Pass_GetListElement>().SetData(Items[i]);
		}

		if (m_IsBuy) {
			m_SUI.BuyBtn.gameObject.SetActive(true);
			var tdata = TDATA.GetShopTable(Pass.Idx);
			var pid = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == Pass.Idx);
			if(pid == null)
			{
				switch (tdata.m_PriceType)
				{
				case PriceType.Money:
					m_SUI.BuyBtn.GetComponent<Button>().interactable = USERINFO.m_Money >= tdata.GetPrice();
					break;
				case PriceType.Cash:
					m_SUI.BuyBtn.GetComponent<Button>().interactable = USERINFO.m_Cash >= tdata.GetPrice();
					break;
				}
			}
			m_SUI.BuyBtn.SetData(tdata.m_Idx);
		}
		else m_SUI.BuyBtn.gameObject.SetActive(false);
	}

	void TimeUI()
	{
		if (Pass == null) return;
		long time = (long)((Pass.Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001f);
		m_SUI.Time.text = TimeSpan.FromSeconds(time).ToString(TDATA.GetString(5028));
	}

	private void Update()
	{
		TimeUI();
	}

	public void ClickBuy() {
		if (m_Action != null) return;
		Close(1);
	}
	public void ClickTerms() {
		if (m_Action != null) return;
		string url = "";
#if NOT_USE_NET
		url = "http://59.13.192.250:11000/Files/PFA/Offer_{0}.txt";
#else
		url = WEB.GetConfig(EServerConfig.Offer_url);
#endif

		url = string.Format(url, APPINFO.m_LanguageCode);

		POPUP.Set_WebView(TDATA.GetString(513), url);
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
