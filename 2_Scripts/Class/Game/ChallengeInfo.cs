using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public class ChallengeReward
{
	/// <summary> UID </summary>
	public long UID { get; set; }

	/// <summary> Ÿ�� </summary>
	public ChallengeMode Mode { get; set; }

	/// <summary> ����(min, max) </summary>
	public int[] rank { get; set; } = new int[2];

	/// <summary> ���󳻿� </summary>
	public PostReward item { get; set; } = new PostReward();
}

public class MyChallenge : ClassMng
{
	public ChallengeInfo Befor;
	public ChallengeInfo Now;
	public List<ChallengeInfo> Week = new List<ChallengeInfo>();
	public List<ChallengeInfo> WeekEnd = new List<ChallengeInfo>();

	// ���� ç���� ����
	public ChallengeType Next;
	public long NextSTime;


	// ���� �ð� üũ��
	public DateTime UTime;

	/// <summary>
	/// ç���� ������ �ε� ����
	/// <para>0 : ���ſϷ�</para>
	/// <para>1 : ������</para>
	/// </summary>
	public int DataLoadState = -1;
	/// <summary> �����͸� �ε��� �ð� </summary>
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
	/// <summary> �׷��ȣ </summary>
	public int No;
	/// <summary> �׷��ȣ </summary>
	public int Group;
	/// <summary> Ÿ�� </summary>
	public ChallengeType Type;
	/// <summary> ç���� �ð� (0 : ����, 1 : ��)</summary>
	public long[] Times = new long[2];

	/// <summary> 100�������� ���� ���� </summary>
	public List<RES_RANKING_INFO> RankUsers;
	/// <summary> �ڽ��� ���� </summary>
	public RES_RANKING_INFO MyInfo;

	/// <summary> ������ ���� </summary>
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

	/// <summary> ���� �ð� </summary>
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


