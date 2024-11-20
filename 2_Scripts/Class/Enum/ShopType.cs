public enum ShopType
{
	/// <summary> 가차 </summary>
	Gacha = 0,
	/// <summary> 맴버쉽 </summary>
	Membership,
	/// <summary> 블랙마켓 (암시장) </summary>
	BlackMarket,
	/// <summary> 유료 화폐 상점 </summary>
	GoldTooth,
	/// <summary> 달러 상점 </summary>
	Bank,
	/// <summary> 시즌 패스 </summary>
	SeasonPass,
	/// <summary> 에너지 (스테이지 선택 화면에서 팝업으로 출력) </summary>
	Energy,
	/// <summary> 스페셜 패키지 </summary>
	Billiboard,
	/// <summary> 픽업 가챠 </summary>
	SpecialGacha,
	/// <summary> 금니 재화 </summary>
	Cash,
	/// <summary> 달러 </summary>
	Doller,
	/// <summary> 유료 화폐 개수 </summary>
	TicketPrice,
	/// <summary>  </summary>
	None,
	Max = None
}

public enum ShopGroupType
{
	/// <summary> 픽업 가챠 그룹 </summary>
	PickUpGacha,
	/// <summary> 노말 가챠 그룹 </summary>
	NormalGacha,
	/// <summary> 금니 그룹 </summary>
	Cash,
	/// <summary> 달러 그룹 </summary>
	Doller,
	/// <summary> 에너지 그룹 </summary>
	Energy,
	/// <summary> 월정액 그룹 </summary>
	Membership,
	/// <summary> 시즌패스 그룹 </summary>
	SeasonPass,
	/// <summary>  </summary>
	None
}

/// <summary> shoptable에서 패키지의 그룹을 m_Level에서 읽어 쓰는 중 </summary>
public enum PackageGroupType
{
	/// <summary> 챕터 클리어 패키지 </summary>
	ChapterClear,
	/// <summary> 한정판매 또는 특정 레벨이나 수치 달성했을때 오픈되는 아이템 추천 패키지 </summary>
	Recommend,
	/// <summary> 일간/주간으로 제공하는 아이템 패키지 </summary>
	Daily,
	/// <summary> 인사파일 관련 아이템 패키지 </summary>
	Survivors,
	/// <summary> 혈청/DNA관련 </summary>
	Growth,
	/// <summary> 부활권 아이템 패키지 </summary>
	Help,
	/// <summary> 금니/달러 충전 패키지 </summary>
	Charge,
	/// <summary> 이벤트 </summary>
	Event
}

public enum TagType
{
	None,
	SEASON,
	DOUBLE,
}
