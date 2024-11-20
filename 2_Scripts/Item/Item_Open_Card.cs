using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Item_Open_Card : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Transform Panel;
		public SpriteRenderer Img;
		public SpriteRenderer GradeFrame;
		public ItemCardRenderer CardRenderer;
		public GameObject CharPieceGroup;
		public TextMeshPro Name;
		public SpriteRenderer BloodImg;
		public Item_PickUpCharCard CharCard;
		public GameObject[] Card;
	}

	[SerializeField] SUI m_UI;
#pragma warning restore 0649
	IEnumerator m_Play;
	Vector3 m_CreateEndAngle;

	private void Awake() {
		m_UI.Card[0].SetActive(false);
		m_UI.Card[1].SetActive(false);
		m_UI.CharPieceGroup.SetActive(false);
		m_UI.Img.gameObject.SetActive(false);
	}
	public void SetStartScale(Vector3 v3SScale) {
		m_UI.Panel.transform.localScale = v3SScale;
	}
	public void SetData(OpenItem info, float CreateTime, int Offset) {
		iTween.Stop(gameObject);
		if (m_Play != null) {
			StopCoroutine(m_Play);
			m_Play = null;
		}
		m_UI.Card[(int)info.m_Type].SetActive(true);
		switch (info.m_Type) {
			case OpenItemType.Character:
				m_UI.CharCard.SetData(info.m_CharInfo);
				break;
			case OpenItemType.Item:
				TItemTable itable = TDATA.GetItemTable(info.m_Idx);
				if (itable.m_Type == ItemType.CharaterPiece) {
					m_UI.CharPieceGroup.SetActive(true);
					m_UI.CardRenderer.SetMainTexture(itable.GetItemImg());
				}
				else { 
					m_UI.Img.gameObject.SetActive(true);
					m_UI.Img.sprite = itable.GetItemImg();
				}
				// TODO FIX GRADE
				m_UI.GradeFrame.sprite = BaseValue.CharFrame(0);
				m_UI.Name.text = info.m_Cnt < 2 ? itable.GetName() : string.Format("{0} X {1}", itable.GetName(), info.m_Cnt);
				break;
		}
	   

		m_Play = Play(CreateTime, Offset);
		StartCoroutine(m_Play);
	}

	IEnumerator Play(float CreateTime, int Offset) {
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

	public void MoveBag(Vector3 bagPos, float MoveTime) {
		iTween.MoveTo(gameObject, iTween.Hash("position", bagPos, "time", MoveTime, "easetype", "easeOutCubic"));
		iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one * 0.3f, "time", MoveTime, "easetype", "easeOutCubic"));
		iTween.ValueTo(gameObject, iTween.Hash("from", 1f, "to", 0f, "time", MoveTime - 0.1f, "easetype", "easeInCubic", "onupdate", "SetAlpha"));

		Destroy(gameObject, MoveTime);
	}

	void SetAlpha(float _amount) {
		GetComponent<RenderAlpha_Controller>().Alpha = _amount;
	}

	void SetDissolve(float _amount) {
		m_UI.BloodImg.material.SetFloat("_DissolveValue", _amount);
	}
}
