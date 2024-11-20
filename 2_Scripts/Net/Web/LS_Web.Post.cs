using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public enum RewardState
{
	/// <summary> 대기중 </summary>
	Idle,
	/// <summary> 보상 받음 </summary>
	Get,
	/// <summary> 보상 없음 </summary>
	None
}

public partial class LS_Web
{
	public class REQ_POSTINFO : REQ_BASE
	{
		/// <summary> 마지막으로 받은 우편함 정보 UID </summary>
		public long LUID;
	}


	public class RES_ALL_POSTINFO : RES_BASE
	{
		public List<RES_POSTINFO> Posts = new List<RES_POSTINFO>();
	}


	public class RES_POSTREWARD
	{
		/// <summary> 보상 종류 </summary>
		public RewardKind Kind = 0;
		/// <summary> 인덱스 </summary>
		public int Idx = 0;
		/// <summary> 개수 </summary>
		public int Cnt = 1;
		/// <summary> 캐릭터 등급 </summary>
		public int Grade = 0;
		/// <summary> 캐릭터, 장비 레벨 </summary>
		public int LV = 1;
		/// <summary> 보상 상태 </summary>
		public RewardState State;

		public RES_POSTREWARD() { }

		public RES_POSTREWARD(PostReward info) {
			SetData(info);
		}

		public void SetData(PostReward info) {
			Kind = info.Kind;
			Idx = info.Idx;
			Cnt = info.Cnt;
			State = info.State;
			Grade = info.Grade;
			LV = info.LV;
		}
		public PostReward GetInfo()
		{
			return new PostReward()
			{
				Kind = Kind,
				Idx = Idx,
				Cnt = Cnt,
				Grade = Grade,
				LV = LV,
				State = State
			};
		}
	}

	public class RES_POSTINFO : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 우편 타입 </summary>
		public RewardPos Type;
		/// <summary> 알림 정보 </summary>
		public List<long> Values;
		/// <summary> 보상 목록 </summary>
		public List<RES_POSTREWARD> Items = new List<RES_POSTREWARD>();
		/// <summary> 수령 종료시간 </summary>
		public long ETime = 0;
		/// <summary> 0:받기전, 1:보상 받음, 2:제거됨 </summary>
		public int State = 0;

		/// <summary> Hive 전용 표시 메세지 </summary>
		public Dictionary<string, PostInfo.Msg> Message = null;

		public RES_POSTINFO() { }

		public RES_POSTINFO(PostInfo info) {
			SetData(info);
		}

		public void SetData(PostInfo info) {
			UID = info.UID;
			Type = info.Type;
			Values = info.Values;
			ETime = info.ETime;//LSUtile.GetUnixTimestampMillis(info.ETime);
			Items.AddRange(info.Rewards.FindAll(t => t != null && t.Idx != 0 && t.Kind != RewardKind.None).Select(o => new RES_POSTREWARD(o)));
			State = info.State;
			Message = info.Message;
		}



		///// <summary> 고유번호 </summary>
		//public long UID;
		///// <summary> 제목 인덱스 </summary>
		//public int Title = 0;
		///// <summary> 설명 인덱스 </summary>
		//public int Msg = 0;
		///// <summary> 보상 목록 </summary>
		//public List<RES_POSTREWARD> Items = new List<RES_POSTREWARD>();
		///// <summary> 수령 종료시간 </summary>
		//public long ETime = 0;
		///// <summary>
		///// "State : 0:받기전, 1:보상 받음, 2:제거됨(목록 제거가 없으므로 1이상이면 제거함)
		///// <para>현재는 리워드가 1개뿐이라 받기 성공하면 무조건 1이지만</para>
		///// <para>2개이상일경우 하나만 받아질수도 있으므로 이경우 0으로</para>
		///// <para>보냄 다시 요청하면 받지 않은 아이템만 지급함</para>
		///// <para>보상 상태는 리워드의 State 참조"</para>
		///// </summary>
		//public int State;
	}

	public void SEND_REQ_POSTINFO(Action<RES_ALL_POSTINFO> action)
	{
		REQ_POSTINFO _data = new REQ_POSTINFO();
		_data.UserNo = USERINFO.m_UID;
		_data.LUID = USERINFO.m_Posts.Count < 1 ? 0 : USERINFO.m_Posts.Max(o => o.UID);

		SendPost(Protocol.REQ_POSTINFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_ALL_POSTINFO res = WEB.ParsResData<RES_ALL_POSTINFO>(data);
			if (res.IsSuccess())
			{
				// 전체 받기
				USERINFO.SetDATA(res.Posts);
			}
			action?.Invoke(res);
		});
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Post Get Item
	public class REQ_POST_REWARD : REQ_BASE
	{
		/// <summary> 마지막으로 받은 우편함 정보 UID </summary>
		public List<long> UIDs = new List<long>();
		/// <summary> 픽업 or 선택권 데이터 인덱스 </summary>
		public List<int> PickIdxs;
	}
	public void SEND_REQ_POST_REWARD(Action<RES_BASE> action, List<long> UIDS, List<int> _pickupidx = null)
	{
		REQ_POST_REWARD _data = new REQ_POST_REWARD();
		_data.UserNo = USERINFO.m_UID;
		_data.UIDs = new List<long>();
		_data.UIDs.AddRange(UIDS);
		_data.PickIdxs = _pickupidx;

		SendPost(Protocol.REQ_POST_REWARD, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(WEB.ParsResData<RES_BASE>(data));
		});
	}

}
