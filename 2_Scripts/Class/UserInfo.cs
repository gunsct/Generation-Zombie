using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
#if UNITY_EDITOR
using System.Globalization;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;
using static LS_Web;
using System.Text;
using GPresto.Protector.Variables;

public partial class UserInfo : ClassMng
{
	public class StageSelect
	{
		/// <summary> CaseSelectTable Idx </summary>
		public int Idx;
		/// <summary> 선택지를 노출시킨 다이얼로그 Idx </summary>
		public int Dlg;
		/// <summary> 선택지 보상 </summary>
		public List<RES_REWARD_ITEM> Rewards;
	}

	public class StageIdx
	{
		/// <summary> 던전 위치(요일던전용) </summary>
		public int Pos;
		/// <summary> 던전 요일 (0 : 일요일 ~ 6 : 토요일) </summary>
		public DayOfWeek Week;
		/// <summary> 스테이지 : 인덱스, 던전 레벨 </summary>
		public int Idx;
		/// <summary> 클리어 정보(스테이지 : 인덱스, 던전 : 최고레벨) </summary>
		public int Clear;
		/// <summary> 도전 횟수 </summary>
		public int PlayCount;
		/// <summary> 덱 위치 </summary>
		public int DeckPos;
		/// <summary> 시드 선택 인덱스 0이 아닐경우 시드선택을 해야됨 </summary>
		public int AltReward;
		/// <summary> 챕터 보상이 있을경우 시드선택이후 보상을 받아야됨 </summary>
		public int ChapterReward;
		/// <summary> 토크 선택 정보 </summary>
		public List<StageSelect> Selects = new List<StageSelect>();
	}

	internal void InsertSysMsgInfo(RES_SYSTEM_MSG rES_SYSTEM_MSG) {
		throw new NotImplementedException();
	}

	public class Stage : ClassMng
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 스테이지 모드 </summary>
		public StageContentType Mode;
		/// <summary> 스테이지 => 스테이지 인덱스, 던전 => 레벨, 요일던전 => ((요일 - 1) * 2 + pos </summary>
		public List<StageIdx> Idxs = new List<StageIdx>();
		/// <summary> 하루마다 초기화 0 : 일반 입장 제한, 1 : 구매 제한 </summary>
		public int[] PlayLimit = new int[2];
		/// <summary> 마지막 플레이한 시간 (limit초기화용) </summary>
		public long LTime;

		public long GetItemCnt(bool _isBuy) {
			CalcLimit();
			return PlayLimit[_isBuy ? 1 : 0];
		}

		public int GetTicketPrice()
		{
			switch (Mode) {
				case StageContentType.Bank: return TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_LIMIT_BANK).GetPrice();
				case StageContentType.Academy: return TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_LIMIT_ACADEMY).GetPrice();
				case StageContentType.University: return TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_LIMIT_UNIVERSITY).GetPrice();
				case StageContentType.Cemetery: return TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_LIMIT_CEMETERY).GetPrice();
				case StageContentType.Factory: return TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_LIMIT_FACTORY).GetPrice();
				case StageContentType.Subway: return TDATA.GetShopTable(BaseValue.SHOP_IDX_STAGE_LIMIT_SUBWAY).GetPrice();
			}
			return 0;
			//return BaseValue.TICKET_PRICE + (int)(GetItemMax(true) - GetItemCnt(true)) * BaseValue.TICKET_PRICEUP;
		}

		public int GetItemMax(bool IsBuyMax) {
			if(IsBuyMax) return BaseValue.GetStageBuyLimit(Mode);
			else
			{
				switch (Mode)
				{
				case StageContentType.Academy: return TDATA.GetConfig_Int32(ConfigType.Mode_Academy_Count);
				case StageContentType.Bank: return TDATA.GetConfig_Int32(ConfigType.Mode_Bank_Count);
				case StageContentType.University: return TDATA.GetConfig_Int32(ConfigType.Mode_University_Count);
				case StageContentType.Tower: return TDATA.GetConfig_Int32(ConfigType.Mode_Tower_Count);
				case StageContentType.Cemetery: return TDATA.GetConfig_Int32(ConfigType.Mode_Cemetery_Count);
				case StageContentType.Factory: return TDATA.GetConfig_Int32(ConfigType.Mode_Factory_Count);
				case StageContentType.Subway: return TDATA.GetConfig_Int32(ConfigType.Mode_Subway_Count);
				}
				return TDATA.GetConfig_Int32(ConfigType.StageEnergyMax);
			}
		}
		public void CalcLimit() {
			long now = (long)UTILE.Get_ServerTime_Milli();
			long temp = LTime / (86400000L);
			bool IsOneDay = temp != now / 86400000L;
			if (!IsOneDay) return;
			CountInit();
		}
		public void ItemCntInit() {
			CalcLimit();
		}

		/// <summary> 입장권 사용 클라용</summary>
		public void UseItem() {
			if (GetItemMax(false) < 1) return;
			CalcLimit();
			// 아이템 실시간 체크
			LTime = (long)UTILE.Get_ServerTime_Milli();
			PlayLimit[0]--;
		}

		/// <summary> 입장수, 구입횟수 초기화 </summary>
		public void CountInit() {
			// 하루단위 변환
			PlayLimit[0] = GetItemMax(false);
			PlayLimit[1] = GetItemMax(true);
		}

		/// <summary> 던전 입장 가능 수량 체크 </summary>
		public bool IS_CanGoStage() {
			return GetItemCnt(false) > 0 || GetItemMax(false) == 0;
		}
		/// <summary> 던전 입장권 구입 수량 체크 </summary>
		public bool IS_CanBuy() {
			return GetItemCnt(true) > 0;
		}
		/// <summary> 던전 티켓 구매 클라용</summary>
		public void ButItem() {
			PlayLimit[0] += 3;
			PlayLimit[1] -= 1;
		}
		/// <summary> 스테이지인 경우 마지막 스테이지인지 체크 </summary>
		public bool IS_LastStage() {
			return Idxs[USERINFO.GetDifficulty()].Clear == Idxs[USERINFO.GetDifficulty()].Idx;
		}
	}

	public class TimeItem : ClassMng
	{
		/// <summary> 카운트 </summary>
		[JsonProperty] long _Cnt;
		[JsonIgnore] public long Cnt { set { _Cnt = value; } get { CalcCnt(); return _Cnt; } }
		/// <summary> 갱신 시간(카운트가 진행된 시간) </summary>
		[JsonProperty] double _STime;
		[JsonIgnore] public double STime { set { _STime = value; } get { CalcCnt(); return _STime; } }

		long _CheckTime;
		int _MaxCnt;

		public void SetCheckData(long checktime, int max) {
			_CheckTime = checktime;
			_MaxCnt = max;
		}

		public void CalcCnt()
		{
			double now = UTILE.Get_ServerTime_Milli();
			// 맥스치일때는 계산할 필요 없음
			// 사용할때 시간을 갱신함
			if (_Cnt >= _MaxCnt)
			{
				//_STime = now;
				return;
			}
			double gap = now - _STime;
			if (gap < 0) return;
			int addCnt = (int)(gap / _CheckTime);
			if (_Cnt + addCnt >= _MaxCnt) {
				_Cnt = _MaxCnt;
				_STime = now;
			}
			else {
				_Cnt += addCnt;
				_STime += _CheckTime * addCnt;

				DLGTINFO?.f_RFShellUI?.Invoke(_Cnt);
				MAIN.Save_UserInfo();
			}
		}
		/// <summary> 초단위 시간 </summary>
		public double GetRemainTime() {
			if (UTILE.Get_ServerTime_Milli() - STime < 0) return 0;
			else return (_CheckTime - (UTILE.Get_ServerTime_Milli() - STime)) * 0.001d;
		}
		/// <summary> 최대까지 남은 시간 초단위 시간 </summary>
		public double GetMaxRemainTime()
		{
			if (_Cnt >= _MaxCnt) return 0;
			return (GetRemainTime() * 1000 + Math.Max(0, _MaxCnt - _Cnt - 1) * _CheckTime) * 0.001d;//GetRemainTime() * 1000
		}
		public int GetMaxCnt() {
			return _MaxCnt;
		}
	}

	/// <summary> 고유 번호 </summary>
	public GPLong m_UID;
	/// <summary> 길드 정보 </summary>
	public GuildInfo m_Guild = new GuildInfo();
	/// <summary> 길드 추방 체크 정보 </summary>
	public GuildKickInfo m_GuildKickCheck = new GuildKickInfo();

	/// <summary> 유저 닉네임 </summary>
	string _Name = string.Empty;
	[JsonIgnore] public string m_Name { set { _Name = value; } get { return BaseValue.GetUserName(_Name); } }
	public bool IS_SetName { get { return !string.IsNullOrWhiteSpace(_Name); } }
	/// <summary> 추천코드 </summary>
	public string MyRefCode { get { return Utile_Class.UserNoEncrypt(m_UID); } }
	/// <summary> 국가 코드 </summary>
	public string m_Nation;

	public bool IsFirstNameSet { get { return string.IsNullOrWhiteSpace(_Name); } }
	/// <summary> 유저 레벨 </summary>
	public GPInt m_LV = 1;
	/// <summary> 프로필 이미지 </summary>
	public int m_Profile = 0;
	/// <summary> 경험치 </summary>
	public GPLong[] m_Exp = new GPLong[(int)EXPType.Max];
	/// <summary> 보상 연출용 지급 전 캐릭터 경험치 </summary>
	[JsonIgnore] public long m_BExp;
	/// <summary> 재화 </summary>
	public GPLong m_Money = 0;
	/// <summary> 보상 연출용 지급 전 재화 </summary>
	[JsonIgnore] public long m_BMoney;
	/// <summary> 캐쉬, 0:무료 ,1:유료 </summary>
	public GPLong[] _Cash = new GPLong[2];
	/// <summary> 보상 연출용 지급 전 캐시량 </summary>
	[JsonIgnore] public long m_BCash;
#if NOT_USE_NET
	public long m_Cash { set { _Cash[1] = value; } get { return _Cash[0] + _Cash[1]; } }
#else
	[JsonIgnore] public long m_Cash { get { return _Cash[0] + _Cash[1]; } }
