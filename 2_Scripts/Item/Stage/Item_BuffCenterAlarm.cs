using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_BuffCenterAlarm : ObjMng
{
	[System.Serializable]
	public struct SUI
	{
		public Animator Anim;
		public Sprite[] IconSprite;
		public Image Icon;
		public TextMeshProUGUI Name;
		public TextMeshProUGUI Value;
		public GameObject[] UpDown;
		public GameObject RefugeeAlarm;
		public Image RefugeeImg;
		public HorizontalLayoutGroup HorLayoutGroup;
	}
	[SerializeField]
	SUI m_SUI;

	private void Awake() {
		m_SUI.UpDown[0].SetActive(false);
		m_SUI.UpDown[1].SetActive(false);
		m_SUI.RefugeeAlarm.SetActive(false);
	}
	public void SetData(StageCardType _type, object _val, StageCardInfo _info = null) {
		SetRefugeeInfo(_info);

		int per = Mathf.FloorToInt((float)_val * 100);
		int nper = Mathf.FloorToInt((float)_val);

		m_SUI.Value.gameObject.SetActive(false);
		m_SUI.Icon.enabled = false;

		switch (_type) {
			case StageCardType.HpUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30174), per, nper);
				break;
			case StageCardType.AtkUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30171), per, nper);
				break;
			case StageCardType.DefUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30172), per, nper);
				break;
			case StageCardType.EnergyUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30173), per, nper);
				break;
			case StageCardType.MenUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30178), per, nper);
				break;
			case StageCardType.HygUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30177), per, nper);
				break;
			case StageCardType.SatUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30176), per, nper);
				break;
			/// <summary> 속도 증가 </summary>
			case StageCardType.SpeedUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30180), per, nper);
				break;
			/// <summary> 크리티컬 확률 증가 </summary>
			case StageCardType.CriticalUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30181), per, nper);
				break;
			/// <summary> 크리티컬 데미지 증가 </summary>
			case StageCardType.CriticalDmgUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30182), per, nper);
				break;
			/// <summary> 행동력 회복량 증가 </summary>
			case StageCardType.APRecoveryUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30185), per, nper);
				break;
			/// <summary> 행동력 소모량 감소 </summary>
			case StageCardType.APConsumDown:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30186), per, nper);
				break;
			/// <summary> 체력 회복량 증가 </summary>
			case StageCardType.HealUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30187), per, nper);
				break;
			/// <summary> 캐릭터, 장비 레벨 증가 </summary>
			case StageCardType.LevelUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30188), per, nper);
				break;
			/// <summary> 타임어택 시간이 증가 </summary>
			case StageCardType.TimePlus:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30165), per, nper);
				break;
			/// <summary> 헤드샷 확률 증가 </summary>
			case StageCardType.HeadShotUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30183), per, nper);
				break;
			case StageCardType.AddGuard:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30098), per, nper);
				break;
			case StageCardType.RecoveryMen:
				SetData(StatType.Men, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 30178));
				return;
			case StageCardType.RecoveryHyg:
				SetData(StatType.Hyg, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 30177));
				return;
			case StageCardType.RecoverySat:
				SetData(StatType.Sat, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 30176));
				return;
			case StageCardType.PerRecoveryMen:
				SetData(StatType.Men, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 34203));
				return;
			case StageCardType.PerRecoveryHyg:
				SetData(StatType.Hyg, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 34202));
				return;
			case StageCardType.PerRecoverySat:
				SetData(StatType.Sat, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 34201));
				return;
			case StageCardType.RecoveryHp:
			case StageCardType.RecoveryHpPer:
				SetData(StatType.HP, nper, TDATA.GetString(ToolData.StringTalbe.Etc, 30174));
				return;
			case StageCardType.RecoveryAP:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30179), per, nper);
				break;
			case StageCardType.LimitTurnUp:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, nper > 0 ? 30271 : 30184), per, nper);
				break;
			case StageCardType.AddRerollCount:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30218), per, nper);
				break;
			case StageCardType.AllUpAdr:
				m_SUI.Name.text = string.Format(TDATA.GetString(ToolData.StringTalbe.Etc, 30157), per, nper);
				break;
			default:
				m_SUI.Icon.enabled = false;
				m_SUI.Value.text = string.Empty;
				m_SUI.Name.text = string.Empty;
				Destroy(gameObject);
				break;
		}

		m_SUI.UpDown[(float)_val > 0 ? 0 : 1].SetActive(true);
		m_SUI.Anim.SetTrigger((float)_val > 0 ? "Up" : "Down");
		StartCoroutine(IE_AutoDestroy());
	}
	public void SetData(StatType _type, int _val, string _name = "", StageCardInfo _info = null)
	{
		SetRefugeeInfo(_info);
		m_SUI.Name.text = _name.Equals("") ? TDATA.GetStatString(_type) : _name;
		m_SUI.Icon.enabled = true;
		switch (_type) {
			case StatType.Men:
				m_SUI.Icon.sprite = m_SUI.IconSprite[0];
				m_SUI.Name.color = Utile_Class.GetCodeColor("#FEFE00");
				break;
			case StatType.Hyg:
				m_SUI.Icon.sprite = m_SUI.IconSprite[1];
				m_SUI.Name.color = Utile_Class.GetCodeColor("#17E7E7");
				break;
			case StatType.Sat:
				m_SUI.Icon.sprite = m_SUI.IconSprite[2];
				m_SUI.Name.color = Utile_Class.GetCodeColor("#F97401");
				break;
			case StatType.HP:
				m_SUI.Icon.sprite = m_SUI.IconSprite[3];
				m_SUI.Name.color = Utile_Class.GetCodeColor("#D12E2D");
				break;
			default:
				m_SUI.Icon.enabled = false;
				break;
		}
		m_SUI.Value.gameObject.SetActive(true);
		m_SUI.Value.text = _val.ToString();
		m_SUI.UpDown[_val > 0 ? 0 : 1].SetActive(true);
		m_SUI.Anim.SetTrigger(_val > 0 ? "Up" : "Down");

		StartCoroutine(IE_AutoDestroy());
	}
	void SetRefugeeInfo(StageCardInfo _info) {
		m_SUI.HorLayoutGroup.padding.right = 50;
		if (_info == null) return;
		else if (_info.IS_EnemyCard && _info.m_TEnemyData.ISRefugee()) {
			m_SUI.HorLayoutGroup.padding.right = 240;
			m_SUI.RefugeeAlarm.SetActive(true);
			m_SUI.RefugeeImg.sprite = _info.GetRealImg();
		}
		else return;
	}
	IEnumerator IE_AutoDestroy() {
		yield return new WaitForEndOfFrame();
		yield return new WaitUntil(() => m_SUI.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

		Destroy(gameObject);
	}
}
