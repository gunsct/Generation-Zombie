

public enum ZombieType
{
	/// <summary> 별다른 특징은 없음. </summary>
	Normal = 0,
	/// <summary> 대사에 게임 팁이 섞여있음. </summary>
	Tip,
	/// <summary> 주기적으로 확인 할 때 노트 전투 발생 - 승리 시 1~3단계 DNA 랜덤 획득 </summary>
	NoteBattle,
	/// <summary> 다른 사육장에 좀비가 2마리 미만으로 있을 경우 NoteBattle 좀비와 동일한 행동 양상을 보임 </summary>
	TwoUnder,
	/// <summary> 다른 사육장에 좀비가 2마리 이상 있을 경우 NoteBattle 좀비와 동일한 행동 양상을 보임 </summary>
	TwoMore,
	/// <summary> 기본 안정도의 +- 20% </summary>
	BaseSafeProb,
	/// <summary> 안정도의 +5~20%가 추가로 감소 </summary>
	AddSafeProb,
	/// <summary> 안정도의 -5~20%가 덜 감소 </summary>
	MinusSafeProb,
	/// <summary>  </summary>
	Max
}