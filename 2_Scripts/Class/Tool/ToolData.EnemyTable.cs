using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// 절대 순서 변경하면 안됨
/// 툴데이터에서 번호를 사용함
public enum EEnemyType
{
	None = 0,
	/// <summary> 동물 </summary>
	Animal,
	/// <summary> 좀비 </summary>
	Zombie,
	/// <summary> 돌연변이 </summary>
	Mutant,
	/// <summary> 마피아 </summary>
	Mafia,
	/// <summary> 탐색자 </summary>
	Scavenger,
	/// <summary> 광신도 </summary>
	Zealot,
	/// <summary> 갱스터 </summary>
	Gangster,
	/// <summary> 특수부대 </summary>
	Wolfs,
	/// <summary> 난민 </summary>
	SatRefugee,
	MenRefugee,
	HygRefugee,
	HpRefugee,
	RandomRefugee,//4개 스탯중 2개
	AllRefugee,//4개 스탯 전부
	/// <summary> Npc </summary>
	Npc,
	/// <summary> 기타 </summary>
	Etc,
	//감염된 난민
	SatInfectee,
	MenInfectee,
	HygInfectee,
	HpInfectee,
	RandomInfectee,
	Allinfectee,
	//재료주는 피난민
	MaterialRefugee,
	End
}

/// <summary> 종족 </summary>
/// 절대 순서 변경하면 안됨
/// 툴데이터에서 번호를 사용함
public enum EEnemyTribe
{
	None = 0,
	/// <summary> 동물 </summary>
	Animal,
	/// <summary> 좀비 </summary>
	Zombie,
	/// <summary> 돌연변이 </summary>
	Mutant,
	/// <summary> 인간 </summary>
	Human,
	End
}

/// <summary> 종족 </summary>
/// 절대 순서 변경하면 안됨
/// 툴데이터에서 번호를 사용함
public enum EEnemyGrade
{
	None = 0,
	Normal,
	Elite,
	Boss,
	End
}

public enum EEnemyStat
{
	HP = 0,
	ATK,
	DEF,
	SPD,
	HIDING,
	/// <summary> 적이 공격시 감소되는 생존 스텟 수치 조정 </summary>
	ATKSURVSTAT,
	/// <summary> 적 사망 시 감소하는 생존 스텟 수치 조정 </summary>
	DEADSURVSTAT,
	End
}
public enum EBodyType
{
	Head,
	Body,
	Arm,
	Leg,
	End
}

public enum EEnemyAIAtkType
{
	/// <summary> 일반적으로 공격하며 대상이 사망 시 대상 카드 제거 및 상단의 카드가 아래로 내려옴 </summary>
	Normal = 0,
	/// <summary> 공격 후 대상이 사망 시 대상을 Value02에 기입된 ID로 변경 </summary>
	Infection,
	/// <summary> 공격하지 않음 </summary>
	None
}

public enum EEnemyAIMoveType
{
	/// <summary> 자신을 공격 가능한 대상이 상하좌우 위치에 있을 경우 반대 방향으로 이동거리 만큼 이동 </summary>
	Coward = 0,
	/// <summary> 가장 가까운 자신의 Hostility에 등록된 대상을 추적 (동일한 거리에 있는 대상이 다수 일 경우 랜덤으로 추적) </summary>
	Tracker,
	/// <summary> 랜덤한 방향으로 Value01값 만큼 이동합니다. </summary>
	Wanderer,
	/// <summary> 플레이어의 반대 방향으로 이동 </summary>
	Runer,
	/// <summary> 가장 가까운 자신의 Hostility에 등록된 먹을것 대상을 추적 (동일한 거리에 있는 대상이 다수 일 경우 랜덤으로 추적) </summary>
	EatTracker,
	/// <summary> 가장 가까운 자신의 Hostility에 등록된 특정 카드 인덱스 대상을 추적 (동일한 거리에 있는 대상이 다수 일 경우 랜덤으로 추적) </summary>
	SpecialTracker,
	/// <summary> 플레이어의 반대 방향으로 이동, 이동 안할때는 3*3 내 랜덤하게 1장을 value02로 바꿈</summary>
	Arsonist01,
	/// <summary> 전투없이 플레이어쪽으로 전진 </summary>
	Stalker,
	/// <summary> 이동하지 않음 </summary>
	None
}
public class TEnemyTable : ClassMng
{
	public struct Body
	{
		public int Prob;
		public float Ratio;
	}
	public class ReduceStat
	{
		public StatType Type;
		public int Val;
	}
	public class AI<T>
	{
		/// <summary> 타입 </summary>
		public T m_Type;

