using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TStageMaterialTable : ClassMng
{
	public StageMaterialType m_Material;
	public string m_StageCardImg;
	public int m_Name;

	public TStageMaterialTable(CSV_Result pResult)
	{
		m_Material = pResult.Get_Enum<StageMaterialType>();
		m_StageCardImg = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_StageCardImg) && !m_StageCardImg.Contains("/"))
			Debug.LogError($"[ StageMaterialTable ({m_Material}) ] m_StageCardImg 패스 체크할것");
#endif
		m_Name = pResult.Get_Int32();
	}

	public Sprite GetStateCardImg()
	{
		return UTILE.LoadImg(m_StageCardImg, "png");
	}
	public string GetName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
}

public class TStageMaterialTableMng : ToolFile
{
	public Dictionary<StageMaterialType, TStageMaterialTable> DIC_Type = new Dictionary<StageMaterialType, TStageMaterialTable>();

	public TStageMaterialTableMng() : base("Datas/StageMaterialTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TStageMaterialTable data = new TStageMaterialTable(pResult);
		DIC_Type.Add(data.m_Material, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// StageMaterialTable
	TStageMaterialTableMng m_StageMaterial = new TStageMaterialTableMng();

	public TStageMaterialTable GetStageMaterialTable(StageMaterialType type)
	{
		if (!m_StageMaterial.DIC_Type.ContainsKey(type)) return null;
		return m_StageMaterial.DIC_Type[type];
	}
}

