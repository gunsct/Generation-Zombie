using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Item_PDA_Making_CharEquipHelp : Item_PDA_Base
{
	public override void SetData(Action<object, object[]> CloaseCB, object[] args) {
		base.SetData(CloaseCB, args);
	}
	public override void OnClose() {
		PlayEffSound(SND_IDX.SFX_0121);
		m_CloaseCB?.Invoke(Item_PDA_Making.State.Main, null);
	}
}
