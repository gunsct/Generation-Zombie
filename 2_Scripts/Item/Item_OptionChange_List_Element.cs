using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_OptionChange_List_Element : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Prob;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(TRandomStatTable _tdata) {
		m_SUI.Name.text = string.Format("{0} : {1:0.#}% ~ {2:0.#}%", TDATA.GetStatString(_tdata.m_Stat), (float)_tdata.m_Val[0] / 100f, (float)_tdata.m_Val[1] / 100f);
		m_SUI.Prob.text = string.Format("{0:0.##}%", (float)_tdata .m_Prob / (float)TDATA.GetRandomStatGroup(_tdata.m_StatGroup).m_TotalProb * 100f);
	}
}
