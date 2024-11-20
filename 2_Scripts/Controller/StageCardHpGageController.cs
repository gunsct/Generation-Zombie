using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageCardHpGageController : ObjMng
{
	[SerializeField] SpriteRenderer m_Gauge;
	// iTween. onupdate의경우 해당 함수가 있어야되는데 Item_Stage의경우 gameobject의 iTween상태를 체크해서하는 연출들이 많아 클래스를 뺌
	[SerializeField] Sprite[] m_HPBImg;
	[SerializeField] Sprite[] m_HPNImg;
	[SerializeField] Sprite[] m_FrameImg;
	//[SerializeField] Texture[] m_HPBImg;
	//[SerializeField] Texture[] m_HPNImg;
	[SerializeField] RenderAlpha_Controller m_Alpha;
	StageCardInfo m_Info;

	private void Awake()
	{
		m_Gauge.material.SetFloat("_FillBackAmount", 1f);
		m_Gauge.material.SetFloat("_FillAmount", 1f);
	}

	public void SetActive(bool Active)
	{
		if(gameObject.activeSelf != Active) gameObject.SetActive(Active);
	}

	void SetHPAmount(float _amount)
	{
		m_Gauge.material.SetFloat("_FillBackAmount", _amount);
	}

	public void SetAlpha(float alpha)
	{
		m_Alpha.Alpha = alpha;
	}

	public void SetHP(StageCardInfo info, bool Init = false)
	{
		m_Info = info;
		int MaxHP = info.GetMaxStat(EEnemyStat.HP);
		int HP = info.GetStat(EEnemyStat.HP);
		if (!info.IS_EnemyCard || HP == MaxHP)
		{
			if (gameObject.activeSelf) gameObject.SetActive(false);
			return;
		}
		if (!gameObject.activeSelf) gameObject.SetActive(true);
		float amount = (float)HP / (float)MaxHP;

		int pos = info.ISRefugee ? 0 : 1;

		m_Gauge.sprite = m_FrameImg[pos];
		m_Gauge.material.SetTexture("_FillBackTex", m_HPBImg[pos].texture);
		m_Gauge.material.SetTexture("_FillTex", m_HPNImg[pos].texture);

		m_Gauge.material.SetFloat("_FillAmount", amount);
		iTween.Stop(m_Gauge.gameObject);
		float Now = m_Gauge.material.GetFloat("_FillBackAmount");
		if (Init)
		{
			m_Gauge.material.SetFloat("_FillBackAmount", amount);
		}
		else
		{
			if (Now > amount) iTween.ValueTo(gameObject, iTween.Hash("from", Now, "to", amount, "time", 1f, "easetype", "easeInCubic", "onupdate", "SetHPAmount"));
			else m_Gauge.material.SetFloat("_FillBackAmount", amount);
		}
	}
}
