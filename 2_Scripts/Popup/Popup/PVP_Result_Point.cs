using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using UnityEngine.UI;

public class PVP_Result_Point : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject[] WInLosePanel;   //0:win 1:lose
		public Image[] Arrows;				//0:시즌1:리그
		public TextMeshProUGUI[] LPTxt;		//0:현재1:증감
		public TextMeshProUGUI[] SPTxt;     //0:현재1:증감
	}
	[SerializeField] SUI m_SUI;
	RES_PVP_USER_DETAIL m_PreInfo;			//이전 점수
	RES_PVP_END m_NowInfo;                  //현재 점수
	int[] m_LP = new int[2];				//이전 점수, 증감 점수
	int[] m_SP = new int[2];				//이전 점수, 증감 점수
	bool Is_Win;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PreInfo = (RES_PVP_USER_DETAIL)aobjValue[0];
		m_NowInfo = (RES_PVP_END)aobjValue[1];
		Is_Win = (bool)aobjValue[2];
		m_SP[0] = (int)m_PreInfo.Point[0]; 
		m_SP[1] = m_NowInfo.Point[0];
		m_LP[0] = (int)m_PreInfo.Point[1];
		m_LP[1] = m_NowInfo.Point[1];

		m_SUI.Anim.SetTrigger(Is_Win ? "Victory" : "Defeat");
		base.SetData(pos, popup, cb, aobjValue);
	}

	public override void SetUI() {
		base.SetUI();

		m_SUI.WInLosePanel[0].SetActive(Is_Win);
		m_SUI.WInLosePanel[1].SetActive(!Is_Win);

		TW_SPCounting(Mathf.Max(0f, m_SP[0]));
		TW_LPCounting(Mathf.Max(0f, m_LP[0]));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_SP[0], "to", Mathf.Max(0f, m_SP[0] + m_SP[1]), "time", 1.5f, "delay", 0.5f, "onupdate", "TW_SPCounting"));
		iTween.ValueTo(gameObject, iTween.Hash("from", m_LP[0], "to", Mathf.Max(0f, m_LP[0] + m_LP[1]), "time", 1.5f, "delay", 0.5f, "onupdate", "TW_LPCounting"));

		m_SUI.Arrows[0].gameObject.SetActive(m_SP[1] != 0);
		m_SUI.Arrows[0].sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_SV_{0}", 0 < m_SP[1] ? "Up" : "Down"), "png");
		m_SUI.SPTxt[1].gameObject.SetActive(m_SP[1] != 0);
		m_SUI.SPTxt[1].text = Utile_Class.CommaValue(m_SP[1]);
		m_SUI.SPTxt[1].color = 0 < m_SP[1] ? Utile_Class.GetCodeColor("98FF86") : Utile_Class.GetCodeColor("FF693D");

		m_SUI.Arrows[1].gameObject.SetActive(m_LP[1] != 0);
		m_SUI.Arrows[1].sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_SV_{0}", 0 < m_LP[1] ? "Up" : "Down"), "png");
		m_SUI.LPTxt[1].gameObject.SetActive(m_LP[1] != 0);
		m_SUI.LPTxt[1].text = Utile_Class.CommaValue(m_LP[1]);
		m_SUI.LPTxt[1].color = 0 < m_LP[1] ? Utile_Class.GetCodeColor("98FF86") : Utile_Class.GetCodeColor("FF693D");
	}

	void TW_SPCounting(float _amount) {
		m_SUI.SPTxt[0].text = Utile_Class.CommaValue(Mathf.RoundToInt(_amount));
	}
	void TW_LPCounting(float _amount) {
		m_SUI.LPTxt[0].text = Utile_Class.CommaValue(Mathf.RoundToInt(_amount));
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
