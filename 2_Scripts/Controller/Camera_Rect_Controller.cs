using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Camera_Rect_Controller : ObjMng {
	private void Start()
	{
		GetComponent<Camera>().rect = POPUP.GetCameraRect();
	}
}
