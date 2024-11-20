using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TutoKind
{
	[InspectorName("사용안함")]
	None = 0,
	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	// InGame
	[InspectorName("101 스테이지")]//수정
	Stage_101,
	[InspectorName("102 스테이지")]//수정
	Stage_102,
	[InspectorName("103 스테이지")]//수정
	Stage_103,
	[InspectorName("104 스테이지")]
	Stage_104,
	[InspectorName("105 스테이지")]
	Stage_105,
	[InspectorName("203 스테이지")]
	Stage_203,
	[InspectorName("204 스테이지")]
	Stage_204,
	[InspectorName("206 스테이지")]
	Stage_206,
	[InspectorName("301 스테이지")]//신규
	Stage_301,
	[InspectorName("304 스테이지")]//신규
	Stage_304,
	[InspectorName("401 스테이지")]
	Stage_401,
	[InspectorName("403 스테이지")]
	Stage_403,
	[InspectorName("501 스테이지")]//신규
	Stage_501,
	[InspectorName("601 스테이지")]
	Stage_601,
	[InspectorName("701 스테이지")]//신규
	Stage_701,
	[InspectorName("801 스테이지")]
	Stage_801,
	//[InspectorName("노트전투")]
	//NoteBattle,

	/////////////////////////////////////////////////////////////////////////////////////////////////////////
	// OutContent
	[InspectorName("덱 시너지")]//수정
	DeckSynergy,
	[InspectorName("장비&캐릭터 강화")]
	EquipCharLVUP,
	[InspectorName("캐릭터 승급")]
	CharGradeUP,
	[InspectorName("상점 보급 상자")]
	ShopSupplyBox,
	[InspectorName("상점 장비 뽑기")]
	ShopEquipGacha,
	[InspectorName("신규유저 미션")]
	NewMission,
	[InspectorName("제작")]
	Making,
	[InspectorName("댄 공장")]
	Factory,
	//[InspectorName("보일 사관학교")]
	//Academy,
	[InspectorName("로지은행")]
	Bank,
	[InspectorName("연구")]
	Research,
	//[InspectorName("혈청")]
	//Serum,
	[InspectorName("고든 타워")]
	Tower,
	[InspectorName("캐릭터 DNA")]
	DNA,
	[InspectorName("램버튼 공동 묘지")]
	Cemetery,
	[InspectorName("좀비 사육장")]
	Zombie,
	[InspectorName("탐험")]
	Adventure,
	[InspectorName("하버드 대학")]
	University,
	//[InspectorName("하드")]
	//Hard_Stage,
	//[InspectorName("나이트메어")]
	//Nightmare_Stage,
	[InspectorName("유저 평가")]
	UserReview,
	[InspectorName("DNA 생성")]
	DNA_Make,
	[InspectorName("지하철")]
	Subway,
	[InspectorName("PVP_Main")]
	PVP_Main,
	[InspectorName("PVP_Play")]
	PVP_Play,
	[InspectorName("Guild")]
	Guild,
	[InspectorName("PickupGacha")]
	PickupGacha,
	[InspectorName("Replay")]
	Replay,
	[InspectorName("덱세팅 캐릭터 정보")]
	DeckCharInfo,
	/// <summary> 달성 </summary>
	End
}

