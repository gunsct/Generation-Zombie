using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TEnemyDropTable : ClassMng
{
	public int m_Idx;
	public int m_Gid;
	public int m_CardIdx;
	public int m_Prop;
	public TEnemyDropTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Gid = pResult.Get_Int32();
		m_CardIdx = pResult.Get_Int32(); 
		m_Prop = pResult.Get_Int32();
	}

}
public class TEnemyDropTableMng : ToolFile
{
	public Dictionary<int, TEnemyDropTable> DIC_Idx = new Dictionary<int, TEnemyDropTable>();
	public Dictionary<int, List<TEnemyDropTable>> DIC_Gidx = new Dictionary<int, List<TEnemyDropTable>>();

	public TEnemyDropTableMng() : base("Datas/EnemyDropTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Gidx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEnemyDropTable data = new TEnemyDropTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Gidx.ContainsKey(data.m_Gid)) DIC_Gidx.Add(data.m_Gid, new List<TEnemyDropTable>());
		DIC_Gidx[data.m_Gid].Add(data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemyDropTable
	TEnemyDropTableMng m_EnemyDrop = new TEnemyDropTableMng();

	public TEnemyDropTable GetEnemyDropTable(int _idx) {
		if (!m_EnemyDrop.DIC_Idx.ContainsKey(_idx)) return null;
		return m_EnemyDrop.DIC_Idx[_idx];
	}
	public List<TEnemyDropTable> GetEnemyDropGroupTable(int _gid) {
		if (!m_EnemyDrop.DIC_Gidx.ContainsKey(_gid)) return null;
		return m_EnemyDrop.DIC_Gidx[_gid];
	}
	public TEnemyDropTable GetRandEnemyDropTable(int _gid) {
		if (!m_EnemyDrop.DIC_Gidx.ContainsKey(_gid)) return null;
		List<TEnemyDropTable> group = m_EnemyDrop.DIC_Gidx[_gid];
		TEnemyDropTable randtable = null;
		int total = group.Sum(o => o.m_Prop);
		int rand = UTILE.Get_Random(0, total);
		int sum = 0;
		for (int i = 0; i < group.Count; i++) {
			sum += group[i].m_Prop;
			if(rand <= sum) {
				randtable = group[i];
				break;
			}
		}

		return randtable;
	}
}

