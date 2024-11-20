using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static LS_Web;

public class Item_PDA_ZombieFarm_Main : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public GameObject Prefab;
		public Transform Bucket;
		public ScrollRect CageScroll;
		public GameObject CatchCntGroup;
		public TextMeshProUGUI CatchCnt;
		public GameObject DNAMakeAlarm;
		public GameObject DNAMakeLock;

		public GameObject[] TutoObj;//0:DNA생성 버튼
	}

	[SerializeField] private SUI m_SUI;
	List<Item_Zp_Element> ItemZpElementList = new List<Item_Zp_Element>();
	// 초기 사육장 오픈 수 GlobalWeightTable = StartZombieSlot
	private List<long> cageZombies;

	public override void SetData(Action<object, object[]> CloaseCB, object[] args)
	{
		base.SetData(CloaseCB, args);

		SetUI();
		if (TUTO.IsTuto(TutoKind.Zombie, (int)TutoType_Zombie.Select_PDA_ZombieFram)) TUTO.Next();
		else if (TUTO.IsTuto(TutoKind.DNA_Make, (int)TutoType_DNA_Make.Select_PDA_ZombieFram)) TUTO.Next();
	}
	void SetUI() {
		cageZombies = USERINFO.m_CageZobie.Select(o => o.m_UID).ToList();

		ItemZpElementList.Clear();
		UTILE.Load_Prefab_List(BaseValue.ZOMBIE_CAGE_MAX, (RectTransform)m_SUI.Bucket, (RectTransform)m_SUI.Prefab.transform);
		for (var i = 0; i < m_SUI.Bucket.childCount; i++) {
			var itemZpElement = m_SUI.Bucket.GetChild(i).GetComponent<Item_Zp_Element>();
			ItemZpElementList.Add(itemZpElement);
			itemZpElement.SetData(i, ClickViewRoom, m_CloaseCB);
		}
		m_SUI.CatchCntGroup.SetActive(USERINFO.m_NotCageZombie.Count > 0);
		m_SUI.CatchCnt.text = USERINFO.m_NotCageZombie.Count.ToString();
		bool dnamakeopen = USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1;
		m_SUI.DNAMakeAlarm.SetActive(USERINFO.IS_CanMakeAnyDNA() && dnamakeopen);
		m_SUI.DNAMakeLock.SetActive(!dnamakeopen);
	}
	
	public void ClickViewRoom(int cageIdx) {
		PlayEffSound(SND_IDX.SFX_0121);
		if (USERINFO.IsLockCage(cageIdx)) {
			StartCoroutine(SetCageScrollMove(USERINFO.CageCnt));
			USERINFO.OpenZombieCageProcess((succ)=> {
				if (succ) {
					PlayEffSound(SND_IDX.SFX_1100);
					SetUI();
				}
			});
		}
		else {
			TW_ScrollVer(1f - (float)cageIdx / (float)BaseValue.ZOMBIE_CAGE_MAX);
			m_CloaseCB.Invoke(Item_PDA_ZombieFarm.State.RoomInfo, new object[] { cageIdx });
			m_SUI.DNAMakeAlarm.SetActive(USERINFO.IS_CanMakeAnyDNA());
		}
	}

	public void ClickDNAMaking() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_ZombieFarm_Main, 0)) return;
		int openidx = BaseValue.CONTENT_OPEN_IDX(ContentType.CharDNA) + 1;
		if (USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx < openidx) {
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, string.Format(TDATA.GetString(273), openidx / 100, openidx % 100, TDATA.GetString(325)));
			return;
		}
		PlayEffSound(SND_IDX.SFX_0121);
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAMaking, (result, obj) => {
			m_SUI.DNAMakeAlarm.SetActive(USERINFO.IS_CanMakeAnyDNA());
		}, DNAMaking.PreState.ZombieFarm);
	}
	public void ClickZombieList()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_ZombieFarm_Main, 1)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		m_CloaseCB?.Invoke(Item_PDA_ZombieFarm.State.CatchedList, null);
	}
	public void ClickAllGet()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_ZombieFarm_Main, 2)) return;
		PlayEffSound(SND_IDX.SFX_0121);

		List<RES_REWARD_BASE> rewards = new List<RES_REWARD_BASE>();
		for (int i = 0; i < USERINFO.m_ZombieRoom.Count; i++) {
			var roomrewards = USERINFO.m_ZombieRoom[i].GetStackReward();
			for (int j = 0; j < roomrewards.Count; j++) {
				var overridereward = rewards.Find(o => o.GetIdx() == roomrewards[j].GetIdx());
				if (overridereward == null) rewards.Add(roomrewards[j]);
				else ((RES_REWARD_ITEM)overridereward).Cnt += ((RES_REWARD_ITEM)roomrewards[j]).Cnt;
			}
		}
		if (rewards.Count < 1) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(984));
			return;
		}
		else {
			m_CloaseCB?.Invoke(Item_PDA_ZombieFarm.State.AllGetConfirm, new object[] { rewards });
			m_SUI.DNAMakeAlarm.SetActive(USERINFO.IS_CanMakeAnyDNA());
		}
	}
	IEnumerator SetCageScrollMove(int _pos = 0) {
		ScrollLock(true);
		float pre = m_SUI.CageScroll.verticalNormalizedPosition;
		float next = 1f - (float)_pos / (float)BaseValue.ZOMBIE_CAGE_MAX;
		iTween.ValueTo(gameObject, iTween.Hash("from", pre, "to", next, "onupdate", "TW_ScrollVer", "time", 0.3f));

		yield return new WaitForSeconds(0.3f);

		ScrollLock(false);
	}
	void TW_ScrollVer(float _amount) {
		m_SUI.CageScroll.verticalNormalizedPosition = _amount;
	}
	public override void OnClose()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_ZombieFarm_Main, 3)) return;
		m_CloaseCB.Invoke(Item_PDA_ZombieFarm.State.End, null);
	}

	public virtual void ScrollLock(bool _lock)
	{
		m_SUI.CageScroll.enabled = !_lock;
	}

	public Item_Zp_Element GetListItem(int Pos)
	{
		return m_SUI.Bucket.GetChild(Pos).GetComponent<Item_Zp_Element>();
	}
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}

}