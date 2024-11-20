using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public enum SND_IDX
{
	NONE = 0,
	// 배경
	/// <summary> 타이틀 </summary>
	BGM_0002 = 2,
	/// <summary> 메인 </summary>
	BGM_0011 = 11,
	BGM_0012 = 12,
	BGM_0013 = 13,
	BGM_0014 = 14,
	BGM_0015 = 15,
	/// <summary> 연합 </summary>
	BGM_0020 = 20,
	/// <summary> 연합 상점 </summary>
	BGM_0021 = 21,
	/// <summary> 메인화면_공중폭격_siren </summary>
	BGM_0050 = 50,
	/// <summary> 메인화면_화재 </summary>
	BGM_0051 = 51,
	/// <summary> 메인화면_어둠 </summary>
	BGM_0052 = 52,
	/// <summary> 프로필 설정 </summary>
	BGM_0053 = 53,
	/// <summary> 스테이지 </summary>
	BGM_0100 = 100,
	/// <summary> 스테이지 </summary>
	BGM_0101 = 101,
	/// <summary> 스테이지 </summary>
	BGM_0102 = 102,
	/// <summary> 스테이지 </summary>
	BGM_0103 = 103,
	/// <summary> 대전(일반, 정예) </summary>
	BGM_0500 = 500,
	/// <summary> 대전(보스) </summary>
	BGM_0502 = 502,
	/// <summary> 트레이닝 </summary>
	BGM_0650 = 650,
	BGM_0651 = 651,
	/// <summary> PVP 로비 </summary>
	BGM_0660 = 660,
	/// <summary> PVP 인게임 </summary>
	BGM_0661 = 661,
	/// <summary> 긴급구조 배경음 </summary>
	BGM_0700 = 700,
	/// <summary> 할로윈 이벤트 스테이지 </summary>
	BGM_0300 = 300,
	/// <summary> 할로윈 이벤트 팝업 </summary>
	BGM_1000 = 1000,
	/// <summary> 캐릭터 뽑기 팝업, 자동차 도착부터 재생 </summary>
	BGM_1001 = 1001,
	BGM_9999 = 9999,

	// 이펙트
	/// <summary> 터치 기본 </summary>
	SFX_0001 = 100001,
	/// <summary> 터치 인터렉션 </summary>
	SFX_0002 = 100002,
	/// <summary> 획득 </summary>
	SFX_0003 = 100003,
	/// <summary> 장비 장착, 가방 확장 </summary>
	SFX_0004 = 100004,
	/// <summary> 장비 해제 </summary>
	SFX_0005 = 100005,
	/// <summary> 화면전환 </summary>
	SFX_0007 = 100007,
	/// <summary> pvp 덱에 캐릭터 포함/제외시킬 경우 </summary>
	SFX_0010 = 100010,
	/// <summary> pvp 덱 공격/방어팀 스왑 </summary>
	SFX_0011 = 100011,
	/// <summary> 경매장 새로고침, 일일임무 새로고침, 장비 강화 연출 </summary>
	SFX_0012 = 100012,
	/// <summary> 신규 제작 팝업 </summary>
	SFX_0100 = 100100,
	/// <summary> 챌린지 팝업 </summary>
	SFX_0101 = 100101,
	/// <summary> 챌린지 팝업 </summary>
	SFX_0102 = 100102,
	/// <summary> 스테이지 목표 팝업 </summary>
	SFX_0103 = 100103,
	/// <summary> 생산 시작, 연구 시작 버튼 인터렉션 </summary>
	SFX_0104 = 100104,
	/// <summary> 연구 완료 연출 </summary>
	SFX_0105 = 100105,
	/// <summary> 상단 달러 올라가는 효과음 </summary>
	SFX_0108 = 100108,
	/// <summary> 인벤토리 처분 효과음 </summary>
	SFX_0109 = 100109,
	/// <summary> 달러로 구매시 </summary>
	SFX_0110 = 100110,
	/// <summary> 초시계, 미션, 상점 등급 상승 </summary>
	SFX_0111 = 100111,
	/// <summary> 생존자 승급창 진입/후퇴(파일열리고 접힘), 동료정보 팝업 </summary>
	SFX_0112 = 100112,
	/// <summary> 생존자 승급 연출 </summary>
	SFX_0113 = 100113,
	/// <summary> 개별/전체 동료에게 선물보내기 </summary>
	SFX_0115 = 100115,
	/// <summary> 동료신청 버튼 </summary>
	SFX_0116 = 100116,
	/// <summary> 동료요청 받기 버튼 </summary>
	SFX_0117 = 100117,
	/// <summary> 동료삭제 경고팝업 </summary>
	SFX_0118 = 100118,
	/// <summary> PDA 진입시 </summary>
	SFX_0120 = 100120,
	/// <summary> PDA 터치 </summary>
	SFX_0121 = 100121,
	/// <summary> 제작, 연구, 파견 시작 </summary>
	SFX_0122 = 100122,
	/// <summary> 제작, 연구, 파견 완료 </summary>
	SFX_0123 = 100123,
	/// <summary> 상점 진입 </summary>
	SFX_0130 = 100130,
	/// <summary> 상점 패키지 진입 </summary>
	SFX_0131 = 100131,
	/// <summary> VIP 진입 </summary>
	SFX_0140 = 100140,
	/// <summary> VIP 레벨업  </summary>
	SFX_0141 = 100141,
	/// <summary> VIP 보상해금 </summary>
	SFX_0142 = 100142,
	/// <summary> VIP 보상획득 </summary>
	SFX_0143 = 100143,
	/// <summary> 유저 활동 알림음 1 </summary>
	SFX_0160 = 100160,
	/// <summary> 유저 활동 알림음 2 </summary>
	SFX_0161 = 100161,
	/// <summary> 출석체크 체크 </summary>
	SFX_0170 = 100170,
	/// <summary> 출석체크 보상팝업 </summary>
	SFX_0171 = 100171,
	/// <summary> 사이렌 </summary>
	SFX_0180 = 100180,
	/// <summary> 다운타운 버튼 - 공장 </summary>
	SFX_0181 = 100181,
	/// <summary> 다운타운 버튼 - 지하철 </summary>
	SFX_0182 = 100182,
	/// <summary> 다운타운 버튼 - 대학 </summary>
	SFX_0183 = 100183,
	/// <summary> 다운타운 버튼 - 공동묘지 </summary>
	SFX_0184 = 100184,
	/// <summary> 다운타운 버튼 - 사관학교 </summary>
	SFX_0185 = 100185,
	/// <summary> 다운타운 버튼 - 은행 </summary>
	SFX_0186 = 100186,
	/// <summary> 다운타운 버튼 - 타워 </summary>
	SFX_0187 = 100187,
	/// <summary> 컨텐츠 잠금해제 연출 </summary>
	SFX_0190 = 100190,
	/// <summary> 챌린지 종료 팝업 </summary>
	SFX_0191 = 100191,
	/// <summary> 챌린지 다음 </summary>
	SFX_0192 = 100192,
	/// <summary> 타워 랜덤 이벤트 카드 돌아가는 소리 </summary>
	SFX_0193 = 100193,
	/// <summary> 긴급임무 진입 </summary>
	SFX_0195 = 100195,
	/// <summary> 하드/나메 시작 디버프 카드 </summary>
	SFX_0196 = 100196,
	/// <summary> 카드 이동, 우편함 진입 </summary>
	SFX_0200 = 100200,
	/// <summary> 카드 제거, 동료삭제 완료 </summary>
	SFX_0205 = 100205,
	/// <summary> 카드 획득 </summary>
	SFX_0206 = 100206,
	/// <summary> 발자국 소리 </summary>
	SFX_0208 = 100208,
	/// <summary> 미션 가이드 달성도 상승시 </summary>
	SFX_0209 = 100209,
	/// <summary> 전투 보상 팝업 뜰 때 </summary>
	SFX_0210 = 100210,
	/// <summary> 제작 1단계 </summary>
	SFX_0211 = 100211,
	/// <summary> 제작 2단계 </summary>
	SFX_0212 = 100212,
	/// <summary> 제작 3단계 </summary>
	SFX_0213 = 100213,
	/// <summary> 하드/나메 시작 디버프 카드 </summary>
	SFX_0215 = 100215,
	SFX_0216 = 100216,
	/// <summary> 적 카드 사망 </summary>
	SFX_0220 = 100220,
	/// <summary> 카드 가루될떄 </summary>
	SFX_0221 = 100221,
	/// <summary> 성공 </summary>
	SFX_0298 = 100298,
	/// <summary> 실패 </summary>
	SFX_0299 = 100299,
	/// <summary> 스테이지 선택 장전 </summary>
	SFX_0300 = 100300,
	/// <summary> 스테이지 진입 격발 </summary>
	SFX_0301 = 100301,
	/// <summary> 챕터보상 팝업 시작, 소탕권 사용 </summary>
	SFX_0310 = 100310,
	/// <summary> 스테이지 클리어 보상 박힐때, 소탕권 아이템 마다 </summary>
	SFX_0313 = 100313,
	/// <summary> 스테이지 처치 보상 팝업 </summary>
	SFX_0320 = 100320,
	/// <summary> 스테이지 처치 보상 팝업 좋은것 획득 </summary>
	SFX_0321 = 100321,
	/// <summary> 챕터 완료 보상 팝업 시작</summary>
	SFX_0330 = 100330,
	/// <summary> 챕터 완료 보상 팝업 중간 연출 + 보상공개</summary>
	SFX_0331 = 100331,
	/// <summary> 다이얼로그 선택지 등장 </summary>
	SFX_0340 = 100340,
	/// <summary> 다이얼로그 선택지 선택</summary>
	SFX_0341 = 100341,
	/// <summary> 다이얼로그 입장 직후 1회</summary>
	SFX_0344 = 100344,
	/// <summary> 다이얼로그 각각의 말풍선이 나올때</summary>
	SFX_0345 = 100345,
	/// <summary> 긴급구조 시작 </summary>
	SFX_0350 = 100350,
	/// <summary> 긴급구조 신호탄 시작 </summary>
	SFX_0351 = 100351,
	/// <summary> 긴급구조 신호탄 지속 </summary>
	SFX_0352 = 100352,
	/// <summary> 타이머 초침 </summary>
	SFX_0353 = 100353,
	/// <summary> 사격 </summary>
	SFX_0400 = 100400,
	/// <summary> 사격(pistol) </summary>
	SFX_0401 = 100401,
	/// <summary> 도박 진행 </summary>
	SFX_0402 = 100402,
	/// <summary> 도박 완료 </summary>
	SFX_0403 = 100403,
	/// <summary> 전파방해 스킬 </summary>
	SFX_0404 = 100404,
	/// <summary> 폭발 </summary>
	SFX_0410 = 100410,
	/// <summary> 조명탄 </summary>
	//SFX_0411 = 100411,
	/// <summary> 형광 스틱 </summary>
	//SFX_0412 = 100412,
	/// <summary> 조명탄 </summary>
	SFX_0413 = 100413,
	/// <summary> 베기 </summary>
	SFX_0420 = 100420,
	/// <summary> 에너미 서로 공격할때 </summary>
	SFX_0421 = 100421,
	SFX_0422 = 100422,
	/// <summary> 찌르기 </summary>
	SFX_0430 = 100430,
	/// <summary> 활 </summary>
	SFX_0431 = 100431,
	/// <summary> 치기 </summary>
	SFX_0440 = 100440,
	/// <summary> 치기(blunt) </summary>
	SFX_0441 = 100441,
	/// <summary> 찍기(axe) </summary>
	SFX_0442 = 100442,
	/// <summary> HP 회복 </summary>
	SFX_0450 = 100450,
	/// <summary> 정신력 회복 </summary>
	SFX_0451 = 100451,
	/// <summary> 포만감 회복 </summary>
	SFX_0452 = 100452,
	/// <summary> 청결도 회복 </summary>
	SFX_0453 = 100453,
	/// <summary> 행동력 회복 </summary>
	SFX_0454 = 100454,
	/// <summary> 버프 </summary>
	SFX_0460 = 100460,
	/// <summary>  </summary>
	SFX_0461 = 100461,
	/// <summary> 제한시간, 제한턴, 재굴림 횟수 버프 </summary>
	SFX_0462 = 100462,
	/// <summary> 생존스탯 감소 </summary>
	SFX_0470 = 100470,
	/// <summary> 디버프 </summary>
	SFX_0471 = 100471,
	/// <summary> hp, 행동력 감소 디버프 </summary>
	SFX_0472 = 100472,
	/// <summary>  레오나르도1010, 섬광탄, 타겟지정시(푸이육) </summary>
	SFX_0480 = 100480,
	/// <summary> 체력 일정 이하 경고 </summary>
	SFX_0491 = 100491,
	/// <summary> 포만감 일정 이하 경고 </summary>
	SFX_0492 = 100492,
	/// <summary> 위생 일정 이하 경고 </summary>
	SFX_0493 = 100493,
	/// <summary> 정신력 일정 이하 경고 </summary>
	SFX_0494 = 100494,
	/// <summary> 타이머 10초 이하 경고 </summary>
	SFX_0495 = 100495,
	/// <summary> 스테이지에서 캐릭터 스킬 사용가능상태 알림 </summary>
	SFX_0496 = 100496,
	/// <summary> 샷건 </summary>
	SFX_0500 = 100500,
	/// <summary> 공중 지원 </summary>
	SFX_0507 = 100507,
	/// <summary> RangeAtk </summary>
	SFX_0549 = 100549,
	/// <summary> 머신건 </summary>
	SFX_0600 = 100600,
	/// <summary> 손전등, 형광스틱 </summary>
	SFX_0603 = 100603,
	/// <summary> 불번짐 </summary>
	SFX_0606 = 100606,
	/// <summary> 소화기 1단계 </summary>
	SFX_0607 = 100607,
	/// <summary> 소화기 2단계 </summary>
	SFX_0608 = 100608,
	/// <summary> 소화기 3단계 </summary>
	SFX_0609 = 100609,
	/// <summary> SmokeBomb </summary>
	SFX_0610 = 100610,
	/// <summary> 소방관 시너지 사운드 </summary>
	SFX_0611 = 100611,
	/// <summary> Paralysis </summary>
	SFX_0613 = 100613,
	/// <summary> 화염병 </summary>
	SFX_0617 = 100617,
	/// <summary> 화염방사기 </summary>
	SFX_0618 = 100618,
	/// <summary> 네이팜</summary>
	SFX_0619 = 100619,
	/// <summary> 드릴 </summary>
	SFX_0620 = 100620,
	/// <summary> 화염 번질때</summary>
	SFX_0621 = 100621,
	/// <summary> 카드 선택 불가 사슬 </summary>
	SFX_0622 = 100622,
	/// <summary> 카드 선택 불가 사슬 터치 </summary>
	SFX_0623 = 100623,
	/// <summary> Bite_Hit </summary>
	SFX_0701 = 100701,
	/// <summary> Zombie Bite_Hit </summary>
	SFX_0702 = 100702,
	/// <summary> Catch_Hit </summary>
	SFX_0711 = 100711,
	/// <summary> Chain_Attack_Hit </summary>
	SFX_0721 = 100721,
	/// <summary> Split </summary>
	SFX_0732 = 100732,
	/// <summary> 동물이 음식먹을때 </summary>
	SFX_0799 = 100799,
	/// <summary> 슬래시 노트 </summary>
	SFX_0900 = 100900,
	/// <summary> AllNote_Good </summary>
	SFX_0902 = 100902,
	/// <summary> Normal_Good </summary>
	SFX_0910 = 100910,
	/// <summary> Normal_Perfect </summary>
	SFX_0911 = 100911,
	/// <summary> 콤보 칠때마다 </summary>
	SFX_0920 = 100920,
	/// <summary> 콤보 끝까지 치면(퍼펙트) </summary>
	SFX_0921 = 100921,
	/// <summary> 차지 굿 </summary>
	SFX_0930 = 100930,
	/// <summary> 차지 퍼펙트 </summary>
	SFX_0931 = 100931,
	/// <summary> 차지 게이지 찰때 </summary>
	SFX_0935 = 100935,
	/// <summary> 체인 굿 </summary>
	SFX_0940 = 100940,
	/// <summary> 체인 퍼펙트 </summary>
	SFX_0941 = 100941,
	/// <summary> 잡힌직후 사슬등장 </summary>
	SFX_0950 = 100950,
	/// <summary> 사슬터치 </summary>
	SFX_0951 = 100951,
	/// <summary> 사슬파괴 </summary>
	SFX_0952 = 100952,
	/// <summary> 노트전투 타임 오버 미스 </summary>
	SFX_0990 = 100990,
	/// <summary> 상점 구매 완료, 금니 소모 효과음 </summary>
	SFX_1010 = 101010,
	/// <summary> 장비뽑기 연출 시작 </summary>
	SFX_1011 = 101011,
	/// <summary> pvp 상점 구매 </summary>
	SFX_1014 = 101014,
	/// <summary> 장비 투입 사운드 추가 </summary>
	SFX_1055 = 101055,
	/// <summary> 장비 강화 UI 진입 </summary>
	SFX_1069 = 101069,
	/// <summary> 재조립 진행 </summary>
	SFX_1075 = 101075,
	/// <summary> 좀비 저장 </summary>
	SFX_1100 = 101100,
	/// <summary> 좀비 파기 </summary>
	SFX_1101 = 101101,
	/// <summary> DNA 주입 </summary>
	SFX_1102 = 101102,
	/// <summary> DNA 주입 결과 분석 </summary>
	SFX_1103 = 101103,
	/// <summary> 좀비 분해 </summary>
	SFX_1106 = 101106,
	/// <summary> DNA합성 진입 </summary>
	SFX_1110 = 101110,
	/// <summary> DNA합성 선택 </summary>
	SFX_1111 = 101111,
	/// <summary> DNA합성 취소 </summary>
	SFX_1112 = 101112,
	/// <summary> DNA합성 진행 </summary>
	SFX_1113 = 101113,
	/// <summary> DNA합성 성공, 옵션 변경 </summary>
	SFX_1114 = 101114,
	/// <summary> DNA합성 실패 </summary>
	SFX_1115 = 101115,
	/// <summary> DNA옵션 추가 </summary>
	SFX_1116 = 101116,
	/// <summary> 사육장 좀비 배치 </summary>
	SFX_1120 = 101120,
	/// <summary> pvp 덱 저장 </summary>
	SFX_1400 = 101400,
	/// <summary> 리그, 시즌 종료 </summary>
	SFX_1411 = 101411,
	/// <summary> 티어 보상 목록 좌우 스크롤 </summary>
	SFX_1412 = 101412,
	/// <summary> 시즌 순위 보상 팝업 </summary>
	SFX_1413 = 101413,
	/// <summary> pvp 상대 새로고침 </summary>
	SFX_1414 = 101414,
	/// <summary> 습격 </summary>
	SFX_1500 = 101500,
	/// <summary> 연합 가입 성공 </summary>
	SFX_1501 = 101501,
	/// <summary> 연합 창설 성공 </summary>
	SFX_1502 = 101502,
	/// <summary> 연합 탈퇴/해산 </summary>
	SFX_1503 = 101503,
	/// <summary> Zombie_Spawn </summary>
	SFX_1510 = 101510,
	/// <summary> Zombie_Attack </summary>
	SFX_1511 = 101511,
	/// <summary> Zombie_Hit </summary>
	SFX_1512 = 101512,
	/// <summary> 메인화면 문 </summary>
	SFX_0150 = 100150,
	SFX_0151 = 100151,
	SFX_0152 = 100152,
	SFX_1513 = 101513,
	SFX_1514 = 101514,
	SFX_1516 = 101516,
	SFX_1517 = 101517,
	SFX_1518 = 101518,
	/// <summary> 연합 연구 시작 </summary>
	SFX_1520 = 101520,
	/// <summary> 연합 연구 완료 </summary>
	SFX_1522 = 101522,
	/// <summary> 연합 연구 기여 완료 </summary>
	SFX_1523 = 101523,
	/// <summary> 메인화면 형광등 </summary>
	SFX_0153 = 100153,
	SFX_0154 = 100154,
	SFX_0155 = 100155,
	/// <summary> 피난민 사망 </summary>
	SFX_1601 = 101601,
	/// <summary> 장비강화 화면 진입 </summary>
	SFX_1609 = 101609,
	/// <summary> 캐릭터 뽑기 연출 효과음</summary>
	SFX_1000 = 101000,
	SFX_1001 = 101001,
	SFX_1002 = 101002,
	SFX_1003 = 101003,
	SFX_1004 = 101004,
	SFX_1005 = 101005,
	/// <summary> 캐릭터 뽑기 5성, 5성미만 </summary>
	SFX_1006 = 101006,
	SFX_1007 = 101007,
	/// <summary> 장비뽑기 최상위 </summary>
	SFX_1012 = 101012,
	/// <summary> 장비뽑기 최상위 제외 </summary>
	SFX_1013 = 101013,
	/// <summary> 상점 패키지 팝업 </summary>
	SFX_1019 = 101019,
	/// <summary> DNA-Equip 전환 </summary>
	SFX_1070 = 101070,
	/// <summary> DNA 장착 </summary>
	SFX_1071 = 101071,
	/// <summary> 장비 개조 진행 </summary>
	SFX_1050 = 101050,
	/// <summary> 장비 개조 완료 </summary>
	SFX_1051 = 101051,
	/// <summary> 장비 레벨업 게이지 상승, 상점 등급 게이지 상승 </summary>
	SFX_1060 = 101060,
	/// <summary> 장비 레벨업 완료 </summary>
	SFX_1061 = 101061,
	/// <summary> 혈청 진입 </summary>
	SFX_1080 = 101080,
	/// <summary> 혈청 배경음 </summary>
	SFX_1081 = 101081,
	/// <summary> 혈청 주입 성공 </summary>
	SFX_1082 = 101082,
	/// <summary> 보급상자 연출 </summary>
	SFX_1020 = 101020,
	/// <summary> 트레이닝 중간 실패 </summary>
	SFX_1200 = 101200,
	/// <summary> 트레이닝 중간 성공 </summary>
	SFX_1201 = 101201,
	/// <summary> 트레이닝 달리기 성공 </summary>
	SFX_1210 = 101210,
	/// <summary> 트레이닝 계단오르기 성공 </summary>
	SFX_1211 = 101211,
	/// <summary> 트레이닝 턱걸이 성공 </summary>
	SFX_1212 = 101212,
	/// <summary> 트레이닝 배팅 성공 </summary>
	SFX_1213 = 101213,
	/// <summary> 다운타운->pvp </summary>
	SFX_1300 = 101300,
	/// <summary> pvp 시작버튼 </summary>
	SFX_1301 = 101301,
	/// <summary> pvp 인게임 입장 직후 환호성 </summary>
	SFX_1302 = 101302,
	/// <summary> pvp 인게임 레디 파이트 </summary>
	SFX_1303 = 101303,
	/// <summary> pvp 적 죽였을때 </summary>
	SFX_1310 = 101310,
	SFX_1311 = 101311,
	SFX_1312 = 101312,
	/// <summary> pvp 아군 죽었을때 </summary>
	SFX_1320 = 101320,
	SFX_1321 = 101321,
	SFX_1322 = 101322,
	/// <summary> pvp 적 서폿 도망갔을 때 </summary>
	SFX_1330 = 101330,
	SFX_1331 = 101331,
	SFX_1332 = 101332,
	/// <summary> pvp 내 생존 스탯 감소시 </summary>
	SFX_1340 = 101340,
	/// <summary> pvp 서포터 도망 안갔을때 </summary>
	SFX_1345 = 101345,
	/// <summary> pvp 아군이 마지막 적 죽였을때(적 hp 0) </summary>
	SFX_1350 = 101350,
	/// <summary> pvp 승리 </summary>
	SFX_1360 = 101360,
	/// <summary> pvp 패배 </summary>
	SFX_1361 = 101361,
	/// <summary> 경매장 진입 </summary>
	SFX_1800 = 101800,
	/// <summary> 경매장 즉시 && 직접 입찰 </summary>
	SFX_1801 = 101801,
	/// <summary> 인벤토리 진입 </summary>
	SFX_1900 = 101900,
	/// <summary> 친구 진입, 우편함 진입, 경매장 좌우 물품 이동버튼 </summary>
	SFX_1901 = 101901,
	/// <summary> 다운타운->보일사관학교-> 운동 종류 인터렉션 -> 단계리스트 뜰 때 </summary>
	SFX_1902 = 101902,
	/// <summary> 돌발 이벤트 인간 등장 </summary>
	SFX_1910 = 101910,
	//트레이닝 사운드
	/// <summary> 달리기 성공 </summary>
	SFX_2000 = 102000,
	/// <summary> 달리기 실패 </summary>
	SFX_2001 = 102001,
	/// <summary> 계단 성공 </summary>
	SFX_2010 = 102010,
	/// <summary> 게단 실패 </summary>
	SFX_2011 = 102011,
	/// <summary> 털걸이 성공 </summary>
	SFX_2020 = 102020,
	/// <summary> 털걸이 실패 </summary>
	SFX_2021 = 102021,
	/// <summary> 배팅 성공 </summary>
	SFX_2030 = 102030,
	/// <summary> 배팅 실패 </summary>
	SFX_2031 = 102031,

	/// <summary> 할로윈 호박 등장 </summary>
	SFX_3000 = 103000,
	/// <summary> 할로윈 호박 뚜껑 </summary>
	SFX_3001 = 103001,
	/// <summary> 할로윈 호박 클릭, 단계 선택 </summary>
	SFX_3002 = 103002,
	/// <summary> 할로윈 스테이지 입장 </summary>
	SFX_3003 = 103003,
	/// <summary> 할로윈 번개 </summary>
	SFX_3010 = 103010,
	SFX_3011 = 103011,
	SFX_3012 = 103012,
	/// <summary> 할로윈 임무 완료, 과자주기 </summary>
	SFX_3030 = 103030,
	/// <summary> 할로윈 과자 보상 </summary>
	SFX_3031 = 103031,
	/// <summary> 할로윈 스테이지 목표 카운팅 </summary>
	SFX_3060 = 103060,
	/// <summary> 할로윈 메뉴이동, 가이드, 추천 생존자, 보상목록 </summary>
	SFX_3050 = 103050,

	/// <summary> 다이얼로그 효과음 </summary>
	SFX_9000 = 109000,
	SFX_9001 = 109001,
	SFX_9002 = 109002,
	SFX_9003 = 109003,
	SFX_9004 = 109004,
	SFX_9005 = 109005,
	SFX_9006 = 109006,
	SFX_9007 = 109007, 
	SFX_9008 = 109008,
	SFX_9009 = 109009,
	SFX_9010 = 109010,
	SFX_9011 = 109011,
	SFX_9012 = 109012,
	SFX_9013 = 109013,
	SFX_9014 = 109014,
	SFX_9015 = 109015,
	SFX_9016 = 109016,
	SFX_9017 = 109017,
	SFX_9018 = 109018,
	SFX_9019 = 109019,
	SFX_9020 = 109020,
	SFX_9021 = 109021,
	SFX_9022 = 109022,
	SFX_9023 = 109023,
	/// <summary> 유리 깨짐</summary>
	SFX_9401 = 109401,
	/// <summary> 특별 선택지 지속 </summary>
	SFX_9403 = 109403,
	/// <summary> 특별 선택지 선택 </summary>
	SFX_9404 = 109404,
	/// <summary> 튜토리얼 사운드 </summary>
	SFX_9500 = 109500,//시작
	SFX_9510 = 109510,//말풍선
	SFX_9530 = 109530,//포커스
	SFX_9531 = 109531,//손가락
	/// <summary> 프로필 시작 </summary>
	SFX_9600 = 109600,
	/// <summary> 프로필 초상화 선택 완료 </summary>
	SFX_9601 = 109601,
	/// <summary> 프로필 도장 애니 </summary>
	SFX_9602 = 109602,
	/// <summary> 프로필 완료 도장 </summary>
	SFX_9603 = 109603,
	/// <summary>메인화면_공중폭격_비행기 </summary>
	SFX_9704 = 109704,
	/// <summary>메인화면_공중폭격_진동1 </summary>
	SFX_9705 = 109705,
	/// <summary>메인화면_공중폭격_진동2 </summary>
	SFX_9706 = 109706,
	/// <summary>메인화면_공중폭격_진동3 </summary>
	SFX_9707 = 109707,

	///보이스 
	/// <summary> 스테이지 사운드 </summary>
	/// 좀비 관련
	VOC_0101 = 200101,
	VOC_0102 = 200102,
	VOC_0103 = 200103,
	VOC_0104 = 200104,
	VOC_0105 = 200105,
	VOC_0106 = 200106,
	VOC_0111 = 200111,
	VOC_0112 = 200112,
	VOC_0113 = 200113,
	VOC_0114 = 200114,
	/// 피난민 관련
	VOC_0121 = 200121,
	VOC_0122 = 200122,
	VOC_0123 = 200123,
	/// 광신도 관련
	VOC_0131 = 200131,
	VOC_0132 = 200132,
	VOC_0133 = 200133,
	/// <summary> 노트 전투 </summary>
	VOC_1500 = 201500,//습격
	VOC_1510 = 201510,//시작
	// 좀비 
	VOC_1551 = 201551,//남
	VOC_1552 = 201552,
	VOC_1553 = 201553,
	VOC_1554 = 201554,//고통
	VOC_1555 = 201555,
	VOC_1556 = 201556,
	VOC_1557 = 201557,
	VOC_1558 = 201558,
	VOC_1559 = 201559,
	VOC_1561 = 201561,//여
	VOC_1562 = 201562,
	VOC_1563 = 201563,
	VOC_1564 = 201564,//고통
	VOC_1565 = 201565,
	VOC_1566 = 201566,
	VOC_1567 = 201567,
	VOC_1568 = 201568,
	VOC_1569 = 201569,
	VOC_1581 = 201581,
	VOC_1582 = 201582,
	VOC_1583 = 201583,
	VOC_1584 = 201584,
	VOC_1585 = 201585,
	VOC_1586 = 201586,
	VOC_1587 = 201587,
	VOC_1588 = 201588,
	VOC_1589 = 201589,
	// 인간
	VOC_1601 = 201601,//남
	VOC_1602 = 201602,
	VOC_1603 = 201603,
	VOC_1611 = 201611,//여
	VOC_1612 = 201612,
	VOC_1613 = 201613, 
	VOC_1651 = 201651,//고통 남
	VOC_1652 = 201652,
	VOC_1653 = 201653,
	VOC_1654 = 201654,
	VOC_1655 = 201655,
	VOC_1656 = 201656,
	VOC_1657 = 201657,
	VOC_1658 = 201658,
	VOC_1659 = 201659,
	VOC_1661 = 201661,//고통 여
	VOC_1662 = 201662,
	VOC_1663 = 201663,
	VOC_1664 = 201664,
	VOC_1665 = 201665,
	VOC_1666 = 201666,
	VOC_1667 = 201667,
	VOC_1668 = 201668,
	VOC_1669 = 201669,
	VOC_1681 = 201681,
	VOC_1682 = 201682,
	VOC_1683 = 201683,
	VOC_1684 = 201684,
	VOC_1685 = 201685,
	VOC_1686 = 201686,
	VOC_4328 = 204328,
	VOC_5218 = 205218,
	//동물
	VOC_1700 = 201700,
	VOC_1701 = 201701,
	VOC_1702 = 201702,
	VOC_1703 = 201703,
	VOC_1704 = 201704,
	VOC_1705 = 201705,
	VOC_1706 = 201706,
	VOC_1707 = 201707,
	VOC_1708 = 201708,
	VOC_1709 = 201709,
	VOC_1710 = 201710,
	VOC_1711 = 201711,
	VOC_1712 = 201712,
	VOC_1720 = 201720,
	VOC_1721 = 201721,
	VOC_1722 = 201722,
	VOC_1730 = 201730,
	VOC_1731 = 201731,
	VOC_1732 = 201732,
	VOC_1740 = 201740,
	VOC_1741 = 201741,
	VOC_1742 = 201742,
	VOC_1750 = 201750,
	VOC_1751 = 201751,
	VOC_1752 = 201752,
	VOC_1753 = 201753,
	VOC_1754 = 201754,
	VOC_1755 = 201755,
	VOC_1756 = 201756,
	VOC_1757 = 201757,
	VOC_1758 = 201758,
	VOC_1759 = 201759,
	VOC_1760 = 201760,
	VOC_1761 = 201761,
	VOC_1762 = 201762,
	VOC_1780 = 201780,
	VOC_1781 = 201781,
	VOC_1782 = 201782,
	VOC_1790 = 201790,
	VOC_1791 = 201791,
	VOC_1792 = 201792,
	VOC_1800 = 201800,
	VOC_1801 = 201801,
	VOC_1802 = 201802,
	VOC_1810 = 201810,
	VOC_1811 = 201811,
	VOC_1812 = 201812,

	VOC_0300 = 200300,
	VOC_0301 = 200301,
	VOC_0302 = 200302,
	VOC_0310 = 200310,
	/// <summary> 피격 보이스</summary>
	//
	VOC_2000 = 202000,
	VOC_2001 = 202001,
	VOC_2010 = 202010,
	VOC_2011 = 202011,
	VOC_2020 = 202020,
	VOC_2021 = 202021,
	VOC_2030 = 202030,
	VOC_2031 = 202031,
	VOC_3243 = 203243,
	VOC_3253 = 203253,
	VOC_3263 = 203263,

	/// <summary> 메인화면, 스테이지 대기 </summary>
	VOC_3001 = 203001,//남자
	VOC_3002 = 203002,
	VOC_3003 = 203003,
	VOC_3004 = 203004,
	VOC_3005 = 203005,
	VOC_3006 = 203006,
	VOC_3007 = 203007,
	VOC_3008 = 203008,
	VOC_3009 = 203009,
	VOC_3010 = 203010,
	VOC_3011 = 203011,
	VOC_3012 = 203012,
	VOC_3013 = 203013,
	VOC_3014 = 203014,
	VOC_3015 = 203015,
	VOC_3016 = 203016,
	VOC_3017 = 203017,
	VOC_3018 = 203018,
	VOC_3019 = 203019,
	VOC_3020 = 203020,
	VOC_3021 = 203021,
	VOC_3022 = 203022,
	VOC_3023 = 203023,
	VOC_3024 = 203024,
	VOC_3025 = 203025,
	VOC_3026 = 203026,
	VOC_3027 = 203027,
	VOC_3028 = 203028,
	VOC_3029 = 203029,
	VOC_3030 = 203030,
	VOC_3031 = 203031,
	VOC_3032 = 203032,
	VOC_3033 = 203033,
	VOC_3034 = 203034,
	VOC_3035 = 203035,
	VOC_3036 = 203036,
	VOC_3037 = 203037,
	VOC_3038 = 203038,
	VOC_3039 = 203039,
	VOC_3040 = 203040,
	VOC_3101 = 203101,//여자
	VOC_3102 = 203102,
	VOC_3103 = 203103,
	VOC_3104 = 203104,
	VOC_3105 = 203105,
	VOC_3106 = 203106,
	VOC_3107 = 203107,
	VOC_3108 = 203108,
	VOC_3109 = 203109,
	VOC_3110 = 203110,
	VOC_3111 = 203111,
	VOC_3112 = 203112,
	VOC_3113 = 203113,
	VOC_3114 = 203114,
	VOC_3115 = 203115,
	VOC_3116 = 203116,
	VOC_3117 = 203117,
	VOC_3118 = 203118,
	VOC_3119 = 203119,
	VOC_3120 = 203120,
	VOC_3121 = 203121,
	VOC_3122 = 203122,
	VOC_3123 = 203123,
	VOC_3124 = 203124,
	VOC_3125 = 203125,
	VOC_3126 = 203126,
	VOC_3127 = 203127,
	VOC_3128 = 203128,
	VOC_3129 = 203129,
	VOC_3130 = 203130,
	VOC_3131 = 203131,
	VOC_3132 = 203132,
	VOC_3133 = 203133,
	VOC_3134 = 203134,
	VOC_3135 = 203135,
	VOC_3136 = 203136,
	VOC_3137 = 203137,
	VOC_3138 = 203138,
	VOC_3139 = 203139,
	VOC_3140 = 203140,
	/// <summary> 메인화면 캐릭터 관리</summary>
	VOC_3201 = 203201,//남자
	VOC_3202 = 203202,
	VOC_3211 = 203211,//여자
	VOC_3212 = 203212,
	/// <summary> 메인화면 다운타운 </summary>
	VOC_3221 = 203221,//남자
	VOC_3222 = 203222,
	VOC_3231 = 203231,//여자
	VOC_3232 = 203232,
	/// <summary> 메인화면 Z패드</summary>
	VOC_3241 = 203241,//남자
	VOC_3242 = 203242,
	VOC_3251 = 203251,//여자
	VOC_3252 = 203252,
	/// <summary> 메인화면 상점</summary>
	VOC_3261 = 203261,//남자
	VOC_3262 = 203262,
	VOC_3271 = 203271,//여자
	VOC_3272 = 203272,

	/// <summary> 공용 보이스</summary>
	//진행 성공
	VOC_3401 = 203401,//남자
	VOC_3402 = 203402,
	VOC_3403 = 203403,
	VOC_3404 = 203404,
	VOC_3405 = 203405,
	VOC_3406 = 203406,
	VOC_3407 = 203407,
	VOC_3408 = 203408,
	VOC_3409 = 203409,
	VOC_3411 = 203411,//여자
	VOC_3412 = 203412,
	VOC_3413 = 203413,
	VOC_3414 = 203414,
	VOC_3415 = 203415,
	VOC_3416 = 203416,
	VOC_3417 = 203417,
	//진행 실패
	VOC_3421 = 203421,//남자
	VOC_3422 = 203422,
	VOC_3423 = 203423,
	VOC_3431 = 203431,//여자
	VOC_3432 = 203432,
	VOC_3433 = 203433,
	/// <summary> 캐릭터 보이스 </summary>
	//남자
	VOC_4001 = 204001,//바실
	VOC_4002 = 204002,
	VOC_4003 = 204003,
	VOC_4004 = 204004,
	VOC_4005 = 204005,
	VOC_4006 = 204006,
	VOC_4007 = 204007,
	VOC_4011 = 204011,//케빈
	VOC_4012 = 204012,
	VOC_4013 = 204013,
	VOC_4014 = 204014,
	VOC_4015 = 204015,
	VOC_4016 = 204016,
	VOC_4017 = 204017,
	VOC_4021 = 204021,//레인
	VOC_4022 = 204022,
	VOC_4023 = 204023,
	VOC_4024 = 204024,
	VOC_4025 = 204025,
	VOC_4026 = 204026,
	VOC_4027 = 204027,
	VOC_4031 = 204031,//알렉스
	VOC_4032 = 204032,
	VOC_4033 = 204033,
	VOC_4034 = 204034,
	VOC_4035 = 204035,
	VOC_4036 = 204036,
	VOC_4037 = 204037,
	VOC_4041 = 204041,//빅터
	VOC_4042 = 204042,
	VOC_4043 = 204043,
	VOC_4044 = 204044,
	VOC_4045 = 204045,
	VOC_4046 = 204046,
	VOC_4047 = 204047,
	VOC_4051 = 204051,//로웰
	VOC_4052 = 204052,
	VOC_4053 = 204053,
	VOC_4054 = 204054,
	VOC_4055 = 204055,
	VOC_4056 = 204056,
	VOC_4057 = 204057,
	VOC_4061 = 204061,//레오나르도
	VOC_4062 = 204062,
	VOC_4063 = 204063,
	VOC_4064 = 204064,
	VOC_4065 = 204065,
	VOC_4066 = 204066,
	VOC_4067 = 204067,
	VOC_4071 = 204071,//핸더슨
	VOC_4072 = 204072,
	VOC_4073 = 204073,
	VOC_4074 = 204074,
	VOC_4075 = 204075,
	VOC_4076 = 204076,
	VOC_4077 = 204077,
	VOC_4081 = 204081,//아렉시스
	VOC_4082 = 204082,
	VOC_4083 = 204083,
	VOC_4084 = 204084,
	VOC_4085 = 204085,
	VOC_4086 = 204086,
	VOC_4087 = 204087,
	VOC_4091 = 204091,//아돌프
	VOC_4092 = 204092,
	VOC_4093 = 204093,
	VOC_4094 = 204094,
	VOC_4095 = 204095,
	VOC_4096 = 204096,
	VOC_4097 = 204097,
	VOC_4101 = 204101,//프레데릭
	VOC_4102 = 204102,
	VOC_4103 = 204103,
	VOC_4104 = 204104,
	VOC_4105 = 204105,
	VOC_4106 = 204106,
	VOC_4107 = 204107,
	VOC_4111 = 204111,//안토니오
	VOC_4112 = 204112,
	VOC_4113 = 204113,
	VOC_4114 = 204114,
	VOC_4115 = 204115,
	VOC_4116 = 204116,
	VOC_4117 = 204117,
	VOC_4121 = 204121,//찰스
	VOC_4122 = 204122,
	VOC_4123 = 204123,
	VOC_4124 = 204124,
	VOC_4125 = 204125,
	VOC_4126 = 204126,
	VOC_4127 = 204127,
	VOC_4131 = 204131,//잭슨
	VOC_4132 = 204132,
	VOC_4133 = 204133,
	VOC_4134 = 204134,
	VOC_4135 = 204135,
	VOC_4136 = 204136,
	VOC_4137 = 204137,
	VOC_4141 = 204141,//이안
	VOC_4142 = 204142,
	VOC_4143 = 204143,
	VOC_4144 = 204144,
	VOC_4145 = 204145,
	VOC_4146 = 204146,
	VOC_4147 = 204147,
	VOC_4151 = 204151,//쥬드
	VOC_4152 = 204152,
	VOC_4153 = 204153,
	VOC_4154 = 204154,
	VOC_4155 = 204155,
	VOC_4156 = 204156,
	VOC_4157 = 204157,
	VOC_4161 = 204161,//저스틴
	VOC_4162 = 204162,
	VOC_4163 = 204163,
	VOC_4164 = 204164,
	VOC_4165 = 204165,
	VOC_4166 = 204166,
	VOC_4167 = 204167,
	VOC_4171 = 204171,//크라머
	VOC_4172 = 204172,
	VOC_4173 = 204173,
	VOC_4174 = 204174,
	VOC_4175 = 204175,
	VOC_4176 = 204176,
	VOC_4177 = 204177,
	VOC_4181 = 204181,//쿠퍼
	VOC_4182 = 204182,
	VOC_4183 = 204183,
	VOC_4184 = 204184,
	VOC_4185 = 204185,
	VOC_4186 = 204186,
	VOC_4187 = 204187,
	VOC_4191 = 204191,//루카스
	VOC_4192 = 204192,
	VOC_4193 = 204193,
	VOC_4194 = 204194,
	VOC_4195 = 204195,
	VOC_4196 = 204196,
	VOC_4197 = 204197,
	VOC_4201 = 204201,//데릭
	VOC_4202 = 204202,
	VOC_4203 = 204203,
	VOC_4204 = 204204,
	VOC_4205 = 204205,
	VOC_4206 = 204206,
	VOC_4207 = 204207,
	VOC_4211 = 204211,//오웬
	VOC_4212 = 204212,
	VOC_4213 = 204213,
	VOC_4214 = 204214,
	VOC_4215 = 204215,
	VOC_4216 = 204216,
	VOC_4217 = 204217,
	VOC_4221 = 204221,//브라이슨
	VOC_4222 = 204222,
	VOC_4223 = 204223,
	VOC_4224 = 204224,
	VOC_4225 = 204225,
	VOC_4226 = 204226,
	VOC_4227 = 204227,
	VOC_4231 = 204231,//필립
	VOC_4232 = 204232,
	VOC_4233 = 204233,
	VOC_4234 = 204234,
	VOC_4235 = 204235,
	VOC_4236 = 204236,
	VOC_4237 = 204237,
	VOC_4241 = 204241,//칼슨
	VOC_4242 = 204242,
	VOC_4243 = 204243,
	VOC_4244 = 204244,
	VOC_4245 = 204245,
	VOC_4246 = 204246,
	VOC_4247 = 204247,
	VOC_4251 = 204251,//조쉬
	VOC_4252 = 204252,
	VOC_4253 = 204253,
	VOC_4254 = 204254,
	VOC_4255 = 204255,
	VOC_4256 = 204256,
	VOC_4257 = 204257,
	VOC_4261 = 204261,//조나단
	VOC_4262 = 204262,
	VOC_4263 = 204263,
	VOC_4264 = 204264,
	VOC_4265 = 204265,
	VOC_4266 = 204266,
	VOC_4267 = 204267,
	VOC_4271 = 204271,//조세프
	VOC_4272 = 204272,
	VOC_4273 = 204273,
	VOC_4274 = 204274,
	VOC_4275 = 204275,
	VOC_4276 = 204276,
	VOC_4277 = 204277,
	VOC_4281 = 204281,//파비앙
	VOC_4282 = 204282,
	VOC_4283 = 204283,
	VOC_4284 = 204284,
	VOC_4285 = 204285,
	VOC_4286 = 204286,
	VOC_4287 = 204287,
	VOC_4291 = 204291,//가브리엘 남매 
	VOC_4292 = 204292,
	VOC_4293 = 204293,
	VOC_4294 = 204294,
	VOC_4295 = 204295,
	VOC_4296 = 204296,
	VOC_4297 = 204297,
	VOC_4301 = 204301,//마크
	VOC_4302 = 204302,
	VOC_4303 = 204303,
	VOC_4304 = 204304,
	VOC_4305 = 204305,
	VOC_4306 = 204306,
	VOC_4307 = 204307,
	VOC_4311 = 204311,//토니
	VOC_4312 = 204312,
	VOC_4313 = 204313,
	VOC_4314 = 204314,
	VOC_4315 = 204315,
	VOC_4316 = 204316,
	VOC_4317 = 204317,
	VOC_4321 = 204321,//오티스
	VOC_4322 = 204322,
	VOC_4323 = 204323,
	VOC_4324 = 204324,
	VOC_4325 = 204325,
	VOC_4326 = 204326,
	VOC_4327 = 204327,
	VOC_4331 = 204331,//에디
	VOC_4332 = 204332,
	VOC_4333 = 204333,
	VOC_4334 = 204334,
	VOC_4335 = 204335,
	VOC_4336 = 204336,
	VOC_4337 = 204337,
	//여자
	VOC_5001 = 205001,//아멜리아
	VOC_5002 = 205002,
	VOC_5003 = 205003,
	VOC_5004 = 205004,
	VOC_5005 = 205005,
	VOC_5006 = 205006,
	VOC_5007 = 205007,
	VOC_5011 = 205011,//신시아
	VOC_5012 = 205012,
	VOC_5013 = 205013,
	VOC_5014 = 205014,
	VOC_5015 = 205015,
	VOC_5016 = 205016,
	VOC_5017 = 205017,
	VOC_5021 = 205021,//다나
	VOC_5022 = 205022,
	VOC_5023 = 205023,
	VOC_5024 = 205024,
	VOC_5025 = 205025,
	VOC_5026 = 205026,
	VOC_5027 = 205027,
	VOC_5031 = 205031,//에스핀
	VOC_5032 = 205032,
	VOC_5033 = 205033,
	VOC_5034 = 205034,
	VOC_5035 = 205035,
	VOC_5036 = 205036,
	VOC_5037 = 205037,
	VOC_5041 = 205041,//레베카
	VOC_5042 = 205042,
	VOC_5043 = 205043,
	VOC_5044 = 205044,
	VOC_5045 = 205045,
	VOC_5046 = 205046,
	VOC_5047 = 205047,
	VOC_5051 = 205051,//제마
	VOC_5052 = 205052,
	VOC_5053 = 205053,
	VOC_5054 = 205054,
	VOC_5055 = 205055,
	VOC_5056 = 205056,
	VOC_5057 = 205057,
	VOC_5061 = 205061,//헤디
	VOC_5062 = 205062,
	VOC_5063 = 205063,
	VOC_5064 = 205064,
	VOC_5065 = 205065,
	VOC_5066 = 205066,
	VOC_5067 = 205067,
	VOC_5071 = 205071,//아리아나
	VOC_5072 = 205072,
	VOC_5073 = 205073,
	VOC_5074 = 205074,
	VOC_5075 = 205075,
	VOC_5076 = 205076,
	VOC_5077 = 205077,
	VOC_5081 = 205081,//셀린
	VOC_5082 = 205082,
	VOC_5083 = 205083,
	VOC_5084 = 205084,
	VOC_5085 = 205085,
	VOC_5086 = 205086,
	VOC_5087 = 205087,
	VOC_5091 = 205091,//카리나
	VOC_5092 = 205092,
	VOC_5093 = 205093,
	VOC_5094 = 205094,
	VOC_5095 = 205095,
	VOC_5096 = 205096,
	VOC_5097 = 205097,
	VOC_5101 = 205101,//제이미
	VOC_5102 = 205102,
	VOC_5103 = 205103,
	VOC_5104 = 205104,
	VOC_5105 = 205105,
	VOC_5106 = 205106,
	VOC_5107 = 205107,
	VOC_5111 = 205111,//로이드
	VOC_5112 = 205112,
	VOC_5113 = 205113,
	VOC_5114 = 205114,
	VOC_5115 = 205115,
	VOC_5116 = 205116,
	VOC_5117 = 205117,
	VOC_5121 = 205121,//메켄지
	VOC_5122 = 205122,
	VOC_5123 = 205123,
	VOC_5124 = 205124,
	VOC_5125 = 205125,
	VOC_5126 = 205126,
	VOC_5127 = 205127,
	VOC_5131 = 205131,//노엘
	VOC_5132 = 205132,
	VOC_5133 = 205133,
	VOC_5134 = 205134,
	VOC_5135 = 205135,
	VOC_5136 = 205136,
	VOC_5137 = 205137,
	VOC_5141 = 205141,//페이지
	VOC_5142 = 205142,
	VOC_5143 = 205143,
	VOC_5144 = 205144,
	VOC_5145 = 205145,
	VOC_5146 = 205146,
	VOC_5147 = 205147,
	VOC_5151 = 205151,//에블리
	VOC_5152 = 205152,
	VOC_5153 = 205153,
	VOC_5154 = 205154,
	VOC_5155 = 205155,
	VOC_5156 = 205156,
	VOC_5157 = 205157,
	VOC_5161 = 205161,//크리시
	VOC_5162 = 205162,
	VOC_5163 = 205163,
	VOC_5164 = 205164,
	VOC_5165 = 205165,
	VOC_5166 = 205166,
	VOC_5167 = 205167,
	VOC_5171 = 205171,//코디
	VOC_5172 = 205172,
	VOC_5173 = 205173,
	VOC_5174 = 205174,
	VOC_5175 = 205175,
	VOC_5176 = 205176,
	VOC_5177 = 205177,
	VOC_5181 = 205181,//엘리시아
	VOC_5182 = 205182,
	VOC_5183 = 205183,
	VOC_5184 = 205184,
	VOC_5185 = 205185,
	VOC_5186 = 205186,
	VOC_5187 = 205187,
	VOC_5191 = 205191,//가브리엘 남매
	VOC_5192 = 205192,
	VOC_5193 = 205193,
	VOC_5194 = 205194,
	VOC_5195 = 205195,
	VOC_5196 = 205196,
	VOC_5197 = 205197,
	VOC_5201 = 205201,//헤레이스
	VOC_5202 = 205202,
	VOC_5203 = 205203,
	VOC_5204 = 205204,
	VOC_5205 = 205205,
	VOC_5206 = 205206,
	VOC_5207 = 205207,
	VOC_5211 = 205211,//주디스
	VOC_5212 = 205212,
	VOC_5213 = 205213,
	VOC_5214 = 205214,
	VOC_5215 = 205215,
	VOC_5216 = 205216,
	VOC_5217 = 205217,
	VOC_5221 = 205221,//미란다
	VOC_5222 = 205222,
	VOC_5223 = 205223,
	VOC_5224 = 205224,
	VOC_5225 = 205225,
	VOC_5226 = 205226,
	VOC_5227 = 205227,
	/// <summary> 피난민 보이스</summary>
	// 남자
	VOC_6901 = 206901,
	VOC_6902 = 206902,
	VOC_6904 = 206904,
	VOC_6905 = 206905,
	VOC_6911 = 206911,
	VOC_6912 = 206912,
	VOC_6914 = 206914,
	VOC_6915 = 206915,
	//여자  
	VOC_6921 = 206921,
	VOC_6922 = 206922,
	VOC_6924 = 206924,
	VOC_6925 = 206925,
	VOC_6931 = 206931,
	VOC_6932 = 206932,
	VOC_6934 = 206934,
	VOC_6935 = 206935,

	VOC_7000 = 207000,
	VOC_7001 = 207001,
	VOC_7010 = 207010,
	VOC_7011 = 207011
}

