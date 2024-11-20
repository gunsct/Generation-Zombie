using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TGuild_ResearchTable : ClassMng
{
	public class UnLock
	{
		/// <summary> 연결 연구 인덱스 </summary>
		public int m_ResIdx;
		/// <summary> 길드 레벨 </summary>
		public int m_LV;
	}

	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 그룹번호 </summary>
	public int m_Group;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 내용 </summary>
	public int m_Desc;
	/// <summary> 1개당 기여도 </summary>
	public int m_Exp;
	/// <summary> 레벨 정보(연구의 레벨) </summary>
	public int m_LV;
	/// <summary> 인덱스 </summary>
	public UnLock m_Unlock = new UnLock();
	/// <summary> 연구 효과 </summary>
	public TResearchTable.Effect m_Eff = new TResearchTable.Effect();
	/// <summary> 필요 재료 </summary>
	public TResearchTable.Material m_Mat = new TResearchTable.Material();
	/// <summary> 타입번호 </summary>
	public int m_TypeNo;

	public TGuild_ResearchTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Group = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_Unlock.m_ResIdx = pResult.Get_Int32();
		m_Unlock.m_LV = pResult.Get_Int32();
		m_Eff.m_Eff = pResult.Get_Enum<ResearchEff>();
		m_Eff.m_Value = pResult.Get_Float();
		m_Mat.m_Idx = pResult.Get_Int32();
		m_Mat.m_Count = pResult.Get_Int32();
		m_TypeNo = pResult.Get_Int32();
		m_Exp = pResult.Get_Int32();
		m_LV = pResult.Get_Int32();
	}

	public string GetName()
	{
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Name), m_LV);
	}

	public string ValueToString()
	{
		var value = m_Eff.m_Value;
		switch (m_Eff.m_Eff)
		{
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
			return $"{value}";
		}
		value *= 100f;
		return $"{Mathf.RoundToInt(value)}%";
	}

	public string GetDesc()
	{
		var value = m_Eff.m_Value;
		switch(m_Eff.m_Eff)
		{
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
			break;
		default:
			value *= 100f;
			break;
		}
		return string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc), Mathf.RoundToInt(value), m_LV);
	}

	public Sprite GetPoto(int Mode = 2)
	{
		string imgname = $"St_18_{Mode}";
		switch(m_TypeNo)
		{
		case 2: imgname = $"St_17_{Mode}"; break;
		case 4: imgname = $"St_3_{Mode}"; break;
		}

		return UTILE.LoadImg($"BG/StageImage/{imgname}", "png");
	}
}
public class TGuild_ResearchTableMng : ToolFile
{
	public Dictionary<int, List<TGuild_ResearchTable>> DIC_Group = new Dictionary<int, List<TGuild_ResearchTable>>();
	public Dictionary<int, TGuild_ResearchTable> DIC_IDX = new Dictionary<int, TGuild_ResearchTable>();
	public List<TGuild_ResearchTable> Datas = new List<TGuild_ResearchTable>();

	public int MaxStep;

	public TGuild_ResearchTableMng() : base("Datas/Guild_ResearchTable")
	{
	}

	public override void DataInit()
	{
		DIC_Group.Clear();
		DIC_IDX.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TGuild_ResearchTable table = new TGuild_ResearchTable(pResult);
		if (!DIC_Group.ContainsKey(table.m_Group)) DIC_Group.Add(table.m_Group, new List<TGuild_ResearchTable>());
		if (!DIC_IDX.ContainsKey(table.m_Idx)) DIC_IDX.Add(table.m_Idx, table);
		DIC_Group[table.m_Group].Add(table);
		Datas.Add(table);
		if (MaxStep < table.m_Group) MaxStep = table.m_Group;
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// Guild_ResearchTableTable
	TGuild_ResearchTableMng m_Guild_res = new TGuild_ResearchTableMng();

	public List<TGuild_ResearchTable> GetGuildResGroupList(int Group)
	{
		if (!m_Guild_res.DIC_Group.ContainsKey(Group)) return new List<TGuild_ResearchTable>();
		return m_Guild_res.DIC_Group[Group];
	}
	public TGuild_ResearchTable GetGuildRes(int Idx)
	{
		if (!m_Guild_res.DIC_IDX.ContainsKey(Idx)) return null;
		return m_Guild_res.DIC_IDX[Idx];
	}

	public bool IsCanGuildRes()
	{
		int LV;
		long Exp;
		USERINFO.m_Guild.Calc_Exp(out LV, out Exp);
		var list = m_Guild_res.Datas.FindAll(o => !USERINFO.m_Guild.EndRes.Contains(o.m_Idx));
		return list.Find(o => {
			if (o.m_Unlock.m_LV > LV) return false;
			if (o.m_Unlock.m_ResIdx != 0 && !USERINFO.m_Guild.EndRes.Contains(o.m_Unlock.m_ResIdx)) return false;
			return true;
		}) != null;
	}

	public int GetGuild_ResMaxStep()
	{
		return m_Guild_res.MaxStep;
	}

	public int GetGuild_ResStep(List<int> EndRes)
	{
		if (EndRes == null || EndRes.Count < 1) return 1;
		int Re = m_Guild_res.MaxStep;
		for(int i = 1; i < m_Guild_res.MaxStep; i++)
		{
			if (!m_Guild_res.DIC_Group.ContainsKey(i)) continue;
			var list = m_Guild_res.DIC_Group[i].Select(o => o.m_Idx).ToList();
			// 차집합  ( list.Except(EndRes) )
			// 교집합  ( list.Intersect(EndRes) )
			// 합집합  ( list.Union(EndRes) )
			var inter = list.Intersect(EndRes).Count();
			// 완료와의 교집합 개수가 현재 단계의 개수와 다르면 현재 단계
			// 같으면다음 단계 확인
			if (inter != list.Count()) return i;
		}

		return m_Guild_res.MaxStep;
	}
}

