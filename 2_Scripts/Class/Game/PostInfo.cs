using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static LS_Web;

// 서버랑 통일을위해 m_ 제거
public class PostReward : ClassMng
{
	/// <summary> 보상 종류 </summary>
	public RewardKind Kind = RewardKind.None;
	/// <summary> 인덱스 </summary>
	public int Idx;
	/// <summary> 개수 </summary>
	public int Cnt;
	/// <summary> 캐릭터 등급 </summary>
	public int Grade = 0;
	/// <summary> 캐릭터 및 장비 레벨 </summary>
	public int LV = 1;
	/// <summary> 캐릭터 패시브 스킬 </summary>
	public int PSLV = 1;
	/// <summary> 캐릭터 액티브 스킬 </summary>
	public int ASLV = 1;
	/// <summary> 등급 추가 스탯 </summary>
	public List<ItemStat> AddStat = new List<ItemStat>();
	/// <summary> 보상 상태 </summary>
	public RewardState State;


	public RES_REWARD_BASE Get_RES_REWARD_BASE()
	{
		switch (Kind)
		{
		case RewardKind.Character:
			return new RES_REWARD_CHAR()
			{
				Type = Res_RewardType.Char,
				Idx = Idx,
				LV = LV,
				Grade = Grade == 0 ? TDATA.GetCharacterTable(Idx).m_Grade : Grade
			};
		case RewardKind.Item:
			var titemdata = TDATA.GetItemTable(Idx);
			switch (titemdata.m_Type)
			{
			case ItemType.DNA:
				return new RES_REWARD_DNA()
				{
					Type = Res_RewardType.DNA,
					Idx = Idx,
					Lv = LV,
					Grade = TDATA.GetDnaTable(Idx).m_Grade

				};
			case ItemType.Zombie:
				return new RES_REWARD_ZOMBIE()
				{
					Type = Res_RewardType.DNA,
					Idx = Idx,
					Grade = Grade == 0 ? TDATA.GetZombieTable(Idx).m_Grade : Grade
				};
			default:
				if (titemdata.GetEquipType() == EquipType.End)
				{
					return new RES_REWARD_ITEM()
					{
						Type = Res_RewardType.Item,
						Idx = Idx,
						Cnt = Cnt
					};
				}
				else
				{
					return new RES_REWARD_ITEM() {
						Type = Res_RewardType.Item,
						Idx = Idx,
						Cnt = 1,
						LV = LV
					};
				}
			}
		case RewardKind.DNA:
			return new RES_REWARD_DNA()
			{
				Type = Res_RewardType.DNA,
				Idx = Idx,
				Grade = TDATA.GetDnaTable(Idx).m_Grade
			};
		case RewardKind.Zombie:
			return new RES_REWARD_ZOMBIE()
			{
				Type = Res_RewardType.Zombie,
				Idx = Idx,
				Grade = Grade == 0 ? TDATA.GetZombieTable(Idx).m_Grade : Grade
			};
		}
		return null;
	}
}

public class PostInfo : ClassMng
{
	public class Msg
	{
		public string title;
		public string body;
	}
	/// <summary> 고유번호 </summary>
	public long UID;
	/// <summary> 우편 타입 </summary>
	public RewardPos Type;
	/// <summary> 알림 정보 </summary>
	public List<long> Values;
	/// <summary> 보상 목록 </summary>
	public List<PostReward> Rewards = new List<PostReward>();

	/// <summary> Hive 전용 표시 메세지 </summary>
	public Dictionary<string, Msg> Message = null;
	/// <summary> 수령 종료시간 </summary>
	public long ETime = 0;
	/// <summary> 0:받기전, 1:보상 받음, 2:제거됨 </summary>
	public int State = 0;

	public void SetDATA(RES_POSTINFO data)
	{
		UID = data.UID;
		Type = data.Type;
		Values = data.Values;
		Rewards.Clear();
		Rewards.AddRange(data.Items.Select(o => o.GetInfo()));
		List<PostReward> cashs = new List<PostReward>();
		for (int i = Rewards.Count - 1; i >= 0; i--) {
			if(Rewards[i].Idx == BaseValue.PAYCASH_IDX || Rewards[i].Idx == BaseValue.CASH_IDX) {
				cashs.Add(Rewards[i]);
				Rewards.RemoveAt(i);
			}
		}
		if(cashs.Count > 0) {
			PostReward cash = new PostReward();
			cash.Kind = RewardKind.Item;
			cash.Idx = BaseValue.CASH_IDX;
			for(int i = 0; i < cashs.Count; i++) {
				cash.Cnt += cashs[i].Cnt;
			}
			Rewards.Add(cash);
		}
		ETime = data.ETime;
		State = data.State;
		Message = data.Message;
	}
	public List<RES_REWARD_BASE> GetRewards() {
		if (Rewards == null) return new List<RES_REWARD_BASE>();
		return Rewards.Select(r => r.Get_RES_REWARD_BASE()).ToList();
	}

