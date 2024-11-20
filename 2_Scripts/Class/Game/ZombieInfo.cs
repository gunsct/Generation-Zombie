using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using static LS_Web;

public class ZombieInfo : ClassMng
{
    public int m_Idx;
    public long m_UID;
    public bool m_Use;

    [JsonIgnore] public TZombieTable m_TData => TDATA.GetZombieTable(m_Idx);
    [JsonIgnore] public string m_Name => m_TData.GetName();

    public int m_Grade;
    public ZombieType m_Type;
    /// <summary> 좀비 상태 </summary>
    public ZombieState m_State;

    public List<int> m_DnaList = new List<int>();

    /// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
    public ZombieInfo() { }
    
    /// <summary>
    /// 우리 & 좀비 데이터 함께 포함
    /// </summary>
    /// <param name="idx"> 0 일 때 비고 잠긴 우리 </param>
    public ZombieInfo(int idx, long uid = 0)
    {
        if (uid == 0) uid = Utile_Class.GetUniqeID();

        m_UID = uid;
        m_Idx = idx;

        if (idx != 0)
        {
            m_Grade = m_TData.m_Grade;
            m_Type = UTILE.Get_Random<ZombieType>(ZombieType.Normal, ZombieType.Max);
        }
    }
    public void SetDATA(RES_ZOMBIEINFO data)
    {
        m_UID = data.UID;
        m_Idx = data.Idx;
        m_State = data.State;
        m_Grade = m_TData.m_Grade;
    }

    public void InsertDNA(int dnaIdx)
    {
        m_DnaList.Add(dnaIdx);
    }
    public bool IS_GradeMax() {
        return TDATA.GetDnaCombinationTable(m_Grade + 1) == null;
    }
    public Dictionary<int, float> GetTimeReward() {
        Dictionary<int, float> timerewards = new Dictionary<int, float>();
        var rewards = m_TData.m_TimeRewards;
        for (int i = 0; i < rewards.Count; i++) {
            timerewards.Add(rewards[i].Idx, (float)3600f * (1f + USERINFO.GetSkillValue(SkillKind.RNATimeDown)) / rewards[i].Cnt);
        }
        return timerewards;
    }
}

//좀비 획득
//아이템 -> 뜯으면 좀비

//좀비 우리



//보상타입 추가 좀비

//확율적으로 더쌔지거나 죽어서 거름(DNA)을 남기거나


