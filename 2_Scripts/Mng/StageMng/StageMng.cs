using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static LS_Web;

public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Instance
	private static StageMng m_Instance = null;
	public static StageMng Instance
	{
		get
		{
			return m_Instance;
		}
	}

	public static bool IsValid()
	{
		return m_Instance != null;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Value
#pragma warning disable 0649
	[System.Serializable]
	struct SStage
	{
		public Transform StageObjPanel;
		public GameObject ScalePanel;
		public SpriteRenderer BG;
		public GameObject Prefab;
		public GameObject ActionPanel;
		public Transform[] Panel;
	}
	[System.Serializable]
	struct SDamage
	{
		public GameObject Prefab;
		public Transform Panel;
	}
	[SerializeField] SDamage m_Damage;
	[SerializeField] Transform m_LowEffPanel;
	[SerializeField] Transform m_DarkMaskPanel;
	[SerializeField] Transform[] m_BGEffPanel;
	[SerializeField] SStage m_Stage;
	[SerializeField] Item_Stage_Area m_Area;
	IEnumerator m_StageAction;
	int m_SelectLine = 0;
	int m_SelectPos = 1;
	int m_PreSelectPos = 1;
	int m_ActionMovePos = 0;
	float m_MoveAddX = 0, m_MoveAddY = 0;
	public Item_Stage m_SelectStage { get { return m_ViewCard[m_SelectLine][m_SelectPos]; } }

	public Item_Stage[][] m_ViewCard = new Item_Stage[BaseValue.STAGE_LINE][];
	/// <summary> 가상 이동 배치를 바꾸기위한 복사본 </summary>
	public Item_Stage[][] m_VirtualCopyCard = new Item_Stage[BaseValue.STAGE_LINE][];
	/// <summary> 유틸 카드 중앙에 복사된것</summary>
	Item_Stage m_SelectVirture;

	public StageCheck m_Check { get { return STAGEINFO.m_Check; } }

	public int AI_MAXLINE { get { return m_CardLastLine == -1 || m_CardLastLine > 5 ? 5 : m_CardLastLine; } }//보이는 라인 5번쨰 줄(0~4)

	public StageUser m_User { get { return STAGEINFO.m_User; } }
	public Item_Stage_Char[] m_Chars = new Item_Stage_Char[5];//3-1-0-2-4 순서로
	public Item_Stage_Char m_CenterChar;
	Item_Stage_Char m_UseSkillChar = null;
	bool m_SkillZoom = false;
	bool m_IS_Jumping = false;
	bool m_IS_KillFirstLine = false;//첫 라인의 적을 죽인 경우
	public bool m_IS_GameAccel = false;//배속기능 사용 여부
	public float m_Timer;//타임어택용
	public bool IS_PassTimer;//시간 체크 가능 시점 체크
	public int[] m_RebornSrv = new int[4];//생존스탯, 체력 부활 카운트
	public int m_DarkPatrolCnt = 0;
	public int m_ContinueCnt = 0;
	int m_OneTurnKillCnt = 0;
	//몇초 이상 대기상태일때 가이드용
	float m_IdleTimer = 0f;				
	IEnumerator m_IdleTouchGuideCor;
	List<Item_Stage> m_IdleCard = new List<Item_Stage>();
	List<Item_Stage_Char> m_IdleChar = new List<Item_Stage_Char>();

	public Main_Stage m_MainUI;
	//포지션 바꾸면 애니메이션도 바꿔야함
	[SerializeField] Vector3[] m_CharsPos = { new Vector3(-2.2f, -2.28f, 0), new Vector3(-1.1f, -2.28f, 0), new Vector3(0f, -2.28f, 0), new Vector3(1.1f, -2.28f, 0), new Vector3(2.2f, -2.28f, 0) };
	[SerializeField] Vector3[] m_3CharsPos = { new Vector3(-1.1f, -2.28f, 0), new Vector3(0f, -2.28f, 0), new Vector3(1.1f, -2.28f, 0), new Vector3(-2.2f, -2.28f, 0), new Vector3(2.2f, -2.28f, 0) };
	[SerializeField] Vector3 m_CharsPosScale = new Vector3(0.85f, 0.85f, 1f);//{ Vector3.one * 0.9f, Vector3.one, Vector3.one * 0.9f };

	/// <summary> 스테이지 활성 패널 위치들 0 : 초기화, 1 : 카드 사용, 2 : 캐릭터 스킬 사용, 3 : 카드제작 사용 4 : 캐릭터 쿨타임 카드용 </summary>
	[SerializeField] Vector3[] m_ActivePanelPos = { new Vector3(0f, 0f, -5f), new Vector3(0f, -1.3f, -4f), new Vector3(0f, 0f, -4f), new Vector3(0f, -0.4f, -4f), new Vector3(0f, 0.8f, -4f) };
	/// <summary> 이펙트 풀 </summary>
	public class FX
	{
		public GameObject Obj;
		public Vector3 Scale;
		public Vector3 Rot;
	}
	Dictionary<string, List<FX>> m_FXPool = new Dictionary<string, List<FX>>();

	const int RIPEMEAT_CARDIDX = 16057;
	const int BURNMEAT_CARDIDX = 16058;

#pragma warning restore 0649
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Process
	// Start is called before the first frame update
	void Awake()
	{
#if NOT_USE_NET && STAGE_TEST
		if (MAIN.IS_BackState(MainState.START))
		{
			Debug.LogError("StageTest 씬을 통해서 실행할 것!!!!!!");
			MAIN.Exit();
			return;
		}
#elif UNITY_EDITOR
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(-1);
			MAIN.ReStart();
			return;
		}
#endif
		m_Instance = this;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private void Start()
	{
		m_Stage.BG.sprite = STAGEINFO.m_BG;
		SetBGEff();

		m_StageAction = StartAction();
		StartCoroutine(m_StageAction);
	}

	private void Update()
	{
		STAGEINFO.SetRunTime(Time.deltaTime);
		//아무것도 안하고 있으면 4초 후 첫줄 강조
		IdleGoalTouchGuide();
		SelectCheck();
		CameraMoveUpdate();
		Transform tfPanel = m_Stage.Panel[1];
		// 프로세스마다 호출해서 좌표 맞춰주기
		for (int i = 0; i < m_Lights.Count; i++) m_Lights[i].Update(tfPanel.localPosition.x + m_MoveAddX, tfPanel.localPosition.y + m_MoveAddY);
		for (int i = 0; i < m_AIStopInfos.Count; i++) m_AIStopInfos[i].Update(tfPanel.localPosition.x + m_MoveAddX, tfPanel.localPosition.y + m_MoveAddY);
		for (int i = 0; i < m_BurnInfos.Count; i++) m_BurnInfos[i].Update(tfPanel.localPosition.x + m_MoveAddX, tfPanel.localPosition.y + m_MoveAddY);
		for (int i = 0; i < m_AiBlockRangeAtkInfos.Count; i++) m_AiBlockRangeAtkInfos[i].Update(tfPanel.localPosition.x + m_MoveAddX, tfPanel.localPosition.y + m_MoveAddY);
		for (int i = 0; i < m_AiBlockAtkInfos.Count; i++) m_AiBlockAtkInfos[i].Update(tfPanel.localPosition.x + m_MoveAddX, tfPanel.localPosition.y + m_MoveAddY);
		for (int i = 0; i < m_StreetLightInfos.Count; i++) m_StreetLightInfos[i].Update(tfPanel.localPosition.x + m_MoveAddX, tfPanel.localPosition.y + m_MoveAddY);
		//타임어택
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time && STAGEINFO.m_Result == StageResult.None && IS_PassTimer && !POPUP.IS_PopupUI()) {
			m_Timer += Time.deltaTime / Time.timeScale;
			float time = STAGEINFO.m_TStage.m_Fail.m_Value;
			if (time > 0) {
				//시너지
				float? synergy2 = STAGE.m_User.GetSynergeValue(JobType.Scientist, 1);
				if (synergy2 != null) {
					time = time + (float)synergy2;
					if (STAGE_USERINFO.m_SynergyUseCnt[JobType.Scientist] == 0) STAGE_USERINFO.ActivateSynergy(JobType.Scientist);

					Utile_Class.DebugLog_Value("Scientist 시너지 발동 " + "변화 전 -> 후 : 전 :" + STAGEINFO.m_TStage.m_Fail.m_Value.ToString() + " 후 : " + time.ToString());
					//STAGE.m_User.m_SynergyUseCnt[JobType.Scientist]++;
				}

				//TimePlus 버프카드 적용
				time = time * (1f + STAGE_USERINFO.GetBuffValue(StageCardType.TimePlus));

				DLGTINFO?.f_RFModeTimer?.Invoke(Math.Max(0, time - m_Timer));
				//m_MainUI.RefreshTimer(Math.Max(0, time - m_Timer));
				int presec = Mathf.FloorToInt(time - (m_Timer - Time.deltaTime));
				int sec = Mathf.FloorToInt(time - m_Timer);
				if (presec != sec && sec <= 10) PlayEffSound(SND_IDX.SFX_0495);
				if (STAGEINFO.m_Check.IsFail() == StageFailKind.Time) StageFail(StageFailKind.Time);
			}
		}
	}

	public void Init()
	{
		if (m_ActionCor != null) {
			StopCoroutine(m_ActionCor);
			m_ActionCor = null;
		}

		m_Stage.ScalePanel.GetComponent<RenderAlpha_Controller>().Alpha = 1f;
		// 라인 생성의 갭을 맞추기위해 선택 카드 변경
		m_ActionMovePos = m_SelectPos = 1;
		m_CardLastLine = -1;
		SetMoveAddX(0); 
		SetMoveAddY(0);
		LoadStage();
		CamDataInit();
		SetBGFXSort(true);

		TStageTable tdata = STAGEINFO.m_TStage;
		int AddTurn = 0;
		TimeInit(tdata.m_LimitTurn + AddTurn, tdata.m_StartTime);

		DeckInfo PlayDeck = USERINFO.m_PlayDeck;
		int deckcnt = PlayDeck.GetDeckCharCnt();
		for (int i = 0; i < 5; i++) {
			if (i < deckcnt) {
				CharInfo cinfo = USERINFO.GetChar(PlayDeck.m_Char[i]);
				m_Chars[i].SetData(i, cinfo, 0);
			}
			else m_Chars[i].SetData(i, null);
		}
		//캐릭터 밴하는 모드들 체크
		m_BanChars.Clear();
		Mode_BanChars();
		//3인세팅일때 중앙 캐릭터 지정
		m_CenterChar = deckcnt >= 3 ? ( deckcnt == 3 ? m_Chars[1] : m_Chars[2]) : m_Chars[0];
		//타임어택
		m_Timer = 0f;
		IS_PassTimer = false;
		//부활 카운트 
		for (int i = 0; i < m_RebornSrv.Length; i++) m_RebornSrv[i] = 0;
		m_DarkPatrolCnt = 0;
		//재도전시 이어하기 카운팅 초기화
		m_ContinueCnt = 0;
		//턴당 킬 카운트 초기화
		m_OneTurnKillCnt = 0;
		InitTouchGuide();
		PlayStageBGSound();
	}

	/// <summary> 스테이지 들어오기 전에 얻은 버프가 있으면 추가 </summary>
	void SetDialogReward() {
		List<UserInfo.StageSelect> selects = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Selects;
		List<RES_REWARD_ITEM> dlcard = new List<RES_REWARD_ITEM>();
		for(int i = 0;i< selects.Count; i++) {
			dlcard.AddRange(selects[i].Rewards);
		}
		if (dlcard == null || dlcard.Count < 1) return;
		for (int i = 0; i < dlcard.Count; i++) {
			TStageCardTable table = TDATA.GetStageCardTable(dlcard[i].Idx);
			if (table.IS_BuffCard()) { //버프류
				SetBuff(EStageBuffKind.Stage, dlcard[i].Idx);//버프 적용
				if (table.m_Type == StageCardType.LevelUp) STAGE_USERINFO.StatReset();
			}
			else {
				switch (table.m_Type) {
					//즉시 회복류
					case StageCardType.RecoveryAP:
						StartCoroutine(SelectAction_RecoveryAP(Mathf.RoundToInt(table.m_Value1)));
						break;
					case StageCardType.LimitTurnUp:
						StartCoroutine(SelectAction_LimitTurnUp(Mathf.RoundToInt(table.m_Value1)));
						break;
					case StageCardType.AddRerollCount:
						StartCoroutine(SelectAction_AddRerollCount(table.m_Value1));
						break;
						//재료 조합의 재료 및 유틸카드
					case StageCardType.Material: AddMaterial((StageMaterialType)Mathf.RoundToInt(table.m_Value1), dlcard[i].Cnt); break;
					case StageCardType.Sniping: m_MainUI?.GetMakeUtileCard(StageMaterialType.Sniping, dlcard[i].Cnt); break;
					case StageCardType.Shotgun: m_MainUI?.GetMakeUtileCard(StageMaterialType.ShotGun, dlcard[i].Cnt); break;
					case StageCardType.MachineGun: m_MainUI?.GetMakeUtileCard(StageMaterialType.GatlingGun, dlcard[i].Cnt); break;
					case StageCardType.AirStrike: m_MainUI?.GetMakeUtileCard(StageMaterialType.AirStrike, dlcard[i].Cnt); break;
					case StageCardType.ShockBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.ShockBomb, dlcard[i].Cnt); break;
					case StageCardType.Dynamite: m_MainUI?.GetMakeUtileCard(StageMaterialType.Dynamite, dlcard[i].Cnt); break;
					case StageCardType.Grenade: m_MainUI?.GetMakeUtileCard(StageMaterialType.Grenade, dlcard[i].Cnt); break;
					case StageCardType.LightStick: m_MainUI?.GetMakeUtileCard(StageMaterialType.LightStick, dlcard[i].Cnt); break;
					case StageCardType.FlashLight: m_MainUI?.GetMakeUtileCard(StageMaterialType.FlashLight, dlcard[i].Cnt); break;
					case StageCardType.StarShell: m_MainUI?.GetMakeUtileCard(StageMaterialType.Flare, dlcard[i].Cnt); break;
					case StageCardType.ThrowExtin: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireSpray, dlcard[i].Cnt); break;
					case StageCardType.PowderExtin: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireExtinguisher, dlcard[i].Cnt); break;
					case StageCardType.PowderBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.PowderBomb, dlcard[i].Cnt); break;
					case StageCardType.FireBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireBomb, dlcard[i].Cnt); break;
					case StageCardType.FireGun: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireGun, dlcard[i].Cnt); break;
					case StageCardType.NapalmBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.NapalmBomb, dlcard[i].Cnt); break;
					case StageCardType.RecoveryHp:
						if (TDATA.GetStageMakingData(dlcard[i].Idx) != null) {
							if (table.m_Value1 == 0.1f) m_MainUI?.GetMakeUtileCard(StageMaterialType.MedBottle, dlcard[i].Cnt);
							else if (table.m_Value1 == 0.25f) m_MainUI?.GetMakeUtileCard(StageMaterialType.FirstAidKit, dlcard[i].Cnt);
							else if (table.m_Value1 == 0.5f) m_MainUI?.GetMakeUtileCard(StageMaterialType.CureKit, dlcard[i].Cnt);
						}
						else AddStat(StatType.HP, Mathf.RoundToInt(table.m_Value1), false, false); break;
					case StageCardType.RecoveryMen:
						if (TDATA.GetStageMakingData(dlcard[i].Idx) != null) {
							if (table.m_Value1 == 20) m_MainUI?.GetMakeUtileCard(StageMaterialType.Candle, dlcard[i].Cnt);
							else if (table.m_Value1 == 50) m_MainUI?.GetMakeUtileCard(StageMaterialType.Perfume, dlcard[i].Cnt);
							else if (table.m_Value1 == 120) m_MainUI?.GetMakeUtileCard(StageMaterialType.Drug, dlcard[i].Cnt);
							break;
						}
						else AddStat(StatType.Men, Mathf.RoundToInt(table.m_Value1), false, false); break;
					case StageCardType.RecoveryHyg:
						if (TDATA.GetStageMakingData(dlcard[i].Idx) != null) {
							if (table.m_Value1 == 20) m_MainUI?.GetMakeUtileCard(StageMaterialType.Disinfectant, dlcard[i].Cnt);
							else if (table.m_Value1 == 50) m_MainUI?.GetMakeUtileCard(StageMaterialType.Soap, dlcard[i].Cnt);
							else if (table.m_Value1 == 120) m_MainUI?.GetMakeUtileCard(StageMaterialType.Shampoo, dlcard[i].Cnt);
							break;
						}
						else AddStat(StatType.Hyg, Mathf.RoundToInt(table.m_Value1), false, false); break;
					case StageCardType.RecoverySat:
						if (TDATA.GetStageMakingData(dlcard[i].Idx) != null) {
							if (table.m_Value1 == 20) m_MainUI?.GetMakeUtileCard(StageMaterialType.Bread, dlcard[i].Cnt);
							else if (table.m_Value1 == 50) m_MainUI?.GetMakeUtileCard(StageMaterialType.Hamburger, dlcard[i].Cnt);
							else if (table.m_Value1 == 120) m_MainUI?.GetMakeUtileCard(StageMaterialType.Steak, dlcard[i].Cnt);
							break;
						}
						else AddStat(StatType.Sat, Mathf.RoundToInt(table.m_Value1), false, false); break;
					case StageCardType.PerRecoveryMen:
						AddStat(StatType.Men, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * table.m_Value1), false, false); break;
					case StageCardType.PerRecoveryHyg:
						AddStat(StatType.Hyg, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * table.m_Value1), false, false); break;
					case StageCardType.PerRecoverySat:
						AddStat(StatType.Sat, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * table.m_Value1), false, false); break;
				}
			}
		}
	}
	public Item_Stage_Char GetChar(int idx)
	{
		for(int i = m_Chars.Length - 1; i > -1; i--)
		{
			if (m_Chars[i] == null) continue;
			if (m_Chars[i].m_Info == null) continue;
			if (m_Chars[i].m_Info.m_Idx == idx) return m_Chars[i];
		}
		return null;
	}

	void SetMoveAddX(float _value)
	{
		m_MoveAddX = _value;
	}
	void SetMoveAddY(float _value)
	{
		m_MoveAddY = _value;
	}

	public void Restart() {
		// 스테이지 진입만큼 소모한후 완료되면 실행할것
		StopAllCoroutines();
		m_CorSkill = null;
		m_SkillChar = null;
		ShowArea(false);
		AllOffEff();
		//기존에 있던 카드들 전부 삭제
		List<Item_Stage> allcard = new List<Item_Stage>();

		for (int i = 0; i < m_ViewCard.Length; i++) {
			for (int j = 0; j < m_ViewCard[i].Length; j++) {
				if (m_ViewCard[i][j] != null) {
					allcard.Add(m_ViewCard[i][j]);
					m_ViewCard[i][j] = null;
				}
			}
		}
		for (int i = m_Stage.Panel[0].childCount - 1; i > -1; i--) {
			Item_Stage card = m_Stage.Panel[0].GetChild(i).GetComponent<Item_Stage>();
			if (!allcard.Contains(card)) DestroyImmediate(card.gameObject);
		}
		for (int i = m_Stage.Panel[1].childCount - 1; i > -1; i--) {
			Item_Stage card = m_Stage.Panel[1].GetChild(i).GetComponent<Item_Stage>();
			if (!allcard.Contains(card)) DestroyImmediate(card.gameObject);
		}
		for (int i = allcard.Count - 1; i > -1; i--) {
			DestroyImmediate(allcard[i].gameObject);
		}
		if (m_SelectVirture != null) DestroyImmediate(m_SelectVirture.gameObject);


		m_IS_KillFirstLine = false;
		m_IS_Jumping = false;

		m_Stage.Panel[1].transform.localPosition = Vector3.zero;
		STAGEINFO.PlayInit();
		RemoveLightAll();
		RemoveAIStopAll();
		RemoveAiBlockRangeAtkAll();
		RemoveBurnAll();
		RemoveStreetLightAll();
		m_StageAction = StartAction();
		StartCoroutine(m_StageAction);
	}

	public bool IS_SelectAction()
	{
		return m_StageAction != null;
	}

	public bool IS_SelectAction_Pause()
	{
		if (!MAIN.IS_State(MainState.STAGE)) return true;
		if (POPUP.IS_PopupUI()) return true;
		if (POPUP.IS_MsgUI()) return true;
		return false;
	}

	void SelectCheck()
	{
		//아이템 사용시 카드 눌러서 사용
		if (MAIN.IS_State(MainState.STAGE) && !POPUP.IS_MsgUI() && POPUP.GetPopup()?.m_Popup == PopupName.Stage_CardUse) {
			if (TouchCheck()) {
				m_HoldTimer = 0f;
				Ray ray = m_MyCam.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hit = Physics.RaycastAll(ray, m_MyCam.farClipPlane);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					Item_Stage card = hitobj.GetComponent<Item_Stage>();
					Item_Stage_Char user = hitobj.GetComponent<Item_Stage_Char>();
					Stage_CardUse popup = POPUP.GetPopup().GetComponent<Stage_CardUse>();
					if (m_CorSkill != null && user != null && m_SkillChar == user || card != null && card == m_SelectVirture) {
						if(popup.Is_OkBtnActive) 
							popup.Click_OkCancle(0);
					}
				}
			}
		}
		if (IS_SelectAction_Pause() || IS_SelectAction()) {
			if(POPUP.GetPopup()?.m_Popup == PopupName.Stage_Pause) m_TouchState = ETouchState.NONE;
			m_HoldTimer = 0f;
			return;
		}
		if (AutoCamPosInit == true && m_TouchState == ETouchState.PRESS) {
			if (!m_MainUI.IsShowSkillInfo() && !m_MainUI.IsShowSynergyInfo() && !m_MainUI.IsShowAlarmToolTip() && !m_MainUI.IsShowDebuffCardInfo() && !m_MainUI.IsShowCardInfo()) {
				Ray ray = m_MyCam.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hit = Physics.RaycastAll(ray, m_MyCam.farClipPlane);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					Item_Stage card = hitobj.GetComponent<Item_Stage>();
					if (card != null) {
						if (TUTO.TouchCheckLock(TutoTouchCheckType.StageCard, card)) continue;
						if (POPUP.IS_PopupUI()) break;
						//정보
						m_HoldTimer += Time.deltaTime;
						if (m_HoldTimer > 0.9f) {
							m_HoldTimer = 0f;
							m_MainUI.ShowCardInfo(card);
							m_TouchState = ETouchState.NONE;
							return;
						}
					}
				}
			}
		}
		else m_HoldTimer = 0f;
		if(m_TouchState == ETouchState.MOVE) {
			if (m_MainUI.IsShowSkillInfo() || m_MainUI.IsShowSynergyInfo() || m_MainUI.IsShowAlarmToolTip() || m_MainUI.IsShowDebuffCardInfo()) {
				m_MainUI.ShowSkillInfo(false);
				m_MainUI.OffSynergyInfo();
				m_MainUI.OffAlarmToolTip();
				m_MainUI.OffAlarmDebuffCardToolTip();
				m_MainUI.ShowSuvStatInfo(false);
			}
		}
		if (TouchCheck()) {
			m_HoldTimer = 0f;
			//if (m_MainUI.IsShowSuvStatInfo()) {
			//	m_MainUI.ShowSuvStatInfo(false);
			//}
			if (m_MainUI.IsShowSkillInfo() || m_MainUI.IsShowSynergyInfo() || m_MainUI.IsShowAlarmToolTip() || m_MainUI.IsShowDebuffCardInfo() ) {
				m_MainUI.ShowSkillInfo(false);
				m_MainUI.OffSynergyInfo();
				m_MainUI.OffAlarmToolTip();
				m_MainUI.OffAlarmDebuffCardToolTip(); 
				m_MainUI.ShowSuvStatInfo(false);
			}
			else {
				Ray ray = m_MyCam.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hit = Physics.RaycastAll(ray, m_MyCam.farClipPlane);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					Item_Stage card = hitobj.GetComponent<Item_Stage>();
					Item_Stage_Char user = hitobj.GetComponent<Item_Stage_Char>();
					if (card != null) {
						if (TUTO.TouchCheckLock(TutoTouchCheckType.StageCard, card)) continue;
						if (POPUP.IS_PopupUI()) break;
						// 스테이지 카드 선택
						if (!card.IS_CanTouch) break;
						if (card.m_Line != 0) break;
						if (card.IS_Lock) {
							card.LockTouch();
							STAGE_USERINFO.CharSpeech(DialogueConditionType.CardLock);
							break;
						}
						if (card.m_Info.IS_RoadBlock) {
							STAGE_USERINFO.CharSpeech(DialogueConditionType.Wall);
							break;
						}

						m_StageAction = SetSelectCard(card);
						StartCoroutine(m_StageAction);

						if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.SelectStageCard_1)) TUTO.Next();

						break;
					}
					else if (user != null && user.m_Info != null) {
						// 정보 버튼 클릭 확인
						for (int j = 0; j < hit.Length; j++) {
							hitobj = hit[j].transform.gameObject;
							if (!hitobj.activeSelf) continue;
							if (hitobj == user.GetInfoBtn) {
								//if (!TUTO.IsTutoPlay() || (TUTO.IsTutoPlay() && TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Focus_SkillInfoBtn)))
								if (!POPUP.IS_PopupUI() && !POPUP.IS_TutoUI())
									m_MainUI.ShowSkillInfo(true, user.m_Info.m_Skill[0].m_LV, user.m_Info.IS_SetEquip(), user.m_Info.m_Skill[0].m_TData);
								return;
							}
						}
						if (TUTO.TouchCheckLock(TutoTouchCheckType.StageCard_Char, user)) continue;
						// 캐릭터 스킬 사용 
						if (!user.IS_UseActiveSkill() || STAGE_USERINFO.m_RemainActiveSkillCnt == 0) {
							//DialogueConditionType dlconditype = DialogueConditionType.None;
							if (STAGE_USERINFO.m_AP[0] < user.m_Info.GetNeedAP())
								STAGE_USERINFO.CharSpeech(DialogueConditionType.LowAP, user);
							else if (user.m_SkillCoolTime > 1)
								STAGE_USERINFO.CharSpeech(DialogueConditionType.CoolTime, user);
							break;
						}

						m_UseSkillChar = user;
						m_StageAction = UseUseSkill(m_UseSkillChar);
						StartCoroutine(m_StageAction);

						if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Select_0_1021Char)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Select_1_1031Char)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Select_2_1052Char)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Select_3_1029Char)) TUTO.Next();
						//else if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Select_Char_1024)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Select_Char_1012)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.Select_Char_1013)) TUTO.Next();
						else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Select_Char_1002)) TUTO.Next();
						//if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Select_3Char)) TUTO.Next();
						//else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Select_2Char)) TUTO.Next();
						//else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Select_1Char)) TUTO.Next();
						break;
					}
				}
			}
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Eff
	public GameObject StartEff(Transform tfChar, string effPath, bool isLowEff = false, float _scaleratio = 1f)
	{
		Transform panel = isLowEff ? m_LowEffPanel : m_Damage.Panel;
		FX eff = null;
		if (m_FXPool.ContainsKey(effPath)) {
			m_FXPool[effPath].RemoveAll(o => o.Obj == null);
			eff = m_FXPool[effPath].Find(o => !o.Obj.activeSelf);
		}
		if (eff == null) {
			GameObject obj = UTILE.LoadPrefab(effPath, true, panel);
			if (!m_FXPool.ContainsKey(effPath)) m_FXPool.Add(effPath, new List<FX>());
			eff = new FX() { Obj = obj, Scale = obj.transform.localScale, Rot = obj.transform.localEulerAngles };
			m_FXPool[effPath].Add(eff);
		}
		eff.Obj.transform.position = GetStage_EffPos(tfChar.position);
		eff.Obj.transform.localEulerAngles = eff.Rot;
		eff.Obj.transform.localScale = _scaleratio * eff.Scale / panel.localScale.x;
		eff.Obj.SetActive(true);
		return eff.Obj;
	}
	public GameObject StartEff(Vector3 position, string effPath, bool isLowEff = false)
	{
		Transform panel = isLowEff ? m_LowEffPanel : m_Damage.Panel;
		FX eff = null;
		if (m_FXPool.ContainsKey(effPath)) {
			m_FXPool[effPath].RemoveAll(o => o.Obj == null);
			eff = m_FXPool[effPath].Find(o => !o.Obj.activeSelf);
		}
		if (eff == null) {
			GameObject obj = UTILE.LoadPrefab(effPath, true, panel);
			if (!m_FXPool.ContainsKey(effPath)) m_FXPool.Add(effPath, new List<FX>());
			eff = new FX() { Obj = obj, Scale = obj.transform.localScale, Rot = obj.transform.localEulerAngles };
			m_FXPool[effPath].Add(eff);
		}
		eff.Obj.transform.position = GetStage_EffPos(position);
		eff.Obj.transform.localEulerAngles = eff.Rot;
		eff.Obj.transform.localScale = eff.Scale / panel.localScale.x;
		eff.Obj.SetActive(true);
		return eff.Obj;
	}
	public void EndEff(string effPath) {
		if (m_FXPool.ContainsKey(effPath)) {
			for(int i = m_FXPool[effPath].Count - 1; i > -1; i--) {
				m_FXPool[effPath][i].Obj.SetActive(false);
			}
		}
	}
	public void AllOffEff() {
		for (int i = 0; i < m_FXPool.Count; i++) {
			var fxs = m_FXPool.ElementAt(i).Value;
			for (int j = 0; j < fxs.Count; j++) {
				if(fxs[j].Obj != null) fxs[j].Obj.SetActive(false);
			}
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Making
	public void AddMaterial(StageMaterialType type, int Cnt = 1, Vector3 _spos = default(Vector3))
	{
		int cnt = Cnt;
		//시너지
		float? synergy2 = STAGE.m_User.GetSynergeValue(JobType.Engineer, 1);
		if (synergy2 != null && UTILE.Get_Random(0f,1f) < (float)synergy2) {
			cnt += 1;
			STAGE_USERINFO.ActivateSynergy(JobType.Engineer);
			Utile_Class.DebugLog_Value("Engineer 시너지 발동 " + "변화 전 -> 후 : 전 :" + Cnt.ToString() + " 후 : " + cnt.ToString());
			//STAGE.m_User.m_SynergyUseCnt[JobType.Engineer]++;
		}
		//카드 수와 가용량 넘겨 연출
		m_MainUI?.GetMatCard(type, cnt, _spos);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stat
	public int AddStat(StatType type, int value, bool UseCheck = true, bool _usesynergy = true)
	{
		int val = m_User.AddStat(type, value, _usesynergy);
		if (UseCheck)
		{
			m_Check.Check(StageCheckType.Rec_Stat, (int)type, value);
			if (STAGE_USERINFO.Is_UseStat(type)) {
				if (value > 0) PlayAddStatSnd(type);
				if (value < 0) PlayEffSound(SND_IDX.SFX_0470);
			}
		}
		return val;
	}

	public int AddAP(int _val, bool _usecheck = true, bool _usesynergy = true) {
		int val = STAGE_USERINFO.m_AP[0] + _val;
		STAGE_USERINFO.m_AP[0] = Mathf.Clamp(val, 0, STAGE_USERINFO.m_AP[1]);
		if (_usecheck) {
			//m_Check.Check(StageCheckType.Rec_Stat, (int)type, value);
		}
		return val;
	}
	public void SetBuff(EStageBuffKind kind, int idx)
	{
		TStageCardTable data = TDATA.GetStageCardTable(idx);
		StageCardType type = data.m_Type;
		float befor = 0;
		// 스텟 MAX 증가는 현재 수치도 같이 증가 시켜준다.
		// 클리어 조건에 문제가 되지 않도록 Stage에서 올려준다.
		switch (type)
		{
		/// <summary> HP 회복 </summary>
		case StageCardType.HpUp:
			befor = m_User.GetMaxStat(StatType.HP);
			break;
		/// <summary> 포만도 증가 </summary>
		case StageCardType.SatUp:
			befor = m_User.GetMaxStat(StatType.Sat);
			break;
		/// <summary> 청결도 증가 </summary>
		case StageCardType.HygUp:
			befor = m_User.GetMaxStat(StatType.Hyg);
			break;
		/// <summary> 정신력 증가 </summary>
		case StageCardType.MenUp:
			befor = m_User.GetMaxStat(StatType.Men);
			break;
		}

		float prebuffval = m_User.GetBuffValue(type);
		m_User.SetBuff(kind, idx);
		//알람
		if (m_AddStatAlarm.ContainsKey(m_CenterChar.transform) && m_AddStatAlarm[m_CenterChar.transform] == null) m_AddStatAlarm.Remove(m_CenterChar.transform);
		if (!m_AddStatAlarm.ContainsKey(m_CenterChar.transform)) {
			m_AddStatAlarm.Add(m_CenterChar.transform, UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>());
		}
		m_AddStatAlarm[m_CenterChar.transform].SetData(m_CenterChar.transform, type, m_User.GetBuffValue(type) - prebuffval);

		switch (type)
		{
		/// <summary> HP 회복 </summary>
		case StageCardType.HpUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) - befor), false, false);
			break;
		/// <summary> 포만도 증가 </summary>
		case StageCardType.SatUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) - befor), false);
			break;
		/// <summary> 청결도 증가 </summary>
		case StageCardType.HygUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) - befor), false);
			break;
		/// <summary> 정신력 증가 </summary>
		case StageCardType.MenUp:
			AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) - befor), false);
			break;
		}
		
		//하드/나이트메어 시작 디버프 및 조합칸 버프디버프 
		switch (type) {
			case StageCardType.MergeDelete:
				PlayEffSound(SND_IDX.SFX_0215);
				break;
			case StageCardType.MergeFailChance:
				PlayEffSound(SND_IDX.SFX_0216);
				break;
			case StageCardType.HateBuff:
				PlayEffSound(SND_IDX.SFX_0472);
				break;
			case StageCardType.SkillStatus:
			case StageCardType.TurnAllStatus:
			case StageCardType.TurnStatusHp:
				PlayEffSound(SND_IDX.SFX_0471);
				break;
			case StageCardType.TurnStatusHyg:
			case StageCardType.TurnStatusMen:
			case StageCardType.TurnStatusSat:
				PlayEffSound(SND_IDX.SFX_0470);
				break;
			case StageCardType.MergeSlotCount:
				PlayEffSound(SND_IDX.SFX_0196);
				break;
			default:
				//안좋은 효과 받은 경우
				if (prebuffval > m_User.GetBuffValue(type)) {
					StartEff(m_CenterChar.transform, "Effect/Stage/Eff_Debuff_Ch");
					switch (type) {
						case StageCardType.HpUp:
						case StageCardType.RecoveryAP:
						case StageCardType.TimePlus:
						case StageCardType.LimitTurnUp:
						case StageCardType.AddRerollCount:
							PlayEffSound(SND_IDX.SFX_0472);
							break;
						default:
							PlayEffSound(SND_IDX.SFX_0470);
							break;
					}
				}
				else if (prebuffval < m_User.GetBuffValue(type)) {
					switch (type) {
						case StageCardType.TimePlus:
						case StageCardType.LimitTurnUp:
						case StageCardType.AddRerollCount:
							PlayEffSound(SND_IDX.SFX_0462);
							break;
						default:
							PlayEffSound(SND_IDX.SFX_0461);
							break;
					}
				}
				break;
		}
		//--m_MainUI?.GetBuffFX();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Battle & Reward

	IEnumerator StartBattle(EBattleMode Mode, Item_Stage _card, IEnumerator delay = null)
	{
		StageCardInfo enemyInfo = _card.m_Info;
#if !STAGE_NO_BATTLE
		if (m_AutoPlay) {
			for(int i = 0; i < m_ViewCard[0].Length; i++) {
				if (m_ViewCard[0][i] != null && !m_ViewCard[0][i].m_Info.IS_EnemyCard) m_ViewCard[0][i].m_Sort.sortingOrder = 0;
			}
			yield return AutoBattle(_card, _card.transform.position, _card.transform.localScale, _card.m_Sort.sortingOrder + 1);
			for (int i = 0; i < m_ViewCard[0].Length; i++) {
				if (m_ViewCard[0][i] != null && m_ViewCard[0][i] != _card) m_ViewCard[0][i].m_Sort.sortingOrder = 3;
			}
			yield break;
		}
		switch (STAGEINFO.m_StageContentType)
		{
		case StageContentType.Cemetery:
		case StageContentType.Stage:
		case StageContentType.University:
		case StageContentType.Tower:
		case StageContentType.Factory:
		case StageContentType.Replay:
		case StageContentType.ReplayHard:
		case StageContentType.ReplayNight:
			if (!_card.m_Info.IS_Boss) {
				for (int i = 0; i < m_ViewCard[0].Length; i++) {
					if (m_ViewCard[0][i] != null && !m_ViewCard[0][i].m_Info.IS_EnemyCard) m_ViewCard[0][i].m_Sort.sortingOrder = 0;
				}
				yield return AutoBattle(_card, _card.transform.position, _card.transform.localScale, _card.m_Sort.sortingOrder + 1);
				for (int i = 0; i < m_ViewCard[0].Length; i++) {
					if (m_ViewCard[0][i] != null && m_ViewCard[0][i] != _card) m_ViewCard[0][i].m_Sort.sortingOrder = 3;
				}
				//전투 종료 후 스탯 감소
				if (STAGE_USERINFO.Is_UseStat(StatType.Men)) {
					float? synergyLT = STAGE_USERINFO.GetSynergeValue(JobType.Lunatic, 1);
					if (synergyLT != null && _card.m_Info.m_TEnemyData.m_Tribe == EEnemyTribe.Human)
						STAGE_USERINFO.ActivateSynergy(JobType.Lunatic);
					else {
						float val = m_User.GetMaxStat(StatType.Men) * USERINFO.GetSkillValue(SkillKind.BattleEndMen) - _card.m_Info.m_TEnemyData.GetBTReduceStat(StatType.Men, _card.m_Info.m_LV);
						yield return AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(val));
					}
				}
				if (STAGE_USERINFO.Is_UseStat(StatType.Hyg)) {
					float val = m_User.GetMaxStat(StatType.Hyg) * USERINFO.GetSkillValue(SkillKind.BattleEndHyg) - _card.m_Info.m_TEnemyData.GetBTReduceStat(StatType.Hyg, _card.m_Info.m_LV);
					yield return AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(val));
				}
				if (STAGE_USERINFO.Is_UseStat(StatType.Sat)) {
					float val = m_User.GetMaxStat(StatType.Sat) * USERINFO.GetSkillValue(SkillKind.BattleEndSat) - (BaseValue.STGBATTLE_REDUCTION_SAT(USERINFO.GetDifficulty()) + _card.m_Info.m_TEnemyData.GetBTReduceStat(StatType.Sat, _card.m_Info.m_LV));
					yield return AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(val));
				}
				yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * USERINFO.GetSkillValue(SkillKind.BattleEndHp) - _card.m_Info.m_TEnemyData.GetBTReduceStat(StatType.HP, _card.m_Info.m_LV)));
					
				yield break;
			}
			break;
		}

		AsyncOperation pAsync = null;
		POPUP.GetWorldUIPanel().gameObject.SetActive(false);
		// 액션이 끝날때 까지 대기
		yield return BattleStartAction();
		SetActiveStage(false);
		if (delay != null) yield return delay;
		TEnemyTable enemyTable = TDATA.GetEnemyTable(enemyInfo.m_EnemyIdx);
		pAsync = MAIN.StateChange(MainState.BATTLE, SceneLoadMode.ADDITIVESTART, () =>
		{
			MAIN.ActiveScene(() =>
			{
				m_MyCam.gameObject.SetActive(false);
				BATTLE.Init(Mode, enemyInfo.m_EnemyIdx, enemyInfo.m_LV, enemyInfo.GetStat(EEnemyStat.HP), () => {
					// 전투 종료
					m_MyCam.gameObject.SetActive(true);
					PlayStageBGSound();
					if (!CheckEnd() && !enemyTable.ISRefugee())
					{
						m_Check.Check(StageCheckType.KillEnemy, enemyTable.m_Idx);
						m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemyTable.m_Type);
						m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemyTable.m_Tribe);
						m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemyTable.m_Grade);
						m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
						CheckEnd();
					}
				}, true);
			});
		});
		yield return new WaitWhile(() => !MAIN.IS_State(MainState.STAGE));

		SetActiveStage(true);

		yield return BattleEndAction();
		POPUP.GetWorldUIPanel().gameObject.SetActive(true);

		//실패조건이 전투횟수일 경우 유아이 갱신
		STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);
