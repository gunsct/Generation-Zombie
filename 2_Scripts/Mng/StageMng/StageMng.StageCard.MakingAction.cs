using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Card Making Process 필요 함수
	public void StartStageMakingCardAction(int cardidx, Action _cb)
	{
		m_StageAction = MakingAction_Proc(cardidx, _cb);
		StartCoroutine(m_StageAction);
	}
	Item_Stage MakingAction_Proc_CardCreate(int cardidx, Action EndCB = null)
	{
		float actiontime = 0.2f;//0.3->0.2
		Item_Stage card = CreateStageCard(-1, 0, new Vector3(0, -8.72f, 0), cardidx, m_Stage.ActionPanel.transform);
		m_SelectVirture = card;
		card.transform.localScale = BaseValue.STAGE_SELECT_LINE_SCALE;
		card.GetComponent<SortingGroup>().sortingOrder = 4;
		card.ActiveDark(false);
		card.Action(EItem_Stage_Card_Action.FadeIn, 0f, (obj) => { EndCB?.Invoke(); }, actiontime);
		for (int i = 0; i < m_Chars.Length; i++) m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, null, actiontime);
		for (int i = 0; i < m_ViewCard[0].Length; i++) m_ViewCard[0][i].SetAccent(false);
		//for (int i = 0; i < m_ViewCard[0].Length; i++) m_ViewCard[0][i].Action(EItem_Stage_Card_Action.TargetOff);

		return card;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Card Making Process Action

	IEnumerator MakingAction_Proc(int cardidx, Action _cb) {
		FirstLineBump(false);
		m_MainUI.SetPathLine();
		TStageMakingTable table = TDATA.GetStageMakingData(cardidx);
		TStageCardTable tdata = TDATA.GetStageCardTable(cardidx);
		Action cb = () => {
			_cb?.Invoke();
			m_Check.Check(StageCheckType.CardUse, (int)tdata.m_Type);
		};
		switch (tdata.m_Type)
		{
		/// <summary> 저격 가능한 타겟들이 포커싱되고 해당 타겟을 선택하면 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
		case StageCardType.Sniping:
			yield return MakingAction_Proc_Sniping(cardidx, cb);
			break;
		/// <summary> 목표 카드 선택 시 해당 카드를 기점으로 3x3 범위에 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
		case StageCardType.Grenade:
		case StageCardType.TimeBomb:
			yield return MakingAction_Proc_Grenade(cardidx, cb);
			break;
		/// <summary> 목표 카드 선택 시 해당 카드가 포함된 행 전부를 공격한다. [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
		case StageCardType.Dynamite:
			yield return MakingAction_Proc_Dynamite(cardidx, cb);
			break;
		/// <summary> 5x5의 범위 내에 랜덤으로 10회 공격. [파티 공격력] * [지정된 비율] 공격력 </summary>
		case StageCardType.MachineGun:
			yield return MakingAction_Proc_MachineGun(cardidx, cb);
			break;
		/// <summary> HP 회복 </summary>
		case StageCardType.RecoveryHp:
			yield return InstantAction(cardidx, cb, AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * tdata.m_Value1)));
			//yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(m_User.GetMaxStat(StatType.HP) * tdata.m_Value1));
			break;
		case StageCardType.RecoveryHpPer:
			yield return InstantAction(cardidx, cb, AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(m_User.GetStat(StatType.Heal) * tdata.m_Value1)));
			//yield return AddStat_Action(m_CenterChar.transform, StatType.HP, Mathf.RoundToInt(m_User.GetStat(StatType.Heal) * tdata.m_Value1));
			break;
		/// <summary> 포만감 회복 </summary>
		case StageCardType.RecoverySat:
			yield return InstantAction(cardidx, cb, AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(tdata.m_Value1)));
			//value = tdata.m_Value1;
			//yield return AddStat_Action(m_CenterChar.transform, StatType.Sat, Mathf.RoundToInt(value));
			break;
		/// <summary> 정신력 회복 </summary>
		case StageCardType.RecoveryMen:
			yield return InstantAction(cardidx, cb, AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(tdata.m_Value1)));
			//value = tdata.m_Value1;
			//yield return AddStat_Action(m_CenterChar.transform, StatType.Men, Mathf.RoundToInt(value));
			break;
		/// <summary> 청결도 회복 </summary>
		case StageCardType.RecoveryHyg:
			yield return InstantAction(cardidx, cb, AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(tdata.m_Value1)));
			//value = tdata.m_Value1;
			//yield return AddStat_Action(m_CenterChar.transform, StatType.Hyg, Mathf.RoundToInt(value));
			break;
		/// <summary> 행동력 회복 </summary>
		case StageCardType.RecoveryAP:
			yield return InstantAction(cardidx, cb, SelectAction_RecoveryAP(tdata.m_Value1));
			//yield return SelectAction_RecoveryAP(tdata.m_Value1);
			break;
		/// <summary> HP 회복 </summary>
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
			yield return InstantAction(cardidx, cb, () => { SetBuff(EStageBuffKind.Stage, cardidx); });
			//SetBuff(EStageBuffKind.Stage, cardidx);
			break;
		/// <summary> 방어 횟수 </summary>
		case StageCardType.AddGuard:
			yield return InstantAction(cardidx, cb, SelectAction_AddGuard(tdata.m_Value1));
			//yield return SelectAction_AddGuard(tdata.m_Value1);
			break;
		/// <summary> 5x5 범위 내 모든 카드의 위치를 랜덤하게 다시 배치한다. </summary>
		case StageCardType.AllShuffle:
			yield return MakingAction_Proc_AllShuffle(cardidx, cb);
			break;
		/// <summary> 5x5 범위 내의 모든 카드를 다시 뽑는다. </ summary>
		case StageCardType.AllConversion:
			yield return MakingAction_Proc_AllConversion(cardidx, cb);
			break;
		case StageCardType.LightStick:
			yield return MakingAction_Proc_LightStick(cardidx, cb);
			break;
		case StageCardType.FlashLight:
			yield return MakingAction_Proc_FlashLight(cardidx, cb);
			break;
		case StageCardType.PowderExtin:
			yield return MakingAction_Proc_PowderExtin(cardidx, cb);
			break;
		case StageCardType.ThrowExtin:
			yield return MakingAction_Proc_ThrowExtin(cardidx, cb);
			break;
		case StageCardType.SmokeBomb:
			yield return MakingAction_Proc_SmokeBomb(cardidx, cb);
			break;
		case StageCardType.Paralysis:
			yield return MakingAction_Proc_Paralysis(cardidx, cb);
			break;
		case StageCardType.ShockBomb:
			yield return MakingAction_Proc_ShockBomb(cardidx, cb);
			break;
		case StageCardType.C4:
			yield return MakingAction_Proc_C4(cardidx, cb);
			break;
		case StageCardType.Shotgun:
			yield return MakingAction_Proc_Shotgun(cardidx, cb);
			break;
		case StageCardType.AirStrike:
			yield return MakingAction_Proc_AirStrike(cardidx, cb);
			break;
		case StageCardType.StarShell:
			yield return MakingAction_Proc_StarShell(cardidx, cb);
			break;
		case StageCardType.PowderBomb:
			yield return MakingAction_Proc_PowderBomb(cardidx, cb);
			break;
		case StageCardType.FireBomb:
			yield return MakingAction_Proc_FireBomb(cardidx, cb);
			break;
		case StageCardType.FireGun:
			yield return MakingAction_Proc_FireGun(cardidx, cb);
			break;
		case StageCardType.NapalmBomb:
			yield return MakingAction_Proc_NapalmBomb(cardidx, cb);
			break;
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		// 성공 실패 체크
		if (CheckEnd() && STAGEINFO.m_Result == StageResult.Clear) yield break;

		//첫줄 선택시 방향 표시
		m_MainUI.SetPathLine(m_ViewCard[0][0], m_ViewCard[0][1], m_ViewCard[0][2]);
		FirstLineBump(true);
		m_StageAction = null;
	}
	IEnumerator InstantAction(int _cardidx, Action _cb, IEnumerator _ie) {
		bool isAction = true;
		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			item.Action(EItem_Stage_Card_Action.TargetOff);
		}
		Item_Stage card = MakingAction_Proc_CardCreate(_cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		m_ActionCor = _ie;
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
			else {
				card.ActiveCardUse(false);
				StartCoroutine(m_ActionCor);
				m_ActionCor = null;
				_cb?.Invoke();
			}
		}, true, 0);

		yield return new WaitWhile(() => m_ActionCor != null);
		//yield return action;

		// 종료 연출
		yield return MakingAction_Proc_End(card);
		if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.Delay_Mdeicine)) TUTO.Next();
	}
	IEnumerator InstantAction(int _cardidx, Action _cb, Action _cb2) {
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(_cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		isAction = true;
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 0) {
				card.ActiveCardUse(false);
				_cb?.Invoke();
				_cb2?.Invoke(); 
				isAction = false;
			}
			else {
				isAction = false;
			}
		}, true);

		yield return new WaitWhile(() => isAction);
		//yield return action;

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Base Making Action
	IEnumerator MakingAction_Proc_CardRemove(Item_Stage card)
	{
		if (card == null) yield break;
		bool isAction = true;
		card.ActiveCardUse(false);
		card.Action(EItem_Stage_Card_Action.FadeOut, 0, (obj) =>
		{
			isAction = false;
			Destroy(card.gameObject);
			//RemoveStage(obj);
		});

		if(m_SelectStage == null)
		for (int i = 0; i < m_ViewCard[0].Length; i++)
		{
			Item_Stage item = m_ViewCard[0][i];
			if (item == null) continue;
			item.Action(EItem_Stage_Card_Action.Scale);
		}
		yield return new WaitWhile(() => isAction);
	}
	IEnumerator MakingAction_Proc_Start(Item_Stage card, Action<int> _cb = null, bool _cancle = false, int _panelpos = 3, Action _anicb = null)
	{
		float movetime = 0.2f;//0.3->0.2
		GameObject Activepanel = m_Stage.ActionPanel.gameObject;

		iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[_panelpos], "time", movetime, "easetype", "easeOutQuad"));
		iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[_panelpos], "time", movetime, "easetype", "easeOutQuad"));
		SetBGFXSort(false);

		List<Item_Stage_Char> actionChar = new List<Item_Stage_Char>();
		for (int i = 0; i < m_Chars.Length; i++)
		{
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeOut, 0f, (obj) => { actionChar.Remove(obj); }, movetime);
			actionChar.Add(m_Chars[i]);
		}

		switch (card.m_Info.m_TData.m_Type)
		{
			/// <summary> 저격 가능한 타겟들이 포커싱되고 해당 타겟을 선택하면 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Sniping:
			/// <summary> 목표 카드 선택 시 해당 카드를 기점으로 3x3 범위에 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
			case StageCardType.Grenade:
			/// <summary> 십자 범위에 공격. 데미지 효과는 수류탄과 동일 [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.FireBomb:
			/// <summary> 십자 범위에 공격. 데미지 효과는 수류탄과 동일 [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.FireGun:
			/// <summary> 십자 범위에 공격. 데미지 효과는 수류탄과 동일 [파티 공격력] * [지정된 비율] 공격력 </summary>
			case StageCardType.NapalmBomb:
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
			case StageCardType.Drill:
			case StageCardType.RandomAtk:
			case StageCardType.CardPull:
			case StageCardType.Explosion:
			case StageCardType.StopCard:
			case StageCardType.PlusMate:
			case StageCardType.DownLevel:
				m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
				m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse, (res, obj) => {
					LockCamScroll = false;
				}, card.m_Info.GetName(), card.m_Info.m_TData.GetInfo(), null, BaseValue.GetAreaIcon(card.m_Info.m_TData.m_Type), false, -1, -1, true, _cancle, _cb).GetComponent<Stage_CardUse>();
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
				m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
				m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse,  (res, obj)=> {
					LockCamScroll = false;
				}, card.m_Info.GetName(), card.m_Info.m_TData.GetInfo(), null, BaseValue.GetAreaIcon(card.m_Info.m_TData.m_Type), false, -1, -1, false, _cancle, _cb).GetComponent<Stage_CardUse>();
				LockCamScroll = true;
				break;
			default:
				m_MainUI.StartPlayAni(Main_Stage.AniName.Out);
				m_SkillUseInfoPopup = POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_CardUse, (res, obj)=> {
					LockCamScroll = false;
				}, card.m_Info.GetName(), card.m_Info.m_TData.GetInfo(), null, BaseValue.GetAreaIcon(card.m_Info.m_TData.m_Type), false, -1, -1, false, _cancle, _cb).GetComponent<Stage_CardUse>();
				LockCamScroll = true;
				break;
		}

		yield return new WaitWhile(() => actionChar.Count > 0);
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Activepanel));
		_anicb?.Invoke();
	}
	IEnumerator MakingAction_Proc_End(Item_Stage card = null)
	{
		// 제거 연출
		yield return MakingAction_Proc_CardRemove(card);
		float movetime = 0.2f;//0.3->0.2
		GameObject Activepanel = m_Stage.ActionPanel.gameObject;

		ShowArea(false);
		AutoCamPosInit = true;

		// 카드 사용으로인해 죽은 카드들 연출
		yield return Check_DieCardAction();
		if (m_SkillUseInfoPopup != null)
		{
			m_MainUI.StartPlayAni(Main_Stage.AniName.In);
			// 팝업 닫아주기
			m_SkillUseInfoPopup.Close();
			m_SkillUseInfoPopup = null;

		}

		iTween.MoveTo(Activepanel, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
		iTween.MoveTo(m_LowEffPanel.gameObject, iTween.Hash("position", m_ActivePanelPos[0], "time", movetime, "easetype", "easeOutQuad"));
		SetBGFXSort(true);

		List<Item_Stage_Char> actionChar = new List<Item_Stage_Char>();
		for (int i = 0; i < m_Chars.Length; i++)
		{
			m_Chars[i].Action(EItem_Stage_Char_Action.FadeIn, 0f, (obj) => { actionChar.Remove(obj); }, movetime);
			actionChar.Add(m_Chars[i]);
		}

		yield return new WaitWhile(() => actionChar.Count > 0);

		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(Activepanel));


		SetView_Area_Arc(card, AI_MAXLINE, false);

		// 여기까지오면 팝업이 없어야되는데 SkillUsePopup이 연출때문에 나중에 닫히는 현상이 있어 여기서 대기해줌
		// 제거시 튜토리얼이 진행 안되는 현상 발생
		yield return new WaitWhile(() => POPUP.IS_PopupUI());

		List<Item_Stage> ActiveCards = new List<Item_Stage>();
		for (int i = 0; i < 3; i++) {
			Item_Stage item = m_ViewCard[0][i];
			ActiveCards.Add(item);
			item.Action(EItem_Stage_Card_Action.Scale, 0f, (obj) => {
				//obj.m_Target = null;//타겟있으면 터치가 안되?
				ActiveCards.Remove(obj);
				obj.TW_ScaleBumping(true);
			});
		}

		yield return new WaitWhile(() => ActiveCards.Count > 0);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Making Action
	IEnumerator MakingAction_Proc_Sniping(int cardidx, Action _cb) {
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_Sniping(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res)=> {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);

		if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Sniping_Action_Start)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Sniping_Action_Start)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.Sniping_Action_Start)) TUTO.Next();
		// 저격 연출
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return action;

		// 종료 연출
		yield return MakingAction_Proc_End(card);

		if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.Sniping_Action_End)) TUTO.Next();
		//else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.Sniping_Action_End)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.Sniping_Action_End)) TUTO.Next();
	}
	IEnumerator MakingAction_Proc_Grenade(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_Grenade(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		// 시작 연출
		//yield return MakingAction_Proc_Start(card);

		// 수류탄 연출
		//yield return Action_Grenade(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	
	IEnumerator MakingAction_Proc_Dynamite(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_Dynamite(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		// 저격 연출
		//yield return Action_Dynamite(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_MachineGun(int cardidx, Action _cb)
	{
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);
		int cancle = -1;
		yield return MakingAction_Proc_Start(card, (res) => {
			cancle = res;
		}, true, 3, ()=> {
			if (!TUTO.IsTutoPlay()) {
				List<Item_Stage> AreaCards = new List<Item_Stage>();
				for (int j = 0, jMax = j + 5, Start = card.m_Pos - 1; j < jMax; j++, Start++) {
					if (j > AI_MAXLINE) break;
					int End = Math.Min(Start + 5, m_ViewCard[j].Length);
					for (int i = Start; i < End; i++) {
						if (i < 0) continue;
						Item_Stage item = m_ViewCard[j][i];
						if (item == null) continue;
						if (item.IS_Die()) continue;
						StageCardInfo info = item.m_Info;
						AreaCards.Add(item);
					}
				}
				ShowArea(true, AreaCards);
			}
		});

		yield return new WaitWhile(() => cancle < 0);

		if (cancle == 0) {
			_cb?.Invoke();
			card.ActiveCardUse(false);
			yield return Action_MachineGun(card, 0);
		}
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_MachineGun(card, 0);
		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_AllShuffle(int cardidx, Action _cb)
	{
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		int cancle = -1;
		yield return MakingAction_Proc_Start(card, (res) => {
			cancle = res;
		}, true, 3, ()=> {
			if (!TUTO.IsTutoPlay()) {
				List<Item_Stage> AreaCards = new List<Item_Stage>();
				for (int j = 0, jMax = j + 5, Start = card.m_Pos - 1; j <= jMax; j++, Start++) {
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
					}
				}
				ShowArea(true, AreaCards);
			}
		});


		yield return new WaitWhile(() => cancle < 0);

		if (cancle == 0) {
			_cb?.Invoke();
			card.ActiveCardUse(false);
			yield return Action_AllShuffle(card, 0);
		}

		//yield return MakingAction_Proc_Start(card);

		//yield return Action_AllShuffle(card, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_AllConversion(int cardidx, Action _cb)
	{
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		int cancle = -1;
		yield return MakingAction_Proc_Start(card, (res) => {
			cancle = res;
		}, true, 3, ()=> {
			if (!TUTO.IsTutoPlay()) {
				List<Item_Stage> AreaCards = new List<Item_Stage>();
				for (int j = 0, jMax = j + 5, Start = card.m_Pos - 1; j <= jMax; j++, Start++) {
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
					}
				}
				ShowArea(true, AreaCards);
			}
		});


		yield return new WaitWhile(() => cancle < 0);

		if (cancle == 0) {
			_cb?.Invoke();
			card.ActiveCardUse(false);
			yield return Action_AllConversion(card, 0);
		}

		//yield return MakingAction_Proc_Start(card);

		//yield return Action_AllConversion(card, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_LightStick(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_LightStick(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();

		//yield return MakingAction_Proc_Start(card);

		//// 저격 연출
		//yield return Action_LightStick(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_FlashLight(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_FlashLight(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);

		if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Light_Action_Start)) TUTO.Next();

		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);


		// 선택이 무조건 1번째 줄이어야함
		//yield return Action_FlashLight(card, activecards);

		// 종료 연출
		yield return MakingAction_Proc_End(card);

		if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.Light_Action_End)) TUTO.Next();
	}
	IEnumerator MakingAction_Proc_PowderExtin(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_PowderExtin(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_PowderExtin(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_ThrowExtin(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_ThrowExtin(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_ThrowExtin(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_SmokeBomb(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_SmokeBomb(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_SmokeBomb(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_Paralysis(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_Paralysis(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_Paralysis(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}

	IEnumerator MakingAction_Proc_ShockBomb(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_ShockBomb(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.ShockBoom_Action_Start)) TUTO.Next();
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);


		//yield return Action_ShockBomb(card, activecards, 0);

		yield return MakingAction_Proc_End(card);

		if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.ShockBomb_Action_End)) TUTO.Next();
	}

	IEnumerator MakingAction_Proc_C4(int cardidx, Action _cb)
	{
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		int cancle = -1;
		yield return MakingAction_Proc_Start(card, (res) => {
			cancle = res;
		}, true, 3, ()=> {
			if (!TUTO.IsTutoPlay()) {
				List<Item_Stage> AreaCards = new List<Item_Stage>();
				List<Item_Stage> TargetCards = new List<Item_Stage>();
				BoomAreaTarget(card, card, out AreaCards, out TargetCards, 0);
				for (int i = TargetCards.Count - 1; i > -1; i--) {
					Item_Stage item = TargetCards[i];
					if (item.IS_Die()) continue;
				}
				ShowArea(true, AreaCards);
			}
		});

		yield return new WaitWhile(() => cancle < 0);

		if (cancle == 0) {
			_cb?.Invoke();
			card.ActiveCardUse(false);
			yield return Action_C4(card, 0);
		}
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_C4(card, 0);

		yield return MakingAction_Proc_End(card);
	}

	IEnumerator MakingAction_Proc_Shotgun(int cardidx, Action _cb)
	{
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		//yield return MakingAction_Proc_Start(card);

		int cancle = -1;
		yield return MakingAction_Proc_Start(card, (res) => {
			cancle = res;
		}, true, 3, ()=> {
			if (!TUTO.IsTutoPlay()) {
				List<Item_Stage> AreaCards = new List<Item_Stage>();
				int Start = card.m_Pos > 0 && !m_IS_KillFirstLine ? card.m_Pos : (card.m_Line > -1 ? card.m_Pos - 1 : 0);
				for (int j = 0, jMax = j + 2; j < jMax; j++, Start++)//샷건은 첫라인 맨 왼쪽부터
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
					}
				}
				ShowArea(true, AreaCards);
			}
		});

		yield return new WaitWhile(() => cancle < 0);

		if (cancle == 0) {
			_cb?.Invoke();
			card.ActiveCardUse(false);
			yield return Action_Shotgun(card, 0);
		}
		//yield return Action_Shotgun(card, 0);
		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_AirStrike(int cardidx, Action _cb)
	{
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		card.ActiveCardUse(true);
		yield return new WaitWhile(() => isAction);

		int cancle = -1;
		yield return MakingAction_Proc_Start(card, (res) => {
			cancle = res;
		}, true, 3, ()=> {
			if (!TUTO.IsTutoPlay()) {
				ShowArea(true, activecards);
			}
		});

		yield return new WaitWhile(() => cancle < 0);

		if (cancle == 0) {
			_cb?.Invoke();
			card.ActiveCardUse(false);
			yield return Action_AirStrike(card, activecards, 0);
		}
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_AirStrike(card, activecards, 0);
		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_StarShell(int cardidx, Action _cb) {
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_StarShell(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//yield return Action_StarShell(card, activecards, 0);

		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_PowderBomb(int cardidx, Action _cb) {
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;

		}
		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_PowderBomb(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if (use) _cb?.Invoke();

		//yield return MakingAction_Proc_Start(card);

		//yield return Action_PowderBomb(card, activecards, 0);

		yield return MakingAction_Proc_End(card);
	}

	IEnumerator MakingAction_Proc_FireBomb(int cardidx, Action _cb) {
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_FireBomb(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if(use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//// 저격 연출
		//yield return Action_FireBomb(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_FireGun(int cardidx, Action _cb) {
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_FireGun(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if(use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//// 저격 연출
		//yield return Action_FireGun(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
	IEnumerator MakingAction_Proc_NapalmBomb(int cardidx, Action _cb) {
		//가능여부 체크
		Item_Stage vircard = new Item_Stage();
		m_SelectVirture = vircard;
		vircard.m_Line = -1;
		vircard.m_Pos = 0;
		vircard.SetData(new StageCardInfo(cardidx));
		List<Item_Stage> activecards = SetView_Area_Target(vircard, AI_MAXLINE, 0);
		if (activecards.Count < 1) {
			STAGE_USERINFO.CharSpeech(DialogueConditionType.UnUseItem);
			yield return MakingAction_Proc_End();
			yield break;
		}

		// 생성 연출
		bool isAction = true;
		Item_Stage card = MakingAction_Proc_CardCreate(cardidx, () => { isAction = false; });
		yield return new WaitWhile(() => isAction);

		bool use = true;
		m_ActionCor = Action_NapalmBomb(card, activecards, 0);
		yield return MakingAction_Proc_Start(card, (res) => {
			if (res == 1) {
				use = false;
				StopCoroutine(m_ActionCor);
				m_ActionCor = null;
			}
		}, true);
		StartCoroutine(m_ActionCor);
		yield return new WaitWhile(() => m_ActionCor != null);
		if(use) _cb?.Invoke();
		//yield return MakingAction_Proc_Start(card);

		//// 저격 연출
		//yield return Action_NapalmBomb(card, activecards, 0);

		// 종료 연출
		yield return MakingAction_Proc_End(card);
	}
}
