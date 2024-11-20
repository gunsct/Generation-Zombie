using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_PDA_ZombieFarm_ZombieInfo : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI MakeTimeDesc;
		public TextMeshProUGUI RoomNum;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Grade;
		public Image Portrait;
		public Transform Bucket;
		public Transform Element;//Item_RewardList_Item
		public Animator KillAnim;
		public Animator GrinderAnim;
	}

	[SerializeField] private SUI m_SUI;

	int m_RoomPos = 0;
	ZombieInfo m_Info;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		m_Info = (ZombieInfo)args[0];
		m_RoomPos = (int)args[1];

		SetUI();
	}
	void SetUI() {
		m_SUI.MakeTimeDesc.text = string.Format(TDATA.GetString(973), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, Mathf.RoundToInt(3600f * (1f - USERINFO.GetSkillValue(SkillKind.RNATimeDown)))));
		m_SUI.RoomNum.text = (m_RoomPos + 1).ToString();
		m_SUI.Name.text = m_Info.m_TData.GetName();
		m_SUI.Portrait.sprite = m_Info.m_TData.GetItemBigImg();
		m_SUI.Portrait.transform.localScale = Vector3.one * (m_SUI.Portrait.sprite.name.Contains("84_Enemy") ? 0.63f : 1f);
		m_SUI.Grade.text = string.Format("<color=#9CCD9C>Z</color>{0}", UTILE.Get_RomaNum(m_Info.m_Grade));

		Dictionary<int, float> timerewards = m_Info.GetTimeReward();
		UTILE.Load_Prefab_List(timerewards.Count, m_SUI.Bucket, m_SUI.Element);
		for (int i = 0; i < timerewards.Count; i++) {
			KeyValuePair<int, float> reward = timerewards.ElementAt(i);
			Item_PDA_RNA_Element element = m_SUI.Bucket.GetChild(i).GetComponent<Item_PDA_RNA_Element>();
			element.SetData(reward.Key, reward.Value);
		}
	}
	public void ClickDecomposition() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.ZombieDecomposition, (res, obj) => { 
			if(res == 1) {
				List<RES_REWARD_BASE> rewards = obj.GetComponent<ZombieDecomposition>().GetReward();
				StartCoroutine(IE_KillAction(rewards));
			}
		}, new List<ZombieInfo>() { m_Info });
	}
	IEnumerator IE_KillAction(List<RES_REWARD_BASE> _rewards) {
		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();

		m_SUI.KillAnim.SetTrigger("Kill");
		m_SUI.KillAnim.SetTrigger("Form_Kill");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.GrinderAnim));
		m_SUI.GrinderAnim.SetTrigger("Roll");
		PlayEffSound(SND_IDX.SFX_1106);
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.KillAnim, 1, 1));
		m_SUI.GrinderAnim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.GrinderAnim));

		if(_rewards == null) OnClose();
		else MAIN.SetRewardList(new object[] { _rewards }, OnClose);
	}
	public override void OnClose() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Play_PDA_Cloase, 0)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		m_CloaseCB?.Invoke(Item_PDA_ZombieFarm.State.RoomInfo, new object[] { m_RoomPos });
	}
}