using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static Utile_Class;

public class Item_Event_10_Mission : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Timer;
		public Transform Bucket;
		public Transform Element;		//Item_Event_10_Mission_Element
	}
	[SerializeField] SUI m_SUI;
	MyFAEvent m_Event;
	IEnumerator m_TimerCor;

	private void Update() {
		if (m_Event == null) return;
		m_SUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(TimeStyle.single, m_Event.GetRemainEndTime() * 0.001d));
	}
	public void SetData(MyFAEvent _event) {
		m_Event = _event;

		List<MissionData> missions = USERINFO.m_Mission.Get_Missions(MissionMode.Event_CharMission);
		missions.FindAll(o => o.State[0] != RewardState.Get || (o.State[0] == RewardState.Get && o.m_TData.m_LinkIdx == 0));
		MissionInfo.Sort(missions);
		UTILE.Load_Prefab_List(missions.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < missions.Count; i++) {
			Item_Event_10_Mission_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Event_10_Mission_Element>();
			element.SetData(missions[i], GetReward);
		}
	}
	public void GetReward(MissionData _info) {
		if (IS_EvtEnd()) return;

		if (_info.State[0] == RewardState.Get) return;
		if (!_info.IS_Complete()) return;
		PlayEffSound(SND_IDX.SFX_3030);
		WEB.SEND_REQ_MISSION_REWARD((res) => {
			if (!res.IsSuccess()) {
				WEB.SEND_REQ_MISSIONINFO((res) => { SetData(m_Event); });
				return;
			}
			MAIN.SetRewardList(new object[] { res.GetRewards() }, () => { SetData(m_Event); });
		}, new List<MissionData>() { _info });
	}
	bool IS_EvtEnd() {
		if (m_Event.GetRemainEndTime() <= 0) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(2036));
			return true;
		}
		else return false;
	}
}
