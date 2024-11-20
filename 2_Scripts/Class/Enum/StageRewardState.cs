public enum StageRewardState
{
	/// <summary> 아무 제한 없음 </summary>
	Normal,
	/// <summary> 전투 보상 중 일부가 뒤집어지지 않아서 보이지 않음 (단, 랜덤을 원한다면 선택 가능) </summary>
	Blind,
	/// <summary> 전투 보상이나 필드 카드 중에 1장의 카드가 랜덤하게 선택이 불가능해짐 (어떤 카드인지는 보임) </summary>
	CardRandomLock,
	/// <summary>정신력 수치가 일정 수치 이하로 내려가면 전투 보상과 필드 카드 중 1장이 랜덤하게 확률적으로 잠겨서 선택이 불가능해짐(앞에 다른 카드로 덮어씌울 것이라 보이지 않아도 무관)  </summary>
	MenLow,
	/// <summary> 디버프로 RewardCardLock 있으면 전투 보상이 랜덤으로 N개 선택 불가 </summary>
	DebuffRewardCardLock,
}