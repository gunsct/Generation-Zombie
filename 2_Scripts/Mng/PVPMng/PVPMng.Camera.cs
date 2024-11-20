using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class PVPMng : ObjMng
{
	public enum CamActionType
	{
		Pos_Card_1,
		Pos_Card_2,
		Pos_Card_3,
		Pos_Player,
		Zoom_Out,       //공습 시 연출 시작 때
		Zoom_OutToIdle, //공습 연출 종료 때
		Battle_Hit,     //적이나 플레이어나 데미지 발생할때
		Battle_Hit_2,     //적이나 플레이어나 데미지 발생할때
		Battle_Hit_Cri, //위랑 같은데 크리티컬일때
		Shake_0,            //코드로 커스텀한 쉐이크
		Shake_1,        //크고 짧게 흔들리는 쉐이크
		Shake_2         //잘고 길게 흔들리는 쉐이크
	}
	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Camera Scroll
	public Camera m_MyCam;
	public Animator m_CamAnim;

	void InitCamAnim() {
		m_CamAnim.SetTrigger("Pos_Idle");
		m_CamAnim.SetTrigger("Zoom_Idle");
		m_CamAnim.SetTrigger("AniPanel_Idle");
		m_MyCam.transform.localPosition = Vector3.zero;
		m_MyCam.transform.localEulerAngles = Vector3.zero;
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Camera shake
	IEnumerator m_CamActionCor;
	IEnumerator CamShake(float delay = 0.05f, float duration = 0.6f, Vector3? strength = null, AnimationCurve curve = null) {
		if (strength == null) strength = new Vector3(0.1f, 0.1f, 0.5f);
		if (curve == null) curve = AnimationCurve.Linear(0, 1, 1, 0);
		yield return new WaitForSeconds(delay);

		float time = 0f;
		float CAMERA_SHAKE_MULTIPLIER = 10.0f;
		Camera cam = m_MyCam;
		Vector3 initpos = cam.transform.localPosition;
		while (time < duration) {
			cam.transform.localPosition = initpos;
			float delta = Mathf.Clamp01(time / duration);
			var randomVec = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value);
			var shakeVec = Vector3.Scale(randomVec, (Vector3)strength) * (UnityEngine.Random.value > 0.5f ? -1 : 1);
			var shakeVector = shakeVec * curve.Evaluate(delta) * CAMERA_SHAKE_MULTIPLIER;
			cam.transform.localPosition += cam.transform.rotation * shakeVector;
			yield return null;
			time += Time.deltaTime;
		}
		yield return null;
	}
	public void CamAction(CamActionType _type, float _delay = 0f, float _duration = 0f, Vector3? _strength = null) {
		m_CamActionCor = IE_CamAction(_type, _delay, _duration, _strength);
		StartCoroutine(m_CamActionCor);
	}
	IEnumerator IE_CamAction(CamActionType _type, float _delay = 0f, float _duration = 0f, Vector3? _strength = null) {
		if (_type == CamActionType.Shake_0) {
			m_CamActionCor = CamShake(_delay, _duration, _strength);
			yield return m_CamActionCor;
		}
		else {
			if (_delay > 0f) yield return new WaitForSeconds(_delay);

			m_CamAnim.SetTrigger(_type.ToString());
			if (_type == CamActionType.Battle_Hit_Cri && POPUP.GetMainUI().m_Popup == PopupName.Stage)
				((Main_Stage)POPUP.GetMainUI()).SetCriScreenFX();

			if (_duration > 0f) yield return new WaitForSeconds(_duration);
			else yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_CamAnim));
		}
		m_CamActionCor = null;
	}
	public bool IS_EndCamAction { get { return m_CamActionCor == null; } }
}
