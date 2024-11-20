using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TowerMng : ObjMng
{
	IEnumerator SelectAction(Item_Tower_Element card)
	{
		//랜덤에서 미리 뽑는것 때문에 밖에서 체크해줘야함
		TStageCardTable cardtable = null;
		if (card.m_Info.m_TData.m_Type == StageCardType.Tower_Status) {
			TIngameRewardTable rewardtable = TDATA.GetPickIngameReward(Mathf.RoundToInt(card.m_Info.m_TData.m_Value1), 0, CheckBattleReward);
			cardtable = TDATA.GetStageCardTable(rewardtable.m_Val);
		}
		yield return SelectAction_CardProc(card, cardtable);
		m_Map.SelectEnd();
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Map.gameObject));
		m_PlayAction = null;
	}

	IEnumerator SelectAction_CardProc(Item_Tower_Element card, TStageCardTable _cardtable = null)
	{
		StageCardType type = card.m_Info.m_TData.m_Type; 
		switch (type)
		{
			case StageCardType.Enemy:
				yield return SelectAction_CardProc_Battle(card);
				break;
			case StageCardType.Tower_Entrance:yield break;
			case StageCardType.Tower_Refugee:
				yield return SelectAction_CardProc_Refugee(card);
				break;
			case StageCardType.Tower_Rest:
				yield return SelectAction_CardProc_Rest(card);
				break;
			case StageCardType.Tower_Status:
				yield return SelectAction_CardProc_Status(card, _cardtable);
				break;
			case StageCardType.Tower_SupplyBox:
				yield return SelectAction_CardProc_SupplyBox(card);
				break;
			case StageCardType.Tower_OpenEvent:
				yield return SelectAction_CardProc_OpenEvent(card);
				break;
			case StageCardType.Tower_SecrectEvent:
				yield return SelectAction_CardProc_SecrectEvent(card);
				break;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd()) yield return new WaitForSeconds(0.5f);
		yield break;
	}

	IEnumerator SelectAction_CardProc_Battle(Item_Tower_Element card)
	{
		StageCardInfo info = card.m_Info;

		bool IsAniPlay = true;
		m_Map.PlayAni(Item_TowerBG.AniName.End, () => {
			IsAniPlay = false;
		});
		yield return new WaitWhile(() => IsAniPlay);

		TEnemyTable enemyTable = info.m_TEnemyData;

#if !STAGE_NO_BATTLE
		AsyncOperation pAsync = null;
		pAsync = MAIN.StateChange(MainState.BATTLE, SceneLoadMode.ADDITIVESTART, () =>
		{
			MAIN.ActiveScene(() =>
			{
				m_MyCam.gameObject.SetActive(false);
				BATTLE.Init(EBattleMode.Normal, info.m_EnemyIdx, info.m_LV, info.GetStat(EEnemyStat.HP), () => {
					PlayStageBGSound();
					m_MyCam.gameObject.SetActive(true);
					if(!CheckEnd() && !enemyTable.ISRefugee())
					{
						m_Check.Check(StageCheckType.KillEnemy, enemyTable.m_Idx);
						m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemyTable.m_Type);
						m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemyTable.m_Tribe);
						m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemyTable.m_Grade);
						CheckEnd();
					}
				}, true);
			});
		});
		yield return new WaitWhile(() => !MAIN.IS_State(MainState.TOWER));

		if (STAGE_USERINFO.Is_UseStat(StatType.Men)) AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * USERINFO.GetSkillValue(SkillKind.BattleEndMen)));
		if (STAGE_USERINFO.Is_UseStat(StatType.Hyg)) AddStat(StatType.Hyg, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) * USERINFO.GetSkillValue(SkillKind.BattleEndHyg)));
		if (STAGE_USERINFO.Is_UseStat(StatType.Sat)) AddStat(StatType.Sat, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) * USERINFO.GetSkillValue(SkillKind.BattleEndSat)));
		AddStat(StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * USERINFO.GetSkillValue(SkillKind.BattleEndHp)));
		//체력 세팅
		DLGTINFO?.f_RfHPUI?.Invoke(STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetStat(StatType.HP), STAGE_USERINFO.GetMaxStat(StatType.HP));
		DLGTINFO?.f_RfHPLowUI?.Invoke(STAGE_USERINFO.GetStat(StatType.HP) < STAGE_USERINFO.GetMaxStat(StatType.HP) * 0.3f);


#else
		m_Check.Check(StageCheckType.KillEnemy, enemyTable.m_Idx);
		m_Check.Check(StageCheckType.KillEnemy_Type, (int)enemyTable.m_Type);
		m_Check.Check(StageCheckType.KillEnemy_Tribe, (int)enemyTable.m_Tribe);
		m_Check.Check(StageCheckType.KillEnemy_Grade, (int)enemyTable.m_Grade);
