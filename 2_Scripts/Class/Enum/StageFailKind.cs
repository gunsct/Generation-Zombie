
public enum StageFailKind {
	/// <summary> 스테이지 실패 조건 으로 패배 </summary>
	FailType,
	/// <summary> 턴 카운트 제한 </summary>
	Turn,
	/// <summary> HP 0 </summary>
	HP,
	/// <summary> 정신 0 </summary>
	Men,
	/// <summary> 위생 0 </summary>
	Hyg,
	/// <summary> 포만감 0 </summary>
	Sat,
	/// <summary> 훈련 시간제한 </summary>
	Time,
	/// <summary> 전투 횟수 제한 </summary>
	TurmoilCount,
	/// <summary> 다른 임무가 남아있을 때 </summary>
	OtherMission,
	None
}