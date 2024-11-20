using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TStageCondition<T> : ClassMng
{
	public T m_Type;
	public int m_Value;
	public float m_Cnt;
	public string m_IconCard;   // StageClearType 조건에서만 사용

	public Sprite GetIcon_Card() {
		return UTILE.LoadImg(m_IconCard, "png");
	}
}
public class TStageTable : ClassMng
{
	public class ClearReward
	{
		public RewardKind m_Kind;
		public int m_Idx;
		public int m_Count;
	}
	public class StagePlayType
	{
		public StagePlayType(PlayType _type, int _val1, int _val2) {
			m_Type = _type;
			m_Val[0] = _val1;
			m_Val[1] = _val2;
		}
		public PlayType m_Type;
		public int[] m_Val = new int[2];
	}
	public class Gimmick
	{
		public string Img;
		public int Name;
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 다음 스테이지 인덱스 </summary>
	public int m_NextIdx;
	/// <summary> 스테이지 진행 유형 </summary>
	public StageModeType m_Mode;
	/// <summary> 스테이지 난이도 유형 </summary>
	public StageDifficultyType m_DifficultyType;
	/// <summary> 스테이지 레벨 </summary>
	public int m_LV;
	/// <summary> 스테이지 명 - StringTable_Stage의 Index 참조 </summary>
	public int m_Name;
	/// <summary> 스테이지 목적 설명 - StringTable_Stage의 Index 참조 </summary>
	public int m_Info;
	/// <summary> 스테이지 실패 설명 </summary>
	public List<int> m_FailInfo = new List<int>();
	/// <summary> 스테이지 메인 이미지 - Prefab 명 - \Assets\Resources\Prefabs\Item\MainMenu\Stg_BG </summary>
	public string m_PrefabName;
	/// <summary> 환경 이펙트</summary>
	public List<BGMaskType> m_BGs = new List<BGMaskType>();
	/// <summary> 어둠 시작 단계</summary>
	public int m_DarkLv;
	/// <summary> 추천 직업 - 파티 세팅 시 해당 캐릭터에 추천 마크 출력 </summary>
	public List<JobType> m_RecommendJob = new List<JobType>();
	/// <summary> 사용 할 수 없는 직업 </summary>
	public List<JobType> m_LockJob = new List<JobType>();
	/// <summary> 출현하는 기믹 </summary>
	public List<Gimmick> m_Gimmick = new List<Gimmick>();
	/// <summary> 목표가 단일, 연속, 동시인지 구분 </summary>
	public ClearMethodType m_ClearMethod;
	/// <summary> 승리 조건 모두 만족 해야됨 </summary>
	public List<TStageCondition<StageClearType>> m_Clear = new List<TStageCondition<StageClearType>>();
	/// <summary> 패배 조건 </summary>
	public TStageCondition<StageFailType> m_Fail = new TStageCondition<StageFailType>();
	/// <summary> 시작 시 비율 </summary>
	public float[] m_Stat = new float[(int)StatType.Atk];
	/// <summary> 시작 시 AP </summary>
	public int m_AP;
	/// <summary> 시작 소모 에너지 </summary>
	public int m_Energy;
	/// <summary> 0 : 시작 토크, 1 : 종료 토크 </summary>
	public int[] m_TalkDlg = new int[2];

	/// <summary> 0 : 시작 시간, 1 : 종료 시간</summary>
	public int m_StartTime;

	/// <summary> 턴당 감소 스탯,멘탈-위생-허기 순 </summary>
	public int[] m_ReduceStat = new int[3];

