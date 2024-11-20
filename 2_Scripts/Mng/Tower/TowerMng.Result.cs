using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class TowerMng : ObjMng
{
	/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 성공 실패 체크
	public bool CheckEnd()
	{
		if (STAGEINFO.m_Result != StageResult.None) return false;

		StageFailKind kind = m_Check.IsFail();
		if (m_Check.IsClear()) {
			// 클리어 조건 충족이면서 출구라면 클리어
			StageClear();
			return true;
		}
		else if (kind != StageFailKind.None)
		{
			StageFail(kind);
			return true;
		}
		return false;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage 종료
	public void StageClear() {
		STAGEINFO.m_Result = StageResult.Clear;
		MAIN.TimeSlowNFast(0.02f, 0.3f, 1f);
		StopAllCoroutines();
#if NOT_USE_NET
		OnClear();
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
				if (tdata != null)
				{
					var stage = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()];
					stage.Clear = tdata.m_Idx;
					//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear == 103) {
					//	PlayerPrefs.SetInt($"View_Intro_{USERINFO.m_UID}", 1);
					//	PlayerPrefs.Save();
					//}
					if (tdata.m_NextIdx != 0) stage.Idx = tdata.m_NextIdx;
					stage.PlayCount = 0;
					stage.ChapterReward = TDATA.GetChapterTable(USERINFO.GetDifficulty(), stage.Clear) == null ? 0 : stage.Clear;
					TStageTable stagetable = TDATA.GetStageTable(stage.Clear, USERINFO.GetDifficulty());
					if (stagetable.m_ClearEvent) USERINFO.m_AddEvent = TDATA.GetRandEventTable().m_Idx;
					stage.Selects.Clear();
					USERINFO.SetMainMenuAlarmVal(MainMenuType.Stage);
				}
				break;
			default:
				USERINFO.OutGameClear();
				break;
			}
			MAIN.Save_UserInfo();
#else
			//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Clear == 103) {
			//	PlayerPrefs.SetInt($"View_Intro_{USERINFO.m_UID}", 1);
			//	PlayerPrefs.Save();
			//}
#endif

			AsyncOperation pAsync = null;
			pAsync = MAIN.StateChange(MainState.PLAY, SceneLoadMode.BACKGROUND, () => {
				MAIN.ActiveScene();
			});
		}
#if NOT_USE_NET
		);
#else
		, res);
#endif
	}

	public void StageFail(StageFailKind failkind) {
		if (STAGEINFO.m_Result == StageResult.Fail) return;
		MAIN.TimeSlowNFast(0.02f, 0.3f, 1f);
		StopAllCoroutines();
		STAGEINFO.m_Result = StageResult.Fail;

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Stage_FailCause, (result, obj) => {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.StageFailed, (result, obj) => {
				if (result == 0) {
					MAIN.Save_UserInfo();
					AsyncOperation pAsync = null;
					pAsync = MAIN.StateChange(MainState.PLAY, SceneLoadMode.BACKGROUND, () => {
						MAIN.ActiveScene();
					});
				}
			}, failkind);
		}, failkind);
	}
}
