using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	[SerializeField] Animator m_StateAnimator;
	IEnumerator StartAction()
	{
		if (!TUTO.IsTutoPlay() && STAGEINFO.m_PlayType == StagePlayType.Stage) TUTO.Start(TutoStartPos.Stage);

		POPUP.GetWorldUIPanel().gameObject.SetActive(true);
		m_AutoPlay = false;
		//3인 세팅일때 중앙 정렬
		int deckcharcnt = USERINFO.m_PlayDeck.GetDeckCharCnt();
		//if (deckcharcnt == 3) m_CharsPos = m_3CharsPos;

		for (int i = 0; i < m_Chars.Length; i++)
		{
			iTween.Stop(m_Chars[i].gameObject);
			m_Chars[i].transform.localPosition = m_CharsPos[i];
			m_Chars[i].transform.localScale = m_CharsPosScale;
		}

		for (int i = 0; i < BaseValue.STAGE_LINE; i++)
		{
			if (m_ViewCard[i] == null)
			{
				int cnt = 3 + i * 2;
				m_ViewCard[i] = new Item_Stage[cnt];
				m_VirtualCopyCard[i] = new Item_Stage[cnt];
			}
			else
			{
				for (int j = 0; j < m_ViewCard[i].Length; j++)
				{
					if (m_ViewCard[i][j] != null)
					{
						RemoveStage(m_ViewCard[i][j]);
						m_ViewCard[i][j] = null;
					}
				}
			}
		}

		AutoCamPosInit = true;

		Init();

		bool PopupCloase = false;
		Action<int, GameObject> StartPopupEndCB = (result, obj) => {
			PopupCloase = true;
		};
		m_MainUI = POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Stage, null, StartPopupEndCB).GetComponent<Main_Stage>();
		m_Stage.ActionPanel.GetComponent<RenderAlpha_Controller>().Alpha = 0f;
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) m_Chars[i].SetAlpha(0f);

		m_StateAnimator.enabled = false;
		m_StateAnimator.enabled = true;
		m_StateAnimator.SetTrigger("Start");
		double time = UTILE.Get_Time();
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_StateAnimator));
		time = UTILE.Get_Time() - time;
		m_StateAnimator.enabled = false;
		yield return new WaitWhile(() => !PopupCloase);
		
		if (STAGEINFO.m_PlayType == StagePlayType.Stage && USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount < 2 && !STAGEINFO.IS_ReplayStg) {
			List<TStageMakingTable> list = TDATA.GetStageMakingList().FindAll(t => t.m_Condition.m_Type == StageMakingConditionType.Stage && t.m_Condition.m_Value == STAGEINFO.m_Idx);
			for (int i = 0; i < list.Count; i++) {
				PopupCloase = false;
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Making_New, (result, obj) => {
					PopupCloase = true;
				}, list[i]);
				yield return new WaitWhile(() => !PopupCloase);
			}
		}
		//다이얼로그 선택지 보상 적용,팝업 여기에 작업함녀됨
		if (STAGEINFO.m_PlayType == StagePlayType.Stage && !STAGEINFO.IS_ReplayStg) {
			SetDialogReward();
		}
		//버프 디버프 있으니 여기서 갱신 한번 더
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++)
		{
			m_Chars[i].SetAlpha(1f);
			m_Chars[i].SetSkillAlarmAlpha(0f);
			m_Chars[i].SetAPUI(m_User.m_AP[0]);
			m_Chars[i].SetSkillCoolTimeUI();
		}

		List<Item_Stage> actioncards = new List<Item_Stage>();
		for (int i = m_ViewCard[0].Length - 1; i > -1; i--) {
			Item_Stage card = m_ViewCard[0][i];
			actioncards.Add(card);
			m_ViewCard[0][i].Action(EItem_Stage_Card_Action.Scale, 0f, (obj) => {
				actioncards.Remove(obj);
				obj.TW_ScaleBumping(true);
			});
		}
		yield return new WaitWhile(() => actioncards.Count > 0);
		yield return Check_NullCardAction(true);

		//디버프에 KnockDownChar있으면 N명 캐릭터 액티브 스킬 사용 불가
		if (STAGE_USERINFO.ISBuff(StageCardType.ConKnockDownChar)) {
			int bancnt = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConKnockDownChar));
			bancnt = Mathf.Min(bancnt, USERINFO.m_PlayDeck.GetDeckCharCnt() - m_BanChars.Count);
			List<int> pos = new List<int>();
			for(int i =0;i< USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
				if (m_BanChars.Contains(m_Chars[i])) continue;
				pos.Add(i);
			}
			for (int i = 0; i < bancnt; i++) {
				int rand = UTILE.Get_Random(0, pos.Count);
				int select = pos[rand];
				m_Chars[select].BanChar(true);
				m_Chars[select].SetAPUI(0);
				m_BanChars.Add(m_Chars[select]);
				pos.RemoveAt(rand);
			}
		}

		yield return StageCardLock();
		//가로등 모드는 스테이지 끝날때까지 지속
		TStageTable.StagePlayType mode = STAGEINFO.m_TStage.GetMode(PlayType.StreetLight);
		if (mode != null) {
			yield return Mode_StreetLight(mode);
			yield return Check_LightOnOff();
		}
		yield return Check_NullCardAction(true);

		if (TUTO.IsTuto(TutoKind.Stage_101, (int)TutoType_Stage_101.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_102, (int)TutoType_Stage_102.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_103, (int)TutoType_Stage_103.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_104, (int)TutoType_Stage_104.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_204, (int)TutoType_Stage_204.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_206, (int)TutoType_Stage_206.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_301, (int)TutoType_Stage_301.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_304, (int)TutoType_Stage_304.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_701, (int)TutoType_Stage_701.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_401, (int)TutoType_Stage_401.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_403, (int)TutoType_Stage_403.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_501, (int)TutoType_Stage_501.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_601, (int)TutoType_Stage_601.StageStart_Loading)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.Stage_801, (int)TutoType_Stage_801.StageStart_Loading)) TUTO.Next();

		yield return new WaitWhile(() => POPUP.IS_PopupUI());
		//첫줄 선택시 방향 표시
		m_MainUI.SetPathLine(m_ViewCard[0][0], m_ViewCard[0][1], m_ViewCard[0][2]);
		FirstLineBump(true);

		IS_PassTimer = true;
		m_StageAction = null;
	}

	/// <summary> 실패 상태에서 이어 시작하기 </summary>
	public void ContinueStart() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Continue_Use);
		m_ContinueCnt++;
		//스테이지 결과 초기화
		STAGEINFO.m_Result = StageResult.None;

		//생존스탯 초기화 후 디버프도 체크
		for (StatType i = StatType.Men; i <= StatType.HP; i++) {
			float preval = STAGE_USERINFO.m_Stat[(int)i, 0];
			STAGE_USERINFO.m_Stat[(int)i, 0] = STAGE_USERINFO.m_Stat[(int)i, 1];
			if (i < StatType.SurvEnd) DLGTINFO.f_RfStatUI?.Invoke(i, STAGE_USERINFO.m_Stat[(int)i, 0], preval, STAGE_USERINFO.m_Stat[(int)i, 1]);
			else DLGTINFO.f_RfHPUI?.Invoke(STAGE_USERINFO.m_Stat[(int)i, 0], preval, STAGE_USERINFO.m_Stat[(int)i, 1]);
		}
		STAGE_USERINFO.SetDebuff();

		//스킬 쿨타임 초기화
		for (int i = 0; i < USERINFO.m_PlayDeck.GetDeckCharCnt(); i++) {
			m_Chars[i].SkillColoTimeInit();
		}

		//턴수 초기화
		if (STAGE_USERINFO.IS_TurnUse()) {
			int turn = BaseValue.CONTINUE_LIMITTURNPLUS;
			STAGE_USERINFO.m_TurnCnt = Mathf.Max(STAGE_USERINFO.m_TurnCnt - turn, 0);
			m_User.m_Turn = Mathf.Min(m_User.m_Turn + turn, STAGEINFO.m_TStage.m_LimitTurn + Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.LimitTurnUp)));
			DLGTINFO?.f_RFClockChangeUI?.Invoke(m_User.m_Turn, m_User.m_Time);
		}

		//시간제한, 전투횟수 초기화
		switch (STAGEINFO.m_TStage.m_Fail.m_Type) {
			case StageFailType.Time:
				m_Timer = Mathf.Max(m_Timer - BaseValue.CONTINUE_TIMEPLUS, 0); 
				DLGTINFO?.f_RFModeTimer?.Invoke(STAGEINFO.m_TStage.m_Fail.m_Value);
				IS_PassTimer = true;
				break;
			default:
				break;
		}
		if (STAGE.m_ModeCnt.ContainsKey(PlayType.TurmoilCount)) {
			STAGE.m_ModeCnt[PlayType.TurmoilCount] = Mathf.RoundToInt(STAGEINFO.m_TStage.m_Fail.m_Cnt);
			STAGE.m_MainUI.RefreshModeAlarm(PlayType.TurmoilCount,0, true);
		}

		m_MainUI.AccToggleCheck();
	}
}
