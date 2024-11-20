using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LS_Web;

public class Item_PDA_ZombieFarm_RoomInfo : Item_PDA_Base
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI MakeTimeDesc;
		public TextMeshProUGUI Name;
		public Transform GenBucket;
		public Transform GenElement;    //Item_PDA_RNA_Element
		public TextMeshProUGUI InCnt;
		public Transform ZombieBucket;
		public Transform ZombieElement; //Item_Zp_Zombie_Element
		public TextMeshProUGUI TimerRatio;
		public Transform RewardBucket;
		public Transform RewardElement; //Item_RewardList_Item
		public TextMeshProUGUI[] RFBtnNum;
		public GameObject[] RFBtnObj;
		public GameObject Empty;
		public Image GetBtn;
		public Material[] GetBtnMat;
		public GameObject[] BtnAlarms;
	}

	[SerializeField] private SUI m_SUI;

	int m_RoomPos;
	float m_Timer;
	ZombieRoomInfo m_RInfo;
	List<RES_REWARD_BASE> m_Getrewards = new List<RES_REWARD_BASE>();

	private void Update() {
		if (m_RInfo != null) {
			m_Timer += Time.deltaTime;
			if (m_Timer >= 1f) {
				m_Timer = 0f;
				SetReward();
			}
		}
	}
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
		m_RoomPos = (int)args[0];

		m_RInfo = USERINFO.m_ZombieRoom.Find(o => o.Pos == m_RoomPos);
		SetUI();
	}
	void SetUI() {
		m_SUI.MakeTimeDesc.text = string.Format(TDATA.GetString(973), UTILE.GetSecToTimeStr(Utile_Class.TimeStyle.single, Mathf.RoundToInt(3600f * (1f - USERINFO.GetSkillValue(SkillKind.RNATimeDown)))));
		m_SUI.Name.text = string.Format("ROOM # <color=#B9CFAF><size=123%><B>{0}</B></size></color>", m_RInfo.Pos + 1);
		m_SUI.InCnt.text = string.Format("{0}/{1}", m_RInfo.ZUIDs.Count, BaseValue.ZOMBIE_CAGE_INSIZE);

		//시간당 RNA
		Dictionary<int, float> timerewards = m_RInfo.GetTimeReward();
		m_SUI.Empty.SetActive(timerewards.Count < 1);
		UTILE.Load_Prefab_List(timerewards.Count, m_SUI.GenBucket, m_SUI.GenElement);
		for (int i = 0; i < timerewards.Count; i++) {
			KeyValuePair<int, float> reward = timerewards.ElementAt(i);
			Item_PDA_RNA_Element element = m_SUI.GenBucket.GetChild(i).GetComponent<Item_PDA_RNA_Element>();
			element.SetData(reward.Key, reward.Value);
		}
		//좀비 카드
		UTILE.Load_Prefab_List(m_RInfo.ZUIDs.Count, m_SUI.ZombieBucket, m_SUI.ZombieElement);
		for (int i = 0; i < m_RInfo.ZUIDs.Count; i++) {
			Item_Zp_Zombie_Element element = m_SUI.ZombieBucket.GetChild(i).GetComponent<Item_Zp_Zombie_Element>();
			element.SetData(USERINFO.GetZombie(m_RInfo.ZUIDs[i]), m_RoomPos, m_CloaseCB);
		}
		SetReward();

		int pre = m_RoomPos - 1;
		m_SUI.RFBtnObj[0].SetActive(pre >= 0);
		if (pre < 0) pre = USERINFO.m_ZombieRoom.Count - 1;
		int next = m_RoomPos + 1;
		m_SUI.RFBtnObj[1].SetActive(next <= USERINFO.m_ZombieRoom.Count - 1);
		if (next > USERINFO.m_ZombieRoom.Count - 1) next = 0;
		m_SUI.RFBtnNum[0].text = string.Format("# <color=#B9CFAF><size=123%><B>{0}</B></size></color>", pre + 1);
		m_SUI.RFBtnNum[1].text = string.Format("# <color=#B9CFAF><size=123%><B>{0}</B></size></color>", next + 1);

		m_SUI.BtnAlarms[0].SetActive(m_RInfo.ZUIDs.Count < BaseValue.ZOMBIE_CAGE_INSIZE && USERINFO.m_NotCageZombie.Count > 0);
	}
	void SetReward() {
		m_Getrewards = m_RInfo.GetStackReward();
		UTILE.Load_Prefab_List(m_Getrewards.Count, m_SUI.RewardBucket, m_SUI.RewardElement);
		for (int i = 0; i < m_Getrewards.Count; i++) {
			Item_RewardList_Item element = m_SUI.RewardBucket.GetChild(i).GetComponent<Item_RewardList_Item>();
			element.SetData(m_Getrewards[i], null, false);
			element.transform.localScale = Vector3.one * 0.6f;
		}
		//int retime = Mathf.RoundToInt(86400f * (1f - USERINFO.GetSkillValue(SkillKind.RNATimeDown)));
		float ratio = m_RInfo.ZUIDs.Count < 1 ? 0f : Mathf.Min((float)m_RInfo.GetPastTime() / 86400f * 100f, 100f);//흐른시간 / 12시간 최대 1 m_Info.PTime
		int sec = Mathf.Max(0, 86400 - (int)((UTILE.Get_ServerTime_Milli() - m_RInfo.PTime) * 0.001d));
		m_SUI.TimerRatio.text = string.Format("{0} ({1:0.0}%)", UTILE.GetSecToTimeStr(sec), ratio);
		m_SUI.GetBtn.material = m_SUI.GetBtnMat[m_Getrewards.Count < 1 ? 0 : 1];
		m_SUI.BtnAlarms[1].SetActive(ratio >= 80f);
	}
	/// <summary> 세팅 페이지로 변경 </summary>
	public void ClickSetRoom() {
		m_CloaseCB?.Invoke(Item_PDA_ZombieFarm.State.SetRoom, new object[] { m_RInfo.Pos, false });
	}
	/// <summary> 지금까지 모인 보상 수거 </summary>
	public void ClickGetReward() {
		if (m_Getrewards.Count < 1) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, string.Empty, TDATA.GetString(984));
			return;
		}
		WEB.SEND_REQ_ZOMBIE_PRODUCE((res) => {
			if (res.IsSuccess()) {
				MAIN.SetRewardList(new object[] { res.GetRewards() }, () => {
					SetReward();
				});
				SetUI();
			}
		}, new List<int>() { m_RoomPos });
	}
	public void ClickChangeRoom(int _add) {
		PlayEffSound(SND_IDX.SFX_0121);
		m_RoomPos += _add;
		if (m_RoomPos < 0) m_RoomPos = USERINFO.m_ZombieRoom.Count - 1;
		else if (m_RoomPos > USERINFO.m_ZombieRoom.Count - 1) m_RoomPos = 0;
		m_RInfo = USERINFO.m_ZombieRoom.Find(o=>o.Pos == m_RoomPos);
		SetUI();
	}
}