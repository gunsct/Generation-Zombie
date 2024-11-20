public enum RewardPos
{
	/// <summary> 일반 메세지 </summary>
	Msg = 0,
	/// <summary> 운영자가 보냄 </summary>
	Mng,
	/// <summary> 스테이지에서 지급 </summary>
	Stage,
	/// <summary> 챕터 보상 </summary>
	Chapter,
	/// <summary> 스테이지 양자 택일 </summary>
	AltReward,
	/// <summary> 탐사 </summary>
	Adv,
	/// <summary> 생산 </summary>
	Making,
	/// <summary> 상점 구매 </summary>
	Shop,
	/// <summary> 스테이지 대화 </summary>
	Talk,
	/// <summary> 우편함에서 지급 </summary>
	Post,
	/// <summary> 좀비 승급 실패 또는 파괴 </summary>
	Zombie,
	/// <summary> 챌린지 보상
	/// <para> 신규 : new long[] { (long)info.Type, Rank, (int)ChallengeMode.New }</para>
	/// <para> 일반 : new long[] { (long)info.Type, Rank, (int)ChallengeMode.Normal }</para>
	/// <para> 주간 : new long[] { (long)type, user.Rank, (int)ChallengeMode.Week }</para>
	/// </summary>
	Challenge,
	/// <summary> 돌발 이벤트(블랙 마켓) </summary>
	AddEvent,
	/// <summary> 업적 </summary>
	Achieve,
	/// <summary> 인앱 구매(가방 사이즈 체크가 필요없음) </summary>
	InApp,
	/// <summary> 이벤트 보상 </summary>
	Event,
	/// <summary> 경매
	/// <para>경매 종료 : new long[] { 0 }</para>
	/// <para>상위 입찰가 유저가 발생하여 입찰 실패(금니 반환) : new long[] { 1 }</para>
	/// </summary>
	Auction,
	/// <summary> 연구 </summary>
	Research,
	/// <summary> 미션 </summary>
	Mission,
	/// <summary> DNA 생성 </summary>
	Making_DNA,
	/// <summary> PVP
	/// <para> 리그 : new long[] { 0, MyRank.Rank, MyRank.PVPRank }</para>
	/// <para> 시즌 : new long[] { 1, MyRank.Rank, MyRank.PVPRank, Percent }</para>
	/// </summary>
	PVP,
	/// <summary> 캐릭터 </summary>
	Char,
	/// <summary> 아이템 </summary>
	Item,
	/// <summary> 길드 </summary>
	Guild,
	/// <summary> 가방 </summary>
	Inven,
	/// <summary> Hive 아이템 지급
	/// <para> new long[] { ReasonType, TitleIdx, MsgIdx }</para>
	/// </summary>
	Hive,
	/// <summary> Coda </summary>
	Coda,
	/// <summary> 캠프 자원 생산 </summary>
	CampRes,
	/// <summary> 데일리 팩키지 아이템
	/// <para> new long[] { 데일리팩 Idx, 진행 스탭 }</para>
	/// </summary>
	DailyPackItem,
	End
}

public enum ReasonType
{
	/// <summary> 과금오류 (Payment Error) </summary>
	pe = 0,
	/// <summary> 보상(게임오류) (Reward Game Error) </summary>
	rge,
	/// <summary> 보상(고객불만) (Reward Consumer Dissatisfaction) </summary>
	rcd,
	/// <summary> 보상(쿠폰오류) (Reward Coupon Error) </summary>
	rce,
	/// <summary> 보상(해외) (Reward Overseas) </summary>
	ro,
	/// <summary> 재화변경지급(Asset Exchange) </summary>
	ae,
	/// <summary> 이벤트(Event) </summary>
	e,
	/// <summary> 이벤트(자동) (Event Automatic) </summary>
	ea,
	/// <summary> 대량쿠폰(Massive Coupon) </summary>
	mc,
	/// <summary> 고유쿠폰(Unique Coupon) </summary>
	uc,
	/// <summary> 인게임 빌링(Billing – HIVE IAP v2) </summary>
	b,
	/// <summary> 러비웹샵 빌링(Lebi Billing) </summary>
	lb,
	/// <summary> 오퍼월(Cpi Offerwall) </summary>
	co,
	/// <summary> 프로모션(Promotion) – 크로스배너, 오퍼월, UA 모두 동일하게 사용 
	/// <para>subReason ->  : 프로모션 하위 요청 상세</para>
	/// <para>subReason -> 1 : 크로스 일반 배너 CPI</para>
	/// <para>subReason -> 2 : 크로스 일반 배너 CPA</para>
	/// <para>subReason -> 3 : 크로스 대배너 CPI</para>
	/// <para>subReason -> 4 : 크로스 대배너 CPA</para>
	/// <para>subReason -> 5 : 오퍼월 일반 CPI</para>
	/// <para>subReason -> 6 : 오퍼월 일반 CPA</para>
	/// <para>subReason -> 7 : 오퍼월 스페셜 CPI</para>
	/// <para>subReason -> 8 : 오퍼월 스페셜 CPA</para>
	/// <para>subReason -> 9 : UA CPI</para>
	/// <para>subReason -> 10 : UA CPA</para>
	/// <para>subReason -> 11 : UC CPI</para>
	/// </summary>
	p,
	/// <summary> 스트리밍 보상(Streaming Reward) </summary>
	sr,
	/// <summary> 테스트(CS) (Test CS) </summary>
	tcs,
	/// <summary> 테스트(GM) (Test GM) </summary>
	tgm,
	/// <summary> 테스트(PM) (Test PM) </summary>
	tpm,
	/// <summary> 테스트(QA) (Test QA) </summary>
	tqa,
	/// <summary> 테스트(개발) (Test Developer) </summary>
	td,
	/// <summary> 테스트(외부지급) (Test Guest) </summary>
	tg,
	/// <summary> 테스트(홍보/사업) (Test Market, Business) </summary>
	tmb,
	/// <summary> 테스트(해외) (Test Overseas) (Test Developer) </summary>
	to,
	/// <summary> 회수(기타) (Retrieve Etc) </summary>
	re,
	/// <summary> 회수(환불) (Retrieve Refund) </summary>
	rr,
	/// <summary> 대량요청(Massive Request) </summary>
	mr,
	/// <summary> 기타(Etc) </summary>
	etc
}