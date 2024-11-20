using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using static LS_Web;
using UnityEngine.UI;
using System.Text;

public class DNAMaking : PopupBase
{
	public enum PreState
	{
		Inventory,
		ZombieFarm
	}
	[Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Animator[] TabAnims;
		public TextMeshProUGUI Title; //섹 DNA 생산
		public TextMeshProUGUI[] GradeProb;
		public TextMeshProUGUI[] Dollar;
		public Mat[] Mats;
		public Image MakeBtnBG;
		public Sprite[] MakeBtnBGImg;
		public GameObject MakeBtnGlow;
		public GameObject[] Alarmas;

		public GameObject[] TutoObj;//0:DNA 종류 레이아웃, 1:info, 2:making
	}
	[Serializable]
	public struct SRUI
	{
		public Image Icon;
		public Sprite[] IconImg;
		public Item_RewardDNA_Card DNA;
		public TextMeshProUGUI[] Name; // 이름 DNA - 로마숫자
		public TextMeshProUGUI Desc;
	}
	[Serializable]
	public struct Mat
	{
		public GameObject Group;
		public Item_RewardList_Item Card;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Cnt;         //<color=#58AE4E>200</color> / 200  <color=#FF5151>200</color> / 200
		public GameObject Lack;
	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SRUI m_SRUI;
	DNABGType m_SelectType = DNABGType.Red;
	List<TMakingTable> m_TDatas;
	TMakingTable m_TData;
	IEnumerator m_Action;
	GachaGroup m_GachaGroup;
	PreState m_PreState;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		m_PreState = (PreState)aobjValue[0];
		m_TDatas = TDATA.GetGroupMakingTable(MakingGroup.DNA);
		m_TDatas.Sort((before, after) => {
			return before.m_ItemIdx.CompareTo(after.m_ItemIdx);
		});
		m_TData = m_TDatas[(int)m_SelectType - 1];
		StartCoroutine(m_Action = InitColorDelay());

		base.SetData(pos, popup, cb, aobjValue);
		if (TUTO.IsTuto(TutoKind.DNA_Make, (int)TutoType_DNA_Make.Select_DNA_Making)) TUTO.Next();
	}
	public override void SetUI() {
		base.SetUI();

		m_SUI.Title.text = string.Format("{0} DNA {1}", BaseValue.GetDNAColorName(m_SelectType), TDATA.GetString(812));

		Dictionary<int, int> gradeprop = new Dictionary<int, int>();
		m_GachaGroup = TDATA.GetGachaGroup(m_TData.m_ItemIdx);
		for(int i = 0;i< m_GachaGroup.m_List.Count; i++) {
			if (!gradeprop.ContainsKey(m_GachaGroup.m_List[i].m_RewardGrade)) gradeprop.Add(m_GachaGroup.m_List[i].m_RewardGrade, 0);
			gradeprop[m_GachaGroup.m_List[i].m_RewardGrade] += m_GachaGroup.m_List[i].m_Prob;
		}
		for(int i = 0;i< gradeprop.Count; i++) {
			m_SUI.GradeProb[i].text = string.Format("{0:0.0}%", (float)gradeprop[i + 1] / (float)m_GachaGroup.m_TotalProb * 100f);
		}
		var mats = m_TData.m_Mats;
		for (int i = 0; i < m_SUI.Mats.Length; i++) {
			m_SUI.Mats[i].Group.SetActive(mats.Count > i);
			if (mats.Count > i) {
				RES_REWARD_BASE mat = MAIN.GetRewardData(RewardKind.Item, mats[i].m_Idx, mats[i].m_Count)[0];
				int getcnt = USERINFO.GetItemCount(mats[i].m_Idx);
				int needcnt = Mathf.RoundToInt(mats[i].m_Count * (1f - USERINFO.GetSkillValue(SkillKind.DNAProduceDown)));
				m_SUI.Mats[i].Card.SetData(mat, null, false);
				m_SUI.Mats[i].Card.SetCntActive(false);
				m_SUI.Mats[i].Name.text = TDATA.GetItemTable(mat.GetIdx()).GetName();//#58AE4E, #FF5151
				m_SUI.Mats[i].Cnt.text = string.Format("<color={0}>{1}</color> / {2}", getcnt >= needcnt ? "#58AE4E" : "#FF5151", getcnt, needcnt);
				m_SUI.Mats[i].Lack.SetActive(getcnt < needcnt);
			}
		}

		int dollar = m_TData.m_Dollar;
		m_SUI.Dollar[0].text =  Utile_Class.CommaValue(USERINFO.m_Money);
		m_SUI.Dollar[1].text = Utile_Class.CommaValue(dollar);
		m_SUI.Dollar[0].color = BaseValue.GetUpDownStrColor(USERINFO.m_Money, dollar);

		bool canmake = m_TData.GetCanMake();
		m_SUI.MakeBtnBG.sprite = canmake ? m_SUI.MakeBtnBGImg[0] : m_SUI.MakeBtnBGImg[1];
		m_SUI.MakeBtnGlow.SetActive(canmake);

		for (int i = 0; i < 4; i++) {
			m_SUI.Alarmas[i].SetActive(m_TDatas[i].IS_EnoughMat() && m_TDatas[i].IS_EnoughDollar());
		}
	}
	IEnumerator InitColorDelay() {
		for (int i = 0; i < m_SUI.TabAnims.Length; i++) {
			m_SUI.TabAnims[i].SetTrigger("NotSelect");
		}
		yield return new WaitForEndOfFrame();
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 35f / 56f, 1));

		m_Action = null;
		ClickSelectType((int)DNABGType.Red);
	}
	public void ClickMake()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DNAMaking, 0)) return;
		if (m_Action != null) return;

		if (!USERINFO.CheckBagSize()) {
			WEB.StartErrorMsg(EResultCode.ERROR_INVEN_SIZE);
			return;
		}
		if (!m_TData.IS_EnoughDollar()) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.StartLackPop(BaseValue.DOLLAR_IDX);
			return;
		}
		if (!m_TData.IS_EnoughMat()) {
			PlayCommVoiceSnd(VoiceType.Fail);
			POPUP.Set_MsgBox(PopupName.Msg_CenterAlarm, "", TDATA.GetString(374));
			return;
		}
