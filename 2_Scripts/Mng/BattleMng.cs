using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum EBattleState
{
	Loading = 0,
	ShowMission,
	GetStartReward,
	Ready,
	Play,
	Result,
	End
}

public enum EUserBattleState
{
	ShowMission = 0,
	GetStartReward,
	Idle,
	Atk,
	Def,
	PowerBattle,
	Reward,
	End
}
public enum EBattleDir
{
	Left = 0,
	Center,
	Right,
	All,
	End
}
public enum EPlayerState
{
	Idle = 0,
	Def,
	End
}

public enum ENoteHitState
{
	MISS = 0,
	PERPECT,
	GOOD,
	End
}

public class BattleMng : ObjMng
{
#if STAGE_TEST
#elif BATTLE_TEST
	[Header("테스트용 연결")]
	public int m_TestMonIdx = 1;
	public int m_TestUserMaxHP = 1000;
	public int m_TestUserAtk = 100;
	public int m_TestUserDef = 100;
	public int m_TestUserShield = 20;
	public int m_TestEnemyLV = 1;
	public int m_TestRecoveryStamina = 10;
#endif

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Instance
	private static BattleMng m_Instance = null;
	public static BattleMng Instance { get { return m_Instance; } }

	[Header("플레용 연결")]
	[SerializeField] SpriteRenderer m_BG;
	[SerializeField] public Transform[] m_NotePanel;
	List<Item_Battle_Note_Base> m_UseNote = new List<Item_Battle_Note_Base>();
	Dictionary<ENoteType, List<Item_Battle_Note_Base>> m_NotePool = new Dictionary<ENoteType, List<Item_Battle_Note_Base>>();
	[SerializeField, ReName(typeof(ENoteType))]
	GameObject[] m_NotePrefab = new GameObject[(int)ENoteType.Random];
	[SerializeField, ReName(typeof(ENoteType))]
	GameObject[] m_DmgPrefab = new GameObject[(int)ENoteType.Random];
	Dictionary<int, Vector2> m_Touch = new Dictionary<int, Vector2>();
#if UNITY_EDITOR
	bool m_MousePressed = false;
#endif
	[SerializeField] Enemy_Controller m_Enemy;
	[SerializeField] Transform m_Bg;
	[SerializeField] AnimationCurve m_ClearTimeScale = AnimationCurve.EaseInOut(0, 0f, 1f, 1f);
	[HideInInspector] public BattleUI m_MainUI;
	

	[HideInInspector] public EBattleState m_State;
	[HideInInspector] public EUserBattleState m_UserState;
	[HideInInspector] public EBattleDir m_MyPos;
	[HideInInspector] public EPlayerState m_DefState;
	[HideInInspector] public long m_GuardCheckID;

	[HideInInspector] public IEnumerator m_Proc;
	[HideInInspector] public IEnumerator m_Eva = null;
	[HideInInspector] public EBattleDir m_EvaCamMoveDir;

	Vector3[] m_CamSize = new Vector3[2];
	public Vector3[] CAM_SIZE { get { return m_CamSize; } }
#pragma warning disable 0649
	[System.Serializable]
	public struct SStaminaColor
	{
		[Tooltip("색상으로 표시될 비율")]
		public Color color;
		[Tooltip("색상으로 표시될 비율")]
		public float rate;
		[Tooltip("스테미나가 차는 속도")]
		public float speed;
	}
	public SStaminaColor[] m_StaminaRateInfo;
#pragma warning restore 0649
	/// <summary> 첫호출인지 </summary>
	bool m_IsFirst;
	/// <summary> 피버모드 </summary>
	bool m_Fiver;
	/// <summary> 맞았는지 </summary>
	bool m_IsEnemyAtk;
	/// <summary> 방어 시도 시간 </summary>
	//double m_DefTime;
	[HideInInspector] public float m_AtkTime = 0f;
	[SerializeField, ReName("유저 회피동작 이동시간", "유저 회피후 대기시간", "유저 회피후 복귀시간"), Tooltip("유저의 회피관련 시간정보")]
	float[] m_EvaTime = new float[3] { 0.3f, 0.3f, 0.3f };
	float[] m_AutoHealDelay = new float[2] { 0f, 0.5f };//타이머,자동힐 간격
	/// <summary> 시간 제한 타이머 </summary>
	public float m_Timer;
	/// <summary> 라운드 수 0:현재 1:최대 </summary>
	[SerializeField] int[] m_Round = new int[2];
	/// <summary> 라운드 체크용 0:적턴 1:유저턴 </summary>
	[SerializeField] bool[] m_Turn = new bool[2];
	/// <summary> 가드 게이지 </summary>
	[SerializeField] int[] m_GuardGauge = new int[2];
	[SerializeField] GameObject m_EvaEff;

	public float GetGuardGauge { get { return (float)m_GuardGauge[0] / (float)m_GuardGauge[1]; } }
	float m_GuardGaugeRecvTimer;

	List<EnemyNoteTable> m_PrePickNote = new List<EnemyNoteTable>();

	public static bool IsValid()
	{
		return m_Instance != null;
	}

	private void Awake()
	{
		m_Instance = this;
#if NOT_USE_NET
#   if STAGE_TEST
		if (MAIN.IS_BackState(MainState.START))
		{
			Debug.LogError("StageTest 씬을 통해서 실행할 것!!!!!!");
			MAIN.Exit();
			return;
		}
#   elif BATTLE_TEST
		if (MAIN.IS_BackState(MainState.START)) TDATA.LoadDefaultTables(-1);
		BATTLEINFO.SetEnemy(m_TestMonIdx, m_TestEnemyLV);
		if(BATTLEINFO.m_EnemyTData == null)
		{
			List<int> list = TDATA.GetEnemyIdxs();
			m_TestMonIdx = list[UTILE.Get_Random(0, list.Count)];
		}
		Init(EBattleMode.EnemyAtk, m_TestMonIdx, m_TestEnemyLV);
#   elif UNITY_EDITOR
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(-1);
			MAIN.StateChange(MainState.TITLE);
		}
#   endif
#elif UNITY_EDITOR
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(-1);
			MAIN.ReStart();
		}
#endif
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}
#if UNITY_EDITOR
	KeyCode defkey;
#endif
	private void Update()
	{
		STAGEINFO.SetRunTime(Time.deltaTime);
		switch (m_State)
		{
		case EBattleState.Play:
				break;
		default: return;
		}
#if UNITY_EDITOR

		if (BATTLEINFO.m_State == EUserBattleState.Def)
		{
			if (Input.GetKeyDown(KeyCode.A)) OnEva(EBattleDir.Left);
			else if (Input.GetKeyDown(KeyCode.D)) OnEva(EBattleDir.Right);
			else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D)) OnEva(EBattleDir.Center);
			else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Space)) OnDef(true);
			else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.Space)) OnDef(false);
		}
