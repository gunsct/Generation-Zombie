public enum UpTierType
{
	NONE,		//승급 불가
	LEAGUERANK, //특정 순위 이상 승급
	POINT		//특정 포인트 이상 승급
}
public enum DownTierType
{
	NONE,		//강등 불가
	LEAGUERANK, //특정 순위 이상 강등
}
public enum UserPos
{
	My = 0,
	Target,
	Max
}
public enum PVPDeckPos
{
	Atk,
	Def
}
public enum PVPPosType
{
	None,
	/// <summary> 전투원 </summary>
	Combat,
	/// <summary> 서포터 </summary>
	Supporter
}
public enum PVPDmgType
{
	Bad,
	Normal,
	Good,
	Heal,
	Miss
}
public enum PVPArmorType
{
	/// <summary> 없음. </summary>
	None,
	/// <summary> 경갑 방어구</summary>
	LightArmor,
	/// <summary> 가죽 방어구</summary>
	Leather,
	/// <summary>천 방어구 </summary>
	Fabric,
	Max
}
public enum PVPSkillType
{
	/// <summary>  </summary>
	None,
	/// <summary> 어태커 스킬 </summary>
	Combat,
	/// <summary> 진영 스탯 변경, 현재 가장 낮은 생존 수치 변경 (정신력, 청결도, 포만도) , val1 스탯, val2 무시 val3 값 </summary>
	LowStatus,
	/// <summary> 진영 스탯 변경, (HP, 정신력, 청결도, 포만도) , val1 스탯, val2 무시 val3 값 </summary>
	Status,
	/// <summary> 스테이터스 버프, (스피드, 명중률, 치명타, 치명타 데미지 등등) , val1 스탯, val2 대상수 val3 값, 퍼센트랑 절대값이랑 따로 있으니 계산은 구별해줘야함   </summary>
	StatusBuff,
	/// <summary> 진영 스탯 변경, val1,2 스탯 넘버, 3은 값 </summary>
	TwoStat,
	/// <summary> 행동 게이지 한 번에 회복 val2 대상(1 or 5), val3 값 </summary>
	Ap,

}
public enum PVPEquipAtkType
{
	/// <summary>  </summary>
	None,
	/// <summary> 관통 공격 타입 : 경갑 타입 방어구에 추가 피해 </summary>
	Through,
	/// <summary> 베기 공격 타입 : 천 타입 방어구에 추가 피해 </summary>
	Cut,
	/// <summary> 충격 공격 타입 : 가죽 타입 방어구에 추가 피해 </summary>
	Shock
}
public enum PVPAtkTargetType
{
	/// <summary> </summary>
	None,
	/// <summary> 랜덤 대상 지정 </summary>
	Random,
	/// <summary> "전투력이 가장 강한 적을 공격 or 전투력이 가장 강한 적을 중심으로 범위 공격" </summary>
	Stronger,
	/// <summary>모든 적을 공격 </summary>
	All,
	/// <summary>"현재 HP가 가장 낮은 적을 공격 or 현재 HP가 가장 낮은 적을 중심으로 범위 공격" </summary>
	LowHP,
	/// <summary> "공격하려는 아군과 가장 가까운 위치에 있는 적 공격 or 공격하려는 아군과 가장 가까운 위치에 있는 적을 중심으로 범위 공격"</summary>
	Near,
	/// <summary> 랜덤으로 자리 고르고 그 좌우 </summary>
	RandomNear
}
public enum PVPTurnType
{
	/// <summary> </summary>
	None,
	/// <summary> "아군 타겟" </summary>
	Team,
	/// <summary> "적 타겟" </summary>	
	Enemy
}
public enum PVPRewardRankType
{
	RANK,
	RATIO
}
public enum PVPFailCause
{
	Men,
	Sat,
	Hyg,
	HP,
	Turn
}