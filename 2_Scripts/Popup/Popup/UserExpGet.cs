using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class UserExpGet : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI AddExp;
		public Image Portrait;
		public TextMeshProUGUI Lv;
		public TextMeshProUGUI Exp;
		public Image ExpGague;
		public TextMeshProUGUI UpLv;
	}
	[SerializeField] SUI m_SUI;
	RES_REWARD_USEREXP m_UserExp = new RES_REWARD_USEREXP();
	int m_BLv;
	long m_BExp;
	long m_AExp;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		m_UserExp = (RES_REWARD_USEREXP)aobjValue[0];
		StartCoroutine(Action());
	}
	IEnumerator Action() {
		//계정 경험치 받는 경우
		if (m_UserExp.AExp > 0) {

			m_SUI.Portrait.sprite = TDATA.GetUserProfileImage(USERINFO.m_Profile);
			m_SUI.AddExp.text = string.Format("+ {0}", m_UserExp.AExp.ToString("N0"));
			m_SUI.Exp.text = string.Format("{0} / {1}", 0, TDATA.GetExpTable(m_UserExp.NLV).m_UserExp.ToString("N0"));
			m_SUI.Lv.text = m_SUI.UpLv.text = string.Format("Lv. {0}", m_UserExp.NLV.ToString("N0"));

			m_SUI.Anim.SetTrigger("1_Exp");
			yield return new WaitForEndOfFrame();
			yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim, 100f / 133f));
			float time = 0.8f;

			PlayEffSound(SND_IDX.SFX_1060);

			m_BLv = m_UserExp.BLV;
			m_BExp = m_UserExp.BExp;
			m_AExp = 0;
			long nowexpmax = TDATA.GetExpTable(m_BLv).m_UserExp;
			m_SUI.Lv.text = string.Format("Lv. {0}", m_BLv.ToString("N0"));

			TExpTable bedata = TDATA.GetExpTable(m_BLv);
			if (USERINFO.m_Exp[0] >= bedata.m_UserExp) {
				m_SUI.Exp.text = string.Format("{0} / {1}", bedata.m_UserExp, bedata.m_UserExp);
				m_SUI.ExpGague.fillAmount = 1f;
			}
			else {
				m_SUI.Exp.text = string.Format("{0} / {1}", m_BExp.ToString("N0"), nowexpmax.ToString("N0"));
				m_SUI.ExpGague.fillAmount = Mathf.Clamp(m_BExp / (float)nowexpmax, 0f, 1f);
				iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", (float)m_UserExp.AExp, "onupdate", "TW_UserExp", "time", time, "easetype", "linear"));
			}

			yield return new WaitForSeconds(time + 0.2f);
			yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));
			//받은 경험치로 레벨업 하는 경우
			if (m_UserExp.BLV != m_UserExp.NLV) {
				m_SUI.Anim.SetTrigger("3_LvUp");
				yield return new WaitForEndOfFrame();
				yield return new WaitForSeconds(0.15f);

				m_SUI.Lv.text = m_SUI.UpLv.text = string.Format("Lv. {0}", m_UserExp.NLV.ToString("N0"));

				yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_SUI.Anim));
			}
			else
				m_SUI.Anim.SetTrigger("2_Touch");
		}
	}
	void TW_UserExp(float _amount) {
		float exp = _amount - m_AExp;
		float nowexp = m_BExp + exp;
		long nowexpmax = TDATA.GetExpTable(m_BLv).m_UserExp;
		if (nowexp > nowexpmax) {
			nowexp = nowexpmax;
			m_AExp = (long)_amount;
			m_BExp = 0; 
			if (m_BLv < BaseValue.CHAR_MAX_LV) { 
				m_BLv++;
				TW_UserLvCounting(m_BLv);
				DelayPlayFXSND(1.5f, SND_IDX.SFX_0111);
			}
		}
		m_SUI.Exp.text = string.Format("{0} / {1}", nowexp.ToString("N0"), nowexpmax.ToString("N0"));
		m_SUI.ExpGague.fillAmount = Mathf.Clamp(nowexp / (float)nowexpmax, 0f, 1f);
	}
	void TW_UserLvCounting(float _amount) {
		m_SUI.Lv.text = m_SUI.UpLv.text = string.Format("Lv. {0}", _amount.ToString("N0"));
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
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 1f, 1));
		base.Close(_result);
	}
}
