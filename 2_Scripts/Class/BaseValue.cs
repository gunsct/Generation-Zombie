using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.Globalization;
#endif
using UnityEngine;
using UnityEngine.Networking;

public class BaseValue : ClassMng
{
	public static readonly int[] INIT_CHARS = { 1034, 1005, 1018 };
	/// <summary> 닉네임 글자수 제한 </summary>
	readonly public static int NICKNAME_LENGTH = 16;
	readonly public static int GUILD_INTRO_LENGTH = 30;
	readonly public static int GUILD_NOTICE_LENGTH = 100;
	readonly public static int GUILD_MAX_REQ_CNT = 30;
	public static readonly int STAGE_LINE = 10;
	public static readonly Vector3 STAGE_BG_SCALE = new Vector3(0.85f, 0.85f, 0.85f);
	public static readonly Vector3 STAGE_INTERVER = new Vector3(5.3f, 6.6f, 0);
	public static readonly float STAGE_SELECT_LINE_EPOSX = 6.2f;
	public static readonly float STAGE_SELECT_LINE_EPOSY = -0.75f;
	public static readonly Vector3 STAGE_SELECT_LINE_SCALE = new Vector3(1.2f, 1.2f, 1f);
	public static readonly float STAGE_INIT_SCALE = 1f;
	public static readonly float STAGE_MOVE_TIME = 0.2f;//0.3->0.2
	public static readonly float STAGE_STEP1_TIMESCALE = 1.5f;
	public static readonly float STAGE_STEP2_TIMESCALE = 2.5f;
	public static readonly float STAGE_STEP3_TIMESCALE = 4f; //2.5 -> 3.75에서 3단계로 분할

	public static int STAGE_ACC_STEP_SAVE { get {
#if STAGE_TEST
			return PlayerPrefs.GetInt($"AccelBtn_{ MainMng.Instance.USERINFO.m_UID}", 0);
#else
			if (MainMng.Instance.USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < UNLOCK_DOUBLE_SPEED) return 0;
			else return PlayerPrefs.GetInt($"AccelBtn_{ MainMng.Instance.USERINFO.m_UID}", 0);
#endif
	} }
	public static float STAGE_STEP_TIMESCALE(int _pos) {
		if (_pos == 0) return STAGE_STEP1_TIMESCALE;
		else if (_pos == 1) return STAGE_STEP2_TIMESCALE;
		else return STAGE_STEP3_TIMESCALE;
	}
	public static int STAGE_ACC_STEP_NEXT() {
		int step = PlayerPrefs.GetInt($"AccelBtn_{ MainMng.Instance.USERINFO.m_UID}", 0);
		step = step == 2 ? 0 : step + 1;
		PlayerPrefs.SetInt($"AccelBtn_{ MainMng.Instance.USERINFO.m_UID}", step);
		PlayerPrefs.Save();
		return step;
	}

	public static readonly Color[] GradeStarColor = { Color.white, new Color(1f, 242f / 255f, 0f), new Color(1f, 141f / 255f, 0f) };


	public static readonly int SUDDEN_EVENT_PROB = 20;
	public static readonly int BATTLE_PROB = 20;
	public static readonly float RISKRISE_TIME = 3f;

