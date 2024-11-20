/// <summary> 스텟 종류 </summary>
public enum SkillKind
{
	None = 0,
	//////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 고정 번호(서버에서 사용중이므로 값변경되면 안됨)
	/// <summary> 달러 획득량 증가(스테이지 보상만) </summary>
	GetDoller = 1,
	/// <summary> 경험치 획득량 증가(스테이지 보상만) </summary>
	GetExp,
	/// <summary> 연구 시간 감소 </summary>
	AllResearchSpeedUp,
	/// <summary> 제작 시간 감소 </summary>
	MakingSpeedUp,
	/// <summary> 탐험 시간 감소 </summary>
	ExploreTimeUp,
	/// <summary> 보급 상자 충전 시간(광고X) 감소 </summary>
	SupplyBoxDown,
	/// <summary> 장비 강화 달러 할인 </summary>
	EquipLevelUpSale,
	/// <summary> DNA 재료 요구량 감소 </summary>
	DNAProduceDown,
	/// <summary> 유저 경험치 증가, *클라는 안씀* </summary>
	UserExpUp,
	/// <summary> 케이지 속 좀비들의 RNA 추출 시간 감소 </summary>
	RNATimeDown,
	/// <summary> 최대 총알 충전 개수 증가 </summary>
	BulletMaxUp,
	/// <summary> 일일 탐험 개수 증가 *클라는 안씀* </summary>
	AdventureCountUp,

	/// <summary> 정신력 맥시멈 수치가 증가한다. </summary>
	MenMax,
	/// <summary> 위생력 맥시멈 수치가 증가한다. </summary>
	HygMax,
	/// <summary> 허기짐 맥시멈 수치가 증가한다. </summary>
	SatMax,
	/// <summary> 음식 섭취 시 허기짐 수치가 추가로 감소한다. </summary>
	SatChargUp,
	/// <summary> 위생 수치가 감소되는 모든 상황에서 덜 감소한다. </summary>
	Immunity,
	/// <summary> 포만감 수치가 감소되는 모든 상황에서 덜 감소한다. </summary>
	LessEater,
	/// <summary> 정신력 수치가 감소되는 모든 상황에서 덜 감소한다. </summary>
	CoolMental,
	/// <summary> 치료 아이템 사용 시 일정 확률로 치료 효과가 2배가 된다. </summary>
	RecoveryUp,
	/// <summary> 음식 섭취 시 배고픔 감소 및 정신력 수치도 증가한다. </summary>
	Epicurean,
	/// <summary> 크리티컬 공격 대미지가 증가한다. </summary>
	CriUp,
	/// <summary> 일반노트 대미지가 증가한다. </summary>
	NormalNoteAtkUp,
	/// <summary> 슬래쉬노트 대미지가 증가한다. </summary>
	SliceNoteAtkUp,
	/// <summary> 콤보노트 대미지가 증가한다. </summary>
	ComboNoteAtkUp,
	/// <summary> 체인노트 대미지가 증가한다. </summary>
	ChainNoteAtkUp,
	/// <summary> 챠지노트 대미지가 증가한다. </summary>
	ChargeNoteAtkUp,
	/// <summary> 콤보 성공횟수에 따라 직후 하는 굿판정 이상의 공격에 추가대미지를 준다. </summary>
	ComboAddDmg,

