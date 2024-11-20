using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Time

	void TimeInit(int Turn, int Time)
	{
		if (Turn < 0) Turn = 0;
		m_User.m_Turn = Turn;

		//시너지
		float? synergy2 = m_User.GetSynergeValue(JobType.Spy, 1);
		if (synergy2 != null) {
			m_User.m_Turn += Mathf.RoundToInt((float)synergy2);
			STAGE_USERINFO.ActivateSynergy(JobType.Spy);
			Utile_Class.DebugLog_Value("Spy 시너지 발동 " + "변화 전 -> 후 : 전 :" + Turn.ToString() + " 후 : " + m_User.m_Turn.ToString());
			//m_User.m_SynergyUseCnt[JobType.Spy]++;
		}

		m_User.m_Time = Time;
		DLGTINFO.f_RFClockUI?.Invoke(m_User.m_Turn, m_User.m_Time);
	}

	IEnumerator AddTime(int addTime = 1) {
		yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);

		float? synergy2 = 0f;

		m_User.m_TurnCnt++;
		m_User.m_Time++;

		if (m_User.m_Time == 24) {
			m_User.m_Time = 0;
		}
		if (m_User.IS_TurnUse()) {
			DLGTINFO?.f_RFClockChangeUI?.Invoke(Mathf.Max(0, --m_User.m_Turn), m_User.m_Time);
			m_MainUI.TurnSpeech();
		}
		else DLGTINFO?.f_RFClockChangeUI?.Invoke(0, m_User.m_Time);

		TStageTable tdat = STAGEINFO.m_TStage;

		if (STAGEINFO.m_TStage.GetMode(PlayType.BanActive) == null) {
			int aprecov = tdat.m_APRecovery;
			float per = 1f;
			per += STAGE_USERINFO.GetBuffValue(StageCardType.APRecoveryUp);
			per += USERINFO.ResearchValue(ResearchEff.APSpeedUp);
			aprecov = Mathf.RoundToInt(aprecov * per);
			if (STAGE_USERINFO.ISBuff(StageCardType.ConActiveAP)) aprecov = 0;
			//멘탈 부족에 따른 행동력 회복 감소
			if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusAP)) {
				aprecov= Mathf.RoundToInt(aprecov * (1f - STAGE_USERINFO.m_DebuffValues[DebuffType.MinusAP]));
			}
			int preval = m_User.m_AP[0];
			m_User.m_AP[0] = Mathf.Clamp(m_User.m_AP[0] + aprecov, 0, m_User.m_AP[1]);

			//시너지
			synergy2 = m_User.GetSynergeValue(JobType.Athlete, 1);
			if (synergy2 != null ) {
				m_User.m_AP[0] = Mathf.Min(m_User.m_AP[0] + (STAGE_USERINFO.ISBuff(StageCardType.ConActiveAP) ? 0 :Mathf.RoundToInt((float)synergy2)), m_User.m_AP[1]);
				STAGE_USERINFO.ActivateSynergy(JobType.Athlete);
				Utile_Class.DebugLog_Value("Athlete 시너지 발동 " + "변화 전 -> 후 : 전 :" + (m_User.m_AP[0] - (STAGE_USERINFO.ISBuff(StageCardType.ConActiveAP) ? 0 : Mathf.RoundToInt((float)synergy2))).ToString() + " 후 : " + m_User.m_AP[0].ToString());
				//m_User.m_SynergyUseCnt[JobType.Athlete]++;
			}

			for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
				m_Chars[i].SetAPUI(m_User.m_AP[0]);
			}
			//행동력 회복 스킬이나 시너지도 추가 될거임
			DLGTINFO?.f_RfAPUI?.Invoke(m_User.m_AP[0], preval, m_User.m_AP[1]);
		}
		//유아이도 연동
		float value = 0f;

		if (STAGE_USERINFO.GetStat(StatType.Hyg) > 0) {
			value = -tdat.m_ReduceStat[(int)StatType.Hyg] - Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * (STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusHyg) + STAGE_USERINFO.GetBuffValue(StageCardType.TurnAllStatus)));// 음수로 들어가있음
			value -= value * USERINFO.GetSkillValue(SkillKind.Immunity);
			//시너지
			synergy2 = m_User.GetSynergeValue(JobType.Nurse, 1);
			if (synergy2 != null) {
				bool active = STAGE_USERINFO.GetMaxStat(StatType.Hyg) > STAGE_USERINFO.GetStat(StatType.Hyg);
				value += active ? STAGE_USERINFO.GetMaxStat(StatType.Hyg) * (float)synergy2 : 0;
				if (active) STAGE_USERINFO.ActivateSynergy(JobType.Nurse);
				Utile_Class.DebugLog_Value("Nurse 시너지 발동 " + "변화 전 -> 후 : 전 :" + (value - STAGE_USERINFO.GetMaxStat(StatType.Hyg) * (float)synergy2).ToString() + " 후 : " + value.ToString());
			}
			StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(value)));
		}

		if (STAGE_USERINFO.GetStat(StatType.Men) > 0) {
			value = -tdat.m_ReduceStat[(int)StatType.Men] - Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * (STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusMen) + STAGE_USERINFO.GetBuffValue(StageCardType.TurnAllStatus)));// 음수로 들어가있음
			value -= value * USERINFO.GetSkillValue(SkillKind.CoolMental);
			//시너지
			synergy2 = m_User.GetSynergeValue(JobType.Counselor, 1);
			if (synergy2 != null) {
				bool active = STAGE_USERINFO.GetMaxStat(StatType.Men) > STAGE_USERINFO.GetStat(StatType.Men);
				value += active ? STAGE_USERINFO.GetMaxStat(StatType.Men) * (float)synergy2 : 0;
				if(active) STAGE_USERINFO.ActivateSynergy(JobType.Counselor);
				Utile_Class.DebugLog_Value("Counselor 시너지 발동 " + "변화 전 -> 후 : 전 :" + (value - STAGE_USERINFO.GetMaxStat(StatType.Men) * (float)synergy2).ToString() + " 후 : " + value.ToString());
				//m_User.m_SynergyUseCnt[JobType.Counselor]++;
			}
			StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(value)));
		}

		if (STAGE_USERINFO.GetStat(StatType.Sat) > 0) {
			value = -tdat.m_ReduceStat[(int)StatType.Sat] - Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * (STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusSat) + STAGE_USERINFO.GetBuffValue(StageCardType.TurnAllStatus)));// 음수로 들어가있음
			value -= value * USERINFO.GetSkillValue(SkillKind.SatChargUp);
			//시너지
			synergy2 = m_User.GetSynergeValue(JobType.Shef, 1);
			if (synergy2 != null) {
				bool active = STAGE_USERINFO.GetMaxStat(StatType.Sat) > STAGE_USERINFO.GetStat(StatType.Sat);
				value += active ? STAGE_USERINFO.GetMaxStat(StatType.Sat) * (float)synergy2 : 0;
				if (active) STAGE_USERINFO.ActivateSynergy(JobType.Shef);
				Utile_Class.DebugLog_Value("Shef 시너지 발동 " + "변화 전 -> 후 : 전 :" + (value - STAGE_USERINFO.GetMaxStat(StatType.Sat) * (float)synergy2).ToString() + " 후 : " + value.ToString());
				//m_User.m_SynergyUseCnt[JobType.Shef]++;
			}
			StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(value)));
		}
		if (STAGE_USERINFO.GetStat(StatType.HP) > 0) {
			value = - Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * (STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusHp)));// 음수로 들어가있음
			StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(value)));
		}
		//시너지
		synergy2 = m_User.GetSynergeValue(JobType.Doctor, 1);
		bool turn3 = STAGE_USERINFO.m_TurnCnt % 3 == 0;
		bool canrecovhp = STAGE_USERINFO.GetStat(StatType.HP) > 0 && STAGE_USERINFO.GetStat(StatType.HP) < STAGE_USERINFO.GetMaxStat(StatType.HP);
		if (STAGE_USERINFO.m_TurnCnt > 0 && turn3 || synergy2 != null) {
			//체력 회복 관련, 체력이 0 이상일 경우
			if (canrecovhp && turn3 && !STAGE_USERINFO.ISBuff(StageCardType.NoAutoHeal)) {
				value = STAGE_USERINFO.GetStat(StatType.Heal) * BaseValue.HPRECV_TURNRATIO(USERINFO.GetDifficulty());
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(Mathf.Max(0, value)), StatType.None, 0, null, TDATA.GetString(849)));
				//시너지
			}
			if (canrecovhp && synergy2 != null) {
				bool active = STAGE_USERINFO.GetMaxStat(StatType.HP) > STAGE_USERINFO.GetStat(StatType.HP);
				value += active ? STAGE_USERINFO.GetMaxStat(StatType.HP) * (float)synergy2 : 0;
				if (active) STAGE_USERINFO.ActivateSynergy(JobType.Doctor);
				Utile_Class.DebugLog_Value("Doctor 시너지 발동 " + "변화 전 -> 후 : 전 :" + (STAGE_USERINFO.GetStat(StatType.Heal) * BaseValue.HPRECV_TURNRATIO(USERINFO.GetDifficulty())).ToString() + " 후 : " + value.ToString());
				//m_User.m_SynergyUseCnt[JobType.Doctor]++;
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(Mathf.Max(0, value))));
			}
		}

		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) m_Chars[i].CheckSkillCoolTime(addTime);
		m_Check.Check(StageCheckType.Survival, 0);

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		// TimeBomb 체크
		yield return TurnAction_StageCardProc_TimeBomb();

		//시너지
		synergy2 = STAGE.m_User.GetSynergeValue(JobType.Firefighter, 1);
		if (synergy2 != null && m_User.m_TurnCnt != 0 && m_User.m_TurnCnt % Mathf.RoundToInt((float)synergy2) == 0) {
			yield return TurnAction_StageCardProc_Fire();
			STAGE_USERINFO.ActivateSynergy(JobType.Firefighter);
			m_Check.Check(StageCheckType.SuppressionF, 0, 1);
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		//매턴 체크해야 하는 모드들
		List<TStageTable.StagePlayType> modes = new List<TStageTable.StagePlayType>();
		List<PlayType> checktype = new List<PlayType>() { PlayType.FieldAirstrike, PlayType.FireSpread, PlayType.CardShuffle };
		for(int i = 0;i< checktype.Count; i++) {
			TStageTable.StagePlayType mode = STAGEINFO.m_TStage.GetMode(checktype[i]);
			if (mode != null) modes.Add(mode);
		}
		for (int i = 0;i< modes.Count; i++) {
			switch (modes[i].m_Type) {
				case PlayType.FieldAirstrike: yield return Mode_FieldAirstrike(modes[i]); break; 
				case PlayType.FireSpread: yield return Mode_FireSpread(modes[i]); break;
				case PlayType.CardShuffle: yield return Mode_CardShuffle(modes[i]); break;
			}
		}
		//모드로 죽은 카드 변환이나 댕기기 해줘야함
		yield return Check_DieCardAction();

		//순찰 체크
		DarkPatrolCheck();
		//은신 체크
		HideCheck();

		//턴단위 디버프 체크
		//N턴마다 머지 탭 아이템 모두 제거
		if (STAGE_USERINFO.ISBuff(StageCardType.MergeDelete) && m_User.m_TurnCnt % Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MergeDelete)) == 0) {
			m_MainUI.GetMaking.DiscardAll();
		}
		//생존스텟에 따른 디버프 체크
		STAGE_USERINFO.SetDebuff();

		m_MainUI.RefreshDebuffCardCount(StageCardType.ConRandomChoice);
		m_MainUI.RefreshDebuffCardCount(StageCardType.MergeDelete);
		m_MainUI.RefreshDebuffCardCount(StageCardType.ConSkipTurn);
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;
	}

	IEnumerator TurnAction_StageCardProc_TimeBomb()
	{
		// 범위내 터져야할 지뢰들을 찾는다.
		List<Item_Stage> timebombcards = new List<Item_Stage>();
		for (int j = 0, jMax = AI_MAXLINE; j <= jMax; j++)
		{
			for (int i = 0; i < m_ViewCard[j].Length; i++)
			{
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.m_Info.m_NowTData.m_Type != StageCardType.TimeBomb) continue;
				item.m_Info.m_Turn--;
				item.SetTurn(true);
				if (item.m_Info.m_Turn < 1) timebombcards.Add(item);
			}
		}

		if (timebombcards.Count < 1) yield break;


		for (int k = timebombcards.Count - 1; k > -1; k--) {//삭제된 타임봄이 재생성됬을때 예외처리
			if (!timebombcards[k].m_Info.IS_TypeTarget(StageCardType.TimeBomb) || timebombcards[k].m_Info.m_Turn > 0) continue;
			if (timebombcards[k].m_Line == 0) {
				int damage = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * timebombcards[k].m_Info.m_TData.m_Value1);
				StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, -damage));
			}
			yield return SelectAction_StageCardProc_TimeBomb(timebombcards[k]);
		}
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;
	}

	/// <summary> 매턴마다 5*5에서 화염카드 하나를 삭제 </summary>
	IEnumerator TurnAction_StageCardProc_Fire() {
		List<Item_Stage> offcards = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();

		for (int j = 0, Start = 0, End = Start + 3; j <= AI_MAXLINE; j++, End += 2) {
			for (int i = Start; i < End; i++) {
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				StageCardInfo info = item.m_Info;
				// 피난민은 저격 불가
				if (info.m_TData.m_Type != StageCardType.Fire) continue;
				targets.Add(item);
				offcards.Add(item);
				item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => { offcards.Remove(obj); });
			}
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		if (targets.Count < 1) yield break;
		yield return new WaitForSeconds(0.5f);

		// 대상이 아닌놈들 꺼주기
		int randpos = UTILE.Get_Random(0, targets.Count);
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage oncards = targets[i];
			if (i == randpos) continue;
			targets.Remove(oncards);
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, targets);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {

			Item_Stage target = targets[i];
			// 이펙트
			GameObject eff = StartEff(target.transform, "Effect/Stage/Eff_StageCard_Fireext_Throw_S");
			PlayEffSound(SND_IDX.SFX_0611);

			offcards.Add(target);
			target.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[target.m_Line][target.m_Pos] = null;
		}

		yield return new WaitWhile(() => offcards.Count > 0);
	}
}
