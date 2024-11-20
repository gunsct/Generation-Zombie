using System.Collections.Generic;


public class MyCharEva
{
	/// <summary> 점수(0~10 1점당 0.5) </summary>
	public int Point = 0;
	/// <summary> 내 평가 정보 </summary>
	public List<EvaData> values = new List<EvaData>();
}

public class EvaData
{
	/// <summary> 평가 위치 </summary>
	public int Pos = 0;
	/// <summary> 평가 인덱스 </summary>
	public int Value = 0;
}

/// <summary> 총 평가 인원수는 EvaCnt.Cnt의 총 합으로 계산 계산후 반 올림할것 0.25 = 0</summary>
public class CharEva
{
	/// <summary> 총 점수 </summary>
	public long Point = 0;
	/// <summary> 총 평가 정보 평가 정보 </summary>
	public List<EvaCnt> Info = new List<EvaCnt>();
}

public class EvaCnt
{
	/// <summary> 평가 위치 </summary>
	public int Idx = 0;
	/// <summary> 평가 인덱스 </summary>
	public int Cnt = 0;
}

