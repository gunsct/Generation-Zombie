using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class Item_AdventrueList : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Name;
		public GameObject ItemPrefab;
		public Material ItemMaterial;
		public Transform RewardBucket;
		public GameObject[] NeedCharCnt;
		public Item_DifficultyGroup DiffGroup;
		public GameObject[] TimeInfo;
		public TextMeshProUGUI[] TimeTxt;
		public Image TimeBar;
		public GameObject[] Btns;
	}
	[SerializeField]
	SUI m_SUI;
	public TimeContentState m_State { get { return m_Info.m_State; } }
	Action<Item_AdventrueList> m_CB;
	Action m_AllGetAlarmCB;
	public TAdventureTable m_TData { get { return m_Info.m_TData; } }
	public AdventureInfo m_Info;
	int m_UIState;
	private void Awake() {
		m_SUI.TimeBar.fillAmount = 0;
	}
	private void Update() {
		if(m_State == TimeContentState.Play && m_UIState != 2) {
			m_SUI.TimeTxt[1].text = UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.day_hr_min_sec, m_Info.GetRemainTime());
			m_SUI.TimeBar.fillAmount = (float)(1f - m_Info.GetRemainTime() * 1000d / m_Info.GetMaxTime());
			if (m_Info.IS_Complete()) RefreshUI();
		}
	}
	public void SetData(AdventureInfo _info, Action<Item_AdventrueList> _cb, Action _alarmcb) {
		m_CB = _cb;
		m_AllGetAlarmCB = _alarmcb;
		m_Info = _info;
		m_UIState = -1;
		//공통 유아이
		//탐험 이름
		m_SUI.Name.text = m_TData.GetName();
		//등급
		m_SUI.DiffGroup.SetData(m_TData.m_AdventureGrade);
		//인원수
		for (int i = 0; i < m_SUI.NeedCharCnt.Length; i++) {
			m_SUI.NeedCharCnt[i].SetActive(i < m_TData.m_PartyCount ? true :false);
		}
		//기본 시간
		m_SUI.TimeTxt[0].text = string.Format(TDATA.GetString(226), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.day_hr_min_sec, m_TData.GetTime() * 0.001f));
		//상태별 유아이(버튼, 시간)
		RefreshUI();
		//보상
		UTILE.Load_Prefab_List(m_TData.m_Reward.Count, m_SUI.RewardBucket, m_SUI.ItemPrefab.transform);
		for (int i = 0; i < m_TData.m_Reward.Count; i++) {
			Item_RewardItem_Card card = m_SUI.RewardBucket.GetChild(i).GetComponent<Item_RewardItem_Card>();
			card.SetMaterial(m_SUI.ItemMaterial);
			card.transform.GetComponent<Animator>().enabled = false;
			card.transform.localScale = Vector3.one * 0.6f;
			TAdventureTable.ADReward reward = m_TData.m_Reward[i];
			card.SetData(reward.m_Idx, reward.m_Cnt);
		}
	}
	/// <summary> 상태별 유아이 </summary>
	public void RefreshUI() {
		int activepos = 0;
		switch (m_State)
		{
		case TimeContentState.Play:
			if (m_Info.IS_Complete()) {
				m_AllGetAlarmCB?.Invoke();
				activepos = 2;
			}
			else activepos = 1;
			break;
		}
		if (m_UIState == activepos) return;
		for(int i = 0; i < 3; i++)
		{
			bool active = i == activepos;
			m_SUI.TimeInfo[i].SetActive(active);
			m_SUI.Btns[i].SetActive(active);
		}
		m_UIState = activepos;
	}
	/// <summary> 파견 세팅창 연결 </summary>
	public void ClickDispatch() {
		m_CB?.Invoke(this);
	}
	/// <summary> 가속 구입 메시지 연결 </summary>
	public void ClickFastDispatch() {
		PlayEffSound(SND_IDX.SFX_0121);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, string.Empty, TDATA.GetString(227), (result, obj) => {
			if (result == 1) {
				if (obj.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
					m_CB?.Invoke(this);
				}
				else {
					POPUP.StartLackPop(BaseValue.CASH_IDX);
				}
			}
		}, PriceType.Cash, BaseValue.CASH_IDX, BaseValue.GetTimePrice(ContentType.Explorer, m_Info.GetRemainTime()), false);
	}
	/// <summary> 보상 수령 </summary>
	public void ClickComplete() {
		m_CB?.Invoke(this);
	}
}
