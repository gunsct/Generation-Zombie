using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Stage_MakingInfo : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public GameObject InfoPrefab;
		public Transform Bucket;
		public GameObject[] Panels;
		public GameObject[] DebuffFX;
		public GameObject CloseBtn;
	}
	[SerializeField]
	SUI m_SUI;
	List<TStageMakingTable> m_CanMakes = new List<TStageMakingTable>();
	bool Is_Debuff;

	public GameObject GetCloseBtn { get { return m_SUI.CloseBtn; } }
	public GameObject GetFirstMerge { get { return m_SUI.Bucket.GetChild(0).gameObject; } }
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_CanMakes = (List<TStageMakingTable>)aobjValue[0];
		Is_Debuff = (bool)aobjValue[1];
		for(int i = 0;i<m_SUI.DebuffFX.Length;i++) m_SUI.DebuffFX[i].SetActive(Is_Debuff);
		SetList();
	}
	/// <summary> 제작 가능 리스트업 </summary>
	void SetList() {
		m_CanMakes.Sort((TStageMakingTable _a, TStageMakingTable _b)=>{
			return _b.m_Condition.m_Value.CompareTo(_a.m_Condition.m_Value);
		});

		for (int i = 0;i< m_CanMakes.Count; i++) {
			Item_Stage_Maing_InfoElement element = Utile_Class.Instantiate(m_SUI.InfoPrefab, m_SUI.Bucket).GetComponent<Item_Stage_Maing_InfoElement>();
			int step = TDATA.GetMakingGrade(m_CanMakes[i].m_MatType);
			element.SetData(m_CanMakes[i], Is_Debuff && step == 1);
		}
	}

	public GameObject GetScrollPanel()
	{
		return m_SUI.Panels[0];
	}

	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		base.Close(Result);
	}
}
