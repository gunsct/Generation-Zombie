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
	public class PVPUserCampInfo
	{
		/// <summary> 0:캠프, 1:창고 레벨 </summary>
		public int[] BuildLV { get; set; } = new int[] { 1, 1 };
	}

	#region REQ_PVP_GROUP
	public class RES_PVP_BASE : RES_BASE
	{
		/// <summary> 리그 상태 0 : 준비중, 1 : 진행중 </summary>
		public int state;
		/// <summary> 시작시간 </summary>
		public long stime;
		/// <summary> 종료시간 </summary>
		public long etime;
		/// <summary> 시즌 시작시간 </summary>
		public long sstime;
		/// <summary> 시즌 종료시간 </summary>
		public long setime;
		/// <summary> 리그 번호 </summary>
		public int LeagueNo;
		/// <summary> 시즌 번호 </summary>
		public int SeasonNo;
	}

	public class RES_KILLINFO
	{
		/// <summary> 킬수 </summary>
		public int Kill;
		/// <summary> 보상 받은 횟수 </summary>
		public int RewardCnt;
		/// <summary> 마지막 업데이트 시간 </summary>
		public long UTime;

		public void InitInfo()
		{
			if (MainMng.Instance.UTILE.IsSameDay(UTime)) return;
			Kill = 0;
			RewardCnt = 0;
			UTime = (long)MainMng.Instance.UTILE.Get_ServerTime_Milli();
		}
	}

	public class RES_PVP_GROUP : RES_PVP_BASE
	{
		/// <summary> 그룹번호 </summary>
		public long UID = 0;
		/// <summary> 랭크 인덱스 </summary>
		public int Rankidx = 0;
		/// <summary> 유저 정보, 언랭은 자기자신만 내려옴 </summary>
		public List<RES_PVP_USER_BASE> Users;

		public RES_KILLINFO MyKillInfo = new RES_KILLINFO();
		/// <summary> 지난 리그 및 시즌 보상 받을 상태
		/// <para> 0 : 시즌 보상 </para>
		/// <para> 1 : 리그 보상 </para>
		/// </summary>
		public RewardState[] RState = new RewardState[2];

		/// <summary> 내 최대 승급 랭크 인덱스 </summary>
		public int MyMaxRankIdx;
		/// <summary> 보상을 받은 최초 달성 랭킹  </summary>
		public List<int> RankReward = new List<int>();
		/// <summary> 대전 상대 </summary>
		public RES_PVP_USER_DETAIL Target;

		/// <summary> Camp 건물 정보 </summary>
		public List<RES_CAMP_BUILD_INFO> CampBuilds = new List<RES_CAMP_BUILD_INFO>();
	}

	public class RES_PVP_RANK_REWARD_STATE
	{
		/// <summary> 리그 상태 0 : 준비중, 1 : 진행중 </summary>
		public int state;
		/// <summary> 시작시간 </summary>
		public long stime;
		/// <summary> 종료시간 </summary>
		public long etime;

		/// <summary> 리그 번호 </summary>
		public int LeagueNo;
		/// <summary> 시즌 번호 </summary>
		public int SeasonNo;
	}
	public class RES_PVP_USER_BASE
	{
		/// <summary> 순위 </summary>
		public int Rank;
		/// <summary> 유저 번호 </summary>
		public long UserNo;
		/// <summary> 점수 </summary>
		public long Power;
		/// <summary> 0:season, 1:league </summary>
		public long[] Point = new long[2];

		/// <summary> 이름 </summary>
		public string Name;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 프로필 이미지 </summary>
		public int Profile;
		/// <summary> 국가 코드 </summary>
		public string Nation;
		/// <summary> PVP 캠프 정보(대상만 정보있음 그외에서는 null) </summary>
		public PVPUserCampInfo CampInfo;
		/// <summary> 마지막 전투 시간 </summary>
		public long BTime;
		/// <summary> 일일 전투 횟수 </summary>
		public int DayPlayCnt;

		public int GetDayPlayCnt()
		{
			if (!MainMng.Instance.UTILE.IsSameDay(BTime)) DayPlayCnt = 0;
			return DayPlayCnt;
		}
	}

	public class RES_PVP_CHAR
	{
		/// <summary> 덱위치 </summary>
		public int Pos;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 등급 </summary>
		public int Grade;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 스텟 정보(해당 스텟의 정보가 없을 수 있음 없으면 0으로 계산해야됨) </summary>
		public Dictionary<StatType, float> Stat = new Dictionary<StatType, float>();

		public int GetCombatPower()
		{
			float cp = 0;
			for (StatType i = 0; i < StatType.Max; i++)
			{
				cp += Stat.ContainsKey(i) ? Stat[i] * BaseValue.COMBAT_POWER_RATIO(i) : 0;
			}
			return (int)cp;
		}
}
	#endregion

	#region REQ_PVP_GROUP_INFO
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_PVP_GROUP
	// 자신의 PVP 리그 그룹 정보
	public void SEND_REQ_PVP_GROUP_INFO(Action<RES_PVP_GROUP> action)
	{
		// 튜토리얼은 정산중일때 할 수 있으므로 가라 데이터 만들어서 넣어준다.
		if (TUTO.IsTuto(TutoKind.PVP_Main, (int)TutoType_PVP_Main.Select_PVP))
		{
			RES_PVP_GROUP res = new RES_PVP_GROUP();
			res.result_code = EResultCode.SUCCESS;
			res.state = 1;
			res.etime = (long)(UTILE.Get_ServerTime_Milli() + 3600000L);

			res.MyMaxRankIdx = res.Rankidx = 103;
			res.Users = new List<RES_PVP_USER_BASE>();
			res.Users.Add(new RES_PVP_USER_BASE() { UserNo = USERINFO.m_UID, LV = USERINFO.m_LV, Name = USERINFO.m_Name, Nation = USERINFO.m_Nation, Profile = USERINFO.m_Profile });
			res.RState[0] = RewardState.Get;
			res.RState[1] = RewardState.Get;
			var chars = new List<RES_PVP_CHAR>();
			chars.Add(new RES_PVP_CHAR() { Idx = 1027, LV = 38, Grade = 5, Pos = 0, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 140 }, { StatType.Def, 70 }, { StatType.Heal, 154 }, { StatType.HP, 154 }, { StatType.Speed, 31 }, { StatType.Critical, 0.1269f }, { StatType.CriticalDmg, 0.2813f }, { StatType.Sat, 24 }, { StatType.Hyg, 14 }, { StatType.Men, 22 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1014, LV = 37, Grade = 5, Pos = 1, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 202 }, { StatType.Def, 54 }, { StatType.Heal, 101 }, { StatType.HP, 148 }, { StatType.Speed, 30 }, { StatType.Critical, 0.1261f }, { StatType.CriticalDmg, 0.2786f }, { StatType.Sat, 24 }, { StatType.Hyg, 12 }, { StatType.Men, 24 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1039, LV = 41, Grade = 5, Pos = 2, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 151 }, { StatType.Def, 89 }, { StatType.Heal, 222 }, { StatType.HP, 196 }, { StatType.Speed, 49 }, { StatType.Critical, 0.1357f }, { StatType.CriticalDmg, 0.3107f }, { StatType.Sat, 19 }, { StatType.Hyg, 23 }, { StatType.Men, 21 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1056, LV = 40, Grade = 5, Pos = 3, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 151 }, { StatType.Def, 83 }, { StatType.Heal, 151 }, { StatType.HP, 167 }, { StatType.Speed, 30 }, { StatType.Critical, 0.1285f }, { StatType.CriticalDmg, 0.2868f }, { StatType.Sat, 19 }, { StatType.Hyg, 23 }, { StatType.Men, 21 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1004, LV = 35, Grade = 5, Pos = 4, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 157 }, { StatType.Def, 47 }, { StatType.Heal, 157 }, { StatType.HP, 111 }, { StatType.Speed, 35 }, { StatType.Critical, 0.1251f }, { StatType.CriticalDmg, 0.2754f }, { StatType.Sat, 22 }, { StatType.Hyg, 22 }, { StatType.Men, 16 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1008, LV = 35, Grade = 5, Pos = 5, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 126 }, { StatType.Def, 63 }, { StatType.Heal, 126 }, { StatType.HP, 138 }, { StatType.Speed, 28 }, { StatType.Critical, 0.1251f }, { StatType.CriticalDmg, 0.2754f }, { StatType.Sat, 30 }, { StatType.Hyg, 20 }, { StatType.Men, 10 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1031, LV = 38, Grade = 5, Pos = 6, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 105 }, { StatType.Def, 70 }, { StatType.Heal, 175 }, { StatType.HP, 154 }, { StatType.Speed, 31 }, { StatType.Critical, 0.1269f }, { StatType.CriticalDmg, 0.2813f }, { StatType.Sat, 14 }, { StatType.Hyg, 31 }, { StatType.Men, 16 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1034, LV = 36, Grade = 5, Pos = 7, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 161 }, { StatType.Def, 71 }, { StatType.Heal, 129 }, { StatType.HP, 107 }, { StatType.Speed, 29 }, { StatType.Critical, 0.1253f }, { StatType.CriticalDmg, 0.2759f }, { StatType.Sat, 18 }, { StatType.Hyg, 22 }, { StatType.Men, 20 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1042, LV = 35, Grade = 5, Pos = 8, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 107 }, { StatType.Def, 63 }, { StatType.Heal,  94 }, { StatType.HP, 208 }, { StatType.Speed, 25 }, { StatType.Critical, 0.1251f }, { StatType.CriticalDmg, 0.2754f }, { StatType.Sat, 30 }, { StatType.Hyg, 20 }, { StatType.Men, 10 } } });
			chars.Add(new RES_PVP_CHAR() { Idx = 1035, LV = 36, Grade = 5, Pos = 9, Stat = new Dictionary<StatType, float>() { { StatType.Atk, 129 }, { StatType.Def, 48 }, { StatType.Heal, 161 }, { StatType.HP, 142 }, { StatType.Speed, 26 }, { StatType.Critical, 0.1253f }, { StatType.CriticalDmg, 0.2759f }, { StatType.Sat, 18 }, { StatType.Hyg, 24 }, { StatType.Men, 18 } } });

			int cp = 0;
			for (int i = 0; i < chars.Count; i++) cp += chars[i].GetCombatPower();
			res.Target = new RES_PVP_USER_DETAIL()
			{
				UserNo = 100,
				LV = 38,
				Name = "CraftyHunter77",
				Nation = "NG",
				Profile = USERINFO.m_Profile,
				Chars = chars,
				Power = cp
			};
			action?.Invoke(res);
			return;
		}