	public string GetTitle()
	{
		switch (Type)
		{
		/// <summary> 이벤트 보상 </summary>
		case RewardPos.Event:
			return TDATA.GetString(ToolData.StringTalbe.Post, (int)Values[1]);
		/// <summary> 운영자가 보냄 </summary>
		case RewardPos.Mng:
			return TDATA.GetString(ToolData.StringTalbe.Post, 5003);
		case RewardPos.Challenge:
			return TDATA.GetString(ToolData.StringTalbe.Post, 5004);
		case RewardPos.Adv:
			return TDATA.GetString(ToolData.StringTalbe.Post, 5005);
		case RewardPos.Making:
			return TDATA.GetString(ToolData.StringTalbe.Post, 5006);
		case RewardPos.Shop:
		case RewardPos.AddEvent:
			return TDATA.GetString(ToolData.StringTalbe.Post, 5007);
		case RewardPos.Auction:
			return TDATA.GetString(ToolData.StringTalbe.Post, 3);
		case RewardPos.PVP://0:리그1:시즌
			bool league = (int)Values[0] == 0;
			int ranking = (int)Values[1];
			TPvPRankTable tdata = TDATA.GeTPvPRankTable((int)Values[2]);
			int titleidx = 0;
			if (ranking == 1) titleidx = league ? 5018 : 5015;
			else if (ranking > 1 && ranking <= 10) titleidx = league ? 5018 : 5016;
			else titleidx = league ? 5018 : 5017;
			return TDATA.GetString(ToolData.StringTalbe.Post, titleidx);
		case RewardPos.Hive:
			if(Message != null && Message.Count > 0)
			{
				string lang = HIVE.GetHiveLanguageCode(APPINFO.m_LanguageCode);
				if (Message.ContainsKey(lang)) return Message[lang].title;
				else if(Message.ContainsKey("en")) return Message["en"].title;
				return Message.First().Value.title;
			}
			return TDATA.GetString(ToolData.StringTalbe.Post, (int)Values[1]);
		case RewardPos.DailyPackItem:
			var tshop = TDATA.GetShopTable((int)Values[0]);
			return string.Format(TDATA.GetString(ToolData.StringTalbe.Post, 5024), tshop.GetName());
		default://TODO:문구들 추가되면 케이스 추가
			return TDATA.GetString(ToolData.StringTalbe.Post, 1);
			///// <summary> 스테이지에서 지급 </summary>
			//case RewardPos.Stage: break;
			///// <summary> 챕터 보상 </summary>
			//case RewardPos.Chapter: break;
			///// <summary> 스테이지 대화 </summary>
			//case RewardPos.Talk: break;
			///// <summary> 우편함에서 지급 </summary>
			//case RewardPos.Post: break;
			///// <summary> 좀비 승급 실패 또는 파괴 </summary>
			//case RewardPos.Zombie: break;
			///// <summary> 업적 </summary>
			//case RewardPos.Achieve: break;
			///// <summary> 인앱 구매(가방 사이즈 체크가 필요없음) </summary>
			//case RewardPos.InApp: break;
		}
	}

	public string GeMsg(string rewardname)
	{
		switch (Type)
		{
		/// <summary> 이벤트 보상 </summary>
		case RewardPos.Event:
			return TDATA.GetString(ToolData.StringTalbe.Post, (int)Values[2]);
		/// <summary> 운영자가 보냄 </summary>
		case RewardPos.Mng:
			return TDATA.GetString(ToolData.StringTalbe.Post, 6003);
		case RewardPos.Challenge:
			return TDATA.GetString(ToolData.StringTalbe.Post, 6004);
		case RewardPos.Adv:
			return TDATA.GetString(ToolData.StringTalbe.Post, 6005);
		case RewardPos.Making:
			return TDATA.GetString(ToolData.StringTalbe.Post, 6006);
		case RewardPos.Shop:
		case RewardPos.AddEvent:
			return TDATA.GetString(ToolData.StringTalbe.Post, 6007);
		case RewardPos.Auction:
			string msg = string.Empty;
			if ((int)Values[0] == 1) return TDATA.GetString(ToolData.StringTalbe.Post, 4);
			else if ((int)Values[0] == 0) return string.Format(TDATA.GetString(ToolData.StringTalbe.Post, 5), USERINFO.m_Name);
			return "";
		case RewardPos.PVP://0:리그1:시즌
			bool league = (int)Values[0] == 0;
			int ranking = (int)Values[1];
			TPvPRankTable tdata = TDATA.GeTPvPRankTable((int)Values[2]);
			int msgidx = 0;
			if (ranking == 1) msgidx = league ? 6018 : 6015;
			else if (ranking > 1 && ranking <= 10) msgidx = league ? 6018 : 6016;
			else msgidx = league ? 6018 : 6017;
			return string.Format(TDATA.GetString(ToolData.StringTalbe.Post, msgidx), tdata.GetRankName(), tdata.GetTierName());
		case RewardPos.Hive:
			if (Message != null && Message.Count > 0)
			{
				string lang = HIVE.GetHiveLanguageCode(APPINFO.m_LanguageCode);
				if (Message.ContainsKey(lang)) return Message[lang].body;
				else if (Message.ContainsKey("en")) return Message["en"].body;
				return Message.First().Value.body;
			}
			return TDATA.GetString(ToolData.StringTalbe.Post, (int)Values[2]);
		case RewardPos.DailyPackItem:
			return string.Format(TDATA.GetString(ToolData.StringTalbe.Post, 6025), (int)Values[1]);
		default://TODO:문구들 추가되면 케이스 추가
			return string.Format(TDATA.GetString(ToolData.StringTalbe.Post, 2), rewardname, Rewards[0].Cnt);
			///// <summary> 스테이지에서 지급 </summary>
			//case RewardPos.Stage: break;
			///// <summary> 챕터 보상 </summary>
			//case RewardPos.Chapter: break;
			///// <summary> 스테이지 대화 </summary>
			//case RewardPos.Talk: break;
			///// <summary> 우편함에서 지급 </summary>
			//case RewardPos.Post: break;
			///// <summary> 좀비 승급 실패 또는 파괴 </summary>
			//case RewardPos.Zombie: break;
			///// <summary> 업적 </summary>
			//case RewardPos.Achieve: break;
			///// <summary> 인앱 구매(가방 사이즈 체크가 필요없음) </summary>
			//case RewardPos.InApp: break;
		}
	}
}

