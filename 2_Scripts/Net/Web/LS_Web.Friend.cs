using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	public enum Friend_State
	{
		/// <summary> 수락 대기 </summary>
		Idle = 0,
		/// <summary> 친구 </summary>
		Friend,
		/// <summary> 초대한 친구 </summary>
		Invited,
		/// <summary> 제거된 친구 </summary>
		Deleted,
		/// <summary>  </summary>
		End
	}
	public enum Friend_Gift_State
	{
		/// <summary> 지급 안함 </summary>
		None = 0,
		/// <summary> 지급 함 </summary>
		Gift,
		/// <summary> 받음 </summary>
		Get,
		/// <summary>  </summary>
		End
	}

	#region REQ_FRIENDINFO
	public class RES_FRIEND : RES_BASE
	{
		/// <summary> 제거 횟수 0부터 시작 </summary>
		public int RemoveCnt;
		/// <summary> 제거한 시간(하루가 지났으면 자동으로 초기화 하면됨) </summary>
		public long RemoveTime;
		/// <summary> 선물 보낸 횟수 </summary>
		public int RecieveCnt;
		/// <summary> 선물 받은 횟수 </summary>
		public long RecieveTime;
		/// <summary> 친구 리스트 (요청 포함됨) </summary>
		public List<RES_FRIENDINFO> Friends = new List<RES_FRIENDINFO>();
	}

	public class RES_FRIENDINFO : RES_RECOMMEND_USER
	{
		/// <summary> 친구 상태 </summary>
		public Friend_State State;
		/// <summary> 최초 생성 시간 (초대 보내거나 받은 시간) </summary>
		public long CTime;
		/// <summary> 선물관련 시간정보 0 : 보낸시간, 1 : 받은시간, 2 : 획득시간 </summary>
		public long[] GTimes = new long[3];

		public Friend_Gift_State GetGiftState()
		{
			// 검색 대상 아님
			if(State != Friend_State.Friend && State != Friend_State.Deleted) return Friend_Gift_State.None;
			// 받은적 없음
			if (CTime > GTimes[1]) return Friend_Gift_State.None;
			// 받은 시간 체크
			if (MainMng.Instance.UTILE.IsSameDay(GTimes[2])) return Friend_Gift_State.Get;
			// 받은적 없음
			if (GTimes[1] <= GTimes[2]) return Friend_Gift_State.None;
			return Friend_Gift_State.Gift;
		}

		public bool IsSendGift()
		{
			return !MainMng.Instance.UTILE.IsSameDay(GTimes[0]);
		}

		public string GetInviteCode()
		{
			return Utile_Class.UserNoEncrypt(UserNo);
		}
	}

	public void SEND_REQ_FRIEND(Action<RES_FRIEND> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_FRIEND, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_FRIEND>(data);
			if(res.IsSuccess()) USERINFO.m_Friend.SetDATA(res);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_FRIEND_FIND
	public class REQ_FRIEND_FIND : REQ_BASE
	{
		public string Code;
	}
	public class RES_FRIEND_FIND : RES_BASE
	{
		public List<RES_RECOMMEND_USER> Users = new List<RES_RECOMMEND_USER>();
	}

	public void SEND_REQ_FRIEND_FIND(Action<RES_FRIEND_FIND> action, string code)
	{
		REQ_FRIEND_FIND _data = new REQ_FRIEND_FIND();
		_data.UserNo = USERINFO.m_UID;
		_data.Code = code;

		SendPost(Protocol.REQ_FRIEND_FIND, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_FRIEND_FIND>(data));
		});
	}
	#endregion

	#region REQ_FRIEND_RECOMMEND
	public class RES_FRIEND_RECOMMEND : RES_BASE
	{
		public List<RES_RECOMMEND_USER> Users = new List<RES_RECOMMEND_USER>();
	}

	public class RES_RECOMMEND_USER : RES_BASE
	{
		/// <summary> 유저번호 </summary>
		public long UserNo;
		/// <summary> 닉네임 </summary>
		public string Name;
		[JsonIgnore] public string m_Name { get { return BaseValue.GetUserName(Name); } }
		/// <summary> 국가 </summary>
		public string Nation;
		/// <summary> 프로필 이미지 </summary>
		public int Profile;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 메인 스테이지 </summary>
		public int Stage;
		/// <summary> 전투력 </summary>
		public int Power;
		/// <summary> 마지막 업데이트 시간 </summary>
		public long UTime;

		public string Get_RefCode()
		{
			return Utile_Class.UserNoEncrypt(UserNo);
		}
	}
	public void SEND_REQ_FRIEND_RECOMMEND(Action<RES_FRIEND_RECOMMEND> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_FRIEND_RECOMMEND, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_FRIEND_RECOMMEND>(data));
		});
	}
	#endregion

	#region REQ_FRIEND_INVITE
	public class REQ_FRIEND_INVITE : REQ_BASE
	{
		public string Code;
	}

	public void SEND_REQ_FRIEND_INVITE(Action<RES_BASE> action, string Code)
	{
		REQ_FRIEND_INVITE _data = new REQ_FRIEND_INVITE();
		_data.UserNo = USERINFO.m_UID;
		_data.Code = Code;

		SendPost(Protocol.REQ_FRIEND_INVITE, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	#endregion

	#region REQ_FRIEND_DELETE
	public class REQ_FRIEND_DELETE : REQ_BASE
	{
		public long Target;
	}
	public void SEND_REQ_FRIEND_DELETE(Action<RES_BASE> action, long target_userno)
	{
		REQ_FRIEND_DELETE _data = new REQ_FRIEND_DELETE();
		_data.UserNo = USERINFO.m_UID;
		_data.Target = target_userno;

		SendPost(Protocol.REQ_FRIEND_DELETE, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.m_Friend.RemoveCnt++;
			USERINFO.m_Friend.RemoveTime = (long)UTILE.Get_ServerTime_Milli();
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	#endregion

	#region REQ_FRIEND_ACCEPT
	public class REQ_FRIEND_ACCEPT : REQ_BASE
	{
		public long Target;
	}

	public class RES_FRIEND_ACCEPT : RES_BASE
	{
		public RES_FRIENDINFO Info;
	}
	public void SEND_REQ_FRIEND_ACCEPT(Action<RES_BASE> action, long Target)
	{
		REQ_FRIEND_ACCEPT _data = new REQ_FRIEND_ACCEPT();
		_data.UserNo = USERINFO.m_UID;
		_data.Target = Target;

		SendPost(Protocol.REQ_FRIEND_ACCEPT, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_FRIEND_ACCEPT res = ParsResData<RES_FRIEND_ACCEPT>(data);
			if(res.IsSuccess())
			{
				USERINFO.m_Friend.Friends.RemoveAll(o => o.UserNo == res.Info.UserNo);
				USERINFO.m_Friend.Friends.Add(res.Info);
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_FRIEND_GIFT_GIVE
	public class REQ_FRIEND_GIFT_GIVE : REQ_BASE
	{
		public List<long> Targets;
	}
	public class RES_FRIEND_GIFT_GIVE : RES_BASE
	{
		public List<long> Targets;
	}
	public void SEND_REQ_FRIEND_GIFT_GIVE(Action<RES_BASE> action, List<long> Targets)
	{
		REQ_FRIEND_GIFT_GIVE _data = new REQ_FRIEND_GIFT_GIVE();
		_data.UserNo = USERINFO.m_UID;
		_data.Targets = Targets;

		SendPost(Protocol.REQ_FRIEND_GIFT_GIVE, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_FRIEND_GIFT_GIVE>(data);
			if (res.IsSuccess() && Targets.Count > 0)
			{
				USERINFO.Check_Mission(MissionType.Friend, 0, 0, Targets.Count);
				long time = (long)UTILE.Get_ServerTime_Milli();
				USERINFO.m_Friend.Friends.FindAll(o =>
				{
					if (res.Targets.Contains(o.UserNo)) o.GTimes[0] = time;
					return true;
				});
			};
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_FRIEND_GIFT_GET
	public class REQ_FRIEND_GIFT_GET : REQ_BASE
	{
		public List<long> Targets;
	}
	public class RES_FRIEND_GIFT_GET : RES_BASE
	{
		public List<long> Targets;
	}

	public void SEND_REQ_FRIEND_GIFT_GET(Action<RES_BASE> action, List<long> Targets)
	{
		REQ_FRIEND_GIFT_GET _data = new REQ_FRIEND_GIFT_GET();
		_data.UserNo = USERINFO.m_UID;
		_data.Targets = Targets;

		SendPost(Protocol.REQ_FRIEND_GIFT_GET, JsonConvert.SerializeObject(_data), (result, data) =>
		{
			var res = ParsResData<RES_FRIEND_GIFT_GET>(data);
			if (res.IsSuccess() && Targets.Count > 0)
			{
				long time = (long)UTILE.Get_ServerTime_Milli();

				USERINFO.m_Friend.Friends.FindAll(o =>
				{
					if (res.Targets.Contains(o.UserNo)) o.GTimes[2] = time;
					return true;
				});
			};
			action?.Invoke(res);
		});
	}
	#endregion
}