#if NOT_USE_NET
		POPUP.Set_MsgBox(PopupName.Msg_CommingSoon, string.Empty, TDATA.GetString(876));
#else
		// ★ 시즌 데이터가 없을때는 RES_PVP_BASE 갱신정보가 내려옴
		// ERROR_NOT_FOUND_USER : 내정보없음 또는 대상 검색 실패
		// ERROR_PVP_JOIN : 최초 등록 실패(DB 등록 실패)
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		SendPost(Protocol.REQ_PVP_GROUP, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_PVP_GROUP>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res.CampBuilds);
			action?.Invoke(res);
		});
#endif
	}

	#endregion

	#region REQ_PVP_USER_DETAIL_INFO
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_PVP_USER_DETAIL_INFO
	// 해당 유저의 상세 정보
	public class RES_PVP_USER_DETAIL : RES_PVP_USER_BASE
	{
		public List<RES_PVP_CHAR> Chars = new List<RES_PVP_CHAR>();
		/// <summary> PVPPerAtkHygUp, PVPPerSupSatUp, PVPPerSupHygUp, PVPPerSupMenUp </summary>
		public Dictionary<ResearchEff, float> Research = new Dictionary<ResearchEff, float>();
	}

	public class REQ_PVP_USER_DETAIL_INFO : REQ_BASE
	{
		public int LeagueNo;
		public long Target;
	}
	public class RES_PVP_USER_DETAIL_INFO : RES_BASE
	{
		public RES_PVP_USER_DETAIL User;
	}
	public void SEND_REQ_PVP_USER_DETAIL_INFO(Action<RES_PVP_USER_DETAIL_INFO> action, int LeagueNo, long TargetUserNo)
	{
		// ERROR_NOT_FOUND_USER : 대상 검색 실패
		REQ_PVP_USER_DETAIL_INFO _data = new REQ_PVP_USER_DETAIL_INFO();
		_data.UserNo = USERINFO.m_UID;
		_data.LeagueNo = LeagueNo;
		_data.Target = TargetUserNo;
		SendPost(Protocol.REQ_PVP_USER_DETAIL_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_PVP_USER_DETAIL_INFO>(data));
		});
	}
	#endregion


	#region REQ_PVP_SEARCH_USER
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_PVP_SEARCH_USER
	// 대전 상대 검색
	public class REQ_PVP_SEARCH_USER : REQ_BASE
	{
		public bool Reset;
	}
	public class RES_PVP_SEARCH_USER : RES_BASE
	{
		public RES_PVP_USER_DETAIL User;
	}

	public void SEND_REQ_PVP_SEARCH_USER(Action<RES_PVP_SEARCH_USER> action, bool IsReset)
	{
		// ERROR_PVP_STATE : PVP 진행중 아님
		// ERROR_SHOP_LIMIT : 구매 제한
		// ERROR_NOT_FOUND_USER : 내정보없음 또는 대상 검색 실패
		// 그외 : 금액 부족
		REQ_PVP_SEARCH_USER _data = new REQ_PVP_SEARCH_USER();
		_data.UserNo = USERINFO.m_UID;
		_data.Reset = IsReset;
		SendPost(Protocol.REQ_PVP_SEARCH_USER, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_PVP_SEARCH_USER>(data));
		});
	}
	#endregion

	#region REQ_PVP_START
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_PVP_START
	// 대전 시작( 약탈이 들어가면서 로그데이터로 변경)
	public class REQ_PVP_START : REQ_BASE
	{
		/// <summary> 반격일 경우 약탈 로그의 인덱스 </summary>
		public int CounterIdx;
		/// <summary> 입장권 구매 </summary>
		public bool IsCash;
	}
	public class RES_PVP_START : RES_BASE
	{
		/// <summary> 대전 유저 정보
		/// <para>0 : 내 정보</para>
		/// <para>1 : 상대 정보</para>
		/// </summary>
		public RES_PVP_USER_DETAIL[] Users = new RES_PVP_USER_DETAIL[2];
	}

	public void SEND_REQ_PVP_START(Action<RES_PVP_START> action, bool _iscash, int _counteridx = 0)
	{
		// ERROR_PVP_STATE : PVP 진행중 아님
		// ERROR_NOT_FOUND_USER : 유저 정보 없음
		// ERROR_PVP_TARGET_NO : 타겟 검색 필요(등록된 검색 대상이 0으로 되어있음)
		REQ_PVP_START _data = new REQ_PVP_START();
		_data.UserNo = USERINFO.m_UID;
		_data.IsCash = _iscash;
		_data.CounterIdx = _counteridx;
		SendPost(Protocol.REQ_PVP_START, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_PVP_START>(data);
			if(res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.PlayPVP, 0, 0, 1);
				if(_iscash) USERINFO.m_ShopInfo.SetBuyInfo(BaseValue.PVP_TICKET_SHOP_IDX, 1);
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_PVP_END
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_PVP_END(사용안함)
	// 대전 종료

	public class REQ_PVP_END : REQ_BASE
	{
		/// <summary> 반격일 경우 약탈 로그의 인덱스 </summary>
		public int CounterIdx;

		/// <summary> 상대 유저번호 </summary>
		public long Target;
		/// <summary> 승패 </summary>
		public bool IsWIN;
		/// <summary> 킬 수 </summary>
		public int KillCnt;
	}

	public class RES_PVP_END : RES_BASE
	{
		/// <summary> 내 그룹 유저 랭킹 정보 </summary>
		public List<RES_PVP_USER_BASE> Users;
		public RES_KILLINFO MyKillInfo = new RES_KILLINFO();
		/// <summary> 지급된 포임트 전투중 다른사람이 내 방어덱과 전투를 했을수있으므로 초기화되는 포인트와 다를 수 있음
		/// <para>0:시즌 포인트</para>
		/// <para>1:리그 포인트</para>
		/// </summary>
		public int[] Point = new int[2];

		public RES_PVP_USER_BASE GetMyInfo(long _uid) {
			return Users.Find(o => o.UserNo == _uid);
		}
	}

	public void SEND_REQ_PVP_END(Action<RES_PVP_END> action, long Target, bool IsWin, int KillCnt, int _counteridx = 0)
	{
		// ERROR_PVP_STATE : PVP 진행중 아님
		// ERROR_NOT_FOUND_USER : 유저 정보 없음
		// ERROR_PVP_TARGET_NO : 타겟 번호 다름
		REQ_PVP_END _data = new REQ_PVP_END();
		_data.UserNo = USERINFO.m_UID;
		_data.Target = Target;
		_data.IsWIN = IsWin;
		_data.KillCnt = KillCnt;
		_data.CounterIdx = _counteridx;
		SendPost(Protocol.REQ_PVP_END, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_PVP_END>(data);
			if (IsWin && res.IsSuccess()) USERINFO.Check_Mission(MissionType.VicPVP, 0, 0, 1);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_PVP_GROUP
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_PVP_REWARD
	// PVP 보상 받기
	public enum PVP_RewardKind
	{
		/// <summary> 일일 킬 수 보상 </summary>
		DayKill = 0,
		/// <summary> 리그 보상 </summary>
		League,
		/// <summary> 시즌 보상 </summary>
		Season,
		/// <summary> 첫 랭크 달성 보상 </summary>
		FirstRank,
		End
	}

	public class REQ_PVP_REWARD : REQ_BASE
	{
		public PVP_RewardKind Kind;
		/// <summary> PVP_RewardKind.FirstRank 일때 사용된 랭크 인덱스 </summary>
		public int RankIdx;
	}

	public class RES_PVP_REWARD : RES_BASE
	{
		/// <summary> 리그 또는 시즌 번호 </summary>
		public int No;
		/// <summary> 받은 보상의 랭크인덱스 </summary>
		public int RankIdx;
		/// <summary> 3위까지의 유저 정보 </summary>
		public List<RES_RANKING_INFO> RankUsers;
		/// <summary> 자신의 정보 </summary>
		public RES_RANKING_INFO MyInfo;
		/// <summary> 보상후 정보 </summary>
		public RES_KILLINFO MyKillInfo;
	}

	public void SEND_REQ_PVP_REWARD(Action<RES_PVP_REWARD> action, PVP_RewardKind kind, int RankIdx)
	{
		// ERROR_PVP_STATE : PVP 진행중 아님
		// ERROR_PVP_NOT_REWARD_KIND : 잘못된 보상 종류 들어옴
		// ERROR_NOT_FOUND_USER : 리그, 시즌의 유저 정보 없음
		// ERROR_PVP_REWARD_CNT : kill 보상 횟수 넘어감
		// ERROR_PVP_KILL_CNT : kill 수 달성 안됨
		// ERROR_NOT_FOUND_PVP_LEAGUE : 리그 정보 없음(첫 리그에 호출됨)
		// ERROR_REWARD : 이미 지급됨 또는 지급 대상 아님
		// ERROR_TOOLDATA : 툴데이터 못찾음
		// ERROR_NOT_FOUND_PVP_SEASON : 시즌 정보 없음(첫 시즌에 호출됨)
		REQ_PVP_REWARD _data = new REQ_PVP_REWARD();
		_data.UserNo = USERINFO.m_UID;
		_data.Kind = kind;
		_data.RankIdx = RankIdx;
		SendPost(Protocol.REQ_PVP_REWARD, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_PVP_REWARD>(data));
		});
	}
	#endregion

}