	//////////ActiveStart
	/// <summary> 목표 타겟 하나에게 큰 데미지(사용 캐릭터의 공격력 x 효과 비율) </summary>
	HeadShot,
	/// <summary> HP 회복(사용 캐릭터의 공격력 x 효과 비율) </summary>
	Heal,
	/// <summary> 전방 3칸에 데미지(사용 캐릭터의 공격력 x 효과 비율) </summary>
	ShotGun,
	/// <summary> 긴급 처방, 정신력이 크게 감소하지만 체력을 회복한다.(Value 01 = 감소 정신력, Value 02 = 회복 체력 (만분율)) </summary>
	PainHeal,
	/// <summary> 이번 선택을 생략하고 다음 라인으로 한칸 이동합니다. </summary>
	Jump,
	/// <summary> 목표 카드 한장을 현재 중간 위치로 당겨옵니다. </summary>
	CardPull,
	/// <summary> 목표 1개 라인의 모든 적들에게 피해를 준다. </summary>
	AirStrike,
	/// <summary> 전방 3장의 카드 중 하나를 선택하여 높은 데미지를 준다. </summary>
	BackStep,
	/// <summary> 아무 액티브 스킬 중 하나를 랜덤으로 사용한다. CoolReset 제외 </summary>
	LearningAbility,
	/// <summary> 선택한 특정 카드들을 모두 특정 카드로 변경한다. (Value02 = 변경될 카드 ID) </summary>
	ChangeCard,
	/// <summary> Area 범위 내의 모든 폭발성 기믹
	/// <para>(수류탄, 다이너마이트, 화염병, 오래된 지뢰, 시한 폭탄)을 발동시킨다.</para> </summary>
	Explosion,
	/// <summary> 가로 한 행을 전체 공격한다. (여자모드 스킬), 세로 한 열을 전체 공격한다. (남자모드 스킬) </summary>
	TransverseAtk,
	/// <summary> 아군 캐릭터 스킬쿨타임 초기화 </summary>
	CoolReset,
	/// <summary> (보스 사용 불가) 최대 체력을 40% 소모하여 적 하나를 반드시 처치한다. </summary>
	BlowAtk,
	/// <summary> Area 범위 내 모든 피난민 카드를 한 번에 획득한다. </summary>
	Incitement,
	/// <summary> Area 범위 내 적 하나의 이동을 3턴 제한한다. </summary>
	StopCard,
	/// <summary> (어둠 스테이지)1턴 동안 선택한 한 열의 어둠이 사라진다. </summary>
	SpotLight,
	/// <summary> 선택한 한 행에 있는 모든 적의 이동을 Value02턴 제한한다. </summary>
	StopCardTran,
	/// <summary> 선택한 Area(홀수만 가능, 1, 3, 5) 범위 카드 하나의 화염을 제거한다. </summary>
	RemoveFire,
	/// <summary> 오래된 지뢰, 시한 폭탄 카드를 유틸리티 카드로 변환한다. </summary>
	BombSpecialist,
	/// <summary> Area 영역 내 모든 적을 사격하여 공격한다. </summary>
	RangeAtk,
	/// <summary> Area 영역의 카드를 섞음 (카드 섞기과 동일 기능) </summary>
	Shuffle,
	/// <summary> 선택한 Area 범위 내 카드를 다른 카드로 변경한다. (Value02 = 변환될 스테이지 카드 ID) </summary>
	ChangeCardArea,
	/// <summary> Area범위 내 무작위 적들에게 총 Value01회 공격한다. (Value 02 = 타격 횟수) </summary>
	RandomAtk,
	/// <summary> 생존 스텟 회복 (Value01 = 스텟인덱스) </summary>
	RecoverySrv,
	/// <summary> 재료 카드를 다른 재료 카드로 변환한다. (Value 01 = 변환 할 재료카드 ID, Value 02 = 변환 될 재료카드 ID) </summary>
	ChangeMate,
	/// <summary> 3턴 동안 (턴수 고정) 모든 적이 움직이지 않는다. </summary>
	StopAll,
	/// <summary> 범위 내 모든 특정 재료를 습득한다. (Value02 = 재료 카드 ID) </summary>
	GetCards,
	/// <summary> 범위 내 재료카드의 개수가 +1 증가한다. </summary>
	PlusMate,
	/// <summary> 액티브 스킬 사용 시 재료 카드를 습득한다. (1 ~ 5개 랜덤으로) (Value 01 = 재료 ID, Value 02 = Max 개수) </summary>
	GetMate,
	/// <summary> 현재 포만도가 크게 감소하지만 체력을 회복한다.(Value 01 = 감소 포만도 (절대값), Value 02 = 회복 체력(만분율)) </summary>
	PainSat,
	/// <summary> 현재 청결도가 크게 감소하지만 체력을 회복한다. (Value 01 = 감소 청결도 (절대값), Value 02 = 회복 체력(만분율)) </summary>
	PainHyg,
	/// <summary> 스킬 사용 행동력을 회복한다. (Value01 = 행동력, Value 02 = 회복 수치(절대값)) </summary>
	RecoveryMove,
	/// <summary> 선택 범위 내 찢긴 시체 카드 삭제 후 약간의 정신력 회복(Value 01 = StageCardTable ID 참조, Value 02 = 정신력 회복 수치 (백분율)) </summary>
	SacrificeTornBody,
	/// <summary> 선택 범위 내 상한 음식 카드 삭제 후 약간의 포만도 회복(Value 01 = StageCardTable ID 참조, Value 02 = 포만도 회복 수치 (백분율)) </summary>
	SacrificePit,
	/// <summary> 선택 범위 내 썩은 쓰레기장 카드 삭제 후 약간의 청결도 회복(Value 01 = StageCardTable ID 참조, Value 02 = 청결도 회복 수치 (백분율)) </summary>
	SacrificeGarbage,
	/// <summary> 선택한 범위 내 에너미의 레벨을 N ~ N% 사이 랜덤으로 다운 그레이드 한다.(Value 01 = 레벨 감소치 최소값, Value 02 = 레벨 감소치 최대값)* 남는 값은 반내림으로 처리 </summary>
	DownLevel,
	/// <summary> 청결, 포만, 정신력 모두 회복한다. (Value 02 = 3개 스테이터스 회복량 (백분율)) </summary>
	AllRecoverySrv,
	/// <summary> 스킬 사용 후 N턴간 공습이 발생하지 않는다.(Value 02 = 공습이 발생하지 않을 턴 수) </summary>
	BanAirStrike,
	/// <summary> 선택한 목표 타겟과 양 옆의 적들에게 데미지를 준다.(Value 02 = 데미지 : 사용 캐릭터의 공격력 x 효과 비율) </summary>
	WideAttack,
	/// <summary> 스킬 사용 시, 선택한 영역 안에 있는 특정 카테고리의 적 카드가 삭제된다. (도망치는 컨셉)(Value 02 = 적 카테고리 ID)(None = 0, 동물 = 1, 좀비 = 2, 돌연변이 = 3, 인간 = 4) </summary>
	DeleteEnemyTribe,
	/// <summary> 적 선택 시, 적과의 전투 이전에 해당 적의 보상을 먼저 습득한다. 해당 스킬을 사용한 적을 선택하여 전투 시에는 보상을 습득할 수 없다.(Value 02 = 스틸 실패 확률) 스틸 실패 시 다른 행동을 취할 수 없고 바로 전투가 시작된다. </summary>
	SteelItem,
	/// <summary> 스킬 사용 시 HP와 청결도를 회복한다. (HP 회복 값 : 사용 캐릭터의 회복력 x ValueBase, Value01 = 스테이터스 타입, Value02 = 스테이터스 회복량 (백분율)) </summary>
	HealPlus,
	/// <summary> 선택한 3x3 범위 내 적의 이동을 N턴 동안 금지시키며, 범위 내 원거리 에너미의 공격도 금지시킨다. (Value01 = 이동 금지, 공격 금지 턴 수, 피해 : 사용 캐릭터의 공격력 x ValueBase) </summary>
	StopCardPlus,
	/// <summary> 필드 위 특정 재료 카드를 특정 그룹 내의 랜덤 카드로 변환한다. (Value01 = 바뀔 재료 카드 Type No, Value02 = IngameGroupRewardTable의 GroupIndex 참조) </summary>
	ChangeRandomDrop,
	/// <summary> "특정 카드를 삭제하고, 삭제된 특정 카드의 개수만큼 특정 스테이터스를 회복한다.
	///Value01 = 삭제될 특정 카드 [Default_StageCardTable의 Index 참조], Value02 = 회복 스테이터스 넘버, Value03 = 개수 당 회복될 수치 값 (절대 값)"
	///</summary>
	RecoveryStatus,
	/// <summary>  "스킬 사용 시, 보유 중인 행동력을 모두 소모하고 그 값만큼 HP를 회복한다.
	///Value01 = 행동력 1당 HP 회복률, 잃은 체력 비례로 HP 회복"
	/// </summary>
	APHP,
	/// <summary> 잠긴 카드의 잠김 상태를 해제한다. </summary>
	Unlock,
	/// <summary> 현재 내 제작탭의 아이템 하나를 랜덤으로 복사한다. (제작 탭의 보유 개수가 최대일 경우 사용할 수 없음.) </summary>
	CopyMaterial,
	/// <summary> (어둠 스테이지) 사용 시 N턴 동안 적 카드의 윤곽이 드러난다. </summary>
	DarkPatrol,
	/// <summary> "머지 탭의 모든 아이템을 버리고 개수만큼 HP를 회복한다.
	///Value01 = 버리는 개수 당 회복 HP 회복률, 잃은 체력 비례로 HP 회복"
	///</summary>
	DropItemHp,
	/// <summary> "현재 체력이 크게 감소하지만 행동력을 회복한다.
	///Value01 = 감소 HP(백분율), Value02 = 행동력 회복 수치(절대값)"
	/// </summary>
	HPAP,
	/// <summary> (Boss 타겟팅 불가) 피해를 입은 적 하나를 반드시 처치한다. </summary>
	LastAttack,
	/// <summary> 입력 시 바로 도박 카드 기능이 실행된다. </summary>
	Gamble,
	/// <summary> 사용 시 지정한 벽 카드 하나를 파괴한다. </summary>
	DestoryWall,
	/// <summary> "기재된 스테이터스를 디버프 해제 조건 수치까지 회복 시킨다.
	///Value 01 = 스테이터스01, Value02 = 스테이터스02, Value03 = 스테이터스03"
	///</summary>
	UnDebuff,
	/// <summary> 3x3 영역 내 모든 종류의 에너미들이 혼란상태가 되어 플레이어나 피난민을 공격하지 않는다.  </summary>
	DontAttack,
	/// <summary> 3x3 영역 내 해로운 효과를 주는 카드를 삭제한다. </summary>
	DeleteBadCard,
	/// <summary> 현재 구성된 파티 내 캐릭터의 액티브 스킬 하나를 랜덤으로 사용한다. </summary>
	LearningAbility02,
	/// <summary> 스킬 사용 시, N(2)턴간 모든 아군이 은신 상태로 진입하여 전투가 발생하지 않는다. </summary>
	Hide,
	/// <summary> 멀리 있는 카드 하나를 선택, 선택한 카드를 하단의 머지 탭에 보관한다. </summary>
	KeepMaterial,
	/// <summary> 3x3 범위 내 인간형 적을 랜덤의 피난민 카드로 변환한다. </summary>
	MakeRefugee,
	/// <summary> 제작창에 있는 아이템을 소모하여 AP를 회복한다. </summary>
	DropItemAP,
	/// <summary> 범위 내 지정 종족 개수만큼 랜덤 종류의 재료를 1개씩 획득한다. </summary>
	CountTribeMaterial,
	/// <summary> 사용 시 지정한 벽이나 지뢰를 제거, 에너미에게 사용 시 약간의 피해를 준다. </summary>
	DestoryWall02,
	/// <summary> 재료 카드 또는 바닥 카드를 오래된 지뢰 카드로 변경
	/// val 0:변환될 카드 타입 1, 1:변환될 카드 타입2, 2:변환할 카드 타입
	/// </summary>
	ChangeCard02,
	/// <summary> 랜덤 무기 카드 1개를 머지 탭에 보관 </summary>
	RandomWeapon,
	/// <summary> 선택한 카드 중심으로 범위 내 피난민을 제외한 에너미를 N회 공격한다. </summary>
	RandomAtk02,
	/// <summary> 5x5 영역 내 랜덤의 적 3명을 0번째 라인의 N장의 카드와 위치를 변경한다. </summary>
	EnemyPull,
	//////////ActiveEnd

