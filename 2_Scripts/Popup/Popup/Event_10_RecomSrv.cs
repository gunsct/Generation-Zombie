using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Event_10_RecomSrv : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Transform Bucket;
		public Transform Element;
	}
	[SerializeField] SUI m_SUI;
	List<FAEventData_StageChar> m_Chars = new List<FAEventData_StageChar>();
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Chars = (List<FAEventData_StageChar>)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {

		Dictionary<JobType, List<FAEventData_StageChar>> jobtochars = new Dictionary<JobType, List<FAEventData_StageChar>>();
		for(int i = 0;i< m_Chars.Count; i++) {
			TCharacterTable tdata = TDATA.GetCharacterTable(m_Chars[i].Idx);
			if (!jobtochars.ContainsKey(tdata.m_Job[0])) jobtochars.Add(tdata.m_Job[0], new List<FAEventData_StageChar>());
			jobtochars[tdata.m_Job[0]].Add(m_Chars[i]);
		}
		UTILE.Load_Prefab_List(jobtochars.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < jobtochars.Count; i++) {
			Item_Event_10_RecomSrv_ElementGroup element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Event_10_RecomSrv_ElementGroup>();
			element.SetData(jobtochars.ElementAt(i).Key, jobtochars.ElementAt(i).Value);
		}
		base.SetUI();
	}
}
