using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_RESEARCHINFO : REQ_BASE
	{
		public ResearchType Type;
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}


	public class RES_ALL_RESEARCHINFO : RES_BASE
	{
		public List<RES_RESEARCHINFO> Researchs = new List<RES_RESEARCHINFO>();
	}

	public class RES_RESEARCHINFO : RES_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 연구 인덱스 </summary>
		public int Idx;
		/// <summary> 연구 레벨 </summary>
		public int LV;
		/// <summary> 0:시작시간, 1:종료시간 </summary>
		public long[] Times = new long[2];
		/// <summary> 연구 타입 </summary>
		public ResearchType Type;
		/// <summary> 상태 </summary>
		public TimeContentState State;
	}

	public void SEND_REQ_RESEARCHINFO(Action<RES_ALL_RESEARCHINFO> action, ResearchType Type)
	{
		SEND_REQ_RESEARCHINFO(action, Type, null);
	}
	public void SEND_REQ_RESEARCHINFO(Action<RES_ALL_RESEARCHINFO> action, List<long> UIDs)
	{
		SEND_REQ_RESEARCHINFO(action, ResearchType.End, UIDs);
	}

	public void SEND_REQ_RESEARCHINFO(Action<RES_ALL_RESEARCHINFO> action, ResearchType Type = ResearchType.End, List<long> UIDs = null)
	{
		REQ_RESEARCHINFO _data = new REQ_RESEARCHINFO();
		_data.UserNo = USERINFO.m_UID;
		_data.Type = Type;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);

		SendPost(Protocol.REQ_RESEARCHINFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ALL_RESEARCHINFO res = WEB.ParsResData<RES_ALL_RESEARCHINFO>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				USERINFO.SetDATA(res.Researchs);
			}
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Research Start
	public class REQ_RESEARCH_START : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 타입 </summary>
		public ResearchType Type;
		/// <summary> 인덱스 </summary>
		public int Idx;
	}
	public class RES_RESEARCH_START : RES_BASE
	{
		public RES_RESEARCHINFO Research;
	}

	public void SEND_REQ_RESEARCH_START(Action<RES_RESEARCH_START> action, ResearchInfo info)
	{
		REQ_RESEARCH_START _data = new REQ_RESEARCH_START();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.m_UID;
		_data.Type = info.m_Type;
		_data.Idx = info.m_Idx;

		SendPost(Protocol.REQ_RESEARCH_START, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_RESEARCH_START res = WEB.ParsResData<RES_RESEARCH_START>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				USERINFO.Check_Mission(MissionType.Research, 0, 0, 1);
				USERINFO.SetDATA(res.Research);
			}
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Research End
	public class REQ_RESEARCH_END : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 캐시 사용 유무 </summary>
		public bool IsCash;
	}
	public class RES_RESEARCH_END : RES_BASE
	{
		/// <summary> 연구 정보 </summary>
		public RES_RESEARCHINFO Research;
	}

	public void SEND_REQ_RESEARCH_END(Action<RES_RESEARCH_END> action, long UID, bool IsCash = false)
	{
		REQ_RESEARCH_END _data = new REQ_RESEARCH_END();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.IsCash = IsCash;

		SendPost(Protocol.REQ_RESEARCH_END, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_RESEARCH_END res = WEB.ParsResData<RES_RESEARCH_END>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				USERINFO.SetDATA(res.Research);
				//연구 받고 장비랑 캐릭터 전투력 측정
				var equips = USERINFO.m_Items.FindAll(o => o.m_TData.GetEquipType() != EquipType.End);
				for (int i = 0; i < equips.Count; i++) equips[i].GetCombatPower();
				USERINFO.GetUserCombatPower();
			}
			action?.Invoke(res);
		});
	}
}
