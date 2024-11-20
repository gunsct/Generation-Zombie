using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using Coffee.UIEffects;

[System.Serializable] public class DicItem_BeginnerQuest_AllClear_State : SerializableDictionary<MissionState, GameObject> { }
public class Item_BeginnerQuest : ObjMng
{
	public enum TabMissionState
	{
		Play = 0,
		Lock,
		End
	}

	[Serializable]
	public struct STabUI
	{
		public Transform Prefab;//Item_BeginnerQuest_Tab
		public ScrollRect Scroll;
	}
	[Serializable]
	public struct SClearMissionUI
	{
		public GameObject Obj;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
		public Item_RewardList_Item Reward;
		[ReName("Mission", "Lock", "GetMark", "RewardGet")]
		public GameObject[] Actives;
		public TextMeshProUGUI LockLabel;
	}
	[Serializable]
	public struct SCompGetMissionUI
	{
		public GameObject Obj;
		public Item_RewardList_Item Reward;

	}
	[Serializable]
	public struct SMissionsUI
	{
		public Transform Prefab;//Item_BeginnerQuest_Element
		public ScrollRect Scroll;
	}
	[Serializable]
	public struct SAllClearRewardUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
		public Item_GradeGroup Grade;

		public DicItem_BeginnerQuest_AllClear_State State;
	}

	[Serializable]
	public struct SUI
	{
		public STabUI Tabs;
		public SAllClearRewardUI AllClearReward;

		public SClearMissionUI Clear;
		public SCompGetMissionUI CompGet;

		public SMissionsUI Missions;
	}
	[SerializeField] SUI m_SUI;
	NewNDailyMission m_Parent;
	Item_BeginnerQuest_Tab m_SelectTab;
	Dictionary<int, List<MissionData>> m_AllMissions;
	MissionData m_AllClearMission;
	MissionData m_ClearMission;
	List<MissionData> m_Missions;
	bool Is_Action;
	public void SetData(NewNDailyMission _parent) {
		m_Parent = _parent;
		m_AllMissions = USERINFO.m_Mission.GetBeginnerQuest();
		m_AllClearMission = m_AllMissions.Last().Value.Find(o => o.m_TData.m_Check[0].m_Type == MissionType.ModeClear);
		m_SelectTab = null;
		LoadTabList();
	}

	public void LoadTabList()
	{
		Item_BeginnerQuest_Tab Comple = null;
		Item_BeginnerQuest_Tab Open = null;
		Item_BeginnerQuest_Tab Select = null;

		UTILE.Load_Prefab_List(m_AllMissions.Count, m_SUI.Tabs.Scroll.content, m_SUI.Tabs.Prefab);
		for(int i = 0; i < m_AllMissions.Count; i++)
		{
			Item_BeginnerQuest_Tab element = m_SUI.Tabs.Scroll.content.GetChild(i).GetComponent<Item_BeginnerQuest_Tab>();
			element.SetData(i, string.Format(TDATA.GetString(912), i + 1), SelectTab);
			SetAlram(i);

			var group = m_AllMissions.ElementAt(i);
			var clearmiss = group.Value.Find(o => o.m_TData.m_Check[0].m_Type == MissionType.BeginnerQuestClear);
			if (clearmiss.STime <= UTILE.Get_ServerTime_Milli())
			{
				Open = element;
				if (group.Value.Any(o => !o.IS_End() && o.IS_Complete())) Comple = element;
			}
		}

		Select = Comple != null ? Comple : Open;

		if (Select != null)
		{
			Select.OnClick();
			// 해당 위치로 이동
			var rtfitem = (RectTransform)Select.transform;
			var layoutgroup = m_SUI.Tabs.Scroll.content.GetComponent<HorizontalLayoutGroup>();
			float scrollValue = (rtfitem.anchoredPosition.x + rtfitem.rect.x - layoutgroup.padding.left) / (m_SUI.Tabs.Scroll.content.rect.width - m_SUI.Tabs.Scroll.viewport.rect.width);
			m_SUI.Tabs.Scroll.horizontalNormalizedPosition = Mathf.Clamp(scrollValue, 0f, 1f);
		}
		else
		{
			m_SUI.Tabs.Scroll.content.GetChild(0).GetComponent<Item_BeginnerQuest_Tab>().OnClick();
			m_SUI.Tabs.Scroll.horizontalNormalizedPosition = 0;
		}
	}

	bool SelectTab(Item_BeginnerQuest_Tab item)
	{
		if (m_SelectTab == item) return false;
		if(m_SelectTab != null)
		{
			m_SelectTab.SetActive(false);
			// 알람 상태 셋팅
			SetAlram(m_SelectTab.m_Pos);
			m_SelectTab = null;
		}
		m_SUI.Missions.Scroll.verticalNormalizedPosition = 1f;
		m_SelectTab = item;
		SetUI();
		return true;
	}

	void SetUI()
	{
		SetAllClearRewardUI();
		var group = m_AllMissions.ElementAt(m_SelectTab.m_Pos);
		m_ClearMission = group.Value.Find(o => o.m_TData.m_Check[0].m_Type == MissionType.BeginnerQuestClear);
		m_Missions = group.Value.FindAll(o => o.m_TData.m_Check[0].m_Type != MissionType.BeginnerQuestClear && o.m_TData.m_Check[0].m_Type != MissionType.ModeClear);
		SetAlram(m_SelectTab.m_Pos);
		SetClearRewardUI();
		LoadMissions();
	}

	TabMissionState GetTabState(int pos)
	{
		var group = m_AllMissions.ElementAt(pos);
		var clear = group.Value.Find(o => o.m_TData.m_Check[0].m_Type == MissionType.BeginnerQuestClear);
		TabMissionState state = TabMissionState.Play;
		if (clear.STime > UTILE.Get_ServerTime_Milli()) state = TabMissionState.Lock;
		else if (clear.IS_End()) state = TabMissionState.End;
		return state;
	}

	void SetAlram(int pos)
	{
		var item = m_SUI.Tabs.Scroll.content.GetChild(pos).GetComponent<Item_BeginnerQuest_Tab>();
		var group = m_AllMissions.ElementAt(item.m_Pos);
		var state = GetTabState(pos);
		var clear = group.Value.Find(o => o.m_TData.m_Check[0].m_Type == MissionType.BeginnerQuestClear);
		Item_BeginnerQuest_Tab.AlramMode mode = Item_BeginnerQuest_Tab.AlramMode.None;
		if (state == TabMissionState.Lock) mode = Item_BeginnerQuest_Tab.AlramMode.Lock;
		else if(state == TabMissionState.End) mode = Item_BeginnerQuest_Tab.AlramMode.Complete;
		else if(group.Value.Any(o => !o.IS_End() && o.IS_Complete())) mode = Item_BeginnerQuest_Tab.AlramMode.Notice;
		item.SetAlram(mode);
	}

	//모든 단계 클리어 보상
	void SetClearRewardUI()
	{
		var state = GetTabState(m_SelectTab.m_Pos);
		var tdata = m_ClearMission.m_TData;
		switch (state)
		{
		case TabMissionState.Lock:
			m_SUI.Clear.Actives[0].SetActive(false);
			m_SUI.Clear.Actives[1].SetActive(true);
			m_SUI.Clear.LockLabel.text = string.Format(TDATA.GetString(1079), tdata.m_ModeGid);
			m_SUI.Clear.Obj.SetActive(false);
			m_SUI.CompGet.Obj.SetActive(false);
			break;
		default:
			RES_REWARD_BASE reward = m_ClearMission.m_TData.m_Rewards[0].Get_RES_REWARD_BASE();
			bool canget = !m_ClearMission.IS_End() && m_ClearMission.IS_Complete();
			m_SUI.Clear.Obj.SetActive(!canget);
			m_SUI.CompGet.Obj.SetActive(canget);
			if (canget) {
				m_SUI.CompGet.Reward.SetData(reward, IsStartEff: false);
			}
			else {
				m_SUI.Clear.Actives[0].SetActive(true);
				m_SUI.Clear.Actives[1].SetActive(false);
				m_SUI.Clear.Actives[2].SetActive(state == TabMissionState.End);
				m_SUI.Clear.Actives[3].SetActive(!m_ClearMission.IS_End() && m_ClearMission.IS_Complete());
				m_SUI.Clear.Reward.SetData(reward, IsStartEff: false);
				m_SUI.Clear.Name.text = tdata.GetName();
				m_SUI.Clear.Cnt.text = string.Format("{0} / {1}", m_ClearMission.GetCnt(0), tdata.m_Check[0].m_Cnt);
			}
			break;
		}
	}

	void LoadMissions()
	{
		MissionInfo.Sort(m_Missions);
		var state = GetTabState(m_SelectTab.m_Pos);
		UTILE.Load_Prefab_List(m_Missions.Count, m_SUI.Missions.Scroll.content, m_SUI.Missions.Prefab);

		for (int i = 0; i < m_Missions.Count; i++)
		{
			Item_BeginnerQuest_Element element = m_SUI.Missions.Scroll.content.GetChild(i).GetComponent<Item_BeginnerQuest_Element>();
			element.SetData(m_Missions[i], state, Click_MissionReward, Click_GoQuest);
		}
	}


	//모든 단계 클리어 보상
	void SetAllClearRewardUI()
	{
		TMissionTable tmission = m_AllClearMission.m_TData;
		TCharacterTable data = TDATA.GetCharacterTable(tmission.m_Rewards[0].Idx);
		m_SUI.AllClearReward.Name.text = string.Format(TDATA.GetString(914), data.GetCharName());
		m_SUI.AllClearReward.Cnt.text = string.Format("{0} / {1}", m_AllClearMission.GetCnt(0), tmission.m_Check[0].m_Cnt);
		m_SUI.AllClearReward.Grade.SetData(data.m_Grade);

		if(m_AllClearMission.IS_End())
		{
			m_SUI.AllClearReward.State[MissionState.Idle].SetActive(false);
			m_SUI.AllClearReward.State[MissionState.Reward].SetActive(false);
			m_SUI.AllClearReward.State[MissionState.End].SetActive(true);
		}
		else if(m_AllClearMission.IS_Complete())
		{
			m_SUI.AllClearReward.State[MissionState.Idle].SetActive(false);
			m_SUI.AllClearReward.State[MissionState.Reward].SetActive(true);
			m_SUI.AllClearReward.State[MissionState.End].SetActive(false);
		}
		else
		{
			m_SUI.AllClearReward.State[MissionState.Idle].SetActive(true);
			m_SUI.AllClearReward.State[MissionState.Reward].SetActive(false);
			m_SUI.AllClearReward.State[MissionState.End].SetActive(false);
		}
	}

	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 버튼
	/// <summary> 전체완료보상 정보 </summary>
	public void ClickViewCharInfo()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.MissionBtn, 8)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_NotGet, null, m_AllClearMission.m_TData.m_Rewards[0].Idx, Info_Character_NotGet.State.Normal);
	}

	public void Click_AllClearReward()
	{
		GetReward(m_AllClearMission);
	}

	public void Click_ClearReward()
	{
		GetReward(m_ClearMission);
	}

	void Click_MissionReward(MissionData mission)
	{
		GetReward(mission);
	}

	void Click_GoQuest(MissionData mission)
	{
		m_Parent.Click_GoQuest(mission);
	}

	void GetReward(MissionData mission)
	{
		if (Is_Action) return;
		if (mission.IS_End()) return;
		if (!mission.IS_Complete()) return;
		Is_Action = true;
		List<RES_REWARD_BASE> Rewards = new List<RES_REWARD_BASE>();
#if NOT_USE_NET
		Rewards = MAIN.SetReward(mission.m_TData.m_Rewards[0].Kind, mission.m_TData.m_Rewards[0].Idx, mission.m_TData.m_Rewards[0].Cnt, true);
		MAIN.SetRewardList(new object[] { Rewards }, () => SetUI());
		USERINFO.Check_Mission(MissionType.ModeClear, (int)mission.m_TData.m_Mode, 0, 1);
#else
		WEB.SEND_REQ_MISSION_REWARD((res) => {
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_MISSIONINFO((res) => { SetUI(); });
				return;
			}
			Rewards.AddRange(res.GetRewards());
			MAIN.SetRewardList(new object[] { Rewards }, () => SetUI());
			Is_Action = false;
		}, new List<MissionData>() { mission });
#endif
	}
	//튜토
	public GameObject GetMissionList() {
		return m_SUI.Missions.Scroll.gameObject;
	}
}
