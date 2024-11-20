using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static ToolData;

public class RewardInfo
{
	public RewardKind Kind;
	public int Idx;
	public int Cnt;
	public int LV = 1;
	//서버에는 등급 안씀
	public int Grade = 1;
}

public class TEventTable : ClassMng
{
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이벤트 타입 </summary>
	public AddEventType m_EventType;
	/// <summary> 해당 스테이지 끝난 뒤부터 나올 수 있음</summary>
	public int m_StageLock;
	/// <summary> 타이틀 </summary>
	public int m_Title;
	/// <summary> 아이콘 </summary>
	public string m_Icon;
	/// <summary> enemyidx </summary>
	public int m_EnemyIdx;
	/// <summary> 이벤트 값 </summary>
	public int m_Val;
	/// <summary> 보상 </summary>
	public List<RewardInfo> m_Rewards = new List<RewardInfo>();
	/// <summary> 이벤트 확률 </summary>
	public int m_Prob;
	public TEventTable(CSV_Result pResult)
	{
		m_Idx = pResult.Get_Int32();
		m_EventType = pResult.Get_Enum<AddEventType>();
		m_StageLock = pResult.Get_Int32();
		m_Title = pResult.Get_Int32();
		m_Icon = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Icon) && !m_Icon.Contains("/"))
			Debug.LogError($"[ EventTable ({m_Idx}) ] m_Icon 패스 체크할것");
#endif
		m_EnemyIdx = pResult.Get_Int32();
		m_Val = pResult.Get_Int32();
		for (int i = 0; i < 3; i++) {
			RewardKind type = pResult.Get_Enum<RewardKind>();
			if(type == RewardKind.None) {
				pResult.NextReadPos();
				pResult.NextReadPos();
			}
			else {
				RewardInfo reward = new RewardInfo() {
					Kind = type,
					Idx = pResult.Get_Int32(),
					Cnt = pResult.Get_Int32()
				};
				m_Rewards.Add(reward);
			}
		}
		m_Prob = pResult.Get_Int32();
	}
	public string GetTitle() {
		return TDATA.GetString(StringTalbe.Dialog, m_Title);
	}
	public Sprite GetIcon() {
		return UTILE.LoadImg(m_Icon, "png");
	}
}

public class TEventTableMng : ToolFile
{
	public Dictionary<int, TEventTable> DIC_Idx = new Dictionary<int, TEventTable>();

	public TEventTableMng() : base("Datas/EventTable")
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TEventTable data = new TEventTable(pResult);
		DIC_Idx.Add(data.m_Idx, data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// EventTable
	TEventTableMng m_Event = new TEventTableMng();

	public TEventTable GetEventTable(int idx)
	{
		if (!m_Event.DIC_Idx.ContainsKey(idx)) return null;
		return m_Event.DIC_Idx[idx];
	}
	public TEventTable GetRandEventTable() {
		List<TEventTable> tables = (new List<TEventTable>(m_Event.DIC_Idx.Values)).FindAll(o => o.m_Prob > 0 && o.m_StageLock <= USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx).ToList();
		int dropprop = UTILE.Get_Random(0, tables.Sum(o => o.m_Prob));
		for (int i = 0; i < tables.Count; i++) {
			TEventTable table = tables[i];
			if (dropprop < table.m_Prob) return table;
			dropprop -= table.m_Prob;
		}
		return null;
	}
}
