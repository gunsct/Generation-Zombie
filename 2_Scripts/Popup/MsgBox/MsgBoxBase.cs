using System;
using System.Collections;
using UnityEngine;

public enum EMsgBtn
{
	// OK 버튼
	BTN_NO = 0
	// YES 버튼
	, BTN_YES
	// 이용약관
	, BTN_Terms_of_Service
	// 개인정보 취급방침
	, BTN_Privacy_Policy
	// 청약철회 방침
	, BTN_Offer
};
public enum EMsgBoxAni
{
	// 시작 연출
	Start = 0
	// 종료 연출
	, Close
};

public class MsgBoxBase : PopupBase
{
	Animator m_Ani;
	// 팝업창 안닫히게 하기
	// 연결순서상 닫히고 각 작업을 하기때문에 막아야하는경우 사용
	// 예 ) 유저 블럭 상태 
	// 기존 : 이메일 보내기 요청 후 어플 종료
	// 변경 : 하이브 문의하기 이용 앱이 종료가되면 하이브 작동이 안되므로 유지 & 하이브 문의하기 창을 닫은후 해당 팝업이 유지가 되어야함
	public bool isCloaseLock = false;
	/// <summary> 연출에 의한 클릭 막기 </summary>
	protected bool m_ISAniPlay { get {  return m_IsAniTransition != null || (!IsSkip && Utile_Class.IsAniPlay(m_Ani)); } }
	IEnumerator m_IsAniTransition = null;
	/// <summary> 애니 연출 상태 막음 스킵하기 </summary>
	public bool IsSkip = false;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		IsSkip = false;
		base.SetData(pos, popup, cb, aobjValue);
		isCloaseLock = false;
		m_Ani = GetComponent<Animator>();
		StartCoroutine(StartCheck());
	}

	IEnumerator StartCheck()
	{
		yield return Utile_Class.CheckAniPlay(m_Ani);
		//if (TUTO.IsTuto(TutoKind.Gacha, (int)TutoType_Gacha.View_CostMsgBox)) TUTO.Next();
	}

	public virtual void SetMsg(string Title, string Msg) {}

	public void PlayAni(EMsgBoxAni AniName)
	{
		if (m_Ani)
		{
			if (m_IsAniTransition != null) StopCoroutine(m_IsAniTransition);
			m_IsAniTransition = AniInTransitionCheck();
			StartCoroutine(m_IsAniTransition);

			m_Ani.SetTrigger(AniName.ToString());
		}
	}

	public virtual void OnNO()
	{
		OnClickBtn((int)EMsgBtn.BTN_NO);
	}

	public virtual void OnYES()
	{
		OnClickBtn((int)EMsgBtn.BTN_YES);
	}

	public virtual void OnClickBtn(int btn)
	{
		if (m_ISAniPlay) return;
		Close(btn);
	}

	public override void Close(int Result = 0)
	{
		if (m_Popup == PopupName.Msg_None) return;
		if (m_Popup != PopupName.Msg_CenterAlarm && TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (isCloaseLock) return;
		//if (TUTO.IsTuto(TutoKind.Gacha, (int)TutoType_Gacha.Select_CostMsgBox_Yes)) TUTO.Next();
		StartCoroutine(Close((EMsgBtn)Result));
	}


	public IEnumerator Close(EMsgBtn btn)
	{
		PlayAni(EMsgBoxAni.Close);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile (() => m_ISAniPlay);
		base.Close((int)btn);
	}
	IEnumerator AniInTransitionCheck()
	{
		// 애니가 시작된후에는 한프레임 쉬어주어야함
		yield return new WaitForFixedUpdate();
		m_IsAniTransition = null;
	}
}