#endif
	/// <summary> PVP 랭크 </summary>
	public int m_PVPRank;
	/// <summary> 0:season, 1:league </summary>
	public long[] m_PVPpoint;
	/// <summary> PVP 재화 </summary>
	public GPLong m_PVPCoin;

	/// <summary> 길드 코인 </summary>
	public GPLong m_GCoin;
	/// <summary> 길드 탈퇴한 시간 </summary>
	public long m_GRTime;

	/// <summary> 보유중이 캐릭터 </summary>
	public List<CharInfo> m_Chars = new List<CharInfo>();

	/// <summary> 가방 크기 </summary>
	public int m_InvenSize;
	/// <summary> 좀비 보유가능 개수 </summary>
	public int m_ZombieInvenSize;
	/// <summary> 보유중인 아이템 </summary>
	public List<ItemInfo> m_Items = new List<ItemInfo>();
	/// <summary> 장착되어 사용중인 아이템 리스트 </summary>
	[JsonIgnore] public List<long> m_EqItems = new List<long>();
	[JsonIgnore] public Dictionary<EquipType, List<ItemInfo>> m_ItemDic = new Dictionary<EquipType, List<ItemInfo>>();

	/// <summary> 보유중인 모든 DNA </summary>
	public List<DNAInfo> m_DNAs = new List<DNAInfo>();

	/// <summary> 장착되어 사용중인 DNA 리스트 </summary>
	[JsonIgnore] public List<long> m_EqDNAs = new List<long>();
	public int m_InvenUseSize { get { return (m_Items.Count - m_EqItems.Count) + (m_DNAs.Count - m_EqDNAs.Count); } }
	/// <summary> 스테이지 진입시 사용할 덱 </summary>
	public int m_SelectDeck = 0;
	/// <summary> 관리창 덱 페이지5 + 각 던전별 1개(요일 1,2도 포함) 20개 PVP(atk, def) </summary>
	public DeckInfo[] m_Deck = new DeckInfo[BaseValue.MAX_DECK_CNT];
	[JsonIgnore] public DeckInfo m_PlayDeck { get { return TUTO.CheckUseCloneDeck() ? TUTO.m_CloneDeck : m_Deck[m_SelectDeck]; } }
	/// <summary> 연구 </summary>
	public List<ResearchInfo> m_Researchs = new List<ResearchInfo>();
	/// <summary> 제작 </summary>
	public List<MakingInfo> m_Making = new List<MakingInfo>();
	/// <summary> 탐험 목록 </summary>
	public List<AdventureInfo> m_Advs = new List<AdventureInfo>();
	/// <summary> 튜토리얼 체크 </summary>
	public TutoCheck m_Tuto = new TutoCheck();

	/// <summary> 스테이지 정보 </summary>
	public Dictionary<StageContentType, Stage> m_Stage = new Dictionary<StageContentType, Stage>();

	public TimeItem m_Energy = new TimeItem();

	/// <summary> 유저 캐릭터 전체 시너지 </summary>
	[JsonIgnore] public List<JobType> m_SynergyAll = new List<JobType>();

	/// <summary> 생산 레벨 (연구에의해 결정됨 일단 저장데이터에서 제거) </summary>
	[JsonIgnore] public int m_MakeLV { get { return 1 + Mathf.RoundToInt(ResearchValue(ResearchEff.MakingLevelUp)); } }

	/// <summary> 우편함 목록 </summary>
	public List<PostInfo> m_Posts = new List<PostInfo>();

	/// <summary> 보유중인 모든 좀비 </summary>
	public List<ZombieInfo> m_Zombies = new List<ZombieInfo>();

	/// <summary> 좀비 방(Cage) </summary>
	public List<ZombieRoomInfo> m_ZombieRoom = new List<ZombieRoomInfo>();

	/// <summary> 구매한 사육장 개수 </summary>
	public int m_CageCnt;
	[JsonIgnore] public int CageCnt => m_ZombieRoom.Count;// + BaseValue.START_ZOMBIE_SLOT;
	/// <summary> 우리 안에 있는 좀비 </summary>
	[JsonIgnore] public List<ZombieInfo> m_CageZobie { get { return USERINFO.m_Zombies.FindAll(o => o.m_State == ZombieState.Cage); } }
	/// <summary> 우리 안에 없는 좀비 </summary>
	[JsonIgnore] public List<ZombieInfo> m_NotCageZombie { get { return USERINFO.m_Zombies.FindAll(o => o.m_State == ZombieState.Idle); } }


	/// <summary> 캠프 빌드 동기화 </summary>
	public Dictionary<CampBuildType, CampBuildInfo> m_CampBuild = new Dictionary<CampBuildType, CampBuildInfo>();

	/// <summary> 돌발 이벤트 정보 </summary>
	public AddEventInfo AddEvent;
	/// <summary> 스테이지 종료 후 추가 이벤트 인덱스 </summary>
	public int m_AddEvent;
	/// <summary> 스테이지 종료 후 추가 이벤트에 쓰이는 NPC 이름 </summary>
	public string m_AddEventName;
	/// <summary> 유저 업적 정보 </summary>
	public AchieveInfo m_Achieve = new AchieveInfo();

	/// <summary> 유저 컬렉션 정보 </summary>
	public CollectionInfo m_Collection = new CollectionInfo();


	public MyChallenge m_MyChallenge = new MyChallenge();

	/// <summary> 이벤트 </summary>
	public FAEventMng m_Event = new FAEventMng();

	/// <summary> 장비 뽑기 경험치 </summary>
	public long m_ShopEquipGachaExp;

	/// <summary> 보급상자 레벨
	/// <para> 최초에 받아온후 클라에서 알아서 셋팅함</para>
	/// <para> 레벨업 체크 : m_SupplyBoxLV != TDATA.GetSupplyBoxLV(m_Stage[StageContentType.Stage])</para>
	/// <para> 맥스 레벨 : TDATA.GetSupplyBoxMaxLV()</para>
	/// </summary>
	public int m_SupplyBoxLV;
	public class ShopEquipGachaLvExp
	{
		public int Lv;
		public long Exp;

		public float GetExp { get { return (float)Exp; } }
		public int GetLv { get { return Math.Min(Lv, MainMng.Instance.m_ToolData.GetEquipGachaTableList().Count); } }
	}
	/// <summary> 유저활동 더미 닉네임 중복 체크용 </summary>
	[JsonIgnore] public Dictionary<int, int> m_UserActivityDummyName = new Dictionary<int, int>();
	[JsonIgnore] public RES_SHOP_INFO m_ShopInfo = new RES_SHOP_INFO();
	/// <summary> 현재 뽑혀있는 암시장 목록 </summary>
	[JsonIgnore] public Dictionary<int, bool> m_BlackMarketBuy = new Dictionary<int, bool>();
	[JsonIgnore] public Dictionary<ShopResetType, Dictionary<int, bool>> m_PVPStoreBuy = new Dictionary<ShopResetType, Dictionary<int, bool>>();
	[JsonIgnore] public RES_SYSTEM_MSG m_SysMsgInfo = new RES_SYSTEM_MSG();

	/// <summary> 경매장 데이터(이전데이터로 사용) </summary>
	[JsonIgnore] public AuctionInfo m_Auction = new AuctionInfo();
	[JsonIgnore] public List<UserPickCharInfo> m_ClearUserPickInfo = new List<UserPickCharInfo>();

	public MissionInfo m_Mission = new MissionInfo();

	public MyFriendInfo m_Friend = new MyFriendInfo();

	[JsonIgnore] Dictionary<int, RecommendGoodsInfo> m_RcmdInfos = new Dictionary<int, RecommendGoodsInfo>();
	public long m_Mileage;
	/// <summary>
	/// Play 메인에 들어올 때 마다 조건들 체크해서 세팅
	/// </summary>
	public void SetRecommendGoods(ShopAdviceCondition _type) {
		string goods = PlayerPrefs.GetString(string.Format("RECOMMEND_GOODS_{0}_{1}", USERINFO.m_UID, _type), string.Empty);
		if (string.IsNullOrEmpty(goods)) m_RcmdInfos.Clear();
		else m_RcmdInfos = JsonConvert.DeserializeObject<Dictionary<int, RecommendGoodsInfo>>(goods);
		
		if(m_RcmdInfos.Count> 0) {
			for(int i = m_RcmdInfos.Count - 1;i >= 0; i--) {
				RecommendGoodsInfo rcinfo = m_RcmdInfos.ElementAt(i).Value;
				if (rcinfo.m_SSACTData == null ) m_RcmdInfos.Remove(m_RcmdInfos.ElementAt(i).Key);
			}
		}
		
		List<TShopAdviceConditionTable> datas = TDATA.GetCanAdviceTables(_type);
		for (int i = 0;i< datas.Count; i++) {
			int sidx = datas[i].m_GoodsIdx;
			RecommendGoodsInfo rcgi = m_RcmdInfos.ContainsKey(sidx) ? m_RcmdInfos[sidx] : null;
			if (rcgi == null) {
				m_RcmdInfos.Add(sidx, new RecommendGoodsInfo() { m_SIdx = sidx, m_UTime = UTILE.Get_ServerTime() });
				PlayerPrefs.SetString(string.Format("NEW_RECOMMEND_GOODS_{0}", USERINFO.m_UID), sidx.ToString());
				PlayerPrefs.SetInt(string.Format("NEW_STORE_GOODS_{0}_{1}", USERINFO.m_UID, sidx), 1);
			}
		}

		string info = JsonConvert.SerializeObject(m_RcmdInfos);
		PlayerPrefs.SetString(string.Format("RECOMMEND_GOODS_{0}_{1}", USERINFO.m_UID, _type), info);
		PlayerPrefs.Save();
	}
	/// <summary>
	/// 전체 저장된거에서 뽑을 수 있는것만 추려서 빼내기
	/// </summary>
	public List<RecommendGoodsInfo> GetRecommendGoods(ShopAdviceCondition _type) {
		m_RcmdInfos.Clear();
		string data = PlayerPrefs.GetString(string.Format("RECOMMEND_GOODS_{0}_{1}", USERINFO.m_UID, _type), string.Empty);
		if (!string.IsNullOrEmpty(data)) {
			m_RcmdInfos = JsonConvert.DeserializeObject<Dictionary<int, RecommendGoodsInfo>>(data);
		}
		//if (m_RcmdInfos.Count < 1) SetRecommendGoods(_type);
		if (m_RcmdInfos.Count > 0) {
			for (int i = m_RcmdInfos.Count - 1; i >= 0; i--) {
				if (m_RcmdInfos.ElementAt(i).Value.m_SSACTData == null) m_RcmdInfos.Remove(m_RcmdInfos.ElementAt(i).Key);
			}
		}
		List<RecommendGoodsInfo> canlist = m_RcmdInfos.Values.ToList();

		canlist = canlist.FindAll(o => o.IS_CanListUp && (o.m_SSACTData == null ? false : (o.m_SSACTData.m_CloseType == ShopAdviceCloseType.Time ? o.GetRemainTime > 0 : true)));//구매까지 시간 남아있고 구매 가능한것만
		return canlist;
	}
	public bool IsLockCage(int idx) {
		return idx >= CageCnt;
	}

	public bool CanOpenCage() {
		return CageCnt < BaseValue.ZOMBIE_CAGE_MAX;
	}

	public void EnergyCheck()
	{
		m_Energy.SetCheckData(TDATA.GetConfig_Int32(ConfigType.StageEnergyTime) * 1000, BaseValue.MAX_ENERGY + Mathf.RoundToInt(ResearchValue(ResearchEff.BulletMaxUp) + GetSkillValue(SkillKind.BulletMaxUp)));
	}

	public void OpenZombieCageProcess(Action<bool> YesCallback) {
		int openCost = TDATA.GetShopTable(BaseValue.SHOP_IDX_ZOMBIE_CAGE).GetPrice();
		string strTitle = TDATA.GetString(354);
		string strMsg = TDATA.GetString(355);
		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost, strTitle, strMsg, (result, sender) => {
			var button = (EMsgBtn)result;
			switch (button) {
				case EMsgBtn.BTN_YES:
					if (sender.GetComponent<Msg_YN_Cost>().IS_CanBuy) {
						if (CanOpenCage()) {
#if NOT_USE_NET
							//FireBase-Analytics
							MAIN.GoldToothStatistics(GoldToothContentsType.ZombieCageBuy, CageCnt + 1);

							m_CageCnt++;
							GetCash(-openCost);

							YesCallback?.Invoke(true);
							MAIN.Save_UserInfo();
#else
							WEB.SEND_REQ_ZOMBIE_CAGE_OPEN((res) => {
								if (!res.IsSuccess()) {
									WEB.SEND_REQ_ALL_INFO(res2 => { });
									WEB.StartErrorMsg(res.result_code);
									return;
								}

								//FireBase-Analytics
								MAIN.GoldToothStatistics(GoldToothContentsType.ZombieCageBuy, CageCnt + 1);

								YesCallback?.Invoke(true);
							});
#endif
						}
					}
					else {
						YesCallback?.Invoke(false);
						POPUP.StartLackPop(BaseValue.CASH_IDX);
					}
					break;
				case EMsgBtn.BTN_NO:
					YesCallback?.Invoke(false);
					break;
			}
		}, PriceType.Cash, BaseValue.CASH_IDX, openCost, false);
	}
#if NOT_USE_NET
	public UserInfo() {
#if UNITY_EDITOR
		if (!EditorApplication.isPlaying) return;
#endif
		for (StageContentType i = StageContentType.Stage; i < StageContentType.End; i++) {
			Stage info = new Stage();
			m_Stage.Add(i, info);
			info.UID = Utile_Class.GetUniqeID();
			info.Mode = i;
			info.ItemCntInit();
			switch (i) {
				case StageContentType.Stage:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 101, Week = DayOfWeek.Sunday, DeckPos = 0 } },
					{ new StageIdx() { Pos = 1, Idx = 101, Week = DayOfWeek.Sunday, DeckPos = 0 } },
					{ new StageIdx() { Pos = 2, Idx = 101, Week = DayOfWeek.Sunday, DeckPos = 0 } }
				};
					break;
				case StageContentType.Academy:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 5 } },
					{ new StageIdx() { Pos = 1, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 5 } },
					{ new StageIdx() { Pos = 2, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 5 } },
					{ new StageIdx() { Pos = 3, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 5 } }
				};
					break;
				case StageContentType.Bank:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 6 } }
				};
					break;
				case StageContentType.University:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Monday, DeckPos = 7 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Tuesday, DeckPos = 8 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Wednesday, DeckPos = 9 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Thursday, DeckPos = 10 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Friday, DeckPos = 11 } },
					{ new StageIdx() { Pos = 1, Idx = 1, Week = DayOfWeek.Monday, DeckPos = 12 } },
					{ new StageIdx() { Pos = 1, Idx = 1, Week = DayOfWeek.Tuesday, DeckPos = 13 } },
					{ new StageIdx() { Pos = 1, Idx = 1, Week = DayOfWeek.Wednesday, DeckPos = 14 } },
					{ new StageIdx() { Pos = 1, Idx = 1, Week = DayOfWeek.Thursday, DeckPos = 15 } },
					{ new StageIdx() { Pos = 1, Idx = 1, Week = DayOfWeek.Friday, DeckPos = 16 } }
				};
					break;
				case StageContentType.Tower:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 17 } }
				};
					break;
				case StageContentType.Cemetery:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 18 } }
				};
					break;
				case StageContentType.Factory:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 19 } }
				};
					break;
				case StageContentType.Subway:
					info.Idxs = new List<StageIdx>() {
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Sunday, DeckPos = 20 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Monday, DeckPos = 20 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Tuesday, DeckPos = 20 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Wednesday, DeckPos = 20 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Thursday, DeckPos = 20 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Friday, DeckPos = 20 } },
					{ new StageIdx() { Pos = 0, Idx = 1, Week = DayOfWeek.Saturday, DeckPos = 20 } },
				};
					break;
			}
		}
		for (int i = 0; i < m_Deck.Length; i++) m_Deck[i] = new DeckInfo();
		EnergyCheck();
		m_InvenSize = BaseValue.INVEN_BASE_CNT;
		for (int i = 0; i < BaseValue.START_ZOMBIE_SLOT; i++) m_ZombieRoom.Add(new ZombieRoomInfo() { Pos = i });
	}
#else
	public UserInfo()
	{
		m_Collection.Init();
		for (int i = 0; i < m_Deck.Length; i++) m_Deck[i] = new DeckInfo();
		EnergyCheck();
	}
