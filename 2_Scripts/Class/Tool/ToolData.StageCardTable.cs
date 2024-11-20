using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TStageCardAppearInfo
{
	public StageCardAppearType m_Type;
	public int m_Value;
	public int m_Cnt;
}
public class TStageCardTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 해당 카드가 등장하는 Stage ID </summary>
	public int m_Stage;
	/// <summary> 카드 이름(String Table 참조) </summary>
	public int m_Name;
	/// <summary> 카드 설명(String Table 참조) </summary>
	public int m_Info;
	/// <summary> 카드 타입 </summary>
	public StageCardType m_Type;
	/// <summary> 카드 등장 조건 </summary>
	public TStageCardAppearInfo m_Appear = new TStageCardAppearInfo();
	/// <summary> 조건 만족 시 새로운 라인에서 등장할 확률 </summary>
	public int m_Prob;
	/// <summary> 어둠카드일때의 다른 카드들의 등장 확률 </summary>
	public int m_DarkProb;
	/// <summary> 한 페이지(10줄) 내에 최대 등장 가능 개수 0이면 상관않함</summary>
	public int m_LimitCount;
	/// <summary> 카드 타입 값 </summary>
	public float m_Value1;
	/// <summary> 카드 타입 값 </summary>
	public float m_Value2;
	/// <summary> 한 라인을 전부 채우는 타입 카드 </summary>
	public bool m_IsEndType;
	/// <summary> 카드 이미지 명 </summary>
	public string m_Img;
	public TStageCardAppearInfo m_Disappear = new TStageCardAppearInfo();
	public bool m_AutoGetBuff;
	public TStageCardTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Stage = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Info = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<StageCardType>();
		m_Appear.m_Type = pResult.Get_Enum<StageCardAppearType>();
		m_Appear.m_Value = pResult.Get_Int32();
		m_Appear.m_Cnt = pResult.Get_Int32();
		m_Prob = pResult.Get_Int32();
		m_DarkProb = pResult.Get_Int32();
		m_LimitCount = pResult.Get_Int32();
		m_Value1 = pResult.Get_Float();
		m_Value2 = pResult.Get_Float();
		m_IsEndType = pResult.Get_Boolean();
		m_Img = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Img) && !m_Img.Contains("/"))
			Debug.LogError($"[ StageCardTable ({m_Idx}) ] m_Img 패스 체크할것");
