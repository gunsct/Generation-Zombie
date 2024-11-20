using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static Item_BeginnerQuest;

public class Item_BeginnerQuest_Element : ObjMng
{
	public enum AniName
	{
		Normal = 0,
		Lock,
		Get,
		Complete
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Item_RewardList_Item Reward;
		public Image Gauge;
		public TextMeshProUGUI Cnt;
		public TextMeshProUGUI Name;
	}
	[SerializeField] SUI m_SUI;
	MissionData m_Info;
	Action<MissionData> m_CBGet;
	Action<MissionData> m_CBGo;
	TabMissionState m_ClearState;
	TMissionTable m_TData { get { return m_Info.m_TData; } }
	public MissionData GetInfo {get { return m_Info; } }
	public TMissionTable.TMissionCheck MCheck { get { return m_TData.m_Check[0]; } }

	public void SetData(MissionData _info, TabMissionState state, Action<MissionData> _cbget, Action<MissionData> _cbgo) {
		m_Info = _info;
		m_CBGet = _cbget;
		m_CBGo = _cbgo;
		m_ClearState = state;
		//ui
		m_SUI.Name.text = string.Format(m_TData.GetName(), MCheck.m_Cnt);
		m_SUI.Cnt.text = string.Format("{0} / {1}", m_Info.GetCnt(0), MCheck.m_Cnt);
		m_SUI.Gauge.fillAmount = m_Info.GetPer();
		m_SUI.Reward.SetData(MAIN.GetRewardData(m_TData.m_Rewards[0].Kind, m_TData.m_Rewards[0].Idx, m_TData.m_Rewards[0].Cnt)[0], null, false);
		SetState();
	}

	/// <summary> 상태별 세팅 </summary>
	void SetState() {
		AniName ani = m_ClearState == TabMissionState.Lock ? AniName.Lock : AniName.Normal;
		if (m_ClearState == TabMissionState.Lock) ani = AniName.Lock;
		else if(m_Info.IS_End()) ani = AniName.Complete;
		else if (m_Info.m_TData.m_Check[0].m_Cnt <= m_Info.GetCnt(0) && m_ClearState != TabMissionState.Lock) ani = AniName.Get;
		SetAnim(ani);
		//m_Info.State[0] = _state;
		//m_SUI.Btns[0].SetActive(m_Info.State[0] != RewardState.Get && !m_Info.IS_Complete() && MCheck.m_Type != MissionType.DailyQuestClear);
		//m_SUI.Btns[1].SetActive(m_Info.State[0] != RewardState.Get && m_Info.IS_Complete());
		//if(m_SUI.Btns[2] != null) m_SUI.Btns[2].SetActive(MCheck.m_Type != MissionType.DailyQuestClear && m_Info.State[0] != RewardState.Get && m_RFCnt < 4);
		//m_SUI.BtnShadow.enabled = MCheck.m_Type != MissionType.DailyQuestClear || (MCheck.m_Type == MissionType.DailyQuestClear && m_Info.State[0] != RewardState.Get && m_Info.IS_Complete());
		//m_SUI.GetMark.SetActive(m_Info.State[0] != RewardState.Get && m_Info.IS_Complete());
		//m_SUI.CompMark.SetActive(m_Info.State[0] == RewardState.Get && m_Info.IS_Complete()); 

		//switch (m_Info.State[0]) {
		//	case RewardState.None:
		//		m_SUI.Anim.SetTrigger("Normal");
		//		break;
		//	case RewardState.Idle:
		//		m_SUI.Anim.SetTrigger("Normal");
		//		break;
		//	case RewardState.Get:
		//		m_SUI.Anim.SetTrigger("Complete");
		//		break;
		//}
	}
	public void SetAnim(AniName _anim) {
		m_SUI.Anim.SetTrigger(_anim.ToString());
	}
	/// <summary> 보상 받기 </summary>
	public void Click_GetReward() {
		if (m_ClearState == TabMissionState.Lock) return;
		if (m_Info.IS_End()) return;
		if (!m_Info.IS_Complete()) return;
		m_CBGet?.Invoke(m_Info);
	}
	/// <summary> 미션 바로 가기 </summary>
	public void Click_GoQuest()
	{
		//if (m_ClearState == TabMissionState.Lock) return;
		if (m_Info.IS_End()) return;
		if (m_Info.IS_Complete()) return;
		m_CBGo?.Invoke(m_Info);
	}
}