	/// <summary> 지급되는 경험치로 중앙 경험치 저장고에 누적 </summary>
	public int m_ClearExp;
	/// <summary> 지급되는 유저 겨험치 </summary>
	public int m_ClearUserExp;
	/// <summary> 지급되는 달러 </summary>
	public int m_ClearMoney;
	/// <summary> 지급되는 골드 </summary>
	public int m_ClearGold;
	/// <summary> 지급되는 아이템 </summary>
	public List<ClearReward> m_ClearReward = new List<ClearReward>();
	/// <summary> 돌발 이벤트 사용유무 </summary>
	public bool m_ClearEvent;
	/// <summary> 시간별 레벨 증가량 </summary>
	public float m_AddEnemyLV;
	/// <summary> 스테이지 턴수제한 </summary>
	public int m_LimitTurn;
	/// <summary> 스테이지 배경 </summary>
	public string m_StageBG;
	/// <summary> 메인화면 스테이지 메뉴 배경 </summary>
	public string m_ChapterBG;
	/// <summary> 권장 전투력 </summary>
	public int m_RecommandCP;
	/// <summary> 챕터 보상 인덱스 </summary>
	public int m_Value;
	/// <summary> 클리어 목표 </summary>
	public List<int> m_ClearDesc = new List<int>();
	/// <summary> 턴당 행동력 회복량 </summary>
	public int m_APRecovery;
	/// <summary> 스테이지 플레이 타입 </summary>
	public List<StagePlayType> m_PlayType = new List<StagePlayType>();
	/// <summary> 머지 제작 칸수 </summary>
	public int m_MakingCnt;
	/// <summary> 초기 재굴림 횟수 </summary>
	public int m_ReRollCnt;
	/// <summary> 어려운 스테이지인지, 1번 보상 따로 유아이로 보여줌 </summary>
	public int m_Difficulty;
	/// <summary> 나이트메어, 아포칼립스에서 추가되는 적 등장 확률 </summary>
	public int m_AddEnemyProb;
	/// <summary> 시작 선택 보상 </summary>
	public int m_StartReward;
	/// <summary> 플레이어 레벨, 에너미 레벨 차이 </summary>
	public int m_EnemyLevelRange;
	/// <summary> 배경음 </summary>
	public SND_IDX m_BGM;
	public List<SND_IDX> m_BGV = new List<SND_IDX>();
	/// <summary> 덱 인원 제한 </summary>
	public int m_DeckCharLimit;
	public string[] m_StageImg = new string[2];
	public List<int> m_NeedChars = new List<int>();
	public bool m_NoRescue;
	public bool m_InZombie;
	/// <summary> 긴급미션 보상 </summary>
	public ClearReward[] m_ReplayReward = new ClearReward[2];
	/// <summary> 참조 스테이지 카드 테이블 이름 </summary>
	public string StageCardTableName;

	/// <summary> 실패 보상 캐릭터 경험치 </summary>
	public int m_FailExp;
	/// <summary> 실패 보상 달러 </summary>
	public int m_FailMoney;
	/// <summary> 실패 보상 아이템 </summary>
	public ClearReward m_FailReward;

	public int GetDarkLv { get { return Mathf.Max(0, m_DarkLv); } }
	public bool Is_Dark { get { return m_DarkLv > -1; } }


	public TStageTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32(); 
		m_NextIdx = pResult.Get_Int32();
		m_Mode = pResult.Get_Enum<StageModeType>();
		m_DifficultyType = pResult.Get_Enum<StageDifficultyType>();
		m_LV = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Info = pResult.Get_Int32();
		for (int i = 0; i < 2; i++) {
			int val = pResult.Get_Int32();
			if (val != 0)
				m_FailInfo.Add(val);
		}
		m_PrefabName = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_PrefabName) && !m_PrefabName.Contains("/"))
			Debug.LogError($"[ StageTable ({m_Idx}) ] m_PrefabName 패스 체크할것");
