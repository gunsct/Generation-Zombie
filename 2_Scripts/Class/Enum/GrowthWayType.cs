public enum GrowthWayType
{
	/// <summary> 캐릭터 레벨 업 가능 시, 캐릭터 장비 레벨 업 가능 시, 캐릭터 승급 가능 시 </summary>
	CharacterUp = 0,
	/// <summary> 현재 플레이 할 수 있는 다운타운 스테이지가 있을 경우 </summary>
	GetMaterial,
	/// <summary> 상점은 알람에서 제외 </summary>
	Shop,
	/// <summary> 긴급임무 </summary>
	Replay,
	End,
}