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
	public enum ZombieState
	{
		Idle = 0,
		Cage
	}
	public class REQ_ZOMBIEINFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}

	public class RES_ALL_ZOMBIEINFO : RES_BASE
	{
		public List<REQ_ZOMBIEINFO> Zombies = new List<REQ_ZOMBIEINFO>();
	}
	public class RES_ZOMBIEINFO : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 상태 </summary>
		public ZombieState State;
	}

	public void SEND_REQ_ZOMBIEINFO(Action<RES_ALL_ZOMBIEINFO> action, params long[] UIDs)
	{
		REQ_ZOMBIEINFO _data = new REQ_ZOMBIEINFO();
		_data.UserNo = USERINFO.m_UID;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);

		SendPost(Protocol.REQ_ZOMBIEINFO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ALL_ZOMBIEINFO>(data));
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ZOMBIE_ROOM_INFO

	public class REQ_ZOMBIE_ROOM_INFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<int> Poss = new List<int>();
	}


	public class RES_ALL_ZOMBIE_ROOM_INFO : RES_BASE
	{
		public List<RES_ZOMBIE_ROOM_INFO> ZRooms = new List<RES_ZOMBIE_ROOM_INFO>();
	}

	public class RES_ZOMBIE_ROOM_INFO : RES_BASE
	{
		/// <summary> 인덱스 </summary>
		public int Pos = 0;
		/// <summary> 좀비들(Idx) </summary>
		public List<long> ZUIDs = new List<long>();
		/// <summary> 생성 시작 시간 </summary>
		public long PTime;
	}

	public void SEND_REQ_ZOMBIE_ROOM_INFO(Action<RES_ALL_ZOMBIE_ROOM_INFO> action, params int[] Poss)
	{
		//ERROR_NOT_FOUND_CAGE			케이지 못찾음

		REQ_ZOMBIE_ROOM_INFO _data = new REQ_ZOMBIE_ROOM_INFO();
		_data.UserNo = USERINFO.m_UID;
		if (Poss != null) _data.Poss.AddRange(Poss);

		SendPost(Protocol.REQ_ZOMBIE_ROOM_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ALL_ZOMBIE_ROOM_INFO res = ParsResData<RES_ALL_ZOMBIE_ROOM_INFO>(data);
			if(res.IsSuccess())	USERINFO.SetDATA(res.ZRooms);
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ZOMBIE_CAGE_OPEN
	public class RES_ZOMBIE_CAGE_OPEN : RES_BASE
	{
		public RES_ZOMBIE_ROOM_INFO RoomInfo;
	}

	public void SEND_REQ_ZOMBIE_CAGE_OPEN(Action<RES_ZOMBIE_CAGE_OPEN> action)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_ZOMBIE_CAGE_OPEN, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ZOMBIE_CAGE_OPEN res = ParsResData<RES_ZOMBIE_CAGE_OPEN>(data);
			if (res.IsSuccess())
			{
				USERINFO.SetDATA(res.RoomInfo);
				USERINFO.m_ShopInfo.SetBuyInfo(BaseValue.SHOP_IDX_ZOMBIE_CAGE, 1);
			}
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ZOMBIE_SET
	public class REQ_ZOMBIE_SET : REQ_BASE
	{
		/// <summary> 방 번호 </summary>
		public int RoomNo;
		/// <summary> 장착할 좀비 고유 번호 </summary>
		public long UID;
	}
	public class RES_ZOMBIE_SET : RES_BASE
	{
		/// <summary> 방 정보 </summary>
		public RES_ZOMBIE_ROOM_INFO RoomInfo;
	}

	public void SEND_REQ_ZOMBIE_SET(Action<RES_BASE> action, ZombieInfo info, int roomno)
	{
		REQ_ZOMBIE_SET _data = new REQ_ZOMBIE_SET();
		_data.UserNo = USERINFO.m_UID;
		_data.RoomNo = roomno;
		_data.UID = info.m_UID;
		SendPost(Protocol.REQ_ZOMBIE_SET, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ZOMBIE_SET res = ParsResData<RES_ZOMBIE_SET>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.PlaceZombie, 0, 0, 1);
				USERINFO.SetDATA(res.RoomInfo);
			}
			action?.Invoke(res);
		});
	}

	public void SEND_REQ_ZOMBIE_UPGRADE(Action<RES_BASE> action, ZombieInfo Info, long DUID)
	{
	}


	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ZOMBIE_DESTROY
	public class REQ_ZOMBIE_DESTROY : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}

	public class RES_ZOMBIE_DESTROY : RES_BASE
	{
		/// <summary> 방 정보 </summary>
		public List<RES_ZOMBIE_ROOM_INFO> RoomInfos;
	}

	public void SEND_REQ_ZOMBIE_DESTROY(Action<RES_BASE> action, List<ZombieInfo> info)
	{
		REQ_ZOMBIE_DESTROY _data = new REQ_ZOMBIE_DESTROY();
		_data.UserNo = USERINFO.m_UID;
		for (int i = 0; i < info.Count; i++) {
			if(info[i] != null) _data.UIDs.Add(info[i].m_UID);
		}
		SendPost(Protocol.REQ_ZOMBIE_DESTROY, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ZOMBIE_DESTROY res = ParsResData<RES_ZOMBIE_DESTROY>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.ZombieDestory, 0, 0, info.Count);
				for (int i = 0; i < info.Count; i++) {
					if (info[i] == null) continue;
					USERINFO.SetDATA(res.RoomInfos);
					USERINFO.m_Zombies.RemoveAll(o => o.m_UID == info[i].m_UID);
				}
			}
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_ZOMBIE_PRODUCE
	public class REQ_ZOMBIE_PRODUCE : REQ_BASE
	{
		/// <summary> 케이지 위치 </summary>
		public List<int> Poss;
	}
	public class RES_ZOMBIE_PRODUCE : RES_BASE
	{
		/// <summary> 방 정보 </summary>
		public List<RES_ZOMBIE_ROOM_INFO> RoomInfos;
	}

	public void SEND_REQ_ZOMBIE_PRODUCE(Action<RES_ZOMBIE_PRODUCE> action, List<int> Poss)
	{
		REQ_ZOMBIE_PRODUCE _data = new REQ_ZOMBIE_PRODUCE();
		_data.UserNo = USERINFO.m_UID;
		_data.Poss = Poss;
		SendPost(Protocol.REQ_ZOMBIE_PRODUCE, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ZOMBIE_PRODUCE res = ParsResData<RES_ZOMBIE_PRODUCE>(data);
			if (res.IsSuccess())
			{
				USERINFO.Check_Mission(MissionType.GetZombieRNA, 0, 0, res.RoomInfos.Count);
				USERINFO.SetDATA(res.RoomInfos);
			}
			action?.Invoke(res);
		});
	}
}
