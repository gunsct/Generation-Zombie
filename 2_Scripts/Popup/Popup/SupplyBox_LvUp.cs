using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class SupplyBox_LvUp : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image[] Lv;
		public TextMeshProUGUI[] Plus;//0:경험치,1:달러,2:금니,3:계정경험치
		public Sprite[] Nums;
		public Image BoxGradeIcon;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		int prelv = (int)aobjValue[0];
		int nowlv = (int)aobjValue[1];
		TSupplyBoxTable predata = TDATA.GetSupplyBoxTable(prelv);
		TSupplyBoxTable nowdata = TDATA.GetSupplyBoxTable(nowlv);

		m_SUI.Lv[0].sprite = m_SUI.Nums[nowlv % 10]; 
		m_SUI.Lv[1].sprite = m_SUI.Nums[nowlv % 100 / 10]; 
		m_SUI.Lv[2].sprite = m_SUI.Nums[nowlv / 100];
		
		m_SUI.Plus[0].text = string.Format("+{0}", Utile_Class.CommaValue(Mathf.RoundToInt((nowdata.Exp[0] + nowdata.Exp[1] - predata.Exp[0] - predata.Exp[1]) * 0.5f)));
		m_SUI.Plus[1].text = string.Format("+{0}", Utile_Class.CommaValue(Mathf.RoundToInt((nowdata.Dollar[0] + nowdata.Dollar[1] - predata.Dollar[0] - predata.Dollar[1]) * 0.5f)));
		//m_SUI.Plus[2].text = string.Format("+{0}", Utile_Class.CommaValue(Mathf.RoundToInt((nowdata.Cash[0] + nowdata.Cash[1] - predata.Cash[0] - predata.Cash[1]) * 0.5f)));
		m_SUI.Plus[3].text = string.Format("+{0}", Utile_Class.CommaValue(Mathf.RoundToInt((nowdata.UserExp[0] + nowdata.UserExp[1] - predata.UserExp[0] - predata.UserExp[1]) * 0.5f)));

		int grade = USERINFO.m_SupplyBoxLV + Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.SupplyBoxGradeUp));
		m_SUI.BoxGradeIcon.sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Icon_SupplyBox_{0}", Mathf.Min((int)(grade / 2) + 1, 5)), "png");

		PlayEffSound(SND_IDX.SFX_0111);
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
