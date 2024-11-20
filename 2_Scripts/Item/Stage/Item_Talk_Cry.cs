using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Item_Talk_Cry : ObjMng
{
#pragma warning disable 0649
	[SerializeField] TextMeshProUGUI m_Talk;
	CanvasGroup m_Group;
#pragma warning restore 0649

	public void SetData(string Talk, float time)
	{
		m_Talk.text = Talk;
		m_Group = GetComponent<CanvasGroup>();
		SetAlpha(1f);
		StartCoroutine(Timer(time));
	}

	IEnumerator Timer(float time)
	{
		yield return new WaitForSeconds(time);
		float endtime = 0.5f;
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", endtime, "onupdate", "SetAlpha"));
		yield return new WaitForSeconds(endtime);
		Destroy(gameObject);
	}

	void SetAlpha(float _amount)
	{
		m_Group.alpha = _amount;
	}
}
