using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ESkillATKType
{
	/// <summary> 없음</summary>
	None
	/// <summary> 타격</summary>
	,Hit
	/// <summary> 잡기</summary>
	,Catch
	/// <summary> 연사</summary>
	,Continuous
	/// <summary> 타겟이펙트이동형</summary>
	,Move
	/// <summary> 물기 공격</summary>
	,Bite
	/// <summary> 일반 타격</summary>
	,Attack
	/// <summary> 베는 공격</summary>
	,Slash
	/// <summary> 타액 뱉기</summary>
	,Spit
	/// <summary> 할퀴기 </summary>
	,Scratch
	/// <summary> 여러 번 연속 타격 </summary>
	,MultiHit
	/// <summary> 여러 번 연속 물기</summary>
	,MultiBite
	/// <summary> 여러 번 연속 타격</summary>
	,MultiAttack
	/// <summary>  여러 번 연속 베기</summary>
	,MultiSlash
	/// <summary>  여러 번 연속 할퀴기 </summary>
	,MultiScratch
	/// <summary> 좀비 물기</summary>
	,ZombieBite
	/// <summary> 여러 번 연속 좀비 물기</summary>
	,ZombieMultiBite
	/// <summary> 여러 번 연속 타액 뱉기</summary>
	, ZombieMultiSpit
	, End
}
public enum ESkillType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 강탈 - 확률적으로 플레이어가 가진 [재료] 아이템 하나를 제거합니다. </summary>
	Steal,
	End
}
public enum ESkillFontSizeType
{
	/// <summary> 없음 </summary>
	None,
	/// <summary> 작음 </summary>
	Small,
	/// <summary> 중간 </summary>
	Medium,
	/// <summary> 큼 </summary>
	Large
}

public class TEnemyStageSkillTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 에너미스킬그룹인덱스.  </summary>
	public int m_EnemySkillGID;
	/// <summary> 에너미스킬확률. </summary>
	public int m_Prob;
	/// <summary> 스킬공격타입.(공격 이팩트 연출용) </summary>
	public ESkillATKType m_ATKType;
	/// <summary> 스킬 이펙트 이름 </summary>
	public List<string> m_FXNames = new List<string>();
	/// <summary> 스킬 효과 </summary>
	public ESkillType m_SkillType;
	/// <summary> 스킬공격시 대미지폰트사이즈. </summary>
	public ESkillFontSizeType m_FontSize;
	/// <summary> 공격 거리 (대각선은 세지 않음) </summary>
	public int m_Range;
	/// <summary> 데미지 비율 </summary>
	public float m_DmgRatio;
	/// <summary> 한번에 나오는 공격이펙트 개수 최소 최대. </summary>
	public int[] m_AtkCnt = new int[2];
	/// <summary> 스킬 효과 </summary>
	public List<int> m_SkillVal = new List<int>();
	/// <summary> 에너미에게 공격 받을시 감소하는 정신력, 위생, 허기 수치. </summary>
	public int[] m_ReduceStat = new int[(int)StatType.SurvEnd];

	public int Get_SrvDmg(StatType type) {
		return m_ReduceStat[(int)type];
	}


	public TEnemyStageSkillTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_EnemySkillGID = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
		m_ATKType = pResult.Get_Enum<ESkillATKType>();
		for(int i = 0; i < 3; i++) {
			string name = pResult.Get_String();
			if (name.Equals("")) continue;
			m_FXNames.Add(name);
		}
		m_SkillType = pResult.Get_Enum<ESkillType>();
		m_FontSize = pResult.Get_Enum<ESkillFontSizeType>();
		m_Range = pResult.Get_Int32();
		m_DmgRatio = pResult.Get_Float();
		for (int i = 0; i < 2; i++) {
			m_AtkCnt[i] = pResult.Get_Int32();
		}
		for(int i = 0; i < 3; i++) {
			int val = pResult.Get_Int32();
			if (val != 0) m_SkillVal.Add(val);
		}
		for (int i = 0; i < 3; i++) {
			m_ReduceStat[i] = pResult.Get_Int32();
		}
	}
}
public class TEnemyStageSkillTableMng : ToolFile
{
	public Dictionary<int, TEnemyStageSkillTable> DIC_SkillGID = new Dictionary<int, TEnemyStageSkillTable>();

	public TEnemyStageSkillTableMng() : base("Datas/EnemyStageSkillTable")
	{
	}

	public override void CheckData() { }

	public override void DataInit()
	{
		DIC_SkillGID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEnemyStageSkillTable data = new TEnemyStageSkillTable(pResult);
		if (!DIC_SkillGID.ContainsKey(data.m_EnemySkillGID))
			DIC_SkillGID.Add(data.m_EnemySkillGID, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemyStageSkillTable
	TEnemyStageSkillTableMng m_EnemyStageSkill = new TEnemyStageSkillTableMng();

	public TEnemyStageSkillTable GetEnemyStageSkillTableGroup(int _gid) {
		if (!m_EnemyStageSkill.DIC_SkillGID.ContainsKey(_gid)) return null;
		return m_EnemyStageSkill.DIC_SkillGID[_gid];
	}
}

