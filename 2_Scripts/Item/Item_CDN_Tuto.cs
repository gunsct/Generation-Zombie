using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_CDN_Tuto : ObjMng
{
	public enum State
	{
		CDN,
		DataLoad
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Desc;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(State _state) {
		StartCoroutine(AutoEnd(_state));
	}

	IEnumerator AutoEnd(State _state) {
		m_SUI.Anim.SetTrigger(_state == State.CDN ? "Start_CDN" : "Start_Loading");
		yield return new WaitForEndOfFrame();
		if (_state == State.DataLoad) yield return new WaitForSeconds(5f);
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		Destroy(gameObject);
	}

}
