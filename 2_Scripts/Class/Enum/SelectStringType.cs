public enum SelectStringType
{
	/// <summary> 없음, 일반 선택지</summary>
	None = 0,
	/// <summary> 일정 시간 뒤 내용이 변하는 선택지 </summary>  
	TimeChange,
	/// <summary> 선택할 수 없는 선택지 </summary>  
	TimeBlock,
	/// <summary> 이벤트 타입에 등장하는 선택지 </summary>  
	Event,
	End
}