#endif
		m_Disappear.m_Type = pResult.Get_Enum<StageCardAppearType>();
		m_Disappear.m_Value = pResult.Get_Int32();
		m_Disappear.m_Cnt = pResult.Get_Int32();
		m_AutoGetBuff = pResult.Get_Boolean();
	}

	public int GetProb() {
		//난이도별 추가 적 등장 확률
		int val = m_Prob;
		if (m_Type == StageCardType.Enemy) {
			if (val > 0) {
				val += STAGEINFO.m_TStage.m_AddEnemyProb;
				if(!TDATA.GetEnemyTable(Mathf.RoundToInt(m_Value1)).ISRefugee())
					val += Mathf.RoundToInt(val * STAGE_USERINFO.GetBuffValue(StageCardType.EnemyProbUp));
			}
		}
		return val;
	}
	/// <summary> 일반이나 어둠일떄 중 더 높은 확률 /// </summary>
	public int GetHighProb() {
		return Mathf.Max(GetProb(), m_DarkProb);
	}
	public string GetName(int? value1 = null, int? value2 = null)
	{
		int v1 = value1 == null ? Mathf.RoundToInt(m_Value1 * 100f) : value1.Value;
		int v2 = value2 == null ? Mathf.RoundToInt(m_Value1) : value2.Value;
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Name), v1, v2);
	}
	/// <summary> 도박, 버프알람 등 m_Name이 아닌 m_Type으로 쓰는 공용 이름</summary>
	public string GetNameBuffType(int? value1 = null, int? value2 = null) {
		int v1 = value1 == null ? Mathf.RoundToInt(m_Value1 * 100f) : value1.Value;
		int v2 = value2 == null ? Mathf.RoundToInt(m_Value1) : value2.Value;
		int etcidx = 0;
		if (m_Type == StageCardType.Material) {
			switch (Mathf.RoundToInt(m_Value1)) {
				case 0: etcidx = 30074; break;
				case 1: etcidx = 30075; break;
				case 2: etcidx = 30076; break;
				case 3: etcidx = 30077; break;
				case 4: etcidx = 30078; break;
				case 5: etcidx = 30079; break;
				case 6: etcidx = 30080; break;
				case 7: etcidx = 30081; break;
			}
			return string.Format("{0} x{1}",TDATA.GetString(ToolData.StringTalbe.Etc, etcidx), Mathf.RoundToInt(m_Value2));
		}
		else {
			switch (m_Type) {
				/// <summary> HP 최대 증가 </summary>
				case StageCardType.HpUp: etcidx = 30174; break;
				/// <summary> 공격력 상승 </summary>
				case StageCardType.AtkUp: etcidx = 30171; break;
				/// <summary> 방어력 상승 </summary>
				case StageCardType.DefUp: etcidx = 30172; break;
				/// <summary> 기력 회복 속도 증가 </summary>
				case StageCardType.EnergyUp: etcidx = 30173; break;
				/// <summary> 포만도 증가 </summary>
				case StageCardType.SatUp: etcidx = 30176; break;
				/// <summary> 청결도 증가 </summary>
				case StageCardType.HygUp: etcidx = 30177; break;
				/// <summary> 정신력 증가 </summary>
				case StageCardType.MenUp: etcidx = 30178; break;
				/// <summary> 속도 증가 </summary>
				case StageCardType.SpeedUp: etcidx = 30180; break;
				/// <summary> 크리티컬 확률 증가 </summary>
				case StageCardType.CriticalUp: etcidx = 30181; break;
				/// <summary> 크리티컬 데미지 증가 </summary>
				case StageCardType.CriticalDmgUp: etcidx = 30182; break;
				/// <summary> 행동력 회복량 증가 </summary>
				case StageCardType.APRecoveryUp: etcidx = 30185; break;
				/// <summary> 행동력 소모량 감소 </summary>
				case StageCardType.APConsumDown: etcidx = 30186; break;
				/// <summary> 체력 회복량 증가 </summary>
				case StageCardType.HealUp: etcidx = 30187; break;
				/// <summary> 캐릭터, 장비 레벨 증가 </summary>
				case StageCardType.LevelUp: etcidx = 30188; break;
				/// <summary> 타임어택 시간이 증가 </summary>
				case StageCardType.TimePlus: etcidx = 30165; break;
				/// <summary> 헤드샷 확률 증가 </summary>
				case StageCardType.HeadShotUp: etcidx = 30183; break;
				case StageCardType.RecoveryHp: etcidx = 30174; break;
				case StageCardType.RecoveryHpPer: etcidx = 30174; break;
				case StageCardType.RecoveryMen: etcidx = 30178; break;
				case StageCardType.RecoveryHyg: etcidx = 30177; break;
				case StageCardType.RecoverySat: etcidx = 30176; break;
				case StageCardType.PerRecoveryMen: etcidx = 34203; break;
				case StageCardType.PerRecoveryHyg: etcidx = 34202; break;
				case StageCardType.PerRecoverySat: etcidx = 34201; break;
				case StageCardType.RecoveryAP: etcidx = 30179; break;
				case StageCardType.AddGuard: etcidx = 30098; break;
				case StageCardType.LimitTurnUp: etcidx = 30184; break;
				case StageCardType.AddRerollCount: etcidx = 30218; break;
				case StageCardType.BanAirStrike: etcidx = 30266; break;
				case StageCardType.Jump: etcidx = 30267; break;
			}
			return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, etcidx), v1, v2);
		}
	}
	public string GetInfo(int? value1 = null, int? value2 = null)
	{
		int v1 = value1 == null ? Mathf.RoundToInt(m_Value1 * 100f) : value1.Value;
		int v2 = value2 == null ? Mathf.RoundToInt(m_Value1) : value2.Value;
		if (m_Type == StageCardType.Material && STAGE_USERINFO.ISBuff(StageCardType.RandomMaterial))
			return "???";
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Info), v1, v2);
	}
	public string GetOnlyInfo() {
		if (m_Info == 0) return string.Empty;
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Info);
	}
	public Sprite GetImg() {
		return UTILE.LoadImg(m_Img, "png");
	}
	public bool IS_LineCard()
	{
		switch (m_Type)
		{
		case StageCardType.Chain:
			return true;
		}
		return m_IsEndType;

	}

	public bool IS_BuffCard() {
		switch (m_Type) {
			/// <summary> HP 최대 증가 </summary>
			case StageCardType.HpUp:
			/// <summary> 공격력 상승 </summary>
			case StageCardType.AtkUp:
			/// <summary> 방어력 상승 </summary>
			case StageCardType.DefUp:
			/// <summary> 기력 회복 속도 증가 </summary>
			case StageCardType.EnergyUp:
			/// <summary> 포만도 증가 </summary>
			case StageCardType.SatUp:
			/// <summary> 청결도 증가 </summary>
			case StageCardType.HygUp:
			/// <summary> 정신력 증가 </summary>
			case StageCardType.MenUp:
			case StageCardType.Synergy:
			/// <summary> 속도 증가 </summary>
			case StageCardType.SpeedUp:
			/// <summary> 크리티컬 확률 증가 </summary>
			case StageCardType.CriticalUp:
			/// <summary> 크리티컬 데미지 증가 </summary>
			case StageCardType.CriticalDmgUp:
			/// <summary> 행동력 회복량 증가 </summary>
			case StageCardType.APRecoveryUp:
			/// <summary> 행동력 소모량 감소 </summary>
			case StageCardType.APConsumDown:
			/// <summary> 체력 회복량 증가 </summary>
			case StageCardType.HealUp:
			/// <summary> 캐릭터, 장비 레벨 증가 </summary>
			case StageCardType.LevelUp:
			/// <summary> 타임어택 시간이 증가 </summary>
			case StageCardType.TimePlus:
			/// <summary> 헤드샷 확률 증가 </summary>
			case StageCardType.HeadShotUp:
			/// <summary> "구멍 뚫린 가방 : 머지 슬롯 N칸 사용 불가 (보통 1칸으로 설정 예정)" </summary>
			case StageCardType.ConMergeSlotDown:
			case StageCardType.MergeSlotCount:
			/// <summary> "제한된 선택 : 해당 스테이지 필드 위 모든 재료 카드 개수 N개로 일괄 변경 (보통 1개로 설정 예정)" </summary>	
			case StageCardType.MaterialCountDown:
			/// <summary> "부상 : 회복력 N% 감소(보통 90%로 설정 예정)" </summary>
			case StageCardType.Wound:
			/// <summary> "신중한 선택 : 제한적인 행동력 컨셉 시작 행동력 : N 턴마다 행동력 회복 : 0 (고정) (보통 시작 행동력 100, 턴 당 행동력 회복 0으로 설정 예정)" </summary>
			case StageCardType.ConActiveAP:
			/// <summary> 암흑 : 해당 스테이지 내 남은 턴 수 보이지 않음 </summary>
			case StageCardType.DarkTurn:
			/// <summary> 아직 한 발 남았다 : 캐릭터 액티브 스킬 사용 횟수 N회 제한 </summary>
			case StageCardType.ConActiveSkillLimit:
			/// <summary> 어색한 움직임 : 액티브 스킬 사용 시 행동력 소모 값 증가 </summary>
			case StageCardType.ApPlus:
			/// <summary> 잡식성 : 스테이지 내 재료 카드의 내용이 보이지 않고, 카드 선택 시 랜덤 종류의 재료, 랜덤 개수로 지급 </summary>
			case StageCardType.RandomMaterial:
			/// <summary>  버프 알레르기 : 버프 카드 습득해도 버프가 적용되지 않음 </summary>
			case StageCardType.HateBuff:
			/// <summary> "N턴 마다 0번째 라인의 카드 선택이 랜덤으로 자동 선택 됨 예) Value01에 5라고 입력 시, 5번째 턴에 랜덤으로 자동 선택" </summary>
			case StageCardType.ConRandomChoice:
			/// <summary> "망각 : N턴 마다 머지 탭의 모든 아이템이 사라짐 예) Value01에 5라고 입력 시, 5번째 턴에 랜덤으로 자동 선택" </summary>
			case StageCardType.MergeDelete:
			/// <summary>  합심 : 더 이상 에너미들이 서로를 공격하지 않음 (위치 변경만) </summary>
			case StageCardType.ConEnemyTeamwork:
			/// <summary> 무게 초과 : N턴 마다 한 라인 선택을 스킵하고 다음 라인으로 넘어감 </summary>
			case StageCardType.ConSkipTurn:        
			/// <summary> "무너진 길 : 매 턴 마다 0번째 라인 카드 중 N개 선택 불가 (StageTable의 PlayType_CardLock과 동일 기능)" </summary>
			case StageCardType.ConStageCardLock:
			/// <summary>  차선책 : 전투 보상이 랜덤으로 N개 선택 불가 </summary>
			case StageCardType.ConRewardCardLock:
			/// <summary> "블라인드 : 전투 보상 습득 페이지에서 전투 보상 내용 확인 불가 예) 카드 이미지와 설명 모두 확인 불가" </summary>
			case StageCardType.ConBlindRewardInfo:
			/// <summary> 행동 불능 : 랜덤으로 N명의 캐릭터 액티브 스킬 사용 불가 (스테이터스는 적용) </summary>
			case StageCardType.ConKnockDownChar:
			/// <summary> "고장난 기계 : 재료를 모아 머지 탭에서 제작 시 N% 확률로 제작 실패 제작 실패 시 제작에 사용된 모든 재료는 사라짐" </summary>
			case StageCardType.MergeFailChance:
			/// <summary> 출혈 : 액티브 스킬을 사용할 때 마다 HP 감소 (매번 감소) </summary>
			case StageCardType.SkillHp:
			/// <summary>  울렁증 : 액티브 스킬 사용할 때 마다 모든 스테이터스 감소 (매번 감소) </summary>
			case StageCardType.SkillStatus:
			/// <summary> 해당 스테이지의 1단계 아이템 제작 요구 재료 개수가 N개씩 증가 </summary>
			case StageCardType.MoreMaterial:
			/// <summary> 하단 체력바가 보이지 않음 </summary>
			case StageCardType.NoHpBar:
			/// <summary> 에너미 등장 확률이 대폭 상승함 </summary>
			case StageCardType.EnemyProbUp:
			/// <summary> HP 자동 회복 삭제 </summary>
			case StageCardType.NoAutoHeal:
			/// <summary> 청결, 포만, 정신이 턴마다 일정 수치(value1) 감소 (백분율) </summary>
			case StageCardType.TurnAllStatus:
			/// <summary> 생존스탯(value1)이 턴마다 일정 수치(value2) 감소 (백분율) </summary>
			case StageCardType.TurnStatusMen:
			case StageCardType.TurnStatusHyg:
			case StageCardType.TurnStatusSat:
			case StageCardType.TurnStatusHp:
			/// <summary> 에너미가 무조건 전진 </summary>
			case StageCardType.GoEnemy:
			/// <summary> 스테이지 내 모든 몬스터 즉사 몬스터(value1)로 변경 </summary>
			case StageCardType.ConDeadly:
				return true;
		}
		return false;
	}
	
	public bool IS_NotFrameCard() {
		switch (m_Type) {
			case StageCardType.Roadblock:
			case StageCardType.AllRoadblock:
				return true;
			default:return false;
		}
	}
	public bool IS_CanChangeType(StageCardType _type) {
		if (m_Type == _type) return false;
		if (m_Type == StageCardType.Roadblock || m_Type == StageCardType.AllRoadblock) return false;
		if (m_IsEndType) return false;
		switch (_type) {
			case StageCardType.Fire:
				switch (m_Type) {
					case StageCardType.Ash:
						return false;
					case StageCardType.Material:
						if (m_Value1 == 4)
							return false;
						else return true;
					case StageCardType.PowderExtin:
					case StageCardType.PowderBomb:
					case StageCardType.ThrowExtin:
						return false;
					default: return true;
				}
			default:return true;
		}
	}

	public bool IS_UtileCard() {
		switch (m_Type) {
			case StageCardType.Sniping:
			case StageCardType.Grenade:
			case StageCardType.ShockBomb:
			case StageCardType.StarShell:
			case StageCardType.PowderBomb:
			case StageCardType.Dynamite:
			case StageCardType.LightStick:
			case StageCardType.PowderExtin:
			case StageCardType.ThrowExtin:
			case StageCardType.SmokeBomb:
			case StageCardType.Paralysis:
			case StageCardType.FireBomb:
			case StageCardType.FireGun:
			case StageCardType.NapalmBomb:
			case StageCardType.Drill:
			case StageCardType.RandomAtk:
			case StageCardType.CardPull:
			case StageCardType.Explosion:
			case StageCardType.StopCard:
			case StageCardType.PlusMate:
			case StageCardType.DownLevel:
			case StageCardType.AllShuffle:
			case StageCardType.AllConversion:
			case StageCardType.SupplyBox:
			case StageCardType.Shotgun:
			case StageCardType.MachineGun:
			case StageCardType.AirStrike:
			case StageCardType.C4:
			case StageCardType.TimeBomb:
				return true;
			default:return false;
		}

	}
}

