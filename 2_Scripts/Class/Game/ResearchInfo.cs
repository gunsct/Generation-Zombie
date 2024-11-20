using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public class ResearchInfo : ClassMng
{
	public long m_UID;
	public int m_Idx;
	public int m_LV;
	public long[] m_Times = new long[2];
	public ResearchType m_Type;
	public TimeContentState m_State;
	[JsonIgnore] public TResearchTable m_TData { get { return TDATA.GetResearchTable(m_Type, m_Idx, m_GetLv); } }
	[JsonIgnore] public int m_GetLv { get { return Math.Min(m_LV, m_MaxLV); } }
	[JsonIgnore] public int m_MaxLV { get { return TDATA.GetResearchTable_MaxLV(m_Type, m_Idx); } }

	public void SetDATA(RES_RESEARCHINFO data)
	{
		m_UID = data.UID;
		m_Idx = data.Idx;
		m_LV = data.LV;
		m_Times[0] = data.Times[0];
		m_Times[1] = data.Times[1];
		m_Type = data.Type;
		m_State = data.State;
	}

	public bool Is_Open(bool IsZeroLV = true)
	{
		bool re = true;
		TResearchTable data = IsZeroLV ? TDATA.GetResearchTable(m_Type, m_Idx, 0) : m_TData;
		//스테이지 제한
		re = Is_StgUnLock(IsZeroLV);
		if (!re) return re;
		// 오픈 상태인지 확인
		for (int i = 0; i < 3; i++)
		{
			TResearchTable.Preced preced = data.m_Preced[i];
			if (preced.m_Idx == 0) continue;
			ResearchInfo Info = USERINFO.GetResearchInfo(m_Type, preced.m_Idx);
			if(Info.m_GetLv < preced.m_LV)
			{
				re = false;
				break;
			}
		}
		return re;
	}
	public bool Is_StgUnLock(bool IsZeroLV = true) {
		TResearchTable data = IsZeroLV ? TDATA.GetResearchTable(m_Type, m_Idx, 0) : m_TData;
		return data.IsUnlock();
	}
	/// <summary> 남은 시간 </summary>
	public double GetRemainTime()
	{
		return Math.Max(0, m_Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001d;
	}
	/// <summary> 총 시간 </summary>
	public long GetMaxTime()
	{
		if (m_State == TimeContentState.Play) return m_Times[1] - m_Times[0];
		return m_TData.GetTime();
	}

	public bool IS_Complete()
	{
		if (m_State != TimeContentState.Play) return false;
		return m_Times[1] < UTILE.Get_ServerTime_Milli();
	}

	public void OnComplete(Action<RES_RESEARCH_END> EndCB)
	{
		RES_RESEARCH_END res = new RES_RESEARCH_END();
#if NOT_USE_NET
		if (!IS_Complete())
		{
			int cash = BaseValue.GetTimePrice(ContentType.Research,GetRemainTime());
			if (USERINFO.m_Cash < cash)
			{
				res.result_code = EResultCode.ERROR_CASH;
				EndCB?.Invoke(res);
				return;
			}
			
			//FireBase-Analytics
			MAIN.GoldToothStatistics(GoldToothContentsType.ResearchTime, m_Idx);

			USERINFO.GetCash(-cash);
		}
		m_LV++;
		m_State = TimeContentState.Idle;
		MAIN.Save_UserInfo();
		EndCB?.Invoke(res);
#else
		WEB.SEND_REQ_RESEARCH_END((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				return;
			}
			if (!IS_Complete())
			{
				//FireBase-Analytics
				MAIN.GoldToothStatistics(GoldToothContentsType.ResearchTime, m_Idx);
			}
			EndCB?.Invoke(res);
		}, m_UID, !IS_Complete());
#endif
	}
}