#endif

		if(!IsDie())
		{
			//switch(BATTLEINFO.m_State)
			//{
			//case EUserBattleState.Atk:
			//case EUserBattleState.Def:
			// 초당 스테미너 회복 넣기
			StageUser User = BATTLEINFO.m_User;

			if (m_EvaCamMoveDir == EBattleDir.Center && Time.deltaTime > 0) {
				//스테미너 회복
				float rate = User.GetStat(StatType.Sta) / (float)User.GetMaxStat(StatType.Sta);
				float per = Time.deltaTime;
				for (int i = BATTLE.m_StaminaRateInfo.Length - 1; i > -1; i--) {
					if (BATTLE.m_StaminaRateInfo[i].rate < rate) {
						per *= BATTLE.m_StaminaRateInfo[i].speed;
						break;
					}
				}

				float skillmul = 1f;
				switch (BATTLEINFO.m_EnemyTData.m_Type) {
					case EEnemyType.Zombie: skillmul += USERINFO.GetSkillValue(SkillKind.ZomEnergyUp); break;
					case EEnemyType.Animal: skillmul += USERINFO.GetSkillValue(SkillKind.AnimalEnergyUp); break;
					case EEnemyType.Mutant: skillmul += USERINFO.GetSkillValue(SkillKind.MutantEnergyUp); break;
					case EEnemyType.Mafia:
					case EEnemyType.Scavenger:
					case EEnemyType.Zealot:
					case EEnemyType.Gangster:
					case EEnemyType.Wolfs:
					case EEnemyType.SatRefugee:
					case EEnemyType.MenRefugee:
					case EEnemyType.HygRefugee:
					case EEnemyType.HpRefugee:
					case EEnemyType.RandomRefugee:
					case EEnemyType.AllRefugee:
					case EEnemyType.SatInfectee:
					case EEnemyType.MenInfectee:
					case EEnemyType.HygInfectee:
					case EEnemyType.HpInfectee:
					case EEnemyType.RandomInfectee:
					case EEnemyType.Allinfectee:
					case EEnemyType.Npc:
					case EEnemyType.MaterialRefugee:
						skillmul += USERINFO.GetSkillValue(SkillKind.HumanEnergyUp); break;
				}

				switch (BATTLEINFO.m_EnemyTData.m_Grade) {
					case EEnemyGrade.Normal: skillmul += USERINFO.GetSkillValue(SkillKind.NormalEnergyUp); break;
					case EEnemyGrade.Elite: skillmul += USERINFO.GetSkillValue(SkillKind.EliteEnergyUp); break;
					case EEnemyGrade.Boss: skillmul += USERINFO.GetSkillValue(SkillKind.BossEnergyUp); break;
				}
				User.m_Stat[(int)StatType.Sta, 0] = Mathf.Min(User.m_Stat[(int)StatType.Sta, 0] + User.GetStat(StatType.RecSta) * skillmul * per, User.GetMaxStat(StatType.Sta));
				m_MainUI.SetStamina();

				////가드 게이지 회복
				//if (m_GuardGauge[0] < m_GuardGauge[1]) {
				//	m_GuardGaugeRecvTimer += Time.deltaTime;
				//	if (m_GuardGaugeRecvTimer >= 0.1f) {
				//		m_GuardGauge[0] += BaseValue.BATTLE_GUARD_AUTORECV_TICK;
				//		m_MainUI.SetGuardGauge(0.1f);
				//		m_GuardGaugeRecvTimer -= 0.1f;
				//	}
				//}
			}
			//else m_GuardGaugeRecvTimer = 0;

			//0.5초당 파티 회복력만큼 체력 회복
			if (m_AutoHealDelay[0] >= m_AutoHealDelay[1]) {
				int prehp = Mathf.RoundToInt(User.m_Stat[(int)StatType.HP, 0]);
				if (prehp < Mathf.RoundToInt(User.m_Stat[(int)StatType.HP, 1])) {
					float value = User.GetStat(StatType.Heal) * BaseValue.HPRECV_TICKRATIO(USERINFO.GetDifficulty());
					//청결도에 따른 디버프
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusHpRecovery)) {
						value -= value * STAGE_USERINFO.m_DebuffValues[DebuffType.MinusHpRecovery];
					}

					User.m_Stat[(int)StatType.HP, 0] = Mathf.Clamp(User.m_Stat[(int)StatType.HP, 0] + value, 0, User.GetMaxStat(StatType.HP));
					if(!m_MainUI.Is_GetDmg) m_MainUI.SetUserHP(false, prehp);
				}
				m_AutoHealDelay[0] -= m_AutoHealDelay[1];
			}
			else m_AutoHealDelay[0] += Time.deltaTime;
			//    break;
			//}
		}
		else {
			BATTLEINFO.m_Result = EBattleResult.LOSE;
			GameOver();
		}
		TouchCheck();

		//타임어택
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time && STAGEINFO.m_Result == StageResult.None && m_State == EBattleState.Play && !POPUP.IS_PopupUI()) {
			m_Timer += Time.deltaTime;
			float time = STAGEINFO.m_TStage.m_Fail.m_Value;
			if (time > 0) {
				//시너지
				float? synergy2 = BATTLEINFO.m_User.GetSynergeValue(JobType.Scientist, 1);
				if (synergy2 != null) {
					time = time + (float)synergy2;
					if (STAGE_USERINFO.m_SynergyUseCnt[JobType.Scientist] == 0) STAGE_USERINFO.ActivateSynergy(JobType.Scientist);

					Utile_Class.DebugLog_Value("Scientist 시너지 발동 " + "변화 전 -> 후 : 전 :" + STAGEINFO.m_TStage.m_Fail.m_Value.ToString() + " 후 : " + time.ToString());
					//STAGE.m_User.m_SynergyUseCnt[JobType.Scientist]++;
				}

				//TimePlus 버프카드 적용
				time = time * (1f + STAGE_USERINFO.GetBuffValue(StageCardType.TimePlus));

				
				DLGTINFO?.f_RFModeTimer?.Invoke(Math.Max(0, time - m_Timer), true);
				//m_MainUI.RefreshTimer(Math.Max(0, time - m_Timer));
				int presec = Mathf.FloorToInt(time - (m_Timer - Time.deltaTime));
				int sec = Mathf.FloorToInt(time - m_Timer);
				if (presec != sec && sec <= 10) PlayEffSound(SND_IDX.SFX_0495);
				if (STAGEINFO.m_Check.IsFail() == StageFailKind.Time) {
					BATTLEINFO.m_Result = EBattleResult.LOSE;
					GameOver();
				}
			}
		}
	}

	public void Init(EBattleMode battleMode, int EnemyIdx, int LV, int EnemyHP = 0, System.Action EndCB = null, bool isFirst = false, bool _viewresult = true)
	{
		for (int i = m_UseNote.Count - 1; i > -1; i--) m_UseNote[i].NoteEnd();

		m_GuardCheckID = 0;
		m_IsFirst = isFirst;
		if (m_IsFirst) {
			BATTLEINFO.m_ContinuityCnt = 0;
			m_Timer = 0f;
		}
		BATTLEINFO.m_Result = EBattleResult.NONE;
		BATTLEINFO.m_BattleMode = battleMode;
		BATTLEINFO.m_ViewResult = _viewresult;
		if (STAGEINFO.m_BG != null)  m_BG.sprite = STAGEINFO.m_BG;
		CameraViewSizeInfo CamSize = Utile_Class.GetViewWorldSizeInfo(Camera.main);

		m_CamSize[0] = CamSize.origin[0] + CamSize.direction[0] / CamSize.direction[0].z * Mathf.Abs(CamSize.origin[0].z);
		m_CamSize[1] = CamSize.origin[1] + CamSize.direction[1] / CamSize.direction[1].z * Mathf.Abs(CamSize.origin[1].z);

		m_Round[0] = 0;
		m_Round[1] = STAGEINFO.m_StageContentType == StageContentType.Bank ? 0 : TDATA.GetEnemyTable(EnemyIdx).m_BattleRoundLimit;

		m_GuardGauge[0] = m_GuardGauge[1] = BaseValue.BATTLE_GUARD_MAX;

		BATTLEINFO.m_BattleEndCB = EndCB;
		BATTLEINFO.SetEnemy(EnemyIdx, LV);
		BATTLEINFO.SetData();
		if(EnemyHP > 0) BATTLEINFO.m_EnemyHP = EnemyHP;

		if(BATTLEINFO.m_EnemyTData.m_Grade == EEnemyGrade.Boss) PlayBGSound(SND_IDX.BGM_0502);
		else PlayBGSound(SND_IDX.BGM_0500);

		Time.timeScale = 1f;
		if (m_MainUI == null) m_MainUI = (BattleUI)POPUP.Set_Popup(PopupPos.BATTLE, PopupName.BattleUI);
		m_MainUI.Init();
		m_MainUI.SetRound(m_Round[0] + 1, m_Round[1]);
		StopAllCoroutines();

		m_NotePool.Clear();
		for (int i = m_NotePanel[1].childCount - 1; i > -1; i--) GameObject.Destroy(m_NotePanel[1].GetChild(i).gameObject);
		for (int i = m_NotePanel[0].childCount - 1; i > -1; i--) GameObject.Destroy(m_NotePanel[0].GetChild(i).gameObject);

		if (m_Enemy != null) GameObject.Destroy(m_Enemy.gameObject);
		m_Enemy = UTILE.LoadPrefab(string.Format("Enemy/{0}", BATTLEINFO.m_EnemyTData.GetNotePrefabName()), true).GetComponent<Enemy_Controller>();
		m_Enemy.SetData(BATTLEINFO.m_EnemyIdx);
		m_Enemy.transform.position = Vector3.zero;

		m_MyPos = EBattleDir.Center;
		m_DefState = EPlayerState.Idle;

		switch (STAGEINFO.m_StageModeType)
		{
		case StageModeType.NoteBattle:
			if (m_IsFirst) StateChange(EBattleState.ShowMission);
			else
			{
				StateChange(EBattleState.Ready);
			}
			break;
		default:
			StateChange(EBattleState.Ready);
			break;
		}
	}
	IEnumerator m_PlayStateAction;
	public void StateChange(EBattleState state)
	{
		if (m_PlayStateAction != null)
		{
			StopCoroutine(m_PlayStateAction);
			m_PlayStateAction = null;
		}
		m_State = state;
		switch (state)
		{
		case EBattleState.ShowMission:
			m_PlayStateAction = Play_ShowMission();
			break;
		case EBattleState.GetStartReward:
			m_PlayStateAction = Play_GetStartMission();
			break;
		case EBattleState.Ready:
			m_PlayStateAction = Play_Ready();
			break;
		case EBattleState.Result:
			UserStateChange(EUserBattleState.End);
			break;
		}
		if(m_PlayStateAction != null)   StartCoroutine(m_PlayStateAction);
	}

	public void UserStateChange(EUserBattleState state, params object[] args)
	{
		if(m_Proc != null)
		{
			StopCoroutine(m_Proc);
			m_Proc = null;
		}

		if (m_State > EBattleState.Play) return;
		switch (state)
		{
		case EUserBattleState.Atk:
			m_EvaCamMoveDir = EBattleDir.Center;
			OnDef(false);
			break;
		}
		BATTLEINFO.m_State = state;
		float time;
		switch (state)
		{
		case EUserBattleState.Atk:
			OnEva(EBattleDir.Center);
			m_Fiver = false;
			m_AtkTime = 0;
			time = BATTLEINFO.m_EnemyTData.GetDefTime();
			m_Proc = Play_AttackTurn(time);
			m_Touch.Clear();
			StartCoroutine(Play_AttackTImer(time));
			break;
		case EUserBattleState.Def:
			m_Fiver = false;
			OnDef(false);
			m_Proc = Play_DefTurn();
			break;
		}
		if(m_MainUI != null) m_MainUI.SetState(BATTLEINFO.m_State, args);
		if (m_Proc != null) StartCoroutine(m_Proc);
	}

	public Enemy_Controller GetEnemy()
	{
		return m_Enemy;
	}
	void TouchCheck()
	{
		switch(m_State)
		{
		case EBattleState.Play:
			if (BATTLEINFO.m_State != EUserBattleState.Atk) return;
			break;
		default: return;
		}
		int id = 0;
		Vector2 touchpos;
#if UNITY_EDITOR
		touchpos = Input.mousePosition;
		if (Input.GetMouseButtonDown(id))
		{
			m_MousePressed = true;
			OnPressed(id, touchpos);
		}
		else if (m_MousePressed && Input.GetMouseButton(id))
		{
			OnMoved(id, touchpos);
		}
		else if (m_MousePressed && Input.GetMouseButtonUp(id))
		{
			OnReleased(id, touchpos);
		}
#else
		for (int i = 0; i < Input.touchCount; i++)
		{
			Touch tc = Input.GetTouch(i);
			id = tc.fingerId;
			touchpos = tc.position;
			if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
				OnPressed(id, touchpos);
			}
			else if (Input.GetTouch(i).phase == TouchPhase.Moved)
			{
				OnMoved(id, touchpos);
			}
			else if (Input.GetTouch(i).phase == TouchPhase.Ended)
			{
				OnReleased(id, touchpos);
			}
		}
