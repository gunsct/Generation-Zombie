using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_PDA_Making_Ready : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
	}
	[SerializeField]
	SUI m_SUI;

	private void OnEnable() {
		StartCoroutine(AnimEnd());
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

	}

	IEnumerator AnimEnd() {
		m_SUI.Anim.SetTrigger("Start");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		PlayEffSound(SND_IDX.SFX_0123);
		m_CloaseCB?.Invoke(Item_PDA_Making.State.Main, null);
	}
}
