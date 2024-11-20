using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public class ChallengeReward
{
	/// <summary> UID </summary>
	public long UID { get; set; }

	/// <summary> 타입 </summary>
	public ChallengeMode Mode { get; set; }

	/// <summary> 순위(min, max) </summary>
	public int[] rank { get; set; } = new int[2];

	/// <summary> 보상내용 </summary>
	public PostReward item { get; set; } = new PostReward();
}

public class MyChallenge : ClassMng
{
	public ChallengeInfo Befor;
	public ChallengeInfo Now;
	public List<ChallengeInfo> Week = new List<ChallengeInfo>();
	public List<ChallengeInfo> WeekEnd = new List<ChallengeInfo>();

	// 다음 챌린지 정보
	public ChallengeType Next;
	public long NextSTime;


	// 갱신 시간 체크용
	public DateTime UTime;

	/// <summary>
	/// 챌린지 데이터 로드 상태
	/// <para>0 : 수신완료</para>
	/// <para>1 : 수신중</para>
	/// </summary>
	public int DataLoadState = -1;
	/// <summary> 데이터를 로드한 시간 </summary>
	public long LoadTime = 0;

	public ChallengeInfo GetChallenge(ChallengeMode mode, ChallengeType type)
	{
		switch(mode)
		{
		case ChallengeMode.Week: return Week.Find(o => o.Type == type);
		}
		return Now;
	}
}

public class ChallengeInfo : ClassMng
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

	public void SetData(RES_CHALLENGEINFO data)
	{
		if(data == null)
		{
			Group = 0;
			return;
		}
		No = data.No;
		Group = data.Group;
		Type = data.Type;
		Times = data.Times;
		RankUsers = data.RankUsers;
		MyInfo = data.MyInfo;
		CRewards = data.CRewards;
	}
	public string GetName()
	{
		return TDATA.GetChallengeName(Type);
	}

	public string GetInfo()
	{
		return TDATA.GetChallengeInfo(Type);
	}

	/// <summary> 남은 시간 </summary>
	public double GetRemainTime()
	{
		return Math.Max(0, Times[1] - UTILE.Get_ServerTime_Milli()) * 0.001d;
	}

	public Sprite GetImg()
	{
		return UTILE.LoadImg(string.Format("BG/Challenge/Challenge_{0}", Type.ToString()), "png");
	}

	public ChallengeReward GerRankReward(int Rank)
	{
		if (CRewards == null || Rank < 1) return null;
		return CRewards.Find(o => o.rank[0] <= Rank && o.rank[1] >= Rank || o.rank[0] <= Rank && o.rank[1] == 0);
	}
}


