using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

public class PVP_Research_EffectList : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Transform Bucket;
		public Transform Element;       //Item_PVP_Research_EffectList_Element
		public GameObject Empty;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		List<ResearchInfo> infos = new List<ResearchInfo>();
		for (ResearchType i = ResearchType.Camp_Attack; i <= ResearchType.Camp_Defense_Steal; i++) {
			infos.AddRange(USERINFO.ResearchInfos(i).FindAll(o=>o.m_GetLv > 0));
		}
		if (infos.Count > 0) {
			UTILE.Load_Prefab_List(infos.Count, m_SUI.Bucket, m_SUI.Element);
			for (int i = 0; i < infos.Count; i++) {
				Item_PVP_Research_EffectList_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PVP_Research_EffectList_Element>();
				var tdata = infos[i].m_TData;
				element.SetData(tdata.GetIcon(), tdata.GetName(), USERINFO.ResearchValue(tdata.m_Eff.m_Eff, true));
			}
		}
		m_SUI.Empty.SetActive(infos.Count == 0);

		base.SetData(pos, popup, cb, aobjValue);
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
