public enum PlayType
{
	/// <summary> None </summary>
	None,
	/// <summary> 해당 스테이지에서 캐릭터 액티브 스킬 사용 불가 </summary>
	BanActive,   
	/// <summary> 스테이지 진행 단위가 턴이 아닌 시간으로 변경, (PlayTypeValue = 제한 시간, 초 단위로 표기, 300일 경우 5분)-> 선택지 선택 시 N초 안에 선택 안할 시 실패 </summary>
	ChangeTime,
	/// <summary> "스테이지 턴 제한과 타임 어택 시간 동시 표기, (PlayTypeValue 01 = 제한 시간, 제한 턴은 LimitTurn 칼럼 참조)" </summary>
	TurnTime,	
	/// <summary> "캐릭터 중 가장 전투력이 높은 캐릭터 N명 행동, (PlayTypeValue = 행동 불능 대상 수), (액티브 스킬만 사용 불가)"</summary>
	HighCharOut,	
	/// <summary> "캐릭터 중 가장 전투력이 낮은 캐릭터 N명 행동 불능, (PlayTypeValue = 행동 불능 대상 수), (액티브 스킬만 사용 불가)"</summary>
	LowCharOut,	
	/// <summary> "랜덤 캐릭터 N명 행동 불능, (PlayTypeValue = 행동 불능 대상 수), (액티브 스킬만 사용 불가)"</summary>
	RandomCharOut,	
	/// <summary> "0번째 라인의 선택 가능한 카드 3장 중 랜덤으로 N 장은 선택 불가, (PlayTypeValue = 랜덤 선택 불가 카드 개수)"</summary>
	CardLock,
	/// <summary> 0번째 라인에 이전 선택한 방향 카드가 선택 불가. </summary>
	EasyCardLock,
	/// <summary> "N턴 마다 공습 발생, 필드의 적과 플레이어에게 데미지 (턴 수 고정), (PlayTypeValue01 = 공습 발생 턴 수, PlayTypeValue02 = 감소될 체력 %, 백분율)"</summary>
	FieldAirstrike,
	/// <summary> 스킬 쿨타임 없음</summary>
	NoCool,
	/// <summary>0번째 라인의 카드 세 장이 뒷 면으로 뒤집히고 섞인 상태에서 고르는 모드 </summary>
	RandomPick,
	/// <summary> 잠입, N번이상 전투하면 패배</summary>
	TurmoilCount,
	/// <summary> 전투 보상 중 일부가 뒤집어지지 않아서 보이지 않음 </summary>
	Blind,
	/// <summary> 전투 보상이나 필드 카드 중에 1장의 카드가 랜덤하게 선택이 불가능해짐 </summary>
	CardRandomLock,
	/// <summary> 어둠 스테이지에서 랜덤 위치로 카드를 밝힘. (PlayTypeValue 01 = 가로등 생성 개수, PlayTypeValue 02 = 가로등 발생 턴 수) </summary>
	StreetLight,
	/// <summary> 화염 스테이지 N턴마다 화재 번짐 </summary>
	FireSpread,
	/// <summary> 행동력 회복 0 </summary>
	APRecvZero,
	/// <summary> N턴 마다 5x5 범위 내 모든 카드의 위치를 랜덤하게 다시 배치한다. (타워에서만 사용) </summary>
	CardShuffle,
	End
}