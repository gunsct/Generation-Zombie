using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

public class Item_StgDebuffCardAlarm : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public GameObject Prefab;//Item_StgDebuffCardAlarmElement
	}
	[SerializeField] SUI m_SUI;
	Dictionary<StageCardType, Item_StgDebuffCardAlarmElement> m_Debuffs = new Dictionary<StageCardType, Item_StgDebuffCardAlarmElement>();
	public void AddData(KeyValuePair<StageCardType, float> _val) {
		if (!m_Debuffs.ContainsKey(_val.Key)) {
			Item_StgDebuffCardAlarmElement element = Utile_Class.Instantiate(m_SUI.Prefab, transform).GetComponent<Item_StgDebuffCardAlarmElement>();
			element.SetData(_val.Key, _val.Value);
			m_Debuffs.Add(_val.Key, element);
		}
	}
	public void DeleteData(StageCardType _type) {
		if (m_Debuffs.ContainsKey(_type)) {
			Destroy(m_Debuffs[_type].gameObject);
			m_Debuffs.Remove(_type);
		}
	}
	public void OffInfo() {
		for(int i = 0; i < m_Debuffs.Count; i++) {
			m_Debuffs.ElementAt(i).Value.ViewInfo(false);
		}
	}

	public bool IS_OnInfo() {
		for (int i = 0; i < m_Debuffs.Count; i++) {
			if(m_Debuffs.ElementAt(i).Value.IS_OnAalarm()) return true;
		}
		return false;
	}

	public void RefreshCount(StageCardType _type, int _cnt) {
		if(m_Debuffs.ContainsKey(_type)) m_Debuffs[_type].RefreshCount(_cnt);
	}
}
