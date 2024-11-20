using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : ClassMng
{
	/// <summary> 아이템 고유번호 </summary>
	public long m_Uid;
	/// <summary> 아이템 인덱스 </summary>
	public int m_Idx;
	/// <summary> 아이템 레벨 </summary>
	public int m_Lv;
	/// <summary> 아이템 개수</summary>
	public int m_Stack;
	/// <summary> 추가 스탯 </summary>
	public List<ItemStat> m_AddStat = new List<ItemStat>();
	/// <summary> 현 경험치 </summary>
	public int m_Exp = 0;
	/// <summary> 아이템 등급 </summary>
	[JsonIgnore] public int m_Grade { set { } get { return m_TData.m_Grade; } }
	/// <summary> 아이템 최대 레벨 </summary>
	[JsonIgnore] public int m_MaxLV { get { return BaseValue.ITEM_GRADE_MAX_LV(m_Grade); } }

	/// <summary> 아이템 툴데이터 </summary>
	[JsonIgnore] public TItemTable m_TData { get { return TDATA.GetItemTable(m_Idx); } }

	/// <summary> 전용 장비 툴데이터 null일경우 일반 장비 </summary>
	[JsonIgnore] public TEquipSpecialStat m_TSpecialStat { get { return TDATA.GetEquipSpecialStat(m_Idx); } }
	/// <summary> 필요 경험치 툴데이터 </summary>
	[JsonIgnore] public TEquipExpTable m_TExpData { get { return TDATA.GetEquipExpTable(m_TData.GetEquipType(), m_Grade, m_Lv); } }

	[JsonIgnore] public bool m_GetAlarm;

	/// <summary> 판매나 강화할때 카운트 </summary>
	[JsonIgnore] public int m_TempCnt;

	/// <summary> 전투력 </summary>
	[JsonIgnore] public int m_CP;
	/// <summary> 잠금 </summary>
	public bool m_Lock;
	/// <summary> 터치 블럭 </summary>
	[JsonIgnore] public bool m_NotSelect;
	/// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
	public ItemInfo() { }
	/// <param name="uid"> 고유번호 </param>
	/// <param name="_idx"> 인덱스 </param>
	/// <param name="_lv"> 레벨 </param>
	/// <param name="_stack"> 개수 </param>
	/// <param name="skill"> 스킬 </param>
	public ItemInfo(int _idx, long uid = 0, int _lv = 1, int _stack = 1)
	{
#if NOT_USE_NET
		if (uid == 0) uid = Utile_Class.GetUniqeID();
#endif
		m_Uid = uid;
		m_Idx = _idx;
		m_Lv = _lv;
		m_Stack = _stack;
		// TODO FIX GRADE
		m_Grade = m_TData.m_Grade;
		for (int i = BaseValue.ITEM_OPTION_CNT(m_Grade) - 1; i > -1; i--)
		{
			// 등급에 맞는 옵션 셋팅해준다.
			TRandomStatTable table = TDATA.GetPickRandomStat(m_TData.m_RandStatGroup);
			if (table == null) return;
			m_AddStat.Add(new ItemStat() {
#if NOT_USE_NET
				m_Stat = table.m_Stat,
				m_Val = table.GetVal()
#else
				m_Stat = StatType.None,
				m_Val = 0
#endif
			});
		}
		m_GetAlarm = true;
	}
	public void SetDATA(LS_Web.RES_ITEMINFO data)
	{
		m_Idx = data.Idx;
		m_Exp = data.EXP;
		m_Grade = data.Grade;
		m_Lock = data.Lock;
		if (m_TData.GetInvenGroupType() == ItemInvenGroupType.Equipment)
		{
			m_Lv = data.LV;
			m_Stack = 1;
		}
		else
		{
			m_Lv = 1;
			m_Stack = data.Cnt;
		}
		m_AddStat.Clear();
		for (int i = 0; i < data.AddStat.Count; i++) m_AddStat.Add(data.AddStat[i].GetItemStat());

		if(m_TData.GetEquipType() != EquipType.End) m_CP = GetCombatPower();
	}



	///// <summary> 레벨 업, 성공시 1~5 레벨업 실패시 재료만 삭제 </summary>
	//public bool SetLvUp()
	//{
	//    USERINFO.DeleteItem(m_TData_Consol.m_MatIdx, m_TData_Consol.m_MatCount);
	//    USERINFO.ChangeMoney(-m_TData_Consol.m_Dollar);
	//    if (m_TData_Consol.GetCanUpgrade(m_AddUpgradeProb))
	//    {
	//        m_AddUpgradeProb = 0;
	//        m_Lv = UnityEngine.Mathf.Min(m_Lv + 1, BaseValue.ITEM_MAX_LEVEL);
	//        return true;
	//    }
	//    else
	//    {
	//        m_AddUpgradeProb += m_TData_Consol.m_FailUpProb;
	//        return false;
	//    }
	//}

	public float GetStat(StatType _type, int? lv = null,CharInfo eqchar = null) {
		float re = 0;
		int LV = lv == null ? m_Lv : Mathf.Min(m_MaxLV, lv.Value);


		List<StatType> testratiostat = new List<StatType>() { StatType.HP, StatType.Atk, StatType.Def, StatType.Heal };
		float baseratio = 1f;
#if STAGE_TEST
		if (testratiostat.Contains(_type)) baseratio += PlayerPrefs.GetFloat("STAGE_TEST_STATRATIO");
#endif

		//기본 장비 스탯
		// 더이상 증가가 없다
		// 이전 레벨의 데이터로 가져온다.
		List<ItemStat> stats = m_TData.m_Stat;
		for (int i = 0; i < stats.Count; i++)
		{
			if (stats[i].m_Stat != _type) continue;
			re += stats[i].GetValue(LV);
		}
		float per = 1f;
		switch (m_TData.GetEquipType()) {
			case EquipType.Weapon: per += USERINFO.ResearchValue(ResearchEff.WeaponStatUp); break;
			case EquipType.Helmet: per += USERINFO.ResearchValue(ResearchEff.HelmetStatUp); break;
			case EquipType.Costume: per += USERINFO.ResearchValue(ResearchEff.CostumeStatUp); break;
			case EquipType.Shoes: per += USERINFO.ResearchValue(ResearchEff.ShoesStatUp); break;
			case EquipType.Accessory: per += USERINFO.ResearchValue(ResearchEff.AccStatUp); break;
		}
		per += USERINFO.GetEquipGachaLvBonus(m_TData.GetEquipType());
		return re * baseratio * per;
	}

	public float GetOptionValue(StatType _type, CharInfo eqchar = null)
	{
		float per = 0;
		//추가 옵션
		for (int i = 0; i < m_AddStat.Count; i++)
		{
			if (m_AddStat[i].m_Stat != _type) continue;
			per += m_AddStat[i].m_Val;
		}

		// 전용 장비 체크 장착한 케릭터만 확인
		if (eqchar != null && eqchar.m_EquipUID[(int)m_TData.GetEquipType()] == m_Uid)
		{
			if (m_TSpecialStat != null && m_TSpecialStat.m_Char == eqchar.m_Idx)
			{
				ItemStat stats = m_TSpecialStat.m_Stat;
				if (stats.m_Stat == _type) per += stats.m_Val;
			}
		}

		return per * 0.0001f;
	}
	/// <summary> 랭크업시 추가 스탯 </summary>
	public void SetGradeStat() {
		if (m_AddStat.Count >= BaseValue.ITEM_OPTION_CNT(m_Grade)) return;
		TRandomStatTable table = TDATA.GetPickRandomStat(m_TData.m_RandStatGroup);
		if (table == null) return;
		m_AddStat.Add(new ItemStat() { 
			m_Stat = table.m_Stat,
			m_Val = table.GetVal()
		});
	}

	public void DeleteLastAddOption() {
		if (m_AddStat.Count < 1) return;
		m_AddStat.RemoveAt(m_AddStat.Count - 1);
	}

	public int GetCombatPower(int? _lv = null) {
		int lv = _lv == null ? m_Lv : _lv.Value;

		float cp = 0;
		for (StatType i = StatType.Men; i < StatType.Max; i++) {
			float add = GetStat(i, lv);
			//float per = GetOptionValue(i);
			//cp += (add + add * per) * BaseValue.COMBAT_POWER_RATIO(i);
			cp += add * BaseValue.COMBAT_POWER_RATIO(i);
		}

		// 전용 장비
		if (m_TSpecialStat != null) cp += BaseValue.SPECIAL_ITEM_POWER;
		if(_lv != null) return Mathf.RoundToInt(cp);
		else return m_CP = Mathf.RoundToInt(cp);
	}

	/// <summary> 레벨업 가능 체크, 레벨과 재료 </summary>
	public bool IS_LvUP()
	{
		return m_Lv < m_MaxLV;
	}

	public void Use(int Cnt, Action _cb)
	{
		if (!m_TData.Is_UseItem()) return;
		if (m_Stack < Cnt) Cnt = m_Stack;
		// 사용 아니템은 보상이 1개만 됨
		// 여러개일경우는 다른방식으로 해야될듯
		
		for(int i = 0; i < Cnt; i++) TDATA.GetGachaItem(m_TData);

#if NOT_USE_NET
		m_Stack -= Cnt;
		if (m_Stack < 1) USERINFO.DeleteItem(m_Uid);

		if (Cnt > 0) {
			GachaGroup group = TDATA.GetGachaGroup(m_TData.m_Value);
			TGachaGroupTable grouptable = group.m_List[0];

			switch (m_TData.m_Type) {
				case ItemType.GoldTeethBundle:
					USERINFO.GetCash(grouptable.m_RewardCount * Cnt); break;
				case ItemType.DollarBundle:
					USERINFO.ChangeMoney(grouptable.m_RewardCount * Cnt); break;
				case ItemType.ExpBundle:
					USERINFO.SetIngameExp(grouptable.m_RewardCount * Cnt); break;
				case ItemType.BulletBundle:
					USERINFO.GetShell(grouptable.m_RewardCount * Cnt); break;
			}
		}
		_cb?.Invoke();
		MAIN.Save_UserInfo();
#else
//네트워크
#endif
	}
	
	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_TData.m_Name);
	}

	public string GetGradeGroupName()
	{
		if (m_TData.GetInvenGroupType() == ItemInvenGroupType.CharaterPiece) return m_TData.GetInvenGroupName();
		return string.Format("{0} {1}", BaseValue.GradeName(m_Grade), m_TData.GetInvenGroupName());
	}

	public int GetCellPrice(int? Cnt = null) {
		return m_TData.GetPrice() * (Cnt == null ? m_TempCnt : Cnt.Value);
	}

	public float GetExpPrice(int? Cnt = null) {
		int Price = 0;
		if (m_TData.GetInvenGroupType() == ItemInvenGroupType.Equipment) Price = BaseValue.ITEM_EXP_MONEY(m_TExpData.m_Exp[1]) * (Cnt == null ? 1 : (Cnt > 0 ? 1 : -1));
		else Price = BaseValue.ITEM_EXP_MONEY(m_TData.m_Value * (Cnt == null ? m_TempCnt : Cnt.Value));

		return Price;
	}

	public int GetExp()
	{
		var tdata = m_TData;
		if (tdata.m_Type == ItemType.ConsolidationMaterial) return tdata.m_Value;
		EquipType type = tdata.GetEquipType();
		if (type == EquipType.End) return 0;
		int grade = m_Grade;
		TEquipExpTable tExp = TDATA.GetEquipExpTable(type, m_Grade, m_Lv);
		return tExp.m_Exp[1];
	}

	public TEquipExpTable CalcLV(out int lv, out int exp, long AddExp)
	{
		var tdata = m_TData;
		lv = m_Lv;
		exp = m_Exp;
		long temp = m_Exp + AddExp;
		int MaxLV = m_MaxLV;
		EquipType type = tdata.GetEquipType();
		if (type == EquipType.End) return null;
		int grade = m_Grade;
		TEquipExpTable tExp = TDATA.GetEquipExpTable(type, grade, lv);
		while (tExp != null)
		{
			if (lv == MaxLV) break;
			if (temp < tExp.m_Exp[0]) break;
			lv++;
			temp -= tExp.m_Exp[0];
			tExp = TDATA.GetEquipExpTable(type, grade, lv);
		}
		exp = (int)temp;
		return tExp;
	}

	public long GetNeedExp(int LV)
	{
		if (m_Lv >= LV) return 0;
		var tdata = m_TData;
		EquipType type = tdata.GetEquipType();
		if (type == EquipType.End) return 0;
		TEquipExpTable tExp = TDATA.GetEquipExpTable(type, m_Grade, m_Lv);
		if (tExp == null) return 0;
		long re = tExp.m_Exp[0] - m_Exp;
		for(int i = m_Lv+1; i < LV; i++)
		{
			tExp = TDATA.GetEquipExpTable(type, m_Grade, i);
			re += tExp.m_Exp[0];
		}
		return re;
	}
}
