using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Item_Drop_Card : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Transform Panel;
		public SpriteRenderer Img;
		public TextMeshPro Name;
		public TextMeshPro Sub;
		public SpriteRenderer BloodImg;
	}

	[SerializeField] SUI m_UI;
#pragma warning restore 0649
	IEnumerator m_Play;
	Vector3 m_CreateEndAngle;

	public void SetStartScale(Vector3 v3SScale)
	{
		m_UI.Panel.transform.localScale = v3SScale;
	}
	public void SetData(DropItem info, float CreateTime, int Offset)
	{
		iTween.Stop(gameObject);
		if (m_Play != null)
		{
			StopCoroutine(m_Play);
			m_Play = null;
		}
		TItemTable tdata = TDATA.GetItemTable(info.m_Idx);
		m_UI.Img.sprite = tdata.GetItemImg();
		m_UI.Name.text = info.m_Cnt < 2 ? tdata.GetName() : string.Format("{0} X {1}", tdata.GetName(), info.m_Cnt);
		m_UI.Sub.text = tdata.GetInvenGroupName();

		m_Play = Play(CreateTime, Offset);
		StartCoroutine(m_Play);
	}

	IEnumerator Play(float CreateTime, int Offset)
	{
		GetComponent<RenderAlpha_Controller>().Alpha = 1f;
		transform.localScale = Vector3.one;
		m_UI.BloodImg.material.SetFloat("_DissolveValue", 0f);

		Vector3 EndAngle = new Vector3(UTILE.Get_Random(-12f, 12f), UTILE.Get_Random(-20f, 20f), UTILE.Get_Random(-5f, 5f));
		Vector3 StartAngle = new Vector3(UTILE.Get_Random(15f, 50f) * (UTILE.Get_Random(0, 1) == 1 ? -1f : 1f)
			, UTILE.Get_Random(35f, 80f) * (UTILE.Get_Random(0, 1) == 1 ? -1f : 1f)
			, UTILE.Get_Random(-5f, 5f) * (UTILE.Get_Random(0, 1) == 1 ? -1f : 1f));
		Vector3 Gap = EndAngle - StartAngle;
		transform.eulerAngles = StartAngle;

		iTween.RotateTo(m_UI.Panel.gameObject, iTween.Hash("rotation", EndAngle, "time", CreateTime, "easetype", "easeOutQuad"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 0f, "to", 1f, "delay", 0.3f + (Offset + 1) * 0.1f, "time", 2f, "easetype", "easeOutCubic", "onupdate", "SetDissolve"));
		yield return new WaitForSeconds(CreateTime);
		// 천천히 더 이동해준다.
		iTween.RotateTo(m_UI.Panel.gameObject, iTween.Hash("rotation", EndAngle + Gap, "time", 15f, "easetype", "linear"));

		yield return new WaitForSeconds(3f);
		m_Play = null;
	}

	public void MoveBag(Vector3 bagPos, float MoveTime)
	{
		iTween.MoveTo(gameObject, iTween.Hash("position", bagPos, "time", MoveTime, "easetype", "easeOutCubic"));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.3f, "time", MoveTime, "easetype", "easeOutCubic"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", MoveTime - 0.1f, "easetype", "easeInCubic", "onupdate", "SetAlpha"));

		Destroy(gameObject, MoveTime);
	}

	void SetAlpha(float _amount)
	{
		GetComponent<RenderAlpha_Controller>().Alpha = _amount;
	}

	void SetDissolve(float _amount)
	{
		m_UI.BloodImg.material.SetFloat("_DissolveValue", _amount);
	}
}
