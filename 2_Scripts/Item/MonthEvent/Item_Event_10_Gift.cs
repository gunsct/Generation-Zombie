using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_Event_10_Gift : ObjMng
{
	[Serializable]
	public struct SCUI
	{
		public Item_RewardList_Item Reward;
		public TextMeshProUGUI Title;
		public Image Gauge;
	}
    [Serializable]
    public struct SUI
	{
		public SCUI ClearReward;
		public Transform Element;   //Item_Event_10_Gift_Element
		public Transform Bucket;
		public TextMeshProUGUI[] ItemCnts;
		public TextMeshProUGUI Timer;
	}
	[SerializeField] SUI m_SUI;
	List<MissionData> m_Missions = new List<MissionData>();
	MissionData m_ClearMission;
	MyFAEvent m_Event;
	double m_Timer = 0;

	private void Update() {
		if (m_Event == null) return;
		double timer = (UTILE.Get_ZeroTime() + 86400000 - UTILE.Get_ServerTime_Milli()) * 0.001d;
		if (m_Timer < timer) SetData(m_Event);
		if (timer < 0) timer = 0;
		
		m_SUI.Timer.text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, timer);
		m_Timer = timer;
	}
	public void SetData(MyFAEvent _event) {
		m_Event = _event;
		m_Timer = (UTILE.Get_ZeroTime() + 86400000 - UTILE.Get_ServerTime_Milli()) * 0.001d;
		DLGTINFO?.f_RFCashUI(USERINFO.m_Cash, USERINFO.m_Cash);
		m_Missions = USERINFO.m_Mission.Get_Missions(MissionMode.Event_miniGame);
		MissionInfo.Sort(m_Missions);
		m_ClearMission = USERINFO.m_Mission.Get_Missions(MissionMode.Event_miniGame_Clear)[0];

		//클리어보상
		PostReward cleardata = m_ClearMission.m_TData.m_Rewards[0];
		List<RES_REWARD_BASE> clearrewards = MAIN.GetRewardData(cleardata.Kind, cleardata.Idx, cleardata.Cnt, true, false);
		m_SUI.ClearReward.Reward.SetData(clearrewards[0], null, false);
		m_SUI.ClearReward.Title.text = string.Format(TDATA.GetString(2010), m_ClearMission.GetCnt(0), m_ClearMission.m_TData.m_Check[0].m_Cnt);
		m_SUI.ClearReward.Gauge.fillAmount = (float)m_ClearMission.GetCnt(0) / (float)m_ClearMission.m_TData.m_Check[0].m_Cnt;
		//이외
		UTILE.Load_Prefab_List(m_Missions.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i< m_Missions.Count; i++) {
			Item_Event_10_Gift_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Event_10_Gift_Element>();
			element.SetData(m_Missions[i], Click_GetReward, Click_Refresh);
		}

		for (int i = 0; i < m_SUI.ItemCnts.Length; i++) {
			m_SUI.ItemCnts[i].text = USERINFO.GetItemCount(BaseValue.EVENT_10_ITEMIDX[i]).ToString();
		}
	}
	public void Click_GetClearReward() {
		Click_GetReward(m_ClearMission);
	}
	public void Click_GetReward(MissionData _info) {
		if (IS_EvtEnd()) return;
		if (_info.State[0] == RewardState.Get) return;
		if (!_info.IS_Complete()) {
			if (_info != m_ClearMission) {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Event_10_Gift_Purchase, (res, obj) => {
					if (res == 0) return;
					PlayEffSound(_info.m_TData.m_Mode == MissionMode.Event_miniGame_Clear ? SND_IDX.SFX_3031 : SND_IDX.SFX_3030);
					USERINFO.m_Mission.GetReward(_info, 0, (res) => {
						if (res.IsSuccess()) {
							MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
								WEB.SEND_REQ_MISSIONINFO((res) => {
									if (res.IsSuccess()) SetData(m_Event);
								});

							});
						}
					});
				}, _info);
			}
			else POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2026));
		}
		else {
			PlayEffSound(_info.m_TData.m_Mode == MissionMode.Event_miniGame_Clear ? SND_IDX.SFX_3031 : SND_IDX.SFX_3030);
			USERINFO.m_Mission.GetReward(_info, 0, (res) => {
				if (res.IsSuccess()) {
					MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
						WEB.SEND_REQ_MISSIONINFO((res) => {
							if (res.IsSuccess()) {
								PlayerPrefs.DeleteKey(string.Format("EVENT_FACE_MSG_{0}", _info.UID));
								SetData(m_Event);
							}
						});
					});
				}
			});
		}
	}
	public void Click_Refresh(Item_Event_10_Gift_Element _info) {
		if (IS_EvtEnd()) return;

		PlayEffSound(SND_IDX.SFX_3050);
		TShopTable sdata = TDATA.GetShopTable(BaseValue.SHOP_IDX_EVENT_MISSION_RESET);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(2035), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					WEB.SEND_REQ_MISSION_RESET((res) => {
						if (res.IsSuccess()) {
							PlayerPrefs.DeleteKey(string.Format("EVENT_FACE_MSG_{0}", _info.m_Info.UID));
							SetData(m_Event);
						}
					}, sdata.m_Idx, _info.m_Info);
				}
				else {
					POPUP.StartLackPop(sdata.m_PriceIdx);
				}
			}
		}, sdata.m_PriceType, sdata.m_PriceIdx, sdata.GetPrice(), false);
	}
	public void Click_RewardList() {
		if (IS_EvtEnd()) return;

		PlayEffSound(SND_IDX.SFX_3050);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Event_10_Gift_RewardList);
	}

	bool IS_EvtEnd() {
		if (m_Event.GetRemainEndTime() <= 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2036));
			return true;
		}
		else return false;
	}
}
