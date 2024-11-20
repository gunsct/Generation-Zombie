using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LS_Web;
using static ToolData;

public class TStringMng : ToolFile
{
	public Dictionary<int, string> DIC_Idx = new Dictionary<int, string>();

	public TStringMng(string Path) : base(Path)
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		int idx = pResult.Get_Int32();
		string value = pResult.Get_String();
		if (DIC_Idx.ContainsKey(idx)) DIC_Idx.Remove(idx);
		DIC_Idx.Add(idx, value);
	}

	public string GetRandomString()
	{
		if (DIC_Idx.Count < 1) return "";
		int pos = UTILE.Get_Random(0, DIC_Idx.Count);
		List<string> values = new List<string>(DIC_Idx.Values);
		return values[pos];
	}
}

public partial class ToolData : ClassMng
{
	public enum StringTalbe
	{
		UI = 0,
		Dialog,
		Post,
		Etc,
		ToolTip,
		Push,
		Max
	}
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// String
	Dictionary<StringTalbe, TStringMng> m_Strings = new Dictionary<StringTalbe, TStringMng>();

	void SetNewList()
	{
		m_Strings.Clear();
		m_Strings.Add(StringTalbe.UI, new TStringMng(string.Format("Datas/String_UI_{0}", APPINFO.m_LanguageCode)));
		m_Strings.Add(StringTalbe.Dialog, new TStringMng(string.Format("Datas/String_DialogTable_{0}", APPINFO.m_LanguageCode)));
		m_Strings.Add(StringTalbe.Etc, new TStringMng(string.Format("Datas/String_Etc_{0}", APPINFO.m_LanguageCode)));
		m_Strings.Add(StringTalbe.Post, new TStringMng(string.Format("Datas/String_Post_{0}", APPINFO.m_LanguageCode)));
		m_Strings.Add(StringTalbe.ToolTip, new TStringMng(string.Format("Datas/String_ToolTip_{0}", APPINFO.m_LanguageCode)));
		m_Strings.Add(StringTalbe.Push, new TStringMng(string.Format("Datas/String_Push_{0}", APPINFO.m_LanguageCode)));
	}

	public void LoadString()
	{
		SetNewList();

		List<TStringMng> list = new List<TStringMng>(m_Strings.Values);
		for (int i = list.Count - 1; i > -1; i--) list[i].Load();
	}

	/// <summary> 테스트 안해봄 추후 싱크로드가 필요하면 셋팅할것 </summary>
	public IEnumerator LoadStringAsync(Action<string, long> FileChange, Action<long> Proc)
	{
		SetNewList();
		List<TStringMng> list = new List<TStringMng>(m_Strings.Values);
		for (int i = list.Count - 1; i > -1; i--) yield return list[i].Load_Async(FileChange, Proc);
	}


	public string GetString(StringTalbe table, int idx)
	{
#if UNITY_EDITOR
		if (!m_Strings.ContainsKey(table)) return string.Format("{0}_{1}", table.ToString(), idx);
		if (!m_Strings[table].DIC_Idx.ContainsKey(idx)) return string.Format("{0}_{1}", table.ToString(), idx);
		if (m_Strings[table].DIC_Idx[idx].Length < 1) return string.Format("{0}_{1}", table.ToString(), idx);
#else
		if (!m_Strings.ContainsKey(table)) return "";
		if (!m_Strings[table].DIC_Idx.ContainsKey(idx)) return "";
#endif
		return m_Strings[table].DIC_Idx[idx];
	}

	public string GetString(int idx)
	{
		return GetString(StringTalbe.UI, idx);
	}

	public string GetStatString(StatType stat, float? value = null)
	{
		int idx = 0;
		switch (stat) {
			case StatType.HeadShot: idx = 4051; break;
			case StatType.ActionPointDecrease: idx = 10022; break;
			case StatType.SuccessAttackPer: idx = 10032; break;
			default: idx = 10001 + (int)stat; break;
		}
		string statname = GetString(StringTalbe.Etc, idx); ;
		if (value != null) return string.Format("{0} +{1:N2}%", statname, value.Value * 0.01f);
		return GetString(StringTalbe.Etc, idx);
	}

	public string GetStatString(ItemStat stat)
	{
		return GetStatString(stat.m_Stat, stat.m_Val);
	}

	public string GetEquipTypeName(EquipType type)
	{
		switch(type)
		{
		case EquipType.Weapon:		return GetString(25);
		case EquipType.Helmet:		return GetString(26);
		case EquipType.Costume:		return GetString(27);
		case EquipType.Shoes:		return GetString(28);
		case EquipType.Accessory:	return GetString(29);
		}
		return "";
	}

	public string GetToolTip()
	{
		return m_Strings[StringTalbe.ToolTip].GetRandomString();
	}

