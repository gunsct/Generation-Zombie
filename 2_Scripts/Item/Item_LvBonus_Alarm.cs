using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class Item_LvBonus_Alarm : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI[] Title;
		public Transform Bucket;
		public Transform Element;   //Item_LvBonus_StatGroup
	}
	[SerializeField] SUI m_SUI;
	public void SetData(Dictionary<StatType, float> _vals, int _lv) {
		m_SUI.Title[0].text = m_SUI.Title[1].text = string.Format(TDATA.GetString(10075), _lv);
		UTILE.Load_Prefab_List(_vals.Count, m_SUI.Bucket, m_SUI.Element);
		for(int i = 0; i < _vals.Count; i++) {
			KeyValuePair<StatType, float> stat = _vals.ElementAt(i);
			m_SUI.Bucket.GetChild(i).GetComponent<Item_LvBonus_StatGroup>().SetData(stat.Key, stat.Value);
		}
		StartCoroutine(IE_AutoDestroy());
	}
	IEnumerator IE_AutoDestroy() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 10f / 110f));
		for (int i = 0; i < m_SUI.Bucket.childCount; i++) {
			m_SUI.Bucket.GetChild(i).GetComponent<Item_LvBonus_StatGroup>().SetStartAnim();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, (10f + 15f * (i + 1)) / 110f));
		}

		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Destroy(gameObject);
	}
}
