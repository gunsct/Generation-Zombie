
using System;
using System.Collections.Generic;
using UnityEngine;

public class FAEventData_Attendance : FAEventData
{
	/// <summary> 회차 번호 </summary>
	public int No { get; set; }
	/// <summary> 일자별 보상 내역 </summary>
	public List<PostReward> Reward { get; set; } = new List<PostReward>();
}