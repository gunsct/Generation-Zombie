public enum ClearMethodType {
	/// <summary> 클리어 조건1만 하면 완료</summary>
	None,
	/// <summary> 연속 미션 - 01~03 순서로 하나씩 미션을 지급</summary>
	Continuity,
	/// <summary> 동시 미션 - 01~03을 동시에 지급</summary>
	SameTime,
}