		/// <summary> AI 형태를 구성하는 값 - 예 : 방황이라면 방황 할때 최대 이동하는 횟수, 변환이라면 변환되는 카드 ID</summary>
		public int[] m_Values = new int[2];
		/// <summary> AI를 한번 실행 후 다음 실행까지의 최소&최대 턴 (최대 턴과 최소 턴 사이에서 랜덤하게 결정)</summary>
		public int[] m_Repeat = new int[2];

		public int GetRepeatCnt()
		{
			return Utile_Class.Get_RandomStatic(m_Repeat[0], m_Repeat[1]);
		}
	}

	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 타입 </summary>
	public EEnemyType m_Type;
	/// <summary> 종족 </summary>
	public EEnemyTribe m_Tribe { get {

			switch (m_Type)
			{
			case EEnemyType.Animal: return EEnemyTribe.Animal;
			case EEnemyType.Zombie: return EEnemyTribe.Zombie;
			case EEnemyType.Mutant:	return EEnemyTribe.Mutant;
			default:				return EEnemyTribe.Human;
			}
		}
	}


	/// <summary> 등급 </summary>
	public EEnemyGrade m_Grade;
	/// <summary> 신체 확률 </summary>
	public Body[] m_Body = new Body[(int)EBodyType.End];
	public int m_BodyProbMax;
	// 몬스터 공격
	public EnemySkillTableGroup m_Skill;
	/// <summary> 공격 횟수 </summary>
	public int[] m_AtkCnt = new int[2];
	/// <summary> 다음 공격 시간 </summary>
	public float[] m_AtkTime = new float[2];
	// 몬스터 방어
	/// <summary> 노트 그룹 아이디 </summary>
	public EnemyNoteTableGroup m_Note;
	/// <summary> 방어 시간 </summary>
	public float[] m_DefTime = new float[2];
	/// <summary> 다음 노트 생성 시간 </summary>
	public float[] m_NoteTime = new float[2];
	/// <summary> 노트 생성 개수 </summary>
	public int[] m_NoteCnt = new int[2];
	public string m_PrefabName;
	public AI<EEnemyAIAtkType> m_AtkAI = new AI<EEnemyAIAtkType>();
	public AI<EEnemyAIMoveType> m_MoveAI = new AI<EEnemyAIMoveType>();
	public int m_RewardLV;
	/// <summary> 인게임 보상 그룹 인덱스 IngameRewardTable 참조 </summary>
	public int m_RewardGID;
	/// <summary> 스텟 </summary>
	public float[] m_Stat = new float[(int)EEnemyStat.End];
	public List<ReduceStat> m_ReduceStat = new List<ReduceStat>();
	public int m_BattleRoundLimit;

	/// <summary> 좀비 연구 그룹 인덱스(그룹의 연구 레벨에 맞는 추가 공격력 % 비율로 계산됨) </summary>
	public int m_ZombieResIdx;
	/// <summary> 듀얼 피격음 </summary>
	public List<SND_IDX> m_HitVoice = new List<SND_IDX>();
	/// <summary> enemydroptable gid 연결</summary>
	public int m_DropCardGid = 0;
	/// <summary> 전투보상을 지정 그룹으로 전부 나오게 할건지 </summary>
	public bool m_AllGroup;
	/// <summary> 전투 보상 취소 여부 </summary>
	public bool m_RewardCancle;
	/// <summary> 즉살 여부 </summary>
	public bool m_Deadly;
	/// <summary> 자동획득음(피난민류) </summary>
	public List<SND_IDX> m_RescueVoice = new List<SND_IDX>();
	public int[] m_NoteSatDecMinMax = new int[2];
	/// <summary> 디버프 보상만 있는 경우 </summary>
	public bool m_OnlyDebuffBox;
	public TEnemyTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<EEnemyType>();
		m_Grade = pResult.Get_Enum<EEnemyGrade>();
		m_BodyProbMax = 0;
		for (int i = (int)EBodyType.Head, iMax = (int)EBodyType.End; i < iMax; i++)
		{
			m_Body[i].Prob = pResult.Get_Int32();
			m_Body[i].Ratio = pResult.Get_Float();

			m_BodyProbMax += m_Body[i].Prob;
		}

