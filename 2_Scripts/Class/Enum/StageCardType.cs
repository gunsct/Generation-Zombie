/// <summary> 스테이지 카드 종류 </summary>
public enum StageCardType
{
	/// <summary> 없음. </summary>
	None = 0,
	/// <summary> Exit에 도달 시 스테이지 승리 처리 </summary>
	Exit,
	/// <summary> 카드 생성 시 그룹 내에서 배치된 몬스터 중 하나로 결정되어 출력 </summary>
	Enemy,
	/// <summary> 저격 가능한 타겟들이 포커싱되고 해당 타겟을 선택하면 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
	Sniping,
	/// <summary> 목표 카드 선택 시 해당 카드를 기점으로 3x3 범위에 [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
	Grenade,
	/// <summary> 화염 폭탄, 단일 타겟에게 데미지 주고 일정 확률로 화상 상태로 만듬</summary>
	FireBomb,
	/// <summary> 목표 카드 선택 시 해당 카드가 포함된 행 전부를 공격한다. [파티 공격력] * [지정된 비율] 만큼 공격 </summary>
	Dynamite,
	/// <summary> N회 입력 시 (1회 당 1턴 소모) 벽이 깨지며 다른 랜덤 카드로 변환된다. </summary>
	Wall,
	/// <summary> 습득 시 아이템 획득 </summary>
	SupplyBox,
	/// <summary> 피난민 무생물 버전, 스테이지 목표 용(보급상자 x개 모으기) </summary>
	BigSupplyBox,
	/// <summary> 5x5의 범위 내에 랜덤으로 10회 공격. [파티 공격력] * [지정된 비율] 공격력 </summary>
	MachineGun,
	/// <summary> HP 회복 </summary>
	RecoveryHp,
	/// <summary> HP 회복 </summary>//*현재 안쓰임 삭제해야할듯
	RecoveryHpPer,
	/// <summary> 포만감 회복 </summary>
	RecoverySat,
	/// <summary> 정신력 회복 </summary>
	RecoveryMen,
	/// <summary> 청결도 회복 </summary>
	RecoveryHyg,
	/// <summary> 최대 HP 상승 </summary>
	HpUp,
	/// <summary> 공격력 상승 </summary>
	AtkUp,
	/// <summary> 방어력 상승 </summary>
	DefUp,
	/// <summary> 기력 회복 속도 증가 </summary>
	EnergyUp,
	/// <summary> 방어 횟수 </summary>
	AddGuard,
	/// <summary> 포만도 증가 </summary>
	SatUp,
	/// <summary> 청결도 증가 </summary>
	HygUp,
	/// <summary> 정신력 증가 </summary>
	MenUp,
	/// <summary> 5x5 범위 내 모든 카드의 위치를 랜덤하게 다시 배치한다. </summary>
	AllShuffle,
	/// <summary> 5x5 범위 내의 모든 카드를 다시 뽑는다. </summary>
	AllConversion,
	/// <summary> 찢긴 시체 : 전방 3장의 카드에 찢긴 시체 카드가 들어올 경우 정신력 감소 (피난민과 같은 판정) </summary>
	TornBody,
	/// <summary> 썩은 쓰레기장 : 전방 3장의 카드에 썩은 쓰레기장 카드가 들어올 경우 포만감 감소 (피난민과 같은 판정) </summary>
	Garbage,
	/// <summary> 오물 구덩이 : 전방 3장의 카드에 오물구덩이 카드가 들어올 경우 청결도 감소 (피난민과 같은 판정) </summary>
	Pit,
	/// <summary> 어둠 카드 : 어둠이 덮여 있는 카드는 어떤 카드인지 확인할 수 없으며, 다만 어둠이 덮여 있는 카드가 전방 3장 선택 가능한 카드에
	/// <para>들어올 경우 어둠이 사라지고 카드의 실체가 드러난다.</para>
	/// <para>*어둠 카드 밝힐 시 해당 스테이지에 등록 되어 있는 스테이지 카드 중 랜덤으로 변환. </para>
	/// </summary>
	Dark,
	/// <summary> 형광 스틱 : 지정한 특정 카드 위치로 형광 스틱을 던져 N턴 동안 3x3 범위 내 어둠을 밝혀 카드 모습을 확인시켜준다. </summary>
	LightStick,
	/// <summary> 손전등 : 지정한 한 열의 어둠 카드가 N턴 동안 사라진다. </summary>
	FlashLight,
	/// <summary> 시체 : 유닛을 데미지를 주는 기믹으로 처치 시 (카드 삭제 기믹 제외) 해당 카드는 '시체' 카드로 변환된다.
	/// <para> 해당 카드는 체력이 존재하지 않으며, 카드를 삭제하는 기믹의 효과는 적용된다.</para>
	/// <para>전방 3장의 카드에 시체 카드가 들어와 해당 카드를 선택 시 랜덤 보상을 준다.</para>
	/// <para>(예전 아이템 습득 카드 연출처럼 카드 입력 시 타들어가는 연출 후 보상 아이템 등장)</para>
	/// </summary>
	DeadBody,
	/// <summary> 오래된 지뢰 : 해당 카드는 턴이 지날 경우 아래로 내려오지만, 다른 유닛과 위치가 변환되지 않는다.
	/// <para>해당 카드의 십자 반경 내 유닛이 존재할 경우 폭발하며, 반경 내 적에게 피해를 준다.</para>
	/// <para>(유닛 외 카드는 피해를 입지 않는다.)</para>
	/// <para>*전방 3장의 카드 목록에 오래된 지뢰가 들어올 경우 내가 피해를 입는다.</para>
	/// <para>*데미지는 [현재 파티 최대 체력] * [지정된 비율] 이다.</para>
	/// </summary>
	OldMine,
	/// <summary> 
	/// 던지면 수류탄, 타임오버로 터지면 3*3 삭제, 0번라인이면 플레이어에게 value1 * 최대체력 만큼 피해
	/// </summary>
	TimeBomb,
	/// <summary>화염 카드이며, 해당 카드는 주변의 카드 중 하나를 턴마다 랜덤으로 화염 카드로 변환한다.
	/// <para>화염 카드는 소화기 기믹 카드를 사용해 화염을 제거하지 않는 이상 사라지지 않는다.</para>
	/// </summary>
	Fire,
	/// <summary> "잿더미 : '화염'효과가 옮겨 붙은 카드를 소화기 기믹으로 삭제 시 해당 카드가 '잿더미'로 변환된다.(유닛, 기믹 카드 모두)
	/// <para>해당 카드는 체력이 존재하지 않고, 카드를 삭제하는 기믹의 효과는 적용된다.</para>
	/// <para>전방 3장의 카드에 잿더미가 들어올 경우, 해당 카드 입력 시 랜덤 보상을 준다.</para>
	/// <para>(예전 캐비닛 연출처럼 카드가 타버리는 연출 후 보상 아이템 등장)"</para>
	/// </summary>
	Ash,
	/// <summary> 분말 소화기 : 3x3 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
	PowderExtin,
	/// <summary> 투척 소화기 : 투척 소화기 : 1x1 범위 내 '화염' 효과 및 화염 카드를 제거한다. </summary>
	ThrowExtin,
	/// <summary> 연막탄 : 3x3 범위에 연막탄을 던져 '연막'을 형성한다.
	/// <para>연막탄 범위 속 카드들은 N턴 동안 이동할 수 없다.</para>
	/// </summary>
	SmokeBomb,
	/// <summary> 사슬 : 한 라인 전체(행)가 사슬 카드로만 형성되어 턴이 지날때마다 내려오며, 
	/// <para>기믹으로 '사슬 카드' 하나라도 제거 시 해당 라인의 사슬 카드는 모두 사라진다.</para>
	/// <para>*전방 3장의 카드에 사슬이 올 경우(3장 전부 사슬로 채워질 경우의 수 밖에 없음) 데미지를 입고 다음 턴으로.</para>
	/// <para>(턴 소모 + 데미지)</para>
	/// <para>*데미지는 [현재 파티 최대 체력] * [지정된 비율] 이다.</para>
	/// <para>1. 턴수(limitcount)로 생성</para>
	/// <para>2. 카드생성 확률에서는 제거</para>
	/// <para>3. 몬스터 죽음이나 다른 요인에의한 줄변경이 되면 안됨</para>
	/// <para>   해당 라인부터 카드 생성으로들어감</para>
	/// <para>4. 수류탄및 저격의 타겟이 되어야되며 한방이면 사슬 제거</para>
	/// </summary>
	Chain,
	/// <summary> 마비 다트 : 유닛에게 직접적으로 사용 가능하며, N턴 동안 이동이 불가능한 속박 효과를 부여한다. </summary>
	Paralysis,
	/// <summary> 좀비 하이브 : 좀비의 거처, 하이브 중심으로 3x3 범위 내 카드가 턴마다 한 장씩 랜덤 순서로, 랜덤의 좀비 카드로 변환된다.
	/// <para>(에너미 그룹)</para>
	/// <para>→ 하이브도 체력이 존재하지만 다른 카드와 위치가 교체되지 않으며, 저격이나 수류탄 류로 피해를 입혀 처치 가능.</para>
	/// <para>(화염병 및 다이너마이트 같이 카드를 삭제시키는 카드로도 파괴 가능)</para>
	/// <para>*전방 3장의 카드에 하이브가 들어올 경우 100% 확률로 좀비의 습격이 진행된다. </para>
	/// </summary>
	Hive,
	/// <summary> 냄새 폭탄 : 지정한 카드 위치에 냄새 폭탄을 던진다.
	/// <para>해당 카드 중심으로 3x3 범위 내 인간형 적이 있을 경우, 해당 카드와 반대 방향으로 이동한다. </para>
	/// </summary>
	SmellBomb,
	/// <summary> 소리 폭탄 : N턴 동안 소리를 내는 소리 폭탄을 지정한 카드 위치에 던진다.
	/// <para>필드 전체의 유닛들의 이동 방향이 소리를 내는 폭탄 쪽으로 향한다.</para>
	/// </summary>
	SoundBomb,
	/// <summary> 시너지 효과 카드 </summary>
	Synergy,
	/// <summary> 소탕용 카운트 체크 </summary>
	CountBox,
	/// <summary> 액티브 효과 카드 </summary>
	Support,
	/// <summary> 사용 시 선택한 카드를 제거한다. </summary>
	ShockBomb,
	/// <summary> 사용 시 5x5 범위 내 모든 카드를 제거한다. </summary>
	C4,
	/// <summary> 재료 </summary>
	Material,
	/// <summary> 샷건 카드, 전방 3x2 범위를 공격한다. </summary>
	Shotgun,
	/// <summary> 공중지원 카드, 액티브 스킬 공중지원과 효과가 동일하다. </summary>
	AirStrike,
	/// <summary> 5x5 범위 내 모든 어둠을 N턴 동안 삭제한다. </summary>
	StarShell,
	/// <summary> 5X5 범위 내 모든 화염을 삭제한다. </summary>
	PowderBomb,
	/// <summary> 행동력 회복 </summary>
	RecoveryAP,
	/// <summary> 스피드 업 [최종 스피드] = [본래 스피드] * (1 + [증가 비율]) </summary>
	SpeedUp,
	/// <summary> 크리티컬 확률 업 [최종 크리티컬 확률] = [본래 크리티컬 확률] + [증가 비율] </summary>
	CriticalUp,
	/// <summary> 크리티컬 데미지 업 [최종 크리티컬 데미지] = [본래 크리티컬 데미지] * (1 + [증가 비율]) </summary>
	CriticalDmgUp,
	/// <summary> 제한 턴 증가, 해당 값만큼 제한턴 추가 </summary>
	LimitTurnUp,
	/// <summary> 행동력 회복량 증가 [최종 턴당 회복량] = [본래 턴당 회복량] * (1 + [증가 비율]) </summary>
	APRecoveryUp,
	/// <summary> 행동력 소비량 감소 [최종 소비량] = [본래 소비량] * (1 - [감소 비율]) </summary>
	APConsumDown,
	/// <summary> 회복력 증가 [최종 회복력] = [본래 회복력] * (1 + [증가 비율]) </summary>
	HealUp,
	/// <summary> 레벨 증가(캐릭터 레벨과 착용 장비레벨 모두 증가) </summary>
	LevelUp,
	/// <summary> 화염 폭탄, 3*3 타겟에게 데미지 주고 일정 확률로 화상 상태로 만듬 </summary>
	FireGun,
	/// <summary> 네이팜 5*5 타겟에게 데미지 주고 일정 확률로 화상 상태로 만듬</summary>
	NapalmBomb,
	/// <summary> 과학자 시너지 효과와 동일. 타임어택 스테이지의 시간을 n% 늘려준다.  (Value01 = 추가 스테이지 시간 (백분율) </summary>
	TimePlus,
	/// <summary> 캐릭터 액티브 스킬인 도약 스킬 사용, 사용 시 해당 값만큼 카드 라인을 스킵한다. </summary>
	Jump,
	/// <summary> 캐릭터 액티브 스킬인 알코올 중독 스킬 사용, 사용 시 선택한 3x3 범위 내 에너미의 레벨을 N ~ N 사이 랜덤으로 다운 그레이드 한다. (Value 01 = 레벨 감소치 최소값, Value 02 = 레벨 감소치 최대값) *비고 : 남는 값은 반내림으로 처리 </summary>
	DownLevel,
	/// <summary> 사용 시 특정 캐릭터의 액티브 스킬 쿨타임이 초기화된다. </summary>
	AllCoolReset,
	/// <summary> 캐릭터 액티브 스킬인 기분 좋은 망각 스킬 사용, 사용 시 모든 캐릭터의 액티브 스킬 쿨타임이 감소한다. (Value 01 = 감소 쿨타임 수치 (절대값) </summary>
	CoolReset,
	/// <summary> 사용 시 모든 능력치 공격력/방어력/회복력을 일정 이상 상승한다. </summary>
	AllUpAdr,
	/// <summary> 캐릭터 액티브 스킬인 생존 본능 스킬 사용, 위생/포만/정신력을 약간씩 회복한다.  </summary>
	AllRecoverySrv,
	/// <summary> 캐릭터 액티브 스킬인 작살총 스킬 사용, 사용 시 종류에 상관 없이 카드 한 장을 선택하여 중앙으로 가져온다. </summary>
	CardPull,
	/// <summary> 캐릭터 액티브 스킬인 대폭발 스킬 사용, 사용 시 필드 위에 배치 된 폭발성 기믹 (수류탄, 다이너마이트, 오래된 지뢰, 시한 폭탄) 중  3x3 범위 내 기믹을 발동시킨다. </summary>
	Explosion,
	/// <summary> 캐릭터 액티브 스킬인 전파 방해 스킬 사용, 사용시 다음 공습까지의 턴수를 N턴 늘려준다. (Value 01 = 공습이 발생하지 않을 턴 수) </summary>
	BanAirStrike,
	/// <summary> 장애물 </summary>
	Roadblock,
	/// <summary> 드릴 </summary>
	Drill,
	/// <summary> 헤드샷을 명중 시킬 확률이 증가합니다. (일반 필드 전용) ( [최종 확률] = [기존 확률] + [추가 확률] </summary>
	HeadShotUp,
	/// <summary> 3x3 범위 내 무작위 적들을 N회 공격한다. (Value 01 = 타격 횟수, 피해 값 : 사용 캐릭터의 공격력 x ValueVase) </summary>
	RandomAtk,
	/// <summary> 선택한 3x3 범위 내 적의 이동을 N턴 동안 금지시키며, 피해를 준다.(Value01 = 이동 금지 턴 수, 피해 : 사용 캐릭터의 공격력 x ValueBase) </summary>
	StopCard,
	/// <summary> 3x3 범위 내 모든 재료 카드의 개수를 +1 추가 한다. (맥시멈 카드는 개수가 추가되지 않는다.) </summary>
	PlusMate,
	/// <summary> 파괴 물가 장애물 </summary>
	AllRoadblock,
	/// <summary>  타워 전용_선택 시 스테이터스 회복해주는 피난민 카드 4장이 리스트로 출력, 선택 시 스테이터스 회복 </summary>
	Tower_Refugee,
	/// <summary> 타워 전용_선택 시 랜덤으로 출력되는 카드 리스트 중 하나 선택하여 보상 습득 </summary>     
	Tower_SupplyBox,
	/// <summary> 타워 전용_선택 시 휴식 카드 연출 출력되며 HP 및 3개 스테이터스 회복 </summary>     
	Tower_Rest,
	/// <summary>  타워 전용_선택 시 특정 스테이터스 감소 또는 증가 </summary>     
	Tower_Status,
	/// <summary> 타워 전용_시작 지점 </summary>
	Tower_Entrance,
	/// <summary> 타워_공개 이벤트 </summary>
	Tower_OpenEvent,
	/// <summary> 타워_비공개 이벤트 </summary>
	Tower_SecrectEvent,
	/// <summary> 도박 카드 </summary>
	Gamble,
	/// <summary> "구멍 뚫린 가방 : 머지 슬롯 N칸 사용 불가 (보통 1칸으로 설정 예정)" </summary>
	ConMergeSlotDown,
	/// <summary> "제한된 선택 : 해당 스테이지 필드 위 모든 재료 카드 개수 N개로 일괄 변경 (보통 1개로 설정 예정)" </summary>	
	MaterialCountDown,		
	/// <summary> "부상 : 회복력 N% 감소(보통 90%로 설정 예정)" </summary>
	Wound,
	/// <summary> "신중한 선택 : 제한적인 행동력 컨셉 시작 행동력 : N 턴마다 행동력 회복 : 0 (고정) (보통 시작 행동력 100, 턴 당 행동력 회복 0으로 설정 예정)" </summary>
	ConActiveAP,
	/// <summary> 암흑 : 해당 스테이지 내 남은 턴 수 보이지 않음 </summary>
	DarkTurn,
	/// <summary> 아직 한 발 남았다 : 캐릭터 액티브 스킬 사용 횟수 N회 제한 </summary>
	ConActiveSkillLimit,
	/// <summary> 어색한 움직임 : 액티브 스킬 사용 시 행동력 소모 값 증가 </summary>
	ApPlus,
	/// <summary> 잡식성 : 스테이지 내 재료 카드의 내용이 보이지 않고, 카드 선택 시 랜덤 종류의 재료, 랜덤 개수로 지급 </summary>
	RandomMaterial,
	/// <summary>  버프 알레르기 : 버프 카드 습득해도 버프가 적용되지 않음 </summary>
	HateBuff,
	/// <summary> "N턴 마다 0번째 라인의 카드 선택이 랜덤으로 자동 선택 됨 예) Value01에 5라고 입력 시, 5번째 턴에 랜덤으로 자동 선택" </summary>
	ConRandomChoice,
	/// <summary> "망각 : N턴 마다 머지 탭의 모든 아이템이 사라짐 예) Value01에 5라고 입력 시, 5번째 턴에 랜덤으로 자동 선택" </summary>
	MergeDelete,
	/// <summary>  합심 : 더 이상 에너미들이 서로를 공격하지 않음 (위치 변경만) </summary>
	ConEnemyTeamwork,
	/// <summary> 무게 초과 : N턴 마다 한 라인 선택을 스킵하고 다음 라인으로 넘어감 </summary>
	ConSkipTurn,        
	/// <summary> "무너진 길 : 매 턴 마다 0번째 라인 카드 중 N개 선택 불가 (StageTable의 PlayType_CardLock과 동일 기능)" </summary>
	ConStageCardLock,
	/// <summary>  차선책 : 전투 보상이 랜덤으로 N개 선택 불가 </summary>
	ConRewardCardLock,
	/// <summary> "블라인드 : 전투 보상 습득 페이지에서 전투 보상 내용 확인 불가 예) 카드 이미지와 설명 모두 확인 불가" </summary>
	ConBlindRewardInfo,
	/// <summary> 행동 불능 : 랜덤으로 N명의 캐릭터 액티브 스킬 사용 불가 (스테이터스는 적용) </summary>
	ConKnockDownChar,
	/// <summary> "고장난 기계 : 재료를 모아 머지 탭에서 제작 시 N% 확률로 제작 실패 제작 실패 시 제작에 사용된 모든 재료는 사라짐" </summary>
	MergeFailChance,
	/// <summary> 출혈 : 액티브 스킬을 사용할 때 마다 HP 감소 (매번 감소) </summary>
	SkillHp,
	/// <summary>  울렁증 : 액티브 스킬 사용할 때 마다 모든 스테이터스 감소 (매번 감소) </summary>
	SkillStatus,
	/// <summary> 재굴림 증감 </summary>
	AddRerollCount,
	/// <summary> 정신력 회복/감소 퍼센트 </summary>
	PerRecoveryMen,
	/// <summary> 위생 회복/감소 퍼센트 </summary>
	PerRecoveryHyg,
	/// <summary> 허기 회복/감소 퍼센트 </summary>
	PerRecoverySat,
	/// <summary> 은신 버프 </summary>
	Hide,
	/// <summary> 아무것도 안하는 타입 </summary>
	Empty,
	/// <summary> 아무것도 안하는데 카운팅은 함 </summary>
	CountEmpty,
	/// <summary> "구멍 메운 가방 : 머지 슬롯 N칸 사용 가능 (보통 1칸으로 설정 예정)" </summary>
	MergeSlotCount,
	/// <summary> 3*3 폭발해서 에너미의 최대HP * Value01 만큼,3턴 동안 데미지, 화상으로 사망시 기본 0 화염으로 변경 / 에너미 이외는 value02 화염카드로 변경 </summary>
	Oil,
	/// <summary> 5*5 폭발해서 에너미의 최대HP * Value01 만큼,3턴 동안 데미지, 화상으로 사망시 기본 0 화염으로 변경 / 에너미 이외는 value02 화염카드로 변경 </summary>
	GasStation,
	/// <summary> 습득 시 아이템 획득, change 대신 rewardproc 탐 </summary>
	Supplybox02,
	/// <summary> 습득시 머지칸으로 들어가는 완제품 카드들 </summary>
	SaveCard,
	/// <summary> 해당 스테이지의 1단계 아이템 제작 요구 재료 개수가 N개씩 증가 </summary>
	MoreMaterial,
	/// <summary> 하단 체력바가 보이지 않음 </summary>
	NoHpBar,
	/// <summary> 에너미 등장 확률이 대폭 상승함
	///(AppearProp 값 + (AppearProb x 상승 확률)로 계산)
	///(기재된 등장 확률 : 300 일 경우 300 + (300x0.3)로 계산) 
	///	</summary>
	EnemyProbUp,
	/// <summary> HP 자동 회복 삭제 </summary>
	NoAutoHeal,
	/// <summary> 청결, 포만, 정신이 턴마다 일정 수치(value1) 감소 (백분율) </summary>
	TurnAllStatus,
	/// <summary> 생존스탯(value1)이 턴마다 일정 수치(value2) 감소 (백분율) </summary>
	TurnStatusMen,
	TurnStatusHyg,
	TurnStatusSat,
	TurnStatusHp,
	/// <summary> 에너미가 무조건 전진 </summary>
	GoEnemy,
	/// <summary> 스테이지 내 모든 몬스터 즉사 몬스터(value1)로 변경 </summary>
	ConDeadly,
	/// <summary> 자동습득하는 선택 보상 상자 </summary>
	Item_RewardBox,
	/// <summary> 유저 피해 안입히는 지뢰 </summary>
	Allymine,
	/// <summary>  </summary>
	End
}
/// <summary> 스테이지 카드 등장 조건 타입 </summary>
public enum StageCardAppearType
{
	/// <summary> 특정 카드 사용 </summary>
	CardUse,
	/// <summary> 날짜가 지남 </summary>
	Time,
	/// <summary> 적 사냥 </summary>
	KillEnemy,
	/// <summary> 퀘스트 조건 완료 시 </summary>
	ClearCondition,
	/// <summary> 없음. </summary>
	None
}
/// <summary> 스테이지 제작 재료 종류 </summary>
public enum StageMaterialType
{
	/// <summary> 총알 </summary>
	Bullet = 0,
	/// <summary> 화약 </summary>
	GunPowder,
	/// <summary> 약품 </summary>
	Medicine,
	/// <summary> 베터리 </summary>
	Battery,
	/// <summary> 가루 </summary>
	Powder,
	/// <summary> 식재료 </summary>
	Food,
	/// <summary> 알콜 </summary>
	Alcohol,
	/// <summary> 허브 </summary>
	Herb,
	/// <summary> 가솔린 </summary>
	Gasoline,
	/// <summary> 기본재료 </summary>
	DefaultMat = Gasoline,
	/// <summary> 저격 </summary>
	Sniping,
	/// <summary> 샷건 </summary>
	ShotGun,
	/// <summary> 개틀링 </summary>
	GatlingGun,
	/// <summary> 공중지원 </summary>
	AirStrike,
	/// <summary> 충격탄 </summary>
	ShockBomb,
	/// <summary> 수류탄 </summary>
	Grenade,
	/// <summary> 다이너마이트 </summary>
	Dynamite,
	/// <summary> 약병 </summary>
	MedBottle,
	/// <summary> 구급상자 </summary>
	FirstAidKit,
	/// <summary> 치료 키트 </summary>
	CureKit,
	/// <summary> 빵 </summary>
	Bread,
	/// <summary> 햄버거 </summary>
	Hamburger,
	/// <summary> 스테이크 </summary>
	Steak,
	/// <summary> 소독제(청결) </summary>
	Disinfectant,
	/// <summary> 비누 </summary>
	Soap,
	/// <summary> 샴푸 </summary>
	Shampoo,
	/// <summary> 양초 </summary>
	Candle,
	/// <summary> 향수 </summary>
	Perfume,
	/// <summary> 각성제 </summary>
	Drug,
	/// <summary> 형광 스틱 </summary>
	LightStick,
	/// <summary> 손전등 </summary>
	FlashLight,
	/// <summary> 조명탄 </summary>
	Flare,
	/// <summary> 소화 스프레이 </summary>
	FireSpray,
	/// <summary> 소화기 </summary>
	FireExtinguisher,
	/// <summary> 포말 소화기 </summary>
	PowderBomb,
	/// <summary> 화염 폭탄 </summary>
	FireBomb,
	/// <summary> 화염 폭탄 </summary>
	FireGun,
	/// <summary> 네이팜 </summary>
	NapalmBomb,
	/// <summary> c4 </summary>
	C4,
	/// <summary> 없음. </summary>
	None
}
/// <summary> 스테이지 제작 오픈 조건 </summary>
public enum StageMakingConditionType
{
	/// <summary> 없음 - 기본적으로 열려 있음 </summary>
	None,
	/// <summary> 캐릭터가 파티에 포함되어 있음 </summary>
	Character,
	/// <summary> 목표 연구가 완료되어있음. </summary>
	Research,
	/// <summary> 목표 스테이지를 클리어 했어야 함 </summary>
	Stage,
	/// <summary>  </summary>
	End
}