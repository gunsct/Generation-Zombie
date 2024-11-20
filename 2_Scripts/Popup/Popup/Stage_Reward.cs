using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

//받은 보상은 StageUserinfo로
public class Stage_Reward : PopupBase
{
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Animator[] CardAnims;
		public Item_Reward_Card[] Card;
		public SpriteRenderer TimeBar;
		public RectTransform Block;
		public GameObject LBtn;
		public GameObject RBtn;
		public GameObject CloseBtn;
		public TextMeshPro reRollPriceTxt;
		public TextMeshPro reRollTxt;
		public GameObject[] LockFXs;
		public GameObject[] GoldCardFXs;
		public GameObject BGFX;
		public GameObject CancleBtn;
	}
	[SerializeField]
	SUI m_SUI;
	[SerializeField] bool m_CanSelect = false;
	List<StageCardInfo> m_Reward = new List<StageCardInfo>();
	public StageCardInfo m_SelectReward;
	/// <summary> 보상 안받고 끄는 경우 </summary>
	public bool m_NotReward;
	float[] m_Timer = new float[2];
	int m_RerollCashPrice;
	private int[] groupid;
	private int lv;
	private Predicate<TIngameRewardTable> CheckBattleReward;
	bool m_CanCancle;
	bool m_AllGroup;
	bool m_BattleReward;
	Vector3 m_Spos;
	bool m_PickCommMat;
	int m_CommGid;

	IEnumerator m_Action; //end ani check
	private void Awake() {
		InitFX();
	}
	void InitFX() {
		if(STAGEINFO.m_StageModeType == StageModeType.Tower) {
			m_SUI.LBtn.SetActive(false);
			m_SUI.RBtn.SetActive(false);
			m_SUI.CloseBtn.SetActive(false);
		}
		for (int i = 0; i < 3; i++) {
			m_SUI.LockFXs[i].SetActive(false);
			m_SUI.GoldCardFXs[i].SetActive(false);
		}
	}
	void RefreshResolutin() {
		transform.position = Vector3.zero;
		Vector3 scale = Vector3.one / Canvas_Controller.SCALE;
		scale.z = 1;
		transform.localScale = scale;

		m_SUI.BGFX.transform.localScale = Vector3.one * Mathf.Max(1f, Canvas_Controller.ASPECT / ((float)Screen.width / (float)Screen.height));
	}
	private void Update() {
		RefreshResolutin();
		if (m_Action != null) return;

		//고르는 제한 시간 다되면 미선택으로 꺼짐
		if (m_CanSelect && m_Timer[0] > 0f) {
			m_Timer[0] -= Time.deltaTime;

			m_SUI.TimeBar.material.SetFloat("_Progress", Mathf.Clamp(m_Timer[0] / m_Timer[1], 0f, 1f));

			if (m_Timer[0] <= 0f) {
				m_CanSelect = false;
				m_NotReward = true;

				//if (STAGEINFO.m_StageModeType == StageModeType.Stage) {
				//	Item_Stage_Char charcard = STAGE.m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
				//	TConditionDialogueGroupTable table = charcard.m_Info.m_TData.GetSpeechTable(DialogueConditionType.RewardTime);
				//	if (table != null && table.IS_CanSpeech()) STAGE_USERINFO.SetSpeech(table, Utile_Class.GetCanvasPosition(charcard.transform.position + new Vector3(0f, -charcard.transform.position.y - 1f, 0f)), 1.5f);
				//}

				List<int> pos = new List<int>();
				for (int i = 0; i < 3; i++) {
					if (m_SUI.Card[i].IsLock()) continue;
					pos.Add(i);
				}
				if (pos.Count > 0) {// && STAGEINFO.m_StageModeType == StageModeType.Tower
					StartCoroutine(SelectReward(pos[UTILE.Get_Random(0, pos.Count)]));
					m_CanSelect = false;
				}
				else {
					STAGE_USERINFO.m_leftReRollCnt++;
					Close();
				}
			}
		}
		bool cantouch = false;
		if (MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) cantouch = STAGE.TouchCheck();
		else if (MAIN.IS_State(MainState.TOWER) && POPUP.GetMainUI().m_Popup == PopupName.Stage) cantouch = TOWER.TouchCheck();

		if (cantouch && m_CanSelect) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hit = Physics.RaycastAll(ray, Camera.main.farClipPlane);
			for (int i = 0; i < hit.Length; i++) {
				GameObject hitobj = hit[i].transform.gameObject;
				if (!hitobj.activeSelf) continue;

				if (m_SUI.LBtn == hitobj) {
					ReRoll(true);
					return;
				}

				if (m_SUI.RBtn == hitobj) {
					ReRoll(false);
					return;
				}

				if (m_SUI.CloseBtn == hitobj) {
					// 보상 포기
					m_CanSelect = false;
					STAGE_USERINFO.m_leftReRollCnt++;

					m_NotReward = true;
					Close();
					return;
				}
				Item_Reward_Card hitcard = hitobj.GetComponent<Item_Reward_Card>();
				if (hitcard != null) {
					for (int j = 0; j < 3; j++) {
						if (m_SUI.Card[j] == hitcard) {
							if (m_SUI.Card[j].IsLock()) {
								m_SUI.LockFXs[j].GetComponent<Eff_Card_Chain>().SetAnim(Eff_Card_Chain.AnimState.Touch);
							}
							else {
								StartCoroutine(SelectReward(j));
								m_CanSelect = false;
							}
							return;
						}
					}
				}
			}
		}
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		Vector3 panelpos = Utile_Class.GetWorldPosition(Vector2.zero, -10f);
		panelpos.z = 0;
		transform.position = panelpos;
		m_SUI.Block.localPosition = -transform.localPosition;
		//스테이지가 메인일떄 사이즈랑 위치
		float diff = 0;
		float h = 2560 - diff;
		if (MAIN.IS_State(MainState.STAGE) && POPUP.GetMainUI().m_Popup == PopupName.Stage) {
			diff = 230 + 50;
			h = 2560 - diff;
		}
		m_SUI.Block.sizeDelta = new Vector2(m_SUI.Block.sizeDelta.x, h);
		m_SUI.Block.localPosition += new Vector3(0f, diff * 0.5f, 0f);

		RefreshResolutin();

		m_RerollCashPrice = STAGE_USERINFO.GetRewardRerollPrice();
		SetSpecialRerollBtn();

		groupid = (int[]) (aobjValue[0]);
		m_CommGid = groupid.Length > 2 ? groupid[2] : BaseValue.BATTLE_REWARD_COMMON_GID;
		lv = (int) (aobjValue[1]);
		CheckBattleReward = (Predicate<TIngameRewardTable>) (aobjValue[2]);
		m_CanCancle = (bool)aobjValue[3];
		m_SUI.CancleBtn.SetActive(m_CanCancle);
		if (aobjValue.Length > 4) m_AllGroup = (bool)aobjValue[4];
		m_BattleReward = aobjValue.Length > 5 ? (bool)aobjValue[5] : true;
		m_Spos = aobjValue.Length > 6 ? (Vector3)aobjValue[6] : Vector3.zero;
		m_PickCommMat = aobjValue.Length > 7 ? (bool)aobjValue[7] : false;

		//속도 스탯으로 시간 결정
		SetTime();
		m_SUI.TimeBar.material.SetFloat("_Progress", Mathf.Clamp(m_Timer[0] / m_Timer[1], 0f, 1f));
		//m_SUI.Anim.speed = 1.2f;
		m_SUI.BGFX.GetComponent<Animator>().SetTrigger("Start");
		StartCoroutine(IE_SetCard(true, false, m_AllGroup));

		PlayEffSound(SND_IDX.SFX_0320);
	}
	void SetSpecialRerollBtn() {
		m_RerollCashPrice = STAGE_USERINFO.GetRewardRerollPrice();
		if (USERINFO.m_Cash < m_RerollCashPrice) {
			m_SUI.LBtn.GetComponent<SpriteRenderer>().color = Color.white * 0.5f;
			//m_SUI.LBtn.GetComponent<BoxCollider>().enabled = false;
			m_SUI.reRollPriceTxt.color = new Color(1f, 0.3f, 0.3f, 1f);
		}
	}
	IEnumerator IE_SetCard(bool isStart, bool isPremium = false, bool isAllGroup = false) {
		InitFX();

		if (!isStart) {
			m_SUI.Anim.SetTrigger("Refresh");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 5f / 80f));
		}

		m_Reward = new List<StageCardInfo>();
		for(int i = 0; i < 3; i++)
		{
			// TODO 임시 금니 보상
			int gid = 0;
			int idx = 0;
			if (isAllGroup)
				gid = isPremium ? 99999999 : groupid[0];
			else {
				if (isPremium) gid = 99999999;
				else if (m_PickCommMat && MAIN.IS_State(MainState.STAGE)) {
					if (groupid.Length > 1 && groupid[1] != 0) {
						idx = TDATA.GetRandEnemyDropTable(groupid[1]).m_CardIdx;
					}
					else {
						if (m_Reward.Count == 0) gid = m_CommGid;
						else if (m_Reward.Count == 1) {
							List<TStageCardTable> datas = STAGEINFO.GetMaterialCardIdxs();
							if (datas.Count > 0) idx = datas[UTILE.Get_Random(0, datas.Count)].m_Idx;    // 재료로 변경
							else gid = m_CommGid;
						}
						else if (m_Reward.Count == 2) {
							gid = groupid[0];
						}
					}
				}
				else gid = m_Reward.Count < 2 ? m_CommGid : groupid[0];
			}

			if (gid != 0) {
				TIngameRewardTable table = TDATA.GetPickIngameReward(gid, lv, CheckBattleReward, m_BattleReward);
				idx = table.m_Val;
			}
			m_Reward.Add(new StageCardInfo(idx));
		}

		List<StageRewardState> state = new List<StageRewardState>();
		List<int> pos = new List<int>() { 0, 1, 2 };

		//디버프 BlindRewardInfo 있으면 전투보상 전부 blind
		if (STAGE_USERINFO.ISBuff(StageCardType.ConBlindRewardInfo)) {
			yield return new WaitForEndOfFrame();
			for (int i = 0; i < m_Reward.Count; i++) {
				m_SUI.Card[i].SetData(m_Reward[i], StageRewardState.Blind);
			}
		}
		else {
			if (STAGE_USERINFO.Is_UseStat(StatType.Men) &&
				STAGE_USERINFO.GetStat(StatType.Men) / STAGE_USERINFO.GetMaxStat(StatType.Men) < 0.3f) {
				state.Add(StageRewardState.MenLow);
			}

			if (STAGEINFO.m_TStage.m_PlayType.Find((t) => t.m_Type == PlayType.Blind) != null) {
				state.Add(StageRewardState.Blind);
			}

			if (STAGEINFO.m_TStage.m_PlayType.Find((t) => t.m_Type == PlayType.CardRandomLock) != null) {
				state.Add(StageRewardState.CardRandomLock);
			}

			if (STAGE_USERINFO.ISBuff(StageCardType.ConRewardCardLock)) {
				state.Add(StageRewardState.DebuffRewardCardLock);
			}

			for (int i = 0; i < state.Count; i++) {
				pos.RemoveAt(UTILE.Get_Random(0, pos.Count));
			}
			yield return new WaitForEndOfFrame();
			for (int i = 0, statepos = state.Count; i < m_Reward.Count; i++) {
				m_SUI.Card[i].SetData(m_Reward[i], pos.Contains(i) ? StageRewardState.Normal : state[--statepos]);
			}
		}

		PlayEffSound(SND_IDX.SFX_0210);


		if (isStart) {
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 20f / 62f));

			if (MAIN.IS_State(MainState.STAGE) && !POPUP.IS_ViewPopup(PopupName.Stage_CardUse)){
				Main_Stage stage = POPUP.GetMainUI().GetComponent<Main_Stage>();
				stage.StartPlayAni(Main_Stage.AniName.StageReward);
			}

			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 50f / 62f));
		}
		if(!isStart) {
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 44f / 49f));
		}

		for (int i = 0; i < m_Reward.Count; i++) m_SUI.LockFXs[i].SetActive(m_SUI.Card[i].IsLock());
		for (int i = 0; i < 3; i++) if (isPremium) m_SUI.GoldCardFXs[i].SetActive(true);

		m_CanSelect = true;
	}

	public override void SetUI()
	{
		base.SetUI();
		RefreshReRoll();
	}

	IEnumerator SelectReward(int _pos) {
		m_SelectReward = m_Reward[_pos];
		if(m_SUI.Card[_pos].m_State != StageRewardState.Normal) {
			m_SUI.Card[_pos].SetData(m_SelectReward, StageRewardState.Normal, true);
			yield return new WaitForSeconds(1f);
		}

		PlayEffSound(SND_IDX.SFX_0321);
		StartCoroutine(EndAnimation(60f, 95f));

		m_SUI.BGFX.GetComponent<Animator>().SetTrigger("End");

		TStageCardTable data = TDATA.GetStageCardTable(m_SelectReward.m_Idx);
		StageCardType type = data.m_Type;
		switch (_pos) {
			case 0: m_SUI.Anim.SetTrigger(type == StageCardType.Gamble ? "Use_L" :"End_L"); break;
			case 1: m_SUI.Anim.SetTrigger(type == StageCardType.Gamble ? "Use_C" : "End_C"); break;
			case 2: m_SUI.Anim.SetTrigger(type == StageCardType.Gamble ? "Use_R" : "End_R"); break;
		}
		float value;
		switch (type)
		{
			/// <summary> HP 회복 </summary>
			case StageCardType.RecoveryHp:
				value = data.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * value));
				else BATTLE.AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * value));
				break;
			case StageCardType.RecoveryHpPer:
				value = data.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * value));
				else BATTLE.AddStat(StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * value));
				break;
			/// <summary> 포만감 회복 </summary>
			case StageCardType.RecoverySat:
				value = data.m_Value1;
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.Sat, Mathf.RoundToInt(value));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Sat, Mathf.RoundToInt(value));
				else BATTLE.AddStat(StatType.Sat, Mathf.RoundToInt(value));
				break;
			/// <summary> 정신력 회복 </summary>
			case StageCardType.RecoveryMen:
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.Men, Mathf.RoundToInt(data.m_Value1));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Men, Mathf.RoundToInt(data.m_Value1));
				else BATTLE.AddStat(StatType.Men, Mathf.RoundToInt(data.m_Value1));
				break;
			/// <summary> 청결도 회복 </summary>
			case StageCardType.RecoveryHyg:
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.Hyg, Mathf.RoundToInt(data.m_Value1));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Hyg, Mathf.RoundToInt(data.m_Value1));
				else BATTLE.AddStat(StatType.Hyg, Mathf.RoundToInt(data.m_Value1));
				break;
			/// <summary> 포만감 회복 </summary>
			case StageCardType.PerRecoverySat:
				int sat = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * data.m_Value1);
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.Sat, sat);
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Sat, sat);
				else BATTLE.AddStat(StatType.Sat, sat);
				break;
			/// <summary> 정신력 회복 </summary>
			case StageCardType.PerRecoveryMen:
				int men = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * data.m_Value1);
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.Men, men);
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Men, men);
				else BATTLE.AddStat(StatType.Men, men);
				break;
			/// <summary> 청결도 회복 </summary>
			case StageCardType.PerRecoveryHyg:
				int hyg = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * data.m_Value1);
				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.AddStat_Action(m_SUI.Card[_pos].transform, StatType.Hyg, hyg);
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.AddStat(StatType.Hyg, hyg);
				else BATTLE.AddStat(StatType.Hyg, hyg);
				break;
			case StageCardType.RecoveryAP:
				if (MAIN.IS_State(MainState.STAGE))
					yield return STAGE.SelectAction_RecoveryAP(Mathf.RoundToInt(data.m_Value1));
				break;
			case StageCardType.LimitTurnUp:
				if (MAIN.IS_State(MainState.STAGE))
					yield return STAGE.SelectAction_LimitTurnUp(Mathf.RoundToInt(data.m_Value1));
				break;
			case StageCardType.AddRerollCount:
				if (MAIN.IS_State(MainState.STAGE))
					yield return STAGE.SelectAction_AddRerollCount(data.m_Value1);
				break;
			case StageCardType.BigSupplyBox:
				if (MAIN.IS_State(MainState.STAGE)) STAGE.m_Check.Check(StageCheckType.GetBox, 0, Mathf.Max(1, (int)data.m_Value1), false);
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.m_Check.Check(StageCheckType.GetBox, 0, Mathf.Max(1, (int)data.m_Value1), false);
				break;
		}

		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));

		if (m_SelectReward.m_TData.IS_BuffCard()) {
			if(type == StageCardType.Synergy) {
				if (MAIN.IS_State(MainState.STAGE)) STAGE.SetBuff(EStageBuffKind.Synergy, Mathf.RoundToInt(data.m_Value1));
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.SetBuff(EStageBuffKind.Synergy, Mathf.RoundToInt(data.m_Value1));
				else BATTLE.SetBuff(EStageBuffKind.Synergy, Mathf.RoundToInt(data.m_Value1));
			}
			else {
				if (MAIN.IS_State(MainState.STAGE)) STAGE.SetBuff(EStageBuffKind.Stage, m_SelectReward.m_Idx);
				else if (MAIN.IS_State(MainState.TOWER)) TOWER.SetBuff(EStageBuffKind.Stage, m_SelectReward.m_Idx);
				else BATTLE.SetBuff(EStageBuffKind.Stage, m_SelectReward.m_Idx);
				//레벨업은 스텟 리셋
				if (type == StageCardType.LevelUp) STAGE_USERINFO.StatReset();
			}
		}
		switch (type)
		{
			case StageCardType.Material:
				STAGE?.AddMaterial((StageMaterialType)Mathf.RoundToInt(data.m_Value1), Mathf.RoundToInt(data.m_Value2), m_Spos);
				break;
			case StageCardType.Gamble:
				TGambleCardTable gambletable = TDATA.GetGambleCardTable(m_SelectReward.m_GambleIdx);
				float randprop = UTILE.Get_Random(0f, 1f);
				float? synergySD = STAGE_USERINFO.GetSynergeValue(JobType.Swindler, 1);
				if (synergySD != null) {
					randprop = Mathf.Clamp(randprop - (float)synergySD, 0f, 1f);
					STAGE_USERINFO.ActivateSynergy(JobType.Swindler);
					Utile_Class.DebugLog_Value("Swindler 도박 확률 증가");
				}
				GameObject popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Gamble, null, gambletable, randprop).gameObject;

				yield return new WaitWhile(() => popup != null);

				if (MAIN.IS_State(MainState.STAGE)) yield return STAGE.SelectAction_StageCardProc_Gamble(gambletable, randprop);
				else if (MAIN.IS_State(MainState.TOWER)) yield return TOWER.SelectAction_StageCardProc_Gamble(gambletable, randprop);
				else BATTLE.SelectAction_StageCardProc_Gamble(gambletable, randprop);
				break;
		}
		Close(m_SelectReward.m_Idx);
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result = 0) {
		if (_result == 0) {
			StartCoroutine(EndAnimation(30f, 30f));

			m_SUI.BGFX.GetComponent<Animator>().SetTrigger("End");

			m_SUI.Anim.SetTrigger("Not");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.BGFX.GetComponent<Animator>()));
		}

		base.Close(_result);
	}

	IEnumerator EndAnimation(float targetFrame, float tortalFrame) {
		InitFX();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, targetFrame / tortalFrame));
		if (MAIN.IS_State(MainState.STAGE) && !POPUP.IS_ViewPopup(PopupName.Stage_CardUse)) {
			Main_Stage stage = POPUP.GetMainUI().GetComponent<Main_Stage>();
			stage.StartPlayAni(Main_Stage.AniName.In);
		}
	}

	private void ReRoll(bool isSpecial)
	{
		m_CanSelect = false;


		//시너지
		bool reducnt = true;
		bool premium = false;
		if (STAGE_USERINFO.m_leftReRollCnt > 0) {
			float? synergySD = STAGE_USERINFO.GetSynergeValue(JobType.Swindler, 0);
			if (synergySD != null && UTILE.Get_Random(0f, 1f) < (float)synergySD) {
				//2:8 높은 레벨 - 굴림 수 소모 x
				if (UTILE.Get_Random(0, 10) < 2) {
					premium = true;
				}
				else reducnt = false;

				STAGE_USERINFO.ActivateSynergy(JobType.Swindler);
				Utile_Class.DebugLog_Value("Swindler 재굴림시 2:8로 스페셜 or 굴림수 x");
			}
		}
		if (isSpecial)
		{
			// 금니 갯수 ? 
			// 금니 사용 ?

			m_RerollCashPrice = STAGE_USERINFO.GetRewardRerollPrice();
			if (USERINFO.m_Cash >= m_RerollCashPrice)
			{

#if NOT_USE_NET
				//속도 스탯으로 시간 결정
				SetTime();
				m_SUI.TimeBar.material.SetFloat("_Progress", 1f);
				
				StartCoroutine(IE_SetCard(false, true, m_AllGroup));

				STAGE_USERINFO.m_specialReRollCnt++;
				
				USERINFO.GetCash(-m_RerollCashPrice);

				SetSpecialRerollBtn();
#else
				SendStageReRoll((res) => {
					SetTime();
					m_SUI.TimeBar.material.SetFloat("_Progress", 1f);
				
					StartCoroutine(IE_SetCard(false, true, m_AllGroup));

					STAGE_USERINFO.m_specialReRollCnt++;

					SetSpecialRerollBtn();
					RefreshReRoll();
				});
#endif
			}
			else {
				// 금니가 부족하면
				POPUP.StartLackPop(BaseValue.CASH_IDX, true);
				m_CanSelect = true;
			}
		}
		else
		{
			if (STAGE_USERINFO.m_leftReRollCnt == 0)
			{
				// 광고중 시간 정지 광고보고 횟수 추가
				//TODO:광고때 이어넣기
				//if(광고를 봣으면)
				//STAGE_USERINFO.m_leftReRollCnt++;
				////광고창 뜨면 m_CanSelect = false 해줘야함
				m_CanSelect = true;
			}
			else
			{
				//속도 스탯으로 시간 결정
				SetTime();
				m_SUI.TimeBar.material.SetFloat("_Progress", 1f);//Mathf.Clamp(m_Timer[0] / m_Timer[1], 0f, 1f) * 0.5f

				StartCoroutine(IE_SetCard(false, premium, m_AllGroup));

				if(reducnt) STAGE_USERINFO.m_leftReRollCnt--;
			}
		}
		
		RefreshReRoll();
	}
	
	void SendStageReRoll(Action<LS_Web.RES_STAGE_REROLLING> cb) {
		UserInfo.StageIdx stageidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
		UserInfo.Stage stage = USERINFO.m_Stage[StageContentType.Stage];
		WEB.SEND_REQ_STAGE_REROLLING((res) => {
			if (!res.IsSuccess()) {
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 
				});
				return;
			}
			cb?.Invoke(res);
		}, stage.UID, stageidx.Week, stageidx.Pos, STAGEINFO.m_Idx, STAGEINFO.EUID);
	}
	void SetTime() {
		m_Timer[0] = m_Timer[1] = Mathf.Clamp(STAGE_USERINFO.GetStat(StatType.Speed) * 0.2f, 2f, 10f);
	}
	private void RefreshReRoll()
	{
		int price = STAGE_USERINFO.GetRewardRerollPrice();

		m_SUI.reRollPriceTxt.text = $"x{price}";

		//광고로 전환
		if (STAGE_USERINFO.m_leftReRollCnt == 0) {
			m_SUI.reRollTxt.text = TDATA.GetString(332);
		}
		else {

			m_SUI.reRollTxt.text = $"{TDATA.GetString(331)} ({STAGE_USERINFO.m_leftReRollCnt})";
		}
	}
}
