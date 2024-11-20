using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DNAOptionChange_List : PopupBase
{
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Transform Element;   // Item_OptionChange_List_Element
		public Transform Bucket;
	}
	[SerializeField] SUI m_SUI;
	int m_Gid;

	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Gid = (int)aobjValue[0];

		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		List<TRandomStatTable> tdatas = TDATA.GetRandomStatGroup(m_Gid).m_List;
		tdatas.Sort((TRandomStatTable _b, TRandomStatTable _a)=>{
			return _a.m_Prob.CompareTo(_b.m_Prob);
		});
		UTILE.Load_Prefab_List(tdatas.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < tdatas.Count; i++) {
			Item_OptionChange_List_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_OptionChange_List_Element>();
			element.SetData(tdatas[i]);
		}

		base.SetUI();
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
