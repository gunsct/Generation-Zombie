using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	public class REQ_ITEMINFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}

	public class RES_ALL_ITEMINFO : RES_BASE
	{
		public List<RES_ITEMINFO> Items = new List<RES_ITEMINFO>();
	}

	public class RES_ITEM_STAT
	{
		public StatType Type;
		public float Value;
		public float ValueUp;

		public ItemStat GetItemStat()
		{
			return new ItemStat() { m_Stat = Type, m_Val = Value, m_ValUp = ValueUp };
		}
	}

	public class RES_ITEMINFO : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 등급 </summary>
		public int Grade;
		/// <summary> 개수(장비일때는 레벨) </summary>
		public int Cnt;
		/// <summary> 스킬 레벨 </summary>
		public List<RES_ITEM_STAT> AddStat;
		/// <summary> 경험치 </summary>
		public int EXP = 0;
		/// <summary> 잠금 상태 </summary>
		public bool Lock;
	}

	public void SEND_REQ_ITEMINFO(Action<RES_ALL_ITEMINFO> action, params long[] UIDs)
	{
		REQ_ITEMINFO _data = new REQ_ITEMINFO();
		_data.UserNo = USERINFO.m_UID;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);
		SendPost(Protocol.REQ_ITEMINFO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ALL_ITEMINFO>(data));
		});
	}

	public class REQ_USE_ITEM
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 사용 개수 </summary>
		public int Cnt;
	}
	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_LOCK

	public void SEND_REQ_ITEM_LOCK(Action<RES_BASE> action, long[] UIDs)
	{
		REQ_ITEMINFO _data = new REQ_ITEMINFO();
		_data.UserNo = USERINFO.m_UID;
		_data.UIDs.AddRange(UIDs);
		SendPost(Protocol.REQ_ITEM_LOCK, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_LVUP
	public class REQ_ITEM_LVUP : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 재료 </summary>
		public List<REQ_USE_ITEM> Material = new List<REQ_USE_ITEM>();
	}

	public class RES_ITEM_LVUP : RES_BASE
	{
		/// <summary> 추가 경험치 </summary>
		public long AddExp;
		/// <summary> 증가 레벨 </summary>
		public int AddLV;
	}

	public void SEND_REQ_ITEM_LVUP(Action<RES_ITEM_LVUP> action, ItemInfo info, List<REQ_USE_ITEM> Mats)
	{
		REQ_ITEM_LVUP _data = new REQ_ITEM_LVUP();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = info.m_Uid;
		_data.Material.AddRange(Mats);
		int BeforLV = info.m_Lv;
		SendPost(Protocol.REQ_ITEM_LVUP, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ITEM_LVUP res = ParsResData<RES_ITEM_LVUP>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.EquipLevelUp, 0, 0, res.AddLV);
				if (info.m_Lv != BeforLV)
				{
					USERINFO.Check_MissionUpDown(MissionType.EquipLevelUp, BeforLV, info.m_Lv, 1);
					USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Level_Count, BeforLV, info.m_Lv);
				}
			}
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_OPCHANGE
	public class REQ_ITEM_OPCHANGE : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 옵션 위치 </summary>
		public int Pos;
	}

	public class RES_ITEM_OPCHANGE : RES_BASE
	{
		/// <summary> 옵션 위치 </summary>
		public int Pos;
	}

	public void SEND_REQ_ITEM_OPCHANGE(Action<RES_ITEM_OPCHANGE> action, long UID, int pos)
	{
		REQ_ITEM_OPCHANGE _data = new REQ_ITEM_OPCHANGE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Pos = pos;

		SendPost(Protocol.REQ_ITEM_OPCHANGE, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ITEM_OPCHANGE>(data));
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// SEND_REQ_ITEM_SELL
	public class REQ_ITEM_SELL : REQ_BASE
	{
		/// <summary> 판매 아이템 </summary>
		public List<REQ_USE_ITEM> Items = new List<REQ_USE_ITEM>();
	}

	public void SEND_REQ_ITEM_SELL(Action<RES_BASE> action, List<REQ_USE_ITEM> Mats)
	{
		REQ_ITEM_SELL _data = new REQ_ITEM_SELL();
		_data.UserNo = USERINFO.m_UID;
		_data.Items.AddRange(Mats);

		SendPost(Protocol.REQ_ITEM_SELL, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_OPOPEN
	public class REQ_ITEM_OPOPEN : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
	}

	public class RES_ITEM_OPOPEN : RES_BASE
	{
		/// <summary> 성공여부 </summary>
		public bool IsSuc;
		/// <summary> 변경된 옵션 위치 </summary>
		public int Pos;
	}

	public void SEND_REQ_ITEM_OPOPEN(Action<RES_ITEM_OPOPEN> action, long UID)
	{
		REQ_ITEM_OPOPEN _data = new REQ_ITEM_OPOPEN();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;

		SendPost(Protocol.REQ_ITEM_OPOPEN, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ITEM_OPOPEN>(data));
		});
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_REMAKE
	public class REQ_ITEM_REMAKE : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
	}

	public class RES_ITEM_REMAKE : RES_BASE
	{
		/// <summary> 고유 번호(선택용) </summary>
		public long UID;

		/// <summary> 선택할 아이템 </summary>
		public List<RES_ITEMINFO> ReMake = new List<RES_ITEMINFO>();
	}

	public void SEND_REQ_ITEM_REMAKE(Action<RES_ITEM_REMAKE> action, long UID)
	{
		REQ_ITEM_REMAKE _data = new REQ_ITEM_REMAKE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;

		SendPost(Protocol.REQ_ITEM_REMAKE, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ITEM_REMAKE>(data));
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_REMAKE_SELECT
	public class REQ_ITEM_REMAKE_SELECT : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 선택 아이템 고유 번호 </summary>
		public long SUID;
	}

	public void SEND_REQ_ITEM_REMAKE_SELECT(Action<RES_BASE> action, long UID, long SUID)
	{
		REQ_ITEM_REMAKE_SELECT _data = new REQ_ITEM_REMAKE_SELECT();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.SUID = SUID;

		SendPost(Protocol.REQ_ITEM_REMAKE_SELECT, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_ITEM_UPGRADE
	public class REQ_ITEM_UPGRADE : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public long UID;
		/// <summary> 재료 아이템 UID </summary>
		public long MUID;
	}

	public void SEND_REQ_ITEM_UPGRADE(Action<RES_BASE> action, ItemInfo TItem, ItemInfo MItem)
	{
		REQ_ITEM_UPGRADE _data = new REQ_ITEM_UPGRADE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = TItem.m_Uid;
		_data.MUID = MItem.m_Uid;

		int BeforGrade = TItem.m_Grade;
		SendPost(Protocol.REQ_ITEM_UPGRADE, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.Equip_Grade_Count, BeforGrade, TItem.m_Grade);
				USERINFO.m_Collection.Check(CollectionType.Equip, TItem.m_Idx, TItem.m_Grade);
			}
			action?.Invoke(res);
		});
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_CHAGE_CHAR_PIECE
	public class REQ_CHAGE_CHAR_PIECE : REQ_BASE
	{
		public enum ChangeMode
		{
			/// <summary> 무기명 인사파일로 교체
			/// <para>생성되는 무기명 인사파일 공식 : (int)([Idx의 아이템 테이블 Value값] * [글로벌 테이블 PersonnelFileChangeRatio 값] / 100) * [Cnt] </para>
			/// </summary>
			NormalPiece = 0,
			/// <summary> 캐릭터 인사파일로 교체
			/// <para>필요한 무기명 인사파일 개수 공식 : (int)([Cnt] * [Idx의 아이템 테이블 Value값]) </para>
			/// </summary>
			CharPiece,
			End
		}
		/// <summary> 교체될 모드 </summary>
		public ChangeMode Mode;
		/// <summary> 캐릭터 피스 인덱스 </summary>
		public int Idx;
		/// <summary> 캐릭터 인사파일 개수 </summary>
		public int Cnt;
	}

	public void SEND_REQ_CHAGE_CHAR_PIECE(Action<RES_BASE> action, REQ_CHAGE_CHAR_PIECE.ChangeMode Mode, int Idx, int Cnt)
	{
		REQ_CHAGE_CHAR_PIECE _data = new REQ_CHAGE_CHAR_PIECE();
		_data.UserNo = USERINFO.m_UID;
		_data.Mode = Mode;
		_data.Idx = Idx;
		_data.Cnt = Cnt;

		SendPost(Protocol.REQ_CHAGE_CHAR_PIECE, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
}
