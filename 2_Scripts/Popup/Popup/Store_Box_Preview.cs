using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;
using System.Linq;

public class Store_Box_Preview : PopupBase
{
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Image[] Lv;
		public Sprite[] Nums;
		public Image BoxGradeIcon;
		public GameObject Reward;
		public GameObject RewardPrefab;
		public Transform RewardIconPrefab;
		public Transform Bucket;
		public TextMeshProUGUI Exp;
		public TextMeshProUGUI UserExp;
		public TextMeshProUGUI Money;
	}
	[SerializeField] SUI m_SUI;
	IEnumerator m_Action; //end ani check
	Dictionary<ItemType, List<RES_REWARD_BASE>> m_AllReward = new Dictionary<ItemType, List<RES_REWARD_BASE>>();
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		TSupplyBoxTable sbtdata = (TSupplyBoxTable)aobjValue[0];
		int grade = (int)aobjValue[1];

		m_SUI.Lv[0].sprite = m_SUI.Nums[grade % 10];
		m_SUI.Lv[1].sprite = m_SUI.Nums[grade % 100 / 10];
		m_SUI.Lv[2].sprite = m_SUI.Nums[grade / 100];
		m_SUI.BoxGradeIcon.sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Icon_SupplyBox_{0}", Mathf.Min((int)(grade / 2) + 1, 5)), "png");

		//묶음 보상 표시
		Dictionary<int, int> groups = sbtdata.m_Reward;
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		for (int i = 0; i < groups.Count; i++) {
			rewards.AddRange(TDATA.GetGachaItem_All(groups.ElementAt(i).Key, false));
		}
		List<RES_REWARD_BASE> chartickets = rewards.FindAll(o => o.GetIdx() == BaseValue.CHAR_GACHA_TICKET_IDX);
		int[] ctcnt = new int[2];
		ctcnt[0] = chartickets.Min(o => o.GetCnt());
		ctcnt[1] = chartickets.Max(o => o.GetCnt());
		rewards.RemoveAll(o => o.GetIdx() == BaseValue.CHAR_GACHA_TICKET_IDX);

		List<RES_REWARD_BASE> eqtickets = rewards.FindAll(o => o.GetIdx() == BaseValue.EQ_GACHA_TICKET_IDX);
		int[] eqtcnt = new int[2];
		eqtcnt[0] = eqtickets.Min(o => o.GetCnt());
		eqtcnt[1] = eqtickets.Max(o => o.GetCnt());
		rewards.RemoveAll(o => o.GetIdx() == BaseValue.EQ_GACHA_TICKET_IDX);


		m_AllReward = USERINFO.GetRewardGroup(rewards);
		for (int i = 0; i < m_AllReward.Count; i++) {
			var val = m_AllReward.ElementAt(i);
			Item_Store_Box_Preview_Element reward = Utile_Class.Instantiate(m_SUI.Reward, m_SUI.Bucket).GetComponent< Item_Store_Box_Preview_Element>();
			int[] cnt = new int[2];
			cnt[0] = val.Value.Min(o => o.GetCnt());
			cnt[1] = val.Value.Max(o => o.GetCnt());
			reward.SetData(val.Key, val.Value, cnt);
		}

		//개별 보상 표시
		m_SUI.Exp.text = string.Format("{0}~{1}", sbtdata.Exp[0], sbtdata.Exp[1]);
		m_SUI.UserExp.text = string.Format("{0}~{1}", sbtdata.UserExp[0], sbtdata.UserExp[1]);
		m_SUI.Money.text = string.Format("{0}~{1}", sbtdata.Dollar[0], sbtdata.Dollar[1]);

		RES_REWARD_ITEM charticket = new RES_REWARD_ITEM() { Idx = BaseValue.CHAR_GACHA_TICKET_IDX, Type = Res_RewardType.Item };
		Item_Store_Box_Preview_Element rewardcharticket = Utile_Class.Instantiate(m_SUI.Reward, m_SUI.Bucket).GetComponent<Item_Store_Box_Preview_Element>();
		rewardcharticket.SetData(charticket, ctcnt);

		RES_REWARD_ITEM eqticket = new RES_REWARD_ITEM() { Idx = BaseValue.EQ_GACHA_TICKET_IDX, Type = Res_RewardType.Item };
		Item_Store_Box_Preview_Element rewardeqticket = Utile_Class.Instantiate(m_SUI.Reward, m_SUI.Bucket).GetComponent<Item_Store_Box_Preview_Element>();
		rewardeqticket.SetData(eqticket, eqtcnt);

		//RES_REWARD_ITEM gold = new RES_REWARD_ITEM() { Idx = BaseValue.CASH_IDX, Type = Res_RewardType.Item };
		//Item_Store_Box_Preview_Element rewardgold = Utile_Class.Instantiate(m_SUI.Reward, m_SUI.Bucket).GetComponent<Item_Store_Box_Preview_Element>();
		//rewardgold.SetData(gold, sbtdata.Cash);

		RES_REWARD_ITEM passticket = new RES_REWARD_ITEM() { Idx = BaseValue.CLEARTICKET_IDX, Type = Res_RewardType.Item };
		Item_Store_Box_Preview_Element rewardticket = Utile_Class.Instantiate(m_SUI.Reward, m_SUI.Bucket).GetComponent<Item_Store_Box_Preview_Element>();
		rewardticket.SetData(passticket, sbtdata.PassTicket);

	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");

		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		base.Close(_result);
	}
}
