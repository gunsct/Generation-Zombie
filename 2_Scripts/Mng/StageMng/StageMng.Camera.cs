using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class StageMng : ObjMng
{
	public enum CamActionType
	{
		Pos_Card_1,
		Pos_Card_2,
		Pos_Card_3,
		Pos_Player,
		Zoom_Out,       //공습 시 연출 시작 때
		Zoom_OutToIdle, //공습 연출 종료 때
		Zoom_Out_Up,	//당기면서 위 보여줌
		Zoom_In_Down,	//내려가며 다시 원위치
		Battle_Hit,     //적이나 플레이어나 데미지 발생할때
		Battle_Hit_2,     //적이나 플레이어나 데미지 발생할때
		Battle_Hit_Cri, //위랑 같은데 크리티컬일때
		Shake_0,            //코드로 커스텀한 쉐이크
		Shake_1,        //크고 짧게 흔들리는 쉐이크
		Shake_2         //잘고 길게 흔들리는 쉐이크
	}
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
	IEnumerator CamShake(float delay = 0.05f, float duration = 0.6f, Vector3? strength = null, AnimationCurve curve = null)
	{
		if (strength == null) strength = new Vector3(0.1f, 0.1f, 0.5f);
		if (curve == null) curve = AnimationCurve.Linear(0, 1, 1, 0);
		yield return new WaitForSeconds(delay);

		float time = 0f;
		float CAMERA_SHAKE_MULTIPLIER = 10.0f;
		Camera cam = m_MyCam;
		Vector3 initpos = cam.transform.localPosition;
		while (time < duration)
		{
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
	/// <summary>
	/// AutoCamPosInit는 사용 전후로 직접 컨트롤
	/// 크리티컬, 헤드샷, 즉사면 섬광효과 및 줌 오버
	/// </summary>
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

	////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// Camera Scroll
	bool UseCamScroll = true;
	bool AutoCamPosInit = true;
	bool LockCamScroll = false;
	readonly float CAMAREASIZE = 12f;
	float CamMoveGapX = 0f;
	Vector3 m_TouchPoint;
	ETouchState m_TouchState;
	CameraArea2D m_CamArea;
	public Camera m_MyCam;
	public Animator m_CamAnim;
	long m_MainUIScrollInitDelay;
	float m_MoveTime = 0f;
	void CamDataInit()
	{
		m_MyCam.transform.position = new Vector3(0, 0, -10);
		m_CamArea = new CameraArea2D();
		m_CamArea.Init(m_MyCam, CAMAREASIZE, 0f);

		UseCamScroll = m_CamArea.IS_UseArea();
		LockCamScroll = false;
		InitCamAnim();
	}

	void InitCamMove()
	{
		CamMoveGapX = 0;
	}

	void SetCamMove(float AddX)
	{
		CamMoveGapX += AddX;
	}

	void CameraMoveUpdate()
	{
		if (m_CamArea == null) return;
		if (!UseCamScroll) return;
		if (LockCamScroll) return;
		if (!MAIN.IS_State(MainState.STAGE)) return;
		if (POPUP.IS_PopupUI() && (POPUP.GetPopup().m_Popup == PopupName.Stage_Reward || POPUP.GetPopup().m_Popup == PopupName.Stage_StartReward)) return;
		//if (TUTO.IsTuto(TutoKind.Stage_203, (int)TutoType_Stage_203.Focus_First_Line)) return;
		if (AutoCamPosInit && m_TouchState == ETouchState.NONE)
		{
			// 카메라 위치 자동 초기화
			CamMoveGapX = m_MyCam.transform.position.x * -1f;
		}
		else if(!TUTO.IsCameraMove())
		{
			CamMoveGapX = 0; 
			m_MoveTime += Time.deltaTime;
			return;
		}

		// 카메라 이동
		float movex = 0f;
		float fTime = Time.deltaTime;
		if (Mathf.Abs(CamMoveGapX) >= 0.1f)
		{
			float fG = CamMoveGapX * 0.3f;
			movex += fG;
			CamMoveGapX -= fG;
			if (Mathf.Abs(CamMoveGapX) < 0.001f)
			{
				movex += CamMoveGapX;
				CamMoveGapX = 0;
			}
			if (AutoCamPosInit && m_TouchState != ETouchState.NONE) m_MainUI.StartPlayAni(Main_Stage.AniName.ScrollOut);
			m_MainUIScrollInitDelay = (long)UTILE.Get_Time_Milli();
			m_MoveTime = 0f;
		}
		else if (AutoCamPosInit && m_TouchState == ETouchState.NONE) {
			m_MyCam.transform.position = new Vector3(Mathf.Lerp(m_MyCam.transform.position.x, 0f, 0.5f), m_MyCam.transform.position.y, m_MyCam.transform.position.z);
			if (m_MainUIScrollInitDelay > 0)
			{
				m_MainUI.StartPlayAni(Main_Stage.AniName.ScrollIn);
				m_MainUIScrollInitDelay = 0;
			}
		}
		else m_MoveTime += Time.deltaTime;

		Vector3 move = new Vector3(movex, 0f, 0f);
		m_MyCam.transform.position += move;

		Vector3 OverGap = m_CamArea.GetOverGap();
		m_MyCam.transform.position += OverGap;
		if (OverGap.x > 0.01f) CamMoveGapX = 0;
	}
}
