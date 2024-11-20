using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class Item_AdventureList_Auto : ObjMng
{
	[Serializable]
	public struct SUI
	{
		public TextMeshProUGUI Name;
		public Item_DifficultyGroup DiffGroup;
		public GameObject CheckIcon;
		public Transform RewardBucket;
		public GameObject ItemPrefab;
		public Material ItemMaterial;
		public Item_GradeGroup PartyGrade;
		public Item_Adv_CharCountGroup CharCount;
		public GameObject CharPrefab;
		public Transform CharBucket;
	}
	[SerializeField]
	SUI m_SUI;
	TAdventureTable m_TData { get { return m_Info.m_TData; } }
	public List<long> m_Chars = new List<long>();
	public AdventureInfo m_Info;
	public bool m_IsConfirm = false;

	public void SetData(AdventureInfo _data, List<long> _chars) {
		m_Info = _data;
		m_Chars = _chars;
		//이름
		m_SUI.Name.text = m_TData.GetName();
		//등급
		m_SUI.DiffGroup.SetData(m_TData.m_AdventureGrade);
		//파티 등급  
		m_SUI.PartyGrade.SetData(m_TData.m_PartyGrade);

		int gradecnt = 0;
		for (int i = 0; i < m_Chars.Count; i++) {
			if (USERINFO.GetChar(m_Chars[i]).m_Grade >= m_TData.m_PartyGrade) gradecnt++;
		}
		//파티등급 인원
		m_SUI.CharCount.SetData(gradecnt, m_TData.m_PartyGradeCount);
		//파견 가능 여부 아이콘
		m_IsConfirm = m_Chars.Count == m_TData.m_PartyCount && gradecnt >= m_TData.m_PartyGradeCount;
		m_SUI.CheckIcon.SetActive(m_IsConfirm);
		//보상
		
		
		for (int i = 0; i < m_TData.m_Reward.Count; i++) {
			Item_RewardItem_Card card = Utile_Class.Instantiate(m_SUI.ItemPrefab, m_SUI.RewardBucket).GetComponent<Item_RewardItem_Card>();
			card.SetMaterial(m_SUI.ItemMaterial);
			card.transform.GetComponent<Animator>().enabled = false;
			card.transform.localScale = Vector3.one * 0.6f;
			TAdventureTable.ADReward reward = m_TData.m_Reward[i];
			card.SetData(reward.m_Idx, reward.m_Cnt);
		}
		//캐릭터
		for(int i = 0;i< _chars.Count; i++) {
			Item_Char_Adventure_Simple adchar = Utile_Class.Instantiate(m_SUI.CharPrefab, m_SUI.CharBucket).GetComponent<Item_Char_Adventure_Simple>();
			adchar.SetData(USERINFO.GetChar(_chars[i]));
		}
	}
	/// <summary> 탐험 하겠다 </summary>
	public void ClickConfirm() {
		PlayEffSound(SND_IDX.SFX_0121);
		m_IsConfirm = !m_IsConfirm;
		m_SUI.CheckIcon.SetActive(m_IsConfirm);
	}

	public void ClearBucket()
	{
		List<Transform> removeTrList = new List<Transform>();
		for (int i = 0; i < m_SUI.RewardBucket.childCount; i++)
		{
			removeTrList.Add(m_SUI.RewardBucket.GetChild(i));
		}

		for (int i = 0; i < m_SUI.CharBucket.childCount; i++)
		{
			removeTrList.Add(m_SUI.CharBucket.GetChild(i));
		}

		foreach (var tr in removeTrList)
		{
			Destroy(tr.gameObject);
		}
	}
}
