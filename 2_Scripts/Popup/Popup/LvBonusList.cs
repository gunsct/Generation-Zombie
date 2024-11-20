using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

public class LvBonusList : PopupBase
{
   [Serializable]
   public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI LV;
		public Transform Bucket;
		public Transform Element;   //Item_Info_Char_DNAStat_Sub
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		Dictionary<StatType, float> stats = USERINFO.GetCharLvStatBonus();
		m_SUI.LV.text = USERINFO.m_Chars.Sum(o => o.m_LV).ToString();
		UTILE.Load_Prefab_List(stats.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < stats.Count; i++) {
			Item_Info_Char_DNAStat_Sub element = m_SUI.Bucket.GetChild(i).GetComponentInChildren<Item_Info_Char_DNAStat_Sub>();
			element.SetData(stats.ElementAt(i).Key, stats.ElementAt(i).Value, stats.ElementAt(i).Key == StatType.Critical);
		}
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
