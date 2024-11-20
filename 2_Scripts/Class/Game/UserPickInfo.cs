using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static LS_Web;

public class UserPickChar
{
	/// <summary> 인덱스 </summary>
	public int Idx;
	/// <summary> 횟수 </summary>
	public int Cnt;
}
public class UserPickCharInfo
{
	/// <summary> 스테이지 컨텐츠 </summary>
	public StageContentType Type;
	/// <summary> 플레이 할 요일(요일던전용) </summary>
	public DayOfWeek Week;
	/// <summary> 위치 </summary>
	public int Pos;
	/// <summary> Stage : 인덱스, 던전 : 레벨 </summary>
	public int Idx;
	/// <summary> 총 합 </summary>
	public long Total;
	/// <summary> 선택 정보 최대 5개 </summary>
	public List<UserPickChar> Chars = new List<UserPickChar>();
}