public enum TutoTouchCheckType
{
	/// <summary> args[0] -> Item_Stage </summary>
	StageCard = 0,
	/// <summary> args[0] -> Item_Stage_Char </summary>
	StageCard_Char,
	/// <summary> args[0] -> ETouchState.PRESS, Item_Battle_Note_Base, pos </summary>
	Battle_Note,
	/// <summary> args[0] -> EBattleDir </summary>
	Battle_Eva,
	Battle_Def,
	/// <summary> args[0] -> MainMenuType </summary>
	Play_Menu,
	/// <summary> 스테이지 페이지 드래그 </summary>
	Play_StagePageDrag,
	/// <summary> args[0] -> 0 : Inven, 1 : StageStart, 2 : 첼린지, 3 : 우편, 4 : 공지, 5 : 친구, 6 : 일일퀘스트, 7 : 시즌패스, 8:보급상자, 9:연합,10:리플레이(긴급임무),11:이벤트, 12월정액제 </summary>
	Play_Btn,
	/// <summary> 트레이닝 노트 </summary>
	Training_Note,
	/// <summary> 팝업 닫기 args[0] -> Result
	/// <para> args[1] -> PopupName</para>
	/// <para> 주의사항 팝업 닫기의 경우 2번씩 호출되는 경우가 있으므로 터치 체크에서는 Next호출을 하지 않는다.</para>
	/// </summary>
	PopupOnClose,
	/// <summary> args[0] -> 0 : 시너지, 1:스테이지 정보</summary>
	DeckSetting,
	/// <summary> args[0] -> pos </summary>
	DeckSetting_SelectDeck,
	/// <summary> args[0] -> pos </summary>
	DeckSetting_SelectDeckSlot,
	/// <summary> args[0] -> 0 : on, 1 : off </summary>
	DeckSetting_ListPage,
	/// <summary> args[0] -> Item_CharManageCard.State
	/// <para> args[1] => Item_CharManageCard</para>
	/// </summary>
	Item_CharManageCard_Select,
	/// <summary> args[0] -> 0 : Synergy_All </summary>
	SynergyDeck,
	/// <summary> args[0] -> 0 : Pause, 1 : TimeScale. 2 : 제작 레시피, 3:스테이지 정보 </summary>
	StageMenu,
	/// <summary> args[0] -> Item_Stage_MakeCard </summary>
	StageMaking,
	/// <summary> 0:infolist, 1:close </summary>
	StageMakingList,
	/// <summary> args[0] -> StageDifficultyType </summary>
	StageDifficulty,
	/// <summary> args[0] -> 0 : Detail, 1 : Synergy, 2: LvUP, 3 : RankUP, 4 : Equip. 5 : AutoEquip, 6 : ViewDNA, 7 : ViewEquip, 8 : ViewSerum, 9 : Story, 10:CharChange, 11:SkillIcon, 12:rating
	/// <para> args[1] -> EquipType </para>
	/// </summary>
	Info_Char_Btn,
	/// <summary> args[0] -> 0 : Condition, 1 : Order </summary>
	Info_Sorting,
	/// <summary> args[0] -> idx </summary>
	Info_Item_ToolTip,
	/// <summary> args[0] -> idx </summary>
	Item_Item_Card,
	/// <summary> args[0] -> Item_EqChange
	/// <para>args[1] -> 0 : change, 1 : info</para>
	/// </summary>
	EquipChange,
	/// <summary> args[0] -> 0 : lvup, 1 : unequip, 2: change, 3: optionchange, 4:optionunlock </summary>
	Info_Item_Equip,
	/// <summary> args[0] -> 0 : expset </summary>
	EquipLevelUp,
	/// <summary> args[0] -> 0 : tab, 1 : sell, 2 : ViewInfo, 3 : AddInven
	/// <para>args[1] -> EMenu</para>
	/// <para>args[2] -> Item_Inventory_Item</para>
	/// </summary>
	Inventory,
	/// <summary> args[0] -> Item_PDA_Menu.State </summary>
	Play_PDA_MainMenu,
	/// <summary> PDA Item 닫기 args[0] -> 0 : base, 1 : researchmenu, 2 : researchtree </summary>
	Play_PDA_Cloase,
	/// <summary> args[0] -> ResearchType </summary>
	Play_PDA_Research_Menu,
	/// <summary> args[0] -> Item_PDA_Research_Element </summary>
	Item_PDA_Research_Element,
	/// <summary> args[0] -> StageContentType </summary>
	Dungeon_Menu,
	/// <summary> args[0] -> 0 : cancel, 1 : Confirm, 2 : detail
	/// <para>args[1] -> MakingInfo</para>
	/// <para>args[2] -> idx</para>
	/// </summary>
	Making,
	/// <summary> args[0] -> 0 : fast, 1 : cloase, 2 : allreward, 3 : reset, 4 : Dispatch
	/// <para>args[1] -> Item_AdventrueList</para>
	/// </summary>
	Adventure,
	/// <summary> args[0] -> 0:패스, 1:추천, 2:달러, 3:뽑기, 4:보급상자, 5:암시장, 6:캐시, 7:경매장, 8:블랙마캣 갱신, 9:보급상자 목록, 10:마일리지상점 </summary>
	ShopBuy,
	/// <summary> args[0] -> 0 : 연속 가차, 1 : 1회 가차 </summary>
	ShopBuy_Gacha,
	/// <summary> args[0] -> 0 : 1회 가차, 1 : 10연차, 2: 30연차 </summary>
	ShopBuy_ItemGacha,
	/// <summary> args[0] -> 0 : 청약철회 </summary>
	ShopEtc,
	/// <summary> 상점 가챠 확률표 args[0] -> 0: 캐릭터 1:장비</summary>
	ShopGachaProp,
	/// <summary> 상점 메뉴 0:메인,1:패키지:2:패스3:옥션</summary>
	ShopMenu,
	/// <summary> args[0] -> _pos </summary>
	Dungeon_Training,
	/// <summary> args[0] -> 0 : play, 1 : skip, : Buy </summary>
	Dungeon_Detail,
	/// <summary> args[0] -> DayOfWeek
	/// <para>args[1] -> pos</para>
	/// </summary>
	Dungeon_Daily,
	/// <summary> args[0] -> m_Idx </summary>
	Serum_Cell,
	/// <summary> 통계 </summary>
	Serum_Statistics,
	/// <summary> args[0] -> 0 : Ready </summary>
	Cungeon_Tower_Btn,
	/// <summary> 스테이지 시너지 툴팁 </summary>
	StageSynergyToolTip,
	/// <summary> 스테이지 모드 툴팁 </summary>
	StageModeToolTip,
	/// <summary> 스테이지 디버프 툴팁 </summary>
	StageDebuffToolTip,
	/// <summary> 스테이지 배속 버튼 </summary>
	Stage_Accel,
	/// <summary> 스테이지나 타워 정보 버튼 </summary>
	Stage_Info,
	/// <summary> 추천상품 배너 </summary>
	GoodsBanner,
	/// <summary> 0:닫기,1:일일미션탭, 2:신규유저탭, 3:보상받기, 4:단계보상받기, 5:전체보상받기 6:바로가기, 7:새로고침 8 신규유저최종보상정보 </summary>
	MissionBtn,
	/// <summary> args[0] -> 0 : ClickDNAMaking, 1 : ClickZombieList, 2 : ClickAllGet, 3 : OnClose </summary>
	Item_PDA_ZombieFarm_Main,
	/// <summary> args[0] -> 0 : ClickViewRoom, 1 : ClickSetting, 2 : ClickGetReward
	/// <para>args[1] -> pos</para>
	/// </summary>
	Item_Zp_Element_Btn,
	/// <summary> args[0] -> 0 : OnClose, 1 : ClickDel </summary>
	Item_PDA_ZombieFarm_SetRoom,
	/// <summary> args[0] -> 0 : ClickSet
	/// <para>args[1] -> pos</para>
	/// </summary>
	Item_ZombieFarm_Catched_Element,
	/// <summary> args[0] -> 0 : ClickMake, 1 : ClickSelectType, 2 : ClickViewList, 3 : ClickConfirm
	/// <para>args[1] -> args[0] == 1 : pos</para>
	/// </summary>
	DNAMaking,
	/// <summary> args[0] -> 0 : OnClose </summary>
	Dungeon_Subway,
	/// <summary> args[0] -> 0 : Click_GoBuy </summary>
	Dungeon_Info,
	/// <summary> args[0] -> 0 : ClickTab, 1 : ClickRanking, 2 : ClickResearch, 3 : ClickGoPvPDeck, 4 : ClickGoBattle, 5 : ClickGetReward
	/// <para>args[1] -> args[0] == 0 : pos</para> </summary>
	PVP_Main,
	/// <summary> args[0] -> 0 : ClickSlot, 1 : ClickCharCard, 2 : ClickBtTap, 3 : ClickDeckPosSwap, 4 : ClickGoBattle, 5 : ClickGetReward
	/// <para>args[1] -> args[0] == 1 : Item_CharManageCard</para>
	/// <para>args[1] -> args[0] == 2 : pos</para>
	/// </summary>
	PVP_DeckSetting,
	/// <summary> args[0] -> 0 : Close, 1 : Click_CreateGuild, 2 : Click_Shop, 3 : Click_Search, 4 : Click_GuildDetailInfo, 5 : Click_ResetList
	/// <para>args[1] -> args[0] == 4 : RES_GUILDINFO_SIMPLE</para>
	/// </summary>
	Union_JoinList,
	/// <summary> 상점 픽업 가챠 버튼 </summary>
	PickupGacha_Btn,
	/// <summary> 피스메이커 정보 </summary>
	PieceMakerInfo,
	/// <summary> </summary>
	End
}

