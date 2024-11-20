using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TZombieSerumDropTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Grade;
	public int m_Gid;
	public int[] m_Cnt = new int[2];

	public TZombieSerumDropTable(CSV_Result pResult) {
		m_Grade = pResult.Get_Int32();
		m_Gid = pResult.Get_Int32();
		m_Cnt[0] = pResult.Get_Int32();
		m_Cnt[1] = pResult.Get_Int32();
	}
}
public class TZombieSerumDropTableMng : ToolFile
{
	public Dictionary<int, TZombieSerumDropTable> DIC_Type = new Dictionary<int, TZombieSerumDropTable>();

	public TZombieSerumDropTableMng() : base("Datas/ZombieSerumDropTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TZombieSerumDropTable data = new TZombieSerumDropTable(pResult);
		DIC_Type.Add(data.m_Grade, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ZombieSerumDropTable
	TZombieSerumDropTableMng m_ZombieSerumDrop = new TZombieSerumDropTableMng();

	public TZombieSerumDropTable GetZombieSerumDropTable(int _gid) {
		if (!m_ZombieSerumDrop.DIC_Type.ContainsKey(_gid)) return null;
		return m_ZombieSerumDrop.DIC_Type[_gid];
	}
}

