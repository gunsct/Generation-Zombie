using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	/// <summary> 모드별 카운트나 딜레이 용도</summary>
	public Dictionary<PlayType, int> m_ModeCnt = new Dictionary<PlayType, int>();

	/// <summary> "N 턴 마다 공습 발생, 필드의 적과 플레이어에게 % 데미지 (턴 수 고정) </summary>
	IEnumerator Mode_FieldAirstrike(TStageTable.StagePlayType _mode) {
		if (m_ModeCnt[PlayType.FieldAirstrike] > 0) {
			m_ModeCnt[PlayType.FieldAirstrike]--;
			yield break;
		}
		STAGE.m_MainUI.RefreshModeAlarm(PlayType.FieldAirstrike, -1);
		if (m_User.m_TurnCnt != 0 && STAGE.m_MainUI.GetModeAlarm(PlayType.FieldAirstrike).m_Val[0] == 0) {
			AutoCamPosInit = false;
			yield return IE_CamAction(CamActionType.Zoom_Out);
			List<Item_Stage> areacards = new List<Item_Stage>();
			List<Item_Stage> ActiveCards = new List<Item_Stage>();
			for (int j = 0, End = Math.Min(3, m_ViewCard[j].Length); j <= AI_MAXLINE; j++, End += 2) {
				for (int i = 0; i < End; i++) {
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					if (item.IS_Die()) continue;
					StageCardInfo info = item.m_Info;
					if (!info.IS_DmgTarget()) continue;
					ActiveCards.Add(item);
					areacards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
						areacards.Remove(item);
					});
				}
			}

			yield return new WaitWhile(() => areacards.Count > 0);

			PlayEffSound(SND_IDX.SFX_0507);
			// 이펙트
			Vector3 pos = new Vector3(0, BaseValue.STAGE_INTERVER.y * 2 * m_Stage.Panel[1].lossyScale.y, 0f) + m_Stage.Panel[1].position;
			StartEff(pos, "Effect/Stage/Eff_ChSkill_AirStrike");
			yield return IE_CamAction(CamActionType.Shake_2, 0.85f, 0.8f);
			CamAction(CamActionType.Shake_1);

			int dmg = Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * _mode.m_Val[1] / 100);
			//시너지
			float? synergy1 = m_User.GetSynergeValue(JobType.Soldier, 0);
			if (synergy1 != null) {
				dmg = Mathf.RoundToInt(dmg * (float)(1f - synergy1));
				STAGE_USERINFO.ActivateSynergy(JobType.Soldier);
				Utile_Class.DebugLog_Value("Soldier 시너지 발동 " + "변화 전 -> 후 : 전 :" + (Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * _mode.m_Val[1] / 100)).ToString() + " 후 : " + dmg.ToString());
				//m_User.m_SynergyUseCnt[JobType.Soldier]++;
			}
			//플레이어 피격
			if (dmg > 0) {
				DuelDamageFontFX(m_CenterChar.transform.position, -dmg);
				AddStat(StatType.HP, -dmg, -dmg > 0);
			}
			//스테이지 카드들 피격
			List<Item_Stage> chains = new List<Item_Stage>();
			// 타겟 데미지 주기
			for (int i = ActiveCards.Count - 1; i > -1; i--) {
				Item_Stage item = ActiveCards[i];
				if (item == null) continue;
				if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
				if (item.IS_Die()) continue;
				StageCardInfo info = item.m_Info;

				if (info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = 0;
					chains.Add(item);
				}
				else if (info.IS_DmgTarget()) {
					dmg = Mathf.RoundToInt(info.GetMaxStat(EEnemyStat.HP) * (float)(_mode.m_Val[1] / 100f));
					item.SetDamage(false, dmg);
				}
			}

			yield return Action_TargetOff(ActiveCards);

			if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

			ActiveCards.Clear();
			for (int i = 0; i < 3; i++) {
				Item_Stage card = m_ViewCard[0][i];
				if (card == null) continue;
				if (card.IS_Die()) continue;
				ActiveCards.Add(card);
				card.Action(EItem_Stage_Card_Action.Scale, 0f, (obj) => {
					//obj.m_Target = null;//타겟있으면 터치가 안되?
					ActiveCards.Remove(obj);
					obj.TW_ScaleBumping(true);
				});
			}

			//yield return new WaitWhile(() => ActiveCards.Find((t) => t.IS_NoAction == false) != null);
			yield return new WaitWhile(() => ActiveCards.Count > 0);
			STAGE.m_MainUI.RefreshModeAlarm(PlayType.FieldAirstrike, _mode.m_Val[0]);
			yield return IE_CamAction(CamActionType.Zoom_OutToIdle);
			AutoCamPosInit = true;
		}
		yield break;
	}

	List<Item_Stage_Char> m_BanChars = new List<Item_Stage_Char>();
	int m_StageExceptCharCnt = 0;
	/// <summary> 캐릭터들 행동불능 체크해서 중복 제외하고 락걸어줌 </summary>
	void Mode_BanChars() {
		List<Item_Stage_Char> chars = new List<Item_Stage_Char>();
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			chars.Add(m_Chars[i]);
		}
		//stage_except 테이블에 있는거 먼저 잠그고 그 카운트 만큼 빼주기
		m_StageExceptCharCnt = 0;
		TStageExceptTable exceptdata = TDATA.GeTStageExceptTable(STAGEINFO.m_Idx, (StageDifficultyType)USERINFO.GetDifficulty());
		if (exceptdata != null) {
			List<int> exceptidxs = exceptdata.m_Chars;
			for (int i = chars.Count - 1; i > -1; i--) {
				if (exceptidxs.Contains(chars[i].m_Info.m_Idx)) {
					if (!m_BanChars.Contains(chars[i])) {
						m_BanChars.Add(chars[i]);
						chars.Remove(chars[i]);
						m_StageExceptCharCnt++;
					}
				}
			}
		}

		for (int i = 0; i < STAGEINFO.m_TStage.m_PlayType.Count; i++) {
			switch (STAGEINFO.m_TStage.m_PlayType[i].m_Type) {
				case PlayType.BanActive:
					for (int j = chars.Count - 1; j > -1; j--) {
						if (!m_BanChars.Contains(chars[j])) {
							m_BanChars.Add(chars[j]);
							chars.Remove(chars[j]);
						}
					}
					break;
				case PlayType.HighCharOut://캐릭터 중 가장 전투력이 높은 캐릭터 N명 행동 불가
					//for (int j = 0; j < chars.Count; j++) chars[i].m_Info.GetCombatPower();
					chars.Sort((Item_Stage_Char _a, Item_Stage_Char _b) => {
						if (_a.m_Info.m_CP > _b.m_Info.m_CP) return -1;
						else if (_a.m_Info.m_CP < _b.m_Info.m_CP) return 1;
						return 0;
					});
					for (int j = Math.Min(STAGEINFO.m_TStage.m_PlayType[i].m_Val[0] - m_StageExceptCharCnt, chars.Count) - 1; j > -1; j--) {
						if (!m_BanChars.Contains(chars[j])) {
							m_BanChars.Add(chars[j]);
							chars.Remove(chars[j]);
						}
					}
					break;
				case PlayType.LowCharOut://캐릭터 중 가장 전투력이 낮은 캐릭터 N명 행동 불가
					//for (int j = 0; j < chars.Count; j++) chars[i].m_Info.GetCombatPower();
					chars.Sort((Item_Stage_Char _a, Item_Stage_Char _b) => {
						if (_a.m_Info.m_CP > _b.m_Info.m_CP) return 1;
						else if (_a.m_Info.m_CP < _b.m_Info.m_CP) return -1;
						return 0;
					});
					for (int j = Math.Min(STAGEINFO.m_TStage.m_PlayType[i].m_Val[0] - m_StageExceptCharCnt, chars.Count) - 1; j > -1; j--) {
						if (!m_BanChars.Contains(chars[j])) {
							m_BanChars.Add(chars[j]);
							chars.Remove(chars[j]);
						}
					}
					break;
				case PlayType.RandomCharOut://랜덤 캐릭터 N명 행동 불가
					for (int j = Math.Min(STAGEINFO.m_TStage.m_PlayType[i].m_Val[0] - m_StageExceptCharCnt, chars.Count)  - 1; j > -1; j--) {
						int randpos = UTILE.Get_Random(0, chars.Count);
						if (!m_BanChars.Contains(chars[randpos])) {
							m_BanChars.Add(chars[randpos]);
							chars.Remove(chars[randpos]);
						}
					}
					break;
			}
		}

		for (int i = 0; i < m_BanChars.Count; i++) {
			m_BanChars[i].BanChar(true);
			m_BanChars[i].SetAPUI(0);
		}
	}

	/// <summary> 카드 세팅이 다 끝난 후 cardlock 모드면 0라인 3장중 N장 선택불가 </summary>
	IEnumerator Mode_CardLock() {
		List<Item_Stage> actioncards = new List<Item_Stage>();
		List<int> pos = new List<int>();
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			if (!m_ViewCard[0][i].IS_Lock && !m_ViewCard[0][i].m_Info.IS_RoadBlock)
				pos.Add(i);
		}

		if (pos.Count < 2) yield break;

		for (int i = 0; i < Mathf.Min(STAGEINFO.m_TStage.GetMode(PlayType.CardLock).m_Val[0], pos.Count); i++) {
			int randpos = pos[UTILE.Get_Random(0, pos.Count)];
			pos.Remove(randpos);
			Item_Stage card = m_ViewCard[0][randpos];
			if (card == null) continue;
			actioncards.Add(card);
			card.Action(EItem_Stage_Card_Action.Lock, 0, (obj) => {
				actioncards.Remove(obj);
			});
		}
		yield return new WaitWhile(() => actioncards.Count > 0);
	}
	/// <summary> 카드 세팅 다 끝난 후 easycardlock 모드면 0라인 3장중 이전에 선택한 방향 1장 선택 불가, 최초는 랜덤 </summary>
	IEnumerator Mode_EasyCardLock(bool _first = false) {
		List<Item_Stage> actioncards = new List<Item_Stage>();
		List<int> pos = new List<int>();
		int lockpos = 0;
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			if (!m_ViewCard[0][i].IS_Lock && !m_ViewCard[0][i].m_Info.IS_RoadBlock)
				pos.Add(i);
		}

		if (pos.Count < 2) yield break;

		if (_first) lockpos = pos[UTILE.Get_Random(0, pos.Count)];
		else if (pos.Contains(m_PreSelectPos)) lockpos = m_PreSelectPos;
		else yield break;

		Item_Stage card = m_ViewCard[0][lockpos];
		if (card == null) yield break;

		actioncards.Add(card);
		card.Action(EItem_Stage_Card_Action.Lock, 0, (obj) => {
			actioncards.Remove(obj);
		});

		yield return new WaitWhile(() => actioncards.Count > 0);
	}

	class StreetPosScale
	{
		public Vector2 pivot;
		public int size;
		public bool IS_IN(StreetPosScale _ps) {
			float y1 = pivot.x;
			float y2 = _ps.pivot.x;
			float x1 = pivot.y - pivot.x * 2 + 1;
			float x2 = _ps.pivot.y - _ps.pivot.x * 2 + 1;
			float s1 = size;
			float s2 = _ps.size;
			if (y1 > y2) {
				if (x1 > x2) {//3사분면
					if (x1 - s1 <= x2 + s2 && y1 - s1 <= y2 + s2)
						return true;
				}
				else {//4사분면
					if (x1 + s1 >= x2 - s2 && y1 + s1 <= y2 + s2)
						return true;
				}
			}
			else {
				if (x1 > x2) {//2사분면
					if (x1 - s1 <= x2 + s2 && y1 + s1 >= y2 - s2)
						return true;
				}
				else {//1사분면
					if (x1 + s1 >= x2 - s2 && y1 + s1 >= y2 - s2)
						return true;
				}
			}
			return false;
		}
	}
	/// <summary> 어둠 스테이지에서 랜덤 위치로 카드를 밝힘. (PlayTypeValue 01 = 가로등 생성 개수, PlayTypeValue 02 = 가로등 발생 턴 수) </summary>
	IEnumerator Mode_StreetLight(TStageTable.StagePlayType _mode) {
		STAGE.m_MainUI.RefreshModeAlarm(PlayType.StreetLight, 0);
		List<StreetPosScale> effps = new List<StreetPosScale>();
		List<List<Vector2>> pos = new List<List<Vector2>>() {
			new List<Vector2>() { new Vector2(4, 5)},
			new List<Vector2>() { new Vector2(4, 3), new Vector2(4, 7)},
			new List<Vector2>() { new Vector2(4, 2), new Vector2(4, 5), new Vector2(4, 8) } }
		;
		//mask _AddAlpha 0이 1칸 0.3 3칸
		int cnt = _mode.m_Val[0];
		for (int i = 0; i < cnt; i++) {
			StreetPosScale ps = new StreetPosScale();
			int scale = _mode.m_Val[1];
			ps.pivot = pos[cnt - 1][i] + new Vector2(UTILE.Get_Random(-1, 2), UTILE.Get_Random(-1, 2));
			ps.pivot.x = Mathf.Clamp(ps.pivot.x, 0, AI_MAXLINE);
			ps.pivot.y = Mathf.Clamp(ps.pivot.y, 0, m_ViewCard[(int)ps.pivot.x].Length - 1);

			//ps.pivot = Vector2.zero;
			//ps.size = scale < 2 ? 1 : 2;
			//while (ps.pivot == Vector2.zero) {
			//	int line = Mathf.Min(AI_MAXLINE, UTILE.Get_Random(3, 5));
			//	int pos = UTILE.Get_Random(1, line * 2 + 2);
			//	ps.pivot = new Vector2(line, pos);
			//	StreetPosScale effin = effps.Find(o => o.IS_IN(ps));
			//	if (effin != null)
			//		ps.pivot = Vector3.zero;
			//}

			if (ps.pivot.x > AI_MAXLINE) continue;
			//라이트 설정 생성
			LightInfo lightinfo = new LightInfo(LightMode.StreetLight, scale < 3 ? 1 : 2);
			lightinfo.SetTarget((int)ps.pivot.x, (int)ps.pivot.y);
			lightinfo.SetTurn(99999);
			//이펙트 등록
			GameObject Eff = StartEff(m_ViewCard[(int)ps.pivot.x][(int)ps.pivot.y].transform, string.Format("Effect/Stage/BG_StreetLight_0{0}", UTILE.Get_Random(1, 3)), true);
			Eff.transform.localScale = Vector3.one;
			lightinfo.SetEff(Eff);
			//라이트 추가
			AddLight(lightinfo);

			//원형가로등추가
			StreetLightInfo streetlightinfo = new StreetLightInfo();
			streetlightinfo.SetTarget((int)ps.pivot.x, (int)ps.pivot.y);
			streetlightinfo.SetTurn(99999);
			//이펙트 등록
			Eff = UTILE.LoadPrefab("Effect/Stage/Eff_StageCard_StreetLight", true, STAGE.m_DarkMaskPanel);
			Eff.transform.position = m_ViewCard[(int)ps.pivot.x][(int)ps.pivot.y].transform.position;
			Eff.transform.GetChild(0).transform.localScale = new Vector3(10f + 5f * (scale < 3 ? scale : scale + 1), 12f + 6f * (scale < 3 ? scale : scale + 1), 1f);
			Eff.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_AddAlpha", 0);
			//Eff.transform.localScale = Vector3.one * scales[_mode.m_Val[1]][UTILE.Get_Random(0, scales[_mode.m_Val[1]].Count)];
			streetlightinfo.SetEff(Eff);
			//라이트 추가
			AddStreetLightInfo(streetlightinfo);

			effps.Add(ps);
		}

		//STAGE.m_MainUI.RefreshModeAlarm(PlayType.StreetLight, _mode.m_Val[1]);

		yield break;
	}

	IEnumerator Mode_FireSpread(TStageTable.StagePlayType _mode) {
		if (m_ModeCnt[PlayType.FireSpread] > 0) {
			m_ModeCnt[PlayType.FireSpread]--;
			yield break;
		}
		STAGE.m_MainUI.RefreshModeAlarm(PlayType.FireSpread, -1);
		if (m_User.m_TurnCnt != 0 && STAGE.m_MainUI.GetModeAlarm(PlayType.FireSpread).m_Val[0] == 0) {
			yield return SelectAction_StageCardAI_Fire();
			STAGE.m_MainUI.RefreshModeAlarm(PlayType.FireSpread, _mode.m_Val[0]);
		}

		// 타겟 설정에서 꺼져있으므로 켜준다.
		List<Item_Stage> activecards = new List<Item_Stage>();
		for (int i = 0; i < 3; i++) {
			Item_Stage card = m_ViewCard[0][i];
			if (card == null) continue;
			if (card.IS_Die()) continue;
			activecards.Add(card);
			card.Action(EItem_Stage_Card_Action.Scale, 0f, (obj) => {
				//obj.m_Target = null;//타겟있으면 터치가 안되?
				obj.TW_ScaleBumping(true);
				activecards.Remove(obj);
			});
		}
		yield return new WaitWhile(() => activecards.Count > 0);
	}
	IEnumerator Mode_CardShuffle(TStageTable.StagePlayType _mode) {
		if (m_ModeCnt[PlayType.CardShuffle] > 0) {
			m_ModeCnt[PlayType.CardShuffle]--;
			yield break;
		}
		STAGE.m_MainUI.RefreshModeAlarm(PlayType.CardShuffle, -1);
		if (m_User.m_TurnCnt != 0 && STAGE.m_MainUI.GetModeAlarm(PlayType.CardShuffle).m_Val[0] == 0) {
			yield return Action_AllShuffle(m_ViewCard[0][1]);
			STAGE.m_MainUI.RefreshModeAlarm(PlayType.CardShuffle, _mode.m_Val[0]);
		}
	}
}
