using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Item_Event_10_Gift_Element : ObjMng
{
	public class FaceMsg
	{
		public int CharIdx;
		public int DLIdx;
	}
    [Serializable]
    public struct SUI
	{
		public Animator Anim;
		public Image Portrait;
		public TextMeshProUGUI Desc;
		public Item_RewardList_Item Reward;
		public TextMeshProUGUI[] ItemCnts;
		public GameObject[] ItemGroup;
		public Image[] ItemIcon;
		public Material[] FontMats;
	}
	[SerializeField] SUI m_SUI;
	public MissionData m_Info;
	Action<MissionData> m_CB;
	Action<Item_Event_10_Gift_Element> m_CB2;

	public void SetData(MissionData _info, Action<MissionData> _cb, Action<Item_Event_10_Gift_Element> _cb2) {
		m_Info = _info;
		m_CB = _cb;
		m_CB2 = _cb2;

		FaceMsg facemsg = null;
		string evtstr = PlayerPrefs.GetString(string.Format("EVENT_FACE_MSG_{0}", _info.UID));
		if(!string.IsNullOrEmpty(evtstr)) facemsg = JsonConvert.DeserializeObject<FaceMsg>(evtstr);
		if(facemsg == null) {
			List<TCharacterTable> chardatas = TDATA.GetAllCharacterInfos();
			facemsg = new FaceMsg() { CharIdx = chardatas[UTILE.Get_Random(0, chardatas.Count)].m_Idx, DLIdx = UTILE.Get_Random(201, 211) };
			PlayerPrefs.SetString(string.Format("EVENT_FACE_MSG_{0}", _info.UID), JsonConvert.SerializeObject(facemsg));
			PlayerPrefs.Save();
		}
		m_SUI.Portrait.sprite = TDATA.GetCharacterTable(facemsg.CharIdx).GetPortrait();
		m_SUI.Desc.text = TDATA.GetString(ToolData.StringTalbe.Dialog, facemsg.DLIdx);

		PostReward cleardata = _info.m_TData.m_Rewards[0];
		List<RES_REWARD_BASE> reward = MAIN.GetRewardData(cleardata.Kind, cleardata.Idx, cleardata.Cnt);
		m_SUI.Reward.SetData(reward[0], null, false);
		for (int i = 0; i < m_SUI.ItemGroup.Length; i++) {
			if (i < _info.m_TData.m_Check.Count) {
				m_SUI.ItemCnts[i].text = _info.m_TData.m_Check[i].m_Cnt.ToString();
				m_SUI.ItemIcon[i].sprite = TDATA.GetItemTable(_info.m_TData.m_Check[i].m_Val[0]).GetItemImg();
				if (_info.m_TData.m_Check[i].m_Cnt <= _info.GetCnt(i)) {//충분
					m_SUI.ItemCnts[i].color = Utile_Class.GetCodeColor("#C0FFA2");
					m_SUI.ItemCnts[i].fontMaterial = m_SUI.FontMats[0];
				}
				else {//부족
					m_SUI.ItemCnts[i].color = Utile_Class.GetCodeColor("#FF845A");
					m_SUI.ItemCnts[i].fontMaterial = m_SUI.FontMats[1];
				}
				m_SUI.ItemGroup[i].SetActive(true);
			}
			else
				m_SUI.ItemGroup[i].SetActive(false);
		}

		switch (m_Info.State[0]) {
			case RewardState.Idle:
				m_SUI.Anim.SetTrigger(m_Info.IS_Complete() ? "Active" : "Deactive");
				break;
			case RewardState.Get:
				m_SUI.Anim.SetTrigger("Complete");
				break;
			case RewardState.None:
				m_SUI.Anim.SetTrigger("Deactive");
				break;
		}
	}
	public void Click_Reward() {
		m_CB?.Invoke(m_Info);
	}
	public void Click_Refresh() {
		m_CB2?.Invoke(this);
	}
}
