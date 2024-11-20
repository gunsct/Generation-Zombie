public enum TowerEventType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 입구 </summary>    
	Entrance,
	/// <summary> 일반 에너미 </summary>    
	NormalEnemy,
	/// <summary> 정예 에너미 </summary> 
	EliteEnemy,
	/// <summary> 보스 에너미 (목표) </summary>  
	Boss,
	/// <summary> 공개 이벤트 </summary>    
	OpenEvent,
	/// <summary> 비공개 이벤트 </summary>   
	SecrectEvent
}
public enum TowerSOEventType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 보급 상자 (일반), 이로운 카드 중 하나를 선택 </summary>
	NormalSupplyBox,
	/// <summary> 보급 상자 (고급), 이로운 카드 중 하나를 선택 </summary>
	EpicSupplyBox,
	/// <summary> 피난민 </summary>
	Refugee,
	/// <summary> 휴식 </summary>
	Rest,
	/// <summary> 해로운 카드 중 하나를 선택 </summary>
	BadSupplyBox,
	/// <summary> 스테이터스 버프, 디버프 카드 (HP 30% 회복 or 감소, 정신력, 포만감, 청결 30 증가 or 감소) </summary>
	StatusBuffEvent,
	/// <summary> 해당 스테이지에 배치 된 에너미 중 (보스 에너미 제외) 하나가 기습, 기습 시 EnemyEvent와 같은 프로세스로 진행 </summary>
	SuddenAttack
}
public enum TowerSOType
{
	Open,
	Secret,
	All
}