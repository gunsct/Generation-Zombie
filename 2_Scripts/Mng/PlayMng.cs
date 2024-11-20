using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public class PlayMng : ObjMng
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// Instance
	private static PlayMng m_Instance = null;
	public static PlayMng Instance
	{
		get
		{
			return m_Instance;
		}
	}

	List<RES_SYSTEM_MSG> m_SysMsgs = new List<RES_SYSTEM_MSG>();
	Main_Play m_MainUI;

	public static bool IsValid()
	{
		return m_Instance != null;
	}

	// Start is called before the first frame update
	void Awake()
	{
		m_Instance = this;
	}

	private void OnDestroy()
	{
		m_Instance = null;
	}

	private IEnumerator Start()
	{
#if NOT_USE_NET
#	if UNITY_EDITOR
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(-1);
			MAIN.Load_UserInfo();		
		}
#endif
#elif UNITY_EDITOR
		if (MAIN.IS_BackState(MainState.START))
		{
			TDATA.LoadDefaultTables(-1);
			MAIN.ReStart();
			yield break;
		}
#endif

		bool end = false;
		HIVE.ReLoadPromotion();

#if !NOT_USE_NET
		USERINFO.m_Friend.LoadFriendInfo(() => { end = true; });
		yield return new WaitWhile(() => !end);