#if NOT_USE_NET
		USERINFO.ChangeMoney(-m_TData.m_Dollar);
		for (int i = 0; i < m_TData.m_Mats.Count; i++) {
			USERINFO.DeleteItem(m_TData.m_Mats[i].m_Idx, m_TData.m_Mats[i].m_Count);
		}
		int idx = 0;
		List<TDnaTable> alldna = TDATA.GetAllDnaTable(); 
		alldna = alldna.FindAll(o => o.m_BGType == m_SelectType);
		idx = alldna[UTILE.Get_Random(0, alldna.Count)].m_Idx;
		int grade = TDATA.GetDnaTable(idx).m_Grade;
		int lv = 1;
		DNAInfo info = USERINFO.InsertDNA(idx, lv);
		RES_REWARD_DNA reward = new RES_REWARD_DNA() {
			UID = info.m_UID,
			Idx = idx,
			Grade = grade,
			Lv = lv,
			Type = Res_RewardType.DNA
		};
		MAIN.Save_UserInfo();
		StartCoroutine(m_Action = IE_Result(reward));
#else
		WEB.SEND_REQ_DNA_CREATE((res) => {
			if (res.IsSuccess()) {
				StartCoroutine(m_Action = IE_Result(res.GetRewards()[0]));
			}
		}, m_TData.m_ItemIdx, 1);
#endif
	}
	IEnumerator IE_Result(RES_REWARD_BASE _reward) {
		RES_REWARD_DNA reward = (RES_REWARD_DNA)_reward;
		float cnt = 0f;
		List<int> idxs = new List<int>();
		int rand = 0;
		for (int i = 0; i < 26; i++) idxs.Add(i);

		m_SUI.Anim.SetTrigger("Merge");
		PlayEffSound(SND_IDX.SFX_1113);
		yield return new WaitForEndOfFrame();

		m_SRUI.DNA.SetData(reward.Idx, 0, reward.Lv, reward.UID);
		while (cnt < 120) {
			if(idxs.Count < 1) for (int i = 0; i < 26; i++) idxs.Add(i);
			rand = idxs[UTILE.Get_Random(0, idxs.Count)];
			idxs.Remove(rand);
			m_SRUI.Icon.sprite = m_SRUI.IconImg[rand];
			yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, cnt / 205f));
			cnt += 2;
		}
		yield return new WaitWhile(() => Utile_Class.IsAniPlay(m_SUI.Anim, 130f / 205f));
		PlayEffSound(SND_IDX.SFX_1114);

		TDnaTable data = TDATA.GetDnaTable(reward.Idx);
		m_SRUI.Icon.sprite = data.GetIcon();
		m_SRUI.Name[0].text = m_SRUI.Name[1].text = string.Format("{0} - {1}", data.GetName(), UTILE.Get_RomaNum(data.m_Grade));
		StringBuilder  desc = new StringBuilder(data.GetDesc());
		if(reward.UID != 0)
		{
			var dna = USERINFO.GetDNA(reward.UID);
			if(dna != null && dna.m_AddStat.Count > 0)
			{
				desc.Append($"\n");
				for (int i = 0; i < dna.m_AddStat.Count; i++) desc.Append($"\n{dna.m_AddStat[i].GetStatString()}");
			}
		}
		m_SRUI.Desc.text = desc.ToString();
		m_Action = null;
	}
	public void ClickSelectType(int _pos)
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DNAMaking, 1, _pos)) return;
		if (m_Action != null) return;

		PlayEffSound(SND_IDX.SFX_1111);
		m_SelectType = (DNABGType)_pos;
		m_TData = m_TDatas[(int)m_SelectType - 1];
		m_SUI.Anim.SetTrigger(m_SelectType.ToString());
		for(int i = 0; i < m_SUI.TabAnims.Length; i++) {
			m_SUI.TabAnims[i].SetTrigger(i == _pos - 1 ? "Select" : "NotSelect");
		}
		SetUI();
	}
	public void ClickViewList()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DNAMaking, 2)) return;
		if (m_Action != null) return;
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.DNAList, null, PopupName.DNAMaking, null, m_PreState);
	}
	public void ClickConfirm()
	{
		if (TUTO.TouchCheckLock(TutoTouchCheckType.DNAMaking, 3)) return;
		if (Utile_Class.IsAniPlay(m_SUI.Anim)) return;
		SetUI();
		m_SUI.Anim.SetTrigger("MergeToMain");
	}
	public void ClickGoInven() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Inventory, null, Inventory.EMenu.DNA);
		base.Close();
	}
	public override void Close(int Result = 0) {
		if(m_PreState == PreState.ZombieFarm) {

		}
		else if(m_PreState == PreState.Inventory) {
			POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.Inventory, null, Inventory.EMenu.DNA);
		}
		base.Close(Result);
	}

	///////튜토용
	public GameObject GetTutoObj(int _idx)
	{
		return m_SUI.TutoObj[_idx];
	}
}
