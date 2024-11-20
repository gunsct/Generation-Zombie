using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class HubInfo : PopupBase
{
	public enum State
	{
		Survivor,
		Research,
		Equip,
		None
	}
	[Serializable]
	public struct SCUI {
		public TextMeshProUGUI[] Stats;     //0atk,1def,2hp,3heal
		public TextMeshProUGUI LvSum;
		public TextMeshProUGUI[] GetCnts;
		public Transform[] Buckets;         //0special,1atk,2def,3hp,4heal
		public Transform SpElement;			//Item_HubInfo_Srv_Element
		public Transform NmElement;
	}
	[Serializable]
	public struct SRUI {
		public Image[] StatBG;
		public Sprite[] StatBGImg;
		public TextMeshProUGUI[] Stat;
		public Color[] StatColor;
	}
	[Serializable]
	public struct SEUI {
		public TextMeshProUGUI[] Title;
		public TextMeshProUGUI[] Stat;
		public Image[] StatBG;
		public Sprite[] StatBGImg;
	}
	[Serializable]
	public struct SUI {
		public Animator Anim;
		public GameObject[] TabPanel;
		public GameObject[] TabBtnOn;
		public GameObject[] TabBtnOff;
		public Image TitleDeco;
		public Sprite[] TitleDecoImg;
		public TextMeshProUGUI Title;

	}
	[SerializeField] SUI m_SUI;
	[SerializeField] SCUI m_SCUI;
	[SerializeField] SRUI m_SRUI;
	[SerializeField] SEUI m_SEUI;
	State m_State = State.None;
	IEnumerator m_Action; //end ani check

	private void Start() {
		DLGTINFO?.f_RFHubInfoUI?.Invoke(-1);
	}
	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		Click_Tab(0);
		base.SetData(pos, popup, cb, aobjValue);
	}
	public override void SetUI() {
		base.SetUI();
	}
	public void Click_Tab(int _pos) {
		if (m_State == (State)_pos) return;
		for(int i = 0; i < 3; i++) {
			m_SUI.TabBtnOff[i].SetActive(i != _pos);
			m_SUI.TabBtnOn[i].SetActive(i == _pos);
			m_SUI.TabPanel[i].SetActive(i == _pos);
		}
		switch ((State)_pos) {
			case State.Survivor:
				SetSurvivor();
				m_SUI.Title.text = string.Format(TDATA.GetString(10845), TDATA.GetString(82));
				break;
			case State.Research:
				SetResearch();
				m_SUI.Title.text = string.Format(TDATA.GetString(10845), TDATA.GetString(205));
				break;
			case State.Equip:
				SetEquip();
				m_SUI.Title.text = string.Format(TDATA.GetString(10845), TDATA.GetString(10846));
				break;
		}
		m_SUI.TitleDeco.sprite = m_SUI.TitleDecoImg[_pos];
		m_State = (State)_pos;
	}
	#region Survivor
	void SetSurvivor() {
		Dictionary<StatType, float> stats = USERINFO.GetCharLvStatBonus();
		Dictionary<StatType, List<TCharacterTable>> statgroup = new Dictionary<StatType, List<TCharacterTable>>();
		Dictionary<SkillKind, List<TCharacterTable>> skillgroup = new Dictionary<SkillKind, List<TCharacterTable>>();
		List<TCharacterTable> alldata = TDATA.GetAllCharacterInfos();

		for(int i = 0; i < alldata.Count; i++) {
			TCharacterTable cdata = alldata[i];
			for(int j=(int)SkillType.Passive1;j<= (int)SkillType.Passive2; j++) {
				TSkillTable sdata = TDATA.GetSkill(cdata.m_SkillIdx[j]);
				if (sdata == null) continue;
				StatType stype = sdata.GetStatType();
				if(stype == StatType.None) {
					if (!skillgroup.ContainsKey(sdata.m_Kind)) skillgroup.Add(sdata.m_Kind, new List<TCharacterTable>());
					skillgroup[sdata.m_Kind].Add(cdata);
				}
				else {
					if (!statgroup.ContainsKey(stype)) statgroup.Add(stype, new List<TCharacterTable>());
					statgroup[stype].Add(cdata);
				}
			}
		}
		//특수
		List<TCharacterTable> skgroup = skillgroup.SelectMany(o => o.Value).ToList();
		int sgcnt = skgroup.Count;
		int getsgcnt = 0;
		UTILE.Load_Prefab_List(sgcnt, m_SCUI.Buckets[0], m_SCUI.SpElement);
		for (int i = 0; i < skgroup.Count; i++) {
			Item_HubInfo_Srv_Element element = m_SCUI.Buckets[0].GetChild(i).GetComponent<Item_HubInfo_Srv_Element>();
			if (element.SetData(skgroup[i], StatType.None, SetSurvivor)) getsgcnt++;
		}
		for(int i = skgroup.Count - 1; i >= 0; i--) {
			Item_HubInfo_Srv_Element element = m_SCUI.Buckets[0].GetChild(i).GetComponent<Item_HubInfo_Srv_Element>();
			if (!element.Is_Get) element.transform.SetAsLastSibling();
		}
		m_SCUI.GetCnts[0].text = string.Format(TDATA.GetString(10849), getsgcnt, sgcnt);
		//스탯별
		List<StatType> stypes = new List<StatType>() { StatType.Atk, StatType.Def, StatType.HP, StatType.Heal };
		for(int i = 0; i < stypes.Count; i++) {
			if (!statgroup.ContainsKey(stypes[i])) continue;
			List<TCharacterTable> tdatas = statgroup[stypes[i]];
			int ngcnt = tdatas.Count;
			int getngcnt = 0;
			UTILE.Load_Prefab_List(ngcnt, m_SCUI.Buckets[i +1 ], m_SCUI.NmElement);
			for(int j = 0;j< tdatas.Count; j++) {
				Item_HubInfo_Srv_Element element = m_SCUI.Buckets[i + 1].GetChild(j).GetComponent<Item_HubInfo_Srv_Element>();
				if (element.SetData(tdatas[j], stypes[i], SetSurvivor)) getngcnt++;
			}
			for (int j = tdatas.Count - 1; j >= 0 ; j--) {
				Item_HubInfo_Srv_Element element = m_SCUI.Buckets[i + 1].GetChild(j).GetComponent<Item_HubInfo_Srv_Element>();
				if(!element.Is_Get) element.transform.SetAsLastSibling();
			}
			m_SCUI.GetCnts[i + 1].text = string.Format(TDATA.GetString(10850 + i), getngcnt, ngcnt);
		}

		m_SCUI.Stats[0].text = stats.ContainsKey(StatType.Atk) ? string.Format("+ {0}",Utile_Class.CommaValue(stats[StatType.Atk])) : "0";
		m_SCUI.Stats[1].text = stats.ContainsKey(StatType.Def) ? string.Format("+ {0}", Utile_Class.CommaValue(stats[StatType.Def])) : "0";
		m_SCUI.Stats[2].text = stats.ContainsKey(StatType.HP) ? string.Format("+ {0}", Utile_Class.CommaValue(stats[StatType.HP])) : "0";
		m_SCUI.Stats[3].text = stats.ContainsKey(StatType.Heal) ? string.Format("+ {0}", Utile_Class.CommaValue(stats[StatType.Heal])) : "0";
		m_SCUI.LvSum.text = Utile_Class.CommaValue(USERINFO.m_Chars.Sum(o => o.m_LV));
	}
	public void Click_LvSumInfo() {
		POPUP.Set_Popup(PopupPos.POPUPUI, PopupName.LvBonusList, null);
	}
	/// <summary>
	/// 캐릭터 관리로 보내기
	/// </summary>
	public void Click_GoCharGacha() {
		Main_Play main = (Main_Play)POPUP.GetMainUI();
		main.MenuChange((int)MainMenuType.Shop, false); 
		Shop shop = main.GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
		shop.StartPos(main.m_State != MainMenuType.Shop, ShopGroup.Gacha);
		Close();
	}
	#endregion
	#region Research
	void SetResearch() {
		//훈련 공방생회
		//개발 장비5타입
		float val = 0f;
		//훈련
		val = USERINFO.ResearchValue(ResearchEff.AtkUp);// + USERINFO.ResearchValue(ResearchEff.PVPAtkUp);
		m_SRUI.Stat[0].text = string.Format(TDATA.GetString(10855), TDATA.GetStatString(StatType.Atk), val * 100f);//10855. 10860
		m_SRUI.Stat[0].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[0].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.DefUp);// + USERINFO.ResearchValue(ResearchEff.PVPDefUP);
		m_SRUI.Stat[1].text = string.Format(TDATA.GetString(10855), TDATA.GetStatString(StatType.Def), val * 100f);//10855. 10860
		m_SRUI.Stat[1].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[1].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.HealthMaxUp);// + USERINFO.ResearchValue(ResearchEff.PVPHpUP);
		m_SRUI.Stat[2].text = string.Format(TDATA.GetString(10855), TDATA.GetStatString(StatType.HP), val * 100f);//10855. 10860
		m_SRUI.Stat[2].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[2].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.HealUp);// + USERINFO.ResearchValue(ResearchEff.PVPAtkUp);
		m_SRUI.Stat[3].text = string.Format(TDATA.GetString(10855), TDATA.GetStatString(StatType.Heal), val * 100f);//10855. 10860
		m_SRUI.Stat[3].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[3].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		//개발
		val = USERINFO.ResearchValue(ResearchEff.WeaponStatUp);
		m_SRUI.Stat[4].text = string.Format(TDATA.GetString(10860), TDATA.GetEquipTypeName(EquipType.Weapon), val * 100f);
		m_SRUI.Stat[4].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[4].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.HelmetStatUp);
		m_SRUI.Stat[5].text = string.Format(TDATA.GetString(10860), TDATA.GetEquipTypeName(EquipType.Helmet), val * 100f);
		m_SRUI.Stat[5].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[5].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.CostumeStatUp);
		m_SRUI.Stat[6].text = string.Format(TDATA.GetString(10860), TDATA.GetEquipTypeName(EquipType.Costume), val * 100f);
		m_SRUI.Stat[6].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[6].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.ShoesStatUp);
		m_SRUI.Stat[7].text = string.Format(TDATA.GetString(10860), TDATA.GetEquipTypeName(EquipType.Shoes), val * 100f);
		m_SRUI.Stat[7].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[7].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
		val = USERINFO.ResearchValue(ResearchEff.AccStatUp);
		m_SRUI.Stat[8].text = string.Format(TDATA.GetString(10860), TDATA.GetEquipTypeName(EquipType.Accessory), val * 100f);
		m_SRUI.Stat[8].color = m_SRUI.StatColor[val > 0f ? 1 : 0];
		m_SRUI.StatBG[8].sprite = m_SRUI.StatBGImg[val > 0f ? 1 : 0];
	}
	public void Click_GoResearch() {
		Main_Play main = (Main_Play)POPUP.GetMainUI();
		main.MenuChange((int)MainMenuType.PDA, false);
		Item_PDA_Menu pda = main.GetMenuUI(MainMenuType.PDA).GetComponent<Item_PDA_Menu>();
		pda.ClickMenu(2);
		Close();
	}
	#endregion
	#region Equip
	void SetEquip() {
		//공,속  방,힐  체,힐  체,방  방,힐
		for(EquipType i = EquipType.Weapon;i< EquipType.End; i++) {
			m_SEUI.Title[(int)i].text = string.Format(TDATA.GetString(10856), TDATA.GetEquipTypeName(i));
			string name = TDATA.GetString(10855);
			float val = USERINFO.GetEquipGachaLvBonus(i) * 100f;
			switch (i) {
				case EquipType.Weapon:
					m_SEUI.Stat[(int)i * 2].text = string.Format(name, TDATA.GetStatString(StatType.Atk), val);
					m_SEUI.Stat[(int)i * 2 + 1].text = string.Format(name, TDATA.GetStatString(StatType.Speed), val);
					break;
				case EquipType.Helmet:
					m_SEUI.Stat[(int)i * 2].text = string.Format(name, TDATA.GetStatString(StatType.Def), val);
					m_SEUI.Stat[(int)i * 2 + 1].text = string.Format(name, TDATA.GetStatString(StatType.Heal), val);
					break;
				case EquipType.Costume:
					m_SEUI.Stat[(int)i * 2].text = string.Format(name, TDATA.GetStatString(StatType.HP), val);
					m_SEUI.Stat[(int)i * 2 + 1].text = string.Format(name, TDATA.GetStatString(StatType.Heal), val);
					break;
				case EquipType.Shoes:
					m_SEUI.Stat[(int)i * 2].text = string.Format(name, TDATA.GetStatString(StatType.HP), val);
					m_SEUI.Stat[(int)i * 2 + 1].text = string.Format(name, TDATA.GetStatString(StatType.Def), val);
					break;
				case EquipType.Accessory:
					m_SEUI.Stat[(int)i * 2].text = string.Format(name, TDATA.GetStatString(StatType.Def), val);
					m_SEUI.Stat[(int)i * 2 + 1].text = string.Format(name, TDATA.GetStatString(StatType.Heal), val);
					break;
			}
			m_SEUI.Stat[(int)i * 2].alpha = m_SEUI.Stat[(int)i * 2 + 1].alpha = val > 0 ? 1f : 85f / 255f;
			m_SEUI.StatBG[(int)i * 2].sprite = m_SEUI.StatBG[(int)i * 2 + 1].sprite = m_SEUI.StatBGImg[val > 0 ? 0 : 1];
		}
	}
	public void Click_GoItemGacha() {
		Main_Play main = (Main_Play)POPUP.GetMainUI();
		main.MenuChange((int)MainMenuType.Shop, false);
		Shop shop = main.GetMenuUI(MainMenuType.Shop).GetComponent<Shop>();
		shop.StartPos(main.m_State != MainMenuType.Shop, ShopGroup.ItemGacha);
		Close();
	}
	#endregion
	public override void Close(int Result = 0) {
		if (TUTO.TouchCheckLock(TutoTouchCheckType.PopupOnClose, Result, m_Popup)) return;
		if (m_Action != null) return;
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
