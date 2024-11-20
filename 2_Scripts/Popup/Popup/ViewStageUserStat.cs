using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class ViewStageUserStat : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Transform Txt;
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		UTILE.Load_Prefab_List(STAGE_USERINFO.m_Stat.GetLength(0), m_SUI.Bucket, m_SUI.Txt);

		for(int i = 0;i< STAGE_USERINFO.m_Stat.GetLength(0); i++) {
			Text txt = m_SUI.Bucket.GetChild(i).GetComponent<Text>();
			float[] stat = new float[2];
			stat[0] = STAGE_USERINFO.GetStat((StatType)i);
			stat[1] = STAGE_USERINFO.GetMaxStat((StatType)i);
			string ccode = "#FFFFFF";
			if (stat[0] < stat[1]) ccode = "#D2533C";
			else if(stat[0] > stat[1]) ccode = "#498E41";
			else ccode = "#FFFFFF";

			string now = string.Format("<color={0}>{1}</color>", ccode, stat[0]);
			txt.text = string.Format("{0} \n기본:{1} // 현재:{2}", i == 3 ? "HP" : ((StatType)i).ToString(), stat[1], now);
		}
	}
}
