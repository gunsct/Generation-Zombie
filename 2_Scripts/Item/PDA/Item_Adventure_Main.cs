using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Adventure_Main : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public GameObject[] Menus;
		public GameObject NAutoListPrefab;
		public Transform Bucket;
		public Item_Adventure_Detail Detail;
		public Item_Adventure_AutoDispatch AutoDispatch;
		public ScrollRect Scroll;
		public Button ResetBtn;
		public Button AllGetBtn;
		public TextMeshProUGUI Lv;
		public Animator m_AllBtnAnim;
		public GameObject m_AllBtnAlarm;
		public Animator Ani;
	}
	[SerializeField] SUI m_SUI;

	List<Item_AdventrueList> m_Items = new List<Item_AdventrueList>();
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	private void OnEnable() {
		StartCoroutine(StartAction());
	}
	/// <summary> 메인에서 탐험 들어올때 </summary>
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		if (TUTO.IsTuto(TutoKind.Adventure, (int)TutoType_Adventure.Select_Adventure)) TUTO.Next();
		m_SUI.Lv.text = string.Format("Lv{0}", Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.AdventureLevelUp)));
		//m_SUI.Scroll.verticalNormalizedPosition = 1f;
		SetAllGetBtn();
		ListGenerate();
	}
	void SetAllGetBtn() {
		List<AdventureInfo> infos = USERINFO.m_Advs.FindAll((t) => t.IS_Complete());
		bool on = infos != null ? infos.Count > 1 : false;
		m_SUI.m_AllBtnAnim.SetTrigger(on ? "Highlight" : "Normal");
		m_SUI.m_AllBtnAlarm.SetActive(on);
	}
	IEnumerator StartAction()
	{
		yield return Utile_Class.CheckAniPlay(m_SUI.Ani);
		if (TUTO.IsTuto(TutoKind.Adventure, (int)TutoType_Adventure.ViewAdventure)) TUTO.Next(this);
	}

	/// <summary> 임무 리스트 갱신, 파견, 임무 수령후 호출 </summary>
	void ListGenerate() {
		//탐험 시작 안한것들 삭제하고 일일 리스트에서도 제거
		UTILE.Load_Prefab_List(USERINFO.m_Advs.Count, m_SUI.Bucket, m_SUI.NAutoListPrefab.transform);

		m_Items.Clear();
		// m_SUI.Bucket.childCount로 for문을 돌리지말것
		// 실제 오브젝트 제거는 한프레임 뒤에 반응하므로 childCount 개수를 안맞음 
		// Load_Prefab_List에서 뒤에부터 제거 되므로 실 데이터의 개수로만해도 상관없음
		for (int i = USERINFO.m_Advs.Count - 1; i > -1; i--) {
			Item_AdventrueList item = m_SUI.Bucket.GetChild(i).GetComponent<Item_AdventrueList>();
			item.SetData(USERINFO.m_Advs[i], SetPlayCB, SetAllGetBtn);
			m_Items.Add(item);
		}
		ListSort();
	}
	void ListSort() {
		//정렬
		m_Items.Sort((Item_AdventrueList befor, Item_AdventrueList after) => {
			if (befor.m_Info.m_State != after.m_Info.m_State) return after.m_Info.m_State.CompareTo(befor.m_Info.m_State);
			return after.m_TData.m_AdventureGrade.CompareTo(befor.m_TData.m_AdventureGrade);
		});

		for (int i = 0; i < m_Items.Count; i++) m_Items[i].transform.SetSiblingIndex(i);

		m_SUI.ResetBtn.interactable = m_Items.FindAll(o => o.m_State == TimeContentState.Idle).Count > 0;
		m_SUI.AllGetBtn.interactable = m_Items.FindAll(o => o.m_Info.IS_Complete()).Count > 0;
	}

	/// <summary> 임무 파견 버튼 콜백</summary>
	public void SetPlayCB(Item_AdventrueList _list) {//TAdventureTable _data, Item_AdventrueList.State _state, AdventureInfo _info = null
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Adventure, 4, _list)) return;

		switch(_list.m_State)
		{
		case TimeContentState.Idle:
			m_CloaseCB?.Invoke(Item_PDA_Adventure.State.Detail, new object[] { _list });
			break;
		case TimeContentState.Play:
#if NOT_USE_NET
			if (_list.m_Info.IS_Complete())
			{
				// 보상 받기
				SetReward(_list.m_Info);
					PlayCommVoiceSnd(VoiceType.Success);
				}
			else
			{
				int price = BaseValue.GetTimePrice( ContentType.Explorer, _list.m_Info.GetRemainTime());
				if (USERINFO.m_Cash < price)
				{
					POPUP.StartLackPop(BaseValue.CASH_IDX);
						PlayCommVoiceSnd(VoiceType.Fail);
						return;
				}
				// 가속후 보상받기
				USERINFO.GetCash(-price);
				SetReward(_list.m_Info);
				PlayEffSound(SND_IDX.SFX_0123);
				PlayCommVoiceSnd(VoiceType.Success);
			}
			USERINFO.m_Advs.Remove(_list.m_Info);
			MAIN.Save_UserInfo();
			MAIN.SetRewardList(new object[] { m_Rewards }, () => { m_Rewards.Clear(); });
			//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList, (result, obj) => {
			//	m_Rewards.Clear();
			//}, m_Rewards);
			ListGenerate();
#else
			bool usecash = !_list.m_Info.IS_Complete();
			if (usecash && USERINFO.m_Cash < BaseValue.GetTimePrice(ContentType.Explorer, _list.m_Info.GetRemainTime()))
			{
				POPUP.StartLackPop(BaseValue.CASH_IDX);
				PlayCommVoiceSnd(VoiceType.Fail);
				return;
			}

			WEB.SEND_REQ_ADV_END((res) =>
			{
				if (!res.IsSuccess())
				{
					WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
					return;
				}
				ListGenerate();

				m_Rewards.Clear();

				
				if (res.Rewards == null) {
					PlayCommVoiceSnd(VoiceType.Fail);
					return;
				}
				m_Rewards.AddRange(res.GetRewards());
				if(m_Rewards.Count > 0)
				{
					if (usecash) {
						PlayEffSound(SND_IDX.SFX_0123);
						PlayCommVoiceSnd(VoiceType.Success);
					}
					else PlayCommVoiceSnd(VoiceType.Success);
					MAIN.SetRewardList(new object[] { m_Rewards }, () => { m_Rewards.Clear(); });
					//POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.RewardList, (result, obj) => {
					//	m_Rewards.Clear();
					//}, m_Rewards);
				}
			}, new List<long>() { _list.m_Info.m_UID }, usecash);
#endif
				break;
		case TimeContentState.End:
			USERINFO.m_Advs.Remove(_list.m_Info);
			MAIN.Save_UserInfo();
			ListGenerate();
			break;
		}
	}

	void SetReward(AdventureInfo info)
	{
		List<TAdventureTable.ADReward> rewards = info.m_TData.m_Reward;
		for(int i = 0; i < rewards.Count; i++)
		{
			TAdventureTable.ADReward ritem = rewards[i];
			TItemTable table = TDATA.GetItemTable(ritem.m_Idx);
			var item = USERINFO.InsertItem(rewards[i].m_Idx, rewards[i].m_Cnt);
			switch (table.m_Type)
			{
			case ItemType.Dollar:
				m_Rewards.Add(new RES_REWARD_MONEY()
				{
					Type = Res_RewardType.Money,
					Befor = USERINFO.m_Money - ritem.m_Cnt,
					Now = USERINFO.m_Money,
					Add = ritem.m_Cnt
				});
				break;
			case ItemType.Cash:
				m_Rewards.Add(new RES_REWARD_MONEY()
				{
					Type = Res_RewardType.Cash,
					Befor = USERINFO.m_Cash - ritem.m_Cnt,
					Now = USERINFO.m_Cash,
					Add = ritem.m_Cnt
				});
				break;
			case ItemType.Exp:
				m_Rewards.Add(new RES_REWARD_MONEY()
				{
					Type = Res_RewardType.Exp,
					Befor = USERINFO.m_Exp[1] - ritem.m_Cnt,
					Now = USERINFO.m_Exp[1],
					Add = ritem.m_Cnt
				});
				break;
			case ItemType.Energy:
				m_Rewards.Add(new RES_REWARD_MONEY()
				{
					Type = Res_RewardType.Energy,
					Befor = USERINFO.m_Energy.Cnt - ritem.m_Cnt,
					Now = USERINFO.m_Energy.Cnt,
					Add = ritem.m_Cnt
				});
				break;
			case ItemType.InvenPlus:
				m_Rewards.Add(new RES_REWARD_MONEY()
				{
					Type = Res_RewardType.Inven,
					Befor = USERINFO.m_InvenSize - ritem.m_Cnt,
					Now = USERINFO.m_InvenSize,
					Add = ritem.m_Cnt
				});
				break;
			default:
				m_Rewards.Add(new RES_REWARD_ITEM()
				{
					Type = Res_RewardType.Item,
					UID = item.m_Uid,
					Idx = ritem.m_Idx,
					Cnt = table.GetEquipType() == EquipType.End ? ritem.m_Cnt : 1
				});
				break;
			}
		}
	}
  
	/// <summary> 목록 초기화 </summary>
	public void ClickResetList()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Adventure, 3, null)) return;
		if (m_Items.FindAll(o => o.m_State == TimeContentState.Idle).Count < 1) return;
		int price = TDATA.GetShopTable(BaseValue.SHOP_IDX_ADV_RESET).GetPrice();
		if (USERINFO.m_Cash < price)
		{
			POPUP.StartLackPop(BaseValue.CASH_IDX);
			return;
		}
		PlayEffSound(SND_IDX.SFX_0121);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(229), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					SEND_REQ_ADV_RESET();
				}
				else {
					POPUP.StartLackPop(BaseValue.CASH_IDX);
				}
			}
		}, PriceType.Cash, BaseValue.CASH_IDX, price, false);
	}

	void SEND_REQ_ADV_RESET()
	{
		PlayCommVoiceSnd(VoiceType.Success);
#if NOT_USE_NET
		USERINFO.GetCash(-TDATA.GetShopTable(BaseValue.SHOP_IDX_ADV_RESET).GetPrice());
		USERINFO.Reset_AdvList();

		//FireBase-Analytics
		MAIN.GoldToothStatistics(GoldToothContentsType.AdventureReset, Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.AdventureLevelUp)));

		//*목록 리셋(서버로 셋업중 하나 불러오기
		ListGenerate();
		m_SUI.Scroll.verticalNormalizedPosition = 1f;
