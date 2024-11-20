using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using static LS_Web;
using TMPro;

public class ChapterClearReward : PopupBase
{
    [Serializable]
    public struct SUI
	{
		public Animator Anim;//CharChange -> start 끝나고, End 
		public Image[] ChapNum10;
		public Image[] ChapNum1;
		public GameObject CloseBtn;
		public Item_RewardList_Item[] RewardCard;//캐릭터 조각 받으면 캐릭터 체크해서 0번카드 세팅해줘야할듯
		public Item_CharacterCard CharRewardCard;
		public GameObject[] PickupObj;
		public Item_CharacterCard PickChar;
		public GameObject NewCharMark;
	}
	[SerializeField] SUI m_SUI;
	RES_REWARD_BASE m_Reward;
	GameObject m_CrntInfo;
	IEnumerator m_Action; //end ani check

	private void Awake() {
		m_SUI.CloseBtn.SetActive(false);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_Reward = (RES_REWARD_BASE)aobjValue[0];

		StartCoroutine(AnimSND());
		int chapnum = USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx / 100 - 1;
		for (int i = 0; i < m_SUI.ChapNum10.Length; i++) {
			if (chapnum / 10 == 0) m_SUI.ChapNum10[i].gameObject.SetActive(false);
			else m_SUI.ChapNum10[i].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum / 10), "png");
		}
		for (int i = 0; i < m_SUI.ChapNum1.Length; i++) {
			m_SUI.ChapNum1[i].sprite = UTILE.LoadImg(string.Format("Font/StgMain_ImgFont/ImageFont_Chp_{0}", chapnum % 10), "png");
		}

		m_SUI.NewCharMark.SetActive(m_Reward.Type == Res_RewardType.Char);
		switch (m_Reward.Type) {
			case Res_RewardType.Item:
				m_SUI.RewardCard[0].gameObject.SetActive(m_Reward.result_code != EResultCode.SUCCESS_REWARD_PIECE);
				m_SUI.CharRewardCard.gameObject.SetActive(m_Reward.result_code == EResultCode.SUCCESS_REWARD_PIECE);
				if (m_Reward.result_code == EResultCode.SUCCESS_REWARD_PIECE) {//캐릭터 받았는데 조각으로 들어온 경우
																			   //1번에 조각 넣기
					m_SUI.RewardCard[1].SetData(m_Reward, null, false);
					//0번째에 보유 캐릭터 넣기
					CharInfo charinfo = USERINFO.m_Chars.Find(t => t.m_TData.m_PieceIdx == m_Reward.GetIdx());
					m_SUI.CharRewardCard.SetData(charinfo);
					StartCoroutine(CharChangeAction());
				}
				else {
					m_SUI.RewardCard[0].SetData(m_Reward, null, false);
					StartCoroutine(IE_ItemInfoAction());
				}
				break;
			default:
				m_SUI.RewardCard[0].gameObject.SetActive(m_Reward.Type != Res_RewardType.Char);
				m_SUI.CharRewardCard.gameObject.SetActive(m_Reward.Type == Res_RewardType.Char);
				if (m_Reward.Type == Res_RewardType.Char) m_SUI.CharRewardCard.SetData(USERINFO.GetChar(m_Reward.GetIdx()));
				else m_SUI.RewardCard[0].SetData(m_Reward, null, false);
				switch (m_Reward.Type) {
					case Res_RewardType.Char:
						StartCoroutine(IE_CharInfoAction());
						break;
					case Res_RewardType.Cash:
					case Res_RewardType.Energy:
					case Res_RewardType.Exp:
					case Res_RewardType.Inven:
					case Res_RewardType.Money:
						StartCoroutine(IE_ItemInfoAction());
						break;
					default:
						StartCoroutine(PickUpAction());
						break;
				}
				break;
		}

		base.SetData(pos, popup, cb, aobjValue);
	}
	IEnumerator IE_ItemInfoAction() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		bool delay = false;
		ItemInfo iteminfo = null;
		if (m_Reward.Type == Res_RewardType.Item && TDATA.GetItemTable(m_Reward.GetIdx()).GetEquipType() != EquipType.End)
		{
			// 장비류만 정보 보이기
			iteminfo = USERINFO.GetItem(((RES_REWARD_ITEM)m_Reward).UID);
			delay = true;
			POPUP.ViewItemInfo((result, obj) => { delay = false; }, new object[] { iteminfo, m_Popup, null }).OnlyInfo();
		}
		yield return new WaitWhile(() => delay);

		yield return PickUpAction();
	}
	IEnumerator IE_CharInfoAction() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));

		bool delay = true;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Info_Char_NotGet, (result, obj) => { delay = false; }, m_Reward.GetIdx(), Info_Character_NotGet.State.Normal);

		yield return new WaitWhile(() => delay == true);

		yield return PickUpAction();
	}
	IEnumerator CharChangeAction() {
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => (!m_SUI.RewardCard[0].gameObject.activeSelf && !m_SUI.CharRewardCard.gameObject.activeSelf) || Utile_Class.IsAniPlay(m_SUI.Anim));

		m_SUI.Anim.SetTrigger("CharChange");

		yield return IE_ItemInfoAction();
	}
	IEnumerator PickUpAction() {
		int stgidx = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx;
		bool addpick = BaseValue.IS_PickupOpenStage(stgidx);
		int charidx = 0;
		List<TCharacterTable> datas = TDATA.GetAllCharacterInfos();
		for(int i = 0; i < datas.Count; i++) {
			if (datas[i].m_SelectivePickupStage == stgidx) {
				charidx = datas[i].m_Idx;
				break;
			}
		}

		if (addpick || charidx != 0) {
			m_SUI.PickupObj[0].SetActive(addpick);
			m_SUI.PickupObj[1].SetActive(charidx != 0);
			if (charidx != 0) {
				m_SUI.PickChar.SetData(charidx);
				m_SUI.PickChar.SetGrade(m_SUI.PickChar.m_TData.m_Grade);
				m_SUI.PickChar.SetCharState(true);
			}

			m_SUI.Anim.SetTrigger("Urgent");
			yield return new WaitForEndOfFrame();
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		}
		m_SUI.CloseBtn.SetActive(true);
	}
	IEnumerator AnimSND() {
		PlayEffSound(SND_IDX.SFX_0310);
		PlayEffSound(SND_IDX.SFX_0330);
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 200f / 360f));
		PlayEffSound(SND_IDX.SFX_0331);
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("End");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
