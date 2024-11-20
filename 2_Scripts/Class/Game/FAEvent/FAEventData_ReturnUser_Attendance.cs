
using System;
using System.Collections.Generic;
using UnityEngine;

public class FAEventData_ReturnUser_Attendance : FAEventData
{
	/// <summary> 일자별 보상 내역 </summary>
	public List<FAEventData_Attendance> Attendance { get; set; } = new List<FAEventData_Attendance>();
	public List<int> Missions { get; set; } = new List<int>();
}