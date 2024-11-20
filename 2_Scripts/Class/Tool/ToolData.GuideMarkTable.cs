using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TGuideMarkTableMng : ToolFile
{
	public Dictionary<int, string> DIC_IDX = new Dictionary<int, string>();

	public TGuideMarkTableMng() : base("Datas/GuideMarkTable")
	{
	}

	public override void DataInit()
	{
		DIC_IDX.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		int idx = pResult.Get_Int32();
		if (DIC_IDX.ContainsKey(idx)) return;
		string path = pResult.Get_String();
		DIC_IDX.Add(idx, path);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// GuideMarkTabl
	TGuideMarkTableMng m_GuildMark = new TGuideMarkTableMng();

	public string GetGuideMark_Path(int idx) {
		if (!m_GuildMark.DIC_IDX.ContainsKey(idx)) return "";
		return m_GuildMark.DIC_IDX[idx];
	}

	public UnityEngine.Sprite GetGuideMark(int idx)
	{
		string path = GetGuideMark_Path(idx);
		if (string.IsNullOrEmpty(path)) return null;
		return UTILE.LoadImg(path, "png");
	}
}

