using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static Utile_Class;
using static LS_Web;
using System.Linq;

public class Event_Welcome : PopupBase
{
	public enum MissionAniName
	{
		Deact = 0,
		Act,
		Recieved,
		None
	}
	[Serializable]
	public class MissionUI
	{
		public Animator Anim;
		public TextMeshProUGUI Desc;
		public Item_RewardList_Item Reward;
	}
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Msg;
		public TextMeshProUGUI Timer;
		public MissionUI[] Missions;
	}
	[SerializeField] SUI m_SUI;
	MyFAEvent m_Event;
	SND_IDX m_NowBG;
	List<MissionData> m_Miss;
	private IEnumerator Start()
	{
		yield return StartTimer();
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_NowBG = SND.GetNowBG;
		//PlayBGSound(SND_IDX.BGM_1000);
		m_Event = (MyFAEvent)aobjValue[0];
		m_Miss = USERINFO.m_Mission.Get_Missions(MissionMode.OpenEvent).FindAll(o => o.SIdx == m_Event.EventUID && o.IsPlayMission());
		m_Miss.Sort((befor, after) => {
			var tbe = befor.m_TData;
			var taf = after.m_TData;
			if (tbe.m_Check[0].m_Cnt != taf.m_Check[0].m_Cnt) return tbe.m_Check[0].m_Cnt.CompareTo(taf.m_Check[0].m_Cnt);
			return befor.Idx.CompareTo(after.Idx);
		});

		int rewcnt = m_Miss.Sum(o => o.m_TData.m_Rewards[0].Cnt);
		var rew = m_Miss[0].m_TData.m_Rewards[0].Get_RES_REWARD_BASE();
		m_SUI.Msg.text = string.Format(TDATA.GetString(1099), rewcnt, rew.GetName());
		m_SUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_Event.GetRemainEndTime() * 0.001d));
		base.SetData(pos, popup, cb, aobjValue);
	}

	IEnumerator StartTimer()
	{
		while(true)
		{
			var remain = m_Event.GetRemainEndTime() * 0.001d;
			m_SUI.Timer.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, remain));
			if (remain <= 0) break;
			yield return new WaitForEndOfFrame();
		}
	}

	public override void SetUI() {
		base.SetUI();

		for (int i = 0; i < m_SUI.Missions.Length; i++)
		{
			SetMissionAni(i);
			var info = m_Miss[i];
			var tdata = info.m_TData;
			int cnt = info.GetCnt(0);
			int max = tdata.m_Check[0].m_Cnt;
			MissionAniName ani = MissionAniName.Deact;
			if (info.IS_End()) ani = MissionAniName.Recieved;
			else if (cnt >= max) ani = MissionAniName.Act;
			m_SUI.Missions[i].Anim.SetTrigger(ani.ToString());
		}
	}

	void SetMissionAni(int pos)
	{
		var info = m_Miss[pos];
		var tdata = info.m_TData;
		int cnt = info.GetCnt(0);
		int max = tdata.m_Check[0].m_Cnt;

		MissionAniName ani = MissionAniName.Deact;
		if (info.IS_End()) ani = MissionAniName.Recieved;
		else if (cnt >= max) ani = MissionAniName.Act;
		m_SUI.Missions[pos].Anim.SetTrigger(ani.ToString());


		m_SUI.Missions[pos].Desc.text = $"{tdata.GetName()}({cnt}/{max})";
		List<RES_REWARD_BASE> reward = MAIN.GetRewardData(tdata.m_Rewards[0]);
		m_SUI.Missions[pos].Reward.SetData(tdata.m_Rewards[0].Get_RES_REWARD_BASE(), null, false);
	}

	public void OnClick_Reward(int Pos)
	{
		var info = m_Miss[Pos];
		PlayEffSound(SND_IDX.SFX_3030);
		WEB.SEND_REQ_MISSION_REWARD((res) => {
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_MISSIONINFO((res) => { SetMissionAni(Pos); });
				return;
			}
			MAIN.SetRewardList(new object[] { res.GetRewards() }, () => { SetMissionAni(Pos); });
		}, new List<MissionData>() { info });
	}

	public override void Close(int Result = 0)
	{
		//PlayBGSound(m_NowBG);
		base.Close(Result);
	}
}
