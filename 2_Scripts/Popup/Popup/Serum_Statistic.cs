using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Serum_Statistic : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image[] BlockNum;
		public TextMeshProUGUI CharName;
		public GameObject StatElement;//Item_SerumStat
		public Transform[] StatBuckets;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	CharInfo m_Char;
	int m_BlockNum;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_Char = (CharInfo)aobjValue[0];
		m_BlockNum = (int)aobjValue[1];

		SetBlockFont(m_BlockNum);

		m_SUI.CharName.text = m_Char.m_TData.GetCharName();

		for (int i = 0; i < (int)StatType.Max; i++) {
			float selfabs = m_Char.GetSelfSerum((StatType)i, StatValType.ABS, SerumTargetType.Self);
			float selfratio = m_Char.GetSelfSerum((StatType)i, StatValType.Ratio, SerumTargetType.Self);
			float allabs = USERINFO.GetAllSerum((StatType)i, StatValType.ABS);
			float allratio = USERINFO.GetAllSerum((StatType)i, StatValType.Ratio);
			if (selfabs > 0f || selfratio > 0f) {
				Item_SerumStat selfstat = Instantiate(m_SUI.StatElement, m_SUI.StatBuckets[0]).GetComponent<Item_SerumStat>();
				selfstat.SetData((StatType)i, selfabs, selfratio);
			}
			if (allabs > 0f || allratio > 0f) {
				Item_SerumStat allstat = Instantiate(m_SUI.StatElement, m_SUI.StatBuckets[1]).GetComponent<Item_SerumStat>();
				allstat.SetData((StatType)i, allabs, allratio);
			}
		}
	}

	void SetBlockFont(int _block) {
		int block10 = Mathf.Clamp(_block / 10, 0, 9);
		int block1 = Mathf.Clamp(_block % 10, 0, 9);
		m_SUI.BlockNum[0].sprite = UTILE.LoadImg(string.Format("UI/UI_Serum/Font_Number_{0}", block10), "png");
		m_SUI.BlockNum[1].sprite = UTILE.LoadImg(string.Format("UI/UI_Serum/Font_Number_{0}", block1), "png");
	}

	public void ClickClose() {
		if (m_Action != null) return;
		StartCoroutine(m_Action = IE_CloseAction(0));
	}
	IEnumerator IE_CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		Close(_result);
	}
}
