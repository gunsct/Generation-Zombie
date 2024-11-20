using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutoStart : PopupBase
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Image Icon;
		public TextMeshProUGUI Talk;
	}
	[SerializeField] SUI m_SUI;
#pragma warning restore 0649

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue)
	{
		base.SetData(pos, popup, cb, aobjValue);
		if (aobjValue != null) {
			PlayEffSound(SND_IDX.SFX_9500);
			int talk = (int)aobjValue[0];

			if (talk > 0)
			{
				TDialogTable dlg = TDATA.GetDialogTable(talk);
				if (dlg != null)
				{
					TTalkerTable talker = dlg.GetTalker();
					m_SUI.Icon.sprite = talker.GetSprPortrait();
					m_SUI.Talk.text = dlg.GetDesc();
					return;
				}
			}
		}
		m_SUI.Talk.text = TDATA.GetString(453);
	}
}
