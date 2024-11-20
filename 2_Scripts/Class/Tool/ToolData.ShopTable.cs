using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using static LS_Web;

public class TShopTable : ClassMng
{
	public class Reward
	{
		/// <summary> RewardType 1 </summary>
		public RewardKind m_ItemType;
		/// <summary> 판매 상품 1 (ItemTable index) </summary>
		public int m_ItemIdx;
		/// <summary> 판매 수량 1 </summary>
		public int m_ItemCnt;
	}
	/// <summary> 인덱스 </summary>
	public int m_Idx;
	/// <summary> 이름 </summary>
	public int m_Name;
	/// <summary> 설명 </summary>
	public int m_Info;
	/// <summary> 마켓 </summary>
	public EMarketType m_Market;
	/// <summary> 구매 레벨 </summary>
	public int m_Level;
	/// <summary> 그룹 </summary>
	public ShopGroup m_Group;
	/// <summary> No or Prob 정렬순서 or 확율 (동순 일 경우 Index 빠른 순서) </summary>
	public int m_NoOrProb;
	public List<Reward> m_Rewards = new List<Reward>();
	///// <summary> 화폐 Index (ItemTable의 Index 참조), -1일 경우 현금, 0일 경우 금니(유료 화폐) </summary>
	//public int m_PriceIdx;
	/// <summary> 화폐 타입 </summary>
	public PriceType m_PriceType;
	/// <summary> 타입이 아이템일때 인덱스</summary>
	public int m_PriceIdx;
	/// <summary> 가격 </summary>
	public int m_Price;
	/// <summary> 금액 변동 값 </summary>
	public int m_UpPrice;
	/// <summary> 금액 맥스 </summary>
	public int m_MaxPrice;
	/// <summary> 포인트 </summary>
	public int m_Point;
	public string m_Img;
	/// <summary> 원가 </summary>
	public int m_FirstPrice;
	/// <summary>시간 (분) / 상점의 해당 제품의 구매 횟수가 초기화 되는 시간 </summary>
	public int m_ResetTime;
	/// <summary> ResetTime 내에 상점에서 해당 상품을 구매 가능한 횟수 / 0일 경우 무제한 </summary>
	public int m_LimitCnt;
	/// <summary> 리셋 타입  </summary>
	public ShopResetType m_ResetType;
	/// <summary> 시즌, 더블 등 태그 표기 </summary>
	public TagType m_TagType;

	public TShopTable(CSV_Result pResult) {
		m_Idx = pResult.Get_Int32();
		m_Name = pResult.Get_Int32();
		m_Info = pResult.Get_Int32();
		m_Market = pResult.Get_Enum<EMarketType>();
		m_Level = pResult.Get_Int32();
		m_Group = pResult.Get_Enum<ShopGroup>();
		m_NoOrProb = pResult.Get_Int32();
		for(int i = 0; i < 4; i++) {
			Reward reward = new Reward() {
				m_ItemType = pResult.Get_Enum<RewardKind>(),
				m_ItemIdx = pResult.Get_Int32(),
				m_ItemCnt = pResult.Get_Int32()
			};
			if (reward.m_ItemIdx == 0 && reward.m_ItemCnt == 0) continue;
			m_Rewards.Add(reward);
		}
		m_PriceType = pResult.Get_Enum<PriceType>();
		m_PriceIdx = pResult.Get_Int32();
		m_Price = pResult.Get_Int32();
		m_UpPrice = pResult.Get_Int32();
		m_MaxPrice = pResult.Get_Int32();
		m_Point = pResult.Get_Int32();
		m_Img = pResult.Get_String();
#if USE_LOG_MANAGER
		if (!string.IsNullOrEmpty(m_Img) && !m_Img.Contains("/"))
			Debug.LogError($"[ ShopTable ({m_Idx}) ] m_Img 패스 체크할것");
#endif
		m_FirstPrice = pResult.Get_Int32();
		m_ResetTime = pResult.Get_Int32();
		m_LimitCnt = pResult.Get_Int32();
		m_ResetType = pResult.Get_Enum<ShopResetType>();
		m_TagType = pResult.Get_Enum<TagType>();
	}

