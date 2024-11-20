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
	// 업적 정보 받기
	public class AchieveEnd : ClassMng
	{
		/// <summary> 보상 받은 그룹 인덱스 </summary>
		public int Idx;
		/// <summary> 받은 보상의 레벨 </summary>
		public List<int> LVs = new List<int>();
	}
	public class RES_ACHIEVE_INFO : RES_BASE
	{
		/// <summary> 완료된 업적(보상을 수령한 업적들) </summary>
		public List<AchieveEnd> EAchieve = new List<AchieveEnd>();

		/// <summary> 업적 누적 체크용 데이터 </summary>
		public List<AchieveData> Datas = new List<AchieveData>();
	}

	public void SEND_REQ_ACHIEVE_INFO(Action<RES_ACHIEVE_INFO> action, int Idx)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_ACHIEVE_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ACHIEVE_INFO res = ParsResData<RES_ACHIEVE_INFO>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res);
			action?.Invoke(res);
		});
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 업적 보상 받기
	public class REQ_ACHIEVE_REWARD_LIST : REQ_BASE
	{
		public List<REQ_ACHIEVE_REWARD> List;
	}

	public class REQ_ACHIEVE_REWARD
	{
		/// <summary> 그룹 인덱스 </summary>
		public int Idx;
		/// <summary> 보상받을 레벨 </summary>
		public int LV;
	}

	/// <summary> 돌발 이벤트 NPC 토크 선택지 선택 </summary>
	/// <param name="Idx">선택한 선택지 인덱스</param>
	public void SEND_REQ_ACHIEVE_REWARD(Action<RES_ACHIEVE_INFO> action, List<REQ_ACHIEVE_REWARD> list)
	{
		REQ_ACHIEVE_REWARD_LIST _data = new REQ_ACHIEVE_REWARD_LIST();
		_data.UserNo = USERINFO.m_UID;
		_data.List = list;

		SendPost(Protocol.REQ_ACHIEVE_REWARD, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_ACHIEVE_INFO res = ParsResData<RES_ACHIEVE_INFO>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res);
			action?.Invoke(res);
		});
	}
}
