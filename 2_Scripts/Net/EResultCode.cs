using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EResultCode
{
	// SUCCESS
	public const ushort SUCCESS = 0x0000;							// 성공
	public const ushort SUCCESS_NEW_AUTH = 0x0001;					// 성공 신규 생성
	public const ushort SUCCESS_REWARD_PIECE = 0x0002;				// 캐릭터가 있어 피스로 지급됨(RES_REWARD 에서 사용)
	public const ushort SUCCESS_POST = 0x0003;						// 가방이 부족하여 일부 우편함으로 수령됨(보상창 연출이 끝난후 센터 메세지 박스로 내용 표시해주어야함)
	public const ushort SUCCESS_INVEN = 0x0004;						// 가방이 부족하여 일부 아이템이 지급 안됨(우편함에서 사용, 메세지는 필요없음 UI만 다시 셋팅필요)

	// Common Error 0xFF00 ~ 0xFFFFF
	public const ushort ERROR_SERVER_EXCEPTION = 0xFFFF;			// 서버 에러
	public const ushort ERROR_DB_EXCEPTION = 0xFFFE;				// 디비 에러
	public const ushort ERROR_PARAMETER = 0xFFFD;					// 파라메터 에러
	public const ushort ERROR_SERVER_CHECK = 0xFFFC;				// 서버 점검중
	public const ushort ERROR_NEW_VER = 0xFFFB;						// 버전 업그레이드 필요
	public const ushort ERROR_NOT_USE_APK = 0xFFFA;					// 사용 불가 APK
	public const ushort ERROR_USERNO = 0xFFF9;						// 잘못된 유저번호
	public const ushort ERROR_TOOLDATA = 0xFFF8;					// 툴데이터 정보 없음
	public const ushort ERROR_UID = 0xFFF7;							// 잘못된 UID
	public const ushort ERROR_POS = 0xFFF6;							// 위치값 잘못됨
	public const ushort ERROR_TIME = 0xFFF5;						// 시간 정보 오류(상점구매 시간이 안됨등 시간이 잘못되었을때 오류)
	public const ushort ERROR_FILTER = 0xFFF4;						// 필터 걸림
	public const ushort ERROR_DONE = 0xFFF3;						// 이미 진행 함
	public const ushort ERROR_DIF_DIVICE = 0xFFF2;					// 다른 기기 접속
	public const ushort ERROR_USED_NAME = 0xFFF1;					// 사용 중인 이름
	public const ushort ERROR_LENGTH = 0xFFF0;						// 글자수 제한
	public const ushort ERROR_DELETE_USER = 0xFFEF;					// 제거요청중 유저
	public const ushort ERROR_CROSS_CHECK_DATA_NULL = 0xFFEE;		// 라르고 소프트 (데이터 null)
	public const ushort ERROR_CROSS_CHECK = 0xFFED;					// 라르고 소프트 (보안프로그램 크로스체크 실패)
	public const ushort ERROR_PROTOCOL_CODE = 0xFFEC;				// 프로토콜 결과 재전송 오류

	// Login Error 0xEF00 ~ 0xEFFFF
	public const ushort ERROR_USED_UUID = 0xEFFF;					// 이미 사용중인 UUID
	public const ushort ERROR_USER_BLOCK = 0xEFFE;					// 블럭 유저
	public const ushort ERROR_AGREE = 0xEFFD;						// 이용약관 수신동의 필요
	public const ushort ERROR_ENERGY = 0xEFFC;						// 플레이할 에너지 부족
	public const ushort ERROR_CASH = 0xEFFB;						// 캐시 부족
	public const ushort ERROR_MONEY = 0xEFFA;						// 머니 부족
	public const ushort ERROR_ETC_ITEM = 0xEFF9;					// Etc 아이템 부족
	public const ushort ERROR_MAX_LV = 0xEFF8;						// 최대 레벨
	public const ushort ERROR_CHAR_EXP = 0xEFF7;					// 캐릭터 경험치 부족
	public const ushort ERROR_LOGIN = 0xEFF6;						// 로그인 필요 AUTH부터 다시 해야됨
	public const ushort ERROR_LOGIN_ID = 0xEFF5;					// 로그인 아이디 정보 오류
	public const ushort ERROR_PVPCOIN = 0xEFF4;						// PVP코인부족
	public const ushort ERROR_GCOIN = 0xEFF3;						// 길드코인부족
	public const ushort ERROR_MILEAGE = 0xEFF2;						// 마일리지 부족
	public const ushort ERROR_CAMP_RES = 0xEFF1;					// 캠프 자원 부족

	// UserInfo Error 0xDF00 ~ 0xDFFFF
	public const ushort ERROR_NOT_FOUND_USER = 0xDFFF;				// 유저 못찾음(유저번호 오류)
	public const ushort ERROR_NOT_FOUND_CHAR = 0xDFFE;				// 캐릭터 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_ITEM = 0xDFFD;				// 아이템 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_STAGE = 0xDFFC;				// 스테이지 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_DECK = 0xDFFB;				// 덱정보 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_TUTO = 0xDFFA;				// 튜토리얼 정보 못찾음
	public const ushort ERROR_NOT_FOUND_ADV = 0xDFF9;				// 탐사 정보 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_RES = 0xDFF8;				// 연구 정보 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_MAKING = 0xDFF7;			// 생산 정보 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_POST = 0xDFF6;				// 우편 정보 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_DNA = 0xDFF5;				// DNA 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_ZOMBIE = 0xDFF4;			// 좀비 못찾음(UID 오류)
	public const ushort ERROR_INVEN_SIZE = 0xDFF3;					// 가방 사이즈 부족
	public const ushort ERROR_NOT_OPEN = 0xDFF2;					// 컨텐츠 오픈 안됨(연구 탭 포함)
	public const ushort ERROR_CAGE_SIZE = 0xDFF1;					// 케이지 사이즈 부족
	public const ushort ERROR_NICKNAME = 0xDFF0;					// 닉네임 설정 오류
	public const ushort ERROR_PROFILE = 0xDFEF;						// 프로필 이미지 오류
	public const ushort ERROR_NOT_FOUND_MISSION = 0xDFEE;			// 미션 못찾음(UID 오류)
	public const ushort ERROR_NOT_FOUND_CAMP_BUILD = 0xDFED;		// 캠프 건물 정보 못찾음

	// Stage Error 0xCF00 ~ 0xCFFFF
	public const ushort ERROR_NOT_PLAY_STAGE = 0xCFFF;				// 플레이할 스테이지가 아님
	public const ushort ERROR_END_PLAY_STAGE = 0xCFFE;				// 종료된 스테이지(더이상 없음)
	public const ushort ERROR_BUY_CNT = 0xCFFD;						// 구매 횟수 제한
	public const ushort ERROR_NOT_BUY_STAGE = 0xCFFC;				// 구매 가능한 스테이지가 아님
	public const ushort ERROR_PLAY_CNT = 0xCFFB;					// 입장 횟수 제한
	public const ushort ERROR_PLAYCODE = 0xCFFA;					// 플레이 코드 오류
	public const ushort ERROR_ALT_INFO = 0xCFF9;					// 시드선택 정보 오류
	public const ushort ERROR_REWARD = 0xCFF8;						// 보상 지급 오류

	// Char Error 0xBF00 ~ 0xBFFFF
	public const ushort ERROR_NOT_EQUIP_ITEM = 0xBFFF;				// 장착 아이템 아님
	public const ushort ERROR_MAX_GRADE = 0xBFFE;					// 최대 등급
	public const ushort ERROR_CHAR_PRE_SERUM = 0xBFFD;				// 선행 혈청 주입 필요
	public const ushort ERROR_CHAR_GRADE = 0xBFFC;                  // 등급 필요
	public const ushort ERROR_CHAR_DNA_SLOT_OPEN = 0xBFFB;          // 슬롯이 오픈된 상태
	public const ushort ERROR_CHAR_LEARNED_SERUM = 0xBFFA;			// 이미 배운 혈청

	// Item Error 0xAF00 ~ 0xAFFFF
	public const ushort ERROR_ITEM_OPTION_POS = 0xAFFF;				// 옵션 변경 불가(위치 잘못됨)

	// Adventure Error 0x9F00 ~ 0x9FFFF
	public const ushort ERROR_ADV_SET_CHAR_CNT = 0x9FFF;			// 배치 캐릭터 개수 부족
	public const ushort ERROR_ITEM_OPTION_OPEN_MAX = 0xAFFE;		// 옵션 슬롯 최대치
	public const ushort ERROR_ITEM_NOT_FOUND_REMAKE = 0xAFFD;		// 재조립 정보 없음
	public const ushort ERROR_ITEM_REMAKE_PROB = 0xAFFC;			// 아이템 재조립 확률정보 오류
	public const ushort ERROR_ITEM_DEF_GRADE = 0xAFFB;				// 아이템 등급이 다름
	public const ushort ERROR_ITEM_EQUIPPED = 0xAFFA;				// 장착중인 아이템
	public const ushort ERROR_ITEM_NEED_LV = 0xAFF9;				// 아이템 필요 레벨
	public const ushort ERROR_ITEM_DEF_EQUIPTYPE = 0xAFF8;			// 아이템 장착 부위가 다름
	public const ushort ERROR_ITEM_MAX_GRADE = 0xAFF7;				// 아이템 최대등급
	public const ushort ERROR_ITEM_SELECT_IDX = 0xAFF6;				// 선택 정보 없음

	// Research Error 0x8F00 ~ 0x8FFFF
	public const ushort ERROR_RES_PLAY = 0x8FFF;					// 진행중인 연구 있음
	public const ushort ERROR_RES_PRECED = 0x8FFE;					// 선행 연구 필요
	public const ushort ERROR_RES_MAX_LV = 0x8FFD;					// 최대 레벨
	public const ushort ERROR_RES_TIME = 0x8FFC;					// 완료 시간이 안됨
	public const ushort ERROR_RES_CAMP_LV = 0x8FFB;					// 캠프 레벨업 필요

	// Making Error 0x7F00 ~ 0x7FFFF
	public const ushort ERROR_MAKING_IDX = 0x7FFF;					// 이미 제작중인 인덱스
	public const ushort ERROR_MAKING_LV = 0x7FFE;					// 제작 불가 레벨


	// Zombie Error 0x6F00 ~ 0x6FFF
	public const ushort ERROR_ZOMBIE_GRADE = 0x6FFF;				// 좀비 등급의 문제가 있음
	public const ushort ERROR_MAX_CAGE = 0x6FFE;					// 케이지 구매 횟수 
	public const ushort ERROR_NOT_FOUND_CAGE = 0x6FFD;				// 케이지 못찾음(생성 오류)
	public const ushort ERROR_MAX_ROOM_SIZE = 0x6FFC;				// 공간 부족

	// DNA Error 0x5F00 ~ 0x5FFF
	public const ushort ERROR_DNA_LV = 0x5FFF;						// DNA 레벨에 문제가 있음
	public const ushort ERROR_DNA_MAX_LV = 0x5FFE;					// DNA 최대 레벨
	public const ushort ERROR_DNA_EQUIP = 0x5FFD;					// DNA 장착중


	// Event Error 0x4F00 ~ 0x4FFF
	public const ushort ERROR_NOT_EVENT = 0x4FFF;					// 이벤트 아님
	public const ushort ERROR_EVENT_LIMIT = 0x4FFE;					// 이벤트 제한 확인

	// Shop Error 0x3F00 ~ 0x3FFF
	public const ushort ERROR_SHOP_INAPP = 0x3FFF;					// 인앱 상품 잘못된 프로토콜 호출됨 또는 인앱 상품 오류
	public const ushort ERROR_SHOP_PUID = 0x3FFD;					// PUID 정보 없음
	public const ushort ERROR_SHOP_IDX = 0x3FFC;					// 구매 시도 인덱스와 다른 인덱스가 사용됨 PUID 정보의 인덱스와 다름
	public const ushort ERROR_SHOP_RECEIPT = 0x3FFB;				// 구매 영수증 오류
	public const ushort ERROR_AUCTION_PRICE = 0x3FFA;				// 이전 값보다 낮은금액(정보 획득후 타유저가 먼저 입찰할경우 발생할 수 있음)
	public const ushort ERROR_SHOP_LIMIT = 0x3FF9;					// 구매 횟수 제한
	public const ushort ERROR_AUCTION_TIME = 0x3FF8;				// 마감됨
	public const ushort ERROR_BUY_TIME = 0x3FF7;					// 구매필요
	public const ushort ERROR_BUY_INFO = 0x3FF6;					// 구매 정보 없음
	public const ushort ERROR_PACK_INFO = 0x3FF5;					// 패키지 정보 없음
	public const ushort ERROR_SHOP_BUYED = 0x3FF4;					// 이미 구매함
	public const ushort ERROR_SHOP_BUY_CANCEL = 0x3F01;				// 구매 팝업 취소
	public const ushort ERROR_SHOP_BUY_MARKET_ERROR = 0x3F00;		// 마켓 SDK 에러

	// Friend Error 0x2F00 ~ 0x2FFF
	public const ushort ERROR_FRIEND_CNT = 0x2FFF;					// 친구 수 제한
	public const ushort ERROR_FRIEND_RICEIVE_INVITE_CNT = 0x2FFE;	// 친구 초대 받기 제한
	public const ushort ERROR_FRIEND = 0x2FFD;						// 이미 친구임
	public const ushort ERROR_FRIEND_CODE = 0x2FFC;					// 추천 코드 오류
	public const ushort ERROR_NOT_FOUND_FRIEND = 0x2FFB;			// 거절한 친구(초대 목록에 없음)
	public const ushort ERROR_FRIEND_DEL_CNT = 0x2FFA;				// 친구 제거 제한
	public const ushort ERROR_FRIEND_DELETED = 0x2FF9;				// 제거된 친구 초대 제한
	public const ushort ERROR_MY_FRIEND_CNT = 0x2FF8;				// 내 친구수 제한

	// Mission Error 0x1F00 ~ 0x1FFF
	public const ushort ERROR_MISSION_CNT = 0x1FFF;					// 미션 달성 안됨
	public const ushort ERROR_MISSION_RESET_CNT = 0x1FFE;           // 미션 리셋 횟수


	// Guild Error 0x0F00 ~ 0x0FFF
	public const ushort ERROR_NOT_FOUND_GUILD = 0x0FFF;				// 길드 못찾음
	public const ushort ERROR_GUILD_JOIN = 0x0FFE;					// 가입된 길드가 있음
	public const ushort ERROR_GUILD_JOIN_TIME = 0x0FFD;				// 길드 재가입 및 생성 제한 시간
	public const ushort ERROR_GUILD_MAX_MEMBER = 0x0FFC;			// 인원수 제한 (자동 가입의경우 요청 목록으로 들어감)
	public const ushort ERROR_GUILD_GRADE = 0x0FFB;					// 권한 없음
	public const ushort ERROR_GUILD_NOTICE_LENGTH = 0x0FFA;			// 글자수 제한
	public const ushort ERROR_GUILD_INTRO_LENGTH = 0x0FF9;			// 글자수 제한
	public const ushort ERROR_GUILD_END_RESEARCH = 0x0FF8;			// 종료된 연구
	public const ushort ERROR_GUILD_PLAY_RESEARCH = 0x0FF7;			// 진행중인 연구가 있음
	public const ushort ERROR_GUILD_DIF_RESEARCH = 0x0FF6;			// 진행 연구가 다름
	public const ushort ERROR_GUILD_CHECK_RESEARCH_UNLOCK = 0x0FF5;	// 연구 불가 언락 정보 확인
	public const ushort ERROR_GUILD_RESEARCH_CNT = 0x0FF4;			// 연구 횟수 오류
	public const ushort ERROR_GUILD_NOT_MEMBER = 0x0FF3;			// 길드 맴버가 아님
	public const ushort ERROR_GUILD_MANY_REQ = 0x0FF2;				// 길드 가입 요청이 너무 많음
	public const ushort ERROR_GUILD_REQ_LV = 0x0FF1;				// 길드 가입 레벨 제한
	public const ushort ERROR_GUILD_DESTROY = 0x0FF0;				// 길드 해산 필요

	// Deck Error 0x0E00 ~ 0x0EFF
	public const ushort ERROR_DECK_SET_CHAR_CNT = 0x0EFF;			// 배치 캐릭터 개수 부족

	// PVP Error 0x0D00 ~ 0x0DFF
	public const ushort ERROR_PVP_JOIN = 0x0DFF;					// PVP 데이터 생성 오류
	public const ushort ERROR_PVP_STATE = 0x0DFE;					// PVP 진행중 아님
	public const ushort ERROR_PVP_TARGET_NO = 0x0DFD;				// PVP 타겟 정보 잘못됨
	public const ushort ERROR_PVP_NOT_REWARD_KIND = 0x0DFC;			// PVP 보상 종류 잘못됨
	public const ushort ERROR_PVP_REWARD_CNT = 0x0DFB;				// PVP 보상 횟수 오류
	public const ushort ERROR_PVP_KILL_CNT = 0x0DFB;				// PVP 킬 보상 킬 수 부족
	public const ushort ERROR_NOT_FOUND_PVP_LEAGUE = 0x0DFA;		// PVP 리그 정보 보류
	public const ushort ERROR_NOT_FOUND_PVP_SEASON = 0x0DF9;		// PVP 시즌 정보 보류
	public const ushort ERROR_PVP_LIMIT = 0x0DF8;					// PVP 입장 제한

	// Camp Error 0x0C00 ~ 0x0CFF
	public const ushort ERROR_CAMP_RES_PRODUCTION = 0x0CFF;			// CAMP 자원 생산 중
	public const ushort ERROR_BUILD_LV = 0x0CFE;					// 건물 레벨 부족
	public const ushort ERROR_BUILD_RES_MAX = 0x0CFD;				// 자원 보관 최대치 초과
	public const ushort ERROR_BUILD_MAX_LV = 0x0CFC;				// 건물 맥스레벨
	public const ushort ERROR_BUILD_NOT_IDLE = 0x0CFB;				// 건물 이 작업중임
	public const ushort ERROR_CAMP_PLUNDER_LOG = 0x0CFA;			// 로그 데이터 찾지 못함
	public const ushort ERROR_CAMP_COUNT_ATK = 0x0CF9;				// 반격 불가
	public const ushort ERROR_CAMP_NOT_RES_PRODUCTION = 0x0CF8;		// CAMP 자원 생산 중이 아님
}

public class EStateError
{
	public const ushort SUCCESS = 200;							// 성공
	public const ushort ERROR_NETERROR = 405;					// 네트워크 에러
	public const ushort ERROR_SERVER_EXCEPTION = 0;				// 서버 에러
	public const ushort ERROR_DATA_PROCESS = 1;					// 수신에 성공했지만 데이터에 문제가있음
	public const ushort ERROR_TIMEOUT = 504;					// 타임아웃
}
