using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using Coffee.UIEffects;
using UnityEngine.Rendering;
using System.Linq;

public class Item_CharManageCard : Item_CharacterCard
{
	public enum Mode
	{
		None,
		Management,
		DeckSetting,
		DeckAdventure,
		PVP,
		Event
	}
	
	public enum State
	{
		None,
		Click,
		Hold
	}
	Mode m_Mode;
	public State m_State;
	[SerializeField] GameObject RankUpFrameAlarm;
	[SerializeField] Transform m_Bucket; //드래그할때 움직일 오브제
	[SerializeField] GameObject[] m_CheckInDeck;
	[SerializeField] GameObject m_CheckRecommand;
	[SerializeField] GameObject[] m_CheckNeed;
	[SerializeField] GameObject m_CheckExcept;
	[SerializeField] GameObject m_NewAlarm;
	[SerializeField] GameObject DiffLockJobGroup;
	[SerializeField] GameObject[] DiffLockJobIcon;
	[SerializeField] Image[] DiffLockJobIconImg;
	[SerializeField] GameObject SortGroup;
	[SerializeField] Image SortIcon;
	[SerializeField] Sprite[] SortSprites;
	[SerializeField] TextMeshProUGUI SortVal;
	[SerializeField] Item_CharManageCard_EventReward EventReward;
	Action<Item_CharManageCard> m_CB;
	public bool m_UseLock;
	public bool m_Recommand;
	public bool m_Need;
	public bool m_Except;
	List<Item_CharManageCard> m_AllList;
	public new CharInfo m_Info { get { return USERINFO.GetChar(m_Idx); } }

	private void Awake() {
		for(int i = 0;i< m_CheckInDeck.Length;i++) m_CheckInDeck[i].SetActive(false);
		SetCheckRecommand(false);
		SetCheckNeed(false);
		SetCheckExcept(false);
		SetNewAlarm(false);
		DiffLockJobGroup.SetActive(false);
	}
	public void SetData(int _idx, List<Item_CharManageCard> _list, Mode _mode = Mode.None, Action<Item_CharManageCard> _cb = null) {
		m_Idx = _idx;
		m_AllList = _list;
		m_Mode = _mode;
		m_CB = _cb;

		m_State = State.None;
		if (m_Mode == Mode.PVP) {
			m_SUI.JobIcon[0].transform.parent.localPosition = m_SUI.JobIconPos[2];
			m_SUI.JobIcon[1].transform.parent.localPosition = m_SUI.JobIconPos[3];
			m_SUI.JobIcon[0].transform.parent.gameObject.SetActive(m_TData.m_PVPPosType == PVPPosType.Combat);
			m_SUI.JobIcon[1].transform.parent.gameObject.SetActive(m_TData.m_PVPPosType == PVPPosType.Combat);
			m_SUI.JobIcon[0].sprite = BaseValue.GetPVPEquipAtkIcon(TDATA.GeTPVPSkillTable(m_TData.m_PVPSkillIdx).m_AtkType);
			m_SUI.JobIcon[1].sprite = BaseValue.GetPVPEquipDefIcon(m_TData.m_PVPArmorType);
		}
		else {
			m_SUI.JobIcon[0].transform.parent.localPosition = m_SUI.JobIconPos[0];
			m_SUI.JobIcon[1].transform.parent.localPosition = m_SUI.JobIconPos[1];
			for (int i = 0; i < m_SUI.JobIcon.Length; i++) {
				if (i < m_TData.m_Job.Count) {
					m_SUI.JobIcon[i].sprite = m_TData.GetJobIcon()[i];
					m_SUI.JobIcon[i].transform.parent.gameObject.SetActive(true);
				}
				else
					m_SUI.JobIcon[i].transform.parent.gameObject.SetActive(false);
			}
		}
		m_SUI.CharImg.sprite = m_TData.GetPortrait();

		EventReward.gameObject.SetActive(m_Mode == Mode.Event);
		if(m_Mode == Mode.Event) {
			MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => BaseValue.EVENT_LIST.Contains(o.Type));
			if(evt == null) EventReward.gameObject.SetActive(false);
			FAEventData_StageChar charreward = null;
			switch (evt.Type) {
				case LS_Web.FAEventType.Stage_Minigame:
					FAEventData_Stage_Minigame minigame = (FAEventData_Stage_Minigame)evt.RealData;
					charreward = minigame.StageCharReward.Find(o => o.Idx == m_Idx);
					break;
				case LS_Web.FAEventType.GrowUP:
					FAEventData_GrowUP growup = (FAEventData_GrowUP)evt.RealData;
					charreward = growup.StageCharReward.Find(o => o.Idx == m_Idx);
					break;
			}
			if (charreward == null) EventReward.gameObject.SetActive(false);
			else {
				m_Recommand = true;
				EventReward.SetData(charreward.Reward.Idx, charreward.Reward.Cnt);
			}
		}