		if (MainMng.IsValid()) m_Skill = TDATA.GetEnemySkillTableGroup(pResult.Get_Int32());
		else pResult.Get_Int32();

		m_AtkCnt[0] = pResult.Get_Int32();
		m_AtkCnt[1] = pResult.Get_Int32();

		m_AtkTime[0] = pResult.Get_Float();
		m_AtkTime[1] = pResult.Get_Float();

		m_DefTime[0] = pResult.Get_Float();
		m_DefTime[1] = pResult.Get_Float();

		m_NoteTime[0] = pResult.Get_Float();
		m_NoteTime[1] = pResult.Get_Float();

		m_NoteCnt[0] = pResult.Get_Int32();
		m_NoteCnt[1] = pResult.Get_Int32();

		if (MainMng.IsValid()) m_Note = TDATA.GetEnemyNoteTableGroup(pResult.Get_Int32());
		else pResult.Get_Int32();

		m_PrefabName = pResult.Get_String();

		m_AtkAI.m_Type = pResult.Get_Enum<EEnemyAIAtkType>();
		m_AtkAI.m_Values[0] = pResult.Get_Int32();
		m_AtkAI.m_Values[1] = pResult.Get_Int32();
		m_AtkAI.m_Repeat[0] = pResult.Get_Int32();
		m_AtkAI.m_Repeat[1] = pResult.Get_Int32();

		m_MoveAI.m_Type = pResult.Get_Enum<EEnemyAIMoveType>();
		m_MoveAI.m_Values[0] = pResult.Get_Int32();
		m_MoveAI.m_Values[1] = pResult.Get_Int32();
		m_MoveAI.m_Repeat[0] = pResult.Get_Int32();
		m_MoveAI.m_Repeat[1] = pResult.Get_Int32();

		m_RewardLV = pResult.Get_Int32();

		m_RewardGID = pResult.Get_Int32();

		for(int i = 0; i < (int)EEnemyStat.End - 2;i++) {
			m_Stat[i] = pResult.Get_Float();
		}

		pResult.NextReadPos();
		
		for(int i = 0; i < 2; i++) {
			StatType type = pResult.Get_Enum<StatType>();
			if (type == StatType.None) pResult.NextReadPos();
			else m_ReduceStat.Add(new ReduceStat() { Type = type, Val = pResult.Get_Int32() });
		}
		m_BattleRoundLimit = pResult.Get_Int32();

		m_ZombieResIdx = pResult.Get_Int32();

		for (int i = 0; i < 3; i++) {
			SND_IDX sidx = pResult.Get_Enum<SND_IDX>();
			if (sidx != SND_IDX.NONE) m_HitVoice.Add(sidx);
		}

		m_DropCardGid = pResult.Get_Int32();

		m_AllGroup = pResult.Get_Boolean();
		m_RewardCancle = pResult.Get_Boolean();
		m_Deadly = pResult.Get_Boolean();

		for (int i = 0; i < 2; i++) {
			SND_IDX sidx = pResult.Get_Enum<SND_IDX>();
			if (sidx != SND_IDX.NONE) m_RescueVoice.Add(sidx);
		}
		for (int i = 0; i < 2; i++) m_NoteSatDecMinMax[i] = pResult.Get_Int32();

