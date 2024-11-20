using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Action
	IEnumerator m_ActionCor = null;
	IEnumerator Action_TargetOff(List<Item_Stage> ActiveCards) {
		if (ActiveCards.Count < 1) yield break;
		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (m_ViewCard[oncard.m_Line][oncard.m_Pos] == null) continue;

			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		yield return new WaitWhile(() => offcards.FindAll(o=>o != null).Count > 0);
	}
	IEnumerator Action_Sniping(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		if (m_AutoPlay) {
			targetcard = ActiveCards[UTILE.Get_Random(0, ActiveCards.Count)];
		}
		else {
			AutoCamPosInit = false;
			yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
			if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.SelectSnipingTarget)) TUTO.Next();
			//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.SelectSnipingTarget)) TUTO.Next();
			else if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.SelectSnipingTarget)) TUTO.Next();
		}
		ShowArea(false);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncards = ActiveCards[i];
			if (oncards == targetcard) continue;
			if (oncards.IS_Die()) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		List<Item_Stage> targets = new List<Item_Stage>() { targetcard };
		for(int i = 1; i < 2; i++) {
			if (targetcard.m_Line + i > AI_MAXLINE) break;
			Item_Stage linetarget = m_ViewCard[targetcard.m_Line + i][targetcard.m_Pos + i];
			if (linetarget != null) targets.Add(linetarget);
		}
		ShowArea(true, targets);

		yield return new WaitWhile(() => offcards.Count > 0);

		for (int r = 0; r < targets.Count; r++) {
			Item_Stage target = targets[r];
			if (target == null) continue;
			if (m_ViewCard[target.m_Line][target.m_Pos] == null) continue;

			PlayEffSound(SND_IDX.SFX_0400);
			// 이펙트
			GameObject eff = StartEff(target.transform, "Effect/Stage/Eff_StageCard_Sniping");
			eff.transform.localScale *= 0.6f;

			if (target.m_Info.m_NowTData.m_Type == StageCardType.OldMine || target.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(target, new List<List<Item_Stage>>() { targets });
			else if (target.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(target, new List<List<Item_Stage>>() { targets });
			}
			else {
				if (target.m_Info.IS_EnemyCard) {
					float ratio = 1f;
					if (r == 1) ratio = 0.5f;
					else if (r == 2) ratio = 0.3f;
					int dmg = GetAtkDmg(target, null, false, false, true, card, ratio);

					target.SetDamage(false, dmg);

					if (target.IS_Die()) {
						GetKillEnemy(target, null, false, true);
					}

					offcards.Add(target);
					target.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
						offcards.Remove(obj);
					});
					if (r < targets.Count) yield return new WaitForSeconds(0.2f);
				}
				else {
					// 이펙트
					eff = StartEff(target.transform, "Effect/Stage/Eff_StageCard_Sniping");
					eff.transform.localScale *= 0.6f;
				}
			}
		}
		yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.25f, Vector3.one * 0.025f);//0.4->0.25
		yield return new WaitWhile(() => offcards.Count > 0);
		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_Grenade(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {

		Item_Stage targetcard = null;

		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		if (m_AutoPlay) {
			targetcard = ActiveCards[UTILE.Get_Random(0, ActiveCards.Count)];
		}
		else {
			AutoCamPosInit = false;
			yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		}
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AfterTargetCards = new List<Item_Stage>(); 
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> actioncards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		BoomAreaTarget(card, targetcard, out AreaCards, out TargetCards, startline);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();

		PlayEffSound(SND_IDX.SFX_0410);
		// 이펙트
		StartEff(targetcard.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.3f, Vector3.one * 0.025f);//0.5->0.3

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
					item.m_ChainDieEffPosX = targetcard.transform.position.x;
					chains.Add(item);
					TargetCards.Remove(item);
				}
				else {
					if (item.m_Info.IS_Boss && item.m_Info.m_TData.m_IsEndType) {
						item.SetDamage(false, Mathf.RoundToInt(item.m_Info.GetStat(EEnemyStat.HP) * card.m_Info.m_TData.m_Value2));
						TargetCards.Remove(item);
						if (item.IS_Die()) {
							GetKillEnemy(item, null, false, true);
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
							item.PlayHitSND();
							GetKillEnemy(item, null, false, true);
						}
					}
				}
			}
		}// 타겟 데미지 주기
		if(AfterTargetCards.Count > 0) { 
			for (int i = AfterTargetCards.Count - 1; i > -1; i--) {
				Item_Stage item = AfterTargetCards[i];
				if (item == null) continue;
				if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

				if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine) {
					yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { AfterTargetCards });
					if(m_ViewCard[item.m_Line][item.m_Pos] != null) {
						actioncards.Add(item);
						item.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
							actioncards.Remove(obj);
							RemoveStage(obj);
						});
						m_ViewCard[item.m_Line][item.m_Pos] = null;
					}
					AfterTargetCards.Remove(item);
				}
			}
		}
		if (firecnt > 0) m_Check.Check(StageCheckType.SuppressionF, 0, firecnt);

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		yield return new WaitWhile(() => TargetCards.FindAll(o => o != null).Count > 0);
		yield return new WaitWhile(() => AfterTargetCards.FindAll(o => o != null).Count > 0);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	IEnumerator Action_Dynamite(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {

		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AfterTargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> actioncards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점과 같은 라인
		BoomAreaTarget(card, targetcard, out AreaCards, out TargetCards, startline);

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();
		PlayEffSound(SND_IDX.SFX_0410);
		// 이펙트
		StartEff(targetcard.transform, "Effect/Stage/Eff_StagCard_Dynamite");
		IEnumerator camshake = IE_CamAction(CamActionType.Shake_0, 0f, 0.7f, Vector3.one * 0.025f);//1->0.7
		StartCoroutine(camshake);

		yield return new WaitForSeconds(0.5f);

		int firecnt = 0;
		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			if (item.m_Info.m_TData.m_Type == StageCardType.OldMine || item.m_Info.m_TData.m_Type == StageCardType.Allymine) {
				TargetCards.Remove(item);
				AfterTargetCards.Add(item);
				continue;
			}
			else {
				if (item.m_Info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = targetcard.transform.position.x;
					chains.Add(item);
					TargetCards.Remove(item);
				}
				else {
					if (item.m_Info.IS_Boss && item.m_Info.m_TData.m_IsEndType) {
						item.SetDamage(false, Mathf.RoundToInt(item.m_Info.GetStat(EEnemyStat.HP) * card.m_Info.m_TData.m_Value2));
						TargetCards.Remove(item);
						if (item.IS_Die()) {
							GetKillEnemy(item, null, false, true);
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
							item.PlayHitSND();
							GetKillEnemy(item, null, false, true);
						}
					}
				}
			}
		}
		if (AfterTargetCards.Count > 0) {
			for (int i = AfterTargetCards.Count - 1; i > -1; i--) {
				Item_Stage item = AfterTargetCards[i];
				if (item == null) continue;
				if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

				if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine) {
					yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { AfterTargetCards });
					if (m_ViewCard[item.m_Line][item.m_Pos] != null) {
						actioncards.Add(item);
						item.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
							actioncards.Remove(obj);
							RemoveStage(obj);
						});
						m_ViewCard[item.m_Line][item.m_Pos] = null;
					}
					AfterTargetCards.Remove(item);
				}
			}
		}

		if (firecnt > 0) m_Check.Check(StageCheckType.SuppressionF, 0, firecnt);

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		yield return new WaitWhile(() => TargetCards.FindAll(o=>o != null).Count > 0);
		yield return new WaitWhile(() => AfterTargetCards.FindAll(o => o != null).Count > 0);
		yield return new WaitWhile(() => !IS_EndCamAction);
		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_MachineGun(Item_Stage card, int startline = 1) {
		LockCamScroll = true;
		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> oncards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 앞에서부터 5 * 5 위치
		//	5,4		5,5		5,6		5,7		5,8
		//	4,3		4,4		4,5		4,6		4,7
		//	3,2		3,3		3,4		3,5		3,6
		//	2,1		2,2		2,3		2,4		2,5
		//	1,0		1,1		1,2		1,3		1,4
		//					0,1		<-- 시작 위치
		for (int j = startline, jMax = j + 5, Start = card.m_Pos - 1; j < jMax; j++, Start++) {
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 5, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				StageCardInfo info = item.m_Info;
				AreaCards.Add(item);
				if (!info.IS_DmgTarget() && !info.IS_ExplosionTarget()) continue;
				TargetCards.Add(item);
				oncards.Add(item);
				item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
					oncards.Remove(obj);
				});
			}
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => oncards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();
		PlayEffSound(SND_IDX.SFX_0600);
		// 이펙트
		GameObject eff = StartEff(card.transform, "Effect/Stage/Eff_StageCard_MachineGun");
		yield return IE_CamAction(CamActionType.Shake_0, 0f, 1.2f, Vector3.one * 0.01f);//1.83->1.2

		// 타겟 데미지 주기
		int atk = Mathf.RoundToInt(m_User.GetStat(StatType.Atk));
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
			StageCardInfo info = item.m_Info;

			if (info.m_NowTData.m_Type == StageCardType.OldMine || info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { TargetCards });
			else if (info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { TargetCards });
			}
			else {
				if (info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = 0;
					chains.Add(item);
				}
				else if (item.m_Info.IS_DmgTarget()) {
					int dmg = GetAtkDmg(item, null, false, false, true, card);

					item.SetDamage(false, dmg);

					if (item.IS_Die()) {
						GetKillEnemy(item, null, false, true);
					}
				}
			}
		}

		yield return Action_TargetOff(TargetCards);

		double time = UTILE.Get_Time();
		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);
		time = 1f - (UTILE.Get_Time() - time);
		if (time > 0) yield return new WaitForSeconds((float)time);
		m_ActionCor = null;
		LockCamScroll = false;
	}
	IEnumerator Action_AllShuffle(Item_Stage card, int startline = 1) {
		LockCamScroll = true;
		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> Check = new List<Item_Stage>();
		List<Item_Stage> actioncard = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 5*5 위치
		//	5,4		5,5		5,6		5,7		5,8
		//	4,3		4,4		4,5		4,6		4,7
		//	3,2		3,3		3,4		3,5		3,6
		//	2,1		2,2		2,3		2,4		2,5
		//	1,0		1,1		1,2		1,3		1,4
		//					0,1
		for (int j = startline, jMax = j + 5, Start = card.m_Pos - 1; j <= jMax; j++, Start++) {
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 5, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				if (item.m_Info.IS_RoadBlock) continue;
				if (item.m_Info.m_TData.IS_LineCard()) continue;
				AreaCards.Add(item);
				TargetCards.Add(item);
				Check.Add(item);
				actioncard.Add(item);
				// 타겟을 초기화해주어 다시 검색하도록 한다.
				item.m_Target = null;
				item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
					actioncard.Remove(obj);
				});
			}
		}
		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => actioncard.Count > 0);

		List<Item_Stage> moveCards = new List<Item_Stage>();
		// 1:1 대응으로 움직임
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage moveCard = TargetCards[i];
			if (moveCards.Contains(moveCard)) continue;

			// 자기자신은 체크에서 제거
			Check.Remove(moveCard);
			if (Check.Count < 1) {
				moveCard.Action(EItem_Stage_Card_Action.TargetOff);
				continue;
			}
			// 대상 정하기
			int Rand = UTILE.Get_Random(0, Check.Count);
			Item_Stage target = Check[Rand];

			//CardLock 모드로 잠겨있는거는 서로 바꿔줌
			if (moveCard.IS_Lock || target.IS_Lock) {
				bool[] MTLock = new bool[2] { moveCard.IS_Lock, target.IS_Lock };
				bool[] check = new bool[2] { false, false };
				moveCard.Action(MTLock[1] ? EItem_Stage_Card_Action.Lock : EItem_Stage_Card_Action.UnLock, 0f, (obj) => {
					check[0] = true;
				});
				target.Action(MTLock[0] ? EItem_Stage_Card_Action.Lock : EItem_Stage_Card_Action.UnLock, 0f, (obj) => {
					check[1] = true;
				});
				yield return new WaitWhile(() => check[0] == true && check[1] == true);
			}

			m_ViewCard[target.m_Line][target.m_Pos] = moveCard;
			m_ViewCard[moveCard.m_Line][moveCard.m_Pos] = target;
			target.ISMove = true;
			moveCard.ISMove = true;
			// 액션 카드 등록
			actioncard.Add(moveCard);

			// 이동하고있는 카드들 셋팅
			moveCards.Add(target);
			moveCards.Add(moveCard);

			// 대상 탐색에서 제거
			Check.Remove(target);

			moveCard.Action(EItem_Stage_Card_Action.MoveTarget, 0, (obj) => {
				actioncard.Remove(obj);
			}, target);
		}

		yield return new WaitWhile(() => actioncard.Count > 0);

		// 딜레이 주기
		yield return new WaitForSeconds(0.5f);
		m_ActionCor = null;
		LockCamScroll = false;
	}
	IEnumerator Action_AllConversion(Item_Stage card, int startline = 1) {
		LockCamScroll = true;
		List<Item_Stage> actioncard = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 5*5 위치
		//	5,4		5,5		5,6		5,7		5,8
		//	4,3		4,4		4,5		4,6		4,7
		//	3,2		3,3		3,4		3,5		3,6
		//	2,1		2,2		2,3		2,4		2,5
		//	1,0		1,1		1,2		1,3		1,4
		//					0,1
		for (int j = startline, jMax = j + 5, Start = card.m_Pos - 1; j <= jMax; j++, Start++) {
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 5, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				if (item.m_Info.IS_RoadBlock) continue;
				if (item.m_Info.m_TData.IS_LineCard()) continue;
				StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
				if (info.IS_Boss) continue;
				if (info.m_TData.IS_LineCard()) continue;
				AreaCards.Add(item);
				//변환할 카드가 어둠카드면 리얼인덱스 넣어줘야함
				if (info.IsDarkCard) {
					item.SetCardChange(info.m_RealIdx);
				}
				else item.SetCardChange(info.m_Idx);
				actioncard.Add(item);
				item.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => {
					actioncard.Remove(obj);
				});
			}
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => actioncard.Count > 0);

		// 딜레이 주기
		yield return new WaitForSeconds(0.5f);
		m_ActionCor = null;
		LockCamScroll = false;
	}
	IEnumerator Action_LightStick(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		for (int j = targetcard.m_Line - 1, jMax = j + 3, Start = targetcard.m_Pos - 2; j < jMax; j++, Start++) {
			if (j < startline) continue;
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 3, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				AreaCards.Add(item);
				StageCardInfo info = item.m_Info;
				if (!info.IS_DmgTarget()) continue;
				TargetCards.Add(item);
			}
		}

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => offcards.Count > 0);

		TStageCardTable tdata = card.m_Info.m_TData;
		// 라이트 설정 생성
		LightInfo lightinfo = new LightInfo(LightMode.LightStick, 1);
		lightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos);
		lightinfo.SetTurn(Mathf.RoundToInt(tdata.m_Value1));

		PlayEffSound(SND_IDX.SFX_0603);
		// 이펙트 등록
		GameObject Eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_LightStick", true);
		Eff.transform.localScale = Vector3.one;
		lightinfo.SetEff(Eff);

		AddLight(lightinfo);

		//원형가로등추가
		StreetLightInfo streetlightinfo = new StreetLightInfo();
		streetlightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos, true);
		streetlightinfo.SetTurn(3);
		//이펙트 등록
		Eff = UTILE.LoadPrefab("Effect/Stage/Eff_StageCard_StreetLight", true, STAGE.m_DarkMaskPanel);
		Eff.transform.position = m_ViewCard[targetcard.m_Line][targetcard.m_Pos].transform.position;
		Eff.transform.localScale = Vector3.one;
		streetlightinfo.SetEff(Eff);
		//라이트 추가
		AddStreetLightInfo(streetlightinfo);

		yield return Action_TargetOff(TargetCards);

		yield return Check_LightOnOff();

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_FlashLight(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {

		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);

		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.SelectLightTarget)) TUTO.Next();
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 타겟 지점에서부터 위로 5라인까지 한줄
		//  5,6
		//	4,5
		//	3,4
		//	2,3
		//  1.2
		for (int j = 1; j <= AI_MAXLINE; j++) {
			int i = targetcard.m_Pos + j - 1;
			Item_Stage item = m_ViewCard[j][i];
			if (item == null) continue;
			if (item.IS_Die()) continue;
			if (item.IS_Lock) continue;
			AreaCards.Add(item);
			TargetCards.Add(item);
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (oncard.IS_Lock) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => offcards.Count > 0);

		TStageCardTable tdata = card.m_Info.m_TData;
		// 라이트 설정 생성
		LightInfo lightinfo = new LightInfo(LightMode.FlashLight);
		lightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos);
		int turn = Mathf.RoundToInt(tdata.m_Value1);
		lightinfo.SetTurn(turn);

		PlayEffSound(SND_IDX.SFX_0603);

		// 이펙트 등록
		GameObject Eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_FlashLight", true);
		Eff.transform.localScale = Vector3.one;
		lightinfo.SetEff(Eff);

		AddLight(lightinfo);

		yield return Check_LightOnOff();
		yield return Action_TargetOff(TargetCards);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_PowderExtin(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {

		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();

		int Cnt = 3;
		int j = targetcard.m_Line - (Cnt >> 1);
		int jMax = j + Cnt;
		int Start = targetcard.m_Pos - (Cnt - 1);
		int End;

		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		for (; j < jMax; j++, Start++) {
			if (j < startline) continue;
			if (j > AI_MAXLINE) break;
			End = Math.Min(Start + Cnt, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				AreaCards.Add(item);
				StageCardInfo info = item.m_Info;
				if (info.m_NowTData.m_Type != StageCardType.Fire) continue;
				TargetCards.Add(item);
			}
		}

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}
		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0608);
		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Fireext");
		yield return new WaitForSeconds(0.5f);

		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			int ashidx = Mathf.RoundToInt(item.m_Info.m_TData.m_Value1);
			if (ashidx == 0) {
				List<TStageCardTable> ashs = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Ash);
				if (ashs.Count > 0) ashidx = ashs[UTILE.Get_Random(0, ashs.Count)].m_Idx;
			}
			if (ashidx == 0) ashidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Ash);
			StageCardInfo info = item.m_Info;
			item.SetCardChange(ashidx);
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		m_Check.Check(StageCheckType.SuppressionF, 0, TargetCards.Count);
		// 이펙트 시간만큼 대기
		yield return new WaitForSeconds(1f);
		//yield return new WaitWhile(() => eff != null);

		yield return Action_TargetOff(TargetCards);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_ThrowExtin(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();

		int Cnt = 1;
		int j = targetcard.m_Line - (Cnt >> 1);
		int jMax = j + Cnt;
		int Start = targetcard.m_Pos - (Cnt - 1);
		int End;

		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		for (; j < jMax; j++, Start++) {
			if (j < startline) continue;
			if (j > AI_MAXLINE) break;
			End = Math.Min(Start + Cnt, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				AreaCards.Add(item);
				StageCardInfo info = item.m_Info;
				if (info.m_NowTData.m_Type != StageCardType.Fire) continue;
				TargetCards.Add(item);
			}
		}

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0607);
		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Fireext_Throw_S");
		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			int ashidx = Mathf.RoundToInt(item.m_Info.m_TData.m_Value1);
			if (ashidx == 0) {
				List<TStageCardTable> ashs = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Ash);
				if (ashs.Count > 0) ashidx = ashs[UTILE.Get_Random(0, ashs.Count)].m_Idx;
			}
			if (ashidx == 0) ashidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Ash);
			StageCardInfo info = item.m_Info;
			item.SetCardChange(ashidx);
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		m_Check.Check(StageCheckType.SuppressionF, 0, TargetCards.Count);
		// 이펙트 시간만큼 대기
		yield return new WaitForSeconds(1f);
		//yield return new WaitWhile(() => eff != null);

		yield return Action_TargetOff(TargetCards);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_SmokeBomb(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {

		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		for (int j = targetcard.m_Line - 1, jMax = j + 3, Start = targetcard.m_Pos - 2; j < jMax; j++, Start++) {
			if (j < startline) continue;
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 3, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				AreaCards.Add(item);
				StageCardInfo info = item.m_Info;
				if (!info.IS_DmgTarget()) continue;
				TargetCards.Add(item);
			}
		}

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0610);
		// 이펙트 등록
		GameObject Eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_SmokeBomb", true);
		Eff.transform.localScale = Vector3.one;

		TStageCardTable tdata = card.m_Info.m_TData;
		// 이동불가 설정 생성
		AIStopInfo aistop = new AIStopInfo(AiStopMode.AREA3);
		aistop.SetTarget(targetcard.m_Line, targetcard.m_Pos);
		aistop.SetTurn(Mathf.RoundToInt(tdata.m_Value1));
		aistop.SetEff(Eff);
		AddAIStopInfo(aistop);

		//원거리 공격 차단 생성
		AiBlockRangeAtkInfo aiblockrangeastk = new AiBlockRangeAtkInfo(AiBlockRangeAtkMode.AREA1);
		aiblockrangeastk.SetTarget(targetcard);
		aiblockrangeastk.SetTurn(Mathf.RoundToInt(tdata.m_Value1));
		AddAiBlockRangeAtkInfo(aiblockrangeastk);

		yield return Check_LightOnOff();

		yield return Action_TargetOff(new List<Item_Stage>() { targetcard });

		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	IEnumerator Action_Paralysis(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> AreaCards = new List<Item_Stage>();
		AreaCards.Add(targetcard);

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (oncard.IS_Lock) continue;
			if (targetcard == oncard) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0613);
		StartEff(targetcard.transform, "Effect/Stage/Eff_Debuff_Card");
		// 이펙트 등록
		GameObject Eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Paralysis", true);
		Eff.transform.localScale = Vector3.one;

		TStageCardTable tdata = card.m_Info.m_TData;
		// 이동불가 설정 생성
		AIStopInfo aistop = new AIStopInfo(AiStopMode.AREA1);
		aistop.SetTarget(targetcard);
		aistop.SetTurn(Mathf.RoundToInt(tdata.m_Value1));
		aistop.SetEff(Eff);
		AddAIStopInfo(aistop);

		//원거리 공격 차단 생성
		AiBlockRangeAtkInfo aiblockrangeastk = new AiBlockRangeAtkInfo(AiBlockRangeAtkMode.AREA1);
		aiblockrangeastk.SetTarget(targetcard);
		aiblockrangeastk.SetTurn(Mathf.RoundToInt(tdata.m_Value1));
		AddAiBlockRangeAtkInfo(aiblockrangeastk);

		yield return new WaitForSeconds(1f);

		yield return Check_LightOnOff();

		yield return Action_TargetOff(new List<Item_Stage>() { targetcard });

		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	IEnumerator Action_ShockBomb(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.SelectShockBombTarget)) TUTO.Next();
		ShowArea(false);



		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncards = ActiveCards[i];
			if (oncards == targetcard) continue;
			if (oncards.IS_Die()) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		List<Item_Stage> targets = new List<Item_Stage>() { targetcard };
		ShowArea(true, targets);

		yield return new WaitWhile(() => {
			return offcards.Count > 0;
			});

		PlayEffSound(SND_IDX.SFX_0410);
		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Sniping");
		eff.transform.localScale *= 0.6f;

		yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.3f, Vector3.one * 0.01f);//0.4->0.3

		if (targetcard.m_Info.m_NowTData.m_Type == StageCardType.OldMine || targetcard.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
			yield return SelectAction_StageCardProc_OldMine(targetcard);
		else {
			if (targetcard.m_Info.m_TData.m_Type == StageCardType.Chain) {
				targetcard.m_ChainDieEffPosX = targetcard.transform.position.x;
				yield return SelectAction_ChainDie(targets);
			}
			else {
				if (targetcard.m_Info.IS_Boss && targetcard.m_Info.m_TData.m_IsEndType) {
					targetcard.SetDamage(false, Mathf.RoundToInt(targetcard.m_Info.GetStat(EEnemyStat.HP) * card.m_Info.m_TData.m_Value2));
					if (targetcard.IS_Die()) {
						GetKillEnemy(targetcard, null, false, true);
					}
				}
				else {
					offcards.Add(targetcard);
					if (targetcard.m_Info.m_NowTData.m_Type == StageCardType.Fire) m_Check.Check(StageCheckType.SuppressionF, 0, 1);
					targetcard.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
						// 카드 pool 이동
						offcards.Remove(obj);
						RemoveStage(obj);
					});
					m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = null;
					if (targetcard.m_Info.IS_EnemyCard) {
						targetcard.PlayHitSND();
						GetKillEnemy(targetcard, null, false, true);
					}
				}
				yield return new WaitWhile(() => offcards.Count > 0);
			}
		}
		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	IEnumerator Action_C4(Item_Stage card, int startline = 1) {
		LockCamScroll = true;
		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AfterTargetCards = new List<Item_Stage>(); 
		List<Item_Stage> oncards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> actioncards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 앞에서부터 5 * 5 위치
		//	5,4		5,5		5,6		5,7		5,8
		//	4,3		4,4		4,5		4,6		4,7
		//	3,2		3,3		3,4		3,5		3,6
		//	2,1		2,2		2,3		2,4		2,5
		//	1,0		1,1		1,2		1,3		1,4
		//					0,1		<-- 시작 위치
		BoomAreaTarget(card, card, out AreaCards, out TargetCards, startline);
		if (TargetCards.Count < 1) yield break;
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			if (item.IS_Die()) continue;
			oncards.Add(item);
			item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
				oncards.Remove(obj);
			});
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => oncards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();

		// 이펙트
		Vector3 effpos;// = new Vector3(0, BaseValue.STAGE_INTERVER.y * 2 * m_Stage.Panel[1].lossyScale.y, 0f) + m_Stage.Panel[1].position;
		if (startline == 1) effpos = new Vector3(card.transform.localPosition.x * m_Stage.Panel[1].lossyScale.x, BaseValue.STAGE_INTERVER.y * 2 * m_Stage.Panel[1].lossyScale.y, 0f) + m_Stage.Panel[1].position;
		else effpos = new Vector3(0, BaseValue.STAGE_INTERVER.y * 2 * m_Stage.Panel[1].lossyScale.y, 0f) + m_Stage.Panel[1].position;

		PlayEffSound(SND_IDX.SFX_0410);
		GameObject eff = StartEff(effpos, "Effect/Stage/Eff_StageCard_C4");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.5f, Vector3.one * 0.02f);//0.75->0.5

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
					item.m_ChainDieEffPosX = 0;
					chains.Add(item);
					TargetCards.Remove(item);
				}
				else {
					if (item.m_Info.IS_Boss && item.m_Info.m_TData.m_IsEndType) {
						item.SetDamage(false, Mathf.RoundToInt(item.m_Info.GetStat(EEnemyStat.HP) * card.m_Info.m_TData.m_Value2));
						TargetCards.Remove(item);
						if (item.IS_Die()) {
							GetKillEnemy(item, null, false, true);
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
							item.PlayHitSND();
							GetKillEnemy(item, null, false, true);
						}
					}
				}
			}
		}
		if (AfterTargetCards.Count > 0) {
			for (int i = AfterTargetCards.Count - 1; i > -1; i--) {
				Item_Stage item = AfterTargetCards[i];
				if (item == null) continue;
				if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

				if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine) {
					yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { AfterTargetCards });
					if (m_ViewCard[item.m_Line][item.m_Pos] != null) {
						actioncards.Add(item);
						item.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
							actioncards.Remove(obj);
							RemoveStage(obj);
						});
						m_ViewCard[item.m_Line][item.m_Pos] = null;
					}
					AfterTargetCards.Remove(item);
				}
			}
		}
		if (firecnt > 0) m_Check.Check(StageCheckType.SuppressionF, 0, firecnt);

		double time = UTILE.Get_Time();
		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		yield return new WaitWhile(() => TargetCards.FindAll(o => o != null).Count > 0);
		yield return new WaitWhile(() => AfterTargetCards.FindAll(o => o != null).Count > 0);

		time = 1f - (UTILE.Get_Time() - time);
		if (time > 0) yield return new WaitForSeconds((float)time);
		m_ActionCor = null;
		LockCamScroll = false;
	}

	IEnumerator Action_Shotgun(Item_Stage card, int startline = 1) {
		LockCamScroll = true;
		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> oncards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 앞에서부터 5 * 5 위치
		//	2,2		2,3		2,4
		//	1,1		1,2		1,3
		//			0,1		<-- 시작 위치

		//Start = card.m_Pos > 0 && !m_IS_KillFirstLine ? card.m_Pos : 0;
		//첫줄에서 적을 죽이면 보상카드가 나와서 pos 0이고 supplybox, Ash는 선택한 카드의 pos로 옮겨가기 떄문에 
		//제작으로 생성된 카드는  첫줄 사살 아니고 pos 0이고 line -1이라 0line0pos여야함
		//첫줄 사살해서 보상으로 생성된 카드는 pos1인데 startline0 이어서 카드 pos -1이어야함
		int Start = card.m_Pos > 0 && !m_IS_KillFirstLine ? card.m_Pos : (card.m_Line > -1 ? card.m_Pos - 1 : 0) + startline;
		for (int j = startline, jMax = j + 2; j < jMax; j++, Start++)//샷건은 첫라인 맨 왼쪽부터
		{
			if (j > AI_MAXLINE) break;
			int End = Math.Min(Start + 3, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				StageCardInfo info = item.m_Info;
				AreaCards.Add(item);
				if (!info.IS_DmgTarget() && !info.IS_ExplosionTarget()) continue;
				TargetCards.Add(item);
				oncards.Add(item);
				item.Action(EItem_Stage_Card_Action.TargetOn, 0f, (obj) => {
					oncards.Remove(obj);
				});
			}
		}

		ShowArea(true, AreaCards);
		yield return new WaitWhile(() => oncards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();

		PlayEffSound(SND_IDX.SFX_0500);
		// 이펙트
		GameObject eff = StartEff(card.transform, "Effect/Stage/Eff_ChSkill_ShotGun");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.25f, Vector3.one * 0.025f);

		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
			StageCardInfo info = item.m_Info;
			if (info.m_NowTData.m_Type == StageCardType.OldMine || info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { TargetCards });
			else if (info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { TargetCards });
			}
			else {
				if (info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = 0;
					chains.Add(item);
				}
				else if (item.m_Info.IS_DmgTarget()) {
					int dmg = GetAtkDmg(item, null, false, false, true, card);

					item.SetDamage(false, dmg);

					if (item.IS_Die()) {
						GetKillEnemy(item, null, false, true);
					}
				}
			}
		}

		yield return Action_TargetOff(TargetCards);

		double time = UTILE.Get_Time();
		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);
		time = 1f - (UTILE.Get_Time() - time);
		if (time > 0) yield return new WaitForSeconds((float)time);
		m_ActionCor = null;
		LockCamScroll = false;
	}

	IEnumerator Action_AirStrike(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		LockCamScroll = true;
		AutoCamPosInit = false;
		yield return IE_CamAction(CamActionType.Zoom_Out);

		PlayEffSound(SND_IDX.SFX_0507);
		// 이펙트
		Vector3 pos = new Vector3(0, BaseValue.STAGE_INTERVER.y * 2 * m_Stage.Panel[1].lossyScale.y, 0f) + m_Stage.Panel[1].position;
		StartEff(pos, "Effect/Stage/Eff_ChSkill_AirStrike");
		yield return new WaitForSeconds(0.5f);
		yield return IE_CamAction(CamActionType.Shake_2, 0.6f, 0.8f);//0.85->0.6
		CamAction(CamActionType.Shake_1);

		List<Item_Stage> chains = new List<Item_Stage>();

		// 타겟 데미지 주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage item = ActiveCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			StageCardInfo info = item.m_Info;
			if (item.IS_Die()) continue;

			if (info.m_NowTData.m_Type == StageCardType.OldMine || info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { ActiveCards });
			else if (info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { ActiveCards });
			}
			else {
				if (info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = 0;
					chains.Add(item);
				}
				else if (info.IS_DmgTarget()) {
					int dmg = GetAtkDmg(item, null, false, false, true, card);

					item.SetDamage(false, dmg);

					if (item.IS_Die()) {
						GetKillEnemy(item, null, false, true);
					}
				}
			}
		}

		yield return Action_TargetOff(ActiveCards);

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);
		yield return IE_CamAction(CamActionType.Zoom_OutToIdle);

		AutoCamPosInit = true;
		m_ActionCor = null;
		LockCamScroll = false;
	}

	/// <summary> 조명탄, 5x5 범위 내 모든 어둠을 N턴 동안 삭제한다.  </summary>
	IEnumerator Action_StarShell(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();

		int Cnt = 5;
		int j = targetcard.m_Line - (Cnt >> 1);
		int jMax = j + Cnt;
		int Start = targetcard.m_Pos - (Cnt - 1);
		int End;

		// 타겟 데미지 주기
		// 타겟 지점에서부터 5*5 위치
		for (; j < jMax; j++, Start++) {
			if (j < startline) continue;
			if (j > AI_MAXLINE) break;
			End = Math.Min(Start + Cnt, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				AreaCards.Add(item);
				TargetCards.Add(item);
			}
		}

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}
		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		TStageCardTable tdata = card.m_Info.m_TData;
		// 라이트 설정 생성
		LightInfo lightinfo = new LightInfo(LightMode.StarShell, 2);
		lightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos);
		lightinfo.SetTurn(Mathf.RoundToInt(tdata.m_Value1));

		PlayEffSound(SND_IDX.SFX_0413);
		// 이펙트 등록
		GameObject Eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StarShell", true);
		Eff.transform.localScale = Vector3.one;
		lightinfo.SetEff(Eff);

		AddLight(lightinfo);

		yield return Action_TargetOff(TargetCards);

		yield return Check_LightOnOff();

		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	/// <summary> 분말 폭탄, 5X5 범위 내 모든 화염을 삭제한다. </summary>
	IEnumerator Action_PowderBomb(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();

		int Cnt = 5;
		int j = targetcard.m_Line - (Cnt >> 1);
		int jMax = j + Cnt;
		int Start = targetcard.m_Pos - (Cnt - 1);
		int End;

		// 타겟 데미지 주기
		// 타겟 지점에서부터 5*5 위치
		for (; j < jMax; j++, Start++) {
			if (j < startline) continue;
			if (j > AI_MAXLINE) break;
			End = Math.Min(Start + Cnt, m_ViewCard[j].Length);
			for (int i = Start; i < End; i++) {
				if (i < 0) continue;
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				AreaCards.Add(item);
				StageCardInfo info = item.m_Info;
				if (info.m_NowTData.m_Type != StageCardType.Fire) continue;
				TargetCards.Add(item);
			}
		}

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}
		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0609);
		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Fireext_Throw");
		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			int ashidx = Mathf.RoundToInt(item.m_Info.m_TData.m_Value1);
			if (ashidx == 0) {
				List<TStageCardTable> ashs = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Ash);
				if (ashs.Count > 0) ashidx = ashs[UTILE.Get_Random(0, ashs.Count)].m_Idx;
			}
			if (ashidx == 0) ashidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Ash);
			StageCardInfo info = item.m_Info;
			item.SetCardChange(ashidx);
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		m_Check.Check(StageCheckType.SuppressionF, 0, TargetCards.Count);
		// 이펙트 시간만큼 대기
		yield return new WaitForSeconds(1f);
		//yield return new WaitWhile(() => eff != null);

		yield return Action_TargetOff(TargetCards);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	IEnumerator Action_FireBomb(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncards = ActiveCards[i];
			if (oncards == targetcard) continue;
			if (oncards.IS_Die()) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		List<Item_Stage> targets = new List<Item_Stage>() { targetcard };
		ShowArea(true, targets);

		yield return new WaitWhile(() => offcards.Count > 0);


		PlayEffSound(SND_IDX.SFX_0617);
		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_FireBomb");
		eff.transform.localScale *= 0.6f;

		if (targetcard.m_Info.m_NowTData.m_Type == StageCardType.OldMine || targetcard.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
			yield return SelectAction_StageCardProc_OldMine(targetcard);
		else if (targetcard.m_Info.IS_OilGas()) {
			yield return SelectAction_StageCardProc_OilGasStation(targetcard);
		}
		else {
			if (targetcard.m_Info.m_TData.m_Type == StageCardType.Chain) {
				yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.3f, Vector3.one * 0.01f);//0.4->0.3
				targetcard.m_ChainDieEffPosX = targetcard.transform.position.x;
				yield return SelectAction_ChainDie(targets);
			}
			else {
				int fireidx = Mathf.RoundToInt(card.m_Info.m_TData.m_Value2);
				if (fireidx == 0) {
					List<TStageCardTable> fires = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Fire);
					if (fires.Count > 0) fireidx = fires[UTILE.Get_Random(0, fires.Count)].m_Idx;
				}
				if (fireidx == 0) fireidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Fire);

				// 이펙트 등록
				if (targetcard.m_Info.IS_EnemyCard) {
					int dmg = GetAtkDmg(targetcard, null, false, false, true, card);

					targetcard.SetDamage(false, dmg);

					if (targetcard.IS_Die()) {
						m_Check.Check(StageCheckType.Fire_Enemy, (int)targetcard.m_Info.m_TEnemyData.m_Type, 1);
						m_Check.Check(StageCheckType.Fire_Card, (int)targetcard.m_Info.m_TData.m_Type, 1);

						GetKillEnemy(targetcard, null, false, true);

						offcards.Add(targetcard);

						if(targetcard.m_Info.m_TEnemyData.m_Tribe == EEnemyTribe.Animal) {
							fireidx = UTILE.Get_Random(0f, 1f) >= 0.6f ? BURNMEAT_CARDIDX : RIPEMEAT_CARDIDX;
						}
						targetcard.SetCardChange(fireidx);//잿더미 인덱스
						targetcard.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
					}
					else {
						if (UTILE.Get_Random(0f, 1f) < BaseValue.GetBurnProp()) {//
															//화상 설정 생성
							if (IS_BurnInfoCard(targetcard)) PlusBurnInfo(targetcard, 3);
							else {
								GameObject Eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Fire_Top", true);
								Eff.transform.localScale = Vector3.one;
								BurnInfo burn = new BurnInfo(BurnMode.AREA1);
								burn.SetTarget(targetcard);
								burn.SetTurn(3);
								burn.SetEff(Eff);
								AddBurnInfo(burn);

								// 라이트 설정 생성
								LightInfo lightinfo = new LightInfo(LightMode.LightStick, 1);
								lightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos);
								lightinfo.SetTurn(3);
								AddLight(lightinfo);
								//원형가로등추가
								StreetLightInfo streetlightinfo = new StreetLightInfo();
								streetlightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos, true);
								streetlightinfo.SetTurn(3);
								//이펙트 등록
								Eff = UTILE.LoadPrefab("Effect/Stage/Eff_StageCard_StreetLight", true, STAGE.m_DarkMaskPanel);
								Eff.transform.position = m_ViewCard[targetcard.m_Line][targetcard.m_Pos].transform.position;
								Eff.transform.localScale = Vector3.one;
								streetlightinfo.SetEff(Eff);
								//라이트 추가
								AddStreetLightInfo(streetlightinfo);
							}
						}
					}
				}
				else if (targetcard.m_Info.IS_BurnTarget()) {
					m_Check.Check(StageCheckType.Fire_Card, (int)targetcard.m_Info.m_TData.m_Type, 1);

					offcards.Add(targetcard);
					targetcard.SetCardChange(fireidx);//잿더미 인덱스
					targetcard.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
				}
				yield return new WaitWhile(() => offcards.FindAll(o => o != null).Count > 0);
			}
		}

		yield return Action_TargetOff(new List<Item_Stage>() { targetcard });

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_FireGun(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		if (m_AutoPlay) {
			targetcard = ActiveCards[UTILE.Get_Random(0, ActiveCards.Count)];
		}
		else {
			AutoCamPosInit = false;
			yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		}
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		BoomAreaTarget(card, targetcard, out AreaCards, out TargetCards, startline);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();

		PlayEffSound(SND_IDX.SFX_0618);
		// 이펙트
		StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_FireGun");

		yield return IE_CamAction(CamActionType.Shake_0, 1f, 0.35f, Vector3.one * 0.025f);//0.5->0.35

		// 타겟 데미지 주기
		for (int i = 0;i< TargetCards.Count;i++) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { TargetCards });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { TargetCards });
			}
			else {
				if (item.m_Info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = targetcard.transform.position.x;
					chains.Add(item);
				}
				else {
					int fireidx = Mathf.RoundToInt(card.m_Info.m_TData.m_Value2);
					if (fireidx == 0) {
						List<TStageCardTable> fires = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Fire);
						if (fires.Count > 0) fireidx = fires[UTILE.Get_Random(0, fires.Count)].m_Idx;
					}
					if (fireidx == 0) fireidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Fire);

					// 이펙트 등록
					if (item.m_Info.IS_EnemyCard) {
						int dmg = GetAtkDmg(item, null, false, false, true, card);

						item.SetDamage(false, dmg);
						if (item.IS_Die()) {
							m_Check.Check(StageCheckType.Fire_Enemy, (int)item.m_Info.m_TEnemyData.m_Type, 1);
							m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);
							GetKillEnemy(item, null, false, true);

							offcards.Add(item);

							if (item.m_Info.m_TEnemyData.m_Tribe == EEnemyTribe.Animal) {
								fireidx = UTILE.Get_Random(0f, 1f) >= 0.6f ? BURNMEAT_CARDIDX : RIPEMEAT_CARDIDX;
							}
							item.SetCardChange(fireidx);//잿더미 인덱스
							item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
						}
						else {
							//화상 설정 생성
							if (UTILE.Get_Random(0f, 1f) < BaseValue.GetBurnProp()) {
								if (IS_BurnInfoCard(targetcard)) PlusBurnInfo(item, 3);
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
					}
					else if (item.m_Info.IS_BurnTarget()) {
						m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);

						offcards.Add(item);
						item.SetCardChange(fireidx);//잿더미 인덱스
						item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
					}
				}
			}
		}

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		//yield return new WaitWhile(() => offcards.FindAll(o => o != null).Count > 0);
		yield return new WaitWhile(() => offcards.FindAll(o => TargetCards.Contains(o) && o != null).Count > 0);

		yield return Action_TargetOff(TargetCards);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_NapalmBomb(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		if (m_AutoPlay) {
			targetcard = ActiveCards[UTILE.Get_Random(0, ActiveCards.Count)];
		}
		else {
			AutoCamPosInit = false;
			yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		}
		ShowArea(false);

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 타겟 지점에서부터 3*3 위치
		//	4,5		4,6		4,7
		//	3,4		3,5		3,6
		//	2,3		2,4		2,5
		BoomAreaTarget(card, targetcard, out AreaCards, out TargetCards, startline);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			if (oncard.IS_Die()) continue;
			if (TargetCards.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, AreaCards);

		yield return new WaitWhile(() => offcards.Count > 0);

		List<Item_Stage> chains = new List<Item_Stage>();

		PlayEffSound(SND_IDX.SFX_0619);
		// 이펙트
		StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_NapalmBomb");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.35f, Vector3.one * 0.025f);//0.5->0.35

		// 타겟 데미지 주기
		for (int i = 0;i< TargetCards.Count;i++) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { TargetCards });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { TargetCards });
			}
			else {
				if (item.m_Info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = targetcard.transform.position.x;
					chains.Add(item);
				}
				else {
					int fireidx = Mathf.RoundToInt(card.m_Info.m_TData.m_Value2);
					if (fireidx == 0) {
						List<TStageCardTable> fires = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == StageCardType.Fire);
						if (fires.Count > 0) fireidx = fires[UTILE.Get_Random(0, fires.Count)].m_Idx;
					}
					if (fireidx == 0) fireidx = STAGEINFO.GetDefaultCardIdx(StageCardType.Fire);

					// 이펙트 등록
					if (item.m_Info.IS_EnemyCard) {
						int dmg = GetAtkDmg(item, null, false, false, true, card);

						item.SetDamage(false, dmg);
						if (item.IS_Die()) {
							m_Check.Check(StageCheckType.Fire_Enemy, (int)item.m_Info.m_TEnemyData.m_Type, 1);
							m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);
							GetKillEnemy(item, null, false, true);

							offcards.Add(item);
							if (item.m_Info.m_TEnemyData.m_Tribe == EEnemyTribe.Animal) {
								fireidx = UTILE.Get_Random(0f, 1f) >= 0.6f ? BURNMEAT_CARDIDX : RIPEMEAT_CARDIDX;
							}
							item.SetCardChange(fireidx);//잿더미 인덱스
							item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); }); 
						}
						else {
							//화상 설정 생성
							if (UTILE.Get_Random(0f, 1f) < BaseValue.GetBurnProp()) {
								if (IS_BurnInfoCard(targetcard)) PlusBurnInfo(item, 3);
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
					}
					else if (item.m_Info.IS_BurnTarget()) {
						m_Check.Check(StageCheckType.Fire_Card, (int)item.m_Info.m_TData.m_Type, 1);

						offcards.Add(item);
						item.SetCardChange(fireidx);//잿더미 인덱스
						item.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); }); 
					}
				}
			}
		}

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		//yield return new WaitWhile(() => offcards.FindAll(o => o != null).Count > 0);
		yield return new WaitWhile(() => offcards.FindAll(o => TargetCards.Contains(o) && o != null).Count > 0);

		yield return Action_TargetOff(TargetCards);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_DownLevel(Item_Stage _card, List<Item_Stage> ActiveCards, int startline = 1) {
		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		List<Item_Stage> offcards = new List<Item_Stage>();

		int Area = 3;
		int ShiftArea = Area / 2;
		int StartLine = targetcard.m_Line - ShiftArea;
		int EndLine = StartLine + Area;
		int StartPos = targetcard.m_Pos - ShiftArea - ShiftArea;
		for (int i = StartLine; i < EndLine; i++, StartPos++) {
			if (i < 0) continue;
			if (i > AI_MAXLINE) continue;
			int EndPos = StartPos + Area;
			for (int j = StartPos; j < EndPos; j++) {
				if (j < 0) continue;
				if (j >= m_ViewCard[i].Length) continue;
				Item_Stage item = m_ViewCard[i][j];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				if (!item.m_Info.IS_EnemyCard) continue;
				if (!area.Contains(item)) area.Add(item);
				if (!targets.Contains(item)) targets.Add(item);
				item.Action(EItem_Stage_Card_Action.TargetOn);
			}
		}

		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0471);

		TStageCardTable table = _card.m_Info.m_TData;
		float downratio = UTILE.Get_Random(table.m_Value1, table.m_Value2);
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			StartEff(item.transform, "Effect/Stage/Eff_Debuff_Card");
			offcards.Add(item);
			item.m_Info.m_LV = Mathf.Max(1, Mathf.FloorToInt(item.m_Info.m_LV * (100f - downratio) * 0.01f));
			item.Action(EItem_Stage_Card_Action.ChangeVal, 0.6f, (obj) => {//1->0.6
				offcards.Remove(item);
			});
		}
		yield return new WaitForSeconds(1f);
		yield return new WaitWhile(() => offcards.Count > 0);
		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_CardPull(Item_Stage _card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		yield return new WaitWhile(() => Mathf.Abs(CamMoveGapX) > 0.01f);
		// 중앙 카드 제거
		List<Item_Stage> actioncards = new List<Item_Stage>();

		Item_Stage MoveCard = null;
		int pos = 0;
		if (startline == 0) {
			MoveCard = m_ViewCard[0][1];
		}
		else {
			pos = _card.m_Pos + 1;
			MoveCard = m_ViewCard[1][pos];
		}
		
		actioncards.Add(MoveCard);

		MoveCard.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
			// 카드 pool 이동
			actioncards.Remove(obj);
		});

		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncards = ActiveCards[i];
			if (oncards == targetcard) continue;
			if (oncards == MoveCard) continue;
			actioncards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { actioncards.Remove(obj); });
		}

		yield return new WaitWhile(() => actioncards.Count > 0);

		// 선택카드 자리 변경
		m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = MoveCard;
		m_ViewCard[startline][pos] = targetcard;
		actioncards.Add(targetcard);
		targetcard.Action(EItem_Stage_Card_Action.MoveTarget, 0, (obj) => {
			actioncards.Remove(obj);
		}, MoveCard);

		// 앞라인 켜주기
		for (int i = 0; i < m_ViewCard[startline].Length; i++) {
			if (i == pos) continue;
			Item_Stage item = m_ViewCard[startline][i];
			ActiveCards.Remove(item);
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		yield return new WaitWhile(() => actioncards.Count > 0);

		actioncards.Add(targetcard);
		targetcard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { actioncards.Remove(obj); });
		yield return new WaitWhile(() => actioncards.Count > 0);

		m_ViewCard[MoveCard.m_Line][MoveCard.m_Pos] = null;
		RemoveStage(MoveCard);
		AutoCamPosInit = true;
		m_ActionCor = null;
	}

	IEnumerator Action_Explosion(Item_Stage _card, List<Item_Stage> ActiveCards, int startline = 1) {
		List<Item_Stage> showcards = new List<Item_Stage>(ActiveCards);
		ActiveCards.Clear();
		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		if (showcards.Count > 0) ShowArea(true, showcards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, showcards);
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		//
		int Area = 3;
		int ShiftArea = Area / 2;
		int StartLine = targetcard.m_Line - ShiftArea;
		int EndLine = StartLine + Area;
		int StartPos = targetcard.m_Pos - ShiftArea - ShiftArea;
		for (int i = StartLine; i < EndLine; i++, StartPos++) {
			if (i < 0) continue;
			if (i > AI_MAXLINE) continue;
			int EndPos = StartPos + Area;
			for (int j = StartPos; j < EndPos; j++) {
				if (j < 0) continue;
				if (j >= m_ViewCard[i].Length) continue;
				Item_Stage item = m_ViewCard[i][j];
				if (item == null) continue;
				if (item.IS_Die()) continue;
				if (item.IS_Lock) continue;
				if (item.m_Info.IS_RoadBlock) continue;
				TStageCardTable tdata = item.m_Info.m_NowTData;
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
						continue;
				}
				if (!area.Contains(item)) area.Add(item);
				if (!ActiveCards.Contains(item)) ActiveCards.Add(item);
				item.Action(EItem_Stage_Card_Action.TargetOn);
			}
		}
		// 대상이 아닌놈들 꺼주기
		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = showcards.Count - 1; i > -1; i--) {
			Item_Stage oncards = showcards[i];
			if (ActiveCards.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		m_Area.transform.position = m_Stage.Panel[1].transform.position;
		m_Area.Clear();

		Dictionary<Item_Stage, List<Item_Stage>> alltargets = new Dictionary<Item_Stage, List<Item_Stage>>();
		List<Item_Stage> chains = new List<Item_Stage>();
		List<Item_Stage> checkAction = new List<Item_Stage>();
		List<Item_Stage> actioncards = new List<Item_Stage>();
		int atk = Mathf.RoundToInt(m_User.GetStat(StatType.Atk));
		for (int m = ActiveCards.Count - 1; m > -1; m--) {
			Item_Stage card = ActiveCards[m];
			TStageCardTable tdata = card.m_Info.m_NowTData;
			StageCardType cardtype = tdata.m_Type;
			area.Clear();
			List<Item_Stage> targets;
			BoomAreaTarget(card, card, out area, out targets, 1, cardtype == StageCardType.C4);
			m_Area.AddCard(area);
			alltargets.Add(card, targets);
		}

		m_Area.Show();

		List<Item_Stage> EnemyDieCheck = new List<Item_Stage>();
		// 연출의 꼬임 방지
		// 죽는 카드
		for (int m = ActiveCards.Count - 1; m > -1; m--) {
			Item_Stage card = ActiveCards[m];
			TStageCardTable tdata = card.m_Info.m_NowTData;
			StageCardType cardtype = tdata.m_Type;
			List<Item_Stage> targets = alltargets[card];
			CardActionDamageType damagetype = GetActionDamageType(cardtype);
			if (damagetype != CardActionDamageType.Die && cardtype != StageCardType.OldMine && cardtype != StageCardType.Allymine) continue;
			PlayEffSound(SND_IDX.SFX_0410);
			switch (cardtype) {
				case StageCardType.Grenade:
				case StageCardType.TimeBomb:
				case StageCardType.OldMine:
				case StageCardType.Allymine:
					StartEff(card.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");
					break;
				case StageCardType.Dynamite:
					StartEff(card.transform, "Effect/Stage/Eff_StagCard_Dynamite");
					break;
				case StageCardType.ShockBomb:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_Sniping");
					break;
				case StageCardType.FireBomb:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_FireBomb");
					break;
				case StageCardType.C4:
					GameObject eff = StartEff(card.transform, "Effect/Stage/Eff_StageCard_Sniping");
					eff.transform.localPosition += new Vector3(0, BaseValue.STAGE_INTERVER.y * 3f, 0);
					eff.transform.localScale *= 5f;
					break;
				case StageCardType.NapalmBomb:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_NapalmBomb");
					break;
				default:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_Sniping");
					break;
			}

			for (int i = targets.Count - 1; i > -1; i--) {
				Item_Stage item = targets[i];
				StageCardInfo info = item.m_Info;
				TStageCardTable targettdata = info.m_NowTData;
				if (targettdata.m_Type == StageCardType.Chain) {
					if (chains.Find(t => t.m_Line == item.m_Line) == null) {
						item.m_ChainDieEffPosX = card.transform.position.x;
						chains.Add(item);
					}
				}
				else {
					if (!EnemyDieCheck.Contains(item) && item.m_Info.IS_EnemyCard) {
						EnemyDieCheck.Add(item);
						item.PlayHitSND();
						GetKillEnemy(item, null, false, true);
					}

					if (!checkAction.Contains(item)) {
						actioncards.Add(item);
						checkAction.Add(item);
						item.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
							actioncards.Remove(obj);
							// 카드 pool 이동;
							RemoveStage(obj);
						});
						m_ViewCard[item.m_Line][item.m_Pos] = null;
					}
				}
			}

			if (!checkAction.Contains(card)) {
				actioncards.Add(card);
				checkAction.Add(card);
				card.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
					// 카드 pool 이동;
					actioncards.Remove(obj);
					RemoveStage(obj);
				});
				m_ViewCard[card.m_Line][card.m_Pos] = null;
			}
		}

		// 데미지 카드
		for (int m = ActiveCards.Count - 1; m > -1; m--) {
			Item_Stage card = ActiveCards[m];
			TStageCardTable tdata = card.m_Info.m_NowTData;
			StageCardType cardtype = tdata.m_Type;
			List<Item_Stage> targets = alltargets[card];
			CardActionDamageType damagetype = GetActionDamageType(cardtype);
			if (damagetype != CardActionDamageType.Damage || cardtype == StageCardType.OldMine || cardtype == StageCardType.Allymine) continue;
			PlayEffSound(SND_IDX.SFX_0410);

			switch (cardtype) {
				case StageCardType.Grenade:
				case StageCardType.TimeBomb:
				case StageCardType.OldMine:
				case StageCardType.Allymine:
					StartEff(card.transform, "Effect/Stage/Eff_StagCard_Grenade_3x3");
					break;
				case StageCardType.Dynamite:
					StartEff(card.transform, "Effect/Stage/Eff_StagCard_Dynamite");
					break;
				case StageCardType.ShockBomb:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_Sniping");
					break;
				case StageCardType.FireBomb:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_FireBomb");
					break;
				case StageCardType.C4:
					GameObject eff = StartEff(card.transform, "Effect/Stage/Eff_StageCard_Sniping");
					eff.transform.localPosition += new Vector3(0, BaseValue.STAGE_INTERVER.y * 3f, 0);
					eff.transform.localScale *= 5f;
					break;
				case StageCardType.NapalmBomb:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_NapalmBomb");
					break;
				default:
					StartEff(card.transform, "Effect/Stage/Eff_StageCard_Sniping");
					break;
			}

			for (int i = targets.Count - 1; i > -1; i--) {
				Item_Stage item = targets[i];
				StageCardInfo info = item.m_Info;
				TStageCardTable targettdata = info.m_NowTData;
				if (targettdata.m_Type == StageCardType.Chain) {
					if (chains.Find(t => t.m_Line == item.m_Line) == null) {
						item.m_ChainDieEffPosX = card.transform.position.x;
						chains.Add(item);
					}
				}
				else {
					if (item.m_Info.m_TEnemyData == null) continue;
					int Damage = 0;
					switch (cardtype) {
						case StageCardType.Grenade:
							Damage = GetAtkDmg(item, null, false, false, true, card);
							break;
						case StageCardType.TimeBomb:
							Damage = Mathf.RoundToInt(info.GetMaxStat(EEnemyStat.HP) * card.m_Info.m_TData.m_Value1) + Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(item.m_Info.m_TEnemyData));
							break;
						case StageCardType.OldMine:
						case StageCardType.Allymine:
							Damage = card.m_Info.GetMaxStat(EEnemyStat.HP);
							break;
					}
					int dmg = Damage;

					item.SetDamage(false, dmg, 1, 0, false);
					if (!EnemyDieCheck.Contains(item) && item.IS_Die()) {
						EnemyDieCheck.Add(item);
						GetKillEnemy(item, null, false, true);
					}
					if (!checkAction.Contains(item)) {
						checkAction.Add(item);
						actioncards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
							actioncards.Remove(obj);
						});
					}
				}
			}

			if (!checkAction.Contains(card)) {
				actioncards.Add(card);
				checkAction.Add(card);
				card.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) => {
					// 카드 pool 이동;
					actioncards.Remove(obj);
					RemoveStage(obj);
				});
				m_ViewCard[card.m_Line][card.m_Pos] = null;
			}
		}
		yield return IE_CamAction(CamActionType.Shake_0, 0f, 1.2f, Vector3.one * 0.01f);//1.83->1.2
		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		yield return new WaitWhile(() => actioncards.Count > 0);

		yield return new WaitForSeconds(0.5f);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_Drill(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		Item_Stage targetcard = null;
		int cnt = Mathf.Max(1, Mathf.RoundToInt(card.m_Info.m_TData.m_Value1));

		for (int repeat = 0; repeat < cnt; repeat++) {
			if (ActiveCards.Count < 1) break;

			for (int i = 0; i < ActiveCards.Count; i++) {
				ActiveCards[i].ActiveDark(true, true);
				ActiveCards[i].FlashingDark(true);
			}
			AutoCamPosInit = false;
			if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
			yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
			ShowArea(false);

			for (int i = 0; i < ActiveCards.Count; i++) {
				ActiveCards[i].FlashingDark(false);
			}
			List<Item_Stage> offcards = new List<Item_Stage>();
			// 대상이 아닌놈들 꺼주기
			for (int i = ActiveCards.Count - 1; i > -1; i--) {
				Item_Stage oncards = ActiveCards[i];
				if (oncards == targetcard) continue;
				if (oncards.IS_Die()) continue;
				offcards.Add(oncards);
				oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
			}

			List<Item_Stage> targets = new List<Item_Stage>() { targetcard };
			ShowArea(true, targets);

			yield return new WaitWhile(() => offcards.Count > 0);


			PlayEffSound(SND_IDX.SFX_0620);
			// 이펙트
			GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Drill");
			eff.transform.localScale *= 0.6f;
			yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.3f, Vector3.one * 0.008f);//0.4->0.3

			yield return new WaitForSeconds(0.4f);

			if (targetcard.m_Info.m_TData.m_Type == StageCardType.Chain) {
				targetcard.m_ChainDieEffPosX = targetcard.transform.position.x;
				yield return SelectAction_ChainDie(targets);
			}
			else {
				offcards.Add(targetcard);
				targetcard.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
					// 카드 pool 이동
					offcards.Remove(obj);
					RemoveStage(obj);
				});
				m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = null;
				yield return new WaitWhile(() => offcards.Count > 0);
			}

			ActiveCards.Remove(targetcard);

			yield return Check_NullCardAction();
		}
		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_RandomAtk(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(null, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.m_Info.IS_RoadBlock) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets, SkillAreaType.Area05);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncards = ActiveCards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		TStageCardTable table = card.m_Info.m_NowTData;

		List<Item_Stage> chains = new List<Item_Stage>();
		List<Item_Stage> Diecheck = new List<Item_Stage>();
		List<Item_Stage> check = new List<Item_Stage>();
		check.AddRange(targets);
		int atk = Mathf.RoundToInt(m_User.GetStat(StatType.Atk));
		// N회 공격
		for (int k = Mathf.RoundToInt(table.m_Value2); k > 0; k--) {
			List<Item_Stage> list = targets.FindAll(t => {
				if (t == null) return false;
				if (t.IS_Die()) return false;
				if (chains.Contains(t)) return false;
				if (chains.Find(a => a.m_Line == t.m_Line) != null) return false;
				return true;
			});

			if (list.Count < 1) break;
			Item_Stage item = list[UTILE.Get_Random(0, list.Count)]; 
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			PlayEffSound(SND_IDX.SFX_0400);
			// 이펙트
			GameObject eff = StartEff(item.transform, "Effect/Stage/Eff_StageCard_Sniping");
			eff.transform.localScale *= 0.6f;
			check.Remove(item);
			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { targets });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { targets });
			}
			else {
				if (item.m_Info.IS_EnemyCard) {
					int dmg = GetAtkDmg(item, null, false, false, true, card);

					item.SetDamage(false, dmg);
					if (!Diecheck.Contains(item) && item.IS_Die()) {
						Diecheck.Add(item);
						GetKillEnemy(item, null, false, true);
					}
				}
				else if (item.m_Info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = item.transform.position.x;
					chains.Add(item);
				}
			}
			yield return new WaitForSeconds(UTILE.Get_Random(0.1f, 0.5f));

			CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);
		}

		// 대상이 아닌놈들 꺼주기
		for (int i = check.Count - 1; i > -1; i--) {
			Item_Stage oncards = check[i];
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_StopCard(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(null, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_AIEnemy()) return false;
			return true;
		}, ref area, ref targets, SkillAreaType.Area05);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncard = ActiveCards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		TStageCardTable table = card.m_Info.m_NowTData;

		float times = table.m_Value1;
		int atk = Mathf.RoundToInt(m_User.GetStat(StatType.Atk));
		atk += Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(targetcard.m_Info.m_TEnemyData));

		PlayEffSound(SND_IDX.SFX_0471);
		for (int i = 0; i < targets.Count; i++) {
			int dmg = GetAtkDmg(targets[i], null, false, false, true, card);

			targets[i].SetDamage(false, dmg);
			if (targets[i].IS_Die()) {
				GetKillEnemy(targets[i], null, false, true);
			}
			else {
				StartEff(targets[i].transform, "Effect/Stage/Eff_Debuff_Card");
				// 이펙트 등록
				GameObject Eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Paralysis", true);
				Eff.transform.localScale = Vector3.one;

				//정지 설정 생성
				AIStopInfo aistop = new AIStopInfo(AiStopMode.AREA1);
				aistop.SetTarget(targets[i]);
				aistop.SetTurn(Mathf.RoundToInt(table.m_Value2));
				aistop.SetEff(Eff);
				AddAIStopInfo(aistop);

				//원거리 공격 차단 생성
				AiBlockRangeAtkInfo aiblockrangeastk = new AiBlockRangeAtkInfo(AiBlockRangeAtkMode.AREA1);
				aiblockrangeastk.SetTarget(targets[i]);
				aiblockrangeastk.SetTurn(Mathf.RoundToInt(table.m_Value2));
				AddAiBlockRangeAtkInfo(aiblockrangeastk);
			}
		}
		yield return new WaitForSeconds(1f);
		AutoCamPosInit = true;
		m_ActionCor = null;
	}
	IEnumerator Action_PlusMate(Item_Stage card, List<Item_Stage> ActiveCards, int startline = 1) {
		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		if (ActiveCards.Count > 0) ShowArea(true, ActiveCards);
		yield return SelectAction_StageCardProc_CheckSelect((obj) => { targetcard = obj; }, startline, ActiveCards);
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(null, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Material) return false;
			return true;
		}, ref area, ref targets, SkillAreaType.Area05);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = ActiveCards.Count - 1; i > -1; i--) {
			Item_Stage oncards = ActiveCards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			item.m_Info.m_PlusCnt += Mathf.RoundToInt(card.m_Info.m_NowTData.m_Value2);
			item.Action(EItem_Stage_Card_Action.ChangeVal, 0.6f, (obj) => {//1->0.6
				targets.Remove(obj);
			});
		}

		yield return new WaitWhile(() => targets.Count > 0);

		AutoCamPosInit = true;
		m_ActionCor = null;
	}
}
