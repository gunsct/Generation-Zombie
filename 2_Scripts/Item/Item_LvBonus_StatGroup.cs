using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_LvBonus_StatGroup : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public Animator Anim;
        public Image Icon;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Val;
    }
    [SerializeField] SUI m_SUI;

    public void SetData(StatType _type, float _val) {
        m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)_type), "png");
        m_SUI.Name.text = TDATA.GetStatString(_type);
        m_SUI.Val.text = _type == StatType.Critical ? string.Format("+{0:0.####}%", _val * 100f) : string.Format("+{0}", Mathf.RoundToInt(_val));
    }
    public void SetStartAnim() {
        m_SUI.Anim.SetTrigger("Start");
    }
}
