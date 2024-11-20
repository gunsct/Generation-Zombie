using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;

public class Item_PDA_ZombieFarm_SetRoom : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI MakeTimeDesc;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;
		public Item_Zp_Zombie_Element[] ZobieCards;
		public Transform RNABucket;
		public Transform RNAElement;
		public Transform ZombieBucket;
		public Transform ZombieElement;//Item_ZombieFarm_Catched_Element

		public GameObject[] Empty;
		public Animator[] SortingBtnAnim;

		public ScrollRect ZombieScroll;
		public GameObject[] TutoObj;//0:좀비 리스트
	}

	[SerializeField] SUI m_SUI;
	ZombieRoomInfo m_RInfo;
	List<Item_ZombieFarm_Catched_Element> m_ZombieList = new List<Item_ZombieFarm_Catched_Element>();
	int m_RNASort = 4100;
	bool Is_PreMain;
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);

		m_RInfo = USERINFO.m_ZombieRoom.Find(o=>o.Pos == (int)args[0]);
		Is_PreMain = (bool)args[1];
		SetUI();

		if (TUTO.IsTuto(TutoKind.Zombie, (int)TutoType_Zombie.Select_Zombie_Room)) TUTO.Next();
	}
	void SetUI() {
		m_SUI.MakeTimeDesc.text = string.Format(TDATA.GetString(973), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, Mathf.RoundToInt(3600f * (1f - USERINFO.GetSkillValue(SkillKind.RNATimeDown)))));
		m_SUI.Name.text = string.Format("ROOM # <color=#B9CFAF><size=123%><B>{0}</B></size></color>", m_RInfo.Pos + 1);
		m_SUI.Cnt.text = string.Format("{0}/{1}", m_RInfo.ZUIDs.Count, BaseValue.ZOMBIE_CAGE_INSIZE);
		//좀비 아이콘 세팅
		for (int i = 0; i < m_SUI.ZobieCards.Length; i++)
		{
			if (i < m_RInfo.ZUIDs.Count)
			{
				ZombieInfo zinfo = USERINFO.GetZombie(m_RInfo.ZUIDs[i]);
				m_SUI.ZobieCards[i].SetData(zinfo, 0, null);
				m_SUI.ZobieCards[i].GetIcon.sprite = zinfo.m_TData.GetItemBigImg();
			}
			else m_SUI.ZobieCards[i].SetData(null, 0, null);
		}
		//시간당 RNA
		Dictionary<int, float> timerewards = m_RInfo.GetTimeReward();
		m_SUI.Empty[0].SetActive(timerewards.Count < 1);
		UTILE.Load_Prefab_List(timerewards.Count, m_SUI.RNABucket, m_SUI.RNAElement);
		for (int i = 0; i < timerewards.Count; i++)
		{
			KeyValuePair<int, float> reward = timerewards.ElementAt(i);
			Item_PDA_RNA_Element element = m_SUI.RNABucket.GetChild(i).GetComponent<Item_PDA_RNA_Element>();
			element.SetData(reward.Key, reward.Value);
		}
		//USERINFO.m_NotCageZombie
		//보유중인 좀비 정보 넣기
		m_SUI.Empty[1].SetActive(USERINFO.m_NotCageZombie.Count < 1);
		m_ZombieList.Clear();
		UTILE.Load_Prefab_List(USERINFO.m_NotCageZombie.Count, m_SUI.ZombieBucket, m_SUI.ZombieElement);
		for (int i = 0; i < USERINFO.m_NotCageZombie.Count; i++)
		{
			Item_ZombieFarm_Catched_Element element = m_SUI.ZombieBucket.GetChild(i).GetComponent<Item_ZombieFarm_Catched_Element>();
			element.SetData(Item_ZombieFarm_Catched_Element.State.Set, USERINFO.m_NotCageZombie[i], SetRoom);
			m_ZombieList.Add(element);
		}

		ClickRNASort(4100);
	}
	public void ClickRNASort(int _idx) {//4101~4105
		m_RNASort = _idx;
		for (int i = 0; i < m_SUI.SortingBtnAnim.Length; i++) {
			m_SUI.SortingBtnAnim[i].SetTrigger(i + 4100 == _idx ? "On" : "Off");
		}
		SetSort();
	}
	void SetSort() {
		for(int i = 0;i< m_ZombieList.Count;i++){
			Item_ZombieFarm_Catched_Element zombiecard = m_ZombieList[i];
			//zombiecard.GetComponent<RectTransform>().SetAsFirstSibling();
			zombiecard.gameObject.SetActive(m_RNASort == 4100 || zombiecard.GetComponent<Item_ZombieFarm_Catched_Element>().m_Info.GetTimeReward().ContainsKey(m_RNASort));
		}
	}
	void SetRoom(bool _set, ZombieInfo _info) {
#if NOT_USE_NET
#else
		WEB.SEND_REQ_ZOMBIE_SET((res) => {
			if (res.IsSuccess()) {
				PlayEffSound(SND_IDX.SFX_1120);
				SetUI();
			}
		}, _info, m_RInfo.Pos);
#endif
	}
	public void ClickDel(int _pos)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_ZombieFarm_SetRoom, 1)) return;
		if (_pos > m_RInfo.ZUIDs.Count - 1) return;
		ZombieInfo zinfo = USERINFO.GetZombie(m_RInfo.ZUIDs[_pos]);

		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.ZombieDecomposition, (res, obj) => {
			if (res == 1) {
				List<RES_REWARD_BASE> rewards = obj.GetComponent<ZombieDecomposition>().GetReward();
				if (rewards == null) {
					SetUI();
				}
				else MAIN.SetRewardList(new object[] { rewards }, () => {
					SetUI();
				});
			}
		}, new List<ZombieInfo>() { zinfo });
//		POPUP.Set_MsgBox(PopupName.Msg_YN, TDATA.GetString(980), TDATA.GetString(981), (result, obj) => {
//			if ((EMsgBtn)result == EMsgBtn.BTN_YES) {
//#if NOT_USE_NET
//#else
//				WEB.SEND_REQ_ZOMBIE_DESTROY((res) => {
//					if (res.IsSuccess()) {
//						if (res.GetRewards() == null) SetUI();
//						else MAIN.SetRewardList(new object[] { res.GetRewards() }, SetUI);
						
//					}
//				}, new List<ZombieInfo>() { zinfo });
//#endif
//			}
//		});
	}
	public override void OnClose() {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.Item_PDA_ZombieFarm_SetRoom, 0)) return;
		PlayEffSound(SND_IDX.SFX_0121);
		if (Is_PreMain) m_CloaseCB?.Invoke(Item_PDA_ZombieFarm.State.Main, null);
		else m_CloaseCB?.Invoke(Item_PDA_ZombieFarm.State.RoomInfo, new object[] { m_RInfo.Pos });
	}

	///////튜토용
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}

	public virtual void ScrollLock(bool _lock)
	{ 
		m_SUI.ZombieScroll.enabled = !_lock;
	}

	public Item_ZombieFarm_Catched_Element GetZombieListItem(int Pos)
	{
		return m_SUI.ZombieBucket.GetChild(Pos).GetComponent<Item_ZombieFarm_Catched_Element>();
	}
	
}
