using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdventureInfo : ClassMng
{
	public long m_UID;
	public int m_Idx;
	public long[] m_Times = new long[2];
	public List<long> m_Chars = new List<long>();  //배정된 캐릭터
	public TimeContentState m_State;
	[JsonIgnore] public TAdventureTable m_TData { get { return TDATA.GetAdventureTable(m_Idx); } }

	/// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
	public AdventureInfo() { }
	public AdventureInfo(int idx)
	{
		m_UID = Utile_Class.GetUniqeID();
		m_Idx = idx;
		m_Times[0] = m_Times[0] = 0;
		m_State = TimeContentState.Idle;
	}

	public void SetDATA(LS_Web.RES_ADVINFO data)
	{
		m_Idx = data.Idx;
		m_Times[0] = data.Times[0];
		m_Times[1] = data.Times[1];
		m_Chars.Clear();
		m_Chars.AddRange(data.Chars);
		m_State = data.State;
	}
	/// <summary> 등록된 시간과 호출 시간으로 연구가 완료 체크 </summary>
	public bool IS_Complete() {
		return m_State == TimeContentState.Play && GetRemainTime() <= 0;
	}

	public void SetPlay(List<long> CUIDS)
	{
		m_Chars.Clear();
		m_Chars.AddRange(CUIDS);
		m_Times[0] = (long)UTILE.Get_ServerTime_Milli();
		m_Times[1] = m_Times[0] + (long)m_TData.GetTime();
		m_State = TimeContentState.Play;
	}

	/// <summary> 남은 시간 </summary>
	public double GetRemainTime() {
		return Math.Max(0, m_Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001d;
	}
	public long GetMaxTime() {
		if (m_State == TimeContentState.Play) return m_Times[1] - m_Times[0];
		return m_TData.GetTime();
	}
	/// <summary> 상태 변경 </summary>
	public void SetState(TimeContentState _state) {
		m_State = _state;
	}
}
