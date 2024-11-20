using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_MoneyUI : Item_MoneyGold
{
	public override void Awake() {
		base.Awake();

		if (MainMng.IsValid()) {
			DLGTINFO.f_RFMoneyUI += SetData;
		}
	}
	public override void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFMoneyUI -= SetData;
		}
	}
	public override void Active_AutoCheck(bool Active)
	{
		base.Active_AutoCheck(Active);
		DLGTINFO.f_RFMoneyUI -= SetData;
		if (Active) DLGTINFO.f_RFMoneyUI += SetData;
	}
}
