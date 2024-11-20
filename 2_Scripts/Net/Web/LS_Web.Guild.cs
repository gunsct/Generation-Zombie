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
	#region Guild Base Info
	public enum GuildInfoMode
	{
		Base = 0,
		Member = 1,
		EndRes = Member << 1,
		Items = EndRes << 1,
		All = Member | EndRes | Items
	}
	public class REQ_GUILD : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 맴버, 연구, 아이템 전부 받기 </summary>
		public int InfoMode;
	}

	public class RES_GUILDINFO_SIMPLE : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 이름 </summary>
		public string Name;
		/// <summary> 국가 코드 </summary>
		public string Nation;
		/// <summary> 인삿말 </summary>
		public string Intro;
		/// <summary> 공지사항 </summary>
		public string Notice;
		/// <summary> 길드 마크 </summary>
		public int Icon;
		/// <summary> 경험치(총 기여도) </summary>
		public long Exp;
		/// <summary> 가입방법 </summary>
		public GuildJoinType JoinType;
		/// <summary> 가입 레벨 </summary>
		public int JoinLV;
		/// <summary> 진행중 연구 인덱스 </summary>
		public int ResIdx;
		/// <summary> 연구 경험치 </summary>
		public long ResExp;

		public int UserCnt;
		public int MaxUserCnt;
		public long TPower;


		public void Calc_Exp(out int LV, out long Exp)
		{
			MainMng.Instance.TDATA.GetGuild_LV(this.Exp, out LV, out Exp);
		}

		public void Copy(RES_GUILDINFO_SIMPLE info)
		{
			UID = info.UID;
			Name = info.Name;
			Nation = info.Nation;
			Intro = info.Intro;
			Notice = info.Notice;
			Icon = info.Icon;
			Exp = info.Exp;
			JoinType = info.JoinType;
			JoinLV = info.JoinLV;
			ResIdx = info.ResIdx;
			ResExp = info.ResExp;
			UserCnt = info.UserCnt;
			MaxUserCnt = info.MaxUserCnt;
			TPower = info.TPower;
		}
		public Sprite GetGuilMark()
		{
			return MainMng.Instance.TDATA.GetGuideMark(Icon);
		}
	}

	public class RES_GUILDINFO : RES_GUILDINFO_SIMPLE
	{
		/// <summary> 길드원 </summary>
		public List<RES_GUILD_USER> Users = new List<RES_GUILD_USER>();
		/// <summary> 완료된 연구 정보</summary>
		public List<int> EndRes = new List<int>();
		/// <summary> 길드 아이템</summary>
		public List<RES_GUILD_ITEM> Items = new List<RES_GUILD_ITEM>();


		/// <summary> 길드 코인 </summary>
		public long m_GCoin;
		/// <summary> 길드 탈퇴한 시간 </summary>
		public long m_GRTime;
	}

	public class RES_GUILD_REQUSER
	{
		/// <summary> 고유번호 </summary>
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
		public long Power;
		/// <summary> 유저 마지막 플레이 시간 </summary>
		public long UTime;
		/// <summary> 요청 시작 </summary>
		public long CTime;
	}

	public class RES_GUILD_USER : RES_GUILD_REQUSER
	{
		/// <summary> 등급 </summary>
		public GuildGrade Grade;
		/// <summary> 포인트(연구등 소모) </summary>
		public long Point = 0;
		/// <summary> 일일 연구 횟수 </summary>
		public int ResCnt = 0;
		/// <summary> 출석시간 </summary>
		public long ATime;
		/// <summary> 연구시간 </summary>
		public long RTime;

		public void ResetResCnt()
		{
			if (!MainMng.Instance.UTILE.IsSameDay(RTime)) ResCnt = 0;
		}

		public int GetMaxResCnt()
		{
			var main = MainMng.Instance;
			var guild = main.USERINFO.m_Guild;
			if (guild.ResIdx == 0) return 0;
			var tdata = main.TDATA.GetGuildRes(guild.ResIdx);
			var limit = (int)(tdata.m_Mat.m_Count - guild.ResExp);
			ResetResCnt();
			return Mathf.Max(0, Mathf.Min(limit, main.TDATA.GetConfig_Int32(ConfigType.GuildResearchLimit) - ResCnt));
		}
	}


	public class RES_GUILD_BUFF
	{
		/// <summary> 등급 </summary>
		public GuildBuffType Type;
		/// <summary> type별 효과 </summary>
		public int Eff = 0;
		/// <summary> 값 </summary>
		public float Value;
	}
	public class RES_GUILD_ITEM
	{
		/// <summary> 인덱스 </summary>
		public int Idx = 0;
		/// <summary> 개수 </summary>
		public int Cnt;
	}
	#endregion

	#region REQ_GUILD
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD
	// 길드 상세 정보
	// RES 데이터의 UID == 0 이면 정보 없음
	// ★ 해당 프로토콜 호출시 길마가 장기 미접속(출첵)인 경우 길마 변경됨(가장 최근 출첵한 유저)
	public void SEND_REQ_GUILD(Action<RES_GUILDINFO> action, int InfoMode, long GUID = 0)
	{
		REQ_GUILD _data = new REQ_GUILD();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = GUID;
		_data.InfoMode = InfoMode;
		SendPost(Protocol.REQ_GUILD, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_GUILDINFO>(data);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_CREATE
	public class REQ_GUILD_CREATE : REQ_BASE
	{
		/// <summary> 이름 </summary>
		public string Name;
		/// <summary> 인삿말 </summary>
		public string Intro;
		/// <summary> 아이콘 </summary>
		public int Icon;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_CREATE
	// 길드 생성
	public void SEND_REQ_GUILD_CREATE(Action<RES_GUILDINFO> action, int icon, string Name, string Intro)
	{
		REQ_GUILD_CREATE _data = new REQ_GUILD_CREATE();
		_data.UserNo = USERINFO.m_UID;
		_data.Name = Name;
		_data.Intro = Intro;
		_data.Icon = icon;
		SendPost(Protocol.REQ_GUILD_CREATE, JsonConvert.SerializeObject(_data), (result, data) => {
			//ERROR_NICKNAME		길드명 없음 (길드명을 입력해주세요.)
			//ERROR_TOOLDATA		서버에 상점아이템 데이터 못찾음 (일반 에러코드 메세지)
			//ERROR_USED_NAME		사용중인 길드명 (이미 사용중인 길드명 입니다.)
			//ERROR_NOT_FOUND_USER	유저정보 없음 (잘못된 정보입니다. 또는 일반 에러코드 메세지)
			//ERROR_GUILD_JOIN		유저가 길드에 가입되어있음("다른길드에 가입이 되어있는 상태입니다" 메세지 출력후 길드 정보 받은다음 UI 길드 정보 페이지로 바꿔주어야됨)
			//ERROR_GUILD_JOIN_TIME	시간제한 걸림 (길드 생성은 탈퇴 후 24시간 이후 가능합니다.)
			var res = ParsResData<RES_GUILDINFO>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.Guild, 0, 0, 1);
				USERINFO.m_Guild.SetData(res);
				USERINFO.m_Guild.SaveLV();
				USERINFO.m_Guild.SaveMyGrade();
				USERINFO.m_Guild.Set_AlramOff();
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_DESTROY
	public class REQ_GUILD_DESTROY : REQ_BASE
	{
		public long UID;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_DESTROY
	// 길드 해산
	public void SEND_REQ_GUILD_DESTROY(Action<RES_BASE> action)
	{
		REQ_GUILD_DESTROY _data = new REQ_GUILD_DESTROY();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = USERINFO.m_Guild.UID;
		SendPost(Protocol.REQ_GUILD_DESTROY, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_BASE>(data);
			// ERROR_NOT_FOUND_USER 잘못된 유저 정보 (일반 에러코드 메세지)
			// ERROR_NOT_FOUND_GUILD 잘못된 길드 정보 (일반 에러코드 메세지)
			// ERROR_GUILD_GRADE 마스터 전용 기능 (마스터만 가능한 기능입니다.또는 권한이 없습니다.)
			if (res.IsSuccess())
			{
				USERINFO.m_Guild = new GuildInfo();
				USERINFO.m_Guild.Set_AlramOff();
				USERINFO.m_GuildKickCheck.Save();
				USERINFO.m_Guild.SaveMyGrade();
				USERINFO.m_GRTime = (long)UTILE.Get_ServerTime_Milli();
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_FIND
	public class REQ_GUILD_FIND : REQ_BASE
	{
		public string Name;
	}

	public class RES_GUILD_FIND : RES_BASE
	{
		/// <summary> 찾은 길드 정보 </summary>
		public List<RES_GUILDINFO_SIMPLE> Guilds = new List<RES_GUILDINFO_SIMPLE>();
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_FIND
	// 길드 찾기
	public void SEND_REQ_GUILD_FIND(Action<RES_GUILD_FIND> action, string Name)
	{
		REQ_GUILD_FIND _data = new REQ_GUILD_FIND();
		_data.UserNo = USERINFO.m_UID;
		_data.Name = Name;
		SendPost(Protocol.REQ_GUILD_FIND, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_GUILD_FIND>(data));
		});
	}
	#endregion

	#region REQ_GUILD_RECOMMEND
	public class REQ_GUILD_RECOMMEND : REQ_BASE
	{
		public List<long> Befor = new List<long>();
	}
	public class RES_GUILD_RECOMMEND : RES_BASE
	{
		// 추천 길드
		public List<RES_GUILDINFO_SIMPLE> Guilds = new List<RES_GUILDINFO_SIMPLE>();

		// 내 요청 길드 정보
		public RES_GUILDINFO_SIMPLE MyJoin;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_RECOMMEND
	// 추천 길드
	public void SEND_REQ_GUILD_RECOMMEND(Action<RES_GUILD_RECOMMEND> action, List<long> BeforGuild)
	{
		REQ_GUILD_RECOMMEND _data = new REQ_GUILD_RECOMMEND();
		_data.UserNo = USERINFO.m_UID;
		_data.Befor = BeforGuild;
		SendPost(Protocol.REQ_GUILD_RECOMMEND, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_GUILD_RECOMMEND>(data);
			if (res.MyJoin == null) res.MyJoin = new RES_GUILDINFO_SIMPLE();
			if (res.MyJoin.UID != 0 && !res.Guilds.Exists(o => o.UID == res.MyJoin.UID)) res.Guilds.Add(res.MyJoin);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_JOIN
	public class REQ_GUILD_JOIN : REQ_BASE
	{
		public long UID;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_JOIN
	// 길드 가입 신청
	// RES_GUILDINFO 의 UID = 0이면 요청으로 들어감(자동수락의경우도 길드 맴버수가 꽉찼다면 요청으로 들어감)"

	public void SEND_REQ_GUILD_JOIN(Action<RES_GUILDINFO> action, long GUID)
	{
		//ERROR_NOT_FOUND_USER		유저정보 오류 (일반 에러코드 메세지)
		//ERROR_NOT_FOUND_GUILD		길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_JOIN			길드에 이미 가입되어있음(갱신이 필요한 상태)
		//ERROR_GUILD_JOIN_TIME		시간제한 걸림 (길드 가입은 탈퇴 후 24시간 이후 가능합니다.)
		//ERROR_GUILD_MANY_REQ		가입 요청이 너무 많음
		//ERROR_GUILD_REQ_LV		길드 가입 레벨 제한

		REQ_GUILD_JOIN _data = new REQ_GUILD_JOIN();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = GUID;
		SendPost(Protocol.REQ_GUILD_JOIN, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_GUILDINFO>(data));
		});
	}
	#endregion

	#region REQ_CANCEL_GUILD_JOIN
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_CANCEL_GUILD_JOIN
	// 가입 요청 취소하기 (길드 생성때 사용)

	public void SEND_REQ_CANCEL_GUILD_JOIN(Action<RES_BASE> action)
	{
		//ERROR_NOT_FOUND_USER		유저정보 오류 (일반 에러코드 메세지)
		//ERROR_GUILD_JOIN			길드에 이미 가입되어있음(갱신이 필요한 상태)

		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		SendPost(Protocol.REQ_CANCEL_GUILD_JOIN, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_GUILDINFO>(data);
			if (res.IsSuccess())
			{
				if(res.UID != 0)
				{
					USERINFO.Check_Mission(MissionType.Guild, 0, 0, 1);
					USERINFO.m_Guild.SetData(res);
					USERINFO.m_Guild.SaveLV();
					USERINFO.m_Guild.SaveMyGrade();
					USERINFO.m_Guild.Set_AlramOff();
				}
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_REQUSER_LIST
	public class REQ_GUILD_REQUSER_LIST : REQ_BASE
	{
		public long UID;
	}
	public class RES_GUILD_REQUSER_LIST : RES_BASE
	{
		/// <summary> 신청 유저 </summary>
		public List<RES_GUILD_REQUSER> Users = new List<RES_GUILD_REQUSER>();
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_REQUSER_LIST
	// 길드 가입 신청자 리스트
	public void SEND_REQ_GUILD_REQUSER_LIST(Action<RES_GUILD_REQUSER_LIST> action, long GUID)
	{
		REQ_GUILD_REQUSER_LIST _data = new REQ_GUILD_REQUSER_LIST();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = GUID;
		SendPost(Protocol.REQ_GUILD_REQUSER_LIST, JsonConvert.SerializeObject(_data), (result, data) => {
			var res = ParsResData<RES_GUILD_REQUSER_LIST>(data);
			if(res.IsSuccess()) USERINFO.m_Guild.SetData(res.Users);
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_REQUSER_APPLY
	public class REQ_GUILD_REQUSER_APPLY : REQ_BASE
	{
		public long UID;
		public long Target;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_REQUSER_LIST
	// 길드 가입 수락 (마스터 권한)
	public void SEND_REQ_GUILD_REQUSER_APPLY(Action<RES_GUILDINFO> action, long userno)
	{
		//ERROR_NOT_FOUND_USER		유저 정보 오류 (일반 에러코드 메세지)
		//ERROR_GUILD_JOIN			대상이 길드가 있음 (다른 길드에 가입된 상태 입니다.)
		//ERROR_NOT_FOUND_GUILD		길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_GRADE			권한 없음 (마스터만 가능한 기능입니다. 또는 권한이 없습니다.)
		//ERROR_GUILD_MAX_MEMBER	길드 인원 최대 (수용 가능한 길드원이 최대입니다.)

		REQ_GUILD_REQUSER_APPLY _data = new REQ_GUILD_REQUSER_APPLY();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = USERINFO.m_Guild.UID;
		_data.Target = userno;
		SendPost(Protocol.REQ_GUILD_REQUSER_APPLY, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_GUILDINFO>(data);
			if (res.IsSuccess())
			{
				// 자신의 길드정보 갱신해주기
				USERINFO.m_Guild.SetData(res);
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_REQUSER_REJECT
	public class REQ_GUILD_REQUSER_REJECT : REQ_BASE
	{
		public long UID;
		public List<long> Targets;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_REQUSER_REJECT
	// 길드 가입 거절 (마스터 권한)
	public void SEND_REQ_GUILD_REQUSER_REJECT(Action<RES_BASE> action, List<long> usernos)
	{
		//ERROR_NOT_FOUND_USER		유저 정보 오류 (일반 에러코드 메세지)
		//ERROR_NOT_FOUND_GUILD		길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_GRADE			권한 없음 (마스터만 가능한 기능입니다. 또는 권한이 없습니다.)

		REQ_GUILD_REQUSER_REJECT _data = new REQ_GUILD_REQUSER_REJECT();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = USERINFO.m_Guild.UID;
		_data.Targets = usernos;
		SendPost(Protocol.REQ_GUILD_REQUSER_REJECT, JsonConvert.SerializeObject(_data), (result, data) => {
			// usernos로 읽은 리스트들 제거할것
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	#endregion

	#region REQ_GUILD_REMOVE_USER
	public class REQ_GUILD_REMOVE_USER : REQ_BASE
	{
		public long UID;
		public long Target;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_REQUSER_LIST
	// 길드 탈퇴(추방)
	public void SEND_REQ_GUILD_REMOVE_USER(Action<RES_BASE> action, long target)
	{
		// target == 0 자기자신
		//ERROR_NOT_FOUND_USER	유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_GRADE		자기자신이 마스터계정(마스터는 길드 탈퇴가 불가능 합니다.)
		REQ_GUILD_REMOVE_USER _data = new REQ_GUILD_REMOVE_USER();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = USERINFO.m_Guild.UID;
		_data.Target = target;
		SendPost(Protocol.REQ_GUILD_REMOVE_USER, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				// 자신의 길드정보 갱신해주기
				if (target == USERINFO.m_UID)
				{
					USERINFO.m_Guild = new GuildInfo();
					USERINFO.m_Guild.Set_AlramOff();
					USERINFO.m_GuildKickCheck.Save();
					USERINFO.m_Guild.SaveMyGrade();
					USERINFO.m_GRTime = (long)UTILE.Get_ServerTime_Milli();
				}
				else
				{
					USERINFO.m_Guild.Users.RemoveAll(o => o.UserNo == target);
					USERINFO.m_Guild._UserCnt = USERINFO.m_Guild.Users.Count;
				}
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_MEMBER_GRADE_CHANGE
	public class REQ_GUILD_MEMBER_GRADE_CHANGE : REQ_BASE
	{
		public long GUID;
		public long Target;
		public GuildGrade Grade;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_MEMBER_GRADE_CHANGE
	// 길드 맴버 등급 수정
	public void SEND_REQ_GUILD_MEMBER_GRADE_CHANGE(Action<RES_BASE> action, long Member, GuildGrade grade)
	{
		//ERROR_NOT_FOUND_USER	유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_GRADE		마스터 권한 필요 (마스터만 가능한 기능입니다.또는 권한이 없습니다.)
		REQ_GUILD_MEMBER_GRADE_CHANGE _data = new REQ_GUILD_MEMBER_GRADE_CHANGE();
		_data.UserNo = USERINFO.m_UID;
		_data.GUID = USERINFO.m_Guild.UID;
		_data.Target = Member;
		_data.Grade = grade;
		SendPost(Protocol.REQ_GUILD_MEMBER_GRADE_CHANGE, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_Guild.GradeChange(Member, grade);
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_APPLY_MASTAR
	public class REQ_GUILD_APPLY_MASTAR : REQ_BASE
	{
		public long GUID;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_MEMBER_GRADE_CHANGE
	// 길드 맴버 등급 수정
	public void SEND_REQ_GUILD_APPLY_MASTAR(Action<RES_BASE> action)
	{
		//ERROR_NOT_FOUND_USER	유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_GRADE		마스터 권한 필요 (마스터만 가능한 기능입니다.또는 권한이 없습니다.)
		REQ_GUILD_APPLY_MASTAR _data = new REQ_GUILD_APPLY_MASTAR();
		_data.UserNo = USERINFO.m_UID;
		_data.GUID = USERINFO.m_Guild.UID;
		SendPost(Protocol.REQ_GUILD_APPLY_MASTAR, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_Guild.GradeChange(USERINFO.m_Guild.MyInfo, GuildGrade.Master);
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_INFO_CHANGE
	public enum GUILD_INFO_CHANGE_MODE
	{
		/// <summary> 길드명 </summary>
		Name = 0,
		/// <summary> 길드 마크 </summary>
		Icon,
		/// <summary> 인삿말 </summary>
		Intro,
		/// <summary> 공지 </summary>
		Notice,
		/// <summary> 가입 레벨 </summary>
		JoinLV,
		/// <summary> 가입 방식 </summary>
		JoinType,
	}
	public class REQ_GUILD_INFO_CHANGE : REQ_BASE
	{
		public long UID;
		/// <summary> 변경할 데이터 </summary>
		public GUILD_INFO_CHANGE_MODE Mode;
		/// <summary> 값 </summary>
		public string Value;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_INFO_CHANGE
	// 길드 정보 수정
	public void SEND_REQ_GUILD_INFO_CHANGE(Action<RES_BASE> action, GUILD_INFO_CHANGE_MODE mode, string value)
	{
		//ERROR_NOT_FOUND_USER		유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_GRADE			마스터 권한 필요 (마스터만 가능한 기능입니다.또는 권한이 없습니다.)
		//ERROR_GUILD_INTRO_LENGTH	인삿말 글자수 초과(30글자) (일반 에러코드 메세지)
		REQ_GUILD_INFO_CHANGE _data = new REQ_GUILD_INFO_CHANGE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = USERINFO.m_Guild.UID;
		_data.Mode = mode;
		_data.Value = value;
		SendPost(Protocol.REQ_GUILD_INFO_CHANGE, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				switch (mode)
				{
				/// <summary> 길드명 </summary>
				case GUILD_INFO_CHANGE_MODE.Name: USERINFO.m_Guild.Name = value; break;
				/// <summary> 길드 마크 </summary>
				case GUILD_INFO_CHANGE_MODE.Icon: USERINFO.m_Guild.Icon = int.Parse(value); break;
				/// <summary> 인삿말 </summary>
				case GUILD_INFO_CHANGE_MODE.Intro: USERINFO.m_Guild.Intro = value; break;
				/// <summary> 공지 </summary>
				case GUILD_INFO_CHANGE_MODE.Notice: USERINFO.m_Guild.Notice = value; break;
				/// <summary> 가입 레벨 </summary>
				case GUILD_INFO_CHANGE_MODE.JoinLV: USERINFO.m_Guild.JoinLV = int.Parse(value); break;
				/// <summary> 가입 방식 </summary>
				case GUILD_INFO_CHANGE_MODE.JoinType: USERINFO.m_Guild.JoinType = (GuildJoinType)int.Parse(value); break;
				}
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_ATTENDANCE
	public class REQ_GUILD_ATTENDANCE : REQ_BASE
	{
		public long UID;
	}
	public class RES_GUILD_ATTENDANCE : RES_BASE
	{
		public RES_GUILD_USER User;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_ATTENDANCE
	// 출첵
	public void SEND_REQ_GUILD_ATTENDANCE(Action<RES_GUILD_ATTENDANCE> action)
	{
		//ERROR_NOT_FOUND_USER		유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_NOT_FOUND_GUILD		길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_TIME				시간제한 걸림 (이미 출석 체크를 하셨습니다.)
		REQ_GUILD_ATTENDANCE _data = new REQ_GUILD_ATTENDANCE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = USERINFO.m_Guild.UID;
		SendPost(Protocol.REQ_GUILD_ATTENDANCE, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_GUILD_ATTENDANCE>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.GuildCheck, 0, 0, 1);

				if (USERINFO.m_Guild != null && USERINFO.m_Guild.MyInfo != null) 
					USERINFO.m_Guild.MyInfo.ATime = (long)UTILE.Get_ServerTime_Milli();
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_RES_START
	public class REQ_GUILD_RES_START : REQ_BASE
	{
		public int Idx;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_RES_START
	// 출첵
	public void SEND_REQ_GUILD_RES_START(Action<RES_BASE> action, int Idx)
	{
		//ERROR_NOT_FOUND_USER				유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_NOT_FOUND_GUILD				길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_END_RESEARCH			이미 종료된 연구
		//ERROR_GUILD_CHECK_RESEARCH_UNLOCK	언락 조건 안됨
		//ERROR_GUILD_NOT_MEMBER			길드 맴버 아님
		//ERROR_GUILD_GRADE					마스터 권한 필요
		REQ_GUILD_RES_START _data = new REQ_GUILD_RES_START();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = Idx;
		SendPost(Protocol.REQ_GUILD_RES_START, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess()) 
			{
				USERINFO.m_Guild.ResIdx = Idx;
				USERINFO.m_Guild.ResExp = 0;
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_RES_STOP
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_RES_STOP
	// 출첵
	public void SEND_REQ_GUILD_RES_STOP(Action<RES_BASE> action, int Idx)
	{
		//ERROR_NOT_FOUND_USER		유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_NOT_FOUND_GUILD		길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_NOT_MEMBER	길드 맴버 아님
		//ERROR_GUILD_GRADE			마스터 권한 필요
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		SendPost(Protocol.REQ_GUILD_RES_STOP, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_Guild.ResIdx = 0;
				USERINFO.m_Guild.ResExp = 0;
			}
			action?.Invoke(res);
		});
	}
	#endregion

	#region REQ_GUILD_RES_GIVE
	public class REQ_GUILD_RES_GIVE : REQ_BASE
	{
		public int Idx;
		public int Cnt;
	}
	public class RES_GUILD_RES_GIVE : RES_BASE
	{
		/// <summary> 타입 0이면 완료 </summary>
		public int Idx;
		/// <summary> 경험치 </summary>
		public long Exp;
		/// <summary> 연구시간 </summary>
		public long RTime;
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_GUILD_RES_START
	// 출첵
	public void SEND_REQ_GUILD_RES_GIVE(Action<RES_GUILD_RES_GIVE> action, int Idx, int Cnt)
	{
		//ERROR_NOT_FOUND_USER				유저 정보 없음 (일반 에러코드 메세지)
		//ERROR_NOT_FOUND_GUILD				길드 정보 없음 (일반 에러코드 메세지)
		//ERROR_GUILD_DIF_RESEARCH			진행 연구정보가 다름
		//ERROR_GUILD_END_RESEARCH			이미 종료된 연구
		//ERROR_GUILD_NOT_MEMBER			길드 맴버가 아님
		REQ_GUILD_RES_GIVE _data = new REQ_GUILD_RES_GIVE();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = Idx;
		_data.Cnt = Cnt;
		SendPost(Protocol.REQ_GUILD_RES_GIVE, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_GUILD_RES_GIVE>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.GetGuildResearch, 0, 0, 1);
				if (res.Idx == 0)
				{
					var tdata = TDATA.GetGuildRes(Idx);
					if (tdata.m_Eff.m_Eff == ResearchEff.MemberMaxUp) USERINFO.m_Guild.MaxUserCnt += (int)tdata.m_Eff.m_Value;
					// 완료됨
					USERINFO.m_Guild.EndRes.Add(Idx);
				}
				USERINFO.m_Guild.ResIdx = res.Idx;
				USERINFO.m_Guild.ResExp = res.Exp;
				USERINFO.m_Guild.MyInfo.RTime = res.RTime;
				USERINFO.m_Guild.MyInfo.ResCnt += Cnt;
			}
			action?.Invoke(res);
		});
	}
	#endregion
}
