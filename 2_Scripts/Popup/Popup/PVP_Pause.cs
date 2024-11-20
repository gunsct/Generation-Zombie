using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PVP_Pause : PopupBase
{
    [Serializable]
    public struct SUI
	{

	}
	[SerializeField] SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
		Time.timeScale = 0f;
	}

	public void ClickResume() {
		Close(0);
	}
	public void ClickSurrender() {
		Close(1);
	}
}
