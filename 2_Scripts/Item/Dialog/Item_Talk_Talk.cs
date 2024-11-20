using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Talk_Talk : ObjMng
{
	[System.Serializable]
	struct SUI{
		public DialogTalkType Type;
		public Animator Ani;
		public TextMeshProUGUI Speech;
		public GameObject[] Tails;
		public TextMeshProEffect TextFX;
	}
	[SerializeField]
	SUI m_SUI;
	public GameObject CrntTail;
	bool Is_Fold = false;

	private void Awake() {
		if(m_SUI.TextFX != null) m_SUI.TextFX.enabled = false;
	}
	public bool IsAction()
	{
		return !m_SUI.TextFX.IsFinished;
	}

	public void Set_Action_Finishied() {
		if (m_SUI.TextFX != null && m_SUI.TextFX.enabled) m_SUI.TextFX.Finish();
	}
	public void Set_Action_Start() {
		if(m_SUI.TextFX != null && m_SUI.TextFX.enabled) m_SUI.TextFX.Play();
	}

	public void SetData(DialogTalkType _type, string _talk, bool _fold = false, float _foldtime = 1.5f) {
		m_SUI.Type = _type;
		Is_Fold = _fold;
		if (m_SUI.TextFX != null)
		{
			m_SUI.TextFX.enabled = Is_Fold;
			m_SUI.TextFX.DurationInSeconds = _foldtime;
		}
		switch (_type) {
			case DialogTalkType.Normal:
				m_SUI.Ani.SetTrigger("Say_Ani");
				CrntTail = m_SUI.Tails[0];
				break;
			case DialogTalkType.Think:
				m_SUI.Ani.SetTrigger("Think_Ani");
				CrntTail = m_SUI.Tails[1];
				break;
			case DialogTalkType.Shout:
				m_SUI.Ani.SetTrigger("Shout_Ani");
				break;
		}
		m_SUI.Speech.text = _talk;
		if(Is_Fold) Set_Action_Start();
	}
	/// <summary> 다음 대사 나올때 이전 대사 꺼주는데서 호출 </summary>
	public void Stop() {
		if (Is_Fold) Set_Action_Finishied();
		switch (m_SUI.Type) {
			case DialogTalkType.Normal:
				m_SUI.Ani.SetTrigger("Say_Stop");
				break;
			case DialogTalkType.Think:
				m_SUI.Ani.SetTrigger("Think_Stop");
				break;
			case DialogTalkType.Shout:
				m_SUI.Ani.SetTrigger("Shout_Stop");
				break;
		}
	}
}
