/// <summary> 스텟 종류 </summary>
/// 절대 순서 변경하면 안됨
/// 툴데이터에서 번호를 사용함
public enum BuffType
{
	/// <summary> HP 증가 </summary>
	HPMAX = 0,
	/// <summary> 공격력 상승 </summary>
	ATK,
	/// <summary> 방어력 상승 </summary>
	DEF,
	/// <summary> 기력 회복 속도 증가 </summary>
	RECSTA,
	/// <summary> 포만도 증가 </summary>
	SATMAX,
	/// <summary> 청결도 증가 </summary>
	HYGMAX,
	/// <summary> 정신력 증가 </summary>
	MENMAX,
	/// <summary> 도트힐 </summary>
	HEAL,
	/// <summary> 종료 </summary>
	None
}