public enum TutoStartPos
{
	NONE = 0,
	/// <summary> 플레이화면 시작 연축 끝 </summary>
	PlayStart,
	/// <summary> 팝업 시작 </summary>
	POPUP_START,
	/// <summary> 팝업 제거 </summary>
	POPUP_REMOVE,
	/// <summary> 스테이지 시작 </summary>
	Stage,
	/// <summary> 트레이닝 시작 </summary>
	Training,
	/// <summary> PVP </summary>
	PVP,
	TitleEnd,
	End
}

public partial class TutoCheck : ClassMng
{
	//가상덱
	[JsonIgnore] public List<CharInfo> m_CloneChars = new List<CharInfo>();
	[JsonIgnore] public DeckInfo m_CloneDeck = new DeckInfo();

	public Dictionary<TutoKind, int> m_TutoState = new Dictionary<TutoKind, int>();
	[JsonIgnore] public TutoKind m_NowTuto = TutoKind.None;
	[JsonIgnore] public Dictionary<TutoKind, int> m_TutoEndValue = new Dictionary<TutoKind, int>() {
		{ TutoKind.Stage_101, (int)TutoType_Stage_101.End },
		{ TutoKind.Stage_102, (int)TutoType_Stage_102.End },
		{ TutoKind.Stage_103, (int)TutoType_Stage_103.End },
		{ TutoKind.Stage_104, (int)TutoType_Stage_104.End },
		{ TutoKind.Stage_105, (int)TutoType_Stage_105.End },
		{ TutoKind.Stage_701, (int)TutoType_Stage_701.End },
		//{ TutoKind.Stage_201, (int)TutoType_Stage_201.End },
		{ TutoKind.Stage_203, (int)TutoType_Stage_203.End },
		{ TutoKind.Stage_204, (int)TutoType_Stage_204.End },
		{ TutoKind.Stage_206, (int)TutoType_Stage_206.End },
		{ TutoKind.Stage_301, (int)TutoType_Stage_301.End },
		{ TutoKind.Stage_304, (int)TutoType_Stage_304.End },
		{ TutoKind.Stage_401, (int)TutoType_Stage_401.End },
		{ TutoKind.Stage_403, (int)TutoType_Stage_403.End },
		{ TutoKind.Stage_501, (int)TutoType_Stage_501.End },
		{ TutoKind.Stage_601, (int)TutoType_Stage_601.End },
		{ TutoKind.Stage_801, (int)TutoType_Stage_801.End },
		{ TutoKind.DeckSynergy, (int)TutoType_DeckSynergy.End },
		{ TutoKind.EquipCharLVUP, (int)TutoType_EquipCharLVUP.End },
		{ TutoKind.DeckCharInfo, (int)TutoType_DeckCharInfo.End },
		{ TutoKind.CharGradeUP, (int)TutoType_CharGradeUP.End },
		{ TutoKind.ShopSupplyBox, (int)TutoType_ShopSupplyBox.End },
		{ TutoKind.ShopEquipGacha, (int)TutoType_ShopEquipGacha.End },
		{ TutoKind.NewMission, (int)TutoType_ShopEquipGacha.End },
		{ TutoKind.Making, (int)TutoType_Making.End },
		{ TutoKind.Factory, (int)TutoType_Factory.End },
		//{ TutoKind.Academy, (int)TutoType_Academy.End },
		{ TutoKind.Bank, (int)TutoType_Bank.End },
		{ TutoKind.Research, (int)TutoType_Research.End },
		//{ TutoKind.Serum, (int)TutoType_Serum.End },
		{ TutoKind.Tower, (int)TutoType_Tower.End },
		{ TutoKind.DNA, (int)TutoType_DNA.End },
		{ TutoKind.Cemetery, (int)TutoType_Cemetery.End },
		{ TutoKind.Zombie, (int)TutoType_Zombie.End },
		{ TutoKind.Adventure, (int)TutoType_Adventure.End },
		{ TutoKind.University, (int)TutoType_University.End },
		//{ TutoKind.Hard_Stage, (int)TutoType_Hard.End },
		//{ TutoKind.Nightmare_Stage, (int)TutoType_Nightmare.End },
		{ TutoKind.UserReview, (int)TutoType_UserReview.End },
		{ TutoKind.DNA_Make, (int)TutoType_DNA_Make.End },
		{ TutoKind.Subway, (int)TutoType_Subway.End },
		{ TutoKind.PVP_Main, (int)TutoType_PVP_Main.End },
		{ TutoKind.PVP_Play, (int)TutoType_PVP_Play.End },
		{ TutoKind.Guild, (int)TutoType_Guild.End },
		{ TutoKind.PickupGacha, (int)TutoType_Pickup.End },
		{ TutoKind.Replay, (int)TutoType_Replay.End }
	};

	// 순서에 맞는 UI 셋팅을 하는 도중 연타로인해넥스트가 될 수 있으므로 임시 막기용
	[JsonIgnore] bool IsSet = false;
	[JsonIgnore] Action<int, object[]> m_Next;
	[JsonIgnore] Func<bool> m_IsCameraMove;
	[JsonIgnore] Func<TutoTouchCheckType, object[], bool> m_TouchCheckLock;

	public void SetDATA(List<LS_Web.RES_TUTOINFO> data)
	{
		m_TutoState.Clear();
		for (int i = 0; i < data.Count; i++) SetDATA(data[i]);
	}

	public void SetDATA(LS_Web.RES_TUTOINFO data)
	{
		if (m_TutoState.ContainsKey(data.Type)) m_TutoState[data.Type] = data.No;
		else m_TutoState.Add(data.Type, data.No);
	}

	public bool IsEndTuto(TutoKind kind = TutoKind.None, int no = -1)
	{
#if SKIP_TUTO || STAGE_TEST
		return true;
#else
		if (!m_TutoState.ContainsKey(kind)) m_TutoState.Add(kind, 0);
		if (no > -1) return m_TutoState[kind] > no;
		return m_TutoState[kind] >= m_TutoEndValue[kind];
#endif
	}

	/// <summary> 튜토리얼이 진행중인지 체크 </summary>
	public bool IsTutoPlay()
	{
		return m_NowTuto != TutoKind.None;
	}
	/// <summary> 튜토리얼이 진행중인지 체크 </summary>
	public bool IsTuto(TutoKind kind = TutoKind.None, int no = 0)
	{
		if (m_NowTuto == kind)
		{
			if (!m_TutoState.ContainsKey(kind)) m_TutoState.Add(kind, 0);
			if (no != 0) return no == m_TutoState[kind];
			return true;
		}
		return false;
	}
	public T GetTutoState<T>(TutoKind kind = TutoKind.None) where T : System.Enum
	{
		if (kind == TutoKind.None) kind = m_NowTuto;
		if (kind == TutoKind.None) return (T)(object)0;
		return (T)(object)m_TutoState[kind];
	}

