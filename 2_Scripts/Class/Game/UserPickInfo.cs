using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using static LS_Web;

public class UserPickChar
{
	/// <summary> �ε��� </summary>
	public int Idx;
	/// <summary> Ƚ�� </summary>
	public int Cnt;
}
public class UserPickCharInfo
{
	/// <summary> �������� ������ </summary>
	public StageContentType Type;
	/// <summary> �÷��� �� ����(���ϴ�����) </summary>
	public DayOfWeek Week;
	/// <summary> ��ġ </summary>
	public int Pos;
	/// <summary> Stage : �ε���, ���� : ���� </summary>
	public int Idx;
	/// <summary> �� �� </summary>
	public long Total;
	/// <summary> ���� ���� �ִ� 5�� </summary>
	public List<UserPickChar> Chars = new List<UserPickChar>();
}


