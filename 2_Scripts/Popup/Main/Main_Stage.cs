using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class Main_Stage : PopupBase
{
	public enum AniName {
		In = 0,
		Out,
		Start,
		Ready,
		StageReward,
		ScrollIn,
		ScrollOut
	}

	[System.Serializable]
	public struct SkillInfo {
		public GameObject Active;
		public Item_Skill_Card Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Info;
		public Color[] NameCol;
	}
	[Serializable]
	public struct SurvivalStatInfo {
		public GameObject Obj;
		public Image Icon;
		public Image[] MatIcon;
		public TextMeshProUGUI[] MatName;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Info;
	}
	[Serializable]
	public struct SynergyAlarm {
		public Item_Synergy_Stage[] Alarms;
	}
	[System.Serializable]
	public struct SUI {
		public Transform GuideParent;
		public GameObject GuideFX;
		public GameObject GuidePrefab;
		public Item_Survival SrvStat;
		public Animator Ani;
		public Item_Stage_Make Making;
		public SkillInfo SkillInfo;
		public Item_SurvStatInfo SurvStatInfo;
		public SynergyAlarm SynergyAlarm;
		public GameObject AccelBtn;
		public Transform ModeParant;
		public GameObject ModeAlarmPrefab;
		public Transform DebuffParent;
		public GameObject DebuffAlarmPrefab;
		public Item_Debuff_Alarm DebuffCenterAlarm;
		public Item_StgDebuffCardAlarm DebuffCardAlarm;
		public Item_AlarmToolTip AlarmToolTip;
		public GameObject TimerObj;
		public Item_SpeechBubble Speech;
		public GameObject ClockObj;
		public GameObject HPObj;
		public GameObject TowerBGGrad;
		public GameObject GuideBtn;
		public Animator CriScreenFX;
		[Header("튜토리얼용")]
		public GameObject[] Panels;
		public GameObject[] Path;
		public CanvasGroup PathAlpha;
		public Item_APUI APGauge;
	}
	[SerializeField] SUI m_SUI;
	Item_Guide m_Guides;
	List<Item_StgModeAlarm> m_ModeAlarms = new List<Item_StgModeAlarm>();
	Dictionary<StatType, Item_StgDebuffAlarm> m_DebuffAlarms = new Dictionary<StatType, Item_StgDebuffAlarm>();
	bool[,] m_UsedDebuffCenterAlarm = new bool[(int)StatType.SurvEnd, 3];

	public Item_Stage[] m_PathTargets = new Item_Stage[3];
	public Vector3[] m_PathGap;
	bool m_IsViewPath;
	AniName m_AniState;
	Coroutine m_CorLoopSND;
	List<SND_IDX> m_LoopSND = new List<SND_IDX>();
	public void StopLoopSND() {
		if (m_CorLoopSND != null) {
			StopCoroutine(m_CorLoopSND);
		}
		SND.StopEff();
	}
	public GameObject GetClockObj { get { return m_SUI.ClockObj; } }
	public GameObject GetTimerObj { get { return m_SUI.TimerObj; } }
	public GameObject GetStatObj(StatType _type) { return m_SUI.SrvStat.StatObj(_type); }
	public GameObject GetModeAlarmObj(PlayType _type) { return m_ModeAlarms.Find((t) => t.m_Mode == _type)?.gameObject; }
	public GameObject GetHpObj { get { return m_SUI.HPObj; } }
	public GameObject GetMissionGuideObj { get { return m_Guides.gameObject; } }
	public GameObject GetGuideParentObj { get { return m_SUI.GuideParent.gameObject; } }
	public Item_Stage_Make GetMaking { get { return m_SUI.Making; } }
	public Item_APUI GetAPGauge { get { return m_SUI.APGauge; } }
	/// <summary> 0:HP, 1:AP</summary>
	public bool IS_Anim(AniName _name) {
		return m_SUI.Ani.GetCurrentAnimatorStateInfo(0).IsName(_name.ToString());
	}
	private void Awake() {
		m_SUI.PathAlpha.gameObject.SetActive(false);
	}
	private void Update() {
		if (m_IsViewPath) {
			for (int i = 0; i < 3; i++) {
				if (m_PathTargets[i] != null) {
					if(TUTO.IsTutoPlay() && TUTO.POPUP.GetTutoUI() != null && TUTO.POPUP.GetTutoUI().IS_Focus) m_SUI.Path[i].SetActive(false);
					else {
						m_SUI.Path[i].SetActive(true);
						m_SUI.Path[i].transform.position = Utile_Class.GetCanvasPosition(m_PathTargets[i].transform.position + m_PathGap[i]);// + m_PathGap[i] * 0.5f;
					}
				}
				else m_SUI.Path[i].SetActive(false);
			}
		}
	}
	public void SetPathLine(Item_Stage _left = null, Item_Stage _center = null, Item_Stage _right = null) {
		if (!m_IsViewPath) return;
		m_PathTargets[0] = _left;
		m_PathTargets[1] = _center;
		m_PathTargets[2] = _right;
		bool on = false;
		for (int i = 0; i < 3; i++) {
			if (m_PathTargets[i] != null) {
				on = true;
				//Vector3 pos = Utile_Class.GetCanvasPosition(m_PathTargets[i].transform.position - m_PathTargets[i].ImgSize * 0.5f) / Canvas_Controller.SCALE;
				//Vector3 temp = Utile_Class.GetCanvasPosition(m_PathTargets[i].transform.position + m_PathTargets[i].ImgSize * 0.5f) / Canvas_Controller.SCALE;
				m_PathGap[i] = new Vector3(0f, m_PathTargets[i].ImgSize.y * 0.5f, 0f);
				m_SUI.Path[i].transform.position = Utile_Class.GetCanvasPosition(m_PathTargets[i].transform.position + m_PathGap[i]);// + m_PathGap[i] * 0.5f;
				m_SUI.Path[i].SetActive(true);
			}
		}
		SetPathLineAlpha(on);
	}
	public void SetPathLineAlpha(bool _on) {
		if (!m_IsViewPath) return;
		if (_on) {
			m_SUI.PathAlpha.gameObject.SetActive(true);
		}
		iTween.StopByName(gameObject, "PathAlpha");
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SUI.PathAlpha.alpha, "to", _on ? 1f : 0f, "onupdate", "TW_PathAlpha", "oncomplete", "TW_PathAlphaEnd", "oncompleteparams", _on, "time", 0.5f, "name", "PathAlpha"));
	}
	void TW_PathAlphaEnd(bool _on) {
		if (!_on) {
			m_SUI.PathAlpha.gameObject.SetActive(false);
		}
	}
	void TW_PathAlpha(float _amount) {
		m_SUI.PathAlpha.alpha = _amount;
	}
	bool IS_NewGuide() {
		bool first = false;
		for (int i = 0; i < STAGEINFO.m_TStage.m_PlayType.Count; i++) {
			first = TDATA.GetStageFirstPlayType(STAGEINFO.m_TStage.m_PlayType[i].m_Type, STAGEINFO.m_TStage.m_DifficultyType, STAGEINFO.m_Idx);
			if (first) return true;
		}
		List<TStageGuideTable> newdatas = STAGEINFO.m_TStage.m_DifficultyType == StageDifficultyType.Normal ? TDATA.GetStageGuideTable(STAGEINFO.m_Idx) : null;
		return newdatas == null ? false : newdatas.Count > 0;
	}
	void Init(Action<int, GameObject> StartPopupEndCB) {
		m_SUI.TowerBGGrad.SetActive(STAGEINFO.m_StageModeType == StageModeType.Tower);
		m_SUI.GuideBtn.SetActive(STAGEINFO.m_StageModeType == StageModeType.Stage || STAGEINFO.m_StageModeType == StageModeType.Tower);
		m_SUI.GuideBtn.GetComponent<Animator>().SetTrigger(IS_NewGuide() ? "On" : "Off");
		if (m_CorLoopSND != null) {
			StopCoroutine(m_CorLoopSND);
		}
		m_CorLoopSND = StartCoroutine(IE_LoopSND());
		StartPlayAni(AniName.Ready);
		ShowSkillInfo(false);
		ShowSuvStatInfo(false);
		OffSynergyInfo();
		OffAlarmToolTip();
		OffAlarmDebuffCardToolTip();
		m_SUI.ModeParant.gameObject.SetActive(false);
		m_SUI.DebuffCenterAlarm.gameObject.SetActive(false);
		//for (int i = m_DebuffAlarms.Count - 1; i > -1; i--) Destroy(m_DebuffAlarms.ElementAt(i).Value.gameObject);
		//m_DebuffAlarms.Clear();
		for (int i = 0; i < m_UsedDebuffCenterAlarm.GetLength(0); i++) {
			for (int j = 0; j < m_UsedDebuffCenterAlarm.GetLength(1); j++) {
				m_UsedDebuffCenterAlarm[i, j] = false;
			}
		}
		TStageTable table = STAGEINFO.m_TStage;
		//사용 스탯 세팅
		for (int i = 0; i < 3; i++) {
			StatType type = StatType.Men + i;
			if (!STAGE_USERINFO.Is_UseStat(type)) m_SUI.SrvStat.StatOff(i);
			else DLGTINFO?.f_RfStatUI?.Invoke(type, STAGE_USERINFO.GetStat(type), STAGE_USERINFO.GetStat(type), STAGE_USERINFO.GetMaxStat(type));
		}
		m_SUI.SrvStat.SetTrans();
		//체력 세팅
		DLGTINFO?.f_RfHPUI?.Invoke(STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetMaxStat(StatType.HP));
		DLGTINFO?.f_RfHPLowUI?.Invoke(STAGE_USERINFO.GetStat(StatType.HP) < STAGE_USERINFO.GetMaxStat(StatType.HP) * 0.3f);
		//행동력 갱신
		DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], STAGE_USERINFO.m_AP[0], STAGE_USERINFO.m_AP[1]);
		m_SUI.Making.SetData();

		for (int i = 0; i < m_SUI.SynergyAlarm.Alarms.Length; i++) {
			if (i < STAGE_USERINFO.m_Synergys.Count) {
				m_SUI.SynergyAlarm.Alarms[i].SetData(STAGE_USERINFO.m_Synergys.ElementAt(i).Key);
				m_SUI.SynergyAlarm.Alarms[i].gameObject.SetActive(true);
			}
			else
				m_SUI.SynergyAlarm.Alarms[i].gameObject.SetActive(false);
		}

		StartCoroutine(IE_Init(StartPopupEndCB));
	}

	IEnumerator IE_Init(Action<int, GameObject> StartPopupEndCB) {
		Time.timeScale = BaseValue.STAGE_STEP1_TIMESCALE;
		//시계있는경우는 미션에서 더미가 액션하기때문에 여기서 미리 본체는 해둬야함
		DLGTINFO.f_RFClockUI?.Invoke(STAGE_USERINFO.m_Turn, STAGE_USERINFO.m_Time);
		GameObject obj = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Mission, STAGEINFO.m_TStage.m_StartReward == 0 ? StartPopupEndCB : null, STAGEINFO.m_Idx).gameObject;
		yield return new WaitUntil(() => obj == null);

		//if (TUTO.IsTuto(TutoKind.Stage_201, (int)TutoType_Stage_201.StageStart_Loading)) {
		//	obj = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_StartReward, StartPopupEndCB, 10018).gameObject;
		//	yield return new WaitUntil(() => obj == null);
		//}
		if (STAGEINFO.m_TStage.m_StartReward != 0)
		{
			obj = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_StartReward, StartPopupEndCB, STAGEINFO.m_TStage.m_StartReward).gameObject;//999999
			yield return new WaitUntil(() => obj == null);
			//StartPopupEndCB?.Invoke(0, null);
		}

		StartPlayAni(AniName.Start);
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => m_SUI.Ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 25f / 190f);

		yield return new WaitUntil(() => MAIN.ISLoading == false);
		m_Guides.gameObject.SetActive(true);
		m_Guides.GuideTransRefresh();

		//시작 대사, 보이스
		if (MAIN.IS_State(MainState.STAGE) && !TUTO.IsTutoPlay()) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.StartStage);

			Item_Stage_Char randchar = STAGE.m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
			if (randchar != null) {
				SND_IDX startvocidx = randchar.m_Info.m_TData.GetVoice(TCharacterTable.VoiceType.StageStart, randchar.m_TransverseAtkMode);
				PlayVoiceSnd(new List<SND_IDX>() { startvocidx });
			}
		}

		yield return new WaitWhile(() => POPUP.IS_PopupUI());

		bool timeatk = STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time;
