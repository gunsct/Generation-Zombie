using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[System.Serializable] public class DicSortingCheckObj : SerializableDictionary<SortingType, GameObject> { }
public class SortingOption : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public DicSortingCheckObj Select;
	}
	[SerializeField] SUI m_SUI;
	[HideInInspector] public SortingType m_Type;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_Type = (SortingType)aobjValue[0];
		Check_SelectObj();
		Dictionary<SortingType, bool> usetype = (Dictionary<SortingType, bool>)aobjValue[1];
		for (int i = 0; i < usetype.Count; i++) {
			KeyValuePair<SortingType, bool> val = usetype.ElementAt(i);
			m_SUI.Select[val.Key].transform.parent.gameObject.SetActive(val.Value);
			switch(val.Key)
			{
			case SortingType.FILTER_Grade_1:
			case SortingType.FILTER_Grade_2:
			case SortingType.FILTER_Grade_3:
			case SortingType.FILTER_Grade_4:
			case SortingType.FILTER_Grade_5:
			case SortingType.FILTER_Grade_6:
			case SortingType.FILTER_Grade_7:
			case SortingType.FILTER_Grade_8:
			case SortingType.FILTER_Grade_9:
			case SortingType.FILTER_Grade_10:
				m_SUI.Select[val.Key].transform.parent.GetComponent<Item_Button_SortOption>().SetName(string.Format(TDATA.GetString(1107), val.Key - SortingType.FILTER_Grade_1 + 1));
				break;
			}
			
		}
	}

	void Check_SelectObj()
	{
		foreach (var obj in m_SUI.Select)
		{
			if (obj.Value == null) continue;
			obj.Value.SetActive(obj.Key == m_Type);
		}
	}

	[EnumAction(typeof(SortingType))]
	public void ClickType(int _type) {
		if (m_Action != null) return;
		m_Type = (SortingType)_type;
		Check_SelectObj();
		Close(_type);
	}
	public void ClickClose() {
		Close((int)m_Type);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
