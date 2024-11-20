using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public class MakingInfo : ClassMng
{
	public long m_UID;
	public int m_Idx;       //레시피 인덱스
	public int m_Count;     //한번에 제작 수 최대 50개까지
	public long[] m_Times = new long[2];   //제작 시작 시간
	public TimeContentState m_State = TimeContentState.Idle;
	[JsonIgnore] public TMakingTable m_TData { get { return TDATA.GetMakingTable(m_Idx); } }

	public MakingInfo() {}
	/// <summary> 연구에 들어가는 캐릭터  </summary>
	public MakingInfo(int _idx, int _makecnt) {
		m_UID = Utile_Class.GetUniqeID();
		m_Idx = _idx;
		m_Count = _makecnt;
		m_Times[0] = (long)UTILE.Get_ServerTime_Milli();
		m_Times[1] = m_Times[0] + GetMaxTime() * m_Count;
		m_State = TimeContentState.Play;
	}
	public void SetDATA(RES_MAKINGINFO data)
	{
		m_UID = data.UID;
		m_Idx = data.Idx;
		m_Count = data.Cnt;
		m_Times[0] = data.Times[0];
		m_Times[1] = data.Times[1];
		m_State = data.State;
	}

	/// <summary> 등록된 시간과 호출 시간으로 연구가 완료 체크 </summary>
	public bool IS_Complete() {
		return m_State == TimeContentState.Play && GetRemainTime() <= 0;
	}
	//public void SetPlay(List<long> CUIDS) {
	//    m_Times[0] = (long)UTILE.Get_ServerTime_Milli();
	//    m_Times[1] = m_Times[0] + (long)m_TData.GetTime();
	//    m_State = AdvState.Play;
	//}
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
	public void PastComplete() {
		m_Times[1] = (long)UTILE.Get_ServerTime_Milli();
		m_State = TimeContentState.Play;
	}
}
