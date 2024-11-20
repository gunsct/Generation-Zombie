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
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Shop
	public class RES_SHOP_INFO : RES_BASE {
		public List<RES_SHOP_ITEM_INFO> Infos = new List<RES_SHOP_ITEM_INFO>();
		public List<RES_SHOP_PID_INFO> PIDs = new List<RES_SHOP_PID_INFO>();
		public List<RES_SHOP_USER_BUY_INFO> BUYs = new List<RES_SHOP_USER_BUY_INFO>();
		public List<RES_SHOP_DAILYPACK_INFO> PACKs = new List<RES_SHOP_DAILYPACK_INFO>();
		public List<RES_SHOP_SEASON_INFO> Seasons = new List<RES_SHOP_SEASON_INFO>();

		public int MileageNo;

		RES_SHOP_SEASON_INFO _NowSeason;

		List<RES_SHOP_ITEM_INFO> _Infos;
		public List<RES_SHOP_ITEM_INFO> UseInfos {
			get {
				Check_Infos();
				return _Infos;
			}

		}
		public RES_SHOP_SEASON_INFO NowSeason
		{
			get
			{
				Check_Infos();
				return _NowSeason;
			}

		}
		// 진행중이 패스
		[JsonIgnore] public List<RES_SHOP_ITEM_INFO> PassInfo { get => UseInfos.FindAll(o => {
			if (o.UseType != ShopUseType.Time) return false;
			var tdata = MainMng.Instance.TDATA.GetShopTable(o.Idx);
			if (tdata == null) return false;
			if (tdata.m_Group != ShopGroup.Pass) return false;
			var servertime = MainMng.Instance.UTILE.Get_ServerTime_Milli();
			if (o.Times[1] <= servertime) return false;
			if (o.Times[0] > servertime) return false;
			return true;
		}); }
		public List<RES_SHOP_ITEM_INFO> GetInfos(ShopGroup _group) {
			return UseInfos.FindAll(o => {
				if (o.UseType == ShopUseType.None) return false;
				if (o.UseType == ShopUseType.Time) {
					var servertime = MainMng.Instance.UTILE.Get_ServerTime_Milli();
					if (o.Times[1] <= servertime) return false;
					if (o.Times[0] > servertime) return false;
				}
				var tdata = MainMng.Instance.TDATA.GetShopTable(o.Idx);
				if (tdata == null) return false;
				if (tdata.m_Group != _group) return false;
				return true;
			});
		}

		public bool IsPassBuy()
		{
			var Pass = PlayPass();
			if (Pass == null) return false;
			var buy = BUYs.Find(o => o.Idx == Pass.Idx);
			return buy != null && buy.UTime >= Pass.Times[0];
		}
		public RES_SHOP_ITEM_INFO PlayPass()
		{
			var list = PassInfo;
			if (list == null || list.Count < 1) return null;
			return list[0];
		}

		public void SetBuyInfo(int Idx, int Cnt = 1)
		{
			var buy = BUYs.Find(o => o.Idx == Idx);
			if (buy == null)
			{
				buy = new RES_SHOP_USER_BUY_INFO()
				{
					Idx = Idx,
					Cnt = 0,
					UTime = 0,
				};
				BUYs.Add(buy);
			}
			buy.Cnt += Cnt;
			buy.UTime = (long)MainMng.Instance.m_Utile.Get_ServerTime_Milli();

			//var tdata = MainMng.Instance.m_ToolData.GetShopTable(Idx);
			//switch (tdata.m_Group)
			//{
			//case ShopGroup.DailyPack:
			//	SetPackageInfo(new RES_SHOP_DAILYPACK_INFO() { Idx = Idx, Stap = 1, UTime = buy.UTime });
			//	break;
			//}
		}

		public void SetPackageInfo(List<RES_SHOP_DAILYPACK_INFO> datas)
		{
			for(int i = 0; i < datas.Count; i++) SetPackageInfo(datas[i]);
		}

		public void SetPackageInfo(RES_SHOP_DAILYPACK_INFO data)
		{
			var info = PACKs.Find(o => o.Idx == data.Idx);
			if (info == null) PACKs.Add(data);
			else
			{
				info.Stap = data.Stap;
				info.UTime = data.UTime;
				info.ETime = data.ETime;
			}
		}

		public int GetEnergyPrice()
		{
			var tshop = MainMng.Instance.m_ToolData.GetShopTable(BaseValue.ENERGY_SHOP_IDX);
			return tshop.GetPrice();
		}

		void Check_Infos()
		{
			bool change = _Infos == null;
			if (Seasons != null && Seasons.Count > 0)
			{
				if (_NowSeason == null || (_NowSeason.ETime < MainMng.Instance.UTILE.Get_ServerTime_Milli()))
				{
					_NowSeason = Seasons.Find(o => o.IS_NowSeason);
					change = true;
				}
			}
			else if (_NowSeason != null)
			{
				_NowSeason = null;
				change = true;
			}
			if (change)
			{
				// 시즌데이터 체크하고
				if (_NowSeason == null)
				{
					_Infos = new List<RES_SHOP_ITEM_INFO>();
					_Infos.AddRange(Infos);
				}
				else
				{
					_Infos = _NowSeason.GetItemInfos();
					// 시즌데이터에 없는놈 Infos에서 찾아서 넣기
					_Infos.AddRange(Infos.FindAll(o => !_NowSeason.Items.Contains(o.Idx)));
				}
			}
		}
	}

	public class RES_SHOP_ITEM_INFO : RES_BASE
	{
		/// <summary> 아이템 인덱스 </summary>
		public int Idx;
		/// <summary> 사용 타입 </summary>
		public ShopUseType UseType;
		/// <summary> ShopUseType.Time 일때 사용(0:시작시간, 1:종료시간) </summary>
		public long[] Times = new long[2];
	}

	public class RES_SHOP_PID_INFO : RES_BASE
	{
		/// <summary> 사용 재화 </summary>
		public int Idx;
		/// <summary> PID </summary>
		public string PID;
		/// <summary> 마켓 금액을 받지 못할경우 사용될 금액 </summary>
		public string PriceText;
		/// <summary> 세일 정보 표기 null 이거나 Empty 일때 세일정보 없음 </summary>
		public string SaleText;
		
	}

	public class RES_SHOP_USER_BUY_INFO : RES_BASE
	{
		/// <summary> 상점 인덱스 </summary>
		public int Idx;
		/// <summary> 구매횟수 </summary>
		int _Cnt;
		/// <summary> 구매횟수 </summary>
		public int Cnt {
			set { _Cnt = value; }
			get
			{
				ObjMng mng = MainMng.Instance;
				var tshop = mng.TDATA.GetShopTable(Idx);
				if(tshop.m_Group == ShopGroup.Guild_master)
				{
					if (mng.USERINFO.m_Guild.Items.Exists(o => o.Idx == tshop.m_Rewards[0].m_ItemIdx)) return 1;
					else return 0;
				}
				long time = GetTime();
				if (time == 0) return _Cnt;
				if (time <= mng.UTILE.Get_ServerTime_Milli()) return 0;
				switch(tshop.m_ResetType)
				{
				case ShopResetType.Season:
					// 시즌의 시작시간이랑 업데이트시간을 비교해야함
					var season = mng.USERINFO.m_ShopInfo.NowSeason;
					if (season != null && UTime <= season.STime) return 0;
					break;
				}

				return _Cnt;
			}
		}
		/// <summary> 마지막 구매한 시간 </summary>
		public long UTime { get; set; }
		/// <summary> 구독상품 남은 기간 </summary>
		public long GetTime()
		{
			ObjMng mng = MainMng.Instance;
			var tdata = mng.TDATA.GetShopTable(Idx);
			if (tdata.m_LimitCnt < 1) return 0;
			long time = UTime;
			switch (tdata.m_ResetType)
			{
			case ShopResetType.ZeroTime:
				if (tdata.m_ResetTime < 1) return 0;
				time = (long)(mng.UTILE.Get_ZeroTime(time) + tdata.m_ResetTime * 60000L);
				break;
			case ShopResetType.AddTime:
				if (tdata.m_ResetTime < 1) return 0;
				time += tdata.m_ResetTime * 60000L;
				break;
			case ShopResetType.DayOfWeek:
				// 해당 요일이 지났을때
				time = (long)(mng.UTILE.Get_ZeroTime(time));
				time += (7 + (tdata.m_ResetTime - (int)Utile_Class.Get_ServerDateTime().DayOfWeek)) * 86400000L;
				break;
			case ShopResetType.Season:
				var season = mng.USERINFO.m_ShopInfo.NowSeason;
				if (season == null) return 0;
				time = season.ETime;
				break;
			default: return 0;
			}
			return time;
		}
	}

	public class RES_SHOP_DAILYPACK_INFO : RES_BASE
	{
		////////////////////////////////////////////////////////////////////////////////////////////////
		// 최초한번은 구매시 보상이 받아지고 REQ_SHOP_GET_DAILYPACKITEM 으로 보상 받음
		// 구매 : SEND_REQ_SHOP_BUY 또는 SEND_REQ_SHOP_BUY_INAPP
		// 이 후 보상 받기 : SEND_REQ_SHOP_GET_DAILYPACKITEM
		// RES_SHOP_USER_BUY_INFO가 null이거나 Cnt값이 0이면 구매필요
		// SEND_REQ_SHOP_GET_DAILYPACKITEM의 GetTime() == 0 이면 보상 받기 가능
		/// <summary> 상점 인덱스 </summary>
		public int Idx { get; set; }
		/// <summary> 마지막에 받은 보상 단계 (구매시 즉시 지급 1부터 시작) </summary>
		public int Stap { get; set; }
		/// <summary> 마지막 구매한 시간 </summary>
		public long UTime { get; set; }
		/// <summary> 종료 시간 </summary>
		public long ETime { get; set; }

		/// <summary> 다음 보상까지 남은 시간  </summary>
		public long GetTime()
		{
			ObjMng mng = MainMng.Instance;
			long etime = (long)mng.UTILE.Get_ZeroTime(UTime) + 86400000L;
			return Math.Max(0, etime - (long)mng.UTILE.Get_ServerTime_Milli());
		}

		public bool IsReward()
		{
			var mng = MainMng.Instance;
			var now = (long)mng.UTILE.Get_ServerTime_Milli();
			return ETime > now && !mng.UTILE.IsSameDay(UTime);
		}

		public bool IsPlayPack()
		{
			var mng = MainMng.Instance;
			var now = (long)mng.UTILE.Get_ServerTime_Milli();
			return ETime > now;
		}

		public long GetLastTime() {
			var mng = MainMng.Instance;
			//int allstep = mng.TDATA.GeTPackageRewardGroupTable(Idx).Count;
			//if (allstep == Stap) return 0;
			//long etime = GetTime() + 86400000L * (allstep - Stap);
			//return etime;
			return ETime - (long)mng.UTILE.Get_ServerTime_Milli();
		}
	}
	public class RES_SHOP_SEASON_INFO : RES_BASE
	{
		public List<int> Items { get; set; }
		public long STime { get; set; }
		public long ETime { get; set; }
		
		public List<RES_SHOP_ITEM_INFO> GetItemInfos() {
			return Items.Select(o => new RES_SHOP_ITEM_INFO() { UseType = ShopUseType.Time, Idx = o, Times = new long[] { STime, ETime } }).ToList();
		}
		public bool IS_NowSeason { get { 
			double time = MainMng.Instance.UTILE.Get_ServerTime_Milli();
			return time >= STime && time < ETime;
		} }
	}

	/// <summary> 상점 정보 </summary>
	public void SEND_REQ_SHOP_INFO(Action<RES_SHOP_INFO> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;
		SendPost(Protocol.REQ_SHOP_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_SHOP_INFO>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_BUY

	void SHOP_BUY_RESULT(Action<ushort, string> CB = null)
	{

	}

	public class REQ_SHOP_BUY : REQ_BASE
	{
		public class Buy_Item
		{
			public int Idx;
			public int Cnt;
		}

		/// <summary> 상점 아이템 정보 </summary>
		public List<Buy_Item> Items = new List<Buy_Item>();

		// 인앱때만 사용함
		/// <summary> 상점 구매 시도 마켓 </summary>
		public EMarketType Market;
		/// <summary> 구매 정보 </summary>
		public string Receipt;
		/// <summary> 구매 개수 </summary>
		public int Cnt;

		/// <summary> 길드 창고로 구매 </summary>
		public bool IsInsertGuild;

		/// <summary> 픽업 데이터 인덱스 </summary>
		public List<int> PickIdxs = new List<int>();
		/// <summary> 블루스택등 에뮬레이터로 구매한것인지 확인 </summary>
		public bool IsEmul;
#if UNITY_EDITOR
		/// <summary> 유니티 에디터 상태 </summary>
		public string IsEditer;
#endif
	}

	/// <summary> 돌발이벤트 블랙마켓 아이템 구매 </summary>
	/// <param name="idx">구매할 아이템 인덱스</param>
	public void SEND_REQ_SHOP_BUY(Action<RES_BASE> action, List<REQ_SHOP_BUY.Buy_Item> _buyiteminfo, List<int> _pickup = null)
	{
		REQ_SHOP_BUY _data = new REQ_SHOP_BUY();
		_data.UserNo = USERINFO.m_UID;
		_data.Items = _buyiteminfo;
		_data.PickIdxs = _pickup != null ? _pickup : USERINFO.GetGachaPickUp();
		_data.IsEmul = APPINFO.IsEmulator();
		var Group = _buyiteminfo.GroupBy(o => TDATA.GetShopTable(o.Idx).m_Group, o => TDATA.GetShopTable(o.Idx)).ToDictionary(o => o.Key, o => o.ToList());
		_data.IsInsertGuild = Group.ContainsKey(ShopGroup.Guild_master);
		SendPost(Protocol.REQ_SHOP_BUY, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess()) {
				switch (TDATA.GetShopTable(_buyiteminfo[0].Idx).m_PriceType) {
					case PriceType.Money: PlayEffSound(SND_IDX.SFX_0110); break;
				}

				if (_buyiteminfo[0].Idx == BaseValue.ENERGY_SHOP_IDX) {
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(803));
				}

				for (int i = 0; i < _buyiteminfo.Count; i++) {
					TShopTable tdata = TDATA.GetShopTable(_buyiteminfo[i].Idx);
					USERINFO.m_ShopInfo.SetBuyInfo(_buyiteminfo[i].Idx, _buyiteminfo[i].Cnt);

					switch (tdata.m_Group) {
						case ShopGroup.BlackMarket:
							USERINFO.Check_Mission(MissionType.BlackMarket, 0, 0, 1);
							break;
						case ShopGroup.Gacha:
							USERINFO.Check_Mission(MissionType.CharGacha, 0, 0, res.GetRewards().Count);
							break;
						case ShopGroup.ItemGacha:
							USERINFO.Check_Mission(MissionType.ItemGacha, 0, 0, res.GetRewards().Count);
							break;
						case ShopGroup.PVPShop:
							USERINFO.Check_Mission(MissionType.BuyPVPStore, 0, 0, 1);
							break;
						case ShopGroup.Guild_normal_DNA:
						case ShopGroup.Guild_normal_Char:
						case ShopGroup.Guild_member:
						case ShopGroup.Guild_master:
							USERINFO.Check_Mission(MissionType.ButGuildStore, 0, 0, 1);
							break;
						case ShopGroup.Pass:
							DLGTINFO.f_RFVIPBtn?.Invoke();
							break;
					}

					switch (tdata.m_Idx) {
						case BaseValue.CONTINUETICKET_SHOP_IDX:
						case BaseValue.CONTINUEAD_SHOP_IDX:
							USERINFO.Check_Mission(MissionType.Continue, 0, 0, 1);
							break;
					}

					switch(tdata.m_PriceType)
					{
					case PriceType.AD:
					case PriceType.AD_AddTime:
					case PriceType.AD_InitTime:
						HIVE.Analytics_ad_reward();
						break;
					}

					if (_data.IsInsertGuild) {
						for(int j = 0; j < tdata.m_Rewards.Count; j++)
						{
							var temp = USERINFO.m_Guild.Items.Find(o => o.Idx == tdata.m_Rewards[j].m_ItemIdx);
							if (temp == null)
							{
								temp = new RES_GUILD_ITEM();
								temp.Idx = tdata.m_Rewards[j].m_ItemIdx;
								USERINFO.m_Guild.Items.Add(temp);
							}
							temp.Cnt++;
						}
					}
				}
			}
			HIVE.AutoCheck_LocalPush();
			action?.Invoke(res);
		});
	}
	public class PayLoad
	{
		public long UserNo { get; set; }
		public int Idx { get; set; }
		public long PUID { get; set; }
	}

	public class RES_SHOP_BUY_INAPP : RES_BASE
	{
		/// <summary> PayLoad </summary>
		public string PayLoad;
	}

	/// <summary> 구매 검증 PUID 받기 </summary>
	/// <param name="idx">구매할 아이템 인덱스</param>
	/// <param name="PUID">검증 PUID</param>
	/// <param name="market">구매 마켓</param>
	/// <param name="Receipt">영수증</param>
	public void SEND_REQ_SHOP_BUY_INAPP(Action<RES_BASE> action, EMarketType market, string Receipt, string payload)
	{
		REQ_SHOP_BUY _data = new REQ_SHOP_BUY();
		_data.UserNo = USERINFO.m_UID;
		//_data.Items = new List<REQ_SHOP_BUY.Buy_Item>() { new REQ_SHOP_BUY.Buy_Item() { Idx = payloaddata.Idx, Cnt = 1 } };
		_data.Market = market;
		_data.Receipt = Receipt;
		_data.IsEmul = APPINFO.IsEmulator();
#if UNITY_EDITOR
		_data.IsEditer = payload;
#endif
		SendPost(Protocol.REQ_SHOP_INAPP, JsonConvert.SerializeObject(_data), (result, data) => {

			RES_SHOP_BUY_INAPP res = ParsResData<RES_SHOP_BUY_INAPP>(data);
			if (res.IsSuccess())
			{
				if(!string.IsNullOrEmpty(res.PayLoad))
				{
					var payloaddata = GetPayLoadData(res.PayLoad);
					USERINFO.m_ShopInfo.SetBuyInfo(payloaddata.Idx, 1);
					if (TDATA.GetShopTable(payloaddata.Idx).m_Group == ShopGroup.BlackMarket) USERINFO.Check_Mission(MissionType.BlackMarket, 0, 0, 1);
				}
				HIVE.AutoCheck_LocalPush();
			}
			action?.Invoke(res);
		});
	}

	public PayLoad GetPayLoadData(string data)
	{
		// base64해제
		byte[] abyData = Convert.FromBase64String(data);

		// 복호화
		Utile_Class.Decode(abyData, abyData.Length, 0);
		var json = Encoding.UTF8.GetString(abyData);
		return JsonConvert.DeserializeObject<PayLoad>(json, new JsonConverter[] { new RES_REWARD_BASE_Conv() });
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// PayLoad
	public class REQ_SHOP_PUID : REQ_BASE
	{
		/// <summary> 상점 아이템 인덱스 </summary>
		public int Idx;
	}
	public class RES_SHOP_PUID : RES_BASE
	{
		/// <summary> PayLoad </summary>
		public string PayLoad;
	}
	/// <summary> 구매 검증 PUID 받기 </summary>
	/// <param name="idx">구매할 아이템 인덱스</param>
	public void SEND_REQ_SHOP_PUID(Action<RES_SHOP_PUID> action, int idx)
	{
		REQ_SHOP_PUID _data = new REQ_SHOP_PUID();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = idx;
		SendPost(Protocol.REQ_SHOP_PUID, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_SHOP_PUID>(data));
		});
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// inven buy
	public class REQ_SHOP_BUY_INVEN : REQ_BASE
	{
		/// <summary> 구매 개수 </summary>
		public int Cnt;
	}
	/// <summary> 구매 검증 PUID 받기 </summary>
	/// <param name="idx">구매할 아이템 인덱스</param>
	public void SEND_REQ_SHOP_BUY_INVEN(Action<RES_BASE> action, int cnt)
	{
		REQ_SHOP_BUY_INVEN _data = new REQ_SHOP_BUY_INVEN();
		_data.UserNo = USERINFO.m_UID;
		_data.Cnt = cnt;
		SendPost(Protocol.REQ_SHOP_BUY_INVEN, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess()) {
				USERINFO.m_ShopInfo.SetBuyInfo(BaseValue.SHOP_IDX_INVEN, cnt);
			}
			action?.Invoke(res);
		});
	}

