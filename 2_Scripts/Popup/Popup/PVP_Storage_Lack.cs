using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class PVP_Storage_Lack : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject[] MatGroup;
		public TextMeshProUGUI[] MatCnt;
		public TextMeshProUGUI Desc;
		public GameObject CancleBtn;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		List<int> posidxs = (List<int>)aobjValue[0];
		int mode = (int)aobjValue[1];
		CampBuildInfo info = USERINFO.m_CampBuild[CampBuildType.Storage];
		TPVP_Camp_Storage tdata = TDATA.GetPVP_Camp_Storage(info.LV);
		for(int i = 0; i < 3; i++) {
			if (posidxs.Contains(i)) {
				m_SUI.MatCnt[i].text = string.Format("{0}/{1}", info.Values[i], tdata.m_SaveMat[i]);
				m_SUI.MatCnt[i].color = BaseValue.GetUpDownStrColor(tdata.m_SaveMat[i], info.Values[i]);
			}
			else m_SUI.MatGroup[i].SetActive(false);
		}

		m_SUI.Desc.text = TDATA.GetString(mode == 0 ? 6235 : 6225);
		m_SUI.CancleBtn.SetActive(mode == 0);

		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
