using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_Atd_Element : ObjMng
{
#pragma warning disable 0649
	[System.Serializable]
	struct SUI
	{
		public Animator Ani;
		public TextMeshProUGUI Day;
		public Item_RewardList_Item Item;
		public TextMeshProUGUI Desc;
	}

	[SerializeField] SUI m_sUI;
	PostReward m_Reward;
	RES_REWARD_BASE m_Res;
#pragma warning restore 0649
	public void SetData(PostReward reward, int Day, bool ViewNumDay = false)
	{
		m_Reward = reward;
		m_sUI.Day.text = ViewNumDay ? Day.ToString() : string.Format(TDATA.GetString(472), Day);

		m_Res = reward.Get_RES_REWARD_BASE();
		m_sUI.Item.gameObject.SetActive(m_Res != null);
		if(m_sUI.Desc != null) m_sUI.Desc.gameObject.SetActive(m_Res != null);
		if (m_Res != null) {
			m_sUI.Item.SetData(m_Res);
			if (m_sUI.Desc != null) {
				if (m_Res.GetCnt() > 1) m_sUI.Desc.text = string.Format("{0} x{1}", m_Res.GetName(), m_Res.GetCnt());
				else m_sUI.Desc.text = m_Res.GetName();
			}
		}
	}

	public void SetState(bool IsNow, bool IsGet)
	{
		m_sUI.Ani.SetTrigger(IsGet ? "Get" : "NotGet");
		m_sUI.Ani.SetTrigger(IsNow ? "Now" : "NotNow");
	}

	public void StartGetAction(Action EndCB)
	{
		StartCoroutine(GetActionCheck(EndCB));
	}

	public IEnumerator GetActionCheck(Action EndCB)
	{
		yield return new WaitForSeconds(0.7f);
		m_sUI.Ani.SetTrigger("GetAni");
		// 이전에는 한프레임 쉬어주면 트리거 전환이 됐었는데 체크가 안되서 해당 애니 이름인지 확인해서 넘겨줌
		yield return new WaitWhile(() => { return !m_sUI.Ani.GetCurrentAnimatorStateInfo(0).IsName("GetAni"); });	// 플레이전
		yield return new WaitWhile(() => { return m_sUI.Ani.GetCurrentAnimatorStateInfo(0).IsName("GetAni"); });    // 플레이후
		EndCB?.Invoke();
	}

	public void OnClick()
	{
		if (m_Reward != null) POPUP.ViewItemToolTip(GetRewardInfo(), (RectTransform)transform);
	}

	public RES_REWARD_BASE GetRewardInfo()
	{
		return m_Res;
	}
}
