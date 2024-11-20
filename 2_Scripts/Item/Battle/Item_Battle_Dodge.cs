using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_Battle_Dodge : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
	}
	[SerializeField] SUI m_SUI;

	public void SetData() {
		StartCoroutine(Ani());
	}
	IEnumerator Ani() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		GameObject.Destroy(gameObject);
	}
}
