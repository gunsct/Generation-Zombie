public enum DialogueConditionType
{
	/// <summary>  </summary>
	None = 0,
	/// <summary> 스테이지 시작 시 </summary>
	StartStage,
	/// <summary> 피난민 습득 시 </summary>
	RescueRefugee,
	/// <summary> 원거리 공격 피격 시 </summary>
	HitShot,
	/// <summary> 정신력 스테이터스 상승 시 </summary>
	MenUp,
	/// <summary> 정신력 스테이터스 하락 시 </summary>
	MenDown,
	/// <summary> 포만감 스테이터스 상승 시 </summary>
	SatUp,
	/// <summary> 포만감 스테이터스 하락 시 </summary>
	SatDown,
	/// <summary> 위생 스테이터스 상승 시 </summary>
	HygUp,
	/// <summary> 위생 스테이터스 하락 시 </summary>
	HypDown,
	/// <summary> 행동력 부족으로 스킬 사용 불가 시 </summary>
	LowAP,
	/// <summary> 액티브 스킬 사용 시 </summary>
	UseSkill,
	/// <summary> 액티브 스킬 사용 불가 시 예) 목표로 지정할 대상이 없을 경우 </summary>
	UnSkill,
	/// <summary> 남은 턴 수가 일정 횟수 이하일 시 (예 : Value01 = 3 일 경우 현재 남은 턴수가 3턴일 때 출력) </summary>
	LimitTime,
	/// <summary> 확률적으로 스킬 사용 실패 시 </summary>
	SkillFail,
	/// <summary> 벽 카드 터치 시 </summary>
	Wall,
	/// <summary> 전투 보상 시간이 전부 지나 보상을 선택 및 습득하지 못했을 경우 </summary>
	RewardTime,
	/// <summary>  </summary>
	ZombieViolentTalk,
	/// <summary>  </summary>
	ZombieGoodTalk,
	/// <summary>  </summary>
	ZombieSmartTalk,
	/// <summary>  </summary>
	ZombieSociableTalk,
	/// <summary>  </summary>
	ZombieUnsociableTalk,
	/// <summary>  </summary>
	ZombieUnpredictableTalk,
	/// <summary>  </summary>
	ZombieUnstableTalk,
	/// <summary>  </summary>
	ZombieStableTalk,
	/// <summary> 체력이 일정 수치 이하일 시 </summary>
	UnderHp,
	/// <summary> 적 처치 시 </summary>
	KillEnemy,
	/// <summary> 재료 아이템 머지 시 </summary>
	MergeItem,
	/// <summary> 헤드샷 성공 시 </summary>
	HeadShot,
	/// <summary> 최고 단계 머지 성공 시 </summary>
	HighMergeItem,
	/// <summary> 0번째 라인에서 Enemy 카드 외 다른 카드 선택 시 </summary>
	OtherSelect,
	/// <summary> 포만감 디버프 효과 적용 시 </summary>
	SatDebuffOn,
	/// <summary> 포만감 디버프 효과 해제 시 </summary>
	SatDebuffOff,
	/// <summary> 청결도 디버프 효과 적용 시 </summary>
	HygDebuffOn,
	/// <summary> 청결도 디버프 효과 해제 시 </summary>
	HygDebuffOff,
	/// <summary> 정신력 디버프 효과 적용 시 </summary>
	MenDebuffOn,
	/// <summary> 정신력 디버프 효과 해제 시 </summary>
	MenDebuffOff,
	/// <summary> 카드락으로 잠긴 카드 선택 경우 </summary>
	CardLock,
	/// <summary> 쿨타임이 있어 스킬을 사용할 수 없을 경우 </summary>
	CoolTime,
	/// <summary> 하단 머지탭에서 눌러 사용하는 카드 중 현재 사용 조건이 불충족 되어 사용할 수 없는 경우 </summary>
	UnUseItem,
	/// <summary> 캐릭터 뽑기 대사 </summary>
	Gacha,
	/// <summary>  </summary>
	End,
}

public enum CharDialogueType
{
	None,
	Normal,
	Say,
	Think,
	Shout,
	Negative
}

public enum PersonalityType
{
	None
	,Attack
	,Negative
	,Defend
}