public class TStageCardTableMng : ToolFile
{
	/// <summary> Idx = 챕터 * 100 + 스테이지 번호 1 ~ </summary>
	public Dictionary<int, List<TStageCardTable>> DIC_Group = new Dictionary<int, List<TStageCardTable>>();
	public Dictionary<int, TStageCardTable> DIC_Idx = new Dictionary<int, TStageCardTable>();

	public TStageCardTableMng(string[] Path) : base(Path)
	{
	}

	public override void DataInit()
	{
		DIC_Group.Clear();
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStageCardTable data = new TStageCardTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Group.ContainsKey(data.m_Stage)) DIC_Group.Add(data.m_Stage, new List<TStageCardTable>());
		DIC_Group[data.m_Stage].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StageCardTable
	TStageCardTableMng m_StageCard;

	public void LoadStageCardTable(string filename = null) {
		//기본 테이블만 필요로 하는 곳들이 테이블 전부 부르기 전에 필요한 경우, 이전걸 다 지워버려서 이미 기본이 있으면 패스
		if (string.IsNullOrEmpty(filename) && m_StageCard != null && GetStageCardGroup(0) != null) return;
		List<string> Paths = new List<string>();
		if(!string.IsNullOrEmpty(filename)) Paths.Add($"Datas/{filename}");
		Paths.Add("Datas/Default_StageCardTable");
		m_StageCard = new TStageCardTableMng(Paths.ToArray());
		m_StageCard.Load();
	}


