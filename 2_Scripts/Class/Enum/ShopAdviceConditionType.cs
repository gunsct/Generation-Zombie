public enum ShopAdviceCondition
{
	None,
	Shop,
	PopUp
}
public enum ShopAdviceConditionType
{
	None,
	/// <summary> Value 값에 해당 하는 계정 레벨 도달 시 </summary>
	UserLevel,
	/// <summary> Value값에 해당 Index의 캐릭터가 미획득 상태일 경우 </summary>
	NotHaveChar,
	/// <summary> Value값에 해당 하는 Index의 캐릭터가 획득 상태일 경우 </summary>
	HaveChar,
	/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 Level이 Value값에 도달 했을 경우</summary>
	CharLevel,
	/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 등급이 Value값에 도달 했을 경우</summary>
	CharGrade,
	/// <summary> (앞선 획득한 캐릭터/등급과 함께 사용) 목표 캐릭터의 목표 등급에서 최고 레벨 달성 시</summary>
	MaxLevel,
	/// <summary> Value값에 해당하는 스테이지에 진입 </summary>
	NormalStage,
	HardStage,
	NightmareStage,
	/// <summary> Value값에 해당하는 스테이지 클리어 이후 </summary>
	StageClear,
	HardStageClear,
	NightmareStageClear,
	/// <summary> Value값에 해당하는 연속 실패 시 </summary>
	FailCount,
	/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 혈청이 Value값에 해당하는 페이지에 도달 시</summary>
	SerumBlock,
	/// <summary> 매일 첫 접속 시 </summary>
	FirstConnect,
	/// <summary>목표 ShopIndex구매 했을 경우 활성화 </summary>
	BuyItem,
	/// <summary> 목표 ShopIndex 구매수량이 한개 이상일때</summary>
	BuyOneMoreItem,
	/// <summary> 이벤트 </summary>
	Event
	/// <summary> </summary>

}

public enum ShopAdviceCloseType
{
	/// <summary>  </summary>
	None,
	/// <summary> Value 값에 해당 하는 계정 레벨 도달 시 </summary>
	UserLevel,
	/// <summary> Value값에 해당 Index의 캐릭터가 미획득 상태일 경우 </summary>
	NotHaveChar,
	/// <summary> Value값에 해당 하는 Index의 캐릭터가 획득 상태일 경우 </summary>
	HaveChar,
	/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 Level이 Value값에 도달 했을 경우 </summary>
	CharLevel,
	/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 등급이 Value값에 도달 했을 경우 </summary>
	CharGrade,
	/// <summary> (앞선 획득한 캐릭터/등급과 함께 사용) 목표 캐릭터의 목표 등급에서 최고 레벨 달성 시 </summary>
	MaxLevel,
	/// <summary> Value값에 해당하는 노말 스테이지에 진입 </summary>
	NormalStage,
	/// <summary> Value값에 해당하는 나이트메어 스테이지에 진입 </summary>
	HardStage,
	/// <summary> Value값에 해당하는 아포칼립스 스테이지에 진입 </summary>
	NightmareStage,
	/// <summary> Value값에 해당하는 스테이지 클리어 이후 </summary>
	StageClear,
	HardStageClear,
	NightmareStageClear,
	/// <summary> Value값에 해당하는 연속 실패 시 </summary>
	FailCount,
	/// <summary> (앞선 획득한 캐릭터와 함께 사용) 목표 캐릭터의 혈청이 Value값에 해당하는 페이지에 도달 시 </summary>
	SerumBlock,
	/// <summary> 오픈 후 시간이 진행 된 후 </summary>
	Time,
	/// <summary> 목표 ShopIndex 구매수량이 한개 이상일때</summary>
	BuyOneMoreItem,
	/// <summary> </summary>
}