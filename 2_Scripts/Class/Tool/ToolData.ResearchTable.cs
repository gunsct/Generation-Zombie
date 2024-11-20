using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ResearchType
{
	Research = 0,
	Training,
	Remodeling,
	Camp_Attack,
	Camp_Survive,
	Camp_Defense_Steal,
	End
}
public class TResearchTable : ClassMng
{
	public enum UnLockType
	{
		None = -1,
		Normal = 0,
		Hard,
		Nightmare,
		PVPCamp
	}
	public class Preced
	{
		/// <summary> 인덱스 </summary>
		public int m_Idx;
		/// <summary> 레벨 </summary>
		public int m_LV;
	}
	public class Effect
	{
		/// <summary> 효과 </summary>
		public ResearchEff m_Eff;
		/// <summary> 수치 </summary>
		public float m_Value;

		public string GetName()
		{
			switch (m_Eff)
			{
			case ResearchEff.BulletMaxUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13016);
			case ResearchEff.AtkUp:						return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13001);
			case ResearchEff.DefUp:						return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13002);
			case ResearchEff.HealthMaxUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13003);
			case ResearchEff.MenMaxUp:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13004);
			case ResearchEff.HygMaxUp:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13005);
			case ResearchEff.SatMaxUp:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13006);
			case ResearchEff.WeaponTimeUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13007);
			case ResearchEff.HelmetTimeUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13008);
			case ResearchEff.CostumeTimeUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13009);
			case ResearchEff.ShoesTimeUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13010);
			case ResearchEff.AccessoryTimeUp:			return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13011);
			case ResearchEff.SpecialEquipTimeUp:		return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13012);
			case ResearchEff.ResearchMaterialTimeUp:	return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13013);
			case ResearchEff.CraftMaterialTimeUp:		return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13014);
			case ResearchEff.ExploreTimeUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13015);
			case ResearchEff.MemberMaxUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13017);
			case ResearchEff.PVPAtkUp:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13018);
			case ResearchEff.PVPSpeedUp:				return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13019);
			case ResearchEff.PVPHitUp:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13020);
			case ResearchEff.PVPDefUP:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13021);
			case ResearchEff.PVPHpUP:					return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13022);
			case ResearchEff.PVPPerDefMenUP:			return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13023);
			case ResearchEff.PVPPerDefSatUP:			return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13024);
			case ResearchEff.PVPPerDefHygUP:			return MainMng.Instance.m_ToolData.GetString(ToolData.StringTalbe.Etc, 13025);
			}
			return "";
		}
	}

	public class Material
	{
		/// <summary> 인덱스 </summary>
		public int m_Idx;
		/// <summary> 개수 </summary>
		public int m_Count;
	}
	public class TreePos
	{
		/// <summary> 라인 </summary>
		public int m_Line;
		/// <summary> 위치 </summary>
		public int m_Pos;
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Desc;
	/// <summary> 레벨 </summary>
	public int m_Lv;
	/// <summary>  </summary>
	public ResearchType m_Type;
	/// <summary> 선행조건 </summary>
	public Preced[] m_Preced = new Preced[3];
	/// <summary> 효과 </summary>
	public Effect m_Eff = new Effect();
	/// <summary> 시간 </summary>
	public int m_Time;
	/// <summary> 필요 재료 </summary>
	public List<Material> m_Mat = new List<Material>();
	/// <summary> 트리 위치 정보 </summary>
	public TreePos m_Pos = new TreePos();
	/// <summary> 아이콘 </summary>
	public string m_Icon;
	/// <summary> 연구 언락 조건 </summary>
	public UnLockType m_UnLockType;
	/// <summary> 연구 언락 조건 값 </summary>
	public int m_UnLockVal;

	public TResearchTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_Lv = pResult.Get_Int32();
		m_Type = pResult.Get_Enum<ResearchType>();
		// 선행조건은 데이터 3개를 다 들고있는다 최초의경우 조건 인덱스가 0이므로
		for(int i = 0; i < 3; i++)
		{
			m_Preced[i] = new Preced();
			m_Preced[i].m_Idx = pResult.Get_Int32();
			m_Preced[i].m_LV = pResult.Get_Int32();
		}
		m_Eff.m_Eff = pResult.Get_Enum<ResearchEff>();
		m_Eff.m_Value = pResult.Get_Int32();
		m_Time = pResult.Get_Int32();

		m_Mat.Clear();
		for (int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx != 0) m_Mat.Add(new Material() { m_Idx = idx, m_Count = pResult.Get_Int32() });
			else pResult.NextReadPos();
		}

		m_Pos.m_Line = pResult.Get_Int32();
		m_Pos.m_Pos = pResult.Get_Int32();

		m_Icon = pResult.Get_String();
		m_UnLockType = pResult.Get_Enum<UnLockType>();
		m_UnLockVal = pResult.Get_Int32();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Icon) && !m_Icon.Contains("/"))
			Debug.LogError($"[ ResearchTable ({m_Idx}) ] m_Icon 패스 체크할것");
