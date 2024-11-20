
public enum StageClearType
{
	/// <summary> 목표 카드를 선택 했을 경우 (유틸리티나 스킬에 의한 선택은 제외)
	/// <para> Value : 목표 Card ID - 해당 스테이지의 StageCardTable의 Index 참조 </para>
	/// <para> cnt : 선택 요구 개수 </para>
	/// </summary>
	CardUse,
	/// <summary> 특정 몬스터 죽이기 (0 아무몬스터)
	/// <para> Value : 몬스터 인덱스 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy,
	/// <summary> 특정 타입 몬스터 죽이기(0 아무몬스터)
	/// <para> Value : 몬스터 타입번호 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy_Type,
	/// <summary> 특전 종족의 몬스터 죽이기 (0 마무 몬스터)
	/// <para> Value : 몬스터 종족 번호 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy_Tribe,
	/// <summary> 특정 등급 내 몬스터 죽이기 (0 아무 그룹 몬스터)
	/// <para> Value : 몬스터 등급 번호 </para>
	/// <para> cnt : 목표 횟수 </para>
	/// </summary>
	KillEnemy_Grade,
	/// <summary> 생존 스테이터스를 일정 수치 이상 회복
	/// <para> Value : 스텟 번호 </para>
	/// <para> cnt : 목표 수치 </para>
	/// </summary>
	Rec_Stat,
	/// <summary> 목표 시간만큼 생존
	/// <para> cnt : 목표 시간(단위 : 시) </para>
	/// </summary>
	Survival,
	/// <summary> 피난민 목표 횟수만큼 구출
	/// <para> Value01 : 목표 수치 </para>
	/// </summary>
	Rescue,
	Rescue_Refugee,
	Rescue_Infectee,
	/// <summary> 아무 스킬 사용 시 클리어
	/// <para> cnt : 생성 개수 </para>
	/// </summary>
	UseSkill,
	/// <summary> 필드의 ‘화염’ 카드를 N회 이상 소화시킬 경우 클리어.</summary>
	SuppressionF,
	/// <summary> 필드의 ‘보급 상자’를 N회 이상 습득하였을 경우 클리어.</summary>
	GetBox,
	/// <summary> 트레이닝 </summary>
	Training,
	/// <summary> 필드에서 N회 이상 '제작'할 경우 클리어. </summary>
	AnyMaking,
	/// <summary> Value01 : 디폴트 카드 테이블의 타입 번호 0은 전부, Value02 개수 </summary>
	Fire_Card,
	/// <summary> Value01 : 에너미의 타입 번호 0은 전부, Value02 개수 </summary>
	Fire_Enemy,
	/// <summary> Value 01 : 특정 감염된 피난민 인덱스 (0일 경우 모든 감염된 피난민), count01 : 목표 횟수 </summary>
	Kill_infectee,
	/// <summary> 한 턴 내에 n마리 이상의 적을 처치해야지만 성공 카운트가 1 올라가는 목표, count01 : 목표 횟수 </summary>
	KillEnemy_Turn,
	/// <summary> 없음 </summary>
	None

}

public enum StageFailType
{
	/// <summary> 배틀 횟수가 목표 횟수에 도달했을 경우 패배 </summary>
	TurmoilCount = 0,
	/// <summary> 목표 시간이 지남 </summary>
	Time,
	/// <summary> 없음 </summary>
	None
}