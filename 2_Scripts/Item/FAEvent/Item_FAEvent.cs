using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_FAEvent : ObjMng
{
	public enum State
	{
		In = 0,
		Out
	}
	Animator m_Ani;
	[HideInInspector] public MyFAEvent m_Data;
	[HideInInspector] public float m_OutAni_Idletime = 16f / 60f;
	bool m_AutoReward;
	public virtual void SetData(MyFAEvent data)
	{
		m_Ani = gameObject.GetComponent<Animator>();
		m_Data = data;
	}

	public IEnumerator PlayAni(State state)
	{
		Utile_Class.AniResetAllTriggers(m_Ani);
		string name = state.ToString();
		m_Ani.SetTrigger(name);
		if (state != State.Out) yield break;

		yield return new WaitWhile(() => { return !m_Ani.GetCurrentAnimatorStateInfo(0).IsName(name); });
		AnimatorStateInfo info = m_Ani.GetCurrentAnimatorStateInfo(0);
		// 시작 연출 타임
		float actiontime = m_OutAni_Idletime / info.length;
		float distorytime = Mathf.Max(info.length - m_OutAni_Idletime, 0);
		yield return new WaitWhile(() => {
			info = m_Ani.GetCurrentAnimatorStateInfo(0);
			return info.normalizedTime < actiontime;
		});

		GameObject.Destroy(gameObject, distorytime);
	}

	public void ViewReward(Action EndCB)
	{
		StartCoroutine(RewardAction(EndCB));
	}

	public virtual IEnumerator RewardAction(Action EndCB)
	{
		if(!m_Data.IsReward())
		{
			EndCB?.Invoke();
			yield break;
		}
		bool check = false;
		bool IsNew = false;
		WEB.SEND_REQ_CHECK_MY_FAEVENT_INFO((res) => {
			// 이전값과 달라졌으면
			IsNew = m_Data.Values.SequenceEqual(res.Values);
			m_Data.SetDATA(res);
			if (res.Rewards != null)
			{
				List<RES_REWARD_BASE> Rewards = res.GetRewards();
				if (Rewards.Count > 0)
				{
					CheckItemAction(() => {
						PlayEffSound(SND_IDX.SFX_0171);
						POPUP.Set_MsgBox(PopupName.Msg_RewardGet_Center, (btn, obj) => { check = true; }, Rewards, Msg_RewardGet_Center.Action.Get);
						if (MAIN.IS_State(MainState.PLAY)) POPUP.GetMainUI().GetComponent<Main_Play>().SetPostAlram();
					});
					return;
				}
			}
			check = true;
		}, m_Data);

		yield return new WaitUntil(() => check);
		EndCB?.Invoke();
	}

	public virtual void CheckItemAction(Action CB)
	{
		CB?.Invoke();
	}
}
