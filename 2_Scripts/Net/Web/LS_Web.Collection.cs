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
	// 컬렉션 정보
	public class CollectionLV
	{
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 레벨 </summary>
		public int LV;
	}

	public class CollectionData
	{
		public CollectionType Type;
		/// <summary> 인덱스 </summary>
		public int Idx;
		/// <summary> 체크할 값 </summary>
		public int Value;
	}
	public class RES_COLLECTION_INFO : RES_BASE
	{
		/// <summary> 레벨 </summary>
		public List<CollectionLV> LV = new List<CollectionLV>();

		/// <summary> Data </summary>
		public List<CollectionData> Datas = new List<CollectionData>();
	}

	public void SEND_REQ_COLLECTION_INFO(Action<RES_COLLECTION_INFO> action, int Idx)
	{
		REQ_BASE _data = new REQ_BASE();
		_data.UserNo = USERINFO.m_UID;

		SendPost(Protocol.REQ_COLLECTION_INFO, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_COLLECTION_INFO res = ParsResData<RES_COLLECTION_INFO>(data);
			if (res.IsSuccess()) USERINFO.SetDATA(res);
			action?.Invoke(res);
		});
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////
	// 컬렉션 레벨업
	public class REQ_COLLECTION_LVUP : REQ_BASE
	{
		public List<int> List;
	}

	public void SEND_REQ_COLLECTION_LVUP(Action<RES_COLLECTION_INFO> action, List<int> GIdx)
	{
		REQ_COLLECTION_LVUP _data = new REQ_COLLECTION_LVUP();
		_data.UserNo = USERINFO.m_UID;
		_data.List = GIdx;

		SendPost(Protocol.REQ_COLLECTION_LVUP, JsonConvert.SerializeObject(_data), (result, data) => {
			RES_COLLECTION_INFO res = ParsResData<RES_COLLECTION_INFO>(data);
			if (res.IsSuccess()) {
				USERINFO.SetDATA(res);
				//연구 받고 장비랑 캐릭터 전투력 측정
				USERINFO.GetUserCombatPower();
			}
			action?.Invoke(res);
		});
	}
}