public enum VoiceType
{
	Success,
	Fail
}
[RequireComponent(typeof(AudioListener))]
public class SoundMng : ObjMng
{
	private static SoundMng m_Instance = null;
	public static SoundMng Instance
	{
		get
		{
			if(m_Instance == null)
			{
				m_Instance = Utile_Class.SetInstance(typeof(SoundMng)) as SoundMng;
				DontDestroyOnLoad(m_Instance.gameObject);   //씬 전환시 안날라가게...
															// 생성되었으니 다른 로그인 씬으로 넘겨준다.
				m_Instance.Init();
			}
			return m_Instance;
		}
	}

	SND_IDX				m_eBgSoundNo = SND_IDX.NONE;
	public SND_IDX GetNowBG { get { return m_eBgSoundNo; } }
	AudioSource			PlayBgSnd;
	List<AudioSource>	PlayBgEffSnds = new List<AudioSource>();
	List<AudioSource>	PlayEffSnds = new List<AudioSource>();
	Dictionary<int, AudioClip> EffSndClips = new Dictionary<int, AudioClip>();
	List<int>			PlayEffNos = new List<int>();
	List<int>			DirectPlayEffNos = new List<int>();
	//List<SND_IDX>		AllVoice = new List<SND_IDX>() {
	//	SND_IDX.SFX_3001,
	//	SND_IDX.SFX_3002,
	//	SND_IDX.SFX_3003,
	//	/// <summary> 특정 컨텐츠 진입때 재생 </summary>
	//	SND_IDX.SFX_3241,
	//	SND_IDX.SFX_3242,
	//	SND_IDX.SFX_3243,
	//	SND_IDX.SFX_3251,
	//	SND_IDX.SFX_3252,
	//	SND_IDX.SFX_3253,
	//	SND_IDX.SFX_3261,
	//	SND_IDX.SFX_3262,
	//	SND_IDX.SFX_3263,
	//	/// <summary> 공통 진행성공 </summary>
	//	SND_IDX.SFX_3401,
	//	SND_IDX.SFX_3402,
	//	SND_IDX.SFX_3403,
	//	/// <summary> 공통 진행불가 </summary>
	//	SND_IDX.SFX_3421,
	//	SND_IDX.SFX_3422,
	//	SND_IDX.SFX_3423,
	//	/// <summary> 공통 진행완료 </summary>
	//	SND_IDX.SFX_3441,
	//	SND_IDX.SFX_3442,
	//	SND_IDX.SFX_3443
	//};

