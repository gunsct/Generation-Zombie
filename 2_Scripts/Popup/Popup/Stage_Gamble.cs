using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Stage_Gamble : PopupBase
{
    [Serializable]
    struct SUI
	{
		public Animator Anim;
		public Sprite[] Numbers;
		public Transform[] NumberGroup;
		public TextMeshProUGUI Prop;
		public Image Card;
		public TextMeshProUGUI Name;
	}
	[SerializeField]
	SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		TGambleCardTable gambletable = (TGambleCardTable)aobjValue[0];
		float checkprop = (float)aobjValue[1];
		int succprop = Mathf.FloorToInt((gambletable.m_SuccProp == 1f ? 0.99f : gambletable.m_SuccProp) * 100);
		bool succ = 1 - gambletable.m_SuccProp > checkprop;
		int randprop = succ ? Mathf.Min(99, UTILE.Get_Random(succprop + 1, 99)) : Mathf.Max(0, UTILE.Get_Random(0, succprop));

		m_SUI.Anim.SetTrigger(succ ? "Win" : "Lose");

		m_SUI.Prop.text = succprop.ToString();

		TStageCardTable cardtable = TDATA.GetStageCardTable(gambletable.m_ResultIdx[succ ? 0 : 1]);
		m_SUI.Card.sprite = cardtable.GetImg();
		m_SUI.Name.text = cardtable.GetNameBuffType();

		int num10 = randprop / 10;//pos 8 end
		int num1 = randprop % 10;//pos 1 end
		for(int i = 0,ten = num10 < 8 ? num10 + 2 : num10 - 8, one = num1 < 1 ? num1 + 1 : num1 - 9; i < 10; i++, ten++, one--) {
			if (ten > 9) ten -= 10;
			else if (ten < 0) ten += 10;
			if (one > 9) one -= 10;
			else if (one < 0) one += 10;
			//one = 10 - one;
			m_SUI.NumberGroup[0].GetChild(i).GetChild(0).GetComponent<Image>().sprite = m_SUI.Numbers[ten];
			m_SUI.NumberGroup[1].GetChild(i).GetChild(0).GetComponent<Image>().sprite = m_SUI.Numbers[one];
		}
		StartCoroutine(IE_Close());
	}

	IEnumerator IE_Close() {
		PlayEffSound(SND_IDX.SFX_0402);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		m_SUI.Anim.SetTrigger("End");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 67f/165f));
		PlayEffSound(SND_IDX.SFX_0403);
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		base.Close();
	}
}