	public static readonly int STAGE_MAKE_GETMAX = 7;
	public static int TOWER_LIMIT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.MaxTowerLimit); } }
	/// <summary> 달러 인덱스 </summary>
	public static int DOLLAR_IDX { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.DOLLAR_IDX); } }
	/// <summary> 경험치 인덱스 </summary>
	public static int EXP_IDX { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.EXP_IDX); } }
	/// <summary> 캐시 인덱스 </summary>
	public static int CASH_IDX { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.CASH_IDX); } }
	/// <summary>  유료 캐시 인덱스 </summary>
	public static readonly int PAYCASH_IDX = 132;
	/// <summary>  2배속 해금 아이템 인덱스 </summary>
	public static readonly int SPEED_IDX = 131;
	/// <summary> 박스 인덱스 </summary>
	public static int ENERGY_IDX { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ENERGY_IDX); } }
	public static readonly int PVPCOIN_IDX = 135;

	/// <summary> 길드 코인(길드 상점용 재화) </summary>
	public static readonly int GUILDCOIN_IDX = 6002;
	/// <summary> 길드 기여도 </summary>
	public static readonly int GUILDPOINT_IDX = 6001;

	/// <summary> 이어하기 티켓 인덱스 </summary>
	public static readonly int CONTINUETICKET_IDX = 133;
	public static int DNABOX_IDX(DNABGType _type) {
		switch (_type) {
			case DNABGType.Red: return 2050001;
			case DNABGType.Blue: return 2050002;
			case DNABGType.Green: return 2050004;
			case DNABGType.Purple: return 2050003;
		}
		return 2050001;
	}
	public static readonly int MILEAGE_IDX = 136;
	public static readonly int CAMP_RES_JUNK_IDX = 137;
	public static readonly int CAMP_RES_CULTIVATE_IDX = 138;
	public static readonly int CAMP_RES_CHEMICAL_IDX = 139;
	public static int CAMP_RES_IDX(int _pos) {
		switch (_pos) {
			case 0: return CAMP_RES_JUNK_IDX;
			case 1: return CAMP_RES_CULTIVATE_IDX;
			case 2: return CAMP_RES_CHEMICAL_IDX;
			default:return 0;
		}
	}
	public static int CAMP_RES_IDX(LS_Web.Res_RewardType _type) {
		switch (_type) {
			case LS_Web.Res_RewardType.CampRes_Junk: return CAMP_RES_JUNK_IDX;
			case LS_Web.Res_RewardType.CampRes_Cultivate: return CAMP_RES_CULTIVATE_IDX;
			case LS_Web.Res_RewardType.CampRes_Chemical: return CAMP_RES_CHEMICAL_IDX;
			default: return 0;
		}
	}
	public static LS_Web.Res_RewardType CAMP_RES_TYPE(int _pos) {
		switch (_pos) {
			case 0: return LS_Web.Res_RewardType.CampRes_Junk;
			case 1: return LS_Web.Res_RewardType.CampRes_Cultivate;
			case 2: return LS_Web.Res_RewardType.CampRes_Chemical;
			default: return 0;
		}
	}
	public static int CAMP_RES_POS(int _idx) {
		switch (_idx) {
			case 137: return 0;
			case 138: return 1;
			case 139: return 2;
			default: return -1;
		}
	}

	public static readonly int ENERGY_SHOP_IDX = 129;
	public static readonly int BLACKMARKET_REFRHES_SHOP_IDX = 6999;
	public static readonly int BLACKMARKET_ADSREFRHES_SHOP_IDX = 203;
	public const int CONTINUETICKET_SHOP_IDX = 131;
	/// <summary> 이어하기 골드 구매 </summary>
	public const int CONTINUETICKET_GOLD_SHOP_IDX = 135;
	public const int CONTINUEAD_SHOP_IDX = 130;
	public const int PVP_TICKET_SHOP_IDX = 136;
	public static readonly int COMMON_PERSONNELFILE_IDX = 1011;
	public static readonly int ADS_SUPPLYBOX_SHOP_IDX = 201;
	public static readonly int NORMAL_SUPPLYBOX_SHOP_IDX = 5101;
	public static readonly int MONTHLY_SUPPLYBOX_SHOP_IDX = 5102;
	public static readonly int ADSGACHAONE_SHOP_IDX = 204;
	public static readonly int ADSITEMGACHAONE_SHOP_IDX = 304;
	public static readonly int PASS_LV_SHOP_IDX = 132;
	public static readonly int[] MISSIONRFRESH_SHOP_IDX = new int[4] { 311, 312, 313, 314 };
	public static readonly int PVPSTORE_REFRESH_SHOP_IDX = 418;
	public static readonly int REPLAY_REFRESH_SHOP_IDX = 207;
	public static readonly int CHAR_GACHA_TICKET_IDX = 3000003;
	public static readonly int EQ_GACHA_TICKET_IDX = 3000004;


	/// <summary> DNA 슬롯 3번째 칸 오픈 </summary>
	public const int SHOP_IDX_DNA_SLOT_OPEN_3 = 401;
	/// <summary> DNA 슬롯 4번째 칸 오픈 </summary>
	public const int SHOP_IDX_DNA_SLOT_OPEN_4 = 402;
	/// <summary> DNA 슬롯 5번째 칸 오픈 </summary>
	public const int SHOP_IDX_DNA_SLOT_OPEN_5 = 403;

	/// <summary> 좀비 케이지 확장 </summary>
	public const int SHOP_IDX_ZOMBIE_CAGE = 405;
	public static int Get_Grade_Shop_Idx_EqOp(int _grade, bool _open) {
		switch (_grade) {
			case 1: return _open ? 431 : 421;
			case 2: return _open ? 432 : 422;
			case 3: return _open ? 433 : 423;
			case 4: return _open ? 434 : 424;
			case 5: return _open ? 435 : 425;
			case 6: return _open ? 436 : 426;
			case 7: return _open ? 437 : 427;
			case 8: return _open ? 438 : 428;
			case 9: return _open ? 439 : 429;
			case 10: return _open ? 440 : 430;
			default: return _open ? 431 : 421;
		}
	}
	/// <summary> 탐험 목록 리셋 </summary>
	public const int SHOP_IDX_ADV_RESET = 409;
	/// <summary> 가방 구매 </summary>
	public const int SHOP_IDX_INVEN = 410;
	/// <summary> 스테이지 보상 다시 굴리기 </summary>
	public const int SHOP_IDX_STAGE_REROLLING = 411;

	/// <summary> 다운타운(사관학교) 티겟 구매 </summary>
	public const int SHOP_IDX_STAGE_LIMIT_ACADEMY = 412;
	/// <summary> 다운타운(은행) 티겟 구매 </summary>
	public const int SHOP_IDX_STAGE_LIMIT_BANK = 413;
	/// <summary> 다운타운(댄공장) 티겟 구매 </summary>
	public const int SHOP_IDX_STAGE_LIMIT_FACTORY = 414;
	/// <summary> 다운타운(공동묘지) 티겟 구매 </summary>
	public const int SHOP_IDX_STAGE_LIMIT_CEMETERY = 415;
	/// <summary> 다운타운(대학) 티겟 구매 </summary>
	public const int SHOP_IDX_STAGE_LIMIT_UNIVERSITY = 416;
	/// <summary> 다운타운(지하철) 티겟 구매 </summary>
	public const int SHOP_IDX_STAGE_LIMIT_SUBWAY = 417;
	/// <summary> 연합 생성 </summary>
	public const int SHOP_IDX_UNION_MAKE = 205;
	public const int SHOP_IDX_UNION_NAME_CHANGE = 206;
	/// <summary> 이벤트 미션 리셋 </summary>
	public const int SHOP_IDX_EVENT_MISSION_RESET = 4005;
	public const int ITEM_IDX_UNION_BASE_MARK = 6004;
	/// <summary> 캐릭터,장비 뽑기 티켓 구매 </summary>
	public const int SHOP_IDX_CHARGACHA_TICKET = 133;
	public const int SHOP_IDX_ITEMGACHA_TICKET = 134;
	public const int SHOP_IDX_MONTHLY_PACKAGE = 11001;

	public static string GetUserName(string name)
	{
		return string.IsNullOrWhiteSpace(name) ? "Unknown" : name;
	}

	public static int GetStageLimitItem(StageContentType type)
	{
		switch (type)
		{
		case StageContentType.Bank: return SHOP_IDX_STAGE_LIMIT_BANK;
		case StageContentType.Academy: return SHOP_IDX_STAGE_LIMIT_ACADEMY;
		case StageContentType.University: return SHOP_IDX_STAGE_LIMIT_UNIVERSITY;
		case StageContentType.Cemetery: return SHOP_IDX_STAGE_LIMIT_CEMETERY;
		case StageContentType.Factory: return SHOP_IDX_STAGE_LIMIT_FACTORY;
		case StageContentType.Subway: return SHOP_IDX_STAGE_LIMIT_SUBWAY;
		}
		return 0;
	}

	public static int GetStageBuyLimit(StageContentType type)
	{
		var shopidx = GetStageLimitItem(type);
		if (shopidx == 0) return 0;
		var tshop = MainMng.Instance.m_ToolData.GetShopTable(shopidx);
		if (tshop == null) return 0;
		return tshop.m_LimitCnt;
	}

	public const int NORMALSTGIDX = 101;
	public const int HARDSTGIDX = 3101;
	public const int NIGHTMARESTGIDX = 5601;
	public static int GetDiffStgIdx(int _sidx) {
		if (_sidx < HARDSTGIDX) return 0;
		else if (_sidx < NIGHTMARESTGIDX) return 1;
		else return 2;
	}
	/// <summary> 가방 인덱스 </summary>
	public static int INVEN_IDX { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.INVEN_IDX); } }
	/// <summary> 소탕권 인덱스 </summary>
	public static int CLEARTICKET_IDX { get { return 3000001; } }
	/// <summary> 소탕권 인덱스 </summary>
	public static int CHARTICKET_IDX { get { return 3000003; } }
	/// <summary> 소탕권 인덱스 </summary>
	public static int ITEMTICKET_IDX { get { return 3000004; } }
	public static int NAMECHANGETICKET_IDX { get { return MainMng.Instance.m_ToolData.GetShopTable(120).m_Rewards[0].m_ItemIdx; } }
	public static readonly int NAMECHANGE_SHOP_IDX = 120;

	public static readonly int SKILL_TYPE_IDX = 38; //스킬 타입 UIString 시작 인덱스
	/// <summary> 캐릭터 최대 랭크 </summary>
	public static int CHAR_MAX_RANK { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.CharMaxRank); } }
	public static int CHAR_MAX_LV { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UserMaxLevel); } }
	/// <summary> 기본 인벤토리 개수 </summary>
	public static int INVEN_BASE_CNT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.BaseInvenCount); } }
	/// <summary> 인벤토리 구매 단위 </summary>
	public static int INVEN_BUY_CNT { get { return MainMng.Instance.m_ToolData.GetShopTable(SHOP_IDX_INVEN).m_Rewards[0].m_ItemCnt; } }
	/// <summary> 확장 가능한 인벤의 최대 개수 </summary>
	public static int INVEN_SLOT_MAX { get {
			TShopTable data = MainMng.Instance.m_ToolData.GetShopTable(SHOP_IDX_INVEN);
			return INVEN_BASE_CNT + (data.m_Rewards[0].m_ItemCnt * data.m_LimitCnt);} }
	/// <summary> 아이템 누적 수량 </summary>
	public static int ITEM_MAXCNT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ItemMaxCount); } }
	/// <summary> 전용장비 추가 전투력 </summary>
	public static int SPECIAL_ITEM_POWER { get { return 1 + MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SpecialStatBenefit); } }
	/// <summary> 전투 보상 최대 레벨 </summary>
	public static int INGAME_REWARD_MAXLV { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.InGameRewardMaxLevel); } }


	readonly public static int MAX_DECK_CNT = 23;
	public const int MAX_DECK_POS_PVP_ATK = 21;
	public const int MAX_DECK_POS_PVP_DEF = 22;

	public static int PVP_TURN_LIMIT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.PVPTurnLimit); } }
	public static float PVP_INIT_TURN_SEED { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PVPInitialTurnSeed); } }
	public static int PVP_HEAL_TURN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.PVPHealTurn); } }
	public static float PVP_HEAL_RATE { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PVPHealRate); } }
	public static float PVP_DEF_RATE { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PVPDefRate); } }
	public static float PVP_DMG_VARIATION { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PVPDmgVariation); } }
	public static float PVP_ATK_MIN_RATIO { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PvPAttackMinRatio); } }
	public static float PVP_ATK_MAX_RATIO { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PvPAttackMaxRatio); } }
	public static int PVP_DAY_PLAY_COUNT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.PVP_Count); } }

	public static float SUCCESS_ATTACK_PER { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.SuccessAttackPer); } }
	public static float SUCCESS_ATTACK_PER_LIMIT { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.SuccessAttackPerLimit); } }

	public static Color GetPVPRankColor(int _rank) {
		switch (_rank) {
			case 1: return Utile_Class.GetCodeColor("#BE7C65");
			case 2: return Utile_Class.GetCodeColor("#444554");
			case 3: return Utile_Class.GetCodeColor("#836430");
			case 4: return Utile_Class.GetCodeColor("#202B44");
			case 5: return Utile_Class.GetCodeColor("#451A55");
			case 6: return Utile_Class.GetCodeColor("#F6DA8F");
			case 7: return Utile_Class.GetCodeColor("#F6DA8F");
			default: return Utile_Class.GetCodeColor("#BE7C65");
		}
	}
	public static Sprite GetPVPPosIcon(PVPPosType _type) {
		switch (_type) {
			case PVPPosType.Combat: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Pos_Atk_White", "png");
			case PVPPosType.Supporter: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Pos_Sup_White", "png");
			default: return null;
		}
	}
	public static Sprite GetPVPEquipAtkIcon(PVPEquipAtkType _type) {
		switch (_type) {
			case PVPEquipAtkType.Through: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_A_1", "png"); 
			case PVPEquipAtkType.Cut: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_A_2", "png"); 
			case PVPEquipAtkType.Shock: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_A_3", "png"); 
			default:  return null;
		}
	}
	public static Sprite GetPVPEquipDefIcon(PVPArmorType _type) {
		switch (_type) {
			case PVPArmorType.LightArmor: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_D_1", "png"); 
			case PVPArmorType.Leather: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_D_2", "png"); 
			case PVPArmorType.Fabric: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_D_3", "png"); 
			default: return null;
		}
	}
	public static Sprite GetPVPSkillIcon(PVPSkillType _type) {
		switch (_type) {
			case PVPSkillType.Combat: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_D_1", "png");
			case PVPSkillType.LowStatus: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_D_2", "png");
			case PVPSkillType.Status: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_D_3", "png");
			case PVPSkillType.StatusBuff: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_A_1", "png");
			case PVPSkillType.TwoStat: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Type_A_2", "png");
			default: return null;
		}
	}
	public static string GetPVPPosName(PVPPosType _type) {
		switch (_type) {
			case PVPPosType.Combat: return MainMng.Instance.m_ToolData.GetString(10057);
			case PVPPosType.Supporter: return MainMng.Instance.m_ToolData.GetString(10058);
			default: return string.Empty;
		}
	}
	public static string GetPVPEquipAtkName(PVPEquipAtkType _type) {
		switch (_type) {
			case PVPEquipAtkType.Through: return MainMng.Instance.m_ToolData.GetString(10059);
			case PVPEquipAtkType.Cut: return MainMng.Instance.m_ToolData.GetString(10060);
			case PVPEquipAtkType.Shock: return MainMng.Instance.m_ToolData.GetString(10061);
			default: return string.Empty;
		}
	}
	public static string GetPVPEquipDefName(PVPArmorType _type) {
		switch (_type) {
			case PVPArmorType.LightArmor: return MainMng.Instance.m_ToolData.GetString(10062);
			case PVPArmorType.Leather: return MainMng.Instance.m_ToolData.GetString(10063);
			case PVPArmorType.Fabric: return MainMng.Instance.m_ToolData.GetString(10064);
			default: return string.Empty;
		}
	}
	/// <summary> 시작 캐릭터 </summary>
	public static int START_CHARACTER(int _pos) {
		switch (_pos) {
			case 0: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.StartChar1);
			case 1: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.StartChar2);
			case 2: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.StartChar3);
		}
		return 0;
	}
	/// <summary> 4번째 덱 언락 클리어 스테이지 </summary>
	public static int DECKSLOT4_UNLOCK_STAGE { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SlotUnlock1); } }
	/// <summary> 5번째 덱 언락 클리어 스테이지 </summary>
	public static int DECKSLOT5_UNLOCK_STAGE { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SlotUnlock2); } }
	/// <summary> 스테이지 듀얼 전투당 감소 포만감 </summary>
	public static int REDUCTION_SAT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReductionSat); } }

	/// <summary> 아이템 등급업 레벨 </summary>
	public static int ITEM_EXP_MONEY(long exp) {
		return Mathf.RoundToInt(exp * MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.Item_Exp_Money));
	}

	/// <summary> 아이템 옵션 개수 </summary>
	public static int ITEM_OPTION_CNT(int _grade) {
		if (_grade < MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.RandomStat_Grade1)) return 0;
		else if (_grade < MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.RandomStat_Grade2)) return 1;
		else if (_grade < MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.RandomStat_Grade3)) return 2;
		else return 3;
	}
	/// <summary> 아이템 최대 레벨 </summary>
	public static int ITEM_MAX_LV(Grade _grade) {
		return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.EquipMaxLevel);
	}

	/// <summary> 최대 등급 레벨 0~9 </summary>
	public const int MAX_ITEM_GRADE = 10;
	/// <summary> DNA 최대 등급 </summary>
	public const int MAX_DNA_GRADE = 5;
	public const int MAX_DNA_LV = 6;

	/// <summary> 아이템 등급별 최대 레벨 </summary>
	public static int ITEM_GRADE_MAX_LV(int _grade) {
		ConfigType type = (ConfigType)Enum.Parse(typeof(ConfigType), string.Format("EquipGrade{0}MaxLevel", _grade));
		return MainMng.Instance.m_ToolData.GetConfig_Int32(type);
	}
	public static int ITEM_GRADEUP_MONEY(int _grade) {
		ConfigType type = (ConfigType)Enum.Parse(typeof(ConfigType), string.Format("Grade{0}UpPrice", _grade));
		return MainMng.Instance.m_ToolData.GetConfig_Int32(type);
	}

	/// <summary> 재조립시 레벨 1당 붙는 달러 </summary>
	public static int REASSEMBLTY_PRICE { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReassemblyPrice); } }

	/// <summary> 장비 옵션 잠금 해제 확률</summary>
	public static float EQUIP_OPTIONSLOT_OPENPROB(int _pos) {
		return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.Slot1OpenProb + _pos);
	}
	// TODO FIX 글로벌 웨이트 적용
	public static int ZOMBIE_KEEP_MAX { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ZombieKeepMax); } }
	/// <summary> 좀비 키우기 시작 슬롯 수 </summary>
	public static int START_ZOMBIE_SLOT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.StartZombieSlot); } }
	/// <summary> 좀비 실험실 오픈스테이지 </summary>
	public static int ZOMBIE_CAGE_OPEN => MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ZombieCageOpen);
	/// <summary> 최대 좀비 케이지 수 </summary>
	public static int ZOMBIE_CAGE_MAX => MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ZombieCageMax);
	public static int ZOMBIE_CAGE_INSIZE = 5;
	public static int RNA_PRODUCTION_MAX_TIME { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.RNAProductionMaxTime); } }
	/// <summary> 노트 전투 가드 최대 수치 </summary>
	public static int BATTLE_GUARD_MAX => MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.BattleGuardMax);
	/// <summary> 노트 전투 가드 0.1초당 회복 수치 </summary>
	public static int BATTLE_GUARD_AUTORECV_TICK => MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.AutoGuardRecvTickRatio);
	/// <summary> 컬러코드 ex)#ffffff 로 크기 비교해서 컬러 반환 </summary>
	public static Color GetUpDownStrColor(double _Now, double _Need) {
		return GetUpDownStrColor(_Now, _Need, "#D2533C", "#498E41");
	}

	/// <summary> 컬러코드 ex)#ffffff 로 크기 비교해서 컬러 반환 </summary>
	public static Color GetUpDownStrColor(double _Now, double _Need, string _Down, string _UP) {
		return Utile_Class.GetCodeColor(_Now < _Need ? _Down : _UP);
	}

	public static Color GradeColor(int _grade) {
		return Utile_Class.GetCodeColor(GradeColorCode(_grade));
	}
	public static string GradeColorCode(int _grade) {
		switch (_grade) {
			case 1: return "#969A89";
			case 2: return "#68965E";
			case 3: return "#6290C3";
			case 4: return "#9857AD";
			case 5: return "#E25239";
			case 6: return "#E25239";
			case 7: return "#E25239";
			case 8: return "#E25239";
			case 9: return "#E25239";
			case 10: return "#E25239";
		}
		return "#969A89";
	}
	public static string GradeName(int _grade) {
		return string.Format("{0} {1}", MainMng.Instance.m_ToolData.GetString(274), _grade);
	}
	public static KeyValuePair<int, string> StatGradeName(float _ratio) {
		int idx = 0;
		int grade = 0;
		if (_ratio < 0.2f) {
			idx = 950;
			grade = 1;
		}
		else if (0.2f <= _ratio && _ratio < 0.4f) {
			idx = 951;
			grade = 2;
		}
		else if (0.4f <= _ratio && _ratio < 0.6f) {
			idx = 952;
			grade = 3;
		}
		else if (0.6f <= _ratio && _ratio < 0.8f) {
			idx = 953;
			grade = 4;
		}
		else if (0.8f <= _ratio) {
			idx = 954;
			grade = 5;
		}
		return new KeyValuePair<int, string>( grade, MainMng.Instance.m_ToolData.GetString(idx));
	}
	/// <summary> 1 기본 </summary>
	public static Sprite GradeFrame(ItemType _type, int _grade, int _idx = 0) {
		string name = string.Empty;
		switch (_type) {
			case ItemType.DNA:
			case ItemType.DNAMaterial:
				if(_idx == 4105) name = "Card/Frame/CardFrame_SuperRNA";
				else name = "Card/Frame/CardFrame_DNA";
				break;
			case ItemType.Zombie:
				name = "Card/Frame/CardFrame_Zombie";
				break;
			case ItemType.CharaterPiece:
				name = string.Format("Card/Frame/Frame_Charpiece_{0}", _grade);
				break;
			default:
				name = string.Format("Card/Frame/CardFrame2_{0}", _grade);
				break;
		}
		return MainMng.Instance.m_Utile.LoadImg(name, "png");
	}

	/// <summary> 1 기본 </summary>
	public static Sprite ItemGradeBG(ItemType _type, int _grade) {
		string name = string.Empty;
		switch (_type) {
			case ItemType.Zombie:
				name = "Card/Frame/BG_Zombie";
				break;
			default:
				name = string.Format("Card/Frame/BGGrade_Item_{0}", _grade);
				break;
		}
		return MainMng.Instance.m_Utile.LoadImg(name, "png");
	}
	public static Sprite GetInfoGradeBG(int _grade) {
		return MainMng.Instance.m_Utile.LoadImg(string.Format("UI/BG/BG_Popup_Grade_{0}", _grade), "png");
	}
	public static Color GradeFrameGlowColor(int _grade) {
		switch (_grade) {
			case 6: return Utile_Class.GetCodeColor("#FFE77C");
			case 7: return Utile_Class.GetCodeColor("#8AD132");
			case 8: return Utile_Class.GetCodeColor("#D083FF");
			case 9: return Utile_Class.GetCodeColor("#FF58AE");
			case 10: return Utile_Class.GetCodeColor("#FF5858");
			default: return Utile_Class.GetCodeColor("#FFE77C");
		}
	}
	/// <summary> 0:프레임 1:카운트배경 </summary>
	public static Color[] RNAFrameColor(int _idx) {
		Color[] color = new Color[3];
		switch (_idx) {
			case 4101:
				color[0] = Utile_Class.GetCodeColor("#FF4242");
				color[1] = Utile_Class.GetCodeColor("#A6291F");
				color[2] = Utile_Class.GetCodeColor("#B44137");
				break;
			case 4102:
				color[0] = Utile_Class.GetCodeColor("#729CFF");
				color[1] = Utile_Class.GetCodeColor("#3750C0");
				color[2] = Utile_Class.GetCodeColor("#376DC0");
				break;
			case 4103:
				color[0] = Utile_Class.GetCodeColor("#6DC56E");
				color[1] = Utile_Class.GetCodeColor("#2D7B3F");
				color[2] = Utile_Class.GetCodeColor("#428050");
				break;
			case 4104:
				color[0] = Utile_Class.GetCodeColor("#B266FF");
				color[1] = Utile_Class.GetCodeColor("#7A3587");
				color[2] = Utile_Class.GetCodeColor("#7F3F8C");
				break;
			case 4105:
				color[0] = Utile_Class.GetCodeColor("#FFFFFF");
				color[1] = Utile_Class.GetCodeColor("#FFFFFF");
				color[1].a = 110f / 225f;
				color[2] = Utile_Class.GetCodeColor("#737073");
				break;
		}
		return color;
	}
	public static Sprite CharFrame(int grade) {
		return MainMng.Instance.m_Utile.LoadImg(string.Format("Card/Frame/CharFrame_{0}", grade), "png");
	}
	public static Sprite CharOriginFrame(int _origingrade, int _grade) {
		return MainMng.Instance.m_Utile.LoadImg(string.Format("Card/Frame/CharFrame_Origin_{0}_{1}", _origingrade, _grade), "png");
	}
	public static Sprite CharBG(int grade) {
		return MainMng.Instance.m_Utile.LoadImg(string.Format("Card/Frame/BGGrade_{0}", grade), "png");
	}

	public static Sprite GetItemIcon(ItemType type) {
		switch (type) {
			case ItemType.Dollar: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_EmptyShell", "png");
			case ItemType.Exp: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Small_Exp", "png");
			case ItemType.Cash: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_GoldTeeth", "png");
			case ItemType.Energy: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Energy", "png");
			case ItemType.InvenPlus: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/JobIcon_Lawyer", "png");
			case ItemType.PVPCoin: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_PVP_Coin", "png");
			case ItemType.Guild_Coin: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Union_Coin", "png");
			case ItemType.Mileage: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_GatchaMileage", "png");
		}
		return null;
	}
	public static Sprite GetItemIcon(int _idx) {
		switch (_idx) {
			case 3000003: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Ticket_Gatcha_1", "png");
			case 3000004: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Ticket_Gatcha_2", "png");
		}
		return MainMng.Instance.TDATA.GetItemTable(_idx).GetItemImg();
	}
	public static Sprite GetGroupItemIcon(ItemType _type) {
		switch (_type) {
			case ItemType.Weapon:
			case ItemType.Blunt:
			case ItemType.Blade:
			case ItemType.Axe:
			case ItemType.Bow:
			case ItemType.Pistol:
			case ItemType.Shotgun:
			case ItemType.Rifle:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Weapon", "png");
			case ItemType.Helmet: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Helmet", "png");
			case ItemType.Costume: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Costume", "png");
			case ItemType.Shoes: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Shoes", "png");
			case ItemType.Accessory: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Accessory", "png");
			case ItemType.CharaterPiece: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_CharaterPiece", "png");
			case ItemType.ConsolidationMaterial: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_ConsolidationMaterial", "png");
			case ItemType.EquipmentMaterial: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_EquipmentMaterial", "png");
			case ItemType.Equip: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Equip", "png");
			case ItemType.ReassemblyMaterial: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_ReassemblyMaterial", "png");
			case ItemType.Zombie: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_Zombie", "png");
			case ItemType.DNA: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_DNA", "png");
			case ItemType.DNAMaterial: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Random_DNAMateral", "png");
			case ItemType.Etc: return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Box_1", "png");
			default: return null;
		}
	}
	public static string GetGroupItemName(ItemType _type) {
		switch (_type) {
			case ItemType.Weapon: return MainMng.Instance.m_ToolData.GetString(25);
			case ItemType.Helmet: return MainMng.Instance.m_ToolData.GetString(26);
			case ItemType.Costume: return MainMng.Instance.m_ToolData.GetString(27);
			case ItemType.Shoes: return MainMng.Instance.m_ToolData.GetString(28);
			case ItemType.Accessory: return MainMng.Instance.m_ToolData.GetString(29);
			case ItemType.CharaterPiece: return MainMng.Instance.m_ToolData.GetString(113);
			case ItemType.ConsolidationMaterial: return MainMng.Instance.m_ToolData.GetString(107);
			case ItemType.EquipmentMaterial: return MainMng.Instance.m_ToolData.GetString(830);
			case ItemType.Equip: return MainMng.Instance.m_ToolData.GetString(112);
			case ItemType.ReassemblyMaterial: return MainMng.Instance.m_ToolData.GetString(831);
			case ItemType.DNA: return MainMng.Instance.m_ToolData.GetString(340);
			case ItemType.DNAMaterial: return MainMng.Instance.m_ToolData.GetString(10065);
			case ItemType.Zombie: return MainMng.Instance.m_ToolData.GetString(425);
			case ItemType.Etc: return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30015);
		}
		return null;
	}
	public static string GetPriceTypeName(PriceType _type, int _idx = 0) {
		switch (_type) {
			case PriceType.AD:
			case PriceType.AD_AddTime:
			case PriceType.AD_InitTime:
				return MainMng.Instance.m_ToolData.GetString(5101);
			case PriceType.Time:
				return MainMng.Instance.m_ToolData.GetString(212);
			case PriceType.Cash:
				return MainMng.Instance.m_ToolData.GetItemTable(CASH_IDX).GetName();
			case PriceType.Money:
				return MainMng.Instance.m_ToolData.GetItemTable(DOLLAR_IDX).GetName();
			case PriceType.Pay:
				return null;
			case PriceType.Energy:
				return MainMng.Instance.m_ToolData.GetItemTable(ENERGY_IDX).GetName();
			case PriceType.Item:
				return MainMng.Instance.m_ToolData.GetItemTable(_idx).GetName();
			case PriceType.PVPCoin:
				return MainMng.Instance.m_ToolData.GetItemTable(PVPCOIN_IDX).GetName();
			case PriceType.GuildCoin:
				return MainMng.Instance.m_ToolData.GetItemTable(GUILDCOIN_IDX).GetName();
			case PriceType.Mileage:
				return MainMng.Instance.m_ToolData.GetItemTable(MILEAGE_IDX).GetName();
		}
		return null;
	}
	public static string GetDNAColorName(DNABGType _type) {
		switch (_type) {
			case DNABGType.None: return MainMng.Instance.m_ToolData.GetString(941);
			case DNABGType.Red: return MainMng.Instance.m_ToolData.GetString(942);
			case DNABGType.Blue: return MainMng.Instance.m_ToolData.GetString(943);
			case DNABGType.Green: return MainMng.Instance.m_ToolData.GetString(944);
			case DNABGType.Purple: return MainMng.Instance.m_ToolData.GetString(945);
		}
		return null;
	}
	/// <summary> 전투력 공식 보정 수치 </summary>
	public static float COMBAT_POWER_RATIO(StatType _type) {
		switch (_type) {
			case StatType.Men: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.MenRevision);
			case StatType.Hyg: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.HygRevision);
			case StatType.Sat: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.SatRevision);
			case StatType.HP: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.HPRevision);
			case StatType.Atk: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.AtkRevision);
			case StatType.Def: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.DefRevision);
			case StatType.Sta: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.StaRevision);
			case StatType.RecSta: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.StaRecRevision);
			case StatType.Guard: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BlockRevision);
			case StatType.Heal: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.HealRevision);
			case StatType.NormalNote: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.NormalNoteRevision);
			case StatType.SlashNote: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.SlashNoteRevision);
			case StatType.ComboNote: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ComboNoteRevision);
			case StatType.ChargeNote: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ChargeNoteRevision);
			case StatType.ChainNote: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ChainNoteRevision);
		}
		return 0;
	}
	public static float NOTE_DMG_RATIO(ENoteType _type) {
		switch (_type) {
			case ENoteType.Normal:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.NormalNoteDmg);
			case ENoteType.Slash:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.SlashNoteDmg);
			case ENoteType.Combo:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ComboNoteDmg);
			case ENoteType.Charge:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ChargeNoteDmg);
			case ENoteType.Chain:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ChainNoteDmg);
		}
		return 1f;
	}

	public static Sprite GetStatMark(EEnemyType type, int idx) {
		switch (type) {
			case EEnemyType.SatRefugee:
			case EEnemyType.SatInfectee:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiStat_03", "png");
			case EEnemyType.MenRefugee:
			case EEnemyType.MenInfectee:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiStat_01", "png");
			case EEnemyType.HygRefugee:
			case EEnemyType.HygInfectee:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiStat_02", "png");
			case EEnemyType.HpRefugee:
			case EEnemyType.HpInfectee:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiStat_04", "png");
			case EEnemyType.RandomRefugee:
			case EEnemyType.RandomInfectee:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiStat_05", "png");
			case EEnemyType.AllRefugee:
			case EEnemyType.Allinfectee:
				return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiStat_06", "png");
			case EEnemyType.MaterialRefugee:
				switch ((StageMaterialType)idx) {
					case StageMaterialType.Bullet:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_01", "png");
					case StageMaterialType.GunPowder:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_02", "png");
					case StageMaterialType.Medicine:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_03", "png");
					case StageMaterialType.Battery:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_04", "png");
					case StageMaterialType.Powder:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_05", "png");
					case StageMaterialType.Food:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_06", "png");
					case StageMaterialType.Alcohol:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_07", "png");
					case StageMaterialType.Herb:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_08", "png");
					case StageMaterialType.Gasoline:
						return MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_FugiMate_09", "png");
				}
				break;
		}
		return null;
	}
	public static float GetStat(int LV, float value) {
		float times = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPMagnification);
		// 갭차
		float gap = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPGap);
		// 기준 레벨
		float blv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPReferLevel);
		// 보정치
		float bcv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPCorrectionValue);
		// POWER([배율],Level /[갭차])*[기준레벨] *[보정치]
		return (blv + blv * (1 + times * LV) * Mathf.Pow(times + 1, LV / gap)) * bcv * value;
		//return Mathf.Pow(times, LV / gap) * blv * bcv * value;
	}

	public static int CONTENT_OPEN_IDX(ContentType _type, StageDifficultyType diff = StageDifficultyType.Normal) {
		var toolData = MainMng.Instance.m_ToolData;
		switch (_type) {
			case ContentType.Bank: return toolData.GetConfig_Int32(ConfigType.BankOpen);
			case ContentType.Academy: return toolData.GetConfig_Int32(ConfigType.AcademyOpen);
			case ContentType.University: return toolData.GetConfig_Int32(ConfigType.UniversityOpen);
			case ContentType.Tower: return toolData.GetConfig_Int32(ConfigType.TowerOpen);
			case ContentType.Cemetery: return toolData.GetConfig_Int32(ConfigType.CemeteryOpen);
			case ContentType.Factory: return toolData.GetConfig_Int32(ConfigType.FactoryOpen);
			case ContentType.Subway: return toolData.GetConfig_Int32(ConfigType.SubwayOpen);
			case ContentType.Store: return toolData.GetConfig_Int32(ConfigType.StoreOpen);
			case ContentType.Research: return toolData.GetConfig_Int32(ConfigType.ResearchOpen);
			case ContentType.Character: return toolData.GetConfig_Int32(ConfigType.CharacterOpen);
			case ContentType.Making: return toolData.GetConfig_Int32(ConfigType.MakingOpen);
			case ContentType.Explorer: return toolData.GetConfig_Int32(ConfigType.ExplorerOpen);
			case ContentType.ZombieFarm: return toolData.GetConfig_Int32(ConfigType.ZombieCageOpen);
			case ContentType.PDA: return 0;
			case ContentType.Serum: return toolData.GetConfig_Int32(ConfigType.SerumOpen);
			case ContentType.ShopSupplyBox: return toolData.GetConfig_Int32(ConfigType.ShopSupplyBoxOpen);
			case ContentType.CharDNA: return toolData.GetConfig_Int32(ConfigType.CharDNAOpen);
			case ContentType.PvP: return toolData.GetConfig_Int32(ConfigType.PvPOpen);
			case ContentType.Guild: return toolData.GetConfig_Int32(ConfigType.GuildOpen);
			case ContentType.Replay:
				switch(diff)
				{
				case StageDifficultyType.Hard: return toolData.GetConfig_Int32(ConfigType.ReplayOpen);
				case StageDifficultyType.Nightmare: return toolData.GetConfig_Int32(ConfigType.ReplayNightOpen);
				}
				return toolData.GetConfig_Int32(ConfigType.ReplayNightOpen);
		}
		return 0;
	}

	public static Sprite GetNationIcon(string code) {
		Sprite re = MainMng.Instance.m_Utile.LoadImg($"UI/Nation/Nation_{code}", "png");
		if (re == null) re = MainMng.Instance.m_Utile.LoadImg("UI/Nation/Nation_NONE", "png");
		return re;
	}
	public static int MAX_ENERGY { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.StageEnergyMax); } }
	public static int ENERGY_CHARGE_TIME { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.StageEnergyTime); } }
	public static int BASIC_AP { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.BasicAP); } }
	public static int ADVENTURE_BASE_COUNT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.AdventureBaseCount); } }
	public static int ADVENTURE_WAITING_RATIO { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.AdventureWaitingRatio); } }
	public static int THREEWAY_TUTO { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ThreeWayTuto); } }
	public static int GetDeckSlotCnt(int _idx) {
		if (DECKSLOT5_UNLOCK_STAGE < _idx) return 5;
		else if (DECKSLOT4_UNLOCK_STAGE < _idx) return 4;
		return 3;
	}
	///// <summary> 제작 오픈 스테이지 </summary>
	public static int CHAREQUIP_MAKING_OPEN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.PrivateEquipOpen); } }
	//MakingOpen,
	///// <summary> 연구 오픈 스테이지 </summary>
	/// <summary> 캐릭터 등급 겹쳐서 나왔을 때 영혼석 개수 </summary>
	public static int STAR_OVERLAP(int _grade) {
		return MainMng.Instance.m_ToolData.GetConfig_Int32((ConfigType)Enum.Parse(typeof(ConfigType), string.Format("StarOverlap{0}", _grade.ToString())));
	}
	public static int STAR_OVERLAP_Grade(int Cnt) {
		for (int i = 1; i < 11; i++) if (Cnt == STAR_OVERLAP(i)) return i;
		return 0;
	}

	public static int GetTimePrice(ContentType content, double _sec) {
		ConfigType type = ConfigType.TimePrice;
		switch (content) {
			case ContentType.Research: type = ConfigType.ResearchTimePrice; break;
			case ContentType.Making: type = ConfigType.ProductionTimePrice; break;
			case ContentType.Explorer: type = ConfigType.AdventureTimePrice; break;
		}
		long gap = (long)_sec + 59;
		return (int)(MainMng.Instance.m_ToolData.GetConfig_Int32(type) * Math.Max(1, (gap / 60)));
	}

	public static float CalcStatValue(StatType type, float stat, float times) {
		switch (type) {
			case StatType.Guard:
			case StatType.NormalNote:
			case StatType.SlashNote:
			case StatType.ComboNote:
			case StatType.ChargeNote:
			case StatType.ChainNote:
			case StatType.MenDecreaseDef:
			case StatType.HygDecreaseDef:
			case StatType.SatDecreaseDef:
			case StatType.Critical:
			case StatType.CriticalDmg:
			case StatType.HeadShot:
				return stat + times;
		}
		return stat * (1f + times);
	}

	/// <summary> 나이트메어 오픈 스테이지 </summary>
	public static int HARD_OPEN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.HardOpen); } }
	/// <summary> 아포칼립스 오픈 스테이지 </summary>
	public static int NIGHTMARE_OPEN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.NightmareOpen); } }
	/// <summary> 난이도별 스테이지에서 전투 시 감소되는 포만도 수치 </summary>
	public static int STGBATTLE_REDUCTION_SAT(int _difficulty) {
		switch ((StageDifficultyType)_difficulty) {
			case StageDifficultyType.Normal: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReductionSat);
			case StageDifficultyType.Hard: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.HardReductionSat);
			case StageDifficultyType.Nightmare: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.NightmareReductionSat);
		}
		return 0;
	}
	/// <summary> 난이도 스테이지 턴 당 자동 체력 회복값 </summary>
	public static float HPRECV_TURNRATIO(int _difficulty) {
		switch (_difficulty) {
			case 0: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.AutoHPRecvTurnRatio);
			case 1:
			case 2: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.HardNightAutoHPRecvTurnRatio);
			default: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.AutoHPRecvTurnRatio);
		}
	}
	/// <summary> 난이도 스테이지 노트전투 0.5초 당 자동 체력 회복값 </summary>
	public static float HPRECV_TICKRATIO(int _diffculty) {
		switch (_diffculty) {
			case 0:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.AutoHPRecvTickRatio);
			case 1:
			case 2:
				return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.HardNightAutoHPRecvTickRatio);
			default: return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.AutoHPRecvTickRatio);
		}
	}
	/// <summary> 전투보상 공용 그룹 아이디</summary>
	public static int BATTLE_REWARD_COMMON_GID { get { return 100000001; } }
	public static int BATTLE_REWARD_COMMON2_GID { get { return 100000002; } }
	/// <summary> 취소 가능 보상상자 summary>
	public static int ITEM_REWARDBOX_CANCLE_IDX { get { return 40000055; } }
	/// <summary> 취소 불가능 보상상자 summary>
	public static int ITEM_REWARDBOX_NOTCANCLE_IDX { get { return 40000056; } }

	/// <summary> 캐릭터 스토리 오픈된 슬롯(해당 슬롯까지만 오픈됨) </summary>
	public static int CHAR_OPEN_STORY_SLOT(int Grade) {
		int[] cnt = { MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UnlockBehindSlotGrade01)
				, MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UnlockBehindSlotGrade02)
				, MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UnlockBehindSlotGrade03)
				, MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UnlockBehindSlotGrade04)
				, MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UnlockBehindSlotGrade05)
		};
		for (int i = cnt.Length - 1; i > -1; i--) {
			if (cnt[i] <= Grade) return i;
		}
		return 0;
	}

	/// <summary> 캐릭터 스토리 캐시 보상 개수 </summary>
	public static int CHAR_OPEN_STORY_REWARD_CNT(int Slot) {
		ConfigType type = (ConfigType)Enum.Parse(typeof(ConfigType), string.Format("RewardBehindStory{0:D2}", Slot + 1));
		return MainMng.Instance.m_ToolData.GetConfig_Int32(type);
	}
	/// <summary> 캐릭터 스토리 캐시 보상 개수 </summary>
	public static int CHAR_OPEN_STORY_GRADE(int Slot) {
		ConfigType type = (ConfigType)Enum.Parse(typeof(ConfigType), string.Format("UnlockBehindSlotGrade{0:D2}", Slot + 1));
		return MainMng.Instance.m_ToolData.GetConfig_Int32(type);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 대전
	public static readonly float[] NOTE_SCALE = { 0.275f, 0.35f, 0.425f };

	/// <summary> 회피 거리</summary>
	public static readonly float BATTLE_EVA_DIS = 2f;
	/// <summary> 회피시 스태미나 소모량  </summary>
	public static int EVA_STAMINA_VALUE { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.EVA_STAMINA_VALUE); } }
	/// <summary> 회피 유지시 스태미나 소모량  </summary>
	public static int EVA_STAMINA_KEEP_VALUE { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.EVA_STAMINA_KEEP_VALUE); } }
	public static float BATTLE_DEF_DMG_RATIO { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.GuardDamageRatio); } }
	/// <summary> NPC간 전투 데미지 증가 </summary>
	public static int NpctoNpcDmg { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.NpctoNpcDmg); } }

	public static int GetDamage(int Atk, int AtkLV, int Def, float Times, ENoteHitState noteHitState) {
		if (Def < 1) Def = 1;
		float max = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.MaxDefRate);
		float bpmag = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPMagnification);
		float bpgap = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPGap);
		float bprflv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPReferLevel);
		float defconst = 5f;
		float defvalue = 0.5f;
		float defratio = MainMng.Instance.m_ToolData.GetEnemyLevelTable(AtkLV).GetStat(EEnemyStat.DEF);
		
		double Damage = (double)Atk * (1f - Mathf.Min(max, (float)Def / (float)((bprflv + bprflv * (1f + bpmag * AtkLV) * Mathf.Pow(bpmag + 1, AtkLV / bpgap)) * defvalue * defconst * defratio + Def)));
		Damage *= Times;

		return Math.Max(1, (int)(Damage * Utile_Class.Get_RandomStatic(0.95f, 1.05f)));
	}
	/// <summary> pvp 데미지 경감률</summary>
	public static float GetPVPDmgDefRatio(float _atkerlv, float _targetdef) {
		float max = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.MaxDefRate);
		float bpmag = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPMagnification);
		float bpgap = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPGap);
		float bprflv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPReferLevel);
		float defconst = 2f;

		return Mathf.Min(max, _targetdef / (float)(bprflv + bprflv * (1f + bpmag * _atkerlv) * Mathf.Pow(bpmag + 1, _atkerlv / bpgap)) * defconst * PVP_DEF_RATE + _targetdef);
	}
	public static float GetPVPHeal(float _heal, int _lv) {

		float bpmag = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPMagnification);
		float bpgap = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPGap);
		float bprflv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPReferLevel);
		float healrate = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.PVPHealRate);
		
		return _heal / (bprflv + bprflv * (1f + bpmag * _lv) * Mathf.Pow(bpmag + 1, _lv / bpgap)) * 2 * healrate + _heal;
	}
	public static float GetPassiveStat(float _val, int _lv) {
		float bpmag = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPMagnification);
		float bpgap = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPGap);
		float bprflv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPReferLevel);

		return (bprflv + bprflv * (1f + bpmag * _lv) * Mathf.Pow(bpmag + 1, _lv / bpgap)) * _val;
	}
	public static float SAFE_BP_GAP { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.SafeBPGap); } }
	public static float RISK_BP_GAP { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.RiskBPGap); } }
	public static float DANGER_BP_GAP { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.DangerBPGap); } }
	public static float GetCPDmgRatio(bool _isplayer, bool _pvp = false, bool _precal = false) {
		if (MainMng.Instance.STAGEINFO.m_StageContentType == StageContentType.Subway) return 1f;
		float stagecp = MainMng.Instance.STAGEINFO.m_TStage.m_RecommandCP;
		if (stagecp == 0) return 1f;
		float playercp = MainMng.Instance.USERINFO.m_PlayDeck.GetCombatPower(_pvp, _precal);
		float ratio = (playercp - stagecp) / stagecp;
		var tdata = MainMng.Instance.TDATA.GetDamageAdjustmentTable(ratio);
		if (tdata == null) return 1f;
		return 1f + (_isplayer ? tdata.Ratio[1] : tdata.Ratio[0]);
	}
	public static int GetNeedCombatPower(int _lv) {
		// 배율
		float mag = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPMagnification);
		// 갭차
		float gap = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPGap);
		// 기준 레벨
		float blv = MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.BPReferLevel);
		// POWER([배율],Level /[갭차])*[기준레벨] *[보정치]
		
		return Mathf.RoundToInt((blv + blv * (1f + mag * _lv) * Mathf.Pow(mag + 1, _lv / gap)) * MainMng.Instance.m_ToolData.GetEnemyLevelTable(_lv).GetStat(EEnemyStat.HP) * 20);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 상태이상
	/// <summary> 공포 : 전투 발생 시 적의 레벨 5증가(랜덤으로 결정된 레벨에 +5) </summary>
	public static readonly int Fear = 5;

	public static Vector3 GetBattleDirPos(EBattleDir dir, float x, float z) {
		switch (dir) {
			case EBattleDir.Left: return new Vector3(-x, 0f, z);
			case EBattleDir.Right: return new Vector3(x, 0f, z);
		}
		return new Vector3(0f, 0f, z);
	}

	public static float CalcValue(float Base, float Add, float Per) {
		return Base * Per + Add;
	}

	public static float GetBurnProp() {
		return 1f;
	}
	/// <summary> 유저 활동 더미 캐릭터 확률</summary>
	public static int USERACTIVITY_CHAR_PROB { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.Alarm_Character_Get); } }
	/// <summary> 유저 활동 더미 전용 장비 확률</summary>
	public static int USERACTIVITY_SETEQUIP_PROB { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.Alarm_Item_Get); } }
	/// <summary> 유저 활동 더미 좀비 확률</summary>
	public static int USERACTIVITY_ZOMBIE_PROB { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.Alarm_Zombie_GradeUp); } }
	/// <summary> 혈청 블럭 최대 수 </summary>
	public static readonly int SERUM_MAXBLOCK = 10;
	/// <summary> 이어하기시 생존스텟 회복 퍼센트 </summary>
	public static float CONTINUE_STATRECV { get { return MainMng.Instance.m_ToolData.GetConfig_Float(ConfigType.ContinueStatRecv); } }
	/// <summary> 이어하기시 턴 추가 </summary>
	public static int CONTINUE_LIMITTURNPLUS { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ContinueLimitTurnPlus); } }
	/// <summary> 이어하기시 제한시간 추가</summary>
	public static int CONTINUE_TIMEPLUS { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ContinueTimePlus); } }
	/// <summary> 이어하기 카운트다운 </summary>
	public static readonly float CONTINUE_COUNTDOWN = 10f;
	public static int PERSONNELFILE_CHANGE_RATIO { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.PersonnelFileChangeRatio); } }
	public static int GET_DIFF_CONTINUEMAX(int _diff) {
		//if (MainMng.Instance.USERINFO.m_ShopInfo.IsPassBuy()) return MainMng.Instance.USERINFO.GetItemCount(CONTINUETICKET_IDX);
		//else 
		if (_diff == 0) return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.NorContinuMax);
		else if (_diff == 1) return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.HardContinuMax);
		else return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.NightContinuMax);
	}
	public static int UNLOCK_DOUBLE_SPEED { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.UnlockDoubleSpeed); } }

	public class DebuffCardInfo {
		public StageCardType Type;
		public Sprite Icon;
		public string Name;
		public string Desc;
	}

	public static DebuffCardInfo GET_DEBUFFCARDINFO(StageCardType _type) {
		DebuffCardInfo info = new DebuffCardInfo();
		info.Type = _type;
		switch (_type) {
			case StageCardType.ConMergeSlotDown:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_06", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30191);
				info.Desc = GetValName(40101, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ConMergeSlotDown));//71008
				break;
			case StageCardType.MaterialCountDown:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_07", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30192);
				info.Desc = GetValName(40102, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.MaterialCountDown));//71009
				break;
			case StageCardType.Wound:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_08", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30193);
				info.Desc = GetValName(40103, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.Wound));//71010
				break;
			case StageCardType.ConActiveAP:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_09", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30194);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 71011);
				break;
			case StageCardType.DarkTurn:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_10", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30195);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40105);
				break;
			case StageCardType.ConActiveSkillLimit:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_11", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30196);
				info.Desc = GetValName(40106, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ConActiveSkillLimit));//71013
				break;
			case StageCardType.ApPlus:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_12", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30197);
				info.Desc = GetValName(40107, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ApPlus));//71014
				break;
			case StageCardType.RandomMaterial:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_13", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30198);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40108);
				break;
			case StageCardType.HateBuff:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_14", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30199);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40109);
				break;
			case StageCardType.ConRandomChoice:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_15", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30200);
				info.Desc = GetValName(40110, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ConRandomChoice));//71017
				break;
			case StageCardType.MergeDelete:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_16", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30201);
				info.Desc = GetValName(40111, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.MergeDelete));//71018
				break;
			case StageCardType.ConEnemyTeamwork:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_17", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30202);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40112);
				break;
			case StageCardType.ConSkipTurn:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_18", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30203);
				info.Desc = GetValName(40113, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ConSkipTurn));//71020
				break;
			case StageCardType.ConStageCardLock:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_19", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30204);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 71021);
				break;
			case StageCardType.ConRewardCardLock:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_20", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30205);
				info.Desc = GetValName(40115, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ConRewardCardLock));//71022
				break;
			case StageCardType.ConBlindRewardInfo:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_21", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30206);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40116);
				break;
			case StageCardType.ConKnockDownChar:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_22", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30207);
				info.Desc = GetValName(40117, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.ConKnockDownChar));//71024
				break;
			case StageCardType.MergeFailChance:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_23", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30208);
				info.Desc = GetValName(40118, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.MergeFailChance));//71025
				break;
			case StageCardType.SkillHp:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_24", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30209);
				info.Desc = GetValName(40119, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.SkillHp));//71026
				break;
			case StageCardType.SkillStatus:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_25", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 30210);
				info.Desc = GetValName(40120, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.SkillStatus));//71027
				break;
			case StageCardType.MoreMaterial:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_26", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34301);
				info.Desc = GetValName(40131, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.MoreMaterial));
				break;
			case StageCardType.NoHpBar:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_27", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34302);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40132);
				break;
			case StageCardType.EnemyProbUp:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_28", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34303);
				info.Desc = GetValName(40133, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.EnemyProbUp));
				break;
			case StageCardType.NoAutoHeal:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_29", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34304);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40134);
				break;
			case StageCardType.TurnAllStatus:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_30", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34305);
				info.Desc = GetValName(40135, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.TurnAllStatus));
				break;
			case StageCardType.TurnStatusMen:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_31", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34306);
				info.Desc = GetValName(40136, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusMen));
				break;
			case StageCardType.TurnStatusHyg:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_32", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34307);
				info.Desc = GetValName(40137, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusHyg));
				break;
			case StageCardType.TurnStatusSat:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_33", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34308);
				info.Desc = GetValName(40138, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusSat));
				break;
			case StageCardType.TurnStatusHp:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_34", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34309);
				info.Desc = GetValName(40139, MainMng.Instance.STAGE_USERINFO.GetBuffValue(StageCardType.TurnStatusHp));
				break;
			case StageCardType.GoEnemy:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_35", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34310);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40140);
				break;
			case StageCardType.ConDeadly:
				info.Icon = MainMng.Instance.m_Utile.LoadImg("UI/Icon/Icon_Gimmik_36", "png");
				info.Name = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 34311);
				info.Desc = MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 40141);
				break;
		}
		return info;
	}
	public static string GetValName(int idx, float value, ToolData.StringTalbe type = ToolData.StringTalbe.Etc) {
		return string.Format(MainMng.Instance.m_ToolData.GetString(type, idx), Mathf.RoundToInt(value * 100f), Mathf.RoundToInt(value));
	}
	public class StagePlayTypeInfo
	{
		public Sprite m_Icon;
		public string m_Name;
		public string m_Desc;
	}

	public static StagePlayTypeInfo GetStagePlayTypeInfo(PlayType _type) {
		StagePlayTypeInfo info = new StagePlayTypeInfo();
		switch (_type) {
			case PlayType.FieldAirstrike:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_01", "png");
				break;
			case PlayType.NoCool:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_02", "png");
				break;
			case PlayType.TurmoilCount:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_03", "png");
				break;
			case PlayType.StreetLight:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_04", "png");
				break;
			case PlayType.FireSpread:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_05", "png");
				break;
			case PlayType.CardShuffle:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_37", "png");
				break;
			case PlayType.CardLock:
			case PlayType.EasyCardLock:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_19", "png");
				break;
			case PlayType.CardRandomLock:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_20", "png");
				break;
			case PlayType.Blind:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_21", "png");
				break;
			case PlayType.RandomCharOut:
			case PlayType.HighCharOut:
			case PlayType.LowCharOut:
			case PlayType.BanActive:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_22", "png");
				break;
			case PlayType.APRecvZero:
				info.m_Icon = MainMng.Instance.UTILE.LoadImg("UI/Icon/Icon_Gimmik_09", "png");
				break;
		}
		switch (_type) {
			case PlayType.FieldAirstrike:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70001);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71001);
				break;
			case PlayType.NoCool:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70002);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71002);
				break;
			case PlayType.StreetLight:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71004);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71005);
				break;
			case PlayType.FireSpread:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70004);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71006);
				break;
			case PlayType.TurmoilCount:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70003);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71003);
				break;
			case PlayType.APRecvZero:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70005);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71007);
				break;
			case PlayType.CardShuffle:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70006);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71028);
				break;
			case PlayType.CardLock:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70007);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71021);
				break;
			case PlayType.EasyCardLock:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70008);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71029);
				break;
			case PlayType.CardRandomLock:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70009);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71022);
				break;
			case PlayType.Blind:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70010);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71030);
				break;
			case PlayType.RandomCharOut:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70011);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71031);
				break;
			case PlayType.HighCharOut:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70012);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71032);
				break;
			case PlayType.LowCharOut:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70013);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71033);
				break;
			case PlayType.BanActive:
				info.m_Name = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 70011);
				info.m_Desc = MainMng.Instance.TDATA.GetString(ToolData.StringTalbe.Etc, 71034);
				break;
		}
		return info;
	}
	public static Sprite GetAreaIcon(StageCardType _cardtype) {
		SkillAreaType type = SkillAreaType.None;
		switch (_cardtype) {
			/// <summary> 선택한 카드 1장에만 효과 적용 </summary>
			case StageCardType.ShockBomb:
			case StageCardType.ThrowExtin:
			case StageCardType.FireBomb:
			case StageCardType.Paralysis:
			case StageCardType.Drill:
				type = SkillAreaType.Area01; break;
			/// <summary> 선택한 카드를 중심으로 3x3 영역에 효과 적용 </summary>
			case StageCardType.Grenade:
			case StageCardType.LightStick:
			case StageCardType.PowderExtin:
			case StageCardType.SmokeBomb:
			case StageCardType.FireGun:
			case StageCardType.RandomAtk:
			case StageCardType.Explosion:
			case StageCardType.StopCard:
			case StageCardType.PlusMate:
			case StageCardType.DownLevel:
			case StageCardType.TimeBomb:
				type = SkillAreaType.Area05; break;
			/// <summary> 선택한 카드를 중심으로 5x5 영역에 효과 적용 </summary>
			case StageCardType.StarShell:
			case StageCardType.PowderBomb:
			case StageCardType.NapalmBomb:
			case StageCardType.CardPull:
			case StageCardType.AllShuffle:
			case StageCardType.AllConversion:
				type = SkillAreaType.Area06; break;
			/// <summary> 선택한 카드가 배치된 행에 효과 적용 </summary>
			case StageCardType.Dynamite:
				type = SkillAreaType.Area09; break;
			/// <summary> 선택한 카드가 배치된 행과 위, 아래 행에 효과 적용 </summary>
			case StageCardType.AirStrike:
				type = SkillAreaType.Area19; break;
			/// <summary> 선택한 카드가 배치된 열에 효과 적용 </summary>
			case StageCardType.FlashLight:
				type = SkillAreaType.Area13; break;
			/// <summary> 플레이어 캐릭터 기준 2x3 영역에 효과 적용 </summary>
			case StageCardType.Shotgun:
				type = SkillAreaType.Area21; break;
			/// <summary> 선택한 타겟 하나와 하나 위 카드에 효과 적용 </summary>
			case StageCardType.Sniping:
				type = SkillAreaType.Area22; break;
			/// <summary> 플레이어 캐릭터 기준 5x5 영역에 효과 적용 </summary>
			case StageCardType.MachineGun:
			case StageCardType.C4:
				type = SkillAreaType.Area25; break;
			default:
				break;
		}
		return GetAreaIcon(type);
	}
	public static Sprite GetAreaIcon(SkillAreaType _areatype) {
		return MainMng.Instance.m_Utile.LoadImg(string.Format("UI/Icon/Skill_{0}", _areatype.ToString()), "png");
	}
	public static int GetSelectivePickupOpenCnt(int _stgidx) {
		if (MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen4) <= _stgidx) return 4;
		else if (MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen3) <= _stgidx) return 3;
		else if (MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen2) <= _stgidx) return 2;
		else if (MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen1) <= _stgidx) return 1;
		else return 0;
	}
	public static int GetSelectivePickupOpenStage(int _pos) {
		switch (_pos) {
			case 0: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen1);
			case 1: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen2);
			case 2: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen3);
			case 3: return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen4);
		}
		return 0;
	}
	public static bool IS_PickupOpenStage(int _idx) {
		if (_idx == MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen1)) return true;
		else if (_idx == MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen2)) return true;
		else if (_idx == MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen3)) return true;
		else if (_idx == MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.SelectivePickupOpen4)) return true;
		else return false;
	}
	/// <summary> 현 챕터에서 몇 챕터 이전까지 뽑을지 </summary>
	public static int REPLAY_CHAPTER { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReplayChapter); } }
	public static int REPLAY_OPEN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReplayOpen); } }
	public static int REPLAY_HARD_OPEN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReplayHardOpen); } }
	public static int REPLAY_NIGHT_OPEN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReplayNightOpen); } }
	public static int REPLAY_OPEN_CNT { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.ReplayOpenCount); } }
	////////////////////////////////////////////////////////////////////////
	/// 이벤트
	public static int[] EVENT_10_ITEMIDX = new int[4] { 2001, 2002, 2003, 2004 };
	public static int EVENT_11_ITEMIDX = 1012;
	public static List<LS_Web.FAEventType> EVENT_LIST = new List<LS_Web.FAEventType>() { LS_Web.FAEventType.OpenEvent, LS_Web.FAEventType.Stage_Minigame, LS_Web.FAEventType.GrowUP };
	////////////////////////////////////////////////////////////////////////
	/// 튜토리얼
	/// <summary> 메인 화면에서 일정 시간 작동 없을 경우, Game start 강조 연출 출력될 마지막 스테이지 </summary>
	public static int TUTO_STARTBTN { get { return MainMng.Instance.m_ToolData.GetConfig_Int32(ConfigType.TutoStartButton); } }

	/// <summary> 랭킹 스트링 UIString Table 606 스트링 이용 </summary>
	/// <param name="rank">0 : -위, 1~ : 1위</param>
	public static string GetRank(int rank)
	{
		if (rank == 0) return string.Format(MainMng.Instance.m_ToolData.GetString(606), "-");
		return string.Format(MainMng.Instance.m_ToolData.GetString(606), Utile_Class.GetNationRankNum(MainMng.Instance.m_AppInfo.m_Language, rank));
	}
	
	public static string GetGuildJoinType(GuildJoinType type)
	{
		switch (type)
		{
		case GuildJoinType.Auto: return MainMng.Instance.m_ToolData.GetString(6017);
		case GuildJoinType.Approval: return MainMng.Instance.m_ToolData.GetString(6018);
		case GuildJoinType.Private: return MainMng.Instance.m_ToolData.GetString(6174);
		}
		return "";
	}
}
