using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 성공 실패 체크
	public enum ResultType
	{

	}
	public bool CheckEnd(StageCheckType type = StageCheckType.End) {
		if (STAGEINFO.m_Result != StageResult.None) return false;
		StageFailKind kind = m_Check.IsFail();
		if (m_Check.IsClear()) {
			// 클리어 조건 충족이면서 출구라면 클리어
			if (type == StageCheckType.AnyMaking) {
				STAGEINFO.m_Result = StageResult.Clear;
				if(STAGEINFO.m_Result != StageResult.None) Invoke("StageClear", 1f);
				//StageClearDelay(type);
			}
			else
				StageClear();
			return true;
		}
		else if (kind != StageFailKind.None) {
			StageFail(kind);
			return true;
		}
		return false;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage 종료
	public void StageClearDelay(StageCheckType type = StageCheckType.End) {
		STAGEINFO.m_Result = StageResult.Clear;
		StartCoroutine(IE_StageClear(type));
	}
	public IEnumerator IE_StageClear(StageCheckType type = StageCheckType.End) {
		if(type == StageCheckType.AnyMaking)
			yield return new WaitWhile(() => m_MainUI.GetMaking.GetState() != Item_Stage_Make.State.None);
		StageClear();
	} 
	public void StageClear() {
		STAGEINFO.m_Result = StageResult.Clear;
		MAIN.TimeSlowNFast(0.02f, 0.3f, 1f);
		StopAllCoroutines();
#if NOT_USE_NET
		OnClear();
		// 업적 카운트 체크
		if(STAGEINFO.m_PlayType == StagePlayType.Stage)
		{
			switch(STAGEINFO.m_Pos)
			{
			case 0: USERINFO.m_Achieve.Check_Achieve(AchieveType.Normal_Stage_Clear); break;
			case 1: USERINFO.m_Achieve.Check_Achieve(AchieveType.Hard_Stage_Clear); break;
			case 2: USERINFO.m_Achieve.Check_Achieve(AchieveType.Nightmare_Stage_Clear); break;
			}
		}
#else
		STAGEINFO.StageClear((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 클리어 다시 시도
					StageClear();
				});
				return;
			}

			OnClear(res);
		});
#endif
	}

#if NOT_USE_NET
	void OnClear()
#else
	void OnClear(LS_Web.RES_STAGE_CLEAR res)
#endif
	{
		POPUP.GetWorldUIPanel().gameObject.SetActive(true);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Result, (result, obj) => {
#if NOT_USE_NET
			switch (STAGEINFO.m_PlayType)
			{
			case StagePlayType.Stage:
				TStageTable tdata = STAGEINFO.m_TStage;
				if (tdata != null) {
					var stage = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
					stage.Clear = tdata.m_Idx;
					//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear == 103) {
					//	PlayerPrefs.SetInt($"View_Intro_{USERINFO.m_UID}", 1);
					//	PlayerPrefs.Save();
					//}
					if (tdata.m_NextIdx != 0)stage.Idx = tdata.m_NextIdx;
					stage.PlayCount = 0;
					stage.ChapterReward = TDATA.GetChapterTable(USERINFO.GetDifficulty(), stage.Clear) == null ? 0 : stage.Clear;
					TStageTable stagetable = TDATA.GetStageTable(stage.Clear, USERINFO.GetDifficulty());
					if (stagetable.m_ClearEvent) USERINFO.m_AddEvent = TDATA.GetRandEventTable().m_Idx;
					stage.Selects.Clear();
					USERINFO.SetMainMenuAlarmVal(MainMenuType.Stage);
					USERINFO.Check_StageClear(USERINFO.m_Stage[STAGEINFO.GetContentType()].Mode, STAGEINFO.m_Pos, STAGEINFO.m_Idx);
				}
				break;
			default:
				USERINFO.OutGameClear();
				break;
			}
#else
			//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear == 103) {
			//	PlayerPrefs.SetInt($"View_Intro_{USERINFO.m_UID}", 1);
			//	PlayerPrefs.Save();
			//}
#endif
			GoPlay();
		}
#if NOT_USE_NET
		);
