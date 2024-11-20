public enum SelectGroupType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 보통 선택지 레이아웃 </summary>
	Normal,
	/// <summary> 타임 어택 보통 레이아웃 출력. 선택지 고르는 제한 시간 30초, [Value01 = 제한 시간(초)] 
	/// 제한 시간 내에 선택하지 않을 경우. 
	/// (1) GID 그룹 내 Hide 값을 TRUE로 가진 선택지가 있을 경우 해당 선택지 자동 선택.
	/// (2) GID 그룹 내 Hide 값이 모두 FALSE일 경우 FALSE인 선택지 중 하나 랜덤으로 선택. 
	/// </summary>  
	NormalTime,
	/// <summary> 운명 선택지 레이아웃 </summary> 
	Fated,
	/// <summary> 타임 어택 운명 레이아웃 출력. 선택지 고르는 제한 시간 30초, [Value01 = 제한 시간(초)] 
	/// 제한 시간 내에 선택하지 않을 경우. 
	/// (1) GID 그룹 내 Hide 값을 TRUE로 가진 선택지가 있을 경우 해당 선택지 자동 선택.
	/// (2) GID 그룹 내 Hide 값이 모두 FALSE일 경우 FALSE인 선택지 중 하나 랜덤으로 선택. 
	/// </summary>  
	FatedTime

}