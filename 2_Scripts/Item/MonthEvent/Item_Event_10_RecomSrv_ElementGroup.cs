using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Event_10_RecomSrv_ElementGroup : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Image JobIcon;
		public TextMeshProUGUI JobName;
		public Transform Bucket;
		public Transform Element;   //Item_Event_10_RecomSrv_Element
	}
	[SerializeField] SUI m_SUI;
	
	public void SetData(JobType _type, List<FAEventData_StageChar> _chars) {
		TSynergyTable tdata = TDATA.GetSynergyTable(_type);
		m_SUI.JobIcon.sprite = tdata.GetIcon();
		m_SUI.JobName.text = tdata.GetName();

		UTILE.Load_Prefab_List(_chars.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < _chars.Count; i++) {
			Item_Event_10_RecomSrv_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_Event_10_RecomSrv_Element>();
			element.SetData(_chars[i]);
		}
	}
}