#endif
	public void SetDATA(RES_ALL_INFO data, bool IsDataInit = true) {
		if (IsDataInit) {
			m_Stage.Clear();
			m_Chars.Clear();
			m_Items.Clear();
			m_Tuto.m_TutoState.Clear();
			m_Advs.Clear();
			m_Researchs.Clear();
			m_Making.Clear();
			m_Posts.Clear();

			m_Achieve.Init();
			m_Collection.Init();
			m_CampBuild.Clear();
		}

		SetDATA(data.User);
		SetDATA(data.Researchs);
		SetDATA(data.Collection);
		SetDATA(data.DNAs);
		SetDATA(data.Items);
		SetDATA(data.Chars);
		SetDATA(data.Decks);
		//위에 캐릭터 스탯관련 계산들어가는건 순서 바꾸지 말것 스탯관련-캐릭터-덱은 고정
		SetDATA(data.Stages);
		SetDATA(data.Advs);
		SetDATA(data.Makings);
		SetDATA(data.Zombies);
		SetDATA(data.ZombieRooms);
		SetDATA(data.Posts);
		SetDATA(data.Achieve);
		m_Tuto.SetDATA(data.Tutos);
		SetDATA(data.CampBuilds);
		m_EqItems.Clear();
		Check_AllEquipItem();

		CheckSynergy();
		SetDataBlackMarket();
		SetDataPVPStore(ShopResetType.Season);
		SetDataPVPStore(ShopResetType.DayOfWeek);
		SetDataPVPStore(ShopResetType.ZeroTime);
		SetSysMsgData();
	}

	public void SetDATA(RES_USERINFO data) {
		m_UID = data.UserNo;
		m_Guild.UID = data.GID;
		m_LV = data.LV;
		m_Name = data.Name;
		m_Exp[(int)EXPType.User] = data.Exp[(int)EXPType.User];
		m_Exp[(int)EXPType.Ingame] = data.Exp[(int)EXPType.Ingame];
		m_InvenSize = data.Inven;
		m_ZombieInvenSize = data.ZInven;
		_Cash[0] = data.Cash[0];
		_Cash[1] = data.Cash[1];
		m_Money = data.Money;
		m_Energy.Cnt = data.Energy.Cnt;
		m_Energy.STime = data.Energy.STime;
		m_CageCnt = data.CageCnt;
		m_Profile = data.Profile;
		m_AddEvent = data.AddEventIdx;
		m_ShopEquipGachaExp = data.EQGachaCnt;
		m_PVPRank = data.PVPRank;
		m_PVPpoint = data.PVPpoint;
		m_PVPCoin = data.PVPCoin;
		m_Nation = data.Nation;
		m_GCoin = data.GCoin;
		m_GRTime = data.GRTime;
		m_SupplyBoxLV = data.SupplyBoxLV;
		m_Mileage = data.Mileage;
	}

	public void SetDATA(List<RES_STAGE> data) {
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_STAGE data) {
		if (data == null) return;
		if (!m_Stage.ContainsKey(data.Type)) m_Stage.Add(data.Type, new Stage());
		Stage info = m_Stage[data.Type];
		info.UID = data.UID;
		info.Mode = data.Type;
		info.Idxs = new List<StageIdx>(data.Idxs);
		info.PlayLimit[0] = data.PlayLimit[0];
		info.PlayLimit[1] = data.PlayLimit[1];
		info.LTime = data.LTime;
	}

	public void SetDATA(List<RES_CHARINFO> data) {
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
		USERINFO.GetUserCombatPower();
		EnergyCheck();
	}

	public void SetDATA(RES_CHARINFO data) {
		CharInfo info = m_Chars.Find(o => o.m_UID == data.UID);
		if (info == null) {
			info = new CharInfo();
			info.m_UID = data.UID;
			m_Chars.Add(info);
			info.SetDATA(data);
			// 캐릭터 획득
			m_Achieve.Check_Achieve(AchieveType.Character_Count);
			m_Achieve.Check_AchieveUpDown(AchieveType.Character_Grade_Count, 0, info.m_Grade);
			m_Achieve.Check_AchieveUpDown(AchieveType.Character_Level_Count, 0, info.m_LV);

			m_Collection.Check(CollectionType.Character, info.m_Idx, info.m_Grade);
		}
		else info.SetDATA(data);
		for (int i = 0; i < info.m_EquipUID.Length; i++) AddEquipUID(info.m_EquipUID[i]);
	}

	public void SetDATA(List<RES_ITEMINFO> data) {
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}
	public void SetDATA(RES_ITEMINFO data) {
		if (data.Cnt < 1) {
			DeleteItem(data.UID);
			return;
		}
		ItemInfo info = m_Items.Find(o => o.m_Uid == data.UID);
		if (info == null) {
			info = new ItemInfo();
			info.m_Uid = data.UID;
			
			m_Items.Add(info);
			info.SetDATA(data);
			var tdata = info.m_TData;
			var eqtype = tdata.GetEquipType();
			if (!m_ItemDic.ContainsKey(eqtype)) m_ItemDic.Add(eqtype, new List<ItemInfo>());
			m_ItemDic[eqtype].Add(info);
			// 획득 체크
			if (info.m_TData.GetInvenGroupType() == ItemInvenGroupType.Equipment) {
				m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Level_Count, 0, info.m_Lv);
				m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Grade_Count, 0, info.m_Grade);

				m_Collection.Check(CollectionType.Equip, info.m_Idx, info.m_Grade);
			}
		}
		else info.SetDATA(data);
	}

	public void SetDATA(List<RES_DECKINFO> data) {
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}
	public void SetDATA(RES_DECKINFO data) {
		DeckInfo info = m_Deck[data.Pos];
		if (info == null) info = new DeckInfo(data.UID);
		info.SetDATA(data);
	}

	public void SetDATA(List<RES_ADVINFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}
	public void SetDATA(RES_ADVINFO data) {
		if (data.State == TimeContentState.End) {
			m_Advs.RemoveAll(o => o.m_UID == data.UID);
			return;
		}
		AdventureInfo info = m_Advs.Find(o => o.m_UID == data.UID);
		if (info == null) {
			info = new AdventureInfo();
			info.m_UID = data.UID;
			m_Advs.Add(info);
		}
		info.SetDATA(data);
	}


	public void SetDATA(List<RES_RESEARCHINFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}
	public void SetDATA(RES_RESEARCHINFO data) {
		ResearchInfo info = m_Researchs.Find(o => o.m_UID == data.UID || (o.m_Idx == data.Idx && o.m_Type == data.Type));
		if (info == null) {
			info = new ResearchInfo();
			m_Researchs.Add(info);
		}
		info.SetDATA(data);
		if (info.m_TData.m_Eff.m_Eff == ResearchEff.BulletMaxUp) EnergyCheck();
	}

	public void SetDATA(List<RES_MAKINGINFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_MAKINGINFO data) {
		if (data.State == TimeContentState.End) {
			m_Making.RemoveAll(o => o.m_UID == data.UID);
			return;
		}

		MakingInfo info = m_Making.Find(o => o.m_UID == data.UID);
		if (info == null) {
			info = new MakingInfo();
			m_Making.Add(info);
		}
		info.SetDATA(data);
	}

	public void SetDATA(List<RES_DNAINFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_DNAINFO data) {
		DNAInfo info = m_DNAs.Find(o => o.m_UID == data.UID);
		if (info == null) {
			info = new DNAInfo();
			m_DNAs.Add(info);
			info.SetDATA(data);
		}
		else info.SetDATA(data);
	}
	public void SetDATA(List<RES_ZOMBIEINFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_ZOMBIEINFO data) {
		ZombieInfo info = m_Zombies.Find(o => o.m_UID == data.UID);
		if (info == null) {
			info = new ZombieInfo();
			m_Zombies.Add(info);
		}
		info.SetDATA(data);
	}

	public void SetDATA(List<RES_ZOMBIE_ROOM_INFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_ZOMBIE_ROOM_INFO data) {
		if (data == null) return;
		ZombieRoomInfo info = m_ZombieRoom.Find(o => o.Pos == data.Pos);
		if (info == null)
		{
			info = new ZombieRoomInfo();
			m_ZombieRoom.Add(info);
		}
		info.SetDATA(data);
	}

	public void SetDATA(List<RES_POSTINFO> data) {
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_POSTINFO data) {
		// 모든 보상 지급 완료
		if (data.Items.Find(o => o.State == RewardState.Idle) == null) {
			m_Posts.RemoveAll(o => o.UID == data.UID);
			return;
		}

		PostInfo info = m_Posts.Find(o => o.UID == data.UID);
		if (info == null) {
			info = new PostInfo();
			m_Posts.Add(info);
		}
		info.SetDATA(data);
	}

	public void SetDATA(RES_ACHIEVE_INFO data) {
		m_Achieve.Init(data);
	}

	public void SetDATA(RES_COLLECTION_INFO data) {
		m_Collection.Init(data);
	}
	public void SetDATA(RES_SHOP_INFO data) {
		m_ShopInfo = data;
	}

	public void SetDATA(UserPickCharInfo data)
	{
		m_ClearUserPickInfo.RemoveAll(o => o.Type == data.Type && o.Pos == data.Pos && o.Week == data.Week);
		m_ClearUserPickInfo.Add(data);
	}


	public void SetDATA(List<RES_CAMP_BUILD_INFO> data)
	{
		if (data == null) return;
		for (int i = data.Count - 1; i > -1; i--) SetDATA(data[i]);
	}

	public void SetDATA(RES_CAMP_BUILD_INFO data)
	{
		if (data == null) return;
		if (!m_CampBuild.ContainsKey(data.Build)) m_CampBuild.Add(data.Build, new CampBuildInfo());
		else m_CampBuild[data.Build].SetDATA(data);
	}
	

	/// <summary> 로그인할때 한번 세팅해줌 클라용일때만 따로 호출 </summary>
	public void SetShopData() {
		if (PlayerPrefs.GetString($"SHOP_INFO_{USERINFO.m_UID}", string.Empty).Equals(string.Empty)) {
			m_ShopInfo = new RES_SHOP_INFO();
			string info = JsonConvert.SerializeObject(m_ShopInfo);
			PlayerPrefs.SetString($"SHOP_INFO_{USERINFO.m_UID}", info);
			PlayerPrefs.Save();
		}
		m_ShopInfo = JsonConvert.DeserializeObject<RES_SHOP_INFO>(PlayerPrefs.GetString($"SHOP_INFO_{USERINFO.m_UID}"));
	}
	/// <summary> 클라에서만 쓰임 보급상자 시간 갱신위해 </summary>
	public void SetShopInfo() {
		string info = JsonConvert.SerializeObject(m_ShopInfo);
		PlayerPrefs.SetString($"SHOP_INFO_{USERINFO.m_UID}", info);
		PlayerPrefs.Save();
	}
	/// <summary> 오직 클라에서만 처리라 로그인할때도 호출 </summary>
	public void SetDataBlackMarket() {
		InitBlackMarket();
		m_BlackMarketBuy = JsonConvert.DeserializeObject<Dictionary<int, bool>>(PlayerPrefs.GetString($"SHOP_BLACKMERKET_{USERINFO.m_UID}"));
	}
	/// <summary> 오직 클라에서만 처리라 로그인할 때랑 상품 갱신할 때 사용</summary>
	public void InitBlackMarket(bool _refresh = false) {
		if (PlayerPrefs.GetString($"SHOP_BLACKMERKET_{USERINFO.m_UID}", string.Empty).Equals(string.Empty) || _refresh) {
			m_BlackMarketBuy = new Dictionary<int, bool>();
			string data = JsonConvert.SerializeObject(m_BlackMarketBuy);
			PlayerPrefs.SetString($"SHOP_BLACKMERKET_{USERINFO.m_UID}", data);
			PlayerPrefs.SetString($"SHOP_BLACKMERKET_TIME_{USERINFO.m_UID}", UTILE.Get_ServerTime_Milli().ToString());
			PlayerPrefs.Save();
		}
	}
	/// <summary> 오직 클라에서만 처리라 상품 목록 갱신할떄 호출 </summary>
	public void InsertBlackMarket(int _idx, bool _buy = false) {
		if (!m_BlackMarketBuy.ContainsKey(_idx)) m_BlackMarketBuy.Add(_idx, _buy);
		else m_BlackMarketBuy[_idx] = _buy;

		string data = JsonConvert.SerializeObject(m_BlackMarketBuy);
		PlayerPrefs.SetString($"SHOP_BLACKMERKET_{USERINFO.m_UID}", data);
		PlayerPrefs.Save();
	}/// <summary> 오직 클라에서만 처리라 로그인할때도 호출 </summary>
	public void SetDataPVPStore(ShopResetType _type) {
		InitPVPStore(_type);
		string data = PlayerPrefs.GetString($"SHOP_PVPSTORE_{_type}_{USERINFO.m_UID}");
		m_PVPStoreBuy[_type] = JsonConvert.DeserializeObject<Dictionary<int, bool>>(data);
	}
	/// <summary> 오직 클라에서만 처리라 로그인할 때랑 상품 갱신할 때 사용</summary>
	public void InitPVPStore(ShopResetType _type, bool _refresh = false) {
		if (PlayerPrefs.GetString($"SHOP_PVPSTORE_{_type}_{USERINFO.m_UID}", string.Empty).Equals(string.Empty) || _refresh) {
			if (!m_PVPStoreBuy.ContainsKey(_type)) m_PVPStoreBuy.Add(_type, new Dictionary<int, bool>());
			m_PVPStoreBuy[_type] = new Dictionary<int, bool>();
			string data = JsonConvert.SerializeObject(m_PVPStoreBuy[_type]);
			PlayerPrefs.SetString($"SHOP_PVPSTORE_{_type}_{USERINFO.m_UID}", data);
			PlayerPrefs.SetString($"SHOP_PVPSTORE_TIME_{_type}_{USERINFO.m_UID}", UTILE.Get_ServerTime_Milli().ToString());
			PlayerPrefs.Save();
		}
	}
	/// <summary> 오직 클라에서만 처리라 상품 목록 갱신할떄 호출 </summary>
	public void InsertPVPStore(ShopResetType _type, int _idx, bool _buy = false) {
		if (!m_PVPStoreBuy.ContainsKey(_type)) m_PVPStoreBuy.Add(_type, new Dictionary<int, bool>());
		if (!m_PVPStoreBuy[_type].ContainsKey(_idx)) m_PVPStoreBuy[_type].Add(_idx, _buy);
		m_PVPStoreBuy[_type][_idx] = _buy;

		string data = JsonConvert.SerializeObject(m_PVPStoreBuy[_type]);
		PlayerPrefs.SetString($"SHOP_PVPSTORE_{_type}_{USERINFO.m_UID}", data);
		PlayerPrefs.Save();
	}
	public Dictionary<int, int> InitMileageStore() {
		var infos = USERINFO.m_ShopInfo.GetInfos(ShopGroup.Mileage);
		int maxlv = TDATA.GetGroupShopTable(ShopGroup.Mileage).Max(o => o.m_Level);
		List<TShopTable> tdatas = infos.Select(o => TDATA.GetShopTable(o.Idx)).ToList().FindAll(o => o.m_NoOrProb > 0);//TDATA.GetGroupShopTable(ShopGroup.Mileage).FindAll(o => m_ShopInfos.Find(r => r.Idx == o.m_Idx) != null);//o.m_Level == USERINFO.m_ShopInfo.MileageNo
		Dictionary<int, int> gdata = new Dictionary<int, int>();
		for(int i = 0; i <= maxlv; i++) {
			List<TShopTable> gdatas = tdatas.FindAll(o => o.m_Level == i);
			if (!gdata.ContainsKey(i)) {
				int probsum = gdatas.Sum(o => o.m_NoOrProb);
				int prob = UTILE.Get_Random(0, probsum);
				int preprob = 0;
				for (int j = 0; j < gdatas.Count; j++) {
					preprob += gdatas[j].m_NoOrProb;
					if (preprob >= prob) {
						gdata.Add(i, gdatas[j].m_Idx);
						break;
					}
				}
			}
		}
		string data = JsonConvert.SerializeObject(gdata);
		PlayerPrefs.SetString($"SHOP_MILEAGESTORE_{USERINFO.m_UID}", data);
		PlayerPrefs.Save();

		return gdata;
	}
	public Dictionary<int, int> GetMileageStore() {
		Dictionary<int, int> tdatas = new Dictionary<int, int> ();
		string data = PlayerPrefs.GetString($"SHOP_MILEAGESTORE_{USERINFO.m_UID}");
		if (string.IsNullOrEmpty(data)) tdatas = InitMileageStore();
		else {
			tdatas = JsonConvert.DeserializeObject<Dictionary<int, int>> (data);
			var all = tdatas.Values.ToList();
			var sinfo = USERINFO.m_ShopInfo.GetInfos(ShopGroup.Mileage);
			for(int i = 0; i < tdatas.Count; i++) {
				if(sinfo.Find(o=>o.Idx == all.ElementAt(i)) == null) {
					tdatas = InitMileageStore();
					break;
				}
			}
		}
		
		return tdatas;
	}
	/// <summary> 오직클라에서만 처리라 로그인할때도 호출 </summary>
	public void SetSysMsgData() {
		InitSysMsg();
		m_SysMsgInfo = JsonConvert.DeserializeObject<RES_SYSTEM_MSG>(PlayerPrefs.GetString($"SYSMSG_INFO_{USERINFO.m_UID}"));
	}
	public void InitSysMsg() {
		if (PlayerPrefs.GetString($"SYSMSG_INFO_{USERINFO.m_UID}", string.Empty).Equals(string.Empty)) {
			m_SysMsgInfo = new RES_SYSTEM_MSG();
			string info = JsonConvert.SerializeObject(m_SysMsgInfo);
			PlayerPrefs.SetString($"SYSMSG_INFO_{USERINFO.m_UID}", info);
			PlayerPrefs.Save();
		}
	}
	public void InsertSystemMsgInfo(RES_SYSTEM_MSG _res) {
		m_SysMsgInfo = _res;
		string info = JsonConvert.SerializeObject(m_SysMsgInfo);
		PlayerPrefs.SetString($"SYSMSG_INFO_{USERINFO.m_UID}", info);
		PlayerPrefs.Save();
	}
	public void OutStageCntInit() {
		for (StageContentType i = StageContentType.Bank; i < StageContentType.End; i++) m_Stage[i].ItemCntInit();
	}

	/// <summary> 최근에 한 스테이지(난이도) </summary>
	public int GetDifficulty(bool _onlyview = false) {
		if (_onlyview) {
			return (int)TDATA.GetStageTable(m_Stage[StageContentType.Stage].Idxs[0].Idx).m_DifficultyType;
		}
		else
			return 0;

//		switch (PlayerPrefs.GetInt($"StageDifficulty_{USERINFO.m_UID}")) {
//			case 0: break;
//			case 1:
//#if !STAGE_TEST
//				if (m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.HARD_OPEN) {
//					PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", 0);
//					return 0;
//				}
//#endif
//				break;
//			case 2:
//#if !STAGE_TEST
//				if (m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.NIGHTMARE_OPEN) {
//					if (m_Stage[StageContentType.Stage].Idxs[0].Idx < BaseValue.HARD_OPEN) {
//						PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", 0);
//						return 0;
//					}
//					else {
//						PlayerPrefs.SetInt($"StageDifficulty_{USERINFO.m_UID}", 1);
//						return 1;
//					}
//				}
//#endif
//				break;
//		}
//		PlayerPrefs.Save();
//		return PlayerPrefs.GetInt($"StageDifficulty_{USERINFO.m_UID}");
	}
	public void OutGameClear()//난이도 상승 여기에
	{
		List<TModeTable> tmode = TDATA.GetModeTable(STAGEINFO.m_StageContentType, STAGEINFO.m_Week, STAGEINFO.m_Pos);
		TModeTable crnt = tmode.Find(data => data.m_Difficulty == STAGEINFO.m_LV);
		TModeTable next = tmode.Find(data => data.m_Difficulty == STAGEINFO.m_LV + 1);
		if (next != null) {
			//테이지 => 스테이지 인덱스, 던전 => 레벨, 요일던전 => ((요일 - 1) * 2 + pos
			switch (STAGEINFO.m_StageContentType) //레벨 갱신에 난투, 좀비떼는 마지막 보상 마리까지 잡아야 함<5단계>
			{
				case StageContentType.Bank:
				case StageContentType.Tower:
				case StageContentType.Cemetery:
				case StageContentType.Factory:
					if (m_Stage[STAGEINFO.m_StageContentType].Idxs[0].Clear < STAGEINFO.m_LV) {
						SetMainMenuAlarmVal(MainMenuType.Dungeon);
					}
					m_Stage[STAGEINFO.m_StageContentType].Idxs[0].PlayCount = 1;
					break;
				case StageContentType.Academy:
				case StageContentType.University:
				case StageContentType.Subway:
					StageIdx stg = m_Stage[STAGEINFO.m_StageContentType].Idxs.Find(t => t.Week == STAGEINFO.m_Week && t.Pos == STAGEINFO.m_Pos);
					if (stg.Clear < STAGEINFO.m_LV) {
						SetMainMenuAlarmVal(MainMenuType.Dungeon);
					}
					stg.PlayCount = 1;
					break;
			}
		}
#if NOT_USE_NET
		m_Stage[STAGEINFO.m_StageContentType].UseItem();
		Check_StageClear(m_Stage[STAGEINFO.GetContentType()].Mode, STAGEINFO.m_Pos, STAGEINFO.m_Idx);
#endif
		MAIN.Save_UserInfo();
	}
	/// <summary> 덱 인덱스 설정 </summary>
	public void SetDeckIdx(int _idx) {
		m_SelectDeck = _idx;
	}
	/// <summary> 던전 덱 인덱스 불러오기 </summary>
	public void SetDeckIdx(StageContentType _content, DayOfWeek _day = DayOfWeek.Monday, int _pos = 0) {
		m_SelectDeck = m_Stage[_content].Idxs[_content == StageContentType.University ? ((int)_day - 1) * 2 + _pos : _pos].DeckPos;
	}
	public void SetStageDeckIdx(int _pos) {
		m_Stage[StageContentType.Stage].Idxs[GetDifficulty()].DeckPos = _pos;
	}
	public bool IS_ChangeDeck() {
		for (int i = 0; i < USERINFO.m_Deck.Length; i++) {
			DeckInfo deck = USERINFO.m_Deck[i];
			if (deck.IsChange) return true;
		}
		return false;
	}
	/// <summary> 현재 덱의 시너지 세팅</summary>
	public void CheckSynergy() {
		m_SynergyAll.Clear();
		//시너지 카운팅
		Dictionary<JobType, int> alljob = new Dictionary<JobType, int>();
		for (int i = m_Chars.Count - 1; i > -1; i--) {
			CharInfo info = m_Chars[i];

			List<JobType> jobs = info.m_TData.m_Job;
			for (int j = 0; j < jobs.Count; j++) {
				if (!alljob.ContainsKey(jobs[j])) alljob.Add(jobs[j], 0);
				alljob[jobs[j]]++;
			}
		}

		for (int i = 0; i < alljob.Count; i++) {
			KeyValuePair<JobType, int> data = alljob.ElementAt(i);
			JobType job = data.Key;
			TSynergyTable tsynergy = TDATA.GetSynergyTable(job);
			if (!tsynergy.IS_CanSynergy(data.Value)) continue;
			if (!m_SynergyAll.Contains(job))
				m_SynergyAll.Add(job);
		}

		for (int i = 0; i < m_Deck.Length; i++) m_Deck[i].SetSynergy();
	}

	public CharInfo InsertChar(int _idx, int _grade = 0, int _lv = 1, long uid = 0) {
		CharInfo info = m_Chars.Find(t => t.m_Idx == _idx);
#if NOT_USE_NET
		if (info == null) {
			info = new CharInfo(_idx, uid, _grade, _lv);
			m_Chars.Add(info);
			CheckSynergy();
			SetMainMenuAlarmVal(MainMenuType.Character);
			m_Achieve.Check_Achieve(AchieveType.Character_Count);
			m_Achieve.Check_AchieveUpDown(AchieveType.Character_Grade_Count, 0, info.m_Grade);
			m_Achieve.Check_AchieveUpDown(AchieveType.Character_Level_Count, 0, info.m_LV);

			m_Collection.Check(CollectionType.Character, _idx, _grade);
			MAIN.Save_UserInfo();
		}
#endif
		return info;
	}

	public CharInfo GetChar(long UID) {
		if (TUTO.CheckUseCloneDeck()) {
			return TUTO.m_CloneChars.Find(t => t.m_UID == UID);
		}
		else {
			return m_Chars.Find(t => t.m_UID == UID);
		}
	}
	public CharInfo GetChar(int _idx) {
		if (TUTO.CheckUseCloneDeck()) {
			return TUTO.m_CloneChars.Find(t => t.m_Idx == _idx);
		}
		else {
			return m_Chars.Find(t => t.m_Idx == _idx);
		}
	}

	/// <summary> 현재 덱의 스킬값들 합</summary>
	public float GetSkillValue(SkillKind kind) {
		float value = 0f;

		for (int i = 0; i < m_Chars.Count; i++) {
			value += m_Chars[i].GetSkillValue(kind);
		}
#if STAT_VAL_DEBUG && !ONLY_BATTLE_VAL_DEBUG
		if (value > 0) Utile_Class.DebugLog_Value(kind.ToString() + " 스킬 증가량" + value.ToString());
#endif
		return value;
	}

	public float GetSkillStatValue(StatType type) {
		//스킬연산 추가
		float re = 0f;
		switch (type) {
			case StatType.Men: re += GetSkillValue(SkillKind.MenMax); break;
			case StatType.Hyg: re += GetSkillValue(SkillKind.HygMax); break;
			case StatType.Sat: re += GetSkillValue(SkillKind.SatMax); break;
			case StatType.Atk:
				re += GetSkillValue(SkillKind.AtkUp);
				break;
			case StatType.Def:
				re += GetSkillValue(SkillKind.DefUp);
				break;
			case StatType.HP:
				re += GetSkillValue(SkillKind.TotalHpUp);
				break;
			case StatType.Sta:
				re += GetSkillValue(SkillKind.TotalEnergyUp);
				break;
			case StatType.Heal:
				re += GetSkillValue(SkillKind.HealUp);
				break;
			case StatType.Speed:
				re += GetSkillValue(SkillKind.SpeedUp);
				break;
			case StatType.Critical:
				re += GetSkillValue(SkillKind.CriProbUp);
				break;
			case StatType.CriticalDmg:
				break;
			case StatType.MenDecreaseDef:
				re += GetSkillValue(SkillKind.DefMenUp);
				break;
			case StatType.HygDecreaseDef:
				re += GetSkillValue(SkillKind.DefHygUp);
				break;
			case StatType.SatDecreaseDef:
				re += GetSkillValue(SkillKind.DefSatUp);
				break;
		}

		return re;
	}

	/// <summary> 인게임 경험치 세팅 </summary>
	public long SetIngameExp(long _val, bool IsSkill = false) {
		long value = _val;
		// 스킬은 스테이지 보상만
		if (_val > 0 && IsSkill) {
			float per = 0f;
			per += GetSkillValue(SkillKind.GetExp);
			per += ResearchValue(ResearchEff.ExpUp);
			value += Mathf.RoundToInt(_val * per);
		}
		m_Exp[(int)EXPType.Ingame] = (long)Mathf.Max(m_Exp[(int)EXPType.Ingame] + value, 0);
		return value;
	}
	/// <summary> 계정 경험치 세팅 </summary>
	public void SetUserExp(long _val) {
		long value = _val;
		int MaxLV = BaseValue.CHAR_MAX_LV;
		//기타 공식 적용
		m_Exp[(int)EXPType.User] = (long)Mathf.Max(m_Exp[(int)EXPType.User] + value, 0);
		//레벨업 체크
		TExpTable tdata = TDATA.GetExpTable(m_LV);
		while (tdata != null && m_LV < MaxLV && m_Exp[(int)EXPType.User] >= tdata.m_UserExp) {
			m_Exp[(int)EXPType.User] -= TDATA.GetExpTable(m_LV).m_UserExp;
			m_LV++;
			tdata = TDATA.GetExpTable(m_LV);
		}
	}
	public void GetCash(long _val) {
		long pre = m_Cash;
#if NOT_USE_NET
		m_Cash += _val;
		if (pre < m_Cash) m_Achieve.Check_Achieve(AchieveType.GoldTeeth_Count, 0, m_Cash - pre);
		else if (pre > m_Cash) m_Achieve.Check_Achieve(AchieveType.GoldTeeth_Use, 0, pre - m_Cash);
#endif
		DLGTINFO?.f_RFCashUI?.Invoke(m_Cash, pre);
	}
	public void GetShell(int _val) {
		m_Energy.Cnt += _val;
		DLGTINFO?.f_RFShellUI?.Invoke(m_Energy.Cnt);
		MAIN.Save_UserInfo();
	}
	public ItemInfo GetItem(long UID) {
		if (UID == 0) return null;
		return m_Items.Find(t => t.m_Uid == UID);
	}
	/// <summary> 가방에 있는 아이템 리스트 </summary>
	/// <param name="Mode">0 : 전체</param>
	/// <param name="Mode">1 : 장비</param>
	/// <param name="Mode">2 : 기타</param>
	/// <param name="Mode">3 : 장비 경험치로 사용가능한 상태</param>
	/// <returns></returns>
	public List<ItemInfo> GetBagItems(int Mode = 0, List<long> skip = null)
	{
		switch(Mode)
		{
		case 1: return m_Items.FindAll(o => !IsUseEquipItem(o.m_Uid) && o.m_TData.GetEquipType() != EquipType.End);
		case 2: return m_Items.FindAll(o => !IsUseEquipItem(o.m_Uid) && o.m_TData.GetEquipType() == EquipType.End);
		case 3:
			return m_Items.FindAll(o => {
				if (skip != null && skip.Contains(o.m_Uid)) return false;
				if (IsUseEquipItem(o.m_Uid)) return false;
				if (o.m_TData.m_Type == ItemType.ConsolidationMaterial) return true;
				//if (m_SelectCheck[1] && o.m_TSpecialStat != null) return false;
				return o.m_TData.GetInvenGroupType() == ItemInvenGroupType.Equipment;
			});
		}

		return m_Items.FindAll(o => !IsUseEquipItem(o.m_Uid));
	}

	public int GetItemCount(int _idx) {
		return m_Items.Sum(o => o.m_Idx == _idx ? o.m_Stack : 0);
	}

	/// <summary> 아이템 가방 슬롯 체크 제작때는 인서트 전엔 항상 확인 한다 </summary>
	public bool CheckBagSize() {
		return m_InvenUseSize < m_InvenSize;
	}

	/// <summary> 가방 구매가 가능한 상태인지 확인 </summary>
	public bool IsBuyBagSize()
	{
		return BaseValue.INVEN_SLOT_MAX > m_InvenSize;
	}

	public int GetInvenPrice(int _Cnt = 1) {
		// 지금까지 구매한 개수
		//int now = (m_InvenSize - BaseValue.INVEN_BASE_CNT) / BaseValue.INVEN_BUY_CNT;
		//int after = Math.Min(now + _Cnt, BaseValue.INVEN_SLOT_MAX / BaseValue.INVEN_BUY_CNT);

		return TDATA.GetShopTable(BaseValue.SHOP_IDX_INVEN).GetPrice(_Cnt);
	}

	public void AddInven(int _Cnt = 1) {
		m_InvenSize += _Cnt * BaseValue.INVEN_BUY_CNT;
	}

	public void InsertItem(ItemInfo _item) {
		m_Items.Add(_item);

		PlayerPrefs.SetInt($"InvenNewAlarm_{USERINFO.m_UID}", 1);//인벤토리 알림 킴
		DLGTINFO?.f_RFInvenAlarm?.Invoke(true);

		TItemTable table = TDATA.GetItemTable(_item.m_Idx);
		if (table.GetInvenGroupType() == ItemInvenGroupType.Equipment) {
			PlayerPrefs.SetInt($"InvenNewAlarm_Equip_{USERINFO.m_UID}", 1);//인벤토리 장비 알림 킴
		}
		else {
			if (table.m_Type == ItemType.CharaterPiece)
				PlayerPrefs.SetInt($"InvenNewAlarm_Piece_{USERINFO.m_UID}", 1);
			else
				PlayerPrefs.SetInt($"InvenNewAlarm_ETC_{USERINFO.m_UID}", 1);
		}
	}
	public ItemInfo InsertItem(int _idx, int _cnt = 1) {
		PlayerPrefs.SetInt($"InvenNewAlarm_{USERINFO.m_UID}", 1);//인벤토리 알림 킴
		DLGTINFO?.f_RFInvenAlarm?.Invoke(true);

#if NOT_USE_NET
		ItemInfo info = null;
		TItemTable table = TDATA.GetItemTable(_idx);
		if (table.GetInvenGroupType() == ItemInvenGroupType.Equipment) {
			info = new ItemInfo(_idx);
			m_Items.Add(info);
			m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Grade_Count, 0, info.m_Grade);
			m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Level_Count, 0, info.m_Lv);
			m_Collection.Check(CollectionType.Equip, info.m_Idx, info.m_Grade);
			PlayerPrefs.SetInt($"InvenNewAlarm_Equip_{USERINFO.m_UID}", 1);//인벤토리 장비 알림 킴
		}
		else {
			switch (table.m_Type) {
				case ItemType.Dollar:
					ChangeMoney(_cnt);
					break;
				case ItemType.Cash:
					GetCash(_cnt);
					break;
				case ItemType.Energy:
					GetShell(_cnt);
					break;
				case ItemType.InvenPlus:
					m_InvenSize += _cnt * BaseValue.INVEN_BUY_CNT;
					break;
				default:
					List<ItemInfo> items = m_Items.FindAll(t => t.m_Idx == _idx);
					int Maxcnt = BaseValue.ITEM_MAXCNT;
					for (int i = 0; i < items.Count; i++) {
						info = items[i];
						if (info.m_Stack < Maxcnt) {
							info.m_Stack += _cnt;
							if (info.m_Stack > Maxcnt) {
								_cnt = info.m_Stack - Maxcnt;
								info.m_Stack = Maxcnt;
							}
							else _cnt = 0;
							info.m_GetAlarm = true;
						}
						if (_cnt < 0) break;
					}
					while (_cnt > 0) {
						if (_cnt > Maxcnt) {
							info = new ItemInfo(_idx, 0, 1, Maxcnt);
							_cnt -= Maxcnt;
						}
						else {
							info = new ItemInfo(_idx, 0, 1, _cnt);
							_cnt = 0;
						}
						m_Items.Add(info);
					}
					if (info.m_TData.m_Type == ItemType.CharaterPiece)
						PlayerPrefs.SetInt($"InvenNewAlarm_Piece_{USERINFO.m_UID}", 1);
					else
						PlayerPrefs.SetInt($"InvenNewAlarm_ETC_{USERINFO.m_UID}", 1);
					break;
			}
		}