#if STAGE_TEST
		m_SUI.AccelBtn.SetActive((MAIN.IS_State(MainState.STAGE) && !timeatk) || MAIN.IS_State(MainState.TOWER));//
#else
		m_SUI.AccelBtn.SetActive((MAIN.IS_State(MainState.STAGE) && !timeatk && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.UNLOCK_DOUBLE_SPEED) || MAIN.IS_State(MainState.TOWER));//
#endif
		AccToggleCheck();

		//STAGE_USERINFO.SetDebuff();
		//디버프 카드들 알람으로 빼내기
		for (int i = 0; i < STAGE_USERINFO.m_BuffValues.Count; i++) {
			var data = STAGE_USERINFO.m_BuffValues.ElementAt(i);
			if (STAGE_USERINFO.IS_BadCardBuff(data.Key) && STAGE_USERINFO.ISBuff(data.Key))// data.Value > 0f
				m_SUI.DebuffCardAlarm.AddData(data);
		}
		//if (TUTO.IsTuto(TutoKind.Stage_205, (int)TutoType_Stage_205.Delay_Start)) TUTO.Next();
		//if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Delay_Start)) TUTO.Next();
		//if (TUTO.IsTuto(TutoKind.Stage_201, (int)TutoType_Stage_201.Delay_Reward)) TUTO.Next();
	}
	public void AccToggleCheck() {
//#if STAGE_TEST
//		bool acc = PlayerPrefs.GetInt($"AccelBtn_{USERINFO.m_UID}") == 1;
//#else
//		bool acc = PlayerPrefs.GetInt($"AccelBtn_{USERINFO.m_UID}") == 1 && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx > BaseValue.UNLOCK_DOUBLE_SPEED;
//#endif
		int accstep = BaseValue.STAGE_ACC_STEP_SAVE;

		if (STAGEINFO.IsPlaySpeedUpMode()) {
			if (MAIN.IS_State(MainState.STAGE)) {
				bool timeatk = STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time;
				//if (timeatk) acc = true;
				if (timeatk) accstep = 1;
				else if (TUTO.IsTutoPlay()) accstep = 0;
				//STAGE.m_IS_GameAccel = acc = !timeatk && TUTO.IsTutoPlay() ? false : acc;
			}
			else if(MAIN.IS_State(MainState.TOWER)){
				if (TUTO.IsTutoPlay()) accstep = 0;
			}
			//if (MAIN.IS_State(MainState.TOWER)) TOWER.m_IS_GameAccel = acc = TUTO.IsTutoPlay() ? false : acc;
		}
		else accstep = 0;	//acc = false;

		m_SUI.AccelBtn.GetComponent<Animator>().SetTrigger(accstep.ToString());
		Time.timeScale = BaseValue.STAGE_STEP_TIMESCALE(accstep);// acc ? BaseValue.STAGE_STEP2_TIMESCALE : BaseValue.STAGE_STEP1_TIMESCALE;
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_IsViewPath = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.THREEWAY_TUTO && STAGEINFO.m_StageModeType == StageModeType.Stage;
		for(int i = 0;i< STAGEINFO.m_TStage.m_BGV.Count; i++) {
			m_LoopSND.Add(STAGEINFO.m_TStage.m_BGV[i]);
		}
		Init((Action<int, GameObject>)aobjValue[0]);
		TStageTable table = STAGEINFO.m_TStage;
		//클리어 미션
		SetGuide();
		//모드 알람
		SetModeAlarm();
		//타이머
		m_SUI.TimerObj.SetActive(STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time);
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time) DLGTINFO?.f_RFModeTimer?.Invoke(STAGEINFO.m_TStage.m_Fail.m_Value);
#if !STAGE_TEST
		m_SUI.Making.gameObject.SetActive((USERINFO.GetDifficulty() == 0 && STAGEINFO.m_Idx > 103) || USERINFO.GetDifficulty() > 0 || TUTO.IsEndTuto(TutoKind.Stage_103));
		m_SUI.ClockObj.SetActive(USERINFO.GetDifficulty() == 0 && STAGEINFO.m_Idx > 205);
		bool viewquest = STAGEINFO.IsViewGuideQuest();
		m_SUI.GuideParent.gameObject.SetActive(viewquest);
		m_SUI.GuideFX.SetActive(viewquest);
		m_SUI.HPObj.SetActive((USERINFO.GetDifficulty() == 0 && STAGEINFO.m_Idx > 101) || USERINFO.GetDifficulty() > 0 || TUTO.IsEndTuto(TutoKind.Stage_101));
		m_SUI.APGauge.gameObject.SetActive(STAGEINFO.m_TStage.m_AP > -1 && TUTO.IsEndTuto(TutoKind.Stage_101));
