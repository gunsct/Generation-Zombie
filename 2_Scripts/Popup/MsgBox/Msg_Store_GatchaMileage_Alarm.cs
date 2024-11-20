using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Msg_Store_GatchaMileage_Alarm : MsgBoxBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Cnt;
	}
    [SerializeField] SUI m_SUI;
	int m_Add;
	IEnumerator m_CountingCor;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_Add = (int)aobjValue[0];

		//Set_MsgBox 할때 Msg_Challenge_Alarm, Msg_Store_GatchaMileage_Alarm 두개는 풀링식으로 리스트에 넣고 꺼질때까지 기다리다 나오게 UIMng에 추가
	}
	public override void SetUI() {
		base.SetUI();
		m_CountingCor = IE_Counting();
		StartCoroutine(m_CountingCor);
	}
	public void OnUpStart()
	{
		return;
	}
	IEnumerator IE_Counting() {
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 34f / 110f));

		iTween.ValueTo(gameObject, iTween.Hash("from", 0, "to", m_Add, "onupdate", "TW_Counting", "time", 1f, "name", "GaugeAction"));

		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		// 자동 닫힘
		OnNO();
	}
	void TW_Counting(float _amount) {
		m_SUI.Cnt.text = Mathf.RoundToInt(_amount).ToString();
	}
	bool IsEnd = false;
	public void OnClick()
	{
		if (IsEnd) return;
		IsEnd = true;
		if (m_CountingCor != null)
		{
			StopCoroutine(m_CountingCor);
			m_CountingCor = null;
		}
		iTween.StopByName(gameObject, "GaugeAction");
		m_SUI.Cnt.text = Mathf.RoundToInt(m_Add).ToString();
		IsSkip = true;
		// 자동 닫힘
		OnNO();
	}
}
