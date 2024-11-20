using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class CageInfoa : ClassMng
{
    public int m_Pos;
    public long m_Uid;

    public long m_ZombieUid;
    
    public bool m_IsLock;
    public bool m_IsExist;
    
    /// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
    /*
    public CageInfo() { }
    
    public CageInfo(int pos, long zombieUid, long uid = 0)
    {
        if (uid == 0) uid = Utile_Class.GetUniqeID();

        m_Uid = uid;
        m_Pos = pos;
        m_ZombieUid = zombieUid;
        
        m_IsLock = true;
        m_IsExist = false;
    }
    */

    public void LockCage()
    {
        m_IsLock = true;
        m_IsExist = false;
    }
    
    public void UnlockCage()
    {
        m_IsLock = false;
    }

    // 주입
    public void Upgrade(ItemInfo dnaItem)
    {
        if (m_IsLock)
        {
            return;
        }

        if (!m_IsExist)
        {
            return;
        }

        //if (m_Grade != dnaItem.m_Grade)
        {
            return;
        }
        
        //m_Stability
        
    }

    public void Kill()
    {
        if (m_IsLock)
        {
            return;
        }

        if (!m_IsExist)
        {
            return;
        }

        m_IsExist = false;
    }
}

//좀비 획득
//아이템 -> 뜯으면 좀비

//좀비 우리



//보상타입 추가 좀비

//확율적으로 더쌔지거나 죽어서 거름(DNA)을 남기거나


