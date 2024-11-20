using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
public class TPvPRankTable : ClassMng
{
	/// <summary> 고유 랭크인덱스 </summary>
	public int m_Idx;
	/// <summary> 랭크 번호(민간인: 1 -> 최고 지도자: 6 </summary>
	public int m_Rank;
	/// <summary> 티어번호 </summary>
	public int m_Tier;
	/// <summary> 랭크명 표시용 스트링ID </summary>
	public int m_RankName;
	/// <summary> 티어명 표시용 스트링ID </summary>
	public int m_TierName;
	/// <summary> 리그 그룹 유저수 </summary>
	public int m_EntryCnt;
	/// <summary> 시작 리그포인트 </summary>
	public int m_LeagueEntryPoint;
	/// <summary> 리그 시작 시, 초기화될 티어인덱스: PVPRankTable → Index </summary>
	public int m_LeagueInitTier;
	/// <summary> 시작 시즌 포인트 </summary>
	public int m_SeasonEntryPoint;
	/// <summary> 패배 시, 포인트 차감 여부, TRUE: 패배 시, 포인트 차감없음, FALES: 패배 시, 포인트 차감 </summary>
	public bool m_NoPointDrop;
	/// <summary> 승급 랭크인덱스 </summary>
	public int m_UpTierIdx;
	/// <summary> 승급 조건, NONE: 승급없음, POINT: 리그포인트, LEAGUERANK: 리그 순위</summary>
	public UpTierType m_UpTierType;
	/// <summary> 승급 조건값 </summary>
	public int m_UpTierVal;
	/// <summary> 강등 랭크인덱스 </summary>
	public int m_DownTierIdx;
	/// <summary> 강등 조건, NONE: 강등없음, LEAGUERANK: 리그 순위 </summary>
	public DownTierType m_DownTierType;
	/// <summary> 강등 조건값 </summary>
	public int m_DownTierVal;
	/// <summary> 승리 획득 포인트, 5연승까지 </summary>
	public int[] m_WinRankPoint = new int[5];
	/// <summary> 패배 차감 포인트 </summary>
	public int m_LoseRankPoint;
	/// <summary> 리그/랭크포인트 티어별 격차 보정 </summary>
	public int m_PointGapCorr;
	/// <summary> 상대 생존자 정보 비공개수 </summary>
	public int m_HideMemberCnt;
	/// <summary> 상대 재탐색 시 비용 (달러) </summary>
	public int m_SearchSIdx;
	/// <summary> 티어순서(최하위 티어: 1) </summary>
	public int m_SortingOrder;
	/// <summary> 랭크 아이콘 </summary>
	public string m_RankIcon;
	/// <summary> 티어 아이콘 </summary>
	public string m_TierIcon;
	/// <summary> 랭킹 등락 설명 </summary>
	public int m_RankingDesc;

	public TPvPRankTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Rank = pResult.Get_Int32();
		m_Tier = pResult.Get_Int32();
		m_RankName = pResult.Get_Int32();
		m_TierName = pResult.Get_Int32();
		m_EntryCnt = pResult.Get_Int32();
		m_LeagueEntryPoint = pResult.Get_Int32();
		m_LeagueInitTier = pResult.Get_Int32();
		m_SeasonEntryPoint = pResult.Get_Int32();
		m_NoPointDrop = pResult.Get_Boolean();
		m_UpTierIdx = pResult.Get_Int32();
		m_UpTierType = pResult.Get_Enum<UpTierType>();
		m_UpTierVal = pResult.Get_Int32();
		m_DownTierVal = pResult.Get_Int32();
		m_DownTierType = pResult.Get_Enum<DownTierType>();
		m_DownTierVal = pResult.Get_Int32();
		for(int i = 0;i<5;i++) m_WinRankPoint[i] = pResult.Get_Int32();
		m_LoseRankPoint = pResult.Get_Int32();
		m_PointGapCorr = pResult.Get_Int32();
		m_HideMemberCnt = pResult.Get_Int32();
		m_SearchSIdx = pResult.Get_Int32();
		m_SortingOrder = pResult.Get_Int32();
		m_RankIcon = pResult.Get_String();
		m_TierIcon = pResult.Get_String(); 
		m_RankingDesc = pResult.Get_Int32();
	}

	public string GetRankName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_RankName);
	}
	public string GetTierName() {
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_TierName);
	}
	public Sprite GetRankIcon() {
		return UTILE.LoadImg(m_RankIcon, "png");
	}
	public Sprite GetTierIcon() {
		return UTILE.LoadImg(m_TierIcon, "png");
	}
	public string GetUpDownDesc() {
		return TDATA.GetString(m_RankingDesc);
	}
}

public class TPvPRankTableMng : ToolFile
{
	public List<TPvPRankTable> DIC_Type = new List<TPvPRankTable>();

	public TPvPRankTableMng() : base("Datas/PVPRankTable")
	{
	}

	public override void DataInit()
	{
		DIC_Type.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TPvPRankTable data = new TPvPRankTable(pResult);
		DIC_Type.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// PvPRank
	TPvPRankTableMng m_PvPRank = new TPvPRankTableMng();

	public TPvPRankTable GeTPvPRankTable(int _rank, int _tier) {
		return m_PvPRank.DIC_Type.Find(o=>o.m_Rank == _rank && o.m_Tier == _tier);
	}
	public TPvPRankTable GeTPvPRankTable(int _idx) {
		return m_PvPRank.DIC_Type.Find(o => o.m_Idx == _idx);
	}
	public List<TPvPRankTable> GetAllPVPRankTable() {
		return m_PvPRank.DIC_Type;
	}
}

