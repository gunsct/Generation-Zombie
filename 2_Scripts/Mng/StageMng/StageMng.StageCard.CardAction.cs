using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	public enum CardActionDamageType {
		None = 0,
		Damage,
		Die
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Card Process 필요 함수
	void ShowArea(bool Active, List<Item_Stage> cards = null) {
		m_Area.transform.position = m_Stage.Panel[1].transform.position;
		m_Area.Clear();
		if (Active) {
			m_Area.AddCard(cards);
			m_Area.Show();
		}
	}

	void SetView_Area_Arc(Item_Stage card, int MaxLine, bool Active, int StartLine = 1) {
		if (card) {
			// 라인까지 보여준다.
			for (int j = StartLine, Start = card.m_Pos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
				for (int i = Start; i < End; i++) {
					Item_Stage TempCard = m_ViewCard[j][i];
					if (TempCard == null) continue;
					if (i < Start || i >= End) TempCard.ActiveDark(true);
					else TempCard.ActiveDark(!Active);
				}
			}
		}
		else {
			// 라인까지 보여준다.
			for (int j = StartLine; j <= MaxLine; j++) {
				for (int i = 0; i < m_ViewCard[j].Length; i++) {
					Item_Stage TempCard = m_ViewCard[j][i];
					if (TempCard == null) continue;
					TempCard.ActiveDark(!Active);
				}
			}
		}
	}

	List<Item_Stage> SetView_Area_Target(Item_Stage card, int MaxLine, int StartLine = 1) {
		List<Item_Stage> activecards = new List<Item_Stage>();
		List<Item_Stage> offcards = new List<Item_Stage>();
		StageCardType type = card.m_Info.m_TData.m_Type;
		int startpos = StartLine == 0 ? 0 : card.m_Pos;
		switch (type) {
			case StageCardType.Sniping:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (!item.m_Info.IS_DmgTarget() && !item.m_Info.IS_ExplosionTarget()) continue;
						if (!item.m_Info.IsDark && item.m_Info.ISNotAtkRefugee) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.RandomAtk:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (!item.m_Info.IS_DmgTarget() && !item.m_Info.IS_ExplosionTarget()) continue;
						if (!item.m_Info.IsDark && item.m_Info.ISNotAtkRefugee) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.StopCard:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (!item.m_Info.IS_DmgTarget()) continue;
						if (!item.m_Info.IsDark && item.m_Info.ISRefugee) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.DownLevel:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						if (!info.IS_AIEnemy() || j == 0) {
							if (j == 0) offcards.Add(item);
							continue;
						}
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.Dynamite:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						if (item.m_Info.IS_Boss && !item.m_Info.m_TData.m_IsEndType) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.LightStick:
			case StageCardType.StarShell:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.FlashLight:
				for (int i = 0; i < m_ViewCard[StartLine].Length; i++) {
					Item_Stage item = m_ViewCard[StartLine][i];
					if (item == null) continue;
					if (item.IS_Die()) continue;
					if (item.IS_Lock) continue;
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
				break;
			case StageCardType.PowderExtin:
			case StageCardType.ThrowExtin:
			case StageCardType.PowderBomb:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.m_NowTData.m_Type != StageCardType.Fire) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.TimeBomb:
			case StageCardType.Grenade:
			case StageCardType.ShockBomb:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						if (item.m_Info.IS_Boss && !item.m_Info.m_TData.m_IsEndType) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.FireBomb:
			case StageCardType.FireGun:
			case StageCardType.NapalmBomb:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.IS_Boss && !item.m_Info.m_TData.m_IsEndType) continue;
						if (!item.m_Info.IS_BurnTarget()) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.Drill:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (!item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						if (item.m_Info.m_NowTData.m_Type == StageCardType.AllRoadblock) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.CardPull:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						if (j < StartLine + 1) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						if (tdata.m_Type == StageCardType.Chain) continue;
						if (info.IS_Boss || tdata.IS_LineCard())//보스와 첫줄 제외
						{
							if (j == 0) offcards.Add(item);
							continue;
						}
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.Explosion:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 피난민은 저격 불가
						// 피난민은 저격 불가
						switch (tdata.m_Type) {
							case StageCardType.ShockBomb:
							case StageCardType.Grenade:
							case StageCardType.Dynamite:
							case StageCardType.OldMine:
							case StageCardType.Allymine:
							case StageCardType.TimeBomb:
							case StageCardType.C4:
							case StageCardType.FireBomb:
							case StageCardType.NapalmBomb:
								break;
							default:
								if (j == 0) offcards.Add(item);
								continue;
						}
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.PlusMate:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 찢긴 시체 아니면 제외
						if (tdata.m_Type != StageCardType.Material) {
							if (j == 0) offcards.Add(item);
							continue;
						}
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			case StageCardType.AirStrike:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						if (!info.IS_DmgTarget() && !info.IS_ExplosionTarget()) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			default:
				for (int j = StartLine, Start = startpos, End = Math.Min(Start + 3, m_ViewCard[j].Length); j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						if (!info.IS_DmgTarget()) continue;
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
		}

		return activecards;
	}

	public Vector3 GetStage_EffPos(Vector3 v3TargetPos) {
		// z의 -5 지점이기때문에 맞춰서 이펙트들이 커짐
		// 0지점의 위치를 찾아준다.

		// 스크린의 좌표을 알아낸다.
		Vector2 screenpos = Utile_Class.GetCanvasPosition(v3TargetPos);

		// 스크린좌표로부터 Ray 구해준다.
		Ray ray = m_MyCam.ScreenPointToRay(screenpos);

		// 0 지점 알아내기
		return ray.origin + ray.direction / ray.direction.z * Mathf.Abs(ray.origin.z);
	}

	public void BoomAreaTarget(Item_Stage card, Item_Stage target, out List<Item_Stage> area, out List<Item_Stage> targets, int startline = 1, bool ai = false) {
		StageCardType cardtype = card.m_Info.m_NowTData.m_Type;
		area = new List<Item_Stage>();
		targets = new List<Item_Stage>();

		switch (cardtype) {
			case StageCardType.Grenade:
			case StageCardType.TimeBomb:
			case StageCardType.Oil:
				for (int j = target.m_Line - 1, jMax = j + 3, Start = target.m_Pos - 2; j < jMax; j++, Start++) {
					if (j < startline) continue;
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 3, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						if (info.IS_Boss && !info.m_TData.m_IsEndType) continue;
						area.Add(item);
						targets.Add(item);
					}
				}
				break;
			case StageCardType.Dynamite:
				for (int i = 0, iMax = m_ViewCard[target.m_Line].Length; i < iMax; i++) {
					Item_Stage item = m_ViewCard[target.m_Line][i];
					if (item == null) continue;
					if (item.IS_Die()) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					if (item.m_Info.m_TData.m_IsEndType) continue;
					StageCardInfo info = item.m_Info;
					if (info.IS_Boss && !info.m_TData.m_IsEndType) continue;
					area.Add(item);
					targets.Add(item);
				}
				break;
			case StageCardType.OldMine:
			case StageCardType.Allymine:
				Item_Stage UP = GetDirStageCard(m_ViewCard, EDIR.UP, 1, target.m_Line, target.m_Pos, true);
				Item_Stage LEFT = GetDirStageCard(m_ViewCard, EDIR.LEFT, 1, target.m_Line, target.m_Pos, true);
				Item_Stage DOWN = GetDirStageCard(m_ViewCard, EDIR.DOWN, 1, target.m_Line, target.m_Pos, true);
				Item_Stage RIGHT = GetDirStageCard(m_ViewCard, EDIR.RIGHT, 1, target.m_Line, target.m_Pos, true);

				bool findenemy = (UP && !UP.IS_Die() && UP.m_Info.IS_EnemyCard) || (LEFT && !LEFT.IS_Die() && LEFT.m_Info.IS_EnemyCard) ||
					(DOWN && !DOWN.IS_Die() && DOWN.m_Info.IS_EnemyCard) || (RIGHT && !RIGHT.IS_Die() && RIGHT.m_Info.IS_EnemyCard);
				if ((ai && findenemy) || !ai) {
					if (UP && !UP.IS_Die() && !UP.m_Info.IS_RoadBlock && !UP.m_Info.m_RealTData.m_IsEndType) {
						area.Add(UP);
						if (UP.m_Info.IS_OldMineTarget()) targets.Add(UP);
					}
					if (LEFT && !LEFT.IS_Die() && !LEFT.m_Info.IS_RoadBlock && !LEFT.m_Info.m_RealTData.m_IsEndType) {
						area.Add(LEFT);
						if (LEFT.m_Info.IS_OldMineTarget()) targets.Add(LEFT);
					}
					if (DOWN && !DOWN.IS_Die() && !DOWN.m_Info.IS_RoadBlock && !DOWN.m_Info.m_RealTData.m_IsEndType) {
						area.Add(DOWN);
						if (DOWN.m_Info.IS_OldMineTarget()) targets.Add(DOWN);
					}
					if (RIGHT && !RIGHT.IS_Die() && !RIGHT.m_Info.IS_RoadBlock && !RIGHT.m_Info.m_RealTData.m_IsEndType) {
						area.Add(RIGHT);
						if (RIGHT.m_Info.IS_OldMineTarget()) targets.Add(RIGHT);
					}
				}
				break;
			case StageCardType.C4://씨포는 선택이 아님
				for (int j = target.m_Line + (ai == true ? -2 : 1), jMax = j + 5, Start = target.m_Pos + (ai == true ? -4 : -1); j < jMax; j++, Start++) {
					if (j < startline) continue;
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 5, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						if (info.IS_Boss && !info.m_TData.m_IsEndType) continue;
						area.Add(item);
						targets.Add(item);
					}
				}
				break;
			case StageCardType.GasStation:
				for (int j = target.m_Line - 2, jMax = j + 5, Start = target.m_Pos - 4; j < jMax; j++, Start++) {
					if (j < startline) continue;
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 5, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						if (info.IS_Boss && !info.m_TData.m_IsEndType) continue;
						area.Add(item);
						targets.Add(item);
					}
				}
				break;
			case StageCardType.StarShell:
			case StageCardType.PowderBomb://5*5 범위
				for (int j = target.m_Line - 2, jMax = j + 5, Start = target.m_Pos - 4; j < jMax; j++, Start++) {
					if (j < startline) continue;
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 5, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						StageCardInfo info = item.m_Info;
						area.Add(item);
						targets.Add(item);
					}
				}
				break;
			case StageCardType.FireGun:
				for (int j = target.m_Line - 1, jMax = j + 3, Start = target.m_Pos - 2; j < jMax; j++, Start++) {
					if (j < startline) continue;
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 3, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						if (info.IS_Boss && !info.m_TData.m_IsEndType) continue;
						if (!item.m_Info.IS_BurnTarget()) continue;
						area.Add(item);
						targets.Add(item);
					}
				}
				break;
			case StageCardType.NapalmBomb:
				for (int j = target.m_Line - 2, jMax = j + 5, Start = target.m_Pos - 4; j < jMax; j++, Start++) {
					if (j < startline) continue;
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 5, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						if (info.IS_Boss && !info.m_TData.m_IsEndType) continue;
						if (!item.m_Info.IS_BurnTarget()) continue;
						area.Add(item);
						targets.Add(item);
					}
				}
				break;
		}
	}

	CardActionDamageType GetActionDamageType(StageCardType type) {
		switch (type) {
			case StageCardType.Sniping:
			case StageCardType.MachineGun:
			case StageCardType.Shotgun:
			case StageCardType.AirStrike:
			case StageCardType.OldMine:
			case StageCardType.Allymine:
			case StageCardType.FireBomb:
			case StageCardType.FireGun:
			case StageCardType.NapalmBomb:
				return CardActionDamageType.Damage;
			case StageCardType.ShockBomb:
			case StageCardType.Grenade:
			case StageCardType.TimeBomb:
			case StageCardType.Dynamite:
			case StageCardType.C4:
				return CardActionDamageType.Die;
		}
		return CardActionDamageType.None;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Card Process Action
	/// <summary> 2단계 : 카드 진행 </summary>
	IEnumerator SelectAction_StageCardProc(Item_Stage card, bool _pausecheck = true) {
		FirstLineBump(false);
		StageCardInfo info = card.m_Info;
		TStageCardTable tdata = info.m_TData;
		float value;
		int lv;
		int rewardgid;
		bool allgroup; 
		bool cancanble;
		StageCardType type = tdata.m_Type;
		switch (type) {
			/// <summary> 카드 생성 시 그룹 내에서 배치된 몬스터 중 하나로 결정되어 출력 </summary>
			case StageCardType.Enemy:
				if (card.m_Info.m_TEnemyData.ISRefugee()) {
					// 피난민, 보급상자등 문제가 되는 카드들을 위해 다시 확인
					yield return Check_NullCardAction();
				}
				else {
					EBattleMode battlemode = info.IS_Boss ? EBattleMode.EnemyAtk : EBattleMode.Normal;
					yield return StartBattle(battlemode, card);
					if (m_AutoPlay) break;
#if STAGE_NO_BATTLE
				BATTLEINFO.m_Result = EBattleResult.WIN;
#endif
					if (CheckEnd()) {//승리하면 패스, 사망시 이어하기 기다리기
						if (STAGEINFO.m_Result == StageResult.Clear) yield break;
						else if (STAGEINFO.m_Result == StageResult.Fail) yield return new WaitWhile(() => STAGEINFO.m_Result == StageResult.Fail);
					}
					if (BATTLEINFO.m_Result == EBattleResult.WIN) {
						card.Action(EItem_Stage_Card_Action.Die);
						lv = info.m_TEnemyData.m_RewardLV;
						rewardgid = info.m_TEnemyData.m_RewardGID;
						allgroup = info.m_TEnemyData.m_AllGroup;
						cancanble = info.m_TEnemyData.m_RewardCancle;
						yield return new WaitUntil(() => card.IS_NoAction);

						if (info.m_TEnemyData.m_Grade == EEnemyGrade.Elite)
							yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup, true, -1, card.transform.position, true);
						else
							yield return Action_BattleReward_Rand(card, rewardgid, lv, allgroup);
					}

					yield return new WaitWhile(() => IS_SelectAction_Pause());
				}
				break;
			/// <summary> 저격 가능한 타겟들이 포커싱되고 해당 타겟을 선택하면 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Sniping:
				yield return SelectAction_StageCardProc_Sniping(card);
				break;
			/// <summary> 목표 카드 선택 시 해당 카드를 기점으로 3x3 범위에 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Grenade:
			case StageCardType.TimeBomb:
				yield return SelectAction_StageCardProc_Grenade(card);
				break;
			/// <summary> 십자 범위에 공격. 데미지 효과는 수류탄과 동일 [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.FireBomb:
				yield return SelectAction_StageCardProc_FireBomb(card);
				break;
			/// <summary> 십자 범위에 공격. 데미지 효과는 수류탄과 동일 [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.FireGun:
				yield return SelectAction_StageCardProc_FireGun(card);
				break;
			/// <summary> 십자 범위에 공격. 데미지 효과는 수류탄과 동일 [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.NapalmBomb:
				yield return SelectAction_StageCardProc_NapalmBomb(card);
				break;
			/// <summary> 목표 카드 선택 시 해당 카드가 포함된 행 전부를 공격한다. [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Dynamite:
				yield return SelectAction_StageCardProc_Dynamite(card);
				break;
			/// <summary> N회 입력 시 (1회 당 1턴 소모) 벽이 깨지며 다른 랜덤 카드로 변환된다. </summary>
			case StageCardType.Wall:
				break;
			/// <summary> 습득 시 아이템 획득 </summary>
			case StageCardType.SupplyBox:
				yield return SelectAction_StageCardProc_SupplyBox(card);
				break;
			case StageCardType.BigSupplyBox:
				// 피난민, 보급상자등 문제가 되는 카드들을 위해 다시 확인
				yield return Check_NullCardAction();
				break;
			/// <summary> 5x5의 범위 내에 랜덤으로 10회 공격. [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.MachineGun:
				yield return SelectAction_StageCardProc_MachineGun(card);
				break;
			/// <summary> HP 회복 </summary>
			case StageCardType.RecoveryHp:
				value = tdata.m_Value1;
				StartCoroutine(SelectAction_AddStat(card, StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * value)));
				break;
			case StageCardType.RecoveryHpPer:
				value = tdata.m_Value1;
				StartCoroutine(SelectAction_AddStat(card, StatType.HP, Mathf.RoundToInt(m_User.GetStat(StatType.Heal) * value)));
				break;
			/// <summary> 포만감 회복 </summary>
			case StageCardType.RecoverySat:
				value = tdata.m_Value1;
				StartCoroutine(SelectAction_AddStat(card, StatType.Sat, Mathf.RoundToInt(value)));
				break;
			/// <summary> 정신력 회복 </summary>
			case StageCardType.RecoveryMen:
				StartCoroutine(SelectAction_AddStat(card, StatType.Men, Mathf.RoundToInt(tdata.m_Value1)));
				break;
			/// <summary> 청결도 회복 </summary>
			case StageCardType.RecoveryHyg:
				StartCoroutine(SelectAction_AddStat(card, StatType.Hyg, Mathf.RoundToInt(tdata.m_Value1)));
				break;
			/// <summary> 정신력 퍼센트 회복 </summary>
			case StageCardType.PerRecoveryMen:
				StartCoroutine(SelectAction_AddStat(card, StatType.Men, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * tdata.m_Value1)));
				break;
			/// <summary> 청결도 퍼센트 회복 </summary>
			case StageCardType.PerRecoveryHyg:
				StartCoroutine(SelectAction_AddStat(card, StatType.Hyg, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * tdata.m_Value1)));
				break;
			/// <summary> 허기 퍼센트 회복 </summary>
			case StageCardType.PerRecoverySat:
				StartCoroutine(SelectAction_AddStat(card, StatType.Sat, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * tdata.m_Value1)));
				break;
			/// <summary> 행동력 회복 </summary>
			case StageCardType.RecoveryAP:
				yield return SelectAction_RecoveryAP(tdata.m_Value1);
				break;
			case StageCardType.AddRerollCount:
				yield return SelectAction_AddRerollCount(tdata.m_Value1);
				break;
			/// <summary> HP 최대 증가 </summary>
			case StageCardType.HpUp:
			/// <summary> 공격력 상승 </summary>
			case StageCardType.AtkUp:
			/// <summary> 방어력 상승 </summary>
			case StageCardType.DefUp:
			/// <summary> 기력 회복 속도 증가 </summary>
			case StageCardType.EnergyUp:
			/// <summary> 포만도 증가 </summary>
			case StageCardType.SatUp:
			/// <summary> 청결도 증가 </summary>
			case StageCardType.HygUp:
			/// <summary> 정신력 증가 </summary>
			case StageCardType.MenUp:
			case StageCardType.Synergy:
			/// <summary> 속도 증가 </summary>
			case StageCardType.SpeedUp:
			/// <summary> 크리티컬 확률 증가 </summary>
			case StageCardType.CriticalUp:
			/// <summary> 크리티컬 데미지 증가 </summary>
			case StageCardType.CriticalDmgUp:
			/// <summary> 행동력 회복량 증가 </summary>
			case StageCardType.APRecoveryUp:
			/// <summary> 행동력 소모량 감소 </summary>
			case StageCardType.APConsumDown:
			/// <summary> 체력 회복량 증가 </summary>
			case StageCardType.HealUp:
			/// <summary> 캐릭터, 장비 레벨 증가 </summary>
			case StageCardType.LevelUp:
			/// <summary> 타임어택 시간이 증가 </summary>
			case StageCardType.TimePlus:
			/// <summary> 헤드샷 확률 증가 </summary>
			case StageCardType.HeadShotUp:
			/// <summary> 머지슬롯 잠금, 해제 </summary>
			case StageCardType.ConMergeSlotDown:
			case StageCardType.MergeSlotCount:
				yield return SelectAction_SetBuff(card);
				//레벨업은 스텟 리셋
				if (tdata.m_Type == StageCardType.LevelUp) STAGE_USERINFO.StatReset();
				else if (tdata.m_Type == StageCardType.APRecoveryUp || tdata.m_Type == StageCardType.APConsumDown) {
					for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
						m_Chars[i].RefreshAPText();
						m_Chars[i].SetAPUI(m_User.m_AP[0]);
					}
				}
				break;
			/// <summary> 제한 턴 수 증가 </summary>
			case StageCardType.LimitTurnUp:
				yield return SelectAction_LimitTurnUp(tdata.m_Value1);
				break;
			/// <summary> 방어 횟수 </summary>
			case StageCardType.AddGuard:
				yield return SelectAction_AddGuard(tdata.m_Value1);
				//m_User.m_Stat[(int)StatType.Guard, 0] += (float)Mathf.RoundToInt(tdata.m_Value1);
				break;
			/// <summary> 5x5 범위 내 모든 카드의 위치를 랜덤하게 다시 배치한다. </summary>
			case StageCardType.AllShuffle:
				yield return SelectAction_StageCardProc_AllShuffle(card);
				break;
			/// <summary> 5x5 범위 내의 모든 카드를 다시 뽑는다. </ summary>
			case StageCardType.AllConversion:
				yield return SelectAction_StageCardProc_AllConversion(card);
				break;
			case StageCardType.LightStick:
				yield return SelectAction_StageCardProc_LightStick(card);
				break;
			case StageCardType.FlashLight:
				yield return SelectAction_StageCardProc_FlashLight(card);
				break;
			case StageCardType.DeadBody:
				if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;
				lv = info.m_TEnemyData.m_RewardLV;
				rewardgid = info.m_TEnemyData.m_RewardGID;
				allgroup = info.m_TEnemyData.m_AllGroup;
				cancanble = info.m_TEnemyData.m_RewardCancle;
				yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup);
				break;
			case StageCardType.Ash:
				yield return SelectAction_StageCardProc_Ash(card);
				break;
			case StageCardType.PowderExtin:
				yield return SelectAction_StageCardProc_PowderExtin(card);
				break;
			case StageCardType.ThrowExtin:
				yield return SelectAction_StageCardProc_ThrowExtin(card);
				break;
			case StageCardType.SmokeBomb:
				yield return SelectAction_StageCardProc_SmokeBomb(card);
				break;
			case StageCardType.Paralysis:
				yield return SelectAction_StageCardProc_Paralysis(card);
				break;
			/// <summary> 카드 생성 시 그룹 내에서 배치된 몬스터 중 하나로 결정되어 출력 </summary>
			case StageCardType.Hive:
				yield return SelectAction_StageCardProc_Hive(card);
				break;
			case StageCardType.ShockBomb:
				yield return SelectAction_StageCardProc_ShockBomb(card);
				break;
			case StageCardType.C4:
				yield return SelectAction_StageCardProc_C4(card);
				break;
			case StageCardType.Material:
				yield return SelectAction_Material(card);
				break;
			/// <summary> 샷건 카드, 전방 3x2 범위를 공격한다. </summary>
			case StageCardType.Shotgun:
				yield return SelectAction_Shotgun(card);
				break;
			/// <summary> 공중지원 카드, 액티브 스킬 공중지원과 효과가 동일하다. </summary>
			case StageCardType.AirStrike:
				yield return SelectAction_AirStrike(card);
				break;
			case StageCardType.StarShell:
				yield return SelectAction_StageCardProc_StarShell(card);
				break;
			case StageCardType.PowderBomb:
				yield return SelectAction_StageCardProc_PowderBomb(card);
				break;
			case StageCardType.Jump:
				yield return SelectAction_StageCardProc_Jump(card);
				break;
			case StageCardType.DownLevel:
				yield return SelectAction_StageCardProc_DownLevel(card);
				break;
			case StageCardType.CoolReset:
				yield return SelectAction_StageCardProc_CoolReset(card);
				break;
			case StageCardType.AllCoolReset:
				yield return SelectAction_StageCardProc_AllCoolReset(card);
				break;
			case StageCardType.AllUpAdr:
				yield return SelectAction_StageCardProc_AllUpAdr(card);
				break;
			case StageCardType.AllRecoverySrv:
				yield return SelectAction_StageCardProc_AllRecoverySrv(card);
				break;
			case StageCardType.CardPull:
				yield return SelectAction_StageCardProc_CardPull(card);
				break;
			case StageCardType.Explosion:
				yield return SelectAction_StageCardProc_Explosion(card);
				break;
			case StageCardType.BanAirStrike:
				yield return SelectAction_StageCardProc_BanAirStrike(card);
				break;
			case StageCardType.Drill:
				yield return SelectAction_StageCardProc_Drill(card);
				break;
			case StageCardType.RandomAtk:
				yield return SelectAction_StageCardProc_RandomAtk(card);
				break;
			case StageCardType.StopCard:
				yield return SelectAction_StageCardProc_StopCard(card);
				break;
			case StageCardType.PlusMate:
				yield return SelectAction_StageCardProc_PlusMate(card);
				break;
			case StageCardType.Gamble:
				TGambleCardTable gambletable = TDATA.GetGambleCardTable(card.m_Info.m_GambleIdx);
				float randprop = UTILE.Get_Random(0f, 1f);
				float? synergySD = STAGE_USERINFO.GetSynergeValue(JobType.Swindler, 1);
				if (synergySD != null) {
					randprop = Mathf.Clamp(randprop - (float)synergySD, 0f, 1f);
					STAGE_USERINFO.ActivateSynergy(JobType.Swindler);
					Utile_Class.DebugLog_Value("Swindler 도박 확률 증가");
				}
				GameObject popup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Gamble, null, gambletable, randprop).gameObject;

				yield return new WaitWhile(() => popup != null);

				yield return SelectAction_StageCardProc_Gamble(gambletable, randprop);
				break;
			case StageCardType.Supplybox02:
				yield return SelectAction_StageCardProc_SupplyBox2(card);
				break;
			case StageCardType.SaveCard:
				yield return SelectAction_SaveCard(card);
				break;
			case StageCardType.GasStation:
			case StageCardType.Oil:
				yield return SelectAction_StageCardProc_GetOilGasStation(card);
				break;
			case StageCardType.Item_RewardBox:
				yield return SelectAction_StageCardProc_Item_RewardBox(card);
				break;
		}

		// 중간 대전이 
		if(_pausecheck) yield return new WaitWhile(() => IS_SelectAction_Pause());
		FirstLineBump(true);
		//재료 얻는 경우는 제작 액션 끝날때까지 기다림
		//if (STAGE.m_MainUI != null) yield return new WaitWhile(() => STAGE.m_MainUI.GetCraftState() != Item_Stage_Make.State.None);
	}

	IEnumerator SelectAction_StageCardProc_CheckSelect(Action<Item_Stage> endcb, int startline = 1, List<Item_Stage> _targets = null) {
		Item_Stage selectcard = null;
		while (selectcard == null) {
			if (TouchCheck() && m_MoveTime > 0.3f) {
				// 선택 카드 알아내기
				Ray ray = m_MyCam.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hit = Physics.RaycastAll(ray, m_MyCam.farClipPlane);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					Item_Stage hitcard = hitobj.GetComponent<Item_Stage>();
					if (hitcard == null) continue;
					if (TUTO.TouchCheckLock(TutoTouchCheckType.StageCard, hitcard)) continue;
					if (hitcard.IS_Lock) continue;
					if (hitcard.m_Line < startline) continue;
					if (!hitcard.ISActiveCard()) continue;
					if (_targets != null && !_targets.Contains(hitcard)) continue;
					selectcard = hitcard;
					break;
				}
			}
			yield return null;
		}

		endcb?.Invoke(selectcard);
		if (POPUP.GetPopup().m_Popup == PopupName.Stage_CardUse) {
			POPUP.GetPopup().GetComponent<Stage_CardUse>().SetCancleBtn(1, false);
		}
	}
	IEnumerator SelectAction_StageCardProc_Start(Item_Stage card) {
		if (m_SkillUseInfoPopup != null) {
			// 팝업 닫아주기
			m_SkillUseInfoPopup.Close();
			m_SkillUseInfoPopup = null;
		}
		float movetime = 0.2f;//0.3->0.2
		GameObject Activepanel = m_Stage.ActionPanel.gameObject;

		iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[1], "time", movetime, "easetype", "easeOutQuad"));
		iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[1], "time", movetime, "easetype", "easeOutQuad"));
		card.GetComponent<SortingGroup>().sortingOrder = 4;
		SetBGFXSort(false);

		List<Item_Stage_Char> actionChar = new List<Item_Stage_Char>();
		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, (obj) => {
				actionChar.Remove(obj);
			}, movetime);
			actionChar.Add(m_Chars[i]);

		}

		switch (card.m_Info.m_TData.m_Type) {
			/// <summary> 저격 가능한 타겟들이 포커싱되고 해당 타겟을 선택하면 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Sniping:
			/// <summary> 목표 카드 선택 시 해당 카드를 기점으로 3x3 범위에 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Grenade:
			/// <summary> 사용 시 선택한 카드를 제거한다. </summary>
			case StageCardType.ShockBomb:
			/// <summary> 5x5 범위 내 모든 어둠을 N턴 동안 삭제한다. </summary>
			case StageCardType.StarShell:
			/// <summary> 5X5 범위 내 모든 화염을 삭제한다. </summary>
			case StageCardType.PowderBomb:
			/// <summary> 목표 카드 선택 시 해당 카드가 포함된 행 전부를 공격한다. [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Dynamite:
			/// <summary> 형광 스틱 : 지정한 특정 카드 위치로 형광 스틱을 던져 N턴 동안 3x3 범위 내 어둠을 밝혀 카드 모습을 확인시켜준다. </summary>
			case StageCardType.LightStick:
			/// <summary> 손전등 : 지정한 한 열의 어둠 카드가 N턴 동안 사라진다. </summary>
			case StageCardType.FlashLight:
			/// <summary> 분말 소화기 : 3x3 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
			case StageCardType.PowderExtin:
			/// <summary> 투척 소화기 : 투척 소화기 : 1x1 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
			case StageCardType.ThrowExtin:
			/// <summary> 연막탄 : 3x3 범위에 연막탄을 던져 '연막'을 형성한다.
			/// <para>연막탄 범위 속 카드들은 N턴 동안 이동할 수 없다.</para>
			/// </summary>
			case StageCardType.SmokeBomb:
			/// <summary> 마비 다트 : 유닛에게 직접적으로 사용 가능하며, N턴 동안 이동이 불가능한 속박 효과를 부여한다. </summary>
			case StageCardType.Paralysis:
			case StageCardType.FireBomb:
			case StageCardType.FireGun:
			case StageCardType.NapalmBomb:
			case StageCardType.Drill:
			case StageCardType.RandomAtk:
			case StageCardType.CardPull:
			case StageCardType.Explosion:
			case StageCardType.StopCard:
			case StageCardType.PlusMate:
			case StageCardType.DownLevel:
			case StageCardType.TimeBomb:
				m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
				m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse, (res, obj) => {
					LockCamScroll = false;
				}, card.m_Info.GetName(), card.m_Info.m_TData.GetInfo(), null, BaseValue.GetAreaIcon(card.m_Info.m_TData.m_Type), false, -1 ,-1 , true, false).GetComponent<Stage_CardUse>(); 
				LockCamScroll = false;
				break;
			/// <summary> 5x5 범위 내 모든 카드의 위치를 랜덤하게 다시 배치한다. </summary>
			case StageCardType.AllShuffle:
			/// <summary> 5x5 범위 내의 모든 카드를 다시 뽑는다. </summary>
			case StageCardType.AllConversion:
			/// <summary> 습득 시 아이템 획득 </summary>
			case StageCardType.SupplyBox:
			/// <summary> 샷건 카드, 전방 3x2 범위를 공격한다. </summary>
			case StageCardType.Shotgun:
			/// <summary> 5x5의 범위 내에 랜덤으로 10회 공격. [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.MachineGun:
			case StageCardType.AirStrike:
			case StageCardType.C4:
				m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
				m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse, (res, obj) => {
					LockCamScroll = false;
				}, card.m_Info.GetName(), card.m_Info.m_TData.GetInfo(), null, BaseValue.GetAreaIcon(card.m_Info.m_TData.m_Type), false, -1, -1, false, false).GetComponent<Stage_CardUse>();
				LockCamScroll = true;
				break;
		}


		yield return new WaitWhile(() => actionChar.Count > 0);
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Activepanel));
	}

	IEnumerator SelectAction_StageCardProc_End(Item_Stage card = null) {
		float movetime = 0.2f;//0.3->0.2
		GameObject Activepanel = m_Stage.ActionPanel.gameObject;

		ShowArea(false);
		// 카드 사용으로인해 죽은 카드들 연출
		yield return Check_DieCardAction();

		if (m_SkillUseInfoPopup != null) {
			m_MainUI.StartPlayAni(Main_Stage.AniName.In);
			// 팝업 닫아주기
			m_SkillUseInfoPopup.Close();
			m_SkillUseInfoPopup = null;
		}

		iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
		iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
		SetBGFXSort(true);

		List<Item_Stage_Char> actionChar = new List<Item_Stage_Char>();
		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, (obj) => {
				actionChar.Remove(obj);
			}, movetime);
			actionChar.Add(m_Chars[i]);

		}

		yield return new WaitWhile(() => actionChar.Count > 0);

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Activepanel));

		SetView_Area_Arc(card, AI_MAXLINE, false);
	}

	IEnumerator SelectAction_StageCardProc_Sniping(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_Sniping(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_Grenade(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_Grenade(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_Dynamite(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_Dynamite(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_MachineGun(Item_Stage card) {
		yield return SelectAction_StageCardProc_Start(card);

		yield return Action_MachineGun(card);

		yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_SupplyBox(Item_Stage card) {
		bool ChangeAction = true;
		// 카드 변경
		TIngameRewardTable table = TDATA.GetPickIngameReward(Mathf.RoundToInt(card.m_Info.m_TData.m_Value1), 0, CheckBattleReward);
		card.SetCardChange(table.m_Val);
		card.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => {
			ChangeAction = false;
		});

		yield return new WaitWhile(() => ChangeAction);
		yield return new WaitForSeconds(0.5f);

		m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_TData.m_Type, 1, false);
		yield return SelectAction_StageCardProc(card);
	}

	IEnumerator SelectAction_AddStat(Item_Stage card, StatType type, int value) {
		// 이펙트
		if(value > 0) StartEff(m_CenterChar.transform, "Effect/Stage/Eff_ChSkill_Heal");
		else if(value < 0) StartEff(m_CenterChar.transform, "Effect/Stage/Eff_ChSkill_DeBuff");
		yield return AddStat_Action(card.transform, type, value);
	}

	IEnumerator SelectAction_SetBuff(Item_Stage card) {
		bool isEndAction = false;
		card.Action(EItem_Stage_Card_Action.Get, 0f, (obj) => {
			if (obj.m_Info.m_TData.m_Type == StageCardType.Synergy) SetBuff(EStageBuffKind.Synergy, (int)obj.m_Info.m_Synergy);
			else SetBuff(EStageBuffKind.Stage, obj.m_Info.m_NowIdx);
			isEndAction = true;
		}, m_CenterChar.transform.position);//m_Chars[2 -> 0]
		yield return new WaitWhile(() => !isEndAction);
	}

	IEnumerator SelectAction_StageCardProc_AllShuffle(Item_Stage card) {
		yield return SelectAction_StageCardProc_Start(card);

		yield return Action_AllShuffle(card);

		yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_AllConversion(Item_Stage card) {
		yield return SelectAction_StageCardProc_Start(card);

		yield return Action_AllConversion(card);

		yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_LightStick(Item_Stage card) {
		/// <summary> 형광 스틱 : 지정한 특정 카드 위치로 형광 스틱을 던져 N턴 동안 3x3 범위 내 어둠을 밝혀 카드 모습을 확인시켜준다. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_LightStick(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_FlashLight(Item_Stage card) {
		/// <summary> 형광 스틱 : 지정한 특정 카드 위치로 형광 스틱을 던져 N턴 동안 3x3 범위 내 어둠을 밝혀 카드 모습을 확인시켜준다. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		yield return SelectAction_StageCardProc_Start(card);

		yield return Action_FlashLight(card, activecards);

		yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_Ash(Item_Stage card) {
		/// <summary> "잿더미 : '화염'효과가 옮겨 붙은 카드를 소화기 기믹으로 삭제 시 해당 카드가 '잿더미'로 변환된다.(유닛, 기믹 카드 모두)
		/// <para>해당 카드는 체력이 존재하지 않고, 카드를 삭제하는 기믹의 효과는 적용된다.</para>
		/// <para>전방 3장의 카드에 잿더미가 들어올 경우, 해당 카드 입력 시 랜덤 보상을 준다.</para>
		/// <para>(예전 캐비닛 연출처럼 카드가 타버리는 연출 후 보상 아이템 등장)"</para>
		/// </summary>
		TIngameRewardTable table = TDATA.GetPickIngameReward(Mathf.RoundToInt(card.m_Info.m_TData.m_Value1), 0, CheckBattleReward);
		card.SetCardChange(table.m_Val);
		bool Action = true;
		card.Action(EItem_Stage_Card_Action.Change, 0, (obj) => {
			Action = false;
		});
		yield return new WaitWhile(() => Action);

		m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_TData.m_Type, 1, false);
		yield return SelectAction_StageCardProc(card);
	}
	IEnumerator SelectAction_StageCardProc_SupplyBox2(Item_Stage card) {
		TIngameRewardTable table = TDATA.GetPickIngameReward(Mathf.RoundToInt(card.m_Info.m_TData.m_Value1), 0, CheckBattleReward);

		m_Check.Check(StageCheckType.CardUse, (int)card.m_Info.m_RealTData.m_Type, 1, false);
		StageCardType type = TDATA.GetStageCardTable(table.m_Val).m_Type;
		if(type == StageCardType.BigSupplyBox) m_Check.Check(StageCheckType.GetBox, 0, Mathf.Max(1, (int)card.m_Info.m_RealTData.m_Value1));
		else m_Check.Check(StageCheckType.CardUse, (int)type, 1, false);
		CheckEnd();
		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, null, 0.3f);
		}
		yield return RewardAction_Proc(table.m_Val, 1, 1);

		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, null, 0.3f);
		}
		yield return new WaitForSeconds(0.3f);
	}
	/// <summary> 정예 전투 보상이나 그냥 바닥에 깔리는 선택 상자, 기본 gid는 2, value1이 0이면 에너미의 rewardidx를 연결해준다 아니면 value1 값을 쓴다, m_IngameRewardIdx 변수에 기입 </summary>
	IEnumerator SelectAction_StageCardProc_Item_RewardBox(Item_Stage _card) {
		int[] gidxs = new int[] { (int)_card.m_Info.m_IngameRewardIdx, (int)_card.m_Info.m_DropRewardIdx, (int)_card.m_Info.m_NowTData.m_Value1 > 0 ? BaseValue.BATTLE_REWARD_COMMON_GID : BaseValue.BATTLE_REWARD_COMMON2_GID };
		int lv = (int)_card.m_Info.m_NowTData.m_Value2;

		yield return new WaitWhile(() => STAGEINFO.m_Result != StageResult.None);
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

		}, gidxs, lv, checkBattleReward, _card.m_Info.Is_RewardCancle, (int)_card.m_Info.m_NowTData.m_Value1 != 0, true, Vector3.zero, true);
		yield return new WaitWhile(() => !endreward);

		m_Check.Check(StageCheckType.CardUse, (int)_card.m_Info.m_TData.m_Type, 1, false);

		if (RewardIdx == 0) yield break;

		bool ChangeAction = true;
		_card.SetCardChange(RewardIdx);
		_card.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => {
			ChangeAction = false;
		});

		yield return new WaitWhile(() => ChangeAction);
		_card.m_Info.IsAutoGetLock = true;
		yield return new WaitForSeconds(0.5f);

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
					if (!data.IS_BuffCard())
						yield return SelectAction_StageCardProc(_card);
					break;
			}
		}
		//m_Check.Check(StageCheckType.CardUse, (int)_card.m_Info.m_TData.m_Type, 1, false);
		//yield return SelectAction_StageCardProc(_card);

		//재료 얻는 경우는 제작 액션 끝날때까지 기다림
		if (STAGE.m_MainUI != null) yield return new WaitWhile(() => STAGE.m_MainUI.GetCraft().GetState() != Item_Stage_Make.State.None);
	}

	IEnumerator SelectAction_StageCardProc_PowderExtin(Item_Stage card) {
		/// <summary> 분말 소화기 : 3x3 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_PowderExtin(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_ThrowExtin(Item_Stage card) {
		/// <summary> 투척 소화기 : 1x1 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_ThrowExtin(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_OldMine(Item_Stage _card = null, List<List<Item_Stage>> _targets = null) {
		// 범위내 지뢰들을 찾는다.
		List<Item_Stage> oldminecards = new List<Item_Stage>();
		if (_card != null) {
			oldminecards.Add(_card);
		}
		else {
			for (int j = 0, jMax = AI_MAXLINE; j <= jMax; j++) {
				for (int i = 0; i < m_ViewCard[j].Length; i++) {
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					if (item.m_Info.m_TData.m_Type != StageCardType.OldMine && item.m_Info.m_TData.m_Type != StageCardType.Allymine) continue;
					oldminecards.Add(item);
				}
			}
		}
		if (oldminecards.Count < 1) yield break; 

		List<Item_Stage> actioncards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> UserAtkCards = new List<Item_Stage>();
		Dictionary<Item_Stage, List<Item_Stage>> battledata = new Dictionary<Item_Stage, List<Item_Stage>>();
		Item_Stage temp;
		// 각 지뢰들이 상하좌우에 몬스터들이 있는지 확인함
		for (int i = oldminecards.Count - 1; i > -1; i--) {
			temp = oldminecards[i];
			List<Item_Stage> area, targets;
			//ai일때는 적있을때만, 타겟팅된거면 십자로 전부
			BoomAreaTarget(temp, temp, out area, out targets, 1, _card == null);

			if (temp.m_Line == 0 || targets.Count > 0) {
				if (temp.m_Line == 0 && m_IS_Jumping) continue;
				// 공격 대상이 하나라도 있으면 지뢰 발동
				AreaCards.Add(temp);
				AreaCards.AddRange(area);

				if (temp.m_Line != 0) {
					actioncards.Add(temp);
					temp.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
						actioncards.Remove(obj);
					});
				}
				else if( temp.m_Info.m_NowTData.m_Type == StageCardType.OldMine) UserAtkCards.Add(temp);
				if (targets.Count > 0 || temp.m_Info.m_NowTData.m_Type == StageCardType.OldMine) {
					battledata.Add(temp, new List<Item_Stage>());
					List<Item_Stage> defs = battledata[temp];
					defs.AddRange(targets);
				}
			}
		}
		if (AreaCards.Count < 1) {
			if (_card != null) {
				_card.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
					actioncards.Remove(obj);
					RemoveStage(obj);

					if (_targets != null) {
						for (int r = 0; r < _targets.Count; r++) {
							if (_targets[r] != null && _targets[r].Contains(_card))
								_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == _card)] = null;
						}
					}
				});
				m_ViewCard[_card.m_Line][_card.m_Pos] = null;
				PlayEffSound(SND_IDX.SFX_0410);
				StartEff(_card.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");
				yield return new WaitWhile(() => actioncards.Count > 0);
			}
			yield break;
		}
		
		AutoCamPosInit = false;
		// AI에의해 타겟중 이동하는 애들이 있어 여역 표시도 다시 셋팅하도록 함
		while (actioncards.Count > 0) {
			ShowArea(true, AreaCards);
			yield return null;
		}

		List<Item_Stage> atks = new List<Item_Stage>();
		List<Item_Stage> chains = new List<Item_Stage>();
		atks.AddRange(battledata.Keys);
		if (atks.Count < 1) yield break;

		for (int i = atks.Count - 1; i > -1; i--) {
			temp = atks[i];
			if (temp == null) continue;
			if (m_ViewCard[temp.m_Line][temp.m_Pos] == null) continue;

			actioncards.Add(temp);
			//스킬이나 카드의 타겟 + 지뢰의 타겟에서 해당 카드 제거
			temp.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				actioncards.Remove(obj);
				RemoveStage(obj);
				if (_targets != null) {
					for (int r = 0; r < _targets.Count; r++) {
						if (_targets[r] != null && _targets[r].Contains(temp))
							_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == temp)] = null;
					}
				}
			});
			m_ViewCard[temp.m_Line][temp.m_Pos] = null;

			PlayEffSound(SND_IDX.SFX_0410);
			StartEff(temp.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");

			List<Item_Stage> defs = battledata[temp];
			int damage = 0;
			if (UserAtkCards.Contains(temp)) {
				
				float? synergy1 = m_User.GetSynergeValue(JobType.Soldier, 0);
				if (synergy1 != null) {
					damage = Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.HP) * (float)(1f - synergy1));
					StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, -Mathf.Max(damage, 1)));
					STAGE_USERINFO.ActivateSynergy(JobType.Soldier);
				}
				else {
					damage = Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP));
					StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, -damage));
				}
			}
			//타겟중 oldmine, oil, gasstaion 아닌것만 우선 제거
			for (int j = defs.Count - 1; j > -1; j--) {
				Item_Stage target = defs[j];
				if (target == null) continue;
				if (m_ViewCard[target.m_Line][target.m_Pos] == null) continue;
				if (target.m_Info.IS_ExplosionTarget()) continue;

				if (target.m_Info.m_TData.m_Type == StageCardType.Chain) {
					target.m_ChainDieEffPosX = temp.transform.position.x;
					chains.Add(target);
				}
				else {
					//스킬이나 카드의 타겟 + 지뢰의 타겟에서 해당 카드 제거
					actioncards.Add(target);
					target.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
						actioncards.Remove(obj);
						RemoveStage(obj);
						if (_targets != null) {
							for (int r = 0; r < _targets.Count; r++) {
								if (_targets[r] != null && _targets[r].Contains(target))
									_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == target)] = null;
							}
						}
					});
					m_ViewCard[target.m_Line][target.m_Pos] = null;

					if (target.m_Info.IS_EnemyCard) {
						target.PlayHitSND();
						if(!target.m_Info.m_TEnemyData.ISRefugee()) GetKillEnemy(target, null, false, false);
					}
				}
			}
			yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.35f, Vector3.one * 0.025f);//0.5->0.35
			//yield return Check_NullCardAction();

			//타겟중 oil gasstaion 만 최후에 진행
			for (int j = defs.Count - 1; j > -1; j--) {
				Item_Stage target = defs[j];
				if (target == null) continue;
				if (m_ViewCard[target.m_Line][target.m_Pos] == null) continue;
				if (!target.m_Info.IS_OilGas()) continue;

				List<List<Item_Stage>> targetlist = new List<List<Item_Stage>>();
				if (_targets != null) targetlist.AddRange(_targets);
				targetlist.Add(defs);
				yield return SelectAction_StageCardProc_OilGasStation(target, targetlist);
			}
			//타겟중 oldmine 만 최후에 진행
			for (int j = defs.Count - 1; j > -1; j--) {
				Item_Stage target = defs[j];
				if (target == null) continue;
				if (m_ViewCard[target.m_Line][target.m_Pos] == null) continue;
				if (target.m_Info.m_NowTData.m_Type != StageCardType.OldMine && target.m_Info.m_NowTData.m_Type != StageCardType.Allymine) continue;

				List<List<Item_Stage>> targetlist = new List<List<Item_Stage>>();
				if (_targets != null) targetlist.AddRange(_targets);
				targetlist.Add(defs);
				yield return SelectAction_StageCardProc_OldMine(target, targetlist);
			}
		}

		yield return new WaitWhile(() => actioncards.FindAll(o => o != null).Count > 0);
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;
		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		ShowArea(false);
		// 카드 사용으로인해 죽은 카드들 연출
		if(_targets == null) yield return Check_DieCardAction();
		AutoCamPosInit = true;
	}

	IEnumerator SelectAction_StageCardProc_SmokeBomb(Item_Stage card) {
		/// <summary> 형광 스틱 : 지정한 특정 카드 위치로 형광 스틱을 던져 N턴 동안 3x3 범위 내 어둠을 밝혀 카드 모습을 확인시켜준다. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_SmokeBomb(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_Paralysis(Item_Stage card) {
		/// <summary> 마비 다트 : 유닛에게 직접적으로 사용 가능하며, N턴 동안 이동이 불가능한 속박 효과를 부여한다. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		yield return SelectAction_StageCardProc_Start(card);

		yield return Action_Paralysis(card, activecards);

		yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_Chain(Item_Stage card, Action<Item_Stage> EndCB) {
		Vector3 pos = card.transform.localPosition;
		iTween.ScaleTo(card.gameObject, iTween.Hash("scale", Vector3.one * 1.4f, "isLocal", true, "time", 0.3f, "easetype", "easeOutQuad"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(card.gameObject));
		EndCB?.Invoke(card);
	}

	void ChainLineChange(int line, List<Item_Stage> chains, float EffX, Action<int> EndCB) {
		List<Item_Stage> actioncards = new List<Item_Stage>();
		for (int i = 0; i < m_ViewCard[line].Length; i++) {
			Item_Stage card = m_ViewCard[line][i];
			if (card == null) continue;
			if (chains.Contains(card)) continue;
			if (card.IS_Die()) continue;
			StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
			actioncards.Add(card);
			card.SetCardChange(info.m_Idx);
			card.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
				actioncards.Remove(obj);
				RemoveStage(obj);
				if (actioncards.Count < 1) EndCB?.Invoke(obj.m_Line);
			});
			m_ViewCard[card.m_Line][card.m_Pos] = null;
		}

		Vector3 pos = actioncards[0].transform.position;
		pos.x = EffX;
		StartEff(pos, "Effect/Stage/Eff_StagCard_Dynamite");

		if (actioncards.Count < 1) EndCB?.Invoke(line);
	}

	IEnumerator SelectAction_ChainUI() {
		m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
		bool EffAction = true;
		UIEF_StageCard_Chain eff = UTILE.LoadPrefab("Effect/Stage/UIEF_StageCard_Chain", true, POPUP.GetWorldUIPanel()).GetComponent<UIEF_StageCard_Chain>();
		eff.SetData(m_User.m_Turn, m_User.m_Time, (obj) => {
			EffAction = false;
		});
		yield return new WaitWhile(() => EffAction);
	}
	IEnumerator SelectAction_ChainDie(List<Item_Stage> chains) {
		if (chains.Count < 0) yield break;
		List<Item_Stage> actioncards = new List<Item_Stage>();
		List<int> lines = new List<int>();
		List<float> effx = new List<float>();
		for (int i = chains.Count - 1; i > -1; i--) {
			Item_Stage temp = chains[i];
			if (!lines.Contains(temp.m_Line)) {
				lines.Add(temp.m_Line);
				effx.Add(temp.m_ChainDieEffPosX.Value);
				temp.m_ChainDieEffPosX = null;
			}
			actioncards.Add(temp);
			temp.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				actioncards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[temp.m_Line][temp.m_Pos] = null;
		}

		List<int> actionlines = new List<int>();
		for (int i = lines.Count - 1; i > -1; i--) {
			int line = lines[i];
			ChainLineChange(line, chains, effx[i], (re) => {
				actionlines.Remove(re);
			});
			actionlines.Add(line);
		}
		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.7f, Vector3.one * 0.025f);//1->0.7
		yield return new WaitWhile(() => actioncards.Count > 0);
		yield return new WaitWhile(() => actionlines.Count > 0);
	}

	IEnumerator SelectAction_StageCardProc_Hive(Item_Stage card) {
		StageCardInfo info = card.m_Info;
		bool Action = true;
		card.SetCardChange(Mathf.RoundToInt(info.m_TData.m_Value2));
		card.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => {
			Action = false;
		});
		yield return new WaitWhile(() => Action);

		if (info.m_TEnemyData.ISRefugee()) {
			// 피난민, 보급상자등 문제가 되는 카드들을 위해 다시 확인
			yield return Check_NullCardAction();
		}
		else {
			yield return StartBattle(info.IS_Boss ? EBattleMode.EnemyAtk : EBattleMode.Normal, card);
#if STAGE_NO_BATTLE
			BATTLEINFO.m_Result = EBattleResult.WIN;
#endif
			if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;
			if (BATTLEINFO.m_Result == EBattleResult.WIN) {
				card.Action(EItem_Stage_Card_Action.Die);
				int lv = info.m_TEnemyData.m_RewardLV;
				int rewardgid = info.m_TEnemyData.m_RewardGID;
				bool allgroup = info.m_TEnemyData.m_AllGroup;
				bool cancanble = info.m_TEnemyData.m_RewardCancle;
				yield return new WaitUntil(() => card.IS_NoAction);
				yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup, true, -1, card.transform.position);
			}

			yield return new WaitWhile(() => IS_SelectAction_Pause());
		}
	}

	IEnumerator SelectAction_Fire(Item_Stage card, Action<Item_Stage> EndCB) {
		PlayEffSound(SND_IDX.SFX_0606);
		Vector3 pos = card.transform.localPosition;
		iTween.ScaleTo(card.gameObject, iTween.Hash("scale", Vector3.one * 1.4f, "isLocal", true, "time", 0.3f, "easetype", "easeOutQuad"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(card.gameObject));

		Item_Stage_Char stagechar = m_Chars[UTILE.Get_Random(0, USERINFO.m_PlayDeck.GetDeckCharCnt())];
		if (UTILE.Get_Random(0, 100) < 30) {
			SND_IDX hitvocidx = stagechar.m_Info.m_TData.GetHitVoice(stagechar.m_TransverseAtkMode);
			PlayEffSound(hitvocidx);
		}
		StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, -(int)Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * card.m_Info.m_TData.m_Value2)));
		STAGE.CheckEnd();

		bool Action = true;
		//card.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
		//	Action = false;
		//});
		card.Action(EItem_Stage_Card_Action.DissolveGet, 0f, (obj) => {
			Action = false;
		}, new Vector3(card.transform.position.x, m_CenterChar.transform.position.y, m_CenterChar.transform.position.z));

		yield return new WaitWhile(() => Action);
		EndCB?.Invoke(card);
	}

	IEnumerator SelectAction_StageCardProc_ShockBomb(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_ShockBomb(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_C4(Item_Stage card) {
		yield return SelectAction_StageCardProc_Start(card);

		yield return Action_C4(card);

		yield return SelectAction_StageCardProc_End(card);
	}
	IEnumerator SelectAction_Material(Item_Stage card) {
		bool isEndAction = false;
		card.Action(EItem_Stage_Card_Action.FadeOut, 0f, (obj) => {
			int matcountdown = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MaterialCountDown));
			int matcnt = Mathf.Max(Mathf.RoundToInt(obj.m_Info.m_TData.m_Value2) - matcountdown, 1);
			int randmatdebuff = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.RandomMaterial));
			AddMaterial(randmatdebuff > 0 ? (StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat) : (StageMaterialType)Mathf.RoundToInt(obj.m_Info.m_TData.m_Value1),
				randmatdebuff > 0 ? UTILE.Get_Random(1, randmatdebuff + 1) : Mathf.RoundToInt(matcnt + obj.m_Info.m_PlusCnt), card.transform.position);
			isEndAction = true;
		});
		yield return new WaitWhile(() => !isEndAction);
	}
	IEnumerator SelectAction_Shotgun(Item_Stage card) {
		/// <summary> 샷건 카드, 전방 3x2 범위를 공격한다. </summary>
		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_Shotgun(card, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}
	IEnumerator SelectAction_AirStrike(Item_Stage card) {
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}
		/// <summary> 샷건 카드, 전방 3x2 범위를 공격한다. </summary>
		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_AirStrike(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}
	IEnumerator SelectAction_StageCardProc_StarShell(Item_Stage card) {
		/// <summary> 조명탄 : 5x5 범위 내 어둠카드 N턴동안 제거. </summary>
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_StarShell(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_PowderBomb(Item_Stage card) {
		/// <summary> 분말 폭탄 : 5x5 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
		/// startline 1인건 첫줄 카드를 사용했기 때문
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_PowderBomb(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}
	IEnumerator SelectAction_StageCardProc_FireBomb(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_FireBomb(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}
	IEnumerator SelectAction_StageCardProc_FireGun(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}
		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_FireGun(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	IEnumerator SelectAction_StageCardProc_NapalmBomb(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_NapalmBomb(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}

	//해당 방향 이동후 한줄씩 넘어가야하는데 그냥 가운데 기준으로 넘기는듯
	IEnumerator SelectAction_StageCardProc_Jump(Item_Stage _card, int _line = 0) {
		m_IS_Jumping = true;
		// 카드 라인 변경
		int linecnt = _line == 0 ? Mathf.RoundToInt(_card.m_Info.m_TData.m_Value1) : _line;
		List<Item_Stage> removecards = new List<Item_Stage>();
		List<Item_Stage> firstlinecards = new List<Item_Stage>();

		for (int k = 0; k < linecnt; k++) {
			if (m_CardLastLine == 0) break;

			for (int i = 0; i < m_ViewCard[0].Length; i++) {
				Item_Stage TempCard = m_ViewCard[0][i];
				removecards.Add(TempCard);
				TempCard.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
					// 카드 pool 이동
					RemoveStage(obj);
					removecards.Remove(obj);
				});
				m_ViewCard[0][i] = null;
			}

			for (int j = 1, Start = m_SelectPos, End = Start + 3; j < m_ViewCard.Length; j++, End += 2) {
				if (j == 2) {
					Start = 1;
					End -= m_SelectPos - 1;
				}
				for (int i = 0, Offset = 0; i < m_ViewCard[j].Length; i++) {
					Item_Stage TempCard = m_ViewCard[j][i];
					if (TempCard == null) continue;
					if (i < Start || i >= End ) {
						removecards.Add(TempCard);
						TempCard.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
							// 카드 pool 이동
							RemoveStage(obj);
							removecards.Remove(obj);
						});
					}
					else {
						int line = j - 1;
						TempCard.SetPos(line, Offset);
						m_ViewCard[line][Offset] = TempCard;
						Offset++;
						if (line == 0) {
							firstlinecards.Add(TempCard);
							TempCard.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
								firstlinecards.Remove(obj);
							});
						}
					}
					m_ViewCard[j][i] = null;
				}
				yield return new WaitWhile(() => firstlinecards.Count > 0);
			}

			yield return new WaitWhile(() => removecards.Count > 0);

			List<Item_Stage> actioncards = new List<Item_Stage>();
			// 새로운 라인 추가
			for (int j = 0, jmax = m_ViewCard.Length - 1; j < jmax; j++) {
				for (int i = 0; i < m_ViewCard[j].Length; i++) {
					Item_Stage TempCard = m_ViewCard[j][i];
					if (TempCard == null) continue;
					actioncards.Add(TempCard);
					TempCard.Action(EItem_Stage_Card_Action.Move, 0f, (obj) => {
						actioncards.Remove(obj);
					});
				}
			}

			CreateStageLine(0, m_ViewCard.Length - 1);
			yield return new WaitWhile(() => actioncards.Count > 0);
		}
		m_Check.Check(StageCheckType.Survival, 0, linecnt);
		//m_IS_Jumping = false;
	}
	IEnumerator SelectAction_StageCardProc_DownLevel(Item_Stage _card) {
		List<Item_Stage> activecards = SetView_Area_Target(_card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(_card);
		else
			yield return SelectAction_StageCardProc_Start(_card);

		yield return Action_DownLevel(_card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(_card);
		else
			yield return SelectAction_StageCardProc_End(_card);
	}
	IEnumerator SelectAction_StageCardProc_CoolReset(Item_Stage _card) {
		if (STAGEINFO.m_TStage.m_PlayType.Find(o => o.m_Type == PlayType.NoCool) != null) yield break;

		float movetime = 0.3f;
		bool alllock = true;
		bool[] frameactive = new bool[USERINFO.m_PlayDeck.GetDeckCharCnt()];
		bool overlay = false;
		Item_Stage_Char target = null;
		List<Item_Stage_Char> activechars = new List<Item_Stage_Char>();
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			if (!m_Chars[i].m_SkillLock && m_Chars[i].m_SkillCoolTime > 0) {
				activechars.Add(m_Chars[i]);
				m_Chars[i].SetSkillFrameActive(true);
				m_Chars[i].SetSelectFX(true);
				alllock = false;
			}
		}
		//for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
		//	if (!m_Chars[i].m_SkillLock && m_Chars[i].m_SkillCoolTime > 0) {
		//		frameactive[i] = m_Chars[i].GetSkillFrameActive();
		//		m_Chars[i].SetSkillFrameActive(true);
		//		activechars.Add(m_Chars[i]);
		//		m_Chars[i].SetSelectFX(true);
		//		alllock = false;
		//	}
		//}
		if (alllock) yield break;

		//Utile_Class.GetCanvasPosition
		m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
		m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse, null, _card.m_Info.GetName(), _card.m_Info.m_TData.GetInfo(),
			 null, BaseValue.GetAreaIcon(_card.m_Info.m_TData.m_Type), false, -1, -1, true, false).GetComponent<Stage_CardUse>();

		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			if (!activechars.Contains(m_Chars[i]))
				m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, null, movetime);
		}

		//센터 캐릭터 레이랑 카피된 카드 겹치는지 체크
		if (m_SelectVirture != null) {
			Vector3 pos = Vector3.zero;
			//pos = Utile_Class.GetCanvasPosition(m_CenterChar.transform.position + m_CenterChar.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			Vector3 centerpos = Utile_Class.GetCanvasPosition(m_CenterChar.transform.position + m_CenterChar.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			//pos = Utile_Class.GetCanvasPosition(m_SelectVirture.transform.position - m_SelectVirture.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			Vector3 copypos = Utile_Class.GetCanvasPosition(m_SelectVirture.transform.position - m_SelectVirture.ImgSize * 0.5f) / Canvas_Controller.SCALE;
			if (copypos.y < centerpos.y) overlay = true;
		}
		if (overlay) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 1, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[4], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[4], "time", movetime, "easetype", "easeOutQuad"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}

		yield return SelectAction_SkillCard_CheckSelectChar((obj) => target = obj);

		///특정 캐릭터 선택 하게(캐릭터는 락걸려있으면 안됨)
		PlayEffSound(SND_IDX.SFX_0460);
		// 이펙트
		StartEff(target.transform, "Effect/Stage/Eff_ChSkill_Heal");
		for (int i = 0; i < activechars.Count; i++) {
			activechars[i].SetSkillFrameActive(activechars[i].IS_UseActiveSkill());
			activechars[i].SetSelectFX(false);
		}
		//for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
		//	if (!m_Chars[i].m_SkillLock && m_Chars[i].m_SkillCoolTime > 0) {
		//		m_Chars[i].SetSkillFrameActive(frameactive[i]);
		//	}
		//	m_Chars[i].SetSelectFX(false);
		//}
		//쿨타임 리셋
		target.SkillColoTimeInit();

		yield return new WaitForSeconds(0.5f);

		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, null, movetime);
		}

		if (overlay) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 0, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}
		m_MainUI.StartPlayAni(Main_Stage.AniName.In);
		m_SkillUseInfoPopup.Close();
		m_SkillUseInfoPopup = null;

		yield return new WaitForSeconds(movetime);
	}
	IEnumerator SelectAction_StageCardProc_AllCoolReset(Item_Stage _card) {
		PlayEffSound(SND_IDX.SFX_0460);
		// 이펙트
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			StartEff(m_Chars[i].transform, "Effect/Stage/Eff_ChSkill_Heal");
			m_Chars[i].SkillColoTimeInit();
		}
		yield break;
	}

	IEnumerator SelectAction_StageCardProc_AllUpAdr(Item_Stage _card) {
		float val = _card.m_Info.m_TData.m_Value1;
		PlayEffSound(SND_IDX.SFX_0460);
		// 이펙트
		for (int i = 0; i < m_Chars.Length; i++) {
			StartEff(m_Chars[i].transform, "Effect/Stage/Eff_ChSkill_Heal");
		}
		//안좋은 효과 받은 경우
		if (val < 0) StartEff(m_CenterChar.transform, "Effect/Stage/Eff_Debuff_Ch");

		StageCardType[] types = new StageCardType[3] { StageCardType.AtkUp, StageCardType.DefUp, StageCardType.HealUp };
		for (int i = 0; i < types.Length; i++) {
			float preval = STAGE_USERINFO.GetBuffValue(types[i]);
			if (!STAGE_USERINFO.m_BuffValues.ContainsKey(types[i])) {
				STAGE_USERINFO.m_BuffValues.Add(types[i], val);
			}
			else {
				STAGE_USERINFO.m_BuffValues[types[i]] += val;
			}

			//알람
			if (m_AddStatAlarm.ContainsKey(m_CenterChar.transform) && m_AddStatAlarm[m_CenterChar.transform] == null) m_AddStatAlarm.Remove(m_CenterChar.transform);
			if (!m_AddStatAlarm.ContainsKey(m_CenterChar.transform)) {
				m_AddStatAlarm.Add(m_CenterChar.transform, UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>());
			}
			m_AddStatAlarm[m_CenterChar.transform].SetData(m_CenterChar.transform, types[i], m_User.GetBuffValue(types[i]) - preval);
		}

		yield break;
	}

	IEnumerator SelectAction_StageCardProc_CardPull(Item_Stage _card) {
		List<Item_Stage> activecards = SetView_Area_Target(_card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(_card);
		else
			yield return SelectAction_StageCardProc_Start(_card);

		yield return Action_CardPull(_card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(_card);
		else
			yield return SelectAction_StageCardProc_End(_card);
	}

	IEnumerator SelectAction_StageCardProc_Explosion(Item_Stage _card) {
		List<Item_Stage> activecards = SetView_Area_Target(_card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(_card);
		else
			yield return SelectAction_StageCardProc_Start(_card);

		yield return Action_Explosion(_card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(_card);
		else
			yield return SelectAction_StageCardProc_End(_card);
	}

	IEnumerator SelectAction_StageCardProc_BanAirStrike(Item_Stage card, int _cnt = 0) {
		int cnt = _cnt != 0 ? _cnt : Mathf.RoundToInt(card.m_Info.m_TData.m_Value1);
		if (cnt > 0) {
			StartEff(Vector3.zero, "Effect/Stage/Eff_StageCard_WaveBreak");
			PlayEffSound(SND_IDX.SFX_0404);
		}
		else {
			StartEff(m_CenterChar.transform, "Effect/Stage/Eff_Debuff_Ch");
			PlayEffSound(SND_IDX.SFX_0471);
		}

		if (m_ModeCnt.ContainsKey(PlayType.FieldAirstrike)) {
			//m_ModeCnt[PlayType.FieldAirstrike] += Mathf.RoundToInt(card.m_Info.m_TData.m_Value1);
			STAGE.m_MainUI.RefreshModeAlarm(PlayType.FieldAirstrike, cnt);
			if(STAGE.m_MainUI.GetModeAlarm(PlayType.FieldAirstrike).m_Val[0] == 0)
				yield return Mode_FieldAirstrike(STAGEINFO.m_TStage.GetMode(PlayType.FieldAirstrike));
		}

		yield break;
	}

	IEnumerator SelectAction_StageCardProc_AllRecoverySrv(Item_Stage card) {
		PlayEffSound(SND_IDX.SFX_0460);

		for (StatType i = StatType.Men; i < StatType.SurvEnd; i++) {
			if (!STAGE_USERINFO.Is_UseStat(i)) continue;
			switch (i) {
				case StatType.Men: StartEff(card.transform, "Effect/Stage/Eff_ChSkill_Mental"); break;
				case StatType.Hyg: StartEff(card.transform, "Effect/Stage/Eff_ChSkill_Hygiene"); break;
				case StatType.Sat: StartEff(card.transform, "Effect/Stage/Eff_ChSkill_Fullness"); break;
			}

			yield return AddStat_Action(card.transform, i, Mathf.RoundToInt(m_User.GetMaxStat(i) * card.m_Info.m_TData.m_Value1));
		}
		yield break;
	}

	IEnumerator SelectAction_StageCardProc_Drill(Item_Stage card) {
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_Area_Target(card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(card);
		else
			yield return SelectAction_StageCardProc_Start(card);

		yield return Action_Drill(card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(card);
		else
			yield return SelectAction_StageCardProc_End(card);
	}
	IEnumerator SelectAction_StageCardProc_RandomAtk(Item_Stage _card) {
		List<Item_Stage> activecards = SetView_Area_Target(_card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(_card);
		else
			yield return SelectAction_StageCardProc_Start(_card);

		yield return Action_RandomAtk(_card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(_card);
		else
			yield return SelectAction_StageCardProc_End(_card);
	}
	IEnumerator SelectAction_StageCardProc_StopCard(Item_Stage _card) {
		List<Item_Stage> activecards = SetView_Area_Target(_card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(_card);
		else
			yield return SelectAction_StageCardProc_Start(_card);

		yield return Action_StopCard(_card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(_card);
		else
			yield return SelectAction_StageCardProc_End(_card);
	}
	IEnumerator SelectAction_StageCardProc_PlusMate(Item_Stage _card) {
		List<Item_Stage> activecards = SetView_Area_Target(_card, AI_MAXLINE, m_IS_KillFirstLine ? 0 : 1);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield break;
		}

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_Start(_card);
		else
			yield return SelectAction_StageCardProc_Start(_card);

		yield return Action_PlusMate(_card, activecards, m_IS_KillFirstLine ? 0 : 1);

		if (m_IS_KillFirstLine)
			yield return MakingAction_Proc_End(_card);
		else
			yield return SelectAction_StageCardProc_End(_card);
	}

	public IEnumerator SelectAction_RecoveryAP(float _value) {
		//멘탈 부족에 따른 행동력 회복 감소
		float val = _value;
		int preval = m_User.m_AP[0];
		m_User.m_AP[0] = Mathf.Clamp(m_User.m_AP[0] + Mathf.RoundToInt(val), 0, m_User.m_AP[1]);

		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
		}
		//행동력 회복 스킬이나 시너지도 추가 될거임
		DLGTINFO?.f_RfAPUI?.Invoke(m_User.m_AP[0], preval, m_User.m_AP[1]);
		yield break;
	}
	public IEnumerator SelectAction_LimitTurnUp(float _value) {
		if (STAGE_USERINFO.IS_TurnUse()) {
			STAGE_USERINFO.m_Turn += Mathf.RoundToInt(_value);
			if (STAGE_USERINFO.m_Turn < 0) STAGE_USERINFO.m_Turn = 0;
			DLGTINFO?.f_RFClockChangeUI?.Invoke(STAGE_USERINFO.m_Turn, STAGE_USERINFO.m_Time);

			//알람
			if (m_AddStatAlarm.ContainsKey(m_CenterChar.transform) && m_AddStatAlarm[m_CenterChar.transform] == null) m_AddStatAlarm.Remove(m_CenterChar.transform);
			if (!m_AddStatAlarm.ContainsKey(m_CenterChar.transform)) {
				m_AddStatAlarm.Add(m_CenterChar.transform, UTILE.LoadPrefab("Effect/EF_BuffCenterAlarm", true, POPUP.GetWorldUIPanel()).GetComponent<EF_BuffCenterAlarm>());
			}
			m_AddStatAlarm[m_CenterChar.transform].SetData(m_CenterChar.transform,  StageCardType.LimitTurnUp, _value);
			if(_value > 0) PlayEffSound(SND_IDX.SFX_0462);
		}
		yield break;
	}
	public IEnumerator SelectAction_AddGuard(float _value) {
		m_User.m_Stat[(int)StatType.Guard, 0] += (float)Mathf.RoundToInt(_value);
		if (_value > 0) PlayEffSound(SND_IDX.SFX_0461);
		else if (_value < 0) PlayEffSound(SND_IDX.SFX_0472);
		yield break;
	}
	public IEnumerator SelectAction_AddRerollCount(float _value) {
		m_User.m_leftReRollCnt += Mathf.RoundToInt(_value);
		if(_value > 0) PlayEffSound(SND_IDX.SFX_0462);
		else if (_value < 0) PlayEffSound(SND_IDX.SFX_0472);
		yield break;
	}
	public IEnumerator SelectAction_StageCardProc_Gamble(TGambleCardTable _table, float _prop) {
		//확률대로 뽑고
		TGambleCardTable table = _table;
		float randprop = _prop;
		//확률로 성공실패 뽑고
		TStageCardTable resultcardtable = TDATA.GetStageCardTable(table.m_ResultIdx[1 - table.m_SuccProp > randprop ? 0 : 1]);

		//스탯으로 버프 디버프 체크
		//팝업으로 주사위 굴림 및 결과 보여주고
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
			case StageCardType.ConMergeSlotDown:
			case StageCardType.MergeSlotCount:
				SetBuff(EStageBuffKind.Stage, resultcardtable.m_Idx);
				if (resultcardtable.m_Type == StageCardType.LevelUp) STAGE_USERINFO.StatReset();
				else if(resultcardtable.m_Type == StageCardType.APRecoveryUp || resultcardtable.m_Type == StageCardType.APConsumDown) {
					for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
						m_Chars[i].RefreshAPText();
						m_Chars[i].SetAPUI(m_User.m_AP[0]);
					}
				}
				break;
			case StageCardType.Synergy:
				SetBuff(EStageBuffKind.Synergy, (int)STAGE_USERINFO.CreateSynergy());
				break;
			case StageCardType.RecoveryHp:
				yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryHpPer:
				yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetStat(StatType.Heal) * resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryMen:
				yield return AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryHyg:
				yield return AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.RecoverySat:
				yield return AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.PerRecoveryMen:
				yield return AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Men) * resultcardtable.m_Value1));
				break;
			case StageCardType.PerRecoveryHyg:
				yield return AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Hyg) * resultcardtable.m_Value1));
				break;
			case StageCardType.PerRecoverySat:
				yield return AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.Sat) * resultcardtable.m_Value1));
				break;
			case StageCardType.RecoveryAP:
				yield return SelectAction_RecoveryAP(resultcardtable.m_Value1);
				break;
			case StageCardType.AddGuard:
				yield return SelectAction_AddGuard(resultcardtable.m_Value1);
				break;
			case StageCardType.LimitTurnUp:
				yield return SelectAction_LimitTurnUp(resultcardtable.m_Value1);
				break;
			case StageCardType.AddRerollCount:
				yield return SelectAction_AddRerollCount(resultcardtable.m_Value1);
				break;
			case StageCardType.Jump:
				yield return SelectAction_StageCardProc_Jump(null, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.BanAirStrike:
				yield return SelectAction_StageCardProc_BanAirStrike(null, Mathf.RoundToInt(resultcardtable.m_Value1));
				break;
			case StageCardType.Material:
				AddMaterial((StageMaterialType)Mathf.RoundToInt(resultcardtable.m_Value1), Mathf.RoundToInt(resultcardtable.m_Value2));
				break;
		}
	}
	IEnumerator SelectAction_SaveCard(Item_Stage _card) {
		bool isEndAction = false;
		_card.Action(EItem_Stage_Card_Action.FadeOut, 0f, (obj) => {
			m_MainUI?.GetMakeUtileCard((StageMaterialType)Mathf.RoundToInt(_card.m_Info.m_TData.m_Value1), 1, _card.transform.position);
			isEndAction = true;
		});
		yield return new WaitWhile(() => !isEndAction);
	}
	IEnumerator SelectAction_StageCardProc_GetOilGasStation(Item_Stage _card) {
		bool isEndAction = false;
		_card.Action(EItem_Stage_Card_Action.FadeOut, 0f, (obj) => {
			int matcountdown = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MaterialCountDown)); 
			int matcnt = 0;
			if (_card.m_Info.m_TData.m_Type == StageCardType.Oil) matcnt = UTILE.Get_Random(1, 4);
			else if (_card.m_Info.m_TData.m_Type == StageCardType.GasStation) matcnt = UTILE.Get_Random(3, 6);

			matcnt =  Mathf.Max(matcnt - matcountdown, 1);

			int randmatdebuff = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.RandomMaterial));
			AddMaterial(randmatdebuff > 0 ? (StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat) : StageMaterialType.Gasoline,
				randmatdebuff > 0 ? UTILE.Get_Random(1, randmatdebuff + 1) : Mathf.RoundToInt(matcnt + obj.m_Info.m_PlusCnt));
			isEndAction = true;
		});
		yield return new WaitWhile(() => !isEndAction);
	}
	IEnumerator SelectAction_StageCardProc_OilGasStation(Item_Stage _card, List<List<Item_Stage>> _targets = null, bool _burncheck = true) {
		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> oncards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		Item_Stage item = null;
		BoomAreaTarget(_card, _card, out AreaCards, out TargetCards, 0);
		if (TargetCards.Count < 1) yield break;
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			item = TargetCards[i];
			if (item.IS_Die()) continue;
			if (!item.IS_NoAction) continue;
			oncards.Add(item);
			item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
				oncards.Remove(obj);
			});
		}
		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => oncards.Count > 0);

		//타겟에서 자기자신 제거
		TargetCards.Remove(_card);

		// 범위내 지뢰들을 찾는다.
		List<Item_Stage> actioncards = new List<Item_Stage>();

		actioncards.Add(_card);
		_card.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
			actioncards.Remove(obj);
			RemoveStage(obj);
			if (_targets != null) {
				for (int r = 0; r < _targets.Count; r++) {
					if (_targets[r] != null && _targets[r].Contains(_card))
						_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == _card)] = null;
				}
			}
		});
		m_ViewCard[_card.m_Line][_card.m_Pos] = null;
		//yield return new WaitWhile(() => actioncards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0410);
		StartEff(_card.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.35f, Vector3.one * 0.025f);//0.5->0.35

		for (int i = TargetCards.Count - 1; i > -1; i--) {
			item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
			if (item.m_Info.IS_ExplosionTarget()) continue;
			//스킬이나 카드의 타겟 + 지뢰의 타겟에서 해당 카드 제거
			if (_targets != null) {
				for (int r = 0; r < _targets.Count; r++) {
					if (_targets[r] != null && _targets[r].Contains(item))
						_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == item)] = null;
				}
			}

			if (item.m_Info.m_TData.m_Type == StageCardType.Chain) {
				yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.3f, Vector3.one * 0.01f);//0.4->0.3
				item.m_ChainDieEffPosX = item.transform.position.x;
				yield return SelectAction_ChainDie(TargetCards);
			}
			else {
				int fireidx = Mathf.RoundToInt(_card.m_Info.m_TData.m_Value2);
				if (fireidx == 0) {
					List<TStageCardTable> fires = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Fire);
					if (fires.Count > 0) fireidx = fires[UTILE.Get_Random(0, fires.Count)].m_Idx;
				}
				if (fireidx == 0) fireidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Fire);

				// 이펙트 등록
				if (item.m_Info.IS_EnemyCard) {
					int dmg = GetAtkDmg(item, null, false, false, true, _card);

					item.SetDamage(false, dmg);

					if (item.IS_Die()) {
						if (_burncheck) {
							m_Check.Check(StageCheckType.Fire_Enemy, (int)item.m_Info.m_TEnemyData.m_Type, 1);
							m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);
						}

						GetKillEnemy(item, null, false, false);

						actioncards.Add(item);
						item.SetCardChange(fireidx);//잿더미 인덱스
						item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { 
							actioncards.Remove(obj);
							if (_targets != null) {
								for (int r = 0; r < _targets.Count; r++) {
									if (_targets[r] != null && _targets[r].Contains(item))
										_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == item)] = null;
								}
							}
						});
					}
					else {
						if (IS_BurnInfoCard(item)) PlusBurnInfo(item, 3);
						else {
							GameObject Eff = StartEff(item.transform, "Effect/Stage/Eff_StageCard_Fire_Loop", true);
							Eff.transform.localScale = Vector3.one;
							BurnInfo burn = new BurnInfo(BurnMode.AREA1);
							burn.SetTarget(item);
							burn.SetTurn(3);
							burn.SetEff(Eff);
							AddBurnInfo(burn);

							// 라이트 설정 생성
							LightInfo lightinfo = new LightInfo(LightMode.LightStick, 1);
							lightinfo.SetTarget(item.m_Line, item.m_Pos);
							lightinfo.SetTurn(3);
							AddLight(lightinfo);
							//원형가로등추가
							StreetLightInfo streetlightinfo = new StreetLightInfo();
							streetlightinfo.SetTarget(item.m_Line, item.m_Pos, true);
							streetlightinfo.SetTurn(3);
							//이펙트 등록
							Eff = UTILE.LoadPrefab("Effect/Stage/Eff_StageCard_StreetLight", true, STAGE.m_DarkMaskPanel);
							Eff.transform.position = m_ViewCard[item.m_Line][item.m_Pos].transform.position;
							Eff.transform.localScale = Vector3.one;
							streetlightinfo.SetEff(Eff);
							//라이트 추가
							AddStreetLightInfo(streetlightinfo);
						}
					}
				}
				else if (item.m_Info.IS_BurnTarget()) {
					if (_burncheck) m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);

					actioncards.Add(item);
					item.SetCardChange(fireidx);//잿더미 인덱스
					item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { 
						actioncards.Remove(obj);
						if (_targets != null) {
							for (int r = 0; r < _targets.Count; r++) {
								if (_targets[r] != null && _targets[r].Contains(item))
									_targets[r][_targets[r].FindIndex(0, _targets[r].Count, o => o == item)] = null;
							}
						}
					});
				}
			}
		}
		yield return new WaitWhile(() => actioncards.FindAll(o => o != null).Count > 0);

		//타겟중 oil gasstation 만 최후에 진행
		for (int j = TargetCards.Count - 1; j > -1; j--) {
			item = TargetCards[j];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
			if (!item.m_Info.IS_OilGas()) continue;

			List<List<Item_Stage>> targetlist = new List<List<Item_Stage>>();
			if (_targets != null) targetlist.AddRange(_targets);
			targetlist.Add(TargetCards);
			yield return SelectAction_StageCardProc_OilGasStation(item, targetlist, _burncheck);
		}
		//타겟중 oldmine 만 최후에 진행
		for (int j = TargetCards.Count - 1; j > -1; j--) {
			item = TargetCards[j];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
			if (item.m_Info.m_NowTData.m_Type != StageCardType.OldMine && item.m_Info.m_NowTData.m_Type != StageCardType.Allymine) continue;

			List<List<Item_Stage>> targetlist = new List<List<Item_Stage>>();
			if (_targets != null) targetlist.AddRange(_targets);
			targetlist.Add(TargetCards);
			yield return SelectAction_StageCardProc_OldMine(item, targetlist);
		}

		yield return new WaitWhile(() => actioncards.Count > 0);
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		ShowArea(false);
		yield return Action_TargetOff(TargetCards);
		// 카드 사용으로인해 죽은 카드들 연출
		if(_targets == null) yield return Check_DieCardAction();
	}
	IEnumerator SelectAction_StageCardProc_TimeBomb(Item_Stage _card) {
		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AfterTargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> chains = new List<Item_Stage>();

		BoomAreaTarget(_card, _card, out AreaCards, out TargetCards, 0);

		ShowArea(true, AreaCards);
		PlayEffSound(SND_IDX.SFX_0410);
		// 이펙트
		StartEff(_card.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.35f, Vector3.one * 0.025f);//0.5->0.35

		int firecnt = 0;
		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine) {
				AfterTargetCards.Add(item);
				TargetCards.Remove(item);
				continue;
			}
			else {
				if (item.m_Info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = _card.transform.position.x;
					chains.Add(item);
					TargetCards.Remove(item);
				}
				else {
					if (item.m_Info.IS_Boss && item.m_Info.m_TData.m_IsEndType) {
						item.SetDamage(false, Mathf.RoundToInt(item.m_Info.GetStat(EEnemyStat.HP) * _card.m_Info.m_TData.m_Value1));
						TargetCards.Remove(item);
						if (item.IS_Die()) {
							GetKillEnemy(item, null, false, false);
						}
					}
					else {
						if (item.m_Info.m_NowTData.m_Type == StageCardType.Fire) firecnt++;
						item.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
							// 카드 pool 이동
							TargetCards.Remove(obj);
							RemoveStage(obj);
						});
						m_ViewCard[item.m_Line][item.m_Pos] = null;
						if (item.m_Info.IS_EnemyCard) {
							GetKillEnemy(item, null, false, false);
						}
					}
				}
			}
		}// 타겟 데미지 주기
		if (AfterTargetCards.Count > 0) {
			for (int i = AfterTargetCards.Count - 1; i > -1; i--) {
				Item_Stage item = AfterTargetCards[i];
				if (item == null) continue;
				if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

				if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine) {
					yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { AfterTargetCards });
					AfterTargetCards.Remove(item);
				}
			}
		}
		if (firecnt > 0) m_Check.Check(StageCheckType.SuppressionF, 0, firecnt);

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		yield return new WaitWhile(() => TargetCards.FindAll(o => o != null).Count > 0);
		yield return new WaitWhile(() => AfterTargetCards.FindAll(o => o != null).Count > 0);

		ShowArea(false);

		yield return Check_NullCardAction();
	}
	IEnumerator SkillCard_Start(bool _zoom) {
		float movetime = 0.3f;

		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, null, movetime);
		}

		m_MainUI.StartPlayAni(Main_Stage.AniName.Out);

		if (_zoom) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 1, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[2], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[2], "time", movetime, "easetype", "easeOutQuad"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}
	}

	IEnumerator SkillCard_End(bool _zoom) {
		ShowArea(false);

		// 카드 사용으로인해 죽은 카드들 연출
		yield return Check_DieCardAction();

		//20210602_ASBY:Check_DieCardAction() 위에있다 아래로 옮김, Jump스킬 쓰고 난 뒤 첫줄이 콜라이더가 안켜져서
		// 타겟 설정에서 꺼져있으므로 켜준다.
		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			if (item == null) continue;
			StageCardInfo info = item.m_Info;
			// 피난민은 저격 불가
			item.Action(EItem_Stage_Card_Action.Scale, 0f, (obj)=> { obj.TW_ScaleBumping(true); });
		}

		float movetime = 0.3f;
		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, null, movetime);
		}

		m_MainUI.StartPlayAni(Main_Stage.AniName.In);
		if (_zoom) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 0, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}

		// 여기까지오면 팝업이 없어야되는데 SkillUsePopup이 연출때문에 나중에 닫히는 현상이 있어 여기서 대기해줌
		// 제거시 튜토리얼이 진행 안되는 현상 발생
		yield return new WaitWhile(() => POPUP.IS_PopupUI());
	}

	IEnumerator SelectAction_SkillCard_CheckSelectChar(Action<Item_Stage_Char> endcb) {
		Item_Stage_Char selectcard = null;
		while (selectcard == null) {
			if (TouchCheck()) {
				// 선택 카드 알아내기
				Ray ray = m_MyCam.ScreenPointToRay(Input.mousePosition);
				RaycastHit[] hit = Physics.RaycastAll(ray, m_MyCam.farClipPlane);
				for (int i = 0; i < hit.Length; i++) {
					GameObject hitobj = hit[i].transform.gameObject;
					if (!hitobj.activeSelf) continue;
					Item_Stage_Char hitcard = hitobj.GetComponent<Item_Stage_Char>();
					if (hitcard == null) continue;
					if (hitcard.m_SkillLock) continue;
					if (hitcard.m_SkillCoolTime < 1) continue;

					selectcard = hitcard;
					break;
				}
			}
			yield return null;
		}

		endcb?.Invoke(selectcard);
	}
}
