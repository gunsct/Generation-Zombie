using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EF_EqExp : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI m_Value;
	Action m_Endcb;
	public void SetData(int value, Action endcb = null) {
		m_Value.text = string.Format("Exp +{0}", value);
		m_Endcb = endcb;
		StartCoroutine(CheckEnd());
	}

	IEnumerator CheckEnd()
	{
		Animator ani = GetComponent<Animator>();
		yield return Utile_Class.CheckAniPlay(ani);
		if (m_Endcb == null) GameObject.Destroy(gameObject);
		m_Endcb?.Invoke();
	}
}
