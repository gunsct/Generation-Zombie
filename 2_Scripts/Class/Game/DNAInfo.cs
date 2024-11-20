
using Newtonsoft.Json;
using System.Collections.Generic;
using static LS_Web;

public class DNAInfo : ClassMng
{
    public int m_Idx;
    public long m_UID;

    /// <summary> 추가 스탯 </summary>
    public List<ItemStat> m_AddStat = new List<ItemStat>();

    [JsonIgnore] public TDnaTable m_TData => TDATA.GetDnaTable(m_Idx);
    [JsonIgnore] public TDNALevelTable m_TLData => TDATA.GetDNALevelTable(m_TData.m_BGType, m_Grade, m_Lv);
    [JsonIgnore] public string m_Name => m_TData.GetName();
    [JsonIgnore] public string m_Desc => m_TData.GetDesc();
    [JsonIgnore] public bool m_GetAlarm;
    [JsonIgnore] public int m_Grade { get { return m_TData.m_Grade; } }
    public int m_Lv;
    
    /// <summary> Save & Load 용 Load시 클래스 생성후 데이터를 씌우는 방식이므로 기본 생성자가 있어야함 </summary>
    public DNAInfo() { }

    public DNAInfo(int idx, int lv = 1, long uid = 0)
    {
        if (uid == 0) uid = Utile_Class.GetUniqeID();

        m_UID = uid;
        m_Idx = idx;

        m_Lv = lv;
        for (int i = 0; i < m_Lv - 1; i++) {
            // 등급에 맞는 옵션 셋팅해준다.
            TRandomStatTable table = null;
            if (m_TLData.m_EssentialStatGrant == 0) {
                table = TDATA.GetPickRandomStat(m_TData.m_RandStatGroup);
            }
			else if(m_TLData.m_EssentialStatGrant > 0) {
                table = TDATA.GetRandomStatTable(m_TLData.m_EssentialStatGrant);
            }
            if (table == null) return;
            m_AddStat.Add(new ItemStat() {
#if NOT_USE_NET
				m_Stat = table.m_Stat,
				m_Val = table.GetVal()
#else
                m_Stat = StatType.None,
                m_Val = 0
#endif
            });
        }

        m_GetAlarm = true;
    }

    public void SetDATA(RES_DNAINFO data)
    {
        m_UID = data.UID;
        m_Idx = data.Idx;
        m_Lv = data.LV;

        m_AddStat.Clear();
        for (int i = 0; i < data.AddStat.Count; i++) m_AddStat.Add(data.AddStat[i].GetItemStat());
    }

    public float GetOptionValue(StatType _type) {
        float per = 0;
        //추가 옵션
        for (int i = 0; i < m_AddStat.Count; i++) {
            if (m_AddStat[i].m_Stat != _type) continue;
            per += m_AddStat[i].m_Val;
        }
        return per * 0.0001f;
    }
}