public enum RewardKind
{
	/// <summary> 캐릭터 </summary>
	None = 0,
	/// <summary> 캐릭터 </summary>
	Character,
	/// <summary> 아이템 </summary>
	Item,
	/// <summary> 좀비 </summary>
	Zombie,
	/// <summary> DNA </summary>
	DNA,
	/// <summary> 스테이지 버프용 카드 </summary>
	StageCard,
	/// <summary> 이벤트(스테이지 클리어 보상에서만 적용함) </summary>
	Event,
	/// <summary> addevent 전투용, enemyindx /summary>
	Enemy,
	END
}

public enum AlternativeMode
{
	Prologue = 0,	//프롤로그 모드(3번 캐릭터 선택 후 스토리 지문 출력
	StageEnd		//캐릭터 or 아이템 선택 1번 하고 획득 연출
}