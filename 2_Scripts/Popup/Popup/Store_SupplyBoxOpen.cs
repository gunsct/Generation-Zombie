using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;

public class Store_SupplyBoxOpen : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public ScrollRect Scroll;
		public GameObject RewardPrefab;
		public Transform BoxBucket;
		public Transform ListBucket;
		public GameObject Empty;
		public Image BoxGradeIcon;
		public Image[] Count;
		public Sprite[] CountSprite;
		public GameObject GoodFX;
		public GameObject SkipBtn;
		public Transform NextBtn;
	}
	[SerializeField] SUI m_SUI;
	List<RES_REWARD_BASE> m_Rewards = new List<RES_REWARD_BASE>();
	bool m_IsSkip = false;
	bool m_IsOpen = false;
	bool m_CanClose = false;
	IEnumerator m_Action;
	LS_Web.RES_REWARD_BASE m_UserExp;
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		m_Rewards = (List<RES_REWARD_BASE>)aobjValue[0];
		int grade = (int)aobjValue[1];
		m_SUI.BoxGradeIcon.sprite = UTILE.LoadImg(string.Format("UI/UI_Store/Icon_SupplyBox_{0}", Mathf.Min((int)(grade / 2) + 1, 5)), "png");
		StartCoroutine(OpenBoxAction());
		SetCount(m_Rewards.Count);
	}

	IEnumerator OpenBoxAction() {//get 스킵하면 애니 스피드 1.2배에 0.7초 후에서 0.2초후로
		m_UserExp = m_Rewards.Find(o => o.Type == Res_RewardType.UserExp);
		for (int i = m_Rewards.Count - 1; i > -1; i--) {
			if (m_Rewards[i] == m_UserExp) continue;
			if (m_Rewards[i].GetIdx() == BaseValue.CASH_IDX) continue;
			yield return new WaitWhile(() => m_IsOpen == false && m_IsSkip == false);
			PlayEffSound(SND_IDX.SFX_0313);

			m_SUI.Empty.SetActive(false);

			SetCount(i);
			m_SUI.Anim.SetTrigger("Get");

			Item_RewardList_Item boxreward = Utile_Class.Instantiate(m_SUI.RewardPrefab, m_SUI.BoxBucket).GetComponent<Item_RewardList_Item>();
			boxreward.transform.localScale = Vector3.one * 1.3f;
			boxreward.SetData(m_Rewards[i], null, false);
			Animator boxrewardanim = boxreward.GetComponent<Animator>();
			boxrewardanim.SetTrigger("BoxStart");
			boxrewardanim.speed = m_IsSkip ? 2f : 1f;

			yield return new WaitForSeconds(m_IsSkip ? 0.25f : 0.5f);

			Item_RewardList_Item listreward = Utile_Class.Instantiate(m_SUI.RewardPrefab, m_SUI.ListBucket).GetComponent<Item_RewardList_Item>();
			listreward.transform.localScale = Vector3.one;
			listreward.SetData(m_Rewards[i], null, false);
			Animator listrewardanim = listreward.GetComponent<Animator>();
			listrewardanim.SetTrigger("ListStart");

			m_Rewards.RemoveAt(i);
			m_IsOpen = false;

			yield return new WaitForEndOfFrame();
			m_SUI.Scroll.enabled = m_SUI.Scroll.viewport.rect.height < m_SUI.Scroll.content.rect.height;
		}

		m_SUI.SkipBtn.SetActive(false);
		yield return new WaitForSeconds(0.5f);
		m_SUI.NextBtn.SetParent(transform);//강제로 꺼내줘야함
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		m_CanClose = true;
	}
	void SetCount(int _cnt) {
		m_SUI.Count[0].sprite = m_SUI.CountSprite[_cnt / 10];
		m_SUI.Count[1].sprite = m_SUI.CountSprite[_cnt % 10];
	}
	public void ClickNext() {
		if (!m_CanClose)
			m_IsOpen = true;
		else ClickConfirm();
	}
	public void ClickSkip() {
		m_IsSkip = true;
		m_SUI.SkipBtn.SetActive(false);
	}
	public void ClickConfirm() {
		if (m_Action != null) return;
		m_SUI.Anim.SetTrigger("End_ExpWait");
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.UserExpGet, (result, obj) => { Close(0); }, m_UserExp);
	}
	//애니메이션 트리거로 발동되는 사운드
	public void AnimEvent_BoxSND() {
		PlayEffSound(SND_IDX.SFX_1020);
	}
	public void AnimEvent_ConfirmSND() {
		PlayEffSound(SND_IDX.SFX_9603);
	}
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("BlackEnd");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 1f, 3));
		base.Close(_result);
	}
}
