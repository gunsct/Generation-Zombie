using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_Damage : ObjMng
{
#pragma warning disable 0649
	[SerializeField] Transform m_Blood;
	[SerializeField] Transform m_SizeScale;
	[SerializeField] TextMesh[] m_Values;
	[SerializeField] Animator m_Ani;
	[SerializeField] Item_SeviverDmg[] m_SrvDmg;
#pragma warning restore 0649
	public void SetSize(ENoteSize size)
	{
		float scale = BaseValue.NOTE_SCALE[(int)size];
		m_SizeScale.localScale = new Vector3(scale, scale, 1f);
	}
	public void SetData(ENoteHitState state, int Value, float rate)
	{
		transform.localScale = Vector3.one;
		m_Blood.eulerAngles = new Vector3(0f, 0f, UTILE.Get_Random(0f, 360f));
		// rate = 0.1 -> 0.2 * x = 0, rate = 1 -> 0.2 * x = 0.2
		// rate 범위 0.1~1
		// 추가 0.2만큼만 스케일 조정해야하므로
		// (rate - 0.1) / 0.9를 해줌
		if (state != ENoteHitState.PERPECT)
		{
			float mul = 0.8f + 0.2f * ((rate - 0.1f) / 0.9f);
			if (float.IsNaN(mul) || float.IsPositiveInfinity(mul) || float.IsNegativeInfinity(mul)) mul = 0.8f;
			transform.localScale *= mul;
		}
		for (int i = m_Values.Length - 1; i > -1; i--) m_Values[i].text = string.Format("{0:#;-#;0}", Value);
		StartCoroutine(Ani(state == ENoteHitState.PERPECT ? "Cri" : "Normal"));
		for (int i = m_SrvDmg.Length - 1; i > -1; i--) m_SrvDmg[i].gameObject.SetActive(false);
	}

	public void SetSrvDmg(StatType type, int Dmg)
	{
		for (int i = 0; i < m_SrvDmg.Length; i++)
		{
			Item_SeviverDmg dmg = m_SrvDmg[i];
			if (dmg.gameObject.activeInHierarchy) continue;
			dmg.SetData(type, Dmg);
			break;
		}
	}

	public void SetData(int Value, bool isCri = false, float size = 1f)
	{
		m_SizeScale.localScale = new Vector3(size, size, 1f);
		for (int i = m_Values.Length - 1; i > -1; i--) m_Values[i].text = string.Format("{0:#;-#;0}", Value);
		StartCoroutine(Ani(isCri ? "Cri" : "Normal"));
		for (int i = m_SrvDmg.Length - 1; i > -1; i--) m_SrvDmg[i].gameObject.SetActive(false);
	}

	IEnumerator Ani(string trigger)
	{
		m_Ani.SetTrigger(trigger);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_Ani));
		GameObject.Destroy(gameObject);
	}
}
