using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AlphaBtn : ObjMng
{
	public float AlphaThreshold = 0.1f;

	void Start() {
		this.GetComponent<Image>().alphaHitTestMinimumThreshold = AlphaThreshold;
	}
}
