public enum AchieveType
{
	/// <summary> 스테이지 : 일반 스테이지 클리어 </summary>
	Normal_Stage_Clear = 0,
	/// <summary> 스테이지 : 나이트메어 스테이지 클리어 </summary>
	Hard_Stage_Clear,
	/// <summary> 스테이지 : 아포칼립스 스테이지 클리어 </summary>
	Nightmare_Stage_Clear,
	/// <summary> 스테이지 : 다운타운 훈련장 클리어 </summary>
	Academy_Clear,
	/// <summary> 스테이지 : 하버드 대학교 클리어 </summary>
	University_Clear,
	/// <summary> 스테이지 : 은행 클리어 </summary>
	Bank_Clear,
	/// <summary> 스테이지 : 무덤 클리어 </summary>
	Cemetery_Clear,
	/// <summary> 스테이지 : 스타디움 클리어 </summary>
	Factory_Clear,
	/// <summary> 스테이지 : 타워 클리어 </summary>
	Tower_Clear,
	/// <summary> 성장 : 캐릭터 수 </summary>
	Character_Count,
	/// <summary> 성장 : 캐릭터 등급 달성 수 - 예 : 4등급 캐릭터 10개 달성 </summary>
	Character_Grade_Count,
	/// <summary> 성장 : 캐릭터 등급업 횟수 </summary>
	Character_GradeUp_Count,
	/// <summary> 성장 : 캐릭터 레벨 달성 수 - 예 : 40레벨 캐릭터 10개 달성 </summary>
	Character_Level_Count,
	/// <summary> 성장 : 캐릭터 레벨업 횟수 </summary>
	Character_LevelUp_Count,
	/// <summary> 성장 : 장비 레벨 달성 수 - 예 : 60레벨 장비 10개 수집 </summary>
	Equip_Level_Count,
	/// <summary> 성장 : 장비 등급 달성 수 - 예 : 5등급 장비 10개 수집 </summary>
	Equip_Grade_Count,
	/// <summary> 수집 : 금니 수집 - 획득한 모든 금니 체크 (소모해도 감소 없음) </summary>
	GoldTeeth_Count,
	/// <summary> 수집 : 금니 사용 - 사용 모든 금니 체크 </summary>
	GoldTeeth_Use,
	/// <summary> 수집 : 달러 수집 - 획득한 모든 달러 체크 (소모해도 감소 없음) </summary>
	Dollar_Count,
	/// <summary> 수집 : 달러 사용 - 사용한 모든 달러 체크 </summary>
	Dollar_Use,
	/// <summary> 수집 : 에너지(총알) 사용 </summary>
	Energy_Use,
	/// <summary> 수집 : 제작 수 </summary>
	Making_Count,
	/// <summary> 수집 : 무기 제작 수 </summary>
	Weapon_Making_Count,
	/// <summary> 수집 : 방어구 제작 수 </summary>
	Armor_Making_Count,
	/// <summary> 수집 : 기타 제작 수 </summary>
	Etc_Making_Count,
	/// <summary> 특정 등급 좀비 수집 : Value01에 작성된 Grade에 해당하는 좀비 수집 </summary>
	Zombie_Grade_Count,
	/// <summary> 특정 등급 DNA 수집 : Value01에 작성된 Grade에 해당하는 DNA 수집 </summary>
	DNA_Grade_Count,
	/// <summary> DNA 레벨업 횟수 : Value02만큼 DNA 레벨업 진행 </summary>
	DNA_LevelUp_Count,
	/// <summary> 스테이지 노말, 나이트메어, 아포칼립스 통합 실패 체크 </summary>
	Stage_Fail,
	/// <summary> 공동묘지, 공장, 대학, 사관학교, 은행, 지하철, 타워 & PVP 제외 PVE 콘텐츠 통합 실패 체크 </summary>
	DownTown_Fail,
	/// <summary>  </summary>
	END
}