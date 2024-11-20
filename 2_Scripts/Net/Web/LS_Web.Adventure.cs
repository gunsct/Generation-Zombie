using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_ADVINFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}


	public class RES_ALL_ADVINFO : RES_BASE
	{
		public List<RES_ADVINFO> Advs;
	}

	public class RES_ADVINFO : RES_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 탐사 인덱스 </summary>
		public int Idx;
		/// <summary> 배치된 캐릭터들 </summary>
		public List<long> Chars = new List<long>();
		/// <summary> 0:시작시간, 1:종료시간 </summary>
		public long[] Times = new long[2];
		/// <summary> 상태 </summary>
		public TimeContentState State;
	}

	public void SEND_REQ_ADVINFO(Action<RES_ALL_ADVINFO> action, params long[] UIDs)
	{
		REQ_ADVINFO _data = new REQ_ADVINFO();
		_data.UserNo = USERINFO.m_UID;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);

		SendPost(Protocol.REQ_ADVINFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ALL_ADVINFO res = WEB.ParsResData<RES_ALL_ADVINFO>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				if (UIDs == null) USERINFO.m_Advs.Clear();
				USERINFO.SetDATA(res.Advs);
			}

			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Adventure Reset
	public class RES_ADV_RESET : RES_BASE
	{
		public List<RES_ADVINFO> Advs = new List<RES_ADVINFO>();
	}

	public void SEND_REQ_ADV_RESET(Action<RES_ADV_RESET> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_ADV_RESET, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ADV_RESET res = WEB.ParsResData<RES_ADV_RESET>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.Advs);
			action?.Invoke(res);
		});
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Adventure Start

	public class REQ_ADV_START_INFO
	{
		/// <summary> 고유 번호 </summary>
		public long UID;

		/// <summary> 탐사 배치 캐릭터 </summary>
		public List<long> CUIDS = new List<long>();
	}

	public class REQ_ADV_START : REQ_BASE
	{
		/// <summary> 시작 정보 </summary>
		public List<REQ_ADV_START_INFO> ADVS = new List<REQ_ADV_START_INFO>();
	}

	public void SEND_REQ_ADV_START(Action<RES_ALL_ADVINFO> action, List<REQ_ADV_START_INFO> infos)
	{
		REQ_ADV_START _data = new REQ_ADV_START();
		_data.UserNo = USERINFO.m_UID;
		_data.ADVS.AddRange(infos);

		SendPost(Protocol.REQ_ADV_START, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ALL_ADVINFO res = WEB.ParsResData<RES_ALL_ADVINFO>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				USERINFO.SetDATA(res.Advs);
				USERINFO.Check_Mission(MissionType.ADV, 0, 0, infos.Count);
			}

			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Adventure End

	public class REQ_ADV_END : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
		/// <summary> 캐시 사용 유무 </summary>
		public bool CashUse;
	}

	public class RES_ADV_END : RES_BASE
	{
		/// <summary> 완료된 탐사 정보 </summary>
		public List<RES_ADVINFO> Advs;
	}
	public void SEND_REQ_ADV_END(Action<RES_ADV_END> action, List<long> UIDs, bool IsCash = false)
	{
		REQ_ADV_END _data = new REQ_ADV_END();
		_data.UserNo = USERINFO.m_UID;
		_data.UIDs.AddRange(UIDs);
		_data.CashUse = IsCash;

		SendPost(Protocol.REQ_ADV_END, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ADV_END res = WEB.ParsResData<RES_ADV_END>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				USERINFO.SetDATA(res.Advs);
			}

			action?.Invoke(res);
		});
	}
}
