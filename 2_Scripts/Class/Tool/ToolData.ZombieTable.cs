using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using static LS_Web;

public class TZombieTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 좀비 등급 </summary>
	public int m_Grade;
	/// <summary> 프리팹 이름 </summary>
	public string m_PrefabName;
	/// <summary> 좀비 설명 </summary>
	public int m_Desc;
	public class IdxCnt
	{
		public int Idx;
		public int Cnt;
	}
	public List<IdxCnt> m_TimeRewards = new List<IdxCnt>();
	public List<IdxCnt> m_RemoveRewards = new List<IdxCnt>();

	public TZombieTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Grade = pResult.Get_Int32();
		m_PrefabName = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_PrefabName) && !m_PrefabName.Contains("/"))
			Debug.LogError($"[ ZombieTable ({m_Idx}) ] m_PrefabName 패스 체크할것");
#endif
		m_Desc = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx < 1) pResult.NextReadPos();
			else {
				m_TimeRewards.Add(new IdxCnt() { Idx = idx, Cnt = pResult.Get_Int32() });
			}
		}
		for (int i = 0; i < 3; i++) {
			int idx = pResult.Get_Int32();
			if (idx < 1) pResult.NextReadPos();
			else {
				m_RemoveRewards.Add(new IdxCnt() { Idx = idx, Cnt = pResult.Get_Int32() });
			}
		}
	}
	
	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}
	
	public Sprite GetItemSmallImg()
	{
		return UTILE.LoadImg(string.Format("Card/Stage_Enemy/{0}", Utile_Class.GetFileName(m_PrefabName)), "png");
	}
	public Sprite GetItemBigImg() {
		return UTILE.LoadImg(string.Format("Enemy/{0}", Utile_Class.GetFileName(m_PrefabName)), "png");
	}

	public string GetDesc() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Desc);
	}
}
public class TZombieTableMng : ToolFile
{
	public Dictionary<int, TZombieTable> DIC_Idx = new Dictionary<int, TZombieTable>();

	public TZombieTableMng() : base("Datas/ZombieTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TZombieTable data = new TZombieTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}
public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ZombieTable
	TZombieTableMng m_Zombie = new TZombieTableMng();

	public TZombieTable GetZombieTable(int _idx) {
		if (!m_Zombie.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Zombie.DIC_Idx[_idx];
	}
	public List<TZombieTable> GetAllZombieTable() {
		return new List<TZombieTable>(m_Zombie.DIC_Idx.Values); 
	}
}

