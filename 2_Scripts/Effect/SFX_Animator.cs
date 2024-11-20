using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_Animator : ObjMng
{
	public SND_IDX m_LoopSNDIDX = SND_IDX.NONE;
	AudioSource m_AS;
	bool m_IsCanPlay;
	//애니메이터에 붙여두기만 하고 이벤트 트리거로 사용
	public void PlaySound(SND_IDX _idx) {
		if(m_IsCanPlay) PlayEffSound(_idx, 1f);
	}

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_OBJSndOff += OBJSndOff;
			DLGTINFO.f_OBJSndOn += OBJSndOn;
		}
	}
	private void Start() {
		m_IsCanPlay = true;
		m_AS = PlayEffSound(m_LoopSNDIDX);
		if (m_AS != null) m_AS.loop = true;
	}
	public void OBJSndOff() {
		if (m_AS != null) {
			m_AS.loop = false;
			m_IsCanPlay = false;
		}
	}
	public void OBJSndOn() {
		if (m_AS != null) {
			m_AS.loop = true;
			m_IsCanPlay = true;
		}
	}
	private void OnDisable() {
		OBJSndOff();
	}
	private void OnDestroy() {
		OBJSndOff();
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_OBJSndOff -= OBJSndOff;
			DLGTINFO.f_OBJSndOn -= OBJSndOn;
		}
	}
}
