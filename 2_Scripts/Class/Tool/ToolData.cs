using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class ToolData : ClassMng
{
	[System.Diagnostics.Conditional("TOOL_LOAD_DEBUG")]
	public static void DebugLog(object _log)
	{
		Debug.Log(_log);
	}

	ToolFile[] m_LoadList;

	/// <summary> 초기화 MainMng init 에서 호출하므로 전부 넣으면 최초 어플 실행시 문제됨</summary>
	public void Init()
	{
		m_LoadList = new ToolFile[]
		{
			m_Config,
			m_UserProfileImage,
			// Item
			m_Item,
			m_GachaGroup,
			m_EQSpecialStat,
			m_EQExp,
			m_RandomStat,
			m_Reassembly,
			m_ZombieSerumDrop,
			m_GetGuide,
			// Char
			m_Char,
			m_CharGrade,
			m_CharGradeStat,
			m_Skill,
			m_SkillGrowth,
			m_Synergy,
			m_Stat,
			m_Exp,
			m_Serum,
			m_SerumBlock,
			m_Char_HR,
			m_StatBonus,
			// Stage
			m_Chapter,
			m_Stage,
			m_Mode,
			m_IngameReward,
			m_Guide,
			m_StageMaking,
			m_StageMaterial,
			m_EnemyStageSkill,
			m_EnemyNote,
			m_EnemySkill,
			m_Enemy,
			m_EnemyDrop,
			m_EthnicRelation,
			m_EnemyLV,
			m_Event,
			m_SelectDropGroup,
			m_GambleCard,
			m_StatusDebuff,
			m_EnumyNoteGroup,
			m_DamageAdjustment,
			m_StageExcept,
			m_StageGuide,
			// Tower
			m_TowerMap,
			m_TowerEvent,
			// 훈련
			m_Training,
			// DNA
			m_DNA,
			m_DNALevel,
			m_DNACombination,
			m_DNASetEffect,
			// 좀비,
			m_Zombie,
			m_ZombieResearchLV,
			m_ZombieResearch,
			// 연구
			m_Research,
			// 제작
			m_Making,
			// 탐험
			m_Adventure,
			// 상점
			m_Shop,
			m_SupplyBox,
			m_EquipGacha,
			m_Package,
			m_PickupGachaGroup,
			// Talk
			m_Alternative,
			m_Talker,
			m_Dialog,
			m_ConditionGroup,
			m_CaseSelect,
			m_PerconalityDialogue,
			// 업적
			m_Achievement,
			// 컬렉션
			m_Collection,
			// 미션
			m_Mission,
			//추천상품
			m_ShopAdiveCondtion,
			//PVP
			m_PvPRank,
			m_PVPSkill,
			m_PVPSpeedRevision,
			m_PVPDef,
			m_PVPRankReward,
			m_PVPLeagueReward,
			m_PVPSeasonReward,
			m_PVP_Camp,
			m_PVP_Camp_NodeLevel,
			m_PVP_Camp_Resource,
			m_PVP_Camp_Storage,
			//Guild
			m_GuildMark,
			m_Guild_Exp,
			m_Guild_res,
		};

		LoadString();
		m_Config.Load();

		m_LoadFileName = "";
	}

	public void LoadDefaultTables(int loadlv)
	{
		if(loadlv < 1) LoadString();

		if(loadlv < 0)
		{
			for (int i = 0, iMax = m_LoadList.Length; i < iMax; i++)
			{
				m_LoadList[i].Load();
			}
		}
		else
		{
			m_LoadList[loadlv].Load();
		}
	}

	public IEnumerator LoadAllTablesAsync(bool AllLoad, Action<int> StartCB = null, Action<string, long> FileChange = null, Action<long> Proc = null)
	{
		ToolData.DebugLog("Loading Start LINE_SLIP_CNT :" + ToolFile.LINE_SLIP_CNT);
#if TOOL_LOAD_DEBUG
		var curtime = UTILE.Get_Time();
		var starttime = curtime;
#endif
		int i = 1;
		int Cnt = m_LoadList.Length;
		if(!AllLoad)
		{
			i--;
			Cnt += (int)StringTalbe.Max;
		}
		StartCB?.Invoke(Cnt);

		// 전체 로드일때 스트링 로딩
		if (AllLoad) yield return LoadStringAsync(FileChange, Proc);
		// 읽은 라인 초기화
		ToolFile.LOADED_LINT = 0;
		i = 0;
		// 등록된 넘들만 로딩
		for (int iMax = m_LoadList.Length - i; i < iMax; i++) yield return m_LoadList[i].Load_Async(FileChange, Proc);
		yield return new WaitForEndOfFrame();

		// 가비지 컬렉션 강제실행(메모리 정리)
		System.GC.Collect();
#if TOOL_LOAD_DEBUG
		ToolData.DebugLog("Loading end Total Time : " + (UTILE.Get_Time() - curtime).ToString("0.####"));
#endif
	}

	string m_LoadFileName;
	public void LoadStageData(string filename = null)
	{
		if(m_LoadFileName.Equals(filename)) return;
		LoadStageCardTable(filename);
		m_LoadFileName = filename == null ? "" : filename;
	}

	public void LoadPVPData()
	{
	}
}