		m_OnlyDebuffBox = pResult.Get_Boolean();
	}
	/// <summary> 스탯, enemyleveltable 에서 참조 </summary>
	public int GetStat(EEnemyStat stat, int LV = 1)
	{
		switch (stat) {
			case EEnemyStat.HIDING:
				return Mathf.RoundToInt(m_Stat[(int)stat]);
			default: return Mathf.RoundToInt(BaseValue.GetStat(LV, m_Stat[(int)stat] * TDATA.GetEnemyLevelTable(LV).GetStat(stat)));
		}
		//return m_Stat[(int)stat].Value + (LV - 1) * m_Stat[(int)stat].Up;
	}

	public int GetAtkCnt()
	{
		return UTILE.Get_Random(m_AtkCnt[0], m_AtkCnt[1] + 1);
	}
	public float GetNextAtkTime()
	{
		return UTILE.Get_Random(m_AtkTime[0], m_AtkTime[1]);
	}

	public float GetDefTime()
	{
		return UTILE.Get_Random(m_DefTime[0], m_DefTime[1]);
	}

	public float GetNextNoteTime()
	{
		return Mathf.Max(0.1f, UTILE.Get_Random(m_NoteTime[0], m_NoteTime[1]));
	}

	public int GetCreateNoteCnt()
	{
		return UTILE.Get_Random(m_NoteCnt[0], m_NoteCnt[1]);
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	/// <summary> 타입이나 공격, 감소스탯 등에 따른 정보, 최대 3개 </summary>
	public List<string> GetDescs(TStageCardTable _card = null) {
		List<string> descs = new List<string>();
		TEnemyStageSkillTable table = TDATA.GetEnemyStageSkillTableGroup(m_Skill.m_GroupID);
		StringBuilder desc = new StringBuilder();
		//1순위
		if(m_Deadly) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, table.m_Range > 0 ? 45004 : 45000));
		if (m_OnlyDebuffBox) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45002));
		if (table.m_Range > 0 && !m_Deadly) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45001));
		if (_card != null) {
			if (GetMoveAIType() == EEnemyAIMoveType.Arsonist01) {
				if(TDATA.GetStageCardTable((int)_card.m_Value2)?.m_Type == StageCardType.Fire) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45003));
			}
		}
		switch (m_Type) {
			case EEnemyType.AllRefugee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45100)); break;
			case EEnemyType.RandomRefugee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45101)); break;
			case EEnemyType.HpRefugee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45102)); break;
			case EEnemyType.MenRefugee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45103)); break;
			case EEnemyType.HygRefugee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45104)); break;
			case EEnemyType.SatRefugee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45105)); break;
			case EEnemyType.MaterialRefugee:
				int idx = 0;
				switch (m_RewardGID) {
					case 0: idx = 45200; break;
					case 1: idx = 45201; break;
					case 2: idx = 45202; break;
					case 3: idx = 45203; break;
					case 4: idx = 45204; break;
					case 5: idx = 45205; break;
					case 6: idx = 45206; break;
					case 7: idx = 45207; break;
					case 8: idx = 45208; break;
				}
				descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, idx)); break;
			case EEnemyType.Allinfectee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45300)); break;
			case EEnemyType.RandomInfectee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45301)); break;
			case EEnemyType.HpInfectee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45302)); break;
			case EEnemyType.MenInfectee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45303)); break;
			case EEnemyType.HygInfectee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45304)); break;
			case EEnemyType.SatInfectee: descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45305)); break;
		}
		//2순위
		if (table.m_ReduceStat[(int)StatType.Men] > 1) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45400));
		if (table.m_ReduceStat[(int)StatType.Hyg] > 1) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45401));
		if (table.m_ReduceStat[(int)StatType.Sat] > 1) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45402));
		ReduceStat rs = m_ReduceStat.Find(o => o.Type== StatType.Men);
		if(rs != null) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45500)); 
		rs = m_ReduceStat.Find(o => o.Type == StatType.Hyg);
		if (rs != null) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45501)); 
		rs = m_ReduceStat.Find(o => o.Type == StatType.Sat);
		if (rs != null) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45502));
		//3순위
		if(m_Stat[(int)EEnemyStat.ATK] > 10) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45600));
		if (m_Stat[(int)EEnemyStat.HP] > 999) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45601));
		if (m_Stat[(int)EEnemyStat.SPD] > 100) descs.Add(TDATA.GetString(ToolData.StringTalbe.Etc, 45602));

		return descs;
	}
	public string GetNotePrefabName() {
		return Utile_Class.GetFileName(m_PrefabName);
	}
	public EnemyNoteTable GetNote(List<EnemyNoteTable> _prepick = null)
	{
		if (_prepick == null) return m_Note.GetRandNoteTable();
		else return m_Note.GetNotOverrideNotTable(_prepick);
	}

	public EnemyNoteTable GetRunNote()
	{
		return m_Note.RunNote;
	}

	public int GetAtkGroupPos()
	{
		return m_Skill.GetRandSkill();
	}

	public EnemySkillTable GetSkill(int pos)
	{
		return m_Skill.List[pos];
	}
	/// <summary> 피난민인지 여부 </summary>
	public bool ISRefugee()
	{
		switch(m_Type)
		{
			case EEnemyType.SatRefugee:
			case EEnemyType.MenRefugee:
			case EEnemyType.HygRefugee:
			case EEnemyType.HpRefugee:
			case EEnemyType.RandomRefugee:
			case EEnemyType.AllRefugee:
			case EEnemyType.SatInfectee:
			case EEnemyType.MenInfectee:
			case EEnemyType.HygInfectee:
			case EEnemyType.HpInfectee:
			case EEnemyType.RandomInfectee:
			case EEnemyType.Allinfectee:
			case EEnemyType.MaterialRefugee:
				return true;
		}
		return false;
	}
	public bool ISNotAtkRefugee() {
		switch (m_Type) {
			case EEnemyType.SatRefugee:
			case EEnemyType.MenRefugee:
			case EEnemyType.HygRefugee:
			case EEnemyType.HpRefugee:
			case EEnemyType.RandomRefugee:
			case EEnemyType.MaterialRefugee:
				return true;
		}
		return false;
	}
	public bool IS_BadRefugee() {
		switch (m_Type) {
			case EEnemyType.SatRefugee:
			case EEnemyType.MenRefugee:
			case EEnemyType.HygRefugee:
			case EEnemyType.HpRefugee:
			case EEnemyType.RandomRefugee:
			case EEnemyType.AllRefugee:
			case EEnemyType.MaterialRefugee:
				return false;
			case EEnemyType.SatInfectee:
			case EEnemyType.MenInfectee:
			case EEnemyType.HygInfectee:
			case EEnemyType.HpInfectee:
			case EEnemyType.RandomInfectee:
			case EEnemyType.Allinfectee:
				return true;
		}
		return false;
	}

	public bool ISAtkMoveType()
	{
		return GetMoveAIType() != EEnemyAIMoveType.Coward;
	}
	public EEnemyAIMoveType GetMoveAIType() {
		if (ISGoEnemyDebuff()) m_MoveAI.m_Type = EEnemyAIMoveType.Stalker;
		return m_MoveAI.m_Type;
	}
	public bool ISGoEnemyDebuff() {
		return !ISRefugee() && STAGE_USERINFO.ISBuff(StageCardType.GoEnemy);
	}

	public int GetBTReduceStat(StatType _stat, int _lv) {
		//TEnemyStageSkillTable table =  TDATA.GetEnemyStageSkillTableGroup(m_Skill.m_GroupID);
		//return table.Get_SrvDmg(_stat);
		ReduceStat reducestat = m_ReduceStat.Find(t => t.Type == _stat);
		return reducestat != null ? Mathf.RoundToInt(reducestat.Val * TDATA.GetEnemyLevelTable(_lv).GetStat(EEnemyStat.DEADSURVSTAT)) : 0;
	}
}
public class TEnemyTableMng : ToolFile
{
	public Dictionary<int, TEnemyTable> DIC_Idx = new Dictionary<int, TEnemyTable>();
	public TEnemyTableMng() : base("Datas/EnemyTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEnemyTable data = new TEnemyTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemyTable
	TEnemyTableMng m_Enemy = new TEnemyTableMng();

	public TEnemyTable GetEnemyTable(int idx) {
		if (!m_Enemy.DIC_Idx.ContainsKey(idx)) return null;
		return m_Enemy.DIC_Idx[idx];
	}

	public List<int> GetEnemyIdxs()
	{
		return new List<int>(m_Enemy.DIC_Idx.Keys);
	}
}

