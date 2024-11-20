using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Item_Stage_Char_DNA : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public SpriteRenderer[] Icon;
		public Animator Anim;
	}
    [SerializeField] SUI m_SUI;
	 
	public void SetData(TDnaTable _tdata) {
		m_SUI.Anim.SetTrigger(_tdata.m_BGType.ToString());
		for (int i = 0; i < m_SUI.Icon.Length; i++) m_SUI.Icon[i].sprite = _tdata.GetIcon();
		StartCoroutine(IE_AutoDestory());
	}
	IEnumerator IE_AutoDestory() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		DestroyImmediate(gameObject);
	}
}