		SortGroup.gameObject.SetActive((m_Mode == Mode.DeckSetting || m_Mode == Mode.PVP || m_Mode == Mode.Event) && m_Info != null);
		SetSortVal();

		SetLvGrade();
		SetRankUpAlarm();
		SetNewAlarm(m_Info != null ? m_Info.m_GetAlarm : false);
		MoveInit();
	}
	public void SetCheckInDeck(List<int> _deckIdx) {
		for (int i = 0; i < m_CheckInDeck.Length; i++) m_CheckInDeck[i].SetActive(false);
		if (_deckIdx == null) return;
		for(int i = 0;i< _deckIdx.Count; i++) {
			m_CheckInDeck[_deckIdx[i]].SetActive(true);
		}
	}
	public void SetCheckRecommand(bool _in) {
		m_Recommand = _in;
		m_CheckRecommand.SetActive(m_Recommand);
	}
	public void SetCheckNeed(bool _need) {
		m_Need = _need;
		if (_need) SetCheckRecommand(false);
		for(int i = 0;i< m_CheckNeed.Length;i++) m_CheckNeed[i].SetActive(m_Need);
	}
	public void SetCheckExcept(bool _except) {
		m_Except = _except;
		m_CheckExcept.SetActive(m_Except);
	}
	void MoveInit() {
		m_Bucket.localPosition = Vector3.zero;
	}
	public void SetLvGrade() {
		if (m_Info != null) {//획득 캐릭터
			m_SUI.Lv.text = m_Info.m_LV.ToString();
			SetGrade(m_Info.m_Grade);
			SetCharState(true);
		}
		else {//미획득 캐릭터
			m_SUI.Lv.text = "1";
			SetGrade(0);
			SetCharState(false);
		}
	}

	/// <summary> 난이도에 따른 캐릭터 잠금 표시 </summary>
	public void SetUseLock(bool _lock, JobType _lockjob = JobType.None) {
		m_UseLock = _lock;
		DiffLockJobGroup.SetActive(_lock);
		for(int i = 0;i< 2; i++) {
			if (i < m_TData.m_Job.Count) {
				DiffLockJobIcon[i].SetActive(_lock && m_TData.m_Job[i] == _lockjob);
				DiffLockJobIconImg[i].sprite = m_TData.GetJobIcon()[i];
			}
			else DiffLockJobIcon[i].SetActive(false);
		}
	}
	//관리창 카드 선택
	public void ClickCard()
	{
		if (m_UseLock) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_CharManageCard_Select, State.Click, this)) return;
		m_State = State.Click;
		m_CB?.Invoke(this);
		if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Select_Char_1024)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Select_Char_1036)) TUTO.Next();
	}
	//상세 정보
	public void OpenDetail()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_CharManageCard_Select, State.Hold, this)) return;
		m_State = State.Hold;

		List<CharInfo> charinfos = m_AllList.FindAll(t => t.gameObject.activeSelf == true).Select(o => o.m_Info).ToList();
		USERINFO.ShowCharInfo(m_Idx, charinfos, (result, obj) => {
			m_State = State.None;
			DLGTINFO.f_RFCharInfoCard?.Invoke();
		}, m_Mode == Mode.PVP);
	}
	public void OpenDetailStrSol() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_CharManageCard_Select, State.Hold, this)) return;
		m_State = State.Hold;

		List<CharInfo> charinfos = new List<CharInfo>();
		for(int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			CharInfo info = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[i]);
			charinfos.Add(info);
		}
		USERINFO.ShowCharInfo(m_Idx, charinfos, (result, obj) => {
			m_State = State.None;
			DLGTINFO.f_RFCharInfoCard?.Invoke();
		}, m_Mode == Mode.PVP);
	}
	//랭크업 알람
	public void SetRankUpAlarm() {
		if (m_Info == null) {
			RankUpFrameAlarm.SetActive(false);
		}
		else {
			bool canrankup = m_Info.IS_CanRankUP();
			RankUpFrameAlarm.SetActive(m_Mode == Mode.Management && canrankup);
		}
	}
	public void SetSortVal(SortingType _type = SortingType.CombatPower) {
		if (SortGroup.gameObject.activeSelf) {
			SortGroup.GetComponent<Item_PowerMark>().SetData(_type, m_Info, m_Mode == Mode.PVP);
		}
	}
	public void SetNewAlarm(bool _new) {
		m_NewAlarm.SetActive(_new);
	}
}