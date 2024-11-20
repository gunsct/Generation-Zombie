using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_MoneyGold : ObjMng
{
	[SerializeField]
	TextMeshProUGUI m_ValTxt, m_ChangeTxt, m_PlusMius;
	[SerializeField]
	GameObject m_UpDown;
	long[] m_Vals = new long[2];
	protected bool m_IsAutoCheck = true;
	public virtual void Awake() {
		if (m_UpDown != null)
			m_UpDown.SetActive(false);
	}
	public virtual void OnDestroy() {
		
	}

	public virtual void Active_AutoCheck(bool Active)
	{
		m_IsAutoCheck = false;
	}
	/// <summary> 변경 전, 현재 값 전달시 애니메이션 시간동안 카운팅 </summary>
	public void SetData(long _crntval, long _preval) {
		m_Vals[0] = _crntval;
		m_Vals[1] = _preval;
		if (gameObject.activeInHierarchy == false || _crntval == _preval)
			m_ValTxt.text = Utile_Class.CommaValue(Math.Round((double)m_Vals[0]));
		else  {
			TW_StopCount();
			m_UpDown.SetActive(true);

			AnimatorStateInfo anistate = m_UpDown.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
			float time = anistate.length;

			m_PlusMius.text = _crntval - _preval >= 0f ? "+" : "-";
			iTween.ValueTo(gameObject, iTween.Hash("from", (double)_preval, "to", (double)_crntval, "onupdate", "TW_UpDownVal", "time", time, "name", "Val"));
			iTween.ValueTo(gameObject, iTween.Hash("from", (double)(_crntval - _preval), "to", 0, "onupdate", "TW_UpDownChange", "oncomplete", "TW_ChangeOff", "time", time, "name", "Change"));

			if (_crntval > _preval) {
				SND_IDX snd = SND_IDX.NONE;
				if (this.GetType() == typeof(Item_MoneyUI)) snd = SND_IDX.SFX_0108;
				else if (this.GetType() == typeof(Item_GoldUI)) snd = SND_IDX.SFX_1010;
				else if (this.GetType() == typeof(Item_CoinUI)) snd = SND_IDX.SFX_1010;
				else if (this.GetType() == typeof(Item_GCoinUI)) snd = SND_IDX.SFX_1010;
				else if (this.GetType() == typeof(Item_TicketUI)) snd = SND_IDX.SFX_1010;
				PlayEffSound(snd);
			}
		}
	}
	/// <summary> SetData 트위너에서 호출, 현재 표기 금액 카운팅  </summary>
	void TW_UpDownVal(double _val) {
		m_ValTxt.text = Utile_Class.CommaValue(Math.Round(_val));
	}
	/// <summary> SetData 트위너에서 호출, 변경 금액 카운팅  </summary>
	void TW_UpDownChange(double _val) {
		m_ChangeTxt.text = Utile_Class.CommaValue(Math.Abs(Math.Round(_val)));
	}
	/// <summary> SetData 트위너에서 호출 카운팅 종료 후 오브젝트 끔 </summary>
	public void TW_ChangeOff() {
		m_ValTxt.text = Utile_Class.CommaValue(Math.Round((double)m_Vals[0]));
		m_UpDown.SetActive(false);
	}
	/// <summary> SetData 연속 호출시 트위너, 애니메이션 초기화 </summary>
	void TW_StopCount() {
		iTween.StopByName(gameObject, "Val");
		iTween.StopByName(gameObject, "Change");
		m_UpDown.SetActive(false);
	}

	public void Click_GoStore(bool _cash) {
		if (TUTO.IsTutoPlay()) return;
		//if (TUTO.TouchCheckLock(TutoTouchCheckType.ShopMenu)) return;
		if (POPUP.GetMainUI().m_Popup != PopupName.Play) return;
		if (POPUP.GetMainUI().GetComponent<Main_Play>().m_State == MainMenuType.Shop && !_cash) return;

		ShopGroup? shop = _cash ? ShopGroup.Cash : ShopGroup.Money;

		if (_cash) {
			bool instore = POPUP.GetMainUI().GetComponent<Main_Play>().m_State == MainMenuType.Shop;
			POPUP.Set_Popup(PopupPos.TOOLTIP, PopupName.BuyGold, (res, obj) => {
				if (res == 1) {
					POPUP.Init_PopupUI();
					POPUP.GetMainUI().GetComponent<Main_Play>().MenuChange((int)MainMenuType.Shop, false, Shop.Tab.Shop, shop);
				}
			}, transform, instore);
		}
		else {
			POPUP.Init_PopupUI();
			POPUP.GetMainUI().GetComponent<Main_Play>().MenuChange((int)MainMenuType.Shop, false, Shop.Tab.Shop, shop);
		}
	}
}
