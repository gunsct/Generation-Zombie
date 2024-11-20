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
	public enum ChallengeMode
	{
		/// <summary> 신규유저 </summary>
		New = 0,
		/// <summary> 일반 </summary>
		Normal,
		/// <summary> 주간 </summary>
		Week,
		END
	}

	public enum ChallengeType
	{
		/// <summary> 스테이지 클리어 (일반) : 챌린지 기간 내 클리어한 스테이지 수.</summary>
		StageClear = 0,
		/// <summary> 연구 : 챌린지 기간 내 연구 완료 횟수. </summary>
		Research,
		/// <summary> 생산 : 챌린지 기간 내 생산 완료 횟수. </summary>
		Making,
		/// <summary> 장비 생산 : 챌린지 기간 내 장비 생산 횟수. </summary>
		MakingEquip,
		/// <summary> 타워 : 챌린지 기간이 시작된 후 진행한 층수. </summary>
		Tower,
		/// <summary> 캐릭터 가차 : 챌린지 기간이 시작된 후 캐릭터 가차 진행 횟수. </summary> 
		GachaChar,
		/// <summary> 장비 가챠 : 챌린지 기간 내 장비&DNA가챠 진행 횟수. </summary>
		GachaEquip,
		/// <summary> 캐릭터 레벨업 : 챌린지 기간이 시작된 후 캐릭터 레벨업 수. </summary>
		LevelChar,
		/// <summary> 사용 EXP : 챌린지 기간 내 사용한 캐릭터EXP 총량. </summary>
		UseExp,
		/// <summary> 장비 강화 : 챌린지 기간이 시작된 후 증가 된  장비의 총 전투력. </summary>
		LevelEquip,
		/// <summary> 캐릭터 승급 : 챌린지 기간이 시작된 후 캐릭터 승급횟수. </summary>
		GradeChar,
		/// <summary> 좀비 성장 : 챌린지 기간이 시작된 후 좀비 성장 횟수 (해부해도 카운트 감소 X) </summary>
		LevelZombie,
		/// <summary> DNA 성장 : 챌린지 기간이 시작된 후 DNA 승급 성공 횟수 </summary>
		LevelDNA,
		/// <summary> 금니사용 : 챌린지 기간동안 금니를 사용한 수. </summary>
		UseGoldTeeth,
		/// <summary> 총알사용 : 클리어한 스테이지기준 총알사용(총알무제한때메 이 조건으로 해야할듯). </summary>
		UseBullet,
		/// <summary> 다운타운 클리어 : 다운타운 모든 컨텐츠  클리어 횟수 </summary>
		DownTownClear,
		/// <summary> PVP 승리 </summary>
		PVPWin,
		END
	}

	public class RES_RANKING_INFO
	{
		/// <summary> 순위 </summary>
		public int Rank;
		/// <summary> 유저 번호 </summary>
		public long UserNo;
		/// <summary> 점수 </summary>
		public long Point;
		/// <summary> 이름 </summary>
		public string Name;
		[JsonIgnore] public string m_Name { get { return BaseValue.GetUserName(Name); } }
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 프로필 이미지 </summary>
		public int Profile;
		/// <summary> 국가 코드 </summary>
		public string Nation;
		/// <summary> PVP 랭크 인덱스 </summary>
		public int PVPRank;
		/// <summary> PVP 덱 파워 </summary>
		public int Power;
		/// <summary> 푸시 토큰 종료시 토큰필요 </summary>
		public string pushtoken;
		/// <summary> 푸시 토큰 종료시 토큰필요 </summary>
		public string lang;
		/// <summary> 승급 PVP 랭크 인덱스(리그 보상 받을때만 내려줌) </summary>
		public int Next_PVPRank;
	}

	public class RES_CHALLENGEINFO : RES_BASE
	{
		/// <summary> 그룹번호 </summary>
		public int No;
		/// <summary> 그룹번호 </summary>
		public int Group;
		/// <summary> 타입 </summary>
		public ChallengeType Type;
		/// <summary> 챌린지 시간 (0 : 시작, 1 : 끝)</summary>
		public long[] Times = new long[2];

		/// <summary> 100위까지의 유저 정보 </summary>
		public List<RES_RANKING_INFO> RankUsers;
		/// <summary> 자신의 정보 </summary>
		public RES_RANKING_INFO MyInfo;

		/// <summary> 지급한 보상 </summary>
		public List<ChallengeReward> CRewards;
	}

	public class RES_CHALLENGEINFO_ALL : RES_BASE
	{
		public RES_CHALLENGEINFO Befor;
		public RES_CHALLENGEINFO Now;
		public List<RES_CHALLENGEINFO> Week;
		public List<RES_CHALLENGEINFO> WeekEnd;

		// 다음 챌린지 정보
		public ChallengeType Next;
		public long NextSTime;
	}

	public void SEND_REQ_CHALLENGEINFO_ALL(Action<RES_CHALLENGEINFO_ALL> action = null)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		USERINFO.m_MyChallenge.DataLoadState = 1;

		SendPost(Protocol.REQ_CHALLENGEINFO_All, JsonConvert.SerializeObject(_data), (result, data) => {

			var res = ParsResData<RES_CHALLENGEINFO_ALL>(data);
			if(res.IsSuccess())
			{
				if (res.Befor != null)
				{
					USERINFO.m_MyChallenge.Befor = new ChallengeInfo();
					USERINFO.m_MyChallenge.Befor.SetData(res.Befor);
				}
				else USERINFO.m_MyChallenge.Befor = null;

				if (res.Now != null)
				{
					USERINFO.m_MyChallenge.Now = new ChallengeInfo();
					USERINFO.m_MyChallenge.Now.SetData(res.Now);
				}
				else USERINFO.m_MyChallenge.Now = null;

				USERINFO.m_MyChallenge.Week.Clear();
				if (res.Week?.Count > 0) USERINFO.m_MyChallenge.Week.AddRange(res.Week.Select(o => { var info = new ChallengeInfo(); info.SetData(o); return info; }).ToList());

				USERINFO.m_MyChallenge.WeekEnd.Clear();
				if (res.Week?.Count > 0) USERINFO.m_MyChallenge.WeekEnd.AddRange(res.WeekEnd.Select(o => { var info = new ChallengeInfo(); info.SetData(o); return info; }).ToList());

				USERINFO.m_MyChallenge.Next = res.Next;
				USERINFO.m_MyChallenge.NextSTime = res.NextSTime;
				USERINFO.m_MyChallenge.UTime = DateTime.Now;
				USERINFO.m_MyChallenge.DataLoadState = 0;
				USERINFO.m_MyChallenge.LoadTime = (long)UTILE.Get_ServerTime_Milli();
			}
			action?.Invoke(res);
		});
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_CHALLENGE_RANKING

	public class RES_CHALLENGE_MYRANKING : RES_BASE
	{
		public ChallengeMode Mode;
		public ChallengeType Type;
		public int No;

		public int BeforRank;
		public RES_RANKING_INFO MyInfo;

		public int RankGap;

		public string GetName() {
			return MainMng.Instance.m_ToolData.GetChallengeName(Type);
		}

		public Sprite GetImg() {
			return MainMng.Instance.m_Utile.LoadImg(string.Format("BG/Challenge/Challenge_{0}", Type.ToString()), "png");
		}
	}
}
