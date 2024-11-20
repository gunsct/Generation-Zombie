using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LS_Web;

public class TPVPLeagueRewardTable : ClassMng
{
	public class Reward
	{
		public RewardKind Type;
		public int Idx;
		public int Cnt;
	}
	/// <summary> 고유 랭크인덱스 </summary>
	public int m_Idx;
	/// <summary> 보상지급 최소 순위 </summary>
	public int m_MinRanking;
	/// <summary> 보상지급 최대 순위 </summary>
	public int m_MaxRanking;
	/// <summary> 순위 보상 상점포인트 </summary>
	public int m_RankRewardPVPPoint;
	/// <summary> 순위 보상 금니 </summary>
	public int m_RankRewardTeeth;
	/// <summary> 순위 보상 타입, 인덱스, 카운트 </summary>
	public List<Reward> m_Rewards = new List<Reward>();
	/// <summary>  </summary>
	public List<RES_REWARD_BASE> GetRewards() {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		if (m_RankRewardPVPPoint > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.PVPCoin;
			rmoney.Befor = USERINFO.m_PVPCoin - (long)m_RankRewardPVPPoint;
			rmoney.Now = USERINFO.m_PVPCoin;
			rmoney.Add = m_RankRewardPVPPoint;
			rewards.Add(rmoney);
		}
		if (m_RankRewardTeeth > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Cash;
			rmoney.Befor = USERINFO.m_Cash - m_RankRewardTeeth;
			rmoney.Now = USERINFO.m_Cash;
			rmoney.Add = m_RankRewardTeeth;
			rewards.Add(rmoney);
		}
		for (int i = 0; i < m_Rewards.Count; i++) {
			rewards.AddRange(MAIN.GetRewardData(m_Rewards[i].Type, m_Rewards[i].Idx, m_Rewards[i].Cnt, true));
		}
		return rewards;
	}
	public TPVPLeagueRewardTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_MinRanking = pResult.Get_Int32();
		m_MaxRanking = pResult.Get_Int32();
		m_RankRewardPVPPoint = pResult.Get_Int32();
		m_RankRewardTeeth = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			RewardKind type = pResult.Get_Enum<RewardKind>();
			if(type == RewardKind.None) {
				pResult.NextReadPos();
				pResult.NextReadPos();
			}
			else {
				m_Rewards.Add(new Reward() {
					Type = type,
					Idx = pResult.Get_Int32(),
					Cnt = pResult.Get_Int32(),
				});
			}
		}
	}
}

public class TPVPLeagueRewardTableMng : ToolFile
{
	public List<TPVPLeagueRewardTable> DIC_Type = new List<TPVPLeagueRewardTable>();

	public TPVPLeagueRewardTableMng() : base("Datas/PVPLeagueRewardTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVPLeagueRewardTable data = new TPVPLeagueRewardTable(pResult);
		DIC_Type.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVPLeagueRewardTableMng m_PVPLeagueReward = new TPVPLeagueRewardTableMng();

	public List<TPVPLeagueRewardTable> GetAllTPVPLeagueRewardTable() {
		return m_PVPLeagueReward.DIC_Type;
	}
}

