using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class AuctionItem : ItemInfo
{
	/// <summary> 경매 툴데이터 인덱스 </summary>
	public int TIdx;
	/// <summary> 경매 그룹 </summary>
	public int Group;
	/// <summary> 종류 </summary>
	public RewardKind kind;
	/// <summary> 구매 유저 번호 </summary>
	public long UserNo;
	/// <summary> 유저 닉네임 </summary>
	public string Name;
	[JsonIgnore] public string m_Name { get { return BaseValue.GetUserName(Name); } }
	/// <summary> 유저 국가 </summary>
	public string Nation;
	/// <summary> 유저 프로필 이미지 </summary>
	public int Profile;
	/// <summary> 가격 캐시(0:무료, 1:유료) </summary>
	int[] Price = new int[2];
	/// <summary> 0:시작시간, 1:종료시간 </summary>
	public long[] Times = new long[2];

	/// <summary> 변경 연출을 보여주었는지 확인 </summary>
	public bool IsChangeAction;

	public void SetDATA(LS_Web.RES_AUCTION_ITEM data)
	{
		m_Uid = data.UID;
		base.SetDATA(data);
		TIdx = data.TIdx;
		Group = data.Group;
		kind = data.kind;
		UserNo = data.UserNo;
		Nation = data.Nation;
		Name = data.Name;
		Profile = data.Profile;
		Price[0] = data.Price[0];
		Price[1] = data.Price[1];
		Times[0] = data.Times[0];
		Times[1] = data.Times[1];
		IsChangeAction = false;
	}

	public RES_REWARD_BASE GetReward()
	{
		switch(kind)
		{
		case RewardKind.Character: return new RES_REWARD_CHAR() { Type = Res_RewardType.Char, Idx = m_Idx, LV = m_Lv, Grade = m_Grade };
		case RewardKind.Zombie: return new RES_REWARD_ZOMBIE() { Type = Res_RewardType.Zombie, Idx = m_Idx, Grade = m_Grade };
		case RewardKind.DNA: return new RES_REWARD_ZOMBIE() { Type = Res_RewardType.DNA, Idx = m_Idx, Grade = m_Grade };
		}
		return new RES_REWARD_ITEM() { Type = Res_RewardType.Item, Idx = m_Idx, Cnt = m_Stack, Grade = m_Grade, LV = m_Lv };
	}

	public long GetPrice()
	{
		return Price.Sum();
	}

	public long GetMinBuyPrice()
	{
		// 최소금액 입찰자가 없을때는 해당 가격으로
		if (UserNo < 1) return GetPrice();
		return (long)(GetPrice() * (1f + TDATA.GetConfig_Float(ConfigType.AuctionbidMin)));
	}
}
