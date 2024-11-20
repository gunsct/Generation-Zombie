
using System.Collections.Generic;
using UnityEngine;

public class StageCardInfo : ClassMng
{
	public int m_Idx;
	// 어둠등 감춰진 카드일경우
	public int m_RealIdx;
	public int m_EnemyIdx;
	public int m_LV;
	public int m_HP;
	public int m_Turn;
	public JobType m_Synergy;
	public int m_PlusCnt = 0;
	/// <summary> 0:atk, 1:move </summary>
	public int[] m_RepeatCnt = new int[2];
	public int m_GambleIdx;
	public int m_IngameRewardIdx = 2;
	public int m_DropRewardIdx = 0;
	/// <summary> 정예 처치 보상인가? </summary>
	public bool Is_RewardCancle = false;
	public int m_NowIdx { get { return IsDark ? m_Idx : m_RealIdx; } }
	public TStageCardTable m_TData { get { return TDATA.GetStageCardTable(m_NowIdx); } }
	public TStageCardTable m_RealTData { get { return TDATA.GetStageCardTable(m_RealIdx); } }
	public TStageCardTable m_NowTData { get { return IsDark ? m_RealTData : m_TData; } }
	public TEnemyTable m_TEnemyData { get { return TDATA.GetEnemyTable(m_EnemyIdx); } }
	public TEnemyLevelTable m_TEnemyLvData { get { return TDATA.GetEnemyLevelTable(m_LV); } }
	int m_TUTOAddLV { get { return TUTO.CheckUseCloneDeck() ? 99 : 0; } }
	public bool IS_EnemyCard { get { return m_EnemyIdx > 0; } }
	public bool IS_Boss { get { return IS_EnemyCard && m_TEnemyData.m_Grade == EEnemyGrade.Boss; } }
	/// <summary> 피난민인지 여부 </summary>
	public bool ISRefugee { get { return IS_EnemyCard && m_TEnemyData.ISRefugee(); } }
	public bool ISNotAtkRefugee { get { return IS_EnemyCard && m_TEnemyData.ISNotAtkRefugee(); } }

	public bool IsDarkCard;
	public bool IsLight;
	public bool IsAutoGetLock;
	// 기믹이나 스킬로 공격 데미지가 가능한 카드
	public bool IsDark { get { return !IsLight && IsDarkCard; } }
	public bool IsGradeMark { get {
			if (IsDark) return false;
			if (!IS_EnemyCard) return false;
			switch(m_TEnemyData.m_Grade)
			{
			case EEnemyGrade.Boss:
			case EEnemyGrade.Elite:
				return true;
			}
			return false;
		}
	}
	public bool IS_RoadBlock {
		get {
			TStageCardTable tdata = m_RealTData;
			switch (tdata.m_Type) {
				case StageCardType.Roadblock:
				case StageCardType.AllRoadblock:
					return true;
				default: return false;
			}
		}
	}
	public bool IS_Rec_Stat(StatType _stat) {
		TStageCardTable tdata = m_RealTData;
		switch (_stat) {
			case StatType.Men:
				if ((tdata.m_Type == StageCardType.RecoveryMen || tdata.m_Type == StageCardType.PerRecoveryMen || tdata.m_Type == StageCardType.AllRecoverySrv) && tdata.m_Value1 > 0f) return true;
				else if (tdata.m_Type == StageCardType.Material && (int)tdata.m_Value1 == 7) return true;
				else if (tdata.m_Type == StageCardType.Enemy && (m_TEnemyData.m_Type == EEnemyType.AllRefugee || m_TEnemyData.m_Type == EEnemyType.MenRefugee 
					|| m_TEnemyData.m_Type == EEnemyType.RandomRefugee  || m_TEnemyData.m_Type == EEnemyType.MaterialRefugee)) return true;
				break;
			case StatType.Hyg:
				if ((tdata.m_Type == StageCardType.RecoveryHyg || tdata.m_Type == StageCardType.PerRecoveryHyg || tdata.m_Type == StageCardType.AllRecoverySrv) && tdata.m_Value1 > 0f) return true;
				else if (tdata.m_Type == StageCardType.Material && (int)tdata.m_Value1 == 6) return true;
				else if (tdata.m_Type == StageCardType.Enemy && (m_TEnemyData.m_Type == EEnemyType.AllRefugee || m_TEnemyData.m_Type == EEnemyType.HygRefugee
					|| m_TEnemyData.m_Type == EEnemyType.RandomRefugee || m_TEnemyData.m_Type == EEnemyType.MaterialRefugee)) return true;
				break;
			case StatType.Sat:
				if ((tdata.m_Type == StageCardType.RecoverySat || tdata.m_Type == StageCardType.PerRecoverySat || tdata.m_Type == StageCardType.AllRecoverySrv) && tdata.m_Value1 > 0f) return true;
				else if (tdata.m_Type == StageCardType.Material && (int)tdata.m_Value1 == 5) return true;
				else if (tdata.m_Type == StageCardType.Enemy && (m_TEnemyData.m_Type == EEnemyType.AllRefugee || m_TEnemyData.m_Type == EEnemyType.SatRefugee
					|| m_TEnemyData.m_Type == EEnemyType.RandomRefugee || m_TEnemyData.m_Type == EEnemyType.MaterialRefugee)) return true;
				break;
			case StatType.HP:
				if ((tdata.m_Type == StageCardType.RecoveryHp || tdata.m_Type == StageCardType.RecoveryHpPer || tdata.m_Type == StageCardType.AllRecoverySrv) && tdata.m_Value1 > 0f) return true;
				else if (tdata.m_Type == StageCardType.Material && (int)tdata.m_Value1 == 2) return true;
				else if (tdata.m_Type == StageCardType.Enemy && (m_TEnemyData.m_Type == EEnemyType.AllRefugee || m_TEnemyData.m_Type == EEnemyType.HpRefugee
					|| m_TEnemyData.m_Type == EEnemyType.RandomRefugee || m_TEnemyData.m_Type == EEnemyType.MaterialRefugee)) return true;
				break;
		}
		return false;
	}
	public StageCardInfo(int Idx)
	{
		IsLight = false;
		SetIdx(Idx);
		SetData();
	}
	/// <summary> 카드 세팅이나 변환될때 호출 </summary>
	public void InitVal() {
		m_IngameRewardIdx = 0;
		m_DropRewardIdx = 0;
		Is_RewardCancle = true;
	}
	public void SetIdx(int idx)
	{
		m_RealIdx = m_Idx = idx;
		IsDarkCard = TDATA.GetStageCardTable(m_Idx).m_Type == StageCardType.Dark;
	}

