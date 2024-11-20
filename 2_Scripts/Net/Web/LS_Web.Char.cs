using Newtonsoft.Json;
using System;
using System.Collections.Generic;

public partial class LS_Web
{
	public class REQ_CHARINFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}


	public class RES_ALL_CHARINFO : RES_BASE
	{
		public List<RES_CHARINFO> Chars = new List<RES_CHARINFO>();
	}

	public class RES_CHARINFO : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 등급 </summary>
		public int Grade;
		/// <summary> 스킬 레벨 </summary>
		public int[] SkillLV;
		/// <summary> 장비 UID </summary>
		public long[] EquipUID;

		/// <summary> DNA Slot 오픈 상태 </summary>
		public bool[] DNASlot;

		/// <summary> 장착한 DNA </summary>
		public long[] DNA;

		/// <summary> 각성 상태 </summary>
		public List<int> Serum;
		/// <summary> 각성 블럭 오픈 페이지 </summary>
		public int SPage { get; set; } = 1;
	}

	public void SEND_REQ_CHARINFO(Action<RES_ALL_CHARINFO> action, long UserNo, params long[] UIDs)
	{
		REQ_CHARINFO _data = new REQ_CHARINFO();
		_data.UserNo = UserNo;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);

		SendPost(Protocol.REQ_CHARINFO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ALL_CHARINFO>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_LVUP
	public class REQ_CHAR_LVUP : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 증가 레벨 </summary>
		public int AddLV;
	}

	public class RES_CHAR_LVUP : RES_BASE
	{
		/// <summary> 증가 레벨 </summary>
		public int AddLV;
		/// <summary> 증가한 레벨 </summary>
		public int NowLV;
	}

	public void SEND_REQ_CHAR_LVUP(Action<RES_CHAR_LVUP> action, long UID, CharInfo info, int BeforLV)
	{
		REQ_CHAR_LVUP _data = new REQ_CHAR_LVUP();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.AddLV = info.m_LV - BeforLV;
		SendPost(Protocol.REQ_CHAR_LVUP, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_CHAR_LVUP res = ParsResData<RES_CHAR_LVUP>(data);
			if(res.IsSuccess())
			{
				int AddLV = info.m_LV - BeforLV;
				USERINFO.Check_Mission(MissionType.CharLevelUp, 0, 0, AddLV);
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Character_LevelUp_Count, 0, AddLV);
				USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Character_Level_Count, BeforLV, info.m_LV);
			}
			action?.Invoke(res);
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_SKILL_LVUP
	public class REQ_CHAR_SKILL_LVUP : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 스킬 </summary>
		public SkillType Type;
		/// <summary> 증가 레벨 </summary>
		public int AddLV;
	}

	public class RES_CHAR_SKILL_LVUP : RES_BASE
	{
		/// <summary> 스킬 </summary>
		public SkillType Type;
		/// <summary> 증가 레벨 </summary>
		public int AddLV;
		/// <summary> 증가한 레벨 </summary>
		public int NowLV;
	}

	public void SEND_REQ_CHAR_SKILL_LVUP(Action<RES_CHAR_SKILL_LVUP> action, long UID, SkillType Type, int AddLV)
	{
		REQ_CHAR_SKILL_LVUP _data = new REQ_CHAR_SKILL_LVUP();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Type = Type;
		_data.AddLV = AddLV;
		SendPost(Protocol.REQ_CHAR_SKILL_LVUP, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_CHAR_SKILL_LVUP>(data));
		});
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_CHAR_GRADEUP
	public class REQ_CHAR_GRADEUP : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
	}

	public void SEND_REQ_CHAR_GRADEUP(Action<RES_BASE> action, CharInfo Info)
	{
		REQ_CHAR_GRADEUP _data = new REQ_CHAR_GRADEUP();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = Info.m_UID;
		SendPost(Protocol.REQ_CHAR_GRADEUP, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.GradeUp, 0, 0, 1);
				USERINFO.m_Achieve.Check_Achieve(AchieveType.Character_GradeUp_Count);
				USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Character_Grade_Count, Info.m_Grade - 1, Info.m_Grade);
				USERINFO.m_Collection.Check(CollectionType.Character, Info.m_Idx, Info.m_Grade);
			}
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_EQUIP
	public class REQ_CHAR_EQUIP : REQ_BASE
	{
		/// <summary> 캐릭터 고유 번호 </summary>
		public long CUID;
		/// <summary> 변경 할 아이템 고유 번호들 </summary>
		public List<long> UIDs;
	}

	public class RES_CHAR_EQUIP : RES_BASE
	{
		/// <summary> 장착 변경된 위치 </summary>
		public List<EquipType> EquipPos = new List<EquipType>();
	}

	public void SEND_REQ_CHAR_EQUIP(Action<RES_CHAR_EQUIP> action, long CUID, List<long> UIDs)
	{
		REQ_CHAR_EQUIP _data = new REQ_CHAR_EQUIP();
		_data.UserNo = USERINFO.m_UID;
		_data.CUID = CUID;
		_data.UIDs = UIDs;
		SendPost(Protocol.REQ_CHAR_EQUIP, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.Check_AllEquipItem();
			action?.Invoke(ParsResData<RES_CHAR_EQUIP>(data));
		});
	}
	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_UNEQUIP
	public class REQ_CHAR_UNEQUIP : REQ_BASE
	{
		/// <summary> 캐릭터 고유 번호 </summary>
		public long CUID;
		/// <summary> 아이템 고유 번호 </summary>
		public long IUID;
	}

	public void SEND_REQ_CHAR_UNEQUIP(Action<RES_BASE> action, long UID)
	{
		REQ_CHAR_UNEQUIP _data = new REQ_CHAR_UNEQUIP();
		CharInfo cinfo = USERINFO.GetEquipChar(UID);
		_data.UserNo = USERINFO.m_UID;
		_data.CUID = cinfo.m_UID;
		_data.IUID = UID;
		SendPost(Protocol.REQ_CHAR_UNEQUIP, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.Check_AllEquipItem();
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	public void SEND_REQ_CHAR_UNEQUIPALL(Action<RES_BASE> action, long UID)
	{
		REQ_CHAR_UNEQUIP _data = new REQ_CHAR_UNEQUIP();
		_data.UserNo = USERINFO.m_UID;
		_data.CUID = UID;
		SendPost(Protocol.REQ_CHAR_UNEQUIP, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.Check_AllEquipItem();
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_DNA_SET
	public class REQ_CHAR_DNA_SET : REQ_BASE
	{
		/// <summary> 캐릭터 고유 번호 </summary>
		public long UID;
		/// <summary> 장착 DNA 위치 </summary>
		public int Pos;
		/// <summary> 장착 DNA 고유 번호 </summary>
		public long DUID;
	}

	/// <summary> DNA 장착 </summary>
	/// <param name="UID"> 장착 시킬 캐릭터 UID</param>
	/// <param name="Pos"> 장착 시킬 위치 </param>
	/// <param name="DUID"> 장착 할 DNA UID </param>
	public void SEND_REQ_CHAR_DNA_SET(Action<RES_BASE> action, long UID, int Pos, long DUID)
	{
		//ERROR_NOT_FOUND_CHAR		캐릭터 못찾음(UID 가 0이거나 잘못됨)
		//ERROR_USERNO				유저번호 오류(캐릭터를 보유한 유저가 아님)
		//ERROR_POS					장착 슬롯 오픈 안됨
		//ERROR_NOT_FOUND_DNA		DNA 못찾음(1.장착 안된 슬롯에 해제를 요청했을경우. 2.검색했을때 못찾는 경우)
		REQ_CHAR_DNA_SET _data = new REQ_CHAR_DNA_SET();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Pos = Pos;
		_data.DUID = DUID;
		SendPost(Protocol.REQ_CHAR_DNA_SET, JsonConvert.SerializeObject(_data), (result, data) => {
			USERINFO.Check_AllEquipItem();
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Story
	public class REQ_CHAR_DNA_SLOTOPEN : REQ_BASE
	{
		/// <summary> 캐릭터 고유 번호 </summary>
		public long UID;
		/// <summary> 슬롯 번호 (0~4) </summary>
		public int Pos;
	}

	public void SEND_REQ_CHAR_DNA_SLOTOPEN(Action<RES_BASE> action, CharInfo info, int slot)
	{
		REQ_CHAR_DNA_SLOTOPEN _data = new REQ_CHAR_DNA_SLOTOPEN();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.m_UID;
		_data.Pos = slot;

		SendPost(Protocol.REQ_CHAR_DNA_SLOTOPEN, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess()) {
				info.m_DNASlot[slot] = true;
				int shopidx = 0;
				switch (slot) {
					case 2: shopidx = BaseValue.SHOP_IDX_DNA_SLOT_OPEN_3; break;
					case 3: shopidx = BaseValue.SHOP_IDX_DNA_SLOT_OPEN_4; break;
					case 4: shopidx = BaseValue.SHOP_IDX_DNA_SLOT_OPEN_5; break;
				}
				if (shopidx != 0) USERINFO.m_ShopInfo.SetBuyInfo(shopidx, 1);
			}
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Story

	public void SEND_REQ_CHAR_STORY(Action<RES_BASE> action, CharInfo info, int slot)
	{
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Serum
	public class REQ_CHAR_SERUM : REQ_BASE
	{
		/// <summary> 캐릭터 고유 번호 </summary>
		public long UID;
		/// <summary> 각성 시킬 인덱스 </summary>
		public int Serum;
	}

	public void SEND_REQ_CHAR_SERUM(Action<RES_BASE> action, CharInfo info, int Serum)
	{
		REQ_CHAR_SERUM _data = new REQ_CHAR_SERUM();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.m_UID;
		_data.Serum = Serum;

		SendPost(Protocol.REQ_CHAR_SERUM, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}


	public class REQ_CHAR_SERUM_PAGE_OPEN : REQ_BASE
	{
		/// <summary> 캐릭터 고유 번호 </summary>
		public long UID;
	}

	public void SEND_REQ_CHAR_SERUM_PAGE_OPEN(Action<RES_BASE> action, CharInfo info)
	{
		REQ_CHAR_SERUM_PAGE_OPEN _data = new REQ_CHAR_SERUM_PAGE_OPEN();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.m_UID;

		SendPost(Protocol.REQ_CHAR_SERUM_PAGE_OPEN, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_SET_EVA
	public class REQ_CHAR_SET_EVA : REQ_BASE
	{
		/// <summary> 캐릭터 인덱스 </summary>
		public int Idx;
		/// <summary> 0~10 (1당 0.5) </summary>
		public int Point;
		/// <summary> 평가 정보 </summary>
		public List<EvaData> Datas;
	}

	public void SEND_REQ_CHAR_SET_EVA(Action<RES_BASE> action, int idx, int point, List<EvaData> Datas)
	{
		REQ_CHAR_SET_EVA _data = new REQ_CHAR_SET_EVA();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = idx;
		_data.Point = point;
		_data.Datas = Datas;
		SendPost(Protocol.REQ_CHAR_SET_EVA, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_CHAR_GET_EVA>(data));
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	/// REQ_CHAR_SET_EVA
	public class REQ_CHAR_GET_EVA : REQ_BASE
	{
		/// <summary> 캐릭터 인덱스 </summary>
		public int Idx;
	}


	public class RES_CHAR_GET_EVA : RES_BASE
	{
		/// <summary> 총 평가 정보 </summary>
		public CharEva Data;

		/// <summary> 내 평가 정보 null 이면 평가안함 </summary>
		public MyCharEva MyData;
	}

	public void SEND_REQ_CHAR_GET_EVA(Action<RES_BASE> action, int idx)
	{
		REQ_CHAR_GET_EVA _data = new REQ_CHAR_GET_EVA();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = idx;
		SendPost(Protocol.REQ_CHAR_GET_EVA, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_CHAR_GET_EVA>(data));
		});
	}
}