	/// <summary> 공격력 증가 </summary>
	AtkUp,
	/// <summary> 방어력 증가 </summary>
	DefUp,
	/// <summary> 회복력 증가 </summary>
	HealUp,
	/// <summary> 총 HP량 증가 </summary>
	TotalHpUp,
	/// <summary> 기력 최대치 증가 </summary>
	TotalEnergyUp,
	/// <summary> 좀비 대상 공격력 증가 </summary>
	ZomAtkUp,
	/// <summary> 좀비 대상 방어력 증가 </summary>
	ZomDefUp,
	/// <summary> 좀비 대상 기력 회복력 증가 </summary>
	ZomEnergyUp,
	/// <summary> 인간 대상 공격력 증가 </summary>
	HumanAtkUp,
	/// <summary> 인간 대상 방어력 증가 </summary>
	HumanDefUp,
	/// <summary> 인간 대상 기력 회복력 증가 </summary>
	HumanEnergyUp,
	/// <summary> 돌연변이 대상 공격력 증가 </summary>
	MutantAtkUp,
	/// <summary> 돌연변이 대상 방어력 증가 </summary>
	MutantDefUp,
	/// <summary> 돌연변이 대상 기력 회복력 증가 </summary>
	MutantEnergyUp,
	/// <summary> 동물 대상 공격력 증가 </summary>
	AnimalAtkUp,
	/// <summary> 동물 대상 방어력 증가 </summary>
	AnimalDefUp,
	/// <summary> 동물 대상 기력 회복력 증가 </summary>
	AnimalEnergyUp,
	/// <summary> 보스 대상 공격력 증가 </summary>
	BossAtkUp,
	/// <summary> 보스 대상 방어력 증가 </summary>
	BossDefUp,
	/// <summary> 보스 대상 기력 회복력 증가 </summary>
	BossEnergyUp,
	/// <summary> 일반 등급 대상 공격력 증가 </summary>
	NormalAtkUp,
	/// <summary> 일반 등급 대상 방어력 증가 </summary>
	NormalDefUp,
	/// <summary> 일반 대상 기력 회복력 증가 </summary>
	NormalEnergyUp,
	/// <summary> 엘리트 등급 대상 공격력 증가 </summary>
	EliteAtkUp,
	/// <summary> 엘리트 등급 대상 방어력 증가 </summary>
	EliteDefUp,
	/// <summary> 엘리트 대상 기력 회복력 증가 </summary>
	EliteEnergyUp,
	/// <summary> 전투 종료 시 HP 회복 </summary>
	BattleEndHp,
	/// <summary> 전투 종료 시 멘탈 회복 </summary>
	BattleEndMen,
	/// <summary> 전투 종료 시 위생 회복 </summary>
	BattleEndHyg,
	/// <summary> 전투 종료 시 허기 회복 </summary>
	BattleEndSat,
	/// <summary>  정신력 최대치가 증가한다. </summary>
	MaxMenUp,
	/// <summary> 위생 최대치가 증가한다. </summary>
	MaxHygUp,
	/// <summary> 허기 최대치가 증가한다. </summary>
	MaxSatUp,
	/// <summary> 정신을 감소시키는 공격에 대한 방어력이 증가한다. </summary>
	DefMenUp,
	/// <summary> 위생을 감소시키는 공격에 대한 방어력이 증가한다. </summary>
	DefHygUp,
	/// <summary> 허기을 감소시키는 공격에 대한 방어력이 증가한다. </summary>
	DefSatUp,
	/// <summary> 속도가 증가한다. </summary>
	SpeedUp,
	/// <summary> 치명타 발생 확률이 증가한다. </summary>
	CriProbUp,
	/// <summary>  </summary>
	End
}

