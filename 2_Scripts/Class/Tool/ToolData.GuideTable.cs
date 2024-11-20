using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TGuideTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public StageClearType m_ClearType;
	/// <summary> 목표 </summary>
	public int m_Goal;
	/// <summary> 가이드 프레임 타입 </summary>
	public GuideFrameType m_FrameType;
	/// <summary> 벨류 표시 타입 </summary>
	public bool m_IsRateValue;

	public TGuideTable(CSV_Result pResult) {
		m_ClearType = pResult.Get_Enum<StageClearType>();
		m_Goal = pResult.Get_Int32();
		m_FrameType = pResult.Get_Enum<GuideFrameType>();
		m_IsRateValue = pResult.Get_Boolean();
	}

	public string GetGoal() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Goal);
	}
	public Color GetTextBrightColor() {
		switch (m_FrameType) {
			case GuideFrameType.Normal:
				return Utile_Class.GetCodeColor("#afac8f");
			case GuideFrameType.Rescue:
				return Utile_Class.GetCodeColor("#bae193");
			case GuideFrameType.Enemy:
				return Utile_Class.GetCodeColor("#ec8c7c");
		}
		return Color.white;
	}
	public Color GetTextDarkColor() {
		switch (m_FrameType) {
			case GuideFrameType.Normal:
				return Utile_Class.GetCodeColor("#8a8371");
			case GuideFrameType.Rescue:
				return Utile_Class.GetCodeColor("#829879");
			case GuideFrameType.Enemy:
				return Utile_Class.GetCodeColor("#dc5f51");
		}
		return Color.white;
	}
	public Color GetBGColor() {
		switch (m_FrameType) {
			case GuideFrameType.Normal:
				return Utile_Class.GetCodeColor("#3F4338");
			case GuideFrameType.Rescue:
				return Utile_Class.GetCodeColor("#122B13");
			case GuideFrameType.Enemy:
				return Utile_Class.GetCodeColor("#350B06");
		}
		return Color.white;
	}
	public Color GetFXColor() {
		switch (m_FrameType) {
			case GuideFrameType.Normal:
				return new Color(113f / 255f, 107f / 255f, 99f / 255f, 0f);
			case GuideFrameType.Rescue:
				return new Color(85f / 255f, 108f / 255f, 55f / 255f, 0f);
			case GuideFrameType.Enemy:
				return new Color(255f / 255f, 88f / 255f, 88f / 255f, 0f);
		}
		return Color.white;
	}
}
public class TGuideTableMng : ToolFile
{
	public Dictionary<StageClearType, TGuideTable> DIC_Type = new Dictionary<StageClearType, TGuideTable>();

	public TGuideTableMng() : base("Datas/GuideTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TGuideTable data = new TGuideTable(pResult);
		DIC_Type.Add(data.m_ClearType, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// GuideTable
	TGuideTableMng m_Guide = new TGuideTableMng();

	public TGuideTable GetGuideTable(StageClearType _type) {
		if (!m_Guide.DIC_Type.ContainsKey(_type)) return null;
		return m_Guide.DIC_Type[_type];
	}
}

