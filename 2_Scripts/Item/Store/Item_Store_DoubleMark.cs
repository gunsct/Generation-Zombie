using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Store_DoubleMark : ObjMng
{
	public void SetData(int _sidx) {
		MyFAEvent doubleevent = USERINFO.m_Event.Datas.Find(o => o.Type == LS_Web.FAEventType.FirstPurchase);
		if (doubleevent == null) {
			gameObject.SetActive(false);
			return;
		}
		FAEventData_FirstPurchase realdata = doubleevent.GetRealInfo<FAEventData_FirstPurchase>();
		gameObject.SetActive(realdata.ShopIdxs.Contains(_sidx) && !doubleevent.Values.Contains(_sidx));
	}
}
