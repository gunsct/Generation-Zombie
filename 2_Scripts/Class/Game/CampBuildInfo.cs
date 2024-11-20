using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using static LS_Web;

// 서버랑 통일을위해 m_ 제거
public class CampBuildInfo : ClassMng
{
	/// <summary> 건물 타입 </summary>
	public CampBuildType Build;

	/// <summary> 레벨 </summary>
	public int LV;
	/// <summary> 각 건물별 값들
	/// <para>창고 : 자원 Junk, Cultivate, Chemical </para>
	/// <para>생산 : 자원 생산 종료 시간 Junk, Cultivate, Chemical </para>
	/// 생산은 values 0일때만 레벨업 가능
	/// </summary>
	public long[] Values;

	/// <summary> 0 : 레벨업 시작 시간, 1 : 레벨업 종료 시간, 2 : 마지막 업데이트 시간 </summary>
	public long[] Time;

	public void SetDATA(RES_CAMP_BUILD_INFO data)
	{
		Build = data.Build;
		LV = data.LV;
		Values = new long[data.Values.Length];
		System.Array.Copy(data.Values, 0, Values, 0, data.Values.Length);

		Time = new long[data.Time.Length];
		System.Array.Copy(data.Time, 0, Time, 0, data.Time.Length);
	}

	public bool IS_CanLvUp { get { return Values.Count(o=>o!=0) > 0; } }
	public double GetRemainMakeTime(int _pos) {
		return (Values[_pos] - UTILE.Get_ServerTime_Milli()) * 0.001d;
	}
	public bool IS_MakeComplete(int _pos) {
		return GetRemainMakeTime(_pos) <= 0;
	}
	public double GetRemainLvUpTime { get { return Time[1] - Time[0]; } }
	public bool IS_CanMakeTime(int _pos) {
		return Values[_pos] == 0;// UTILE.Get_ServerTime_Milli() > Values[_pos] && Time[2] > Values[_pos];
	}
	public bool IS_CanGetTime(int _pos) {
		return Values[_pos] > 0 && UTILE.Get_ServerTime_Milli() >= Values[_pos];// && Time[2] < Values[_pos];
	}
}