	public bool IsStart(params object[] args) {

		if (CheckTutoStart(TutoKind.Stage_101, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_102, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_103, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_203, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_204, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_701, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Stage_104, false, args)) return true;
		else if (CheckTutoStart(TutoKind.DeckSynergy, false, args)) return true;
		else if (CheckTutoStart(TutoKind.EquipCharLVUP, false, args)) return true;
		else if (CheckTutoStart(TutoKind.DeckCharInfo, false, args)) return true;
		else if (CheckTutoStart(TutoKind.CharGradeUP, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Gacha, false, args)) return true;
		else if (CheckTutoStart(TutoKind.ShopSupplyBox, false, args)) return true;
		else if (CheckTutoStart(TutoKind.ShopEquipGacha, false, args)) return true;
		else if (CheckTutoStart(TutoKind.NewMission, false, args)) return true; 
		else if (CheckTutoStart(TutoKind.Making, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Factory, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Academy, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Bank, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Subway, false, args)) return true;
		else if (CheckTutoStart(TutoKind.PVP_Main, false, args)) return true;
		else if (CheckTutoStart(TutoKind.PVP_Play, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Guild, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Replay, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Research, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Serum, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Tower, false, args)) return true;
		else if (CheckTutoStart(TutoKind.DNA, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Cemetery, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Zombie, false, args)) return true;
		else if (CheckTutoStart(TutoKind.DNA_Make, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Adventure, false, args)) return true;
		else if (CheckTutoStart(TutoKind.University, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Hard_Stage, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Nightmare_Stage, false, args)) return true;
		else if (CheckTutoStart(TutoKind.UserReview, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_104, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_105, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.Stage_201, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_206, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_301, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_304, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_401, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_403, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_501, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_601, false, args)) return true;
		else if (CheckTutoStart(TutoKind.Stage_801, false, args)) return true;
		else if (CheckTutoStart(TutoKind.PickupGacha, false, args)) return true;
		//else if (CheckTutoStart(TutoKind.NoteBattle, false, args)) return true;
		return false;
	}

	public void Start(params object[] args) {
		if (!IsStart(args)) return;
		// outgame
		// 스테이지 진입전에 진행을 해야함
		CheckTutoStart(TutoKind.Stage_101, true, args);
		CheckTutoStart(TutoKind.Stage_102, true, args);
		CheckTutoStart(TutoKind.Stage_103, true, args);
		CheckTutoStart(TutoKind.Stage_203, true, args);
		CheckTutoStart(TutoKind.Stage_204, true, args);
		CheckTutoStart(TutoKind.Stage_701, true, args);
		CheckTutoStart(TutoKind.DeckSynergy, true, args);
		CheckTutoStart(TutoKind.EquipCharLVUP, true, args);
		CheckTutoStart(TutoKind.DeckCharInfo, true, args);
		CheckTutoStart(TutoKind.CharGradeUP, true, args);
		CheckTutoStart(TutoKind.ShopSupplyBox, true, args);
		CheckTutoStart(TutoKind.ShopEquipGacha, true, args);
		CheckTutoStart(TutoKind.NewMission, true, args); 
		CheckTutoStart(TutoKind.Making, true, args);
		CheckTutoStart(TutoKind.Factory, true, args);
		//CheckTutoStart(TutoKind.Academy, true, args);
		CheckTutoStart(TutoKind.Bank, true, args);
		CheckTutoStart(TutoKind.Subway, true, args);
		CheckTutoStart(TutoKind.PVP_Main, true, args);
		CheckTutoStart(TutoKind.Guild, true, args);
		CheckTutoStart(TutoKind.Replay, true, args);
		CheckTutoStart(TutoKind.Research, true, args);
		//CheckTutoStart(TutoKind.Serum, true, args);
		CheckTutoStart(TutoKind.Tower, true, args);
		CheckTutoStart(TutoKind.DNA, true, args);
		CheckTutoStart(TutoKind.Cemetery, true, args);
		CheckTutoStart(TutoKind.Zombie, true, args);
		CheckTutoStart(TutoKind.DNA_Make, true, args);
		CheckTutoStart(TutoKind.Adventure, true, args);
		CheckTutoStart(TutoKind.University, true, args);
		//CheckTutoStart(TutoKind.Hard_Stage, true, args);
		//CheckTutoStart(TutoKind.Nightmare_Stage, true, args);
		CheckTutoStart(TutoKind.UserReview, true, args);
		CheckTutoStart(TutoKind.PickupGacha, true, args);

		// ingame
		CheckTutoStart(TutoKind.Stage_104, true, args);
		CheckTutoStart(TutoKind.Stage_105, true, args);
		CheckTutoStart(TutoKind.Stage_206, true, args);
		CheckTutoStart(TutoKind.Stage_304, true, args);
		//CheckTutoStart(TutoKind.Stage_201, true, args);
		CheckTutoStart(TutoKind.Stage_301, true, args);
		CheckTutoStart(TutoKind.Stage_401, true, args);
		CheckTutoStart(TutoKind.Stage_403, true, args);
		CheckTutoStart(TutoKind.Stage_501, true, args);
		CheckTutoStart(TutoKind.Stage_601, true, args);
		CheckTutoStart(TutoKind.Stage_801, true, args);
		CheckTutoStart(TutoKind.PVP_Play, true, args);
		//CheckTutoStart(TutoKind.NoteBattle, true, args);
	}

#if UNITY_EDITOR
	// 튜토리얼 해당 부분부터 시작하기위해 값을 맞춰줌
	// 에디터에서만 호출해야함(Game Contol / 튜토리얼 테스트) 전용 호출
	public void RePlayInit(TutoKind kind, UserInfo info, ToolData tdata)
	{
		if (!m_TutoState.ContainsKey(kind)) m_TutoState.Add(kind, 0);
		m_TutoState[kind] = 0;
		switch (kind)
		{
			case TutoKind.Stage_101: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 101; break;
			case TutoKind.Stage_102: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 102; break;
			case TutoKind.Stage_103: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 103; break;
			case TutoKind.Stage_104: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 104; break;
			case TutoKind.Stage_105: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 105; break;
			//case TutoKind.Stage_201: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 201; break;
			case TutoKind.Stage_203: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 203; break;
			case TutoKind.Stage_204: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 204; break;
			case TutoKind.Stage_206: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 206; break;
			case TutoKind.Stage_301: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 301; break;
			case TutoKind.Stage_304: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 304; break;
			case TutoKind.Stage_401: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 401; break;
			case TutoKind.Stage_403: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 403; break;
			case TutoKind.Stage_501: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 501; break;
			case TutoKind.Stage_601: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 601; break;
			case TutoKind.Stage_701: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 701; break;
			case TutoKind.Stage_801: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 801; break;
			case TutoKind.NewMission: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 201; break;
			case TutoKind.DeckSynergy: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 304; break;
			case TutoKind.EquipCharLVUP: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.CharacterOpen); break;
			case TutoKind.DeckCharInfo: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.CharacterOpen); break;
			case TutoKind.CharGradeUP: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 402; break;
			case TutoKind.ShopSupplyBox: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.StoreOpen); break;
			case TutoKind.ShopEquipGacha: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 206; break;
			case TutoKind.Making: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.MakingOpen); break;
			case TutoKind.Factory: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.FactoryOpen); break;
			//case TutoKind.Academy: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.AcademyOpen); break;
			case TutoKind.Bank: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.BankOpen); break;
			case TutoKind.Subway: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.SubwayOpen); break;
			case TutoKind.PVP_Main: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.PvPOpen); break;
			case TutoKind.PVP_Play: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.PvPOpen); break;
			case TutoKind.Guild: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.GuildOpen); break;
			case TutoKind.Research: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.ResearchOpen); break;
			//case TutoKind.Serum: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.SerumOpen); break;
			case TutoKind.Tower: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.TowerOpen); break;
			case TutoKind.DNA: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.CharDNAOpen); break;
			case TutoKind.Cemetery: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.CemeteryOpen); break;
			case TutoKind.Zombie: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.ZombieCageOpen); break;
			case TutoKind.DNA_Make: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 806; break;
			case TutoKind.Adventure: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.ExplorerOpen); break;
			case TutoKind.University: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.UniversityOpen); break;
			//case TutoKind.Hard_Stage:
			//	// 노말 난이도상태일때만 진입할수밖에 없음
			//	PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", 0);
			//	PlayerPrefs.Save();
			//	info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.HardOpen);
			//	break;
			//case TutoKind.Nightmare_Stage:
			//	if (PlayerPrefs.GetInt($"StageDifficulty_{USERINFO.m_UID}", 0) == 2)
			//	{
			//		PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", 0);
			//		PlayerPrefs.Save();
			//	}
			//	info.m_Stage[StageContentType.Stage].Idxs[1].Idx = tdata.GetConfig_Int32(ConfigType.NightmareOpen);
			//	break;
			case TutoKind.PickupGacha: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = 610; break;
			case TutoKind.Replay: info.m_Stage[StageContentType.Stage].Idxs[0].Idx = tdata.GetConfig_Int32(ConfigType.ReplayOpen); break;
		}
	}
