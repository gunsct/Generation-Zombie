using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TDamageAdjustmentTable : ClassMng
{
	/// <summary> 전투력 차 배율 민맥스 </summary>
	public float[] MinMax = new float[2];
	/// <summary> 피격시 데미지 배율 0:에너미, 1:플레이어</summary>
	public float[] Ratio = new float[2];

	public TDamageAdjustmentTable(CSV_Result pResult)
	{
		MinMax[0] = pResult.Get_Float();
		MinMax[1] = pResult.Get_Float();
		Ratio[0] = pResult.Get_Float();
		Ratio[1] = pResult.Get_Float();
	}
}
public class TDamageAdjustmentTableMng : ToolFile
{
	public List<TDamageAdjustmentTable> Datas = new List<TDamageAdjustmentTable>();

	public TDamageAdjustmentTableMng() : base("Datas/DamageAdjustmentTable")
	{
	}

	public override void DataInit()
	{
		Datas.Clear();
	}

	public override void ParsLine(CSV_Result pResult)
	{
		TDamageAdjustmentTable data = new TDamageAdjustmentTable(pResult);
		Datas.Add(data);
	}
}

public partial class ToolData : ClassMng
{
	//////////////////////////////////////////////////////////////////////////////////////////////////
	// DamageAdjustmentTable
	TDamageAdjustmentTableMng m_DamageAdjustment = new TDamageAdjustmentTableMng();

	public TDamageAdjustmentTable GetDamageAdjustmentTable(float _ratio) {
		for (int i = 0; i < m_DamageAdjustment.Datas.Count; i++) {
			var data = m_DamageAdjustment.Datas[i];
			if (data.MinMax[0] <= _ratio && data.MinMax[1] > _ratio) return data;
		}
		return null;
	}
}