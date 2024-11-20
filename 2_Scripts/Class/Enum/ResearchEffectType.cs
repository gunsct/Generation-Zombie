public enum ResearchEff
{
	None = 0,
	/// <summary> 생산시설 건설</summary>
	MakingOpen,
	/// <summary> 생산 속도 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 생산 속도 증가 ([기존 시간] * (1-[Value01]/10000)</para>
	/// </summary>
	MakingSpeedUp,
	/// <summary> 경험치 획득량 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 컨텐츠에서 경험치 획득량 증가 ([기존 보상] * (1+[Value01]/10000)</para>
	/// </summary>
	ExpUp,
	/// <summary> 달러 획득량 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 컨텐츠에서 달러 획득량 증가 ([기존 보상] * (1+[Value01]/10000)</para>
	/// </summary>
	DollarUp,
	/// <summary> 연구 속도 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 연구 속도 증가 ([기존 시간] * (1-[Value01]/10000)</para>
	/// </summary>
	ResearchSpeedUp,
	/// <summary> 에너지 최대치 증가  절대값 총알(스테이지 입장 화폐) 최대치에 추가 </summary>
	BulletMaxUp,
	/// <summary> 탐험 열림
	/// <para>탐험 컨텐츠 열림</para>
	/// </summary>
	AdventureOpen,
	/// <summary> 트레이닝 캠프 설립
	/// <para>훈련 연구 텝 오픈</para>
	/// </summary>
	TrainingOpen,
	/// <summary> 탐험 개수 증가
	/// <para>탐험 진행 가능 개수 증가</para>
	/// </summary>
	AdventureCountUp,
	/// <summary> 탐험 레벨 증가
	/// <para>탐험 레벨 증가</para>
	/// </summary>
	AdventureLevelUp,
	/// <summary> 트레이닝 속도 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 훈련장 연구 속도 증가 ([기존 시간] * (1-[Value01]/10000)</para>
	/// </summary>
	TrainingSpeedUp,
	/// <summary> 폐 공장 발견
	/// <para>개조 연구 텝 오픈</para>
	/// </summary>
	RemodelingOpen,
	/// <summary> 폐 공장 업그레이드 속도 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 개조 연구 속도 증가 ([기존 시간] * (1-[Value01]/10000)</para>
	/// </summary>
	RemodelingSpeedUp,
	/// <summary> HP 최대치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	HealthMaxUp,
	/// <summary> 공격력 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	AtkUp,
	/// <summary> 방어력 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	DefUp,
	/// <summary> 회복력 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	HealUp,
	/// <summary> 행동력 회복 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	APSpeedUp,
	/// <summary> 정신력 최대치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	MenMaxUp,
	/// <summary> 청결도 최대치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	HygMaxUp,
	/// <summary> 포만감 최대치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)
	/// </summary>
	SatMaxUp,
	/// <summary> 정신력 방어력 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	MenDefUp,
	/// <summary> 청결도 방어력 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	HygDefUp,
	/// <summary> 포만감 방어력 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	SatDefUp,
	/// <summary> 기력 최대치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	EnergyMaxUp,
	/// <summary> 방어 횟수 증가
	/// <para>절대값</para>
	/// <para>노트 전투 시 가드 가능 수 증가</para>
	/// </summary>
	GuardMaxUp,
	/// <summary> 기력 회복속도 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 회복 틱 시간 감소 ([기존 시간] * (1-[Value01]/10000)</para>
	/// </summary>
	EnergySpeedUp,
	/// <summary> 속도 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	SpeedUp,
	/// <summary> 무기 능력치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 기본 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	WeaponStatUp,
	/// <summary> 헬멧 능력치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 기본 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	HelmetStatUp,
	/// <summary> 복장 능력치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 기본 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	CostumeStatUp,
	/// <summary> 신발 능력치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 기본 수치 증가 ([기존 수치] * (1+[Value01]/10000)</para>
	/// </summary>
	ShoesStatUp,
	/// <summary> 악세서리 능력치 증가
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 기본 수치 증가 ([기존 수치] * (1+[Value01]/10000)
	/// </summary>
	AccStatUp,
	/// <summary> 무기 성장 달러 할인
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 성장 비용 감소 ([기존 비용] * (1-[Value01]/10000)</para>
	/// </summary>
	WeaponSale,
	/// <summary> 헬멧 성장 달러 할인
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 성장 비용 감소 ([기존 비용] * (1-[Value01]/10000)</para>
	/// </summary>
	HelmetSale,
	/// <summary> 복장 성장 달러 할인
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 성장 비용 감소 ([기존 비용] * (1-[Value01]/10000)</para>
	/// </summary>
	CostumeSale,
	/// <summary> 신발 성장 달러 할인
	/// <para>비율</para>
	/// <para>입력된 수치(만분위)만큼 성장 비용 감소 ([기존 비용] * (1-[Value01]/10000)</para>
	/// </summary>
	ShoesSale,
	/// <summary> 악세서리 성장 달러 할인
	/// <para>비율
	/// <para>입력된 수치(만분위)만큼 성장 비용 감소 ([기존 비용] * (1-[Value01]/10000)</para>
	/// </summary>
	AccSale,
	/// <summary> 생산 레벨 증가
	/// <para>절대값
	/// <para>생산 레벨 증가</para>
	/// </summary>
	MakingLevelUp,
	/// <summary> 박스 등급 증가 </summary>
	SupplyBoxGradeUp,
	/// <summary> 생산 속도 증가 : 무기 </summary>
	WeaponTimeUp,
	/// <summary> 생산 속도 증가 : 헬멧 </summary>
	HelmetTimeUp,
	/// <summary> 생산 속도 증가 : 복장 </summary>
	CostumeTimeUp,
	/// <summary> 생산 속도 증가 : 신발 </summary>
	ShoesTimeUp,
	/// <summary> 생산 속도 증가 : 장신구 </summary>
	AccessoryTimeUp,
	/// <summary> 생산 속도 증가 : 전용 장비 </summary>
	SpecialEquipTimeUp,
	/// <summary> 생산 속도 증가 : 연구 재료 </summary>
	ResearchMaterialTimeUp,
	/// <summary> 생산 속도 증가 : 생산 재료 </summary>
	CraftMaterialTimeUp,
	/// <summary> 플레이어 탐색 완료 시간 감소 </summary>
	ExploreTimeUp,
	/// <summary> 최대 연합원 수 증가 </summary>
	MemberMaxUp,
	/// <summary> PVP 캐릭터 공격력 증가 </summary>
	PVPAtkUp,
	/// <summary> PVP 캐릭터 속도 증가 </summary>
	PVPSpeedUp,
	/// <summary> PVP 캐릭터 명중률 증가 </summary>
	PVPHitUp,
	/// <summary> PVP 캐릭터 방어력 증가 </summary>
	PVPDefUP,
	/// <summary> PVP 캐릭터 최대 체력 증가 </summary>
	PVPHpUP,
	/// <summary> PVP 정신력 감소 시 % 방어 </summary>
	PVPPerDefMenUP,
	/// <summary> PVP 포만도 감소 시 % 방어 </summary>
	PVPPerDefSatUP,
	/// <summary> PVP 청결도 감소 시 % 방어 </summary>
	PVPPerDefHygUP,
	/// <summary> 서포터 도망 확률 감소 </summary>
	PVPPerRunDown,
	/// <summary> 일반 재료 최대 약탈 비율 감소 </summary>
	PVPJunkRatioUP,
	/// <summary> 중급 재료 최대 약탈 비율 감소 </summary>
	PVPCultivateRatioUP,
	/// <summary> 고급 재료 최대 약탈 비율 감소 </summary>
	PVPChemicalRatioUP,
	/// <summary> 일반 재료 최대 약탈 개수 감소 </summary>
	PVPJunkCountDown,
	/// <summary> 중급 재료 최대 약탈 개수 감소 </summary>
	PVPCultivateCountDown,
	/// <summary> 고급 재료 최대 약탈 개수 감소 </summary>
	PVPChemicalCountDown,
	/// <summary> PVP 어태커 공격 시 청결 감소 효과 % 증가 </summary>
	PVPPerAtkHygUp,
	/// <summary> PVP 서포터 스킬 공격 포만도 감소 효과 % 증가 </summary>
	PVPPerSupSatUp,
	/// <summary> PVP 서포터 스킬 공격 청결도 감소 효과 % 증가 </summary>
	PVPPerSupHygUp,
	/// <summary> PVP 서포터 스킬 공격 정신력 감소 효과 % 증가 </summary>
	PVPPerSupMenUp,
	/// <summary> </summary>
	End
}