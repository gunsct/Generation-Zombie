using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public partial class LS_Web
{
	public enum Res_RewardType
	{
		None = 0,
		Money,
		UserExp,
		Exp,
		Cash,
		Energy,
		Inven,
		Char,
		Item,
		Box,
		StageCard,
		DNA,
		Zombie,
		PVPCoin,
		GCoin,
		GPoint,
		Mileage,
		/// <summary> 일반 자원(아이템 인덱스 137) </summary>
		CampRes_Junk,
		/// <summary> 중급 자원(아이템 인덱스 138) </summary>
		CampRes_Cultivate,
		/// <summary> 고급 자원(아이템 인덱스 139) </summary>
		CampRes_Chemical,
		End
	}

	public class RES_REWARDS
	{
		/// <summary> 보상정보 </summary>
		public List<RES_REWARD> Rewards = new List<RES_REWARD>();
	}


	public class RES_REWARD
	{
		public Res_RewardType Type;
		/// <summary> 보상 타입 </summary>
		public List<RES_REWARD_BASE> Infos = new List<RES_REWARD_BASE>();
	}


	public class RES_REWARD_BASE_Conv : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(RES_REWARD_BASE))
			{
				return true;
			}
			return false;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var t = serializer.Deserialize(reader);
			var b = JsonConvert.DeserializeObject<RES_REWARD_BASE>(t.ToString());
			switch (b.Type)
			{
			case Res_RewardType.Money:
			case Res_RewardType.Exp:
			case Res_RewardType.Cash:
			case Res_RewardType.Energy:
			case Res_RewardType.Inven:
			case Res_RewardType.PVPCoin:
			case Res_RewardType.GCoin:
			case Res_RewardType.GPoint:
			case Res_RewardType.Mileage:
			case Res_RewardType.CampRes_Chemical:
			case Res_RewardType.CampRes_Cultivate:
			case Res_RewardType.CampRes_Junk:
				return JsonConvert.DeserializeObject<RES_REWARD_MONEY>(t.ToString());
			case Res_RewardType.UserExp: return JsonConvert.DeserializeObject<RES_REWARD_USEREXP>(t.ToString());
			case Res_RewardType.Char: return JsonConvert.DeserializeObject<RES_REWARD_CHAR>(t.ToString());
			case Res_RewardType.Item:
			case Res_RewardType.StageCard: 
				return JsonConvert.DeserializeObject<RES_REWARD_ITEM>(t.ToString());
			case Res_RewardType.DNA: return JsonConvert.DeserializeObject<RES_REWARD_DNA>(t.ToString());
				case Res_RewardType.Zombie: return JsonConvert.DeserializeObject<RES_REWARD_ZOMBIE>(t.ToString());
			}
			return b;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
	
	public class RES_REWARD_BASE
	{
		public int result_code;
		public Res_RewardType Type;

		public int GetIdx()
		{
			switch(Type)
			{
			case Res_RewardType.Money:		return BaseValue.DOLLAR_IDX;
			case Res_RewardType.Exp:		return BaseValue.EXP_IDX;
			case Res_RewardType.Cash:		return BaseValue.CASH_IDX;
			case Res_RewardType.Energy:		return BaseValue.ENERGY_IDX;
			case Res_RewardType.Inven:		return BaseValue.INVEN_IDX;
			case Res_RewardType.Char:		return ((RES_REWARD_CHAR)this).Idx;
			case Res_RewardType.PVPCoin:	return BaseValue.PVPCOIN_IDX;
			case Res_RewardType.GCoin:		return BaseValue.GUILDCOIN_IDX;
			case Res_RewardType.GPoint:		return BaseValue.GUILDPOINT_IDX;
			case Res_RewardType.Mileage:	return BaseValue.MILEAGE_IDX;
			case Res_RewardType.CampRes_Chemical: return BaseValue.CAMP_RES_CHEMICAL_IDX;
			case Res_RewardType.CampRes_Cultivate: return BaseValue.CAMP_RES_CULTIVATE_IDX;
			case Res_RewardType.CampRes_Junk: return BaseValue.CAMP_RES_JUNK_IDX;
			case Res_RewardType.Item:
			case Res_RewardType.StageCard:
				return((RES_REWARD_ITEM)this).Idx;
			case Res_RewardType.DNA: return ((RES_REWARD_DNA)this).Idx;
			case Res_RewardType.Zombie: return ((RES_REWARD_ZOMBIE)this).Idx;
			}
			return 0;
		}

		public string GetName()
		{
			switch (Type)
			{
			case Res_RewardType.Money: 
			case Res_RewardType.Exp:
			case Res_RewardType.Cash:
			case Res_RewardType.Energy:
			case Res_RewardType.Inven:
			case Res_RewardType.Item:
			case Res_RewardType.CampRes_Chemical:
			case Res_RewardType.CampRes_Cultivate:
			case Res_RewardType.CampRes_Junk:
					return MainMng.Instance.TDATA.GetItemTable(GetIdx()).GetName();
			case Res_RewardType.Char:
				return MainMng.Instance.TDATA.GetCharacterTable(GetIdx()).GetCharName();
			case Res_RewardType.DNA:
				return MainMng.Instance.TDATA.GetDnaTable(GetIdx()).GetName();
			case Res_RewardType.Zombie:
				return MainMng.Instance.TDATA.GetZombieTable(GetIdx()).GetName();
			}
			return "";
		}
		public int GetGrade() {
			switch (Type) {
				case Res_RewardType.Item:
					return MainMng.Instance.TDATA.GetItemTable(GetIdx()).m_Grade;
				case Res_RewardType.Char:
					return MainMng.Instance.TDATA.GetCharacterTable(GetIdx()).m_Grade;
				case Res_RewardType.DNA:
					return MainMng.Instance.TDATA.GetDnaTable(GetIdx()).m_Grade;
				case Res_RewardType.Zombie:
					return MainMng.Instance.TDATA.GetZombieTable(GetIdx()).m_Grade;
			}
			return 0;
		}
		public Sprite GetImage() {
			switch (Type) {
				case Res_RewardType.Money: 
				case Res_RewardType.Exp: 
				case Res_RewardType.Cash: 
				case Res_RewardType.Energy: 
				case Res_RewardType.Inven:
				case Res_RewardType.PVPCoin: 
				case Res_RewardType.GCoin: 
				case Res_RewardType.GPoint: 
				case Res_RewardType.Item:
				case Res_RewardType.Mileage:
				case Res_RewardType.CampRes_Chemical:
				case Res_RewardType.CampRes_Cultivate:
				case Res_RewardType.CampRes_Junk:
					return MainMng.Instance.TDATA.GetItemTable(GetIdx()).GetItemImg();
				case Res_RewardType.Char:
					return MainMng.Instance.TDATA.GetCharacterTable(GetIdx()).GetPortrait();
				case Res_RewardType.DNA:
					return MainMng.Instance.TDATA.GetDnaTable(GetIdx()).GetIcon();
				case Res_RewardType.Zombie:
					return MainMng.Instance.TDATA.GetZombieTable(GetIdx()).GetItemSmallImg();
			}
			return null;
		}
		public int GetCnt() {
			switch (Type) {
				case Res_RewardType.Item: 
					return ((RES_REWARD_ITEM)this).Cnt;
				case Res_RewardType.Money:
				case Res_RewardType.Exp:
				case Res_RewardType.Cash:
				case Res_RewardType.Energy:
				case Res_RewardType.Inven:
				case Res_RewardType.PVPCoin:
				case Res_RewardType.GCoin:
				case Res_RewardType.GPoint:
				case Res_RewardType.Mileage:
				case Res_RewardType.CampRes_Chemical:
				case Res_RewardType.CampRes_Cultivate:
				case Res_RewardType.CampRes_Junk:
					return ((RES_REWARD_MONEY)this).Add;
				case Res_RewardType.UserExp:
					return (int)((RES_REWARD_USEREXP)this).AExp;
			}
			return 1;
		}
	}
	public class RES_REWARD_USEREXP : RES_REWARD_BASE
	{
		/// <summary> 증가량 </summary>
		public long AExp;
		/// <summary> 이전값 </summary>
		public long BExp;
		/// <summary> 현재값 </summary>
		public long NExp;
		/// <summary> 이전레벨 </summary>
		public int BLV;
		/// <summary> 현재 레벨 </summary>
		public int NLV;
	}

	public class RES_REWARD_MONEY : RES_REWARD_BASE
	{
		/// <summary> 획득량 </summary>
		public int Add;
		/// <summary> 이전값 </summary>
		public long Befor;
		/// <summary> 이전값(캐시에서 사용) </summary>
		public long FBefor;
		/// <summary> 현재값 </summary>
		public long Now;
		/// <summary> 이전값(캐시에서 사용) </summary>
		public long FNow;
		/// <summary> 갱신 시간(카운트가 진행된 시간) </summary>
		public long STime;
	}


	public class RES_REWARD_CHAR : RES_REWARD_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 등급 </summary>
		public int Grade;


		public RES_REWARD_CHAR()
		{
			Type = Res_RewardType.Char;
		}
		public void SetData(CharInfo info)
		{
			Type = Res_RewardType.Char;
			UID = info.m_UID;
			Idx = info.m_Idx;
			LV = info.m_LV;
			Grade = info.m_Grade;
		}
	}

	public class RES_REWARD_ITEM : RES_REWARD_BASE
	{
		/// <summary> 장비 고유번호 </summary>
		public long UID = 0;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 레벨 </summary>
		public int LV = 1;
		/// <summary> 개수 </summary>
		public int Cnt;
		/// <summary> (피스 캐릭터 등급) </summary>
		public int Grade;

		public RES_REWARD_ITEM() {
			Type = Res_RewardType.Item;
		}
	}

	public class RES_REWARD_DNA : RES_REWARD_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 등급 </summary>
		public int Grade;
		/// <summary> 레벨 </summary>
		public int Lv;
		public RES_REWARD_DNA() {
			Type = Res_RewardType.DNA;
		}
	}

	public class RES_REWARD_ZOMBIE : RES_REWARD_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 등급 </summary>
		public int Grade;
		public RES_REWARD_ZOMBIE() {
			Type = Res_RewardType.Zombie;
		}
	}
}
