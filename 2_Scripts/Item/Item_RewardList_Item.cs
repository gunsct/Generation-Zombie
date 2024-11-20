using System;
using System.Collections;
using System.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;


public class Item_RewardList_Item : ObjMng
{
	public enum State
	{
		Item,
		Piece,
		DNA,
		Zombie,
		Char
	}
	[System.Serializable]
	public struct SPrefab
	{
		public GameObject Prefab;
		public Type ClassType;
	}

	[System.Serializable]
	public struct SUI
	{
		public Animator Ani;
		public GameObject Active;
		public GameObject Eff;
		public GameObject GoodFrame;
		public Item_RewardItem_Card Item;
		public Item_RewardItem_Card Piece;
		public Item_RewardDNA_Card DNA;
		public Item_ZombieReward_Card Zombie;
		public Item_CharSmall_Card Character;
		public ScrollRectEventTrigger Event;
	}
	[SerializeField] protected SUI m_SUI;
	public int m_Idx;
	protected Action<GameObject> m_SelectCB;
	public bool m_IsStartAni, m_IsStartAniControll;
	public Res_RewardType m_RewardType;
	string m_RewardName;
	long m_UID;
	int m_Grade, m_LV = 1, m_Cnt;
	public string GetRewardName() { return m_RewardName; }
	public int GetGrade() { return m_Grade; }

	private void Awake() {
		if(m_SUI.GoodFrame != null) m_SUI.GoodFrame.SetActive(false);
	}
	public void OnEnable()
	{
		if(!m_IsStartAniControll) StartAnim();
	}

	public void ShowItem(bool Active)
	{
		if(m_SUI.Active != null) m_SUI.Active.SetActive(Active);
	}

	public void StartAnim()
	{
		ShowItem(true);
		if (m_IsStartAni)
		{
			if (m_SUI.Eff != null) m_SUI.Eff.SetActive(true);
			if (m_SUI.Ani != null) m_SUI.Ani.SetTrigger("Start");
		}

		m_IsStartAni = false;
	}

	public void SetData(RES_REWARD_BASE data, Action<GameObject> selectcb = null, bool IsStartEff = true, bool IsStartAniControll = false) {
		RewardKind kind = RewardKind.Item;
		int Idx = data.GetIdx();
		m_RewardType = data.Type;
		switch (m_RewardType)
		{
		case Res_RewardType.Money:
		case Res_RewardType.Cash:
		case Res_RewardType.Exp:
		case Res_RewardType.Energy:
		case Res_RewardType.PVPCoin:
		case Res_RewardType.GCoin:
		case Res_RewardType.GPoint:
		case Res_RewardType.CampRes_Junk:
		case Res_RewardType.CampRes_Cultivate:
		case Res_RewardType.CampRes_Chemical:
		case Res_RewardType.Mileage:
			RES_REWARD_MONEY money = (RES_REWARD_MONEY)data;
			m_Cnt = money.Add;
			break;
		case Res_RewardType.Char:
			//TODO:케릭터 보상은 획득 연출을 만들어서 넣어야됨
			kind = RewardKind.Character;
			RES_REWARD_CHAR ch = (RES_REWARD_CHAR)data;
			m_Grade = ch.Grade;
			m_LV = ch.LV;
			m_UID = ch.UID;
			break;
		case Res_RewardType.Item:
			RES_REWARD_ITEM item = (RES_REWARD_ITEM)data;
			m_Grade = item.Grade;
			if (m_Grade < 1) m_Grade = TDATA.GetItemTable(data.GetIdx()).m_Grade;
			m_Cnt = item.Cnt;
			m_LV = item.LV;
			m_UID = item.UID;
			break;
		case Res_RewardType.DNA:
			kind = RewardKind.DNA;
			RES_REWARD_DNA dna = (RES_REWARD_DNA)data;
			m_Grade = dna.Grade;
			m_LV = dna.Lv;
			m_UID = dna.UID;
			break;
		case Res_RewardType.Zombie:
			kind = RewardKind.Zombie;
			RES_REWARD_ZOMBIE zombie = (RES_REWARD_ZOMBIE)data;
			m_Grade = zombie.Grade;
			m_UID = zombie.UID;
			break;
		default:
			RES_REWARD_ITEM IREW = (RES_REWARD_ITEM)data;
			m_Grade = IREW.Grade;
			m_LV = IREW.LV;
			m_Cnt = IREW.Cnt;
			m_UID = IREW.UID;
			break;
		}

		SetData(kind, Idx, m_Cnt, m_LV, m_Grade, selectcb, IsStartEff, IsStartAniControll);
	}

