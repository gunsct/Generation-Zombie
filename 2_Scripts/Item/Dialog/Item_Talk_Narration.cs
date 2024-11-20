using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Talk_Narration : MonoBehaviour
{
	[System.Serializable]
	struct SUI
	{
		public TextMeshProUGUI Narr;
		public TextMeshProEffect TextFX;
	}
	[SerializeField]
	SUI m_SUI;

	bool Is_Fold = false;

	private void Awake() {
		m_SUI.TextFX.enabled = false;
	}
	public bool IsAction() {
		return !m_SUI.TextFX.IsFinished;
	}

	public void Set_Action_Finishied() {
		if (m_SUI.TextFX.enabled)
			m_SUI.TextFX.Finish();
	}
	public void Set_Action_Start() {
		if (m_SUI.TextFX.enabled)
			m_SUI.TextFX.Play();
	}
	public void SetData(string _narr, bool _fold, float _foldtime = 2.5f) {
		Is_Fold = _fold;
		m_SUI.TextFX.enabled = Is_Fold;
		m_SUI.TextFX.DurationInSeconds = _foldtime;
		m_SUI.Narr.text = _narr;
		if (Is_Fold) Set_Action_Start();
	}
	public void Stop() {
		if (Is_Fold) Set_Action_Finishied();
	}
}
