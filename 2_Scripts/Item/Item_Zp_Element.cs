using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static LS_Web;
using Image = UnityEngine.UI.Image;

public class Item_Zp_Element : ObjMng
{
    [Serializable]
    public struct SUI
    {
        public Animator Anim;
        public TextMeshProUGUI RoomName;
        public TextMeshProUGUI Cnt;
        public Image[] Icon;
        public TextMeshProUGUI[] RNACnt;
        public TextMeshProUGUI RewardRatio;

        public GameObject[] TutoObj;//0:배치 버튼
    }

    [SerializeField] private SUI m_SUI;

    ZombieRoomInfo m_Info;
    int m_Pos;
    Action<int> m_CB;
    Action<object, object[]> m_StateCB;

    float m_Timer;
    IEnumerator m_Action;

	private void Update() {
        if (m_Info != null) {
            m_Timer += Time.deltaTime;
            if (m_Timer >= 1f) {
                m_Timer = 0f;
                SetReward();
            }
        }
    }
	public void SetData(int _pos, Action<int> _cb, Action<object, object[]> _statecb)
    {
        m_Pos = _pos;
        m_Info = USERINFO.m_ZombieRoom.Find(o=>o.Pos == m_Pos);
        m_CB = _cb;
        m_StateCB = _statecb;
        m_SUI.RoomName.text = string.Format("ROOM # <color=#B9CFAF><size=123%><B>{0}</B></size></color>", m_Pos + 1);

        SetUI();
    }
    
    public void SetUI()
    {
        if (m_Info == null || m_Info?.ZUIDs.Count < 1)
        {
            m_SUI.Anim.SetTrigger(m_Info != null ? "Empty" : "Lock");
            for (int i = 0; i < m_SUI.Icon.Length; i++) {
                m_SUI.Icon[i].gameObject.SetActive(false);
            }
        }
        else
        {
            m_SUI.Anim.SetTrigger("Normal");
            for (int i = 0; i < m_SUI.Icon.Length; i++) {
                m_SUI.Icon[i].gameObject.SetActive(i < m_Info.ZUIDs.Count);
                if (i < m_Info.ZUIDs.Count) {
                    ZombieInfo zinfo = USERINFO.GetZombie(m_Info.ZUIDs[i]);
                    m_SUI.Icon[i].sprite = zinfo.m_TData.GetItemBigImg();
                    m_SUI.Icon[i].transform.localScale = Vector3.one * (m_SUI.Icon[i].sprite.name.Contains("84_Enemy") ? 0.63f : 1f);
                }
            }
        }
        m_SUI.Cnt.text = string.Format("{0}/{1}", m_Info == null ? 0 : m_Info.ZUIDs.Count, BaseValue.ZOMBIE_CAGE_INSIZE);

        SetReward();
    }
    void SetReward() {
        List<RES_REWARD_BASE> rewards = m_Info?.GetStackReward();
        for (int i = 0; i < m_SUI.RNACnt.Length; i++) {
            //RNA_IDX - 4101, 4102, 4103, 4104, 4105
            RES_REWARD_ITEM reward = (RES_REWARD_ITEM)rewards?.Find(o => o.GetIdx() % 10 - 1 == i);
            m_SUI.RNACnt[i].text = reward == null ? "0" : reward.Cnt.ToString();
        }
        m_SUI.RewardRatio.gameObject.SetActive(m_Info != null);
        if (m_Info != null) {
            float ratio = Mathf.Clamp((float)m_Info.GetPastTime() / 86400f * 100f, 0f, 100f);//흐른시간 / 12시간 최대 1 m_Info.PTime
            m_SUI.RewardRatio.text = string.Format("{0:0.0}%", ratio);
        }
    }
    public void ClickViewRoom()
    {
        if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_Zp_Element_Btn, 0, m_Pos)) return;
        m_CB?.Invoke(m_Pos);
    }
    public void ClickSetting() {
        if (m_Info == null) return;
        if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_Zp_Element_Btn, 1, m_Pos)) return;
        m_StateCB?.Invoke(Item_PDA_ZombieFarm.State.SetRoom, new object[] { m_Info.Pos, true });
    }
    public void ClickGetReward()
    {
        if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_Zp_Element_Btn, 2, m_Pos)) return;
        if (m_Info.GetStackReward().Count < 1) {
            POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(984));
            return;
		}
		else {
#if NOT_USE_NET
#else
           WEB.SEND_REQ_ZOMBIE_PRODUCE((res) => {
                if (res.IsSuccess()) {
                    MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
                        SetReward();
                    });
                }
            }, new List<int>() { m_Info.Pos });
#endif
        }
    }

    ///////튜토용
    public GameObject GetTutoObj(int _idx)
    {
        return m_SUI.TutoObj[_idx];
    }
}