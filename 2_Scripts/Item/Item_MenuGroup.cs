using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using static LS_Web;

public class Item_MenuGroup : ObjMng
{
	[Serializable]
	public struct SUI {
		public Button[] MenuButton;
		public GameObject[] BtnLockObj;
		public GameObject[] BtnLockFX;
		public TextMeshProUGUI[] LockTxt;
		public GameObject[] BtnAlramObj;
		public GameObject[] PDABtnAlarmObj;
		public TextMeshProUGUI PDABtnAlarmCnt;
	}
	[SerializeField]
	SUI m_SUI;

	void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFPDABtnUI += RefreshPDABtn;
			DLGTINFO.f_RFMainMenuAlarm += SetAlarm;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFPDABtnUI -= RefreshPDABtn;
			DLGTINFO.f_RFMainMenuAlarm -= SetAlarm;
		}
	}

	public void SetData() {//USERINFO.CheckContentUnLock(ContentType.Store, true)
		for (int i = 0; i < m_SUI.BtnLockFX.Length; i++) m_SUI.BtnLockFX[i].SetActive(false);

		//버튼 잠금 표기
		m_SUI.MenuButton[(int)MainMenuType.Shop].GetComponent<Animator>().SetTrigger("Normal");
		m_SUI.BtnLockObj[(int)MainMenuType.Shop].SetActive(!USERINFO.CheckContentUnLock(ContentType.Store));
		m_SUI.LockTxt[(int)MainMenuType.Shop].text = string.Format("{0}-{1}", BaseValue.CONTENT_OPEN_IDX(ContentType.Store) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Store) % 100);
		m_SUI.BtnLockObj[(int)MainMenuType.Stage].SetActive(false);
		m_SUI.MenuButton[(int)MainMenuType.Dungeon].GetComponent<Animator>().SetTrigger("Normal");
		m_SUI.BtnLockObj[(int)MainMenuType.Dungeon].SetActive(false);//!USERINFO.CheckContentUnLock(ContentType.Factory)
		m_SUI.LockTxt[(int)MainMenuType.Dungeon].text = string.Format("{0}-{1}", BaseValue.CONTENT_OPEN_IDX(ContentType.Factory) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Factory) % 100);
		m_SUI.MenuButton[(int)MainMenuType.PDA].GetComponent<Animator>().SetTrigger("Normal");
		m_SUI.BtnLockObj[(int)MainMenuType.PDA].SetActive(!USERINFO.CheckContentUnLock(ContentType.PDA));
		m_SUI.LockTxt[(int)MainMenuType.PDA].text = string.Format("{0}-{1}", BaseValue.CONTENT_OPEN_IDX(ContentType.PDA) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.PDA) % 100);
		m_SUI.MenuButton[(int)MainMenuType.Character].GetComponent<Animator>().SetTrigger("Normal");
		m_SUI.BtnLockObj[(int)MainMenuType.Character].SetActive(!USERINFO.CheckContentUnLock(ContentType.Character));
		m_SUI.LockTxt[(int)MainMenuType.Character].text = string.Format("{0}-{1}", BaseValue.CONTENT_OPEN_IDX(ContentType.Character) / 100, BaseValue.CONTENT_OPEN_IDX(ContentType.Character) % 100);

		USERINFO.SetMainMenuAlarmVal(MainMenuType.Shop, USERINFO.GetStoreSupplyBoxCheck() || USERINFO.GetCheckNewAuctionGoods() ? 1 : 0);

		SetAlarm();
		RefreshPDABtn();
	}
	public void SetLockFX(MainMenuType _type) {
		m_SUI.BtnLockFX[(int)_type].SetActive(true);
		PlayEffSound(SND_IDX.SFX_0190);
	}
	/// <summary> 알람 갱신, 메인에서 메뉴들 꺼질때 호출? </summary>
	public void SetAlarm() {
		bool contunlock = false;
		bool alarm = false;
		for (int i = 0; i < m_SUI.BtnAlramObj.Length; i++) {
			alarm = PlayerPrefs.GetInt(string.Format("MainMenuBtnAlarm_{0}_{1}", ((MainMenuType)i).ToString(), USERINFO.m_UID)) != 0;
			if (m_SUI.BtnAlramObj[i] != null) {
				switch ((MainMenuType)i) {
					case MainMenuType.Shop:
						contunlock = USERINFO.CheckContentUnLock(ContentType.Store);
						break;
					case MainMenuType.Character:
						contunlock = USERINFO.CheckContentUnLock(ContentType.Character);
						break;
					case MainMenuType.Stage:
						contunlock = true;
						break;
					case MainMenuType.Dungeon:
						if (USERINFO.m_Stage[StageContentType.Factory].GetItemCnt(false) > 0) alarm = true;
						//else if (USERINFO.CheckContentUnLock(ContentType.Academy) && USERINFO.m_Stage[StageContentType.Academy].GetItemCnt(false) > 0) alarm = true;
						else if (USERINFO.CheckContentUnLock(ContentType.Bank) && USERINFO.m_Stage[StageContentType.Bank].GetItemCnt(false) > 0) alarm = true;
						else if (USERINFO.CheckContentUnLock(ContentType.Cemetery) && USERINFO.m_Stage[StageContentType.Cemetery].GetItemCnt(false) > 0) alarm = true;
						else if (USERINFO.CheckContentUnLock(ContentType.Subway) && USERINFO.m_Stage[StageContentType.Subway].GetItemCnt(false) > 0) alarm = true;
						contunlock = true;//USERINFO.CheckContentUnLock(ContentType.Factory);
						break;
					case MainMenuType.PDA:
						contunlock = USERINFO.CheckContentUnLock(ContentType.PDA);
						break;
				}
				m_SUI.BtnAlramObj[i].SetActive(alarm && contunlock);
			}
		}
	}
	public GameObject GetMenuBtn(MainMenuType _type) {
		return m_SUI.MenuButton[(int)_type].gameObject;
	}

	public void AllBtnChange(MainMenuType _type) {
		for (int i = 0; i < m_SUI.MenuButton.Length; i++) {
			m_SUI.MenuButton[i].interactable = i != (int)_type;
			Animator anim = m_SUI.MenuButton[i].GetComponent<Animator>();
			Utile_Class.AniResetAllTriggers(anim);
			anim.SetTrigger(i != (int)_type ? "Normal" : "Selected");
		}
		RefreshPDABtn();
		m_SUI.PDABtnAlarmObj[0].GetComponent<CanvasGroup>().alpha = m_SUI.PDABtnAlarmObj[1].GetComponent<CanvasGroup>().alpha = _type == MainMenuType.PDA ? 0f : 1f;
	}
	/// <summary> PDA 완료된것들 알람 카운트 갱신 </summary>
	public void RefreshPDABtn() {
		int advcnt = USERINFO.m_Advs.FindAll((t) => t.IS_Complete()).Count;
		int rsccnt = USERINFO.m_Researchs.FindAll((t) => t.m_TData.m_Type <= ResearchType.Remodeling && t.IS_Complete()).Count;
		int makcnt = USERINFO.m_Making.FindAll((t) => t.IS_Complete()).Count;
		int lv = Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.MakingLevelUp));
		//List<TMakingTable> setmaketdata = TDATA.GetAllMakingTable().FindAll(o => o.m_Group == MakingGroup.PrivateEquip && o.m_LV == lv);
		//bool cansetmak = setmaketdata.Find(o => o.GetCanMake()) != null && USERINFO.m_Stage[StageContentType.Stage].Idxs[0].Idx >= BaseValue.CHAREQUIP_MAKING_OPEN;
		int acvcnt = USERINFO.m_Achieve.GetSucAchieveList().Count;
		int colcnt = USERINFO.m_Collection.GetSucList().Count;
		bool openzf = USERINFO.CheckContentUnLock(ContentType.ZombieFarm, false);
		bool opendna = USERINFO.CheckContentUnLock(ContentType.CharDNA, false);
		int zomcnt = openzf ? USERINFO.m_Zombies.Count - USERINFO.m_CageZobie.Count : 0;
		int cnt = advcnt + rsccnt + makcnt + acvcnt + colcnt + zomcnt;
		bool ismakedna = openzf && opendna  && USERINFO.IS_CanMakeAnyDNA();
		m_SUI.PDABtnAlarmObj[0].SetActive(cnt > 0);//cansetmak
		m_SUI.PDABtnAlarmObj[1].SetActive(cnt > 0 || ismakedna);//cansetmak
		m_SUI.PDABtnAlarmCnt.gameObject.SetActive(cnt > 0);
		m_SUI.PDABtnAlarmCnt.text = cnt.ToString();
	}
}
