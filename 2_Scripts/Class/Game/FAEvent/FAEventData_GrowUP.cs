
using System;
using System.Collections.Generic;
using UnityEngine;
public class FAEventData_GrowUP : FAEventData
{
	public class LVInfo
	{
		/// <summary> 미션 인덱스 </summary>
		public int LV { get; set; }
		/// <summary> 아이템 인덱스 </summary>
		public int Item { get; set; }
		/// <summary> 필요 개수, 경험치 요구량 </summary>
		public int Cnt { get; set; }
		public RewardInfo Reward { get; set; }
	}

	public class Mission
	{
		/// <summary> 미션 인덱스 </summary>
		public int Idx { get; set; }

		/// <summary> 제한 레벨(칠면조) </summary>
		public int Limit { get; set; }
	}

	/// <summary> 이벤트 스테이지 </summary>
	public List<FAEventData_Stage> StageInfos { get; set; } = new List<FAEventData_Stage>();
	/// <summary> 이벤트 스테이지에서 사용되면 추가보상되느 캐릭터 정보 및 보상 내역 </summary>
	public List<FAEventData_StageChar> StageCharReward { get; set; } = new List<FAEventData_StageChar>();
	/// <summary> 키우는 대상의 레벨 및 보상 정보 </summary>
	public List<LVInfo> ExpInfo { get; set; } = new List<LVInfo>();
	/// <summary> 미션 정보 </summary>
	public List<Mission> Missions { get; set; } = new List<Mission>();
	/// <summary> 판매 상품 목록 </summary>
	public List<int> ShopItems { get; set; } = new List<int>();
}
