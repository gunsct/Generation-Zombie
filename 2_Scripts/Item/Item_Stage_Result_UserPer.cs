using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class Item_Stage_Result_UserPer : ObjMng
{
    [Serializable]
    public struct SUI
	{
		public TextMeshProUGUI Per;
	}
	[SerializeField] SUI m_SUI;

	public void SetData(float _preper, float _crntper) {
		iTween.ValueTo(gameObject, iTween.Hash("from", _preper, "to", _crntper, "onupdate", "TW_Counting", "time", 1.5f));
	}
	public void TW_Counting(float _amount) {

		m_SUI.Per.text = string.Format(TDATA.GetString(789), _amount);
	}
}
