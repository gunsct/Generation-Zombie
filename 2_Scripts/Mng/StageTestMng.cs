using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTestMng : ObjMng
{
	public enum Select_StageDifficultyType
	{
		Normal = StageDifficultyType.Normal,
		Hard = StageDifficultyType.Hard,
		Nightmare = StageDifficultyType.Nightmare,
	}

	[System.Serializable]
	public class VCharInfo
	{
		[ReName("인덱스")]
		public int Idx;
		[ReName("레벨")]
		public int LV = 1;
		[ReName(1, 10, "등급")]
		public int Grade = 1;
		[ReName(1, 10, "장비 등급")]
		public int EQ_Grade = 1;
		[ReName("장비 레벨(레벨 테이블 참조 할것)")]
		public int EQ_LV;
		[ReName("DNA")]
		public List<DNA> DNA = new List<DNA>();
	}

	[System.Serializable]
	public class DNA
	{
		[ReName("인덱스")]
		public int Idx;
		[ReName(1, 10, "등급")]
		public int Grade = 1;
		[ReName(1, 5, "레벨")]
		public int Lv = 1;
	}
	[Serializable]
	public struct EquipList
	{
		[ReName("무기", "헬멧", "신발", "옷", "악세서리")]
		public List<int> Idx;
	}
#pragma warning disable 0414
	[HideInInspector] public bool IsUseTeam;
	[ReName("스테이지 난이도")] public StageDifficultyType Diff;
	[ReName("테스트 스테이지 인덱스")] public int StageIdx = 100;
	[ReName("머지탭 클리어 스테이지 인덱스")] public int ClearStageIdx = 100;

	[ReName("세럼 블럭, 위치")] public int[] SerumIdx = new int[2] { 0, 0 };
	[ReName("0번", "1번", "2번", "3번", "4번")] public VCharInfo[] Chars = new VCharInfo[] { new VCharInfo(), new VCharInfo(), new VCharInfo(), new VCharInfo(), new VCharInfo() };

	[ReName("등급 1", "등급 2", "등급 3", "등급 4", "등급 5", "등급 6", "등급 7", "등급 8", "등급 9", "등급 10")]
	public List<EquipList> EquipGradeList = new List<EquipList>() {
		new EquipList(){ Idx = new List<int>(){ 111007, 111008, 111009, 111010, 111011 } },
		new EquipList(){ Idx = new List<int>(){ 111107, 111108, 111109, 111110, 111111 } },
		new EquipList(){ Idx = new List<int>(){ 111207, 111208, 111209, 111210, 111211 } },
		new EquipList(){ Idx = new List<int>(){ 111307, 111308, 111309, 111310, 111311 } },
		new EquipList(){ Idx = new List<int>(){ 111407, 111408, 111409, 111410, 111411 } },
		new EquipList(){ Idx = new List<int>(){ 111507, 111508, 111509, 111510, 111511 } },
		new EquipList(){ Idx = new List<int>(){ 111607, 111608, 111609, 111610, 111611 } },
		new EquipList(){ Idx = new List<int>(){ 111707, 111708, 111709, 111710, 111711 } },
		new EquipList(){ Idx = new List<int>(){ 111807, 111808, 111809, 111810, 111811 } },
		new EquipList(){ Idx = new List<int>(){ 111907, 111908, 111909, 111910, 111911 } }
	};
#pragma warning restore 0414

	// Start is called before the first frame update
	void Awake()
	{
#if STAGE_TEST
		PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", (int)Diff);
		PlayerPrefs.SetInt($"TestStageClearIdx_{USERINFO.m_UID}", ClearStageIdx);
		PlayerPrefs.Save();
		TDATA.LoadDefaultTables(-1);
		Dictionary<int, List<ItemInfo>> eqips = new Dictionary<int, List<ItemInfo>>();
		//List<int> equipGrade = new List<int>() { m_TestStageChar_LL_EquipGrade - 1, m_TestStageChar_L_EquipGrade - 1, m_TestStageChar_C_EquipGrade - 1, m_TestStageChar_R_EquipGrade - 1, m_TestStageChar_RR_EquipGrade - 1 };
		//List<int> Charidx = new List<int>() { m_TestStageChar_LL, m_TestStageChar_L, m_TestStageChar_C, m_TestStageChar_R, m_TestStageChar_RR};
		//List<int> CharRank = new List<int>() { m_TestStageChar_LL_Rank, m_TestStageChar_L_Rank, m_TestStageChar_C_Rank, m_TestStageChar_R_Rank, m_TestStageChar_RR_Rank };
		//List<int> CharLV = new List<int>() { m_TestStageChar_LL_Lv, m_TestStageChar_L_Lv, m_TestStageChar_C_Lv, m_TestStageChar_R_Lv, m_TestStageChar_RR_Lv };
		//List<DNA[]> equipDNA = new List<DNA[]>() { m_TestStageChar_LL_DNA, m_TestStageChar_L_DNA, m_TestStageChar_C_DNA, m_TestStageChar_R_DNA, m_TestStageChar_RR_DNA };

		int userlv = 0;
		for (int i = 0, deckpos = 0; i < 5; i++)
		{
			VCharInfo vChar = Chars[i];
			if (vChar.Idx < 1) continue;
			userlv = userlv < vChar.LV ? vChar.LV : userlv;
			CharInfo cinfo = USERINFO.InsertChar(vChar.Idx, vChar.Grade, vChar.LV);
			var eqidxs = EquipGradeList[vChar.EQ_Grade - 1].Idx;
			for (int j = 0; j < eqidxs.Count; j++)
			{
				ItemInfo info = USERINFO.InsertItem(eqidxs[j]);
				info.m_Lv = vChar.EQ_LV;
				cinfo.m_EquipUID[j] = info.m_Uid;
			}
			for(int j = 0; j < vChar.DNA.Count; j++) {
				DNAInfo info = USERINFO.InsertDNA(vChar.DNA[j].Idx, vChar.DNA[j].Lv);
				cinfo.m_EqDNAUID[j] = info.m_UID;
			}
			cinfo.CheckDNASetFX();

			USERINFO.m_PlayDeck.SetChar(deckpos, cinfo.m_UID);
			deckpos++;
		}
		USERINFO.m_LV = userlv;//TDATA.GetConfig_Int32(ConfigType.UserMaxLevel);
		for (int i = 0; i < TDATA.GetAllCharacterInfos().Count; i++) {
			for(int j = 0;j< Chars.Length; j++) {
				if (TDATA.GetAllCharacterInfos()[i].m_Idx != Chars[j].Idx) USERINFO.InsertChar(TDATA.GetAllCharacterInfos()[i].m_Idx);
			}
		}
		for(int i = 0; i < USERINFO.m_Chars.Count; i++) {
			int serumgid = USERINFO.m_Chars[i].m_TData.m_SerumGroupIdx;
			for (int b = 1; b <= SerumIdx[0]; b++) {
				List<TSerumTable> serums = MainMng.Instance.TDATA.GetSerumTableGroup(serumgid, b);
				if (b == SerumIdx[0]) {
					for (int p = 0; p < SerumIdx[1]; p++) {
						USERINFO.m_Chars[i].InsertSerum(serums[p].m_Idx);
					}
				}
				else {
					for (int p = 0; p < serums.Count; p++) {
						USERINFO.m_Chars[i].InsertSerum(serums[p].m_Idx);
					}
				}
			}
		}

		MAIN.m_State = MainState.STAGE_TEST;
		TModeTable modetable = TDATA.GetModeTable(StageIdx);
		if (modetable != null)
		{
			switch(modetable.m_Content)
			{
			case StageContentType.University:
			case StageContentType.Subway:
				GoWeekModeStage(modetable.m_Content, (DayOfWeek)modetable.m_OpenDay, modetable.m_Pos, modetable.m_Difficulty);
				break;
			default:
				GoModeStage(modetable.m_Content, modetable.m_Difficulty, modetable.m_Pos);
				break;
			}
		}
		else
		{
			GoStage(StageIdx);
		}
		MAIN.m_StartStageTestMng = true;
#else
		Debug.LogError("STAGE_TEST 안켜져있음 !!!!!!!!! 실행 불가");
		MAIN.Exit();
#endif
	}

	public void GoStage(int StageIdx) {
		TStageTable table = TDATA.GetStageTable(StageIdx, USERINFO.GetDifficulty());
		STAGEINFO.SetStage(StagePlayType.Stage, table.m_Mode, StageIdx, 1, DayOfWeek.Sunday, USERINFO.GetDifficulty());
		if(STAGEINFO.m_TStage.m_Mode == StageModeType.Training)
		{
			// 메인 UI위치에 로드해줌
			TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
			POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) =>
			{
				MAIN.Exit();
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

	public void GoModeStage(StageContentType content, int lv, int pos = 0)
	{
		TModeTable tdata = TDATA.GetModeTable(content, lv, DayOfWeek.Sunday, pos);
		if (tdata == null) return;
		STAGEINFO.SetStage(StagePlayType.OutContent, TDATA.GetStageTable(tdata.m_StageIdx).m_Mode, tdata.m_StageIdx, lv, DayOfWeek.Sunday, pos);
		AsyncOperation pAsync = null;

		if (STAGEINFO.m_TStage.m_Mode == StageModeType.Training) {
			// 메인 UI위치에 로드해줌
			TStageCondition<StageClearType> clear = STAGEINFO.m_TStage.m_Clear[0];
			POPUP.Set_Popup(PopupPos.MAINUI, PopupName.Training, (result, obj) => {
				MAIN.Exit();
			}, true, clear.m_Value, Mathf.RoundToInt(clear.m_Cnt), STAGEINFO.m_TStage.m_LimitTurn);
		}
		else {
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
		DayOfWeek day = week;
		if (tdata.m_Content == StageContentType.Subway) {
			day = (DayOfWeek)tdata.m_OpenDay;
		}
		STAGEINFO.SetStage(StagePlayType.OutContent, TDATA.GetStageTable(tdata.m_StageIdx).m_Mode, tdata.m_StageIdx, lv, day, pos);
		AsyncOperation pAsync = null;
		pAsync = MAIN.StateChange(STAGEINFO.GetModeTypeMainState(), SceneLoadMode.BACKGROUND, () =>
		{
			MAIN.ActiveScene();
		});
	}
}
