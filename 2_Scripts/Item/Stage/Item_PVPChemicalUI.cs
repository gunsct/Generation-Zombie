using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_PVPChemicalUI : Item_MoneyGold
{
	public override void Awake() {
		base.Awake();

		if (MainMng.IsValid()) {
			DLGTINFO.f_RFPVPChemicalUI += SetData;
		}
	}
	public override void OnDestroy() {
		if (MainMng.IsValid() && DLGTINFO != null) {
			DLGTINFO.f_RFPVPChemicalUI -= SetData;
		}
	}
	public override void Active_AutoCheck(bool Active)
	{
		base.Active_AutoCheck(Active);
		DLGTINFO.f_RFPVPChemicalUI -= SetData;
		if (Active) DLGTINFO.f_RFPVPChemicalUI += SetData;
	}
}
