
public enum ShopGroup
{
	None = 0,
	/// <summary> 시즌패스 </summary>
	Pass,
	/// <summary> 캐시 </summary>
	Cash,
	/// <summary> 달러 </summary>
	Money,
	/// <summary> 가차(box) </summary>
	Gacha,
	/// <summary> 장비 </summary>
	Equip,
	/// <summary> 보급 상자 </summary>
	SupplyBox,
	/// <summary> 블랙 마켓 </summary>
	BlackMarket,
	/// <summary> 스테이지 종료 뒤 출력되는 이벤트_암상인에 쓰일 목록 </summary>
	Event_BlackMarket,
	/// <summary> 장비 가차 </summary>
	ItemGacha,
	/// <summary> 패키지 상품 </summary>
	Package,
	/// <summary> 정액제 상품 </summary>
	DailyPack,
	/// <summary> pvp 상품 </summary>
	PVPShop,
	/// <summary> 뽑기 마일리지 상점 </summary>
	Mileage,
	/// <summary> 월정액 </summary>
	Monthly,
	/// <summary> 연합에 가입하지 않아도 노출 / DNA / DNATable index 참조 </summary>
	Guild_normal_DNA,
	/// <summary> 연합에 가입하지 않아도 노출 / 캐릭터 인사 파일 </summary>
	Guild_normal_Char,
	/// <summary> 연합 가입 시에만 노출 </summary>
	Guild_member,
	/// <summary> 연합 마스터에게만 공개하는 아이템 </summary>
	Guild_master,
	/// <summary>  </summary>
	End
}

public enum ShopUseType
{
	/// <summary> 일반 </summary>
	Normal = 0,
	/// <summary> 기간제 </summary>
	Time,
	/// <summary> 사용안함 </summary>
	None
}