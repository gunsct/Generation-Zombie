
using System;
using System.Collections.Generic;
using UnityEngine;
public class FAEventData_Stage_Minigame
{
	/// <summary> 이벤트 스테이지 </summary>
	public List<FAEventData_Stage> StageInfos { get; set; } = new List<FAEventData_Stage>();
	/// <summary> 이벤트 스테이지에서 사용되면 추가보상되느 캐릭터 정보 및 보상 내역 </summary>
	public List<FAEventData_StageChar> StageCharReward { get; set; } = new List<FAEventData_StageChar>();
	public List<int> Minigames { get; set; } = new List<int>();
	public List<int> Missions { get; set; } = new List<int>();
	public List<int> ShopItems { get; set; } = new List<int>();
}
