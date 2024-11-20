public enum DialogType
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 지문형 - 한줄글 </summary>
	Narration,
	/// <summary> 대화형 - 대화형 </summary>
	Talk,
	End
}
public enum DialogTalkDir
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 포트레이트가 우측에 출력 </summary>
	Right,
	/// <summary> 포트레이트가 좌측에 출력 </summary>
	Left,
	End
}
public enum DialogTalkType {
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 없음 </summary>
	Normal = 1,
	/// <summary> 없음 </summary>
	Think = 2,
	/// <summary> 없음 </summary>
	Shout = 3,
	End
}
public enum DialogSelectLimitType
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 해당 선택지를 선택 가능한 캐릭터 </summary>
	Job,
	End
}
public enum DialogRewardType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 유틸리티 및 기믹 카드 </summary>
	Card,
	/// <summary> 다음 라인의 카드 교체 </summary>
	NextLineChange,
	/// <summary> 모든 카드 교체 </summary>
	AllChangeCard,
	/// <summary> 히든 Area 추가 (추후) </summary>
	AreaAppear,
	/// <summary> Skill 제공 </summary>
	Skill,
	/// <summary> 끝 </summary>
	End
}