	//void Start()
	//{
	//	AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;
	//}

	bool Is_VolumeZero;
	public void Init()
	{
		AllStop();
	}
	
	// 제거 -> 유니티 2022.3.7f1 업데이트후 발생안함 코드가 있으면 오히려 광고나, 로그인등 다른 Activity창이 출력된후 문제 발생함
//	// 버그 : 폴드 테스트 결과 블루투스 이어폰 연결후 백으로 갔다 다시 돌아오면 사운드 재생이 안된다.
//	// 수정 : OnAudioConfigurationChanged 사용 안함 AndroidManifast android:exported="true" 셋팅

//	// 오디오 연결이 변경됨 알림
//	void OnAudioConfigurationChanged(bool deviceWasChanged)
//	{
//		AudioConfiguration config;
//		if (deviceWasChanged)
//		{
//			config = AudioSettings.GetConfiguration();
//			//config.dspBufferSize = 64;
//			// 설정이 리셋되어버림 사운드가 처음부터 재생됨 현재 블루투스 이어폰 문제를 해결하려면 Reset밖에는 없어보임
//			// 단 이게 무조건 들어가게될경우기존에 잘나오는것들도 처음부터 실행하게되므로 일단 보류
//			AudioSettings.Reset(config);	
//		}
//#if USE_LOG_MANAGER
//		int bufferLength, numBuffers;
//		AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
//		config = AudioSettings.GetConfiguration();
//		Utile_Class.DebugLog(string.Format("OnAudioConfigurationChanged : {0:#,#}Hz {1} {2}samples {3}buffers", config.sampleRate, config.speakerMode.ToString(), config.dspBufferSize, numBuffers));
//		Utile_Class.DebugLog("OnAudioConfigurationChanged : " + (deviceWasChanged ? "Device was changed" : "Reset was called"));
//#endif

//		// 플레이중이었던 사운드들 다시 재생
//		CheckSound();
//	}

//	void OnApplicationPause(bool paused)
//	{
//		if (!paused)
//		{
//#if USE_LOG_MANAGER
//			int bufferLength, numBuffers;
//			AudioSettings.GetDSPBufferSize(out bufferLength, out numBuffers);
//			AudioConfiguration config = AudioSettings.GetConfiguration();
//			Utile_Class.DebugLog(string.Format("OnAudioConfigurationChanged : {0:#,#}Hz {1} {2}samples {3}buffers", config.sampleRate, config.speakerMode.ToString(), config.dspBufferSize, numBuffers));
//#endif
//			// 블루투스 상태에서 Resume시 사운드 재생이 되지 않아 다시 셋팅
//			//OnAudioConfigurationChanged(true);
//			OnAudioConfigurationChanged(true);
//			//CheckSound();
//		}
//		//else PlayBgSnd.Pause();
//	}