#endif
		for (int i = 0; i < 3; i++) {
			BGMaskType fog = pResult.Get_Enum<BGMaskType>();
			if (fog == BGMaskType.None) continue;
			m_BGs.Add(fog);
		}
		m_DarkLv = pResult.Get_Int32();

		for (int i = 0; i < 3; i++) {
			JobType job = pResult.Get_Enum<JobType>();
			if (job != JobType.None && !m_RecommendJob.Contains(job)) m_RecommendJob.Add(job);
		}

		for (int i = 0; i < 3; i++) {
			JobType job = pResult.Get_Enum<JobType>();
			if (job != JobType.None && !m_LockJob.Contains(job)) m_LockJob.Add(job);
		}

		for (int i = 0; i < 3; i++) {
			string img = pResult.Get_String();
			int name = pResult.Get_Int32();
			if (name != 0)
			{
				m_Gimmick.Add(new Gimmick() { Img = img, Name = name });
#if USE_LOG_MANAGER
				if (!string.IsNullOrEmpty(img) && !img.Contains("/"))
					Debug.LogError($"[ StageTable ({m_Idx}) ] m_Gimmick[{i}] 패스 체크할것");
#endif
			}
		}

		m_ClearMethod = pResult.Get_Enum<ClearMethodType>();

		for (int i = 0; i < 3; i++) {
			StageClearType type = pResult.Get_Enum<StageClearType>();
			if (type == StageClearType.None) {
				pResult.NextReadPos();
				pResult.NextReadPos();
				pResult.NextReadPos();
				continue;
			}
			TStageCondition<StageClearType> condition = new TStageCondition<StageClearType>();
			condition.m_Type = type;
			condition.m_Value = pResult.Get_Int32();
			condition.m_Cnt = pResult.Get_Float();
			condition.m_IconCard = pResult.Get_String();
#if USE_LOG_MANAGER
			if (!string.IsNullOrEmpty(condition.m_IconCard) && !condition.m_IconCard.Contains("/"))
				Debug.LogError($"[ StageTable ({m_Idx}) ] condition[{i}].m_IconCard 패스 체크할것");
#endif
			m_Clear.Add(condition);
		}

		m_Fail.m_Type = pResult.Get_Enum<StageFailType>();
		m_Fail.m_Value = pResult.Get_Int32();
		m_Fail.m_Cnt = pResult.Get_Float();

		m_Stat[(int)StatType.HP] = pResult.Get_Float();
		m_Stat[(int)StatType.Men] = pResult.Get_Float();
		m_Stat[(int)StatType.Hyg] = pResult.Get_Float();
		m_Stat[(int)StatType.Sat] = pResult.Get_Float();
		m_AP = pResult.Get_Int32();
		m_Energy = pResult.Get_Int32();

		m_TalkDlg[0] = pResult.Get_Int32();
		m_TalkDlg[1] = pResult.Get_Int32();

		m_StartTime = pResult.Get_Int32();

		m_ReduceStat[0] = pResult.Get_Int32();
		m_ReduceStat[1] = pResult.Get_Int32();
		m_ReduceStat[2] = pResult.Get_Int32();

		m_ClearExp = pResult.Get_Int32();
		m_ClearUserExp = pResult.Get_Int32();
		m_ClearMoney = pResult.Get_Int32();
		m_ClearGold = pResult.Get_Int32();
		for (int i = 0; i < 4; i++) {
			RewardKind kind = pResult.Get_Enum<RewardKind>();
			int idx = pResult.Get_Int32();
			if (idx != 0 && kind != RewardKind.None)
				m_ClearReward.Add(new ClearReward() {
					m_Kind = kind,
					m_Idx = idx,
					m_Count = pResult.Get_Int32()
				});
			else pResult.NextReadPos();
		}
		m_ClearEvent = pResult.Get_Boolean();
		m_AddEnemyLV = pResult.Get_Float();
		m_LimitTurn = pResult.Get_Int32();
		m_StageBG = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_StageBG) && !m_StageBG.Contains("/"))
			Debug.LogError($"[ StageTable ({m_Idx}) ] m_StageBG 패스 체크할것");
#endif
		m_ChapterBG = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_ChapterBG) && !m_ChapterBG.Contains("/"))
			Debug.LogError($"[ StageTable ({m_Idx}) ] m_ChapterBG 패스 체크할것");