#endif

#pragma warning disable 0162
	public bool CheckTutoStart(TutoKind kind, bool IsPlay, params object[] args)
	{
#if SKIP_TUTO || STAGE_TEST
		return false;
#endif
		if (IsTutoPlay()) return false;
		if (IsEndTuto(kind)) return false;
		if (!m_TutoState.ContainsKey(kind)) m_TutoState.Add(kind, 0);

		TutoStartPos pos = (TutoStartPos)args[0];

		// 흐름대로 진행되었다면 꼭 할 필요는 없지만 스테이지같은 경우는 해당 스테이지에서만 발동되도록 해야함
		switch(kind)
		{
			case TutoKind.Stage_101:
				if (pos != TutoStartPos.TitleEnd) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 101) return false;
				break;
			case TutoKind.Stage_102:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 102) return false;
				break;
			case TutoKind.Stage_103:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 103) return false;
				break;
			case TutoKind.Stage_104:
				if (pos != TutoStartPos.Stage) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 104 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			//case TutoKind.Stage_201:
			//	if (pos != TutoStartPos.Stage) return false;
			//	if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 201 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
			//	break;
			case TutoKind.Stage_105:
				if (pos != TutoStartPos.Stage) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 105 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_203:
				if (pos != TutoStartPos.Stage) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 203 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			//case TutoKind.Stage_203:
			//	if (pos != TutoStartPos.POPUP_START) return false;
			//	if ((PopupName)args[1] != PopupName.DeckSetting) return false;
			//	if (((TStageTable)((object[])args[2])[0]).m_Idx != 203) return false;
			//	if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 203 || POPUP.GetMainUI().GetComponent<Main_Play>().m_State != MainMenuType.Stage) return false;
			//	break;
			case TutoKind.Stage_204:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.DeckSetting) return false;
				if (((TStageTable)((object[])args[2])[0]).m_Idx != 204) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 204 || POPUP.GetMainUI().GetComponent<Main_Play>().m_State != MainMenuType.Stage) return false;
				break;
			case TutoKind.Stage_206:
				if (pos != TutoStartPos.Stage) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 206 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_301:
				if (pos != TutoStartPos.Stage) return false;
				//if (MAIN.m_State != MainState.STAGE) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 301 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_304:
				if (pos != TutoStartPos.Stage) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 304 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_401:
				if (pos != TutoStartPos.Stage) return false;
				//if (MAIN.m_State != MainState.STAGE) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 401 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_403:
				if (pos != TutoStartPos.Stage) return false;
				//if (MAIN.m_State != MainState.STAGE) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 403 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_501:
				if (pos != TutoStartPos.Stage) return false;
				//if (MAIN.m_State != MainState.STAGE) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 501 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_601:
				if (pos != TutoStartPos.Stage) return false;
				//if (MAIN.m_State != MainState.STAGE) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 601 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.Stage_701:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.DeckSetting) return false;
					//if (MAIN.m_State != MainState.STAGE) return false;
				if (((TStageTable)((object[])args[2])[0]).m_Idx != 701) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 701 || POPUP.GetMainUI().GetComponent<Main_Play>().m_State != MainMenuType.Stage) return false;
				break;
			case TutoKind.Stage_801:
				if (pos != TutoStartPos.Stage) return false;
				//if (MAIN.m_State != MainState.STAGE) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 801 || STAGEINFO.m_StageContentType != StageContentType.Stage) return false;
				break;
			case TutoKind.NewMission:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 201) return false;
				if (USERINFO.m_Mission.Get_Missions(MissionMode.BeginnerQuest).Count < 1) return false;
				break;
			case TutoKind.DeckSynergy:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.DeckSetting) return false;
				if (((TStageTable)((object[])args[2])[0]).m_Idx != 304) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 304 || POPUP.GetMainUI().GetComponent<Main_Play>().m_State != MainMenuType.Stage) return false;
				break;
			case TutoKind.EquipCharLVUP:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Character)) return false;
				break;
			case TutoKind.DeckCharInfo:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.DeckSetting) return false;
				if (((TStageTable)((object[])args[2])[0]).m_Idx != 203) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 203 || POPUP.GetMainUI().GetComponent<Main_Play>().m_State != MainMenuType.Stage) return false;
				break;
			case TutoKind.CharGradeUP:
				if (pos != TutoStartPos.PlayStart) return false;
				CharInfo charinfo = USERINFO.GetChar(1021);
				if (charinfo != null && charinfo.m_Grade > charinfo.m_TData.m_Grade) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 402) return false;
				break;
			case TutoKind.ShopSupplyBox:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Store)) return false;
				break;
			case TutoKind.ShopEquipGacha:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 206) return false;
				break; 
			case TutoKind.Making:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Making)) return false;
				break;
			case TutoKind.Factory:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.Dungeon_Factory) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.Factory)) return false;
				//if (pos != TutoStartPos.PlayStart) return false;
				//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Factory)) return false;
				break;
			//case TutoKind.Academy:
			//	if (pos != TutoStartPos.POPUP_START) return false;
			//	if ((PopupName)args[1] != PopupName.Dungeon_Training) return false;
			//	if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.Academy)) return false;
				break;
			case TutoKind.Bank:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.Dungeon_Bank) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.Bank)) return false;
				//if (pos != TutoStartPos.PlayStart) return false;
				//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Bank)) return false;
				break;
			case TutoKind.Subway:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.Dungeon_Subway) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.Subway)) return false;
				//if (pos != TutoStartPos.PlayStart) return false;
				//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Subway)) return false;
				break;
			case TutoKind.PVP_Main:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.PvP)) return false;
				break;
			case TutoKind.PVP_Play:
				if (pos != TutoStartPos.PVP) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.PvP)) return false;
				break;
			case TutoKind.Guild:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Guild.UID != 0) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Guild)) return false;
				break;
			case TutoKind.Research:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Research)) return false;
				break;
			//case TutoKind.Serum:
			//	if (pos != TutoStartPos.PlayStart) return false;
			//	if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Serum)) return false;
			//	break;
			case TutoKind.Tower:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.Dungeon_Tower) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.Tower)) return false;
				//if (pos != TutoStartPos.PlayStart) return false;
				//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Tower)) return false;
				break;
			case TutoKind.DNA:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA)) return false;
				break;
			case TutoKind.Cemetery:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.Dungeon_Cemetery) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.Cemetery)) return false;
				//if (pos != TutoStartPos.PlayStart) return false;
				//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Cemetery)) return false;
				break;
			case TutoKind.Zombie:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.ZombieFarm)) return false;
				break;
			case TutoKind.DNA_Make:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1) return false;
				break;
			case TutoKind.Adventure:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.Explorer)) return false;
				break;
			case TutoKind.University:
				if (pos != TutoStartPos.POPUP_START) return false;
				if ((PopupName)args[1] != PopupName.Dungeon_University) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.CONTENT_OPEN_IDX(ContentType.University)) return false;
				//if (pos != TutoStartPos.PlayStart) return false;
				//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.CONTENT_OPEN_IDX(ContentType.University)) return false;
				break;
			//case TutoKind.Hard_Stage:
			//	if (pos != TutoStartPos.PlayStart) return false;
			//	if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.HARD_OPEN) return false;
			//	break;
			//case TutoKind.Nightmare_Stage:
			//	if (pos != TutoStartPos.PlayStart) return false;
			//	// 나이트메어 스테이지로 확인
			//	if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.NIGHTMARE_OPEN) return false;
			//	break;
			case TutoKind.UserReview:
				if (pos != TutoStartPos.PlayStart) return false;
				//if (pos != TutoStartPos.NONE) return false;
				// 304 가챠 튜토리얼을 끝내고 메인화면 진입하였을 때 출력한다
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < 306) return false;
				if (!MAIN.IS_State(MainState.PLAY)) return false;
				if (((Main_Play)POPUP.GetMainUI()).m_State != MainMenuType.Stage) return false;
				break;
			case TutoKind.PickupGacha:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != 610) return false;
				break;
			case TutoKind.Replay:
				if (pos != TutoStartPos.PlayStart) return false;
				if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx != BaseValue.REPLAY_OPEN) return false;
				break;
		}

		if (IsPlay)
		{
			m_TutoState[kind] = 0;
			StartTuto(kind);
		}
		if (POPUP.IS_MsgUI()) {
			PopupBase msg = POPUP.GetMsgBox();
			switch (msg.m_Popup) {
				case PopupName.Msg_Store_GatchaMileage_Alarm:
					msg.Close();
					break;
			}
		}
		return true;
	}