#endif
		if (USERINFO.m_Guild.IsReLoad)
		{
			end = false;
			USERINFO.m_Guild.LoadGuild(() => {
				end = true;
			}, 0, true, true, true);
			yield return new WaitWhile(() => !end);
		}

		SetBGSND();
		m_MainUI = POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play).GetComponent<Main_Play>();

		//튜토리얼 클론 덱(캐릭터) 세팅
		TUTO.SetCloneDeck();
		yield break;
	}


	public bool OnBack()
	{
		return m_MainUI.OnBack();
	}

	public void SetBGSND() {
		TStageTable table = TDATA.GetStageTable(USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx);
		int diff = USERINFO.GetDifficulty();
		int gimmick = 0;//0:없음, 1:화재, 2:어둠, 3:공습

		if (table.Is_Dark) gimmick = 2;
		else {
			for (int i = 0; i < table.m_PlayType.Count; i++) {
				if (table.m_PlayType[i].m_Type == PlayType.FireSpread) {
					gimmick = 1;
					break;
				}
				else if (table.m_PlayType[i].m_Type == PlayType.FieldAirstrike) {
					gimmick = 3;
					break;
				}
			}
		}
		switch (gimmick) {
			case 0:
				List<SND_IDX> idx = new List<SND_IDX>() { SND_IDX.BGM_0011, SND_IDX.BGM_0012, SND_IDX.BGM_0013, SND_IDX.BGM_0014, SND_IDX.BGM_0015 };
				PlayBGSound(idx[UTILE.Get_Random(0, idx.Count)]); break;
			case 1:
				PlayBGSound(SND_IDX.BGM_0051); break;
			case 2:
				PlayBGSound(SND_IDX.BGM_0052); break;
			case 3:
				PlayBGSound(SND_IDX.BGM_0050); break;
		}
	}

	public void GetStagePlayCode(Action<int> action, StageContentType content, int Idx, DayOfWeek week = DayOfWeek.Sunday, int pos = 0, long _euid = 0)
	{
		UserInfo.StageIdx stage = USERINFO.m_Stage[content].Idxs.Find(o => o.Week == week && o.Pos == pos);
		WEB.SEND_REQ_STAGE_START((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.SEND_REQ_STAGE((res2) => { action?.Invoke(res.result_code); });
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			action?.Invoke(res.result_code);
			USERINFO.CheckClearUserPickInfo(content, week, pos, Idx);
		}, USERINFO.m_Stage[content].UID, week, pos, Idx, stage.DeckPos, false, _euid);
	}


	public void GoStage(int StageIdx) {
		USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount++;
		TStageTable table = TDATA.GetStageTable(StageIdx, USERINFO.GetDifficulty());
		STAGEINFO.SetStage(StagePlayType.Stage, table.m_Mode, StageIdx, 1, DayOfWeek.Sunday, USERINFO.GetDifficulty());
		if(STAGEINFO.m_TStage.m_Mode == StageModeType.Training)
		{
			// 메인 UI위치에 로드해줌
			TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
			POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) =>
			{
				// 메인 UI 변경
				POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
				PLAY.SetBGSND();
			}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
		}
		else
		{
			AsyncOperation pAsync = null;
			pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () =>
			{
				MAIN.ActiveScene(() => {
					switch (STAGEINFO.m_StageModeType)
					{
					case StageModeType.NoteBattle:
						BATTLE.Init(EBattleMode.Normal, STAGEINFO.GetCreateEnemyIdx(), STAGEINFO.GetCreateEnemyLV(0, false), 0, null, true);
						break;
					}
				});
			});
		}
	}
	[ContextMenu("sdatktest")]
	void adatktest() {
		SuddenAtkEvent(72, null);
	}
	public void SuddenAtkEvent(int _enemyidx, Action<int> _cb) {
		AsyncOperation pAsync = null;
		STAGEINFO.m_User.Init();
		STAGEINFO.m_StageModeType = StageModeType.None;

		pAsync = MAIN.StateChange(MainState.BATTLE, SceneLoadMode.BACKGROUND, () => {
			MAIN.ActiveScene(() => {
				BATTLE.Init(EBattleMode.Normal, _enemyidx, USERINFO.m_LV, 0, ()=> {
					_cb?.Invoke((int)BATTLEINFO.m_Result);
				}, true, false);
				//돌발이벤트 시간안씀
				POPUP.GetBattleUI().GetComponent<BattleUI>().GetTimerObj.SetActive(false);
			});
		});
	}
	
	public void GoModeStage(StageContentType content, int lv, int pos = 0)
	{
		TModeTable tdata = null;
		DayOfWeek day = DayOfWeek.Sunday;
		switch (content) {
			case StageContentType.Subway:
				UserInfo.SubwayStgIdx subdata = USERINFO.GetSubwayStgIdx();
				day = subdata.m_Day;
				tdata = TDATA.GetModeTable(subdata.m_StgIdx);
				break;
			default:
				day = DayOfWeek.Sunday;
				tdata = TDATA.GetModeTable(content, lv, day, pos);
				break;
		}
		if (tdata == null) return;

		STAGEINFO.SetStage(StagePlayType.OutContent, TDATA.GetStageTable(tdata.m_StageIdx).m_Mode, tdata.m_StageIdx, lv, day, pos);

		switch (content) {
			case StageContentType.University:
			case StageContentType.Subway:
				USERINFO.m_Stage[STAGEINFO.m_StageContentType].Idxs.Find(t => t.Week == STAGEINFO.m_Week && t.Pos == STAGEINFO.m_Pos).PlayCount++;
				break;
			default:
				USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].PlayCount++;
				break;
		}

		if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training) {
			if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training && PlayerPrefs.GetInt($"TrainingGuide_{USERINFO.m_UID}", 0) < 1) {
				PlayerPrefs.SetInt($"TrainingGuide_{USERINFO.m_UID}", 1);
				PlayerPrefs.Save();
				POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Tuto_Video, (result, obj) => {
					// 메인 UI위치에 로드해줌
					TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
					POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) => {
						// 메인 UI 변경
						POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
						PLAY.SetBGSND();
					}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
				}, TutoVideoType.Training);
			}
			else {
				// 메인 UI위치에 로드해줌
				TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
				POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) => {
					// 메인 UI 변경
					POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
					PLAY.SetBGSND();
				}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
			}
		}
		else {
			AsyncOperation pAsync = null;
			pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () => {
				MAIN.ActiveScene(() => {
					switch (STAGEINFO.m_TStage.m_Mode) {
						case StageModeType.NoteBattle:
							BATTLE.Init(EBattleMode.Normal, STAGEINFO.GetCreateEnemyIdx(STAGEINFO.m_StageContentType == StageContentType.Bank), STAGEINFO.GetCreateEnemyLV(0, false), 0, null, true);
							break;
					}
				});
			});
		}
	}

	public void GoWeekModeStage(StageContentType _contenttype, DayOfWeek week, int pos, int lv)
	{
		TModeTable tdata = TDATA.GetModeTable(_contenttype, lv, week, pos);
		if (tdata == null) return;
		STAGEINFO.SetStage(StagePlayType.OutContent, TDATA.GetStageTable(tdata.m_StageIdx).m_Mode, tdata.m_StageIdx, lv, week, pos);
		AsyncOperation pAsync = null;
		pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () =>
		{
			MAIN.ActiveScene();
		});
	}
	public void GoEventState(MyFAEvent _event, int _idx, int _lv) {
		TStageTable table = TDATA.GetStageTable(_idx);
		STAGEINFO.SetStage(StagePlayType.Event, table.m_Mode, _idx, _lv, DayOfWeek.Sunday, 0);
		if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training) {
			// 메인 UI위치에 로드해줌
			TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
			POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) => {
				// 메인 UI 변경
				POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
				PLAY.SetBGSND();
			}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
		}
		else {
			AsyncOperation pAsync = null;
			pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () => {
				MAIN.ActiveScene(() => {
					switch (STAGEINFO.m_StageModeType) {
						case StageModeType.NoteBattle:
							BATTLE.Init(EBattleMode.Normal, STAGEINFO.GetCreateEnemyIdx(), STAGEINFO.GetCreateEnemyLV(0, false), 0, null, true);
							break;
					}
				});
			});
		}
	}
	public void GoReplay(UserInfo.Stage _stg, UserInfo.StageIdx _stgidx) {
		_stgidx.PlayCount++;
		TStageTable table = TDATA.GetStageTable(_stgidx.Idx, USERINFO.GetDifficulty());
		STAGEINFO.SetStage_Replay(_stg, _stgidx);
		if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training) {
			// 메인 UI위치에 로드해줌
			TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
			POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) => {
				// 메인 UI 변경
				POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Play);
				PLAY.SetBGSND();
			}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
		}
		else {
			AsyncOperation pAsync = null;
			pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () => {
				MAIN.ActiveScene(() => {
					switch (STAGEINFO.m_StageModeType) {
						case StageModeType.NoteBattle:
							BATTLE.Init(EBattleMode.Normal, STAGEINFO.GetCreateEnemyIdx(), STAGEINFO.GetCreateEnemyLV(0, false), 0, null, true);
							break;
					}
				});
			});
		}
	}
}
