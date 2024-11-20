using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public partial class LS_Web
{
	public class RES_USERLV : RES_BASE
	{
		/// <summary> Exp </summary>
		public long Exp;
		/// <summary> LV </summary>
		public int LV;
	}

	public class RES_USERINFO : RES_BASE
	{
		/// <summary> 유저 고유번호 </summary>
		public long UserNo;
		/// <summary> 길드 고유 번호 </summary>
		public long GID;
		/// <summary> 닉네임 </summary>
		public string Name;
		/// <summary> 캐시 </summary>
		public long[] Cash;
		/// <summary> 달러 </summary>
		public long Money;
		/// <summary> 마일리지 </summary>
		public long Mileage;
		/// <summary> Exp </summary>
		public long[] Exp;
		/// <summary> 레벨 </summary>
		public int LV;
		/// <summary> 인벤 사이즈 </summary>
		public int Inven;
		/// <summary> 좀비 최대 보유 개수 </summary>
		public int ZInven;
		/// <summary> 좀비 케이지 오픈 구매 횟수 </summary>
		public int CageCnt;
		/// <summary> 진입 아이템 정보 </summary>
		public RES_TIMEITEM Energy;
		/// <summary> 프로필 이미지 </summary>
		public int Profile;

		/// <summary> 돌발 이벤트 정보 </summary>
		public int AddEventIdx;
		/// <summary> 상점 장비 가차 횟수 </summary>
		public long EQGachaCnt;


		/// <summary> 국가 코드 </summary>
		public string Nation;
		/// <summary> 보급상자 레벨 </summary>
		public int SupplyBoxLV;
		/// <summary> 길드 코인 </summary>
		public long GCoin;
		/// <summary> 길드 탈퇴한 시간 </summary>
		public long GRTime;

		/// <summary> PVP 랭크 </summary>
		public int PVPRank;
		/// <summary> 0:rank, 1:league </summary>
		public long[] PVPpoint = new long[2];
		/// <summary> PVP 리그포인트 </summary>
		public long PVPCoin;
	}

	public void SEND_REQ_USERINFO(Action<RES_USERINFO> action, long UserNo)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = UserNo;
		SendPost(Protocol.REQ_USERINFO, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			action?.Invoke(JsonConvert.DeserializeObject<RES_USERINFO>(data));
		});
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Set User Profile
	public class REQ_USER_PROFILE_SET : REQ_BASE
	{
		/// <summary> 프로필 이미지 인덱스 </summary>
		public int Profile;
	}
	public void SEND_REQ_USER_PROFILE_SET(Action<RES_BASE> action, int Profile)
	{
		REQ_USER_PROFILE_SET _data = new REQ_USER_PROFILE_SET();
		_data.UserNo = USERINFO.m_UID;
		_data.Profile = Profile;
		SendPost(Protocol.REQ_USER_PROFILE_SET, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_BASE res = JsonConvert.DeserializeObject<RES_BASE>(data);
			if(res.IsSuccess())
			{
				USERINFO.m_Profile = Profile;
			}
			action?.Invoke(res);
		});
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Set User Profile
	public class REQ_USER_NICKNAME_SET : REQ_BASE
	{
		/// <summary> 닉네임 </summary>
		public string NickName;
		/// <summary> 0 : 구매, 1 : 인벤 사용, 100 : 무료 </summary>
		public int ChangeMode;
	}
	public void SEND_REQ_USER_NICKNAME_SET(Action<RES_BASE> action, string NickName, int ChangeMode)
	{
		REQ_USER_NICKNAME_SET _data = new REQ_USER_NICKNAME_SET();
		_data.UserNo = USERINFO.m_UID;
		_data.NickName = NickName;
		_data.ChangeMode = ChangeMode;
		SendPost(Protocol.REQ_USER_NICKNAME_SET, JsonConvert.SerializeObject(_data), (ushCode, data) =>
		{
			RES_BASE res = JsonConvert.DeserializeObject<RES_BASE>(data);
			if (res.IsSuccess())
			{
				USERINFO.m_Name = NickName;
			}
			action?.Invoke(res);
		});
	}

}
