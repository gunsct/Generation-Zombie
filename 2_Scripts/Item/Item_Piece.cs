using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item_Piece : ObjMng
{
   [System.Serializable]
   public struct SUI
	{
		public Item_Item_Card Card;
		public TextMeshProUGUI[] Count;
		public Image Gauge;
	}
	[SerializeField]
	SUI m_SUI;

	public void SetData(int _idx, int _crntval, int _maxval) {
		TItemTable table = TDATA.GetItemTable(_idx);
		m_SUI.Card.SetData(table.m_Idx, 0, table.m_Grade);
		m_SUI.Count[0].text = _crntval.ToString();
		m_SUI.Count[0].color = BaseValue.GetUpDownStrColor(_crntval, _maxval, "#903030", "#035508");
		m_SUI.Count[1].text = _maxval.ToString();
		m_SUI.Gauge.fillAmount = (float)_crntval / (float)_maxval;
	}
}
