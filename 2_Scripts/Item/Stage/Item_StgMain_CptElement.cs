using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;

public class Item_StgMain_CptElement : ObjMng
{
	public enum State
	{
		/// <summary> 현재 스테이지 </summary>
		NowStg,
		/// <summary> 다음 스테이지들</summary>
		NotClear,
		/// <summary>이전 스테이지들 </summary>
		Clear,
		/// <summary> 현재스테이지 완료될때 </summary>
		Complete,
		/// <summary> 현재 스테이지 챕터 보상 받을때 </summary>
		RecieveReward,
		/// <summary> 현재 스테이지 될 때 </summary>
		Set
	}
	[Serializable]
    public struct SUI
	{
		public Animator Anim;
		public TextMeshProUGUI Number;
		public GameObject RewardGroup;
		public Item_RewardList_Item Card;
		public Image Arrow;
		public GameObject LastRewardFX;
	}
	[SerializeField]
	SUI m_SUI;
	int m_StgIdx;
	State m_State;
	private void Awake() {
		m_SUI.RewardGroup.SetActive(false);
		m_SUI.LastRewardFX.SetActive(false);
	}
	public void SetData(int _viewstgidx, int _nowstageidx, bool _getreward) {
		m_SUI.Number.text = (_viewstgidx % 100).ToString();

		SetCard(_viewstgidx);

		bool changechap = _nowstageidx / 100 < USERINFO.m_Stage[StageContentType.Stage].Idxs[USERINFO.GetDifficulty()].Idx / 100;
		if (_viewstgidx > _nowstageidx) {
			m_SUI.Anim.SetTrigger(State.NotClear.ToString());
		}
		//보여줄 스테이지가 클리어 했고 보상 안받은 경우, 챕터가 바뀐 경우에 따른 애니메이션
		else if (_viewstgidx < _nowstageidx) { 
			m_SUI.Anim.SetTrigger(_nowstageidx - _viewstgidx > (changechap ? 0 : 1) || !_getreward ? State.Clear.ToString() : State.Set.ToString());
		}
		else {
			m_SUI.Anim.SetTrigger(changechap || !_getreward ? State.NowStg.ToString() : State.NotClear.ToString());
		}
	}
	void SetCard(int _viewstgidx) {

		TChapterTable chaptable = TDATA.GetChapterTable(USERINFO.GetDifficulty(), _viewstgidx);
		if (chaptable == null) return;

		RES_REWARD_BASE res = new RES_REWARD_BASE();
		int grade = 1;
		RewardKind kind = chaptable.m_RewardType;
		//chaptable
		switch (kind) {
			case RewardKind.None:
			case RewardKind.Event:
				return;
			case RewardKind.Character:
				CharInfo charInfo = new CharInfo(chaptable.m_Reward, 0, 0, Mathf.Max(1, chaptable.m_Val));
				grade = charInfo.m_Grade;
				RES_REWARD_CHAR rchar = new RES_REWARD_CHAR();
				rchar.SetData(charInfo);
				res = rchar;
				break;
			case RewardKind.Item:
				TItemTable tdata = TDATA.GetItemTable(chaptable.m_Reward);
				RES_REWARD_MONEY rmoney;
				RES_REWARD_ITEM ritem;
				switch (tdata.m_Type) {
					case ItemType.Dollar:
						rmoney = new RES_REWARD_MONEY();
						rmoney.Type = Res_RewardType.Money;
						rmoney.Befor = USERINFO.m_Money - chaptable.m_RewardCount;
						rmoney.Now = USERINFO.m_Money;
						rmoney.Add = chaptable.m_RewardCount;
						res = rmoney;
						break;
					case ItemType.Cash:
						rmoney = new RES_REWARD_MONEY();
						rmoney.Type = Res_RewardType.Cash;
						rmoney.Befor = USERINFO.m_Cash - chaptable.m_RewardCount;
						rmoney.Now = USERINFO.m_Cash;
						rmoney.Add = chaptable.m_RewardCount;
						res = rmoney;
						break;
					case ItemType.Energy:
						rmoney = new RES_REWARD_MONEY();
						rmoney.Type = Res_RewardType.Energy;
						rmoney.Befor = USERINFO.m_Energy.Cnt - chaptable.m_RewardCount;
						rmoney.Now = USERINFO.m_Energy.Cnt;
						rmoney.Add = chaptable.m_RewardCount;
						rmoney.STime = (long)USERINFO.m_Energy.STime;
						res = rmoney;
						break;
					case ItemType.InvenPlus:
						rmoney = new RES_REWARD_MONEY();
						rmoney.Type = Res_RewardType.Inven;
						rmoney.Befor = USERINFO.m_InvenSize - chaptable.m_RewardCount;
						rmoney.Now = USERINFO.m_InvenSize;
						rmoney.Add = chaptable.m_RewardCount;
						res = rmoney;
						break;
					default:
						ritem = new RES_REWARD_ITEM();
						ritem.Type = Res_RewardType.Item;
						ritem.UID = 0;
						ritem.Idx = chaptable.m_Reward;
						ritem.Cnt = chaptable.m_RewardCount;
						ritem.LV = Mathf.Max(1, chaptable.m_Val);
						res = ritem;
						grade = TDATA.GetItemTable(ritem.Idx).m_Grade;
						break;
				}
				break;
			case RewardKind.Zombie:
				ZombieInfo zombieInfo = new ZombieInfo(chaptable.m_Reward);
				RES_REWARD_ZOMBIE zombie = new RES_REWARD_ZOMBIE();
				zombie.UID = zombieInfo.m_UID;
				zombie.Idx = zombieInfo.m_Idx;
				zombie.Grade = zombieInfo.m_Grade;
				res = zombie;
				grade = zombie.Grade;
				break;
			case RewardKind.DNA:
				DNAInfo dnaInfo = new DNAInfo(chaptable.m_Reward);
				RES_REWARD_DNA dna = new RES_REWARD_DNA();
				dna.UID = dnaInfo.m_UID;
				dna.Idx = dnaInfo.m_Idx;
				dna.Grade = dnaInfo.m_Grade;
				dna.Lv = Mathf.Max(1, chaptable.m_Val);
				res = dna;
				grade = dna.Grade;
				break;
		}
		m_SUI.Card.SetData(res, null, false);
		m_SUI.LastRewardFX.SetActive(TDATA.GetStageTable(_viewstgidx).m_NextIdx / 100 != _viewstgidx / 100);
		m_SUI.Arrow.sprite = UTILE.LoadImg($"UI/UI_StgMain/ItemFrameArrow_{grade}", "png");
		m_SUI.Card.gameObject.SetActive(true); 
		m_SUI.RewardGroup.SetActive(true);
	}
	public void SetAnim(State _ani) {
		m_SUI.Anim.SetTrigger(_ani.ToString());
	}
}