#pragma warning restore 0162
	public void StartTuto(TutoKind kind)
	{
		m_NowTuto = kind;
		switch (m_NowTuto)
		{
		case TutoKind.Stage_101:
			m_Next = PlayTuto_Stage_101;
			m_IsCameraMove = IsCameraMove_Stage_101;
			m_TouchCheckLock = TouchCheckLock_Stage_101;
			break;
		case TutoKind.Stage_102:
			m_Next = PlayTuto_Stage_102;
			m_IsCameraMove = IsCameraMove_Stage_102;
			m_TouchCheckLock = TouchCheckLock_Stage_102;
			break;
		case TutoKind.Stage_103:
			m_Next = PlayTuto_Stage_103;
			m_IsCameraMove = IsCameraMove_Stage_103;
			m_TouchCheckLock = TouchCheckLock_Stage_103;
			break;
		case TutoKind.Stage_104:
			m_Next = PlayTuto_Stage_104;
			m_IsCameraMove = IsCameraMove_Stage_104;
			m_TouchCheckLock = TouchCheckLock_Stage_104;
			break;
		case TutoKind.Stage_105:
			m_Next = PlayTuto_Stage_105;
			m_IsCameraMove = IsCameraMove_Stage_105;
			m_TouchCheckLock = TouchCheckLock_Stage_105;
			break;
		//case TutoKind.Stage_201:
		//	m_Next = PlayTuto_Stage_201;
		//	m_IsCameraMove = IsCameraMove_Stage_201;
		//	m_TouchCheckLock = TouchCheckLock_Stage_201;
		//	break;
		case TutoKind.Stage_203:
			m_Next = PlayTuto_Stage_203;
			m_IsCameraMove = IsCameraMove_Stage_203;
			m_TouchCheckLock = TouchCheckLock_Stage_203;
			break;
		case TutoKind.Stage_204:
			m_Next = PlayTuto_Stage_204;
			m_IsCameraMove = IsCameraMove_Stage_204;
			m_TouchCheckLock = TouchCheckLock_Stage_204;
			break;
		case TutoKind.Stage_206:
			m_Next = PlayTuto_Stage_206;
			m_IsCameraMove = IsCameraMove_Stage_206;
			m_TouchCheckLock = TouchCheckLock_Stage_206;
			break;
		case TutoKind.Stage_301:
			m_Next = PlayTuto_Stage_301;
			m_IsCameraMove = IsCameraMove_Stage_301;
			m_TouchCheckLock = TouchCheckLock_Stage_301;
			break;
		case TutoKind.Stage_304:
			m_Next = PlayTuto_Stage_304;
			m_IsCameraMove = IsCameraMove_Stage_304;
			m_TouchCheckLock = TouchCheckLock_Stage_304;
			break;
		case TutoKind.Stage_401:
			m_Next = PlayTuto_Stage_401;
			m_IsCameraMove = IsCameraMove_Stage_401;
			m_TouchCheckLock = TouchCheckLock_Stage_401;
			break;
		case TutoKind.Stage_403:
			m_Next = PlayTuto_Stage_403;
			m_IsCameraMove = IsCameraMove_Stage_403;
			m_TouchCheckLock = TouchCheckLock_Stage_403;
			break;
		case TutoKind.Stage_501:
			m_Next = PlayTuto_Stage_501;
			m_IsCameraMove = IsCameraMove_Stage_501;
			m_TouchCheckLock = TouchCheckLock_Stage_501;
			break;
		case TutoKind.Stage_601:
			m_Next = PlayTuto_Stage_601;
			m_IsCameraMove = IsCameraMove_Stage_601;
			m_TouchCheckLock = TouchCheckLock_Stage_601;
			break;
		case TutoKind.Stage_701:
			m_Next = PlayTuto_Stage_701;
			m_IsCameraMove = IsCameraMove_Stage_701;
			m_TouchCheckLock = TouchCheckLock_Stage_701;
			break;
		case TutoKind.Stage_801:
			m_Next = PlayTuto_Stage_801;
			m_IsCameraMove = IsCameraMove_Stage_801;
			m_TouchCheckLock = TouchCheckLock_Stage_801;
			break;
		case TutoKind.NewMission:
			m_Next = PlayTuto_NewMission;
			m_IsCameraMove = IsCameraMove_NewMission;
			m_TouchCheckLock = TouchCheckLock_NewMission;
			break;
		case TutoKind.DeckSynergy:
			m_Next = PlayTuto_DeckSynergy;
			m_IsCameraMove = IsCameraMove_DeckSynergy;
			m_TouchCheckLock = TouchCheckLock_DeckSynergy;
			break;
		case TutoKind.EquipCharLVUP:
			m_Next = PlayTuto_EquipCharLVUP;
			m_IsCameraMove = IsCameraMove_EquipCharLVUP;
			m_TouchCheckLock = TouchCheckLock_EquipCharLVUP;
			break;
		case TutoKind.DeckCharInfo:
			m_Next = PlayTuto_DeckCharInfo;
			m_IsCameraMove = IsCameraMove_DeckCharInfo;
			m_TouchCheckLock = TouchCheckLock_DeckCharInfo;
			break;
		case TutoKind.CharGradeUP:
			m_Next = PlayTuto_CharGradeUP;
			m_IsCameraMove = IsCameraMove_CharGradeUP;
			m_TouchCheckLock = TouchCheckLock_CharGradeUP;
			break;
		case TutoKind.ShopSupplyBox:
			m_Next = PlayTuto_ShopSupplyBox;
			m_IsCameraMove = IsCameraMove_ShopSupplyBox;
			m_TouchCheckLock = TouchCheckLock_ShopSupplyBox;
			break;
		case TutoKind.ShopEquipGacha:
			m_Next = PlayTuto_ShopEquipGacha;
			m_IsCameraMove = IsCameraMove_ShopEquipGacha;
			m_TouchCheckLock = TouchCheckLock_ShopEquipGacha;
			break;
		case TutoKind.Making:
			m_Next = PlayTuto_Making;
			m_IsCameraMove = IsCameraMove_Making;
			m_TouchCheckLock = TouchCheckLock_Making;
			break;
		case TutoKind.Factory:
			m_Next = PlayTuto_Factory;
			m_IsCameraMove = IsCameraMove_Factory;
			m_TouchCheckLock = TouchCheckLock_Factory;
			break;
		//case TutoKind.Academy:
		//	m_Next = PlayTuto_Academy;
		//	m_IsCameraMove = IsCameraMove_Academy;
		//	m_TouchCheckLock = TouchCheckLock_Academy;
		//	break;
		case TutoKind.Bank:
			m_Next = PlayTuto_Bank;
			m_IsCameraMove = IsCameraMove_Bank;
			m_TouchCheckLock = TouchCheckLock_Bank;
			break;
		case TutoKind.Research:
			m_Next = PlayTuto_Research;
			m_IsCameraMove = IsCameraMove_Research;
			m_TouchCheckLock = TouchCheckLock_Research;
			break;
		//case TutoKind.Serum:
		//	m_Next = PlayTuto_Serum;
		//	m_IsCameraMove = IsCameraMove_Serum;
		//	m_TouchCheckLock = TouchCheckLock_Serum;
		//	break;
		case TutoKind.Tower:
			m_Next = PlayTuto_Tower;
			m_IsCameraMove = IsCameraMove_Tower;
			m_TouchCheckLock = TouchCheckLock_Tower;
			break;
		case TutoKind.DNA:
			m_Next = PlayTuto_DNA;
			m_IsCameraMove = IsCameraMove_DNA;
			m_TouchCheckLock = TouchCheckLock_DNA;
			break;
		case TutoKind.Cemetery:
			m_Next = PlayTuto_Cemetery;
			m_IsCameraMove = IsCameraMove_Cemetery;
			m_TouchCheckLock = TouchCheckLock_Cemetery;
			break;
		case TutoKind.Zombie:
			m_Next = PlayTuto_Zombie;
			m_IsCameraMove = IsCameraMove_Zombie;
			m_TouchCheckLock = TouchCheckLock_Zombie;
			break;
		case TutoKind.DNA_Make:
			m_Next = PlayTuto_DNA_Make;
			m_IsCameraMove = IsCameraMove_DNA_Make;
			m_TouchCheckLock = TouchCheckLock_DNA_Make;
			break;
		case TutoKind.Adventure:
			m_Next = PlayTuto_Adventure;
			m_IsCameraMove = IsCameraMove_Adventure;
			m_TouchCheckLock = TouchCheckLock_Adventure;
			break;
		case TutoKind.University:
			m_Next = PlayTuto_University;
			m_IsCameraMove = IsCameraMove_University;
			m_TouchCheckLock = TouchCheckLock_University;
			break;
		//case TutoKind.Hard_Stage:
		//	m_Next = PlayTuto_Hard;
		//	m_IsCameraMove = IsCameraMove_Hard;
		//	m_TouchCheckLock = TouchCheckLock_Hard;
		//	break;
		//case TutoKind.Nightmare_Stage:
		//	m_Next = PlayTuto_Nightmare;
		//	m_IsCameraMove = IsCameraMove_Nightmare;
		//	m_TouchCheckLock = TouchCheckLock_Nightmare;
		//	break;
		case TutoKind.UserReview:
			m_Next = PlayTuto_UserReview;
			m_IsCameraMove = IsCameraMove_UserReview;
			m_TouchCheckLock = TouchCheckLock_UserReview;
			break;
		case TutoKind.Subway:
			m_Next = PlayTuto_Subway;
			m_IsCameraMove = IsCameraMove_Subway;
			m_TouchCheckLock = TouchCheckLock_Subway;
			break;
		case TutoKind.PVP_Main:
			m_Next = PlayTuto_PVP_Main;
			m_IsCameraMove = IsCameraMove_PVP_Main;
			m_TouchCheckLock = TouchCheckLock_PVP_Main;
			break;
		case TutoKind.PVP_Play:
			m_Next = PlayTuto_PVP_Play;
			m_IsCameraMove = IsCameraMove_PVP_Play;
			m_TouchCheckLock = TouchCheckLock_PVP_Play;
			break;
		case TutoKind.Guild:
			m_Next = PlayTuto_Guild;
			m_IsCameraMove = IsCameraMove_Guild;
			m_TouchCheckLock = TouchCheckLock_Guild;
			break;
		case TutoKind.PickupGacha:
			m_Next = PlayTuto_Pickup;
			m_IsCameraMove = IsCameraMove_Pickup;
			m_TouchCheckLock = TouchCheckLock_Pickup;
			break;
		case TutoKind.Replay:
			m_Next = PlayTuto_Replay;
			m_IsCameraMove = IsCameraMove_Replay;
			m_TouchCheckLock = TouchCheckLock_Replay;
			break;
		}

		Next(kind);
	}

	public void Next(params object[] args)
	{
		IsSet = false;
		POPUP.StopTutoTimer();
		m_TutoState[m_NowTuto]++;
		m_Next?.Invoke(m_TutoState[m_NowTuto], args);
		IsSet = true;
	}

	void SetTutoEnd()
	{
#if !NOT_USE_NET
		// 튜토리얼 종료는 결과를 보내고 데이터를 따로 받지 않는다.
		WEB.SEND_REQ_TUTOEND((res) =>
		{
			if (!res.IsSuccess())
			{
				MAIN.StopAllCoroutines();
				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
					// 로그인부터 다시 시도한다.
					MAIN.ReStart();
				});
				return;
			}
			TUTO.SetDATA(res);
		}, m_NowTuto, (int)m_TutoEndValue[m_NowTuto]);
