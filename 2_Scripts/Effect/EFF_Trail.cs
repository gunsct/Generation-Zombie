using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EFF_Trail : EFF_MoveCardChange
{
#pragma warning disable 0649
	[SerializeField] GameObject m_TrailPrefab;
	[SerializeField] Transform m_EffPanel;
	float m_Time = 0.5f;
	float m_Delay = 0f;
	float m_CBTime = 0f;
#pragma warning restore 0649

	public void SetTrail(GameObject _prefab, float _time = 0.5f, float _delay = 0f, float _cbtile = 0f) {
		m_TrailPrefab = _prefab;
		m_Time = _time;
		m_Delay = _delay;
		m_CBTime = _cbtile;
	}
	protected override IEnumerator Play(Vector3 SPos, Vector3 EPos, Action<GameObject> EndCB) {
		float time = m_Time + m_Delay;
		// 시작 지점과 끝지점 랜덤 셋팅
		float startX = SPos.x;
		float startY = SPos.y;
		float endX = EPos.x;
		float endY = EPos.y;
		float GapX = endX - startX;
		float GapY = endY - startY;
		// 발모양의 각도
		float angle = Mathf.Atan2(GapY, GapX) * Mathf.Rad2Deg;
		Transform trail = Utile_Class.Instantiate(m_TrailPrefab, m_EffPanel).transform;
		trail.position = SPos;
		trail.eulerAngles = new Vector3(0f, 0f, angle);
		iTween.MoveTo(trail.gameObject, iTween.Hash("position", EPos, "time", m_Time, "easetype", "linear"));

		if(m_CBTime > 0f) {
			yield return new WaitForSeconds(m_CBTime);
			EndCB?.Invoke(gameObject);
		}

		yield return new WaitForSeconds(Mathf.Max(0f,time - m_CBTime));

		if (m_CBTime == 0f) {
			EndCB?.Invoke(gameObject);
		}
		Destroy(gameObject);//4 -> 1.5
	}
}
