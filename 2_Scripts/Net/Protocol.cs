using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protocol
{
	///<summary> 배포된 앱 접속 체크용 </summary>
	public static readonly string REQ_APPCHECK = "/API/AppCheck";
	///<summary> 서버셋팅 설정 받기 </summary>
	public static readonly string REQ_CONFIG = "/API/Config";
	///<summary> 서버셋팅 설정 받기 </summary>
	public static readonly string REQ_SERVERTIME = "/API/ServerTime";
	///<summary> 시스템 메세지 받기 </summary>
	public static readonly string REQ_SYSTEM_MSG = "/API/SystemMsg";
	///<summary> Analytics </summary>
	public static readonly string REQ_ANALYTICS = "/API/Analytics";

	///<summary> 서버 접속 및 유저정보 받기 </summary>
	public static readonly string REQ_AUTH = "/API/Auth";

	///<summary> 마지막 프로토콜 데이터 다시 받기 </summary>
	public static readonly string REQ_RE_RES_LSAT_DATA = "/API/LastData";



	///<summary> 오류등 유저정보 다시 받기 </summary>
	public static readonly string REQ_ALL_INFO = "/API/AllInfo";

	///<summary> 계정 연동전 관련 유저 검색 </summary>
	public static readonly string REQ_ACC_INFO = "/API/Acc/Info";
	///<summary> 계정 연동(변경할때만 사용할 것) </summary>
	public static readonly string REQ_ACC_CHANGE = "/API/Acc/Change";
	///<summary> 계정 탈퇴 요청 </summary>
	public static readonly string REQ_ACC_DELETE = "/API/Acc/Delete";




	///<summary> 유저 정보 받기 </summary>
	public static readonly string REQ_USERINFO = "/API/User";
	///<summary> 캐릭터 정보 받기 </summary>
	public static readonly string REQ_CHARINFO = "/API/Char";
	///<summary> 아이템 정보 받기 </summary>
	public static readonly string REQ_ITEMINFO = "/API/Item";
	///<summary> 스테이지 정보 받기 </summary>
	public static readonly string REQ_STAGE = "/API/Stage";
	///<summary> 덱 정보 받기 </summary>
	public static readonly string REQ_DECK = "/API/Deck";
	///<summary> 튜토리얼 정보 받기 </summary>
	public static readonly string REQ_TUTO = "/API/Tuto";
	///<summary> 탐험 정보 받기 </summary>
	public static readonly string REQ_ADVINFO = "/API/Adv";
	///<summary> 연구 정보 받기 </summary>
	public static readonly string REQ_RESEARCHINFO = "/API/Research";
	///<summary> 생산 정보 받기 </summary>
	public static readonly string REQ_MAKINGINFO = "/API/Making";
	///<summary> 우편함 정보 받기 </summary>
	public static readonly string REQ_POSTINFO = "/API/Post";
	///<summary> 좀비 정보 받기 </summary>
	public static readonly string REQ_ZOMBIEINFO = "/API/Zombie";
	///<summary> DNA 정보 받기 </summary>
	public static readonly string REQ_DNAINFO = "/API/DNA";


	///<summary> 유저 프로필 정보 수정하기 </summary>
	public static readonly string REQ_USER_PROFILE_SET = "/API/User/SetProfile";
	///<summary> 유저 닉네임 정보 수정하기 </summary>
	public static readonly string REQ_USER_NICKNAME_SET = "/API/User/SetNickName";
	


	///<summary> 튜토리얼 종료 </summary>
	public static readonly string REQ_TUTOEND = "/API/Tuto/End";



	///<summary> 덱 정보 변경하기 </summary>
	public static readonly string REQ_DECK_SET = "/API/Deck/Set";


	///<summary> 스테이지 입장권 구매 </summary>
	public static readonly string REQ_STAGE_BUY_LIMIT = "/API/Stage/BuyLimit";
	///<summary> 스테이지 시작 </summary>
	public static readonly string REQ_STAGE_START = "/API/Stage/Start";
	///<summary> 스테이지 실패 </summary>
	public static readonly string REQ_STAGE_FAIL = "/API/Stage/Fail";
	///<summary> 스테이지 실패 </summary>
	public static readonly string REQ_STAGE_FAIL_REWARD = "/API/Stage/FailReward";
	///<summary> 스테이지 보상받기 </summary>
	public static readonly string REQ_STAGE_CLEAR = "/API/Stage/Clear";
	///<summary> 소탕권 사용 </summary>
	public static readonly string REQ_STAGE_CLEAR_TICKET = "/API/Stage/ClearTicket";
	///<summary> 긴급 임무 초기화 </summary>
	public static readonly string REQ_RESET_REPLAY = "/API/Stage/ResetRePlay";
	///<summary> 챕터 보상받기 </summary>
	public static readonly string REQ_STAGE_ALTREWARD = "/API/Stage/Alternative";
	///<summary> 챕터 보상받기 </summary>
	public static readonly string REQ_STAGE_CHAPTERREWARD = "/API/Stage/ChapterReward";
	///<summary> 스테이지 토크 선택 보상 받기 </summary>
	public static readonly string REQ_STAGE_TALKSELECT = "/API/Stage/TalkSelect";
	///<summary> 전투보상 다시굴리기 </summary>
	public static readonly string REQ_STAGE_REROLLING = "/API/Stage/Rerolling";
	///<summary> 클리어 유저 픽 정보 받기 </summary>
	public static readonly string REQ_USER_STAGE_PICK_INFO = "/API/Stage/UserPickInfo";



	///<summary> 캐릭터 레벨업 </summary>
	public static readonly string REQ_CHAR_LVUP = "/API/Char/LVUp";
	///<summary> 캐릭터 스킬 레벨업 </summary>
	public static readonly string REQ_CHAR_SKILL_LVUP = "/API/Char/SkillUp";
	///<summary> 캐릭터 등급 업그레이드 </summary>
	public static readonly string REQ_CHAR_GRADEUP = "/API/Char/GradeUp";
	///<summary> 캐릭터 장비 장착 </summary>
	public static readonly string REQ_CHAR_EQUIP = "/API/Char/Equip";
	///<summary> 캐릭터 장비 장착 해제 </summary>
	public static readonly string REQ_CHAR_UNEQUIP = "/API/Char/UnEquip";
	///<summary> 캐릭터 장비 장착 해제 </summary>
	public static readonly string REQ_CHAR_DNA_SET = "/API/Char/SetDNA";
	///<summary> 캐릭터 DNA슬롯 오픈 </summary>
	public static readonly string REQ_CHAR_DNA_SLOTOPEN = "/API/Char/DNASlotOpen";
	///<summary> 캐릭터 각성 </summary>
	public static readonly string REQ_CHAR_SERUM = "/API/Char/Serum";
	///<summary> 캐릭터 각성 페이지 오픈</summary>
	public static readonly string REQ_CHAR_SERUM_PAGE_OPEN = "/API/Char/SerumPageOpen";
	///<summary> 캐릭터 각성 </summary>
	public static readonly string REQ_CHAR_SET_EVA = "/API/Char/Set_Evaluation";
	///<summary> 캐릭터 각성 </summary>
	public static readonly string REQ_CHAR_GET_EVA = "/API/Char/Get_Evaluation";






	///<summary> 아이템 잠금 </summary>
	public static readonly string REQ_ITEM_LOCK = "/API/Item/Lock";
	///<summary> 아이템 레벨업 </summary>
	public static readonly string REQ_ITEM_LVUP = "/API/Item/LVUp";
	///<summary> 아이템 옵션 변경 </summary>
	public static readonly string REQ_ITEM_OPCHANGE = "/API/Item/OPChange";
	///<summary> 아이템 옵션 슬롯 오픈 </summary>
	public static readonly string REQ_ITEM_OPOPEN = "/API/Item/OPOpen";
	///<summary> 아이템 옵션 변경 </summary>
	public static readonly string REQ_ITEM_SELL = "/API/Item/Sell";
	///<summary> 아이템 재조립 </summary>
	public static readonly string REQ_ITEM_REMAKE = "/API/Item/ReMake";
	///<summary> 아이템 재조립 선택 </summary>
	public static readonly string REQ_ITEM_REMAKE_SELECT = "/API/Item/ReMakeSelect";
	///<summary> 아이템 승급 </summary>
	public static readonly string REQ_ITEM_UPGRADE = "/API/Item/Upgrade";
	///<summary> 인사파일 교체 </summary>
	public static readonly string REQ_CHAGE_CHAR_PIECE = "/API/Item/CharPieceChange";


	///<summary> 탐험 시작 </summary>
	public static readonly string REQ_ADV_START = "/API/Adv/Start";
	///<summary> 탐험 종료 </summary>
	public static readonly string REQ_ADV_END = "/API/Adv/End";
	///<summary> 탐험 정보 리셋 </summary>
	public static readonly string REQ_ADV_RESET = "/API/Adv/Reset";



	///<summary> 연구 시작 </summary>
	public static readonly string REQ_RESEARCH_START = "/API/Research/Start";
	///<summary> 연구 종료 </summary>
	public static readonly string REQ_RESEARCH_END = "/API/Research/End";



	///<summary> 생산 시작 </summary>
	public static readonly string REQ_MAKING_START = "/API/Making/Start";
	///<summary> 생산 종료 </summary>
	public static readonly string REQ_MAKING_END = "/API/Making/End";



	///<summary> 우편함 보상 받기 </summary>
	public static readonly string REQ_POST_REWARD = "/API/Post/Get";


	///<summary> 좀비 방 정보 </summary>
	public static readonly string REQ_ZOMBIE_ROOM_INFO = "/API/Zombie/Room";
	///<summary> 좀비 케이지 오픈 </summary>
	public static readonly string REQ_ZOMBIE_CAGE_OPEN = "/API/Zombie/CageOpen";
	///<summary> 좀비 장착 </summary>
	public static readonly string REQ_ZOMBIE_SET = "/API/Zombie/Set";
	///<summary> 좀비 파괴 </summary>
	public static readonly string REQ_ZOMBIE_DESTROY = "/API/Zombie/Destroy";
	///<summary> 케이지 생산 수확 </summary>
	public static readonly string REQ_ZOMBIE_PRODUCE = "/API/Zombie/Produce";

	///<summary> DNA 생성 </summary>
	public static readonly string REQ_DNA_CREATE = "/API/DNA/Create";
	///<summary> DNA 레벨 업 </summary>
	public static readonly string REQ_DNA_UPGRADE = "/API/DNA/Upgrade";
	///<summary> DNA 파괴 </summary>
	public static readonly string REQ_DNA_DESTROY = "/API/DNA/Destroy";
	///<summary> DNA 변형 </summary>
	public static readonly string REQ_DNA_OPCHANGE = "/API/DNA/OPChange";



	///<summary> 챌린지 정보 </summary>
	public static readonly string REQ_CHALLENGEINFO_All = "/API/Challenge/All";


	///<summary> 상점 정보 </summary>
	public static readonly string REQ_SHOP_INFO = "/API/Shop";
	///<summary> 아이템 구매 </summary>
	public static readonly string REQ_SHOP_BUY = "/API/Shop/Buy";
	/// <summary> 구매 검증 PUID 받기 </summary>
	public static readonly string REQ_SHOP_PUID = "/API/Shop/PUID";
	/// <summary> 구매 아이템 받기 </summary>
	public static readonly string REQ_SHOP_INAPP = "/API/Shop/InApp";
	/// <summary> 인벤 구매 </summary>
	public static readonly string REQ_SHOP_BUY_INVEN = "/API/Shop/BuyInven";
	/// <summary> 시즌패스 레벨업 구매 </summary>
	public static readonly string REQ_SHOP_BUY_PASS_LV = "/API/Shop/BuyPassLV";
	/// <summary> 패키지 상품 받기 </summary>
	public static readonly string REQ_SHOP_GET_DAILYPACKITEM = "/API/Shop/DailyPackItem";


	///<summary> 돌발 이벤트 종료 </summary>
	public static readonly string REQ_ADDEVENT_END = "/API/AddEvent/End";
	///<summary> 돌발 이벤트 블랙마켓 아이템 구매 </summary>
	public static readonly string REQ_ADD_EVENT_BLACKMARKET = "/API/AddEvent/BlackMarket";
	///<summary> 돌발 이벤트 습격(노트 전투) </summary>
	public static readonly string REQ_ADD_EVENT_SUDDENENEMY = "/API/AddEvent/SuddenEnemy";
	///<summary> 돌발 이벤트 NPC 대화 </summary>
	public static readonly string REQ_ADD_EVENT_NPC = "/API/AddEvent/Npc";


	/// <summary> 업적 정보 받기 </summary>
	public static readonly string REQ_ACHIEVE_INFO = "/API/Achieve";
	/// <summary> 업적 보상 받기 </summary>
	public static readonly string REQ_ACHIEVE_REWARD = "/API/Achieve/Reward";

	/// <summary> 컬렉션 정보 받기 </summary>
	public static readonly string REQ_COLLECTION_INFO = "/API/Collection";
	/// <summary> 컬렉션 레벨업 </summary>
	public static readonly string REQ_COLLECTION_LVUP = "/API/Collection/LVUP";



	/// <summary> 이벤트 정보 받기 </summary>
	public static readonly string REQ_MY_FAEVENT_INFO = "/API/Event";
	/// <summary> 이벤트 체크 </summary>
	public static readonly string REQ_CHECK_MY_FAEVENT_INFO = "/API/Event/Check";
	/// <summary> 성장(ex. 칠면조 키우기)이벤트 성장시키기 </summary>
	public static readonly string REQ_EVENT_GROWUP = "/API/Event/GrowUP";


	/// <summary> 탑랭킹 </summary>
	public static readonly string REQ_RANKING = "/API/Ranking";
	/// <summary> 내 랭킹 </summary>
	public static readonly string REQ_MY_RANKING = "/API/Ranking/My";


	/// <summary> 경매장 정보 받기 </summary>
	public static readonly string REQ_AUCTION_INFO = "/API/Auction";
	/// <summary> 경매장 아이템 구매 </summary>
	public static readonly string REQ_AUCTION_BUY = "/API/Auction/Buy";



	/// <summary> 친구 정보 받기 </summary>
	public static readonly string REQ_FRIEND = "/API/Friend";
	/// <summary> 친구 찾기 </summary>
	public static readonly string REQ_FRIEND_FIND = "/API/Friend/Find";
	/// <summary> 추천 친구 </summary>
	public static readonly string REQ_FRIEND_RECOMMEND = "/API/Friend/Recommend";
	/// <summary> 친구 초대하기 </summary>
	public static readonly string REQ_FRIEND_INVITE = "/API/Friend/Invite";
	/// <summary> 친구 삭제 or 거절 </summary>
	public static readonly string REQ_FRIEND_DELETE = "/API/Friend/Delete";
	/// <summary> 친구 수락하기 </summary>
	public static readonly string REQ_FRIEND_ACCEPT = "/API/Friend/Accept";
	/// <summary> 선물 하기 </summary>
	public static readonly string REQ_FRIEND_GIFT_GIVE = "/API/Friend/Gift/Give";
	/// <summary> 선물 받기 </summary>
	public static readonly string REQ_FRIEND_GIFT_GET = "/API/Friend/Gift/Get";


	/// <summary> 미션 정보 </summary>
	public static readonly string REQ_MISSIONINFO = "/API/Mission";
	/// <summary> 미션 보상 </summary>
	public static readonly string REQ_MISSION_REWARD = "/API/Mission/Reward";
	/// <summary> 미션 다시 받기 </summary>
	public static readonly string REQ_MISSION_RESET = "/API/Mission/Reset";



	///<summary> 길드 상세 정보 </summary>
	public static readonly string REQ_GUILD = "/API/Guild";
	///<summary> 길드 생성 </summary>
	public static readonly string REQ_GUILD_CREATE = "/API/Guild/Create";
	///<summary> 길드 해산 </summary>
	public static readonly string REQ_GUILD_DESTROY = "/API/Guild/Destroy";
	///<summary> 길드 찾기 </summary>
	public static readonly string REQ_GUILD_FIND = "/API/Guild/Find";
	///<summary> 추천 길드 </summary>
	public static readonly string REQ_GUILD_RECOMMEND = "/API/Guild/Recommend";
	///<summary> 길드 가입 신청 </summary>
	public static readonly string REQ_GUILD_JOIN = "/API/Guild/Join";
	///<summary> 길드 가입 신청 취소 </summary>
	public static readonly string REQ_CANCEL_GUILD_JOIN = "/API/Guild/CancelJoin";
	///<summary> 길드 가입 신청자 리스트 </summary>
	public static readonly string REQ_GUILD_REQUSER_LIST = "/API/Guild/ReqUser";
	///<summary> 길드 가입 수락 </summary>
	public static readonly string REQ_GUILD_REQUSER_APPLY = "/API/Guild/ReqApply";
	///<summary> 길드 가입 거절 </summary>
	public static readonly string REQ_GUILD_REQUSER_REJECT = "/API/Guild/ReqReject";
	///<summary> 길드 탈퇴(추방) </summary>
	public static readonly string REQ_GUILD_REMOVE_USER = "/API/Guild/RemoveUser";
	///<summary> 길드 등급 수정 </summary>
	public static readonly string REQ_GUILD_MEMBER_GRADE_CHANGE = "/API/Guild/GradeChange";
	///<summary> 길드 마스터 위임받기 </summary>
	public static readonly string REQ_GUILD_APPLY_MASTAR = "/API/Guild/MastarApply";
	///<summary> 길드 정보 수정 </summary>
	public static readonly string REQ_GUILD_INFO_CHANGE = "/API/Guild/InfoChange";
	///<summary> 길드 출첵 </summary>
	public static readonly string REQ_GUILD_ATTENDANCE = "/API/Guild/Attendance";
	///<summary> 길드 연구 시작 </summary>
	public static readonly string REQ_GUILD_RES_START = "/API/Guild/ResStart";
	///<summary> 길드 연구 중지 </summary>
	public static readonly string REQ_GUILD_RES_STOP = "/API/Guild/ResStop";
	///<summary> 길드 연구 </summary>
	public static readonly string REQ_GUILD_RES_GIVE = "/API/Guild/ResGive";



	///<summary> 자신의 PVP 리그 그룹 정보 받기 </summary>
	public static readonly string REQ_PVP_GROUP = "/API/PVP";
	///<summary> 해당 유저의 상세 정보 </summary>
	public static readonly string REQ_PVP_USER_DETAIL_INFO = "/API/PVP/UserInfo";
	///<summary> 대전 상대 검색 </summary>
	public static readonly string REQ_PVP_SEARCH_USER = "/API/PVP/SearchUser";
	///<summary> 대전 시작 </summary>
	public static readonly string REQ_PVP_START = "/API/PVP/Start";
	///<summary> 대전 종료 </summary>
	public static readonly string REQ_PVP_END = "/API/PVP/End";
	///<summary> 대전 보상 받기 </summary>
	public static readonly string REQ_PVP_REWARD = "/API/PVP/Reward";

	///<summary> 캠프 건물 정보 받기 </summary>
	public static readonly string REQ_CAMP_BUILD = "/API/Camp/Builds";
	///<summary> 캠프 건물 레벨업 </summary>
	public static readonly string REQ_CAMP_BUILD_LVUP = "/API/Camp/Build/LVUP";
	///<summary> 캠프 자원 생산 시작 </summary>
	public static readonly string REQ_CAMP_RES_START = "/API/Camp/Res/Start";
	///<summary> 캠프 자원 생산 종료 </summary>
	public static readonly string REQ_CAMP_RES_END = "/API/Camp/Res/End";
	///<summary> 캠프 약탈 정보 </summary>
	public static readonly string REQ_CAMP_PLUNDER_LOG = "/API/Camp/Plunder";
}
