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
	public class REQ_STAGE : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
	}

	public class RES_ALL_STAGE : RES_BASE
	{
		public List<RES_STAGE> Stage = new List<RES_STAGE>();
	}

	public class RES_STAGE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 스테이지 모드 </summary>
		public StageContentType Type;
		/// <summary> 스테이지 => 스테이지 인덱스, 던전 => 레벨 </summary>
		public List<UserInfo.StageIdx> Idxs;
		/// <summary> 하루마다 초기화 0 : 일반 입장 제한, 1 : 구매 제한 </summary>
		public int[] PlayLimit = new int[2];
		/// <summary> 마지막 플레이한 시간 (limit초기화용) </summary>
		public long LTime;
	}


    public void SEND_REQ_STAGE(Action<RES_ALL_STAGE> action = null, long UID = 0)
    {
        REQ_STAGE _data = new REQ_STAGE();
        _data.UserNo = USERINFO.m_UID;
        _data.UID = UID;
        SendPost(Protocol.REQ_STAGE, JsonConvert.SerializeObject(_data), (result, data) => {
            RES_ALL_STAGE res = ParsResData<RES_ALL_STAGE>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.Stage);
            action?.Invoke(res);
        });
    }



    /////////////////////////////////////////////////////////////////////////////////////
    // REQ_STAGE_BUY_LIMIT
    public class REQ_STAGE_BUY_LIMIT : REQ_BASE
    {
        /// <summary> 스테이지 정보 고유번호 </summary>
        public long UID;
        /// <summary> 구매 개수 </summary>
        public int Cnt;
    }

    public class RES_STAGE_BUY_LIMIT : RES_BASE
    {
        /// <summary> 스테이지 정보 </summary>
        public RES_STAGE Stage;
    }

    public void SEND_REQ_STAGE_BUY_LIMIT(Action<RES_STAGE_BUY_LIMIT> action, long UID, int Cnt = 1)
    {
        REQ_STAGE_BUY_LIMIT _data = new REQ_STAGE_BUY_LIMIT();
        _data.UserNo = USERINFO.m_UID;
        _data.UID = UID;
        _data.Cnt = Cnt;
        SendPost(Protocol.REQ_STAGE_BUY_LIMIT, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_STAGE_BUY_LIMIT res = ParsResData<RES_STAGE_BUY_LIMIT>(data);
			if (res.IsSuccess())
			{
				int shopidx = BaseValue.GetStageLimitItem(res.Stage.Type);
				if(shopidx != 0) USERINFO.m_ShopInfo.SetBuyInfo(shopidx, 1);
				USERINFO.SetDATA(res.Stage);
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(803));
			}
			action?.Invoke(res);
        });
    }


    /////////////////////////////////////////////////////////////////////////////////////
    // REQ_STAGE_START
    public class REQ_STAGE_START : REQ_BASE
    {
        /// <summary> 스테이지 정보 고유번호 </summary>
        public long UID;
        /// <summary> 플레이 할 요일(요일던전용) </summary>
        public DayOfWeek Week;
        /// <summary> 위치 </summary>
        public int Pos;
        /// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
        public int Idx;
        /// <summary> 사용덱 위치 </summary>
        public int Deck;
        /// <summary> 스테이지 다시 시작으로 들어왔는지 여부 </summary>
        public bool IsReStart;

		/// <summary> 진행한 이벤트 아이디 0이면 일반스테이지 or 다운타운  </summary>
		public long EUID;
	}

	public class RES_STAGE_START : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
		/// <summary> 완료된 길드 연구 정보</summary>
		public List<int> GuildEndRes = new List<int>();

		/// <summary> 시작 스테이지의 플레이 코드 </summary>
		public string PlayCode;

		/// <summary> 이벤트 정보 </summary>
		public RES_MY_FAEVENT_INFO Event;
	}


    public void SEND_REQ_STAGE_START(Action<RES_STAGE_START> action, long UID, DayOfWeek Week, int Pos, int Idx, int Deck, bool IsReStart = false, long _euid = 0)
	{
		REQ_STAGE_START _data = new REQ_STAGE_START();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = Idx;
		_data.Deck = Deck;
        _data.IsReStart = IsReStart;
		_data.EUID = _euid;

		SendPost(Protocol.REQ_STAGE_START, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
            RES_STAGE_START res = ParsResData<RES_STAGE_START>(data);

			if (res.IsSuccess())
			{
				USERINFO.SetDATA(res.Stage);
				STAGEINFO.PlayCode = res.PlayCode;
				STAGEINFO.EUID = _data.EUID;
				var result = USERINFO.m_Guild.EndRes.Except(res.GuildEndRes).ToList();
				// 연구값이 달라지는경우
				// 길드 가입 승인 되었을때
				// 길드 연구가 완료 되었을때
				if (result.Count > 0) USERINFO.m_Guild.IsReLoad = true;
				USERINFO.m_Guild.SetData(res.GuildEndRes);
				RES_SHOP_USER_BUY_INFO continuebuyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.CONTINUETICKET_SHOP_IDX);
				if (continuebuyinfo != null) continuebuyinfo.Cnt = 0;
				RES_SHOP_USER_BUY_INFO continuegoldbuyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.CONTINUETICKET_GOLD_SHOP_IDX);
				if (continuegoldbuyinfo != null) continuegoldbuyinfo.Cnt = 0;
				RES_SHOP_USER_BUY_INFO stagererollbuyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.SHOP_IDX_STAGE_REROLLING);
				if (stagererollbuyinfo != null) stagererollbuyinfo.Cnt = 0;
			}
            action?.Invoke(res);
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_CLEAR
	public class REQ_STAGE_CLEAR : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
		public int Idx;

		/// <summary> 시작 스테이지의 플레이 코드 </summary>
		public string PlayCode;

		/// <summary> 전투력 </summary>
		public int Power;
		/// <summary> 진행턴 </summary>
		public int Turn;
		/// <summary> 진행시간 </summary>
		public int RTime;

		/// <summary> 진행한 이벤트 아이디 0이면 일반스테이지 or 다운타운  </summary>
		public long EUID;
		/// <summary> 이벤트에서 사용한 사용덱 위치 </summary>
		public int EDeck;
	}

	public class RES_STAGE_CLEAR : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
		/// <summary> 돌발 이벤트 정보 </summary>
		public int AddEventIdx;
		/// <summary> 돌발 이벤트 표기 유저 이름 </summary>
		public string AddEventUserName;
		/// <summary> 클리어 상위 퍼센트[0 : 이전값, 1:현재값] (0.1f~100f) </summary>
		public float[] ClearPer = null;
		/// <summary> 이벤트 정보 </summary>
		public RES_MY_FAEVENT_INFO Event;
	}

	public void SEND_REQ_STAGE_CLEAR(Action<RES_STAGE_CLEAR> action, UserInfo.Stage info, DayOfWeek Week, int Pos, int Idx, string PlayCode, int Turn, int RunTime, long _euid = 0, int _edeckpos = 0)
	{
		REQ_STAGE_CLEAR _data = new REQ_STAGE_CLEAR();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = Idx;
		_data.PlayCode = PlayCode;
		_data.Power = USERINFO.m_PlayDeck.m_Char.Sum(o => o < 1 ? 0 : USERINFO.GetChar(o).m_CP);
		_data.Turn = Turn;
		_data.RTime = RunTime;
		_data.EUID = _euid;
		_data.EDeck = _edeckpos;

		SendPost(Protocol.REQ_STAGE_CLEAR, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_STAGE_CLEAR res = ParsResData<RES_STAGE_CLEAR>(data);
			if (res.IsSuccess())
			{
				USERINFO.SetDATA(res.Stage);
				USERINFO.m_Event.SetDATA(res.Event);
				USERINFO.m_AddEvent = res.AddEventIdx;
				USERINFO.m_AddEventName = res.AddEventUserName;
				USERINFO.m_Achieve.Check_StageClear(info.Mode, Pos, 1, Idx, res.Event != null ? res.Event.EventUID : 0);
				if (info.Mode == StageContentType.Stage && Pos == 0)
				{
					switch(Idx)
					{
					case 301:
						HIVE.Analytics_TutorialComplete();
						break;
					case 401:
						HIVE.Analytics_Stage401();
						break;
					}
				}
			}
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_CLEAR
	public class REQ_STAGE_FAIL : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
		public int Idx;


		/// <summary> 실패 사유 </summary>
		public int FailType;
		/// <summary> 전투력 </summary>
		public int Power;
		/// <summary> 진행턴 </summary>
		public int Turn;
		/// <summary> 진행시간 </summary>
		public int RTime;
	}

	public void SEND_REQ_STAGE_FAIL(UserInfo.Stage info, DayOfWeek Week, int Pos, int Idx, string PlayCode, int FailType, int Turn, int RunTime)
	{
		REQ_STAGE_FAIL _data = new REQ_STAGE_FAIL();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = Idx;
		_data.FailType = FailType;
		_data.Power = USERINFO.m_PlayDeck.m_Char.Sum(o => o < 1 ? 0 : USERINFO.GetChar(o).m_CP);
		_data.Turn = Turn;
		_data.RTime = RunTime;

		SendPost(Protocol.REQ_STAGE_FAIL, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_BASE res = ParsResData<RES_BASE>(data);
			if(res.IsSuccess())
			{
				switch(info.Mode)
				{
				case StageContentType.Stage:
					USERINFO.m_Achieve.Check_Achieve(AchieveType.Stage_Fail);
					break;
				case StageContentType.Bank:
				case StageContentType.Academy:
				case StageContentType.University:
				case StageContentType.Tower:
				case StageContentType.Cemetery:
				case StageContentType.Factory:
				case StageContentType.Subway:
					USERINFO.m_Achieve.Check_Achieve(AchieveType.DownTown_Fail);
					break;
				}
				
			}
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_CLEAR
	public class REQ_STAGE_FAIL_REWARD : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
		public int Idx;
	}

	public void SEND_REQ_STAGE_FAIL_REWARD(Action<RES_BASE> action, UserInfo.Stage info, DayOfWeek Week, int Pos, int Idx)
	{
		REQ_STAGE_FAIL_REWARD _data = new REQ_STAGE_FAIL_REWARD();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = Idx;

		SendPost(Protocol.REQ_STAGE_FAIL_REWARD, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_CLEAR_TICKET

	public class REQ_STAGE_CLEAR_TICKET : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
		public int Idx;

		/// <summary> 소탕권 사용 개수 </summary>
		public int Ticket;
	}

	public class RES_STAGE_CLEAR_TICKET : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
		/// <summary> 크리어 체크된 개수 </summary>
		public int ClearCnt = 1;
	}
	public void SEND_REQ_STAGE_CLEAR_TICKET(Action<RES_STAGE_CLEAR_TICKET> action, UserInfo.Stage info, DayOfWeek Week, int Pos, int Idx, int UseTicket = 1)
	{
		REQ_STAGE_CLEAR_TICKET _data = new REQ_STAGE_CLEAR_TICKET();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = Idx;
		_data.Ticket = UseTicket;

		SendPost(Protocol.REQ_STAGE_CLEAR_TICKET, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_STAGE_CLEAR_TICKET res = ParsResData<RES_STAGE_CLEAR_TICKET>(data);
			if (res.IsSuccess())
			{
				USERINFO.SetDATA(res.Stage);
				USERINFO.m_Achieve.Check_StageClear(info.Mode, Pos, res.ClearCnt, Idx);
			}
			action?.Invoke(res);
		});
	}


	#region REQ_RESET_REPLAY
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage RePlay Reset
	public class REQ_RESET_REPLAY : REQ_BASE
	{
		/// <summary> 스테이지 컨텐츠 </summary>
		public StageContentType Content;
	}
	public class RES_RESET_REPLAY : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
	}
	public void SEND_REQ_RESET_REPLAY(Action<RES_RESET_REPLAY> action, StageContentType type)
	{
		REQ_RESET_REPLAY _data = new REQ_RESET_REPLAY();

		_data.UserNo = USERINFO.m_UID;
		_data.Content = type;

		SendPost(Protocol.REQ_RESET_REPLAY, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_RESET_REPLAY res = ParsResData<RES_RESET_REPLAY>(data);
			if (res.IsSuccess()) {
				USERINFO.SetDATA(res.Stage);
				USERINFO.m_ShopInfo.SetBuyInfo(BaseValue.REPLAY_REFRESH_SHOP_IDX, 1);
			}
			action?.Invoke(res);
		});
	}
	#endregion


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_ALTREWARD
	public class REQ_STAGE_ALTREWARD : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;

		/// <summary> 시드 선택 내용 </summary>
		public REQ_ALTERNATIVE_INFO AltInfo;
	}

	public class RES_STAGE_ALTREWARD : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
	}
	public void SEND_REQ_STAGE_ALTREWARD(Action<RES_STAGE_ALTREWARD> action, long UID, DayOfWeek Week, int Pos, REQ_ALTERNATIVE_INFO altinfo)
	{
		REQ_STAGE_ALTREWARD _data = new REQ_STAGE_ALTREWARD();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.AltInfo = altinfo;

		SendPost(Protocol.REQ_STAGE_ALTREWARD, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_STAGE_ALTREWARD res = ParsResData<RES_STAGE_ALTREWARD>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.Stage);
			action?.Invoke(res);
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_CHAPTERREWARD
	public class REQ_STAGE_CHAPTERREWARD : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
	}
	public class RES_STAGE_CHAPTERREWARD : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
	}
	public void SEND_REQ_STAGE_CHAPTERREWARD(Action<RES_STAGE_CHAPTERREWARD> action, long UID, DayOfWeek Week, int Pos)
	{
		REQ_STAGE_CHAPTERREWARD _data = new REQ_STAGE_CHAPTERREWARD();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Week = Week;
		_data.Pos = Pos;

		SendPost(Protocol.REQ_STAGE_CHAPTERREWARD, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_STAGE_CHAPTERREWARD res = ParsResData<RES_STAGE_CHAPTERREWARD>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.Stage);
			action?.Invoke(res);
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_TALKSELECT
	public class REQ_STAGE_TALKSELECT : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> 선택한 CaseSelectTable Idx </summary>
		public int Select;
		/// <summary> 선택을 요구한 다이얼로그 </summary>
		public int Dlg;
	}
	public class RES_STAGE_TALKSELECT : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
	}
	public void SEND_REQ_STAGE_TALKSELECT(Action<RES_STAGE_TALKSELECT> action, long UID, DayOfWeek Week, int Pos, int Dlg, int Select)
	{
		REQ_STAGE_TALKSELECT _data = new REQ_STAGE_TALKSELECT();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Dlg = Dlg;
		_data.Select = Select;

		SendPost(Protocol.REQ_STAGE_TALKSELECT, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_STAGE_TALKSELECT res = ParsResData<RES_STAGE_TALKSELECT>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.Stage);
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Rerolling
	public class REQ_STAGE_REROLLING : REQ_BASE
	{
		/// <summary> 스테이지 정보 고유번호 </summary>
		public long UID;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
		public int Idx;
		/// <summary> 진행한 이벤트 아이디 0이면 일반스테이지 or 다운타운  </summary>
		public long EUID;
	}

	public class RES_STAGE_REROLLING : RES_BASE
	{
		/// <summary> 스테이지 정보 </summary>
		public RES_STAGE Stage;
		/// <summary> 이벤트 정보 </summary>
		public RES_MY_FAEVENT_INFO Event;
	}
	public void SEND_REQ_STAGE_REROLLING(Action<RES_STAGE_REROLLING> action, long UID, DayOfWeek Week, int Pos,int _sidx = 0, long _euid = 0)
	{
		REQ_STAGE_REROLLING _data = new REQ_STAGE_REROLLING();

		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = _sidx;
		_data.EUID = _euid;

		SendPost(Protocol.REQ_STAGE_REROLLING, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_STAGE_REROLLING res = ParsResData<RES_STAGE_REROLLING>(data);
			if (res.IsSuccess())
			{
				USERINFO.SetDATA(res.Stage);
			}
			action?.Invoke(res);
		});
	}

	#region REQ_USER_STAGE_PICK_INFO
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Stage Rerolling
	public class REQ_USER_STAGE_PICK_INFO : REQ_BASE
	{
		/// <summary> 스테이지 컨텐츠 </summary>
		public StageContentType Type;
		/// <summary> 플레이 할 요일(요일던전용) </summary>
		public DayOfWeek Week;
		/// <summary> 위치 </summary>
		public int Pos;
		/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
		public int Idx;
	}

	public class RES_USER_STAGE_PICK_INFO : RES_BASE
	{
		/// <summary> 픽 캐릭터 비율 </summary>
		public UserPickCharInfo PickChar;
	}

	public void SEND_REQ_USER_STAGE_PICK_INFO(Action<RES_USER_STAGE_PICK_INFO> action, StageContentType Type, DayOfWeek Week, int Pos, int Idx)
	{
		REQ_USER_STAGE_PICK_INFO _data = new REQ_USER_STAGE_PICK_INFO();

		_data.UserNo = USERINFO.m_UID;
		_data.Type = Type;
		_data.Week = Week;
		_data.Pos = Pos;
		_data.Idx = Idx;

		SendPost(Protocol.REQ_USER_STAGE_PICK_INFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_USER_STAGE_PICK_INFO res = ParsResData<RES_USER_STAGE_PICK_INFO>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.PickChar);
			action?.Invoke(res);
		});
	}
	#endregion
}
