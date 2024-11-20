using System;
using UnityEngine;

public class Item_Zp_Intro_Slot : ObjMng
{
    [Serializable]
    public struct SUI
    {
        public GameObject Off;
        public GameObject On;
        public GameObject Lock;
    }

    [SerializeField] private SUI m_SUI;

    public enum CageState
    {
        Lock,
        Empty,
        Fill
    }
    
    public void Refresh(CageState cageState)
    {
        switch (cageState)
        {
            case CageState.Lock:
                m_SUI.Off.SetActive(true);
                m_SUI.On.SetActive(false);
                m_SUI.Lock.SetActive(true);
                break;
            case CageState.Empty:
                m_SUI.Off.SetActive(true);
                m_SUI.On.SetActive(false);
                m_SUI.Lock.SetActive(false);
                break;
            case CageState.Fill:
                m_SUI.Off.SetActive(false);
                m_SUI.On.SetActive(true);
                m_SUI.Lock.SetActive(false);
                break;
        }
    }
}
