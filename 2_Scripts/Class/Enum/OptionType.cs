
public enum OptionType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 해당 캐릭터가 공격시 흡혈% </summary>
	Vampire,
	/// <summary> 늑대 : 해당 캐릭터가 공격시 추가데미지% </summary>
	AttackingDmgAdd,
	/// <summary> 곰 : 해당 캐릭터가 공격시 대상방어력감소% </summary>
	AttackingDefDown,
	/// <summary> 오우거 : 해당 캐릭터가 공격시 확률스턴% </summary>
	AttackingStun,
	/// <summary> 사신 : 해당 캐릭터가 공격시 확률즉사% </summary>
	AttackingKill,
	/// <summary> 환영 : 해당 캐릭터가 피격시 완전회피% </summary>
	HitDodge,
	/// <summary> 거북이 : 해당 캐릭터가 피격시 데미지감소% </summary>
	HitDmgDown,
	/// <summary> 고슴도치 : 해당 캐릭터가 피격시 데미지반사% </summary>
	HitThorn,
	/// <summary> 아르마딜로 : 해당 캐릭터가 피격시 방어력증가% </summary>
	HitDefUp,
	/// <summary> 트롤 : 해당 캐릭터가 회복시 추가회복% </summary>
	CureHealAdd,
	/// <summary> 표범 : 해당 캐릭터가 회복시 행동력회복% </summary>
	CureApAdd,
	/// <summary> 베헤모스 : 해당 캐릭터의 행동력소모감소% </summary>
	ApConsumDown,
	/// <summary> 말벌 : 해당 캐릭터의 공격력증가% </summary>
	AtkUp,
	/// <summary> 골렘 : 해당 캐릭터의 방어력증가% </summary>
	DefUp,
	/// <summary> 연금술사 : 해당 캐릭터의 회복력증가% </summary>
	HealUp,
	/// <summary> 오소리 : 해당 캐릭터의 정신 최대치증가% </summary>
	MenUp,
	/// <summary> 물범 : 해당 캐릭터의 위생 최대치증가% </summary>
	HygUp,
	/// <summary> 멧돼지 : 해당 캐릭터의 포만감 최대치증가% </summary>
	SatUp,
	/// <summary> 프랑켄 : 해당 캐릭터 피격시 정신 감소감소% (네이밍 꼬라지보소) </summary>
	HitMenDefUp,
	/// <summary> 리자드맨 : 해당 캐릭터 피격시 위생 감소감소% </summary>
	HitHygDefUp,
	/// <summary> 드레이크 : 해당 캐릭터 피격시 포만감 감소감소% </summary>
	HitSatDefUp,
	/// <summary> 그리폰 : 해당 캐릭터 공격 시 크리티컬 적중 확률 증가 </summary>
	AttackingCriShoot,
	/// <summary> 마녀 : 해당 캐릭터가 공격 시 스킬 쿨타임 초기화 </summary>
	AttackingCoolDown,
	/// <summary> 사스콰치 : 해당 캐릭터가 피격 시 스킬 쿨타임 초기화 </summary>
	HitCoolDown,
	/// <summary> 고블린 : 해당 캐릭터가 공격 시 랜덤한 재료 획득 (레벨에 따라 확률 및 개수 증가) </summary>
	AttackingMaterialAdd,
	/// <summary> 서큐버스 : 해당 캐릭터가 피격 시 랜덤한 재료 획득 (레벨에 따라 확률 및 개수 증가) </summary>
	HitMaterialAdd,
	/// <summary> 베헤모스 : HP로 인한 사망 시 확률적으로 100%로 부활
	/// <para>- 레벨에 따라 %증가</para>
	/// <para>- 중복 착용 시 HP 회복량만 누적시키고 HP에 의한 사망 후 부활은 1스테이지당 1회로 한정</para>
	/// </summary>
	HpResurrection,
	/// <summary> 말벌 : 해당 캐릭터의 공격이 헤드샷으로 명중 시 주변 9칸의 적들 모두 사망
	/// <para>- 스킬에도 적용</para>
	/// </summary>
	ExplosiveHeadshot,
	/// <summary> 골렘	 : 해당 캐릭터의 공격이 헤드샷으로 명중 시 일직선 상의 적들 모두 사망 
	/// <para>- 스킬에도 적용</para>
	/// </summary>
	PenetratingHeadshot,
	/// <summary> 오소리 : 정신으로 인해 사망 시 해당 캐릭터의 최대 정신의 @%가진 채로 부활
	/// <para>- 레벨에 따라 %증가</para>
	/// <para>- 중복 착용 시 정신 회복량만 누적시키고 정신에 의한 사망 후 부활은 1스테이지당 1회로 한정</para>
	/// </summary>
	MenResurrection,
	/// <summary> 물범 : 위생으로 인해 사망 시 해당 캐릭터의 최대 위생의 @%가진 채로 부활
	/// <para>- 레벨에 따라 %증가</para>
	/// <para>- 중복 착용 시 위생 회복량만 누적시키고 위생에 의한 사망 후 부활은 1스테이지당 1회로 한정</para>
	/// </summary>
	HygResurrection,
	/// <summary> 멧돼지 : 포만감으로 인해 사망 시 해당 캐릭터의 최대 포만감의 @%가진 채로 부활
	/// <para>- 레벨에 따라 %증가</para>
	/// <para>- 중복 착용 시 포만감 회복량만 누적시키고 포만감에 의한 사망 후 부활은 1스테이지당 1회로 한정</para>
	/// </summary>
	SatResurrection,
	/// <summary> 프랑켄 : 착용자의 공격으로 상대방이 사망 시 HP, 정신,위생, 포만감 모두 회복
	/// <para>- 레벨에 따라 회복 비율 증가</para>
	/// </summary>
	KilltoHeal,
	/// <summary> 리자드맨 : 착용자의 공격으로 상대방이 사망 시 전체 스킬 쿨 감소
	/// <para>- 레벨에 따라 감소 쿨 증가</para>
	/// </summary>
	KilltoCoolDown,
	/// <summary> 드레이크 : 해당 캐릭터가 피격 시 공격자가 확률적으로 사망 </summary>
	DeathThorn,
	/// <summary>  </summary>
	End
}

public enum DNABGType
{
	None = 0,
	Red,
	Blue,
	Green,
	Purple,
	End
}