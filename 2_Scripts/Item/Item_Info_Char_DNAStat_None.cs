using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Info_Char_DNAStat_None : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public Image BG;
        public Color[] BGcolors;
        public TextMeshProUGUI Desc;
	}
    [SerializeField] SUI m_SUI;

    public void SetData(bool _active, string _txt) {
        m_SUI.BG.color = m_SUI.BGcolors[_active ? 0 : 1];
        m_SUI.Desc.text = _txt;
	}
}