#else
		PlayEffSound(SND_IDX.SFX_0121);
		WEB.SEND_REQ_ADV_RESET((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}

			//FireBase-Analytics
			MAIN.GoldToothStatistics(GoldToothContentsType.AdventureReset, Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.AdventureLevelUp)));

			ListGenerate();
			m_SUI.Scroll.verticalNormalizedPosition = 1f;
		});
#endif
	}

	/// <summary> 빠른 파견 </summary>
	public void ClickFastDispatch() {

		if (TUTO.TouchCheckLock(TutoTouchCheckType.Adventure, 0, null)) return;
		List<Item_AdventrueList> list = new List<Item_AdventrueList>();
		list.AddRange(m_Items.FindAll((t) => t.m_State == TimeContentState.Idle));
		//if (list.Count < 1) return;
		PlayEffSound(SND_IDX.SFX_0121);
		m_CloaseCB?.Invoke(Item_PDA_Adventure.State.AutoDispatch, new object[] { list });
		m_SUI.Scroll.verticalNormalizedPosition = 1f;
	}
	/// <summary> 전체 수령 </summary>
	public void ClickGetAllReward()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Adventure, 2, null)) return;
		var items = m_Items.FindAll(o => o.m_Info.IS_Complete());
		if (items.Count < 1) return;
		PlayEffSound(SND_IDX.SFX_0121);
		m_Rewards.Clear();
#if NOT_USE_NET
		for (int i = 0; i < items.Count; i++)
		{
			SetReward(items[i].m_Info);
			USERINFO.m_Advs.Remove(items[i].m_Info);
		}
		ListGenerate();
		m_SUI.Scroll.verticalNormalizedPosition = 1f;
		MAIN.Save_UserInfo();
		if (m_Rewards.Count > 0)
		{
			MAIN.SetRewardList(new object[] { m_Rewards }, () => { 
				m_Rewards.Clear();
				SetAllGetBtn();
			});
		}
#else
		WEB.SEND_REQ_ADV_END((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			ListGenerate();

			m_Rewards.Clear();
			if (res.Rewards == null) return;
			m_Rewards.AddRange(res.GetRewards());
			if (m_Rewards.Count > 0)
			{
				MAIN.SetRewardList(new object[] { m_Rewards }, () => { 
					m_Rewards.Clear(); 
					SetAllGetBtn();
				});
			}
		}, items.Select(o => o.m_Info.m_UID).ToList(), false);
#endif
	}
	public override void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Adventure, 1, null)) return;
		m_CloaseCB?.Invoke(Item_PDA_Adventure.State.End, null);
	}
}
