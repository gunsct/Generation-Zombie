
using System;
using System.Collections.Generic;
using UnityEngine;

public class FAEventData_Rot_Attendance : FAEventData
{
	/// <summary> 일자별 보상 내역 </summary>
	public List<FAEventData_Attendance> Lists { get; set; } = new List<FAEventData_Attendance>();

	/// <summary> 연속 출석 보상 </summary>
	public List<FAEventData_Attendance> Continue { get; set; } = new List<FAEventData_Attendance>();
}