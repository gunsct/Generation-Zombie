using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class DNADecomposition : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image GradeBG;
		public Item_Inventory_Item Item;
		public TextMeshProUGUI GradeName;
		public TextMeshProUGUI Name;
		public Transform Bucket;
		public Transform Element;//Item_RewardList_Item
	}
	[SerializeField] SUI m_SUI;

	DNAInfo m_Info;
	IEnumerator m_Action;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Info = (DNAInfo)aobjValue[0];
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
		m_SUI.GradeBG.sprite = UTILE.LoadImg(string.Format("UI/BG/BG_Popup_DNA_{0}", (int)m_Info.m_TData.m_BGType - 1), "png");
		m_SUI.Item.SetData(m_Info, null);
		m_SUI.GradeName.text = string.Format("{0}{1} DNA", UTILE.Get_RomaNum(m_Info.m_Grade), TDATA.GetString(274));
		m_SUI.Name.text = m_Info.m_TData.GetName();

		var rewards = m_Info.m_TLData.m_Rewards;
		List<RES_REWARD_BASE> getrewards = new List<RES_REWARD_BASE>();
		for(int i = 0;i< rewards.Count; i++) {
			getrewards.AddRange(MAIN.GetRewardData(RewardKind.Item, rewards[i].Idx, rewards[i].Cnt));
		}
		UTILE.Load_Prefab_List(getrewards.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < getrewards.Count; i++) {
			Item_RewardList_Item element = m_SUI.Bucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			element.SetData(getrewards[i], null, false);
			element.transform.localScale = Vector3.one * 0.8f;
		}
	}
	public void ClickDecomposition() {
		if (m_Action != null) return;
		POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(966), TDATA.GetString(967),  (result, obj) => {
			if(result == 1) {
#if NOT_USE_NET
				USERINFO.DeleteDNA(m_Info);
				var rewards = m_Info.m_TLData.m_Rewards;
				List<RES_REWARD_BASE> dna = new List<RES_REWARD_BASE>();
				for(int i = 0;i< rewards.Count; i++) {
					ItemInfo info = USERINFO.InsertItem(rewards[i].Idx, rewards[i].Cnt);
					dna.Add(new RES_REWARD_ITEM() {
						UID = info.m_Uid,
						Idx = rewards[i].Idx,
						Cnt = rewards[i].Cnt,
					});
				}
				MAIN.Save_UserInfo();
				MAIN.SetRewardList(new object[] { dna }, () => {
					Close(1);
				});
#else
				WEB.SEND_REQ_DNA_DESTROY((res) => {
					if (!res.IsSuccess())
					{
						Close(0);
						WEB.StartErrorMsg(res.result_code, (btn, obj) => { });
						return;
					}
					MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
						Close(1);
					});
				}, m_Info);
#endif
			}
		});
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
