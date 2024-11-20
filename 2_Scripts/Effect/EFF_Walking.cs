using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EFF_Walking : EFF_MoveCardChange
{
#pragma warning disable 0649
	[SerializeField] GameObject m_FootPrefab;
	[SerializeField] Transform m_EffPanel;
#pragma warning restore 0649

	protected override IEnumerator Play(Vector3 SPos, Vector3 EPos, Action<GameObject> EndCB)
	{
		// 시작 지점과 끝지점 랜덤 셋팅
		float startX = SPos.x;
		float startY = SPos.y;
		float endX = EPos.x;
		float endY = EPos.y;
		float GapX = endX - startX;
		float GapY = endY - startY;
		// 발모양의 각도
		float angle = Mathf.Atan2(GapY, GapX) * Mathf.Rad2Deg;
		int FootPos = UTILE.Get_Random(0, 1);
		int count = Mathf.RoundToInt(Vector3.Distance(SPos, EPos) / 0.4f);
		float[] footPosgap = { 0.2f, -0.2f };

		Vector3 pos = new Vector3(startX, startY, 0f);
		Vector3 add = new Vector3(GapX / count, GapY / count, 0f);

		// 발자국 생성해주기
		for (int i = 0; i < count; i++, FootPos = 1 - FootPos, pos += add)
		{
			Transform foot = Utile_Class.Instantiate(m_FootPrefab, m_EffPanel).transform;
			foot.GetComponent<Item_Foot>().SetFootPos(FootPos, angle);
			foot.localScale = Vector3.one;
			Vector3 v3Pos = pos + foot.up * footPosgap[FootPos];
			foot.position = v3Pos;
			if (i < count - 1) yield return new WaitForSeconds(0.08f);//1.5 -> 0.5
		}
		EndCB?.Invoke(gameObject);
		Destroy(gameObject, 0.5f);//4 -> 1.5
	}
}
