using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialInstance : MonoBehaviour
{
	public Renderer m_Renderer;

	private void Awake() {
		Material mat = Instantiate(m_Renderer.material);
		m_Renderer.material = mat;
	}
}
