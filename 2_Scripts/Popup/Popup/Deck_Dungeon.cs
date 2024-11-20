using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck_Dungeon : PopupBase
{
	[Serializable]
	public struct SUI
	{

	}
	[SerializeField]
	SUI m_SUI;

	public override void SetData(PopupPos pos, PopupName popup, Action<int, GameObject> cb, object[] aobjValue) {
		base.SetData(pos, popup, cb, aobjValue);
	}
}