#else
		_card.IS_KilledDuel = true;
		TEnemyTable enemyTable = TDATA.GetEnemyTable(enemyInfo.m_EnemyIdx);
		m_Check.Check(StageCheckType.KillEnemy, enemyTable.m_Idx);
		m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemyTable.m_Type);
		m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemyTable.m_Tribe);
		m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemyTable.m_Grade);
		yield break;
#endif
	}

	/// <summary> 배틀씬 없이 카드끼리 싸우는 자동 전투</summary>
	/// 데미지 공식 다시 적용, 공속도 적용 동시에 패는게 아니라 턴이라는게 존재한다?
	IEnumerator AutoBattle(Item_Stage _card, Vector3 _prepos, Vector3 _prescale, int _layerorder, bool _first = true, bool _enemystun = false) {
		AutoCamPosInit = false;
		List<AtkFXType> fxcheck = new List<AtkFXType>();
		bool enemystun = _enemystun;
		//공겨하는 플레이어 캐릭터
		Item_Stage_Char atker = m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
		//전투대상 확대
		float movehalftime = 0.35f;
		Vector3 prepos = _prepos;
		Vector3 prescale = _prescale;
		int prelayer = _layerorder;
		if (_first) {//적 확대 및 카드락 이미지 제거
			_card.SetLockOnOff(false);
			iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos + new Vector3(0f, BaseValue.STAGE_INTERVER.y * 0.03f, 0f), "time", movehalftime, "easetype", "easeinquart"));
			iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", prescale * 1.3f, "time", movehalftime, "easetype", "easeinquart"));
			_card.m_Sort.sortingOrder = 4;
			yield return new WaitForSeconds(movehalftime + 0.5f);
		}
		//적이 받는 데미지
		StageCardInfo enemyInfo = _card.m_Info;
		float EnemyHP = enemyInfo.GetStat(EEnemyStat.HP);
		int atk = Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Atk));
		atk += Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(enemyInfo.m_TEnemyData));
		float dna = DNACheck(atker, OptionType.AttackingDmgAdd);
		atk += Mathf.RoundToInt(atk * dna);
		dna = DNACheck(atker, OptionType.AttackingDefDown); ;
		float EnemyDamage = BaseValue.GetDamage(atk, 
			STAGE_USERINFO.m_CharLV,
			Mathf.RoundToInt(enemyInfo.GetStat(EEnemyStat.DEF) * (1f - Mathf.Clamp(dna, 0f, 1f))), 
			1, 
			ENoteHitState.End);

		//권장전투력 데미지 배율
		EnemyDamage *= BaseValue.GetCPDmgRatio(true, false, true);

		//크리티컬 데미지 추가 1 -> 100%
		if (UTILE.Get_Random(0f, 1f) < STAGE_USERINFO.GetStat(StatType.Critical)) {
			fxcheck.Add(AtkFXType.Critical);
			EnemyDamage += Mathf.RoundToInt(EnemyDamage * (0.5f + 0.5f * STAGE_USERINFO.GetStat(StatType.CriticalDmg)));
		}

		//암살자 시너지
		float? synergyAS = STAGE.m_User.GetSynergeValue(JobType.Assassin, 1);
		bool assassin = synergyAS != null && UTILE.Get_Random(0f, 1f) < (float)synergyAS && enemyInfo.m_TEnemyData.m_Grade != EEnemyGrade.Boss;
		if (assassin) {
			fxcheck.Add(AtkFXType.Assassin);
			EnemyDamage = EnemyDamage > enemyInfo.GetStat(EEnemyStat.HP) ? EnemyDamage : enemyInfo.GetStat(EEnemyStat.HP);
			Utile_Class.DebugLog_Value("Assassin 시너지 발동 즉사");
		}
		//헤드샷
		bool headshot = UTILE.Get_Random(0, 100) < STAGE_USERINFO.GetStat(StatType.HeadShot) * (100f - enemyInfo.GetStat(EEnemyStat.HIDING)) / 100f;
		if (headshot) {
			fxcheck.Add(AtkFXType.Headshot);
			EnemyDamage = EnemyDamage > enemyInfo.GetStat(EEnemyStat.HP) ? EnemyDamage : enemyInfo.GetStat(EEnemyStat.HP);
		}
		//DNA 즉사
		bool dnakill = UTILE.Get_Random(0f, 1f) < atker.m_Info.GetDNABuff(OptionType.AttackingKill);
		if (dnakill) {
			fxcheck.Add(AtkFXType.DNAKill);
			DNAAlarm(atker.m_Info, OptionType.AttackingKill);
			EnemyDamage = EnemyDamage > enemyInfo.GetStat(EEnemyStat.HP) ? EnemyDamage : enemyInfo.GetStat(EEnemyStat.HP);
		}

		if(m_SelectAtk || enemyInfo.m_TEnemyData.GetStat(EEnemyStat.SPD) <= STAGE_USERINFO.GetStat(StatType.Speed)) {
			yield return PlayerTurn(atker, _card, Mathf.RoundToInt(EnemyDamage), fxcheck);
			Utile_Class.DebugLog_Value(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}", "공격// player_atk", atk, " player_lv:", STAGE_USERINFO.m_CharLV, " enemy_def:", enemyInfo.GetStat(EEnemyStat.DEF), " atk_cnt:", 1, " dmg:", EnemyDamage, "enemy_maxHP", enemyInfo.GetMaxStat(EEnemyStat.HP), "enemy_nowHP", enemyInfo.GetStat(EEnemyStat.HP), "플레이어 전투력", USERINFO.m_PlayDeck.GetCombatPower(false, true)));
	
			//공격시 dna, synergy 체크
			SetAtkDNASynergy(atker, new object[] { Mathf.RoundToInt(EnemyDamage) });
			//적 스턴
			if(!enemystun) enemystun = UTILE.Get_Random(0f, 1f) < atker.m_Info.GetDNABuff(OptionType.AttackingStun);
			if (enemystun) DNAAlarm(atker.m_Info, OptionType.AttackingStun);
			//DNA 말벌,골렘
			if (headshot) {
				HeadShotDNAAction(atker, _card);
			}

			if (enemyInfo.GetStat(EEnemyStat.HP) < 1) {
				GetKillEnemy(_card, atker, true, true, true);
				m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
				BATTLEINFO.m_Result = EBattleResult.WIN;

				//실패조건이 전투횟수일 경우 유아이 갱신
				STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);

				yield break;
			}

			yield return new WaitForSeconds(0.2f);

			if (!enemystun) {
				float stundmg = 0f;
				if (enemyInfo.m_TEnemyData.m_Deadly) stundmg = STAGE_USERINFO.GetMaxStat(StatType.HP);
				yield return EnemyTurn(_card, Mathf.RoundToInt(stundmg));
				if (STAGE_USERINFO.GetStat(StatType.HP) < 1) {
					//전투대상 원래자리로 축소
					iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos, "time", movehalftime, "easetype", "easeinquart"));
					iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", prescale, "time", movehalftime, "easetype", "easeinquart"));
					_card.m_Sort.sortingOrder = prelayer;
					yield return new WaitForSeconds(movehalftime);
					BATTLEINFO.m_Result = EBattleResult.LOSE;
					if (!CheckEnd()) {
						m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
						CheckEnd();
					}
					//실패조건이 전투횟수일 경우 유아이 갱신
					STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);
					yield break;
				}

				//반사데미지 입는 경우
				if (enemyInfo.GetStat(EEnemyStat.HP) < 1) {
					GetKillEnemy(_card, atker, false, false);
					m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
					BATTLEINFO.m_Result = EBattleResult.WIN;

					//실패조건이 전투횟수일 경우 유아이 갱신
					STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);
					yield break;
				}
			}
		}
		else {//플레이어 후공
			if (!enemystun) {
				float stundmg = 0f;
				if (enemyInfo.m_TEnemyData.m_Deadly) stundmg = STAGE_USERINFO.GetMaxStat(StatType.HP);
				yield return EnemyTurn(_card, Mathf.RoundToInt(stundmg));

				if (STAGE_USERINFO.GetStat(StatType.HP) < 1) {
					//전투대상 원래자리로 축소
					iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos, "time", movehalftime, "easetype", "easeinquart"));
					iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", prescale, "time", movehalftime, "easetype", "easeinquart"));
					_card.m_Sort.sortingOrder = prelayer;
					yield return new WaitForSeconds(movehalftime);
					BATTLEINFO.m_Result = EBattleResult.LOSE;

					if (!CheckEnd()) {
						m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
						CheckEnd();
					}
					//실패조건이 전투횟수일 경우 유아이 갱신
					STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);

					yield break;
				}
				//반사데미지 입은 경우
				if (enemyInfo.GetStat(EEnemyStat.HP) < 1) {
					GetKillEnemy(_card, atker, false, false);
					m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
					BATTLEINFO.m_Result = EBattleResult.WIN;
					//실패조건이 전투횟수일 경우 유아이 갱신
					STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);

					yield break;
				}
			}

			yield return new WaitForSeconds(0.2f);

			yield return PlayerTurn(atker, _card, Mathf.RoundToInt(EnemyDamage), fxcheck);
			Utile_Class.DebugLog_Value(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}{12}{13}{14}{15}", "공격// player_atk", atk, " player_lv:", STAGE_USERINFO.m_CharLV, " enemy_def:", enemyInfo.GetStat(EEnemyStat.DEF), " atk_cnt:", 1, " dmg:", EnemyDamage, "enemy_maxHP", enemyInfo.GetMaxStat(EEnemyStat.HP), "enemy_nowHP", enemyInfo.GetStat(EEnemyStat.HP), "플레이어 전투력", USERINFO.m_PlayDeck.GetCombatPower(false, true)));
			
			SetAtkDNASynergy(atker, new object[] { Mathf.RoundToInt(EnemyDamage) });
			//적 스턴
			enemystun = UTILE.Get_Random(0f, 1f) < atker.m_Info.GetDNABuff(OptionType.AttackingStun);
			if(enemystun) DNAAlarm(atker.m_Info, OptionType.AttackingStun);
			
			//DNA 말벌,골렘
			if (headshot) {
				HeadShotDNAAction(atker, _card);
			}
			if (enemyInfo.GetStat(EEnemyStat.HP) < 1) {
				GetKillEnemy(_card, atker, true, true, true);
				m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
				BATTLEINFO.m_Result = EBattleResult.WIN;

				//실패조건이 전투횟수일 경우 유아이 갱신
				STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount, -1);

				yield break;
			}
		}
		yield return new WaitForSeconds(movehalftime);
		//둘다 살아있으면 반복
		if (enemyInfo.GetStat(EEnemyStat.HP) > 0 && STAGE_USERINFO.GetStat(StatType.HP) > 0)
			yield return AutoBattle(_card, prepos, prescale, _card.m_Sort.sortingOrder + 1, false, enemystun);

		AutoCamPosInit = true;
	}
	IEnumerator EnemyTurn(Item_Stage _card, int _dmg = 0) {
		//1.근접이면 센터 캐릭터쪽으로 치는 무브 원거리면 제자리 공격 
		//2.치고 돌아오는 동안 대기
		//3.사운드, 이펙트, 카메라 쉐이킹

		EnemySkillTable skill = _card.m_Info.m_TEnemyData.m_Skill.GetStageAISkill();
		TEnemyStageSkillTable table = TDATA.GetEnemyStageSkillTableGroup(_card.m_Info.m_TEnemyData.m_Skill.m_GroupID);
		Item_Stage_Char target = m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
		Vector3 prepos = _card.gameObject.transform.position;
		Vector3 prescale = _card.gameObject.transform.localScale;
		Vector3 targetpos = target.gameObject.transform.position;
		Vector3 targetfxpos = target.transform.position + new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
		int prelayer = _card.m_Sort.sortingOrder;
		float movehalftime = 0f;
		int atkcnt = UTILE.Get_Random(table.m_AtkCnt[0], table.m_AtkCnt[1] + 1);//스킬 횟수
		
		float dnahdu = target.m_Info.GetDNABuff(OptionType.HitDefUp);
		//플레이어가 받는 데미지
		float dmg = BaseValue.GetDamage(_card.m_Info.GetStat(EEnemyStat.ATK),
			_card.m_Info.m_LV,
			Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Def) * (1f + dnahdu + m_User.GetDefSkillVal(_card.m_Info.m_TEnemyData))),
			1,
			ENoteHitState.End);
		//권장전투력 데미지 배율
		dmg *= BaseValue.GetCPDmgRatio(false, false, true);
		//DNA 버프
		float dnahdd = DNACheck(target, OptionType.HitDmgDown); 
		dmg -= Mathf.RoundToInt(dmg * Mathf.Clamp(dnahdd, 0f, 1f));

		_dmg = _dmg == 0 ? Mathf.RoundToInt(dmg) : _dmg;
		Utile_Class.DebugLog_Value(string.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}", "피격// enemy_idx:", _card.m_Info.m_EnemyIdx, " enemy_atk:", _card.m_Info.GetStat(EEnemyStat.ATK), " enemy_lv:", _card.m_Info.m_LV, " player_def:", STAGE_USERINFO.GetStat(StatType.Def), " atk_cnt:", 1, " dmg:", _dmg));

		bool dodge = UTILE.Get_Random(0f, 1f) < target.m_Info.GetDNABuff(OptionType.HitDodge);
		//모션
		for (int i = 0; i < atkcnt; i++) {
			_card.m_Sort.sortingOrder = 4;//카드위에 올라가는 고정 이펙트들이 오더3임 
			switch (table.m_ATKType) {
				//근접
				case ESkillATKType.None:
				case ESkillATKType.Hit:
				case ESkillATKType.Move:
				case ESkillATKType.Bite:
				case ESkillATKType.Attack:
				case ESkillATKType.Slash:
				case ESkillATKType.Scratch:
				case ESkillATKType.MultiHit:
				case ESkillATKType.MultiBite:
				case ESkillATKType.MultiAttack:
				case ESkillATKType.MultiSlash:
				case ESkillATKType.MultiScratch:
				case ESkillATKType.ZombieBite:
				case ESkillATKType.ZombieMultiBite:
					movehalftime = 0.12f;
					Vector3 pluspos = (targetpos - prepos) * 0.35f;
					pluspos.z = 0f;
					iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos + pluspos, "time", movehalftime, "easetype", "easeinquart"));
					yield return new WaitForSeconds(movehalftime);
					iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos, "time", movehalftime, "easetype", "easeoutquart"));
					break;
				//원거리

				case ESkillATKType.Spit:
				case ESkillATKType.ZombieMultiSpit:
				case ESkillATKType.Continuous:
					movehalftime = 0.1f;
					iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos + new Vector3(0f, -0.75f, 0f), "time", movehalftime, "easetype", "easeinquart"));
					yield return new WaitForSeconds(movehalftime);
					iTween.MoveTo(_card.gameObject, iTween.Hash("position", prepos, "time", movehalftime, "easetype", "easeoutquart"));
					break;
			}

			//사운드
			switch (table.m_ATKType) {
				case ESkillATKType.Hit:
				case ESkillATKType.Attack:
				case ESkillATKType.Move:
				case ESkillATKType.MultiAttack:
				case ESkillATKType.MultiHit:
					PlayEffSound(SND_IDX.SFX_0440);
					break;
				case ESkillATKType.Bite:
					PlayEffSound(SND_IDX.SFX_0701);
					break;
				case ESkillATKType.Slash:
				case ESkillATKType.Scratch:
					PlayEffSound(SND_IDX.SFX_0420);
					break;
				case ESkillATKType.Spit:
				case ESkillATKType.ZombieMultiSpit:
					PlayEffSound(SND_IDX.SFX_0732);
					break;
				case ESkillATKType.MultiBite:
					PlayEffSound(SND_IDX.SFX_0701);
					break;
				case ESkillATKType.MultiSlash:
				case ESkillATKType.MultiScratch:
					PlayEffSound(SND_IDX.SFX_0420);
					break;
				case ESkillATKType.ZombieBite:
				case ESkillATKType.ZombieMultiBite:
					PlayEffSound(SND_IDX.SFX_0702);
					break;
				case ESkillATKType.Continuous:
					PlayEffSound(SND_IDX.SFX_0400);
					break;
			}

			//이펙트
			string fxname = table.m_FXNames.Count > 0 ? string.Format("Effect/Stage/Atk/{0}", table.m_FXNames[UTILE.Get_Random(0, table.m_FXNames.Count)]) : "Effect/Stage/Atk/Eff_EnemyAtk_Hit_01";
			
			int fxcnt = 0;
			switch (table.m_ATKType) {
				case ESkillATKType.MultiHit:
					fxcnt = UTILE.Get_Random(2, 4);
					break;
				case ESkillATKType.MultiAttack:
					fxcnt = UTILE.Get_Random(2, 4);
					break;
				case ESkillATKType.MultiBite:
					fxcnt = UTILE.Get_Random(4, 6);
					break;
				case ESkillATKType.MultiSlash:
					fxcnt = UTILE.Get_Random(2, 5);
					break;
				case ESkillATKType.MultiScratch:
					fxcnt = UTILE.Get_Random(2, 5);
					break;
				case ESkillATKType.ZombieMultiBite:
					fxcnt = UTILE.Get_Random(2, 3);
					break;
				case ESkillATKType.ZombieMultiSpit:
					fxcnt = UTILE.Get_Random(2, 4);
					break;
				default:
					fxcnt = 1;
					break;
			}
			for (int j = 0; j < fxcnt; j++) {
				float delay = 0f;
				switch (table.m_ATKType) {
					case ESkillATKType.MultiHit:
						delay = UTILE.Get_Random(10, 21) * 0.01f;
						break;
					case ESkillATKType.MultiAttack:
						delay = UTILE.Get_Random(10, 21) * 0.01f;
						break;
					case ESkillATKType.MultiBite:
						delay = UTILE.Get_Random(0, 11) * 0.01f;
						break;
					case ESkillATKType.MultiSlash:
						delay = UTILE.Get_Random(10, 21) * 0.01f;
						break;
					case ESkillATKType.MultiScratch:
						delay = UTILE.Get_Random(10, 21) * 0.01f;
						break;
					case ESkillATKType.ZombieMultiBite:
						delay = UTILE.Get_Random(10, 21) * 0.01f;
						break;
					case ESkillATKType.ZombieMultiSpit:
						delay = UTILE.Get_Random(10, 21) * 0.01f;
						break;
				}

				if (fxcnt > 1)
					targetfxpos = m_CenterChar.transform.position + new Vector3(UTILE.Get_Random(-3f, 3f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
				else
					targetfxpos = target.transform.position + new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);

				StartEff(targetfxpos, fxname);
				if (dnahdu > 0) DNAAlarm(target.m_Info, OptionType.HitDefUp);
				if (dnahdd > 0) DNAAlarm(target.m_Info, OptionType.HitDmgDown);
				//플레이어 피격 카메라 쉐이크
				CamAction(CamActionType.Pos_Player);
				CamActionType normalhittype = UTILE.Get_Random(0f, 1f) < 0.7f ? CamActionType.Battle_Hit : CamActionType.Battle_Hit_2;
				CamAction(normalhittype);

				if (dodge) {
					DNAAlarm(target.m_Info, OptionType.HitDodge);
				}
				else {
					DuelDamageFontFX(targetfxpos, -_dmg / fxcnt);
				}
				yield return new WaitForSeconds(delay);
			}

			//데미지 적용
			if (!dodge) {
				if (_dmg >= Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * 0.01f) && UTILE.Get_Random(0, 100) < 30) {
					SND_IDX hitvocidx = target.m_Info.m_TData.GetHitVoice(target.m_TransverseAtkMode);
					PlayEffSound(hitvocidx);
				}
				AddStat(StatType.HP, -_dmg, -_dmg > 0);
				//쿨타임 감소
				DNACheck(target, OptionType.HitCoolDown);
				//랜덤 재료 획득
				DNACheck(target, OptionType.HitMaterialAdd);
			}
			//DNA 반사 데미지
			float dna2 = DNACheck(target, OptionType.HitThorn); 
			int reflectdmg = Mathf.RoundToInt(dna2 * _dmg);
			dna2 = target.m_Info.GetDNABuff(OptionType.DeathThorn);
			if (dna2 > 0) reflectdmg = _card.m_Info.GetMaxStat(EEnemyStat.HP);
			if (reflectdmg > 0) {
				DNAAlarm(target.m_Info, OptionType.DeathThorn);
				CamActionType normalhittype = UTILE.Get_Random(0f, 1f) < 0.7f ? CamActionType.Battle_Hit : CamActionType.Battle_Hit_2;
				CamAction(normalhittype);
				_card.SetDamage(true, reflectdmg);
			}

			yield return new WaitForSeconds(Math.Max(0f, movehalftime - 0.2f));
			_card.m_Sort.sortingOrder = prelayer;
			if (_card.IS_Die()) break;
		}
		//신부 시너지
		float? synergyFT2 = m_User.GetSynergeValue(JobType.Father, 1); 
		for (StatType j = StatType.Men; j < StatType.SurvEnd; j++) {
			if (!STAGE_USERINFO.Is_UseStat(j)) continue;
			float val = table.Get_SrvDmg(j) * _card.m_Info.m_TEnemyLvData.GetStat(EEnemyStat.ATKSURVSTAT) * atkcnt;

			if (synergyFT2 != null) {
				val *= 1f - (float)synergyFT2;
				STAGE_USERINFO.ActivateSynergy(JobType.Father);
			}
			yield return AddStat_Action(m_CenterChar.transform, j, -Mathf.RoundToInt(val));
		}
	}
	IEnumerator PlayerTurn(Item_Stage_Char _atker, Item_Stage _card, int _dmg, List<AtkFXType> _fxcheck) {
		//1.댁중 아무나 근접이면 1번라인 가운데로 치고 오고 원거리면 제자리 공격 연출
		//2.돌아오는 타이밍 대기
		//3.사운드, 이펙트
		//4.맞는 대상 움찔
		Vector3 prepos = _atker.gameObject.transform.position;
		Vector3 prescale = _atker.gameObject.transform.localScale;
		int prelayer = _atker.m_Sort.sortingOrder;
		Vector3 targetpos = _card.gameObject.transform.position;
		Vector3 targetprescale = _card.gameObject.transform.localScale; 
		int targetprelayer = _card.m_Sort.sortingOrder;
		float movetime = 0f;
		float startratio = 0.6f;
		float endratio = 0.4f;
		float attaktimingratio = 1f;
		int atkcnt = 1;
		int addatkcnt = 0;
		List<int> enemyhitanim = new List<int>();

		float? synergyP1 = m_User.GetSynergeValue(JobType.Police, 0);
		float? synergyP2 = m_User.GetSynergeValue(JobType.Police, 1);
		if (synergyP1 != null && synergyP2 != null) {
			bool addcnt = true;
			while (addcnt) {
				float rand = UTILE.Get_Random(0f, 1f);
				addcnt = rand < synergyP2 && atkcnt < Mathf.RoundToInt((float)synergyP1);
				if (addcnt) {
					STAGE_USERINFO.ActivateSynergy(JobType.Police);
					atkcnt++;
					addatkcnt++;
					synergyP2 *= 0.5f;
				}
			}
		}
		for (int i = 0; i < atkcnt; i++) {
			//무기별 모션
			_atker.m_Sort.sortingOrder = 4;//카드위에 올라가는 고정 이펙트들이 오더3임 
			ItemInfo item = _atker.m_Info.GetEquip(EquipType.Weapon);
			ItemType weapontype = item == null ? ItemType.None : item.m_TData.m_Type;
			switch (weapontype) {
				case ItemType.None:
				case ItemType.Blunt:
				case ItemType.Blade:
				case ItemType.Axe:
					movetime = 0.8f;
					attaktimingratio = 0.7f;
					Vector3 pluspos = (targetpos - prepos) * 0.7f;
					pluspos.z = 0f;
					iTween.MoveTo(_atker.gameObject, iTween.Hash("position", prepos + pluspos, "time", movetime * startratio * attaktimingratio, "easetype", "easeinexpo"));
					iTween.ScaleTo(_atker.gameObject, iTween.Hash("scale", prescale * 1.35f, "time", movetime * startratio * attaktimingratio, "easetype", "easeinexpo"));
					yield return new WaitForSeconds(movetime * startratio * attaktimingratio);
					iTween.MoveTo(_atker.gameObject, iTween.Hash("position", prepos, "time", movetime * endratio, "easetype", "easeoutexpo"));
					iTween.ScaleTo(_atker.gameObject, iTween.Hash("scale", prescale, "time", movetime * endratio, "easetype", "easeoutexpo"));
					break;
				case ItemType.Bow:
				case ItemType.Pistol:
				case ItemType.Shotgun:
				case ItemType.Rifle:
					movetime = 0.8f;
					attaktimingratio = 0.5f;
					iTween.MoveTo(_atker.gameObject, iTween.Hash("position", prepos + new Vector3(0f, 0.2f, 0f), "time", movetime * startratio * attaktimingratio, "easetype", "easeoutexpo"));
					iTween.ScaleTo(_atker.gameObject, iTween.Hash("scale", prescale * 1.25f, "time", movetime * startratio * attaktimingratio, "easetype", "easeoutexpo"));
					yield return new WaitForSeconds(movetime * startratio * attaktimingratio);
					iTween.MoveTo(_atker.gameObject, iTween.Hash("position", prepos, "time", movetime * endratio, "easetype", "linear"));
					iTween.ScaleTo(_atker.gameObject, iTween.Hash("scale", prescale, "time", movetime * endratio, "easetype", "linear"));
					break;
			}
			
			//무기별 이펙트, 사운드
			if (_fxcheck.Contains(AtkFXType.Headshot)) {
				StartEff(_card.transform, "Effect/Stage/Atk/Eff_Duel_Headshot_2");
				STAGE_USERINFO.CharSpeech(DialogueConditionType.HeadShot, _atker);
			}
			if (_fxcheck.Contains(AtkFXType.Assassin)) {
				STAGE_USERINFO.ActivateSynergy(JobType.Assassin);
			}

			switch (weapontype) {
				case ItemType.None:
				case ItemType.Blunt:
					PlayEffSound(SND_IDX.SFX_0441);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_Blunt_01");
					break;
				case ItemType.Blade:
					PlayEffSound(SND_IDX.SFX_0420);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_Blade_01");
					break;
				case ItemType.Axe:
					PlayEffSound(SND_IDX.SFX_0441);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_Axe_01");
					break;
				case ItemType.Bow:
					PlayEffSound(SND_IDX.SFX_0431);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_Bow_01");
					break;
				case ItemType.Pistol:
					PlayEffSound(SND_IDX.SFX_0401);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_Pistol_01");
					break;
				case ItemType.Shotgun:
					PlayEffSound(SND_IDX.SFX_0500);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_ShotGun_01");
					break;
				case ItemType.Rifle:
					PlayEffSound(SND_IDX.SFX_0400);
					StartEff(_card.transform, "Effect/Stage/Atk/Eff_ChAtk_Rifle_01");
					break;
			}
			//쉐이크
			if(enemyhitanim.Count < 1) {
				enemyhitanim = new List<int>() { 1, 2, 3, 4 };
			}
			int enemyanimidx = enemyhitanim[UTILE.Get_Random(0, enemyhitanim.Count)];
			enemyhitanim.Remove(enemyanimidx);

			//적 맞는 모션
			_card.m_Sort.sortingOrder = 4;//카드위에 올라가는 고정 이펙트들이 오더3임 
			iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", targetprescale * 1.2f, "time", movetime * 0.25f, "easetype", "easeoutquart"));
			iTween.ScaleTo(_card.gameObject, iTween.Hash("scale", targetprescale, "time", movetime * 0.25, "delay", movetime * 0.25, "easetype", "easeinquart"));
			//카드에 데미지 적용, 적한테 뜨는 데미지 폰트는 기존거 사용
			_card.SetCardAnim(string.Format("Hit_{0}", enemyanimidx));

			int dmg = _dmg;
			if(synergyP1 != null && synergyP2 != null) {//경찰 시너지 추가타는 데미지 반감
				dmg = Mathf.RoundToInt(_dmg * (i >= atkcnt - addatkcnt ? 0.5f : 1f));
			}
			_card.SetDamage(true, dmg);
			switch (_card.m_Pos) {
				case 0: CamAction(CamActionType.Pos_Card_1); break;
				case 1: CamAction(CamActionType.Pos_Card_2); break;
				case 2: CamAction(CamActionType.Pos_Card_3); break;
			}
			bool crishake = _fxcheck.Contains(AtkFXType.Assassin) || _fxcheck.Contains(AtkFXType.Critical)
				|| _fxcheck.Contains(AtkFXType.DNAKill) || _fxcheck.Contains(AtkFXType.Headshot);
			CamActionType normalhittype = UTILE.Get_Random(0f, 1f) < 0.7f ? CamActionType.Battle_Hit : CamActionType.Battle_Hit_2;
			CamAction(crishake ? CamActionType.Battle_Hit_Cri : normalhittype);

			yield return new WaitForSeconds(Mathf.Max(movetime * 0.5f, movetime * startratio * (1 - attaktimingratio)));
			_atker.m_Sort.sortingOrder = prelayer;
			_card.m_Sort.sortingOrder = targetprelayer;

			//랜덤 재료 획득
			float dna = _atker.m_Info.GetDNABuff(OptionType.AttackingMaterialAdd);
			if (dna > 0) STAGE.DNAAlarm(_atker.m_Info, OptionType.AttackingMaterialAdd);
			int dnamatcnt = Mathf.RoundToInt(dna);
			for (int j = 0; j < dnamatcnt; j++) {
				AddMaterial((StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat), 1);
			}
		}
	}
	
	/// <summary> 듀얼시 뜨는 데미지 폰트 </summary>
	void DuelDamageFontFX(Vector3 _pos, int _val) {
		GameObject fx = UTILE.LoadPrefab("Effect/Stage/Atk/Eff_Duel_DmgFontFX", true, POPUP.GetWorldUIPanel());
		fx.transform.position = Utile_Class.GetCanvasPosition(_pos);
		fx.transform.localScale = Vector3.one * 1f;
		fx.GetComponent<EF_DuelFontFX>().SetData(_val);
	}
	public bool CheckBattleReward(TIngameRewardTable reward)
	{
		TStageCardTable tcard = TDATA.GetStageCardTable(reward.m_Val);
		if (tcard == null) return false;
		if (reward.m_Prob < 1) return false;
		switch (tcard.m_Type)
		{
		case StageCardType.Synergy:
			if (!m_User.IS_CreateSynergy()) return false;

			// 스테이지 제한 체크
			if (tcard.m_LimitCount > 0)
			{
				// 생성 가능한 갯수
				int Cnt = 0;
				for (int i = m_ViewCard.Length - 1; i > -1; i--)
				{
					if (m_ViewCard[i] == null) continue;
					for (int j = m_ViewCard[i].Length - 1; j > -1; j--)
					{
						if (m_ViewCard[i][j] == null) continue;
						if (m_ViewCard[i][j].m_Info.m_Idx == tcard.m_Idx)
						{
							Cnt++;
							if (Cnt >= tcard.m_LimitCount) return false;
						}
					}
				}
			}
			break;
		}
		return !tcard.m_IsEndType;
	}

	IEnumerator Action_BattleReward(int groupid, int lv, bool _cancancle, bool _allgroup, bool _battlereward = true, int _pos = -1, Vector3 _spos = default(Vector3), bool _killelite = false)
	{
		yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);

		if (TUTO.IsTuto(TutoKind.Stage_101)) yield break;
		if (TUTO.IsTuto(TutoKind.Stage_102)) yield break;
		//if (TUTO.IsTuto(TutoKind.Stage_103)) yield break;

		bool endreward = false;
		int RewardIdx = 0;
		Predicate<TIngameRewardTable> checkBattleReward = CheckBattleReward;

		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, null, 0.3f);
		}
		m_MainUI.GuideCardLoop(false);

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Reward, (result, obj) => {
			//result는 스테이지 카드 인덱스
			endreward = true;
			RewardIdx = result;
			m_MainUI.GuideCardLoop(true);

			for (int i = 0; i < m_Chars.Length; i++) {
				m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, null, 0.3f);
			}

		}, new int[] { groupid, 0, _killelite ? BaseValue.BATTLE_REWARD_COMMON2_GID : BaseValue.BATTLE_REWARD_COMMON_GID }, lv, checkBattleReward, _cancancle, _allgroup, _battlereward, _spos);
		yield return new WaitWhile(() => !endreward);
		//yield return new WaitWhile(() => RewardIdx < 1);

		if (RewardIdx > 0) {
			TStageCardTable data = TDATA.GetStageCardTable(RewardIdx);
			m_Check.Check(StageCheckType.CardUse, (int)data.m_Type);
			switch (data.m_Type) {//버프카드류는 즉시 삭제될거라 새로 버프카드 추가되면 여기에 넣어줘야함
				case StageCardType.RecoveryHp:
				case StageCardType.RecoveryHpPer:
				case StageCardType.RecoverySat:
				case StageCardType.RecoveryMen:
				case StageCardType.RecoveryHyg:
				case StageCardType.PerRecoverySat:
				case StageCardType.PerRecoveryMen:
				case StageCardType.PerRecoveryHyg:
				case StageCardType.RecoveryAP:
				case StageCardType.Material:
				case StageCardType.Gamble:
				case StageCardType.LimitTurnUp:
				case StageCardType.AddRerollCount:
				//case StageCardType.HpUp:
				//case StageCardType.AtkUp:
				//case StageCardType.DefUp:
				//case StageCardType.EnergyUp:
				//case StageCardType.SatUp:
				//case StageCardType.HygUp:
				//case StageCardType.MenUp:
				//case StageCardType.Synergy:
				//case StageCardType.SpeedUp:
				//case StageCardType.CriticalUp:
				//case StageCardType.CriticalDmgUp:
				//case StageCardType.APRecoveryUp:
				//case StageCardType.APConsumDown:
				//case StageCardType.HealUp:
				//case StageCardType.LevelUp:
				//case StageCardType.TimePlus:
				// 팝업에서 이미 사용됨
					break;
				default:
					if(!data.IS_BuffCard())
					yield return RewardAction_Proc(RewardIdx, -1, _pos);
					break;
			}
		}
		//재료 얻는 경우는 제작 액션 끝날때까지 기다림
		if (STAGE.m_MainUI != null) yield return new WaitWhile(() => STAGE.m_MainUI.GetCraft().GetState() != Item_Stage_Make.State.None);
	}
	IEnumerator Action_BattleReward_Rand(Item_Stage card, int rewardgid, int lv, bool _allgroup) {
		if (TUTO.IsTuto(TutoKind.Stage_101)) yield break;
		if (TUTO.IsTuto(TutoKind.Stage_102)) yield break;
		//if (TUTO.IsTuto(TutoKind.Stage_103)) yield break;
		List<int> gids = new List<int>();
		int idx = 0;
		for (int i = 0; i < 3; i++) {
			int gid = _allgroup ? rewardgid : gids.Count < 2 ? BaseValue.BATTLE_REWARD_COMMON_GID : rewardgid;

			TIngameRewardTable table = TDATA.GetPickIngameReward(gid, lv, CheckBattleReward, true);
			gids.Add(table.m_Val);
		}
		idx = gids[UTILE.Get_Random(0, 3)];

		TStageCardTable tdata = TDATA.GetStageCardTable(idx); ;
		if (tdata.m_Type == StageCardType.BigSupplyBox) m_Check.Check(StageCheckType.GetBox, 0, Mathf.Max(1, (int)tdata.m_Value1));
		else m_Check.Check(StageCheckType.CardUse, (int)tdata.m_Type, 1, false);
		//m_Check.Check(StageCheckType.CardUse, (int)TDATA.GetStageCardTable(idx).m_Type);

		yield return RewardAction_Proc(idx, 1, card.m_Pos, card.transform.localPosition, card.transform.localScale);
	}
	void HeadShotDNAAction(Item_Stage_Char _atker, Item_Stage _defender) {
		if (UTILE.Get_Random(0f, 1f) < _atker.m_Info.GetDNABuff(OptionType.ExplosiveHeadshot)) {
			DNAAlarm(_atker.m_Info, OptionType.ExplosiveHeadshot);
			int Area = 3;
			int ShiftArea = Area / 2;
			int StartLine = _defender.m_Line - ShiftArea;
			int EndLine = StartLine + Area;
			int StartPos = _defender.m_Pos - ShiftArea - ShiftArea;
			for (int i = StartLine; i < EndLine; i++, StartPos++) {
				if (i < 0 || i > AI_MAXLINE) continue;
				int EndPos = StartPos + Area;
				for (int j = StartPos; j < EndPos; j++) {
					if (j < 0 || j >= m_ViewCard[i].Length) continue;
					Item_Stage item = m_ViewCard[i][j];
					if (_defender == item) continue;
					if (item.IS_Die()) continue;
					if (!item.m_Info.IS_DmgTarget()) continue;
					Utile_Class.DebugLog_Value("DNA ExplosiveHeadshot");

					StartEff(item.transform, "Effect/Stage/Atk/Eff_Duel_Headshot_2");
					item.SetDamage(false, item.m_Info.GetMaxStat(EEnemyStat.HP));
					StageCardInfo enemyInfo = item.m_Info;
					if (!CheckEnd() && !enemyInfo.m_TEnemyData.ISRefugee()) {
						m_Check.Check(StageCheckType.KillEnemy, enemyInfo.m_TEnemyData.m_Idx);
						m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemyInfo.m_TEnemyData.m_Type);
						m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemyInfo.m_TEnemyData.m_Tribe);
						m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemyInfo.m_TEnemyData.m_Grade);
						m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
						CheckEnd();
					}
				}
			}
		}
		if (UTILE.Get_Random(0f, 1f) < _atker.m_Info.GetDNABuff(OptionType.PenetratingHeadshot)) {
			DNAAlarm(_atker.m_Info, OptionType.PenetratingHeadshot);
			int StartPos = _defender.m_Pos - _defender.m_Line;
			for (int i = 0; i < AI_MAXLINE; i++, StartPos++) {
				if (StartPos < 0 || StartPos >= m_ViewCard[i].Length) continue;
				Item_Stage item = m_ViewCard[i][StartPos];
				if (_defender == item) continue;
				if (item.IS_Die()) continue;
				if (!item.m_Info.IS_DmgTarget()) continue;
				Utile_Class.DebugLog_Value("DNA PenetratingHeadshot");

				StartEff(item.transform, "Effect/Stage/Atk/Eff_Duel_Headshot_2");
				item.SetDamage(false, item.m_Info.GetMaxStat(EEnemyStat.HP));
				StageCardInfo enemyInfo = item.m_Info;
				if (!CheckEnd() && !enemyInfo.m_TEnemyData.ISRefugee()) {
					m_Check.Check(StageCheckType.KillEnemy, enemyInfo.m_TEnemyData.m_Idx);
					m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemyInfo.m_TEnemyData.m_Type);
					m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemyInfo.m_TEnemyData.m_Tribe);
					m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemyInfo.m_TEnemyData.m_Grade);
					m_Check.Check(StageCheckType.TurmoilCount, STAGEINFO.m_TStage.m_Fail.m_Value, 1);
					CheckEnd();
				}
			}
		}
	}

	int GetAtkDmg(Item_Stage _enemy, Item_Stage_Char _atker, bool _usedna, bool _useskill, bool _useutile, Item_Stage _utilecard = null, float _ratio = 1f) {
		CamActionType camactype = CamActionType.Battle_Hit;
		StageCardInfo enemyInfo = _enemy.m_Info;

		int atk = 0;
		atk = Mathf.RoundToInt(m_User.GetStat(StatType.Atk));
		float dna = 0;
		if (!_useutile && _usedna && _atker != null) {
			dna = DNACheck(_atker, OptionType.AttackingDmgAdd); 
			atk += Mathf.RoundToInt(atk * dna);
		}
		atk += Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(enemyInfo.m_TEnemyData));

		float times = 1f;
		if (_useskill && _atker != null) {
			SkillInfo skillinfo = _atker.m_Info.m_Skill[0];
			times = skillinfo.m_TData.GetValue(skillinfo.m_LV);
		}
		else if (_useutile && _utilecard != null) {
			times = _utilecard.m_Info.m_NowTData.m_Value1;
		}
		if (_atker != null) {
			dna = DNACheck(_atker, OptionType.AttackingDefDown);
		}
		float dmg = BaseValue.GetDamage(atk,
			STAGE_USERINFO.m_CharLV,
			//DNA 방어력 감소
			Mathf.RoundToInt(enemyInfo.GetStat(EEnemyStat.DEF) * (_usedna == true && _atker != null ? (1f - Mathf.Clamp(dna, 0f, 1f)) : 1f)),
			times,
			ENoteHitState.End);

		//권장전투력 데미지 배율
		dmg *= BaseValue.GetCPDmgRatio(true, false, true);
		if (_usedna && _atker != null) {
			//크리티컬 데미지 추가 1 -> 100%
			if (UTILE.Get_Random(0f, 1f) < STAGE_USERINFO.GetStat(StatType.Critical)) {
				dmg += Mathf.RoundToInt(dmg * (0.5f + 0.5f * STAGE_USERINFO.GetStat(StatType.CriticalDmg)));
				camactype = CamActionType.Battle_Hit_Cri;
			}
		}
		/////즉사류
		//암살자 시너지
		float? synergyAS = STAGE.m_User.GetSynergeValue(JobType.Assassin, 1);
		if (synergyAS != null && UTILE.Get_Random(0f, 1f) < (float)synergyAS && enemyInfo.m_TEnemyData.m_Grade != EEnemyGrade.Boss) {
			dmg = dmg > enemyInfo.GetStat(EEnemyStat.HP) ? dmg : enemyInfo.GetStat(EEnemyStat.HP);
			STAGE_USERINFO.ActivateSynergy(JobType.Assassin);

			CamAction(CamActionType.Battle_Hit_Cri);
			return Mathf.RoundToInt(dmg);
		}

		//헤드샷
		bool headshot = UTILE.Get_Random(0, 100) < STAGE_USERINFO.GetStat(StatType.HeadShot) * (100f - enemyInfo.GetStat(EEnemyStat.HIDING)) / 100f;
		if (headshot) {
			//DNA-OptionType.ExplosiveHeadshot
			if (_usedna && _atker != null)  HeadShotDNAAction(_atker, _enemy);

			dmg = dmg > enemyInfo.GetStat(EEnemyStat.HP) ? dmg : enemyInfo.GetStat(EEnemyStat.HP);
			StartEff(_enemy.transform, "Effect/Stage/Atk/Eff_Duel_Headshot_2");
			if (_atker != null) {
				STAGE_USERINFO.CharSpeech(DialogueConditionType.HeadShot, _atker);
			}
			CamAction(CamActionType.Battle_Hit_Cri);
			return Mathf.RoundToInt(dmg);
		}
		if (_usedna) {
			//DNA 즉사
			bool dnakill = UTILE.Get_Random(0f, 1f) < _atker.m_Info.GetDNABuff(OptionType.AttackingKill);
			if (dnakill) {
				DNAAlarm(_atker.m_Info, OptionType.AttackingKill);
				dmg = dmg > enemyInfo.GetStat(EEnemyStat.HP) ? dmg : enemyInfo.GetStat(EEnemyStat.HP);

				CamAction(CamActionType.Battle_Hit_Cri);
				return Mathf.RoundToInt(dmg);
			}
		}

		CamAction(camactype);
		return Mathf.RoundToInt(dmg * _ratio);
	}

	void GetKillEnemy(Item_Stage _enemy, Item_Stage_Char _atker = null, bool _usedna = true, bool _reducestat = false, bool _dueal = false) {
		if (_reducestat && !_dueal && _enemy.m_Info.m_TEnemyData.m_Tribe == EEnemyTribe.Human && STAGE_USERINFO.Is_UseStat(StatType.Men)) {
			int val = _enemy.m_Info.m_TEnemyData.GetBTReduceStat(StatType.Men, _enemy.m_Info.m_LV);
			if(val > 0) {
				float? synergyLT = STAGE_USERINFO.GetSynergeValue(JobType.Lunatic, 1);
				if (synergyLT != null) 
					STAGE_USERINFO.ActivateSynergy(JobType.Lunatic);
				else
					StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, -val));
			}
		}

		if (_atker != null && _usedna) {
			CharInfo charinfo = _atker.m_Info;
			//DNA 리자드맨 
			DNACheck(_atker, OptionType.KilltoCoolDown);
			//DNA 프랑켄
			DNACheck(_atker, OptionType.KilltoHeal);

			STAGE_USERINFO.CharSpeech(DialogueConditionType.KillEnemy, _atker);
		}

		TEnemyTable enemy = _enemy.m_Info.m_TEnemyData;
		if (!enemy.ISRefugee() || (enemy.ISRefugee() && STAGEINFO.m_TStage.m_Clear.Find(t => t.m_Type == StageClearType.KillEnemy_Type) != null)) {
			m_Check.Check(StageCheckType.KillEnemy, enemy.m_Idx);
			m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemy.m_Type);
			m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemy.m_Tribe);
			m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemy.m_Grade);
		}
		if (enemy.IS_BadRefugee()) {
			m_Check.Check(StageCheckType.Kill_infectee, 0);
		}
		m_OneTurnKillCnt++;
		var turnkill = STAGEINFO.m_TStage.m_Clear.Find(t => t.m_Type == StageClearType.KillEnemy_Turn);
		if (turnkill != null && turnkill.m_Value == m_OneTurnKillCnt) m_Check.Check(StageCheckType.KillEnemy_Turn, m_OneTurnKillCnt);
	}
	/// <summary> 4초 이상 아무 행동 안하면 첫줄 목표에 해당하는 카드 및 캐릭터 강조 </summary>
	public void IdleGoalTouchGuide() {
		if (STAGEINFO.m_Result == StageResult.None && m_CorSkill == null && m_StageAction == null && !POPUP.IS_PopupUI() && m_TouchState == ETouchState.NONE) {
			if(m_IdleTimer < 10f)
				m_IdleTimer += Time.deltaTime / Time.timeScale;
			if(m_IdleTimer >= 10f && m_IdleTouchGuideCor == null) {
				Item_Guide guide = m_MainUI.GetMissionGuideObj.GetComponent<Item_Guide>();
				List<Item_Guide_GoalInfo> infos = guide.GetInfos;
				for (int k = 0; k < (STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.SameTime ? infos.Count : 1); k++) {
					var clear = STAGEINFO.m_TStage.m_Clear[infos[k].m_Pos];

					if (clear.m_Type == StageClearType.UseSkill) {
						for (int i = 0; i < 5; i++) {
							Item_Stage_Char character = m_Chars[i];
							if (character.m_Info == null) continue;
							if (character.IS_UseActiveSkill()) m_IdleChar.Add(character);
						}
					}
				}
				for (int i = 0; i < m_ViewCard.Length; i++) {
					if (m_IdleCard.Find(o => o.m_Line != i) != null) break;
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage card = m_ViewCard[i][j];
						//if (m_IdleCard.Count > 5) break;
						if (card == null || card.IS_Die() || card.m_Info.IsDark || !card.IS_ScreenCard()) continue;

						for (int k = 0; k < (STAGEINFO.m_TStage.m_ClearMethod == ClearMethodType.SameTime ? infos.Count : 1); k++) {
							var clear = STAGEINFO.m_TStage.m_Clear[infos[k].m_Pos];
							switch (clear.m_Type) {
								case StageClearType.Survival:
									break;
								case StageClearType.CardUse:
								case StageClearType.Fire_Card:
								case StageClearType.Fire_Enemy:
									if (clear.m_Value == (int)card.m_Info.m_NowTData.m_Type) m_IdleCard.Add(card);
									break;
								case StageClearType.KillEnemy:
									if (!card.m_Info.IS_EnemyCard) continue;
									if (!card.m_Info.m_TEnemyData.ISRefugee() && (clear.m_Value == 0 ? true : card.m_Info.m_TEnemyData.m_Idx == clear.m_Value)) m_IdleCard.Add(card);
									break;
								case StageClearType.KillEnemy_Type:
									if (!card.m_Info.IS_EnemyCard) continue;
									if ( (clear.m_Value == 0 ? true : (int)card.m_Info.m_TEnemyData.m_Type == clear.m_Value)) m_IdleCard.Add(card);
									break;
								case StageClearType.KillEnemy_Tribe:
									if (!card.m_Info.IS_EnemyCard) continue;
									if ((clear.m_Value == 0 ? true : (int)card.m_Info.m_TEnemyData.m_Tribe == clear.m_Value)) m_IdleCard.Add(card);
									break;
								case StageClearType.KillEnemy_Grade:
									if (!card.m_Info.IS_EnemyCard) continue;
									if ((clear.m_Value == 0 ? true : (int)card.m_Info.m_TEnemyData.m_Grade == clear.m_Value)) m_IdleCard.Add(card);
									break;
								case StageClearType.Rec_Stat:
									if (card.m_Info.IS_Rec_Stat((StatType)clear.m_Value)) m_IdleCard.Add(card);
									break;
								case StageClearType.Rescue_Refugee:
									if (!card.m_Info.IS_EnemyCard) continue;
									if (card.m_Info.ISRefugee && !card.m_Info.m_TEnemyData.IS_BadRefugee()) m_IdleCard.Add(card);
									break;
								case StageClearType.Rescue_Infectee:
								case StageClearType.Kill_infectee:
									if (!card.m_Info.IS_EnemyCard) continue;
									if (card.m_Info.ISRefugee && card.m_Info.m_TEnemyData.IS_BadRefugee() && clear.m_Value == 0 ? true : card.m_Info.m_TEnemyData.m_Idx == clear.m_Value) m_IdleCard.Add(card);
									break;
								case StageClearType.SuppressionF:
									if (card.m_Info.m_NowTData.m_Type == StageCardType.Fire) m_IdleCard.Add(card);
									break;
								case StageClearType.GetBox:
									if (card.m_Info.m_NowTData.m_Type == StageCardType.BigSupplyBox || card.m_Info.m_NowTData.m_Type == StageCardType.SupplyBox || card.m_Info.m_NowTData.m_Type == StageCardType.Supplybox02) m_IdleCard.Add(card);
									break;
								case StageClearType.AnyMaking:
									if (card.m_Info.m_NowTData.m_Type == StageCardType.Material) m_IdleCard.Add(card);
									break;
								case StageClearType.KillEnemy_Turn:
									if (card.m_Info.IS_EnemyCard) m_IdleCard.Add(card);
									break;
							}
						}
					}
				}
				StartCoroutine(m_IdleTouchGuideCor = IE_TouchGuide());
			}
		}
		else {
			m_IdleTimer = 0f;
			if (m_IdleTouchGuideCor != null) InitTouchGuide();
		}
	}
	IEnumerator IE_TouchGuide() {
		if (m_IdleCard == null && m_IdleChar == null) {
			InitTouchGuide();
		}
		else {
			for (int i = 0; i < m_IdleCard.Count; i++) {
				m_IdleCard[i].ActiveTouchGuide(true);
			}
			for (int i = 0; i < m_IdleChar.Count; i++) {
				m_IdleChar[i].ActiveTouchGuide(true);
			}
			yield return new WaitForSeconds(2.5f);
			StartCoroutine(m_IdleTouchGuideCor = IE_TouchGuide());
		}
	}
	void InitTouchGuide() {
		if (m_IdleTouchGuideCor != null) StopCoroutine(m_IdleTouchGuideCor);
		m_IdleTouchGuideCor = null;
		m_IdleTimer = 0f;
		for(int i = 0;i< m_IdleCard.Count; i++) {
			m_IdleCard[i].ActiveTouchGuide(false);
		}
		m_IdleCard.Clear(); 
		for (int i = 0; i < m_IdleChar.Count; i++) {
			m_IdleChar[i].ActiveTouchGuide(false);
		}
		m_IdleChar.Clear();
	}
	public void FirstLineBump(bool _bump) {
		for (int i = 0; i < 3; i++) {
			if (m_ViewCard[0][i] == null || m_ViewCard[0][i].IS_Die()) continue;
			m_ViewCard[0][i].TW_ScaleBumping(_bump);
		}
	}

	public void DNAAlarm(CharInfo _char, OptionType _option) {
		for(int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			if(m_Chars[i].m_Info == _char) {
				m_Chars[i].DNAAlarm(_option);
				break;
			}
		}
	}

	void SetBGFXSort(bool _down) {
		m_BGEffPanel[1].GetComponent<SortingGroup>().sortingOrder = _down ? 3 : -10;
	}
}
