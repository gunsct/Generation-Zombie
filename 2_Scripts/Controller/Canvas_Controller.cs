using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Canvas_Controller : MonoBehaviour {
	
#pragma warning disable 0649
	[SerializeField] RectTransform[] rtfCheckList;
	[SerializeField] GameObject sideFrame;
#pragma warning restore 0649
	public static float BASE_SCREEN_WIDTH = 0, BASE_SCREEN_HEIGHT = 0;
	public static float SCALE_SCREEN_WIDTH = 0, SCALE_SCREEN_HEIGHT = 0;
	public static float SCALE = 0;
	public static float ASPECT = 0f;

	Rect CameraRect;

	public float GetScaleW { get { return SCALE_SCREEN_WIDTH; } }
	public float GetScaleH { get { return SCALE_SCREEN_HEIGHT; } }
	private void Awake()
	{
		Check();
	}
	//private void Update()
	//{
	//    Check();
	//}

	public void Check()
	{
		// 0. Get Value
		Rect safeArea = Screen.safeArea;
		CanvasScaler scaler = GetComponent<CanvasScaler>();
		//scaler.referenceResolution = new Vector2(Screen.width, Screen.height);
		ASPECT = scaler.referenceResolution.x / scaler.referenceResolution.y;
#if IPHONE_X_TEST
		if(Screen.width <= Screen.height)
			safeArea = new Rect(0, 114, 1125, 2436 - 114);
		else
			safeArea = new Rect(114, 0, 2436 - 114, 1125);
#endif
		//Vector2 screen = new Vector2(Screen.width, Screen.height);

		Vector2 _saAnchorMin;
		Vector2 _saAnchorMax;
		_saAnchorMin.x = safeArea.x / Screen.width;
		_saAnchorMin.y = safeArea.y / Screen.height;
		_saAnchorMax.x = (safeArea.x + safeArea.width) / Screen.width;
		_saAnchorMax.y = (safeArea.y + safeArea.height) / Screen.height;

		BASE_SCREEN_WIDTH = scaler.referenceResolution.x;
		BASE_SCREEN_HEIGHT = scaler.referenceResolution.y;

		SCALE_SCREEN_WIDTH = Screen.width / BASE_SCREEN_WIDTH;
		SCALE_SCREEN_HEIGHT = Screen.height / BASE_SCREEN_HEIGHT;
		SCALE = Mathf.Min(SCALE_SCREEN_WIDTH, SCALE_SCREEN_HEIGHT);

		for (int i = rtfCheckList.Length - 1; i > -1; i--)
		{
			RectTransform rectTransform = rtfCheckList[i];
			rectTransform.anchorMin = _saAnchorMin;
			rectTransform.anchorMax = _saAnchorMax;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
		}

		float safeX = safeArea.x / Screen.width;
		float safeY = safeArea.y / Screen.height;
		float safeW = safeArea.width / Screen.width;
		float safeH = safeArea.height / Screen.height;

		CameraRect = new Rect(safeX, safeY, safeW, safeH);
		if (sideFrame != null) sideFrame.SetActive(safeArea.width != Screen.width || safeArea.height != Screen.height);
	}

	public void SetMainCameraRect()
	{
		Camera.main.rect = CameraRect;
	}
	public Rect GetCameraRect()
	{
		return CameraRect;
	}

	public void Set()
	{
		Check();
	}
}
