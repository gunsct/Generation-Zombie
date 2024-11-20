using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Stage_Mission : PopupBase
{
	public enum State
	{
		None,
		Mission,
		ClockGuide
	}
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image StageImg;
		public TextMeshProUGUI[] Goal;
		public TextMeshProUGUI FailCondition;
		public Image GuideImg;
		public Image[] SametimeImgs;
		public Transform Clock;
		public Transform CloneClock;
		public GameObject[] SametimeCards;
		public GameObject[] ContinuityCards;
		public GameObject Warning;
		[ReName("Mission Info", "Turn Cnt")]
		public GameObject[] Panels;
	}
	[SerializeField]
	SUI m_SUI;
	Coroutine m_Cor;
	IEnumerator m_Action;
	State m_State;

	private void Awake() {
		m_SUI.CloneClock.gameObject.SetActive(false); 
		for (int i = 0; i < 2; i++) {
			m_SUI.SametimeCards[i].SetActive(false);
			m_SUI.ContinuityCards[i].SetActive(false);
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		PlayEffSound(SND_IDX.SFX_0102);

		int stgidx = (int)aobjValue[0];
		TStageTable table = TDATA.GetStageTable(stgidx, STAGEINFO.m_PlayType == StagePlayType.Stage ? USERINFO.GetDifficulty() : 0);
		m_SUI.Warning.SetActive(table.m_Difficulty > 0);
		if (m_SUI.Warning.activeSelf) m_SUI.Warning.GetComponent<Animator>().SetTrigger(table.m_Difficulty > 1 ? "War2" : "War1");
		m_SUI.StageImg.sprite = TDATA.GetStageTable(stgidx, STAGEINFO.m_PlayType == StagePlayType.Stage ? USERINFO.GetDifficulty() : 0).GetImg();
		TGuideTable guide = TDATA.GetGuideTable(table.m_Clear[0].m_Type);
		TTrainingTable trainingdata = TDATA.GetTrainingTable(table.m_Clear[0].m_Value);
		switch (STAGEINFO.m_TStage.m_Mode) {
			case StageModeType.Training:
				m_SUI.Goal[0].text = m_SUI.Goal[1].text = string.Format(TDATA.GetString(232), table.m_Clear[0].m_Cnt, table.m_LimitTurn - table.m_Clear[0].m_Cnt);
				break;
			default:
				m_SUI.Goal[0].text = m_SUI.Goal[1].text = table.GetInfo();
				break;
		}
		for(int i = 0;i< STAGEINFO.m_TStage.m_Clear.Count; i++) {
			if(i == 0) m_SUI.GuideImg.sprite = STAGEINFO.m_TStage.m_Clear[i].GetIcon_Card();
			else {
				m_SUI.SametimeImgs[i - 1].sprite = STAGEINFO.m_TStage.m_Clear[i].GetIcon_Card();
			}
		}

		if (STAGE_USERINFO.IS_TurnUse()) {
			switch(STAGEINFO.m_TStage.m_Mode)
			{
			case StageModeType.Training:
				m_SUI.FailCondition.text = string.Format(TDATA.GetString(10069), trainingdata.m_Time);
				break;
			default:
				m_SUI.FailCondition.text = string.Format(TDATA.GetString(62), table.m_LimitTurn.ToString());
				break;
			}
		}
		else {
			m_SUI.FailCondition.text = TDATA.GetString(70);
		}
		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.StageStart_Loading) || 
			TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.StageStart_Loading) || 
			TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.StageStart_Loading)){
			m_Action = StartAniCheck();
			StartCoroutine(m_Action);
		}
	}
	private void Update( ) {
		if(m_SUI.CloneClock.gameObject.activeSelf) {
			m_SUI.CloneClock.position = m_SUI.Clock.position;
			m_SUI.CloneClock.eulerAngles = m_SUI.Clock.eulerAngles;
			m_SUI.CloneClock.localScale = m_SUI.Clock.localScale;
			m_SUI.CloneClock.GetComponent<CanvasGroup>().alpha = m_SUI.Clock.GetComponent<CanvasGroup>().alpha * m_SUI.Clock.parent.GetComponent<CanvasGroup>().alpha;
		}
	}
	public GameObject GetMissionInfoPanel()
	{
		return m_SUI.Panels[0];
	}
	public GameObject GetTurnInfoPanel()
	{
		return m_SUI.Panels[1];
	}

	IEnumerator StartAniCheck()
	{
		yield return new WaitForSeconds(169f / 60f);
		m_Action = null;
	}

	public override void Close(int Result = 0)
	{
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;

		if (POPUP.GetMainUI() != null && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			m_Action = IE_ClockGuideAppear();
			StartCoroutine(m_Action);
		}
		else {
			m_Action = IE_MissionClose();
			StartCoroutine(m_Action);
		}

		////난투나 스테이지 보스전이면 끄고 아니면 시계 가이드 연출
		//switch (m_State) {
		//	case State.None:
		//		m_State = State.Mission;
		//		//스테이지 메인 유아이가 있고 켜졌을때는 시계랑 목표 나오게하고 아니면 미션만
		//		if (POPUP.GetMainUI() != null && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
		//			m_Action = IE_ClockGuideAppear();
		//			StartCoroutine(m_Action);
		//		}
		//		else {
		//			m_Action = IE_MissionClose();
		//			StartCoroutine(m_Action);
		//		}
		//		break;
		//	case State.Mission:
		//		m_State = State.ClockGuide;
		//		m_Action = IE_ClockGuideDisAppear();
		//		StartCoroutine(m_Action);
		//		break;
		//}
	}
	IEnumerator IE_MissionClose() {
		if (m_Cor != null) StopCoroutine(m_Cor);
		m_SUI.Anim.SetTrigger("End");
		if (m_SUI.Warning.activeSelf) m_SUI.Warning.GetComponent<Animator>().SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close();
	}
	IEnumerator IE_ClockGuideAppear() {
		if (m_Cor != null) StopCoroutine(m_Cor);
		m_SUI.Anim.SetTrigger("End");
		if (m_SUI.Warning.activeSelf) m_SUI.Warning.GetComponent<Animator>().SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));
		yield return new WaitForSeconds(0.1f);

		PlayEffSound(SND_IDX.SFX_0103);
		//시계돌고
		m_SUI.CloneClock.GetComponent<CanvasGroup>().alpha = 0f;
		m_SUI.CloneClock.gameObject.SetActive(true);
		DLGTINFO.f_RFClockUI?.Invoke(STAGE_USERINFO.m_Turn, STAGE_USERINFO.m_Time, 1f);
		string anim = string.Empty;
		switch (STAGEINFO.m_TStage.m_ClearMethod) {
			case ClearMethodType.None: anim = "TurnGoal"; break;
			case ClearMethodType.Continuity: anim = "Relay"; break;
			case ClearMethodType.SameTime: anim = "Sametime"; break;
		}
		for(int i = 1;i< STAGEINFO.m_TStage.m_Clear.Count; i++) {
			m_SUI.SametimeCards[i - 1].SetActive(STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.SameTime);
			m_SUI.ContinuityCards[i - 1].SetActive(STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.Continuity);
		}
		m_SUI.Anim.SetTrigger(anim);
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));

		m_Action = IE_ClockGuideDisAppear();
		yield return m_Action;
	}
	IEnumerator IE_ClockGuideDisAppear() {
		//사라지는 애니하면서 원래 유아이들 위치로 보내버림 
		string anim = string.Empty;
		switch (STAGEINFO.m_TStage.m_ClearMethod) {
			case ClearMethodType.None: anim = "TurnEnd"; break;
			case ClearMethodType.Continuity: anim = "Relay_End"; break;
			case ClearMethodType.SameTime: anim = "Sametime_End"; break;
		}
		m_SUI.Anim.SetTrigger(anim);
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));
		//가상 시계랑 가이드 카드가 위치로 가면 원래 위치로 진짜 유아이 서서히 켜주고 위치는 그자리에
		base.Close();
	}
}
