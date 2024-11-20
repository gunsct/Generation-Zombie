using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Item_ShellUI : ObjMng
{
	[SerializeField]
	TextMeshProUGUI m_ValTxt, m_Timer;
	[SerializeField]
	GameObject m_TimerGroup;
	Coroutine m_Cor;

	private void Awake() {
		if (MainMng.IsValid()) {
			DLGTINFO.f_RFShellUI += SetData;
		}
	}
	void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFShellUI -= SetData;
		}
	}
	void OnEnable() {
		if(m_Cor != null) {
			StopCoroutine(m_Cor);
		}
		m_Cor = StartCoroutine(IE_MakeTimer());
	}
	/// <summary> 변경 전, 현재 값 전달시 애니메이션 시간동안 카운팅 </summary>
	public void SetData(long _crntval) {
		m_TimerGroup.SetActive(_crntval < USERINFO.m_Energy.GetMaxCnt());
		m_ValTxt.text =  string.Format("<color=#FFE46E>{0}</color> / {1}", _crntval, USERINFO.m_Energy.GetMaxCnt()); //타이머 시작
	}

	/// <summary> 제작중인 아이템들 남은 시간 갱신 </summary>
	IEnumerator IE_MakeTimer() {
		int sec = (int)UTILE.Get_Time();
		SetData(USERINFO.m_Energy.Cnt);
		//m_TimerGroup.SetActive(USERINFO.m_Energy.Cnt < BaseValue.MAX_ENERGY + Mathf.RoundToInt(USERINFO.ResearchValue(ResearchEff.BulletMaxUp)));
		m_Timer.text = UTILE.GetSecToTimeStr(USERINFO.m_Energy.GetRemainTime());


		yield return new WaitUntil(() => sec < (int)UTILE.Get_Time());//실제 초단위가 바뀔때

		m_Cor = StartCoroutine(IE_MakeTimer());
	}
	public void ClickBuy() {
		if (TUTO.IsTutoPlay()) return;
		if (POPUP.GetMainUI().m_Popup != PopupName.Play) return; 

		TShopTable tshop = TDATA.GetShopTable(BaseValue.ENERGY_SHOP_IDX);
		int price = USERINFO.m_ShopInfo.GetEnergyPrice();
		var buyinfo = USERINFO.m_ShopInfo.BUYs.Find(o => o.Idx == BaseValue.ENERGY_SHOP_IDX);
		var cnt = buyinfo == null ? tshop.m_LimitCnt : tshop.m_LimitCnt - buyinfo.Cnt;
		string Msg = Utile_Class.StringFormat(TDATA.GetString(833), TDATA.GetItemTable(BaseValue.ENERGY_IDX).GetName(), tshop.m_Rewards[0].m_ItemCnt);
		var yesbtn = new Msg_YN_Cost_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_YES, Label = string.Format(TDATA.GetString(1076), cnt, tshop.m_LimitCnt), BG = UIMng.BtnBG.Green };
		if (cnt < 1)
		{
			Msg = TDATA.GetString(1078);
			yesbtn.Label = TDATA.GetString(1077);
			yesbtn.BG = UIMng.BtnBG.Red;
			yesbtn.Lock = true;
		}

		POPUP.Set_MsgBox(PopupName.Msg_YN_Cost_BtnControl, string.Empty, Msg, (result, obj) => {
			if(result == 1) {
				if (obj.GetComponent<Msg_YN_Cost_BtnControl>().IS_CanBuy) {
					USERINFO.ITEM_BUY(BaseValue.ENERGY_SHOP_IDX, 1, (res) => {
						SetData(USERINFO.m_Energy.Cnt);
					});
				}
				else {
					POPUP.StartLackPop(tshop.GetPriceIdx());
				}
			}
		}, tshop.m_PriceType, tshop.m_PriceIdx, price
		, new Msg_YN_Cost_BtnControl.BtnInfo() { Btn = EMsgBtn.BTN_NO, Label = TDATA.GetString(11), BG = UIMng.BtnBG.Brown }
		, yesbtn);
	}
}
