using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class Item_Info_Char_DNAStat_Sub : ObjMng
{
    [Serializable]
    public struct SUI
	{
        public Image Icon;
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Desc;
	}
    [SerializeField] SUI m_SUI;

    public void SetData(StatType _stat, float _val, bool _ratio = true) {
        m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)_stat), "png");
        m_SUI.Name.text = TDATA.GetStatString(_stat);
        m_SUI.Desc.text = _ratio ? string.Format("+{0:0.####}%", _val * 100f) : string.Format("+{0}", Mathf.RoundToInt(_val));
    }
    public void SetData(StatType _stat, List<float> _val) {
        m_SUI.Icon.sprite = UTILE.LoadImg(string.Format("UI/Icon/Icon_Char_Stat_{0}", (int)_stat), "png");
        m_SUI.Name.text = TDATA.GetStatString(_stat);
        if(_val.Count < 2)
            m_SUI.Desc.text = string.Format("+{0:0.##}%", _val[0] * 100f);
        else
            m_SUI.Desc.text = string.Format("+{0:0.##}% <color=#665F56AA>/</color> +{1:0.##}%", _val[0] * 100f, _val[1] * 100f);
    }
}
