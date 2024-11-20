using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static LS_Web;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class Gacha_RewardList : PopupBase
{
   [Serializable]
   public struct SUI
	{
		public Animator Anim;
		public Transform Element;//Item_Gacha_RewardList_Element
		public Transform GradeGroup;
		public Transform Bucket;
		public TextMeshProUGUI Title;
		public ScrollRect Scroll;
		public GameObject[] CharTabGroup;
		public Item_Tab[] Tab;

		public TextMeshProUGUI NowEffect;
		public TextMeshProUGUI NextEffect;
	}
	[SerializeField] SUI m_SUI;
	List<int> m_SelectCharIdxs = new List<int>();
	IEnumerator m_Action; //end ani check
	int m_TabPos = 0;
	public class GachaRewardNProb
	{
		public TGachaGroupTable TData;
		public RES_REWARD_BASE Reward;
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);

		int gidx = (int)aobjValue[0];
		bool character = (bool)aobjValue[1];
		m_SUI.CharTabGroup[0].SetActive(character);
		m_SUI.CharTabGroup[1].SetActive(!character);
		m_SelectCharIdxs = USERINFO.GetGachaPickUp();
		m_SUI.Title.text = string.Format(TDATA.GetString(815), TDATA.GetString(character ? 817 : 112));

		if (character) {
			m_SUI.Tab[0].SetData(0, TDATA.GetString(1113), Click_ChangeMenu);
			m_SUI.Tab[1].SetData(1, TDATA.GetString(1008), Click_ChangeMenu);
			m_SUI.Tab[2].SetData(2, TDATA.GetString(1009), Click_ChangeMenu);
			m_SUI.Tab[0].OnClick();
		}
		else {
			GachaGroup group = TDATA.GetGachaGroup(gidx);
			List<RES_REWARD_BASE> rewards = TDATA.GetGachaItem_All(gidx, false, character);
			//등급별 아이템 타입별(11종) 확률 보여주기, 리스트 다 보여주게 될것
			Dictionary<int, List<GachaRewardNProb>> equip = new Dictionary<int, List<GachaRewardNProb>>();
			Dictionary<int, int> probs = new Dictionary<int, int>();
			int sumprob = 0;
			for(int i = m_SUI.Bucket.childCount - 1; i > -1; i--) {
				Destroy(m_SUI.Bucket.GetChild(i).gameObject);
			}

			for (int i = 0; i < rewards.Count; i++) {
				int idx = rewards[i].GetIdx();
				TItemTable tdata = TDATA.GetItemTable(idx);
				TGachaGroupTable gachagroup = group.m_List.Find(o => o.m_RewardIdx == idx);
				GachaRewardNProb repfob = new GachaRewardNProb() { TData = gachagroup, Reward = rewards[i] };

				if (!equip.ContainsKey(tdata.m_Grade)) equip.Add(tdata.m_Grade, new List<GachaRewardNProb>());
				if (!equip[tdata.m_Grade].Contains(repfob)) equip[tdata.m_Grade].Add(repfob);
				if (!probs.ContainsKey(tdata.m_Grade)) probs.Add(tdata.m_Grade, 0);
				probs[tdata.m_Grade] += gachagroup.m_Prob;
				sumprob += gachagroup.m_Prob;
			}

			for (int i = equip.Count - 1; i > -1; i--) {
				int grade = equip.ElementAt(i).Key;
				Item_Gacha_RewardList_Grade gradegroup = Utile_Class.Instantiate(m_SUI.GradeGroup.gameObject, m_SUI.Bucket).GetComponent<Item_Gacha_RewardList_Grade>();
				gradegroup.SetData(grade, (float)probs[grade] / (float)sumprob);

				List<GachaRewardNProb> items = equip[grade];
				items.Sort((GachaRewardNProb _before, GachaRewardNProb _after) => {
					return _before.TData.m_Prob.CompareTo(_after.TData.m_Prob);
				});
				for (int j = 0;j< items.Count;j++) {
					Item_Gacha_RewardList_Element item = Utile_Class.Instantiate(m_SUI.Element.gameObject, m_SUI.Bucket).GetComponent<Item_Gacha_RewardList_Element>();
					item.SetData(items[j].Reward, (float)items[j].TData.m_Prob / (float)sumprob);
				}
			}
		}

		//m_SUI.NowEffect.text = TDATA.GetEquipGachaTable(USERINFO.GetEquipGachaLv().Lv).GetEffectDesc();
		//m_SUI.NextEffect.text = TDATA.GetEquipGachaTable(USERINFO.GetEquipGachaLv().Lv).GetEffectDesc();
	}
	bool Click_ChangeMenu(Item_Tab _tab) {
		m_TabPos = _tab.m_Pos;
		for (int i = 0; i < m_SUI.Tab.Length; i++)
			m_SUI.Tab[i].SetActive(m_SUI.Tab[i].m_Pos == _tab.m_Pos);
		m_SUI.Scroll.verticalNormalizedPosition = 1;
		//등급 순으로 내림차순 해서 등급별로 태그 달리게
		//selectcharidxs 이미 선택된 녀석들은 체크랑 확률표기 다르게
		for (int i = m_SUI.Bucket.childCount - 1;i>= 0; i--) {
			Destroy(m_SUI.Bucket.GetChild(i).gameObject);
		}
		int gid = 0;
		switch (m_TabPos) {
			case 0:
			case 1:
				gid = 1; break;
			case 2: gid = 10; break;
		}
		List<TPickupGachaGroupTable> pickupdatas = TDATA.GetPickupGachaGroupTable(gid);
		int probsum = pickupdatas.Sum(o => o.m_TotalProb);
		List<TCharacterTable> allchars = TDATA.GetAllCharacterInfos();
		Dictionary<int, List<TCharacterTable>> gradechars = new Dictionary<int, List<TCharacterTable>>();
		//등급별 나눔
		for (int i = 0; i < allchars.Count; i++) {
			TCharacterTable data = allchars[i];
			if (!gradechars.ContainsKey(data.m_Grade)) gradechars.Add(data.m_Grade, new List<TCharacterTable>());
			gradechars[data.m_Grade].Add(data);
		}
		for (int i = BaseValue.CHAR_MAX_RANK; i > 0; i--) {
			if (!gradechars.ContainsKey(i)) continue;
			int grade = i;
			TPickupGachaGroupTable pickdata = pickupdatas.Find(o => o.m_RewardGrade == grade);
			if (pickdata == null || pickdata.m_TotalProb == 0) continue;
			List<TCharacterTable> tdatas = gradechars[grade];
			int selectcharcnt = tdatas.FindAll(o => m_SelectCharIdxs.Contains(o.m_Idx)).Count;
			//등급 라벨
			Item_Gacha_RewardList_Grade gradegroup = Utile_Class.Instantiate(m_SUI.GradeGroup.gameObject, m_SUI.Bucket).GetComponent<Item_Gacha_RewardList_Grade>();
			gradegroup.SetData(grade, (float)pickdata.m_TotalProb / (float)probsum, false);
			//해당 등급 캐릭터들
			for (int j = 0; j < tdatas.Count; j++) {
				int selectprob = selectcharcnt == 0 ? 0 : Mathf.Min(pickdata.m_SelectedProb, pickdata.m_TotalProb / selectcharcnt);
				int nonselectprob = (pickdata.m_TotalProb - selectprob * selectcharcnt) / (tdatas.Count - selectcharcnt);
				int prob = 0;

				if (m_SelectCharIdxs.Contains(tdatas[j].m_Idx)) prob = selectprob;
				else prob = nonselectprob;

				if (prob < 1) continue;
				Item_Gacha_RewardList_Element charelement = Utile_Class.Instantiate(m_SUI.Element.gameObject, m_SUI.Bucket).GetComponent<Item_Gacha_RewardList_Element>();
				RES_REWARD_CHAR reward = new RES_REWARD_CHAR() {
					Idx = tdatas[j].m_Idx,
					Grade = tdatas[j].m_Grade,
					LV = 1,
					UID = 0,
					Type = Res_RewardType.Char
				};
				charelement.SetData(reward, (float)prob / (float)probsum, prob == selectprob);
			}
		}
		return true;
	}
	public override void Close(int Result = 0) {
		if (m_Action != null) return;
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		m_Action = CloseAction(Result);
		StartCoroutine(m_Action);
	}
	IEnumerator CloseAction(int _result) {
		m_SUI.Anim.SetTrigger("Close");
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim));
		base.Close(_result);
	}
}