	public void SetData(int _enemyidx = 0)
	{
		IsAutoGetLock = false;
		m_EnemyIdx = 0;
		m_Turn = 0;
		m_RepeatCnt[0] = m_RepeatCnt[1] = 0;
		switch (m_NowTData.m_Type)
		{
			case StageCardType.Enemy:
				if(_enemyidx != 0) SetEnemyIdx(_enemyidx);
				else SetEnemyIdx(Mathf.RoundToInt(m_NowTData.m_Value1));
				break;
			case StageCardType.TimeBomb:
				m_Turn = Mathf.RoundToInt(m_NowTData.m_Value2);
				break;
			case StageCardType.Hive:
				SetEnemyIdx(Mathf.RoundToInt(m_NowTData.m_Value1));
				break;
			case StageCardType.Synergy:
				m_Synergy = STAGE_USERINFO.CreateSynergy();
				break;
			case StageCardType.Gamble:
				m_GambleIdx = TDATA.GetRandGambleCardTable(STAGEINFO.m_StageModeType, Mathf.RoundToInt(m_RealTData.m_Value1)).m_Idx;
				break;
			case StageCardType.Item_RewardBox:
				if (m_NowTData.m_Value1 != 0) m_IngameRewardIdx = (int)m_NowTData.m_Value1;
				break;
		}
	}

	public void SetRealIdx(int idx)
	{
		m_RealIdx = idx;
		switch (m_RealTData.m_Type)
		{
		case StageCardType.Enemy:
			SetEnemyIdx(Mathf.RoundToInt(m_RealTData.m_Value1));
			break;
		case StageCardType.Hive:
			SetEnemyIdx(Mathf.RoundToInt(m_RealTData.m_Value1));
			break;
		case StageCardType.TimeBomb:
			m_Turn = Mathf.RoundToInt(m_RealTData.m_Value2);
			break;
		}

	}

