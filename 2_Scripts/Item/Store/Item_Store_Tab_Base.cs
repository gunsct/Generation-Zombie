using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using static LS_Web;
using System.Linq;
using DanielLochner.Assets.SimpleScrollSnap;

public class Item_Store_Tab_Base : ObjMng
{
	public virtual void SetData(Action CB){}

	public virtual void SetUI() { }

	public virtual void SetScrollState(bool Active) { }
	public void CheckAlarm(Shop.Tab _tab = Shop.Tab.End) {
		((Main_Play)POPUP.GetMainUI()).GetMenuUI(MainMenuType.Shop).GetComponent<Shop>().SetAlarm(_tab);
	}
}
