using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using static LS_Web;
using static PVPInfo;

public partial class PVPMng : ObjMng
{
	IEnumerator IE_StartAction() {

		StateChange(State.Loading);
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_MainUI.GetAnim, 100f / 190f));

		StateChange(State.SelectReward);
		yield return new WaitWhile(() => m_SelectPanel != null);
		yield return new WaitForSeconds(2f);

		if (!TUTO.IsTutoPlay()) {
			TUTO.Start(TutoStartPos.PVP);
			yield return new WaitWhile(() => !TUTO.IsTuto());
		}

		StateChange(State.Play);
	}
}
