
public enum GuildGrade
{
	/// <summary> 일반 유저 </summary>
	Normal = 0,
	/// <summary> 다음 마스터 </summary>
	Spare_Master = 99,
	/// <summary> 마스터 </summary>
	Master = 100,
	/// <summary>  </summary>
	End
}
public enum GuildJoinType
{
	/// <summary> 자동 가입 </summary>
	Auto = 0,
	/// <summary> 승인 받기 </summary>
	Approval,
	/// <summary> 비공개 </summary>
	Private,
	/// <summary>  </summary>
	End
}
public enum GuildBuffType
{
	/// <summary> 없음 </summary>
	None = 0,
	/// <summary> 맴버 수 증가 </summary>
	MemberMaxUp,
	/// <summary>  </summary>
	End
}