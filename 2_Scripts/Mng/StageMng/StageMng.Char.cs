using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	IEnumerator m_CorSkill;//스킬 사용 코루틴
	Item_Stage_Char m_SkillChar;
	Stage_CardUse m_SkillUseInfoPopup;
	IEnumerator UseUseSkill(Item_Stage_Char Char) {
		FirstLineBump(false);
		m_MainUI.SetPathLine();
		Char.GetComponent<SortingGroup>().sortingOrder = 5;
		TSkillTable table = Char.m_Info.m_Skill[0].m_TData;
		SkillKind skill = table.m_Kind;
		//if (Char.IS_UseActiveSkill()) 
		if (m_CorSkill != null) {
			m_CorSkill = null;
		}
		m_SkillChar = null;

		//ASBY:20210518
		m_CorSkill = UseUseSkill(Char, table);
		m_SkillChar = Char;
		StartCoroutine(m_CorSkill);

		yield return new WaitUntil(() => m_CorSkill == null);
		//ASBY:20210518
		//yield return UseUseSkill(Char, table);

		yield return Check_LightOnOff();

		Char.GetComponent<SortingGroup>().sortingOrder = 4;

		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.Char_0_1021_SkillEnd)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Char1_1031_SkillEnd)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Char2_1052_SkillEnd)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Char3_1029_SkillEnd)) TUTO.Next();
		//if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Char3_SkillEnd)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Char2_SkillEnd)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.Char1_SkillEnd)) TUTO.Next();

		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.DL_155))
			yield return new WaitWhile(() => TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.DL_155));

		//if(TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.DL_1095))
		//	yield return new WaitWhile(() => TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.DL_1095));
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		// 피난민등 자동으로 내려오게 하기위해 체크
		yield return Check_NullCardAction();

		//첫줄 선택시 방향 표시
		m_MainUI.SetPathLine(m_ViewCard[0][0], m_ViewCard[0][1], m_ViewCard[0][2]);
		FirstLineBump(true);
		m_StageAction = null;
	}
	IEnumerator UseUseSkill(Item_Stage_Char Char, TSkillTable skill) {
		switch (skill.m_Kind)
		{
			/// <summary> 정신력 맥시멈 수치가 증가한다. </summary>
			case SkillKind.MenMax:
				break;
			/// <summary> 위생력 맥시멈 수치가 증가한다. </summary>
			case SkillKind.HygMax:
				break;
			/// <summary> 허기짐 맥시멈 수치가 증가한다. </summary>
			case SkillKind.SatMax:
				break;
			/// <summary> 음식 섭취 시 허기짐 수치가 추가로 감소한다. </summary>
			case SkillKind.SatChargUp:
				break;
			/// <summary> 위생 수치가 감소되는 모든 상황에서 덜 감소한다. </summary>
			case SkillKind.Immunity:
				break;
			/// <summary> 포만감 수치가 감소되는 모든 상황에서 덜 감소한다. </summary>
			case SkillKind.LessEater:
				break;
			/// <summary> 정신력 수치가 감소되는 모든 상황에서 덜 감소한다. </summary>
			case SkillKind.CoolMental:
				break;
			/// <summary> 치료 아이템 사용 시 일정 확률로 치료 효과가 2배가 된다. </summary>
			case SkillKind.RecoveryUp:
				break;
			/// <summary> 음식 섭취 시 배고픔 감소 및 정신력 수치도 증가한다. </summary>
			case SkillKind.Epicurean:
				break;
			/// <summary> 크리티컬 공격 대미지가 증가한다. </summary>
			case SkillKind.CriUp:
				break;
			/// <summary> 일반노트 대미지가 증가한다. </summary>
			case SkillKind.NormalNoteAtkUp:
				break;
			/// <summary> 슬래쉬노트 대미지가 증가한다. </summary>
			case SkillKind.SliceNoteAtkUp:
				break;
			/// <summary> 콤보노트 대미지가 증가한다. </summary>
			case SkillKind.ComboNoteAtkUp:
				break;
			/// <summary> 체인노트 대미지가 증가한다. </summary>
			case SkillKind.ChainNoteAtkUp:
				break;
			/// <summary> 챠지노트 대미지가 증가한다. </summary>
			case SkillKind.ChargeNoteAtkUp:
				break;
			/// <summary> 콤보 성공횟수에 따라 직후 하는 굿판정 이상의 공격에 추가대미지를 준다. </summary>
			case SkillKind.ComboAddDmg:
				break;
			/// <summary> 목표 타겟 하나에게 큰 데미지(사용 캐릭터의 공격력 x 효과 비율) </summary>
			case SkillKind.HeadShot:
				yield return CharUseSkill_HeadShot(Char, skill);
				break;
			/// <summary> HP 회복(사용 캐릭터의 공격력 x 효과 비율) </summary>
			case SkillKind.Heal:
				yield return CharUseSkill_Heal(Char, skill);
				break;
			/// <summary> 스킬 사용 시 HP와 청결도를 회복한다. (HP 회복 값 : 사용 캐릭터의 회복력 x ValueBase, Value01 = 스테이터스 타입, Value02 = 스테이터스 회복량 (백분율)) </summary>
			case SkillKind.HealPlus:
				yield return CharUseSkill_HealPlus(Char, skill);
				break;
			/// <summary> 전방 3칸에 데미지(사용 캐릭터의 공격력 x 효과 비율) </summary>
			case SkillKind.ShotGun:
				yield return CharUseSkill_ShotGun(Char, skill);
				break;
			/// <summary> 긴급 처방, 정신력이 크게 감소하지만 체력을 회복한다.(Value 01 = 감소 정신력, Value 02 = 회복 체력 (만분율)) </summary>
			case SkillKind.PainHeal:
				yield return CharUseSkill_PainHeal(Char, skill);
				break;
			/// <summary> 이번 선택을 생략하고 다음 라인으로 한칸 이동합니다. </summary>
			case SkillKind.Jump:
				yield return CharUseSkill_Jump(Char, skill);
				break;
			/// <summary> 목표 카드 한장을 현재 중간 위치로 당겨옵니다. </summary>
			case SkillKind.CardPull:
				yield return CharUseSkill_CardPull(Char, skill);
				break;
			/// <summary> 목표 1개 라인의 모든 적들에게 피해를 준다. </summary>
			case SkillKind.AirStrike:
				yield return CharUseSkill_AirStrike(Char, skill);
				break;
			/// <summary> 전방 3장의 카드 중 하나를 선택하여 높은 데미지를 준다. </summary>
			case SkillKind.BackStep:
				yield return CharUseSkill_BackStep(Char, skill);
				break;
			/// <summary> 아무 액티브 스킬 중 하나를 랜덤으로 사용한다. CoolReset 제외 </summary>
			case SkillKind.LearningAbility:
				yield return CharUseSkill_LearningAbility(Char, skill);
				break;
			/// <summary> 선택한 특정 카드들을 모두 특정 카드로 변경한다. (Value02 = 변경될 카드 ID) </summary>
			case SkillKind.ChangeCard:
				yield return CharUseSkill_ChangeCard(Char, skill);
				break;
			case SkillKind.ChangeCard02:
				yield return CharUseSkill_ChangeCard2(Char, skill);
				break;
			/// <summary> Area 범위 내의 모든 폭발성 기믹
			/// <para>(수류탄, 다이너마이트, 화염병, 오래된 지뢰, 시한 폭탄)을 발동시킨다.</para> </summary>
			case SkillKind.Explosion:
				yield return CharUseSkill_Explosion(Char, skill);
				break;
			/// <summary> 가로 한 행을 전체 공격한다. (여자모드 스킬), 세로 한 열을 전체 공격한다. (남자모드 스킬) </summary>
			case SkillKind.TransverseAtk:
				yield return CharUseSkill_TransverseAtk(Char, skill);
				break;
			/// <summary> 아군 캐릭터 스킬쿨타임 초기화 </summary>
			case SkillKind.CoolReset:
				yield return CharUseSkill_CoolReset(Char, skill);
				break;   
			/// <summary> (보스 사용 불가) 최대 체력을 40% 소모하여 적 하나를 반드시 처치한다. </summary>
			case SkillKind.BlowAtk:
				yield return CharUseSkill_BlowAtk(Char, skill);
				break;
			/// <summary> Area 범위 내 모든 피난민 카드를 한 번에 획득한다. </summary>
			case SkillKind.Incitement:
				yield return CharUseSkill_Incitement(Char, skill);
				break;
			/// <summary> Area 범위 내 적 하나의 이동을 3턴 제한한다. </summary>
			case SkillKind.StopCard:
				yield return CharUseSkill_StopCard(Char, skill);
				break;
				/// <summary> Area 범위 내 적 하나의 이동을 3턴 제한한다. </summary>
			case SkillKind.StopCardPlus:
				yield return CharUseSkill_StopCardPlus(Char, skill);
				break;
			/// <summary> (어둠 스테이지)1턴 동안 선택한 한 열의 어둠이 사라진다. </summary>
			case SkillKind.SpotLight:
				yield return CharUseSkill_SpotLight(Char, skill);
				break;
			/// <summary> 선택한 한 행에 있는 모든 적의 이동을 Value02턴 제한한다. </summary>
			case SkillKind.StopCardTran:
				yield return CharUseSkill_StopCardTran(Char, skill);
				break;
			/// <summary> 선택한 Area(홀수만 가능, 1, 3, 5) 범위 카드 하나의 화염을 제거한다. </summary>
			case SkillKind.RemoveFire:
				yield return CharUseSkill_RemoveFire(Char, skill);
				break;
			/// <summary> 오래된 지뢰, 시한 폭탄 카드를 유틸리티 카드로 변환한다. </summary>
			case SkillKind.BombSpecialist:
				yield return CharUseSkill_BombSpecialist(Char, skill);
				break;
			/// <summary> Area 영역 내 모든 적을 사격하여 공격한다. </summary>
			case SkillKind.RangeAtk:
				yield return CharUseSkill_RangeAtk(Char, skill);
				break;
			/// <summary> Area 영역의 카드를 섞음 (카드 섞기과 동일 기능) </summary>
			case SkillKind.Shuffle:
				yield return CharUseSkill_Shuffle(Char, skill);
				break;
			/// <summary> 선택한 Area 범위 내 카드를 다른 카드로 변경한다. (Value02 = 변환될 스테이지 카드 ID) </summary>
			case SkillKind.ChangeCardArea:
				yield return CharUseSkill_ChangeCardArea(Char, skill);
				break;
			/// <summary> 필드 위 특정 재료 카드를 특정 그룹 내의 랜덤 카드로 변환한다. (Value01 = 바뀔 재료 카드 Type No, Value02 = IngameGroupRewardTable의 GroupIndex 참조) </summary>
			case SkillKind.ChangeRandomDrop:
				yield return CharUseSkill_ChangeRandomDrop(Char, skill);
				break;
			/// <summary> Area범위 내 무작위 적들에게 총 Value01회 공격한다. (Value 02 = 타격 횟수) </summary>
			case SkillKind.RandomAtk:
				yield return CharUseSkill_RandomAtk(Char, skill);
				break;
			/// <summary> 생존 스텟 회복 (Value01 = 스텟인덱스) </summary>
			case SkillKind.RecoverySrv:
				yield return CharUseSkill_RecoverySrv(Char, skill);
				break;
			/// <summary> 재료 카드를 다른 재료 카드로 변환한다. (Value 01 = 변환 할 재료카드 ID, Value 02 = 변환 될 재료카드 ID) </summary>
			case SkillKind.ChangeMate:
				yield return CharUseSkill_ChangeMate(Char, skill);
				break;
			/// <summary> 3턴 동안 (턴수 고정) 모든 적이 움직이지 않는다. </summary>
			case SkillKind.StopAll:
				yield return CharUseSkill_StopAll(Char, skill);
				break;
			/// <summary> 범위 내 모든 특정 재료를 습득한다. (Value02 = 재료 카드 ID) </summary>
			case SkillKind.GetCards:
				yield return CharUseSkill_GetCards(Char, skill);
				break;
			/// <summary> 범위 내 재료카드의 개수가 +1 증가한다. </summary>
			case SkillKind.PlusMate:
				yield return CharUseSkill_PlusMate(Char, skill);
				break;
				/// <summary> 액티브 스킬 사용 시 재료 카드를 습득한다. (1 ~ 5개 랜덤으로) (Value 01 = 재료 ID, Value 02 = Max 개수) </summary>
			case SkillKind.GetMate:
				yield return CharUseSkill_GetMate(Char, skill);
				break;
				/// <summary> 현재 포만도가 크게 감소하지만 체력을 회복한다.(Value 01 = 감소 포만도 (절대값), Value 02 = 회복 체력(만분율)) </summary>
			case SkillKind.PainSat:
				yield return CharUseSkill_PainSat(Char, skill);
				break;
				/// <summary> 현재 청결도가 크게 감소하지만 체력을 회복한다. (Value 01 = 감소 청결도 (절대값), Value 02 = 회복 체력(만분율)). </summary>
			case SkillKind.PainHyg:
				yield return CharUseSkill_PainHyg(Char, skill);
				break;
			case SkillKind.RecoveryMove:
				yield return CharUseSkill_RecoveryMove(Char, skill);
				break;
			/// <summary> 선택 범위 내 찢긴 시체 카드 삭제 후 약간의 정신력 회복(Value 01 = StageCardTable ID 참조, Value 02 = 정신력 회복 수치 (백분율)) </summary>
			case SkillKind.SacrificeTornBody:
				yield return CharUseSkill_SacrificeTornBody(Char, skill);
				break;
			/// <summary> 선택 범위 내 상한 음식 카드 삭제 후 약간의 포만도 회복(Value 01 = StageCardTable ID 참조, Value 02 = 포만도 회복 수치 (백분율)) </summary>
			case SkillKind.SacrificePit:
				yield return CharUseSkill_SacrificePit(Char, skill);
				break;
			/// <summary> 선택 범위 내 썩은 쓰레기장 카드 삭제 후 약간의 청결도 회복(Value 01 = StageCardTable ID 참조, Value 02 = 청결도 회복 수치 (백분율)) </summary>
			case SkillKind.SacrificeGarbage:
				yield return CharUseSkill_SacrificeGarbage(Char, skill);
				break;
			/// <summary> 선택한 범위 내 에너미의 레벨을 N ~ N% 사이 랜덤으로 다운 그레이드 한다.(Value 01 = 레벨 감소치 최소값, Value 02 = 레벨 감소치 최대값)* 남는 값은 반내림으로 처리 </summary>
			case SkillKind.DownLevel:
				yield return CharUseSkill_DownLevel(Char, skill);
				break;
			/// <summary> 청결, 포만, 정신력 모두 회복한다. (Value 02 = 3개 스테이터스 회복량 (백분율)) </summary>
			case SkillKind.AllRecoverySrv:
				yield return CharUseSkill_AllRecoverySrv(Char, skill);
				break;
			/// <summary> 스킬 사용 후 N턴간 공습이 발생하지 않는다.(Value 02 = 공습이 발생하지 않을 턴 수) </summary>
			case SkillKind.BanAirStrike:
				yield return CharUseSkill_BanAirStrike(Char, skill);
				break;
			/// <summary> 선택한 목표 타겟과 양 옆의 적들에게 데미지를 준다.(Value 02 = 데미지 : 사용 캐릭터의 공격력 x 효과 비율) </summary>
			case SkillKind.WideAttack:
				yield return CharUseSkill_WideAttack(Char, skill);
				break;
			/// <summary> 스킬 사용 시, 선택한 영역 안에 있는 특정 카테고리의 적 카드가 삭제된다. (도망치는 컨셉)(Value 02 = 적 카테고리 ID)(None = 0, 동물 = 1, 좀비 = 2, 돌연변이 = 3, 인간 = 4) </summary>
			case SkillKind.DeleteEnemyTribe:
				yield return CharUseSkill_DeleteEnemyTribe(Char, skill);
				break;
			/// <summary> 적 선택 시, 적과의 전투 이전에 해당 적의 보상을 먼저 습득한다. 해당 스킬을 사용한 적을 선택하여 전투 시에는 보상을 습득할 수 없다.(Value 02 = 스틸 실패 확률) 스틸 실패 시 다른 행동을 취할 수 없고 바로 전투가 시작된다. </summary>
			case SkillKind.SteelItem:
				yield return CharUseSkill_SteelItem(Char, skill);
				break;
			///<summary>특정 카드를 삭제하고, 삭제된 특정 카드의 개수만큼 특정 스테이터스를 회복한다 </summary>
			case SkillKind.RecoveryStatus:
				yield return CharUseSkill_RecoveryStatus(Char, skill);
				break;
			//스킬 사용 시, 보유 중인 행동력을 모두 소모하고 그 값만큼 HP를 회복한다.
			case SkillKind.APHP:
				yield return CharUseSkill_APHP(Char, skill);
				break;
			//잠긴 카드의 잠김 상태를 해제한다
			case SkillKind.Unlock:
				yield return CharUseSkill_Unlock(Char, skill);
				break;
			//현재 내 제작탭의 아이템 하나를 랜덤으로 복사한다. (제작 탭의 보유 개수가 최대일 경우 사용할 수 없음.) 
			case SkillKind.CopyMaterial:
				yield return CharUseSkill_CopyMaterial(Char, skill);
				break;
			case SkillKind.DropItemHp:
				yield return CharUseSkill_DropItemHp(Char, skill);
				break;
			case SkillKind.HPAP:
				yield return CharUseSkill_HPAP(Char, skill);
				break;
			case SkillKind.LastAttack:
				yield return CharUseSkill_LastAttack(Char, skill);
				break;
			case SkillKind.DestoryWall:
				yield return CharUseSkill_DestoryWall(Char, skill);
				break;
			case SkillKind.UnDebuff:
				yield return CharUseSkill_UnDebuff(Char, skill);
				break;
			case SkillKind.Gamble:
				yield return CharUseSkill_Gamble(Char, skill);
				break;
			case SkillKind.DarkPatrol:
				yield return CharUseSkill_DarkPatrol(Char, skill);
				break;
			case SkillKind.DontAttack:
				yield return CharUseSkill_DontAttack(Char, skill);
				break;
			case SkillKind.DeleteBadCard:
				yield return CharUseSkill_DeleteBadCard(Char, skill);
				break;
			case SkillKind.Hide:
				yield return CharUseSkill_Hide(Char, skill);
				break;
			case SkillKind.LearningAbility02:
				yield return CharUseSkill_LearningAbility2(Char, skill);
				break;
			case SkillKind.KeepMaterial:
				yield return CharUseSkill_KeepMaterial(Char, skill);
				break;
			case SkillKind.MakeRefugee:
				yield return CharUseSkill_MakeRefugee(Char, skill);
				break;
			case SkillKind.DropItemAP:
				yield return CharUseSkill_DropItemAP(Char, skill);
				break;
			case SkillKind.CountTribeMaterial:
				yield return CharUseSkill_CountTribeMaterial(Char, skill);
				break;
			case SkillKind.DestoryWall02:
				yield return CharUseSkill_DestoryWall02(Char, skill);
				break;
			case SkillKind.RandomWeapon:
				yield return CharUseSkill_RandomWeapon(Char, skill);
				break;
			case SkillKind.RandomAtk02:
				yield return CharUseSkill_RandomAtk02(Char, skill);
				break;
			case SkillKind.EnemyPull:
				yield return CharUseSkill_EnemyPull(Char, skill);
				break;
		}
		m_SkillChar = null;
		m_CorSkill = null;//스킬 액션 종료후 널 처리
	}
	

	List<Item_Stage> SetView_CharArea_Target(TSkillTable _skill, int MaxLine, bool AllCard = false, int Area = 0, int value = 0, bool set = false)
	{
		List<Item_Stage> activecards = new List<Item_Stage>();
		List<Item_Stage> offcards = new List<Item_Stage>();
		int spos = 0, sline = 0, intervalX = 0, intervalY = 0;
		switch (_skill.m_Kind)
		{
		case SkillKind.CardPull:
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2)
			{
				for (int i = Start; i < End; i++)
				{
					if (j < 1) continue;
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					offcards.Add(item);
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					StageCardInfo info = item.m_Info;
					TStageCardTable tdata = info.m_NowTData;
					// 피난민은 저격 불가
					//if (info.IS_AtkTarget())
					if (info.IS_Boss || tdata.IS_LineCard()) continue;
					switch (_skill.m_Value[1]) {
						case 0://다트 제외 모든 카드
							if (tdata.m_Type == StageCardType.Chain) continue;
							break;
						case 1://총알
							if (tdata.m_Type != StageCardType.Material) continue;
							if (tdata.m_Value1 != (int)StageMaterialType.Bullet) continue;
							break;
						case 2://약품
							if (tdata.m_Type != StageCardType.Material) continue;
							if (tdata.m_Value1 != (int)StageMaterialType.Medicine) continue;
							break;
						case 3://화약
							if (tdata.m_Type != StageCardType.Material) continue;
							if (tdata.m_Value1 != (int)StageMaterialType.GunPowder) continue;
							break;
						case 4://식자재, 알콜, 허브
							if (tdata.m_Type != StageCardType.Material) continue;
							if (tdata.m_Value1 != (int)StageMaterialType.Food && tdata.m_Value1 != (int)StageMaterialType.Alcohol 
								&& tdata.m_Value1 != (int)StageMaterialType.Herb) continue;
							break;
						case 5://배터리, 가루
							if (tdata.m_Type != StageCardType.Material) continue;
							if (tdata.m_Value1 != (int)StageMaterialType.Battery && tdata.m_Value1 != (int)StageMaterialType.Powder) continue;
							break;
						}
					offcards.Remove(item);
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0)
			{
				for (int i = 0; i < offcards.Count; i++)
				{
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
		case SkillKind.StopCard:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						offcards.Add(item);
						StageCardInfo info = item.m_Info;
						if (!info.IS_DmgTarget()) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.StopAll:
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2)
			{
				for (int i = Start; i < End; i++)
				{
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					offcards.Add(item);
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					StageCardInfo info = item.m_Info;
					if (!info.IS_AIEnemy() || j == 0) continue;
					offcards.Remove(item);
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0)
			{
				for (int i = 0; i < offcards.Count; i++)
				{
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
		case SkillKind.RemoveFire:
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2)
			{
				for (int i = Start; i < End; i++)
				{
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					offcards.Add(item);
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					StageCardInfo info = item.m_Info;
					// 피난민은 저격 불가
					TStageCardTable tdata = info.m_NowTData;
					if (tdata.m_Type != StageCardType.Fire) continue;
					offcards.Remove(item);
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0)
			{
				for (int i = 0; i < offcards.Count; i++)
				{
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
		case SkillKind.BombSpecialist:
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2)
			{
				for (int i = Start; i < End; i++)
				{
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					offcards.Add(item);
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					StageCardInfo info = item.m_Info;
					// 피난민은 저격 불가
					TStageCardTable tdata = info.m_NowTData;
					if (tdata.m_Type != StageCardType.OldMine && tdata.m_Type != StageCardType.Allymine && tdata.m_Type != StageCardType.TimeBomb) continue;
					offcards.Remove(item);
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0)
			{
				for (int i = 0; i < offcards.Count; i++)
				{
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
		case SkillKind.SpotLight:
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2)
			{
				for (int i = Start; i < End; i++)
				{
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					offcards.Add(item);
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					StageCardInfo info = item.m_Info;
					// 피난민은 저격 불가
					offcards.Remove(item);
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0)
			{
				for (int i = 0; i < offcards.Count; i++)
				{
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
		case SkillKind.ChangeMate://특정 재료를 특정 재료로 변경
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
				for (int i = Start; i < End; i++) {
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					offcards.Add(item);
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					if (item.m_Info.m_TData.m_IsEndType) continue;
					StageCardInfo info = item.m_Info;
					TStageCardTable tdata = info.m_NowTData;
					if (tdata.m_Type != StageCardType.Material || (tdata.m_Type == StageCardType.Material && (int)tdata.m_Value1 != (int)_skill.m_Value[0])) continue;
					offcards.Remove(item);
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0) {
				for (int i = 0; i < offcards.Count; i++) {
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
			case SkillKind.Shuffle:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.ChangeCard:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (info.IS_EnemyCard) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.ChangeCard02:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						if (item.m_Info.m_NowTData.m_Type != (StageCardType)Mathf.RoundToInt(_skill.m_Value[0]) && item.m_Info.m_NowTData.m_Type != (StageCardType)Mathf.RoundToInt(_skill.m_Value[1])) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.ChangeCardArea:
			case SkillKind.ChangeRandomDrop:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.m_TData.m_IsEndType) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (info.IS_EnemyCard) continue;
						TStageCardTable tdata = info.m_NowTData;
						if (tdata.m_Type != (StageCardType)(int)_skill.m_Value[0]) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.GetCards:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 피난민은 저격 불가
						if (info.IS_EnemyCard) continue;
						if (tdata.m_Type != StageCardType.Material) continue;
						if ((int)tdata.m_Value1 != (int)_skill.m_Value[1]) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.SacrificeTornBody:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 찢긴 시체 아니면 제외
						if (tdata.m_Type != StageCardType.TornBody) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.SacrificeGarbage:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 찢긴 시체 아니면 제외
						if (tdata.m_Type != StageCardType.Garbage) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.SacrificePit:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 찢긴 시체 아니면 제외
						if (tdata.m_Type != StageCardType.Pit) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.DeleteBadCard:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 찢긴 시체 아니면 제외
						if (tdata.m_Type != StageCardType.Garbage && tdata.m_Type != StageCardType.Pit && tdata.m_Type != StageCardType.TornBody) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.DownLevel:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (!info.IS_DmgTarget()) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.WideAttack:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (!info.IS_DmgTarget() && !info.IS_ExplosionTarget()) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.HeadShot:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (!item.m_Info.IS_DmgTarget() && !item.m_Info.IS_ExplosionTarget()) continue;

						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break; 
			case SkillKind.RandomAtk:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (!item.m_Info.IS_DmgTarget() && !item.m_Info.IS_ExplosionTarget()) continue;

						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.RandomAtk02:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (item.m_Info.ISNotAtkRefugee) continue;
						if (!item.m_Info.IS_DmgTarget() && !item.m_Info.IS_ExplosionTarget()) continue;

						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.DontAttack:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (!item.m_Info.IS_AIEnemy()) continue;
						if (item.m_Info.ISRefugee) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.SteelItem:
				sline = set ? 1 : 0;
				intervalY = set ? 5 : 3;
				intervalX = set ? 5 : 3;
				for (int i = sline; i < sline + intervalY; i++) {
					spos = i + (set ? -1 : 0);
					for (int j = spos; j < spos + intervalX; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						if (!info.IS_DmgTarget()) continue;
						if (info.ISRefugee) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.DeleteEnemyTribe:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						if (!info.IS_DmgTarget()) continue;
						offcards.Remove(item);
						EEnemyType type = (int)item.m_Info.m_TEnemyData.m_Type > 4 ? (EEnemyType)4: item.m_Info.m_TEnemyData.m_Type;
						if (type != (EEnemyType)_skill.m_Value[1]) continue;//특정 타입 적 아니면 제외

						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.Explosion:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 피난민은 저격 불가
						// 피난민은 저격 불가
						switch (tdata.m_Type) {
							case StageCardType.Grenade:
							case StageCardType.Dynamite:
							case StageCardType.OldMine:
							case StageCardType.Allymine:
							case StageCardType.TimeBomb:
							case StageCardType.C4:
								break;
							default:
								continue;
						}
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.Incitement:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (!info.ISRefugee) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.PlusMate:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						// 찢긴 시체 아니면 제외
						if (tdata.m_Type != StageCardType.Material) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.RecoveryStatus:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.m_Info.m_NowTData.m_Type != (StageCardType)Mathf.RoundToInt(_skill.m_Value[0])) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.Unlock:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (!item.IS_Lock) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.LastAttack:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (!item.m_Info.IS_DmgTarget()) continue;
						if (item.m_Info.GetStat(EEnemyStat.HP) == item.m_Info.GetMaxStat(EEnemyStat.HP)) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.DestoryWall:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						offcards.Add(item);
						if (!item.m_Info.IS_RoadBlock) continue;
						if(item.m_Info.m_NowTData.m_Type == StageCardType.AllRoadblock) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.DestoryWall02:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						offcards.Add(item);
						if (!item.m_Info.IS_DmgTarget() && !item.m_Info.IS_RoadBlock && !item.m_Info.IS_ExplosionTarget()) continue;
						if (item.m_Info.m_NowTData.m_Type == StageCardType.AllRoadblock) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.KeepMaterial:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						TStageCardTable tdata = item.m_Info.m_NowTData;
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (tdata.m_Type != StageCardType.Ash) continue;
						if (tdata.IS_LineCard()) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.MakeRefugee:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (!item.m_Info.IS_EnemyCard) continue;
						if (item.m_Info.m_TEnemyData.m_Tribe != EEnemyTribe.Human) continue;
						if (item.m_Info.ISRefugee) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.CountTribeMaterial:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						StageCardInfo info = item.m_Info;
						TStageCardTable tdata = info.m_NowTData;
						if (item == null) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (!info.IS_EnemyCard) continue;
						if (info.m_TEnemyData.m_Tribe != (EEnemyTribe)Mathf.RoundToInt(_skill.m_Value[0])) continue;
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.AirStrike:
			case SkillKind.BackStep:
			case SkillKind.TransverseAtk:
			case SkillKind.RangeAtk:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (!AllCard && !info.IS_DmgTarget() && !item.m_Info.IS_ExplosionTarget()) {
							if (j == 0) offcards.Add(item);
							continue;
						}
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.EnemyPull:
				for (int j = 1, Start = 0, End = Start + 5; j <= 5; j++, Start++, End++) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						offcards.Add(item);
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						if (!item.m_Info.IS_EnemyCard) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						offcards.Remove(item);
						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			case SkillKind.StopCardPlus:
			case SkillKind.StopCardTran:
				for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2) {
					for (int i = Start; i < End; i++) {
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						if (item.IS_Lock) continue;
						if (item.m_Info.IS_RoadBlock) continue;
						//if (item.m_Info.m_TData.m_Type == StageCardType.Hive) continue;
						StageCardInfo info = item.m_Info;
						// 피난민은 저격 불가
						if (!AllCard && !info.IS_DmgTarget()) {
							if (j == 0) offcards.Add(item);
							continue;
						}

						activecards.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				if (activecards.Count > 0 && offcards.Count > 0) {
					for (int i = 0; i < offcards.Count; i++) {
						offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
					}
				}
				break;
			default:
			for (int j = 0, Start = 0, End = Start + 3; j <= MaxLine; j++, End += 2)
			{
				for (int i = Start; i < End; i++)
				{
					Item_Stage item = m_ViewCard[j][i];
					if (item == null) continue;
					if (item.IS_Lock) continue;
					if (item.m_Info.IS_RoadBlock) continue;
					StageCardInfo info = item.m_Info;
					// 피난민은 저격 불가
					if (!AllCard && !info.IS_DmgTarget())
					{
						if (j == 0) offcards.Add(item);
						continue;
					}
					activecards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOn);
				}
			}
			if (activecards.Count > 0 && offcards.Count > 0)
			{
				for (int i = 0; i < offcards.Count; i++)
				{
					offcards[i].Action(EItem_Stage_Card_Action.TargetOff);
				}
			}
			break;
		}
		return activecards;
	}
	/// <summary> 선택된곳 기준으로 범위 체크, 인자로 받은 조건들도 체크 </summary>
	public delegate bool TargetCheck(Item_Stage _target);
	public void SetAction_CharArea_Target(TSkillTable _skill, Item_Stage _selectcard, TargetCheck _check, ref List<Item_Stage> _area, ref List<Item_Stage> _targets, SkillAreaType _areatype = SkillAreaType.None) {
		_area = new List<Item_Stage>();
		_targets = new List<Item_Stage>();
		SkillAreaType area = _areatype != SkillAreaType.None ? _areatype : _skill.m_AreaType;

		int Area = 0;
		int ShiftArea = 0;
		int StartLine = 0;
		int EndLine = 0;
		int StartPos = 0;

		switch (area) {
			/// <summary> 없음 </summary>
			case SkillAreaType.None:
				break;
			/// <summary> 선택한 카드 1장에만 효과 적용 </summary>
			case SkillAreaType.Area01:
				if(_check(_selectcard))
				_targets.Add(_selectcard);
				break;
			/// <summary> 선택한 타겟 하나와 좌측, 우측 카드에 효과 적용 </summary>
			case SkillAreaType.Area02:
				StartPos = _selectcard.m_Pos - 1;
				for (int i = StartPos; i< StartPos + 3; i++) {
					if (i < 0) continue;
					if (i >= m_ViewCard[_selectcard.m_Line].Length) continue;
					Item_Stage item = m_ViewCard[_selectcard.m_Line][i];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			/// <summary> 선택한 타겟 하나와 위, 아래 카드에 효과 적용 </summary>
			case SkillAreaType.Area03:
				StartLine = _selectcard.m_Line - 1;
				StartPos = _selectcard.m_Pos - 1;
				for (int i = StartLine; i < StartLine + 3; i++, StartPos++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					if (StartPos < 0) continue; 
					if (StartPos >= m_ViewCard[i].Length) continue;
					Item_Stage item = m_ViewCard[i][StartPos];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			/// <summary> 선택한 카드를 중심으로 십자가 모양의 영역에 효과 적용 </summary>
			case SkillAreaType.Area04:
				Area = 3;
				ShiftArea = Area / 2;
				StartLine = _selectcard.m_Line - ShiftArea;
				EndLine = StartLine + Area;
				StartPos = _selectcard.m_Pos - ShiftArea - ShiftArea;
				for (int i = StartLine; i < EndLine; i++, StartPos++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					int EndPos = StartPos + Area;
					for (int j = StartPos; j < EndPos; j++) {
						if (j < 0) continue;
						if (j >= m_ViewCard[i].Length) continue;
						//십자 형태
						if (i != _selectcard.m_Line && j != _selectcard.m_Pos - (_selectcard.m_Line - i)) continue;
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드를 중심으로 3x3 영역에 효과 적용 </summary>
			case SkillAreaType.Area05:
				Area = 3;
				ShiftArea = Area / 2;
				StartLine = _selectcard.m_Line - ShiftArea;
				EndLine = StartLine + Area;
				StartPos = _selectcard.m_Pos - ShiftArea - ShiftArea;
				for (int i = StartLine; i < EndLine; i++, StartPos++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					int EndPos = StartPos + Area;
					for (int j = StartPos; j < EndPos; j++) {
						if (j < 0) continue;
						if (j >= m_ViewCard[i].Length) continue;
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if(!_area.Contains(item))_area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드를 중심으로 5x5 영역에 효과 적용 </summary>
			case SkillAreaType.Area06:
				Area = 5;
				ShiftArea = Area / 2;
				StartLine = _selectcard.m_Line - ShiftArea;
				EndLine = StartLine + Area;
				StartPos = _selectcard.m_Pos - ShiftArea - ShiftArea;
				for (int i = StartLine; i < EndLine; i++, StartPos++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					int EndPos = StartPos + Area;
					for (int j = StartPos; j < EndPos; j++) {
						if (j < 0) continue;
						if (j >= m_ViewCard[i].Length) continue;
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 아직 없음 </summary>
			case SkillAreaType.Area07:
				break;
			/// <summary> 아직 없음 </summary>
			case SkillAreaType.Area08:
				break;
			/// <summary> 선택한 카드가 배치된 행에 효과 적용 </summary>
			case SkillAreaType.Area09:
				for (int i = 0; i < m_ViewCard[_selectcard.m_Line].Length; i++) {
					Item_Stage item = m_ViewCard[_selectcard.m_Line][i];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 행과 위의 행에 효과 적용 </summary>
			case SkillAreaType.Area10:
				StartLine = _selectcard.m_Line;
				for (int i = StartLine; i < StartLine + 2; i++) {
					if (i > AI_MAXLINE) continue;
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 행과 아래의 행에 효과 적용 </summary>
			case SkillAreaType.Area11:
				StartLine = _selectcard.m_Line - 1;
				for (int i = StartLine; i < StartLine + 2; i++) {
					if (i < 0) continue;
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 행과 위, 아래 행에 효과 적용 </summary>
			case SkillAreaType.Area12:
				StartLine = _selectcard.m_Line - 1;
				for (int i = StartLine; i < StartLine + 3; i++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 열에 효과 적용 </summary>
			case SkillAreaType.Area13:
				StartPos = _selectcard.m_Pos - _selectcard.m_Line;
				for (int i = 0; i <= AI_MAXLINE; i++, StartPos++) {
					if (StartPos < 0) continue;
					if (StartPos >= m_ViewCard[i].Length) continue;
					Item_Stage item = m_ViewCard[i][StartPos];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 열과 좌측 열에 효과 적용 </summary>
			case SkillAreaType.Area14:
				StartPos = _selectcard.m_Pos - _selectcard.m_Line;
				for (int i = 0; i <= AI_MAXLINE; i++, StartPos++) {
					for (int j = StartPos - 1; j < StartPos + 1; j++) {
						if (j < 0) continue;
						if (j >= m_ViewCard[i].Length) continue;
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 열과 우측 열에 효과 적용 </summary>
			case SkillAreaType.Area15:
				StartPos = _selectcard.m_Pos - _selectcard.m_Line;
				for (int i = 0; i <= AI_MAXLINE; i++, StartPos++) {
					for (int j = StartPos; j < StartPos + 2; j++) {
						if (j < 0) continue;
						if (j >= m_ViewCard[i].Length) continue;
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 카드가 배치된 열과 좌측, 우측 열에 효과 적용 </summary>
			case SkillAreaType.Area16:
				StartPos = _selectcard.m_Pos - _selectcard.m_Line;
				for (int i = 0; i <= AI_MAXLINE; i++, StartPos++) {
					for (int j = StartPos - 1; j < StartPos + 2; j++) {
						if (j < 0) continue;
						if (j >= m_ViewCard[i].Length) continue;
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 0번째 행부터 1번째 행까지 효과 적용 </summary>
			case SkillAreaType.Area17:
				for (int i = 0; i < 2; i++) {
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 0번째 행부터 2번째 행까지 효과 적용 </summary>
			case SkillAreaType.Area18:
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 모든 곳에 효과 적용 </summary>
			case SkillAreaType.Area19:
				for (int i = 0; i <= AI_MAXLINE; i++) {
					for (int j = 0; j < m_ViewCard[i].Length; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 플레이어 캐릭터 기준 1x3 영역에 효과 적용 </summary>
			case SkillAreaType.Area20:
				for (int i = 0;i < 3;i++) {
					Item_Stage item = m_ViewCard[0][i];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			/// <summary> 플레이어 캐릭터 기준 2x3 영역에 효과 적용 </summary>
			case SkillAreaType.Area21:
				StartPos = 0;
				for (int i = 0; i < 2; i++, StartPos++) {
					for (int j = StartPos; j < StartPos + 3; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 선택한 타겟 하나와 하나 위 카드에 효과 적용 </summary>
			case SkillAreaType.Area22:
				StartLine = _selectcard.m_Line;
				StartPos = _selectcard.m_Pos;
				for (int i = StartLine; i < StartLine + 2; i++, StartPos++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					if (StartPos < 0) continue;
					if (StartPos >= m_ViewCard[i].Length) continue;
					Item_Stage item = m_ViewCard[i][StartPos];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;/// <summary> 선택한 타겟 하나와 둘 위 카드까지 효과 적용 </summary>
			case SkillAreaType.Area23:
				StartLine = _selectcard.m_Line;
				StartPos = _selectcard.m_Pos;
				for (int i = StartLine; i < StartLine + 3; i++, StartPos++) {
					if (i < 0) continue;
					if (i > AI_MAXLINE) continue;
					if (StartPos < 0) continue;
					if (StartPos >= m_ViewCard[i].Length) continue;
					Item_Stage item = m_ViewCard[i][StartPos];
					if (_check(item)) {
						if (!_area.Contains(item)) _area.Add(item);
						if (!_targets.Contains(item)) _targets.Add(item);
						item.Action(EItem_Stage_Card_Action.TargetOn);
					}
				}
				break;
			/// <summary> 플레이어 캐릭터 기준 3x3 영역에 효과 적용 </summary>
			case SkillAreaType.Area24:
				StartLine = 1;
				for (int i = StartLine; i < StartLine + 3; i++) {
					for (int j = i; j < i + 3; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
			/// <summary> 플레이어 캐릭터 기준 5x5 영역에 효과 적용 </summary>
			case SkillAreaType.Area25:
				StartLine = 1;
				for (int i = StartLine; i < StartLine + 5; i++) {
					StartPos = i - 1;
					for (int j = StartPos; j < StartPos + 5; j++) {
						Item_Stage item = m_ViewCard[i][j];
						if (_check(item)) {
							if (!_area.Contains(item)) _area.Add(item);
							if (!_targets.Contains(item)) _targets.Add(item);
							item.Action(EItem_Stage_Card_Action.TargetOn);
						}
					}
				}
				break;
		}
	}
	IEnumerator SelectAction_CharSkill_CheckSelect(Action<Item_Stage> endcb, Item_Stage _ignore = null) {
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
					if (_ignore != null && hitcard == _ignore) continue;
					if (TUTO.TouchCheckLock(TutoTouchCheckType.StageCard, hitcard)) continue;
					if (hitcard.IS_Lock) continue;
					if (!hitcard.ISActiveCard()) continue;

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

		if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Focus_Line_0_2_3)) TUTO.Next();
	}

	/// <summary> 1단계 : 카드 페널이동 </summary>
	IEnumerator CharUseSkill_Start(Item_Stage_Char Char, bool _zoom, TSkillTable _table, bool _select = true, List<Item_Stage_Char> _selectchars = null, Action<int> _cb = null, Action _zoomcb = null) {
		if (m_SkillUseInfoPopup != null) {
			// 팝업 닫아주기
			m_SkillUseInfoPopup.Close();
			m_SkillUseInfoPopup = null;
		}
		Char.SetSkillInfoActive(false);
		Char.GetComponent<SortingGroup>().sortingOrder = 6;
		Char.SetSelectFX(_table.m_Kind != SkillKind.CoolReset && !_select);
		float movetime = 0.3f;
		// 선택된 카드와 중앙카드의 위치 변경
		//if (m_CenterChar != Char ) {//&& _selectchars == null
			iTween.MoveTo(Char.gameObject, iTween.Hash("position", m_CharsPos[2], "time", movetime, "easetype", "easeOutCubic", "IsLocal", true));
			iTween.ScaleTo(Char.gameObject, iTween.Hash("scale", m_CharsPosScale, "time", movetime, "easetype", "easeOutCubic", "IsLocal", true));
		//}

		for (int i = 0; i < m_Chars.Length; i++) {
			if (_selectchars != null) {
				if (_selectchars.Contains(m_Chars[i])) continue;
			}
			else {
				if (Char.m_Pos == i) continue;
			}
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, null, movetime);
			//if(Char.m_Info.m_Idx != m_Chars[i].m_Info.m_Idx) Char.SetSelectFX(false);
		}

		m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
		LockCamScroll = !_select;

		SkillAreaType areatype = _table.m_AreaType;
		if (_table.m_Kind == SkillKind.TransverseAtk)
			areatype = Char.m_TransverseAtkMode == 0 ? (Char.m_Info.IS_SetEquip() ? SkillAreaType.Area12 : SkillAreaType.Area09) : (Char.m_Info.IS_SetEquip() ? SkillAreaType.Area16 : SkillAreaType.Area13);
		else if (_table.m_Kind == SkillKind.ChangeCard02 && _table.m_Value[2] == 33) areatype = SkillAreaType.Area04;
		m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse, (res, obj) => {
			LockCamScroll = false;
		}, _table.GetName(), _table.GetInfo(Char.m_Info.GetSkillLV(SkillType.Active)), _table.GetImg(), BaseValue.GetAreaIcon(areatype), 
		Char.m_Info.IS_SetEquip(), Char.m_Info.GetNeedAP(), STAGEINFO.m_TStage.GetMode(PlayType.NoCool) != null ? 0 : Char.m_Info.m_Skill[0].m_TData.m_Cool, _select, !Char.IS_LearningSkill(_table), _cb).GetComponent<Stage_CardUse>();

		if (_zoom) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 1, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[2], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[2], "time", movetime, "easetype", "easeOutQuad"));
			SetBGFXSort(false);
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}
		else yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Char.gameObject));

		_zoomcb?.Invoke();
	}

	IEnumerator CharUseSkill_End(Item_Stage_Char Char, bool _zoom, bool _selectchar = false) {
		LockCamScroll = false;
		if (STAGE_USERINFO.m_RemainActiveSkillCnt != -1 && STAGE_USERINFO.m_RemainActiveSkillCnt > 0) {
			m_MainUI.RefreshDebuffCardCount(StageCardType.ConActiveSkillLimit);
			STAGE_USERINFO.m_RemainActiveSkillCnt--;
		}

		SkillKind skill = Char.m_Info.m_Skill[0].m_TData.m_Kind;

		int AddCoolTime = 0;

		// 행동력 감소
		int preval = m_User.m_AP[0];
		m_User.m_AP[0] = Math.Max(m_User.m_AP[0] - Char.m_Info.GetNeedAP(), 0);
		for(int i = 0;i< USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
			m_Chars[i].SetSelectFX(false);
		}

		//행동력 회복 스킬이나 시너지도 추가 될거임
		DLGTINFO?.f_RfAPUI?.Invoke(m_User.m_AP[0], preval, m_User.m_AP[1]);

		//디버프 skillhp 있으면 사용시 체력 감소
		if (STAGE_USERINFO.ISBuff(StageCardType.SkillHp)) {
			StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * -STAGE_USERINFO.GetBuffValue(StageCardType.SkillHp))));
		}
		//디버프 skillstatus 있으면 사용시 모든스테이터스 감소
		if (STAGE_USERINFO.ISBuff(StageCardType.SkillStatus)) {
			for (int i = 0; i < (int)StatType.SurvEnd; i++) {
				StartCoroutine(AddStat_Action(m_CenterChar.transform, (StatType)i,  -Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat((StatType)i) * STAGE_USERINFO.GetBuffValue(StageCardType.SkillStatus))));
			}
		}

		// 쿨타임 적용
		Char.SetSkillCoolTime(AddCoolTime);
		//쿨타임 감소
		DNACheck(Char, OptionType.AttackingCoolDown);
		//for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
		//	m_Chars[i].SetSkillCoolTimeUI();
		//}
		if (skill == SkillKind.TransverseAtk) Char.Change_TransverseAtk();

		if (m_SkillUseInfoPopup != null) {
			// 팝업 닫아주기
			m_SkillUseInfoPopup.Close();
			m_SkillUseInfoPopup = null;
		}
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
			item.Action(EItem_Stage_Card_Action.Scale, 0f, (obj) => { obj.TW_ScaleBumping(true); });
		}

		float movetime = 0.3f;
		// 선택된 카드와 중앙카드의 위치 변경
		//if (m_CenterChar != Char) {
			iTween.MoveTo(Char.gameObject, iTween.Hash("position", m_CharsPos[Char.m_Pos], "time", movetime, "easetype", "easeOutCubic", "IsLocal", true));
			iTween.ScaleTo(Char.gameObject, iTween.Hash("scale", m_CharsPosScale, "time", movetime, "easetype", "easeOutCubic", "IsLocal", true));
		//}

		for (int i = 0; i < m_Chars.Length; i++) {
			if (Char.m_Pos == i && !_selectchar) continue;
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, null, movetime);
		}

		m_MainUI.StartPlayAni(Main_Stage.AniName.In);
		if (_zoom) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 0, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			SetBGFXSort(true);
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}
		else yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Char.gameObject));
		Char.GetComponent<SortingGroup>().sortingOrder = 6;

		m_Check.Check(StageCheckType.UseSkill, (int)skill, 1);

		// 여기까지오면 팝업이 없어야되는데 SkillUsePopup이 연출때문에 나중에 닫히는 현상이 있어 여기서 대기해줌
		// 제거시 튜토리얼이 진행 안되는 현상 발생
		yield return new WaitWhile(() => POPUP.IS_PopupUI());
		Char.SetSkillInfoActive(true);
	}

	IEnumerator CharUseSkill_HeadShot(Item_Stage_Char Char, TSkillTable skill) {
		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int cnt = 0; cnt < Mathf.RoundToInt(skill.m_Value[0]); cnt++) {
			m_SkillZoom = true;
			// 저격 가능한 몬스터 보여주기
			List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
			if (activecards.Count < 1) {
				SkillSpeech(Char, false);
				if (cnt > 0) {
					AutoCamPosInit = true;
					yield return CharUseSkill_End(Char, m_SkillZoom);
				}
				else
					yield break;
			}

			if (cnt == 0) {
				yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res)=> {
					if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
				});
			}
			if (activecards.Count > 0) ShowArea(true, activecards);
			SkillSpeech(Char, true);

			//타겟 및 범위 지정
			Item_Stage targetcard = null;
			AutoCamPosInit = false;
			yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
			ShowArea(false);

			List<Item_Stage> area = new List<Item_Stage>();
			List<Item_Stage> targets = new List<Item_Stage>();
			SetAction_CharArea_Target(skill, targetcard, (card) => {
				if (card == null) return false;
				if (card.IS_Die()) return false;
				if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
				return true;
			}, ref area, ref targets);


			// 대상이 아닌놈들 꺼주기
			for (int i = activecards.Count - 1; i > -1; i--) {
				Item_Stage oncards = activecards[i];
				if (targets.Contains(oncards)) continue;
				offcards.Add(oncards);
				oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
			}

			yield return new WaitWhile(() => offcards.Count > 0);


			SkillInfo skillinfo = Char.m_Info.m_Skill[0];
			CharInfo charinfo = Char.m_Info;

			ShowArea(true, targets);

			for (int r = 0; r < targets.Count; r++) {
				Item_Stage target = targets[r];
				if (target == null) continue;
				if (m_ViewCard[target.m_Line][target.m_Pos] == null) continue;

				if (target.m_Info.m_NowTData.m_Type == StageCardType.OldMine || target.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
					yield return SelectAction_StageCardProc_OldMine(target, new List<List<Item_Stage>>() { targets });
				else if (target.m_Info.IS_OilGas()) {
					yield return SelectAction_StageCardProc_OilGasStation(target, new List<List<Item_Stage>>() { targets });
				}
				else {
					if (target.m_Info.IS_EnemyCard) {
						PlayEffSound(SND_IDX.SFX_0400);
						// 이펙트
						GameObject eff = StartEff(target.transform, "Effect/Stage/Eff_StageCard_Sniping");
						eff.transform.localScale *= 0.6f;
						float ratio = 1f;
						if (r == 1) ratio = skill.m_Value[1];
						else if (r == 2) ratio = skill.m_Value[2];
						int dmg = GetAtkDmg(target, Char, true, true, false, null, ratio);

						SetAtkDNASynergy(Char, new object[] { dmg });

						target.SetDamage(false, dmg);
						if (target.IS_Die()) {
							GetKillEnemy(target, Char, true, true);
						}

						//yield return CamShake(0.05f, 0.4f, Vector3.one * 0.01f);
						offcards.Add(target);
						target.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
							offcards.Remove(obj);
						});
						if (r < targets.Count) yield return new WaitForSeconds(0.2f); 
					}
					else {
						// 이펙트
						GameObject eff = StartEff(target.transform, "Effect/Stage/Eff_StageCard_Sniping");
						eff.transform.localScale *= 0.6f;
						//yield return CamShake(0.05f, 0.4f, Vector3.one * 0.01f);
					}
				}
			}
		}
		yield return IE_CamAction(CamActionType.Shake_1, 0f, 0.5f);
		//yield return CamShake(0f, 0.5f, Vector3.one * 0.025f);
		yield return new WaitWhile(() => offcards.Count > 0);
		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_Heal(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}


		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;
		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		//DNA 버프
		float AddHP = DNACheck(Char, OptionType.CureHealAdd, new object[] { skill.GetValue(skillinfo.m_LV) });
		yield return AddStat_Action(Char.transform, StatType.HP, Mathf.RoundToInt(AddHP));
		//DNA 버프
		DNACheck(Char, OptionType.CureApAdd);
	}
	IEnumerator CharUseSkill_HealPlus(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;
		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		//DNA 버프
		float AddHP = DNACheck(Char, OptionType.CureHealAdd, new object[] { skill.GetValue(skillinfo.m_LV) });
		yield return AddStat_Action(Char.transform, StatType.HP, Mathf.RoundToInt(AddHP));

		StatType srvstat = (StatType)Mathf.RoundToInt(skill.m_Value[0]);
		float AddStat = STAGE_USERINFO.GetMaxStat(srvstat) * skill.m_Value[1];
		yield return AddStat_Action(Char.transform, srvstat, Mathf.RoundToInt(AddStat));

		//DNA 버프
		DNACheck(Char, OptionType.CureApAdd);
	}
	IEnumerator CharUseSkill_ShotGun(Item_Stage_Char Char, TSkillTable skill) {

		List<Item_Stage> TargetCards = new List<Item_Stage>();
		List<Item_Stage> AreaCards = new List<Item_Stage>();
		List<Item_Stage> oncards = new List<Item_Stage>();
		// 타겟 데미지 주기
		// 전방 3 위치
		//	1,1		1,2		1,3
		//	0,0		0,1		0,2
		SetAction_CharArea_Target(skill, null, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref AreaCards, ref TargetCards);

		if (TargetCards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		m_SkillZoom = true;
		AutoCamPosInit = false;

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		}, ()=> { ShowArea(true, AreaCards); });
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}
		AutoCamPosInit = true;
		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);

		SkillSpeech(Char, true);

		yield return new WaitWhile(() => oncards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0500);
		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_ShotGun");

		yield return IE_CamAction(CamActionType.Shake_0, 0f, 0.25f, Vector3.one * 0.025f);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;
		// 타겟 데미지 주기
		for (int i = TargetCards.Count - 1; i > -1; i--) {
			Item_Stage item = TargetCards[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { TargetCards });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { TargetCards });
			}
			else if (item.m_Info.IS_DmgTarget()) {
				if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;
				StageCardInfo info = item.m_Info;
				int dmg = GetAtkDmg(item, Char, true, true, false);
				//공격시 dna, synergy 체크
				SetAtkDNASynergy(Char, new object[] { dmg });

				item.SetDamage(false, dmg);

				if (item.IS_Die()) {
					GetKillEnemy(item, Char, true, true);
				}
			}
		}

		yield return new WaitForSeconds(0.75f);//1.5f

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_PainHeal(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		//DNA 버프
		float AddHP = DNACheck(Char, OptionType.CureHealAdd, new object[] { skill.GetValue(skillinfo.m_LV) });
		int men = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) * skill.m_Value[0]);
		yield return AddStat_Action(Char.transform, StatType.Men, -men, StatType.HP, Mathf.RoundToInt(AddHP));

		//DNA 버프
		DNACheck(Char, OptionType.CureApAdd);
	}

	IEnumerator CharUseSkill_Jump(Item_Stage_Char Char, TSkillTable skill) {
		if(m_CardLastLine == 0) {
			SkillSpeech(Char, false);
			yield break;
		}
		m_SkillZoom = false;
		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}
		m_IS_Jumping = true;
		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);

		SkillSpeech(Char, true);

		// 카드 라인 변경
		List<Item_Stage> removecards = new List<Item_Stage>();

		// 카드 pool 이동
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

		for (int j = 1, Start = 1, End = Start + 3; j < m_ViewCard.Length; j++, End += 2) {
			for (int i = 0, Offset = 0; i < m_ViewCard[j].Length; i++) {
				Item_Stage TempCard = m_ViewCard[j][i];
				if (TempCard == null) continue;
				if (i < Start || i >= End) {
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
				}
				m_ViewCard[j][i] = null;
			}
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

		yield return new WaitForSeconds(0.75f);//1.5f
		yield return new WaitWhile(() => actioncards.Count > 0);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		m_Check.Check(StageCheckType.Survival, 0);

		m_IS_Jumping = false;
	}

	IEnumerator CharUseSkill_CardPull(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, m_SkillZoom);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		// 앞라인 꺼주기
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			Item_Stage card = m_ViewCard[0][i];
			activecards.Remove(card);
			card.Action(EItem_Stage_Card_Action.TargetOff);
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, true, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);


		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		yield return new WaitWhile(() => Mathf.Abs(CamMoveGapX) > 0.01f);
		// 중앙 카드 제거
		List<Item_Stage> actioncards = new List<Item_Stage>();

		int pos = 1;
		Item_Stage MoveCard = m_ViewCard[0][pos];
		actioncards.Add(MoveCard);

		MoveCard.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
			// 카드 pool 이동
			actioncards.Remove(obj);
		});

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (oncards == targetcard) continue;
			if (oncards == MoveCard) continue;
			actioncards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { actioncards.Remove(obj); });
		}

		yield return new WaitWhile(() => actioncards.Count > 0);

		// 선택카드 자리 변경
		m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = MoveCard;
		m_ViewCard[0][1] = targetcard;
		actioncards.Add(targetcard);
		targetcard.Action(EItem_Stage_Card_Action.MoveTarget, 0, (obj) => {
			actioncards.Remove(obj);
		}, MoveCard);

		// 앞라인 켜주기
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			if (i == 1) continue;
			Item_Stage card = m_ViewCard[0][i];
			activecards.Remove(card);
			card.Action(EItem_Stage_Card_Action.TargetOff);
		}

		yield return new WaitWhile(() => actioncards.Count > 0);

		actioncards.Add(targetcard);
		targetcard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { actioncards.Remove(obj); });
		yield return new WaitWhile(() => actioncards.Count > 0);

		m_ViewCard[MoveCard.m_Line][MoveCard.m_Pos] = null;
		RemoveStage(MoveCard);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_AirStrike(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		
		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		SkillSpeech(Char, true);


		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		if (!Char.m_Info.IS_SetEquip()) {
			AutoCamPosInit = false;
			yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
				if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
			});
			ShowArea(true, activecards);
			yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		}
		else {
			int cancleval = -1;
			yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
				cancleval = res;
			}, () => { ShowArea(true, activecards); });
			yield return new WaitWhile(() => cancleval == -1);
			if (cancleval == 1) {
				yield return SkillCancle(Char, m_SkillZoom);
				yield break;
			}
		}

		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>() { targetcard };
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets);

		yield return Action_TargetOff(activecards);

		yield return IE_CamAction(CamActionType.Zoom_Out);

		PlayEffSound(SND_IDX.SFX_0507);
		// 이펙트

		Vector3 pos;
		if (!Char.m_Info.IS_SetEquip()) pos = new Vector3(0, targetcard.transform.position.y, 0f);
		else pos = new Vector3(0, BaseValue.STAGE_INTERVER.y * 2 * m_Stage.Panel[1].lossyScale.y, 0f) + m_Stage.Panel[1].position;
		StartEff(pos, "Effect/Stage/Eff_ChSkill_AirStrike");

		yield return IE_CamAction(CamActionType.Shake_2, 0.85f, 0.8f);
		CamAction(CamActionType.Shake_1);

		List<Item_Stage> chains = new List<Item_Stage>();

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;
		// 타겟 데미지 주기
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			if (item == null) continue;
			if (item.IS_Die()) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { targets });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { targets });
			}
			else {
				StageCardInfo info = item.m_Info;
				if (info.m_TData.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = 0;
					chains.Add(item);
				}
				else if(info.IS_DmgTarget()){
					int dmg = GetAtkDmg(item, Char, true, true, false);
					//공격시 dna, synergy 체크
					SetAtkDNASynergy(Char, new object[] { dmg });

					item.SetDamage(false, dmg);
					if (item.IS_Die()) {
						GetKillEnemy(item, Char, true, true);
					}
				}
			}
		}

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);
		yield return IE_CamAction(CamActionType.Zoom_OutToIdle);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_BackStep(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, 0);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (oncards == targetcard) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, targets);

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0420);
		// 이펙트
		StartEff(targetcard.transform, "Effect/Stage/Eff_ChSkill_BackStep_L");

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		if (targetcard.m_Info.IS_EnemyCard) {
			int dmg = GetAtkDmg(targetcard, Char, true, true, false);
			//공격시 dna, synergy 체크
			SetAtkDNASynergy(Char, new object[] { dmg });

			targetcard.SetDamage(false, dmg);

			if (targetcard.IS_Die()) {
				GetKillEnemy(targetcard, Char, true, true);
			}
			yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);

			offcards.Add(targetcard);
			targetcard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
			yield return new WaitWhile(() => offcards.Count > 0);
		}
		else {
			yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);
			if (targetcard.m_Info.m_NowTData.m_Type == StageCardType.OldMine || targetcard.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(targetcard);
			else if (targetcard.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(targetcard);
			}


			else if (targetcard.m_Info.m_TData.m_Type == StageCardType.Chain) {
				targetcard.m_ChainDieEffPosX = targetcard.transform.position.x;
				yield return SelectAction_ChainDie(targets);
			}
		}

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_LearningAbility(Item_Stage_Char Char, TSkillTable skill) {
		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}
		skill = TDATA.GetRandomSkill(skill.m_Type == SkillType.SetActive);
		Utile_Class.DebugLog(skill.m_Idx + " : " + skill.m_Kind.ToString());
		Char.LearningAbility(skill);

		yield return UseUseSkill(Char, skill);

		Char.LearningAbility(null);

		//연속 팝업으로 타임스케일 1되서 다시세팅함
		m_MainUI.AccToggleCheck();

		if (m_SkillUseInfoPopup != null) {
			yield return SkillCancle(Char, m_SkillZoom);
		}
	}

	IEnumerator CharUseSkill_ChangeCard(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;

		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		List<Item_Stage> offcards = new List<Item_Stage>();
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (card.m_Info.IS_RoadBlock) return false;
			if (card.m_Info.IS_EnemyCard) return false;
			if (card.m_Info.m_TData.m_IsEndType) return false;
			return true;
		}, ref area, ref targets);

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		if (targets.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		ShowArea(true, area);

		// 변환 해준다.
		int CardID = Mathf.RoundToInt(skill.m_Value[1]);

		offcards.Clear();
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = targets[i];
			offcards.Add(card);

			if (card.m_Info.IsDark && CardID < 1) {
				StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
				card.SetCardChange(info.m_RealIdx);
			}
			else
				card.SetCardChange(CardID < 1 ? CharSkill_CreateStageCardIdx().m_Idx : CardID);
			card.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--)
		{
			Item_Stage card = targets[i];
			offcards.Add(card);
			card.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_ChangeCard2(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;

		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		List<Item_Stage> offcards = new List<Item_Stage>();
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (card.m_Info.m_TData.m_IsEndType) return false;
			if (card.m_Info.m_NowTData.m_Type != (StageCardType)Mathf.RoundToInt(skill.m_Value[0]) && card.m_Info.m_NowTData.m_Type != (StageCardType)Mathf.RoundToInt(skill.m_Value[1])) return false;
			return true;
		}, ref area, ref targets);

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		if (targets.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		ShowArea(true, area);

		// 변환 해준다.
		int CardID = 0;
		List<TStageCardTable> mine = STAGEINFO.GetStageCardGroup().FindAll(t => t.m_Type == (StageCardType)Mathf.RoundToInt(skill.m_Value[2]));
		if (mine.Count > 0) CardID = mine[UTILE.Get_Random(0, mine.Count)].m_Idx;
		if (CardID == 0) CardID = STAGEINFO.GetDefaultCardIdx((StageCardType)Mathf.RoundToInt(skill.m_Value[2]));

		offcards.Clear();
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = targets[i];
			offcards.Add(card);

			if (card.m_Info.IsDark && CardID < 1) {
				StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
				card.SetCardChange(info.m_RealIdx);
			}
			else
				card.SetCardChange(CardID < 1 ? CharSkill_CreateStageCardIdx().m_Idx : CardID);
			card.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = targets[i];
			offcards.Add(card);
			card.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_Explosion(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		activecards.Clear();

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> offcards = new List<Item_Stage>();
		List<Item_Stage> area = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
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
					return false;
			}
			return true;
		}, ref area, ref activecards);

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
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
		float dna = DNACheck(Char, OptionType.AttackingDmgAdd);
		atk += Mathf.RoundToInt(atk * dna);

		for (int m = activecards.Count - 1; m > -1; m--) {
			Item_Stage card = activecards[m];
			TStageCardTable tdata = card.m_Info.m_NowTData;
			StageCardType cardtype = tdata.m_Type;
			area.Clear();
			List<Item_Stage> targets;
			BoomAreaTarget(card, card, out area, out targets);
			m_Area.AddCard(area);
			alltargets.Add(card, targets);
		}

		m_Area.Show();

		List<Item_Stage> EnemyDieCheck = new List<Item_Stage>();
		// 연출의 꼬임 방지
		// 죽는 카드
		for (int m = activecards.Count - 1; m > -1; m--) {
			Item_Stage card = activecards[m];
			TStageCardTable tdata = card.m_Info.m_NowTData;
			StageCardType cardtype = tdata.m_Type;
			List<Item_Stage> targets = alltargets[card];
			CardActionDamageType damagetype = GetActionDamageType(cardtype);
			if (damagetype != CardActionDamageType.Die) continue;
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
						GetKillEnemy(item, Char, false, true);
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
		for (int m = activecards.Count - 1; m > -1; m--) {
			Item_Stage card = activecards[m];
			TStageCardTable tdata = card.m_Info.m_NowTData;
			StageCardType cardtype = tdata.m_Type;
			List<Item_Stage> targets = alltargets[card];
			CardActionDamageType damagetype = GetActionDamageType(cardtype);
			if (damagetype != CardActionDamageType.Damage) continue;
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
							Damage = GetAtkDmg(item, Char, true, true, false);
							break;
						case StageCardType.TimeBomb:
							Damage = Mathf.RoundToInt(info.GetMaxStat(EEnemyStat.HP) * card.m_Info.m_TData.m_Value1) + Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(item.m_Info.m_TEnemyData));
							break;
						case StageCardType.OldMine:
						case StageCardType.Allymine:
							Damage = info.GetMaxStat(EEnemyStat.HP);
							break;
					}

					int dmg = Damage;
					//공격시 dna, synergy 체크
					SetAtkDNASynergy(Char, new object[] { dmg });

					item.SetDamage(false, Damage, 1, 0, false);
					if (!EnemyDieCheck.Contains(item) && item.IS_Die()) {
						EnemyDieCheck.Add(item);
						GetKillEnemy(item, Char, true, true);
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
		yield return IE_CamAction(CamActionType.Shake_0, 0f, 1.83f, Vector3.one * 0.01f);
		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		yield return new WaitForSeconds(0.75f);//1.5f
		yield return new WaitWhile(() => actioncards.Count > 0);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_TransverseAtk(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		int atkmode = Char.m_TransverseAtkMode;

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		if (skill.m_Kind != SkillKind.TransverseAtk) atkmode = UTILE.Get_Random(0, 1);

		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SkillAreaType areatype = SkillAreaType.None;
		areatype = atkmode == 0 ? (Char.m_Info.IS_SetEquip() ? SkillAreaType.Area12  : SkillAreaType.Area09) : (Char.m_Info.IS_SetEquip() ? SkillAreaType.Area16 : SkillAreaType.Area13);

		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets, areatype);

		List<Item_Stage> offcards = new List<Item_Stage>();
		List<Item_Stage> chains = new List<Item_Stage>();
		int atk = Mathf.RoundToInt(STAGE_USERINFO.GetCharStat(charinfo, StatType.Atk));
		float dna = DNACheck(Char, OptionType.AttackingDmgAdd);
		atk += Mathf.RoundToInt(atk * dna);

		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (oncards.m_Line == targetcard.m_Line) continue;
			if (oncards.IS_Lock) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0400);
		GameObject eff;
		if(Char.m_Info.IS_SetEquip())
			eff= StartEff(targetcard.transform, atkmode == 0 ? "Effect/Stage/Eff_ChSkill_Shooting_Hor2" : "Effect/Stage/Eff_ChSkill_Shooting_Ver2");
		else
			eff = StartEff(targetcard.transform, atkmode == 0 ? "Effect/Stage/Eff_ChSkill_Shooting_Hor" : "Effect/Stage/Eff_ChSkill_Shooting_Ver");

		for (int i = 0;i< targets.Count; i++) {
			Item_Stage item = targets[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			StageCardInfo info = item.m_Info;
			TStageCardTable targettdata = info.m_NowTData;
			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { targets });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { targets });
			}
			else {
				if (targettdata.m_Type == StageCardType.Chain) {
					targetcard.m_ChainDieEffPosX = targetcard.transform.position.x;
					chains.Add(item);
				}
				else if (item.m_Info.IS_DmgTarget()){
					int dmg = GetAtkDmg(item, Char, true, true, false);
					//공격시 dna, synergy 체크
					SetAtkDNASynergy(Char, new object[] { dmg });

					item.SetDamage(false, dmg);
					if (item.IS_Die()) {
						GetKillEnemy(item, Char, true, true);
					}
				}
			}
		}

		ShowArea(true, area);
		yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);
		yield return new WaitForSeconds(0.55f);

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_CoolReset(Item_Stage_Char Char, TSkillTable skill) {
		if (STAGEINFO.m_TStage.m_PlayType.Find(o => o.m_Type == PlayType.NoCool) != null) {
			SkillSpeech(Char, false);
			yield break;
		}
		m_SkillZoom = false;

		bool alllock = true;
		List<Item_Stage_Char> activechars = new List<Item_Stage_Char>();
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			if (m_Chars[i] == Char) continue;
			if (!m_Chars[i].m_SkillLock && m_Chars[i].m_SkillCoolTime > 0) {
				activechars.Add(m_Chars[i]);
				alllock = false;
			}
		}
		if (alllock) {
			SkillSpeech(Char, false);
			yield break;
		}

		for (int i = 0; i < activechars.Count; i++) {
			activechars[i].SetSkillFrameActive(true);
			activechars[i].SetSelectFX(true);
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, activechars, (res) => {
			if (res == 1) {
				for(int i = 0;i< activechars.Count; i++) {
					activechars[i].SetSkillFrameActive(activechars[i].IS_UseActiveSkill());
					activechars[i].SetSelectFX(false);
				}
				StartCoroutine(SkillCancle(Char, m_SkillZoom));
			}
		});

		SkillSpeech(Char, true);

		Item_Stage_Char target = null;
		yield return SelectAction_SkillCard_CheckSelectChar((obj) => target = obj);

		///특정 캐릭터 선택 하게(캐릭터는 락걸려있으면 안됨)
		PlayEffSound(SND_IDX.SFX_0460);
		// 이펙트
		StartEff(target.transform, "Effect/Stage/Eff_ChSkill_Heal");
		//쿨타임 리셋
		target.SkillColoTimeInit();

		yield return new WaitForSeconds(0.75f);//1.5f
		yield return CharUseSkill_End(Char, m_SkillZoom, true);
	}

	IEnumerator CharUseSkill_BlowAtk(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (oncards == targetcard) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0430);
		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Sniping");
		eff.transform.localScale *= 0.6f;

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		ShowArea(true, targets);

		StartCoroutine(AddStat_Action(m_CenterChar.transform, StatType.HP, -Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(StatType.HP) * skill.GetValue(skillinfo.m_LV))));

		if (targetcard.m_Info.IS_EnemyCard) {
			// 타겟 데미지 주기
			targetcard.SetDamage(false, targetcard.m_Info.GetMaxStat(EEnemyStat.HP));
			if (targetcard.IS_Die()) {
				GetKillEnemy(targetcard, Char, false, true);
			}

			yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);
			offcards.Add(targetcard);
			targetcard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
			yield return new WaitWhile(() => offcards.Count > 0);
		}
		else if(targetcard.m_Info.m_TData.m_Type == StageCardType.Chain){
			yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);
			targetcard.m_ChainDieEffPosX = targetcard.transform.position.x;
			yield return SelectAction_ChainDie(targets);
		}

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_Incitement(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.ISRefugee) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			m_ViewCard[item.m_Line][item.m_Pos] = null;
			item.Action(EItem_Stage_Card_Action.Get, 0f, (obj) => {
				targets.Remove(obj);
				GetRefugee(obj);
				RemoveStage(obj);
			}, m_CenterChar.transform.position);
			yield return new WaitForEndOfFrame();
			yield return new WaitForSeconds(0.2f);//순차로 들어오게 수정
		}

		yield return new WaitWhile(() => targets.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_StopCard(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		float times = skill.GetValue(skillinfo.m_LV);
		int atk = Mathf.RoundToInt(m_User.GetStat(StatType.Atk));
		atk += Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(targetcard.m_Info.m_TEnemyData));
		float dna = DNACheck(Char, OptionType.AttackingDmgAdd);
		atk += Mathf.RoundToInt(atk * dna);

		PlayEffSound(SND_IDX.SFX_0471);
		for (int i = 0; i < targets.Count; i++) {
			//데미지
			int dmg = GetAtkDmg(targetcard, Char, true, true, false);
			//공격시 dna, synergy 체크
			SetAtkDNASynergy(Char, new object[] { dmg });

			targets[i].SetDamage(false, dmg);
			if (targets[i].IS_Die()) {
				GetKillEnemy(targets[i], Char, true, true);
			}
			else {
				StartEff(targets[i].transform, "Effect/Stage/Eff_Debuff_Card");
				// 이펙트 등록
				GameObject Eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Paralysis", true);
				Eff.transform.localScale = Vector3.one;

				//정지 설정 생성
				AIStopInfo aistop = new AIStopInfo(AiStopMode.AREA1);
				aistop.SetTarget(targets[i]);
				aistop.SetTurn(Mathf.RoundToInt(skill.m_Value[0]));
				aistop.SetEff(Eff);
				AddAIStopInfo(aistop);
			}
		}

		yield return new WaitForSeconds(0.75f);//1f
		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_StopCardPlus(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (card.m_Info.IS_RoadBlock) return false;
			if (!card.m_Info.IS_DmgTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		PlayEffSound(SND_IDX.SFX_0480); 
		StartEff(targetcard.transform, "Effect/Stage/Eff_ChSKill_FlashBomb");

		for (int i = 0; i < targets.Count; i++) {
			//데미지
			int dmg = GetAtkDmg(targetcard, Char, true, true, false);
			//공격시 dna, synergy 체크
			SetAtkDNASynergy(Char, new object[] { dmg });

			targets[i].SetDamage(false, dmg);
			if (targets[i].IS_Die()) {
				GetKillEnemy(targets[i], Char, true, true);
			}
			else {
				StartEff(targets[i].transform, "Effect/Stage/Eff_Debuff_Card");
				// 이펙트 등록
				GameObject Eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Paralysis", true);
				Eff.transform.localScale = Vector3.one;

				//정지 설정 생성
				AIStopInfo aistop = new AIStopInfo(AiStopMode.AREA1);
				aistop.SetTarget(targets[i]);
				aistop.SetTurn(Mathf.RoundToInt(skill.m_Value[0]));
				aistop.SetEff(Eff);
				AddAIStopInfo(aistop);

				//원거리 공격 차단 생성
				AiBlockRangeAtkInfo aiblockrangeastk = new AiBlockRangeAtkInfo(AiBlockRangeAtkMode.AREA1);
				aiblockrangeastk.SetTarget(targets[i]);
				aiblockrangeastk.SetTurn(Mathf.RoundToInt(skill.m_Value[0]));
				aiblockrangeastk.SetEff(Eff);
				AddAiBlockRangeAtkInfo(aiblockrangeastk);
			}
		}

		yield return new WaitForSeconds(0.75f);//1f
		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_SpotLight(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> offcards = new List<Item_Stage>();
		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			return true;
		}, ref area, ref targets);

		for (int i = activecards.Count - 1; i > -1; i--)
		{
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			if (targets.Contains(oncard)) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, targets);
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		// 라이트 설정 생성
		LightInfo lightinfo = new LightInfo(LightMode.LightStick, 1);
		lightinfo.SetTarget(targetcard.m_Line, targetcard.m_Pos);
		int turn = Mathf.RoundToInt(skill.m_Value[1]);
		lightinfo.SetTurn(turn);

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

		yield return Check_LightOnOff();


		yield return Action_TargetOff(area);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_StopCardTran(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_DmgTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();

		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		

		for (int i = 0; i < targets.Count; i++)
		{
			Item_Stage item = targets[i];
			//이펙트 중복 방지
			if (m_AIStopInfos.Find((t) => t.m_Type == AiStopMode.Line && t.m_Line == item.m_Line) != null) continue;
			// 라이트 설정 생성
			AIStopInfo aistop = new AIStopInfo(AiStopMode.Line);

			aistop.SetTarget(item);
			aistop.SetTurn(Mathf.RoundToInt(skill.m_Value[1]));

			PlayEffSound(SND_IDX.SFX_0471);
			StartEff(item.transform, "Effect/Stage/Eff_Debuff_Card");
			// 이펙트 등록
			GameObject obj =  StartEff(item.transform, "Effect/Stage/Eff_ChSkill_StopCardTran", true);
			aistop.SetEff(obj);

			AddAIStopInfo(aistop);
		}

		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_RemoveFire(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Fire) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		// 이펙트
		GameObject eff = StartEff(targetcard.transform, "Effect/Stage/Eff_StageCard_Fireext_Throw_S");
		switch (skill.m_AreaType) {
			case SkillAreaType.Area04:
				eff.transform.localScale *= 3;
				PlayEffSound(SND_IDX.SFX_0608);
				break;
			case SkillAreaType.Area05:
				eff.transform.localScale *= 5;
				PlayEffSound(SND_IDX.SFX_0609);
				break;
			default:
				eff.transform.localScale *= 1;
				PlayEffSound(SND_IDX.SFX_0607);
				break;
		}

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;
		int targetcnt = targets.Count;

		for (int i = 0; i < targets.Count; i++) {
			Item_Stage item = targets[i];
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[item.m_Line][item.m_Pos] = null;
		}

		m_Check.Check(StageCheckType.SuppressionF, 0, targetcnt);

		yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_BombSpecialist(Item_Stage_Char Char, TSkillTable skill) {
		for (int cnt = 0; cnt < 2; cnt++) {
			m_SkillZoom = true;
			// 저격 가능한 몬스터 보여주기
			List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
			if (activecards.Count < 1) {
				SkillSpeech(Char, false);
				yield break;
			}
			yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
				if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
			});

			//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
			if (activecards.Count > 0) ShowArea(true, activecards);
			SkillSpeech(Char, true);

			//타겟 및 범위 지정
			Item_Stage targetcard = null;
			AutoCamPosInit = false;
			yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
			ShowArea(false);

			List<Item_Stage> area = new List<Item_Stage>();
			List<Item_Stage> targets = new List<Item_Stage>();
			SetAction_CharArea_Target(skill, targetcard, (card) => {
				if (card == null) return false;
				TStageCardTable tdata = card.m_Info.m_NowTData;
				switch (tdata.m_Type) {
					case StageCardType.OldMine:
					case StageCardType.Allymine:
					case StageCardType.TimeBomb:
						break;
					default:
						return false;
				}
				return true;
			}, ref area, ref targets);

			List<Item_Stage> offcards = new List<Item_Stage>();

			int CardID = Mathf.RoundToInt(skill.m_Value[1]);
			for (int i = 0; i < targets.Count; i++) {
				if (targets[i].m_Info.IsDark && CardID < 1) {
					StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
					targets[i].SetCardChange(info.m_RealIdx);
				}
				else
					targets[i].SetCardChange(CardID < 1 ? CharSkill_CreateStageCardIdx().m_Idx : CardID);
				offcards.Add(targets[i]);
				targets[i].Action(EItem_Stage_Card_Action.Change, 1f, (obj) => { offcards.Remove(obj); });
			}
			yield return new WaitWhile(() => offcards.Count > 0);

			// 대상이 아닌놈들 꺼주기
			for (int i = activecards.Count - 1; i > -1; i--) {
				Item_Stage oncards = activecards[i];
				offcards.Add(oncards);
				oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
			}
			yield return new WaitWhile(() => offcards.Count > 0);
		}

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_RangeAtk(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_AIEnemy() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		// 이펙트

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		ShowArea(true, area);

		List<Item_Stage> chains = new List<Item_Stage>();
		PlayEffSound(SND_IDX.SFX_0549);
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			if (item == null) continue;
			if (m_ViewCard[item.m_Line][item.m_Pos] == null) continue;

			GameObject eff = StartEff(item.transform, "Effect/Stage/Eff_StageCard_Sniping");
			eff.transform.localScale *= 0.6f;

			StageCardInfo info = item.m_Info;
			TStageCardTable tdata = info.m_NowTData;
			if (item.m_Info.m_NowTData.m_Type == StageCardType.OldMine || item.m_Info.m_NowTData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(item, new List<List<Item_Stage>>() { targets });
			else if (item.m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(item, new List<List<Item_Stage>>() { targets });
			}
			else {
				if (tdata.m_Type == StageCardType.Chain) {
					item.m_ChainDieEffPosX = targetcard.transform.position.x;
					chains.Add(item);
				}
				else if (targetcard.m_Info.IS_DmgTarget()) {
					int dmg = GetAtkDmg(targetcard, Char, true, true, false);
					//공격시 dna, synergy 체크
					SetAtkDNASynergy(Char, new object[] { dmg });

					item.SetDamage(false, dmg);

					offcards.Add(item);
					item.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
					if (item.IS_Die()) {
						GetKillEnemy(item, Char, true, true);
					}
				}
			}
		}

		yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);

		if (chains.Count > 0) yield return SelectAction_ChainDie(chains);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_Shuffle(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;

		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if(card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (card.m_Info.IS_RoadBlock) return false;
			if (card.m_Info.m_TData.IS_LineCard()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> Check = new List<Item_Stage>();
		Check.AddRange(targets);
		List<Item_Stage> actioncard = new List<Item_Stage>();

		ShowArea(true, area);

		List<Item_Stage> moveCards = new List<Item_Stage>();
		// 1:1 대응으로 움직임
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage moveCard = targets[i];
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

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_ChangeCardArea(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		//if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Focus_First_Line)) TUTO.Next();
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>() { targetcard };
		List<Item_Stage> targets = new List<Item_Stage>();
		StageCardType cardtype = (StageCardType)(int)skill.m_Value[0];

		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Lock) return false;
			if (card.m_Info.IS_RoadBlock) return false;
			if (cardtype == StageCardType.None && card.m_Info.IS_EnemyCard) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (cardtype != StageCardType.None && tdata.m_Type != cardtype) return false;
			if (tdata.IS_LineCard()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		// 이펙트

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		ShowArea(true, area);
		// 변환 해준다.
		//TIngameRewardTable table = TDATA.GetPickIngameReward(Mathf.RoundToInt(skill.m_Value[1]));
		int CardID = Mathf.RoundToInt(skill.m_Value[1]); //table.m_Val;

		offcards.Clear();
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = targets[i];
			offcards.Add(card);
			if (card.m_Info.IsDark && CardID < 1) {
				StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
				card.SetCardChange(info.m_RealIdx);
			}
			else
				card.SetCardChange(CardID < 1 ? CharSkill_CreateStageCardIdx().m_Idx : CardID);
			card.Action(EItem_Stage_Card_Action.Change, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--)
		{
			Item_Stage card = activecards[i];
			offcards.Add(card);
			card.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);

		//if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Char_1024_SkillEnd)) TUTO.Next();
	}

	IEnumerator CharUseSkill_ChangeRandomDrop(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});
		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		Item_Stage targetcard = null;

		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>() { targetcard };
		List<Item_Stage> targets = new List<Item_Stage>();
		StageCardType cardtype = (StageCardType)(int)skill.m_Value[0];

		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Lock) return false;
			if (cardtype == StageCardType.None && card.m_Info.IS_EnemyCard) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (cardtype != StageCardType.None && tdata.m_Type != cardtype) return false;
			if (card.m_Info.m_TData.IS_LineCard()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		// 이펙트

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		ShowArea(true, area);
		// 변환 해준다.
		

		offcards.Clear();
		for (int i = targets.Count - 1; i > -1; i--) {
			TIngameRewardTable table = TDATA.GetPickIngameReward(Mathf.RoundToInt(skill.m_Value[1]));
			int CardID = table.m_Val; //Mathf.RoundToInt(skill.m_Value[1]);

			Item_Stage card = targets[i];
			offcards.Add(card);
			if (card.m_Info.IsDark && CardID < 1) {
				StageCardInfo info = CreateStageCardData(new List<StageCardType>() { StageCardType.Roadblock, StageCardType.AllRoadblock });
				card.SetCardChange(info.m_RealIdx);
			}
			else
				card.SetCardChange(CardID < 1 ? CharSkill_CreateStageCardIdx().m_Idx : CardID);
			card.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = activecards[i];
			offcards.Add(card);
			card.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_RandomAtk(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});
		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		List<Item_Stage> chains = new List<Item_Stage>();
		List<Item_Stage> Diecheck = new List<Item_Stage>();
		List<Item_Stage> check = new List<Item_Stage>();
		check.AddRange(targets);

		// N회 공격
		for (int k = Mathf.RoundToInt(skill.m_Value[0]); k > 0; k--) {
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
					int dmg = GetAtkDmg(item, Char, true, true, false);
					//공격시 dna, synergy 체크
					SetAtkDNASynergy(Char, new object[] { dmg });

					item.SetDamage(false, dmg);
					if (!Diecheck.Contains(item) && item.IS_Die()) {
						Diecheck.Add(item);
						GetKillEnemy(item, Char, true, true);
					}
				}
				else if(item.m_Info.m_TData.m_Type == StageCardType.Chain) {
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
		yield return new WaitWhile(() => offcards.FindAll(o => targets.Contains(o)).Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_RecoverySrv(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		StatType type = (StatType)Mathf.RoundToInt(skill.m_Value[0]);
		switch (type) {
			case StatType.Men: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Mental"); break;
			case StatType.Hyg: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Hygiene"); break;
			case StatType.Sat: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Fullness"); break;
			default: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal"); break;
		}

		yield return AddStat_Action(Char.transform, type, Mathf.RoundToInt(m_User.GetMaxStat(type) * skillinfo.GetSkillValue()));
	}
	/// <summary> 재료 카드를 다른 재료 카드로 변환한다. (Value 01 = 변환 할 재료카드 ID, Value 02 = 변환 될 재료카드 ID), 재료타입 인덱스가 들어감 </summary>
	IEnumerator CharUseSkill_ChangeMate(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		}, ()=> { if (activecards.Count > 0) ShowArea(true, activecards); });
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage item = activecards[i];
			item.SetCardChange(CharSkill_CreateMatStageCardIdx((StageMaterialType)(int)skill.m_Value[1]).m_Idx);
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);
		ShowArea(false);

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	/// <summary> 3턴 동안 (턴수 고정) 모든 적이 움직이지 않는다. </summary>
	IEnumerator CharUseSkill_StopAll(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		}, ()=> {
			if (activecards.Count > 0) ShowArea(true, activecards);
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		List<Item_Stage> offcards = new List<Item_Stage>();
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			// 라이트 설정 생성
			AIStopInfo aistop = new AIStopInfo(AiStopMode.AREA1);
			aistop.SetTarget(oncard);
			aistop.SetTurn(Mathf.RoundToInt(skill.m_Value[1]));

			PlayEffSound(SND_IDX.SFX_0471);
			StartEff(oncard.transform, "Effect/Stage/Eff_Debuff_Card");
			// 이펙트 등록
			GameObject Eff = StartEff(oncard.transform, "Effect/Stage/Eff_StageCard_Paralysis", true);
			Eff.transform.localScale = Vector3.one;
			aistop.SetEff(Eff);

			AddAIStopInfo(aistop);
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		yield return new WaitForSeconds(0.75f);//1f
		yield return new WaitWhile(() => offcards.Count > 0);

		ShowArea(false);

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	/// <summary> 범위 내 모든 특정 재료를 습득한다. (Value02 = 재료 카드 ID) </summary>
	IEnumerator CharUseSkill_GetCards(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Material || (int)tdata.m_Value1 != (int)skill.m_Value[1]) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			item.Action(EItem_Stage_Card_Action.FadeOut, 0f, (obj) => {
				targets.Remove(obj);
				TStageCardTable tdata = obj.m_Info.m_NowTData;
				AddMaterial((StageMaterialType)Mathf.RoundToInt(tdata.m_Value1), Mathf.RoundToInt(tdata.m_Value2 + obj.m_Info.m_PlusCnt));
				RemoveStage(obj);
			});
			m_ViewCard[item.m_Line][item.m_Pos] = null;
		}

		yield return new WaitWhile(() => targets.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	/// <summary> 범위 내 재료카드의 개수가 +1 증가한다. </summary>
	IEnumerator CharUseSkill_PlusMate(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Material) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			item.m_Info.m_PlusCnt += 1;
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.ChangeVal, 0.6f, (obj) => {//1->0.6
				offcards.Remove(obj);
			});
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			offcards.Add(item);
			item.Action(EItem_Stage_Card_Action.TargetOff, 1f, (obj) => {
				offcards.Remove(obj);
			});
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	/// <summary> 범위 내 모든 특정 재료를 습득한다. </summary>
	IEnumerator CharUseSkill_GetMate(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);
		yield return new WaitForSeconds(0.75f);//1.5

		yield return CharUseSkill_End(Char, m_SkillZoom);

		AddMaterial((StageMaterialType)Mathf.RoundToInt(skill.m_Value[0]), UTILE.Get_Random(Mathf.RoundToInt(skill.m_Value[1]), Mathf.RoundToInt(skill.m_Value[2]) + 1));

		if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Char_1012_SkillEnd)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.Char_1013_SkillEnd)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.Char_1002_SkillEnd)) TUTO.Next();
	}
	/// <summary> 현재 포만도가 크게 감소하지만 체력을 회복한다.(Value 01 = 감소 포만도 (절대값), Value 02 = 회복 체력(만분율)) </summary>
	IEnumerator CharUseSkill_PainSat(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		float hp = STAGE_USERINFO.GetCharStat(charinfo, StatType.Heal) * skill.GetValue(skillinfo.m_LV);
		int sat = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) * skill.m_Value[0]);
		yield return AddStat_Action(Char.transform, StatType.Sat, -sat, StatType.HP, Mathf.RoundToInt(hp));
	}

	/// <summary> 현재 청결도가 크게 감소하지만 체력을 회복한다. (Value 01 = 감소 청결도 (절대값), Value 02 = 회복 체력(만분율)). </summary>
	IEnumerator CharUseSkill_PainHyg(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		float hp = STAGE_USERINFO.GetCharStat(charinfo, StatType.Heal) * skill.GetValue(skillinfo.m_LV);
		int hyg = Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) * skill.m_Value[0]);
		yield return AddStat_Action(Char.transform, StatType.Hyg, -hyg, StatType.HP, Mathf.RoundToInt(hp));
	}
	/// <summary> 스킬 사용 행동력을 회복한다. (Value01 = 행동력, Value 02 = 회복 수치(절대값)) </summary>
	IEnumerator CharUseSkill_RecoveryMove(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		// 이펙트
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		//멘탈 부족에 따른 행동력 회복 감소
		float val = skill.m_Value[1];
		int preval = m_User.m_AP[0];
		m_User.m_AP[0] = Mathf.Clamp(m_User.m_AP[0] + Mathf.RoundToInt(val), 0, m_User.m_AP[1]);
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
		}
		DLGTINFO?.f_RfAPUI?.Invoke(m_User.m_AP[0], preval, m_User.m_AP[1]);
	}
	IEnumerator CharUseSkill_SacrificeTornBody(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.TornBody) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Mental");
		
		yield return AddStat_Action(Char.transform, StatType.Men, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Men) * skillinfo.GetSkillValue()));

		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_SacrificePit(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Pit) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Fullness");
		yield return AddStat_Action(Char.transform, StatType.Sat, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Sat) * skillinfo.GetSkillValue()));

		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_SacrificeGarbage(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Garbage) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Hygiene");
		yield return AddStat_Action(Char.transform, StatType.Hyg, Mathf.RoundToInt(m_User.GetMaxStat(StatType.Hyg) * skillinfo.GetSkillValue()));

		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_DownLevel(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_EnemyCard) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		float downratio = UTILE.Get_Random(skill.m_Value[0], skill.m_Value[1]);
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage item = targets[i];
			StartEff(item.transform, "Effect/Stage/Eff_Debuff_Card");
			offcards.Add(item);
			item.m_Info.m_LV = Mathf.Max(1, Mathf.FloorToInt(item.m_Info.m_LV * (100f - downratio) * 0.01f));
			item.Action(EItem_Stage_Card_Action.ChangeVal, 0.6f, (obj) => {//1->0.6
				offcards.Remove(item);
			});
		}
		yield return new WaitForSeconds(0.75f);//1f
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_AllRecoverySrv(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		for (int i = (int)StatType.Men; i < (int)StatType.SurvEnd; i++) {
			StatType type = (StatType)i;
			switch (type) {
				case StatType.Men: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Mental"); break;
				case StatType.Hyg: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Hygiene"); break;
				case StatType.Sat: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Fullness"); break;
				default: StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal"); break;
			}

			StartCoroutine(AddStat_Action(Char.transform, type, Mathf.RoundToInt(m_User.GetMaxStat(type) * skillinfo.GetSkillValue())));
		}
	}

	IEnumerator CharUseSkill_BanAirStrike(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		if (m_ModeCnt.ContainsKey(PlayType.FieldAirstrike)) {
			//m_ModeCnt[PlayType.FieldAirstrike] += Mathf.RoundToInt(skill.m_Value[1]);
			STAGE.m_MainUI.RefreshModeAlarm(PlayType.FieldAirstrike, Mathf.RoundToInt(skill.m_Value[1]));
		}

		yield return new WaitForSeconds(0.75f);//1.5f
		PlayEffSound(SND_IDX.SFX_0404);
		StartEff(Vector3.zero, "Effect/Stage/Eff_StageCard_WaveBreak");

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_WideAttack(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		PlayEffSound(SND_IDX.SFX_0430);
		// 이펙트
		for (int i = 0; i < targets.Count; i++) {
			if (targets[i] == null) continue;
			if (m_ViewCard[targets[i].m_Line][targets[i].m_Pos] == null) continue;
			if (targets[i].m_Info.m_TData.m_Type == StageCardType.OldMine || targets[i].m_Info.m_TData.m_Type == StageCardType.Allymine)
				yield return SelectAction_StageCardProc_OldMine(targets[i], new List<List<Item_Stage>>() { targets });
			else if (targets[i].m_Info.IS_OilGas()) {
				yield return SelectAction_StageCardProc_OilGasStation(targets[i], new List<List<Item_Stage>>() { targets });
			}
			else if (targetcard.m_Info.IS_DmgTarget()){
				int dmg = GetAtkDmg(targetcard, Char, true, true, false);
				//공격시 dna, synergy 체크
				SetAtkDNASynergy(Char, new object[] { dmg });

				targets[i].SetDamage(false, dmg);
				if (targets[i].IS_Die()) {
					GetKillEnemy(targets[i], Char, true, true);
				}
				GameObject Eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Sniping");
				Eff.transform.localScale = Vector3.one;
			}
		}

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_DeleteEnemyTribe(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_DmgTarget()) return false;
			EEnemyType type = (int)card.m_Info.m_TEnemyData.m_Type > 4 ? (EEnemyType)4 : card.m_Info.m_TEnemyData.m_Type;
			if (type != (EEnemyType)skill.m_Value[1]) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			if (skill.m_SuccProb < UTILE.Get_Random(0, 1)) continue;
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_SteelItem(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, false, 0, 0, Char.m_Info.IS_SetEquip());
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		//SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		if (UTILE.Get_Random(0f, 1f) > skill.m_SuccProb) {//확률 실패시 대사만 치고 끝 skill.m_SuccProb
			yield return CharUseSkill_End(Char, m_SkillZoom);// rewardaction에서 IS_SelectAction_Pause 계속 돌기때문에 강제로 껏다 켜야함

			PlayEffSound(SND_IDX.SFX_0471);

			STAGE_USERINFO.SetSpeech(TDATA.GetString(ToolData.StringTalbe.Dialog, 5007004), Char.transform, 1.5f);

			yield return new WaitForSeconds(0.75f);//1f

			int lv = targetcard.m_Info.m_TEnemyData.m_RewardLV;
			int rewardgid = targetcard.m_Info.m_TEnemyData.m_RewardGID;
			bool allgroup = targetcard.m_Info.m_TEnemyData.m_AllGroup;
			bool cancanble = targetcard.m_Info.m_TEnemyData.m_RewardCancle;

			yield return Action_BattleReward(rewardgid, lv, cancanble, allgroup, false);
		}
		else {
			PlayEffSound(SND_IDX.SFX_0460);
			STAGE_USERINFO.CharSpeech(DialogueConditionType.SkillFail, Char);
		}

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_RecoveryStatus(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != (StageCardType)Mathf.RoundToInt(skill.m_Value[0])) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		//제거된 수만큼 스테이터스 회복
		StatType stat = (StatType)Mathf.RoundToInt(skill.m_Value[1]);
		yield return AddStat_Action(m_CenterChar.transform, stat, targets.Count * Mathf.RoundToInt(skill.m_Value[2]));

		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_APHP(Item_Stage_Char Char, TSkillTable skill) {
		int nowap = STAGE_USERINFO.m_AP[0];
		if (nowap < 1) {
			SkillSpeech(Char, false);
			yield break;
		}


		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		m_SkillZoom = false;
		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		STAGE_USERINFO.m_AP[0] = 0;
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
		}

		//DNA 버프
		float AddHP = DNACheck(Char, OptionType.CureHealAdd, new object[] { skill.GetValue(skillinfo.m_LV) });
		yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(nowap * skill.m_Value[0] * (STAGE_USERINFO.GetMaxStat(StatType.HP) + AddHP)));

		//DNA 버프
		DNACheck(Char, OptionType.CureApAdd);
	}
	IEnumerator CharUseSkill_Unlock(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		List<Item_Stage> offcards = new List<Item_Stage>();
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		for (int i = activecards.Count - 1; i > -1; i--) {
			offcards.Add(activecards[i]);
			activecards[i].Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, activecards);

		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = activecards.Count - 1; i > -1; i--) {
			offcards.Add(activecards[i]);
			activecards[i].Action(EItem_Stage_Card_Action.UnLock, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}
		yield return new WaitWhile(() => offcards.Count > 0);
		yield return new WaitForSeconds(0.75f);//1f

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_CopyMaterial(Item_Stage_Char Char, TSkillTable skill) {
		int nowmerge = STAGE.m_MainUI.GetCraft().GetEmptyCnt;
		int mergemat = STAGE.m_MainUI.GetCraft().GetCardCnt;
		if (nowmerge < 1 || mergemat < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		STAGE.m_MainUI.GetCraft().RandCopy();
	}
	IEnumerator CharUseSkill_DropItemHp(Item_Stage_Char Char, TSkillTable skill) {
		int nowmerge = STAGE.m_MainUI.GetCraft().GetCardCnt;
		if (nowmerge < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		STAGE.m_MainUI.GetCraft().DiscardAll();

		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		//DNA 버프
		float AddHP = DNACheck(Char, OptionType.CureHealAdd, new object[] { skill.GetValue(skillinfo.m_LV) });
		yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(nowmerge * skill.m_Value[0] * STAGE_USERINFO.GetMaxStat(StatType.HP) + AddHP));

		//DNA 버프
		DNACheck(Char, OptionType.CureApAdd);
	}
	IEnumerator CharUseSkill_HPAP(Item_Stage_Char Char, TSkillTable skill) {
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		float hpratio = STAGE_USERINFO.GetStat(StatType.HP) / STAGE_USERINFO.GetMaxStat(StatType.HP);
		if (hpratio < skill.m_Value[0]) {
			SkillSpeech(Char, false);
			yield break;
		}

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		int pre = STAGE_USERINFO.m_AP[0];
		STAGE_USERINFO.m_AP[0] = Mathf.Min(STAGE_USERINFO.m_AP[0] + Mathf.RoundToInt(skill.m_Value[1]), STAGE_USERINFO.m_AP[1]);
		DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], pre, STAGE_USERINFO.m_AP[1]);

		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
		}

		yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(skill.m_Value[0] * STAGE_USERINFO.GetMaxStat(StatType.HP)));
	}
	IEnumerator CharUseSkill_LastAttack(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_DmgTarget()) return false;
			if (card.m_Info.GetStat(EEnemyStat.HP) == card.m_Info.GetMaxStat(EEnemyStat.HP)) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (oncards == targetcard) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0400);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		ShowArea(true, targets);

		for (int i = targets.Count - 1; i > -1; i--) {
			if (m_ViewCard[targets[i].m_Line][targets[i].m_Pos] == null) continue;
			
			// 이펙트
			GameObject eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Sniping");
			eff.transform.localScale *= 0.6f;

			int atk = Mathf.RoundToInt(STAGE_USERINFO.GetCharStat(charinfo, StatType.Atk));
			atk += Mathf.RoundToInt(atk * m_User.GetAtkSkillVal(targets[i].m_Info.m_TEnemyData));
			float dna = DNACheck(Char, OptionType.AttackingDmgAdd);
			atk += Mathf.RoundToInt(atk * dna);
			// 타겟 데미지 주기
			//시너지
			int dmg = targets[i].m_Info.GetStat(EEnemyStat.HP);
			//공격시 dna, synergy 체크
			SetAtkDNASynergy(Char, new object[] { dmg });

			targets[i].SetDamage(false, dmg);
			if (targets[i].IS_Die()) {
				GetKillEnemy(targets[i], Char, true, true);
			}
		}

		yield return IE_CamAction(CamActionType.Shake_0, 0.05f, 0.4f, Vector3.one * 0.01f);

		offcards.Add(targetcard);
		targetcard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
			offcards.Remove(obj);
		});
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_DestoryWall(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_RoadBlock) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, targets);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);
		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	
	IEnumerator CharUseSkill_UnDebuff(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		bool can = false;
		for (int i = 0; i < 3; i++) {
			StatType stat = (StatType)Mathf.RoundToInt(skill.m_Value[i]);
			switch (stat) {
				case StatType.Men:
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.CardLock) && STAGE_USERINFO.m_DebuffValues[DebuffType.CardLock] > 0f) can = true;
					break;
				case StatType.Hyg:
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusHpRecovery) && STAGE_USERINFO.m_DebuffValues[DebuffType.MinusHpRecovery] > 0f) can = true;
					break;
				case StatType.Sat:
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusAP) && STAGE_USERINFO.m_DebuffValues[DebuffType.MinusAP] > 0f) can = true;
					break;
			}
		}
		if (!can) {
			SkillSpeech(Char, false);
			yield break;
		}

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");

		for (int i = 0; i < 3; i++) {
			can = false;
			StatType stat = (StatType)Mathf.RoundToInt(skill.m_Value[i]);
			switch (stat) {
				case StatType.Men:
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.CardLock) && STAGE_USERINFO.m_DebuffValues[DebuffType.CardLock] > 0f) can = true;
					break;
				case StatType.Hyg:
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusHpRecovery) && STAGE_USERINFO.m_DebuffValues[DebuffType.MinusHpRecovery] > 0f) can = true;
					break;
				case StatType.Sat:
					if (STAGE_USERINFO.m_DebuffValues.ContainsKey(DebuffType.MinusAP) && STAGE_USERINFO.m_DebuffValues[DebuffType.MinusAP] > 0f) can = true;
					break;
			}
			if (can) {
				yield return AddStat_Action(m_CenterChar.transform, stat, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat(stat) * 0.6f - STAGE_USERINFO.GetStat(stat)));
			}
		}
	}
	IEnumerator CharUseSkill_DarkPatrol(Item_Stage_Char Char, TSkillTable skill) {
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		m_DarkPatrolCnt += Mathf.RoundToInt(skill.m_Value[0]);
		DarkPatrolCheck(true);

		yield return new WaitForSeconds(0.75f);//1f

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_Gamble(Item_Stage_Char Char, TSkillTable skill) {
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		TGambleCardTable gambletable = TDATA.GetRandGambleCardTable(STAGEINFO.m_StageModeType);
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

		yield return new WaitForSeconds(0.75f);//1f

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_DontAttack(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (!card.m_Info.IS_AIEnemy()) return false;
			if (card.m_Info.IS_AIEnemy() && card.m_Info.ISRefugee) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;
		int cnt = Mathf.RoundToInt(skill.m_Value[0]);
		PlayEffSound(SND_IDX.SFX_0471);
		for (int i = 0; i < targets.Count; i++) {
			StartEff(targets[i].transform, "Effect/Stage/Eff_Debuff_Card");
			// 이펙트 등록
			GameObject Eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Paralysis", true);
			Eff.transform.localScale = Vector3.one;

			//정지 설정 생성
			AiBlockAtkInfo aiblockatk = new AiBlockAtkInfo(AiBlockAtkMode.AREA1);
			aiblockatk.SetTarget(targets[i]);
			aiblockatk.SetTurn(cnt > 0 ? cnt : 100);
			aiblockatk.SetEff(Eff);
			AddAiBlockAtkInfo(aiblockatk);
		}

		yield return new WaitForSeconds(0.75f);//1f
		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_DeleteBadCard(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Garbage && tdata.m_Type != StageCardType.TornBody && tdata.m_Type != StageCardType.Pit) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
				offcards.Remove(obj);
				RemoveStage(obj);
			});
			m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_LearningAbility2(Item_Stage_Char Char, TSkillTable skill) {
		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}
		bool isset = skill.m_Type == SkillType.SetActive;
		if (isset) isset = UTILE.Get_Random(0f, 1f) < skill.m_Value[0];
		List<Item_Stage_Char> chars = new List<Item_Stage_Char>();
		chars.AddRange(m_Chars);
		chars.RemoveAll(t => t.m_Info == null || (t.m_Info != null && t == Char));
		Item_Stage_Char randchar = chars[UTILE.Get_Random(0, chars.Count)];
		skill = TDATA.GetSkill(randchar.m_Info.m_TData.m_SkillIdx[isset ? (int)SkillType.SetActive : (int)SkillType.Active]);

		Dictionary<int, int> preskillval = new Dictionary<int, int>();
		Char.LearningAbility(skill);
		for(int i = 0;i< Char.m_Info.m_Skill.Length; i++) {
			preskillval.Add(Char.m_Info.m_Skill[i].m_Idx, Char.m_Info.m_Skill[i].m_LV);
			Char.m_Info.m_Skill[i].m_Idx = randchar.m_Info.m_Skill[i].m_Idx;
			Char.m_Info.m_Skill[i].m_LV = randchar.m_Info.m_Skill[i].m_LV;
		}

		int pretransmode = Char.m_TransverseAtkMode;
		Char.m_TransverseAtkMode = randchar.m_TransverseAtkMode;

		yield return UseUseSkill(Char, skill);

		Char.LearningAbility(null); for (int i = 0; i < Char.m_Info.m_Skill.Length; i++) {
			Char.m_Info.m_Skill[i].m_Idx = preskillval.ElementAt(i).Key;
			Char.m_Info.m_Skill[i].m_LV = preskillval.ElementAt(i).Value;
		}
		Char.m_TransverseAtkMode = pretransmode;

		//연속 팝업으로 타임스케일 1되서 다시세팅함
		m_MainUI.AccToggleCheck();

		if (m_SkillUseInfoPopup != null)
			yield return SkillCancle(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_Hide(Item_Stage_Char Char, TSkillTable skill) {
		SkillInfo skillinfo = Char.m_Info.m_Skill[0];

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		if (!STAGE_USERINFO.ISBuff(StageCardType.Hide))
			STAGE_USERINFO.m_BuffValues.Add(StageCardType.Hide, 0);
		if(STAGE_USERINFO.m_BuffValues[StageCardType.Hide] == 0) StartEff(Vector3.zero, "Effect/Stage/Eff_ChSkill_Hide", true);
		STAGE_USERINFO.m_BuffValues[StageCardType.Hide] += skill.m_Value[0];

		yield return new WaitForSeconds(0.75f);//1f

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_KeepMaterial(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (tdata.m_Type != StageCardType.Ash) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = 0; i < targets.Count; i++) {
			offcards.Add(targets[i]);
			m_Check.Check(StageCheckType.CardUse, (int)targets[i].m_Info.m_TData.m_Type, 1, false);

			TIngameRewardTable table = TDATA.GetPickIngameReward(Mathf.RoundToInt(targets[i].m_Info.m_TData.m_Value1), 0, CheckBattleReward);
			targets[i].SetCardChange(table.m_Val);
			bool Action = true;
			targets[i].Action(EItem_Stage_Card_Action.Change, 0, (obj) => {
				Action = false;
			});
			yield return new WaitWhile(() => Action);

			TStageCardTable cardtable = targets[i].m_Info.m_RealTData;
			switch (targets[i].m_Info.m_RealTData.m_Type) {
				//case StageCardType.Material: AddMaterial((StageMaterialType)Mathf.RoundToInt(cardtable.m_Value1), Mathf.RoundToInt(cardtable.m_Value2)); break;
				case StageCardType.Sniping: m_MainUI?.GetMakeUtileCard(StageMaterialType.Sniping, 1); break;
				case StageCardType.Shotgun: m_MainUI?.GetMakeUtileCard(StageMaterialType.ShotGun, 1); break;
				case StageCardType.MachineGun: m_MainUI?.GetMakeUtileCard(StageMaterialType.GatlingGun, 1); break;
				case StageCardType.AirStrike: m_MainUI?.GetMakeUtileCard(StageMaterialType.AirStrike, 1); break;
				case StageCardType.ShockBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.ShockBomb, 1); break;
				case StageCardType.Dynamite: m_MainUI?.GetMakeUtileCard(StageMaterialType.Dynamite, 1); break;
				case StageCardType.Grenade: m_MainUI?.GetMakeUtileCard(StageMaterialType.Grenade, 1); break;
				case StageCardType.LightStick: m_MainUI?.GetMakeUtileCard(StageMaterialType.LightStick, 1); break;
				case StageCardType.FlashLight: m_MainUI?.GetMakeUtileCard(StageMaterialType.FlashLight, 1); break;
				case StageCardType.StarShell: m_MainUI?.GetMakeUtileCard(StageMaterialType.Flare, 1); break;
				case StageCardType.ThrowExtin: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireSpray, 1); break;
				case StageCardType.PowderExtin: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireExtinguisher, 1); break;
				case StageCardType.PowderBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.PowderBomb, 1); break;
				case StageCardType.FireBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireBomb, 1); break;
				case StageCardType.FireGun: m_MainUI?.GetMakeUtileCard(StageMaterialType.FireGun, 1); break;
				case StageCardType.NapalmBomb: m_MainUI?.GetMakeUtileCard(StageMaterialType.NapalmBomb, 1); break;
				default:
					yield return SelectAction_StageCardProc(targets[i], false);
					break;
			}

			if (targets[i] != null && m_ViewCard[targets[i].m_Line][targets[i].m_Pos] != null) {
				targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
					offcards.Remove(obj);
					RemoveStage(obj);
				});
				m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
			}
			else offcards.Remove(targets[i]);
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_MakeRefugee(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;

		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		List<Item_Stage> offcards = new List<Item_Stage>();
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_EnemyCard) return false;
			if (card.m_Info.m_TEnemyData.m_Tribe != EEnemyTribe.Human) return false;
			if (card.m_Info.ISRefugee) return false;
			return true;
		}, ref area, ref targets);

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		if (targets.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		ShowArea(true, area);

		// 변환 해준다.

		offcards.Clear();
		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = targets[i];
			offcards.Add(card);

			int CardID = UTILE.Get_Random(20102, 20107);//Defaultcardtable 피난민 idx
			card.SetCardChange(CardID);
			card.Action(EItem_Stage_Card_Action.Change, 1f, (obj) => { offcards.Remove(obj); });
		}

		yield return new WaitWhile(() => offcards.Count > 0);

		for (int i = targets.Count - 1; i > -1; i--) {
			Item_Stage card = targets[i];
			offcards.Add(card);
			card.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}
		yield return new WaitWhile(() => offcards.Count > 0);

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_DropItemAP(Item_Stage_Char Char, TSkillTable skill) {
		int nowmerge = STAGE.m_MainUI.GetCraft().GetCardCnt;
		if (nowmerge < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		SkillSpeech(Char, true);

		yield return CharUseSkill_End(Char, m_SkillZoom);

		int cnt = STAGE.m_MainUI.GetCraft().DiscardAll();
		int pre = STAGE_USERINFO.m_AP[0];
		STAGE_USERINFO.m_AP[0] = Mathf.Min(STAGE_USERINFO.m_AP[0] + Mathf.RoundToInt(skill.m_Value[0]) * cnt, STAGE_USERINFO.m_AP[1]);
		DLGTINFO?.f_RfAPUI?.Invoke(STAGE_USERINFO.m_AP[0], pre, STAGE_USERINFO.m_AP[1]);

		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
		}

		StartEff(Char.transform, "Effect/Stage/Eff_ChSkill_Heal");
	}
	IEnumerator CharUseSkill_CountTribeMaterial(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			TStageCardTable tdata = card.m_Info.m_NowTData;
			if (!card.m_Info.IS_EnemyCard) return false;
			if (card.m_Info.m_TEnemyData.m_Tribe != (EEnemyTribe)Mathf.RoundToInt(skill.m_Value[0])) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		//랜덤 재료 획득
		float dna = Char.m_Info.GetDNABuff(OptionType.AttackingMaterialAdd);
		if (dna > 0) STAGE.DNAAlarm(Char.m_Info, OptionType.AttackingMaterialAdd);
		int dnamatcnt = Mathf.RoundToInt(dna);
		for (int i = 0; i < targets.Count + dnamatcnt; i++) {
			AddMaterial((StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat), 1);
		}

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_DestoryWall02(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 타겟팅 가능한 대상 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE, true);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.IS_Lock) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_RoadBlock && !card.m_Info.IS_ExplosionTarget()) return false;
			if (card.m_Info.m_NowTData.m_Type == StageCardType.AllRoadblock) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncard = activecards[i];
			if (oncard == null) continue;
			offcards.Add(oncard);
			oncard.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => {
				offcards.Remove(obj);
			});
		}

		ShowArea(true, targets);
		yield return new WaitWhile(() => offcards.Count > 0);

		PlayEffSound(SND_IDX.SFX_0620);

		for (int i = 0; i < targets.Count; i++) {
			if (targets[i] == null) continue;
			if (m_ViewCard[targets[i].m_Line][targets[i].m_Pos] == null) continue;

			// 이펙트
			GameObject eff = StartEff(targets[i].transform, "Effect/Stage/Eff_StageCard_Drill");
			eff.transform.localScale *= 0.6f;

			if (targets[i].m_Info.IS_DmgTarget()) {
				int dmg = GetAtkDmg(targets[i], Char, true, true, false);
				targets[i].SetDamage(false, dmg);
				if (targets[i].IS_Die()) {
					GetKillEnemy(targets[i], Char, true, true);
				}
			}
			else {
				if (targets[i].m_Info.m_TData.m_Type == StageCardType.OldMine || targets[i].m_Info.m_TData.m_Type == StageCardType.Allymine)
					yield return SelectAction_StageCardProc_OldMine(targets[i], new List<List<Item_Stage>>() { targets });
				else if (targets[i].m_Info.IS_OilGas()) {
					yield return SelectAction_StageCardProc_OilGasStation(targets[i], new List<List<Item_Stage>>() { targets });
				}
				else {
					offcards.Add(targets[i]);
					targets[i].Action(EItem_Stage_Card_Action.Die, 0f, (obj) => {
						offcards.Remove(obj);
						RemoveStage(obj);
					});
					m_ViewCard[targets[i].m_Line][targets[i].m_Pos] = null;
				}
			}
		}
		//카메라 쉐이크
		yield return IE_CamAction(CamActionType.Shake_0, 0f, 1f, Vector3.one * 0.008f);

		yield return new WaitWhile(() => offcards.Count > 0);
		yield return new WaitForSeconds(0.75f);//1.5f

		AutoCamPosInit = true;
		yield return CharUseSkill_End(Char, m_SkillZoom);
	}

	IEnumerator CharUseSkill_RandomWeapon(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = false;

		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		});
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}

		yield return CharUseSkill_End(Char, m_SkillZoom);

		List<List<StageMaterialType>> types = new List<List<StageMaterialType>>() {
			new List<StageMaterialType>(){StageMaterialType.Sniping, StageMaterialType.ShockBomb, StageMaterialType.FireBomb },
			new List<StageMaterialType>(){StageMaterialType.ShotGun, StageMaterialType.Grenade, StageMaterialType.FireGun  },
			new List<StageMaterialType>(){StageMaterialType.GatlingGun, StageMaterialType.Dynamite, StageMaterialType.NapalmBomb, StageMaterialType.AirStrike } };

		float rand = UTILE.Get_Random(0f, 1f);
		StageMaterialType type = StageMaterialType.None;
		if (rand < skill.m_Value[0]) type = types[0][UTILE.Get_Random(0, types[0].Count)];
		else if (rand < skill.m_Value[0] + skill.m_Value[1]) type = types[1][UTILE.Get_Random(0, types[1].Count)];
		else if (rand < skill.m_Value[0] + skill.m_Value[1] + skill.m_Value[2]) type = types[2][UTILE.Get_Random(0, types[2].Count)];
		TStageCardTable carddata = TDATA.GetStageCardTable(TDATA.GetStageMakingList().Find(t => t.m_MatType == type).m_CardIdx);

		bool popupend = false;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Skill_Judith, (result, obj) => { popupend = true; }, carddata);
		SkillSpeech(Char, true);

		yield return new WaitWhile(() => !popupend);

		STAGE.m_MainUI.GetCraft().GetUtile(type, 1, Vector3.zero);
	}
	IEnumerator CharUseSkill_RandomAtk02(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;
		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, true, null, (res) => {
			if (res == 1) StartCoroutine(SkillCancle(Char, m_SkillZoom));
		});

		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill);
		if (activecards.Count > 0) ShowArea(true, activecards);
		SkillSpeech(Char, true);

		//타겟 및 범위 지정
		Item_Stage targetcard = null;
		AutoCamPosInit = false;
		yield return SelectAction_CharSkill_CheckSelect((obj) => { targetcard = obj; });
		ShowArea(false);

		List<Item_Stage> area = new List<Item_Stage>();
		List<Item_Stage> targets = new List<Item_Stage>();
		SetAction_CharArea_Target(skill, targetcard, (card) => {
			if (card == null) return false;
			if (card.IS_Die()) return false;
			if (card.m_Info.ISNotAtkRefugee) return false;
			if (!card.m_Info.IS_DmgTarget() && !card.m_Info.IS_ExplosionTarget()) return false;
			return true;
		}, ref area, ref targets);

		List<Item_Stage> offcards = new List<Item_Stage>();
		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targets.Contains(oncards)) continue;
			offcards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { offcards.Remove(obj); });
		}

		ShowArea(true, area);
		yield return new WaitWhile(() => offcards.Count > 0);

		SkillInfo skillinfo = Char.m_Info.m_Skill[0];
		CharInfo charinfo = Char.m_Info;

		List<Item_Stage> chains = new List<Item_Stage>();
		List<Item_Stage> Diecheck = new List<Item_Stage>();
		List<Item_Stage> check = new List<Item_Stage>();
		check.AddRange(targets);

		// N회 공격
		for (int k = Mathf.RoundToInt(skill.m_Value[0]); k > 0; k--) {
			List<Item_Stage> list = targets.FindAll(t => {
				if (t == null) return false;
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
					int dmg = GetAtkDmg(item, Char, true, true, false);

					//공격시 dna, synergy 체크
					SetAtkDNASynergy(Char, new object[] { dmg });

					item.SetDamage(false, dmg);
					if (!Diecheck.Contains(item) && item.IS_Die()) {
						Diecheck.Add(item);
						GetKillEnemy(item, Char, true, true);
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
		yield return new WaitWhile(() => offcards.FindAll(o => targets.Contains(o)).Count > 0);

		AutoCamPosInit = true;

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	IEnumerator CharUseSkill_EnemyPull(Item_Stage_Char Char, TSkillTable skill) {
		m_SkillZoom = true;

		// 저격 가능한 몬스터 보여주기
		List<Item_Stage> activecards = SetView_CharArea_Target(skill, AI_MAXLINE);
		if (activecards.Count < 1) {
			SkillSpeech(Char, false);
			yield break;
		}

		// 앞라인 꺼주기
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			Item_Stage card = m_ViewCard[0][i];
			activecards.Remove(card);
			card.Action(EItem_Stage_Card_Action.TargetOff);
		}

		int cancleval = -1;
		yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false, null, (res) => {
			cancleval = res;
		}, ()=> { if(activecards.Count > 0) ShowArea(true, activecards); });
		yield return new WaitWhile(() => cancleval == -1);
		if (cancleval == 1) {
			yield return SkillCancle(Char, m_SkillZoom);
			yield break;
		}
		//yield return CharUseSkill_Start(Char, m_SkillZoom, skill, false);
		SkillSpeech(Char, true);

		//타겟 설정
		List<Item_Stage> targetcards = new List<Item_Stage>();
		while(targetcards.Count < 3 && targetcards.Count < activecards.Count) {
			Item_Stage target = activecards[UTILE.Get_Random(0, activecards.Count)];
			if (targetcards.Contains(target)) continue;
			targetcards.Add(target);
		}

		ShowArea(false);

		yield return new WaitWhile(() => Mathf.Abs(CamMoveGapX) > 0.01f);
		// 중앙 카드 제거
		List<Item_Stage> actioncards = new List<Item_Stage>();
		List<Item_Stage> movecards = new List<Item_Stage>();
		List<int> poses = new List<int>() { 0, 1, 2 };
		//바뀔 첫열 자리 랜덤
		for (int i = 0; i < targetcards.Count; i++) {
			int rand = UTILE.Get_Random(0, poses.Count);
			int pos = poses[rand];
			poses.RemoveAt(rand);
			Item_Stage MoveCard = m_ViewCard[0][pos];
			movecards.Add(MoveCard);
		}

		// 대상이 아닌놈들 꺼주기
		for (int i = activecards.Count - 1; i > -1; i--) {
			Item_Stage oncards = activecards[i];
			if (targetcards.Contains(oncards)) continue;
			if (movecards.Contains(oncards)) continue;
			actioncards.Add(oncards);
			oncards.Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { actioncards.Remove(obj); });
		}

		yield return new WaitWhile(() => actioncards.Count > 0);
		//첫열 바뀔것 제거
		for (int i = 0; i < targetcards.Count; i++) {
			Item_Stage MoveCard = movecards[i];

			actioncards.Add(MoveCard);
			MoveCard.Action(EItem_Stage_Card_Action.Die, 0, (obj) => {
				// 카드 pool 이동
				actioncards.Remove(obj);
			});
		}

		yield return new WaitWhile(() => actioncards.Count > 0);
		//첫열과 자리 바꿈
		for (int i = 0; i < targetcards.Count; i++) {
			Item_Stage MoveCard = movecards[i];
			Item_Stage targetcard = targetcards[i];
			// 선택카드 자리 변경
			m_ViewCard[targetcard.m_Line][targetcard.m_Pos] = MoveCard;
			m_ViewCard[0][MoveCard.m_Pos] = targetcard;

			actioncards.Add(targetcard);
			targetcard.Action(EItem_Stage_Card_Action.MoveTarget, 0, (obj) => {
				actioncards.Remove(obj);
			}, MoveCard);
		}

		yield return new WaitWhile(() => actioncards.Count > 0);
		//바뀐 뒤 타겟 오프
		for (int i = 0; i < targetcards.Count; i++) {
			Item_Stage MoveCard = movecards[i];
			m_ViewCard[MoveCard.m_Line][MoveCard.m_Pos] = null;
			RemoveStage(MoveCard);

			actioncards.Add(targetcards[i]);
			targetcards[i].Action(EItem_Stage_Card_Action.TargetOff, 0f, (obj) => { actioncards.Remove(obj); });
		}
		yield return new WaitWhile(() => actioncards.Count > 0);

		// 앞라인 켜주기
		for (int i = 0; i < m_ViewCard[0].Length; i++) {
			Item_Stage card = m_ViewCard[0][i];
			activecards.Add(card);
			card.Action(EItem_Stage_Card_Action.TargetOff, 0, (obj)=> { activecards.Remove(card); });
		}
		yield return new WaitWhile(() => actioncards.Count > 0);

		yield return CharUseSkill_End(Char, m_SkillZoom);
	}
	void SkillSpeech(Item_Stage_Char _char, bool _canuse) {
		//TConditionDialogueGroupTable table;
		if (_canuse) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UseSkill, _char);
			SND.StopAllVoice();
			SND_IDX vocidx = _char.m_Info.m_TData.GetVoice(TCharacterTable.VoiceType.Skill, _char.m_TransverseAtkMode);
			PlayVoiceSnd(new List<SND_IDX>() { vocidx });
		}
		else
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnSkill, _char); 
	}

	public IEnumerator SkillCancle(Item_Stage_Char Char, bool _zoom, bool _selectchar = false) {
		AutoCamPosInit = true;
		LockCamScroll = false;
		if (m_SkillUseInfoPopup != null) {
			// 팝업 닫아주기
			m_SkillUseInfoPopup.Close();
			m_SkillUseInfoPopup = null;
		}
		ShowArea(false);

		//20210602_ASBY:Check_DieCardAction() 위에있다 아래로 옮김, Jump스킬 쓰고 난 뒤 첫줄이 콜라이더가 안켜져서
		// 타겟 설정에서 꺼져있으므로 켜준다.
		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			if (item == null) continue;
			StageCardInfo info = item.m_Info;
			// 피난민은 저격 불가
			item.Action(EItem_Stage_Card_Action.Scale, 0f, (obj) => { obj.TW_ScaleBumping(true); });
		}

		float movetime = 0.3f;
		// 선택된 카드와 중앙카드의 위치 변경
		//if (m_CenterChar != Char) {
			iTween.MoveTo(Char.gameObject, iTween.Hash("position", m_CharsPos[Char.m_Pos], "time", movetime, "easetype", "easeOutCubic", "IsLocal", true));
			iTween.ScaleTo(Char.gameObject, iTween.Hash("scale", m_CharsPosScale, "time", movetime, "easetype", "easeOutCubic", "IsLocal", true));
		//}
		for (int i = 0; i < m_Chars.Length; i++) {
			m_Chars[i].SetSelectFX(false);
			//if (Char.m_Pos == i && !_selectchar) continue;
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, null, movetime);
		}

		m_MainUI.StartPlayAni(Main_Stage.AniName.In);
		if (_zoom) {
			iTween.MoveTo(m_Stage.StageObjPanel.gameObject, iTween.Hash("z", 0, "time", movetime, "easetype", "easeOutCubic"));
			GameObject Activepanel = m_Stage.ActionPanel.gameObject;
			iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
			SetBGFXSort(true);
			yield return new WaitWhile(() => Utile_Class.IsPlayiTween(m_Stage.StageObjPanel.gameObject));
		}
		else yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Char.gameObject));
		Char.GetComponent<SortingGroup>().sortingOrder = 6;

		yield return new WaitWhile(() => POPUP.IS_PopupUI());

		Char.SetSkillInfoActive(true); 
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++)
			STAGE.m_Chars[i].SetSkillCoolTimeUI();

		if (m_CorSkill != null) StopCoroutine(m_CorSkill);
		m_CorSkill = null;
		m_SkillChar = null;
	}

	/// <summary> 취소 가능 스킬 체크</summary>
	bool IS_CanSkillCancle(SkillKind _kind) {
		switch (_kind) {
			case SkillKind.HeadShot:
			case SkillKind.CardPull:
			case SkillKind.BackStep:
			case SkillKind.Explosion:
			case SkillKind.TransverseAtk:
			case SkillKind.BlowAtk:
			case SkillKind.StopCard:
			case SkillKind.SpotLight:
			case SkillKind.StopCardTran:
			case SkillKind.RemoveFire:
			case SkillKind.ChangeCardArea:
			case SkillKind.RangeAtk:
			case SkillKind.GetCards:
			case SkillKind.PlusMate:
				return true;

			default: return false;
		}
	}

	void DarkPatrolCheck(bool _add = false) {
		if (m_DarkPatrolCnt < 1) return;
		if(!_add) m_DarkPatrolCnt--;
		for (int j = 0, Start = 0, End = Start + 3; j <= AI_MAXLINE; j++, End += 2) {
			for (int i = Start; i < End; i++) {
				Item_Stage item = m_ViewCard[j][i];
				if (item == null) continue;
				if (!item.m_Info.IS_DmgTarget()) continue;
				if (m_DarkPatrolCnt < 1)
					item.SetDarkPatrolMark(false);
				else {
					item.SetDarkPatrolMark(!item.m_IsLight);
				}
			}
		}
	}

	void HideCheck() {
		if (!STAGE_USERINFO.ISBuff(StageCardType.Hide)) return;
		float val = STAGE_USERINFO.m_BuffValues[StageCardType.Hide];
		float val2 = Mathf.Max(0, val - 1);
		STAGE_USERINFO.m_BuffValues[StageCardType.Hide] = val2;

		if (val > val2) {
			int spcidx = 0;
			switch (val2) {
				case 0:
					spcidx = 5007003;
					EndEff("Effect/Stage/Eff_ChSkill_Hide");
					break;
				case 1: spcidx = 5007002; break;
				case 2: spcidx = 5007001; break;

			}
			for (int i = 0; i < m_Chars.Length; i++) {
				if (m_Chars[i].m_Info == null) continue;
				if (m_Chars[i].m_Info.m_Idx == 1022 && spcidx != 0) {
					STAGE_USERINFO.SetSpeech(TDATA.GetString(ToolData.StringTalbe.Dialog, spcidx), m_Chars[i].transform, 1.5f);
					break;
				}
			}
		}
	}

	/// <summary>
	/// 확률체크하는거랑 알람 연출 따로 돌려야하는건 미포함했음, Hit Dodge 계열
	/// </summary>
	/// <param name="_char"></param>
	/// <param name="_type"></param>
	/// <param name="_vals"></param>
	/// <returns></returns>
	float DNACheck(Item_Stage_Char _char, OptionType _type, object[] _vals = null) {
		var dna = _char.m_Info.GetDNABuff(_type);
		switch (_type) {
			case OptionType.AttackingMaterialAdd:
				if (dna > 0) {
					AddMaterial((StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat), 1);
					DNAAlarm(_char.m_Info, OptionType.AttackingMaterialAdd);
				}
				break;
			case OptionType.HitMaterialAdd:
				if (dna > 0) {
					AddMaterial((StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat), 1);
					DNAAlarm(_char.m_Info, OptionType.HitMaterialAdd);
				}
				break;
			case OptionType.Vampire:
				if (dna > 0) DNAAlarm(_char.m_Info, OptionType.Vampire);
				StartCoroutine(AddStat_Action(_char.transform, StatType.HP, Mathf.RoundToInt((int)_vals[0] * dna)));
				break;
			case OptionType.AttackingCoolDown:
				if (dna > 0) {
					DNAAlarm(_char.m_Info, OptionType.AttackingCoolDown);
					_char.CheckSkillCoolTime(_char.m_Info.m_Skill[0].m_TData.m_Cool);
				}
				break;
			case OptionType.HitCoolDown:
				if (dna > 0) {
					DNAAlarm(_char.m_Info, OptionType.HitCoolDown);
					_char.CheckSkillCoolTime(_char.m_Info.m_Skill[0].m_TData.m_Cool);
				}
				break;
			case OptionType.CureHealAdd:
				if (dna > 0) DNAAlarm(_char.m_Info, OptionType.CureHealAdd);
				float AddHP = Mathf.RoundToInt(STAGE_USERINFO.GetCharStat(_char.m_Info, StatType.Heal) * (float)_vals[0] * (1f + dna));
				return Mathf.RoundToInt(AddHP);
			case OptionType.CureApAdd:
				if (dna > 0) DNAAlarm(_char.m_Info, OptionType.CureApAdd);
				int preap = m_User.m_AP[0];
				m_User.m_AP[0] = Math.Min(Mathf.RoundToInt(m_User.m_AP[0] + m_User.m_AP[1] * dna), m_User.m_AP[1]);
				for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
					m_Chars[i].SetAPUI(m_User.m_AP[0]);
				}
				DLGTINFO?.f_RfAPUI?.Invoke(m_User.m_AP[0], preap, m_User.m_AP[1]);
				break;
			case OptionType.AttackingDmgAdd:
				if (dna > 0) DNAAlarm(_char.m_Info, OptionType.AttackingDmgAdd);
				break;
			case OptionType.AttackingDefDown:
				if (dna > 0) DNAAlarm(_char.m_Info, OptionType.AttackingDefDown);
				break;
			case OptionType.KilltoHeal:
				if (dna > 0f) {
					DNAAlarm(_char.m_Info, OptionType.KilltoHeal);
					for (int i = (int)StatType.Men; i < (int)StatType.SurvEnd; i++) StartCoroutine(AddStat_Action(m_CenterChar.transform, (StatType)i, Mathf.RoundToInt(STAGE_USERINFO.GetMaxStat((StatType)i) * dna)));
				}
				break;
			case OptionType.KilltoCoolDown:
				if (dna > 0f) {
					DNAAlarm(_char.m_Info, OptionType.KilltoCoolDown);
					for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
						m_Chars[i].CheckSkillCoolTime(Mathf.RoundToInt(m_Chars[i].m_SkillCoolTime * dna));
					}
				}
				break;
			case OptionType.HitThorn:
				if (dna > 0) DNAAlarm(_char.m_Info, OptionType.HitThorn);
				break;
		}
		return dna;
	}

	void SetAtkDNASynergy(Item_Stage_Char _char, object[] _vals = null) {
		//흡혈
		DNACheck(_char, OptionType.Vampire, _vals);
		//랜덤 재료 획득
		DNACheck(_char, OptionType.AttackingMaterialAdd);
		//시너지 랜덤 재료 획득
		float? synergyPP0 = m_User.GetSynergeValue(JobType.Pickpocket, 0);
		float? synergyPP1 = m_User.GetSynergeValue(JobType.Pickpocket, 1);
		if (synergyPP0 != null && synergyPP1 != null) {
			if (UTILE.Get_Random(0f, 1f) < synergyPP1 && Mathf.RoundToInt((float)synergyPP0) > 0) {
				AddMaterial((StageMaterialType)UTILE.Get_Random(0, (int)StageMaterialType.DefaultMat), Mathf.RoundToInt((float)synergyPP0));
				STAGE_USERINFO.ActivateSynergy(JobType.Pickpocket);
			}
		}
	}
}
