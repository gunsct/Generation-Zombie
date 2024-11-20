using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Auction
	public class REQ_AUCTION_INFO : REQ_BASE
	{
		public bool IsJoin;
	}
	public class RES_AUCTION_INFO : RES_BASE
	{
		public List<RES_AUCTION_ITEM> Infos = new List<RES_AUCTION_ITEM>();
		public bool IsJoin;
	}
	public class RES_AUCTION_ITEM : RES_ITEMINFO
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
		/// <summary> 유저 국가 </summary>
		public string Nation;
		/// <summary> 유저 프로필 이미지 </summary>
		public int Profile;
		/// <summary> 가격 캐시(0:무료, 1:유료) </summary>
		public int[] Price = new int[2];
		/// <summary> 0:시작시간, 1:종료시간 </summary>
		public long[] Times = new long[2];
	}

	public void SEND_REQ_AUCTION_INFO(Action<RES_AUCTION_INFO> action, bool IsJoin)
	{
		REQ_AUCTION_INFO req = new REQ_AUCTION_INFO();
		req.UserNo = USERINFO.m_UID;
		req.IsJoin = IsJoin;
		SendPost(Protocol.REQ_AUCTION_INFO, JsonConvert.SerializeObject(new REQ_BASE()), (result, data) => {
			RES_AUCTION_INFO res = ParsResData<RES_AUCTION_INFO>(data);
			if(res.IsSuccess() && res.IsJoin) USERINFO.Check_Mission(MissionType.Auction, 0, 0, 1);
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Auction Buy
	public class REQ_AUCTION_BUY : REQ_BASE
	{
		/// <summary> UID </summary>
		public long UID;
		/// <summary> 금액 </summary>
		public long Price;
	}

	public class RES_AUCTION_BUY : RES_BASE
	{
		/// <summary> 구매 정보 </summary>
		public RES_AUCTION_ITEM Info;
	}

	public void SEND_REQ_AUCTION_BUY(Action<RES_AUCTION_BUY> action, long UID, long Price)
	{
		REQ_AUCTION_BUY _data = new REQ_AUCTION_BUY();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = UID;
		_data.Price = Price;
		SendPost(Protocol.REQ_AUCTION_BUY, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_AUCTION_BUY>(data));
		});
	}
}