#endif
		m_RecommandCP = pResult.Get_Int32();
		m_Value = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) {
			int desc = pResult.Get_Int32();
			if (desc != 0) m_ClearDesc.Add(desc);
		}
		m_APRecovery = pResult.Get_Int32();

		for (int i = 0; i < 2; i++) {
			PlayType playtype = pResult.Get_Enum<PlayType>();
			if (playtype == PlayType.None) {
				pResult.NextReadPos();
				pResult.NextReadPos();
			}
			else m_PlayType.Add(new StagePlayType(playtype, pResult.Get_Int32(), pResult.Get_Int32()));
		}

		m_MakingCnt = pResult.Get_Int32();
		m_ReRollCnt = pResult.Get_Int32();
		m_Difficulty = pResult.Get_Int32();
		m_AddEnemyProb = pResult.Get_Int32();
		m_StartReward = pResult.Get_Int32();
		m_EnemyLevelRange = pResult.Get_Int32();
		m_BGM = pResult.Get_Enum<SND_IDX>();
		for (int i = 0; i < 3; i++) {
			SND_IDX idx = pResult.Get_Enum<SND_IDX>();
			if (idx != SND_IDX.NONE) m_BGV.Add(idx);
		}
		m_DeckCharLimit = pResult.Get_Int32();
		m_StageImg[0] = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_StageImg[0]) && !m_StageImg[0].Contains("/"))
			Debug.LogError($"[ StageTable ({m_Idx}) ] m_StageImg[0] 패스 체크할것");
#endif
		m_StageImg[1] = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_StageImg[1]) && !m_StageImg[1].Contains("/"))
			Debug.LogError($"[ StageTable ({m_Idx}) ] m_StageImg[1] 패스 체크할것");
#endif
		for(int i = 0; i < 5; i++) {
			int idx = pResult.Get_Int32();
			if (idx != 0) m_NeedChars.Add(idx);
		}
		m_NoRescue = pResult.Get_Boolean();
		m_InZombie = pResult.Get_Boolean();

		for (int i = 0; i < 2; i++)
		{
			m_ReplayReward[i] = new ClearReward()
			{
				m_Kind = pResult.Get_Enum<RewardKind>(),
				m_Idx = pResult.Get_Int32(),
				m_Count = pResult.Get_Int32()
			};
		}
		StageCardTableName = pResult.Get_String();

		m_FailExp = pResult.Get_Int32();
		m_FailMoney = pResult.Get_Int32();
		m_FailReward = new ClearReward()
		{
			m_Kind = pResult.Get_Enum<RewardKind>(),
			m_Idx = pResult.Get_Int32(),
			m_Count = pResult.Get_Int32()
		};
		if (m_Idx == m_NextIdx) m_NextIdx = 0;
	}


	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}

	public string GetInfo()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Info);
	}
	public List<string> GetFailInfo() {
		List<string> str = new List<string>();
		for(int i = 0; i < m_FailInfo.Count; i++) {
			str.Add(TDATA.GetString(ToolData.StringTalbe.Etc, m_FailInfo[i]));
		}
		return str;
	}
	public string GetBGName() {

		return m_PrefabName;// string.Format("{0}{1}", "Item/MainMenu/Stg_BG/", m_PrefabName);
	}
	public Sprite GetWeekDGGuideImg() {
		return UTILE.LoadImg(m_PrefabName, "png");
	}

	public string GetStageBGName()
	{
		return Utile_Class.GetFileName(m_StageBG);
	}

	public Sprite GetStageBG()
	{
		return UTILE.LoadImg(m_StageBG, "png");
	}
	public Sprite GetChapterBG()
	{
		return UTILE.LoadImg(m_ChapterBG, "png");
	}
	public Sprite GetGimmickImg(int pos)
	{
		if (pos < 0 || m_Gimmick.Count <= pos) return null;
		if (m_Gimmick[pos] == null) return null;
		return UTILE.LoadImg(m_Gimmick[pos].Img, "png");
	}
	public string GetGimminkName(int pos) {
		if (pos < 0 || m_Gimmick.Count <= pos) return null;
		if (m_Gimmick[pos] == null) return null;
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Gimmick[pos].Name);
	}
	public Sprite GetImg(bool _big = true) {
		return UTILE.LoadImg(m_StageImg[_big ? 0 : 1], "png");
		//return UTILE.LoadImg(string.Format("BG/StageImage/St_{0}_{1}", (int)(m_Idx / 100) < 10000 ? (int)(m_Idx / 100) : 0, _big ? 1 : 2), "png");
	}
	public string GetClearDesc(int _pos) {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_ClearDesc[_pos]);
	}
	public StagePlayType GetMode(PlayType _type) {
		return m_PlayType.Find((t) => t.m_Type == _type);
	}
}