	public void SetData(RewardKind kind, int idx, int cnt, int LV = 1, int Grade = 0, Action<GameObject> selectcb = null, bool IsStartEff = true, bool IsStartAniControll = false)
	{
		m_Idx = idx;
		m_SelectCB = selectcb;
		m_IsStartAni = IsStartEff;
		m_IsStartAniControll = IsStartAniControll;
		m_Grade = Grade;
		m_LV = LV;
		m_Cnt = cnt;
		if (m_SUI.Eff != null) m_SUI.Eff.SetActive(false);
		switch(kind)
		{
		case RewardKind.Character:
			SetChar();
			break;
		case RewardKind.Item:
			SetItem();
			break;
		case RewardKind.DNA:
			SetDNA();
			break;
		case RewardKind.Zombie:
			SetZombie();
			break;
		}
	}

	void SetChar()
	{
		SetElementActive(State.Char);
		TCharacterTable tdata = TDATA.GetCharacterTable(m_Idx);
		m_SUI.Character.SetData(m_Idx);
		m_SUI.Character.SetLv(m_LV);
		m_SUI.Character.SetGrade(m_Grade);
		m_Grade = m_SUI.Character.m_Grade;
		m_RewardName = tdata.GetCharName();
	}

	void SetItem()
	{
		TItemTable tdata = TDATA.GetItemTable(m_Idx);
		switch (tdata.m_Type)
		{
		case ItemType.CharaterPiece:
			SetElementActive(State.Piece);
			m_SUI.Piece.SetData(m_Idx, m_Cnt);
			m_Grade = m_SUI.Piece.m_Grade;
			break;
		case ItemType.Cash:
		case ItemType.Energy:
		case ItemType.Exp:
		case ItemType.Dollar:
		case ItemType.PVPCoin:
		case ItemType.Guild_Exp:
		case ItemType.Guild_Coin:
		case ItemType.PVPcampCoin:
			SetElementActive(State.Item);
			m_SUI.Item.SetData(m_Idx, m_Cnt);
			m_Grade = m_SUI.Item.m_Grade;
			break;
		default:
			SetElementActive(State.Item);
			ItemInfo info = USERINFO.GetItem(m_UID);
				if (info != null) m_SUI.Item.SetData(info, null); //SetData(m_Idx, m_Cnt, info.m_Lv, info.m_Grade);
				else m_SUI.Item.SetData(m_Idx, m_Cnt, m_LV, m_Grade);
			m_Grade = m_SUI.Item.m_Grade;
			break;
		}
		m_RewardName = tdata.GetName();
	}

	void SetDNA()
	{
		SetElementActive(State.DNA);
		TDnaTable tdata = TDATA.GetDnaTable(m_Idx);
		m_SUI.DNA.SetData(m_Idx, -1, m_LV, -1);
		m_Grade = m_SUI.DNA.m_Grade;
		m_RewardName = tdata.GetName();
	}

	void SetZombie()
	{
		SetElementActive(State.Zombie);
		TZombieTable tdata = TDATA.GetZombieTable(m_Idx);
		m_SUI.Zombie.SetData(m_Idx, m_Grade);
		m_Grade = m_SUI.Zombie.m_Grade;
		m_RewardName = tdata.GetName();
	}

	void SetElementActive(State _state) {
		if(m_SUI.Item != null) m_SUI.Item.gameObject.SetActive(_state == State.Item);
		if (m_SUI.Piece != null) m_SUI.Piece.gameObject.SetActive(_state == State.Piece);
		if (m_SUI.DNA != null) m_SUI.DNA.gameObject.SetActive(_state == State.DNA);
		if (m_SUI.Zombie != null) m_SUI.Zombie.gameObject.SetActive(_state == State.Zombie);
		if (m_SUI.Character != null) m_SUI.Character.gameObject.SetActive(_state == State.Char);
	}
	public void SetCntActive(bool _active) {
		m_SUI.Item.GetCntGroup.SetActive(_active);
		m_SUI.Piece.GetCntGroup.SetActive(_active);
	}
	public void SetGoodFrame(bool _active) {
		if (m_SUI.GoodFrame != null) m_SUI.GoodFrame.SetActive(_active);
	}
	public virtual void ViewItemInfo()
	{
		m_SelectCB?.Invoke(gameObject);
	}
}