#else
		ItemInfo info = m_Items.Find(o => o.m_Idx == _idx);
#endif
		return info;
	}

	public void DeleteItem(long uid) {
		var info = m_Items.Find(o => o.m_Uid == uid);
		var tdata = info.m_TData;
		var eqtype = tdata.GetEquipType();
		if (m_ItemDic.ContainsKey(eqtype)) m_ItemDic[eqtype].RemoveAll(t => t.m_Uid == uid);
		m_Items.RemoveAll(t => t.m_Uid == uid);
		m_EqItems.RemoveAll(t => t == uid);

	}

	public void DeleteItem(long _uid, int _cnt = 1) {
		ItemInfo info = m_Items.Find(t => t.m_Uid == _uid);
		if (info != null) {
			if (TDATA.GetItemTable(info.m_Idx).GetInvenGroupType() != ItemInvenGroupType.Equipment) {
				if (info.m_Stack > _cnt) info.m_Stack -= _cnt;
				else m_Items.Remove(info);
			}
			else m_Items.Remove(info);
			MAIN.Save_UserInfo();
		}
	}

	public void DeleteItem(int _idx, int _cnt) {
		if (TDATA.GetItemTable(_idx).GetInvenGroupType() != ItemInvenGroupType.Equipment) {
			List<ItemInfo> list = m_Items.FindAll(t => t.m_Idx == _idx);
			for (int i = 0; i < list.Count; i++) {
				ItemInfo info = list[i];
				if (info != null) {
					if (info.m_Stack <= _cnt) {
						m_Items.Remove(info);
						_cnt -= info.m_Stack;
					}
					else {
						info.m_Stack -= _cnt;
						_cnt = 0;
					}
				}
				if (_cnt < 1) break;
			}
		}
		else {
			List<ItemInfo> infos = m_Items.FindAll(t => t.m_Idx == _idx);
			for (int i = 0; i < infos.Count; i++) {
				m_Items.Remove(infos[i]);
				_cnt--;
				if (_cnt < 1) break;
			}
		}
		MAIN.Save_UserInfo();
	}

	public long ChangeMoney(long _val, bool SkillCheck = false) {
		long pre = m_Money;
		long value = _val;
		if (_val > 0 && SkillCheck) {
			float per = GetSkillValue(SkillKind.GetDoller) + ResearchValue(ResearchEff.DollarUp);
			value += Mathf.RoundToInt(_val * per);
		}

		m_Money = (long)Mathf.Max(m_Money + value, 0);

#if NOT_USE_NET
		if (pre < m_Money) m_Achieve.Check_Achieve(AchieveType.Dollar_Count, 0, m_Money - pre);
		else if (pre > m_Money) m_Achieve.Check_Achieve(AchieveType.Dollar_Use, 0, pre - m_Money);
#endif
		MAIN.Save_UserInfo();
		DLGTINFO?.f_RFMoneyUI?.Invoke(m_Money, pre);
		return value;
	}

	public MakingInfo InsertMake(int _idx, int _cnt) {
		MakingInfo makeinfo = new MakingInfo(_idx, _cnt);
		m_Making.Add(makeinfo);
		MAIN.Save_UserInfo();
		return makeinfo;
	}
	public void DeleteMake(MakingInfo _info) {
		MakingInfo info = m_Making.Find(t => t.m_UID == _info.m_UID);
		if (info != null) {
			m_Making.Remove(info);
			MAIN.Save_UserInfo();
		}
	}
	public bool IsCompMaking(MakingGroup _group = MakingGroup.None) {
		if (_group != MakingGroup.None) {
			return m_Making.FindAll((t) => t.m_TData.m_Group == _group && t.IS_Complete()).Count > 0;
		}
		return m_Making.FindAll((t) => t.IS_Complete()).Count > 0;
	}

	public bool IsCompZombieFarm() {
		return m_NotCageZombie.Count > 0;
	}

	public void Check_AdvList() {
#if NOT_USE_NET
		int CTime = PlayerPrefs.GetInt($"AdvCheckTime_{USERINFO.m_UID}", 0);
		int NTime = (int)UTILE.Get_Time_Day();
		if (CTime != NTime) {
			int BCnt = TDATA.GetConfig_Int32(ConfigType.AdventureBaseCount) + Mathf.RoundToInt(ResearchValue(ResearchEff.AdventureCountUp));
			int MCnt = BCnt * TDATA.GetConfig_Int32(ConfigType.AdventureWaitingRatio);
			int NCnt = m_Advs.Count;
			int CCnt = Math.Min(BCnt + NCnt, MCnt);
			// 유저 연구에맞춰 레벨 변경
			int LV = 1;
			// 속도를 올리기 위해서는 전부 받아서 여기서 레벨 체크 및 totalprob 계산을 해주면 된다.
			var tdatas = TDATA.GetAdventureTables(LV);
			tdatas.RemoveAll(o => m_Advs.Any(a => a.m_Idx == o.m_Idx));
			int totalprob = tdatas.Sum(o => o.m_Prob);

			for (int i = NCnt; i < CCnt; i++) {
				int Rand = UTILE.Get_Random(0, totalprob);
				var tadvdata = TDATA.GetAdventureTables(tdatas, Rand);

				// 다음을위해 리스트에서 제거
				tdatas.Remove(tadvdata);
				totalprob -= tadvdata.m_Prob;

				m_Advs.Add(new AdventureInfo(tadvdata.m_Idx));
			}

			PlayerPrefs.SetInt($"AdvCheckTime_{USERINFO.m_UID}", NTime);
			PlayerPrefs.Save();
		}
#else
		WEB.SEND_REQ_ADVINFO((res) =>
		{
			if (!res.IsSuccess()) return;
		});
#endif
	}
	public void Reset_AdvList() {
		var list = m_Advs.FindAll(o => o.m_State == TimeContentState.Idle);
		int LV = 1;
		var tdatas = TDATA.GetAdventureTables(LV);
		tdatas.RemoveAll(o => m_Advs.Any(a => a.m_State == TimeContentState.Play && a.m_Idx == o.m_Idx));
		int totalprob = tdatas.Sum(o => o.m_Prob);
		for (int i = list.Count - 1; i > -1; i--) {
			var info = list[i];
			int Rand = UTILE.Get_Random(0, totalprob);
			var tadvdata = TDATA.GetAdventureTables(tdatas, Rand);

			// 다음을위해 리스트에서 제거
			tdatas.Remove(tadvdata);
			totalprob -= tadvdata.m_Prob;

			info.m_Idx = tadvdata.m_Idx;
		}
	}

	public List<long> GetAdventureChars() {
		return m_Advs.SelectMany(x => x.m_Chars).ToList();
	}
	public bool IsCompAdventuring() {
		return m_Advs.Find((t) => t.IS_Complete()) != null;
	}
	/// <summary> 스테이지 단계에 따른 컨텐츠 락 체크 </summary>
	public bool CheckContentUnLock(ContentType _type, bool _usealarm = false) {
		if (PlayerPrefs.GetInt($"ContentUnlock_{USERINFO.m_UID}") == 1)
			return true;
		else {
			if (_type == ContentType.Serum) {
				TSerumBlockTable tdata = TDATA.GetSerumBlockTable(1);
				bool can = USERINFO.m_Chars.Find(o => o.m_LV >= tdata.m_NeedCharLv) != null;
				if (_usealarm && !can)
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(10810));
				return can;
			}
			else {
				var idx = BaseValue.CONTENT_OPEN_IDX(_type);
				var diff = (int)StageDifficultyType.Normal;
				if (_type == ContentType.Replay) diff = USERINFO.GetDifficulty();
				bool IsOpen = m_Stage[StageContentType.Stage].Idxs[diff].Idx >= idx;
				if (!IsOpen && _usealarm) {
					TStageTable tdata = TDATA.GetStageTable(idx);
					POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), idx / 100, idx % 100, tdata.GetName()));
					//POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(163), BaseValue.CONTENT_OPEN_IDX(_type) / 100, BaseValue.CONTENT_OPEN_IDX(_type) % 100));
				}
				return IsOpen;
			}
		}
	}

	//////////////////////////////////////////////////////////////////////////////////////////////////
	// 장비 장착 관련
	public void Check_AllEquipItem() {
		m_EqItems.Clear();
		m_EqDNAs.Clear();
		for (int i = m_Chars.Count - 1; i > -1; i--) {
			for (int j = 0; j < m_Chars[i].m_EquipUID.Length; j++) AddEquipUID(m_Chars[i].m_EquipUID[j]);
			for (int j = 0; j < m_Chars[i].m_EqDNAUID.Length; j++) AddEqDNAUID(m_Chars[i].m_EqDNAUID[j]);
		}
	}

	public bool IsUseEquipItem(long uid) {
		return m_EqItems.Contains(uid);
	}

	public void AddEquipUID(long uid) {
		if (uid < 1) return;
		if (m_EqItems.Contains(uid)) return;
		m_EqItems.Add(uid);
	}
	public void AddEqDNAUID(long uid)
	{
		if (uid < 1) return;
		if (m_EqDNAs.Contains(uid)) return;
		m_EqDNAs.Add(uid);
	}

	public CharInfo GetEquipChar(long uid) {
		return m_Chars.Find(o => o.m_EquipUID.Contains(uid));
	}
	public void RemoveEquipUID(long uid) {
		CharInfo info = GetEquipChar(uid);
		for (int i = 0; i < info.m_EquipUID.Length; i++) {
			if (info.m_EquipUID[i] == uid) info.m_EquipUID[i] = 0;
		}
		m_EqItems.Remove(uid);
	}

	public void RemoveEquipUID(ItemInfo item) {
		CharInfo info = GetEquipChar(item.m_Uid);
		info.m_EquipUID[(int)item.m_TData.GetEquipType()] = 0;
		m_EqItems.Remove(item.m_Uid);
	}

	public ResearchInfo GetResearchInfo(ResearchType type, int idx) {
		ResearchInfo info = m_Researchs.Find(o => o.m_Type == type && o.m_Idx == idx);
		if (info == null) {
			info = new ResearchInfo() {
				m_Type = type,
				m_Idx = idx,
				m_LV = 0
			};
			m_Researchs.Add(info);
		}

		return info;
	}

	public List<ResearchInfo> ResearchInfos(ResearchType type) {
		return m_Researchs.FindAll(o => o.m_Type == type && o.m_GetLv > 0);
	}

	public float ResearchValue(ResearchEff type, bool _pvp = false) {
		List<ResearchInfo> infos = m_Researchs.FindAll(o => o.m_TData.m_Eff.m_Eff == type && o.m_GetLv > 0);
		if (infos == null) return 0;
		float re = 0;
		if (infos.Count > 0) re += infos.Sum(o => o.m_TData.m_Eff.m_Value);
		else re = TDATA.GetResearch_ZeroLV_Value(type);

		switch (type)
		{
		case ResearchEff.MakingOpen:
		case ResearchEff.BulletMaxUp:
		case ResearchEff.AdventureOpen:
		case ResearchEff.TrainingOpen:
		case ResearchEff.AdventureCountUp:
		case ResearchEff.AdventureLevelUp:
		case ResearchEff.RemodelingOpen:
		case ResearchEff.GuardMaxUp:
		case ResearchEff.MakingLevelUp:
		case ResearchEff.SupplyBoxGradeUp:
		case ResearchEff.MemberMaxUp:
		case ResearchEff.PVPJunkCountDown:
		case ResearchEff.PVPCultivateCountDown:
		case ResearchEff.PVPChemicalCountDown:
			break;
		default: re *= 0.0001f; break;
		}

		float gre = 0;

		List<TGuild_ResearchTable> gres = new List<TGuild_ResearchTable>();
		for (int i = 0; i < m_Guild.EndRes.Count; i++)
		{
			TGuild_ResearchTable tdata = TDATA.GetGuildRes(m_Guild.EndRes[i]);
			//if (!_pvp) {
			//	switch (tdata.m_Eff.m_Eff) {
			//		case ResearchEff.pvp:continue;
			//PVPAtkUp,
			///// <summary> PVP 캐릭터 속도 증가 </summary>
			//PVPSpeedUp,
			///// <summary> PVP 캐릭터 명중률 증가 </summary>
			//PVPHitUp,
			///// <summary> PVP 캐릭터 방어력 증가 </summary>
			//PVPDefUP,
			///// <summary> PVP 캐릭터 최대 체력 증가 </summary>
			//PVPHpUP,
			///// <summary> PVP 정신력 감소 시 % 방어 </summary>
			//PVPPerDefMenUP,
			///// <summary> PVP 포만도 감소 시 % 방어 </summary>
			//PVPPerDefSatUP,
			///// <summary> PVP 청결도 감소 시 % 방어 </summary>
			//PVPPerDefHygUP,
			//	}
			//}
			if (tdata.m_Eff.m_Eff == type) gre += tdata.m_Eff.m_Value;
		}
		return re + gre;
	}

	public bool IsCompResearching(ResearchType type = ResearchType.End) {
		if (type == ResearchType.End) return m_Researchs.Find(o => o.m_State == TimeContentState.Play && o.GetRemainTime() <= 0) != null;
		return m_Researchs.Find(o => o.m_Type == type && o.m_State == TimeContentState.Play && o.GetRemainTime() <= 0) != null;
	}
	public ResearchInfo IsResearching(ResearchType type = ResearchType.End) {
		if (type == ResearchType.End) return m_Researchs.Find(o => o.m_State == TimeContentState.Play);
		return m_Researchs.Find(o => o.m_Type == type && o.m_State == TimeContentState.Play);
	}


	public void SetMainMenuAlarmVal(MainMenuType _type, int _val = 1) {
		PlayerPrefs.SetInt(string.Format("MainMenuBtnAlarm_{0}_{1}", _type.ToString(), USERINFO.m_UID), _val);
		PlayerPrefs.Save();
		DLGTINFO?.f_RFMainMenuAlarm?.Invoke();
	}

	public ZombieInfo GetZombie(long UID) {
		if (UID == 0) return null;
		return m_Zombies.Find(z => z.m_UID == UID);
	}

	public ZombieInfo InsertZombie(int _idx) {
		ZombieInfo info = null;
		info = new ZombieInfo(_idx);
		m_Zombies.Add(info);
		return info;
	}

	public void DeleteZombie(long UID) {
		if (UID == 0) return;
		ZombieInfo zombieInfo = m_Zombies.Find(z => z.m_UID == UID);
		if (zombieInfo != null) {
			DeleteZombie(zombieInfo);
		}
	}

	public void DeleteZombie(ZombieInfo info) {
		if (info == null) return;
		if (m_Zombies.Contains(info)) {
			m_Zombies.Remove(info);
		}

		foreach (var uid in info.m_DnaList) {
			DeleteDNA(uid);
		}
	}

	public DNAInfo GetDNA(long UID) {
		if (UID == 0) return null;
		return m_DNAs.Find(d => d.m_UID == UID);
	}

	public DNAInfo InsertDNA(int idx, int lv = 1) {
		DNAInfo info = new DNAInfo(idx, lv);
		m_DNAs.Add(info);
		m_Achieve.Check_Achieve(AchieveType.DNA_Grade_Count, info.m_TData.m_Grade);
		USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.DNA_LevelUp_Count, 0, info.m_Lv);
		info.m_GetAlarm = true;
		PlayerPrefs.SetInt($"InvenNewAlarm_DNA_{USERINFO.m_UID}", 1);
		return info;
	}

	public void DeleteDNA(long UID) {
		if (UID == 0) return;
		DNAInfo dnaInfo = m_DNAs.Find(d => d.m_UID == UID);
		if (dnaInfo != null) {
			DeleteDNA(dnaInfo);
		}
	}

	public void DeleteDNA(DNAInfo info) {
		if (info == null) return;
		if (m_DNAs.Contains(info)) {
			m_DNAs.Remove(info);
		}
	}
	public bool IS_CanMakeAnyDNA() {
		bool can = false;
		List<TMakingTable> dnas = TDATA.GetGroupMakingTable(MakingGroup.DNA);
		for(int i = 0; i < dnas.Count; i++) {
			if (dnas[i].GetCanMake()) {
				can = true;
				break;
			}
		}
		return can;
	}
	public float GetAllSerum(StatType _type, StatValType _valtype, List<CharInfo> _ignore = null) {
		float val = 0;
		for (int i = 0; i < m_Chars.Count; i++) {
			if (_ignore != null) {
				if (_ignore.Contains(m_Chars[i])) continue;
			}
			for (int j = 0; j < m_Chars[i].m_Serum.Count; j++) {
				TSerumTable serum = TDATA.GetSerumTable(m_Chars[i].m_Serum[j]);
				if (serum == null) continue;
				if (serum.m_TargetType == SerumTargetType.All && serum.m_ValType == _valtype && serum.m_Type == _type) {
					val += serum.m_Val;
				}
			}
		}
		return val;
	}
	public void ShowCharInfo(int Idx, List<CharInfo> _list, System.Action<int, GameObject> cb = null, bool _pvp = false) {
		if (m_Chars.Any(o => o.m_Idx == Idx)) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char, cb, Idx, _list, _pvp ? 1 : 0);
		}
		else {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_NotGet, cb, Idx, Info_Character_NotGet.State.Normal);
		}
	}

	/// <summary> 더미 닉네임 중복체크해서 반환 </summary> 
	public int GetActivityDummyName() {
		string dummydata = PlayerPrefs.GetString($"USERACTIVE_DUMMY_{USERINFO.m_UID}", string.Empty);
		if (dummydata.Equals(string.Empty)) {
			m_UserActivityDummyName = new Dictionary<int, int>();
			dummydata = JsonConvert.SerializeObject(m_UserActivityDummyName);
		}
		m_UserActivityDummyName = JsonConvert.DeserializeObject<Dictionary<int, int>>(dummydata);


		int idx = 0;
		while (idx == 0) {
			idx = UTILE.Get_Random(1000001001, 1000001210);
			if (!m_UserActivityDummyName.ContainsKey(idx)) {
				m_UserActivityDummyName.Add(idx, 0);
				break;
			}
			else idx = 0;
		}
		for (int i = m_UserActivityDummyName.Count - 1; i > -1; i--) {
			KeyValuePair<int, int> data = m_UserActivityDummyName.ElementAt(i);
			if (data.Value > 4) m_UserActivityDummyName.Remove(data.Key);
			else m_UserActivityDummyName[data.Key]++;
		}

		PlayerPrefs.SetString($"USERACTIVE_DUMMY_{USERINFO.m_UID}", JsonConvert.SerializeObject(m_UserActivityDummyName));
		PlayerPrefs.Save();
		return idx;
	}

	public Dictionary<EquipType, List<ItemInfo>> GetUnEquipCPSort(CharInfo _char, bool _setpriority) {
		List<ItemInfo> iteminfos = m_Items.FindAll(t => {
			var tdata = t.m_TData;
			if (tdata.GetEquipType() == EquipType.End) return false;
			if (USERINFO.IsUseEquipItem(t.m_Uid)) return false;
			return tdata.m_Value == 0 || tdata.m_Value == _char.m_Idx;
		});
		//for (int i = 0; i < iteminfos.Count; i++) iteminfos[i].GetCombatPower();
		iteminfos.Sort((before, after) => {
			if (_setpriority) {
				bool beset = before.m_TData.m_Value == _char.m_Idx;
				bool afset = after.m_TData.m_Value == _char.m_Idx;
				if (beset != afset) return afset.CompareTo(beset);
			}
			int bpower = before.m_CP;
			int apower = after.m_CP;
			if (bpower != apower) return apower.CompareTo(bpower);
			return before.m_Idx.CompareTo(after.m_Idx);
		});
		Dictionary<EquipType, List<ItemInfo>> equipinfos = iteminfos.GroupBy(o => o.m_TData.GetEquipType()).ToDictionary(o => o.Key, o => o.ToList());
		for (EquipType i = 0; i < EquipType.Max; i++)
		{
			if (!equipinfos.ContainsKey(i)) equipinfos.Add(i, new List<ItemInfo>());
		}

		return equipinfos;

		//Dictionary<EquipType, List<ItemInfo>> equipinfos = new Dictionary<EquipType, List<ItemInfo>>();


		//List<ItemInfo> Weaponinfos = iteminfos.FindAll(t => t.m_TData.GetEquipType() == EquipType.Weapon && !USERINFO.IsUseEquipItem(t.m_Uid) && (t.m_TData.m_Value == 0 || t.m_TData.m_Value ==_char.m_Idx));
		//List<ItemInfo> Helmetinfos = iteminfos.FindAll(t => t.m_TData.GetEquipType() == EquipType.Helmet && !USERINFO.IsUseEquipItem(t.m_Uid) && (t.m_TData.m_Value == 0 || t.m_TData.m_Value == _char.m_Idx));
		//List<ItemInfo> Costumeinfos = iteminfos.FindAll(t => t.m_TData.GetEquipType() == EquipType.Costume && !USERINFO.IsUseEquipItem(t.m_Uid) && (t.m_TData.m_Value == 0 || t.m_TData.m_Value == _char.m_Idx));
		//List<ItemInfo> Shoesinfos = iteminfos.FindAll(t => t.m_TData.GetEquipType() == EquipType.Shoes && !USERINFO.IsUseEquipItem(t.m_Uid) && (t.m_TData.m_Value == 0 || t.m_TData.m_Value == _char.m_Idx));
		//List<ItemInfo> Accessoryinfos = iteminfos.FindAll(t => t.m_TData.GetEquipType() == EquipType.Accessory && !USERINFO.IsUseEquipItem(t.m_Uid) && (t.m_TData.m_Value == 0 || t.m_TData.m_Value == _char.m_Idx));

		//equipinfos.Add(EquipType.Weapon, Weaponinfos.ConvertAll(t => t));
		//equipinfos.Add(EquipType.Helmet, Helmetinfos.ConvertAll(t => t));
		//equipinfos.Add(EquipType.Costume, Costumeinfos.ConvertAll(t => t));
		//equipinfos.Add(EquipType.Shoes, Shoesinfos.ConvertAll(t => t));
		//equipinfos.Add(EquipType.Accessory, Accessoryinfos.ConvertAll(t => t));

		//for (EquipType i = EquipType.Weapon; i < EquipType.Max; i++) {
		//	equipinfos[i].Sort((before, after) => {
		//		if (_setpriority) {
		//			bool beset = before.m_TData.m_Value == _char.m_Idx;
		//			bool afset = after.m_TData.m_Value == _char.m_Idx;
		//			if (beset != afset) return afset.CompareTo(beset);
		//		}
		//		return after.GetCombatPower().CompareTo(before.GetCombatPower());
		//	});
		//}
		//return equipinfos;
	}
	/// <summary> 유저 프로필 성별 체크 </summary>
	public GenderType GetGender() {
		return TDATA.GetGender(m_Profile);
	}
	/// <summary> 보유 캐릭터 전체 전투력 총합 </summary>
	public long GetUserCombatPower(bool _precal = false) {
		long cp = 0;
		for (int i = 0; i < m_Chars.Count; i++) {
			cp += _precal? m_Chars[i].m_CP : m_Chars[i].GetCombatPower();
		}
		return cp;
	}

	public ShopEquipGachaLvExp GetEquipGachaLv() {
		ShopEquipGachaLvExp data = new ShopEquipGachaLvExp();
		List<TEquipGachaTable> tables = TDATA.GetEquipGachaTableList();
		long exp = m_ShopEquipGachaExp;
		int lv = 1;
		for (int i = 0; i < tables.Count; i++) {
			if (tables[i].m_Exp == 0) break;
			if (exp >= tables[i].m_Exp) {
				lv++;
				exp -= tables[i].m_Exp;
			}
			else break;
		}
		data.Lv = lv;
		data.Exp = exp;
		return data;
	}
	public float GetEquipGachaLvBonus(EquipType _type) {
		float val = 0f;
		for (int i = 1;i <= GetEquipGachaLv().GetLv; i++) {
			TEquipGachaTable tdata = TDATA.GetEquipGachaTable(i);
			switch (_type) {
				case EquipType.Weapon:
					if (tdata.m_EffectType == EquipGachaEffectType.WeaponStatUp) val += tdata.m_EffectVal;
					break;
				case EquipType.Helmet:
					if (tdata.m_EffectType == EquipGachaEffectType.HelmetStatUp) val += tdata.m_EffectVal;
					break;
				case EquipType.Costume:
					if (tdata.m_EffectType == EquipGachaEffectType.CostumeStatUp) val += tdata.m_EffectVal;
					break;
				case EquipType.Shoes:
					if (tdata.m_EffectType == EquipGachaEffectType.ShoesStatUp) val += tdata.m_EffectVal;
					break;
				case EquipType.Accessory:
					if (tdata.m_EffectType == EquipGachaEffectType.AccStatUp) val += tdata.m_EffectVal;
					break;
			}
		}
		return val;
	}
	public void SetMission(MissionData _data) {
		m_Mission.SetData(_data);
	}
	/// <summary> 일일퀘스트 갱신 체크(하루지나면 삭제) </summary>
	public void DailyQuestTimeCheck() {
#if NOT_USE_NET
		if (m_Mission == null) return;
		List<MissionData> missions = new List<MissionData>();
		missions.AddRange(m_Mission.Get_Missions(MissionMode.DailyQuest));
		missions.AddRange(m_Mission.Get_Missions(MissionMode.Day));
		for (int i = missions.Count - 1; i >= 0; i--) {
			if (UTILE.IsSameDay(missions[i].UTime)) continue;
			m_Mission.DelData(missions[i]);
		}
#endif
	}
	/// <summary> 
	/// StageClear : val1 = StageContentType, val2 = 난이도
	/// Making : val1 = 0:아무생산, 1:일반장비, 2:전용장비, 3:연구재료, 4:생산재료
	/// </summary>
	public void Check_Mission(MissionType type, int value1, int value2, int cnt = 1, long EUID = 0) {
		m_Mission.Check_Mission(type, value1, value2, cnt, EUID);
		DLGTINFO?.f_RFMissionAlarm?.Invoke();
		DLGTINFO.f_RFGuidQuestUI?.Invoke(GuidQuestInfo.InfoType.Mission);
	}

	public void Check_MissionUpDown(MissionType type, int min, int max, int cnt = 1, long EUID = 0)
	{
		m_Mission.Check_MissionUpDown(type, min, max, cnt, EUID);
		DLGTINFO?.f_RFMissionAlarm?.Invoke();
		DLGTINFO.f_RFGuidQuestUI?.Invoke(GuidQuestInfo.InfoType.Mission);
	}
	public List<MissionData> GetNowStepNewMission()
	{
		var missiongorup = m_Mission.GetBeginnerQuest();

		foreach (var missions in missiongorup)
		{
			var clearmission = missions.Value.Find(o => o.m_TData.m_Check.Find(o => o.m_Type == MissionType.BeginnerQuestClear) != null);
			if (clearmission.IS_End()) continue;
			// 아직 오픈되지 않은 상태
			if (clearmission.STime > UTILE.Get_ServerTime_Milli()) return null;
			return missions.Value;
		}

		return null;
	}

	public bool CheckSuccNowStepNewMission() {
		bool succ = false;
		List<MissionData> allmission = USERINFO.m_Mission.Get_Missions(MissionMode.BeginnerQuest);
		List<MissionData> spmission = allmission.FindAll(o => o.m_TData.m_Check.Find(c => c.m_Type == MissionType.BeginnerQuestClear) != null);
		//단계별로 전부 클리어했는지 체크
		for (int i = 0; i < spmission.Count; i++) {
			List<MissionData> stepmission = allmission.FindAll(o => o.m_TData.m_ModeGid == i + 1);
			int clearcnt = stepmission.FindAll(o => o.IS_Complete() && o.State[0] == RewardState.Get).Count;
			if (stepmission.Count != clearcnt){
				succ = stepmission.Find(o => o.IS_Complete() && o.State[0] == RewardState.Idle) != null;
				break;
			}
		}

		return succ;
	}

	/// <summary> 클라용 체크 </summary>
	public void Check_StageClear(StageContentType Mode, int pos, int Cnt = 1) {
		USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.All, 0, Cnt);
		switch (Mode) {
			case StageContentType.Stage:
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, pos, Cnt);
				break;
			case StageContentType.Bank:
				USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
				break;
			case StageContentType.Academy:
				USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
				break;
			case StageContentType.University:
				USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
				break;
			case StageContentType.Tower:
				USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
				break;
			case StageContentType.Cemetery:
				USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
				break;
			case StageContentType.Factory:
				USERINFO.Check_Mission(MissionType.StageClear, (int)StageContentType.DownTown, 0, Cnt);
				USERINFO.Check_Mission(MissionType.StageClear, (int)Mode, 0, Cnt);
				break;
		}
	}
	public void ITEM_BUY(int Idx, int Cnt, Action<RES_BASE> CB, bool viewPoup = false, string _title = null, string _msg = null, bool _buy = true, List<int> _pickup = null) {
#if BLOCK_BUY
		TShopTable tdata = TDATA.GetShopTable(Idx);
		if (tdata.m_Group == ShopGroup.Cash || tdata.m_Group == ShopGroup.Pass || tdata.m_Group == ShopGroup.Package || tdata.m_Group == ShopGroup.DailyPack) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(436));
			return;
		}
