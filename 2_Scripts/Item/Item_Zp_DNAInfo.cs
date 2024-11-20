using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Zp_DNAInfo : ObjMng
{
    [Serializable]
    public struct SUI
    {
        public Image Icon;
        public TextMeshProUGUI NameTxt;
    }

    [SerializeField] private SUI m_SUI;
    
    public void SetData(int dnaTableIdx)
    {
        if (dnaTableIdx == -1)
        {
            m_SUI.Icon.gameObject.SetActive(false);
            m_SUI.NameTxt.gameObject.SetActive(false);
            return;
        }
        
        TDnaTable dnaTable = TDATA.GetDnaTable(dnaTableIdx);
        m_SUI.Icon.gameObject.SetActive(true);
        m_SUI.NameTxt.gameObject.SetActive(true);
        m_SUI.Icon.sprite = dnaTable.GetIcon();
        m_SUI.NameTxt.text = dnaTable.GetName();
    }
}