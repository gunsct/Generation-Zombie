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
	public class REQ_DNAINFO : REQ_BASE
	{
		/// <summary> 고유 번호 </summary>
		public List<long> UIDs = new List<long>();
	}

	public class RES_ALL_DNAINFO : RES_BASE
	{
		public List<REQ_DNAINFO> Zombies = new List<REQ_DNAINFO>();
	}
	public class RES_DNAINFO : RES_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID = 0;
		/// <summary> 인덱스 </summary>
		public int Idx = 0;
		/// <summary> 레벨 </summary>
		public int LV = 1;

		/// <summary> 등급 추가 스탯 </summary>
		public List<RES_ITEM_STAT> AddStat = new List<RES_ITEM_STAT>();
	}

	public void SEND_REQ_DNAINFO(Action<RES_ALL_ZOMBIEINFO> action, params long[] UIDs)
	{
		REQ_ZOMBIEINFO _data = new REQ_ZOMBIEINFO();
		_data.UserNo = USERINFO.m_UID;
		if (UIDs != null) _data.UIDs.AddRange(UIDs);

		SendPost(Protocol.REQ_DNAINFO, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_ALL_ZOMBIEINFO>(data));
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_DNA_CREATE
	public class REQ_DNA_CREATE : REQ_BASE
	{
		/// <summary> 인덱스(MakingTable Idx) </summary>
		public int Idx;
		/// <summary> 생산 개수 </summary>
		public int Cnt;
	}
	public void SEND_REQ_DNA_CREATE(Action<RES_BASE> action, int Idx, int Cnt)
	{
		//ERROR_MAKING_IDX			인덱스 오류(0으로 들어옴)
		//ERROR_TOOLDATA			툴데이터 찾지못함(MakingTable, ItemTable)
		//ERROR_MONEY				달러 부족
		REQ_DNA_CREATE _data = new REQ_DNA_CREATE();
		_data.UserNo = USERINFO.m_UID;
		_data.Idx = Idx;
		_data.Cnt = Cnt;

		SendPost(Protocol.REQ_DNA_CREATE, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if(res.IsSuccess()) USERINFO.Check_Mission(MissionType.ProduceDNA, 0, 0, Cnt);
			action?.Invoke(res);
		});
	}

	/////////////////////////////////////////////////////////////////////////////////////
	// REQ_DNA_UPGRADE
	public class REQ_DNA_UPGRADE : REQ_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
	}

	public void SEND_REQ_DNA_UPGRADE(Action<RES_BASE> action, DNAInfo Mats)
	{
		//ERROR_NOT_FOUND_DNA		DNA 찾지못함 (UID가 0이거나 잘못됨)
		//ERROR_TOOLDATA			툴데이터 찾지못함(DNATable, DNALevelTable, RandomStatTable)
		//ERROR_DNA_MAX_LV			최대 레벨에 도달된 DNA
		//ERROR_MONEY				달러 부족
		//ERROR_ETC_ITEM			재료 부족
		REQ_DNA_UPGRADE _data = new REQ_DNA_UPGRADE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = Mats.m_UID;

		SendPost(Protocol.REQ_DNA_UPGRADE, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if(res.IsSuccess())
			{
				var info = res.InitDNAs[0];
				USERINFO.m_Achieve.Check_Achieve(AchieveType.DNA_LevelUp_Count);
				USERINFO.m_Achieve.Check_AchieveUpDown(AchieveType.DNA_LevelUp_Count, info.LV - 1, info.LV);
			}
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_DNA_DESTROY
	public class REQ_DNA_DESTROY : REQ_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
	}

	public void SEND_REQ_DNA_DESTROY(Action<RES_BASE> action, DNAInfo DNA)
	{
		//ERROR_NOT_FOUND_DNA		DNA 찾지못함 (UID가 0이거나 잘못됨)
		//ERROR_TOOLDATA			툴데이터 찾지못함(DNALevelTable)
		//ERROR_DNA_EQUIP			장착중인 DNA
		REQ_DNA_UPGRADE _data = new REQ_DNA_UPGRADE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = DNA.m_UID;

		SendPost(Protocol.REQ_DNA_DESTROY, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_BASE res = ParsResData<RES_BASE>(data);
			if (res.IsSuccess()) USERINFO.m_DNAs.RemoveAll(o => o.m_UID == DNA.m_UID);
			action?.Invoke(res);
		});
	}


	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// REQ_DNA_OPCHANGE
	public class REQ_DNA_OPCHANGE : REQ_BASE
	{
		/// <summary> 고유번호 </summary>
		public long UID;
	}

	public void SEND_REQ_DNA_OPCHANGE(Action<RES_BASE> action, DNAInfo DNA)
	{
		//ERROR_NOT_FOUND_DNA		DNA 찾지못함 (UID가 0이거나 잘못됨)
		//ERROR_TOOLDATA			툴데이터 찾지못함(DNATable, DNALevelTable)
		//ERROR_MONEY				달러 부족
		//ERROR_ETC_ITEM			재료 부족
		REQ_DNA_OPCHANGE _data = new REQ_DNA_OPCHANGE();
		_data.UserNo = USERINFO.m_UID;
		_data.UID = DNA.m_UID;

		SendPost(Protocol.REQ_DNA_OPCHANGE, JsonConvert.SerializeObject(_data), (result, data) => {
			action?.Invoke(ParsResData<RES_BASE>(data));
		});
	}
}