	public List<TStageCardTable> GetStageCardGroup(int StageIdx) {
		if (!m_StageCard.DIC_Group.ContainsKey(StageIdx)) return null;
		return m_StageCard.DIC_Group[StageIdx];
	}

	public TStageCardTable GetStageCardTable(int CardIdx)
	{
		if (!m_StageCard.DIC_Idx.ContainsKey(CardIdx)) return null;
		return m_StageCard.DIC_Idx[CardIdx];
	}
	/// <summary> 재료카드들 인덱스 모음 </summary>
	public List<int> GetMaterialCardIdxs(int _pos) {
		List<int> idxs = new List<int>();
		List<TStageCardTable> tables = m_StageCard.DIC_Group[_pos].FindAll((t) => t.m_Type == StageCardType.Material);
		for(int i = 0; i < tables.Count; i++) {
			idxs.Add(tables[i].m_Idx);
		}
		return idxs;
	}
#pragma warning disable 0168
	public JobType GetSynergyType(StageCardType type)
	{
		string[] split = type.ToString().Split('_');

		string value = split[0];
		if (value == null || value.Length < 1) return JobType.None;
		try
		{
			return (JobType)(object)Enum.Parse(typeof(JobType), value, true);
		}
		catch (Exception e)
		{
			return JobType.None;
		}
	}
#pragma warning restore 0168
}

