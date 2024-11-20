public enum MissionType
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 스테이지 클리어 Value01(value2)
	/// <para> -1 (None) : 스테이지 + 다운타운</para>
	/// <para> 0 : 일반 스테이지 (1 : 나이트메어 / 2 : 아포칼립스)</para>
	/// <para> 1 : 로지 은행</para>
	/// <para> 2 : 보일 사관학교</para>
	/// <para> 3 : 허버트 대학</para>
	/// <para> 4 : 고든 타워</para>
	/// <para> 5 : 램버트 공동 묘지</para>
	/// <para> 6 : 폐허가 된 공장 (댄 공장)</para>
	/// <para> 7 : 지하철</para>
	/// </summary>
	StageClear,
	/// <summary> 일일 퀘스트 클리어 시 (Cnt만 카운트) - 특별 보상 관련</para>
	/// <para> 퀘스트 지급 시 같은 GID 내에 DailyQuestClear 타입이 여러 개 있어도 그 중 하나만 출력</para>
	///  </summary>
	DailyQuestClear,
	/// <summary> 장비 레벨 업 시도 시 클리어 </summary>
	EquipLevelUp,
	/// <summary> Z-PAD의 '생산' 컨텐츠 관련 '시도' 시</para>
	/// <para> "0 : 아무 장비 생산 시 카운트</para>
	/// <para> 1 : '장비 생산' 탭의 아이템 생산 시도 시 카운트</para>
	/// <para> 2 : '전용 장비' 탭의 아이템 생산 시도 시 카운트</para>
	/// <para> 3 : '연구 재료' 탭의 아이템 생산 시도 시 카운트</para>
	/// <para> 4 : '생산 재료' 탭의 아이템 생산 시도 시 카운트"</para>
	/// </summary>
	Making,
	/// <summary> 파견 '시도' 시 클리어 </summary>
	ADV,
	/// <summary> 좀비에게 DNA 주입 시 카운팅 </summary>
	ZombieDNA,
	/// <summary> 좀비 분해 시 카운팅 </summary>
	ZombieDestory,
	/// <summary> 캐릭터 혈청 강화 시 카운팅 </summary>
	Serum,
	/// <summary> 블랙마켓 아이템 구매 시 카운팅 </summary>
	BlackMarket,
	/// <summary> 경매장 입장 횟수 카운팅 </summary>
	Auction,
	/// <summary> 친구에게 총알 보낼 시 카운팅 </summary>
	Friend,
	/// <summary> 총알 사용 </summary>
	UseBullet,
	/// <summary> 캐릭터 레벨 업 1회 시 1 카운팅 </summary>
	CharLevelUp,
	/// <summary> 아이템 획득 </summary>
	GetItem,
	/// <summary> 보급상자 열기 N회 </summary>
	OpenSupplyBox,
	/// <summary> 장비 재조립 N회  </summary>
	Remake,
	/// <summary> 캐릭터 승급 시 카운팅 </summary>
	GradeUp,
	/// <summary> 캐릭터 가차 시 카운팅 (요구 조건이 20회일 경우, 10회 뽑기 시도 시 10 카운팅) </summary>
	CharGacha,
	/// <summary> 장비 가차 시 카운팅 (요구 조건이 20회일 경우, 10회 뽑기 시도 시 10 카운팅) </summary>
	ItemGacha,
	/// <summary> N레벨 이상 생존자 n명 이상 보유 시 카운팅(보상받을때 체크함 미션은 업적처럼 수집하는 데이터가 없음) </summary>
	CharLevel,
	/// <summary> N등급 이상 생존자 n명 이상 보유 시 카운팅(보상받을때 체크함 미션은 업적처럼 수집하는 데이터가 없음) </summary>
	CharGrade,
	/// <summary> 이어하기 n회 실행 시 카운팅 </summary>
	Continue,
	/// <summary> 기재된 특정 스테이지 클리어 시 카운팅 </summary>
	StageIdx,
	/// <summary> 친구 추가 시 카운팅 </summary>
	AddFriend,
	/// <summary> 길드 가입 시 카운팅(보상받을때 체크함 미션은 업적처럼 수집하는 데이터가 없음) </summary>
	Guild,
	/// <summary> 연구 n회 시도 시 카운팅 </summary>
	Research,
	/// <summary> 강화된 스킬을 가진 생존자 n명 보유 (=전용 장비 모두 착용)(보상받을때 체크함 미션은 업적처럼 수집하는 데이터가 없음) </summary>
	SkillUpgrade,
	/// <summary> 초보자 퀘스트 n개 클리어 시 지급 보상 </summary>
	BeginnerQuestClear,
	/// <summary> 상점에서 달러 물품 구매 시 카운팅
	/// <para> ※[블랙마켓 물품 구매 카운팅] 과 [상점에서 달러 물품 구매 시 카운팅] 임무를 동시에 보유하고 있을 경우, [블랙마켓에서 달러 물품을 구매 시] [상점에서 달러 물품 구매 퀘스트] 까지 함께 클리어 </para>
	/// </summary>
	BuyStoreDoller,
	/// <summary> PVP 참여 시 카운팅 </summary>
	PlayPVP,
	/// <summary> PVP 승리 시 카운팅 </summary>
	VicPVP,
	/// <summary> PVP 전용 상점 구매 시 카운팅 </summary>
	BuyPVPStore,
	/// <summary> 연합 출석 시 카운팅 </summary>
	GuildCheck,
	/// <summary> 연합 상점 구매 시 카운팅 </summary>
	ButGuildStore,
	/// <summary> 연합 연구 참여 시 카운팅 </summary>
	GetGuildResearch,
	/// <summary> 좀비 사육장 - 좀비 배치 시 카운팅 </summary>
	PlaceZombie,
	/// <summary> 좀비 사육장 - RNA 회수 시 카운팅 </summary>
	GetZombieRNA,
	/// <summary> 좀비 사육장 - DNA 생산 시 카운팅 </summary>
	ProduceDNA,
	/// <summary> 요구 아이템을 줄 경우 카운팅 시
	/// <para> Value01 : 요구하는 아이템 인덱스</para>
	/// </summary>
	GiveItem,
	/// <summary> 모드가 Event_miniGame인 퀘스트 완료 수를 체크
	/// <para> Value01 : 해당 이벤트를 완료할 수 있는 횟수 (0일 경우 이벤트 종료될 때까지 무한정 클리어 가능)</para>
	/// </summary>
	Event_miniGame_Clear,
	/// <summary> 이벤트 GrowUP 레벨에 따라 카운팅
	/// <para> Value01 : 이벤트 GrowUP의 요구하는 레벨 </para>
	/// </summary>
	EventGrowupLevel,
	/// <summary> 모드가 ReturnUserQuest인 퀘스트 완료 수를 체크</para>
	/// <para> Value01 : 해당 이벤트를 완료할 수 있는 횟수 (0일 경우 이벤트 종료될 때까지 무한정 클리어 가능)</para>
	/// </summary>
	ReturnUserQuestClaer,
	/// <summary> 이벤트 전용 스테이지 클리어
	/// <para> Value01 : 클리어 조건 스테이지</para>
	/// </summary>
	EventStageIdx,
	/// <summary> 특정 모드 미션 올 클리어</summary>
	EventStageClear,
	/// <summary> 이벤트 전용 스테이지 클리어(인덱스)
	/// <para> Value01 : 모드 Enum Index</para>
	/// </summary>
	ModeClear,
	/// <summary>  </summary>
	Max
}

public enum MissionMode
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 일일단위 </summary>
	Day,
	/// <summary> 시즌 패스 </summary>
	Pass,
	/// <summary> 일일 퀘스트 특별 보상 </summary>
	DailyQuest,
	/// <summary> 초보자 전용 퀘스트 </summary>
	BeginnerQuest,
	/// <summary> 이벤트 미니 게임용 미션 </summary>
	Event_miniGame,
	/// <summary> 미니게임 완료 체크용 미션 </summary>
	Event_miniGame_Clear,
	/// <summary> 이벤트 미션 </summary>
	Event_CharMission,
	/// <summary> 이벤트 성장 미션 </summary>
	Event_Growup,
	/// <summary> 이벤트 복귀 유저 미션 </summary>
	ReturnUserQuest,
	/// <summary> 이벤트 복귀 유저 완료 체크용 미션 </summary>
	ReturnUserQuestClaer,
	/// <summary> 가이드 퀘스트 </summary>
	Guide,
	/// <summary> 오픈이벤트용 </summary>
	OpenEvent,
	/// <summary>  </summary>
	Max
}