using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using System.Linq;

public class BuyGold : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Transform m_Panel;
		public TextMeshProUGUI[] Cnts;  //0:무료, 1:유료
		public GameObject Btn;
	}
	[SerializeField]
	SUI m_SUI;
	RectTransform m_Target;
	bool IS_InStore;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Target = (RectTransform)aobjValue[0];
		IS_InStore = (bool)aobjValue[1];
		base.SetData(pos, popup, cb, aobjValue);
	}

	void SetPosition()
	{
		if (m_Target == null) return;
		Canvas.ForceUpdateCanvases();
		RectTransform rtfMask = (RectTransform)transform;
		RectTransform rtfPanel = (RectTransform)m_SUI.m_Panel.transform;
		m_SUI.m_Panel.transform.position = m_Target.position;
		Vector2 v2Pos = rtfPanel.anchoredPosition;
		// 아이콘의 높이만큼 이동 고정 300 + 0.5 으로 셋팅 (UI 셋팅중 사이즈가 사라질수도있음)
		float h = rtfPanel.sizeDelta.y * 0.5f;
		float w = rtfPanel.sizeDelta.x * 0.5f;
		v2Pos.y += 150f;
		v2Pos.y += h;

		float t = rtfMask.rect.height * 0.5f;
		float r = rtfMask.rect.width * 0.5f;

		// 위치 조정
		if (v2Pos.y + h > t)
		{
			v2Pos = rtfPanel.anchoredPosition;
			v2Pos.y -= 150f;
			v2Pos.y -= h;
		}

		if (v2Pos.x + w > r) v2Pos.x = r - w;
		if (v2Pos.x - w < -r) v2Pos.x = -r + w;


		rtfPanel.anchoredPosition = v2Pos;
	}

	private void Update()
	{
		// 스크롤등 움직임 때문에 타겟 위치로 재셋팅
		SetPosition();
	}

	public override void SetUI() {
		m_SUI.Btn.SetActive(!IS_InStore);
		for(int i = 0;i < 2;i++) m_SUI.Cnts[i].text = Utile_Class.CommaValue(USERINFO._Cash[i]);
		SetPosition();
	}
}