	public void SetEnemyIdx(int idx)
	{
		m_EnemyIdx = STAGE_USERINFO.ISBuff(StageCardType.ConDeadly) ? Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.ConDeadly)) : idx;
		int addlv = 0;
		addlv = Mathf.RoundToInt(m_TEnemyData.m_Tribe == EEnemyTribe.Zombie ? STAGE_USERINFO.m_AddZombieLV : STAGE_USERINFO.m_AddLV);

		m_LV = STAGEINFO.GetCreateEnemyLV(addlv);
		m_HP = GetMaxStat(EEnemyStat.HP);
	}

	public Sprite GetImg()
	{
		Sprite re = null;
		string imgname = m_TData.m_Img;
		switch(m_TData.m_Type)
		{
		case StageCardType.Synergy: return TDATA.GetSynergyTable(m_Synergy).GetImg();
		case StageCardType.Material:
			bool behind = STAGE_USERINFO.ISBuff(StageCardType.RandomMaterial);
			return behind ? UTILE.LoadImg("Card/Stage/Stage_0", "png") : TDATA.GetStageMaterialTable((StageMaterialType)Mathf.RoundToInt(m_RealTData.m_Value1)).GetStateCardImg();
		}
		if (string.IsNullOrWhiteSpace(imgname) && !IsDark && IS_EnemyCard) imgname = m_TEnemyData.m_PrefabName;
		if (!string.IsNullOrWhiteSpace(imgname)) re = UTILE.LoadImg(imgname, "png");

		if (re == null) re = UTILE.LoadImg("Card/Stage/CardBack", "png");
		return re;
	}

	public Sprite GetRealImg()
	{
		Sprite re = null;
		string imgname = m_TData.m_Img;
		switch (m_RealTData.m_Type)
		{
		case StageCardType.Synergy: return TDATA.GetSynergyTable(m_Synergy).GetImg();
		case StageCardType.Material: 
				return TDATA.GetStageMaterialTable((StageMaterialType)Mathf.RoundToInt(m_RealTData.m_Value1)).GetStateCardImg();

		}
		if (string.IsNullOrWhiteSpace(imgname) && IS_EnemyCard) imgname = m_TEnemyData.m_PrefabName;
		if (!string.IsNullOrWhiteSpace(imgname)) re = UTILE.LoadImg(imgname, "png");

		if (re == null) re = UTILE.LoadImg("Card/Stage/CardBack", "png");
		return re;
	}

	public Sprite GetEnemyAlphaImg() {
		Sprite re = null;
		string imgname = m_TEnemyData.m_PrefabName;
		re = UTILE.LoadImg(imgname, "png");
		if (re == null) re = UTILE.LoadImg("Enemy/66_Enemy", "png");
		return re;
	}
	public string GetName()
	{
		switch (m_TData.m_Type)
		{
			case StageCardType.Synergy: 
				return TDATA.GetSynergyTable(m_Synergy).GetName();
			case StageCardType.Material:
				int matcountdown = Mathf.RoundToInt(STAGE_USERINFO.GetBuffValue(StageCardType.MaterialCountDown));
				int matcnt = Mathf.Max(Mathf.RoundToInt(m_TData.m_Value2) - matcountdown, 1);
				bool behind = STAGE_USERINFO.ISBuff(StageCardType.RandomMaterial);
				//if (matcnt + m_PlusCnt < 2) return behind ? "???" : m_TData.GetName();
				return string.Format("{0} X {1}", behind ? "???" : m_TData.GetName(), behind ? "???" : Mathf.RoundToInt(matcnt + m_PlusCnt).ToString());
			case StageCardType.Drill: 
				return m_TData.GetName(Mathf.RoundToInt(m_TData.m_Value1 == 0f ? 1 : m_TData.m_Value1 * 100f), Mathf.RoundToInt(m_TData.m_Value1 == 0f ? 1 : m_TData.m_Value1));
		}
		if (!IsDark && IS_EnemyCard) return string.Format("Lv{0} {1}", m_LV + m_TUTOAddLV, m_TEnemyData.GetName());
		return m_TData.GetName(Mathf.RoundToInt(m_TData.m_Value1 * 100f), Mathf.RoundToInt(m_TData.m_Value1));
	}
	public string GetDesc() {
		if (IsDark) return TDATA.GetString(ToolData.StringTalbe.Etc, 40062);
		return m_NowTData.GetInfo();
	}
	public List<string> GetEnemyDesc() {
		List<string> descs = new List<string>();
		if (IsDark) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 40062));
		else {
			string info = m_TData.GetOnlyInfo();
			if (!string.IsNullOrEmpty(info)) descs.Add(info);
			descs.AddRange(m_TEnemyData.GetDescs(m_TData));
		}
		return descs;
	}

	public bool IS_TargetType(EEnemyType type)
	{
		if (!IS_EnemyCard) return false;

		return m_TEnemyData.m_Type == type;
	}

	public int GetStat(EEnemyStat stat)
	{
		if (!IS_EnemyCard) return 0;
		int Re = m_TEnemyData.GetStat(stat, m_LV);
		switch(stat)
		{
		case EEnemyStat.HP:
			Re = m_HP = Mathf.Min(m_HP, GetMaxStat(EEnemyStat.HP));
			break;
		}
		return Re;
	}
	public int GetMaxStat(EEnemyStat stat)
	{
		if (!IS_EnemyCard) return 0;
		int Re = m_TEnemyData.GetStat(stat, m_LV);
		return Re;
	}

	public bool IS_AIEnemy()
	{
		if (!IS_EnemyCard) return false;
		//if (IsDarkCard) return false;
		if (m_TData.m_IsEndType) return false;
		if (m_TData.m_Type == StageCardType.Hive) return false;
		if (m_TEnemyData.m_Grade == EEnemyGrade.Boss && !m_TData.m_IsEndType) return false;
		return true;
	}

	// 기믹이나 스킬로 공격 데미지가 가능한 카드
	public bool IS_DmgTarget()
	{
		//if (IsDark) return false;
		if (m_TData.m_IsEndType) return false;
		if (!IS_EnemyCard) return m_TData.m_Type == StageCardType.Chain;
		if (m_TEnemyData.m_Grade == EEnemyGrade.Boss && !m_TData.m_IsEndType) return false;
		//if (m_TEnemyData.ISRefugee()) return false;
		return true;
	}
	/// <summary> Oil, GasStation, Oldmine </summary>
	public bool IS_ExplosionTarget() {
		if (m_TData.m_IsEndType) return false;
		return m_TData.m_Type == StageCardType.OldMine || m_TData.m_Type == StageCardType.Allymine || m_TData.m_Type == StageCardType.Oil || m_TData.m_Type == StageCardType.GasStation;
	}
	public bool IS_OilGas() {
		if (m_TData.m_IsEndType) return false;
		return m_TData.m_Type == StageCardType.Oil || m_TData.m_Type == StageCardType.GasStation;
	}
	public bool IS_OldMineTarget() {
		if (IS_Boss) return false;
		if (IS_RoadBlock) return false;
		if (m_TData.m_IsEndType) return false;
		return true;
	}

	public bool IS_FireTarget()//화재, 잿더미, 재료-가루 는 제외
	{
		if (IS_Boss) return false;
		if (IS_RoadBlock) return false;
		if (m_TData.m_IsEndType) return false;
		return !m_TData.IS_LineCard() && m_TData.m_Type != StageCardType.Fire && m_TData.m_Type != StageCardType.Ash 
			&& !(m_TData.m_Type == StageCardType.Material && (StageMaterialType)m_TData.m_Value1 == StageMaterialType.Powder);
	}
	public bool IS_BurnTarget()//화재, 잿더미, 재료-가루는 제외, 화상이라 보스카드도 적용
	{
		if (IS_RoadBlock) return false;
		if (m_TData.m_IsEndType) return false;
		return m_TData.m_Type != StageCardType.Fire && m_TData.m_Type != StageCardType.Ash
			&& !(m_TData.m_Type == StageCardType.Material && (StageMaterialType)m_TData.m_Value1 == StageMaterialType.Powder);
	}
	/// <summary> 어둠일때 리얼 데이터 체크</summary>
	public bool IS_TypeTarget(StageCardType _type) {
		StageCardType type = m_TData.m_Type;
		if (type == StageCardType.Dark) {
			if (_type == type) return true;
			else type = m_RealTData.m_Type;
		}
		return _type == type;
	}
	public bool IS_AutoGet() {
		if (IsAutoGetLock) return false;
		switch (m_NowTData.m_Type) {
			case StageCardType.TornBody:
			case StageCardType.Garbage:
			case StageCardType.Pit:
			//case StageCardType.OldMine:
			case StageCardType.Fire:
			case StageCardType.CountBox:
			case StageCardType.BigSupplyBox:
			//case StageCardType.Item_RewardBox:
				return true;
			case StageCardType.Enemy:
				if (ISRefugee) return true;
				else return false;
			case StageCardType.AtkUp:
			case StageCardType.DefUp:
			case StageCardType.HpUp:
			case StageCardType.RecoveryHp:
			case StageCardType.RecoveryAP:
			case StageCardType.SpeedUp:
			case StageCardType.CriticalUp:
			case StageCardType.CriticalDmgUp:
			case StageCardType.HeadShotUp:
			case StageCardType.LimitTurnUp:
			case StageCardType.TimePlus:
			case StageCardType.APRecoveryUp:
			case StageCardType.APConsumDown:
			case StageCardType.HealUp:
			case StageCardType.LevelUp:
			case StageCardType.RecoverySat:
			case StageCardType.RecoveryHyg:
			case StageCardType.RecoveryMen:
			case StageCardType.MergeSlotCount:
			case StageCardType.BanAirStrike:
				return m_NowTData.m_Value1 < 0f || m_NowTData.m_AutoGetBuff;
			default:return false;
		}
	}
	public bool IS_AnimalEatCard() {
		switch (m_NowTData.m_Type) {
			case StageCardType.Material:
				return Mathf.RoundToInt(m_NowTData.m_Value1) == 5 || Mathf.RoundToInt(m_NowTData.m_Value1) == 7;
			case StageCardType.RecoverySat:
				return true;
			default:return false;
		}
	}
}
