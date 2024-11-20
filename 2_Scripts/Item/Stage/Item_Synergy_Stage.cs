using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Synergy_Stage : ObjMng
{
	[Serializable]
	public struct SUI {
		public Animator Anim;
		public Image Icon;
		public GameObject InfoGroup;
		public TextMeshProUGUI InfoName;
		public TextMeshProUGUI InfoDesc;
		public TextMeshProUGUI InfoCnt;
	}
	[SerializeField] SUI m_SUI;

	public JobType m_SynergyType;
	TSynergyTable m_TData { get { return TDATA.GetSynergyTable(m_SynergyType); } }

	private void Awake() {
		m_SUI.InfoGroup.SetActive(false);
	}
	public void SetData(JobType _type) {
		m_SynergyType = _type;
		m_SUI.Icon.sprite = m_TData.GetIcon();
		m_SUI.InfoName.text = m_TData.GetName();
		m_SUI.InfoDesc.text = m_TData.GetDesc();
		m_SUI.InfoCnt.text = m_TData.m_NeedCount.ToString();
		SetAnim();
	}

	public void SetAnim() {
		if (m_TData.GetActiveType() == SynergyActiveType.Active) m_SUI.Anim.SetTrigger("Act");
		else m_SUI.Anim.SetTrigger("ActKeep");
	}

	public void ViewInfo(bool _on) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.StageSynergyToolTip, (int)m_SynergyType)) return;
		if (_on) {
			StartCoroutine(ViewInfoOnDelay());
		}
		else {
			m_SUI.InfoGroup.SetActive(false);
		}
	}
	IEnumerator ViewInfoOnDelay() {
		yield return new WaitForEndOfFrame();
		m_SUI.InfoGroup.SetActive(true);
	}
	public bool IS_ShowInfo() {
		return m_SUI.InfoGroup.activeSelf;
	}
}
