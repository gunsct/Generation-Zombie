using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TEthnicRelationsTable : ClassMng
{
	public EEnemyType m_Type;
	public Dictionary<EEnemyType, float> m_Relations = new Dictionary<EEnemyType, float>();
	//public List<EEnemyType> m_Relations = new List<EEnemyType>();

	public TEthnicRelationsTable(CSV_Result pResult) {
		// 인덱스 제거
		pResult.NextReadPos();
		m_Type = pResult.Get_Enum<EEnemyType>();
		for(int i = 0; i < 20; i++)
		{
			EEnemyType temp = pResult.Get_Enum<EEnemyType>();
			float val = pResult.Get_Float();
			if (temp != EEnemyType.None && !m_Relations.ContainsKey(temp)) m_Relations.Add(temp, val);
		}
	}

	public bool IS_Relation(EEnemyType target)
	{
		return m_Relations.ContainsKey(target);
	}
	public float GetDmgRatio(EEnemyType target) {
		if (!m_Relations.ContainsKey(target)) return 1f;
		return m_Relations[target];
	}
}

public class TEthnicRelationsTableMng : ToolFile
{
	public Dictionary<EEnemyType, TEthnicRelationsTable> DIC_Type = new Dictionary<EEnemyType, TEthnicRelationsTable>();

	public TEthnicRelationsTableMng() : base("Datas/EthnicRelationsTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEthnicRelationsTable data = new TEthnicRelationsTable(pResult);
		DIC_Type.Add(data.m_Type, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EthnicRelationsTable
	TEthnicRelationsTableMng m_EthnicRelation = new TEthnicRelationsTableMng();

	public bool ISEnemyAtkRelation(EEnemyType atk, EEnemyType def) {
		return m_EthnicRelation.DIC_Type[atk].IS_Relation(def);
	}
	public float GetEnemyAtkRatioRelation(EEnemyType atk, EEnemyType def) {
		return m_EthnicRelation.DIC_Type[atk].GetDmgRatio(def);
	}

	public TEthnicRelationsTable GetEnemyAtkRelation(EEnemyType atk)
	{
		return m_EthnicRelation.DIC_Type[atk];
	}
}

