using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_PDA_Collection_LvUp : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Lv;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Desc;
		public GameObject[] AllGroup;
		public TextMeshProUGUI AllCnt;
	}
	[SerializeField] SUI m_SUI;

	private void OnDisable() {
		Destroy(gameObject);
	}

	public void SetData(TCollectionTable _data, int _cnt = 1) {
		m_SUI.Lv.text = string.Format("Lv.{0}", _data.m_LV);
		m_SUI.Name.text = string.Format("{0} {1}", _data.GetName(), TDATA.GetString(438));
		TCollectionTable pre = TDATA.GetCollectionTable(_data.m_Idx, _data.m_LV - 1);
		float val = _data.m_Stat.m_Value - (pre.m_Stat != null ? pre.m_Stat.m_Value : 0);
		m_SUI.Desc.text = string.Format("{0} +{1}", TDATA.GetStatString(_data.m_Stat.m_Type), _data.m_Stat.m_Type == StatType.Critical ? string.Format("{0:0.0}%", val * 100f) : Mathf.RoundToInt(val).ToString());
		for (int i = 0; i < m_SUI.AllGroup.Length; i++) {
			m_SUI.AllGroup[i].SetActive(_cnt > 1);
		}
		m_SUI.AllCnt.text = string.Format("+{0}", _cnt);

		StartCoroutine(ActiveOff());
	}

	IEnumerator ActiveOff() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		Destroy(gameObject);
	}
}