#endif
		var tshop = TDATA.GetShopTable(Idx);
		if(viewPoup && tshop.m_Group == ShopGroup.Pass)
		{
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Store_PassBuy, (result, obj) => {
				if (result == 1)
				{
					SEND_ITEM_BUY(Idx, Cnt, CB);
				}
				else CB?.Invoke(new RES_BASE() { result_code = EResultCode.ERROR_SHOP_BUY_CANCEL });//CB?.Invoke(null);
			}, true);
		}
		else
		{
			int msg = 0;
			bool Is_Buy = true;
			PopupName popup = PopupName.Msg_YN_Cost;
			switch (tshop.m_PriceType)
			{
			case PriceType.Money: 
				msg = 123;
				break;
			case PriceType.Cash: 
				msg = 122;
				break;
			case PriceType.PVPCoin:
				msg = 7014;
				break;
			case PriceType.GuildCoin:
				msg = 6203;
				break;
			case PriceType.Mileage:
				msg = 10867;
				break;
			default: popup = PopupName.NONE; break;
			}
			switch (Idx) {
				case BaseValue.PVP_TICKET_SHOP_IDX: Is_Buy = false; break;
				default: Is_Buy = _buy; break;
			}
			if (viewPoup && popup != PopupName.NONE)
			{
				POPUP.Set_MsgBox(popup, _title != null ? _title : string.Empty, _msg != null ? _msg : TDATA.GetString(msg), (result, obj) => {
					if (result == 1)
					{
						if (Is_Buy) SEND_ITEM_BUY(Idx, Cnt, CB, _pickup);
						else CB?.Invoke(new RES_BASE() { result_code = EResultCode.SUCCESS });
					}
				}, tshop.m_PriceType, tshop.m_PriceIdx, tshop.GetPrice(Cnt));
			}
			else
			{
				if (Is_Buy) SEND_ITEM_BUY(Idx, Cnt, CB, _pickup);
				else CB?.Invoke(new RES_BASE() { result_code = EResultCode.SUCCESS });
			}
		}
	}
	public IEnumerator PassVIPOpenAction()
	{
		var Pass = m_ShopInfo.PassInfo[0];
		bool end = false;
		POPUP.Set_MsgBox(PopupName.Msg_Store_VIPOpen, (btn, obj) => { end = true; });
		yield return new WaitWhile(() => !end);
	}

	void SEND_ITEM_BUY(int Idx, int Cnt, Action<RES_BASE> CB, List<int> _pickup = null)
	{
#if NOT_USE_NET
		var tdata = TDATA.GetShopTable(Idx);
		switch (tdata.m_Group)
		{
		case ShopGroup.BlackMarket:
			USERINFO.Check_Mission(MissionType.BlackMarket, 0, 0, Cnt);
			break;
		case ShopGroup.Gacha:
			USERINFO.Check_Mission(MissionType.CharGacha, 0, 0, (tdata.m_Rewards[0].m_ItemCnt + tdata.m_Rewards[1].m_ItemCnt) * Cnt);
			break;
		case ShopGroup.ItemGacha:
			USERINFO.Check_Mission(MissionType.ItemGacha, 0, 0, (tdata.m_Rewards[0].m_ItemCnt + tdata.m_Rewards[1].m_ItemCnt) * Cnt);
			break;
		}

		switch (tdata.m_Idx)
		{
		case BaseValue.CONTINUETICKET_SHOP_IDX:
		case BaseValue.CONTINUEAD_SHOP_IDX:
			USERINFO.Check_Mission(MissionType.Continue, 0, 0, Cnt);
			break;
		}
		CB.Invoke(new RES_BASE());
#else
		var pid = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == Idx);
		if (pid != null)
		{
			if(ACC.LoginType == ACC_STATE.Guest)
			{
				// 혹시 게스트계정도 구매가능하게하려면
				// Msg_CenterAlarm 제거하고
				// YN 박스에 멘트를
				// "게스트 계정은 게임 삭제 및 디바이스 변경 시 데이터가 삭제될 수 있습니다.\n구매를 진행 하시겠습니까?"
				// 라는거로 바꾸고 ((Main_Play)POPUP.GetMainUI()).GoAccChange(); 이부분을 BuyInAppProc(Idx, Cnt, CB); 이렇게 변경할것

				// 계스트 계정 연동 유도
				//if (!MAIN.IS_State(MainState.PLAY))
				//{
				//	POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(869));
				//	return;
				//}
				POPUP.Set_MsgBox(PopupName.Msg_YN, "", TDATA.GetString(867), (btn, obj) =>
				{
					if ((EMsgBtn)btn == EMsgBtn.BTN_YES) BuyInAppProc(Idx, Cnt, CB);
				});
				return;
			}

			BuyInAppProc(Idx, Cnt, CB);
		}
		else
		{
			switch (Idx) {
				case 207://BaseValue.REPLAY_REFRESH_SHOP_IDX
					CB?.Invoke(new RES_BASE() { result_code = EResultCode.SUCCESS });
					break;
				default:
					WEB.SEND_REQ_SHOP_BUY((res) => {
						if (!res.IsSuccess()) {
							WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
							return;
						}
						var tdata = TDATA.GetShopTable(Idx);
						if (tdata.m_Group == ShopGroup.SupplyBox) Check_Mission(MissionType.OpenSupplyBox, 0, 0, Cnt);
						CB?.Invoke(res);
					}, new List<REQ_SHOP_BUY.Buy_Item>() { new REQ_SHOP_BUY.Buy_Item() { Idx = Idx, Cnt = Cnt } }, _pickup);
					break;
			}
			
		}
		switch (TDATA.GetShopTable(Idx).m_PriceType) {
			case PriceType.Cash: PlayEffSound(SND_IDX.SFX_1010);break;
		}
