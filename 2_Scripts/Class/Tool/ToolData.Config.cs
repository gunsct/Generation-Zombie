using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public enum ConfigType
{
	None = 0,
	/// <summary> 캐릭터 에너지 회복 요구 시간(분) </summary>
	EnergyRecoveryCycle,
	/// <summary> 캐릭터 에너지 회복량 </summary>
	EnergyRecoveryValue,
	/// <summary> 회피시 스태미나 소모량 </summary>
	EVA_STAMINA_VALUE,
	/// <summary> 회피 유지시 스태미나 소모량 </summary>
	EVA_STAMINA_KEEP_VALUE,
	/// <summary> 몬스터 최소 최대 레벨 격차 </summary>
	EnemyLevelRange,
	/// <summary> 최대 레벨 </summary>
	MaxLevel,
	/// <summary> 피난민 포만감 회복량(절대) </summary>
	SatRefugeeCharge,
	/// <summary> 피난민 정신력 회복량(절대) </summary>
	MenRefugeeCharge,
	/// <summary> 청결도 회복량(절대) </summary>
	HygRefugeeCharge,
	/// <summary> HP 회복량 (비율) </summary>
	HpRefugeeCharge,
	/// <summary> 랜덤 생존 스탯 4종 회복량 (비율) </summary>
	RandomRefugeeCharge,
	/// <summary> 생존 스탯4종 (비율) </summary>
	AllRefugeeCharge,
	/// <summary> 캐시 인덱스 </summary>
	CASH_IDX,
	/// <summary> 달러 인덱스 </summary>
	DOLLAR_IDX,
	/// <summary> 경험치 인덱스 </summary>
	EXP_IDX,
	/// <summary> 에너지 인덱스 </summary>
	ENERGY_IDX,
	/// <summary> 인벤 확장 아이템 Index </summary>
	INVEN_IDX,
	/// <summary> 유저 레벨 최대치 </summary>
	UserMaxLevel,
	/// <summary> 캐릭터 랭크 최대치 </summary>
	CharMaxRank,
	/// <summary> 경험치 1당 요구하는 달러 </summary>
	Item_Exp_Money,
	/// <summary> 옵션이 1개 붙는 등급 </summary>
	RandomStat_Grade1,
	/// <summary> 옵션이 1개 붙는 등급 </summary>
	RandomStat_Grade2,
	/// <summary> 옵션이 1개 붙는 등급 </summary>
	RandomStat_Grade3,
	/// <summary> 전투력공식에서 체력 반영 보정수치 </summary>
	HPRevision,
	/// <summary> 전투력공식에서 공격력 반영 보정수치 </summary>
	AtkRevision,
	/// <summary> 전투력공식에서 방어력 반영 보정수치 </summary>
	DefRevision,
	/// <summary> 전투력공식에서 회복력 반영 보정수치 </summary>
	HealRevision,
	/// <summary> 전투력공식에서 기력 반영 보정수치 </summary>
	StaRevision,
	/// <summary> 전투력공식에서 기력회복 반영 보정수치 </summary>
	StaRecRevision,
	/// <summary> 전투력공식에서 기력소모 반영 보정수치 </summary>
	StaConRevision,
	/// <summary> 전투력공식에서 블록 반영 보정수치 </summary>
	BlockRevision,
	/// <summary> 전투력공식에서 기본노트공격 반영 보정수치 </summary>
	NormalNoteRevision,
	/// <summary> 전투력공식에서 베기노트공격 반영 보정수치 </summary>
	SlashNoteRevision,
	/// <summary> 전투력공식에서 연타노트공격 반영 보정수치 </summary>
	ComboNoteRevision,
	/// <summary> 전투력공식에서 챠지노트공격 반영 보정수치 </summary>
	ChargeNoteRevision,
	/// <summary> 전투력공식에서 콤보노트공격 반영 보정수치 </summary>
	ChainNoteRevision,
	/// <summary> 전투력공식에서 청결도 반영 보정수치 </summary>
	HygRevision,
	/// <summary> 전투력공식에서 포만감 반영 보정수치 </summary>
	SatRevision,
	/// <summary> 전투력공식에서 정신력 반영 보정수치 </summary>
	MenRevision,
	/// <summary> 사관학교(Training) 참여 제한 </summary>
	Mode_Academy_Count,
	/// <summary> 로지은행(Bank_NoteBattle) 참여 제한 </summary>
	Mode_Bank_Count,
	/// <summary> 허버트대학교(Univ-요일) 참여 제한 </summary>
	Mode_University_Count,
	/// <summary> 공동묘지(Cemetery) 참여 제한 </summary>
	Mode_Cemetery_Count,
	/// <summary> 댄스타디움(Factory) 참여 제한 </summary>
	Mode_Factory_Count,
	/// <summary> 고든타워(Tower) 참여 제한 </summary>
	Mode_Tower_Count,
	/// <summary> 지하철 참여 제한 </summary>
	Mode_Subway_Count,
	/// <summary> 1성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap1,
	/// <summary> 2성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap2,
	/// <summary> 3성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap3,
	/// <summary> 4성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap4,
	/// <summary> 5성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap5,
	/// <summary> 6성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap6,
	/// <summary> 7성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap7,
	/// <summary> 8성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap8,
	/// <summary> 9성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap9,
	/// <summary> 10성이 겹쳐서 나왔을 때 영혼석 개수 </summary>
	StarOverlap10,
	/// <summary> 상점 오픈 스테이지 </summary>
	StoreOpen,
	/// <summary> 제작 오픈 스테이지 </summary>
	MakingOpen,
	/// <summary> 공동묘지(Cemetery) 오픈 스테이지 </summary>
	CemeteryOpen,
	/// <summary> 로지은행, 일일 던전 - 달러 오픈 스테이지 </summary>
	BankOpen,
	/// <summary> 고든타워(Tower) 오픈 스테이지 </summary>
	TowerOpen,
	/// <summary> 허버트대학교(Univ-요일) 던전 오픈 스테이지 </summary>
	UniversityOpen,
	/// <summary> 댄스타디움(Factory) 오픈 스테이지 </summary>
	FactoryOpen,
	/// <summary> 지하철(Subway) 오픈 스테이지 </summary>
	SubwayOpen,
	/// <summary> 연구 오픈 스테이지 </summary>
	ResearchOpen,
	/// <summary> 캐릭터 창 오픈 스테이지 </summary>
	CharacterOpen,
	/// <summary> 탐험 오픈 스테이지 </summary>
	ExplorerOpen,
	/// <summary> 방어율 최대 조정 비율 </summary>
	MaxDefRate,
	/// <summary> 스테이지 에너지 최대 보유량(시간으로는 해당 개수만큼만 채울 수 있음) </summary>
	StageEnergyMax,
	/// <summary> 스테이지 에너지 1당 시간(초) </summary>
	StageEnergyTime,
	/// <summary> 기본 행동력 </summary>
	BasicAP,
	/// <summary> 암시장 할인율 20% </summary>
	BlackMarketDiscount1,
	/// <summary> 암시장 할인율 40% </summary>
	BlackMarketDiscount2,
	/// <summary> 암시장 할인율 60% </summary>
	BlackMarketDiscount3,
	/// <summary> 암시장 할인율 80% </summary>
	BlackMarketDiscount4,
	/// <summary> 암시장 갱신 시간 </summary>
	BlackMarketRefreshTime,
	/// <summary> 암시장에서 판매할 물건 개수 </summary>
	BlackMarket_Product,
	/// <summary> 1회 (단챠) 당 증가하는 천장 포인트 </summary>
	GachaPoint1,
	/// <summary> 10회 (10연챠) 시 추가 지급 포인트 </summary>
	GachaPoint2,
	/// <summary> 천장 포인트 소모량 </summary>
	UseGachaPoint,
	/// <summary> 시간 가속 금니(1분당) </summary>
	TimePrice,
	/// <summary> 기본 인벤토리 개수 </summary>
	BaseInvenCount,
	/// <summary> 아이템 누적 수량 </summary>
	ItemMaxCount,
	/// <summary> 탐험 기본 출력 개수 </summary>
	AdventureBaseCount,
	/// <summary> 탐험 누적 출력 배율 </summary>
	AdventureWaitingRatio,
	/// <summary> 턴당 자동 체력 회복 비율 </summary>
	AutoHPRecvTurnRatio,
	/// <summary> 틱당(노트전투) 자동 체력 회복 비율 </summary>
	AutoHPRecvTickRatio,
	/// <summary> 전용장비에 붙는 추가 전투력 </summary>
	SpecialStatBenefit,
	/// <summary> 전투보상 최대 레벨 </summary>
	InGameRewardMaxLevel,
	/// <summary> 초반 지급 캐릭터 1 </summary>
	StartChar1,
	/// <summary> 초반 지급 캐릭터 2 </summary>
	StartChar2,
	/// <summary> 초반 지급 캐릭터 3 </summary>
	StartChar3,
	/// <summary> 4번째 캐릭터 슬롯 해금 스테이지 (입력한 스테이지 클리어 이후 해금) </summary>
	SlotUnlock1,
	/// <summary> 5번째 캐릭터 슬롯 해금 스테이지 (입력한 스테이지 클리어 이후 해금) </summary>
	SlotUnlock2,
	/// <summary> 전투 시 감소되는 포만도 수치(절대값) </summary>
	ReductionSat,
	/// <summary> 장비 최고 레벨 </summary>
	EquipMaxLevel,
	/// <summary> Power 배율 </summary>
	BPMagnification,
	/// <summary> Power 갭차 </summary>
	BPGap,
	/// <summary> Power 기준 레벨 </summary>
	BPReferLevel,
	/// <summary> Power 기준 레벨 </summary>
	BPCorrectionValue,
	/// <summary> 등급1 장비 최대 레벨 </summary>
	EquipGrade1MaxLevel,
	/// <summary> 등급2 장비 최대 레벨 </summary>
	EquipGrade2MaxLevel,
	/// <summary> 등급3 장비 최대 레벨 </summary>
	EquipGrade3MaxLevel,
	/// <summary> 등급4 장비 최대 레벨 </summary>
	EquipGrade4MaxLevel,
	/// <summary> 등급5 장비 최대 레벨 </summary>
	EquipGrade5MaxLevel,
	/// <summary> 등급6 장비 최대 레벨 </summary>
	EquipGrade6MaxLevel,
	/// <summary> 등급7 장비 최대 레벨 </summary>
	EquipGrade7MaxLevel,
	/// <summary> 등급8 장비 최대 레벨 </summary>
	EquipGrade8MaxLevel,
	/// <summary> 등급9 장비 최대 레벨 </summary>
	EquipGrade9MaxLevel,
	/// <summary> 등급10 장비 최대 레벨 </summary>
	EquipGrade10MaxLevel,
	/// <summary> NPC간 전투 데미지 증가 </summary>
	NpctoNpcDmg,
	/// <summary> 등급 1의 장비를 2로 상승 시킬 때의 요구 달러 </summary>
	Grade1UpPrice,
	/// <summary> 등급 2의 장비를 3로 상승 시킬 때의 요구 달러 </summary>
	Grade2UpPrice,
	/// <summary> 등급 3의 장비를 4로 상승 시킬 때의 요구 달러 </summary>
	Grade3UpPrice,
	/// <summary> 등급 4의 장비를 5로 상승 시킬 때의 요구 달러 </summary>
	Grade4UpPrice,
	/// <summary> 등급 5의 장비를 6로 상승 시킬 때의 요구 달러 </summary>
	Grade5UpPrice,
	/// <summary> 등급 6의 장비를 7로 상승 시킬 때의 요구 달러 </summary>
	Grade6UpPrice,
	/// <summary> 등급 7의 장비를 8로 상승 시킬 때의 요구 달러 </summary>
	Grade7UpPrice,
	/// <summary> 등급 8의 장비를 9로 상승 시킬 때의 요구 달러 </summary>
	Grade8UpPrice,
	/// <summary> 등급 9의 장비를 10로 상승 시킬 때의 요구 달러 </summary>
	Grade9UpPrice,
	/// <summary> 레벨 1당 붙는 달러 </summary>
	ReassemblyPrice,
	/// <summary> 슬롯 오픈 시 확률 </summary>
	Slot1OpenProb,
	/// <summary> 슬롯 오픈 시 확률 </summary>
	Slot2OpenProb,
	/// <summary> 슬롯 오픈 시 확률 </summary>
	Slot3OpenProb,
	/// <summary> 슬롯 오픈 시 확률 </summary>
	Slot4OpenProb,
	/// <summary> 슬롯 오픈 시 확률 </summary>
	Slot5OpenProb,
	/// <summary> 사관학교(Training) 오픈스테이지 </summary>
	AcademyOpen,
	/// <summary> 좀비 사육장 슬롯 </summary>
	StartZombieSlot,
	/// <summary> 나이트메어 오픈 스테이지 </summary>
	HardOpen,
	/// <summary> 아포칼립스 오픈 스테이지 </summary>
	NightmareOpen,
	/// <summary> 나이트메어 스테이지에서 전투 시 감소되는 포만도 수치 </summary>
	HardReductionSat,
	/// <summary> 아포칼립스 스테이지에서 전투 시 감소되는 포만도 수치 </summary>
	NightmareReductionSat,
	/// <summary> 난이도 스테이지 턴 당 자동 체력 회복값 </summary>
	HardNightAutoHPRecvTurnRatio,
	/// <summary> 난이도 스테이지 노트전투 0.5초 당 자동 체력 회복값 </summary>
	HardNightAutoHPRecvTickRatio,
	/// <summary> 좀비 실험실 오픈스테이지 </summary>
	ZombieCageOpen,
	/// <summary> 최대 좀비 케이지 수 </summary>
	ZombieCageMax,
	/// <summary> 유저 최초 프로필이미지 </summary>
	FirstUserProfile,
	/// <summary> 비하인드 스토리 해금 등급 </summary>
	UnlockBehindSlotGrade01,
	/// <summary> 비하인드 스토리 해금 등급 </summary>
	UnlockBehindSlotGrade02,
	/// <summary> 비하인드 스토리 해금 등급 </summary>
	UnlockBehindSlotGrade03,
	/// <summary> 비하인드 스토리 해금 등급 </summary>
	UnlockBehindSlotGrade04,
	/// <summary> 비하인드 스토리 해금 등급 </summary>
	UnlockBehindSlotGrade05,
	/// <summary> 비하인드 스토리 01 슬롯 해제 시 지급되는 금니 개수 </summary>
	RewardBehindStory01,
	/// <summary> 비하인드 스토리 02 슬롯 해제 시 지급되는 금니 개수 </summary>
	RewardBehindStory02,
	/// <summary> 비하인드 스토리 03 슬롯 해제 시 지급되는 금니 개수 </summary>
	RewardBehindStory03,
	/// <summary> 비하인드 스토리 04 슬롯 해제 시 지급되는 금니 개수 </summary>
	RewardBehindStory04,
	/// <summary> 비하인드 스토리 05 슬롯 해제 시 지급되는 금니 개수 </summary>
	RewardBehindStory05,
	/// <summary> 노트 전투 최대 방패 게이지 수치 </summary>
	BattleGuardMax,
	/// <summary> 노트전투 1초당 오르는 방패 게이지 수치 </summary>
	AutoGuardRecvTickRatio,
	/// <summary> 메인 화면에서 일정 시간 작동 없을 경우, Game start 강조 연출 출력될 마지막 스테이지 </summary>
	TutoStartButton,
	/// <summary> 혈청 오픈 스테이지 </summary>
	SerumOpen,
	/// <summary> 상점_보급 상자 오픈 스테이지 </summary>
	ShopSupplyBoxOpen,
	/// <summary> DNA 페이지 오픈 스테이지 </summary>
	CharDNAOpen,
	/// <summary> 스테이지 카드 선택 방향 알람 </summary>
	ThreeWayTuto,
	/// <summary> 알람용 더미 데이터 확률 (캐릭터 획득) </summary>
	Alarm_Character_Get,
	/// <summary> 알람용 더미 데이터 확률 (전용 장비 제작) </summary>
	Alarm_Item_Get,
	/// <summary> 알람용 더미 데이터 확률 (좀비 등급업) </summary>
	Alarm_Zombie_GradeUp,
	/// <summary> 노트 전투에서 가드시 받는 데미지 비율 </summary>
	GuardDamageRatio,
	/// <summary> 이어하기 기초 스탯 회복 값 (퍼센트) </summary>
	ContinueStatRecv,
	/// <summary> 이어하기 제한 턴 추가 </summary>
	ContinueLimitTurnPlus,
	/// <summary> 이어하기 제한 시간 추가 (단위 : 초) </summary>
	ContinueTimePlus,
	/// <summary> 무기명 인사파일로 전환시 적용 비율 값 </summary>
	PersonnelFileChangeRatio,
	/// <summary> 경매장 시간(분) </summary>
	AuctionTime,
	/// <summary> 제작 시간 감소 비용(분) </summary>
	ProductionTimePrice,
	/// <summary> 매일 친구를 최대 삭제 할 수 있는 수 </summary>
	MaxFriendDelCount,
	/// <summary> 매일 친구에게 받을 수 있는 최대 총알 </summary>
	MaxFriendReceiveCount,
	/// <summary> 입찰 증가금 퍼센트 </summary>
	AuctionbidMin,
	/// <summary> 일일 퀘스트 컨텐츠 오픈 스테이지 </summary>
	DailyQuestOpen,
	/// <summary> 감염된 피난민 포만감 감소량(비율) </summary>
	SatInfecteeCharge,
	/// <summary> 감염된 피난민 정신력 감소량(비율) </summary>
	MenInfecteeCharge,
	/// <summary> 감염된 피난민 청결도 감소량(비율) </summary>
	HygInfecteeCharge,
	/// <summary> 감염된 피난민 HP 감소량 (비율) </summary>
	HpInfecteeCharge,
	/// <summary> 감염된 피난민 무리01 감소량(비율) </summary>
	RandomInfecteeCharge,
	/// <summary> 감염된 피난민 무리02 감소량(비율) </summary>
	AllInfecteeCharge,
	/// <summary> PvP에서 전투력 최소 공격량 </summary>
	PvPAttackMinRatio,
	/// <summary> PvP에서 전투력 최대 공격량 </summary>
	PvPAttackMaxRatio,
	/// <summary> 기본노트 데미지 비율 </summary>
	NormalNoteDmg,
	/// <summary> 슬래시노트 데미지 비율 </summary>
	SlashNoteDmg,
	/// <summary> 콤보노트 데미지 비율 </summary>
	ComboNoteDmg,
	/// <summary> 차지노트 데미지 비율 </summary>
	ChargeNoteDmg,
	/// <summary> 체인노트 데미지 비율 </summary>
	ChainNoteDmg,
	/// <summary> 연구 시간 가속 금니(1분당) </summary>
	ResearchTimePrice,
	/// <summary> 탐험 시간 가속 금니(1분당) </summary>
	AdventureTimePrice,
	/// <summary> 2배속 모드 오픈 스테이지 </summary>
	UnlockDoubleSpeed,
	/// <summary> 2배속 모드 잠금 스테이지 </summary>
	LockDoubleSpeed,
	/// <summary> 아포칼립스 최대 연속 이어하기 횟수 </summary>
	NorContinuMax,
	HardContinuMax,
	NightContinuMax,
	/// <summary> 안전 갭 - 해당 비율보다 같거나 높을 경우 Safe 표시(전투력 녹색으로) </summary>
	SafeBPGap,
	/// <summary> 주의 갭 - 해당 비율과 같거나 낮지만 Danger갭보다 높을 경우 Risk 표시(전투력 노란색으로) </summary>
	RiskBPGap,
	/// <summary> 위험 갭 - 해당 비율과 같거나 낮을 경우 Danger 표시(전투력 붉은색으로) </summary>
	DangerBPGap,
	/// <summary> 개인 장비 제작 오픈 </summary>
	PrivateEquipOpen,
	/// <summary> 길드 생성 시, 길드원 기본 MAX 수 </summary>
	GUILDMAX,
	/// <summary> 길드 출석 시 기본으로 지급해 줄 길드 포인트 </summary>
	GuildAttPoint,
	/// <summary> 좀비 보유 최대 개수 </summary>
	ZombieKeepMax,
	/// <summary> 좀비 보상을 최대 누적 가능한 시간 </summary>
	RNAProductionMaxTime,
	/// <summary> PVP 최대 제한턴수 </summary>
	PVPTurnLimit,
	/// <summary> PVP 최초 행동게이지 랜덤상수 </summary>
	PVPInitialTurnSeed,
	/// <summary> 기본 명중률 상수 </summary>
	SuccessAttackPer,
	/// <summary> 최대 명중률 상수 </summary>
	SuccessAttackPerLimit,
	/// <summary> PVP 방어 상수 </summary>
	PVPDefRate,
	/// <summary> PVP 회복 상수 </summary>
	PVPHealRate,
	/// <summary> PVP 데미지편차 상수 </summary>
	PVPDmgVariation,
	/// <summary> PVP 자연회복턴 </summary>
	PVPHealTurn,
	/// <summary> PvP 컨텐츠 오픈 </summary>
	PvPOpen,
	/// <summary> 길드 컨텐츠 오픈 </summary>
	GuildOpen,
	/// <summary> 길드 출석 시 기본으로 지급해 줄 연합 상점 재화 </summary>
	GuildAttCoin,
	/// <summary> 길드 소속 시 총알 1개 소모할 때마다 얻는 기여도 </summary>
	GuildActiveExp,
	/// <summary> 연합 연구 참여 제한 횟수 (1일) </summary>
	GuildResearchLimit,
	/// <summary> 연합 가입신청 받을 수 있는 최대 인원 수 </summary>
	GuildJoinLimit,
	/// <summary> 진입 시, 선택적 픽업이 오픈될 스테이지ID </summary>
	SelectivePickupOpen1,
	/// <summary> 진입 시, 선택적 픽업 슬롯이 2개로 확장되는 스테이지ID </summary>
	SelectivePickupOpen2,
	/// <summary> 진입 시, 선택적 픽업 슬롯이 3개로 확장되는 스테이지ID </summary>
	SelectivePickupOpen3,
	/// <summary> 진입 시, 선택적 픽업 슬롯이 4개로 확장되는 스테이지ID </summary>
	SelectivePickupOpen4,
	/// <summary> 긴급 임무 탐색 챕터 </summary>
	ReplayChapter,
	/// <summary> 긴급 임무 오픈 </summary>
	ReplayOpen,
	/// <summary> 긴급 임무 오픈 개수 </summary>
	ReplayOpenCount,
	/// <summary> 긴급 임무 오픈(나이트메어) </summary>
	ReplayHardOpen,
	/// <summary> 긴급 임무 오픈(아포칼립스) </summary>
	ReplayNightOpen,
	/// <summary> 1일 PVP 플레이 제한 횟수 </summary>
	PVP_Count,
	/// <summary> 타워 최대층 </summary>
	MaxTowerLimit,
	/// <summary>  </summary>
	End

}

