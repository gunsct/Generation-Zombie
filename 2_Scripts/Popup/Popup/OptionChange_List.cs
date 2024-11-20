using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OptionChange_List : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Transform Element;//Item_OptionChange_List_Element
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;
	int m_RandStatGroup;
	
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_RandStatGroup = (int)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		TRandomStatGroup group = TDATA.GetRandomStatGroup(m_RandStatGroup);
		List<TRandomStatTable> tdatas = group.m_List.FindAll(o => o.m_Prob > 0);
		tdatas.Sort((TRandomStatTable _before, TRandomStatTable _after) => {
			return _before.m_Prob.CompareTo(_after.m_Prob);
		});
		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0;i< tdatas.Count; i++) {
			Item_OptionChange_List_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_OptionChange_List_Element>();
			element.SetData(tdatas[i]);
		}
		base.SetUI();
	}
}