#endif
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	/// <summary>
	/// 0:변경수치만,1:누적수치만,2:누적 변경 수치 둘 다
	/// </summary>
	/// <param name="_mode"></param>
	/// <param name="_diff"></param>
	/// <returns></returns>
	public string GetInfo(int _mode = 0, bool _isprediff = true) {
		float[] val = new float[2];
		TResearchTable ptdata = TDATA.GetResearchTable(m_Type, m_Idx, m_Lv + (_isprediff ? -1 : 1));
		switch (_mode) {
			case 0:
				if (ptdata != null) {
					val[0] = Mathf.Abs(m_Eff.m_Value - ptdata.m_Eff.m_Value);
				}
				break;
			case 1:
				val[0] = m_Eff.m_Value;
				break;
			case 2:
				val[0] = m_Eff.m_Value;
				if (ptdata != null) {
					val[1] = Mathf.Abs(m_Eff.m_Value - ptdata.m_Eff.m_Value);
				}
				break;
		}

		switch (m_Eff.m_Eff) {
			case ResearchEff.MakingOpen:
			case ResearchEff.BulletMaxUp:
			case ResearchEff.AdventureOpen:
			case ResearchEff.TrainingOpen:
			case ResearchEff.AdventureCountUp:
			case ResearchEff.AdventureLevelUp:
			case ResearchEff.RemodelingOpen:
			case ResearchEff.GuardMaxUp:
			case ResearchEff.MakingLevelUp:
			case ResearchEff.SupplyBoxGradeUp:
			case ResearchEff.MemberMaxUp:
			case ResearchEff.PVPJunkCountDown:
			case ResearchEff.PVPCultivateCountDown:
			case ResearchEff.PVPChemicalCountDown:
				if (val[1] > 0)
					return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), Mathf.RoundToInt(val[0]), Mathf.RoundToInt(val[1]));
				else
					return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), Mathf.RoundToInt(val[0]));
			default:
				if (val[1] > 0)
					return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), val[0] * 0.01f, val[1] * 0.01f);
				else
					return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), val[0] * 0.01f);
		}
	}

	public Sprite GetIcon()
	{
		return UTILE.LoadImg(m_Icon, "png");
	}

	public long GetTime() {
		long Re = m_Time * 1000L;
		float per = 0f;
		switch(m_Type)
		{
		case ResearchType.Research: per += USERINFO.ResearchValue(ResearchEff.ResearchSpeedUp) + USERINFO.GetSkillValue(SkillKind.AllResearchSpeedUp); break;
		case ResearchType.Training: per += USERINFO.ResearchValue(ResearchEff.TrainingSpeedUp) + USERINFO.GetSkillValue(SkillKind.AllResearchSpeedUp); break;
		case ResearchType.Remodeling: per += USERINFO.ResearchValue(ResearchEff.RemodelingSpeedUp) + USERINFO.GetSkillValue(SkillKind.AllResearchSpeedUp); break;
		}

		Re -= Mathf.RoundToInt(Re * per);
		Re = Math.Max(Re, 0);
		return Re;
	}

	public bool IsUnlock()
	{
		switch(m_UnLockType)
		{
		case UnLockType.Normal: return m_UnLockVal <= USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)StageDifficultyType.Normal].Idx;
		case UnLockType.Hard: return m_UnLockVal <= USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)StageDifficultyType.Hard].Idx;
		case UnLockType.Nightmare: return m_UnLockVal <= USERINFO.m_Stage[StageContentType.Stage].Idxs[(int)StageDifficultyType.Nightmare].Idx;
		case UnLockType.PVPCamp: return m_UnLockVal <= USERINFO.m_CampBuild[CampBuildType.Camp].LV;
		}
		return true;
	}
}
public class TResearchTableMng : ToolFile
{
	public Dictionary<ResearchType, List<int>> DIC_Group = new Dictionary<ResearchType, List<int>>();
	public Dictionary<ResearchType, int> DIC_MaxLine = new Dictionary<ResearchType, int>();
	public Dictionary<ResearchType, Dictionary<int, int>> DIC_MaxLV = new Dictionary<ResearchType, Dictionary<int, int>>();
	public Dictionary<ResearchType, Dictionary<int, Dictionary<int, TResearchTable>>> DIC_Type = new Dictionary<ResearchType, Dictionary<int, Dictionary<int, TResearchTable>>>();
	public Dictionary<ResearchType, Dictionary<int, Dictionary<int, TResearchTable>>> DIC_Pos = new Dictionary<ResearchType, Dictionary<int, Dictionary<int, TResearchTable>>>();
	public Dictionary<ResearchEff, List<TResearchTable>> DIC_EffGroup = new Dictionary<ResearchEff, List<TResearchTable>>();
	public Dictionary<ResearchEff, int> DIC_ZERO_EFF = new Dictionary<ResearchEff, int>();
	public TResearchTableMng() : base(new string[] { "Datas/ResearchTable", "Datas/PVP_Camp_ResearchTable" })
	{
	}

