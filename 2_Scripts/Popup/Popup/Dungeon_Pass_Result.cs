using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Dungeon_Pass_Result : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public GameObject ItemPrefab;
		public Transform Bucket;
		public TextMeshProUGUI Exp;
		public TextMeshProUGUI Money;
		public TextMeshProUGUI UserExp;
		public ScrollRect Scroll;
	}
	[SerializeField] SUI m_SUI;
	/// <summary>서버서 인벤토리 부족 체크</summary>
	bool m_IsInvenLowGoPost;
	IEnumerator m_EndAction;
	IEnumerator m_RewardAction;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		PlayEffSound(SND_IDX.SFX_0310);

		List<RES_REWARD_BASE> list = new List<RES_REWARD_BASE>();
		int Exp = 0;
		int Money = 0;
		int Cash = 0;
		int UserExp = 0;

#if NOT_USE_NET
		Exp = (int)aobjValue[0];
		Money = (int)aobjValue[1];
		Cash = (int)aobjValue[2];
		UserExp = (int)aobjValue[3];
		list = (List<RES_REWARD_BASE>)aobjValue[4];
#else
		RES_STAGE_CLEAR_TICKET res = (RES_STAGE_CLEAR_TICKET)aobjValue[0];
		list = MAIN.GetRewardList(res.GetRewards());

		RES_REWARD_MONEY rgcoin = (RES_REWARD_MONEY)list.Find(o => o.Type == Res_RewardType.GPoint);
		if (rgcoin != null) {
			list.Remove(rgcoin);
		}
		RES_REWARD_MONEY rexp = (RES_REWARD_MONEY)list.Find(o => o.Type == Res_RewardType.Exp);
		if (rexp != null) {
			Exp += rexp.Add;
			list.Remove(rexp);
		}
		RES_REWARD_MONEY rmoney = (RES_REWARD_MONEY)list.Find(o => o.Type == Res_RewardType.Money);
		if (rmoney != null) {
			Money += rmoney.Add;
			list.Remove(rmoney);
		}
		RES_REWARD_MONEY rcash = (RES_REWARD_MONEY)list.Find(o => o.Type == Res_RewardType.Cash);
		if (rcash != null) {
			Cash += rcash.Add;
			list.Remove(rcash);
		}
		RES_REWARD_USEREXP ruserexp = (RES_REWARD_USEREXP)list.Find(o => o.Type == Res_RewardType.UserExp);
		if (ruserexp != null) {
			UserExp += (int)ruserexp.AExp;
			list.Remove(ruserexp);
		}
		//for (int i = res.Rewards.Count - 1; i > -1; i--) {
		//	RES_REWARDS Base = res.Rewards[i];
		//	for (int j = Base.Rewards.Count - 1; j > -1; j--) {
		//		RES_REWARD reward = Base.Rewards[j];
		//		switch (reward.Type) {
		//			case Res_RewardType.Exp:
		//				Exp += ((RES_REWARD_MONEY)reward.Infos[0]).Add;
		//				break;
		//			case Res_RewardType.Money:
		//				Money += ((RES_REWARD_MONEY)reward.Infos[0]).Add;
		//				break;
		//			case Res_RewardType.Cash://캐시 안준다고 함
		//				Cash += ((RES_REWARD_MONEY)reward.Infos[0]).Add;
		//				break;
		//			case Res_RewardType.UserExp:
		//				UserExp += (int)((RES_REWARD_USEREXP)reward.Infos[0]).AExp;
		//				break;
		//			default:
		//				list.AddRange(reward.Infos);
		//				break;
		//		}
		//	}
		//}
#endif

		m_SUI.Exp.text = Exp.ToString();
		m_SUI.Money.text = Money.ToString();
		m_SUI.UserExp.text = UserExp.ToString();

		m_RewardAction = IE_GetReward(list);
		StartCoroutine(m_RewardAction);
	}

	IEnumerator IE_GetReward(List<RES_REWARD_BASE> _rewards) {
		for (int i = 0; i < _rewards.Count; i++) {
			PlayEffSound(SND_IDX.SFX_0313);

			Item_RewardList_Item reward = Utile_Class.Instantiate(m_SUI.ItemPrefab, m_SUI.Bucket).GetComponent<Item_RewardList_Item>();
			reward.SetData(_rewards[i], null, true);
			reward.gameObject.SetActive(false);
			m_SUI.Scroll.horizontalNormalizedPosition = 1f;
			if (_rewards[i].result_code == EResultCode.SUCCESS_POST && !m_IsInvenLowGoPost) {
				m_IsInvenLowGoPost = true;
				POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(535));
			}
			yield return new WaitForSeconds(0.15f);
			reward.gameObject.SetActive(true);
		}
	}
	public void ClickClose() {
		if (m_EndAction != null) return;
		m_EndAction = IE_CloseAction();
		StartCoroutine(m_EndAction);
	}

	IEnumerator IE_CloseAction() {
		StopCoroutine(m_RewardAction);
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		StopCoroutine(m_RewardAction);
		Close(0);
	}
}