#else
		, res);
#endif
	}

	public void StageFail(StageFailKind failkind) {
		if (STAGEINFO.m_Result == StageResult.Fail) return;
		//TODO: 아이템사용으로 추가 진행, 오로지 스테이지 컨텐츠의 카드깔리는 스테이지 타입에서만 가능
		STAGEINFO.m_Result = StageResult.Fail;
		if (CheckContinue(failkind)) {
			List<int> checkcnt = new List<int>();
			TStageTable stage = STAGEINFO.m_TStage;
			for (int i = 0;i < stage.m_Clear.Count; i++) {
				checkcnt.Add(m_Check.GetClearCnt(i));
			}
			Time.timeScale = 0f;
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_FailCause, (result, obj) => {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_Continue, (result, obj) => {
					if (result == 1) {
						m_MainUI.AccToggleCheck();
						ContinueStart();
					}
					else {
						StopAllCoroutines();
						StartCoroutine(IE_FailDelay(failkind, false));
					}
				}, checkcnt);//m_ContinueCnt
			}, failkind);
		}
		else {
			MAIN.TimeSlowNFast(0.02f, 0.3f, 1f);
			StopAllCoroutines();

			StartCoroutine(IE_FailDelay(failkind, true));
		}
	}
	bool CheckContinue(StageFailKind _failkind) {
		if (STAGEINFO.m_PlayType == StagePlayType.Event) return false;
		if (STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
		if (STAGEINFO.m_StageModeType != StageModeType.Stage) return false;
		//if(!USERINFO.m_ShopInfo.IsPassBuy() && m_ContinueCnt >= BaseValue.GET_DIFF_CONTINUEMAX(USERINFO.GetDifficulty())) return false;
		//난이도별 이어하기 횟수 제한
		//if (m_ContinueCnt >= BaseValue.GET_DIFF_CONTINUEMAX(USERINFO.GetDifficulty())) return false;
		//var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.CONTINUETICKET_SHOP_IDX);
		//if (buyinfo != null && buyinfo.Cnt >= BaseValue.GET_DIFF_CONTINUEMAX(USERINFO.GetDifficulty()) && !USERINFO.m_ShopInfo.IsPassBuy()) return false;
		if (STAGEINFO.m_TStage.m_NoRescue || _failkind == StageFailKind.TurmoilCount) return false;//&& STAGE_USERINFO.m_Turn >= 0 && STAGE_USERINFO.m_Turn < 4
		//if (_failkind != StageFailKind.Turn &&
		//	_failkind != StageFailKind.Time &&
		//	_failkind != StageFailKind.HP && 
		//	_failkind != StageFailKind.Men && 
		//	_failkind != StageFailKind.Sat && 
		//	_failkind != StageFailKind.Hyg) return false;
		return true;
	}
	IEnumerator IE_FailDelay(StageFailKind failkind, bool _usecause) {
		if (_usecause) {
			if (failkind == StageFailKind.Turn)
				yield return new WaitForSecondsRealtime(2f);
			else
				yield return new WaitForSecondsRealtime(0.5f);

			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_FailCause, (result, obj) => {
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.StageFailed, (result, obj) => {
					if (result == 0) GoPlay();
				}, failkind);
			}, failkind);
		}
		else {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.StageFailed, (result, obj) => {
				if (result == 0) GoPlay();
			}, failkind);
		}
		POPUP.GetWorldUIPanel().gameObject.SetActive(true);
	}

	void GoPlay()
	{
		MAIN.Save_UserInfo();
		//AsyncOperation pAsync = null;
		//pAsync = MAIN.StateChange(MainState.PLAY, SceneLoadMode.BACKGROUND, () => {
		//	MAIN.ActiveScene(pAsync);
		//});
		MAIN.StateChange(MainState.PLAY);
	}
}
