using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Tower_RandEventCard : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public Image Img;
	}
    [SerializeField] SUI m_SUI;

    public void SetData(Sprite _img) {
        m_SUI.Img.sprite = _img;
	}
}
