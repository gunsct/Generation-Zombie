using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CharInfo : ClassMng
{
	public class EqDNA
	{
		public int Idx;
		public long Uid;
		public int Grade;
		public int Lv;
	}
	public int m_Idx;
	public long m_UID;
	[JsonIgnore] public TCharacterTable m_TData { get { return TDATA.GetCharacterTable(m_Idx); } }  //테이블 데이터
	[JsonIgnore] public long m_ExpMax { get { return TDATA.GetExpTable(m_LV).m_Exp; } }             //최대 경험치
	[JsonIgnore] public long m_LvMax { get { return Mathf.Min(TDATA.GetCharGradeTable(m_Idx, m_Grade).m_MaxLv, USERINFO.m_LV); } }             //랭크별 최대 레벨
	[JsonIgnore] public long m_StgLvLimit { get { return TDATA.GetCharGradeTable(m_Idx, BaseValue.CHAR_MAX_RANK).m_MaxLv; } }//스테이지에서 최대 레벨
	public long GradeMaxLv(int _grade) {
		return TDATA.GetCharGradeTable(m_Idx, Mathf.Min(_grade, BaseValue.CHAR_MAX_RANK)).m_MaxLv;
	}
	public int m_LV;																				//레벨
	public int m_Grade = 1;																			//캐릭터 등급, 모두 별1개 부터 시작
	public SkillInfo[] m_Skill = new SkillInfo[3];													//보유 스킬 0:액티브, 1/2:패시브

	public long[] m_EquipUID = new long[(int)EquipType.Max];										//장착 장비 UID, UserInfo의 m_Item 인덱스

	/// <summary> DNA슬롯 오픈 유무 </summary>
	public bool[] m_DNASlot = { true, true, true, false, false };
	/// <summary> 장착된 DNA정보 </summary>
	public long[] m_EqDNAUID = new long[5];                                                         //장착 DNA UID, UserInfo의 m_Item 인덱스
	[JsonIgnore] public Dictionary<DNABGType, int> m_EquipDNASetCnt = new Dictionary<DNABGType, int>();
	[JsonIgnore] public bool m_GetAlarm;

	/// <summary> 스토리 보상 개수 고정 5개 </summary>
	public bool[] Story = { false, false, false, false, false };

	public List<int> m_Serum = new List<int>();

	public int m_LockSerumBlockPos = 1;
	[JsonIgnore] public int m_CP;
	[JsonIgnore] public int m_PVPCP;
	[JsonIgnore] public Dictionary<StatType, float> m_Stat = new Dictionary<StatType, float>();
	[JsonIgnore] public Dictionary<StatType, float> m_PVPStat = new Dictionary<StatType, float>();
	/// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
	public CharInfo() { }

	public CharInfo(int Idx, long UID = 0, int _grade = 0, int _lv = 1) {
		m_Idx = Idx;
#if NOT_USE_NET
		if (UID == 0) UID = Utile_Class.GetUniqeID();
#endif
		m_UID = UID;
		if (_grade == 0) {//태생등급
			m_Grade = m_TData.m_Grade;
		}
		else
			m_Grade = _grade;
		m_LV = _lv;

		for (int i = 0; i < 3; i++) {
			m_Skill[i] = new SkillInfo();
			m_Skill[i].m_LV = 1;
			m_Skill[i].m_Idx = m_TData.m_SkillIdx[i];
		}

		m_GetAlarm = true;
	}

	public void SetDATA(LS_Web.RES_CHARINFO data) {
		m_Idx = data.Idx;
		m_Grade = data.Grade;
		m_LV = data.LV;
		for (int i = 0; i < 3; i++) {
			m_Skill[i] = new SkillInfo();
			m_Skill[i].m_LV = 1;// data.SkillLV[i];
			m_Skill[i].m_Idx = m_TData.m_SkillIdx[i];
		}

		m_EquipUID = data.EquipUID;
		m_DNASlot = data.DNASlot;
		m_EqDNAUID = data.DNA;
		CheckDNASetFX();

		m_Serum = data.Serum;
		m_LockSerumBlockPos = data.SPage;

		IS_SetEquip();
		m_CP = GetCombatPower();
		m_PVPCP = GetCombatPower(0, 0, true);
	}

	public bool ISPlayDeckLeader() {
		return USERINFO.m_PlayDeck.m_Char[1] == m_UID;
	}
	/// <summary> 레벨업 가능여부 </summary>
	public KeyValuePair<int, int> CheckLvUp(int _add) {
		if (m_LV >= m_LvMax) return new KeyValuePair<int, int>(0, 0);
		_add = Math.Min((int)m_LvMax - m_LV, _add);
		long[] expsum = new long[_add];
		long[] moneysum = new long[_add];
		for (int i = 0; i < _add; i++) {
			TExpTable expdata = TDATA.GetExpTable(m_LV + i);
			if(expdata == null) {
				expdata = null;
			}
			expsum[i] = expdata.m_Exp;
			if (i > 0) expsum[i] += expsum[i - 1];
			moneysum[i] = TDATA.GetExpTable(m_LV + i).m_Money;
			if (i > 0) moneysum[i] += moneysum[i - 1];
		}
		for(int i = 0; i < _add; i++) {
			if(USERINFO.m_Exp[(int)EXPType.Ingame] < expsum[i]) {
				return new KeyValuePair<int, int>(Math.Max(0, i - 1), 1);
			}
			else if (USERINFO.m_Money < moneysum[i]) {
				return new KeyValuePair<int, int>(Math.Max(0, i - 1), 2);
			}
		}
		//if (USERINFO.m_Exp[(int)EXPType.Ingame] < m_ExpMax) return false;
		//if (USERINFO.m_Money < TDATA.GetExpTable(m_LV).m_Money) return false;
		return new KeyValuePair<int, int>(_add, -1);
	}
	public void SetLvUp(int _add) {
		long[] cost = new long[2];
		for (int i = 0; i < _add; i++) {
			cost[0] += TDATA.GetExpTable(m_LV + i).m_Exp;
			cost[1] += TDATA.GetExpTable(m_LV + i).m_Money;
		}
		USERINFO.SetIngameExp(-cost[0]);
		USERINFO.ChangeMoney(-cost[1]);
		m_LV += _add;
		MAIN.Save_UserInfo();
	}
	public ItemInfo GetEquip(EquipType _type) {
		if (m_EquipUID[(int)_type] == 0) return null;
		return USERINFO.GetItem(m_EquipUID[(int)_type]);
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Skill
	// UI용
	public SkillType GetSkillType(int idx) {
		for (int i = 0; i < m_Skill.Length; i++) {
			if (m_Skill[i].m_Idx == idx) return (SkillType)i;
		}
		return SkillType.None;
	}

	public int GetSkillLVPos(SkillType type) {
		int pos = type == SkillType.Passive1 ? 1 : 0;
		return pos;
	}

	public int GetSkillLV(SkillType type) {
		return m_Skill[GetSkillLVPos(type)].m_LV;
	}

	public int GetSkillMaxLV(SkillType type) {
		return m_Skill[GetSkillLVPos(type)].m_TData.m_MaxLV;
	}

	public void SkillLVUP(SkillType type, int AddLV = 1) {
		int pos = GetSkillLVPos(type);
		m_Skill[pos].m_LV = Math.Min(m_Skill[pos].m_LV + AddLV, GetSkillMaxLV(type));
	}

	// 전투용
	public float GetSkillValue(SkillKind kind) {
		float value = 0f;

		for (int i = 0; i < 3; i++) {
			SkillType skilltype = (SkillType)i;
			TSkillTable skill = m_Skill[i].m_TData;
			if (skill == null) continue;
			if (skill.m_Kind != kind) continue;
			value += skill.GetValue(GetSkillLV(skilltype));
		}

		return value;
	}
	public float GetPassiveStatValue(StatType _type) {
		return m_TData.GetPassiveStatValue(_type, m_LV);
	}
	/// <summary> 착용장비가 전부 전용장비인지 여부 </summary>
	public bool IS_SetEquip() {
		// 서버 검색과 맞춤
		var cnt = m_EquipUID.Select(o => {
			ItemInfo info = USERINFO.GetItem(o);
			return info != null && info.m_TData != null && info.m_TData.m_Value == m_Idx;
		}).ToList().FindAll(r=>r == true);
		bool set =  cnt.Count >= 3;
		//!m_EquipUID.Any(o =>
		//{
		//	ItemInfo info = USERINFO.GetItem(o);
		//	return info == null || info.m_TData == null || info.m_TData.m_Value != m_Idx;
		//});
		m_Skill[0].m_Idx = m_TData.m_SkillIdx[(int)(set ? SkillType.SetActive : SkillType.Active)];
		return set;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stat
	/// <summary> 스탯 계산, 레벨, 등급 넣을 수 있음 뭐 디버프로 레벨 일시적으로 까이거나 하면 쓰기 </summary>
	bool IsAddStat(StatType _type)
	{

		switch (_type)
		{
		case StatType.NormalNote:
		case StatType.SlashNote:
		case StatType.ComboNote:
		case StatType.ChargeNote:
		case StatType.ChainNote:
		case StatType.Critical:
		case StatType.CriticalDmg:
		case StatType.MenDecreaseDef:
		case StatType.HygDecreaseDef:
		case StatType.SatDecreaseDef:
			return true;
		}
		return false;
	}
	public float GetStat(StatType _type, int _lv = 0, int _grade = 0, float times = 0, int _commonaddlv = 0, bool _pvp = false) {
		//		List<StatType> testratiostat = new List<StatType>() { StatType.HP, StatType.Atk, StatType.Def, StatType.Heal };

		//		float per = 1f;
		//		float add = 0;
		//		float baseratio = 1f;
		//		float basestat = 0f;
		//#if STAGE_TEST
		//		if (_type <= StatType.Sta || _type == StatType.Heal)
		//			if (testratiostat.Contains(_type)) baseratio += PlayerPrefs.GetFloat("STAGE_TEST_STATRATIO");
		//#endif
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		StringBuilder log = new StringBuilder(1024);

		//		log.Append($"charidx : {m_Idx} // StatType : {_type} // BaseRatio : {baseratio}");
		//#endif
		//		int lv = _lv == 0 ? m_LV : _lv;
		//		lv = Mathf.Clamp(lv + _commonaddlv, 1, (int)m_LvMax);
		//		int grade = _grade == 0 ? m_Grade : _grade;
		//		// 아이템 체크
		//		for (int j = m_EquipUID.Length - 1; j > -1; j--) {
		//			if (m_EquipUID[j] > 0) {
		//				ItemInfo item = USERINFO.GetItem(m_EquipUID[j]);
		//				if (item == null) continue;
		//				basestat += item.GetStat(_type, Mathf.Clamp(item.m_Lv + _commonaddlv, 1, item.m_MaxLV), this);
		//				if (IsAddStat(_type)) add += item.GetOptionValue(_type, this);//추가옵션이랑 세트옵션
		//				else per += item.GetOptionValue(_type, this);//추가옵션이랑 세트옵션
		//			}
		//		}
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n장비 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif
		//		//add += add_Eq;

		//		//랭크 계산
		//		if (IsAddStat(_type)) add += TDATA.GetCharGradeStatTable(grade).GetStatRatio(_type);
		//		else per += TDATA.GetCharGradeStatTable(grade).GetStatRatio(_type);
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n등급 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif

		//		basestat += TDATA.GetStatTable(m_Idx).GetStat(_type, lv) * baseratio;
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n레벨당 기본 스텟 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif
		//		//연구 계산
		//		switch (_type) {
		//			case StatType.HP: per += USERINFO.ResearchValue(ResearchEff.HealthMaxUp); break;
		//			case StatType.Atk: per += USERINFO.ResearchValue(ResearchEff.AtkUp); break;
		//			case StatType.Def: per += USERINFO.ResearchValue(ResearchEff.DefUp); break;
		//			case StatType.Heal: per += USERINFO.ResearchValue(ResearchEff.HealUp); break;
		//			case StatType.Men: per += USERINFO.ResearchValue(ResearchEff.MenMaxUp); break;
		//			case StatType.Hyg: per += USERINFO.ResearchValue(ResearchEff.HygMaxUp); break;
		//			case StatType.Sat: per += USERINFO.ResearchValue(ResearchEff.SatMaxUp); break;
		//			case StatType.MenDecreaseDef: add += USERINFO.ResearchValue(ResearchEff.MenDefUp); break;
		//			case StatType.HygDecreaseDef: add += USERINFO.ResearchValue(ResearchEff.HygDefUp); break;
		//			case StatType.SatDecreaseDef: add += USERINFO.ResearchValue(ResearchEff.SatDefUp); break;
		//			case StatType.Sta: per += USERINFO.ResearchValue(ResearchEff.EnergyMaxUp); break;
		//			case StatType.RecSta: per += USERINFO.ResearchValue(ResearchEff.EnergySpeedUp); break;
		//			case StatType.Guard: per += USERINFO.ResearchValue(ResearchEff.GuardMaxUp); break;
		//			case StatType.Speed: per += USERINFO.ResearchValue(ResearchEff.SpeedUp); break;
		//		}
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n연구(길드 포함 PVP관련 제거) 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif

		//		//DNA 버프
		//		switch (_type) {
		//			case StatType.Atk: per += GetDNABuff(OptionType.AtkUp); break;
		//			case StatType.Def: per += GetDNABuff(OptionType.DefUp); break;
		//			case StatType.Heal: per += GetDNABuff(OptionType.HealUp); break;
		//			case StatType.Men: per += GetDNABuff(OptionType.MenUp); break;
		//			case StatType.Hyg: per += GetDNABuff(OptionType.HygUp); break;
		//			case StatType.Sat: per += GetDNABuff(OptionType.SatUp); break;
		//			case StatType.Critical: per += GetDNABuff(OptionType.AttackingCriShoot); break;
		//		}
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\nDNA 버프 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif
		//		if (IsAddStat(_type)) {
		//			//DNA 레벨별 붙은 자체 스탯
		//			add += GetDNAVal(_type);
		//			//DNA 세트 효과
		//			add += GetDNASet(_type);
		//		}
		//		else {
		//			//DNA 레벨별 붙은 자체 스탯
		//			per += GetDNAVal(_type);
		//			//DNA 세트 효과
		//			per += GetDNASet(_type);
		//		}
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\nDNA 옵션 + 세트효과 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif

		//		//혈청 버프
		//		float perserum = USERINFO.GetAllSerum(_type, StatValType.Ratio) + GetSelfSerum(_type, StatValType.Ratio, SerumTargetType.Self);
		//		float absserum = USERINFO.GetAllSerum(_type, StatValType.ABS) + GetSelfSerum(_type, StatValType.ABS, SerumTargetType.Self);
		//		per += perserum;
		//		add += absserum;

		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n혈청 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//		log.Append($"\n\t ratio : {perserum} // abs : {absserum}");
		//#endif
		//		// 컬렉션(절대값)
		//		add += USERINFO.m_Collection.GetStatValue(_type);
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n컬렉션 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//#endif
		//		// 레벨합 스탯 보너스
		//		Dictionary<StatType, float> charlvbonus = USERINFO.GetCharLvStatBonus();
		//		if (charlvbonus.ContainsKey(_type)) add += charlvbonus[_type];
		//#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		//		log.Append($"\n레벨 보너스 계산 => BaseStat : {basestat} // Add : {add} // Per : {per}");
		//		log.Append($"\n결과 => {(basestat * per + add)}");
		//		if (_type <= StatType.Sta || _type == StatType.Heal)
		//			Utile_Class.DebugLog(log);
		//#endif

		//		return basestat * per + add;


		List<StatType> testratiostat = new List<StatType>() { StatType.HP, StatType.Atk, StatType.Def, StatType.Heal };
		int lv = _lv == 0 ? m_LV : _lv;
		//lv = Mathf.Clamp(lv + _commonaddlv, 1, (int)m_LvMax);
		int grade = _grade == 0 ? m_Grade : _grade;
		float basecharstat = 0f;
		float baseequip = 0f;
		float baseserum = 0f;
		float addall = 0f;
		float perchargrade = 0;
		float perresearch = 0f;
		float perequip = 0f;
		float basechargetbonus = 0f;
		float baselvbonus = 0f;
		float perdna = 0f;
		float baseratio = 1f;
#if STAGE_TEST
			if (_type <= StatType.Sta || _type == StatType.Heal)
				if (testratiostat.Contains(_type)) baseratio += PlayerPrefs.GetFloat("STAGE_TEST_STATRATIO");
#endif
		//캐릭터 기본
		basecharstat = TDATA.GetStatTable(m_Idx).GetStat(_type, lv) * baseratio;
		//캐릭터 등급
		if (IsAddStat(_type)) addall += TDATA.GetCharGradeStatTable(grade).GetStatRatio(_type);
		else perchargrade = TDATA.GetCharGradeStatTable(grade).GetStatRatio(_type);
		//캐릭터 레벨보너스
		Dictionary<StatType, float> lvbonus = USERINFO.GetCharLvStatBonus();
		if (lvbonus.ContainsKey(_type)) baselvbonus += lvbonus[_type];
		//장비
		for (int j = m_EquipUID.Length - 1; j > -1; j--) {
			if (m_EquipUID[j] > 0) {
				ItemInfo item = USERINFO.GetItem(m_EquipUID[j]);
				if (item == null) continue;
				baseequip += item.GetStat(_type, Mathf.Clamp(item.m_Lv + _commonaddlv, 1, item.m_MaxLV), this);
				if (IsAddStat(_type)) addall += item.GetOptionValue(_type, this);//추가옵션이랑 세트옵션
				else perequip += item.GetOptionValue(_type, this);//추가옵션이랑 세트옵션
			}
		}
		//DNA 버프
		switch (_type) {
			case StatType.Atk: perdna += GetDNABuff(OptionType.AtkUp); break;
			case StatType.Def: perdna += GetDNABuff(OptionType.DefUp); break;
			case StatType.Heal: perdna += GetDNABuff(OptionType.HealUp); break;
			case StatType.Men: perdna += GetDNABuff(OptionType.MenUp); break;
			case StatType.Hyg: perdna += GetDNABuff(OptionType.HygUp); break;
			case StatType.Sat: perdna += GetDNABuff(OptionType.SatUp); break;
			case StatType.Critical: perdna += GetDNABuff(OptionType.AttackingCriShoot); break;
		}
		if (IsAddStat(_type)) {
			//DNA 레벨별 붙은 자체 스탯
			addall += GetDNAVal(_type);
			//DNA 세트 효과
			addall += GetDNASet(_type);
		}
		else {
			//DNA 레벨별 붙은 자체 스탯
			perdna += GetDNAVal(_type);
			//DNA 세트 효과
			perdna += GetDNASet(_type);
		}
		//혈청
		baseserum = USERINFO.GetAllSerum(_type, StatValType.ABS) + GetSelfSerum(_type, StatValType.ABS, SerumTargetType.Self);
		//연구 
		switch (_type) {
			case StatType.HP: perresearch += USERINFO.ResearchValue(ResearchEff.HealthMaxUp); break;
			case StatType.Atk: perresearch += USERINFO.ResearchValue(ResearchEff.AtkUp); break;
			case StatType.Def: perresearch += USERINFO.ResearchValue(ResearchEff.DefUp); break;
			case StatType.Heal: perresearch += USERINFO.ResearchValue(ResearchEff.HealUp); break;
			case StatType.Men: perresearch += USERINFO.ResearchValue(ResearchEff.MenMaxUp); break;
			case StatType.Hyg: perresearch += USERINFO.ResearchValue(ResearchEff.HygMaxUp); break;
			case StatType.Sat: perresearch += USERINFO.ResearchValue(ResearchEff.SatMaxUp); break;
			case StatType.MenDecreaseDef: addall += USERINFO.ResearchValue(ResearchEff.MenDefUp); break;
			case StatType.HygDecreaseDef: addall += USERINFO.ResearchValue(ResearchEff.HygDefUp); break;
			case StatType.SatDecreaseDef: addall += USERINFO.ResearchValue(ResearchEff.SatDefUp); break;
			case StatType.Sta: perresearch += USERINFO.ResearchValue(ResearchEff.EnergyMaxUp); break;
			case StatType.RecSta: perresearch += USERINFO.ResearchValue(ResearchEff.EnergySpeedUp); break;
			case StatType.Guard: perresearch += USERINFO.ResearchValue(ResearchEff.GuardMaxUp); break;
			case StatType.Speed: perresearch += USERINFO.ResearchValue(ResearchEff.SpeedUp); break;
		}
		if (_pvp) {
			switch (_type) {
				case StatType.HP: perresearch += USERINFO.ResearchValue(ResearchEff.PVPHpUP); break;
				case StatType.Atk: perresearch += USERINFO.ResearchValue(ResearchEff.PVPAtkUp); break;
				case StatType.Def: perresearch += USERINFO.ResearchValue(ResearchEff.PVPDefUP); break;
				case StatType.MenDecreaseDef: perresearch += USERINFO.ResearchValue(ResearchEff.PVPPerDefMenUP); break;
				case StatType.HygDecreaseDef: perresearch += USERINFO.ResearchValue(ResearchEff.PVPPerDefHygUP); break;
				case StatType.SatDecreaseDef: perresearch += USERINFO.ResearchValue(ResearchEff.PVPPerDefSatUP); break;
				case StatType.Speed: perresearch += USERINFO.ResearchValue(ResearchEff.PVPSpeedUp); break;
				case StatType.SuccessAttackPer: addall += USERINFO.ResearchValue(ResearchEff.PVPHitUp); break;
				case StatType.Run: basecharstat += USERINFO.ResearchValue(ResearchEff.PVPPerRunDown); break;
			}
		}
		// 컬렉션(절대값)
		addall += USERINFO.m_Collection.GetStatValue(_type);
		var passive = USERINFO.m_Chars.Sum(o => o.GetPassiveStatValue(_type));
		basecharstat += passive;
		// 캐릭터 패시브 스킬 1,2
		//for (int i = 0; i < USERINFO.m_Chars.Count; i++) {
		//	basecharstat += USERINFO.m_Chars[i].GetPassiveStatValue(_type);
		//}

		float val = (
				//(basecharstat * (1f + perchargrade))
				basecharstat + perchargrade
				+ baseequip
				+ baseserum + basechargetbonus + baselvbonus
			   )
				* (1f + perresearch + perequip + perdna)
				+ addall;
		if (_pvp) {
			if (!m_PVPStat.ContainsKey(_type)) m_PVPStat.Add(_type, 0f);
			m_PVPStat[_type] = val;
		}
		else {
			if (!m_Stat.ContainsKey(_type)) m_Stat.Add(_type, 0f);
			m_Stat[_type] = val;
		}
		return val;
		//[최종 능력치] = (
		//					([생존자 기본 능력치] * (1+[등급 보너스%]))
		//					+ ([장비 능력치] * (1+[피스메이커 레벨 효과%] + [연구 효과%]))
		//					+ [혈청 능력치 절대값]+[캐릭터 보유 보너스]+[전체 레벨 보너스]
		//				  ) 
		//					* (1+[연구 효과%]+[장비 랜덤 스텟%]+[DNA 랜덤 스텟%])
		//					+ Add되는것들
	}
	public int GetNeedAP(bool _instage = true) {
		int needap = m_Skill[0].GetSkillAP();
		if (_instage) {
			needap = Mathf.RoundToInt(needap
			* (1f - Math.Min(1, STAGE_USERINFO.GetBuffValue(StageCardType.APConsumDown) + GetDNABuff(OptionType.ApConsumDown)))
			+ Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ApPlus))
			- GetStat(StatType.ActionPointDecrease));
			needap = Mathf.Clamp(needap, 0, STAGE_USERINFO.m_AP[1]);
		}
		else {
			needap = Mathf.RoundToInt(needap
			* (1f - Math.Min(1, GetDNABuff(OptionType.ApConsumDown)))
			- GetStat(StatType.ActionPointDecrease));
		}
		return needap;
	}
	/// <summary> 전투력 계산, 레벨, 등급 넣을 수 있음 뭐 디버프로 레벨 일시적으로 까이거나 하면 쓰기 </summary>
	public int GetCombatPower(int _lv = 0, int _grade = 0, bool _pvp = false) {
		int lv = _lv == 0 ? m_LV : _lv;
		int grade = _grade == 0 ? m_Grade : _grade;

		//CheckDNASetFX();

		//StringBuilder checkdebug = new StringBuilder();
		//checkdebug.AppendLine($"/////////// Char[{m_Idx}] /////////////////");
		float cp = 0;
		for (int i = 0; i < (int)StatType.Max; i++) {
			cp += Mathf.RoundToInt(GetStat((StatType)i, lv, grade, 0, 0, _pvp) * BaseValue.COMBAT_POWER_RATIO((StatType)i));
			//checkdebug.AppendLine($"{(StatType)i} => {GetStat((StatType)i, lv, grade, 0, 0, _pvp)} : {Mathf.RoundToInt(GetStat((StatType)i, lv, grade, 0, 0, _pvp) * BaseValue.COMBAT_POWER_RATIO((StatType)i))}");
		}
		//Debug.Log(checkdebug.ToString());
		if (_lv > 0 || _grade > 0) return (int)cp;
		else return !_pvp ? m_CP = (int)cp : m_PVPCP = (int)cp;
	}
	/////////////////////////////////////////////////////////
	///DNA
	public float GetDNABuff(OptionType _type) {
		//해당 캐릭의 dna 껴진걸 등급으로 값 추출 없으면 0
		float val = 0f;
		bool canbuff = false;
		//for (int i = 0; i < m_EqDNAUID.Length; i++) {
		//	DNAInfo info = USERINFO.GetDNA(m_EqDNAUID[i]);
		//	if (info == null) continue;
		//	if (info.m_TData.m_OptionType == _type) val += info.m_TData.m_OptionVal;
		//}
		// 발동은 0번째 있는 DNA로만 변경함 2023-10-19
		DNAInfo info = USERINFO.GetDNA(m_EqDNAUID[0]);
		if (info == null) return val;
		if (info.m_TData.m_OptionType != _type) return val;

		val += info.m_TData.m_OptionVal;
		switch (_type) {
			case OptionType.AttackingCoolDown:
			case OptionType.AttackingMaterialAdd:
			case OptionType.HitCoolDown:
			case OptionType.HitMaterialAdd:
			case OptionType.HpResurrection:
			case OptionType.DeathThorn:
				canbuff = UTILE.Get_Random(0f, 1f) < val;
				break;
			default: canbuff = true; break;
		}
#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		if (canbuff && val != 0) Utile_Class.DebugLog_Value(string.Format("charidx : {0}, type: {1}, DNAval : {2}", m_Idx, _type.ToString(), val));
#endif
		return canbuff ? val : 0f;//* 0.0001f
	}

	public float GetDNAVal(StatType _type) {
		float val = 0f;
		//DNA 레벨별 붙은 자체 스탯
		for (int i = 0; i < m_EqDNAUID.Length; i++) {
			if (m_EqDNAUID[i] < 1) continue;
			DNAInfo dnainfo = USERINFO.GetDNA(m_EqDNAUID[i]);
			if (dnainfo == null) continue;
			val += dnainfo.GetOptionValue(_type);
		}
		return val;
	}
	public float GetDNASet(StatType _type) {
		float val = 0f;
		for (DNABGType i = DNABGType.None; i < DNABGType.End; i++) {
			if (!m_EquipDNASetCnt.ContainsKey(i)) continue;
			List<TDNASetEffectTable> setdnsfxdatas = TDATA.GetDNASetFXTables(i, m_EquipDNASetCnt[i]);
			if (setdnsfxdatas == null) continue;
			for(int j = 0;j< setdnsfxdatas.Count; j++) {
				if (setdnsfxdatas[j].m_SetFXType == _type) val += setdnsfxdatas[j].m_SetFXVal;
			}
		}
		return val;
	}
	public void CheckDNASetFX() {
		m_EquipDNASetCnt.Clear();
		for (int i = 0; i < m_EqDNAUID.Length; i++) {
			DNAInfo info = USERINFO.GetDNA(m_EqDNAUID[i]);
			if (info == null) continue;
			if (!m_EquipDNASetCnt.ContainsKey(info.m_TData.m_BGType)) m_EquipDNASetCnt.Add(info.m_TData.m_BGType, 0);
			m_EquipDNASetCnt[info.m_TData.m_BGType]++;
		}
#if NOT_USE_NET
		MAIN.Save_UserInfo();
#endif
	}
	
	/////////////////////////////////////////////////////////
	///Rank
	public void SetRankUP() {
		if (m_Grade >= BaseValue.CHAR_MAX_RANK) return;
		TCharacterGradeTable table = TDATA.GetCharGradeTable(m_Idx, m_Grade);
		USERINFO.DeleteItem(table.m_MatIdx, table.m_MatCount);
		USERINFO.ChangeMoney(-table.m_Money);
		++m_Grade;

		MAIN.Save_UserInfo();
	}
	public bool IS_EnoughMat() {
		TCharacterGradeTable table = TDATA.GetCharGradeTable(m_Idx, m_Grade);
		return USERINFO.GetItemCount(table.m_MatIdx) >= table.m_MatCount;
	}
	public bool IS_EnoughMaxLv() {
		return m_LV >= TDATA.GetCharGradeTable(m_Idx, m_Grade).m_MaxLv;
	}
	public bool IS_EnoughRank() {
		return m_Grade < BaseValue.CHAR_MAX_RANK;
	}
	public bool IS_EnoughMoney() {
		TCharacterGradeTable table = TDATA.GetCharGradeTable(m_Idx, m_Grade);
		return USERINFO.m_Money >= table.m_Money;
	}
	public bool IS_CanRankUP() {
		if (!IS_EnoughMoney()) return false;
		if (!IS_EnoughMat()) return false;
		if (!IS_EnoughRank()) return false;
		return true;
	}
	public bool IS_CanLvUP() {
		if (m_LV >= m_LvMax) return false;
		if (USERINFO.m_Exp[(int)EXPType.Ingame] < m_ExpMax) return false;
		if (USERINFO.m_Money < TDATA.GetExpTable(m_LV).m_Money) return false;
		return true;
	}

	public List<ItemInfo> GetEquipItems()
	{
		List<ItemInfo> re = new List<ItemInfo>();
		for (int i = 0; i < m_EquipUID.Length; i++)
		{
			if (m_EquipUID[i] == 0) continue;
			re.Add(USERINFO.GetItem(m_EquipUID[i]));
		}
		return re;
	}

	public bool IS_CanEquipLvUP() {
		for(int i = 0; i < m_EquipUID.Length; i++) {
			if (m_EquipUID[i] == 0) continue;
			ItemInfo info = USERINFO.GetItem(m_EquipUID[i]);
			if (info.IS_LvUP()) return true;
		}
		return false;
	}
	public bool IS_NewStory() {
		int op = BaseValue.CHAR_OPEN_STORY_SLOT(m_Grade);
		for (int i = op; i > -1; i--) if (!Story[i]) return true;
		return false;
	}
	///혈청
	public void InsertSerum(int _idx) {
		m_Serum.Add(_idx);
	}
	public float GetSelfSerum(StatType _type, StatValType _valtype, SerumTargetType _target) {
		float val = 0;
		//self all 계산한거
		for(int i = 0; i < m_Serum.Count; i++) {
			TSerumTable serum = TDATA.GetSerumTable(m_Serum[i]);
			if (serum == null) continue;
			if (serum.m_Type == _type && serum.m_ValType == _valtype && serum.m_TargetType == _target) val += serum.m_Val;
		}
		
		return val;
	}
	public int GetSerumBlockCnt() {
		bool allget = true;
		int block = 1;
		while (allget) {
			List<TSerumTable> tables = TDATA.GetSerumTableGroup(m_TData.m_SerumGroupIdx, block);
			if (tables != null) {
				for (int i = 0; i < tables.Count; i++) {
					if (!m_Serum.Contains(tables[i].m_Idx)) {
						allget = false;
						return block;
					}
				}
			}
			//TODO:blockmax는 오픈때 10개로 늘어날것
			if (allget) {
				block++;
				block = Mathf.Clamp(block, 1, BaseValue.SERUM_MAXBLOCK);
			}
			if (allget && block == BaseValue.SERUM_MAXBLOCK) break;
		}

		return block;
	}

	public bool IS_CanSerum() {
		List<TSerumTable> tables = TDATA.GetSerumTableGroup(m_TData.m_SerumGroupIdx, GetSerumBlockCnt());
		for (int i = 0; i < tables.Count; i++) {
			bool crntopen = m_Serum.Contains(tables[i].m_Idx);
			bool now = true;
			if (tables[i].m_PrecedIdx.Count == 0 && m_Serum.Contains(tables[i].m_Idx)) now = false;
			for (int j = 0; j < tables[i].m_PrecedIdx.Count; j++) {//못얻고 이전거 되있을때
				if (!m_Serum.Contains(tables[i].m_PrecedIdx[j]) || m_Serum.Contains(tables[i].m_Idx)) {
					now = false;
					break;
				}
			}
			if (now) {
				bool mat = USERINFO.GetItemCount(tables[i].m_Material) >= tables[i].m_MatCnt;
				bool money = USERINFO.m_Money >= tables[i].m_DollarCnt;
				if (mat && money) return true;
			}
		}

		return false;
	}

	///튜토용
	public void TutoUnEquipAll() {
		for (int i = 0; i < m_EquipUID.Length; i++) {
			USERINFO.RemoveEquipUID(m_EquipUID[i]);
		}
//#if NOT_USE_NET
//				for (int i = 0; i < equipItems.Count; i++)
//				{
//					USERINFO.RemoveEquipUID(equipItems[i].m_Uid);
//				}

//				MAIN.Save_UserInfo();
//#else
//		WEB.SEND_REQ_CHAR_UNEQUIPALL((res) => {
//			if (!res.IsSuccess()) {
//				WEB.StartErrorMsg(res.result_code, (btn, obj) => {
//					WEB.SEND_REQ_CHARINFO((res2) => {
//					}, USERINFO.m_UID, m_UID);
//				});
//				return;
//			}
//		}, m_UID);
//#endif
	}
}
