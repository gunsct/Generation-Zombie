using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class Item_Foot : ObjMng
{
	[SerializeField] Transform m_SizeScale;


	void Start()
	{
		SetAlpha(1f);
		StartCoroutine(RemoveCheck());
	}

	public void SetFootPos(int nPos, float Angle)
	{
		// 기본 으로 돌려준후 다시 셋팅해준다.
		transform.eulerAngles = Vector3.zero;

		m_SizeScale.eulerAngles = new Vector3(nPos == 1 ? 0f : 180f, 0f, 0f);

		transform.eulerAngles = new Vector3(0f, 0f, Angle);
	}

	void SetAlpha(float alpha)
	{
		GetComponent<RenderAlpha_Controller>().Alpha = alpha;
	}

	IEnumerator RemoveCheck()
	{
		yield return new WaitForSeconds(3f);
		// 체인 제거 및 시계 알파
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", 1f, "easetype", "easeOutQuad", "onupdate", "SetAlpha"));
		yield return new WaitWhile(() => Utile_Class.IsPlayiTween(gameObject));

		Destroy(gameObject);
	}
}