public class TGlobalweightTableMng : ToolFile
{
	public Dictionary<ConfigType, string> DIC_Str = new Dictionary<ConfigType, string>();
	public Dictionary<ConfigType, double> DIC_Double = new Dictionary<ConfigType, double>();
	public TGlobalweightTableMng() : base("Datas/GlobalweightTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_Str.Clear();
		DIC_Double.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		pResult.NextReadPos();
		ConfigType type = pResult.Get_Enum<ConfigType>();
		if (!DIC_Str.ContainsKey(type)) DIC_Str.Add(type, pResult.Get_String(pResult.GetPos()));
		if (!DIC_Double.ContainsKey(type)) DIC_Double.Add(type, pResult.Get_Double(pResult.GetPos()));
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// GlobalweightTable
	TGlobalweightTableMng m_Config = new TGlobalweightTableMng();
#if UNITY_EDITOR
#pragma warning disable 0168
	public void LoadConfigTable_Editor()
	{
		try
		{
#if UNITY_EDITOR
            Utile_Class utile = MainMng.IsValid() ? UTILE : new Utile_Class();
#else
            Utile_Class utile = UTILE;
#endif
            string fullPath = Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/'));
			fullPath += "/Assets/0_PatchData/Datas/";
			fullPath += Utile_Class.Get_FileName_Encode("GlobalweightTable");
			fullPath += ".bytes";

			byte[] abyTemp = null;
			FileStream fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
			int nBufSize = (int)fs.Length;
			abyTemp = new byte[nBufSize];
			fs.Read(abyTemp, 0, nBufSize);
			fs.Close();
			if (abyTemp != null)
			{
				Utile_Class.Decode(abyTemp, abyTemp.Length, 0);
				m_Config.DataInit();
				CSV_Result csv = utile.Get_CsvResult(Encoding.UTF8.GetString(abyTemp));
				int size = csv.Get_LineSize();
				for (int i = 0; i < size; i++, csv.next()) m_Config.ParsLine(csv);
				m_Config.CheckData();
			}
		}
		catch(Exception e)
		{

		}
	}
#pragma warning restore 0168
#endif

	public int GetConfig_Int32(ConfigType type)
	{
		if (!m_Config.DIC_Double.ContainsKey(type)) return 0;
		return (int)m_Config.DIC_Double[type];
	}
	public long GetConfig_Long(ConfigType type)
	{
		if (!m_Config.DIC_Double.ContainsKey(type)) return 0;
		return (long)m_Config.DIC_Double[type];
	}

	public float GetConfig_Float(ConfigType type)
	{
		if (!m_Config.DIC_Double.ContainsKey(type)) return 0;
		return (float)m_Config.DIC_Double[type];
	}

	public string GetConfig_String(ConfigType type)
	{
		if (!m_Config.DIC_Str.ContainsKey(type)) return "";
		return m_Config.DIC_Str[type];
	}
}
