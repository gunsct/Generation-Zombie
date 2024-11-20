using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_HPLow : ObjMng
{
	public Animator m_Anim;
	float m_Timer;
	void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RfHPLowUI += SetData;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RfHPLowUI -= SetData;
		}
	}
	private void Update() {
		if (m_Timer < 1.5f)
			m_Timer += Time.deltaTime / Time.timeScale;
		else {
			m_Timer = 0f;
			PlayEffSound(SND_IDX.SFX_0491);
		}
	}
	public void SetData(bool _on) {
		if (_on) {
			gameObject.SetActive(true); 
		}
		else if (gameObject.activeInHierarchy)
			StartCoroutine(Off());
		else
			gameObject.SetActive(false);
	}
	IEnumerator Off() {
		m_Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => !Utile_Class.IsAniPlay(m_Anim));
		gameObject.SetActive(false);
	}
}