	public string GetName()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Name);
	}

	public string GetInfo()
	{
		return TDATA.GetString(ToolData.StringTalbe.Etc, m_Info);
	}

	public bool IS_PriceAD() {
		return m_PriceType == PriceType.AD || m_PriceType == PriceType.AD_AddTime || m_PriceType == PriceType.AD_InitTime;
	}

	public int GetPriceIdx() {
		switch (m_PriceType) {
			case PriceType.Item: return m_PriceIdx;
			case PriceType.Cash: return BaseValue.CASH_IDX;
			case PriceType.Energy: return BaseValue.ENERGY_IDX;
			case PriceType.Money: return BaseValue.DOLLAR_IDX;
			case PriceType.PVPCoin: return BaseValue.PVPCOIN_IDX;
			case PriceType.GuildCoin: return BaseValue.GUILDCOIN_IDX;
			case PriceType.Mileage: return BaseValue.MILEAGE_IDX;
		}
		return 0;
	}

	public int GetPrice(int Cnt = 1)
	{
		int buycnt = 0;
		var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == m_Idx);
		if (buyinfo != null) buycnt = buyinfo.Cnt;
		int price = m_Price * Cnt;
		if (m_UpPrice > 0) price += m_UpPrice * ((buycnt * 2 + Cnt - 1) * Cnt / 2);
		if (m_MaxPrice > 0) price = Math.Min(price, m_MaxPrice);
		return price;
	}

	public string[] GetPriceTxt() {
		string[] prices = new string[3];
		LS_Web.RES_SHOP_PID_INFO pinfo = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == m_Idx);
		if (pinfo != null) {
			var payprice = IAP.GetPrice(pinfo.PID);
			string price = string.IsNullOrEmpty(payprice) ? pinfo.PriceText : payprice;
			//string unit = Regex.Replace(price, @"\d", "");
			//string num = Regex.Replace(price, @"\D", "");
			bool issale = string.IsNullOrEmpty(pinfo.SaleText);
			prices[0] = price;
			prices[1] = price;
			prices[2] = issale ? string.Empty : pinfo.SaleText;//  string.Format("{0}%", (float)m_Price / (float)m_FirstPrice * 100);
		}
		else {
			switch (m_PriceType) {
				case PriceType.Cash:
				case PriceType.Money:
					string name = m_PriceType == PriceType.Cash ? BaseValue.GetPriceTypeName(PriceType.Cash) : BaseValue.GetPriceTypeName(PriceType.Money);
					prices[0] = string.Format("{0} {1}", name, Utile_Class.CommaValue(GetPrice()));
					prices[1] = string.Format("{0} {1}", name, Utile_Class.CommaValue(m_FirstPrice));
					prices[2] = m_FirstPrice == 0 ? string.Empty : string.Format("{0}%", (float)m_Price / (float)m_FirstPrice * 100);
					break;
			}
		}
		return prices;
	}
	public Sprite GetImg()
	{
		return UTILE.LoadImg(m_Img, "png");
	}
}
public class TShopTableMng : ToolFile
{
	public Dictionary<int, TShopTable> DIC_Idx = new Dictionary<int, TShopTable>();
	public Dictionary<ShopGroup, List<TShopTable>> DIC_Group = new Dictionary<ShopGroup, List<TShopTable>>();

	public TShopTableMng() : base(new string[] { "Datas/ShopTable", "Datas/ShopTable_BlackMarket", "Datas/ShopTable_GuildMarket", "Datas/ShopTable_PvPMarket" })
	{
	}

	public override void DataInit()
	{
		DIC_Idx.Clear();
		DIC_Group.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TShopTable data = new TShopTable(pResult);
		if (data.m_Market != EMarketType.ALL && data.m_Market != APPINFO.Market) return;

		DIC_Idx.Add(data.m_Idx, data);
		if (!DIC_Group.ContainsKey(data.m_Group))
			DIC_Group.Add(data.m_Group, new List<TShopTable>());
		DIC_Group[data.m_Group].Add(data);
	}
	public override void CheckData()
	{
		//그룹별 출력순서 정렬
		for (int i = 0; i < DIC_Group.Count; i++)
		{
			DIC_Group.ElementAt(i).Value.Sort((TShopTable _a, TShopTable _b) => {
				return _a.m_NoOrProb.CompareTo(_b.m_NoOrProb);
			});
		}
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// ShopTable
	TShopTableMng m_Shop = new TShopTableMng();

	public TShopTable GetShopTable(int _idx) {
		if (!m_Shop.DIC_Idx.ContainsKey(_idx)) return null;
		return m_Shop.DIC_Idx[_idx];
	}

	public List<TShopTable> GetGroupShopTable(ShopGroup group) {
		if (!m_Shop.DIC_Group.ContainsKey(group)) return null;
		return m_Shop.DIC_Group[group].FindAll(o=>o.m_Market == EMarketType.ALL || o.m_Market == APPINFO.Market);
	}

	public Dictionary<int, TShopTable> GetAllShopTable() {
		return m_Shop.DIC_Idx
			.Select(o => new KeyValuePair<int, TShopTable>(o.Key, o.Value))
			.Where(o => o.Value.m_Market == EMarketType.ALL || o.Value.m_Market == APPINFO.Market)
			.ToDictionary(o => o.Key, o=>o.Value);
		//return m_Shop.DIC_Idx;
	}
}

