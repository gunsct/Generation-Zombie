using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_PostList : ObjMng
{
	public enum AniName
	{
		Empty = 0,
		Start
	}

	[Serializable]
	public struct SUI
	{
		public RectTransform RotPanel;
		public TextMeshProUGUI Title;
		public TextMeshProUGUI Msg;
		public Item_RewardList_Item Item;
		public GameObject Btn;
		public TextMeshProUGUI LimitTime;
		public Image BG;
		public Color[] BGColor;

		public Animator Ani;
	}
	[SerializeField] SUI m_SUI;

	public PostInfo m_Info;
	Action<Item_PostList> m_ClickCB;
	string m_Rewardname;
	double m_LimitTime;

	private void Update() {
		if (m_Info != null && m_LimitTime > 0) {
			m_LimitTime = (double)(m_Info.ETime - UTILE.Get_ServerTime_Milli()) * 0.001;
			if (m_LimitTime < 0) m_LimitTime = 0;
			m_SUI.LimitTime.text = string.Format(TDATA.GetString(4005), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, m_LimitTime));
		}
	}
	public void SetData(PostInfo info, Action<Item_PostList> _cb, Vector2 pos, Vector3 Rot) {
		m_Info = info;
		m_ClickCB = _cb;

		m_LimitTime = (double)(m_Info.ETime - UTILE.Get_ServerTime_Milli()) * 0.001;

		m_SUI.RotPanel.anchoredPosition = pos;
		m_SUI.RotPanel.eulerAngles = Rot;
		m_SUI.BG.color = m_SUI.BGColor[UTILE.Get_Random(0, m_SUI.BGColor.Length)];
		if (info.Rewards.Count < 1)
		{
			m_SUI.Item.gameObject.SetActive(false);
			m_SUI.Btn.gameObject.SetActive(false);
		}
		else
		{
			m_SUI.Item.gameObject.SetActive(true);
			PostReward reward = info.Rewards[0];
			m_SUI.Btn.gameObject.SetActive(reward.State != RewardState.Get);
			m_SUI.Item.SetData(reward.Get_RES_REWARD_BASE());
		}

		m_SUI.Title.text = info.GetTitle();
		m_SUI.Msg.text = info.GeMsg(m_Rewardname);
	}

	public void StartAni(AniName name)
	{
		m_SUI.Ani.ResetTrigger(name.ToString());
		m_SUI.Ani.SetTrigger(name.ToString());
	}

	/// <summary> 카드 선택시 제작 위해 인덱스 반환 </summary>
	public void ClickBtn() {
		m_ClickCB?.Invoke(this);
	}
}