#endif
		IsAniPlay = true;
		m_Map.PlayAni(Item_TowerBG.AniName.Start, () => { IsAniPlay = false; }, false);

		yield return new WaitWhile(() => IsAniPlay);


#if STAGE_NO_BATTLE
		BATTLEINFO.m_Result = EBattleResult.WIN;
#endif
		if (BATTLEINFO.m_Result == EBattleResult.WIN && card.m_MapData.m_Row != m_Map.m_LastRow && info.m_TEnemyData.m_RewardGID != 0)
		{
			int lv = info.m_TEnemyData.m_RewardLV;
			int rewardgid = info.m_TEnemyData.m_RewardGID;
			bool cancanble = info.m_TEnemyData.m_RewardCancle;
			bool allgroup = info.m_TEnemyData.m_AllGroup;
			yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup);
		}

		yield return new WaitWhile(() => IS_SelectAction_Pause());

		IsAniPlay = true;
		card.PlayAni(Item_Tower_Element.State.Deactive, 0, () => { IsAniPlay = false; });
		yield return new WaitWhile(() => IsAniPlay);
		//card.gameObject.SetActive(false);

		//IsAniPlay = true;
		//bool isNextEnemy = m_Map.CreateEnemyCard(card.m_Pos, () => { IsAniPlay = false; });
		//if (isNextEnemy) yield return new WaitWhile(() => IsAniPlay);
	}

	/// <summary>HP / 정신력 / 포만감/ 청결도 중 선택한 하나의 스테이터스가 30% 회복됨피난민 카드 형식으로 연출 출력되며, 선택 시 즉시 회복 적용 </summary>
	IEnumerator SelectAction_CardProc_Refugee(Item_Tower_Element card) {
		//카드테이블에서 피난민인것만 추림
		List<TStageCardTable> refugeetables = TDATA.GetStageCardGroup(1).FindAll(t => t.m_Type == StageCardType.Enemy && TDATA.GetEnemyTable(Mathf.RoundToInt(t.m_Value1)).ISRefugee());
		bool select = false;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tower_RefugeeReward, (res, obj) => {
			TStageCardTable enemycard = obj.GetComponent<Tower_RefugeeReward>().m_SelectCardTable;
			TEnemyTable enemytable = TDATA.GetEnemyTable(Mathf.RoundToInt(enemycard.m_Value1));
			int value = 0;
			switch (enemytable.m_Type) {
				case EEnemyType.MenRefugee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.MenRefugeeCharge));
					AddStat(StatType.Men, value, true, card.m_Info);
					break;
				case EEnemyType.HygRefugee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.HygRefugeeCharge));
					AddStat(StatType.Hyg, value, true, card.m_Info);
					break;
				case EEnemyType.SatRefugee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.SatRefugeeCharge));
					AddStat(StatType.Sat, value, true, card.m_Info);
					break;
				case EEnemyType.HpRefugee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * TDATA.GetConfig_Float(ConfigType.HpRefugeeCharge));
					AddStat(StatType.HP, value, true, card.m_Info);
					break;
				case EEnemyType.MenInfectee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.MenInfecteeCharge));
					AddStat(StatType.Men, -value, true, card.m_Info);
					break;
				case EEnemyType.HygInfectee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.HygInfecteeCharge));
					AddStat(StatType.Hyg, -value, true, card.m_Info);
					break;
				case EEnemyType.SatInfectee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.SatInfecteeCharge));
					AddStat(StatType.Sat, -value, true, card.m_Info);
					break;
				case EEnemyType.HpInfectee:
					value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * TDATA.GetConfig_Float(ConfigType.HpInfecteeCharge));
					AddStat(StatType.HP, -value, true, card.m_Info);
					break;
			}
			select = true;
		}, refugeetables);

		yield return new WaitWhile(() => select == false);
		////보상선택창에서 
		////그중 하나 뽑기
		//yield return SelectAction_Reward((t) => {
		//	if (t != null) {
		//		TStageCardTable cardtable = rewardcards[t];
		//		TEnemyTable enemytable = TDATA.GetEnemyTable(Mathf.RoundToInt(cardtable.m_Value1));
		//		int value = 0;
		//		switch (enemytable.m_Type) {
		//			case EEnemyType.MenRefugee:
		//				value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) * TDATA.GetConfig_Float(ConfigType.MenRefugeeCharge));
		//				AddStat(StatType.Men, value);
		//				break;
		//			case EEnemyType.HygRefugee:
		//				value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) * TDATA.GetConfig_Float(ConfigType.HygRefugeeCharge));
		//				AddStat(StatType.Hyg, value);
		//				break;
		//			case EEnemyType.SatRefugee:
		//				value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) * TDATA.GetConfig_Float(ConfigType.SatRefugeeCharge));
		//				AddStat(StatType.Sat, value);
		//				break;
		//			case EEnemyType.HpRefugee:
		//				value = Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * TDATA.GetConfig_Float(ConfigType.HpRefugeeCharge));
		//				AddStat(StatType.HP, value);
		//				break;
		//		}
		//	}
		//});
	}
	/// <summary> HP / 정신력 / 포만감/ 청결도가 20%씩 회복 </summary>
	IEnumerator SelectAction_CardProc_Rest(Item_Tower_Element card) {
		for(StatType i =  StatType.Men; i <= StatType.HP; i++) {
			int value = Mathf.RoundToInt(m_User.GetMaxStat((StatType)i) * (i == StatType.HP ? card.m_Info.m_TData.m_Value1 : card.m_Info.m_TData.m_Value2));
			AddStat(i, value);
		}
		yield break;
	}
	/// <summary> 이로운 카드 중 하나를 선택, 전투보상처럼 목록에서 3개 뽑고 그중 한개 선택임 </summary>
	IEnumerator SelectAction_CardProc_SupplyBox(Item_Tower_Element card) {
		bool endreward = false;
		Predicate<TIngameRewardTable> checkBattleReward = CheckBattleReward;
		m_MainUI.GuideCardLoop(false);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Reward, (result, obj) => {
			//result는 스테이지 카드 인덱스
			//if(TDATA.GetStageCardTable(result).m_Type == StageCardType.Gamble) {
			//	SelectAction_StageCardProc_Gamble(obj.GetComponent<Stage_Reward>().m_SelectReward.m_GambleIdx);
			//}
			endreward = true;
			m_MainUI.GuideCardLoop(true);
		}, new int[] { Mathf.RoundToInt(card.m_Info.m_TData.m_Value1) }, 0, checkBattleReward, true, true);
		yield return new WaitWhile(() => !endreward);
	}
	/// <summary> 회복류 버프 카드중 랜덤으로 뽑음</summary>
	IEnumerator SelectAction_CardProc_Status(Item_Tower_Element card, TStageCardTable _cardtable) {
		int value = 0;

		GameObject popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tower_Single, null, _cardtable).gameObject;

		yield return new WaitWhile(() => popup != null);

		if (_cardtable.IS_BuffCard()) {
			SetBuff(EStageBuffKind.Stage, _cardtable.m_Idx);
			if (_cardtable.m_Type == StageCardType.LevelUp) STAGE_USERINFO.StatReset();
		}
		else {
			switch (_cardtable.m_Type) {
				case StageCardType.RecoveryHp:
					value = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * _cardtable.m_Value1);
					AddStat(StatType.HP, value);
					break;
				case StageCardType.RecoveryMen:
					value = Mathf.RoundToInt(_cardtable.m_Value1);
					AddStat(StatType.Men, value);
					break;
				case StageCardType.RecoveryHyg:
					value = Mathf.RoundToInt(_cardtable.m_Value1);
					AddStat(StatType.Hyg, value);
					break;
				case StageCardType.RecoverySat:
					value = Mathf.RoundToInt(_cardtable.m_Value1);
					AddStat(StatType.Sat, value);
					break;
				case StageCardType.PerRecoveryMen:
					value = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * _cardtable.m_Value1);
					AddStat(StatType.Men, value);
					break;
				case StageCardType.PerRecoveryHyg:
					value = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * _cardtable.m_Value1);
					AddStat(StatType.Hyg, value);
					break;
				case StageCardType.PerRecoverySat:
					value = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * _cardtable.m_Value1);
					AddStat(StatType.Sat, value);
					break;
			}
		}
	}
	/// <summary> OpenEvent 중 랜덤으로 1개 발동 </summary>
	IEnumerator SelectAction_CardProc_OpenEvent(Item_Tower_Element card) {
		TTowerEventTable randeventtable = TDATA.GetTowerEventRandTable(Mathf.RoundToInt(card.m_Info.m_TData.m_Value1), TowerSOType.Open);
		TStageCardTable cardtable = TDATA.GetStageCardTable(randeventtable.m_Val);

		GameObject popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TowerRandEvent, (result, obj) => {
			card.SetData(cardtable, card.m_MapData);
			popup = null;
		}, cardtable, card.m_MapData, TowerSOType.Open).gameObject;

		yield return new WaitWhile(() => popup != null);

		yield return SelectAction_CardProc(card);// SelectAction(randcard);
	}
	IEnumerator SelectAction_CardProc_SecrectEvent(Item_Tower_Element card) {
		TTowerEventTable randeventtable = TDATA.GetTowerEventRandTable(Mathf.RoundToInt(card.m_Info.m_TData.m_Value1), TowerSOType.Secret);
		//int cardtableidx = 0;//randeventtable.m_Val;
		//randeventtable.m_Val가 0이면 보스 제외 남은 적 카드들중 하나 복사해서 전투시켜주면 된다
		GameObject popup = null;
		TStageCardTable cardtable = null;
		if (randeventtable.m_Type == TowerSOEventType.SuddenAttack) {
			Item_Tower_Element sdatkenemy = m_Map.m_EnemyCard[UTILE.Get_Random(0, m_Map.m_EnemyCard.Count)];

			popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TowerRandEvent, (result, obj) => {
				card.SetData(sdatkenemy.m_Info.m_TData, card.m_MapData);
				popup = null;
			}, sdatkenemy.m_Info.m_TData, card.m_MapData,TowerSOType.Secret).gameObject;
		}
		else {
			TStageCardTable precardtable = TDATA.GetStageCardTable(randeventtable.m_Val);
			cardtable = precardtable;
			//생존스텟 카드는 이미지 추출 위해 미리 뽑아서 넘김
			if (cardtable.m_Type == StageCardType.Tower_Status) {
				TIngameRewardTable rewardtable = TDATA.GetPickIngameReward(Mathf.RoundToInt(cardtable.m_Value1), 0, CheckBattleReward);
				cardtable = TDATA.GetStageCardTable(rewardtable.m_Val);
			}
			popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.TowerRandEvent, (result, obj) => {
				card.SetData(precardtable, card.m_MapData);
				popup = null;
			}, cardtable, card.m_MapData, TowerSOType.Secret).gameObject;
		}

		yield return new WaitWhile(() => popup != null);
		yield return SelectAction_CardProc(card, cardtable);// SelectAction(card);

	}

	bool CheckBattleReward(TIngameRewardTable reward)
	{
		TStageCardTable tcard = TDATA.GetStageCardTable(reward.m_Val);
		if (tcard == null) return false;
		if (reward.m_Prob < 1) return false;
		switch (tcard.m_Type)
		{
		case StageCardType.Synergy:
			if (!m_User.IS_CreateSynergy()) return false;
			break;
		}
		return !tcard.m_IsEndType;
	}

	IEnumerator Action_BattleReward(int groupid, int lv, bool _cancancle, bool _allgroup) {
		yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);
		bool endreward = false;
		Predicate<TIngameRewardTable> checkBattleReward = CheckBattleReward;
		m_MainUI.GuideCardLoop(false);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Reward, (result, obj) => {
			//result는 스테이지 카드 인덱스
			//if (TDATA.GetStageCardTable(result).m_Type == StageCardType.Gamble) {
			//	SelectAction_StageCardProc_Gamble(obj.GetComponent<Stage_Reward>().m_SelectReward.m_GambleIdx);
			//}
			endreward = true;
			m_MainUI.GuideCardLoop(true);
		}, new int[] { groupid }, lv, checkBattleReward, _cancancle, _allgroup);
		yield return new WaitWhile(() => !endreward);
	}

	IEnumerator SelectAction_Reward(Action<Item_Reward_Card> _cb) {
		Item_Reward_Card card = null;

		while (card == null) {
			if (TouchCheck()) {
				Vector2 worldpos = Utile_Class.GetWorldPosition(Input.mousePosition);
				RaycastHit2D[] hit = Physics2D.RaycastAll(worldpos, Vector2.zero);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					card = hitobj.GetComponent<Item_Reward_Card>();
					if (card == null) continue;
					break;
				}
			}
			yield return null;
		}

		_cb?.Invoke(card);
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
			case StageCardType.AddGuard:
				val = Mathf.RoundToInt(resultcardtable.m_Value1);
				m_User.m_Stat[(int)StatType.Guard, 0] += (float)val;
				if(val > 0) PlayEffSound(SND_IDX.SFX_0461);
				else if (val < 0) PlayEffSound(SND_IDX.SFX_0472);
				break;
			case StageCardType.AddRerollCount:
				val = Mathf.RoundToInt(resultcardtable.m_Value1);
				m_User.m_leftReRollCnt += val;
				if(val > 0) PlayEffSound(SND_IDX.SFX_0462);
				else if (val < 0) PlayEffSound(SND_IDX.SFX_0470);
				break;
		}
	}
}
