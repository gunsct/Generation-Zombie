using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Zp_MateGroup : ObjMng
{
    public enum AnimType
    {
        Empty,
        Set,
        NowSelect,
        Ready
    }

    [Serializable]
    public struct SUI
    {
        public Animator Anim;
        public Item_RewardDNA_Card DnaCard;
        public TextMeshProUGUI NameTxt;
    }

    [SerializeField] private SUI m_SUI;

    private int m_Pos;
    private Action<object, int> m_DnaCallback;
    private Action<int> m_SelectCallback;
    private DNAInfo dnaInfo;
    public DNAInfo DnaInfo => dnaInfo;

    public void SetData(DNAInfo dnaInfo, int pos, Action<object, int> dnaCB = null, Action<int> selectCB = null)
    {
        int idx = -1;
        int grade = -1;
        int lv = 1;
        this.dnaInfo = dnaInfo;
        if (dnaInfo == null) {
            m_SUI.Anim.SetTrigger("Empty");
            m_SUI.DnaCard.gameObject.SetActive(false);
            m_SUI.NameTxt.gameObject.SetActive(false);
        }
        else {
            m_SUI.Anim.SetTrigger("Set");
            m_SUI.DnaCard.gameObject.SetActive(true);
            m_SUI.NameTxt.gameObject.SetActive(true);

            idx = dnaInfo.m_Idx;
            grade = dnaInfo.m_Grade;
            m_SUI.NameTxt.text = dnaInfo.m_Name;
        }

        m_SUI.DnaCard.SetData(idx, pos, lv, -1, dnaCB);

        m_Pos = pos;
        m_DnaCallback = dnaCB;
        m_SelectCallback = selectCB;
    }

    public void PlayAnim(AnimType animType)
    {
        m_SUI.Anim.SetTrigger(animType.ToString());
    }

    public void OnBtnSelectMatClicked()
    {
        if (dnaInfo == null) return;
        m_SelectCallback?.Invoke(m_Pos);
    }
}