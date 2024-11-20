using SoftMasking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_MainMenu_Stg_Bg : MonoBehaviour
{
	public enum Mode
	{
		AllMenuBG = 0,
		StageMenuBG
	}
	[System.Serializable]
	public struct SUI
	{
		public GameObject[] AllModeOffOBJ;
		public GameObject Mask;
		public GameObject SketchFX;
	}
	[SerializeField]
	SUI m_SUI;
	public void SetData(Mode mode) {
		for (int i = 0; i < m_SUI.AllModeOffOBJ.Length; i++) {
			m_SUI.AllModeOffOBJ[i].SetActive(mode == Mode.StageMenuBG);
		}
	}
	public void MaskOff() {
		m_SUI.Mask.SetActive(false);
	}
	public void SetMask(RectTransform _rt) {
		m_SUI.Mask.GetComponent<SoftMask>().separateMask = _rt;
	}
	public void SketchFXOnOff(bool _on) {
		m_SUI.SketchFX.SetActive(_on);
	}
}