	public override void DataInit()
	{
		DIC_Group.Clear();
		DIC_MaxLine.Clear();
		DIC_MaxLV.Clear();
		DIC_Type.Clear();
		DIC_Pos.Clear();
		DIC_EffGroup.Clear();
		DIC_ZERO_EFF.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TResearchTable data = new TResearchTable(pResult);
		if (!DIC_Type.ContainsKey(data.m_Type)) DIC_Type.Add(data.m_Type, new Dictionary<int, Dictionary<int, TResearchTable>>());
		if (!DIC_Type[data.m_Type].ContainsKey(data.m_Idx)) DIC_Type[data.m_Type].Add(data.m_Idx, new Dictionary<int, TResearchTable>());
		if (!DIC_Type[data.m_Type][data.m_Idx].ContainsKey(data.m_Lv)) DIC_Type[data.m_Type][data.m_Idx].Add(data.m_Lv, data);

		// 맥스 레벨
		if (!DIC_MaxLV.ContainsKey(data.m_Type)) DIC_MaxLV.Add(data.m_Type, new Dictionary<int, int>());
		if (!DIC_MaxLV[data.m_Type].ContainsKey(data.m_Idx)) DIC_MaxLV[data.m_Type].Add(data.m_Idx, data.m_Lv);
		else if (DIC_MaxLV[data.m_Type][data.m_Idx] < data.m_Lv) DIC_MaxLV[data.m_Type][data.m_Idx] = data.m_Lv;

		// 인덱스들
		if (!DIC_Group.ContainsKey(data.m_Type)) DIC_Group.Add(data.m_Type, new List<int>());
		if (!DIC_Group[data.m_Type].Contains(data.m_Idx)) DIC_Group[data.m_Type].Add(data.m_Idx);

		// 매스라인
		if (!DIC_MaxLine.ContainsKey(data.m_Type)) DIC_MaxLine.Add(data.m_Type, 0);
		if (DIC_MaxLine[data.m_Type] < data.m_Pos.m_Line) DIC_MaxLine[data.m_Type] = data.m_Pos.m_Line;

		// 위치
		if (data.m_Eff.m_Value != 0) {
			if (!DIC_Pos.ContainsKey(data.m_Type)) DIC_Pos.Add(data.m_Type, new Dictionary<int, Dictionary<int, TResearchTable>>());
			if (!DIC_Pos[data.m_Type].ContainsKey(data.m_Pos.m_Line)) DIC_Pos[data.m_Type].Add(data.m_Pos.m_Line, new Dictionary<int, TResearchTable>());
			if (!DIC_Pos[data.m_Type][data.m_Pos.m_Line].ContainsKey(data.m_Pos.m_Pos)) DIC_Pos[data.m_Type][data.m_Pos.m_Line].Add(data.m_Pos.m_Pos, data);
		}

		//효과별
		if (!DIC_EffGroup.ContainsKey(data.m_Eff.m_Eff)) DIC_EffGroup.Add(data.m_Eff.m_Eff, new List<TResearchTable>());
		if (!DIC_EffGroup[data.m_Eff.m_Eff].Contains(data)) DIC_EffGroup[data.m_Eff.m_Eff].Add(data);

		// 효과 0레벨 관련 수치
		if (!DIC_ZERO_EFF.ContainsKey(data.m_Eff.m_Eff)) DIC_ZERO_EFF.Add(data.m_Eff.m_Eff, 0);
		if (data.m_Lv < 1) DIC_ZERO_EFF[data.m_Eff.m_Eff] += Mathf.RoundToInt(data.m_Eff.m_Value);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ResearchTable
	TResearchTableMng m_Research = new TResearchTableMng();

	public int GetResearchTable_MaxLV(ResearchType type, int idx) {
		if (!m_Research.DIC_MaxLV.ContainsKey(type)) return 0;
		if (!m_Research.DIC_MaxLV[type].ContainsKey(idx)) return 0;
		return m_Research.DIC_MaxLV[type][idx];
	}
	public List<int> GetResearchTable_GroupIdxs(ResearchType type)
	{
		if (!m_Research.DIC_Group.ContainsKey(type)) return new List<int>();
		return m_Research.DIC_Group[type];
	}
	public int GetResearchTable_MaxLine(ResearchType type)
	{
		if (!m_Research.DIC_MaxLine.ContainsKey(type)) return 0;
		return m_Research.DIC_MaxLine[type] + 1;
	}
	public TResearchTable GetResearchTable(ResearchType type, int idx, int _lv)
	{
		if (!m_Research.DIC_Type.ContainsKey(type)) return null;
		if (!m_Research.DIC_Type[type].ContainsKey(idx)) return null;
		if (!m_Research.DIC_Type[type][idx].ContainsKey(_lv)) return null;
		return m_Research.DIC_Type[type][idx][_lv];
	}
	public TResearchTable GetResearchTableGroupFirst(ResearchEff _eff) {
		if (!m_Research.DIC_EffGroup.ContainsKey(_eff)) return null;
		return m_Research.DIC_EffGroup[_eff][0];
	}
	public Dictionary<int, Dictionary<int, TResearchTable>> GetResearchTableGroup(ResearchType type) {
		if(!m_Research.DIC_Type.ContainsKey(type)) return null;
		return m_Research.DIC_Type[type];
	}
	public int GetResearch_ZeroLV_Value(ResearchEff type)
	{
		if (!m_Research.DIC_ZERO_EFF.ContainsKey(type)) return 0;
		return m_Research.DIC_ZERO_EFF[type];
	}
	public TResearchTable GetResearchTablePos(ResearchType type, int line, int pos)
	{
		if (!m_Research.DIC_Pos.ContainsKey(type)) return null;
		if (!m_Research.DIC_Pos[type].ContainsKey(line)) return null;
		if (!m_Research.DIC_Pos[type][line].ContainsKey(pos)) return null;
		return m_Research.DIC_Pos[type][line][pos];
	}
}