#endif
	}



	void OnPressed(int id, Vector2 pos)
	{
		if (m_Touch.ContainsKey(id)) m_Touch.Remove(id);
		m_Touch.Add(id, pos);
		for (int i = 0; i < m_NotePanel[1].childCount; i++)
		{
			Item_Battle_Note_Base note = m_NotePanel[1].GetChild(i).GetComponent<Item_Battle_Note_Base>();
			if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Note, ETouchState.PRESS, note, i)) continue;
			if (note.OnPressed(id, pos)) break;
		}
	}

	void OnMoved(int id, Vector2 pos)
	{
		if(!m_Touch.ContainsKey(id)) m_Touch.Add(id, pos);
		for (int i = 0; i < m_NotePanel[1].childCount; i++)
		{
			Item_Battle_Note_Base note = m_NotePanel[1].GetChild(i).GetComponent<Item_Battle_Note_Base>();
			if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Note, ETouchState.MOVE, note, i)) continue;
			if (note.OnMoved(id, pos)) break;
		}
	}

	void OnReleased(int id, Vector2 pos)
	{
		bool check = false;
		for (int i = 0; i < m_NotePanel[1].childCount; i++)
		{
			if(m_Touch.ContainsKey(id))
			{
				Item_Battle_Note_Base note = m_NotePanel[1].GetChild(i).GetComponent<Item_Battle_Note_Base>();
				if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Note, ETouchState.END, note, i)) continue;
				if (note.OnSliced(m_Touch[id], pos))
				{
					check = true;
					break;
				}
			}
		}


		if (!check)
		{
			for (int i = 0; i < m_NotePanel[1].childCount; i++)
			{
				Item_Battle_Note_Base note = m_NotePanel[1].GetChild(i).GetComponent<Item_Battle_Note_Base>();
				if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Note, ETouchState.END, note, i)) continue;
				if (note.OnReleased(id, pos)) break;
			}
		}
		if (m_Touch.ContainsKey(id)) m_Touch.Remove(id);
	}


	public void EnemyDamage(int Damage)
	{
		if (BATTLEINFO.m_EnemyHP < 1) return;
#if ONE_ATK_MON_DIE
		BATTLEINFO.m_EnemyHP -= BATTLEINFO.m_EnemyHP;
#else
		BATTLEINFO.m_EnemyHP -= Damage;
#endif
		if (BATTLEINFO.m_EnemyHP < 1) {
			PlayEffSound(m_Enemy.GetSnd());
			BATTLEINFO.m_EnemyHP = 0;
			BATTLEINFO.m_Result = EBattleResult.WIN;

			TEnemyTable enemy = BATTLEINFO.m_EnemyTData;
			switch (STAGEINFO.m_StageModeType) {
				case StageModeType.NoteBattle:
					STAGEINFO.m_Check.Check(StageCheckType.KillEnemy, enemy.m_Idx);
					STAGEINFO.m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemy.m_Type);
					STAGEINFO.m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemy.m_Tribe);
					STAGEINFO.m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemy.m_Grade);
					break;
			}
			m_Enemy.StartAni(EEnemyAni.Dead, m_IsFirst ? 1f : 4f);
			ParticleSystem.MainModule particlemouble = m_Enemy.m_DeadFX.main;
			particlemouble.simulationSpeed *= m_IsFirst ? 1f : 4f;

			ClearGame();
		}
		else {
			if(UTILE.Get_Random(0, 100) < 30) PlayEffSound(m_Enemy.GetSnd());
			m_Enemy.StartAni(EEnemyAni.Hit);
		}
		m_MainUI.SetEnemyHP();
	}

	public void UserDamage(int Damage, EnemySkillTable skill = null, bool call_gameover = true)
	{
		StageUser User = BATTLEINFO.m_User;
		if (IsDie()) return;
		int dmg = Damage;

		int prehp = Mathf.RoundToInt(User.m_Stat[(int)StatType.HP, 0]);
		User.m_Stat[(int)StatType.HP, 0] = Mathf.Clamp(User.m_Stat[(int)StatType.HP, 0] - dmg, 0, User.GetMaxStat(StatType.HP));
		m_MainUI.SetUserHP(true, prehp);
		if (IsDie())
		{
			BATTLEINFO.m_Result = EBattleResult.LOSE;
			if (call_gameover) GameOver();
		}
	}

	public void SetSrvDmg(StatType stat, int dmg)
	{
		StageUser user = BATTLEINFO.m_User;
		if (!user.Is_UseStat(stat)) return;
		float preval = user.GetStat(stat);
		user.m_Stat[(int)stat, 0] = Mathf.Clamp(user.m_Stat[(int)stat, 0] + dmg, 0, user.GetMaxStat(stat));

		switch (stat)
		{
		case StatType.Men:
		case StatType.Hyg:
		case StatType.Sat:
			DLGTINFO.f_RfStatUI?.Invoke(stat, user.GetStat(stat), preval, user.GetMaxStat(stat));
			break;
		}
	}

	bool IsDieCheck(StatType stat)
	{
		StageUser User = BATTLEINFO.m_User;
		if (!User.Is_UseStat(stat)) return false;

		return User.GetStat(stat) < 1;
	}

	public bool IsDie()
	{
		return IsDieCheck(StatType.HP) || IsDieCheck(StatType.Men) || IsDieCheck(StatType.Hyg) || IsDieCheck(StatType.Sat);
	}



	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Mission
	IEnumerator Play_ShowMission()
	{
		UserStateChange(EUserBattleState.ShowMission);
		m_Enemy.StartAni(EEnemyAni.Init);

		GameObject obj = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Mission, null, STAGEINFO.m_Idx).gameObject;
		yield return new WaitUntil(() => obj == null);

		StateChange(EBattleState.GetStartReward);
	}
	IEnumerator Play_GetStartMission() {
		UserStateChange(EUserBattleState.GetStartReward);
		if (STAGEINFO.m_TStage.m_StartReward != 0) {
			GameObject obj = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_StartReward, null, STAGEINFO.m_TStage.m_StartReward).gameObject;
			yield return new WaitUntil(() => obj == null);
		}
		StateChange(EBattleState.Ready);
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Ready
	IEnumerator Play_Ready()
	{
		UserStateChange(EUserBattleState.Idle, m_IsFirst ? 1f : 4f);
		m_Enemy.StartAni(m_IsFirst ? EEnemyAni.Start : EEnemyAni.Start_Skip, STAGEINFO.m_StageContentType == StageContentType.Bank && m_IsFirst ? 4f : 1f);
		if (STAGEINFO.m_StageContentType == StageContentType.Bank) m_MainUI.SetKillCount(STAGEINFO.m_Check.m_KillEnemyCnt, Mathf.RoundToInt(STAGEINFO.m_TStage.m_Clear.Find(t => t.m_Type == StageClearType.KillEnemy).m_Cnt));

		yield return new WaitWhile(() => m_Enemy.ISAniPlay());
		yield return new WaitWhile(() => m_MainUI.IS_Action());

		StateChange(EBattleState.Play);
		UserStateChange(BATTLEINFO.GetFirstAtkType());
		m_MainUI.StartRoundCenter(m_Round[1]);
		m_PlayStateAction = null;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 회복 관련
	public void AddStat(StatType type, int value, bool UseCheck = true)
	{
		BATTLEINFO.m_User.AddStat(type, value);
		if (UseCheck && STAGE_USERINFO.Is_UseStat(type)) {
			if (value > 0) PlayAddStatSnd(type);
			if (value < 0) PlayEffSound(SND_IDX.SFX_0470);
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 버프 관련
	public void SetBuff(EStageBuffKind kind, int idx) {
		TStageCardTable data = TDATA.GetStageCardTable(idx);
		StageCardType type = data.m_Type;
		float befor = 0;
		// 스텟 MAX 증가는 현재 수치도 같이 증가 시켜준다.
		// 클리어 조건에 문제가 되지 않도록 Stage에서 올려준다.
		switch (type) {
			/// <summary> HP 회복 </summary>
			case StageCardType.HpUp:
				befor = BATTLEINFO.m_User.GetMaxStat(StatType.HP);
				break;
			/// <summary> 포만도 증가 </summary>
			case StageCardType.SatUp:
				befor = BATTLEINFO.m_User.GetMaxStat(StatType.Sat);
				break;
			/// <summary> 청결도 증가 </summary>
			case StageCardType.HygUp:
				befor = BATTLEINFO.m_User.GetMaxStat(StatType.Hyg);
				break;
			/// <summary> 정신력 증가 </summary>
			case StageCardType.MenUp:
				befor = BATTLEINFO.m_User.GetMaxStat(StatType.Men);
				break;
		}

		float prebuffval = BATTLEINFO.m_User.GetBuffValue(type);
		BATTLEINFO.m_User.SetBuff(kind, idx);

		switch (type) {
			/// <summary> HP 회복 </summary>
			case StageCardType.HpUp:
				int prehp = Mathf.RoundToInt(BATTLEINFO.m_User.GetStat(StatType.HP));
				AddStat(StatType.HP, Mathf.RoundToInt(BATTLEINFO.m_User.GetMaxStat(StatType.HP) - befor), false);
				m_MainUI.SetUserHP(true, prehp);
				break;
			/// <summary> 포만도 증가 </summary>
			case StageCardType.SatUp:
				AddStat(StatType.Sat, Mathf.RoundToInt(BATTLEINFO.m_User.GetMaxStat(StatType.Sat) - befor), false);
				break;
			/// <summary> 청결도 증가 </summary>
			case StageCardType.HygUp:
				AddStat(StatType.Hyg, Mathf.RoundToInt(BATTLEINFO.m_User.GetMaxStat(StatType.Hyg) - befor), false);
				break;
			/// <summary> 정신력 증가 </summary>
			case StageCardType.MenUp:
				AddStat(StatType.Men, Mathf.RoundToInt(BATTLEINFO.m_User.GetMaxStat(StatType.Men) - befor), false);
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
				if (prebuffval < BATTLEINFO.m_User.GetBuffValue(type)) {
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
				else if (prebuffval > BATTLEINFO.m_User.GetBuffValue(type)) {
					switch (type) {
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
				break;
		}
	}
	public IEnumerator SelectAction_StageCardProc_Gamble(TGambleCardTable _table, float _prop) {
		//확률대로 뽑고
		TGambleCardTable table = _table;
		float randprop = _prop;
		int val = 0;
		//확률로 성공실패 뽑고
		TStageCardTable resultcardtable = TDATA.GetStageCardTable(table.m_ResultIdx[1 - table.m_SuccProp > randprop ? 0 : 1]);

		//스탯으로 버프 디버프 체크
		//팝업으로 주사위 굴림 및 결과 보여주고
		yield return new WaitForEndOfFrame();
		//팝업 콜백으로 결과값 적용
		switch (resultcardtable.m_Type) {
			case StageCardType.HpUp:
			case StageCardType.AtkUp:
			case StageCardType.DefUp:
			case StageCardType.EnergyUp:
			case StageCardType.SatUp:
			case StageCardType.HygUp:
			case StageCardType.MenUp:
			case StageCardType.SpeedUp:
			case StageCardType.CriticalUp:
			case StageCardType.CriticalDmgUp:
			case StageCardType.APRecoveryUp:
			case StageCardType.APConsumDown:
			case StageCardType.HealUp:
			case StageCardType.LevelUp:
			case StageCardType.TimePlus:
			case StageCardType.HeadShotUp:
				SetBuff(EStageBuffKind.Stage, resultcardtable.m_Idx);
				if (resultcardtable.m_Type == StageCardType.LevelUp) STAGE_USERINFO.StatReset();
				break;
			case StageCardType.Synergy:
				SetBuff(EStageBuffKind.Synergy, (int)STAGE_USERINFO.CreateSynergy());
				break;
			case StageCardType.RecoveryHp:
				AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryHpPer:
				AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryMen:
				AddStat(StatType.Men, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryHyg:
				AddStat(StatType.Hyg, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.RecoverySat:
				AddStat(StatType.Sat, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.PerRecoveryMen:
				AddStat(StatType.Men, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * resultcardtable.m_Value1));
				break;
			case StageCardType.PerRecoveryHyg:
				AddStat(StatType.Hyg, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * resultcardtable.m_Value1));
				break;
			case StageCardType.PerRecoverySat:
				AddStat(StatType.Sat, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * resultcardtable.m_Value1));
				break;
			case StageCardType.AddGuard:
				val = Mathf.RoundToInt(resultcardtable.m_Value1);
				STAGE_USERINFO.m_Stat[(int)StatType.Guard, 0] += (float)val;
				if (val > 0) PlayEffSound(SND_IDX.SFX_0461);
				else if(val < 0) PlayEffSound(SND_IDX.SFX_0472);
				break;
			case StageCardType.AddRerollCount:
				val = Mathf.RoundToInt(resultcardtable.m_Value1);
				STAGE_USERINFO.m_leftReRollCnt += val;
				if (val > 0) PlayEffSound(SND_IDX.SFX_0462);
				else if (val < 0) PlayEffSound(SND_IDX.SFX_0470);
				break;
		}
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 데미지 관련
	public Transform SetDamage(ENoteHitState state, ENoteType mode, ENoteSize size, int damage, float rate, Vector3 v3Pos, EnemySkillTable skill = null, TEnemyLevelTable lvdata = null)
	{
		Item_Damage eff = Utile_Class.Instantiate(m_DmgPrefab[(int)mode]).GetComponent<Item_Damage>();
		eff.SetData(state, damage, rate);
		if(damage != 0 && skill != null) {
			float mul = m_DefState == EPlayerState.Def ? 1f - BaseValue.BATTLE_DEF_DMG_RATIO : 1f;
			SetSrvDmgFX(eff, StatType.Men, Mathf.RoundToInt(skill.Get_SrvDmg(StatType.Men) * lvdata.GetStat(EEnemyStat.ATKSURVSTAT) * mul));
			SetSrvDmgFX(eff, StatType.Hyg, Mathf.RoundToInt(skill.Get_SrvDmg(StatType.Hyg) * lvdata.GetStat(EEnemyStat.ATKSURVSTAT) * mul));
			SetSrvDmgFX(eff, StatType.Sat, Mathf.RoundToInt(skill.Get_SrvDmg(StatType.Sat) * lvdata.GetStat(EEnemyStat.ATKSURVSTAT) * mul));
		}
		eff.SetSize(size);
		eff.transform.position = v3Pos;
		eff.GetComponent<SortingGroup>().sortingOrder = 4;

		return eff.transform;
	}

	public void SetSrvDmgFX(Item_Damage eff, StatType stat, int dmg)
	{
		if (!BATTLEINFO.m_User.Is_UseStat(stat)) return;
		if(dmg < 0) eff.SetSrvDmg(stat, dmg);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 종료 타임 스케일
	IEnumerator ClearSlowAction(Action _cb)
	{
		float maxtime = m_ClearTimeScale.keys[m_ClearTimeScale.length - 1].time;
		float time = 0f;
		while (time < maxtime)
		{
			Time.timeScale = m_ClearTimeScale.Evaluate(time / maxtime);
			yield return new WaitForEndOfFrame();
			time += Time.unscaledDeltaTime;
		}
		Time.timeScale = 1f;
		_cb?.Invoke();
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Attack
	void SetFiver()
	{
		m_Fiver = true;
	}
	IEnumerator Play_AttackTImer(float MaxTime)
	{
		m_AtkTime = 0f;
		while (m_Fiver || m_AtkTime < MaxTime)
		{
			yield return new WaitForFixedUpdate();
			if (!m_Fiver) m_AtkTime += Time.fixedDeltaTime;
		}

		yield return new WaitWhile(() => m_NotePanel[1].childCount > 0);

		//플레이어 턴 종료 후 포만감 감소
		int satdec = UTILE.Get_Random(BATTLEINFO.m_EnemyTData.m_NoteSatDecMinMax[0], BATTLEINFO.m_EnemyTData.m_NoteSatDecMinMax[1]);
		AddStat(StatType.Sat, -satdec);

		yield return new WaitForSeconds(0.5f);

		CheckRound(1);
		UserStateChange(EUserBattleState.Def);
	}
	int m_NoteCreateCnt;
	IEnumerator Play_AttackTurn(float MaxTime)
	{
		m_NoteCreateCnt = 0;
		yield return new WaitForFixedUpdate();
		while (m_AtkTime < MaxTime && m_State == EBattleState.Play)
		{
			for (int i = BATTLEINFO.m_EnemyTData.GetCreateNoteCnt() - 1; i > -1; i--)
				yield return CreateNote();
			if (m_Fiver) yield return new WaitForSeconds(0.1f);
			else
			{
				float NextTime = BATTLEINFO.m_EnemyTData.GetNextNoteTime();
				yield return new WaitForSeconds(NextTime);
			}
		}

		yield return new WaitForSeconds(1.5f);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Defence
	IEnumerator Play_DefTurn() {
		for (int i = m_UseNote.Count - 1; i > -1; i--) m_UseNote[i].OnClear();

		TEnemyTable tdata = BATTLEINFO.m_EnemyTData;
		int AtkCnt = tdata.GetAtkCnt();
		int DefCnt = 0;

		while (m_State == EBattleState.Play && DefCnt < AtkCnt)
		{
			yield return new WaitWhile(() => m_NotePanel[1].childCount > 0);
			int atktype = m_Enemy.GetAtkType();
			EnemySkillTable skill = BATTLEINFO.GetSkill(atktype);

			//적 공격 시점에 회피 버튼 나오고 회피나 방어 선택까지 대기
			OnDef(false);
			m_MainUI.SetBtnAction(true, skill.m_DefTime);
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => m_MainUI.IS_Eva());

			switch (skill.m_Type)
			{
			case EAtkType.Catch:
				yield return MonAtkCatch(atktype, DefCnt);
				break;
			default:
					yield return MonAtkHitType(atktype, DefCnt);
				break;
			}
			DefCnt++;

			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => !m_IsEnemyAtk);

			if (m_DefState == EPlayerState.Def) {//방어시 피격 직전에 이펙트 터짐
				yield return new WaitForSeconds(m_EvaTime[0]);
			}
			else {//회피 방향 정해진 후 피격 직전에 움직임
				m_Eva = MoveCam(skill);
				yield return m_Eva;
			}
			yield return new WaitForSeconds(BATTLEINFO.m_EnemyTData.GetNextAtkTime());
		}
		
		yield return new WaitWhile(() => m_Enemy.IS_AtkPlay());

		yield return new WaitForSeconds(0.5f);
		CheckRound(0);
		yield return new WaitForSeconds(1f);

		UserStateChange(EUserBattleState.Atk);
	}
	/// <summary> 턴 끝날때 마다 라운드 체크 </summary>
	void CheckRound(int _pos) {
		m_Turn[_pos] = true;
		if (m_Turn[0] == m_Turn[1] == true && m_Round[1] > 0) {
			m_Turn[0] = m_Turn[1] = false;
			m_Round[0]++;
			if (m_Round[0] == m_Round[1]) {
				UserDamage(Mathf.RoundToInt(BATTLEINFO.m_User.GetStat(StatType.HP)));
			}
			else {
				m_MainUI.SetRound(m_Round[0] + 1, m_Round[1]);
				m_MainUI.StartRoundCenter(m_Round[1]);
			}
		}
	}
	/// <summary> 몬스터 공격 성공 체크 </summary>
	/// <returns> 0 : 실패, 1 : 성공, 2 : 방어 </returns>
	public int HitCheck(Enemy_AtkInfo atkinfo)
	{
		int nRe = 0;
		if (m_DefState == EPlayerState.Def)
		{
			nRe = 2;
		}
		else
		{
			switch (atkinfo.m_Pos)
			{
			case EBattleDir.Left:
				switch (m_MyPos)
				{
				case EBattleDir.Left:
					nRe = 1;
					break;
				}
				break;
			case EBattleDir.Center:
				if (m_MyPos == EBattleDir.Center) nRe = 1;
				break;
			case EBattleDir.Right:
				switch (m_MyPos)
				{
				case EBattleDir.Right:
					nRe = 1;
					break;
				}
				break;
			case EBattleDir.All:
				nRe = 1;
				break;
			}
		}
		return nRe;
	}

	IEnumerator MonAtkHitType(int atktype, int createNo)
	{
		EnemySkillTable skill = BATTLEINFO.GetSkill(atktype);
		int atkCnt = skill.GetCnt();
		int hitCnt = 0;
		int suvCnt = 0;
		List<Enemy_AtkInfo> activeatk = new List<Enemy_AtkInfo>();
		// 3번 호출될것이므로 공격은 1번만
		for (int i = 0; i < atkCnt; i++)
		{
			m_IsEnemyAtk = false;
			Enemy_AtkInfo atkinfo;
			atkinfo = m_Enemy.Create_AtkInfo(atktype);

			atkinfo.IsGuardCntCheck = i == 0;
			activeatk.Add(atkinfo);
			m_Enemy.SetAtk(atkinfo, (info) => {
				activeatk.Remove(atkinfo);
				m_Enemy.PlayVoice(1);
				hitCnt++;
				int hit = HitCheck(atkinfo);
				m_Enemy.AtkActionStart(atkinfo);
				switch (hit)
				{
					case 0:
						SetEvaEff(info.m_EffPos);
						break;
					case 1:
					case 2:
						int Damage = BATTLEINFO.GetDamage(BATTLEINFO.GetEnemyAtk(), BATTLEINFO.m_EnemyLV, BATTLEINFO.GetDef());
						if (hit == 2)
						{
							m_MainUI.StartGuardEff();
							Damage = Mathf.RoundToInt(Damage * (1f - BaseValue.BATTLE_DEF_DMG_RATIO));
						}
						//맞았을때 회피위치에 따른 피해량 차이
						//else if (atkinfo.m_Pos == m_MyPos) Damage <<= 1;

						//if (info.IsGuardCntCheck && hit == 2) UseGuard();

						if (Damage > 0) {
							SetHit(atkinfo.m_Pos);
							if (UTILE.Get_Random(0, 100) < 30) {
								TCharacterTable tdata = USERINFO.GetChar(USERINFO.m_PlayDeck.m_Char[UTILE.Get_Random(0, STAGEINFO.m_TStage.m_DeckCharLimit)]).m_TData;
								SND_IDX hitvocidx = tdata.GetHitVoice();
								PlayEffSound(hitvocidx);
							}
						}

						Transform dmgui = SetDamage(ENoteHitState.GOOD, ENoteType.Normal, skill.m_Size, Damage, 1f, info.m_EffPos, skill, BATTLEINFO.m_EnemyLvTData);
						dmgui.localScale *= 2f;
						UserDamage(Damage, skill);

						if (skill != null && Damage > 0) suvCnt++;
						break;
				}
			});
		}
		yield return new WaitWhile(() => activeatk.Count > 0);

		if (suvCnt > 0) {
			float mul = m_DefState == EPlayerState.Def ? 1f - BaseValue.BATTLE_DEF_DMG_RATIO : 1f;
			for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
				SetSrvDmg(i, Mathf.RoundToInt(skill.Get_SrvDmg(i) * BATTLEINFO.m_EnemyLvTData.GetStat(EEnemyStat.ATKSURVSTAT) * mul) * suvCnt);
			}
		}

		m_IsEnemyAtk = true;
	}

	IEnumerator MonAtkCatch(int atktype, int createNo)
	{
		Enemy_AtkInfo atkinfo = m_Enemy.Create_AtkInfo(atktype);
		bool is_hit = false;
		m_IsEnemyAtk = false;
		m_Enemy.SetAtk(atkinfo, (info) =>
		{
			if (IsDie()) return;
			int hit = HitCheck(atkinfo);
			if (hit != 0)
			{
				m_Enemy.AtkActionStart(atkinfo);
				UserStateChange(EUserBattleState.PowerBattle, atkinfo);
			}
			is_hit = hit != 0;
			m_IsEnemyAtk = true;
		});
		if (!is_hit) yield break;//잡히는 모션 타이밍 때문에 탈출

		yield return new WaitWhile(() => m_Enemy.IS_AtkPlay());
		yield return new WaitWhile(() => m_MainUI.ISPB());
	}

	void GameOver() {
		StopAllCoroutines();
		m_PrePickNote.Clear();
		StateChange(EBattleState.Result);
		m_MainUI.GameOver();
	}

	public void ClearGame() {
		m_PrePickNote.Clear();
		StartCoroutine(ClearSlowAction(()=> {
			StopAllCoroutines();
		}));
		StateChange(EBattleState.Result);
		for (int i = m_UseNote.Count - 1; i > -1; i--) m_UseNote[i].OnClear();
		m_MainUI.Clear();
	}

	EnemyNoteTable GetNote(List<EnemyNoteTable> _prepick = null)
	{
		TEnemyTable tdata = BATTLEINFO.m_EnemyTData;
		EnemyNoteTable notetable = null;
		while (notetable == null)
		{
		EnemyNoteTable temp = tdata.GetNote(_prepick);
			// Chain Note는 같은게 있을때 생성하면 안된다.
			// 중복 노트 생성도 안되야 하니 남은게 1개인데 체인일 때도 예외처리한다.
			if (temp.m_Type == ENoteType.Chain && m_UseNote.Find((item) => item.m_Mode == ENoteType.Chain)) {
				var ndatas = tdata.m_Note.GetNotOverrideNotTables(_prepick);
				if (ndatas.Count == 1 && ndatas[0].m_Type == ENoteType.Chain) return null;
				else continue;
			}
			notetable = temp;
		}
		return notetable;
	}

	IEnumerator CreateNote()
	{
		int value = 0;
		TEnemyTable tdata = BATTLEINFO.m_EnemyTData;
		EnemyNoteTable notetable = GetNote(m_PrePickNote);
		if (notetable == null) yield break;
		m_PrePickNote.Add(notetable);
		switch (notetable.m_Type)
		{
		case ENoteType.Combo:
			value = notetable.GetCreateCnt();
			break;
		case ENoteType.Chain:
			yield return CreateChainNote(notetable);
			yield break;
		case ENoteType.Fixing:
			yield return CreateFixingNote(notetable);
			yield break;
		}
		Item_Battle_Note_Base note = null;
		// 확률 체크
		int bodyMax = m_Enemy.m_Body.Count;
		Item_Battle_Note_Base tempNote = GetPoolNote(notetable.m_Type);
		tempNote.SetSize(notetable.m_Size);
		float size = tempNote.GetSize();
		Vector3[] side = { new Vector3(-size, -size, 0), new Vector3(size, size, 0)};

		while (note == null)
		{
			int rand = UTILE.Get_Random(0, tdata.m_BodyProbMax);
			for (int i = 0; i < bodyMax; i++)
			{
				EBodyType bodytype = m_Enemy.m_Body[i].Type;
				int prob = tdata.m_Body[(int)bodytype].Prob;
				if (rand < prob)
				{
					Vector3 pos = m_Enemy.GetRandomPos(bodytype);
					if (!CheckScreenPos(side, pos)) break;
					if (m_Fiver || CheckNotePos(pos))
					{
						note = tempNote;
						note.transform.position = pos;
						note.SetData(bodytype, notetable.m_EndTime, value, m_NoteCreateCnt);
						note.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
						note.SetNoteNo(m_NoteCreateCnt);
						m_NoteCreateCnt++;
					}
					break;
				}
				rand -= prob;
			}
			// 한프레임 대기(대기를 하지않으면 무한 루프가 됨)
			if (note == null) yield return null;
		}
		
		if (note != null)
		{
			note.transform.SetParent(m_NotePanel[1]);
			m_UseNote.Add(note);
		}
	}

	IEnumerator CreateChainNote(EnemyNoteTable notetable)
	{
		TEnemyTable tdata = BATTLEINFO.m_EnemyTData;
		int Cnt = UTILE.Get_Random(notetable.m_Cnt[0], notetable.m_Cnt[1] + 1);
		int bodyMax = m_Enemy.m_Body.Count;

		List<Item_Battle_Note_Chain> chainlist = new List<Item_Battle_Note_Chain>();
		float nextDelay = 0f;
		for (int i = 0; i < Cnt; i++)
		{
			yield return new WaitForSeconds(nextDelay);
			Item_Battle_Note_Chain note = null;
			Item_Battle_Note_Chain tempNote = (Item_Battle_Note_Chain)GetPoolNote(notetable.m_Type);
			tempNote.SetSize(notetable.m_Size);
			float size = tempNote.GetSize();
			Vector3[] side = { new Vector3(-size, -size, 0), new Vector3(size, size, 0) };

			while (note == null)
			{
				int rand = UTILE.Get_Random(0, tdata.m_BodyProbMax);
				for (int j = 0; j < bodyMax; j++)
				{
					EBodyType bodytype = m_Enemy.m_Body[j].Type;
					int prob = tdata.m_Body[(int)bodytype].Prob;
					if (rand < prob)
					{
						Vector3 pos = m_Enemy.GetRandomPos(bodytype);
						if (!CheckScreenPos(side, pos)) break;
						if (m_Fiver || CheckNotePos(pos))
						{
							note = tempNote;
							note.transform.position = pos;
							note.SetData(bodytype, notetable.m_EndTime, i, Cnt);
							yield return null;
							note.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
							nextDelay = note.m_NextCreateDelay;
							m_NoteCreateCnt++;
						}
						break;
					}
					rand -= prob;
				}
				if (note == null) yield return null;
			}
			if (note != null)
			{
				note.transform.SetParent(m_NotePanel[1]);
				m_UseNote.Add(note);
				chainlist.Add(note);
				if (i > 0) {
					chainlist[i - 1].SetNextChain(note);
					note.SetBeforChain(chainlist[i - 1].IS_Clear() ? null : chainlist[i - 1]);//다음 노트 생성 전에 이전 노트가 클리어되면 넥스트는 null
				}
			}

			bool premiss = false;
			for (int j = 0; j < chainlist.Count; j++) {
				if (j > 0 && chainlist[j].m_ISMissNode) {
					premiss = true;
					break;
				}
			}
			if (premiss) yield break;
		}
	}
	/// <summary> Z형태로 7개 노멀, 1개 3콤보</summary>
	IEnumerator CreateFixingNote_1(EnemyNoteTable notetable) {
		Vector3[] pos = new Vector3[8] { new Vector3(-2,3,0), new Vector3(0,3,0), new Vector3(2, 3, 0), new Vector3(0.8f, 0.8f, 0)
			, new Vector3(-0.8f, -0.8f, 0) , new Vector3(-2,-3,0), new Vector3(0,-3,0), new Vector3(2,-3,0)};
		int[] notecnt = new int[2] { 7, 1 };
		float[] notedelay = new float[2] { 0.8f, 1.5f };

		//일반노트7개
		for (int i = 0; i < notecnt[0]; i++, m_NoteCreateCnt++) {
			Item_Battle_Note_Normal normalnote = (Item_Battle_Note_Normal)GetPoolNote(ENoteType.Normal);
			normalnote.SetSize(notetable.m_Size);
			normalnote.transform.position = pos[i];
			normalnote.SetData(EBodyType.Body, notedelay[0], 0, m_NoteCreateCnt);
			normalnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
			normalnote.transform.SetParent(m_NotePanel[1]);
			m_UseNote.Add(normalnote);
			yield return new WaitForSeconds(notedelay[0] - 0.3f);
		}
		//yield return new WaitForSeconds(notedelay[1]);
		//콤보노트 1개
		Item_Battle_Note_Combo combonote = (Item_Battle_Note_Combo)GetPoolNote(ENoteType.Combo);
		combonote.SetSize(ENoteSize.Large);
		combonote.transform.position = pos[7];
		combonote.SetData(EBodyType.Body, notedelay[1], 3, m_NoteCreateCnt);
		combonote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
		combonote.transform.SetParent(m_NotePanel[1]);
		m_UseNote.Add(combonote);
	}
	/// <summary> G형태로 10개 체인 </summary>
	IEnumerator CreateFixingNote_2(EnemyNoteTable notetable) {//EnemyNoteTable notetable
		Vector3[] pos = new Vector3[10] { new Vector3(2.2f,0,0), new Vector3(1.9f,1.9f,0), new Vector3(0, 3, 0), new Vector3(-1.9f, 1.9f, 0)
			, new Vector3(-2.2f, 0, 0) , new Vector3(-1.75f,-1.8f,0), new Vector3(-0.7f,-3,0), new Vector3(0.7f,-3,0), new Vector3(1.75f,-1.8f,0), new Vector3(0,0,0)};
		int notecnt = 10;
		float notedelay = 0.7f;

		List<Item_Battle_Note_Chain> chainlist = new List<Item_Battle_Note_Chain>();
		for (int i = 0; i < notecnt; i++, m_NoteCreateCnt++) {
			yield return new WaitForSeconds(0.2f);
			Item_Battle_Note_Chain chainnote = (Item_Battle_Note_Chain)GetPoolNote(ENoteType.Chain);
			chainnote.SetSize(notetable.m_Size);
			chainnote.transform.position = pos[i];
			chainnote.SetData(EBodyType.Body, notedelay, i, notecnt);
			chainnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
			chainnote.transform.SetParent(m_NotePanel[1]);
			m_UseNote.Add(chainnote);
			chainlist.Add(chainnote);
			if (i > 0) {
				chainlist[i - 1].SetNextChain(chainnote);
				chainnote.SetBeforChain(chainlist[i - 1].IS_Clear() ? null : chainlist[i - 1]);//다음 노트 생성 전에 이전 노트가 클리어되면 넥스트는 null
			}
		}
	}
	/// <summary> 역슬래시형 슬래시3, 홀드1 </summary>
	IEnumerator CreateFixingNote_3(EnemyNoteTable notetable) {
		Vector3[] pos = new Vector3[4] { new Vector3(-1.5f,3,0), new Vector3(-0.5f,1.27f,0), new Vector3(0.5f, -0.46f, 0), new Vector3(1.5f, -2.19f, 0)};
		int[] notecnt = new int[2] { 3, 1 };
		float[] notedelay = new float[2] { 1f, 2f };

		//슬래시 노트 3개
		for (int i = 0; i < notecnt[0]; i++, m_NoteCreateCnt++) {
			Item_Battle_Note_Slice slashnote = (Item_Battle_Note_Slice)GetPoolNote(ENoteType.Slash);
			slashnote.SetSize(notetable.m_Size);
			slashnote.transform.position = pos[i];
			slashnote.SetData(EBodyType.Body, notedelay[0], 0, m_NoteCreateCnt);
			slashnote.m_RotatePanel.eulerAngles = new Vector3(0, 0, 120f);
			slashnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
			slashnote.transform.SetParent(m_NotePanel[1]);
			m_UseNote.Add(slashnote);
			yield return new WaitForSeconds(notedelay[0] - 0.3f);
		}
		//챠지 노트 1개
		Item_Battle_Note_Charge chargenote = (Item_Battle_Note_Charge)GetPoolNote(ENoteType.Charge);
		chargenote.SetSize(notetable.m_Size);
		chargenote.transform.position = pos[3];
		chargenote.SetData(EBodyType.Body, notedelay[1], 0, m_NoteCreateCnt);
		chargenote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
		chargenote.transform.SetParent(m_NotePanel[1]);
		m_UseNote.Add(chargenote);
	}
	/// <summary> 역슬래시형 노멀4, 콤보1 </summary>
	IEnumerator CreateFixingNote_4(EnemyNoteTable notetable) {
		Vector3[] pos = new Vector3[5] { new Vector3(-2f, 3, 0), new Vector3(-1f, 1.5f, 0), new Vector3(0, 0f, 0), new Vector3(1f, -1.5f, 0), new Vector3(2f, -3f, 0) };
		int[] notecnt = new int[2] { 4, 1 };
		float[] notedelay = new float[2] { 0.8f, 1.5f };

		//슬래시 노트 3개
		for (int i = 0; i < notecnt[0]; i++, m_NoteCreateCnt++) {
			Item_Battle_Note_Normal normalnote = (Item_Battle_Note_Normal)GetPoolNote(ENoteType.Normal);
			normalnote.SetSize(notetable.m_Size);
			normalnote.transform.position = pos[i];
			normalnote.SetData(EBodyType.Body, notedelay[0], 0, m_NoteCreateCnt);
			normalnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
			normalnote.transform.SetParent(m_NotePanel[1]);
			m_UseNote.Add(normalnote);
			yield return new WaitForSeconds(notedelay[0]);
		}
		//콤보 노트 1개
		Item_Battle_Note_Combo combonote = (Item_Battle_Note_Combo)GetPoolNote(ENoteType.Combo);
		combonote.SetSize(notetable.m_Size);
		combonote.transform.position = pos[4];
		combonote.SetData(EBodyType.Body, notedelay[1], 3, m_NoteCreateCnt);
		combonote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
		combonote.transform.SetParent(m_NotePanel[1]);
		m_UseNote.Add(combonote);
	} 
	/// <summary> 오망성형 콤보5, 챠지1 </summary>
	IEnumerator CreateFixingNote_5(EnemyNoteTable notetable) {
		Vector3[] pos = new Vector3[6] { new Vector3(1.5f, 2.5f, 0), new Vector3(-2.5f, 0, 0), new Vector3(2.5f, 0, 0), new Vector3(-1.5f, 2.5f, 0), new Vector3(0f, -2.5f, 0), new Vector3(1.5f, 2.5f, 0) };
		int[] notecnt = new int[2] { 5, 1 };
		float[] notedelay = new float[2] { 1f, 2f };

		//콤보 노트 4개
		for (int i = 0; i < notecnt[0] + notecnt[1]; i++, m_NoteCreateCnt++) {
			if (i != 2) {
				Item_Battle_Note_Combo combonote = (Item_Battle_Note_Combo)GetPoolNote(ENoteType.Combo);
				combonote.SetSize(notetable.m_Size);
				combonote.transform.position = pos[i];
				combonote.SetData(EBodyType.Body, notedelay[0], 3, m_NoteCreateCnt);
				combonote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
				combonote.transform.SetParent(m_NotePanel[1]);
				m_UseNote.Add(combonote);
				yield return new WaitForSeconds(notedelay[0] - 0.3f);
			}
			else {
				Item_Battle_Note_Charge chargenote = (Item_Battle_Note_Charge)GetPoolNote(ENoteType.Charge);
				chargenote.SetSize(notetable.m_Size);
				chargenote.transform.position = pos[2];
				chargenote.SetData(EBodyType.Body, notedelay[1], 0, m_NoteCreateCnt);
				chargenote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
				chargenote.transform.SetParent(m_NotePanel[1]);
				m_UseNote.Add(chargenote); 
				yield return new WaitForSeconds(notedelay[1] - 0.3f);
			}
		}
	}
	//고정 노트 자동화
	IEnumerator CreateFixingNote(EnemyNoteTable notetable) {
		List<TEnemyNoteGroupTable> fixingtables = TDATA.GetEnemyNoteGroupTable(notetable.m_Cnt[0]);
		List<List<Item_Battle_Note_Chain>> chainlist = new List<List<Item_Battle_Note_Chain>>();
		chainlist.Add(new List<Item_Battle_Note_Chain>());
		int num = 0;

		ENoteType prenote = ENoteType.Chain;
		for (int i = 0;i< fixingtables.Count; i++, m_NoteCreateCnt++) {
			switch (fixingtables[i].m_NoteType) {
				case ENoteType.Normal:
					Item_Battle_Note_Normal normalnote = (Item_Battle_Note_Normal)GetPoolNote(ENoteType.Normal);
					normalnote.SetSize(fixingtables[i].m_Size);
					normalnote.transform.position = fixingtables[i].GetPos();
					normalnote.SetData(EBodyType.Body, fixingtables[i].m_EndTime, 0, m_NoteCreateCnt);
					normalnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
					normalnote.transform.SetParent(m_NotePanel[1]);
					m_UseNote.Add(normalnote);
					break;
				case ENoteType.Combo:
					Item_Battle_Note_Combo combonote = (Item_Battle_Note_Combo)GetPoolNote(ENoteType.Combo);
					combonote.SetSize(fixingtables[i].m_Size);
					combonote.transform.position = fixingtables[i].GetPos();
					combonote.SetData(EBodyType.Body, fixingtables[i].m_EndTime, fixingtables[i].m_Cnt, m_NoteCreateCnt);
					combonote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
					combonote.transform.SetParent(m_NotePanel[1]);
					m_UseNote.Add(combonote);
					break;
				case ENoteType.Charge:
					Item_Battle_Note_Charge chargenote = (Item_Battle_Note_Charge)GetPoolNote(ENoteType.Charge);
					chargenote.SetSize(fixingtables[i].m_Size);
					chargenote.transform.position = fixingtables[i].GetPos();
					chargenote.SetData(EBodyType.Body, fixingtables[i].m_EndTime, 0, m_NoteCreateCnt);
					chargenote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
					chargenote.transform.SetParent(m_NotePanel[1]);
					m_UseNote.Add(chargenote);
					break;
				case ENoteType.Chain:
					if(prenote != ENoteType.Chain) {//이전 노트가 체인이 아니면 체인 스탭 올림
						chainlist.Add(new List<Item_Battle_Note_Chain>());
						num = 0;
					}

					bool premiss = false;
					for (int j = 0; j < chainlist[chainlist.Count - 1].Count; j++) {
						if (j > 0 && chainlist[chainlist.Count - 1][j].m_ISMissNode) {
							premiss = true;
							break;
						}
					}
					if (premiss) yield break;
					else {
						Item_Battle_Note_Chain chainnote = (Item_Battle_Note_Chain)GetPoolNote(ENoteType.Chain);
						chainnote.SetSize(fixingtables[i].m_Size);
						chainnote.transform.position = fixingtables[i].GetPos();
						chainnote.SetData(EBodyType.Body, fixingtables[i].m_EndTime, num, fixingtables[i].m_Cnt);
						chainnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
						chainnote.transform.SetParent(m_NotePanel[1]);
						m_UseNote.Add(chainnote);
						chainlist[chainlist.Count - 1].Add(chainnote);
						if (num > 0) {
							chainlist[chainlist.Count - 1][num - 1].SetNextChain(chainnote);
							chainnote.SetBeforChain(chainlist[chainlist.Count - 1][num - 1].IS_Clear() ? null : chainlist[chainlist.Count - 1][num - 1]);//다음 노트 생성 전에 이전 노트가 클리어되면 넥스트는 null
						}
						num++;
					}
					break;
				case ENoteType.Slash:
					Item_Battle_Note_Slice slashnote = (Item_Battle_Note_Slice)GetPoolNote(ENoteType.Slash);
					slashnote.SetSize(fixingtables[i].m_Size);
					slashnote.transform.position = fixingtables[i].GetPos();
					slashnote.SetData(EBodyType.Body, fixingtables[i].m_EndTime, 0, m_NoteCreateCnt);
					slashnote.m_RotatePanel.eulerAngles = new Vector3(0, 0, fixingtables[i].m_RotZ);
					slashnote.GetComponent<SortingGroup>().sortingOrder = -m_NoteCreateCnt;
					slashnote.transform.SetParent(m_NotePanel[1]);
					m_UseNote.Add(slashnote);
					break;
			}
			prenote = fixingtables[i].m_NoteType;
			yield return new WaitForSeconds(fixingtables[i].m_Delay);
		}
	}
	bool CheckScreenPos(Vector3[] size, Vector3 pos)
	{
		Vector3 min = pos + size[0];
		Vector3 max = pos + size[1];
		//노트 생성 위치 제한
		return min.x >= m_CamSize[0].x && max.x <= m_CamSize[1].x && min.y >= m_CamSize[0].y * 0.75f && max.y <= m_CamSize[1].y * 0.8f;
	}
	bool CheckNotePos(Vector3 v3Pos)
	{
		for (int i = m_NotePanel[1].childCount - 1; i > -1; i--)
		{
			Item_Battle_Note_Base note = m_NotePanel[1].GetChild(i).GetComponent<Item_Battle_Note_Base>();
			float dis = note.GetRound();
			if (Vector3.Distance(v3Pos, m_NotePanel[1].GetChild(i).position) <= dis) return false;
		}
		return true;
	}

	Item_Battle_Note_Base GetPoolNote(ENoteType mode)
	{
		Item_Battle_Note_Base item;
		if(mode == ENoteType.Random) mode = UTILE.Get_Random<ENoteType>(ENoteType.Normal, ENoteType.Random); 
		if (!m_NotePool.ContainsKey(mode) || m_NotePool[mode].Count < 1) item = Utile_Class.Instantiate(m_NotePrefab[(int)mode], m_NotePanel[0]).GetComponent<Item_Battle_Note_Base>();
		else
		{
			List<Item_Battle_Note_Base> list = m_NotePool[mode];
			item = list[0];
			list.Remove(item);
		}
		return item;
	}

	public void RemoveNote(Item_Battle_Note_Base note)
	{
		// 카드 pool 이동
		note.transform.SetParent(m_NotePanel[0]);
		m_UseNote.Remove(note);
		if (!m_NotePool.ContainsKey(note.m_Mode)) m_NotePool.Add(note.m_Mode, new List<Item_Battle_Note_Base>());
		m_NotePool[note.m_Mode].Add(note);
	}

	public void OnDef(bool Active)
	{
		if (BATTLEINFO.m_State != EUserBattleState.Def) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Def)) return;
		if (Active)
		{
			if (m_Eva != null) return;
			if (BATTLEINFO.GetShildCnt() < 1) return;
			//if (m_GuardGauge[0] < m_GuardGauge[1]) return;
			//m_DefTime = UTILE.Get_Time();
			m_DefState = EPlayerState.Def;
			m_GuardCheckID++;
			//m_GuardGauge[0] = 0;
			//m_MainUI.SetGuardGauge(0.1f);
		}
		else m_DefState = EPlayerState.Idle;
	}

	public void UseGuard()
	{
		BATTLEINFO.UseShild();
		m_MainUI.SetDefCnt();
		if (BATTLEINFO.GetShildCnt() < 1) OnDef(false);
	}

	public bool OnEva(EBattleDir Dir)
	{
		if (Dir == EBattleDir.Center)
		{
			m_EvaCamMoveDir = Dir;
			m_MyPos = m_EvaCamMoveDir;
			return true;
		}
		if (BATTLEINFO.m_State != EUserBattleState.Def) return false;
		if (BATTLEINFO.m_User.GetStat(StatType.Sta) < BATTLEINFO.USE_EVA_STAMINA_VALUE()) {
			m_MainUI.SetEvaBtnAnim((int)Dir, "Lock_Touch");
			return false;
		}
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Eva, Dir)) return false;

		if (m_Eva != null)
		{
			iTween.StopByName(Camera.main.gameObject, "EvaMove");
			StopCoroutine(m_Eva);
			m_Eva = null;
		}
		m_EvaCamMoveDir = Dir;
		m_MyPos = m_EvaCamMoveDir;
		return true;
	}

	IEnumerator MoveCam(EnemySkillTable _skill)
	{
		Transform tfCam = Camera.main.transform;
		float endtime = Mathf.Min(_skill.m_SignTime[0] * 0.5f, m_EvaTime[0]);
		// 중간지점부터 이동으로 판정
		StageUser User = BATTLEINFO.m_User;
		if(m_MyPos != EBattleDir.Center) User.m_Stat[(int)StatType.Sta, 0] = Mathf.Max(0, User.m_Stat[(int)StatType.Sta, 0] - BATTLEINFO.USE_EVA_STAMINA_VALUE());
		m_MainUI.SetStamina();
		iTween.MoveTo(tfCam.gameObject, iTween.Hash("position", BaseValue.GetBattleDirPos(m_MyPos, BaseValue.BATTLE_EVA_DIS, -10f), "time", endtime, "easetype", "easeInOutSine", "name", "EvaMove"));

		yield return new WaitForSeconds(endtime);

		iTween.MoveTo(Camera.main.gameObject, iTween.Hash("position", BaseValue.GetBattleDirPos(EBattleDir.Center, BaseValue.BATTLE_EVA_DIS, -10f), "time", m_EvaTime[2], "easetype", "easeInOutSine", "name", "EvaMove"));
		yield return new WaitForSeconds(m_EvaTime[2]);
		OnEva(EBattleDir.Center);
		m_Eva = null;
	}
	void SetHit(EBattleDir dir)
	{
		m_MainUI.SetHit(dir);
		StartCoroutine(ShakeBG());
	}

	IEnumerator ShakeBG()
	{
		m_Bg.position = new Vector3(0f, 0f, 0f);
		int cnt = 2;
		while(cnt > 0)
		{
			yield return new WaitForEndOfFrame();
			m_Bg.position = new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
			yield return new WaitForEndOfFrame();
			m_Bg.position = new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
			yield return new WaitForEndOfFrame();
			m_Bg.position = new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
			yield return new WaitForEndOfFrame();
			m_Bg.position = new Vector3(UTILE.Get_Random(-0.5f, 0.5f), UTILE.Get_Random(-0.5f, 0.5f), 0f);
			yield return new WaitForEndOfFrame();
			cnt--;
		}
		m_Bg.position = new Vector3(0f, 0f, 0f);
	}

	void SetEvaEff(Vector3 _pos) {
		Transform fx = Utile_Class.Instantiate(m_EvaEff).transform;
		fx.position = _pos;
		fx.localScale = Vector3.one * 0.3f;
		fx.GetComponent<Item_Battle_Dodge>().SetData();
	}
	public void CheatClear() {
		StopAllCoroutines();
		m_State = EBattleState.Result;
	}
}
