using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PVPRanking : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Timer;
	}
	[Serializable]
	public struct SSUI//시즌 정보
	{
		public Transform[] Bucket;//0 전체 랭킹, 1 본인
		public Transform Element;//Item_PvPRanking_Element;
	}
	[Serializable]
	public struct SGUI//그룹 정보
	{
		public Transform[] Bucket;//0 전체 랭킹, 1 본인
		public Transform Element;//Item_PvPRanking_Element;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SSUI m_SSUI;
	[SerializeField] SGUI m_SGUI;

	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		//m_Userinfos 가지고 유아이 세팅
	}

	public void ClickSetTab(int _pos) {

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