	public void AllStop()
	{
		StopBG();
		StopEff();
	}

	public void LoadingStopSND()
	{
		StopBG();
		StopAllVoice();
	}
	
	public void StopBG()
	{
		if (PlayBgSnd != null) {
			PlayBgSnd.Stop();
			m_eBgSoundNo = SND_IDX.NONE;
		}
		for(int i = PlayBgEffSnds.Count - 1; i > -1; i--) PlayBgEffSnds[i].Stop();
	}
	
	public void StopEff(bool _onlyfx = false)
	{
		PlayEffNos.Clear();
		for(int i = PlayEffSnds.Count - 1; i > -1; i--)
		{
			AudioSource sound = PlayEffSnds[i];
			if (_onlyfx && sound.isPlaying && sound.clip.name.Contains("VOC")) continue;
			sound.Stop();
			PlayEffSnds.Remove(sound);
			GameObject.Destroy(sound.gameObject, 0);
		}
	}

	public void SetBGMute(bool bMute)
	{
		if(PlayBgSnd != null)
		{
			PlayBgSnd.mute = bMute;
			for(int i = PlayBgEffSnds.Count - 1; i > -1; i--) PlayBgEffSnds[i].mute = PlayBgSnd.mute;
			PlayBgSound(m_eBgSoundNo);
			DLGTINFO?.f_VideoVolume?.Invoke(bMute ? 0f : 1f);
		}
		// TODO : 추후 옵션값
		//OPTION.SetOption(GOption.EOption.SOUND_BG, !bMute);
	}
	/// <summary> 전체 사운드 뮤트 </summary>
	public void AllMute(bool _mute) {
		Is_VolumeZero = _mute;
		if (PlayBgSnd != null) {
			if (!_mute) PlayBgSnd.mute = Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0));
			else PlayBgSnd.mute = _mute; 
		}
		for (int i = PlayBgEffSnds.Count - 1; i > -1; i--) {
			PlayBgEffSnds[i].mute = _mute;
		}
		for (int i = PlayEffSnds.Count - 1; i > -1; i--) {
			PlayEffSnds[i].mute = _mute;
		}
	}
	public void FXMute(bool _mute) {
		for (int i = PlayBgEffSnds.Count - 1; i > -1; i--) {
			PlayBgEffSnds[i].mute = _mute;
		}
		for (int i = PlayEffSnds.Count - 1; i > -1; i--) {
			PlayEffSnds[i].mute = _mute;
		}
	}
	public bool IS_Play()
	{
		//if(m_eBgSoundNo < SND_IDX.BGM_0002) return false;
		return PlayBgSnd == null || PlayBgSnd.mute;
	}

	void FixedUpdate()
	{
		CheckSound();
		DirectPlayEffNos.Clear();
		PlayEffSound();
	}

	void CheckSound()
	{
		PlayBgSound(m_eBgSoundNo);
		// 이펙트및 다른 사운드는 플레이가 끝났다면 제거 해준다.
		for(int i = PlayEffSnds.Count - 1; i > -1 ; i--)
		{
			AudioSource sound = PlayEffSnds[i];
			if(!sound.isPlaying)
			{
				// 제거
				PlayEffSnds.Remove(sound);
				GameObject.Destroy(sound.gameObject, 0);
			}
		}

		for(int i = PlayBgEffSnds.Count - 1; i > -1; i--)
		{
			AudioSource sound = PlayBgEffSnds[i];
			if(!sound.isPlaying)
			{
				// 제거
				PlayEffSnds.Remove(sound);
				GameObject.Destroy(sound.gameObject, 0);
			}
		}
	}

	/// <summary> 플레이중인 이펙트 사운드 제거 </summary>
	/// <param name="idx"> 제거할 사운드 </param>
	public void StopEffSound(SND_IDX idx)
	{
		string strName = string.Format("Snd_Eff_{0}", (int)idx);
		for(int i = PlayEffSnds.Count - 1; i > -1; i--)
		{
			AudioSource sound = PlayEffSnds[i];
			if(sound.isPlaying && sound.gameObject.name.Equals(strName))
			{
				sound.Stop();
				// 제거
				PlayEffSnds.Remove(sound);
				GameObject.Destroy(sound.gameObject, 0);
			}
		}

		for(int i = PlayBgEffSnds.Count - 1; i > -1; i--)
		{
			AudioSource sound = PlayBgEffSnds[i];
			if(sound.isPlaying && sound.gameObject.name.Equals(strName))
			{
				sound.Stop();
				// 제거
				PlayEffSnds.Remove(sound);
				GameObject.Destroy(sound.gameObject, 0);
			}
		}
	}
	public void StopAllVoice() {
		for (int i = PlayEffSnds.Count - 1; i > -1; i--) {
			AudioSource sound = PlayEffSnds[i];
			if (sound.isPlaying && sound.clip.name.Contains("VOC")) {
				sound.Stop();
				// 제거
				PlayEffSnds.Remove(sound);
				GameObject.Destroy(sound.gameObject, 0);
			}
		}
	}
	public void PlayBgSound(SND_IDX idx)
	{
		if (UTILE == null) return;
		int no = (int)idx;
		if(PlayBgSnd == null)
		{
			GameObject objSound = new GameObject();
			objSound.transform.SetParent(gameObject.transform);
			PlayBgSnd = objSound.AddComponent<AudioSource>();

			// TODO : 추후 옵션값
			PlayBgSnd.mute = Is_VolumeZero || Convert.ToBoolean(PlayerPrefs.GetInt("BGSND_MUTE", 0));// !OPTION.GetOption(GOption.EOption.SOUND_BG);
		}

		//if(idx < SND_IDX.BGM_0002)
		//{
		//	m_eBgSoundNo = idx;
		//	StopBG();
		//	return;
		//}
		if(m_eBgSoundNo == idx && PlayBgSnd.isPlaying) return;
		if(PlayBgSnd.mute)
		{
			m_eBgSoundNo = idx;
			return;
		}

		if (idx == SND_IDX.NONE) return;
		string path = idx.ToString();
		// 이미 플레이중인지 확인한다.
		PlayBgSnd.Stop();
		AudioClip clip = UTILE.LoadSnd(path);
		if(clip == null) return;
		PlayBgSnd.name = string.Format("Snd_Bg_{0}", no);
		PlayBgSnd.clip = clip;
		PlayBgSnd.loop = true;
		PlayBgSnd.Play();
		m_eBgSoundNo = idx;
	}

	// 배경음과 같은 옵션값으로 출력되는 이펙트음
	public void PlayBgEffSound(SND_IDX idx)
	{
		if (UTILE == null) return;
		if (idx == SND_IDX.NONE) return;
		int no = (int)idx;
		if(PlayBgSnd.mute) return;
		// 한프레임에 같은 이펙트음은 하나만 셋팅되도록하기
		AudioClip clip = EffSndClips.ContainsKey(no) ? EffSndClips[no] : null;
		string path = string.Format("e_{0}", no);
		if(clip == null)
		{
			clip = UTILE.LoadSnd(path);
			if(clip == null) return;
			EffSndClips.Add(no, clip);
		}
		if(clip == null) return;
		GameObject obj = new GameObject();
		obj.transform.SetParent(gameObject.transform);
		obj.name = string.Format("Snd_BgEff_{0}", no);
		obj.AddComponent<AudioSource>();
		AudioSource sound = obj.GetComponent<AudioSource>();
		// 이펙트 음은 PlayOneShot을 호출해야 배경음이 중단 안된다.
		//sound.PlayOneShot(clip);		// 배경음의 위치에 사운드를 출력할때 사용함 여기서는 정상적인 체크가 되지않아 사용안함
		sound.clip = clip;
		sound.loop = false;
		sound.Play();
		PlayBgEffSnds.Add(sound);
	}

	public AudioSource DirectPlayEffSound(SND_IDX idx, bool bConfig = false, float _volume = 1f)
	{
		if (UTILE == null) return null;
		if (idx == SND_IDX.NONE) return null;
		int no = (int)idx;


		// TODO : 추후 옵션값
		//if (!bConfig && !OPTION.GetOption(GOption.EOption.SOUND_EFF)) return;
		if (Convert.ToBoolean(PlayerPrefs.GetInt("FXSND_MUTE", 0))) return null;
		if (Is_VolumeZero) return null;

		if (DirectPlayEffNos.Contains(no)) return null;
		DirectPlayEffNos.Add(no);
		if (idx == SND_IDX.SFX_1060) _volume = 0.6f;
		return EffPlay(no, PlayEffSnds, _volume);
	}
	public void DelayPlayEffSound(float _delay, SND_IDX idx, bool bConfig = false, float _volume = 1f) {
		StartCoroutine(DelaySND(_delay, idx, bConfig, _volume));
	}
	IEnumerator DelaySND(float _delay, SND_IDX idx, bool bConfig = false, float _volume = 1f) {
		yield return new WaitForSeconds(_delay);
		DirectPlayEffSound(idx, bConfig, _volume);
	}
	public void PlayEffSound(SND_IDX idx, bool bConfig = false)
	{
		if (UTILE == null) return;
		if (idx == SND_IDX.NONE) return;
		int no = (int)idx;
		// TODO : 추후 옵션값
		//if (!bConfig && !OPTION.GetOption(GOption.EOption.SOUND_EFF)) return;
		if (Convert.ToBoolean(PlayerPrefs.GetInt("FXSND_MUTE", 0))) return;
		if (Is_VolumeZero) return;

		// 한프레임에 같은 이펙트음은 하나만 셋팅되도록하기
		if (PlayEffNos.Contains(no)) return;
		PlayEffNos.Add(no);
	}

	AudioSource EffPlay(int nNo, List<AudioSource> listcontroll, float _volume = 1f)
	{
		if (UTILE == null) return null;
		string path = ((SND_IDX)nNo).ToString();
		AudioSource sound;
		AudioClip clip = EffSndClips.ContainsKey(nNo) ? EffSndClips[nNo] : null;
		if(clip == null)
		{
			clip = UTILE.LoadSnd(path);
			if(clip == null) return null;
			EffSndClips.Add(nNo, clip);
		}
		if(clip == null) return null;
		GameObject obj = new GameObject();
		obj.transform.SetParent(gameObject.transform);
		obj.name = string.Format("Snd_Eff_{0}", nNo);
		obj.AddComponent<AudioSource>();
		sound = obj.GetComponent<AudioSource>();
		// 이펙트 음은 PlayOneShot을 호출해야 배경음이 중단 안된다.
		//sound.PlayOneShot(clip);		// 배경음의 위치에 사운드를 출력할때 사용함 여기서는 정상적인 체크가 되지않아 사용안함
		sound.clip = clip;
		sound.loop = false;
		sound.volume = _volume;
		sound.Play();
		listcontroll.Add(sound);
		return sound;
	}

	void PlayEffSound(float _volume = 1f)
	{
		if(PlayEffNos.Count < 1)	return;
		for(int i = PlayEffNos.Count - 1; i > -1; i--)
		{
			EffPlay(PlayEffNos[i], PlayEffSnds, _volume);
			PlayEffNos.RemoveAt(i);
		}
	}

	public void PlayStageBG() {
		List<SND_IDX> idxs = new List<SND_IDX>() { SND_IDX.BGM_0100, SND_IDX.BGM_0101, SND_IDX.BGM_0102, SND_IDX.BGM_0103 };
		switch (STAGEINFO.m_PlayType) {
			case StagePlayType.Stage:
				if (STAGEINFO.m_TStage.GetDarkLv > 0) PlayBGSound(SND_IDX.BGM_0103);
				else if(STAGEINFO.m_StageContentType == StageContentType.Stage){
					PlayBGSound(STAGEINFO.m_TStage.m_BGM);
				}
				break;
			case StagePlayType.OutContent:
				PlayBGSound(STAGEINFO.m_TStage.m_BGM);
				//PlayBGSound(idxs[UTILE.Get_Random(0, idxs.Count - 1)]);
				break;
			case StagePlayType.Event:
				MyFAEvent evt = USERINFO.m_Event.Datas.Find(o => BaseValue.EVENT_LIST.Contains(o.Type));
				if(evt != null) {
					switch (evt.Prefab) {
						case "Event_10": PlayBGSound(SND_IDX.BGM_0300); break;
						default: PlayBGSound(SND_IDX.BGM_0100); break;
					}
				}
				break;
			default: PlayBGSound(SND_IDX.BGM_0100); break;
		}
	}

	/// <summary> 특정 이펙트 사운드가 재생목록에 있는지 체크 </summary>
	public bool IS_PlayFXSnd(List<SND_IDX> _idx) {
		for (int i = 0; i < _idx.Count; i++) {
			if(PlayEffSnds.Find(o => o.clip.name.Equals(_idx[i].ToString()))) return true;
		}
		return false;
	}
	public bool IS_PlayVoicdSnd() {
		for (int i = 0; i < PlayEffSnds.Count; i++) {
			if (PlayEffSnds.Find(o => o.clip.name.Contains("VOC"))) return true;
		}
		return false;
	}
	public void Play_VoiceSnd(List<SND_IDX> _idxs) {
		//StopAllVoice();
		if (_idxs.Count < 1) return;
		if (!IS_PlayVoicdSnd()) {
			PlayEffSound(_idxs[UTILE.Get_Random(0, _idxs.Count)]);
		}
	}
	public void Play_CommVoiceSnd(VoiceType _type) {
		switch (_type) {
			case VoiceType.Success:
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						Play_VoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3411,
							SND_IDX.VOC_3412,
							SND_IDX.VOC_3413,
							SND_IDX.VOC_3414,
							SND_IDX.VOC_3415,
							SND_IDX.VOC_3416,
							SND_IDX.VOC_3417
						});
						break;
					default:
						Play_VoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3401,
							SND_IDX.VOC_3402,
							SND_IDX.VOC_3403,
							SND_IDX.VOC_3404,
							SND_IDX.VOC_3405,
							SND_IDX.VOC_3406,
							SND_IDX.VOC_3407,
							SND_IDX.VOC_3408,
							SND_IDX.VOC_3409
						});
						break;
				}
				break;
			case VoiceType.Fail:
				switch (USERINFO.GetGender()) {
					case GenderType.Female:
						Play_VoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3431,
							SND_IDX.VOC_3432,
							SND_IDX.VOC_3433
						});
						break;
					default:
						Play_VoiceSnd(new List<SND_IDX>() {
							SND_IDX.VOC_3421,
							SND_IDX.VOC_3422,
							SND_IDX.VOC_3423
						});
						break;
				}
				break;
		}
	}
}
