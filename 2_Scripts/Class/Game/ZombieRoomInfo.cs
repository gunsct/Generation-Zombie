using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static LS_Web;

public class ZombieRoomInfo : ClassMng
{
    /// <summary> 인덱스 </summary>
    public int Pos = 0;
    /// <summary> 좀비들(Idx) </summary>
    public List<long> ZUIDs = new List<long>();
    /// <summary> 생성 시작 시간 </summary>
    public long PTime;

    /// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
    public ZombieRoomInfo() { }
    public void SetDATA(RES_ZOMBIE_ROOM_INFO data)
    {
        Pos = data.Pos;
        ZUIDs = data.ZUIDs;
        PTime = data.PTime;
    }
    /// <summary> 쌓인 보상 </summary>
    public List<RES_REWARD_BASE> GetStackReward() {
        List<RES_REWARD_BASE> getrewards = new List<RES_REWARD_BASE>();
        //시간당 보상
        for (int i = 0; i < ZUIDs.Count; i++) {
            ZombieInfo info = USERINFO.GetZombie(ZUIDs[i]);
            if (info == null) continue;

            var rewards = info.m_TData.m_TimeRewards;
            for (int j = 0; j < rewards.Count; j++) {
                int cnt = Mathf.RoundToInt((float)Mathf.Min(BaseValue.RNA_PRODUCTION_MAX_TIME, (int)((UTILE.Get_ServerTime_Milli() - PTime) * (1f + USERINFO.GetSkillValue(SkillKind.RNATimeDown)) * 0.001d)) / rewards[j].Cnt);
                if (cnt > 0) {
                    var reward = getrewards.Find(o => o.GetIdx() == rewards[j].Idx);
                    if (reward == null) getrewards.AddRange(MAIN.GetRewardData(RewardKind.Item, rewards[j].Idx, cnt));
                    else ((RES_REWARD_ITEM)reward).Cnt += cnt;
                }
            }
        }
        return getrewards;
    }
    public Dictionary<int, float> GetTimeReward() {
        Dictionary<int, float> timerewards = new Dictionary<int, float>();

        //시간당 보상
        for (int i = 0; i < ZUIDs.Count; i++) {
            ZombieInfo info = USERINFO.GetZombie(ZUIDs[i]);
            if (info == null) continue;
            var rewards = info.GetTimeReward();
            for (int j = 0; j < rewards.Count; j++) {
                KeyValuePair<int, float> reward = rewards.ElementAt(j);
                if (timerewards.ContainsKey(reward.Key)) timerewards[reward.Key] += reward.Value;
                else timerewards.Add(reward.Key, reward.Value);
            }
        }
        return timerewards;
    }
    /// <summary> 수거 하고부터 흐른 시간(초) </summary>
    public int GetPastTime() {
        return (int)((UTILE.Get_ServerTime_Milli() - PTime) * 0.001d);
    }
}


