using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static LS_Web;

public class TPVPRankRewardTable : ClassMng
{
	public class KillCnt
	{
		public int Need;
		public int Point;
	}
	/// <summary> 고유 랭크인덱스 </summary>
	public int m_Idx;
	/// <summary> 승리 획득 상점포인트 </summary>
	public int m_WinPoint;
	/// <summary> 단계별 일일 보상 조건 및 포인트 </summary>
	public KillCnt[] m_KillPoint = new KillCnt[3];
	/// <summary> 티어 최초 달성 보상 금니량 </summary>
	public int m_FirstTierReward;

	public TPVPRankRewardTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_WinPoint = pResult.Get_Int32();
		for(int i = 0; i < 3; i++) {
			m_KillPoint[i] = new KillCnt();
			m_KillPoint[i].Need = pResult.Get_Int32();
			m_KillPoint[i].Point = pResult.Get_Int32();
		}
		m_FirstTierReward = pResult.Get_Int32();
	}
	public List<RES_REWARD_BASE> GetReward() {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		if (m_FirstTierReward > 0) {
			RES_REWARD_MONEY rmoney;
			rmoney = new RES_REWARD_MONEY();
			rmoney.Type = Res_RewardType.Cash;
			rmoney.Befor = USERINFO.m_Cash - m_FirstTierReward;
			rmoney.Now = USERINFO.m_Cash;
			rmoney.Add = m_FirstTierReward;
			rewards.Add(rmoney);
		}
		return rewards;
	}
}

public class TPVPRankRewardTableMng : ToolFile
{
	public List<TPVPRankRewardTable> DIC_Type = new List<TPVPRankRewardTable>();

	public TPVPRankRewardTableMng() : base("Datas/PVPRankRewardTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPVPRankRewardTable data = new TPVPRankRewardTable(pResult);
		DIC_Type.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPVPRankRewardTableMng m_PVPRankReward = new TPVPRankRewardTableMng();

	public TPVPRankRewardTable GeTPVPRankRewardTable(int _rankidx) {
		return m_PVPRankReward.DIC_Type.Find(o => o.m_Idx == _rankidx);
	}
	public List<TPVPRankRewardTable> GetAllPVPRankRewardTable() {
		return m_PVPRankReward.DIC_Type;
	}
}

