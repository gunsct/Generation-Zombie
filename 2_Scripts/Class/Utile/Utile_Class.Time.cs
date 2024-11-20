using System;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public partial class Utile_Class
{
#pragma warning disable 0414
	private static readonly DateTime m_UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	private static TimeSpan m_Server_Timezone = TimeSpan.Parse("00:00:00");
#pragma warning restore 0414

	public double Get_Time()
	{
		return (DateTime.Now - m_UnixEpoch).TotalSeconds;
	}

	public double Get_Time_Milli()
	{
		return (DateTime.Now - m_UnixEpoch).TotalMilliseconds;
	}
	public double Get_Time_Day()
	{
		return (DateTime.Now - m_UnixEpoch).TotalDays;
	}

	public double Get_UtcTime(DateTime time)
	{
		return (time - m_UnixEpoch).TotalSeconds;
	}

	public double Get_UtcTime_Milli(DateTime time)
	{
		return (time - m_UnixEpoch).TotalMilliseconds;
	}

	static double diffTime = 0;
	static DateTime syncTime = DateTime.UtcNow;
	static string servertimestring;
	public static string GetTimeLog() {
		return $"\ntime string : {servertimestring}\ntime DateTime : {syncTime}\ndiffTime : {diffTime}";
	}

	public void SetServerTime(string time)
	{
		if (string.IsNullOrEmpty(time)) return;
		//DateTime.TryParse(time, out DateTime dateTime)
		if (DateTime.TryParseExact(time, "yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out DateTime dateTime)) {
			servertimestring = time;
			syncTime = dateTime;
			diffTime = (syncTime - DateTime.UtcNow).TotalMilliseconds;
		}
	}
	public static DateTime Get_ServerDateTime()
	{
		return DateTime.UtcNow.AddMilliseconds(diffTime);
	}

	public double Get_ServerTime()
	{
		// 서버 체크 시간 UtcNow로 변경
		return (Get_ServerDateTime() - m_UnixEpoch).TotalSeconds;// + m_Server_Timezone.TotalSeconds;// Get_Time();// + Get_TimeZoneGap();
	}

	public double Get_ServerTime_Milli()
	{
		// 서버 체크 시간 UtcNow로 변경
		return (Get_ServerDateTime() - m_UnixEpoch).TotalMilliseconds;// + m_Server_Timezone.TotalMilliseconds;
	}

	public static string Get_TimeZone_String()
	{
		TimeSpan zone = TimeZoneInfo.Local.BaseUtcOffset;
		return string.Format("{0:D2}:{1:D2}:{2:D2}", zone.Hours, zone.Minutes, zone.Seconds);
	}

	public static double Get_TimeZone()
	{
		TimeSpan zone = TimeZoneInfo.Local.BaseUtcOffset;
		return zone.TotalSeconds;
	}

	public static double Get_TimeZoneGap()
	{
		return m_Server_Timezone.TotalSeconds - Get_TimeZone();
	}

	public void Set_ServerTimeZone(string UtcTimeZone)
	{
		m_Server_Timezone = TimeSpan.Parse(UtcTimeZone);
	}
	public double Get_ZeroTime() {
		return Get_ZeroTime((long)Get_ServerTime_Milli());
	}
	public double Get_ZeroTime(long time)
	{
		return time - time % 86400000;
	}
	public double Get_RemainDayTime(DayOfWeek _day, double _time = 0) {
		DayOfWeek today = GetServerDayofWeek();
		double remaindaytime = 86400000 - ((_time > 0 ? _time : Get_ServerTime_Milli()) - Get_ZeroTime());
		int remainday = today < _day ? _day - today : 7 + _day - today; 
		double remaintargettime = remaindaytime + 86400000 * remainday;
		return remaintargettime * 0.001d;
	}
	public DayOfWeek GetYesterDayofWeek()
	{
		DateTime today = DateTime.Now.AddDays(-1);
		return today.DayOfWeek;
	}
	public DayOfWeek GetDayofWeek()
	{
		DateTime today = DateTime.Now;
		return today.DayOfWeek;
	}

	public DayOfWeek GetServerDayofWeek()
	{
		return Get_ServerDateTime().DayOfWeek;
	}
	public static bool IsSameDay(DateTime dateTime)
	{
		return Get_ServerDateTime().Date == dateTime.Date;
	}
	public bool IsSameDay(long time)
	{
		return (int)(Get_ServerTime_Milli() / 86400000L) == (int)(time / 86400000L);
	}
	public int GetMonth() {
		DateTime today = DateTime.Now;
		return today.Month;
	}
	public enum TimeStyle
	{
		dd_hh_mm_ss = 0,
		hh_mm_ss,
		mm_ss_ff,
		day_hr_min_sec,
		single,//일, 시, 분, 초 독립사용
		ago,
		ago_join,
		hh_mm,
	}
	public string GetSecToTimeStr(double _sec)
	{
		return GetSecToTimeStr(TimeStyle.hh_mm_ss, _sec);
	}

	public string GetSecToTimeStr(TimeStyle style, double _sec) {
		string mode = string.Empty;
		long temp;
		StringBuilder pattern = new StringBuilder(64);
		if (_sec < 0) _sec = 0;
		switch (style)
		{
		case TimeStyle.day_hr_min_sec:
			if (_sec < 60) pattern.Append("s'{3}'");
			else if(_sec < 3600)
			{
				pattern.Append("m'{2}'");
				if ((int)_sec % 60 > 0) pattern.Append("' 's'{3}'");
			}
			else if (_sec < 86400)
			{
				pattern.Append("h'{1}'");
				if (((int)_sec % 3600) / 60 > 0) pattern.Append("' 'm'{2}'");
				if ((int)_sec % 60 > 0) pattern.Append("' 's'{3}'");
			}
			else
			{
				pattern.Append("d'{0}'");
				if (((int)_sec % 86400) / 3600 > 0) pattern.Append("' 'h'{1}'");
				if (((int)_sec % 3600) / 60 > 0) pattern.Append("' 'm'{2}'");
				if ((int)_sec % 60 > 0) pattern.Append("' 's'{3}'");
			}
			mode = string.Format(pattern.ToString(), MainMng.Instance.m_ToolData.GetString(211), MainMng.Instance.m_ToolData.GetString(212), MainMng.Instance.m_ToolData.GetString(213), MainMng.Instance.m_ToolData.GetString(214));
			break;
		case TimeStyle.mm_ss_ff:
			mode = "mm':'ss':'ff";
			break;
		case TimeStyle.hh_mm_ss:
			mode = "hh':'mm':'ss";
			break;
		case TimeStyle.hh_mm:
			mode = "hh':'mm";
			break;
		case TimeStyle.single:
			if (_sec < 60)
				pattern.Append("s'{3}'");
			else if (_sec < 3600)
				pattern.Append("m'{2}'");
			else if (_sec < 86400)
				pattern.Append("h'{1}'");
			else
				pattern.Append("d'{0}'");
			mode = string.Format(pattern.ToString(), MainMng.Instance.m_ToolData.GetString(211), MainMng.Instance.m_ToolData.GetString(212), MainMng.Instance.m_ToolData.GetString(213), MainMng.Instance.m_ToolData.GetString(214));
			break;
		case TimeStyle.ago:
			temp = (long)_sec;
			if (_sec >= 86400) return string.Format(MainMng.Instance.m_ToolData.GetString(651), (long)_sec / 86400);
			else if (_sec >= 3600) return string.Format(MainMng.Instance.m_ToolData.GetString(652), (long)_sec % 86400 / 3600);
			return string.Format(MainMng.Instance.m_ToolData.GetString(653), ((long)_sec % 3600 + 59) / 60);
		case TimeStyle.ago_join:
			temp = (long)_sec;
			if (_sec >= 86400) return string.Format(MainMng.Instance.m_ToolData.GetString(6049), (long)_sec / 86400);
			else if (_sec >= 3600) return string.Format(MainMng.Instance.m_ToolData.GetString(6050), (long)_sec % 86400 / 3600);
			return string.Format(MainMng.Instance.m_ToolData.GetString(6053), ((long)_sec % 3600 + 59) / 60);
		default:
			if (_sec < 60)
				mode = "ss";
			else if (_sec < 3600)
				mode = "mm':'ss";
			else if (_sec < 86400)
				mode = "hh':'mm':'ss";
			else
				mode = "dd'D 'hh':'mm':'ss";
			break;
		}
		return TimeSpan.FromSeconds(_sec).ToString(mode);
	}
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	/// 화면
	public static void Set_AutoScreenSleep(bool On)
	{
		if (On) Screen.sleepTimeout = SleepTimeout.NeverSleep;
		else Screen.sleepTimeout = SleepTimeout.SystemSetting;
	}

	public enum QualityName
	{
		Low = 0,
		Medium,
		High
	}

	public static void SetQualityLevel(QualityName lv)
	{
		if ((int)lv == QualitySettings.GetQualityLevel()) return;
		QualitySettings.SetQualityLevel((int)lv, true);
	}
}