#else
		m_SUI.APGauge.gameObject.SetActive(STAGEINFO.m_TStage.m_AP > -1);
#endif
		if (TUTO.IsTutoPlay()) {
			if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.StageStart_Loading)) GetStatObj(StatType.Sat).SetActive(false);
			if (TUTO.IsTuto(TutoKind.Stage_601, (int)TutoType_Stage_601.StageStart_Loading)) GetStatObj(StatType.Hyg).SetActive(false);
			if (TUTO.IsTuto(TutoKind.Stage_801, (int)TutoType_Stage_801.StageStart_Loading)) GetStatObj(StatType.Men).SetActive(false);
		}
		
	}
	public void ClickTimeScaleToggle() {
#if STAGE_TEST
#else
		if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.UNLOCK_DOUBLE_SPEED) return;
#endif
		if (TUTO.IsTutoPlay() && !TUTO.IsEndTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.DL_1731)) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(262));
			return;
		}
		if(TUTO.IsTutoPlay() && TUTO.IsEndTuto(TutoKind.Stage_203)) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(262));
			return;
		}
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageMenu, 1)) return;

		int step = BaseValue.STAGE_ACC_STEP_NEXT();
		m_SUI.AccelBtn.GetComponent<Animator>().SetTrigger(step.ToString());
		Time.timeScale = BaseValue.STAGE_STEP_TIMESCALE(step);

		//bool acc = false;
		//if (MAIN.IS_State(MainState.STAGE)) acc = STAGE.m_IS_GameAccel = !STAGE.m_IS_GameAccel;
		//if (MAIN.IS_State(MainState.TOWER)) acc = TOWER.m_IS_GameAccel = !TOWER.m_IS_GameAccel;

		//PlayerPrefs.SetInt($"AccelBtn_{USERINFO.m_UID}", acc ? 1 : 0);
		//PlayerPrefs.Save();
		//m_SUI.AccelBtn.GetComponent<Animator>().SetTrigger(string.Format("x{0}", acc ? 2 : 1));
		//Time.timeScale = acc ? BaseValue.STAGE_STEP2_TIMESCALE : BaseValue.STAGE_STEP1_TIMESCALE;

		int stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;

		if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Focus_AccBtn)) TUTO.Next();
	}
	/// <summary> 가이드 추가, 클리어 타입별로 시작값 세팅 </summary>
	void SetGuide() {
		m_Guides = Utile_Class.Instantiate(m_SUI.GuidePrefab, m_SUI.GuideParent).GetComponent<Item_Guide>();
		m_Guides.transform.SetAsFirstSibling();
		m_Guides.SetData();
		m_Guides.gameObject.SetActive(false); 
		GuideCardLoop(true);
	}
	/// <summary>
	/// 클리어 타입으로 찾아 갱신
	/// </summary>
	/// <param name="_type">클리어타입</param>
	/// <param name="_val">현재 수치</param>
	public void RefreshGuide(StageClearType _type, int _pos) {
		m_Guides.SetRefresh(_type, _pos);
	}
	/// <summary> 클리어 타입으로 찾아 클리어 애니메이션 </summary>
	public void ClearGuide(StageClearType _type, int _pos) {
		m_Guides.SetClear(_type, _pos);
	}

	public Item_Guide GetGuideUI()
	{
		return m_Guides;
	}
	public void GuideCardLoop(bool _loop) {
		StartCoroutine(GuideCardLoopAction(_loop));
	}
	IEnumerator GuideCardLoopAction(bool _loop) {
		yield return new WaitWhile(() => !m_Guides.gameObject.activeSelf);

		m_Guides.SetCardLoop(_loop);
	}
	/// <summary> 스테이지 정보 </summary>
	public void Click_StageInfo() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageMenu, 3)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Info, null, STAGEINFO.m_TStage);
	}
	/// <summary> 스테이지 모드 알람 또는 해당 유아이들</summary>
	void SetModeAlarm() {
		STAGE?.m_ModeCnt.Clear();
		for (int i = m_ModeAlarms.Count - 1; i > -1; i--) {
			Destroy(m_ModeAlarms[i].gameObject);
		}
		//오직 모드만 체크
		List<TStageTable.StagePlayType> modes = STAGEINFO.m_TStage.m_PlayType;
		m_SUI.ModeParant.gameObject.SetActive(modes.Count > 0);
		for (int i = 0; i < modes.Count; i++) {
			switch (modes[i].m_Type) {
				case PlayType.FieldAirstrike:
				case PlayType.NoCool:
				case PlayType.StreetLight:
				case PlayType.FireSpread:
				case PlayType.CardShuffle:
				case PlayType.CardLock:
				case PlayType.EasyCardLock:
				case PlayType.CardRandomLock:
				case PlayType.Blind:
				case PlayType.RandomCharOut:
				case PlayType.HighCharOut:
				case PlayType.LowCharOut:
				case PlayType.BanActive:
					Item_StgModeAlarm alarm = Utile_Class.Instantiate(m_SUI.ModeAlarmPrefab, m_SUI.ModeParant).GetComponent<Item_StgModeAlarm>();
					alarm.SetData(modes[i]);
					m_ModeAlarms.Add(alarm);
					break;
			}
			STAGE?.m_ModeCnt.Add(modes[i].m_Type, 0);
		}
		//실패 조건에 있는데 유아이만 쓸 경우
		switch (STAGEINFO.m_TStage.m_Fail.m_Type) {
			case StageFailType.TurmoilCount:
				m_SUI.ModeParant.gameObject.SetActive(true);
				Item_StgModeAlarm alarm = Utile_Class.Instantiate(m_SUI.ModeAlarmPrefab, m_SUI.ModeParant).GetComponent<Item_StgModeAlarm>();
				alarm.SetData(PlayType.TurmoilCount, UTILE.LoadImg("UI/Icon/Icon_Gimmik_03", "png"), Mathf.RoundToInt(STAGEINFO.m_TStage.m_Fail.m_Cnt));
				m_ModeAlarms.Add(alarm);
				STAGE?.m_ModeCnt.Add(PlayType.TurmoilCount, 0);
				break;
		}
		//ap회복 0일때 
		if(STAGEINFO.m_TStage.m_APRecovery == 0 && STAGEINFO.m_TStage.m_AP > -1) {
			m_SUI.ModeParant.gameObject.SetActive(true);
			Item_StgModeAlarm alarm = Utile_Class.Instantiate(m_SUI.ModeAlarmPrefab, m_SUI.ModeParant).GetComponent<Item_StgModeAlarm>();
			alarm.SetData(PlayType.APRecvZero, UTILE.LoadImg("UI/Icon/Icon_Gimmik_09", "png"), 0);
			m_ModeAlarms.Add(alarm);
		}
	}
	public void RefreshModeAlarm(PlayType _type, int _add = 0, bool _init = false) {
		Item_StgModeAlarm alarm = m_ModeAlarms.Find((t) => t.m_Mode == _type);
		if (alarm != null) alarm.RefreshAlarm(_init ? alarm.GetRemain() : _add);
	}
	public Item_StgModeAlarm GetModeAlarm(PlayType _type) {
		return m_ModeAlarms.Find((t) => t.m_Mode == _type);
	}
	/// <summary> 일시정지 버튼 </summary>
	public void ClickPause() {
		if (StageMng.IsValid() && STAGE.IS_SelectAction()) return;
		else if (TowerMng.IsValid() && TOWER.IS_SelectAction()) return;
		if (STAGEINFO.m_Result != StageResult.None) return;

		if (TUTO.IsTutoPlay()) {
			if(!POPUP.IS_TutoUI()) POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(262));
			return;
		}
		//if (TUTO.TouchCheckLock(TutoTouchCheckType.StageMenu, 0)) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Pause);
	}

	public void StartPlayAni(AniName name) {
		switch(name)
		{
		case AniName.ScrollOut:
			if (m_AniState == AniName.Out) return;
			if (m_AniState == name) return;
			break;
		case AniName.ScrollIn:
			if (m_AniState == AniName.In) return;
			if (m_AniState == name) return;
			break;
		}
		m_AniState = name;
		m_SUI.Ani.SetTrigger(name.ToString());
	}
	/// <summary> 재료 획득시 연출 </summary>
	public void GetMatCard(StageMaterialType _type, int _cnt, Vector3 _spos = default(Vector3)) {
		m_SUI.Making.GetMat(_type, _cnt, _spos);
	}
	public void GetMakeUtileCard(StageMaterialType _type, int _cnt, Vector3 _spos = default(Vector3)) {
		m_SUI.Making.GetUtile(_type, _cnt, _spos);
	}
	public bool RandMatDiscard(int _prob, int _cnt) {
		return m_SUI.Making.RandDiscard(_prob, _cnt);
	}

	public Item_Stage_Make GetCraft() {
		return m_SUI.Making;
	}
	public Item_SpeechBubble GetSpeechBubble() {
		return m_SUI.Speech;
	}
	/// <summary> 남은 턴 알림 </summary>
	public void TurnSpeech() {
		if (STAGEINFO.m_Check.m_IsTurnEndClear) return;
		if (STAGE_USERINFO.m_Turn < 4) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.LimitTime);
		}
	}
	/// <summary> 현재 덱 시너지 보기 </summary>
	//public void ClickSynergyView() {
	//    POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Synergy_Applied);
	//}
	public bool IsShowSynergyInfo() {
		for (int i = 0; i < m_SUI.SynergyAlarm.Alarms.Length; i++) {
			if (m_SUI.SynergyAlarm.Alarms[i].IS_ShowInfo()) return true;
		}
		return false;
	}
	public void OffSynergyInfo() {
		for (int i = 0; i < m_SUI.SynergyAlarm.Alarms.Length; i++) {
			m_SUI.SynergyAlarm.Alarms[i].ViewInfo(false);
		}
	}
	public bool IsShowDebuffCardInfo() {
		return m_SUI.DebuffCardAlarm.IS_OnInfo();
	}
	public void OffAlarmDebuffCardToolTip() {
		m_SUI.DebuffCardAlarm.OffInfo();
	}
	public void RefreshDebuffCardCount(StageCardType _type, int _cnt = 1) {
		m_SUI.DebuffCardAlarm.RefreshCount(_type, _cnt);
	}
	public void ShowSkillInfo(bool active, int LV = 0, bool IsSet = false, TSkillTable skill = null) {
		m_SUI.SkillInfo.Active.SetActive(active);
		if (active) {
			m_SUI.SkillInfo.Icon.SetData(skill.m_Idx, LV, false, IsSet);
			m_SUI.SkillInfo.Name.text = skill.GetName();
			m_SUI.SkillInfo.Name.color = m_SUI.SkillInfo.NameCol[IsSet ? 0 : 1];
			m_SUI.SkillInfo.Info.text = skill.GetInfo(LV);
		}
	}
	public bool IsShowSkillInfo() {
		return m_SUI.SkillInfo.Active.activeSelf;
	}

	//생존스탯 터치시 정보창
	public void ShowSuvStatInfo(bool _active, StatType _type = StatType.None) {
		if(_active && TUTO.TouchCheckLock(TutoTouchCheckType.StageDebuffToolTip, (int)_type)) return;
		m_SUI.SurvStatInfo.gameObject.SetActive(_active);
		if (_active) m_SUI.SurvStatInfo.SetData(_type, STAGE_USERINFO.GetStat(_type), STAGE_USERINFO.GetMaxStat(_type));
	}
	public bool IsShowSuvStatInfo() {
		return m_SUI.SurvStatInfo.gameObject.activeSelf;
	}
	/// <summary> 디버프 알람 세팅 </summary>
	public void SetDebuffAlarm(StatType _stat, StageDebuff _debuff) {
		StartCoroutine(IE_SetDebuffAlarm(_stat, _debuff));
	}
	public IEnumerator IE_SetDebuffAlarm(StatType _stat, StageDebuff _debuff) {
		if (!m_DebuffAlarms.ContainsKey(_stat) && _debuff != null) {
			Item_StgDebuffAlarm alarm = Utile_Class.Instantiate(m_SUI.DebuffAlarmPrefab, m_SUI.DebuffParent).GetComponent<Item_StgDebuffAlarm>();
			m_DebuffAlarms.Add(_stat, alarm);

			switch (_stat) {
				case StatType.Men: STAGE_USERINFO.CharSpeech(DialogueConditionType.MenDebuffOn); break;
				case StatType.Hyg: STAGE_USERINFO.CharSpeech(DialogueConditionType.HygDebuffOn); break;
				case StatType.Sat: STAGE_USERINFO.CharSpeech(DialogueConditionType.SatDebuffOn); break;
			}

			TStatusDebuffTable table = TDATA.GetStatusDebuffTable(_debuff.m_Idx);
			//센터 알람
			if (!m_UsedDebuffCenterAlarm[(int)_stat, TDATA.GetStatusDebuffPos(table)]) {
				m_UsedDebuffCenterAlarm[(int)_stat, TDATA.GetStatusDebuffPos(table)] = true;
				m_SUI.DebuffCenterAlarm.gameObject.SetActive(true);
				m_SUI.DebuffCenterAlarm.SetData(table);
			}
			alarm.SetData(table);

			if (m_SUI.DebuffCenterAlarm.gameObject.activeSelf) {
				yield return new WaitWhile(() => m_SUI.DebuffCenterAlarm.IS_AlarmTiming());
				m_SUI.DebuffCenterAlarm.gameObject.SetActive(false);
			}
		}
		else if (m_DebuffAlarms.ContainsKey(_stat)) {
			if (_debuff == null) {//해당 스탯 디버프가 없음
				m_DebuffAlarms[_stat].Refresh(null, (res) => {
					m_DebuffAlarms.Remove(_stat);
					Destroy(res.gameObject);

					switch (_stat) {
						case StatType.Men: STAGE_USERINFO.CharSpeech(DialogueConditionType.MenDebuffOff); break;
						case StatType.Hyg: STAGE_USERINFO.CharSpeech(DialogueConditionType.HygDebuffOff); break;
						case StatType.Sat: STAGE_USERINFO.CharSpeech(DialogueConditionType.SatDebuffOff); break;
					}
				}, (step)=> {
					for(int i = 2; i > step;i--)
					m_UsedDebuffCenterAlarm[(int)_stat, i] = false;
				});
			}
			else {
				TStatusDebuffTable table = TDATA.GetStatusDebuffTable(_debuff.m_Idx);
				//센터 알람
				if (!m_UsedDebuffCenterAlarm[(int)_stat, TDATA.GetStatusDebuffPos(table)]) {
					m_UsedDebuffCenterAlarm[(int)_stat, TDATA.GetStatusDebuffPos(table)] = true;
					m_SUI.DebuffCenterAlarm.gameObject.SetActive(true);
					m_SUI.DebuffCenterAlarm.SetData(table);

					//yield return new WaitForEndOfFrame();
					//yield return new WaitWhile(() => m_SUI.DebuffCenterAlarm.IS_AlarmTiming(100f / 117f));
				}
				m_DebuffAlarms[_stat].gameObject.SetActive(true);
				m_DebuffAlarms[_stat].Refresh(table, null, (step) => {
					for (int i = 2; i > step; i--)
						m_UsedDebuffCenterAlarm[(int)_stat, i] = false;
				});

				if (m_SUI.DebuffCenterAlarm.gameObject.activeSelf) {
					yield return new WaitWhile(() => m_SUI.DebuffCenterAlarm.IS_AlarmTiming());
					m_SUI.DebuffCenterAlarm.gameObject.SetActive(false);
				}
			}
		}
	}
	public bool IsShowCardInfo() {
		if (!POPUP.IS_PopupUI()) return false; 
		return POPUP.GetPopup().m_Popup == PopupName.Stage_Info_Card;
	}
	public void ShowCardInfo(Item_Stage _card = null) {
		if (IsShowCardInfo()) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Info_Card, null, _card);
	}
	public void SetAlarmToolTip(string _name, string _desc, Vector3 _pos) {
		if(TUTO.TouchCheckLock(TutoTouchCheckType.StageModeToolTip)) return;
		//m_SUI.DebuffCenterAlarm.gameObject.SetActive(false);
		StartCoroutine(SetAlarmToolTipDelay(_name, _desc, _pos));
	}
	IEnumerator SetAlarmToolTipDelay(string _name, string _desc, Vector3 _pos) {
		yield return new WaitForEndOfFrame();
		m_SUI.AlarmToolTip.SetData(_name, _desc, _pos);
		m_SUI.AlarmToolTip.gameObject.SetActive(true);
	}
	public void OffAlarmToolTip() {
		m_SUI.AlarmToolTip.gameObject.SetActive(false);
	}
	public bool IsShowAlarmToolTip() {
		return m_SUI.AlarmToolTip.gameObject.activeSelf;
	}

	IEnumerator IE_LoopSND() {
		if (m_LoopSND.Count < 1) yield break;
		yield return new WaitForSeconds(UTILE.Get_Random(8f, 12f));

		SND_IDX idx = m_LoopSND[UTILE.Get_Random(0, m_LoopSND.Count)];
		PlayEffSound(idx);

		yield return new WaitWhile(() => SND.IS_PlayFXSnd(new List<SND_IDX>() { idx }));

		m_CorLoopSND = StartCoroutine(IE_LoopSND());
	}

	public void ActiveSynergy(JobType _type) {
		for(int i = 0; i< STAGE_USERINFO.m_Synergys.Count; i++) {
			if (m_SUI.SynergyAlarm.Alarms[i].m_SynergyType == _type) m_SUI.SynergyAlarm.Alarms[i].SetAnim();
		}
	}
	public void Click_StageGuide() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Stage_Info)) return;
		float pretimescale = Time.timeScale;
		if (STAGEINFO.m_StageModeType == StageModeType.Stage) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardList, (res, obj) => { Time.timeScale = pretimescale; });
			m_SUI.GuideBtn.GetComponent<Animator>().SetTrigger("Off");
			Time.timeScale = 0f;//팝업 켤떄 타임스케일 1이라 켜진 뒤에 0
		}
		else if (STAGEINFO.m_StageModeType == StageModeType.Tower) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Guide_Tower, (res, obj) => { Time.timeScale = pretimescale; });
			Time.timeScale = 0f;//팝업 켤떄 타임스케일 1이라 켜진 뒤에 0
		}
	}
	public void SetCriScreenFX() {
		m_SUI.CriScreenFX.SetTrigger("Start");
	}
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 튜토리얼용  UI 위치
	public GameObject GetMakingPanel() {
		return m_SUI.Making.gameObject;
	}

	public Item_Stage_MakeCard GetMakingItem(StageMaterialType type) {
		return m_SUI.Making.GetItem(type);
	}

	public GameObject GetInfoPanel() {
		return m_SUI.Making.GetMakingInfoPanel();
	}

	public GameObject GetAPPanel() {
		return m_SUI.Panels[0];
	}
	public GameObject GetAccbtn() {
		return m_SUI.AccelBtn;
	}
	///////////////////////
	///튜토용 UI 조작
	public void TutoSrvStatOn(GameObject _obj, Action _cb, float _delay) {
		StartCoroutine(IE_TutoSrvStatOn(_obj, _cb, _delay));
	}
	IEnumerator IE_TutoSrvStatOn(GameObject _obj, Action _cb, float _delay) {
		_obj.SetActive(true);
		yield return new WaitForSeconds(_delay);
		_cb?.Invoke();
	}

	[ContextMenu("MATTESTPowder")]
	void MATTESTPowder() {
		GetMatCard(StageMaterialType.Powder, 3);
	}
	[ContextMenu("MATTESTAlcohol")]
	void MATTESTAlcohol() {
		GetMatCard(StageMaterialType.Alcohol, 3);
	}
	[ContextMenu("MATTESTHerb")]
	void MATTESTHerb() {
		GetMatCard(StageMaterialType.Herb, 3);
	}
	[ContextMenu("MATTESTMedicine")]
	void MATTESTMedicine() {
		GetMatCard(StageMaterialType.Medicine, 3);
	}
	[ContextMenu("MATTESTGasoline")]
	void MATTESTGasoline() {
		GetMatCard(StageMaterialType.Gasoline, 3);
	}
	[ContextMenu("MATTESTFood")]
	void MATTESTFood() {
		GetMatCard(StageMaterialType.Food, 3);
	}
	[ContextMenu("MATTESTGunPowder")]
	void MATTESTGunPowder() {
		GetMatCard(StageMaterialType.GunPowder, 3);
	}
	[ContextMenu("MATTESTBattery")]
	void MATTESTBattery() {
		GetMatCard(StageMaterialType.Battery, 3);
	}
	[ContextMenu("MATTESTBullet")]
	void MATTESTBullet() {
		GetMatCard(StageMaterialType.Bullet, 3);
	}
	[ContextMenu("MenPTEST")]
	public void MenPTest() {
		STAGE_USERINFO.AddStat(StatType.Men, 10);
	}
	[ContextMenu("MenNTEST")]
	public void MenNTest() {
		STAGE_USERINFO.AddStat(StatType.Men, -10);
	}
	[ContextMenu("HygPTEST")]
	public void HygPTest() {
		STAGE_USERINFO.AddStat(StatType.Hyg, 10);
	}
	[ContextMenu("HygNTEST")]
	public void HygNTest() {
		STAGE_USERINFO.AddStat(StatType.Hyg, -10);
	}
	[ContextMenu("SatPTEST")]
	public void SatPTest() {
		STAGE_USERINFO.AddStat(StatType.Sat, 10);
	}
	[ContextMenu("SatNTEST")]
	public void SatNTest() {
		STAGE_USERINFO.AddStat(StatType.Sat, -10);
	}
	[ContextMenu("MenP5TEST")]
	public void MenP5Test() {
		STAGE_USERINFO.AddStat(StatType.Men, 50);
	}
	[ContextMenu("MenN5TEST")]
	public void MenN5Test() {
		STAGE_USERINFO.AddStat(StatType.Men, -50);
	}
	[ContextMenu("HygP5TEST")]
	public void HygP5Test() {
		STAGE_USERINFO.AddStat(StatType.Hyg, 50);
	}
	[ContextMenu("HygN5TEST")]
	public void HygN5Test() {
		STAGE_USERINFO.AddStat(StatType.Hyg, -50);
	}
	[ContextMenu("SatP5TEST")]
	public void SatP5Test() {
		STAGE_USERINFO.AddStat(StatType.Sat, 50);
	}
	[ContextMenu("SatN5TEST")]
	public void SatN5Test() {
		STAGE_USERINFO.AddStat(StatType.Sat, -50);
	}
}