#endif
	}

	void BuyInAppProc(int Idx, int Cnt, Action<RES_BASE> CB)
	{
		var pid = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == Idx);
		if (pid == null) return;
		HIVE.ShopBuy(Idx, pid.PID, (res) => {
			POPUP.LockConnecting(false);
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
				CB?.Invoke(res);
				return;
			}
			if (TDATA.GetShopTable(Idx).m_Group == ShopGroup.SupplyBox) Check_Mission(MissionType.OpenSupplyBox, 0, 0, 1);
			CB?.Invoke(res);
		});
	}

	public class SubwayStgIdx : ClassMng{
		public DayOfWeek m_Day = DayOfWeek.Sunday;
		public int m_StgIdx = 0;
		public long m_UTime;

		public SubwayStgIdx() {
			m_Day = UTILE.GetServerDayofWeek();
			m_StgIdx = 0;
			m_UTime = (long)UTILE.Get_ServerTime();
		}
	}

	/// <summary> 지하철 스테이지 인덱스 받기 </summary>
	public SubwayStgIdx GetSubwayStgIdx() {
		string str = PlayerPrefs.GetString($"SUBWAY_STGIDX_{USERINFO.m_UID}", string.Empty);
		if (str.Equals(string.Empty)) {
			PlayerPrefs.SetString($"SUBWAY_STGIDX_{USERINFO.m_UID}", JsonConvert.SerializeObject(new SubwayStgIdx()));
			PlayerPrefs.Save();
		}
		str = PlayerPrefs.GetString($"SUBWAY_STGIDX_{USERINFO.m_UID}", string.Empty);
		SubwayStgIdx data = JsonConvert.DeserializeObject<SubwayStgIdx>(str);
		if(data.m_StgIdx == 0 || !UTILE.IsSameDay((long)(data.m_UTime * 1000))) {
			TModeTable table = TDATA.GetModeTable(StageContentType.Subway, (int)UTILE.GetServerDayofWeek() * 2 + UTILE.Get_Random(1, 3), UTILE.GetServerDayofWeek(), 0);
			data.m_Day = UTILE.GetServerDayofWeek();
			data.m_StgIdx = table.m_StageIdx;
			data.m_UTime = (long)UTILE.Get_ServerTime();
			PlayerPrefs.SetString($"SUBWAY_STGIDX_{USERINFO.m_UID}", JsonConvert.SerializeObject(data));
			PlayerPrefs.Save();
		}
		return data;
	}
	public void SetSubwayStgIdx(int _idx, DayOfWeek _day) {
		string str = PlayerPrefs.GetString($"SUBWAY_STGIDX_{USERINFO.m_UID}", string.Empty);
		SubwayStgIdx data = JsonConvert.DeserializeObject<SubwayStgIdx>(str);
		data.m_Day = _day;
		data.m_StgIdx = _idx;
		data.m_UTime = (long)UTILE.Get_ServerTime();
		PlayerPrefs.SetString($"SUBWAY_STGIDX_{USERINFO.m_UID}", JsonConvert.SerializeObject(data));
		PlayerPrefs.Save();
	}

	public void CheckClearUserPickInfo(StageContentType Type, DayOfWeek Week, int Pos, int Idx)
	{
		UserPickCharInfo info = GetClearUserPickInfo(Type, Week, Pos, Idx);
		if (info == null)
		{
			WEB.SEND_REQ_USER_STAGE_PICK_INFO((res) => { }, Type, Week, Pos, Idx);
		}
	}

	public UserPickCharInfo GetClearUserPickInfo(StageContentType Type, DayOfWeek Week, int Pos, int Idx)
	{
		return m_ClearUserPickInfo.Find(o => o.Type == Type && o.Week == Week && o.Pos == Pos && o.Idx == Idx);
	}
	/// <summary> 구매 가능 가격인지 체크 </summary>
	public bool IS_CanBuy(TShopTable _data) {
		RES_SHOP_PID_INFO pinfo = USERINFO.m_ShopInfo.PIDs.Find(o => o.Idx == _data.m_Idx);
		if (pinfo != null) {
			return true;
		}
		switch (_data.m_PriceType) {
			case PriceType.Money:
				return USERINFO.m_Money >= _data.GetPrice();
			case PriceType.Cash:
				return USERINFO.m_Cash >= _data.GetPrice();
			case PriceType.Energy:
				return USERINFO.m_Energy.Cnt >= _data.GetPrice();
			case PriceType.PVPCoin:
				return USERINFO.m_PVPCoin >= _data.GetPrice();
			case PriceType.GuildCoin:
				return USERINFO.m_GCoin >= _data.GetPrice();
			case PriceType.Mileage:
				return USERINFO.m_Mileage >= _data.GetPrice();
			case PriceType.Item:
				if (_data.m_PriceIdx > 0) {
					return USERINFO.GetItemCount(_data.m_PriceIdx) >= _data.GetPrice();
				}
				break;
		}
		return false;
	}
	///////////////////////
	///리워드 리스트 받아 ItemType을 키로 묶어 반환, 묶음 아이콘 등에 사용 가능
	ItemType GetRewardItemType(RES_REWARD_BASE _reward) {
		ItemType type = ItemType.None;
		switch (_reward.Type) {
			case Res_RewardType.Money: type = ItemType.Dollar; break;
			case Res_RewardType.Exp: type = ItemType.Exp; break;
			case Res_RewardType.Cash: type = ItemType.Cash; break;
			case Res_RewardType.Energy: type = ItemType.Energy; break;
			case Res_RewardType.Inven: type = ItemType.InvenPlus; break;
			case Res_RewardType.Char: type = ItemType.Character; break;
			case Res_RewardType.DNA: type = ItemType.DNA; break;
			case Res_RewardType.Zombie: type = ItemType.Zombie; break;
			case Res_RewardType.Item:
				TItemTable tdata = TDATA.GetItemTable(_reward.GetIdx());
				if (tdata.GetInvenGroupType() == ItemInvenGroupType.Equipment) type = ItemType.Equip;
				else type = tdata.m_Type; 
				break;
		}
		return type;
	}
	ItemType GetRewardItemType(PostReward _reward) {
		ItemType type = ItemType.None;
		switch (_reward.Kind) {
			case RewardKind.Character: type = ItemType.Character; break;
			case RewardKind.DNA: type = ItemType.DNA; break;
			case RewardKind.Zombie: type = ItemType.Zombie; break;
			case RewardKind.Item:
				TItemTable tdata = TDATA.GetItemTable(_reward.Idx);
				if (tdata.GetInvenGroupType() == ItemInvenGroupType.Equipment) type = ItemType.Equip;
				else type = tdata.m_Type;
				break;
		}
		return type;
	}
	ItemType GetRewardItemType(RewardInfo _reward) {
		ItemType type = ItemType.None;
		switch (_reward.Kind) {
			case RewardKind.Character: type = ItemType.Character; break;
			case RewardKind.DNA: type = ItemType.DNA; break;
			case RewardKind.Zombie: type = ItemType.Zombie; break;
			case RewardKind.Item:
				TItemTable tdata = TDATA.GetItemTable(_reward.Idx);
				if (tdata.GetInvenGroupType() == ItemInvenGroupType.Equipment) type = ItemType.Equip;
				else type = tdata.m_Type;
				break;
		}
		return type;
	}
	public Dictionary<ItemType, List<RES_REWARD_BASE>> GetRewardGroup(List<RES_REWARD_BASE> _rewards) {
		return _rewards.GroupBy(o => GetRewardItemType(o)).ToDictionary(o => o.Key, o => o.ToList());
	}
	public Dictionary<ItemType, List<PostReward>> GetRewardGroup(List<PostReward> _rewards) {
		return _rewards.GroupBy(o => GetRewardItemType(o)).ToDictionary(o => o.Key, o => o.ToList());
	}
	public Dictionary<ItemType, List<RewardInfo>> GetRewardGroup(List<RewardInfo> _rewards) {
		return _rewards.GroupBy(o => GetRewardItemType(o)).ToDictionary(o => o.Key, o => o.ToList());
	}
	public List<RES_REWARD_BASE> GetExceptOverlap(List<RES_REWARD_BASE> _reward) {
		List<RES_REWARD_BASE> rewards = _reward;
		bool is_overlap = true;

		while (is_overlap) {
			is_overlap = false;
			for (int i = 0; i < rewards.Count; i++) {
				List<RES_REWARD_BASE> overlaps = rewards.FindAll(o => o.GetIdx() == rewards[i].GetIdx());
				if (overlaps.Count > 1) {
					if (overlaps[0].Type == Res_RewardType.Item) {
						((RES_REWARD_ITEM)overlaps[0]).Cnt = 0;
					}
					overlaps.RemoveAt(0);
					rewards = rewards.Except(overlaps).ToList();
					is_overlap = true;
				}
			}
		}

		return rewards;
	}
	public bool GetStoreSupplyBoxCheck() {
		TShopTable freetable = TDATA.GetGroupShopTable(ShopGroup.SupplyBox).Find(o => o.m_Idx == BaseValue.NORMAL_SUPPLYBOX_SHOP_IDX);
		TShopTable adstable = TDATA.GetGroupShopTable(ShopGroup.SupplyBox).Find(o => o.m_Idx == BaseValue.ADS_SUPPLYBOX_SHOP_IDX);
		TShopTable monthtable = TDATA.GetGroupShopTable(ShopGroup.SupplyBox).Find(o => o.m_Idx == BaseValue.MONTHLY_SUPPLYBOX_SHOP_IDX);
		double SUPPLY_TIME = freetable != null ? freetable.GetPrice() * 60 * (1f - USERINFO.GetSkillValue(SkillKind.SupplyBoxDown)) : 0;
		double ADS_SUPPLY_TIME = adstable != null ? adstable.GetPrice() * 60 * (1f - USERINFO.GetSkillValue(SkillKind.SupplyBoxDown)) : 0;
		double MONTHLY_SUPPLY_TIME = monthtable != null ? monthtable.GetPrice() * 60 * (1f - USERINFO.GetSkillValue(SkillKind.SupplyBoxDown)) : 0;

		bool alarm = false;
		if (SUPPLY_TIME > 0)
		{
			RES_SHOP_USER_BUY_INFO m_SupplyBuyInfo = m_ShopInfo.BUYs.Find(o => o.Idx == freetable.m_Idx);
			double supplytime = SUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_SupplyBuyInfo == null ? 0 : m_SupplyBuyInfo.UTime)) * 0.001;
			if (supplytime < SUPPLY_TIME && supplytime > 0) {
			}
			else {
				alarm = true;
			}
		}
		if (!alarm && ADS_SUPPLY_TIME > 0)
		{
			RES_SHOP_USER_BUY_INFO m_AdsSupplyBuyInfo = m_ShopInfo.BUYs.Find(o => o.Idx == adstable.m_Idx);
			double supplytime = ADS_SUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_AdsSupplyBuyInfo == null ? 0 : m_AdsSupplyBuyInfo.UTime)) * 0.001;
			if (supplytime < ADS_SUPPLY_TIME && supplytime > 0) {
			}
			else {
				alarm = true;
			}
		}
		if(!alarm && monthtable != null && MONTHLY_SUPPLY_TIME > 0)
		{
			if (MONTHLY_SUPPLY_TIME > 0)
			{
				RES_SHOP_USER_BUY_INFO m_MonthlySupplyBuyInfo = m_ShopInfo.BUYs.Find(o => o.Idx == monthtable.m_Idx);
				double supplytime = MONTHLY_SUPPLY_TIME - (UTILE.Get_ServerTime_Milli() - (m_MonthlySupplyBuyInfo == null ? 0 : m_MonthlySupplyBuyInfo.UTime)) * 0.001;

				RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs.Find(o => o.Idx == BaseValue.SHOP_IDX_MONTHLY_PACKAGE);
				if (supplytime < MONTHLY_SUPPLY_TIME && supplytime > 0 || packinfo == null || packinfo?.GetLastTime() <= 0)
				{
				}
				else
				{
					alarm = true;
				}
			}
		}
		return alarm;
	}
	public bool GetDailyPack() {
		bool notget = false;
		//구독형 패키지
		for (int i = 0; i < USERINFO.m_ShopInfo.PACKs.Count; i++) {
			//LS_Web.
			RES_SHOP_DAILYPACK_INFO packinfo = USERINFO.m_ShopInfo.PACKs[i];
			RES_SHOP_USER_BUY_INFO buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == packinfo.Idx);
			if (buyinfo != null && buyinfo.GetTime() > 0) {
				notget = true;
				break;
			}
		}
		return notget;
	}
	public bool GetCheckNewAuctionGoods() {
		bool newgoods = false;
		string str = PlayerPrefs.GetString($"Auction_{USERINFO.m_UID}", string.Empty);
		string nowkey = string.Join("|", USERINFO.m_Auction.Items.Select(o => o.m_Uid));
		if (string.IsNullOrEmpty(str)) newgoods = true;
		else {
			newgoods = !str.Equals(nowkey);
		}
		return newgoods;
	}
	public void SetCheckNewAuctionGoods(AuctionInfo _info) {
		PlayerPrefs.SetString($"Auction_{USERINFO.m_UID}", string.Join("|", _info.Items.Select(o => o.m_Uid)));
		PlayerPrefs.Save();
	}
	public Dictionary<StatType, float> GetCharLvStatBonus(int _lv = -1) {
		Dictionary<StatType, float> bonus = new Dictionary<StatType, float>();
		int SumLv = _lv == -1 ? m_Chars.Sum(o => o.m_LV) : _lv;

		List<TStatBonusTable> tdatas = TDATA.GetAllStatBonusTable();
		for(int i = 0; i < tdatas.Count; i++) {
			TStatBonusTable tdata = tdatas[i];
			float val = tdata.GetVal(SumLv);
			if (val == 0f) continue;
			if (!bonus.ContainsKey(tdata.m_Stat)) bonus.Add(tdata.m_Stat, 0);
			bonus[tdata.m_Stat] += val;
		}
		return bonus;
	}
	public int GetCharLvStatBonusNextLv(int _lv = -1) {
		int lv = 0;
		List<TStatBonusTable> tdatas = TDATA.GetAllStatBonusTable();
		for (int i = 0; i < tdatas.Count; i++) {
			TStatBonusTable tdata = tdatas[i];
			int remain = tdata.m_StartLv > _lv ? tdata.m_StartLv - _lv : tdata.m_Gap- (_lv - tdata.m_StartLv) % tdata.m_Gap;
			lv = lv == 0 ? remain : Mathf.Min(lv, remain); 
		}
		return lv;
	}
	public void SetGachaPickUp(List<int> _idxs) {
		string idxs = JsonConvert.SerializeObject(_idxs);
		PlayerPrefs.SetString(string.Format("GACHAPICKUP_{0}", USERINFO.m_UID), idxs);
		PlayerPrefs.Save();
	}
	public List<int> GetGachaPickUp() {
		string data = PlayerPrefs.GetString(string.Format("GACHAPICKUP_{0}", USERINFO.m_UID), string.Empty);
		if (!string.IsNullOrEmpty(data)) {
			return JsonConvert.DeserializeObject<List<int>>(data);
		}
		else return new List<int>();
	}
	public void SetPVPDefLog(List<int> _idxs) {
		string idxs = JsonConvert.SerializeObject(_idxs);
		PlayerPrefs.SetString(string.Format("PVP_DEF_LOG_{0}", USERINFO.m_UID), idxs);
		PlayerPrefs.Save();
	}
	public List<int> GetPVPDefLog(){
		List<int> idxs = null;
		string data = PlayerPrefs.GetString(string.Format("PVP_DEF_LOG_{0}", USERINFO.m_UID), string.Empty);
		if (!string.IsNullOrEmpty(data)) {
			idxs = JsonConvert.DeserializeObject<List<int>>(data);
		}
		else {
			idxs = new List<int>();
			SetPVPDefLog(idxs);
		}

		return idxs;
	}

