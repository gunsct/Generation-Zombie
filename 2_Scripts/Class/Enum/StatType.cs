/// <summary> 스텟 종류 </summary>
/// 절대 순서 변경하면 안됨
/// 툴데이터에서 번호를 사용함
public enum StatType
{
	None = -1,
	/// <summary> 정신 : 0이 되면 사망 </summary>
	Men = 0,
	/// <summary> 위생 : 0이 되면 사망 </summary>
	Hyg = 1,
	/// <summary> 포만감 : 0이 되면 사망 </summary>
	Sat = 2,
	/// <summary> 생존스텟 종료 </summary>
	SurvEnd = 3,
	/// <summary> 체력 : 캐릭터의 체력(HP). </summary>
	HP = SurvEnd,
	/// <summary> 공격력 : 캐릭터의 공격력. </summary>
	Atk = 4,
	/// <summary> 방어력 : 캐릭터의 방어력.  </summary>
	Def = 5,
	/// <summary> 기력 : 캐릭터의 기력.(전투시 회피할때 깎이는 수치) </summary>
	Sta = 6,
	/// <summary> 기력회복 : 캐릭터의 기본 기력회복값(UI에 표기되지 않음) </summary>
	RecSta = 7,
	/// <summary> 가드 횟수 </summary>
	Guard = 8,
	/// <summary> HP 회복량 </summary>
	Heal = 9,
	/// <summary> 기본 노트 공격력 </summary>
	NormalNote = 10,
	/// <summary> 베기 노트 공격력 </summary>
	SlashNote = 11,
	/// <summary> 콤보 노트 공격력 </summary>
	ComboNote = 12,
	/// <summary> 챠지 노트 공격력 </summary>
	ChargeNote = 13,
	/// <summary> 체인 노트 공격력 </summary>
	ChainNote = 14,
	/// <summary> 정신 방어력 </summary>
	MenDecreaseDef = 15,
	/// <summary> 위생 방어력 </summary>
	HygDecreaseDef = 16,
	/// <summary> 포만감 방어력 </summary>
	SatDecreaseDef = 17,
	/// <summary> 속도 </summary>
	Speed = 18,
	/// <summary> 치명타 </summary>
	Critical = 19,
	/// <summary> 치명타 데미지 </summary>
	CriticalDmg = 20,
	/// <summary> 헤드샷 </summary>
	HeadShot = 21,
	/// <summary> 자신의 액션 포인트 소모량 조정 </summary>
	ActionPointDecrease = 22,
	/// <summary> 명중률 </summary>
	SuccessAttackPer = 23,
	/// <summary> 도망 확률 </summary>
	Run = 24,
	/// <summary> 종료 </summary>
	Max
}

public enum StatValType
{
	/// <summary> 퍼센트 </summary>
	Ratio,
	/// <summary> 절대값 </summary>
	ABS
}