#endif
		// 통신요청 이전으로 보내지 말것!!!
		// 안에서 m_NowTuto 값변경함
		SetEnd();
	}

	void SetEnd()
	{
		m_TutoState[m_NowTuto] = m_TutoEndValue[m_NowTuto];
		m_NowTuto = TutoKind.None;
		POPUP.RemoveTutoUI();
		MAIN.Save_UserInfo();
		m_Next = null;
		m_IsCameraMove = null;
		m_TouchCheckLock = null;
	}
	public void SetReset() {
		m_TutoState[m_NowTuto] = 0;
		m_NowTuto = TutoKind.None;
		POPUP.RemoveTutoUI();
		MAIN.Save_UserInfo();
		m_Next = null;
		m_IsCameraMove = null;
		m_TouchCheckLock = null;
	}
	public bool IsCameraMove()
	{
		if (!IsTutoPlay()) return true;
		if (m_IsCameraMove == null) return true;
		return m_IsCameraMove.Invoke();
	}
	IEnumerator m_TouchDelay;

	void Start_ClickDelay()
	{
		m_TouchDelay = ClickDelay();
		MAIN.StartCoroutine(m_TouchDelay);
	}

	IEnumerator ClickDelay()
	{
		// 3D 좌표 클릭으로인해 바로 넘어가는 경우가있어 한프레임 쉬게해주기 위해 호출해줌
		yield return new WaitForEndOfFrame();
		m_TouchDelay = null;
	}

	public bool TouchCheckLock(TutoTouchCheckType checktype, params object[] args)
	{
		if (!IsTutoPlay()) return false;
		if (m_TouchDelay != null) return true;
		if (!m_TutoState.ContainsKey(m_NowTuto)) m_TutoState.Add(m_NowTuto, 0);
		if (m_TouchCheckLock == null) return false;
		if (!IsSet) return true;
		return m_TouchCheckLock.Invoke(checktype, args);
	}

	public void SetCloneDeck() {
		CharInfo info = null;
		int[] Idxs = new int[3] { 1001, 1003, 1056 };//특정 튜토마다 세팅 다르게
		for(int i = 0; i < Idxs.Length; i++) {
			info = m_CloneChars.Find(t => t.m_Idx == Idxs[i]);
			if(info == null) {
				info = new CharInfo(Idxs[i], 0, 0, 1);
				info.m_UID = i + 1;
				m_CloneChars.Add(info);
			}
		}
		for(int i = 0;i< m_CloneChars.Count; i++) {
			m_CloneDeck.SetChar(i, m_CloneChars[i].m_UID, m_CloneChars);
		}
	}

	public bool CheckUseCloneDeck()
	{
#if SKIP_TUTO || STAGE_TEST
		return false;
#else
		//107끝나고 결과창까지는 가상덱 보여줘야해서 201로 체크하는 Equip 시작부터는 가상덱 사용안함.
		//if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < 201) return true;
		//else if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx == 201 && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].ChapterReward != 0) return true;
		return false;
#endif
	}
}