public class TStageTableMng : ToolFile
{
	/// <summary> 난이도 그룹으로 나눔, Idx = 챕터 * 100 + 스테이지 번호 1 ~ </summary>
	public Dictionary<StageDifficultyType, Dictionary<int, TStageTable>> DIC_Idx = new Dictionary<StageDifficultyType, Dictionary<int, TStageTable>>();
	/// <summary> 모드 첫 등장 여부 </summary>
	public Dictionary<PlayType, KeyValuePair<StageDifficultyType, int>> FirstPlayType = new Dictionary<PlayType, KeyValuePair<StageDifficultyType, int>>();

	public TStageTableMng() : base(new string[] { "Datas/StageTable", "Datas/StageTable_DownTown", "Datas/StageTable_Event" })
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		FirstPlayType.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStageTable data = new TStageTable(pResult);
		if (!DIC_Idx.ContainsKey(data.m_DifficultyType)) DIC_Idx.Add(data.m_DifficultyType, new Dictionary<int, TStageTable>());
		DIC_Idx[data.m_DifficultyType].Add(data.m_Idx, data);
		if (m_NowPath.Equals("Datas/StageTable") || m_NowPath.Equals("Datas/StageTable_DownTown") || m_NowPath.Equals("Datas/StageTable_Event")) {
			if (data.m_Fail.m_Type == StageFailType.TurmoilCount && !FirstPlayType.ContainsKey(PlayType.TurmoilCount)) FirstPlayType.Add(PlayType.TurmoilCount, new KeyValuePair<StageDifficultyType, int>(data.m_DifficultyType, data.m_Idx));
			if (data.m_APRecovery == 0 && !FirstPlayType.ContainsKey(PlayType.APRecvZero)) FirstPlayType.Add(PlayType.APRecvZero, new KeyValuePair<StageDifficultyType, int>(data.m_DifficultyType, data.m_Idx));
			for (int i = 0; i < data.m_PlayType.Count; i++) {
				if (!FirstPlayType.ContainsKey(data.m_PlayType[i].m_Type)) FirstPlayType.Add(data.m_PlayType[i].m_Type, new KeyValuePair<StageDifficultyType, int>(data.m_DifficultyType, data.m_Idx));
			}
		}
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StageTable
	TStageTableMng m_Stage = new TStageTableMng();


	public TStageTable GetStageTable(int idx, int _diffculty = 0)
	{
		if (!m_Stage.DIC_Idx.ContainsKey((StageDifficultyType)_diffculty)) return null;
		if (!m_Stage.DIC_Idx[(StageDifficultyType)_diffculty].ContainsKey(idx)) return null;
		return m_Stage.DIC_Idx[(StageDifficultyType)_diffculty][idx];
	}

	public List<TStageTable> GetChapterStages(int Chapter, int _diffculty = 0)
	{
		List<TStageTable> list = new List<TStageTable>();
		TStageTable table = GetStageTable(Chapter * 100 + 1, _diffculty);
		while (table != null)
		{
			list.Add(table);
			int next = table.m_NextIdx;
			if (next / 100 != Chapter) break;
			if (next == 0 || next == table.m_Idx) break;
			table = GetStageTable(next, _diffculty);
		}
		return list;
	}
	public bool GetStageFirstPlayType(PlayType _type, StageDifficultyType _diff, int _idx) {
		return m_Stage.FirstPlayType[_type].Key == _diff && m_Stage.FirstPlayType[_type].Value == _idx;
	}
}