#region REQ_SHOP_BUY_MISSION_CNT
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_STAGE_BUY
	public class REQ_SHOP_BUY_PASS_LV : REQ_BASE
	{
		public List<long> UIDs;
	}

	public class RES_SHOP_BUY_PASS_LV : RES_BASE
	{
		public List<RES_MISSIONINFO> Missions = new List<RES_MISSIONINFO>();
	}
	public void SEND_REQ_SHOP_BUY_PASS_LV(Action<RES_SHOP_BUY_PASS_LV> action, List<long> UIDs)
	{
		REQ_SHOP_BUY_PASS_LV _data = new REQ_SHOP_BUY_PASS_LV();
		_data.UserNo = USERINFO.m_UID;
		_data.UIDs = UIDs;
		SendPost(Protocol.REQ_SHOP_BUY_PASS_LV, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_SHOP_BUY_PASS_LV res = ParsResData<RES_SHOP_BUY_PASS_LV>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_ShopInfo.SetBuyInfo(BaseValue.PASS_LV_SHOP_IDX, 1);
				USERINFO.m_Mission.SetData(res.Missions, false);
			}
			action?.Invoke(res);
		});
	}
#endregion

#region REQ_SHOP_GET_DAILYPACKITEM
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_SHOP_GET_DAILYPACKITEM
	public class REQ_SHOP_GET_DAILYPACKITEM : REQ_BASE
	{
		public int Idx;
	}
	
	public class RES_SHOP_GET_DAILYPACKITEM : RES_BASE
	{
		public RES_SHOP_DAILYPACK_INFO Pack;
	}
	public void SEND_REQ_SHOP_GET_DAILYPACKITEM(Action<RES_SHOP_GET_DAILYPACKITEM> action, int ShopIdx)
	{
		REQ_SHOP_GET_DAILYPACKITEM _data = new REQ_SHOP_GET_DAILYPACKITEM();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = ShopIdx;
		SendPost(Protocol.REQ_SHOP_GET_DAILYPACKITEM, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_SHOP_GET_DAILYPACKITEM res = ParsResData<RES_SHOP_GET_DAILYPACKITEM>(data);
			if (res.IsSuccess()) USERINFO.m_ShopInfo.SetPackageInfo(res.Pack);
			action?.Invoke(res);
		});
	}
#endregion
}
