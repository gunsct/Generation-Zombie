using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattleUI : PopupBase
{
#pragma warning disable 0649

	[System.Serializable]
	struct SStart {
		public GameObject Active;
		public Animator Ani;
	}
	[System.Serializable]
	struct SHP {
		public Image Befor;
		public Image Now;
		public TextMeshProUGUI HP;
	}
	[System.Serializable]
	struct SStaminaColor {
		public Color color;
		public float rate;
	}
	[System.Serializable]
	struct SUser {
		//public SHP HP;
		public GameObject HPObj;
		public Item_Survival SurvStats;
		public Image[] Stamina;
		public Animator Ani;
	}

	[System.Serializable]
	struct SEnemy {
		public SHP HP;
		public TextMeshProUGUI Name;
	}
	[Serializable]
	struct SEva {
		public Animator Ani;
		public Image Timer;
		public Image GuardGauge;
		public GameObject[] Cards;
	}
	[Serializable]
	struct SRound
	{
		public GameObject CountGroup;
		public GameObject CenterAlarm;
		public TextMeshProUGUI Count;
		public TextMeshProUGUI CenterCount;
	}
	[Serializable]
	struct SKill
	{
		public GameObject CountGroup;
		public TextMeshProUGUI Count;
	}
	[System.Serializable]
	struct SMainUI {
		public GameObject Active;
		public Animator Ani;
		public SUser User;
		public SEnemy Enemy;

		public TextMeshProUGUI DefCnt;
		public Image Hit;
		public Animator Btns;
		public SEva m_EvaBtn;
		public Item_SpeechBubble Speech;
		public GameObject TimerObj;
		public SRound m_Round;
		public SKill m_Kill;
		public GameObject PauseBtn;
		public Image PauseBtnIcon;
		public Sprite[] PauseBtnSprites;
		[Header("튜토리얼용")]
		public GameObject[] Panels;
	}
	[ReName("Start Panel"), SerializeField] SStart m_StartUI;
	[ReName("BattleMain Panel"), SerializeField] SMainUI m_MainUI;
	[ReName("Guard Eff"), SerializeField] GameObject m_Guard;
	Animator m_ResultAni;
	[ReName("Power Battle Panel"), SerializeField] PowerBattle m_PB;
	[ReName("Fail Panel"), SerializeField] GameObject m_Fail;
	[ReName("Fadein Panel"), SerializeField] GameObject m_Fade;
	[ReName("Battle Reward Panel"), SerializeField] BattleReward m_BR;

	public bool Is_GetDmg;
	public GameObject GetStatObj(StatType _type) { return m_MainUI.User.SurvStats.StatObj(_type); }
	public GameObject GetHpObj { get { return m_MainUI.User.HPObj; } }
	public GameObject GetTimerObj { get { return m_MainUI.TimerObj; } }
#pragma warning restore 0649

	private void Awake() {
		m_ResultAni = GetComponent<Animator>();
		m_MainUI.m_Round.CenterAlarm.SetActive(false);
		m_MainUI.m_Round.CountGroup.SetActive(false);
		m_MainUI.m_Kill.CountGroup.SetActive(false);
	}

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		Init();
		base.SetData(pos, popup, cb, aobjValue);
	}

	public void Init() {
		StopCoroutine("GuardEffCheck");
		m_Guard.SetActive(false);
		StopCoroutine("PWBattle");
		m_Fail.SetActive(false);
		m_Fade.SetActive(false);
		m_PB.gameObject.SetActive(false);
		m_BR.gameObject.SetActive(false);
		m_MainUI.Hit.color = new Color(1f, 1f, 1f, 0f);
		m_MainUI.Btns.gameObject.SetActive(false);
		m_MainUI.Active.GetComponent<CanvasGroup>().alpha = 1f;
		Is_GetDmg = false;
		//m_MainUI.PauseBtn.SetActive(STAGEINFO.m_StageModeType != StageModeType.None);
		m_MainUI.m_Kill.CountGroup.SetActive(STAGEINFO.m_StageContentType == StageContentType.Bank);
		SetUI();
	}

	public bool ISPB() {
		return m_PB.gameObject.activeSelf;
	}

	public override void SetUI() {
		base.SetUI();
		SetUserHP();
		SetEnemyHP();
		SetDefCnt();
		//m_MainUI.User.HP.Befor.localScale = m_MainUI.User.HP.Now.localScale;
		//m_MainUI.Enemy.HP.Befor.localScale = m_MainUI.Enemy.HP.Now.localScale;
		m_MainUI.Enemy.Name.text = string.Format("Lv{0} {1}", BATTLEINFO.m_EnemyLV, BATTLEINFO.m_EnemyTData.GetName());
		SetStamina();
		//SetGuardGauge();

		TStageTable table = STAGEINFO.m_TStage;
		for (int i = 0; i < 3; i++) {
			StatType type = StatType.Men + i;

			if (!STAGE_USERINFO.Is_UseStat(type)) m_MainUI.User.SurvStats.StatOff((int)type);
			else DLGTINFO.f_RfStatUI?.Invoke(type, STAGE_USERINFO.GetStat(type), STAGE_USERINFO.GetStat(type), STAGE_USERINFO.GetMaxStat(type));
		}
		m_MainUI.User.SurvStats.SetTrans();

		//타이머
		m_MainUI.TimerObj.SetActive(STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time);
		if (STAGEINFO.m_TStage.m_Fail.m_Type == StageFailType.Time) DLGTINFO?.f_RFModeTimer?.Invoke(STAGEINFO.m_TStage.m_Fail.m_Value, true);
	}

	public void SetStamina() {
		StageUser User = BATTLEINFO.m_User;
		float sta = User.GetStat(StatType.Sta);
		bool EvaLock = sta < BATTLEINFO.USE_EVA_STAMINA_VALUE();
		//TODO:스테이나에 따른 비활성처리 애니메이션 추가
		if (IS_Eva()) {
			for (int i = 0; i < 3; i++) {
				SetEvaBtnAnim(i, EvaLock ? "Locked" : "Normal");
			}
		}
		float rate = User.GetStat(StatType.Sta) / (float)User.GetMaxStat(StatType.Sta);

		for (int i = BATTLE.m_StaminaRateInfo.Length - 1; i > -1; i--) {
			if (BATTLE.m_StaminaRateInfo[i].rate < rate) {
				m_MainUI.User.Stamina[1].color = BATTLE.m_StaminaRateInfo[i].color;
				break;
			}
		}


		iTween.StopByName(gameObject, "StaminaAction");
		if (rate < m_MainUI.User.Stamina[1].fillAmount)
			iTween.ValueTo(gameObject, iTween.Hash("from", m_MainUI.User.Stamina[0].fillAmount, "to", rate, "time", 0.5f, "delay", 0.5f, "onupdate", "StaminaAction", "name", "StaminaAction"));
		else m_MainUI.User.Stamina[0].fillAmount = rate;

		m_MainUI.User.Stamina[1].fillAmount = rate;
	}
	void StaminaAction(float value) {
		m_MainUI.User.Stamina[0].fillAmount = value;
	}
	public void SetGuardGauge(float _time = 0f) {
		iTween.StopByName(gameObject, "GaugeAction");
		iTween.ValueTo(gameObject, iTween.Hash("from", m_MainUI.m_EvaBtn.GuardGauge.fillAmount, "to", BATTLE.GetGuardGauge, "time", _time, "onupdate", "TW_GaugeAction", "name", "GaugeAction"));
	}
	void TW_GaugeAction(float _amount) {
		m_MainUI.m_EvaBtn.GuardGauge.fillAmount = _amount;
	}
	public void SetDefCnt() {
		m_MainUI.DefCnt.text = BATTLEINFO.GetShildCnt().ToString();
	}

	public void StartGuardEff() {
		StopCoroutine("GuardEffCheck");
		if (m_Guard.activeSelf) m_Guard.SetActive(false);

		m_Guard.SetActive(true);
		StartCoroutine("GuardEffCheck");
	}

	IEnumerator GuardEffCheck() {
		yield return new WaitForSeconds(0.3f);
		m_Guard.SetActive(false);
	}

	public Vector2 GetEvabtnPos(EBattleDir dir) {
		switch (dir) {
			case EBattleDir.Left: return m_MainUI.m_EvaBtn.Cards[0].transform.position;
			case EBattleDir.Center: return m_MainUI.m_EvaBtn.Cards[1].transform.position;
			case EBattleDir.Right: return m_MainUI.m_EvaBtn.Cards[2].transform.position;
		}
		return Vector2.zero;
	}

	public GameObject GetEvabtn(EBattleDir dir) {
		switch (dir) {
			case EBattleDir.Left: return m_MainUI.m_EvaBtn.Cards[0];
			case EBattleDir.Center: return m_MainUI.m_EvaBtn.Cards[1];
			case EBattleDir.Right: return m_MainUI.m_EvaBtn.Cards[2];
		}
		return null;
	}

	public Vector2 GetDefBtnPos() {
		return m_MainUI.Panels[1].transform.position;
	}

	public GameObject GetDefBtn() {
		return m_MainUI.Panels[0];
	}

	public void SetHit(EBattleDir dir) {
		if (m_IsAniTransition != null) return;
		if (Utile_Class.IsAniPlay(m_MainUI.User.Ani)) return;
		switch (dir) {
			case EBattleDir.Left:
				m_MainUI.User.Ani.SetTrigger("Left");
				break;
			case EBattleDir.Right:
				m_MainUI.User.Ani.SetTrigger("Right");
				break;
			default:
				m_MainUI.User.Ani.SetTrigger("Center");
				break;
		}
		m_IsAniTransition = AniInTransitionCheck();
		StartCoroutine(m_IsAniTransition);
		StartCoroutine(OnHit());
	}
	IEnumerator m_IsAniTransition;
	IEnumerator AniInTransitionCheck() {
		// 애니가 시작된후에는 한프레임 쉬어주어야함
		yield return new WaitForFixedUpdate();
		m_IsAniTransition = null;
	}

	IEnumerator OnHit() {
		m_MainUI.Hit.color = new Color(1f, 1f, 1f, 1f);
		float maxtime = 1f;
		float CurTime = maxtime;
		while (CurTime > 0f) {
			yield return new WaitForFixedUpdate();
			m_MainUI.Hit.color = new Color(1f, 1f, 1f, CurTime / maxtime);
			CurTime -= Time.fixedDeltaTime;
		}
		m_MainUI.Hit.color = new Color(1f, 1f, 1f, 0f);
	}

	public void SetUserHP(bool _atk = false, int _prehp = 0) {
		int MaxHP = Mathf.RoundToInt(BATTLEINFO.m_User.GetMaxStat(StatType.HP));
		int HP = Mathf.RoundToInt(BATTLEINFO.m_User.GetStat(StatType.HP));
		if (_prehp == 0) _prehp = HP;
		DLGTINFO?.f_RfHPUI?.Invoke(HP, _prehp, MaxHP, () => { Is_GetDmg = false; });
		DLGTINFO?.f_RfHPLowUI?.Invoke(HP < MaxHP * 0.3f);
		if(_atk) Is_GetDmg = _atk;
	}

	public void SetEnemyHP() {
		int MaxHP = BATTLEINFO.m_EnemyHPMax;
		float amount = Mathf.Max(0f, (float)BATTLEINFO.m_EnemyHP / (float)MaxHP);
		m_MainUI.Enemy.HP.Now.fillAmount = amount;

		iTween.StopByName(gameObject, "HPTail");
		iTween.ValueTo(gameObject, iTween.Hash("from", m_MainUI.Enemy.HP.Befor.fillAmount, "to", amount, "onupdate", "TW_EnemyHPTail", "time", 0.5f, "delay", 0.2f, "easetype", "easeInOutQuad", "name", "HPTail"));
	}
	void TW_EnemyHPTail(float _amount) {
		m_MainUI.Enemy.HP.Befor.fillAmount = _amount;
	}
	IEnumerator m_PlayAction;

	public void SetState(EUserBattleState state, params object[] args) {
		if (m_PlayAction != null) {
			StopCoroutine(m_PlayAction);
			m_PlayAction = null;
		}
		switch (state) {
			case EUserBattleState.ShowMission:
				m_PlayAction = PlayMission();
				break;
			case EUserBattleState.Idle:
				m_PlayAction = PlayStart((float)args[0]);
				break;
			case EUserBattleState.Atk:
				m_PlayAction = BtnAction(false);
				m_PB.gameObject.SetActive(false);
				break;
			case EUserBattleState.Def:
				//m_PlayAction = BtnAction(true);
				m_PB.gameObject.SetActive(false);
				break;
			case EUserBattleState.PowerBattle:
				m_PlayAction = BtnAction(false);
				m_PB.gameObject.SetActive(true);
				m_PB.SetData(PopupPos.NONE, PopupName.PowerBattle, (result, obj) => {
					if (result == 1) BATTLE.UserStateChange(EUserBattleState.Atk);
				}, args);
				break;
		}
		if (m_PlayAction != null) StartCoroutine(m_PlayAction);
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Ani 연출 연결
	public bool IS_Action() {
		return m_PlayAction != null;
	}
	IEnumerator PlayMission() {
		m_MainUI.Active.SetActive(false);
		m_MainUI.Btns.gameObject.SetActive(false);
		m_PB.gameObject.SetActive(false);
		m_StartUI.Active.SetActive(false);

		yield return new WaitWhile(() => POPUP.IS_PopupUI());
		m_PlayAction = null;
	}

	IEnumerator PlayStart(float _spd = 1f) {
		m_ResultAni.SetTrigger("Start");
		m_ResultAni.speed = _spd;
		m_MainUI.Active.SetActive(false);
		m_MainUI.Btns.gameObject.SetActive(false);
		m_PB.gameObject.SetActive(false);
		if (STAGEINFO.m_StageContentType != StageContentType.Bank) {
			m_StartUI.Active.SetActive(true);
			m_StartUI.Ani.SetTrigger("Start");
			m_StartUI.Ani.speed = _spd;
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_StartUI.Ani));
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_ResultAni));
		MainUIOn();
		m_PlayAction = null;
	}

	public void MainUIOn() {
		m_StartUI.Active.SetActive(false);
		m_MainUI.Active.SetActive(true);
		m_MainUI.Ani.SetTrigger("On");
	}

	public Item_SpeechBubble GetSpeechBubble() {
		return m_MainUI.Speech;
	}
	IEnumerator BtnAction(bool Active, float _timer = 0) {
		iTween.StopByName(gameObject, "BtnTimer");
		if (Active) {
			if (!m_MainUI.Btns.gameObject.activeSelf) {
				m_MainUI.Btns.gameObject.SetActive(true);
				m_MainUI.Btns.SetTrigger("On");
				iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", _timer, "onupdate", "TW_BtnTimer", "oncomplete", "TW_BtnTimeOver", "name", "BtnTimer"));
				yield return new WaitForSeconds(1f);
			}
		}
		else {
			if (m_MainUI.Btns.gameObject.activeSelf) {
				m_MainUI.Btns.SetTrigger("Off");
				yield return new WaitForSeconds(1f);
				m_MainUI.Btns.gameObject.SetActive(false);
			}
		}
	}
	public void SetBtnAction(bool _active, float _timer = 0) {
		StartCoroutine(m_PlayAction = BtnAction(_active, _timer));
	}
	void TW_BtnTimer(float _amount) {
		m_MainUI.m_EvaBtn.Timer.fillAmount = _amount;
	}
	void TW_BtnTimeOver() {
		OnEva((int)EBattleDir.Center);
	}
	public void GameOver() {
		m_Fail.SetActive(true);
		m_ResultAni.SetTrigger("Fail");
		StartCoroutine(IE_StartFadeIn(2f));
	}

	IEnumerator IE_StartFadeIn(float _delay) {
		yield return new WaitForSeconds(_delay);
		m_Fade.SetActive(true);
		StartCoroutine(IE_CallResult(90f / 60f / (STAGEINFO.m_StageContentType == StageContentType.Bank ? 4f : 1f)));
	}

	public void Clear() {
		m_ResultAni.SetTrigger("Succ");
		StartCoroutine(IE_CallResult(177f / 60f / (STAGEINFO.m_StageContentType == StageContentType.Bank ? 4f : 1f)));
	}
	IEnumerator IE_CallResult(float _delay) {
		yield return new WaitForSeconds(_delay);
		BATTLEINFO.Result();
	}
	/// <summary> 선택 방향에 따른 애니메이션 처리 </summary>
	public void OnEva(int _dirtype) {
		EBattleDir dir = (EBattleDir)_dirtype;
		if (BATTLE.OnEva(dir)) {
			iTween.StopByName(gameObject, "BtnTimer");
			StartCoroutine(IE_OnEva(dir));
		}
	}
	IEnumerator IE_OnEva(EBattleDir _dir) {
		m_MainUI.m_EvaBtn.Ani.SetTrigger(_dir.ToString());
		yield return new WaitForEndOfFrame();
		//m_ISEva = true;
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_MainUI.m_EvaBtn.Ani));

		SetBtnAction(false);
		//m_ISEva = false;
	}
	public bool IS_Eva() {
		if (m_MainUI.Btns == null) return false;
		return m_MainUI.Btns.gameObject.activeSelf;
	}
	public void OnDef(bool Active) {
		if (STAGE_USERINFO.GetStat(StatType.Guard) < 1) return;
		BATTLE.OnDef(BATTLE.m_DefState == EPlayerState.Idle && Active);
		if (BATTLE.m_DefState == EPlayerState.Def && Active) StartCoroutine(IE_OnDef());
	}
	IEnumerator IE_OnDef() {
		m_MainUI.m_EvaBtn.Ani.SetTrigger("NotSelect");

		yield return new WaitForEndOfFrame();
		//m_ISEva = true;
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_MainUI.m_EvaBtn.Ani));

		SetBtnAction(false);
		BATTLE.UseGuard();
		//m_ISEva = false;
	}
	public void SetEvaBtnAnim(int _pos, string _trig) {
		Animator anim = m_MainUI.m_EvaBtn.Cards[_pos].GetComponent<Animator>();
		if (anim != null && !anim.GetCurrentAnimatorStateInfo(0).IsName(_trig)) anim.SetTrigger(_trig);
	}

	public void SetRound(int _now, int _max) {
		if (_max < 1) return;

		m_MainUI.m_Round.CountGroup.SetActive(true);
		m_MainUI.m_Round.Count.text = string.Format("{0}/{1}", _now, _max);
		m_MainUI.m_Round.CenterCount.text = _now.ToString();
		m_MainUI.m_Round.Count.color = m_MainUI.m_Round.CenterCount.color = _now == _max ? Utile_Class.GetCodeColor("#FF4349") : Utile_Class.GetCodeColor("#E2D8AE");
	}
	public void StartRoundCenter(int _max) {
		if (_max < 1) return;
		m_MainUI.m_Round.CenterAlarm.SetActive(true);
	}
	public void SetKillCount(int _crnt, int _max) {
		m_MainUI.m_Kill.Count.text = string.Format("{0} / {1}", _crnt, _max);
	}
	public void OnGameOverClick()
	{
		BATTLEINFO.Result();
	}
	/// <summary> 일시정지 버튼 </summary>
	public void ClickPause() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Def, 0)) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Eva, 0)) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Note, 0)) return;
		if (BattleMng.IsValid() && BATTLE.m_State != EBattleState.Play) return;
		if(STAGEINFO.m_StageModeType == StageModeType.None) {
			Time.timeScale = Time.timeScale > 0f ? 0f : 1f;
			m_MainUI.PauseBtnIcon.sprite = m_MainUI.PauseBtnSprites[Time.timeScale > 0f ? 0 : 1];
		}
		else
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Pause);
	}
	public void ClickGuide() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Def, 0)) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Eva, 0)) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Battle_Note, 0)) return;
		if (BattleMng.IsValid() && BATTLE.m_State != EBattleState.Play) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Guide_BattleNote, (res, obj) => { Time.timeScale = 1f; });
		Time.timeScale = 0f;//팝업 켤떄 타임스케일 1이라 켜진 뒤에 0
	}
}