	///////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 챌린지
	public string GetChallengeName(ChallengeType type)
	{
		switch (type)
		{
		case ChallengeType.StageClear: return GetString(StringTalbe.Etc, 1000001);
		case ChallengeType.Research: return GetString(StringTalbe.Etc, 1000004);
		case ChallengeType.Making: return GetString(StringTalbe.Etc, 1000005);
		case ChallengeType.MakingEquip: return GetString(StringTalbe.Etc, 1000006);
		case ChallengeType.Tower: return GetString(StringTalbe.Etc, 1000009);
		case ChallengeType.GachaChar: return GetString(StringTalbe.Etc, 1000010);
		case ChallengeType.GachaEquip: return GetString(StringTalbe.Etc, 1000011);
		case ChallengeType.LevelChar: return GetString(StringTalbe.Etc, 1000012);
		case ChallengeType.UseExp: return GetString(StringTalbe.Etc, 1000013);
		case ChallengeType.LevelEquip: return GetString(StringTalbe.Etc, 1000014);
		case ChallengeType.GradeChar: return GetString(StringTalbe.Etc, 1000016);
		case ChallengeType.LevelZombie: return GetString(StringTalbe.Etc, 1000017);
		case ChallengeType.LevelDNA: return GetString(StringTalbe.Etc, 1000018);
		case ChallengeType.UseGoldTeeth: return GetString(StringTalbe.Etc, 1000020);
		case ChallengeType.UseBullet: return GetString(StringTalbe.Etc, 1000021);
		case ChallengeType.DownTownClear: return GetString(StringTalbe.Etc, 1000002);
		case ChallengeType.PVPWin: return GetString(StringTalbe.Etc, 1000003);
		}
		return "";
	}

	public string GetChallengeInfo(ChallengeType type)
	{
		switch (type)
		{
		case ChallengeType.StageClear: return GetString(ToolData.StringTalbe.Etc, 1005001);
		case ChallengeType.Research: return GetString(ToolData.StringTalbe.Etc, 1005004);
		case ChallengeType.Making: return GetString(ToolData.StringTalbe.Etc, 1005005);
		case ChallengeType.MakingEquip: return GetString(ToolData.StringTalbe.Etc, 1005006);
		case ChallengeType.Tower: return GetString(ToolData.StringTalbe.Etc, 1005009);
		case ChallengeType.GachaChar: return GetString(ToolData.StringTalbe.Etc, 1005010);
		case ChallengeType.GachaEquip: return GetString(ToolData.StringTalbe.Etc, 1005011);
		case ChallengeType.LevelChar: return GetString(ToolData.StringTalbe.Etc, 1005012);
		case ChallengeType.UseExp: return GetString(ToolData.StringTalbe.Etc, 1005013);
		case ChallengeType.LevelEquip: return GetString(ToolData.StringTalbe.Etc, 1005014);
		case ChallengeType.GradeChar: return GetString(ToolData.StringTalbe.Etc, 1005016);
		case ChallengeType.LevelZombie: return GetString(ToolData.StringTalbe.Etc, 1005017);
		case ChallengeType.LevelDNA: return GetString(ToolData.StringTalbe.Etc, 1005018);
		case ChallengeType.UseGoldTeeth: return GetString(ToolData.StringTalbe.Etc, 1005020);
		case ChallengeType.UseBullet: return GetString(ToolData.StringTalbe.Etc, 1005021);
		case ChallengeType.DownTownClear: return GetString(StringTalbe.Etc, 1005002);
		case ChallengeType.PVPWin: return GetString(StringTalbe.Etc, 1005003);
		}
		return "";
	}

	public string GetAchieveTabName(TAchievementTable.Tab tab)
	{
		switch(tab)
		{
		case TAchievementTable.Tab.Growth:		return GetString(421);
		case TAchievementTable.Tab.Collection:	return GetString(423);
		case TAchievementTable.Tab.Zombie:		return GetString(425);
		}
		return GetString(419);
	}

	public string GetAchieveTabInfo(TAchievementTable.Tab tab)
	{
		switch (tab)
		{
		case TAchievementTable.Tab.Growth: return GetString(422);
		case TAchievementTable.Tab.Collection: return GetString(424);
		case TAchievementTable.Tab.Zombie: return GetString(426);
		}
		return GetString(420);
	}

	public string GetCollectionTypeName(CollectionType Type)
	{
		switch (Type)
		{
		case CollectionType.Zombie: return GetString(432);
		case CollectionType.DNA: return GetString(434);
		}
		return GetString(430);
	}

	public string GetCollectionTypeInfo(CollectionType Type)
	{
		switch (Type)
		{
		case CollectionType.Zombie: return GetString(433);
		case CollectionType.DNA: return GetString(435);
		}
		return GetString(431);
	}
}
