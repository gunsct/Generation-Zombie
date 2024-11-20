using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;
using System.Linq;

public class Union_Store : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public Animator Anim;

		public Item_GuildStore_Talk_Control Talk;

		public ScrollRect Scroll;
		public RectTransform Prefab;

		public TextMeshProUGUI Time;

		public GameObject CloseBtn;
	}

	[SerializeField] SUI m_SUI;
	bool IsStart;
	List<TShopTable> m_List = new List<TShopTable>();
	IEnumerator m_TimeCheck;
	private IEnumerator Start()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Anim);
	}

	private void OnEnable()
	{
		if (m_Popup != PopupName.Union_Store) SetUI();
		StartCoroutine(ItemAction());
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {

		IsStart = false;
		base.SetData(pos, popup, cb, aobjValue);
	}
	public void StartTalk()
	{
		m_SUI.Talk.StartTalk(UTILE.Get_Random(101, 104));
	}

	void StartTimeCheck()
	{
		if (m_TimeCheck != null)
		{
			StopCoroutine(m_TimeCheck);
			m_TimeCheck = null;
		}
		m_TimeCheck = Timecheck();
		StartCoroutine(m_TimeCheck);
	}

	public override void SetUI() {
		base.SetUI();
		PlayEffSound(SND_IDX.SFX_1510);
		StartTimeCheck();
		DLGTINFO?.f_RFGCoinUI?.Invoke(USERINFO.m_GCoin, USERINFO.m_GCoin);
		LoadList();
	}

	int GetSortValue(TShopTable tdata)
	{
		switch(tdata.m_Group)
		{
		case ShopGroup.Guild_normal_DNA: return 1;
		case ShopGroup.Guild_normal_Char: return 2;
		case ShopGroup.Guild_master:	return 3;
		}
		return 0;
	}

	public void LoadList()
	{
		m_List.Clear();
		m_List.AddRange(USERINFO.m_Guild.GetMyShopList());
		m_List.Sort((befor, after) =>
		{
			var bvalue = GetSortValue(befor);
			var avalue = GetSortValue(after);
			if (bvalue != avalue) return bvalue.CompareTo(avalue);
			if (befor.m_Level != after.m_Level) return befor.m_Level.CompareTo(after.m_Level);
			return befor.m_Idx.CompareTo(after.m_Idx);
		});

		int Max = m_List.Count;
		UTILE.Load_Prefab_List(Max, m_SUI.Scroll.content, m_SUI.Prefab);

		for (int i = 0; i < Max; i++)
		{
			Item_Union_Store_Element element = m_SUI.Scroll.content.GetChild(i).GetComponent<Item_Union_Store_Element>();
			element.SetData(m_List[i], Click_Item);
		}

		StartCoroutine(ItemAction());
	}

	IEnumerator ItemAction()
	{
		IsStart = true;
		int Max = m_List.Count;
		for (int i = 0; i < Max; i++) m_SUI.Scroll.content.GetChild(i).gameObject.SetActive(false);


		for (int i = 0; i < Max; i++)
		{
			m_SUI.Scroll.content.GetChild(i).gameObject.SetActive(true);

			yield return new WaitForSeconds(0.2f);
		}
	}

	IEnumerator Timecheck()
	{
		var data = GuildShop.Load();
		var etime = data.GetEndTime();
		var gaptime = Math.Max(0f, (etime - UTILE.Get_ServerTime_Milli()) * 0.001d);

		while (gaptime > 0)
		{
			m_SUI.Time.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, gaptime));
			if (gaptime < 0.1f) break;
			yield return new WaitForSeconds(1f - (float)(UTILE.Get_ServerTime() % 1d));
			gaptime = Math.Max(0f, (etime - UTILE.Get_ServerTime_Milli()) * 0.001d);
		}
		m_SUI.Time.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, gaptime));
	}

	public void ActiveCloseBtn(bool Active)
	{
		m_SUI.CloseBtn.SetActive(Active);
	}

	#region Btn

	public void Click_Item(Item_Union_Store_Element item)
	{
		if (!IsStart) return;
		var tdata = item.m_Tdata;
		PlayEffSound(SND_IDX.SFX_1014);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PurchaseConfirm, (res, obj) => {
			if (res == 1)
			{
				if (!USERINFO.IS_CanBuy(tdata))
				{
					POPUP.StartLackPop(tdata.GetPriceIdx());
					return;
				}
				else
				{
					USERINFO.ITEM_BUY(tdata.m_Idx, 1, (res) =>
					{
						if (res == null) return;
						if (!res.IsSuccess())
						{
							if(res.result_code != EResultCode.ERROR_SHOP_BUY_MARKET_ERROR) WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
							return;
						}
						PlayEffSound(tdata.GetPriceIdx() == BaseValue.GUILDCOIN_IDX ? SND_IDX.SFX_1014 : SND_IDX.SFX_1010);


						List<RES_REWARD_BASE> Rewards;
						if (tdata.m_Group == ShopGroup.Guild_master)
						{
							Rewards = new List<RES_REWARD_BASE>();
							for (int i = 0; i < tdata.m_Rewards.Count; i++)
								if (tdata.m_Rewards[i].m_ItemIdx != 0) Rewards.Add(new RES_REWARD_ITEM() { Idx = tdata.m_Rewards[i].m_ItemIdx, Cnt = 1 });
						}
						else
						{
							Rewards = res.GetRewards();
						}
						if (Rewards.Count > 0)
						{
							MAIN.SetRewardList(new object[] { Rewards }, () => {
								Close(1);
								m_SUI.Talk.StartTalk(105);
							});
						}
						item.SetUI();
					});
				}
			}
		}, MAIN.GetRewardBase(tdata, RewardKind.Item)[0], tdata, false);
	}

	public override void Close(int Result = 0)
	{
		if (!IsStart) return;
		base.Close(Result);
	}

	#endregion
}
