using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class DungeonTower : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI[] Floor;
		public TextMeshProUGUI RecommanCP;
		public Transform Bucket;
		public GameObject RewardPrefab;
		public Button Btn;
		public TextMeshProUGUI BtnTxt;
	}
	[SerializeField]
	SUI m_SUI;
	TModeTable m_Modetable;
	TStageTable m_Stagetable;
	List<Item_RewardList_Item> m_Rewards = new List<Item_RewardList_Item>();
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		RefreshUI();
	}

	void RefreshUI() {
		List<TModeTable> tables = TDATA.GetModeTable(StageContentType.Tower, 0, 0);
		int lv = USERINFO.m_Stage[StageContentType.Tower].Idxs[0].Clear;
		m_Modetable = tables[lv];
		m_Stagetable = TDATA.GetStageTable(m_Modetable.m_StageIdx);
		//층수
		for (int i = 0, diff = -2; i < m_SUI.Floor.Length; i++, diff++) {
			if (lv + diff < 1 || lv + diff > tables.Count) m_SUI.Floor[i].transform.parent.gameObject.SetActive(false);
			m_SUI.Floor[i].text = string.Format("{0}<size=50>F</size>", lv + diff);
		}
		//권장 전투력
		m_SUI.RecommanCP.text = string.Format("{0:#,###}", m_Stagetable.m_RecommandCP);

		//보상
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();

		for (int i = m_Rewards.Count - 1; i > -1; i--) {
			Destroy(m_Rewards[i].gameObject);
		}
		m_Rewards.Clear();
		int Exp = m_Stagetable.m_ClearExp;
		int Money = m_Stagetable.m_ClearMoney;
		int Gold = m_Stagetable.m_ClearGold;

		if (Exp > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Exp;
			rmoney.Befor = USERINFO.m_Exp[0] - Exp;
			rmoney.Now = USERINFO.m_Exp[0];
			rmoney.Add = Exp;
			rewards.Add(rmoney);
		}
		if (Money > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Money;
			rmoney.Befor = USERINFO.m_Money - Money;
			rmoney.Now = USERINFO.m_Money;
			rmoney.Add = Money;
			rewards.Add(rmoney);
		}
		if (Gold > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Cash;
			rmoney.Befor = USERINFO.m_Cash - Gold;
			rmoney.Now = USERINFO.m_Cash;
			rmoney.Add = Gold;
			rewards.Add(rmoney);
		}
		for (int i = 0; i < m_Stagetable.m_ClearReward.Count; i++) {
			switch (m_Stagetable.m_ClearReward[i].m_Kind) {
				case RewardKind.None:
					break;
				case RewardKind.Character:
					CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_Idx == m_Stagetable.m_ClearReward[i].m_Idx);
					if (charinfo != null) {
						rewards.Add(new RES_REWARD_ITEM() {
							Type = Res_RewardType.Item,
							Idx = charinfo.m_TData.m_PieceIdx,
							Cnt = TDATA.GetItemTable(charinfo.m_TData.m_PieceIdx).GetEquipType() == EquipType.End ? BaseValue.STAR_OVERLAP(charinfo.m_TData.m_Grade) : 1,
							result_code = EResultCode.SUCCESS_REWARD_PIECE
						});
					}
					else {
						CharInfo charInfo = new CharInfo(m_Stagetable.m_ClearReward[0].m_Idx);
						RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
						rchar.SetData(charInfo);
						rewards.Add(rchar);
					}
					break;
				case RewardKind.Item:
					var itemTable = TDATA.GetItemTable(m_Stagetable.m_ClearReward[i].m_Idx);
					if (itemTable.m_Type == ItemType.RandomBox || itemTable.m_Type == ItemType.AllBox) {//박스는 바로 까서 주기
						List<RES_REWARD_BASE> boxrewards = new List<RES_REWARD_BASE>();
						GachaGroup gachagroup = TDATA.GetGachaGroup(itemTable.m_Value);
						List<RES_REWARD_BASE> items = TDATA.GetGachaItemList(itemTable);

						for (int j = 0; j < items.Count; j++) {
							RES_REWARD_BASE reward = boxrewards.Find((t) => t.GetIdx() == items[j].GetIdx());
							RES_REWARD_BASE rewardprefab = rewards.Find((t) => t.GetIdx() == items[j].GetIdx());

							if (reward == null && rewardprefab == null)
								boxrewards.Add(items[j]);
							else continue;
						}

						for (int j = 0; j < boxrewards.Count; j++) {
							// 캐릭터 보상은 없음
							if (boxrewards[j].Type == Res_RewardType.Char) continue;
							rewards.Add(boxrewards[j]);
						}
					}
					else {
						TItemTable tdata = TDATA.GetItemTable(m_Stagetable.m_ClearReward[i].m_Idx);
						RES_REWARD_MONEY rmoney;
						RES_REWARD_ITEM ritem;
						switch (tdata.m_Type) {
							case ItemType.Exp:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Exp;
								rmoney.Befor = USERINFO.m_Exp[0] - m_Stagetable.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Exp[0];
								rmoney.Add = m_Stagetable.m_ClearReward[i].m_Count;
								rewards.Add(rmoney);
								break;
							case ItemType.Dollar:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Money;
								rmoney.Befor = USERINFO.m_Money - m_Stagetable.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Money;
								rmoney.Add = m_Stagetable.m_ClearReward[i].m_Count;
								rewards.Add(rmoney);
								break;
							case ItemType.Cash:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Cash;
								rmoney.Befor = USERINFO.m_Cash - m_Stagetable.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Cash;
								rmoney.Add = m_Stagetable.m_ClearReward[i].m_Count;
								rewards.Add(rmoney);
								break;
							case ItemType.Energy:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Energy;
								rmoney.Befor = USERINFO.m_Energy.Cnt - m_Stagetable.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_Energy.Cnt;
								rmoney.Add = m_Stagetable.m_ClearReward[i].m_Count;
								rmoney.STime = (long)USERINFO.m_Energy.STime;
								rewards.Add(rmoney);
								break;
							case ItemType.InvenPlus:
								rmoney = new RES_REWARD_MONEY();
								rmoney.Type = Res_RewardType.Inven;
								rmoney.Befor = USERINFO.m_InvenSize - m_Stagetable.m_ClearReward[i].m_Count;
								rmoney.Now = USERINFO.m_InvenSize;
								rmoney.Add = m_Stagetable.m_ClearReward[i].m_Count;
								rewards.Add(rmoney);
								break;
							default:
								ritem = new RES_REWARD_ITEM();
								ritem.Type = Res_RewardType.Item;
								ritem.UID = 0;
								ritem.Idx = m_Stagetable.m_ClearReward[i].m_Idx;
								ritem.Cnt = m_Stagetable.m_ClearReward[i].m_Count;
								rewards.Add(ritem);
								break;
						}
						break;
					}
					break;
				case RewardKind.Zombie:
					ZombieInfo zombieInfo = new ZombieInfo(m_Stagetable.m_ClearReward[i].m_Idx);
					RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
					zombie.UID = zombieInfo.m_UID;
					zombie.Idx = zombieInfo.m_Idx;
					zombie.Grade = zombieInfo.m_Grade;
					rewards.Add(zombie);
					break;
				case RewardKind.DNA:
					DNAInfo dnaInfo = new DNAInfo(m_Stagetable.m_ClearReward[i].m_Idx);
					RES_REWARD_DNA dna = new RES_REWARD_DNA();
					dna.UID = dnaInfo.m_UID;
					dna.Idx = dnaInfo.m_Idx;
					dna.Grade = dnaInfo.m_Grade;
					rewards.Add(dna);
					break;
			}
		}
		for (int i = 0; i < rewards.Count; i++) {
			Item_RewardList_Item reward = Utile_Class.Instantiate(m_SUI.RewardPrefab, m_SUI.Bucket).GetComponent<Item_RewardList_Item>();
			reward.transform.localScale = Vector3.one * 0.46f;
			reward.SetData(rewards[i]);
		}
		//버튼
		m_SUI.Btn.interactable = USERINFO.m_Stage[StageContentType.Tower].IS_CanGoStage();
		m_SUI.BtnTxt.text = TDATA.GetString(152);
	}
	public void ClickReady()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Cungeon_Tower_Btn, 0)) return;

		UserInfo.StageIdx sidx = USERINFO.m_Stage[StageContentType.Tower].Idxs[0];
		if (m_Stagetable.m_Energy > 0 && USERINFO.m_Energy.Cnt < m_Stagetable.m_Energy)
		{
			POPUP.StartLackPop(BaseValue.ENERGY_IDX);
			return;
		}

		DLGTINFO?.f_OBJSndOff?.Invoke();
		SND.StopEff();
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DeckSetting, (result, obj) => {
			if (result == 1) {
#if NOT_USE_NET
				USERINFO.Check_Mission(MissionType.UseBullet, 0, 0, TDATA.GetStageTable(sidx.Idx).m_Energy);
				PLAY.GoModeStage(StageContentType.Tower, USERINFO.m_Stage[StageContentType.Tower].Idxs[0].Clear + 1);
#else
				PLAY.GetStagePlayCode((result) =>
				{
					if (result != EResultCode.SUCCESS)
					{
						SetUI();
						return;
					}
					PLAY.GoModeStage(StageContentType.Tower, sidx.Clear + 1);
				}, StageContentType.Tower, sidx.Idx);
#endif
			}
		}, m_Stagetable, StageContentType.Tower, 0, 0);
	}
}
