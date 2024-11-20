using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	bool m_AutoPlay = false;
	public void StartAutoPlay(float timescale = 1f)
	{
		m_StageAction = AutoPlay(timescale);
		StartCoroutine(m_StageAction);
	}

	public IEnumerator AutoPlay(float timescale = 1f)
	{
		float ts = Time.timeScale;
		Time.timeScale = timescale;
		m_AutoPlay = true;
		while (m_CardLastLine != 0)
		{
			yield return SetSelectCard(m_ViewCard[0][UTILE.Get_Random(0, 3)]);
		}

		m_AutoPlay = false;
		Time.timeScale = ts;
		m_StageAction = null;
	}
}