#region Guilde Quest
	public GuidQuestInfo GetGuideQuest()
	{
		// 일일 미션 체크
		var mission = m_Mission.SuccessMission(MissionMode.Day);
		if (mission != null) return new GuidQuestInfo() { type = GuidQuestInfo.InfoType.Mission, Data = mission };
		// 초보자 미션 체크
		var newmissions = USERINFO.m_Mission.GetBeginnerQuest();
		if (newmissions != null)
		{
			foreach (var missions in newmissions)
			{
				var clearmission = missions.Value.Find(o => o.m_TData.m_Check.Any(o => o.m_Type == MissionType.BeginnerQuestClear));
				if (clearmission.IS_End()) continue;
				// 아직 오픈되지 않은 상태
				if (clearmission.STime > UTILE.Get_ServerTime_Milli()) break;
				var list = missions.Value.FindAll(o => !o.m_TData.m_Check.Any(o => o.m_Type == MissionType.BeginnerQuestClear || o.m_Type == MissionType.ModeClear) && o.IS_Complete() && !o.IS_End());
				if (list.Count < 1) continue;
				list.Sort((befor, after) => befor.Idx.CompareTo(after.Idx));
				mission = list[0];
				if (mission != null) return new GuidQuestInfo() { type = GuidQuestInfo.InfoType.Mission, Data = mission };
			}
		}

		// 업적 체크
		var achieve = m_Achieve.GetSucAchieveList();
		if (achieve.Count > 0) return new GuidQuestInfo() { type = GuidQuestInfo.InfoType.Achieve, Data = achieve[0] };


		// 가이드 퀘스트 유도
		mission = m_Mission.GetGuideMission();
		if(mission != null && !mission.IS_End()) return new GuidQuestInfo() { type = GuidQuestInfo.InfoType.Mission, Data = mission };

		// 초보자 퀘스트
		if (newmissions != null)
		{
			foreach (var missions in newmissions)
			{
				var clearmission = missions.Value.Find(o => o.m_TData.m_Check.Any(o => o.m_Type == MissionType.BeginnerQuestClear));
				if (clearmission.IS_End()) continue;
				// 아직 오픈되지 않은 상태
				if (clearmission.STime > UTILE.Get_ServerTime_Milli()) break;
				var list = missions.Value.FindAll(o => !o.m_TData.m_Check.Any(o => o.m_Type == MissionType.BeginnerQuestClear || o.m_Type == MissionType.ModeClear) && !o.IS_Complete());
				if (list.Count < 1) continue;
				list.Sort((befor, after) => befor.Idx.CompareTo(after.Idx));
				mission = list[0];
				if (mission != null) return new GuidQuestInfo() { type = GuidQuestInfo.InfoType.Mission, Data = mission };
			}
		}

		// 일일 퀘스트 유도
		var missionlist = m_Mission.Get_Missions(MissionMode.Day).FindAll(o => o.State[0] == RewardState.Idle && !o.IS_Complete());
		if (missionlist.Count > 0) return new GuidQuestInfo() { type = GuidQuestInfo.InfoType.Mission, Data = missionlist[0] };

		return null;
	}
#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//

	public RES_ACC_INFO GetACCINFO()
	{
		RES_ACC_INFO re = new RES_ACC_INFO();
		re.UserNo = m_UID;
		re.Cash[0] = _Cash[0];
		re.Cash[1] = _Cash[1];
		re.LV = m_LV;
		re.Profile = m_Profile;
		re.Name = _Name;
		return re;
	}
	/// <summary> 연동 변경 </summary>
	public void ACC_CHANGE(ACC_STATE state, string ID, Action CB)
	{
		WEB.SEND_REQ_ACC_CHANGE((res) =>
		{
			if (!res.IsSuccess())
			{
				WEB.StartErrorMsg(res.result_code);
				return;
			}
			CB?.Invoke();
		}, state, ID);
	}
}
