using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TEnemyNoteGroupTable : ClassMng
{
	/// <summary> 고정 노트 타입 </summary>
	public int m_Gid;
	/// <summary> 구성 노트 타입 </summary>
	public ENoteType m_NoteType;
	/// <summary> 사이즈 </summary>
	public ENoteSize m_Size;
	/// <summary> 종료시간 </summary>
	public float m_EndTime;
	/// <summary> 콤보나 홀드 카운트 </summary>
	public int m_Cnt;
	/// <summary> 등장 딜레이 </summary>
	public float m_Delay;
	/// <summary> 회전 </summary>
	public float m_RotZ;
	/// <summary> X좌표 </summary>
	public float m_PosX;
	/// <summary> Y좌표 </summary>
	public float m_PosY;

	public TEnemyNoteGroupTable(CSV_Result pResult) {
		m_Gid = pResult.Get_Int32();
		m_NoteType = pResult.Get_Enum<ENoteType>();
		m_Size = pResult.Get_Enum<ENoteSize>();
		m_EndTime = pResult.Get_Float();
		m_Cnt = pResult.Get_Int32();
		m_Delay = pResult.Get_Float();
		m_RotZ = pResult.Get_Float();
		m_PosX = pResult.Get_Float();
		m_PosY = pResult.Get_Float();
	}
	public Vector3 GetPos() {
		return new Vector3(m_PosX, m_PosY, 0f);
	}
}
public class TEnemyNoteGroupTableMng : ToolFile
{
	public Dictionary<int, List<TEnemyNoteGroupTable>> DIC_GID = new Dictionary<int, List<TEnemyNoteGroupTable>>();

	public TEnemyNoteGroupTableMng() : base("Datas/EnemyNoteGroupTable")
	{
	}

	public override void CheckData()
	{
	}

	public override void DataInit()
	{
		DIC_GID.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEnemyNoteGroupTable data = new TEnemyNoteGroupTable(pResult);
		if (!DIC_GID.ContainsKey(data.m_Gid))
			DIC_GID.Add(data.m_Gid, new List<TEnemyNoteGroupTable>());
		DIC_GID[data.m_Gid].Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EnemyNoteGroupTable
	TEnemyNoteGroupTableMng m_EnumyNoteGroup = new TEnemyNoteGroupTableMng();

	public List<TEnemyNoteGroupTable> GetEnemyNoteGroupTable(int _gtype) {
		if (!m_EnumyNoteGroup.DIC_GID.ContainsKey(_gtype)) return null;
		return m_EnumyNoteGroup.DIC_GID[_gtype];
	}
}

