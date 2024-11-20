using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TSynergyTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 시너지 타입 </summary>
	public JobType m_SynergyType;
	/// <summary> 시너지 이름 </summary>
	public int m_Name;
	/// <summary> 시너지 설명 </summary>
	public int m_Desc;
	/// <summary> 필요 캐릭터 수 </summary>
	public int m_NeedCount;
	/// <summary> 해당 시너지 발동 시 변경되는 데이터 값 </summary>
	public float[] m_Value = new float[2];
	/// <summary> 버프 리스트에 표시될 시너지 이미지 </summary>
	public string m_Img;
	/// <summary> 알림 사용 여부 </summary>
	public bool m_UseAlarm;

	public TSynergyTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_SynergyType = pResult.Get_Enum<JobType>();
		m_Name = pResult.Get_Int32();
		m_Desc = pResult.Get_Int32();
		m_NeedCount = pResult.Get_Int32();
		m_Value[0] = pResult.Get_Float();
		m_Value[1] = pResult.Get_Float();
		m_Img = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Img) && !m_Img.Contains("/"))
			Debug.LogError($"[ SynergyTable ({m_Idx}) ] m_Img 패스 체크할것");
#endif
		m_UseAlarm = pResult.Get_Boolean();
	}
	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	public string GetDesc() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
	}
	public Sprite GetIcon() {
		return UTILE.LoadImg(m_Img, "png");
	}
	public Sprite GetImg()
	{
		return GetIcon();
	}
	public SynergyActiveType GetActiveType() {
		switch (m_SynergyType) {
			case JobType.Spy:
			case JobType.Explorer:
			case JobType.Athlete:
			case JobType.Guard:
			case JobType.Scientist:
			case JobType.Researcher:
			case JobType.Doctor:
			case JobType.Nurse:
			case JobType.Shef:
			case JobType.Counselor:
			case JobType.Lunatic:
				return SynergyActiveType.NonActive;
			default:
				return SynergyActiveType.Active;
		}
	}
	public bool IS_CanSynergy(int _cnt) {
		return _cnt >= m_NeedCount;
	}
}
public class TSynergyTableMng : ToolFile
{
	public Dictionary<JobType, TSynergyTable> DIC_Type = new Dictionary<JobType, TSynergyTable>();

	public TSynergyTableMng() : base("Datas/SynergyTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TSynergyTable data = new TSynergyTable(pResult);
		DIC_Type.Add(data.m_SynergyType, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// SynergyTable
	TSynergyTableMng m_Synergy = new TSynergyTableMng();

	public TSynergyTable GetSynergyTable(JobType _type) {
		if (!m_Synergy.DIC_Type.ContainsKey(_type)) return null;
		return m_Synergy.DIC_Type[_type];
	}

}

