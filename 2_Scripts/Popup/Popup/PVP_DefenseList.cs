using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;

public class PVP_DefenseList : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Transform Bucket;
		public Transform Element;	//Item_PVP_DefenseList_Element
	}
	[SerializeField] SUI m_SUI;
	List<REQ_CAMP_PLUNDER_LOG_DATA> m_PVPDefLog;
	Action<long, int> m_CB;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PVPDefLog = (List<REQ_CAMP_PLUNDER_LOG_DATA>)aobjValue[0];
		m_CB = (Action<long, int>)aobjValue[1];

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		if (m_PVPDefLog.Count > 0) {
			UTILE.Load_Prefab_List(m_PVPDefLog.Count, m_SUI.Bucket, m_SUI.Element);
			for (int i = 0; i < m_PVPDefLog.Count; i++) {
				Item_PVP_DefenseList_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PVP_DefenseList_Element>();
				element.SetData(m_PVPDefLog[i], Click_Revenge);
			}
		}
		base.SetUI();
	}
	void Click_Revenge(long _uid, int _idx) {
		m_CB?.Invoke(_uid, _idx);
		Close(0);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
