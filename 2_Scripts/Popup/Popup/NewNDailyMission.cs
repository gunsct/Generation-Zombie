using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;

public class NewNDailyMission : PopupBase
{
	public enum Type
	{
		DailyMission,
		BeginnerQuest
	}

	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject[] Panels;//0:Item_DailyMission,1:Item_BeginnerQuest
		public GameObject TabGroup;
		public Item_Tab[] Tab;
		public int[] TabNameIdx;
	}
	[SerializeField] SUI m_SUI;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	Type m_Type;
	IEnumerator m_Action;
	bool Is_AllClearBeginMission;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		Is_AllClearBeginMission = !IsStartBeginner();
		SetTabUI();
		m_Type = !Is_AllClearBeginMission ? Type.BeginnerQuest : Type.DailyMission;
		for(int i = 0;i < m_SUI.Panels.Length; i++) m_SUI.Panels[i].SetActive(false);
		for(int i = 0;i< m_SUI.Tab.Length; i++) m_SUI.Tab[i].SetData(i, TDATA.GetString(m_SUI.TabNameIdx[i]), Click_ChangeMenu);
		CheckAlarm();
		Click_ChangeMenu(m_SUI.Tab[(int)m_Type]);
	}
	public bool IsStartBeginner()
	{
		if (TUTO.IsTuto(TutoKind.NewMission)) return true;
		if (!m_SUI.Tab[(int)Type.BeginnerQuest].gameObject.activeSelf) return false;
		var miss = USERINFO.m_Mission.Get_Missions(MissionMode.BeginnerQuest);
		MissionData allclearbeginner = USERINFO.m_Mission.Get_Missions(MissionMode.BeginnerQuest).Find(o => o.m_TData.m_Check[0].m_Type == MissionType.ModeClear);
		if (allclearbeginner == null) return false;
		return !allclearbeginner.IS_End();
	}

	bool Click_ChangeMenu(Item_Tab _tab) {
		int pos = _tab.m_Pos;
		int checklock = 0;
		if (pos == 0) checklock = 1;
		else if(pos == 1) checklock = 2;
		if (!m_SUI.Tab[pos].gameObject.activeSelf) return false;

		if (TUTO.TouchCheckLock(TutoTouchCheckType.MissionBtn, checklock)) return false;

		m_SUI.Tab[(int)m_Type].SetActive(false);
		m_SUI.Panels[(int)m_Type].SetActive(false);
		m_Type = (Type)pos;
		m_SUI.Tab[pos].SetActive(true);
		m_SUI.Panels[pos].SetActive(true);
		switch ((Type)pos) {
			case Type.DailyMission:
				m_SUI.Panels[pos].GetComponent<Item_DailyMission>().SetData(this);
				break;
			case Type.BeginnerQuest:
				m_SUI.Panels[pos].GetComponent<Item_BeginnerQuest>().SetData(this);
				break;
		}
		return true;
	}

	int GetTabCnt()
	{
		int tabCnt = 0;
		for (int i = 0; i < m_SUI.Tab.Length; i++) if (m_SUI.Tab[i].gameObject.activeSelf) tabCnt++;
		return tabCnt;
	}

	public void SetTabUI()
	{
		m_SUI.Tab[(int)Type.BeginnerQuest].gameObject.SetActive(!Is_AllClearBeginMission);
		m_SUI.Tab[(int)Type.DailyMission].gameObject.SetActive(true);
		bool UseTabGroup = GetTabCnt() > 1;
		m_SUI.TabGroup.SetActive(UseTabGroup);
		m_SUI.Panels[(int)Type.DailyMission].GetComponent<Item_DailyMission>().SetRefeshBtnPosition(UseTabGroup ? 0 : 1);
	}

	public void CheckAlarm() {
		if(m_SUI.Tab[(int)Type.DailyMission].gameObject.activeSelf)m_SUI.Tab[(int)Type.DailyMission].SetAlram(USERINFO.m_Mission.IsSuccess(MissionMode.Day) || USERINFO.m_Mission.IsSuccess(MissionMode.DailyQuest));
		if (m_SUI.Tab[(int)Type.BeginnerQuest].gameObject.activeSelf) {
			var beqlist = USERINFO.m_Mission.Get_Missions(MissionMode.BeginnerQuest);
			var bcq = beqlist.FindAll(o => o.m_TData.m_Check[0].m_Type == MissionType.BeginnerQuestClear);
			int bcqcnum = 0;
			for (int i = 0; i < bcq.Count; i++) {
				if (bcq[i].IS_End() && bcqcnum < bcq[i].m_TData.m_ModeGid + 1) bcqcnum = bcq[i].m_TData.m_ModeGid + 1;
			}
			bool begin = beqlist.Find(o => o.IsPlayMission() && o.State[0] == RewardState.Idle && o.IS_Complete() && o.m_TData.m_ModeGid < bcqcnum) != null;
			m_SUI.Tab[(int)Type.BeginnerQuest].SetAlram(begin);
		}
	}
	/// <summary> 보상 받기 <summary>
	public void Click_GetReward(MissionData _info, Action _scb, Action _fcb) {
		m_Rewards.Clear();
#if NOT_USE_NET
		m_Rewards = MAIN.SetReward(_info.m_TData.m_Rewards[0].Kind, _info.m_TData.m_Rewards[0].Idx, _info.m_TData.m_Rewards[0].Cnt, true);
		MAIN.SetRewardList(new object[] { m_Rewards }, _scb);
		USERINFO.Check_Mission(MissionType.ModeClear, (int)_info.m_TData.m_Mode, 0, 1);
#else
		WEB.SEND_REQ_MISSION_REWARD((res) => {
			if (!res.IsSuccess()) {
				WEB.SEND_REQ_MISSIONINFO((res) => { _fcb.Invoke(); });
				return;
			}

			m_Rewards.AddRange(res.GetRewards());
			MAIN.SetRewardList(new object[] { m_Rewards }, _scb); 
		}, new List<MissionData>() { _info });
#endif
	}
	/// <summary> 해당 미션으로 바로 가기 </summary>
	public void Click_GoQuest(MissionData _info) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.MissionBtn, 6)) return;
		Main_Play play = POPUP.GetMainUI().GetComponent<Main_Play>();
		Item_PDA_Menu pda = play.GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
		Shop shop = play.GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();

		switch (_info.m_TData.m_Check[0].m_Type) {
			case MissionType.StageClear:
				switch (_info.m_TData.m_Check[0].m_Val[0]) {
					case -1:
						break;
					case 0:
						if (USERINFO.GetDifficulty() == _info.m_TData.m_Check[0].m_Val[1]) break;
						Item_StgDiffBtn diffbtn = null;
						switch (_info.m_TData.m_Check[0].m_Val[1]) {
							case 0:
								diffbtn = play.GetMenuUI(MainMenuType.Stage).GetComponent<Item_MainMenu_StgMain>().GetDiffBtn(StageDifficultyType.Normal);
								break;
							case 1:
								diffbtn = play.GetMenuUI(MainMenuType.Stage).GetComponent<Item_MainMenu_StgMain>().GetDiffBtn(StageDifficultyType.Hard);
								break;
							case 2:
								diffbtn = play.GetMenuUI(MainMenuType.Stage).GetComponent<Item_MainMenu_StgMain>().GetDiffBtn(StageDifficultyType.Nightmare);
								break;
						}
						if (diffbtn != null) {
							diffbtn.ClickBtn();
							if (!diffbtn.IS_Cango) return;
						}
						break;
					case 1:
						if (!USERINFO.CheckContentUnLock(ContentType.Bank, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.Bank);
						break;
					case 2:
						if (!USERINFO.CheckContentUnLock(ContentType.Academy, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.Academy);
						break;
					case 3:
						if (!USERINFO.CheckContentUnLock(ContentType.University, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.University);
						break;
					case 4:
						if (!USERINFO.CheckContentUnLock(ContentType.Tower, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.Tower);
						break;
					case 5:
						if (!USERINFO.CheckContentUnLock(ContentType.Cemetery, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.Cemetery);
						break;
					case 6:
						if (!USERINFO.CheckContentUnLock(ContentType.Factory, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.Factory);
						break;
					case 7:
						if (!USERINFO.CheckContentUnLock(ContentType.Subway, true)) return;
						play.MenuChange((int)MainMenuType.Dungeon, false);
						play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().ClickGoDungeon((int)StageContentType.Subway);
						break;
				}
				break;
			case MissionType.DailyQuestClear:
				return;
			case MissionType.CharLevel:
			case MissionType.CharLevelUp:
			case MissionType.CharGrade:
			case MissionType.SkillUpgrade:
			case MissionType.GradeUp:
			case MissionType.EquipLevelUp:
				if (!USERINFO.CheckContentUnLock(ContentType.Character, true)) return;
				play.MenuChange((int)MainMenuType.Character, false);
				break;
			case MissionType.Serum:
				if (!USERINFO.CheckContentUnLock(ContentType.Character, true)) return;
				if (!USERINFO.CheckContentUnLock(ContentType.Serum, true)) return;
				play.MenuChange((int)MainMenuType.Character, false);
				break;
			case MissionType.Making:
				if (!USERINFO.CheckContentUnLock(ContentType.Making, true)) return;
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(3);
				MAIN.GoPDAMaking(pda, _info.m_TData.m_Check[0].m_Val[0]);
				break;
			case MissionType.ADV:
				if (!USERINFO.CheckContentUnLock(ContentType.Explorer, true)) return;
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(1);
				break;
			case MissionType.ZombieDNA:
			case MissionType.ZombieDestory:
				if (!USERINFO.CheckContentUnLock(ContentType.ZombieFarm, true)) return;
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(4);
				break;
			case MissionType.BlackMarket:
				if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
				play.MenuChange((int)MainMenuType.Shop, false);
				shop.StartPos(true, ShopGroup.BlackMarket);
				break;
			case MissionType.Auction:
				if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
				play.MenuChange((int)MainMenuType.Shop, false);
				shop.StateChange((int)Shop.Tab.Auction);
				break;
			case MissionType.Friend:
			case MissionType.AddFriend:
				play.OnFriend();
				break;
			case MissionType.OpenSupplyBox:
				if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
				play.MenuChange((int)MainMenuType.Shop, false);
				shop.StartPos(true, ShopGroup.SupplyBox);
				break;
			case MissionType.CharGacha:
				if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
				play.MenuChange((int)MainMenuType.Shop, false);
				shop.StartPos(true, ShopGroup.Gacha);
				break;
			case MissionType.ItemGacha:
				if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
				play.MenuChange((int)MainMenuType.Shop, false);
				shop.StartPos(true, ShopGroup.ItemGacha);
				break;
			case MissionType.Remake:
				play.ViewInven();
				POPUP.GetPopup().GetComponent<Inventory>().SelectMenu((int)Inventory.EMenu.Equip);
				break;
			case MissionType.Research:
				if (!USERINFO.CheckContentUnLock(ContentType.Research, true)) return;
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(2);
				break;
			case MissionType.BuyStoreDoller:
				if (!USERINFO.CheckContentUnLock(ContentType.Store, true)) return;
				play.MenuChange((int)MainMenuType.Shop, false);
				break;
			case MissionType.PlayPVP:
			case MissionType.VicPVP:
			case MissionType.BuyPVPStore:
				if (!USERINFO.CheckContentUnLock(ContentType.PvP, true)) return;
				play.MenuChange((int)MainMenuType.Dungeon, false);
				play.GetMenuUI(MainMenuType.Dungeon).GetComponent<Item_DungeonMenu>().OpenPVP(()=> {
					if (_info.m_TData.m_Check[0].m_Type == MissionType.BuyPVPStore) {
						POPUP.GetPopup().GetComponent<PVP_Main>().Click_Store();
					}
				});
				break;
			case MissionType.Guild:
			case MissionType.GuildCheck:
			case MissionType.ButGuildStore:
			case MissionType.GetGuildResearch:
				if (!USERINFO.CheckContentUnLock(ContentType.Guild, true)) return;
				play.GoUnion(()=> {
					Union_JoinList union = POPUP.GetPopup().GetComponent<Union_JoinList>();
					if (_info.m_TData.m_Check[0].m_Type == MissionType.ButGuildStore) {
						union.GoShop();
					}
				}, () => {
					Union_Main union = POPUP.GetPopup().GetComponent<Union_Main>();
					if (_info.m_TData.m_Check[0].m_Type == MissionType.ButGuildStore) {
						union.Click_Store();
					}
					else if (_info.m_TData.m_Check[0].m_Type == MissionType.GetGuildResearch) {
						union.GoResearch();
					}
				});
				
				break;
			case MissionType.PlaceZombie:
			case MissionType.GetZombieRNA:
			case MissionType.ProduceDNA:
				if (!USERINFO.CheckContentUnLock(ContentType.ZombieFarm, true)) return;
				if (!USERINFO.CheckContentUnLock(ContentType.PDA, true)) return;
				play.MenuChange((int)MainMenuType.PDA, false);
				pda.ClickMenu(4);
				break;
		}
		Close(0);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		if (m_SUI.Anim != null) {
			m_SUI.Anim.SetTrigger("End");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		}
		base.Close(_result);
	}

	//튜토
	public GameObject GetPanel(int _pos) {
		return m_SUI.Panels[_pos];
	}
	public GameObject GetBtn(int _pos) {
		return m_SUI.Tab[_pos].gameObject;
	}
}
