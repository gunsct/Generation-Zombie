using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TTalkerTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;

	/// <summary> 이름 </summary>
	public int m_Name;

	/// <summary> 프로필 이미지 </summary>
	public string m_Profile;

	public TTalkerTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Profile = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Profile) && !m_Profile.Contains("/"))
			Debug.LogError($"[ TalkerTable ({m_Idx}) ] m_Profile 패스 체크할것");
#endif
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}

	public Sprite GetSprPortrait()
	{
		return UTILE.LoadImg(m_Profile, "png");
	}
}
public class TTalkerTableMng : ToolFile
{
	public Dictionary<int, TTalkerTable> DIC_Idx = new Dictionary<int, TTalkerTable>();
	public List<TTalkerTable> Datas = new List<TTalkerTable>();

	public TTalkerTableMng() : base("Datas/TalkerTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TTalkerTable data = new TTalkerTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// TalkerTable
	TTalkerTableMng m_Talker = new TTalkerTableMng();

	public TTalkerTable GetTalkerTable(int _idx)
	{
		if (!m_Talker.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Talker.DIC_Idx[_idx];
	}

	public Dictionary<int, TTalkerTable> GetAllTalkerTable()
	{
		return m_Talker.DIC_Idx;
	}

	public List<TTalkerTable> GetAllTalkerInfos()
	{
		return m_Talker.Datas;
	}
}