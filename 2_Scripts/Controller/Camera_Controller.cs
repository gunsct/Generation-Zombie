using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : ObjMng
{
	public enum MainTainType
	{
		None,
		Width,
		Height
	}
	public Camera myCam = null;
	[SerializeField]
	Camera[] subCams;
	public MainTainType m_Maintaintype = MainTainType.Width;

	[Range(-1, 1)]
	public int adaptPosition;
	float defaultWidth;
	float defaultHeight;

	public float defaultfieldOfView;

	Vector3 CameraPos;
	[SerializeField] float DefaultFOV = 60f;//1440 * 2560 기준
	float[] Resolution = new float[2];

	private void Awake()
	{
		myCam = GetComponent<Camera>();
		Resolution[0] = Screen.height;
		Resolution[1] = Screen.width;
	}



	private void Update() {
		if (Resolution[0] != Screen.height || Resolution[1] != Screen.width) {
			Resolution[0] = Screen.height;
			Resolution[1] = Screen.width;
			POPUP.GetCC.Check();
			Init();
		}
	}

	private void Start()
	{
		Init();
	}
	void Init() {
		float CanvasAspect = Canvas_Controller.ASPECT;
		CameraPos = myCam.transform.position;
		myCam.fieldOfView = DefaultFOV;
		if (m_Maintaintype == MainTainType.Height) {
			defaultWidth = myCam.fieldOfView;

			if (CanvasAspect > myCam.aspect) {
				defaultHeight = myCam.fieldOfView * CanvasAspect;
			}
			else {
				defaultHeight = myCam.fieldOfView * myCam.aspect;
			}
		}
		else {
			defaultHeight = myCam.fieldOfView;

			if (CanvasAspect > myCam.aspect) {
				defaultWidth = myCam.fieldOfView * CanvasAspect;
			}
			else {
				defaultWidth = myCam.fieldOfView * myCam.aspect;
			}
		}
		SetPosition();
	}
	void SetPosition()
	{
		switch (m_Maintaintype) {
			case MainTainType.Width:
				defaultfieldOfView = defaultWidth / myCam.aspect;
				myCam.fieldOfView = defaultfieldOfView;
				for (int i = 0; i < subCams.Length; i++) {
					subCams[i].fieldOfView = defaultfieldOfView;
				}
				myCam.transform.position = new Vector3(CameraPos.x, CameraPos.y + adaptPosition * (defaultHeight - myCam.fieldOfView), CameraPos.z);
				break;
			case MainTainType.Height:
				defaultfieldOfView = defaultHeight / myCam.aspect;
				myCam.fieldOfView = defaultfieldOfView;
				for (int i = 0; i < subCams.Length; i++) {
					subCams[i].fieldOfView = defaultfieldOfView;
				}
				myCam.transform.position = new Vector3(CameraPos.x, CameraPos.y + adaptPosition * (defaultWidth - myCam.fieldOfView), CameraPos.z);
				break;
			default:
				myCam.transform.position = new Vector3(adaptPosition * adaptPosition * (defaultWidth - myCam.fieldOfView * myCam.aspect), CameraPos.y, CameraPos.z);
				break;
		}
	}
}
