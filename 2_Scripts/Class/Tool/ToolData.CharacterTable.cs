using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TCharacterTable : ClassMng
{
	public enum VoiceType
	{
		StageStart,
		StageClear,
		CharInfo,
		InDeck,
		CharGradeUp,
		Skill,
		CharDraw
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름, EtcString </summary>
	public int m_Name;
	/// <summary> 캐릭터 이야기, EtcString </summary>
	public int m_Desc;
	/// <summary> 직업 이름 </summary>
	public int m_Speech;
	/// <summary> 초상화 </summary>
	public string m_Portrait;
	/// <summary> 직업 </summary>
	public List<JobType> m_Job = new List<JobType>();
	/// <summary> 등급 </summary>
	public int m_Grade;
	/// <summary>  스킬, 액티브1-패시브-패시브2-세트효과 순서 </summary>
	public int[] m_SkillIdx = new int[4];
	/// <summary> 스탯 비중치 </summary>
	public Dictionary<StatType, int> m_StatImport = new Dictionary<StatType, int>();
	/// <summary> 캐릭터 조각 인덱스 </summary>
	public int m_PieceIdx;
	/// <summary> 사용 여부 </summary>
	public bool m_Use;
	/// <summary> TConditionDialogueGroupTable Gid </summary>
	public int m_DialogueGroupID;
	/// <summary> 등급 해금 시 스토리 </summary>
	public int[] m_Story = new int[5];
	/// <summary> 성격 타입 </summary>
	public PersonalityType m_Personality;
	public DNABGType[] m_DNABGType = new DNABGType[3];
	public SND_IDX m_HitVocIdx;
	public int m_SerumGroupIdx;
	/// <summary> 0:스테이지 시작, 1:스테이지 클리어, 2:캐릭터 인포, 3:덱 합류, 4:캐릭터 승급, 5:스킬사용 </summary>
	public SND_IDX[] m_CommVocIdx = new SND_IDX[6];
	public PVPPosType m_PVPPosType;
	public PVPArmorType m_PVPArmorType;
	public int m_PVPSkillIdx;
	/// <summary> 해당 스테이지 진입 시, 긴급모집에서 선택가능해짐 0: 기본적으로 선택가능</summary>
	public int m_SelectivePickupStage;

	public TCharacterTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_Speech = pResult.Get_Int32();
		m_Portrait = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Portrait) && !m_Portrait.Contains("/"))
			Debug.LogError($"[ CharacterTable ({m_Idx}) ] m_Portrait 패스 체크할것");
