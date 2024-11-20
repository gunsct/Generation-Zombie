using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchFX : ObjMng
{
	public static int Mode = 0;
	public Transform[] m_FXParent;
	public List<GameObject>[] m_FXPool = new List<GameObject>[2];
	float m_Timer = 0f;

	private void Start() {
		m_FXPool[0] = new List<GameObject>();
		m_FXPool[1] = new List<GameObject>();
		for (int i = 0; i < 10; i++) {
			GameObject fx1 = UTILE.LoadPrefab("Effect/EF_Touch_Default", true, m_FXParent[0]);
			if (fx1 == null) continue;
			fx1.SetActive(false);
			m_FXPool[0].Add(fx1);
		}
		for (int i = 0; i < 10; i++) {
			GameObject fx2 = UTILE.LoadPrefab("Effect/EF_Touch_HPLow", true, m_FXParent[1]);
			if (fx2 == null) continue;
			fx2.SetActive(false);
			m_FXPool[1].Add(fx2);
		}
	}
	
	void LateUpdate() {
		if(m_Timer > 0f) {
			m_Timer -= Time.deltaTime;
		}
		//if (Input.GetMouseButtonDown(0))
		//{
		//}
		if (Input.GetMouseButtonUp(0))
		{
			if (IS_CanTouch())
			{
				OnTouch(Input.mousePosition);
			}
		}
		Mode = 0;
	}
	bool IS_CanTouch() {
		return m_Timer <= 0f;//<2.8f
	}
	void OnTouch(Vector3 _pos) {
		if (POPUP.GetMainUI() == null) return;

		PlayEffSound(SND_IDX.SFX_0001);
		m_Timer = 0.2f;//3f
		if (!Convert.ToBoolean(PlayerPrefs.GetInt("FINGER_PRINT_ONOFF", 1)))return;

		switch (MAIN.m_State) {
			case MainState.BATTLE:
			case MainState.STAGE:
			case MainState.TOWER:
				float ratio = (float)STAGE_USERINFO.GetStat(StatType.HP) / (float)STAGE_USERINFO.GetMaxStat(StatType.HP);
				GameObject fx = null;
				if (ratio > 0.4f){
					fx = m_FXPool[0].Find(t => t.activeSelf == false);
				}
				else {
					fx = m_FXPool[1].Find(t => t.activeSelf == false);
				}
				if (fx == null) return;
				fx.transform.position = _pos;
				fx.SetActive(true);
				break;
			default:
				GameObject fx2 = m_FXPool[0].Find(t => t.activeSelf == false);
				if (fx2 == null) return;
				fx2.transform.position = _pos;
				fx2.SetActive(true);
				break;
		}
	}
}