#endif
		for (int i = 0; i < 2; i++) {
			JobType job = pResult.Get_Enum<JobType>();
			if (job != JobType.None)
				m_Job.Add(job);
		}
		m_Grade = pResult.Get_Int32();

		m_SkillIdx[(int)SkillType.Active] = pResult.Get_Int32();
		m_SkillIdx[(int)SkillType.SetActive] = pResult.Get_Int32();
		m_SkillIdx[(int)SkillType.Passive1] = pResult.Get_Int32();
		m_SkillIdx[(int)SkillType.Passive2] = pResult.Get_Int32();


		m_StatImport.Add(StatType.Atk, pResult.Get_Int32());
		m_StatImport.Add(StatType.Def, pResult.Get_Int32());
		m_StatImport.Add(StatType.Heal, pResult.Get_Int32());
		m_StatImport.Add(StatType.Men, pResult.Get_Int32());
		m_StatImport.Add(StatType.Hyg, pResult.Get_Int32());
		m_StatImport.Add(StatType.Sat, pResult.Get_Int32());

		m_PieceIdx = pResult.Get_Int32();
		m_Use = pResult.Get_Boolean();
		m_DialogueGroupID = pResult.Get_Int32();
		for (int i = 0; i < 5; i++) m_Story[i] = pResult.Get_Int32();
		m_Personality = pResult.Get_Enum<PersonalityType>();
		for (int i = 0; i < 3; i++) m_DNABGType[i] = pResult.Get_Enum<DNABGType>();

		m_HitVocIdx = pResult.Get_Enum<SND_IDX>();
		m_SerumGroupIdx = pResult.Get_Int32();
		for(int i = 0; i < 6; i++) {
			m_CommVocIdx[i] = pResult.Get_Enum<SND_IDX>();
		}

		m_PVPPosType = pResult.Get_Enum<PVPPosType>();
		m_PVPArmorType = pResult.Get_Enum<PVPArmorType>();
		m_PVPSkillIdx = pResult.Get_Int32();
		m_SelectivePickupStage = pResult.Get_Int32();
	}

	public string GetCharName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetCharDesc() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
	}
	public string GetSpeech() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Speech);
	}
	public SND_IDX GetVoice(VoiceType _pos, int _trans = -1) {
		List<SND_IDX> snds = new List<SND_IDX>();

		bool female = UTILE.Get_Random(0, 10) > 5;
		if (_trans != -1) female = _trans > 0;
		switch (_pos) {
			case VoiceType.StageStart://0:스테이지 시작
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5191, SND_IDX.VOC_5192 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4291, SND_IDX.VOC_4292 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[0], m_CommVocIdx[1] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			case VoiceType.StageClear://1:스테이지 클리어
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5193, SND_IDX.VOC_5194 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4293, SND_IDX.VOC_4294 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[2], m_CommVocIdx[3] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			case VoiceType.CharInfo://2:캐릭터 인포
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5191, SND_IDX.VOC_5192, SND_IDX.VOC_5193, SND_IDX.VOC_5194 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4291, SND_IDX.VOC_4292, SND_IDX.VOC_4293, SND_IDX.VOC_4294 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[0], m_CommVocIdx[1], m_CommVocIdx[2], m_CommVocIdx[3] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			case VoiceType.InDeck://3:덱 합류
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5191, SND_IDX.VOC_5192, SND_IDX.VOC_5193, SND_IDX.VOC_5194 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4291, SND_IDX.VOC_4292, SND_IDX.VOC_4293, SND_IDX.VOC_4294 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[0], m_CommVocIdx[1], m_CommVocIdx[2], m_CommVocIdx[3] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			case VoiceType.CharGradeUp://4:캐릭터 승급
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5193, SND_IDX.VOC_5194 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4293, SND_IDX.VOC_4294 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[2], m_CommVocIdx[3] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			case VoiceType.Skill://5:스킬사용
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5195, SND_IDX.VOC_5196 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4295, SND_IDX.VOC_4296 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[4], m_CommVocIdx[5] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			case VoiceType.CharDraw:
				if (m_Idx == 1049) {//가브리엘 남매
					if (female) snds = new List<SND_IDX>() { SND_IDX.VOC_5191, SND_IDX.VOC_5192 };
					else snds = new List<SND_IDX>() { SND_IDX.VOC_4291, SND_IDX.VOC_4292 };
				}
				else snds = new List<SND_IDX>() { m_CommVocIdx[0], m_CommVocIdx[1] };
				return snds[UTILE.Get_Random(0, snds.Count)];
			default: return SND_IDX.NONE;
		}
	}
	public SND_IDX GetHitVoice(int _trans = -1) {
		if (m_Idx == 1049) {//가브리엘 남매
			bool female = false;
			if (_trans != -1) female = _trans < 1; 
			return female ? SND_IDX.VOC_5197 : SND_IDX.VOC_4297;
		}
		return m_HitVocIdx;
	}
	public Sprite GetPortrait(string changename = "") {
		if (!string.IsNullOrWhiteSpace(changename)) return UTILE.LoadImg(string.Format("{0}_{1}", m_Portrait, changename), "png");
		return UTILE.LoadImg(m_Portrait, "png");
	}
	public Sprite[] GetJobIcon() {
		Sprite[] icons = new Sprite[m_Job.Count];
		for(int i = 0; i < m_Job.Count; i++) {
			icons[i] = UTILE.LoadImg(string.Format("UI/Icon/JobIcon_{0}", m_Job[i].ToString()), "png");
		}
		return icons;
	}
	public int GetStatImport(StatType _type) {
		if (!m_StatImport.ContainsKey(_type)) return 0;
		return m_StatImport[_type];

	}
	public TConditionDialogueGroupTable GetSpeechTable(DialogueConditionType _type) {
		return TDATA.GetTConditionDialogueGroupTable(m_DialogueGroupID, _type);
	}
	public float GetPassiveStatValue(StatType _type, int _lv) {
		float val = 0f;
		for (int i = (int)SkillType.Passive1; i <= (int)SkillType.Passive2; i++) {
			bool is_cal = false;
			TSkillTable skill = TDATA.GetSkill(m_SkillIdx[i]);
			if (skill == null) continue;
			switch (_type) {
				case StatType.Atk: if (skill.m_Kind == SkillKind.AtkUp) is_cal = true; break;
				case StatType.Def: if (skill.m_Kind == SkillKind.DefUp) is_cal = true; break;
				case StatType.Heal: if (skill.m_Kind == SkillKind.HealUp) is_cal = true; break;
				case StatType.HP: if (skill.m_Kind == SkillKind.TotalHpUp) is_cal = true; break;
			}
			if (is_cal) {
				val += skill.m_Base;
			}
		}

		return Mathf.RoundToInt(BaseValue.GetPassiveStat(val, _lv));
	}
}
public class TCharacterTableMng : ToolFile
{
	public Dictionary<int, TCharacterTable> DIC_Use = new Dictionary<int, TCharacterTable>();
	public Dictionary<int, TCharacterTable> DIC_NotUse = new Dictionary<int, TCharacterTable>();
	public Dictionary<JobType, List<TCharacterTable>> DIC_Job = new Dictionary<JobType, List<TCharacterTable>>();
	public List<TCharacterTable> Tables = new List<TCharacterTable>();
	public TCharacterTableMng() : base("Datas/CharacterTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_Use.Clear();
		DIC_Job.Clear();
		Tables.Clear();
		DIC_NotUse.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TCharacterTable data = new TCharacterTable(pResult);
		if (!data.m_Use)
		{
			DIC_NotUse.Add(data.m_Idx, data);
			return;
		}
		DIC_Use.Add(data.m_Idx, data);
		Tables.Add(data);
		for (int j = 0; j < data.m_Job.Count; j++)
		{
			if (!DIC_Job.ContainsKey(data.m_Job[j]))
				DIC_Job.Add(data.m_Job[j], new List<TCharacterTable>());
			DIC_Job[data.m_Job[j]].Add(data);
		}
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// CharacterTable
	TCharacterTableMng m_Char = new TCharacterTableMng();

	public TCharacterTable GetCharacterTable(int idx) {
		if (!m_Char.DIC_Use.ContainsKey(idx)) return null;
		return m_Char.DIC_Use[idx];
	}
	public TCharacterTable GetNonUseCharacterTable(int idx) {
		if (!m_Char.DIC_NotUse.ContainsKey(idx)) return null;
		return m_Char.DIC_NotUse[idx];
	}
	public List<TCharacterTable> GetGroupCharacterTable(JobType _type) {
		if (!m_Char.DIC_Job.ContainsKey(_type)) return null;
		return m_Char.DIC_Job[_type];
	}
	public Dictionary<int, TCharacterTable> GetAllCharacterTable() {
		return m_Char.DIC_Use;
	}

	public List<TCharacterTable> GetAllCharacterInfos()
	{
		return m_Char.Tables;
	}

	public TCharacterTable GetCharacterTableToPiece(int _piece) {
		return m_Char.Tables.Find(o => o.m_PieceIdx == _piece);
	}
}

